using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyGamesWeb.Controllers
{
    [Authorize(Roles = "ShopOwner,Admin")]
    public class ShopStockController : Controller
    {
        private readonly IShopStockService _stock;
        public ShopStockController(IShopStockService stock) => _stock = stock;
        public async Task<IActionResult> Index(int shopId)
            => View(await _stock.GetShopInventory(shopId));

        [HttpPost]
        public async Task<IActionResult> Add(int shopId, int productId, int qty, decimal? priceOverride)
        {
            await _stock.AddOrIncreaseStock(shopId, productId, qty, priceOverride);
            return RedirectToAction(nameof(Index), new { shopId });
        }
       
    }

}
