using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Models.Api;
using LandOLakesDairyDemo.Models.Qa;

namespace LandOLakesDairyDemo.UnitTests.Models;

[TestClass]
public class ModelDefaultsTests
{
    [TestMethod]
    public void Product_DefaultStringsMatchExpectedValues()
    {
        var product = new Product();

        Assert.AreEqual(string.Empty, product.ProductId);
        Assert.AreEqual(string.Empty, product.ProductName);
        Assert.AreEqual(string.Empty, product.Category);
        Assert.AreEqual("Land O Lakes", product.Brand);
        Assert.AreEqual(string.Empty, product.ShortDescription);
        Assert.AreEqual(string.Empty, product.PackageSize);
    }

    [TestMethod]
    public void ProductInputModel_DefaultStringsMatchExpectedValues()
    {
        var inputModel = new ProductInputModel();

        Assert.AreEqual(string.Empty, inputModel.ProductId);
        Assert.AreEqual(string.Empty, inputModel.ProductName);
        Assert.AreEqual(string.Empty, inputModel.Category);
        Assert.AreEqual("Land O Lakes", inputModel.Brand);
        Assert.AreEqual(string.Empty, inputModel.ShortDescription);
        Assert.AreEqual(string.Empty, inputModel.PackageSize);
    }

    [TestMethod]
    public void SeedProductRecord_DefaultStringsMatchExpectedValues()
    {
        var record = new SeedProductRecord();

        Assert.AreEqual(string.Empty, record.ProductId);
        Assert.AreEqual(string.Empty, record.ProductName);
        Assert.AreEqual(string.Empty, record.Category);
        Assert.AreEqual(string.Empty, record.Brand);
        Assert.AreEqual(string.Empty, record.ShortDescription);
        Assert.AreEqual(string.Empty, record.PackageSize);
    }

    [TestMethod]
    public void ApiModels_DefaultValuesMatchExpectedValues()
    {
        var dto = new ProductDto();
        var request = new UpsertProductRequest();

        Assert.AreEqual(string.Empty, dto.ProductId);
        Assert.AreEqual(string.Empty, dto.ProductName);
        Assert.AreEqual(string.Empty, dto.Category);
        Assert.AreEqual(string.Empty, dto.Brand);
        Assert.AreEqual(string.Empty, dto.ShortDescription);
        Assert.AreEqual(string.Empty, dto.PackageSize);
        Assert.IsNotNull(dto.Tags);
        Assert.AreEqual(0, dto.Tags.Length);

        Assert.AreEqual(string.Empty, request.ProductId);
        Assert.AreEqual(string.Empty, request.ProductName);
        Assert.AreEqual(string.Empty, request.Category);
        Assert.AreEqual(string.Empty, request.Brand);
        Assert.AreEqual(string.Empty, request.ShortDescription);
        Assert.AreEqual(string.Empty, request.PackageSize);
        Assert.IsNotNull(request.Tags);
        Assert.AreEqual(0, request.Tags.Length);
    }

    [TestMethod]
    public void ErrorAndQaModels_DefaultValuesMatchExpectedValues()
    {
        var error = new ErrorViewModel();
        var state = new QaRunState();
        var testCaseItem = new QaTestCaseItem();

        Assert.IsFalse(error.ShowRequestId);
        Assert.AreEqual("idle", state.Status);
        Assert.AreEqual(string.Empty, testCaseItem.Id);
        Assert.AreEqual(string.Empty, testCaseItem.Requirement);
        Assert.AreEqual(string.Empty, testCaseItem.Title);
        Assert.AreEqual(string.Empty, testCaseItem.Preconditions);
        Assert.AreEqual(string.Empty, testCaseItem.Steps);
        Assert.AreEqual(string.Empty, testCaseItem.ExpectedResult);
        Assert.AreEqual(string.Empty, testCaseItem.AutomationCoverage);
    }
}