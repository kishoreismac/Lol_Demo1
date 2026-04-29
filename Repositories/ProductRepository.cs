using LandOLakesDairyDemo.Data;
using LandOLakesDairyDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace LandOLakesDairyDemo.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? searchTerm, string? category, bool featuredOnly = false)
    {
        var query = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(product => product.ProductName.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(product => product.Category == category);
        }

        if (featuredOnly)
        {
            query = query.Where(product => product.IsFeatured);
        }

        return await query
            .OrderBy(product => product.ProductName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync()
    {
        return await _dbContext.Products.AsNoTracking()
            .Select(product => product.Category)
            .Distinct()
            .OrderBy(category => category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id, bool trackChanges = false)
    {
        if (trackChanges)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(product => product.Id == id);
        }

        return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<bool> ProductIdExistsAsync(string productId, int? excludingId = null)
    {
        var query = _dbContext.Products.AsQueryable().Where(product => product.ProductId == productId);

        if (excludingId.HasValue)
        {
            query = query.Where(product => product.Id != excludingId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}