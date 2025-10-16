using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ShopsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _um;
    private readonly IShopAccessService _access;
    private readonly IShopStockService _stock;
    private readonly IShopSalesService _shopSales;

    public ShopsController( ApplicationDbContext db, UserManager<IdentityUser> um, IShopAccessService access, IShopStockService stock, IShopSalesService shopSales)
    {
        _db = db; 
        _um = um; 
        _access = access;
        _stock = stock; 
        _shopSales = shopSales;
    }

    // -------- ADMIN ONLY --------
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
        => View(await _db.Shops.AsNoTracking().ToListAsync());

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Shop());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(Shop model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Shops.Add(model);
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Shop created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var shop = await _db.Shops.FindAsync(id);
        if (shop == null) return NotFound();
        return View(shop);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(Shop model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Shop saved.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var shop = await _db.Shops.FindAsync(id);
        if (shop == null) return NotFound();
        _db.Remove(shop);
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Shop deleted.";
        return RedirectToAction(nameof(Index));
    }




    // -------- SHOPOWNER or SiteOwner --------
    [Authorize(Roles = "Admin,ShopOwner")]
    public async Task<IActionResult> MyShops()
    {
        if (User.IsInRole("Admin"))
            return View(await _db.Shops.AsNoTracking().OrderBy(s => s.Name).ToListAsync());

        var userId = _um.GetUserId(User)!;
        var shops = await _db.ShopUsers
            .Where(su => su.UserId == userId)
            .Select(su => su.Shop)
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(shops);
    }

    [Authorize(Roles = "Admin,ShopOwner")]
    public async Task<IActionResult> Stock(int shopId)
    {
       

        var shop = await _db.Shops.FindAsync(shopId);
        if (shop == null) return NotFound();


        var inv = await _stock.GetShopInventory(shopId);
        ViewBag.ShopId = shopId;
        ViewBag.ShopName = shop.Name;


        var existingProductIds = inv.Select(i => i.ProductId).ToHashSet();

    
        var ownerProducts = await _db.Products
            .Where(p => !existingProductIds.Contains(p.Id))
            .OrderBy(p => p.ProductName)
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                OwnerPrice = p.Price,
                OwnerCost = p.CostPrice,
                OwnerQty = _db.Stocks.Where(s => s.ProductId == p.Id).Select(s => (int?)s.Quantity).FirstOrDefault() ?? 0
            })
            .ToListAsync();

        var items = ownerProducts.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = $"{p.ProductName} — owner stock: {p.OwnerQty}",
          
        }).ToList();

        
        ViewBag.ProductsRaw = ownerProducts;
        ViewBag.Products = items;            

        return View("Stock", inv);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ShopOwner")]
    public async Task<IActionResult> AddStock(int shopId, int productId, int qty)
    {
        if (!User.IsInRole("Admin"))
        {
            var userId = _um.GetUserId(User)!;
            if (!await _access.CanManageShop(userId, shopId)) return Forbid();
        }

        if (qty <= 0) { TempData["Err"] = "Quantity must be greater than 0."; return RedirectToAction(nameof(Stock), new { shopId }); }

        await _stock.AddOrIncreaseStock(shopId, productId, qty);
        TempData["Msg"] = "Stock added.";
        return RedirectToAction(nameof(Stock), new { shopId });
    }

    private async Task<bool> EnsureAccess(int shopId)
    {
        if (User.IsInRole("Admin")) return true;
        var userId = _um.GetUserId(User)!;
        return await _access.CanManageShop(userId, shopId);
    }
    public async Task<IActionResult> Pos(int shopId, int? customerId = null, string? tier = null)
    {
        if (!await EnsureAccess(shopId)) return Forbid();

        var shop = await _db.Shops.FindAsync(shopId);
        if (shop == null) return NotFound();

        var inv = await _db.ShopInventories
            .Include(i => i.Product)
            .Where(i => i.ShopId == shopId)
            .OrderBy(i => i.Product.ProductName)
            .ToListAsync();

        ViewBag.ShopId = shopId;
        ViewBag.ShopName = shop.Name;
        ViewBag.CustomerId = customerId;
        ViewBag.Tier = tier ?? "Guest";

        return View("Pos", inv); 
    }

    [HttpPost]
    public async Task<IActionResult> LookupCustomer(int shopId, string phone)
    {
        if (!await EnsureAccess(shopId)) return Forbid();

        try
        {
            var (customer, tier) = await _shopSales.FindOrCreateCustomerByPhone(phone);
            return RedirectToAction(nameof(Pos), new { shopId, customerId = customer.Id, tier = (tier?.ToString() ?? "Guest") });
        }
        catch (Exception ex)
        {
            TempData["Err"] = ex.Message;
            return RedirectToAction(nameof(Pos), new { shopId });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ShopOwner")]
    public async Task<IActionResult> IncreaseQty(int shopId, int shopInventoryId, int addQty)
    {
        // access check for ShopOwner
        if (!User.IsInRole("Admin"))
        {
            var userId = _um.GetUserId(User)!;
            if (!await _access.CanManageShop(userId, shopId)) return Forbid();
        }

        if (addQty <= 0)
        {
            TempData["Err"] = "Quantity must be greater than 0.";
            return RedirectToAction(nameof(Stock), new { shopId });
        }

        var ok = await _stock.IncreaseQuantity(shopInventoryId, addQty);
        TempData[ok ? "Msg" : "Err"] = ok ? "Quantity increased." : "Inventory row not found.";
        return RedirectToAction(nameof(Stock), new { shopId });
    }

    // Checkout 

    [HttpPost]
    public async Task<IActionResult> Checkout(int shopId, int? customerId, List<int> shopInventoryIds, List<int> quantities)
    {
        if (!await EnsureAccess(shopId)) return Forbid();

        var items = shopInventoryIds.Zip(quantities, (id, qty) => (id, qty))
                                    .Where(z => z.qty > 0)
                                    .Select(z => (shopInventoryId: z.id, qty: z.qty))
                                    .ToList();
        if (!items.Any())
        {
            TempData["Err"] = "Add at least one item.";
            return RedirectToAction(nameof(Pos), new { shopId, customerId });
        }

        var sale = await _shopSales.CreateSale(shopId, customerId, items);
        TempData["Msg"] = $"Sale #{sale.Id} completed. Total: {sale.Total:C}";
        return RedirectToAction(nameof(Pos), new { shopId, customerId });
    }

}
