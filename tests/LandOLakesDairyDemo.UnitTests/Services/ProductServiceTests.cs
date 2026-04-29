using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;
using LandOLakesDairyDemo.UnitTests.Fakes;

namespace LandOLakesDairyDemo.UnitTests.Services;

[TestClass]
public class ProductServiceTests
{
    [TestMethod]
    public async Task GetProductsAsync_TrimsSearchTerm()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Salted Butter", "LLD001"),
            CreateProduct(2, "Swiss Cheese", "LLD009", category: "Cheese")
        });

        var service = new ProductService(repository);

        var products = await service.GetProductsAsync("  Salted Butter  ", null);

        Assert.AreEqual(1, products.Count);
        Assert.AreEqual("Salted Butter", products[0].ProductName);
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsValidationError_ForUnsupportedCategory()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository);

        var inputModel = CreateInputModel();
        inputModel.Category = "Yogurt";

        var result = await service.CreateAsync(inputModel);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.ContainsKey("Category"));
        CollectionAssert.AreEqual(new[] { "Choose a supported category." }, result.Errors["Category"]);
        Assert.AreEqual(0, repository.Snapshot().Count);
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsValidationError_ForDuplicateProductId()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Salted Butter", "LLD001")
        });

        var service = new ProductService(repository);
        var inputModel = CreateInputModel();
        inputModel.ProductId = "LLD001";

        var result = await service.CreateAsync(inputModel);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.ContainsKey("ProductId"));
        CollectionAssert.AreEqual(new[] { "Product ID must be unique." }, result.Errors["ProductId"]);
    }

    [TestMethod]
    public async Task CreateAsync_SavesNormalizedProduct()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository);

        var inputModel = CreateInputModel();
        inputModel.ProductId = "  LLD099  ";
        inputModel.ProductName = "  Whipped Butter  ";
        inputModel.Brand = "  Land O Lakes  ";
        inputModel.ShortDescription = "  A whipped butter demo entry.  ";
        inputModel.PackageSize = "  8 oz tub  ";
        inputModel.ImageFileName = "   ";
        inputModel.Tags = " butter, whipped , butter, demo ";

        var result = await service.CreateAsync(inputModel);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Product);
        Assert.AreEqual("LLD099", result.Product.ProductId);
        Assert.AreEqual("Whipped Butter", result.Product.ProductName);
        Assert.AreEqual("Land O Lakes", result.Product.Brand);
        Assert.AreEqual("A whipped butter demo entry.", result.Product.ShortDescription);
        Assert.AreEqual("8 oz tub", result.Product.PackageSize);
        Assert.IsNull(result.Product.ImageFileName);
        Assert.AreEqual("butter, whipped, demo", result.Product.Tags);
        Assert.AreEqual(1, repository.SaveChangesCallCount);
        Assert.AreEqual(1, repository.Snapshot().Count);
    }

    [TestMethod]
    public async Task GetFeaturedProductsAsync_ReturnsRequestedCount()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Whipped Butter", "LLD001", isFeatured: true),
            CreateProduct(2, "Salted Butter", "LLD002", isFeatured: true),
            CreateProduct(3, "American Cheese", "LLD003", category: "Cheese", isFeatured: true)
        });

        var service = new ProductService(repository);

        var featuredProducts = await service.GetFeaturedProductsAsync(2);

        Assert.AreEqual(2, featuredProducts.Count);
        Assert.IsTrue(featuredProducts.All(product => product.IsFeatured));
    }

    [TestMethod]
    public async Task GetInputModelAsync_ReturnsMappedFields_WhenProductExists()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Salted Butter", "LLD001", imageFileName: "salted-butter.jpg", tags: "butter, salted")
        });

        var service = new ProductService(repository);

        var inputModel = await service.GetInputModelAsync(1);

        Assert.IsNotNull(inputModel);
        Assert.AreEqual("LLD001", inputModel.ProductId);
        Assert.AreEqual("Salted Butter", inputModel.ProductName);
        Assert.AreEqual("salted-butter.jpg", inputModel.ImageFileName);
        Assert.AreEqual("butter, salted", inputModel.Tags);
    }

    [TestMethod]
    public async Task UpdateAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository);

        var result = await service.UpdateAsync(42, CreateInputModel());

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.ContainsKey("id"));
        CollectionAssert.AreEqual(new[] { "Product not found." }, result.Errors["id"]);
        CollectionAssert.Contains(repository.GetByIdTrackChangesCalls, true);
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesExistingProduct_WithNormalizedValues()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Salted Butter", "LLD001", imageFileName: "salted-butter.jpg", tags: "butter")
        });

        var service = new ProductService(repository);
        var inputModel = CreateInputModel();
        inputModel.ProductId = "  LLD001  ";
        inputModel.ProductName = "  Salted Butter Updated  ";
        inputModel.Brand = "  Land O Lakes Test  ";
        inputModel.ShortDescription = "  Updated description.  ";
        inputModel.PackageSize = "  16 oz tub  ";
        inputModel.ImageFileName = "   ";
        inputModel.Tags = " butter, updated, butter ";

        var result = await service.UpdateAsync(1, inputModel);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Product);
        Assert.AreEqual("LLD001", result.Product.ProductId);
        Assert.AreEqual("Salted Butter Updated", result.Product.ProductName);
        Assert.AreEqual("Land O Lakes Test", result.Product.Brand);
        Assert.AreEqual("Updated description.", result.Product.ShortDescription);
        Assert.AreEqual("16 oz tub", result.Product.PackageSize);
        Assert.IsNull(result.Product.ImageFileName);
        Assert.AreEqual("butter, updated", result.Product.Tags);
        Assert.AreEqual(1, repository.SaveChangesCallCount);
        CollectionAssert.Contains(repository.GetByIdTrackChangesCalls, true);
    }

    [TestMethod]
    public async Task UpdateAsync_ReturnsValidationError_ForDuplicateProductId_OnExistingProduct()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Salted Butter", "LLD001"),
            CreateProduct(2, "Whipped Butter", "LLD002")
        });

        var service = new ProductService(repository);
        var inputModel = CreateInputModel();
        inputModel.ProductId = "LLD001";

        var result = await service.UpdateAsync(2, inputModel);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Errors.ContainsKey("ProductId"));
        CollectionAssert.AreEqual(new[] { "Product ID must be unique." }, result.Errors["ProductId"]);
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesExistingProduct()
    {
        var repository = new FakeProductRepository(new[]
        {
            CreateProduct(1, "Salted Butter", "LLD001")
        });

        var service = new ProductService(repository);

        var deleted = await service.DeleteAsync(1);

        Assert.IsTrue(deleted);
        Assert.AreEqual(0, repository.Snapshot().Count);
        Assert.AreEqual(1, repository.SaveChangesCallCount);
        CollectionAssert.Contains(repository.GetByIdTrackChangesCalls, true);
    }

    [TestMethod]
    public async Task DeleteAsync_ReturnsFalse_WhenProductDoesNotExist()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository);

        var deleted = await service.DeleteAsync(99);

        Assert.IsFalse(deleted);
        Assert.AreEqual(0, repository.SaveChangesCallCount);
        CollectionAssert.Contains(repository.GetByIdTrackChangesCalls, true);
    }

    private static ProductInputModel CreateInputModel()
    {
        return new ProductInputModel
        {
            ProductId = "LLD099",
            ProductName = "Whipped Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "A whipped butter demo entry.",
            PackageSize = "8 oz tub",
            Price = 4.59m,
            ImageFileName = "whipped-butter.jpg",
            Tags = "butter, whipped"
        };
    }

    private static Product CreateProduct(
        int id,
        string productName,
        string productId,
        string category = "Butter & Spreads",
        bool isFeatured = false,
        string? imageFileName = null,
        string? tags = null)
    {
        return new Product
        {
            Id = id,
            ProductId = productId,
            ProductName = productName,
            Category = category,
            Brand = "Land O Lakes",
            ShortDescription = "Demo product",
            PackageSize = "1 lb box",
            Price = 4.99m,
            IsFeatured = isFeatured,
            ImageFileName = imageFileName,
            Tags = tags,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };
    }
}