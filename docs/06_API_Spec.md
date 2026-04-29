# LandOLakesDairyDemo API Specification

## API Overview
The API exposes lightweight product endpoints for the demo application. It is designed for Swagger visibility, simple frontend integration, and straightforward CRUD demonstrations.

## Base Path
`/api/products`

## Content Type
- Request: `application/json`
- Response: `application/json`

## Resource Model
```json
{
  "id": 1,
  "productId": "LLD001",
  "productName": "Salted Butter",
  "category": "Butter & Spreads",
  "brand": "Land O Lakes",
  "shortDescription": "Classic salted butter for everyday cooking and baking.",
  "packageSize": "1 lb box",
  "price": 5.49,
  "isFeatured": true,
  "imageFileName": "salted-butter.jpg",
  "tags": ["butter", "salted", "baking", "cooking"]
}
```

## Endpoints
### GET /api/products
Returns all products with optional filtering.

Query parameters:
- `category` optional string
- `search` optional string
- `featuredOnly` optional boolean

Example:
`GET /api/products?category=Cheese&search=american`

Response:
- `200 OK` with array of products

### GET /api/products/{id}
Returns a single product by internal numeric ID.

Response:
- `200 OK` with product object
- `404 Not Found` if the product does not exist

### POST /api/products
Creates a new product.

Request body:
```json
{
  "productId": "LLD016",
  "productName": "Whipped Butter",
  "category": "Butter & Spreads",
  "brand": "Land O Lakes",
  "shortDescription": "Light and airy whipped butter spread.",
  "packageSize": "8 oz tub",
  "price": 4.59,
  "isFeatured": false,
  "imageFileName": "whipped-butter.jpg",
  "tags": ["butter", "whipped", "spreadable"]
}
```

Response:
- `201 Created` with created product
- `400 Bad Request` for validation errors
- `409 Conflict` if ProductId already exists

### PUT /api/products/{id}
Updates an existing product.

Response:
- `200 OK` with updated product
- `400 Bad Request` for validation errors
- `404 Not Found` if the product does not exist
- `409 Conflict` if ProductId duplicates another record

### DELETE /api/products/{id}
Deletes an existing product.

Response:
- `204 No Content` on success
- `404 Not Found` if the product does not exist

## DTO Recommendation
### ProductDto
- `id`
- `productId`
- `productName`
- `category`
- `brand`
- `shortDescription`
- `packageSize`
- `price`
- `isFeatured`
- `imageFileName`
- `tags`

### UpsertProductRequest
- `productId`
- `productName`
- `category`
- `brand`
- `shortDescription`
- `packageSize`
- `price`
- `isFeatured`
- `imageFileName`
- `tags`

## Validation Rules
- `productId` is required, unique, and should follow the `LLD###` format.
- `productName` is required.
- `category` is required and must be one of the supported values.
- `brand` is required.
- `shortDescription` is required.
- `packageSize` is required.
- `price` must be greater than 0.
- `tags` may be empty but should be normalized as a string array in the API contract.

## Error Response Shape
```json
{
  "message": "Validation failed.",
  "errors": {
    "productId": ["ProductId is required."],
    "price": ["Price must be greater than 0."]
  }
}
```

## Swagger Notes
- Enable Swagger UI in development.
- Group endpoints under `Products`.
- Include summaries for list, details, create, update, and delete actions.
- Provide example payloads for create and update.

## Controller Design Recommendation
- `ProductsApiController` should delegate all business operations to `IProductService`.
- The controller should not contain EF Core query logic directly.
- Validation may combine model validation and service-level business checks.

## Demo-Focused Non-Functional Requirements
- Endpoints should respond quickly for the small SQLite dataset.
- API contracts should be stable enough for Swagger demo use.
- Error messages should be clear and presenter-friendly.
- The API should be usable locally without authentication for demo speed.