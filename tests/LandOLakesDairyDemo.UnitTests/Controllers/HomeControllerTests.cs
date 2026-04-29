using System.Diagnostics;
using LandOLakesDairyDemo.Controllers;
using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LandOLakesDairyDemo.UnitTests.Controllers;

[TestClass]
public class HomeControllerTests
{
    [TestMethod]
    public async Task Index_ReturnsFeaturedProductsAndCategories()
    {
        var service = new Mock<IProductService>();
        service.Setup(mock => mock.GetFeaturedProductsAsync(6)).ReturnsAsync(new[]
        {
            new Product { Id = 1, ProductId = "LLD001", ProductName = "Salted Butter" }
        });
        service.Setup(mock => mock.GetCategoriesAsync()).ReturnsAsync(new[] { "Butter & Spreads", "Cheese" });

        var controller = new HomeController(service.Object);

        var result = await controller.Index();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as HomeViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.FeaturedProducts.Count);
        CollectionAssert.AreEqual(new[] { "Butter & Spreads", "Cheese" }, model.Categories.ToArray());
    }

    [TestMethod]
    public void Error_ReturnsTraceIdentifier_WhenActivityIsMissing()
    {
        var service = new Mock<IProductService>();
        var controller = new HomeController(service.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { TraceIdentifier = "trace-123" }
            }
        };

        var result = controller.Error();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ErrorViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("trace-123", model.RequestId);
        Assert.IsTrue(model.ShowRequestId);
        Assert.IsNull(Activity.Current);
    }
}