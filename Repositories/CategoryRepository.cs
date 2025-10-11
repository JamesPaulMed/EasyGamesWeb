using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace EasyGamesWeb.Repositories;

public interface ICategoryRepository 
{
    Task <Category?> getCategoryById (int id);

    Task<IEnumerable<Category>> getCategories();

    Task addCategory(Category category);

    Task updateCategory(Category category);

   Task deleteCategory(Category category);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository (ApplicationDbContext context)

    {
        _context = context;
    }

    public async Task addCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }
    public async Task updateCategory(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task deleteCategory(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> getCategoryById(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<IEnumerable<Category>> getCategories()
    {
        return await _context.Categories.ToListAsync();
    }
}
