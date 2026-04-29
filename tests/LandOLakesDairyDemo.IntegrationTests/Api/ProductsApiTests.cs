using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LandOLakesDairyDemo.Models.Api;
using LandOLakesDairyDemo.IntegrationTests.Support;

namespace LandOLakesDairyDemo.IntegrationTests.Api;

[TestClass]
public class ProductsApiTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [TestMethod]
    public async Task GetProducts_ReturnsSeededCatalog()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products", JsonOptions);

        Assert.IsNotNull(products);
        Assert.AreEqual(15, products.Count);
        CollectionAssert.AreEqual(products.Select(product => product.ProductName).OrderBy(name => name).ToList(), products.Select(product => product.ProductName).ToList());
    }

    [TestMethod]
    public async Task GetProducts_FilteredByCategory_ReturnsOnlyRequestedCategory()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products?category=Cheese", JsonOptions);

        Assert.IsNotNull(products);
        Assert.IsTrue(products.Count > 0);
        Assert.IsTrue(products.All(product => product.Category == "Cheese"));
    }

    [TestMethod]
    public async Task GetProduct_ReturnsSingleProduct()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products", JsonOptions);
        var targetId = products![0].Id;

        var response = await client.GetAsync($"/api/products/{targetId}");
        var product = await response.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(product);
        Assert.IsFalse(string.IsNullOrWhiteSpace(product.ProductId));
        Assert.IsNotNull(product.Tags);
    }

    [TestMethod]
    public async Task GetProduct_ReturnsNotFound_WhenProductMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products/999999");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task PostPutDelete_ProductLifecycle_Works()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new UpsertProductRequest
        {
            ProductId = "LLD099",
            ProductName = "Test Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Created by integration tests.",
            PackageSize = "8 oz tub",
            Price = 4.79m,
            ImageFileName = "test-butter.jpg",
            Tags = new[] { "butter", "test" }
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.IsNotNull(createdProduct);

        createRequest.ProductName = "Test Butter Updated";
        createRequest.Price = 4.99m;

        var updateResponse = await client.PutAsJsonAsync($"/api/products/{createdProduct.Id}", createRequest);
        var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.AreEqual("Test Butter Updated", updatedProduct!.ProductName);

        var deleteResponse = await client.DeleteAsync($"/api/products/{createdProduct.Id}");

        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await client.GetAsync($"/api/products/{createdProduct.Id}");
        Assert.AreEqual(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    [TestMethod]
    public async Task PostProduct_MapsAndReturnsNormalizedTags()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new UpsertProductRequest
        {
            ProductId = "LLD120",
            ProductName = "Mapped Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Created to verify tag mapping.",
            PackageSize = "8 oz tub",
            Price = 4.49m,
            Tags = new[] { " butter ", "demo", "butter" }
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.IsNotNull(createdProduct);
        CollectionAssert.AreEqual(new[] { "butter", "demo" }, createdProduct.Tags);

        var persistedProduct = await client.GetFromJsonAsync<ProductDto>($"/api/products/{createdProduct.Id}", JsonOptions);

        Assert.IsNotNull(persistedProduct);
        CollectionAssert.AreEqual(new[] { "butter", "demo" }, persistedProduct.Tags);
    }

    [TestMethod]
    public async Task PostProduct_WithEmptyTags_ReturnsEmptyTagArray()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new UpsertProductRequest
        {
            ProductId = "LLD121",
            ProductName = "No Tags Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Created to verify null tag handling.",
            PackageSize = "8 oz tub",
            Price = 4.59m,
            Tags = Array.Empty<string>()
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", createRequest);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(JsonOptions);

        Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.IsNotNull(createdProduct);
        Assert.IsNotNull(createdProduct.Tags);
        Assert.AreEqual(0, createdProduct.Tags.Length);
    }

    [TestMethod]
    public async Task PostProduct_ReturnsBadRequest_ForDuplicateProductId()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createRequest = new UpsertProductRequest
        {
            ProductId = "LLD001",
            ProductName = "Duplicate Product",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Created to verify duplicate handling.",
            PackageSize = "8 oz tub",
            Price = 4.59m,
            Tags = new[] { "duplicate" }
        };

        var response = await client.PostAsJsonAsync("/api/products", createRequest);
        var error = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("Validation failed.", error.GetProperty("message").GetString());
        Assert.IsTrue(error.GetProperty("errors").TryGetProperty("ProductId", out var productIdErrors));
        Assert.AreEqual("Product ID must be unique.", productIdErrors[0].GetString());
    }

    [TestMethod]
    public async Task PutProduct_ReturnsNotFound_WhenProductMissing()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var updateRequest = new UpsertProductRequest
        {
            ProductId = "LLD122",
            ProductName = "Missing Product",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Created to verify not found handling.",
            PackageSize = "8 oz tub",
            Price = 4.29m,
            Tags = new[] { "missing" }
        };

        var response = await client.PutAsJsonAsync("/api/products/999999", updateRequest);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}