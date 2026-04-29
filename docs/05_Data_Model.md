# LandOLakesDairyDemo Data Model

## Entity Model Summary
The demo centers on a single Product entity. This keeps the data model simple, supports the full catalog and admin feature set, and aligns directly with the seeded catalog.

## Primary Entity
### Product
| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| Id | int | Yes | Internal database key, auto-increment |
| ProductId | string | Yes | Business-friendly identifier such as `LLD001` |
| ProductName | string | Yes | Display name shown in UI and API |
| Category | string | Yes | Allowed values align to demo categories |
| Brand | string | Yes | Default brand is Land O Lakes |
| ShortDescription | string | Yes | Concise catalog summary |
| PackageSize | string | Yes | Examples: `1 lb box`, `1 quart` |
| Price | decimal(10,2) | Yes | Demo display price |
| IsFeatured | bool | Yes | Highlights products on landing page |
| ImageFileName | string | No | Maps to image or placeholder asset |
| Tags | string | No | Stored as comma-separated values for simplicity |
| CreatedUtc | DateTime | Yes | Set on insert |
| UpdatedUtc | DateTime | Yes | Set on insert and update |

## Category Values
- Butter & Spreads
- Cheese
- Whipping Cream & Half & Half

## Logical Relationships
- Product is a standalone entity.
- Category is stored as text rather than a separate table to reduce demo complexity.
- Tags are stored on the product record rather than in a many-to-many structure.

## Validation Rules
| Field | Rule |
| --- | --- |
| ProductId | Required, unique, max 20 characters, pattern like `LLD###` |
| ProductName | Required, max 100 characters |
| Category | Required, must match one of the supported category values |
| Brand | Required, max 60 characters |
| ShortDescription | Required, max 250 characters |
| PackageSize | Required, max 40 characters |
| Price | Required, must be greater than 0 and less than 100 |
| IsFeatured | Required boolean |
| ImageFileName | Optional, max 150 characters |
| Tags | Optional, max 250 characters when stored as delimited string |

## Suggested EF Core Model
```csharp
public class Product
{
    public int Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = "Land O Lakes";
    public string ShortDescription { get; set; } = string.Empty;
    public string PackageSize { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageFileName { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
```

## EF Core Configuration Notes
- Add a unique index on ProductId.
- Add indexes on Category, ProductName, and IsFeatured for small but clear query support.
- Store Price as decimal with two fractional digits.
- Seed initial records using `HasData` or a startup seed routine.

## Search And Filtering Strategy
- Category filter uses exact match on Category.
- Search matches against ProductName, ShortDescription, and Tags.
- Featured filter returns products where IsFeatured is true.

## Data Lifecycle
### Create
- Admin submits product form.
- System validates required fields and uniqueness of ProductId.
- Timestamps are generated.

### Update
- Admin edits an existing product.
- UpdatedUtc is refreshed.
- ProductId remains stable after creation for demo consistency.

### Delete
- Admin deletes a product.
- Product is physically removed from the SQLite database.

## Demo Data Guidance
- Use the 15-product seed catalog already defined for butter, cheese, and cream products.
- Keep image filenames stable even if placeholder images are used.
- Keep Brand consistent unless a future demo needs multi-brand comparison.