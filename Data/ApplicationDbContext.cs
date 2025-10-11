using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet <Product> Products { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartDetails> CartDetails { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<OrderStat> OrderStat { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet <Stocks> Stocks { get; set; } 
    }
}
