using Microsoft.EntityFrameworkCore;
using EasyGamesWeb.Models;

namespace EasyGamesWeb.Repositories
{

    public interface IUserPurchasesService
    {
        Task<IReadOnlyList<UserPurchaseRow>> GetUserPurchases(string userId);
    }

    public class UserPurchasesService : IUserPurchasesService
    {
        private readonly ApplicationDbContext _db;
        public UserPurchasesService(ApplicationDbContext db) { _db = db; }

        public async Task<IReadOnlyList<UserPurchaseRow>> GetUserPurchases(string userId)
        {
            var rows = await _db.Orders
                .AsNoTracking()
                .Where(a => a.UserId == userId
                    && a.isPaid
                    && (
                        a.OrderStat.StatName == "Delivered"
                        || a.OrderStat.StatName == "Completed"
                        || a.OrderStat.StatName == "Shipped"
                    ))
                .SelectMany(a => a.OrderDetail.Select(b => new
                {
                    a.Id,
                    a.CreateDate,
                    ProductName = b.Product.ProductName,
                    b.Quantity,
                    b.UnitPrice,
                    LineTotal = b.UnitPrice * (1.0 * b.Quantity)  
                }))
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

   
            return rows
                .Select(c => new UserPurchaseRow(
                    c.Id, c.CreateDate, c.ProductName, c.Quantity, c.UnitPrice, c.LineTotal))
                .ToList();
        }


    }

}
