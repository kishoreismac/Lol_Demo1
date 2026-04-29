using LandOLakesDairyDemo.Models.Qa;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace LandOLakesDairyDemo.Controllers;

[Route("qa")]
public class QaController : Controller
{
    private static readonly IReadOnlyList<QaTestCaseItem> SmokeTestCases = new List<QaTestCaseItem>
    {
        new()
        {
            Id = "TC-UI-001",
            Requirement = "FR-01 Home and featured products",
            Title = "Home page loads successfully",
            Preconditions = "Application is running with seeded data.",
            Steps = "Open the home page.",
            ExpectedResult = "Hero content, featured products, and navigation actions are visible.",
            AutomationCoverage = "playwright/tests/smoke.spec.js"
        },
        new()
        {
            Id = "TC-UI-002",
            Requirement = "FR-02 Product catalog browsing",
            Title = "Products page loads successfully",
            Preconditions = "Application is running.",
            Steps = "Open the products page.",
            ExpectedResult = "Catalog filters and product cards are visible.",
            AutomationCoverage = "playwright/tests/smoke.spec.js"
        },
        new()
        {
            Id = "TC-UI-003",
            Requirement = "FR-03 Search products",
            Title = "Search by product name returns matching items",
            Preconditions = "Application is running with seeded data.",
            Steps = "Search for 'Butter' on the products page.",
            ExpectedResult = "The catalog narrows to butter-related products.",
            AutomationCoverage = "playwright/tests/smoke.spec.js"
        },
        new()
        {
            Id = "TC-UI-004",
            Requirement = "FR-04 Filter by category",
            Title = "Category filter narrows the catalog",
            Preconditions = "Application is running with seeded data.",
            Steps = "Select the Cheese category on the products page.",
            ExpectedResult = "Only cheese products remain visible.",
            AutomationCoverage = "playwright/tests/smoke.spec.js"
        },
        new()
        {
            Id = "TC-UI-005",
            Requirement = "FR-05 Product details",
            Title = "Product details page opens from the catalog",
            Preconditions = "Application is running with seeded data.",
            Steps = "Open product details from the products page.",
            ExpectedResult = "The details page displays the selected product name, category, price, and tags.",
            AutomationCoverage = "playwright/tests/smoke.spec.js"
        },
        new()
        {
            Id = "TC-UI-006",
            Requirement = "FR-06 Admin CRUD",
            Title = "Admin create, edit, and delete flow works",
            Preconditions = "Application is running and admin UI is accessible.",
            Steps = "Create a temporary product, update it, then delete it from the admin area.",
            ExpectedResult = "The product is added, updated, and removed successfully with status messages.",
            AutomationCoverage = "playwright/tests/smoke.spec.js"
        }
    };

    private readonly QaRunStateService _qaRunStateService;

    public QaController(QaRunStateService qaRunStateService)
    {
        _qaRunStateService = qaRunStateService;
    }

    [HttpGet("testcases")]
    public IActionResult TestCases()
    {
        var viewModel = new QaTestCasesViewModel
        {
            TestCases = SmokeTestCases,
            CurrentState = _qaRunStateService.GetState()
        };

        return View(viewModel);
    }

    [HttpGet("state")]
    public IActionResult GetState()
    {
        return Ok(_qaRunStateService.GetState());
    }

    [HttpPost("state")]
    public IActionResult UpdateState([FromBody] QaRunState state)
    {
        return Ok(_qaRunStateService.UpdateState(state));
    }
}