using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EasyGamesWeb.Models;
namespace EasyGamesWeb.Controllers
{
  
    [Authorize]
    public class PurchasesController : Controller
    {
        private readonly IUserPurchasesService _purchases;
        private readonly UserManager<IdentityUser> _um;
        public PurchasesController(IUserPurchasesService purchases, UserManager<IdentityUser> um)
        { _purchases = purchases; _um = um; }

        public async Task<IActionResult> MyPurchases()
        {
            var userId = _um.GetUserId(User);
            var rows = await _purchases.GetUserPurchases(userId);

            // group by order to show order totals
            var vm = rows.GroupBy(a => new { a.OrderId, a.CreateDate })
                         .Select(b => new Purchases
                         {
                             OrderId = b.Key.OrderId,
                             CreateDate = b.Key.CreateDate,
                             Lines = b.ToList(),
                             OrderTotal = b.Sum(c => c.LineTotal)
                         })
                         .OrderByDescending(c => c.CreateDate)
                         .ToList();

            return View(vm);
        }
    }

}
