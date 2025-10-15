using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<OrderStat> OrderStat => Set<OrderStat>();

        
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartDetails> CartDetails => Set<CartDetails>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetails> OrderDetails => Set<OrderDetails>();

        
        public DbSet<Stocks> Stocks => Set<Stocks>();

        
        public DbSet<OwnerStock> OwnerStocks => Set<OwnerStock>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            
            b.Entity<Product>()
             .HasIndex(p => p.ProductName);

            
            b.Entity<OwnerStock>()
             .HasOne(os => os.Product)
             .WithMany()
             .HasForeignKey(os => os.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
