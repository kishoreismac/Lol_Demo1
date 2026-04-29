using LandOLakesDairyDemo.Data;
using LandOLakesDairyDemo.Repositories;
using LandOLakesDairyDemo.IntegrationTests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LandOLakesDairyDemo.IntegrationTests.Api;

[TestClass]
public class ProductRepositoryTests
{
    [TestMethod]
    public async Task GetProductsAsync_ReturnsProductsInAscendingOrder_AndIgnoresWhitespaceSearch()
    {
        using var factory = new TestWebApplicationFactory();
        using var scope = factory.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        var products = await repository.GetProductsAsync("   ", null);

        Assert.IsTrue(products.Count > 1);
        CollectionAssert.AreEqual(
            products.Select(product => product.ProductName).OrderBy(name => name).ToList(),
            products.Select(product => product.ProductName).ToList());
    }

    [TestMethod]
    public async Task GetByIdAsync_WithTrackChangesTrue_PersistsEntityChanges()
    {
        using var factory = new TestWebApplicationFactory();
        using var scope = factory.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var targetId = dbContext.Products.Select(product => product.Id).First();

        var trackedProduct = await repository.GetByIdAsync(targetId, trackChanges: true);
        Assert.IsNotNull(trackedProduct);

        trackedProduct.ProductName = "Tracked Product Update";
        await repository.SaveChangesAsync();

        var persistedProduct = await dbContext.Products.AsNoTracking().FirstAsync(product => product.Id == targetId);

        Assert.AreEqual("Tracked Product Update", persistedProduct.ProductName);
    }
}