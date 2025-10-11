using EasyGamesWeb.Constants; 
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories
{
    public record UserSaleRow(
        int OrderId,
        DateTime CreateDate,
        string ProductName,
        int Quantity,
        double UnitPrice,
        double UnitCostAtSale,
        double LineRevenue,
        double LineProfit);

    public interface IUserSalesService
    {
        Task<IReadOnlyList<UserSaleRow>> GetUserSalesHistory(string userId);
        Task<decimal> GetUserTotalProfit(string userId);
        Task<UserTierResult> GetUserTier(string userId);
    }

    public class UserSalesService : IUserSalesService
    {
        private readonly ApplicationDbContext _db;
        private readonly TierThresholds _tiers;

        private static readonly string[] Finals = { "DELIVERED", "COMPLETED", "SHIPPED" };

        public UserSalesService(ApplicationDbContext db, TierThresholds tiers)
        {
            _db = db;
            _tiers = tiers;
        }

        // Get all finalized sales for a user
        public async Task<IReadOnlyList<UserSaleRow>> GetUserSalesHistory(string userId)
        {
            var rows = await _db.Orders
                .AsNoTracking()
                .Where(a => a.UserId == userId
                    && a.isPaid
                    && (a.OrderStat.StatName == "Delivered"
                        || a.OrderStat.StatName == "Completed"
                        || a.OrderStat.StatName == "Shipped"))
                .SelectMany(a => a.OrderDetail.Select(b => new
        {
                    OrderId = a.Id,
                    a.CreateDate,
                    ProductName = b.Product.ProductName,
                    Quantity = b.Quantity,
                    UnitPrice = b.UnitPrice,
                    UnitCostAtSale = b.UnitCostAtSale,
                    LineRevenue = b.UnitPrice * (1.0 * b.Quantity),
                    
                    LineProfit = (b.UnitPrice - b.UnitCostAtSale) * (1.0 * b.Quantity)
                }))
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

            return rows
                .Select(c => new UserSaleRow(
                    c.OrderId, c.CreateDate, c.ProductName, c.Quantity,
                    c.UnitPrice, c.UnitCostAtSale, c.LineRevenue, c.LineProfit))
                .ToList();
        }


        // Get total profit
        public async Task<decimal> GetUserTotalProfit(string userId)
        {
            var profitDecimal = await _db.OrderDetails
                .Where(b => b.Order.UserId == userId && b.Order.isPaid && Finals.Contains(b.Order.OrderStat.StatName.Trim().ToUpper()))
                .SumAsync(b =>
                    ((decimal)b.UnitPrice - (decimal)b.UnitCostAtSale) * (decimal)b.Quantity);

            return profitDecimal;   
        }


        // Get tier info
        public async Task<UserTierResult> GetUserTier(string userId)
        {
            var profit = await GetUserTotalProfit(userId);
            return Tiering.FromProfit(profit, _tiers);    
        }
    }
}
