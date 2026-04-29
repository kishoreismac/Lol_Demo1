using LandOLakesDairyDemo.Controllers;
using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace LandOLakesDairyDemo.UnitTests.Controllers;

[TestClass]
public class AdminProductsControllerTests
{
    [TestMethod]
    public async Task Create_ReturnsView_WhenModelStateInvalid()
    {
        var service = CreateServiceMock();
        var controller = CreateController(service.Object);
        controller.ModelState.AddModelError("ProductName", "Required");

        var result = await controller.Create(new ProductInputModel());

        Assert.IsInstanceOfType<ViewResult>(result);
    }

    [TestMethod]
    public async Task Create_RedirectsOnSuccess()
    {
        var service = CreateServiceMock();
        service.Setup(mock => mock.CreateAsync(It.IsAny<ProductInputModel>()))
            .ReturnsAsync((true, new Dictionary<string, string[]>(), new Product { ProductName = "Whipped Butter" }));

        var controller = CreateController(service.Object);

        var result = await controller.Create(CreateInputModel());

        Assert.IsInstanceOfType<RedirectToActionResult>(result);
        Assert.AreEqual("Whipped Butter was added to the catalog.", controller.TempData["StatusMessage"]);
    }

    [TestMethod]
    public async Task Edit_ReturnsNotFound_WhenProductMissing()
    {
        var service = CreateServiceMock();
        service.Setup(mock => mock.GetInputModelAsync(22)).ReturnsAsync((ProductInputModel?)null);

        var controller = CreateController(service.Object);

        var result = await controller.Edit(22);

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task DeleteConfirmed_RedirectsOnSuccess()
    {
        var service = CreateServiceMock();
        service.Setup(mock => mock.DeleteAsync(9)).ReturnsAsync(true);

        var controller = CreateController(service.Object);

        var result = await controller.DeleteConfirmed(9);

        Assert.IsInstanceOfType<RedirectToActionResult>(result);
        Assert.AreEqual("The product was removed from the catalog.", controller.TempData["StatusMessage"]);
    }

    private static Mock<IProductService> CreateServiceMock()
    {
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.GetCategoriesAsync()).ReturnsAsync(new[]
        {
            "Butter & Spreads",
            "Cheese",
            "Whipping Cream & Half & Half"
        });

        return service;
    }

    private static AdminProductsController CreateController(IProductService productService)
    {
        var controller = new AdminProductsController(productService)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };

        return controller;
    }

    private static ProductInputModel CreateInputModel()
    {
        return new ProductInputModel
        {
            ProductId = "LLD099",
            ProductName = "Whipped Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Demo product",
            PackageSize = "8 oz tub",
            Price = 4.59m
        };
    }
}