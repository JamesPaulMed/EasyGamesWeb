using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Controllers
{
    [AllowAnonymous]
    public class UnsubscribeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public UnsubscribeController(ApplicationDbContext db, UserManager<IdentityUser> um)
        { _db = db; _um = um; }


        public async Task<IActionResult> Index(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest();

            var user = await _um.FindByEmailAsync(email);
            if (user != null)
            {
                var pref = await _db.EmailPreferences.FirstOrDefaultAsync(p => p.UserId == user.Id)
                           ?? new EmailPreference { UserId = user.Id };
                pref.AllowMarketing = false;
                pref.UpdatedUtc = DateTime.UtcNow;
                _db.Update(pref);
                await _db.SaveChangesAsync();
            }
            return View();
        }
    }
}
