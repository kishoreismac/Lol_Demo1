using LandOLakesDairyDemo.Controllers;
using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LandOLakesDairyDemo.UnitTests.Controllers;

[TestClass]
public class ProductsControllerTests
{
    [TestMethod]
    public async Task Index_ReturnsCatalogViewModel()
    {
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.GetCategoriesAsync()).ReturnsAsync(new[] { "Butter & Spreads", "Cheese" });
        service.Setup(mock => mock.GetProductsAsync("Butter", "Butter & Spreads", false)).ReturnsAsync(new[]
        {
            new Product { Id = 1, ProductName = "Salted Butter", ProductId = "LLD001", Category = "Butter & Spreads" }
        });

        var controller = new ProductsController(service.Object);

        var result = await controller.Index("Butter", "Butter & Spreads");

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ProductCatalogViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("Butter", model.SearchTerm);
        Assert.AreEqual(1, model.Products.Count);
    }

    [TestMethod]
    public async Task Details_ReturnsNotFound_WhenProductMissing()
    {
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.GetByIdAsync(55)).ReturnsAsync((Product?)null);

        var controller = new ProductsController(service.Object);

        var result = await controller.Details(55);

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
}