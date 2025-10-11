using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace EasyGamesWeb.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
          _db = db;
        }

        public async Task<IEnumerable<Category>> Categories()

        {
            return await _db.Categories.ToListAsync();
        }


        public async Task<IEnumerable<Product>> DisplayProducts(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable <Product> 
                            products = await (
                            from product in _db.Products
                            join category in _db.Categories
                            on product.CategoryId equals category.Id
                            join stocks in _db.Stocks
                            on product.Id equals stocks.ProductId
                            into stocks_product
                            from productWithStocks in stocks_product.DefaultIfEmpty()
                            where string.IsNullOrWhiteSpace(sTerm) || 
                            (product !=null && product.ProductName.ToLower().StartsWith(sTerm))
                            select new Product()
                            {
                                Id = product.Id,
                                Image = product.Image,
                                Price = product.Price,
                                ProductName = product.ProductName,
                                ShortDesc = product.ShortDesc,
                                CategoryId = product.CategoryId,
                                CategoryName = category.CategoryName,
                                Quantity = productWithStocks == null? 0:productWithStocks.Quantity
                            }
                            ).ToListAsync();

            if (categoryId > 0) 
            {
                products = products.Where(a => a.CategoryId == categoryId).ToList();
            }
            return products;
        }
    }
}
