using Microsoft.EntityFrameworkCore;
using EasyGamesWeb.Models;

namespace EasyGamesWeb.Repositories
{
    public interface IShopStockService
    {
        Task<IReadOnlyList<ShopInventory>> GetShopInventory(int shopId);
        Task<ShopInventory> AddOrIncreaseStock(int shopId, int productId, int qty, decimal? unitPriceOverride = null);
        Task<bool> IncreaseQuantity(int shopInventoryId, int addQty);
    }

    public class ShopStockService : IShopStockService
    {
        private readonly ApplicationDbContext _db;
        public ShopStockService(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<ShopInventory>> GetShopInventory(int shopId)
            => await _db.ShopInventories
                .Include(i => i.Product)
                .Where(i => i.ShopId == shopId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<ShopInventory> AddOrIncreaseStock(int shopId, int productId, int qty, decimal? unitPriceOverride = null)
        {
            if (!await _db.Products.AnyAsync(p => p.Id == productId))
                throw new InvalidOperationException("Product not found in owner inventory.");

            var prod = await _db.Products.AsNoTracking().FirstAsync(p => p.Id == productId);

            var inv = await _db.ShopInventories
                .FirstOrDefaultAsync(i => i.ShopId == shopId && i.ProductId == productId);

            if (inv == null)
            {
                inv = new ShopInventory
                {
                    ShopId = shopId,
                    ProductId = productId,
                    UnitPrice = unitPriceOverride ?? prod.Price, 
                    UnitCost = prod.CostPrice,               
                    Quantity = qty
                };
                _db.ShopInventories.Add(inv);
            }
            else
            {
                inv.Quantity += qty;                          
                if (unitPriceOverride.HasValue) inv.UnitPrice = unitPriceOverride.Value;
                inv.LastUpdatedUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return inv;
        }

        public async Task<bool> IncreaseQuantity(int shopInventoryId, int addQty)
        {
            if (addQty <= 0) return false;
            var inv = await _db.ShopInventories.FindAsync(shopInventoryId);
            if (inv == null) return false;

            inv.Quantity += addQty;                 // price stays the same 
            inv.LastUpdatedUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }


    }
}
