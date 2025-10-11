using EasyGamesWeb.Constants;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EasyGamesWeb.Data
{
    public class DbSeeder
    {

        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            //create admin user

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

    }
}
