using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UserOrderRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task changeOrderStat(OrderStatusUpdateModel data)
        {
            var order = await _db.Orders.FindAsync(data.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException($"order with the following id:{data.OrderId} cannot be found");
            }
            order.OrderStatId = data.OrderStatId;
            await _db.SaveChangesAsync();
        }

        public async Task<Order?> getOrderbyId(int id)
        {
            return await _db.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<OrderStat>> getOrderStats()
        {
            return await _db.OrderStat.ToListAsync();
        }

        public async Task togglePayStatus(int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with Id:{orderId} cannot be found");
            }
            order.isPaid = !order.isPaid;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
        {
            var orders = _db.Orders
                .Include(x => x.OrderStat).Include(x => x.OrderDetail)
                .ThenInclude(x => x.Product).ThenInclude(x => x.Categories).AsQueryable();

            if (!getAll)
            {
                var userId = getUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User needs to log-in");
                orders = orders.Where (a => a.UserId == userId);
                return await orders.ToListAsync();
            }
            return await orders.ToListAsync();
        }

        private string getUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
