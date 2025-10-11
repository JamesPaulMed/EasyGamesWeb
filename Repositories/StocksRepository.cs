using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories
{
    public class StocksRepository : IStocksRepository

    {

        private readonly ApplicationDbContext _context;
        public StocksRepository(ApplicationDbContext context)
        {
            _context = context; 
        }

        public async Task<Stocks?> getStocksByProductId(int productId) => await
            _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);

        public async Task manageStocks (StocksDTO stocksToManage)
        {
            var currentStock = await getStocksByProductId(stocksToManage.ProductId);
            if (currentStock is null)
            {
                var stocks = new Stocks { ProductId = stocksToManage.ProductId, Quantity = stocksToManage.Quantity }; 
                _context.Stocks.Add(stocks);
            }
            else
            {
                currentStock.Quantity = stocksToManage.Quantity;
            }

            await _context.SaveChangesAsync();
        }
        public async Task <IEnumerable<StocksDisplayModel>> getStocks(string sTerm = "")
        {
            var stocks = await (from product in _context.Products
                                join stock in _context.Stocks
                                on product.Id equals stock.ProductId
                                into stock_product
                                from stockProduct in stock_product.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) ||
                                product.ProductName.ToLower().Contains(sTerm.ToLower())
                                select new StocksDisplayModel
                                {
                                    ProductId = product.Id,
                                    ProductName = product.ProductName,
                                    Quantity = stockProduct == null ? 0 : stockProduct.Quantity
                                }).ToListAsync();
            return stocks; 
        }

    }

    public interface IStocksRepository
    {

        Task<IEnumerable<StocksDisplayModel>> getStocks(string sTerm = "");
        Task manageStocks(StocksDTO stocksToManage);

        Task<Stocks?> getStocksByProductId(int productId);
    }
}
