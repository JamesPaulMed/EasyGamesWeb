using EasyGamesWeb.Constants;
using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userInDb = await userMgr.FindByEmailAsync(admin.Email);

            if (userInDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }

        public static async Task SeedCategories(IServiceProvider service)
        {
            var db = service.GetRequiredService<ApplicationDbContext>();

            if (!db.Categories.Any())
            {
                db.Categories.AddRange(
                    new Category { CategoryName = "Books" },
                    new Category { CategoryName = "Toys" },
                    new Category { CategoryName = "Games" }
                );
                await db.SaveChangesAsync();
            }
        }

        public static async Task SeedOrderStat(IServiceProvider service)
        {
            var db = service.GetRequiredService<ApplicationDbContext>();

            if (!db.OrderStat.Any())
            {
                db.OrderStat.AddRange(
                    new OrderStat { StatName = "Pending" },
                    new OrderStat { StatName = "Delivered" },
                    new OrderStat { StatName = "Refunded" }
                );
                await db.SaveChangesAsync();
            }
        }

        // NEW: dev demo products + owner stock
        public static async Task SeedProductsAndOwnerStock(IServiceProvider service)
        {
            var db = service.GetRequiredService<ApplicationDbContext>();

            if (!await db.Products.AnyAsync())
            {
                var gamesCatId = db.Categories.FirstOrDefault(c => c.CategoryName == "Games")?.Id;
                var booksCatId = db.Categories.FirstOrDefault(c => c.CategoryName == "Books")?.Id;
                var toysCatId = db.Categories.FirstOrDefault(c => c.CategoryName == "Toys")?.Id;

                var p1 = new Product { ProductName = "Settlers of Catan", CategoryId = gamesCatId, Source = "Asmodee", BuyPrice = 25m, SellPrice = 49.99m, Sku = "CATAN-BASE", IsActive = true };
                var p2 = new Product { ProductName = "The Hobbit", CategoryId = booksCatId, Source = "HarperCollins", BuyPrice = 7m, SellPrice = 14.99m, Sku = "HOBBIT-PB", IsActive = true };
                var p3 = new Product { ProductName = "LEGO Starter Box", CategoryId = toysCatId, Source = "LEGO", BuyPrice = 18m, SellPrice = 39.99m, Sku = "LEGO-START", IsActive = true };

                db.Products.AddRange(p1, p2, p3);
                await db.SaveChangesAsync();

                db.OwnerStocks.AddRange(
                    new OwnerStock { ProductId = p1.Id, Quantity = 20 },
                    new OwnerStock { ProductId = p2.Id, Quantity = 50 },
                    new OwnerStock { ProductId = p3.Id, Quantity = 15 }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
