using LandOLakesDairyDemo.Controllers;
using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Models.Api;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LandOLakesDairyDemo.UnitTests.Controllers;

[TestClass]
public class ProductsApiControllerTests
{
    [TestMethod]
    public async Task CreateProduct_MapsNullTagsToEmptyString()
    {
        ProductInputModel? capturedInputModel = null;
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.CreateAsync(It.IsAny<ProductInputModel>()))
            .Callback<ProductInputModel>(inputModel => capturedInputModel = inputModel)
            .ReturnsAsync((true, new Dictionary<string, string[]>(), new Product { Id = 1, ProductId = "LLD100", ProductName = "Mapped Butter" }));

        var controller = new ProductsApiController(service.Object);
        var request = new UpsertProductRequest
        {
            ProductId = "LLD100",
            ProductName = "Mapped Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Mapping test",
            PackageSize = "8 oz tub",
            Price = 4.99m,
            Tags = null!
        };

        var result = await controller.CreateProduct(request);

        Assert.IsNotNull(capturedInputModel);
        Assert.AreEqual(string.Empty, capturedInputModel.Tags);
        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
    }

    [TestMethod]
    public async Task GetProduct_ReturnsEmptyTagArray_WhenSourceTagsAreNull()
    {
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.GetByIdAsync(5)).ReturnsAsync(new Product
        {
            Id = 5,
            ProductId = "LLD005",
            ProductName = "Butter",
            Category = "Butter & Spreads",
            Brand = "Land O Lakes",
            ShortDescription = "Null tags test",
            PackageSize = "8 oz tub",
            Price = 4.25m,
            Tags = null
        });

        var controller = new ProductsApiController(service.Object);

        var result = await controller.GetProduct(5);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var dto = okResult.Value as ProductDto;
        Assert.IsNotNull(dto);
        Assert.IsNotNull(dto.Tags);
        Assert.AreEqual(0, dto.Tags.Length);
    }

    [TestMethod]
    public async Task GetProduct_TrimsTagsAndRemovesEmptyEntries()
    {
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.GetByIdAsync(9)).ReturnsAsync(new Product
        {
            Id = 9,
            ProductId = "LLD009",
            ProductName = "Cheese",
            Category = "Cheese",
            Brand = "Land O Lakes",
            ShortDescription = "Split tags test",
            PackageSize = "8 oz pack",
            Price = 5.15m,
            Tags = "cheese,  snack , , protein"
        });

        var controller = new ProductsApiController(service.Object);

        var result = await controller.GetProduct(9);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var dto = okResult.Value as ProductDto;
        Assert.IsNotNull(dto);
        CollectionAssert.AreEqual(new[] { "cheese", "snack", "protein" }, dto.Tags);
    }
}