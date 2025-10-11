using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Net.WebSockets;

namespace EasyGamesWeb.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(ApplicationDbContext db, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int productId, int qty)
        {
            string userId = getUserId();
            using var transaction = _db.Database.BeginTransaction();
            try { 
            //cart gets saved
            //cart details go into an error
            
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged in");
            var cart = await getCart(userId);
            if (cart is null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                _db.Carts.Add(cart);
            }

                _db.SaveChanges();
                //Cart Details:
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.CartId == cart.Id && a.ProductId == productId);
                if (cartItem is not null)
                {

                    cartItem.Quantity += qty;


                }
                else
                {
                    var product = _db.Products.Find(productId);
                    cartItem = new CartDetails
                    {
                        ProductId = productId,
                        CartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = product.Price
                    };

                    _db.CartDetails.Add(cartItem);
                }

                _db.SaveChanges();
                transaction.Commit();

           }
            catch (Exception ex)
            {
            }
            var cartItemCount = await getCartItemCount(userId);
            return cartItemCount;
        }

         public async Task<int>RemoveItem(int productId)
        {
            string userId = getUserId();
            //using var transaction = _db.database.begintransaction();
            try
            {
                //cart gets saved
                //cart details go into an error

                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged in");
                var cart = await getCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Invalid Cart");
                _db.SaveChanges();
                //Cart Details:
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.CartId == cart.Id && a.ProductId == productId);
                if (cartItem is null)
                    throw new InvalidOperationException("There is no item in the cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
                //transaction.Commit();               
            }
            catch (Exception ex)
            {
            
            }
            var cartItemCount = await getCartItemCount(userId);
            return cartItemCount;
        }
        
        public async Task<Cart> getUserCart()
        {
            var userId = getUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid User");
            var cart = await _db.Carts
               .Include (a=> a.cartDetails)
               .ThenInclude(a => a.Products)
               .ThenInclude(a => a.Stocks)
               .Include(a => a.cartDetails)
               .ThenInclude(a => a.Products)
               .ThenInclude(a => a.Categories)
               .Where(a => a.UserId == userId).FirstOrDefaultAsync();

            return cart;    

        }
        public async Task <Cart> getCart(string userId)
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> getCartItemCount (string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = getUserId();
            }
            var data = await (from cart in _db.Carts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.CartId
                              where cart.UserId == userId
                              select new { cartDetail.Id })
                              .ToListAsync();
            return data.Count;
        }


        public async Task<bool> StartCheckout (CheckoutModel model)
        {

            using var transaction = _db.Database.BeginTransaction();

            try
            {
                //move from cartdetail to order and order detail
                //neworder , orderdetails
                // remove data in cartdetails

                var userId = getUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User not logged in");
                var cart = await getCart(userId);
                if (cart is null)
                    throw new InvalidOperationException("Not a valid cart");
                var cartDetail = await _db.CartDetails.
                    Where(a => a.CartId == cart.Id).Include(a=>a.Products).ToListAsync();
                
                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Empty Cart");


                var pendingRec = _db.OrderStat.FirstOrDefault(s => s.StatName == "Pending");
                if (pendingRec is null)
                    throw new InvalidOperationException("Order status does not have pending status");
                
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
                    OrderStatId = pendingRec.Id, //pend
                };
                   
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();


                foreach (var item in cartDetail)
                {
                    // Validate stock BEFORE writing line
                    var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.ProductId == item.ProductId);
                    if (stock is null)
                        throw new InvalidOperationException("Stocks is null");

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

                       
                        UnitCostAtSale = item.Products.CostPrice
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
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
