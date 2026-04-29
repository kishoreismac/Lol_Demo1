using LandOLakesDairyDemo.Controllers;
using LandOLakesDairyDemo.Models.Qa;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace LandOLakesDairyDemo.UnitTests.Controllers;

[TestClass]
public class QaControllerTests
{
    [TestMethod]
    public void TestCases_ReturnsExpectedSmokeCatalogAndState()
    {
        var qaService = new QaRunStateService();
        qaService.UpdateState(new QaRunState { CaseId = "TC-UI-003", Title = "Search by product name returns matching items", Status = "running", Notes = "In progress" });
        var controller = new QaController(qaService);

        var result = controller.TestCases();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as QaTestCasesViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("TC-UI-003", model.CurrentState.CaseId);
        Assert.AreEqual(6, model.TestCases.Count);

        CollectionAssert.AreEqual(
            new[]
            {
                "TC-UI-001|FR-01 Home and featured products|Home page loads successfully|Application is running with seeded data.|Open the home page.|Hero content, featured products, and navigation actions are visible.|playwright/tests/smoke.spec.js",
                "TC-UI-002|FR-02 Product catalog browsing|Products page loads successfully|Application is running.|Open the products page.|Catalog filters and product cards are visible.|playwright/tests/smoke.spec.js",
                "TC-UI-003|FR-03 Search products|Search by product name returns matching items|Application is running with seeded data.|Search for 'Butter' on the products page.|The catalog narrows to butter-related products.|playwright/tests/smoke.spec.js",
                "TC-UI-004|FR-04 Filter by category|Category filter narrows the catalog|Application is running with seeded data.|Select the Cheese category on the products page.|Only cheese products remain visible.|playwright/tests/smoke.spec.js",
                "TC-UI-005|FR-05 Product details|Product details page opens from the catalog|Application is running with seeded data.|Open product details from the products page.|The details page displays the selected product name, category, price, and tags.|playwright/tests/smoke.spec.js",
                "TC-UI-006|FR-06 Admin CRUD|Admin create, edit, and delete flow works|Application is running and admin UI is accessible.|Create a temporary product, update it, then delete it from the admin area.|The product is added, updated, and removed successfully with status messages.|playwright/tests/smoke.spec.js"
            },
            model.TestCases.Select(testCase => $"{testCase.Id}|{testCase.Requirement}|{testCase.Title}|{testCase.Preconditions}|{testCase.Steps}|{testCase.ExpectedResult}|{testCase.AutomationCoverage}").ToArray());
    }

    [TestMethod]
    public void GetState_ReturnsOkObjectWithCurrentState()
    {
        var qaService = new QaRunStateService();
        qaService.UpdateState(new QaRunState { CaseId = "TC-UI-005", Title = "Product details page opens from the catalog", Status = "passed", Notes = "Complete" });
        var controller = new QaController(qaService);

        var result = controller.GetState();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var state = okResult.Value as QaRunState;
        Assert.IsNotNull(state);
        Assert.AreEqual("TC-UI-005", state.CaseId);
        Assert.AreEqual("passed", state.Status);
    }

    [TestMethod]
    public void UpdateState_ReturnsUpdatedState()
    {
        var qaService = new QaRunStateService();
        var controller = new QaController(qaService);

        var result = controller.UpdateState(new QaRunState
        {
            CaseId = "TC-UI-006",
            Title = "Admin create, edit, and delete flow works",
            Status = "",
            Notes = "Running from unit test"
        });

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var state = okResult.Value as QaRunState;
        Assert.IsNotNull(state);
        Assert.AreEqual("TC-UI-006", state.CaseId);
        Assert.AreEqual("running", state.Status);
        Assert.AreEqual("Running from unit test", state.Notes);
    }
}