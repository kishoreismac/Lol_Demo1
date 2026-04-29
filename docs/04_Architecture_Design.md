# LandOLakesDairyDemo Architecture Design

## Solution Overview
LandOLakesDairyDemo is a lightweight ASP.NET Core MVC application built on .NET 8. It provides a public product catalog, a simplified admin experience for product management, and REST-style product APIs. The design favors clarity, fast setup, and easy demo flow over enterprise concerns.

## Technology Stack
- ASP.NET Core MVC on .NET 8
- Razor views for server-rendered pages
- Bootstrap for responsive UI styling and layout
- EF Core with SQLite for local persistence
- Simple repository and service pattern for separation of concerns
- Swagger / OpenAPI for API exploration

## Architectural Style
- Monolithic web application for demo simplicity
- MVC for page rendering and user flows
- API controllers for product endpoints
- One primary domain entity: Product
- Seeded SQLite database for predictable demo behavior

## System Overview
The solution consists of a single web application hosting both the UI and API surface.

Request flow:
1. User accesses Razor pages through MVC controllers.
2. Controllers call application services.
3. Services apply lightweight business and validation rules.
4. Repositories interact with EF Core DbContext.
5. SQLite stores seeded product data.
6. API controllers reuse the same service layer and expose JSON endpoints.

## Page Map
- Home / Catalog
  Purpose: show featured products, search, and category filter
- Products / Index
  Purpose: browse full catalog with filters
- Products / Details / {id}
  Purpose: view product details
- Admin / Products
  Purpose: list products for maintenance
- Admin / Products / Create
  Purpose: add a product
- Admin / Products / Edit / {id}
  Purpose: update a product
- Admin / Products / Delete / {id}
  Purpose: confirm and remove a product
- Swagger
  Purpose: browse and test product APIs during the demo

## Component Diagram Description
### Presentation Layer
- MVC Controllers
  Handle catalog and admin page requests.
- Razor Views
  Render Bootstrap-based UI for catalog, details, and admin forms.
- API Controllers
  Expose JSON endpoints for product CRUD and search.

### Application Layer
- ProductService
  Central place for product retrieval, search, filtering, validation, and CRUD orchestration.

### Data Access Layer
- IProductRepository
  Defines CRUD and query operations.
- ProductRepository
  Implements repository behavior using EF Core.
- AppDbContext
  Maps the Product entity and seeds initial data.

### Data Store
- SQLite database
  Stores demo products locally in a file-based database.

## Proposed Project Structure
```text
LandOLakesDairyDemo/
  Controllers/
    HomeController.cs
    ProductsController.cs
    AdminProductsController.cs
  Api/
    ProductsApiController.cs
  Models/
    Product.cs
    ViewModels/
      ProductListViewModel.cs
      ProductFormViewModel.cs
  Data/
    AppDbContext.cs
    SeedData.cs
  Repositories/
    IProductRepository.cs
    ProductRepository.cs
  Services/
    IProductService.cs
    ProductService.cs
  Views/
    Home/
    Products/
    AdminProducts/
  wwwroot/
    images/products/
```

## CRUD Flow
### Read
- User opens catalog page.
- Controller requests filtered products from ProductService.
- Service queries repository using category, search term, and featured flags.
- View renders product cards and filters.

### Create
- Admin opens create form.
- Form posts to controller.
- Controller validates model state and calls ProductService.
- Service enforces business rules and saves through repository.
- User is redirected to admin list or catalog.

### Update
- Admin opens edit form for a product.
- Existing data is loaded from service.
- Edited form posts updated values.
- Service validates and persists changes.

### Delete
- Admin opens delete confirmation.
- Controller loads selected product.
- Confirmed delete calls service and repository.
- User is redirected with a success message.

## UX Notes
- Use a clean Bootstrap layout with visible search and category filtering at the top of the catalog page.
- Present products as cards with image, name, category, short description, and price.
- Keep admin forms short and scannable with inline validation messages.
- Use consistent action buttons for add, edit, delete, and details.
- Favor fast navigation over complex modal interactions.

## Non-Functional Requirements For The Demo
- Startup should be simple enough for local execution on a developer laptop.
- Seed data should be available on first run without manual entry.
- Page loads should feel immediate for the small dataset.
- The UI should remain usable on common laptop and tablet widths.
- Error handling should be friendly and sufficient for live demo recovery.
- Logging should be minimal but adequate for debugging local issues.

## Design Decisions
- SQLite is used to avoid external infrastructure.
- Repository and service layers are kept intentionally thin.
- Authentication is omitted or simulated to preserve demo pace.
- Tags are stored simply for search and display support rather than normalized taxonomy management.

## Implementation Notes
- Seed the database at startup if no products exist.
- Enable Swagger in development mode.
- Use data annotations on view models and entity classes for shared validation.
- Keep API DTOs close to the Product model unless later separation is needed.