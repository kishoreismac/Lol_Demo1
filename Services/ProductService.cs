using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Repositories;

namespace LandOLakesDairyDemo.Services;

public class ProductService : IProductService
{
    private static readonly string[] AllowedCategories =
    {
        "Butter & Spreads",
        "Cheese",
        "Whipping Cream & Half & Half"
    };

    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<IReadOnlyList<Product>> GetProductsAsync(string? searchTerm, string? category, bool featuredOnly = false)
    {
        return _productRepository.GetProductsAsync(searchTerm?.Trim(), category, featuredOnly);
    }

    public async Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(int take)
    {
        var featuredProducts = await _productRepository.GetProductsAsync(null, null, featuredOnly: true);
        return featuredProducts.Take(take).ToList();
    }

    public Task<IReadOnlyList<string>> GetCategoriesAsync()
    {
        return _productRepository.GetCategoriesAsync();
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return _productRepository.GetByIdAsync(id);
    }

    public async Task<ProductInputModel?> GetInputModelAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return null;
        }

        return new ProductInputModel
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Category = product.Category,
            Brand = product.Brand,
            ShortDescription = product.ShortDescription,
            PackageSize = product.PackageSize,
            Price = product.Price,
            IsFeatured = product.IsFeatured,
            ImageFileName = product.ImageFileName,
            Tags = product.Tags
        };
    }

    public async Task<(bool Success, Dictionary<string, string[]> Errors, Product? Product)> CreateAsync(ProductInputModel inputModel)
    {
        var errors = await ValidateAsync(inputModel);
        if (errors.Count > 0)
        {
            return (false, errors, null);
        }

        var timestamp = DateTime.UtcNow;
        var product = new Product
        {
            ProductId = inputModel.ProductId.Trim(),
            ProductName = inputModel.ProductName.Trim(),
            Category = inputModel.Category,
            Brand = inputModel.Brand.Trim(),
            ShortDescription = inputModel.ShortDescription.Trim(),
            PackageSize = inputModel.PackageSize.Trim(),
            Price = inputModel.Price,
            IsFeatured = inputModel.IsFeatured,
            ImageFileName = NormalizeOptional(inputModel.ImageFileName),
            Tags = NormalizeTags(inputModel.Tags),
            CreatedUtc = timestamp,
            UpdatedUtc = timestamp
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return (true, errors, product);
    }

    public async Task<(bool Success, Dictionary<string, string[]> Errors, Product? Product)> UpdateAsync(int id, ProductInputModel inputModel)
    {
        var product = await _productRepository.GetByIdAsync(id, trackChanges: true);
        if (product is null)
        {
            return (false, new Dictionary<string, string[]> { ["id"] = new[] { "Product not found." } }, null);
        }

        var errors = await ValidateAsync(inputModel, id);
        if (errors.Count > 0)
        {
            return (false, errors, null);
        }

        product.ProductId = inputModel.ProductId.Trim();
        product.ProductName = inputModel.ProductName.Trim();
        product.Category = inputModel.Category;
        product.Brand = inputModel.Brand.Trim();
        product.ShortDescription = inputModel.ShortDescription.Trim();
        product.PackageSize = inputModel.PackageSize.Trim();
        product.Price = inputModel.Price;
        product.IsFeatured = inputModel.IsFeatured;
        product.ImageFileName = NormalizeOptional(inputModel.ImageFileName);
        product.Tags = NormalizeTags(inputModel.Tags);
        product.UpdatedUtc = DateTime.UtcNow;

        await _productRepository.SaveChangesAsync();

        return (true, errors, product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id, trackChanges: true);
        if (product is null)
        {
            return false;
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    private async Task<Dictionary<string, string[]>> ValidateAsync(ProductInputModel inputModel, int? excludingId = null)
    {
        var errors = new Dictionary<string, string[]>();

        if (!AllowedCategories.Contains(inputModel.Category))
        {
            errors["Category"] = new[] { "Choose a supported category." };
        }

        if (await _productRepository.ProductIdExistsAsync(inputModel.ProductId.Trim(), excludingId))
        {
            errors["ProductId"] = new[] { "Product ID must be unique." };
        }

        return errors;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return null;
        }

        var normalizedTags = tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", normalizedTags);
    }
}