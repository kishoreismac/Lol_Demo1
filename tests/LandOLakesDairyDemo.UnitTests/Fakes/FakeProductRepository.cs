using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Repositories;

namespace LandOLakesDairyDemo.UnitTests.Fakes;

internal class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    public FakeProductRepository(IEnumerable<Product>? seedProducts = null)
    {
        _products = seedProducts?.ToList() ?? new List<Product>();
    }

    public int SaveChangesCallCount { get; private set; }

    public List<bool> GetByIdTrackChangesCalls { get; } = new();

    public Task<IReadOnlyList<Product>> GetProductsAsync(string? searchTerm, string? category, bool featuredOnly = false)
    {
        IEnumerable<Product> query = _products;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(product => product.ProductName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(product => product.Category == category);
        }

        if (featuredOnly)
        {
            query = query.Where(product => product.IsFeatured);
        }

        return Task.FromResult<IReadOnlyList<Product>>(query.OrderBy(product => product.ProductName).ToList());
    }

    public Task<IReadOnlyList<string>> GetCategoriesAsync()
    {
        return Task.FromResult<IReadOnlyList<string>>(_products.Select(product => product.Category).Distinct().OrderBy(category => category).ToList());
    }

    public Task<Product?> GetByIdAsync(int id, bool trackChanges = false)
    {
        GetByIdTrackChangesCalls.Add(trackChanges);
        return Task.FromResult(_products.FirstOrDefault(product => product.Id == id));
    }

    public Task<bool> ProductIdExistsAsync(string productId, int? excludingId = null)
    {
        var exists = _products.Any(product => product.ProductId == productId && (!excludingId.HasValue || product.Id != excludingId.Value));
        return Task.FromResult(exists);
    }

    public Task AddAsync(Product product)
    {
        product.Id = _products.Count == 0 ? 1 : _products.Max(existingProduct => existingProduct.Id) + 1;
        _products.Add(product);
        return Task.CompletedTask;
    }

    public void Remove(Product product)
    {
        _products.Remove(product);
    }

    public Task SaveChangesAsync()
    {
        SaveChangesCallCount++;
        return Task.CompletedTask;
    }

    public IReadOnlyList<Product> Snapshot() => _products.ToList();
}