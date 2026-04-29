# LandOLakesDairyDemo Test Plan

## Purpose
This test plan defines a lightweight, demo-ready quality strategy for LandOLakesDairyDemo. The goal is to prove the application is stable enough for a 15-minute showcase without introducing enterprise-scale testing overhead.

## Test Objectives
- Confirm the seeded catalog loads and displays correctly.
- Verify product search, category filtering, and product details flows.
- Validate admin create, edit, and delete behavior.
- Verify the product API supports basic CRUD and retrieval scenarios.
- Provide traceability from demo requirements to automated tests.

## In Scope
- MVC controller behavior
- Product service validation and CRUD orchestration
- Product API endpoints
- Browser smoke coverage for key user journeys
- Traceability documentation

## Out Of Scope
- Load testing
- Security penetration testing
- Cross-browser compatibility matrix beyond Chromium smoke runs
- Accessibility certification beyond basic visual checks
- Performance benchmarking beyond simple local responsiveness

## Test Levels
### Unit Tests
- Product service validation and normalization
- MVC controller action results and redirect behavior

### Integration Tests
- Product API list, details, create, update, and delete endpoints using an isolated test host

### End-To-End Smoke Tests
- Home page
- Products page
- Search
- Category filter
- Product details
- Admin create, edit, and delete flow

## Entry Criteria
- Application builds successfully.
- Seed catalog JSON exists and is valid.
- Local dependencies for .NET and Node.js are installed.

## Exit Criteria
- Unit tests pass.
- Integration tests pass.
- Playwright smoke tests pass in headed mode.
- Live QA testcase board highlights the active smoke test during execution.

## Environment
- ASP.NET Core MVC app running locally on Windows
- .NET 8 SDK
- SQLite local database
- MSTest for unit and integration suites
- Playwright with Chromium for smoke coverage

## Requirement Traceability Matrix
| Requirement ID | Requirement Summary | Automated Coverage |
| --- | --- | --- |
| FR-01 | Home page shows featured content and categories | `Home page loads successfully` Playwright smoke test |
| FR-02 | Product catalog page loads | `ProductsControllerTests.Index_ReturnsCatalogViewModel`; `GetProducts_ReturnsSeededCatalog`; `Products page loads successfully` |
| FR-03 | Search by product name | `ProductServiceTests.GetProductsAsync_TrimsSearchTerm`; `Search by product name returns matching items` |
| FR-04 | Filter by category | `GetProducts_FilteredByCategory_ReturnsOnlyRequestedCategory`; `Category filter narrows the catalog` |
| FR-05 | Product details page opens | `ProductsControllerTests.Details_ReturnsNotFound_WhenProductMissing`; `GetProduct_ReturnsSingleProduct`; `Product details page opens from the catalog` |
| FR-06 | Admin create works | `ProductServiceTests.CreateAsync_SavesNormalizedProduct`; `AdminProductsControllerTests.Create_RedirectsOnSuccess`; `Admin create, edit, and delete flow works` |
| FR-07 | Admin edit works | `ProductServiceTests.UpdateAsync_ReturnsNotFound_WhenMissing`; `AdminProductsControllerTests.Edit_ReturnsNotFound_WhenProductMissing`; `Admin create, edit, and delete flow works` |
| FR-08 | Admin delete works | `ProductServiceTests.DeleteAsync_RemovesExistingProduct`; `AdminProductsControllerTests.DeleteConfirmed_RedirectsOnSuccess`; `Admin create, edit, and delete flow works` |
| FR-09 | Product API endpoints work | `GetProducts_ReturnsSeededCatalog`; `GetProduct_ReturnsSingleProduct`; `PostPutDelete_ProductLifecycle_Works` |

## Risks And Mitigations
- Risk: Demo data is changed during smoke tests.
  Mitigation: The Playwright admin CRUD test creates and removes a temporary record in one flow.
- Risk: Root project includes test files by default.
  Mitigation: The web project explicitly excludes `tests/` and `playwright/` folders from SDK file globs.
- Risk: Live demo execution drifts from documented test cases.
  Mitigation: Smoke tests publish their active case to the live QA board at `/qa/testcases`.

## Test Deliverables
- This test plan
- Test case document
- Unit tests
- Integration tests
- Playwright smoke tests
- Live testcase monitor page