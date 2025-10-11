using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyGamesWeb.Repositories;

namespace EasyGamesWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalesController : Controller
    {
        private readonly IAdminSalesService _adminSales;

        public SalesController(IAdminSalesService adminSales) => _adminSales = adminSales;

        public async Task<IActionResult> Dashboard()
        {
            var rows = await _adminSales.GetAllSalesHistory();
            var totalProfit = await _adminSales.GetTotalProfitAll();

            var vm = new SalesDashboardVM
            {
                Rows = rows,
                TotalProfit = totalProfit
            };
            return View(vm);
        }
    }

    public class SalesDashboardVM
    {
        public IReadOnlyList<AdminSaleRow> Rows { get; set; } = Array.Empty<AdminSaleRow>();
        public decimal TotalProfit { get; set; }
    }
}
