// Controllers/ShopAssignmentsController.cs
using EasyGamesWeb.Models;
using EasyGamesWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")] // only site owner/admin can assign
public class ShopAssignmentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;
    private readonly RoleManager<IdentityRole> _roles;

    private const string ShopOwnerRole = "ShopOwner";

    public ShopAssignmentsController(ApplicationDbContext db, UserManager<IdentityUser> users, RoleManager<IdentityRole> roles)
    {
        _db = db; _users = users; _roles = roles;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _db.ShopUsers
            .Include(su => su.Shop)
            .Include(su => su.User)
            .OrderBy(su => su.Shop.Name)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Shops = new SelectList(await _db.Shops.OrderBy(s => s.Name).ToListAsync(), "Id", "Name");
        ViewBag.Users = new SelectList(await _users.Users.OrderBy(u => u.Email).ToListAsync(), "Id", "Email");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int shopId, string userId)
    {
        // ensure role exists
        if (!await _roles.RoleExistsAsync(ShopOwnerRole))
            await _roles.CreateAsync(new IdentityRole(ShopOwnerRole));

        // add mapping if not exists
        var exists = await _db.ShopUsers.AnyAsync(su => su.ShopId == shopId && su.UserId == userId);
        if (!exists)
        {
            _db.ShopUsers.Add(new ShopUser { ShopId = shopId, UserId = userId, IsManager = true });
            await _db.SaveChangesAsync();
        }

        // put user in proprietor role
        var user = await _users.FindByIdAsync(userId);
        if (user != null && !await _users.IsInRoleAsync(user, ShopOwnerRole))
            await _users.AddToRoleAsync(user, ShopOwnerRole);

        TempData["Msg"] = "User assigned to shop.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int shopId, string userId)
    {
        var su = await _db.ShopUsers.FindAsync(shopId, userId);
        if (su != null)
        {
            _db.ShopUsers.Remove(su);
            await _db.SaveChangesAsync();
        }
        TempData["Msg"] = "Assignment removed.";
        return RedirectToAction(nameof(Index));
    }
}
