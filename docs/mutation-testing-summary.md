# Mutation Testing Summary

## Executive Summary

Mutation testing completed successfully for the LandOLakesDairyDemo solution using the existing unit and integration test suites. The pre-mutation baseline was stable: all 16 automated .NET tests passed before Stryker ran.

The overall mutation score was **32.51%**, which indicates the current test suite catches core CRUD and repository behavior but leaves significant gaps in startup wiring, QA-only surfaces, and several mapping/default-value paths. The most important practical finding is that the application logic is partially defended, but many mutants survive because tests do not assert enough on mapped output values, default property values, ordering, or alternate control-flow branches.

The lowest-value survivors are concentrated in DTO/model default-property mutations and the QA dashboard metadata. The highest-risk survivors are in application startup, API mapping, repository ordering/tracking behavior, and product normalization/validation edge cases.

## Tools and Configuration Used

- Runtime and app stack: .NET 8, ASP.NET Core MVC, EF Core SQLite
- Test frameworks executed before mutation: MSTest unit tests and MSTest integration tests
- Mutation tool: **Stryker.NET 4.14.1** from the local tool manifest
- Source project under mutation: `LandOLakesDairyDemo.csproj`
- Attached test projects:
  - `tests/LandOLakesDairyDemo.UnitTests/LandOLakesDairyDemo.UnitTests.csproj`
  - `tests/LandOLakesDairyDemo.IntegrationTests/LandOLakesDairyDemo.IntegrationTests.csproj`
- Report outputs:
  - `.stryker-out/reports/mutation-report.json`
  - `.stryker-out/reports/mutation-report.html`

## Test Execution Status Before Mutation Testing

Baseline validation was completed before mutation analysis:

- Unit tests: **12 passed**
- Integration tests: **4 passed**
- Total automated .NET tests executed: **16 passed, 0 failed**

This confirms the mutation results reflect test effectiveness rather than an unstable baseline.

## Overall Mutation Score

Stryker created **347 mutants** in total.

Global status breakdown:

| Status | Count |
| --- | ---: |
| Killed | 92 |
| Survived | 119 |
| No Coverage | 72 |
| Compile Error | 5 |
| Ignored | 59 |
| Timeout | 0 |

Additional run-level observations:

- Total tested mutants: **211**
- Total skipped mutants: **136**
- Final mutation score: **32.51%**
- Elapsed runtime: about **4 minutes 49 seconds**

Interpretation:

- The absence of timeout mutants is good; the suite is stable and reasonably fast under mutation.
- The 72 no-coverage mutants are a major contributor to the low score.
- The 119 survived mutants show that some code is executed but not asserted deeply enough.

## Results by File and Module

### Files With Tested Mutants

| File | Tested | Killed | Survived | Score |
| --- | ---: | ---: | ---: | ---: |
| `Controllers/ProductsController.cs` | 4 | 4 | 0 | 100.00% |
| `Controllers/AdminProductsController.cs` | 12 | 11 | 1 | 91.67% |
| `Repositories/ProductRepository.cs` | 21 | 18 | 3 | 85.71% |
| `Services/ProductService.cs` | 45 | 32 | 13 | 71.11% |
| `Controllers/ProductsApiController.cs` | 17 | 11 | 6 | 64.71% |
| `Data/DbInitializer.cs` | 13 | 9 | 4 | 69.23% |
| `Program.cs` | 19 | 7 | 12 | 36.84% |
| `Controllers/QaController.cs` | 43 | 0 | 43 | 0.00% |
| `Data/AppDbContext.cs` | 7 | 0 | 7 | 0.00% |
| `Models/Product.cs` | 6 | 0 | 6 | 0.00% |
| `Models/ProductInputModel.cs` | 6 | 0 | 6 | 0.00% |
| `Models/SeedProductRecord.cs` | 6 | 0 | 6 | 0.00% |
| `Models/Api/ProductDto.cs` | 6 | 0 | 6 | 0.00% |
| `Models/Api/UpsertProductRequest.cs` | 6 | 0 | 6 | 0.00% |

### Files With No Directly Tested Mutants but Meaningful No-Coverage Gaps

| File | No Coverage Mutants | Notes |
| --- | ---: | --- |
| `Controllers/AdminProductsController.cs` | 16 | Create/Edit/Delete unhappy paths and helper logic are only partially exercised |
| `Services/QaRunStateService.cs` | 10 | No automated coverage for QA state transitions |
| `Controllers/ProductsApiController.cs` | 9 | Invalid model state and some error response branches untested |
| `Services/ProductService.cs` | 9 | Featured-product path, edit mapping, and delete success/failure branches incomplete |
| `Controllers/HomeController.cs` | 4 | Home page controller behavior is untested |
| `Controllers/QaController.cs` | 4 | QA endpoints not covered by tests |
| `Program.cs` | 4 | Non-development branch and connection-string edge cases not covered |
| `Repositories/ProductRepository.cs` | 3 | Search/filter/order branches still have gaps |
| `Data/DbInitializer.cs` | 2 | Early-return branches for existing data / missing seed file untested |

## Survived Mutant Analysis

### 1. Application Startup and Hosting Wiring

`Program.cs` produced **12 survived mutants** and a low score of **36.84%**. The survivors are concentrated around startup registrations and middleware branch behavior.

Representative survivors:

- Removal of `AddEndpointsApiExplorer()` setup
- Removal of the SQLite registration block
- Removal of the QA singleton registration
- Negation of `app.Environment.IsDevelopment()`
- Removal of middleware calls such as routing/authorization/static files

What this means:

- The integration tests prove the API works, but they do not assert that Swagger services are registered, that the QA service is present, or that the development/non-development pipeline branches behave as intended.

Risk:

- Medium to High. Startup regressions can silently disable features or alter middleware behavior without breaking the current test suite.

### 2. API Mapping and Response Shape Assertions Are Too Shallow

`Controllers/ProductsApiController.cs` produced **6 survived mutants**. The strongest signal is around tag mapping and request-to-input transformation.

Representative survivors:

- `string.Join(", ", request.Tags ?? Array.Empty<string>())` mutations at line 115 all survived
- `(product.Tags ?? string.Empty)` mutation at line 133 survived
- `StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries` mutated at line 134 survived

What this means:

- The integration tests validate status codes and a few values, but they do not verify exact `Tags` mapping semantics for null, empty, whitespace-trimmed, or multi-tag scenarios.

Risk:

- Medium. The API can keep returning successful responses while subtly corrupting tag normalization or serialization behavior.

### 3. Repository Semantics Need Deeper Behavioral Assertions

`Repositories/ProductRepository.cs` performed fairly well overall, but **3 important mutants survived**:

- Search-term whitespace handling at line 20
- Ordering mutation from `OrderBy()` to `OrderByDescending()` at line 35
- Negated `trackChanges` branch in `GetByIdAsync()` at line 51

What this means:

- Current tests prove filtering and retrieval work, but they do not assert stable ascending ordering or the observable difference between tracked and non-tracked entity retrieval.

Risk:

- Medium. These are real behavior regressions that can affect UI ordering and update flows.

### 4. Product Service Logic Is Covered, But Not Deeply Enough

`Services/ProductService.cs` had the largest volume of business-logic mutants: **32 killed**, **13 survived**, **9 no coverage**.

Most important survivors:

- Allowed category constants mutated at lines 11-12 survived because tests only validate one unsupported category and not the full allowed set
- `trackChanges: true` boolean mutation in `UpdateAsync` and `DeleteAsync` survived
- The `"Product not found."` message mutation survived
- `NormalizeOptional()` mutations at line 161 survived
- Validation message mutations for category uniqueness at lines 148 and 153 survived

What this means:

- The service tests are good enough to catch major success/failure paths, but they do not assert enough on the exact normalization result, exact error payload/message content, or tracking-sensitive repository calls.

Risk:

- High. These mutants live inside core domain logic and could alter user-visible validation or persistence behavior.

### 5. Data Initialization Coverage Misses Fallback and Serialization Options

`Data/DbInitializer.cs` had **4 survived mutants**:

- Removal of the null-coalescing fallback after JSON deserialization
- Removal of `PropertyNameCaseInsensitive = true`
- String join mutation for seed tags

What this means:

- Current integration tests validate that seeding works in the happy path, but they do not validate deserialization robustness or exact tag composition.

Risk:

- Medium. Startup data may still load in the current environment while becoming brittle to seed format variations.

### 6. QA Board and Metadata Surfaces Are Effectively Untested

`Controllers/QaController.cs` had **43 survived mutants** and `Services/QaRunStateService.cs` had **10 no-coverage mutants**.

What this means:

- The Playwright tests use the QA board for visualization, but the .NET mutation run does not execute those browser tests, so the QA-specific MVC/controller/service paths are almost entirely unprotected.

Risk:

- Low for core catalog behavior, Medium for demo reliability because the live QA board is part of the showcase experience.

### 7. DTO and Model Default Values Are Not Asserted Directly

Several model files show **0% tested-mutant score** with survived string-default mutations:

- `Models/Product.cs`
- `Models/ProductInputModel.cs`
- `Models/SeedProductRecord.cs`
- `Models/Api/ProductDto.cs`
- `Models/Api/UpsertProductRequest.cs`

What this means:

- Tests instantiate or deserialize these types, but they do not verify default values or data annotation-driven semantics directly.

Risk:

- Low to Medium. Many of these are low-value mutants, but some can still affect model binding defaults and API contracts.

## Missing or Weak Test Coverage Areas

1. Admin controller unhappy paths are incomplete.
The no-coverage mutations in `AdminProductsController` show missing tests for create/edit failures, `id`-not-found branches during edit, delete-not-found, and service-error-to-model-state propagation.

2. Home controller behavior is untested.
There are no unit tests proving `HomeController.Index()` builds the expected view model or `Error()` handles request IDs correctly.

3. QA dashboard endpoints are not covered by .NET tests.
`QaController` and `QaRunStateService` need focused unit/integration tests if the QA board is intended to remain part of the demo.

4. API validation and error responses are only partially checked.
No tests currently assert `ValidationProblem`, duplicate-ID bad requests, update-not-found responses, or exact tag mapping in JSON output.

5. Repository ordering and tracked retrieval semantics are weakly tested.
The surviving order and `trackChanges` mutants show the suite does not verify those repository guarantees.

6. Service normalization and exact validation message behavior are not asserted enough.
Whitespace-only optional fields, `null` optional fields, tag normalization edge cases, and exact validation message content are under-tested.

7. Startup and environment-specific pipeline behavior are under-tested.
`Program.cs` survivors show the suite lacks explicit host-level tests for service registration, Swagger setup, and non-development middleware configuration.

## Recommended New or Improved Test Cases

### High Priority

1. Add API integration tests that create and retrieve products with:
   - `Tags = null`
   - `Tags = []`
   - tags containing whitespace and duplicates
   - assertions on exact serialized/deserialized `Tags`

2. Add unit tests for `ProductService.NormalizeOptional()` behavior indirectly through `CreateAsync` and `UpdateAsync` by asserting:
   - whitespace-only `ImageFileName` becomes `null`
   - trimmed optional values are preserved
   - updated products keep normalized values

3. Add `ProductService.UpdateAsync` and `DeleteAsync` tests for existing products, not only missing-product cases, and assert the tracked entity path is required for persistence.

4. Add repository tests that assert returned product ordering is ascending by `ProductName` and that `GetByIdAsync(trackChanges: true)` behaves differently from non-tracked retrieval in an EF-backed test.

5. Add admin controller tests for:
   - create service failure returns the same view with model errors
   - edit service failure returns `NotFound` when `id` error exists
   - edit validation failure returns the view model
   - delete not found returns `NotFound`

### Medium Priority

1. Add integration tests for API invalid model state and duplicate ID scenarios, asserting exact HTTP status and error payload shape.

2. Add unit tests for `HomeController.Index()` and `HomeController.Error()`.

3. Add tests around `DbInitializer.InitializeAsync()` for:
   - existing products already present
   - missing `seed-catalog.json`
   - case-insensitive seed property deserialization

4. Add targeted host/integration tests that assert key startup wiring exists, especially Swagger services, `QaRunStateService`, and middleware branch behavior.

### Low Priority

1. Add QA controller/service tests if the visual QA board is considered production demo scope.

2. Add DTO/model-focused tests only where defaults materially affect behavior or API contracts.

3. Consider excluding low-value DTO-only files from mutation scope if the team wants the mutation score to reflect business logic rather than passive property containers.

## Priority Action Plan

### High

- Strengthen `ProductService` tests around update success, delete success/failure, normalization, and exact validation output.
- Strengthen `ProductsApiController` integration tests around tag mapping and error responses.
- Add missing unhappy-path coverage for `AdminProductsController`.
- Add repository tests for ordering and tracked retrieval semantics.

### Medium

- Add focused `DbInitializer` tests for fallback branches and serialization options.
- Add startup/pipeline verification around `Program.cs`.
- Add `HomeController` coverage to close easy no-coverage gaps.

### Low

- Add QA board tests if that feature remains part of the formal demo scope.
- Decide whether DTO/default-value mutants should be tested directly or excluded from mutation scope to improve signal quality.

## Bottom Line

The current test suite is effective against the main catalog CRUD flow, but not yet strong enough to defend against a broad range of behavior-preserving mutations. The fastest path to a materially better mutation score is to improve assertions around API mapping, service normalization, repository ordering/tracking behavior, and controller unhappy paths. Those changes should raise both real defect detection capability and the measured score without requiring production-code changes.