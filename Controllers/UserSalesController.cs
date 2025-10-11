using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EasyGamesWeb.Repositories;
using EasyGamesWeb.Constants;

namespace EasyGamesWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserSalesController : Controller
    {
        private readonly IUserSalesService _sales;
        private readonly UserManager<IdentityUser> _userManager;

        public UserSalesController(IUserSalesService sales, UserManager<IdentityUser> userManager)
        {
            _sales = sales;
            _userManager = userManager;
        }

        public async Task<IActionResult> MySales()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var rows = await _sales.GetUserSalesHistory(userId);
            var tier = await _sales.GetUserTier(userId);

            var vm = new UserSalesHistoryVM
            {
                Rows = rows,
                ProfitTotal = tier.Profit,
                Tier = tier.Tier,
                RemainingToNext = tier.RemainingToNext
            };
            return View(vm);
        }
    }

    public class UserSalesHistoryVM
    {
        public IReadOnlyList<UserSaleRow> Rows { get; set; } = new List<UserSaleRow>();
        public decimal ProfitTotal { get; set; }
        public UserTier Tier { get; set; }
        public decimal RemainingToNext { get; set; }
    }
}
