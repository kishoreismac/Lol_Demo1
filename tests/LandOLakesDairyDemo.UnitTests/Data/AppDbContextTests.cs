using LandOLakesDairyDemo.Data;
using LandOLakesDairyDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace LandOLakesDairyDemo.UnitTests.Data;

[TestClass]
public class AppDbContextTests
{
    [TestMethod]
    public void ProductModel_ConfiguresExpectedIndexesAndPrecision()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new AppDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(Product));

        Assert.IsNotNull(entityType);

        var indexes = entityType.GetIndexes().ToList();
        Assert.IsTrue(indexes.Any(index => index.IsUnique && index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(Product.ProductId) })));
        Assert.IsTrue(indexes.Any(index => index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(Product.Category) })));
        Assert.IsTrue(indexes.Any(index => index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(Product.ProductName) })));
        Assert.IsTrue(indexes.Any(index => index.Properties.Select(property => property.Name).SequenceEqual(new[] { nameof(Product.IsFeatured) })));

        var priceProperty = entityType.FindProperty(nameof(Product.Price));
        Assert.IsNotNull(priceProperty);
        Assert.AreEqual(10, priceProperty.GetPrecision());
        Assert.AreEqual(2, priceProperty.GetScale());
    }
}