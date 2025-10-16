using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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



        //Shops DbSets:
        public DbSet<Shop> Shops => Set<Shop>();
        public DbSet<ShopUser> ShopUsers => Set<ShopUser>();
        public DbSet<ShopInventory> ShopInventories => Set<ShopInventory>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<ShopSale> ShopSales => Set<ShopSale>();
        public DbSet<ShopSaleLine> ShopSaleLines => Set<ShopSaleLine>();


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



            // Shop items relationships and restriction  (su -> shopuser, s -> shops, si -> shop inventory)



            b.Entity<ShopUser>()
                .HasKey(su => new { su.ShopId, su.UserId });

            b.Entity<ShopUser>()
                .HasOne(su => su.Shop)
                .WithMany(s => s.ShopUsers)
                .HasForeignKey(su => su.ShopId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting shops will remove shopuser links

            b.Entity<ShopUser>()
                .HasOne(su => su.User)
                .WithMany()
                .HasForeignKey(su => su.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Do not delete users through here

         
            b.Entity<ShopInventory>()
                .HasOne(si => si.Shop)
                .WithMany(s => s.Inventory)
                .HasForeignKey(si => si.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

    
            b.Entity<ShopInventory>()
                .HasOne(si => si.Product)
                .WithMany()
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

         
            b.Entity<ShopInventory>()
                .HasIndex(si => new { si.ShopId, si.ProductId })
                .IsUnique();

        
            b.Entity<ShopSale>()
                .HasOne(ss => ss.Shop)
                .WithMany()
                .HasForeignKey(ss => ss.ShopId)
                .OnDelete(DeleteBehavior.Restrict);

           
            b.Entity<ShopSale>()
                .HasOne(ss => ss.Customer)
                .WithMany()
                .HasForeignKey(ss => ss.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

      
            b.Entity<ShopSaleLine>()
                .HasOne(sl => sl.ShopSale)
                .WithMany(s => s.Lines)
                .HasForeignKey(sl => sl.ShopSaleId)
                .OnDelete(DeleteBehavior.Cascade);

            
            b.Entity<ShopSaleLine>()
                .HasOne(sl => sl.ShopInventory)
                .WithMany()
                .HasForeignKey(sl => sl.ShopInventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Make sure that the money is precise
            b.Entity<ShopInventory>().Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            b.Entity<ShopInventory>().Property(p => p.UnitCost).HasColumnType("decimal(18,2)");
            b.Entity<ShopSaleLine>().Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            b.Entity<ShopSaleLine>().Property(p => p.UnitCost).HasColumnType("decimal(18,2)");
            b.Entity<ShopSaleLine>().Property(p => p.LineTotal).HasColumnType("decimal(18,2)");
            b.Entity<ShopSaleLine>().Property(p => p.LineProfit).HasColumnType("decimal(18,2)");
            b.Entity<ShopSale>().Property(p => p.Subtotal).HasColumnType("decimal(18,2)");
            b.Entity<ShopSale>().Property(p => p.Discount).HasColumnType("decimal(18,2)");
            b.Entity<ShopSale>().Property(p => p.Total).HasColumnType("decimal(18,2)");

            b.Entity<Customer>()
                .HasIndex(c => c.PhoneNormalized)
                .IsUnique();
        }





    }
}
