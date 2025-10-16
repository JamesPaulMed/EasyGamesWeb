using EasyGamesWeb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddTransient<IStocksRepository, StocksRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserPurchasesService, UserPurchasesService>();
builder.Services.AddScoped<IUserSalesService, UserSalesService>();
builder.Services.AddScoped<IAdminSalesService, AdminSalesService>();
builder.Services.AddScoped<IShopStockService, ShopStockService>();
builder.Services.AddScoped<IShopAccessService, ShopAccessService>();
builder.Services.AddScoped<IShopSalesService, ShopSalesService>();
builder.Services.AddScoped<FileService>();


builder.Services.AddScoped(_ => TierThresholds.Default);
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedDefaultData(scope.ServiceProvider);
    await DbSeeder.SeedCategories(scope.ServiceProvider);
    await DbSeeder.SeedOrderStat(scope.ServiceProvider);
    await DbSeeder.SeedProductsAndOwnerStock(scope.ServiceProvider);
}





// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
