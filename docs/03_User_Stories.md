# LandOLakesDairyDemo User Stories

## Shopper Stories
### US-01 View Catalog
As a shopper, I want to view a catalog of dairy products so that I can quickly see what products are available.

Acceptance Criteria
- The catalog shows seeded products on initial load.
- Each product displays key summary information.

### US-02 Browse by Category
As a shopper, I want to filter products by category so that I can focus on butter, cheese, cream, or half & half.

Acceptance Criteria
- The user can select a category filter.
- The catalog updates to show matching products.

### US-03 Search Products
As a shopper, I want to search products by name or keyword so that I can find a product quickly.

Acceptance Criteria
- The user can enter a search term.
- The catalog returns matching products.

### US-04 View Product Details
As a shopper, I want to view product details so that I can understand the product before selecting it.

Acceptance Criteria
- The details view shows name, category, description, and price.
- The details view can be opened from the catalog.

## Admin Stories
### US-05 Add Product
As an admin, I want to add a new product so that the demo catalog can be expanded.

Acceptance Criteria
- The admin can open an add-product form.
- A saved product appears in the catalog.

### US-06 Edit Product
As an admin, I want to edit an existing product so that I can keep product information current.

Acceptance Criteria
- The admin can update product fields.
- Changes appear in the catalog and details view.

### US-07 Delete Product
As an admin, I want to delete a product so that obsolete items can be removed from the catalog.

Acceptance Criteria
- The admin can remove a selected product.
- The deleted product no longer appears in the catalog.

## API Story
### US-08 Expose Product APIs
As a developer or demo presenter, I want product API endpoints so that I can demonstrate backend capability alongside the UI.

Acceptance Criteria
- An endpoint returns the full product list.
- An endpoint returns product details by ID.
- Endpoints exist for create, update, and delete operations.
- Optional category and search query support can be demonstrated.

## Suggested Priority
- Must Have: US-01, US-02, US-03, US-04, US-05, US-06, US-07, US-08

## Definition of Done
- Story behavior is implemented and demonstrable.
- Seed data supports the story in the live walkthrough.
- The story can be explained in business language within the demo timeframe.