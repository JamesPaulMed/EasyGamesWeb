using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories
{
    public interface IShopAccessService
    {
        Task<bool> CanManageShop(string userId, int shopId);
    }

    public class ShopAccessService : IShopAccessService
    {
        private readonly ApplicationDbContext _db;
        public ShopAccessService(ApplicationDbContext db) => _db = db;

        public Task<bool> CanManageShop(string userId, int shopId) =>
            _db.ShopUsers.AnyAsync(su => su.UserId == userId && su.ShopId == shopId);
    }
}
