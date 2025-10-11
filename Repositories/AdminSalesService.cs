using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories
{
    public record AdminSaleRow(
        int OrderId, string UserId, DateTime CreateDate,
        string ProductName, int Quantity,
        double UnitPrice, double UnitCostAtSale,
        double LineRevenue, double LineProfit);

    public record TierSummary(string Tier, decimal TotalProfit, int Users);

    public interface IAdminSalesService
    {
        Task<IReadOnlyList<AdminSaleRow>> GetAllSalesHistory();
        Task<decimal> GetTotalProfitAll();

    }

    public class AdminSalesService : IAdminSalesService
    {
        private readonly ApplicationDbContext _db;
        private static readonly string[] Finals = { "Delivered", "Completed", "Shipped" };

        public AdminSalesService(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<AdminSaleRow>> GetAllSalesHistory()
        {
            
            var rows = await _db.Orders.AsNoTracking()
                .Where(a => a.isPaid && Finals.Contains(a.OrderStat.StatName.Trim()))
                .SelectMany(a => a.OrderDetail.Select(b => new
                {
                    a.Id,
                    a.UserId,
                    a.CreateDate,
                    ProductName = b.Product.ProductName,
                    b.Quantity,
                    b.UnitPrice,          
                    b.UnitCostAtSale
                }))
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

            return rows.Select(c => new AdminSaleRow(
                c.Id, c.UserId, c.CreateDate, c.ProductName, c.Quantity,
                c.UnitPrice, c.UnitCostAtSale,
                c.UnitPrice * c.Quantity,
                (c.UnitPrice - c.UnitCostAtSale) * c.Quantity
            )).ToList();
        }

        public async Task<decimal> GetTotalProfitAll()
        {
            var profitDecimal = await _db.OrderDetails
                .Where(b => b.Order.isPaid && Finals.Contains(b.Order.OrderStat.StatName.Trim()))
                .SumAsync(b =>
                    ((decimal)b.UnitPrice - (decimal)b.UnitCostAtSale) * (decimal)b.Quantity);

                   return profitDecimal;
        }
    }
}
