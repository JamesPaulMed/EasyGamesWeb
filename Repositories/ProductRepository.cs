using Microsoft.EntityFrameworkCore;

namespace EasyGamesWeb.Repositories;

public interface IProductRepository
{

    Task addProduct(Product product);
    Task deleteProduct(Product product);

    Task <Product?> getProductById(int id);

    Task<IEnumerable<Product>> getProducts();

    Task updateProduct(Product product);

}

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository (ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task addProduct(Product product)
    { 
    
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
   
    }

    public async Task updateProduct (Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task deleteProduct(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> getProductById(int id) => await 
        _context.Products.FindAsync(id);


    public async Task <IEnumerable<Product>> getProducts() => await
        _context.Products.Include(a=>a.Category).ToListAsync();
}
