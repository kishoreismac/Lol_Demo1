using LandOLakesDairyDemo.Models;

namespace LandOLakesDairyDemo.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? searchTerm, string? category, bool featuredOnly = false);
    Task<IReadOnlyList<string>> GetCategoriesAsync();
    Task<Product?> GetByIdAsync(int id, bool trackChanges = false);
    Task<bool> ProductIdExistsAsync(string productId, int? excludingId = null);
    Task AddAsync(Product product);
    void Remove(Product product);
    Task SaveChangesAsync();
}