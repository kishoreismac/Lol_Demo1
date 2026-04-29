# LandOLakesDairyDemo Smoke Test Cases

The following test cases define the demo-scope UI smoke coverage for LandOLakesDairyDemo. Every test case in this document is automated in Playwright and is shown on the live QA board at `/qa/testcases` during execution.

| Test Case ID | Requirement | Title | Preconditions | Steps | Expected Result |
| --- | --- | --- | --- | --- | --- |
| TC-UI-001 | FR-01 | Home page loads successfully | App is running with seeded data | Open `/` | Hero content, featured product cards, and navigation actions are visible |
| TC-UI-002 | FR-02 | Products page loads successfully | App is running with seeded data | Open `/Products` | Product cards, search box, and category dropdown are visible |
| TC-UI-003 | FR-03 | Search by product name returns matching items | App is running with seeded data | Search for `Butter` on `/Products` | Only butter-related products remain visible |
| TC-UI-004 | FR-04 | Category filter narrows the catalog | App is running with seeded data | Filter `/Products` by `Cheese` | Only cheese products remain visible |
| TC-UI-005 | FR-05 | Product details page opens from the catalog | App is running with seeded data | Open a product details page from the catalog | Product name, category, price, and tags are shown |
| TC-UI-006 | FR-06, FR-07, FR-08 | Admin create, edit, and delete flow works | App is running and admin UI is accessible | Create a temporary product, edit it, then delete it | Status messages are shown and the temporary product is removed |

## Automation Mapping
- `playwright/tests/smoke.spec.js` covers `TC-UI-001` through `TC-UI-006`.