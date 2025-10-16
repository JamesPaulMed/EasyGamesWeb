using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EasyGamesWeb.Controllers

{
    [Authorize(Roles = "Owner,Admin")]

    public class OwnerShopController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OwnerShopController (ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
      => View(await _db.Shops.AsNoTracking().ToListAsync());

        public IActionResult Create() => View(new Shop());

        [HttpPost]
        public async Task<IActionResult> Create(Shop model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Shops.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shop = await _db.Shops.FindAsync(id);
            if (shop == null) return NotFound();
            return View(shop);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Shop model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var shop = await _db.Shops.FindAsync(id);
            if (shop == null) return NotFound();
            _db.Remove(shop);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
