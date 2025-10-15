using EasyGamesWeb.Data;
using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int productId, int qty)
        {
            string userId = getUserId();

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged in");

                var cart = await getCart(userId);
                if (cart is null)
                {
                    cart = new Cart { UserId = userId };
                    _db.Carts.Add(cart);
                    await _db.SaveChangesAsync(); 
                }

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(cd => cd.CartId == cart.Id && cd.ProductId == productId);

                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var product = await _db.Products.FindAsync(productId)
                                 ?? throw new InvalidOperationException("Product not found");

                    
                    cartItem = new CartDetails
                    {
                        ProductId = productId,
                        CartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = (double)(product.SellPrice > 0 ? product.SellPrice : product.Price)
                    };

                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return await getCartItemCount(userId);
        }

        public async Task<int> RemoveItem(int productId)
        {
            string userId = getUserId();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged in");

                var cart = await getCart(userId) ?? throw new InvalidOperationException("Invalid cart");

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(cd => cd.CartId == cart.Id && cd.ProductId == productId);

                if (cartItem is null)
                    throw new InvalidOperationException("There is no such item in the cart");
                else if (cartItem.Quantity <= 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity -= 1;

                await _db.SaveChangesAsync();
            }
            catch
            {
                
            }

            return await getCartItemCount(userId);
        }

        public async Task<Cart> getUserCart()
        {
            var userId = getUserId();
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("Invalid user");

            var cart = await _db.Carts
                .Include(c => c.cartDetails)
                    .ThenInclude(cd => cd.Products)
                        .ThenInclude(p => p.Stocks)
                .Include(c => c.cartDetails)
                    .ThenInclude(cd => cd.Products)
                        .ThenInclude(p => p.Category) 
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart!;
        }

        public async Task<Cart> getCart(string userId)
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart!;
        }

        public async Task<int> getCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
                userId = getUserId();

            var data = await (from cart in _db.Carts
                              join cartDetail in _db.CartDetails on cart.Id equals cartDetail.CartId
                              where cart.UserId == userId
                              select cartDetail.Id)
                              .ToListAsync();

            return data.Count;
        }

        public async Task<bool> StartCheckout(CheckoutModel model)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var userId = getUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User not logged in");

                var cart = await getCart(userId) ?? throw new InvalidOperationException("Not a valid cart");

                var cartDetail = await _db.CartDetails
                    .Where(cd => cd.CartId == cart.Id)
                    .Include(cd => cd.Products)
                    .ToListAsync();

                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Empty cart");

                var pendingRec = await _db.OrderStat.FirstOrDefaultAsync(s => s.StatName == "Pending")
                                 ?? throw new InvalidOperationException("Order status does not have 'Pending'");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    isPaid = false,
                    OrderStatId = pendingRec.Id
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                foreach (var item in cartDetail)
                {
                    var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.ProductId == item.ProductId)
                                ?? throw new InvalidOperationException("Stocks is null");

                    if (item.Quantity <= 0)
                        throw new InvalidOperationException("Quantity must be positive");

                    if (item.Quantity > stock.Quantity)
                        throw new InvalidOperationException($"Only {stock.Quantity} available in stock");

                    var orderDetail = new OrderDetails
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,

                        
                        UnitPrice = item.UnitPrice,

                        
                        UnitCostAtSale = (double)item.Products.CostPrice
                    };

                    _db.OrderDetails.Add(orderDetail);

                    stock.Quantity -= item.Quantity;
                }

                _db.CartDetails.RemoveRange(cartDetail);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        private string getUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return principal == null ? string.Empty : _userManager.GetUserId(principal) ?? string.Empty;
        }
    }
}
