using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EasyGamesWeb.Controllers
{

    [Authorize(Roles = nameof(Roles.Admin))]
    public class StocksController : Controller
    {

        private readonly IStocksRepository _stocksRepo;

        public StocksController(IStocksRepository stocksRepo)
        {
            _stocksRepo = stocksRepo;
        }

        public async Task<IActionResult> Index (string sTerm = "")
        {
            var stocks = await _stocksRepo.getStocks(sTerm);
            return View(stocks);
        }

        public async Task <IActionResult> manageStocks (int productId)
            {
            var currentStocks = await _stocksRepo.getStocksByProductId(productId);
            var stocks = new StocksDTO
            {
                ProductId = productId,
                Quantity = currentStocks != null ? currentStocks.Quantity : 0
            };
            return View(stocks);
        }
        [HttpPost]

        public async Task <IActionResult> manageStocks(StocksDTO stocks)
        {
            if (!ModelState.IsValid)
                return View(stocks);

            try
            {
                await _stocksRepo.manageStocks(stocks);
                TempData["SuccessMsg"] = "Stock was changed";

            }

            catch (Exception ex) {
                TempData["ErrorMsg"] = "Stock was not changed";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
