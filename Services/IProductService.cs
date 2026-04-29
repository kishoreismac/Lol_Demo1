using LandOLakesDairyDemo.Models;

namespace LandOLakesDairyDemo.Services;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? searchTerm, string? category, bool featuredOnly = false);
    Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(int take);
    Task<IReadOnlyList<string>> GetCategoriesAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<ProductInputModel?> GetInputModelAsync(int id);
    Task<(bool Success, Dictionary<string, string[]> Errors, Product? Product)> CreateAsync(ProductInputModel inputModel);
    Task<(bool Success, Dictionary<string, string[]> Errors, Product? Product)> UpdateAsync(int id, ProductInputModel inputModel);
    Task<bool> DeleteAsync(int id);
}