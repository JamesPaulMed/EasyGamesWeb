using System.Linq;
using System.Threading.Tasks;
using EasyGamesWeb.Data;
using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OwnerStocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnerStocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OwnerStocks
        public async Task<IActionResult> Index()
        {
            var items = await _context.OwnerStocks
                .Include(o => o.Product)
                .OrderBy(o => o.Product.ProductName)   // ✅ ProductName
                .ToListAsync();

            return View(items);
        }

        // GET: OwnerStocks/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.OrderBy(p => p.ProductName), "Id", "ProductName"); // ✅
            return View(new OwnerStock { Quantity = 0 });
        }

        // POST: OwnerStocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Quantity")] OwnerStock ownerStock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ownerStock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products.OrderBy(p => p.ProductName), "Id", "ProductName", ownerStock.ProductId); // ✅
            return View(ownerStock);
        }

        // GET: OwnerStocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ownerStock = await _context.OwnerStocks.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (ownerStock == null) return NotFound();

            ViewData["ProductName"] = ownerStock.Product.ProductName; // ✅
            return View(ownerStock);
        }

        // POST: OwnerStocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity")] OwnerStock ownerStock)
        {
            if (id != ownerStock.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownerStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.OwnerStocks.Any(e => e.Id == ownerStock.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ownerStock);
        }

        // GET: OwnerStocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var ownerStock = await _context.OwnerStocks
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ownerStock == null) return NotFound();

            return View(ownerStock);
        }

        // GET: OwnerStocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ownerStock = await _context.OwnerStocks
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ownerStock == null) return NotFound();

            return View(ownerStock);
        }

        // POST: OwnerStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ownerStock = await _context.OwnerStocks.FindAsync(id);
            if (ownerStock != null)
            {
                _context.OwnerStocks.Remove(ownerStock);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
