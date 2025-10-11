using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EasyGamesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeReposity)
        {
            _homeRepository = homeReposity;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm = "", int categoryId = 0)
        {
            var products = await _homeRepository.DisplayProducts(sterm, categoryId);
            var categories = await _homeRepository.Categories();

            ViewBag.CategoryId = categoryId;   // remember selected category (0 = All)
            ViewBag.STerm = sterm;             // remember search term

            var productModel = new ProductDisplayModel
            {
                Products = products,
                Categories = categories
            };

            return View(productModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
