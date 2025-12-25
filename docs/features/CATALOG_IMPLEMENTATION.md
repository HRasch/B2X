# Catalog Service Implementation

Complete reference for the Catalog Service: API endpoints, models, validators, and usage examples.

## Overview

The Catalog Service manages product data, categories, and brands for B2Connect. It provides:
- Complete CRUD operations for products, categories, brands
- Multi-language content support (LocalizedContent)
- Elasticsearch integration for full-text search
- Fluent validation for all inputs
- Event-driven architecture with domain events

## API Endpoints

### Products

```http
GET    /api/v1/products                    # List all products (paginated)
GET    /api/v1/products/{id}               # Get single product
POST   /api/v1/products                    # Create product
PUT    /api/v1/products/{id}               # Update product
DELETE /api/v1/products/{id}               # Delete product
GET    /api/v1/products/search?q=query     # Full-text search (Elasticsearch)
```

### Categories

```http
GET    /api/v1/categories                  # List all categories
GET    /api/v1/categories/{id}             # Get single category
POST   /api/v1/categories                  # Create category
PUT    /api/v1/categories/{id}             # Update category
DELETE /api/v1/categories/{id}             # Delete category
GET    /api/v1/categories/tree             # Hierarchical tree (parent-child)
```

### Brands

```http
GET    /api/v1/brands                      # List all brands
GET    /api/v1/brands/{id}                 # Get single brand
POST   /api/v1/brands                      # Create brand
PUT    /api/v1/brands/{id}                 # Update brand
DELETE /api/v1/brands/{id}                 # Delete brand
```

## Data Models

### Product

```csharp
public record ProductDto(
    Guid Id,
    string Sku,                           // Uppercase, unique
    LocalizedContent Name,                // Multi-language
    LocalizedContent Description,
    decimal Price,                        // Retail price
    decimal? B2bPrice,                   // Wholesale price (optional)
    int StockQuantity,
    string[] Tags,
    ProductAttributesDto Attributes,
    string[] ImageUrls,
    Guid? CategoryId,
    Guid? BrandId,
    Guid TenantId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive
);

public record LocalizedContent(
    Dictionary<string, string> Values     // Language code → translated text
);
```

### Category

```csharp
public record CategoryDto(
    Guid Id,
    LocalizedContent Name,
    LocalizedContent Description,
    Guid? ParentCategoryId,               // For hierarchies
    string IconUrl,
    int DisplayOrder,
    Guid TenantId,
    DateTime CreatedAt,
    bool IsActive
);
```

### Brand

```csharp
public record BrandDto(
    Guid Id,
    string Name,
    string Description,
    string LogoUrl,
    string Website,
    Guid TenantId,
    DateTime CreatedAt,
    bool IsActive
);
```

## Validation Rules

### Product Validation

- **SKU**: Required, uppercase, 3-20 chars, unique per tenant
- **Name**: Required, 2-255 chars, translations required for configured languages
- **Price**: Required, > 0, max 999999.99
- **Stock**: Required, >= 0
- **ImageUrls**: Valid URLs
- **Tags**: Max 20 tags, each 2-50 chars

### Category Validation

- **Name**: Required, 2-100 chars
- **ParentId**: If set, must exist and not be circular
- **DisplayOrder**: >= 0

### Brand Validation

- **Name**: Required, 2-100 chars, unique
- **LogoUrl**: Valid URL format

See `AOP_VALIDATION_IMPLEMENTATION.md` for validator implementations.

## Usage Examples

### Create Product (via Frontend)

```typescript
// frontend-admin/src/services/api/catalog.ts
const productData = {
  sku: "PROD-001",
  name: { "en": "Test Product", "de": "Testprodukt" },
  description: { "en": "A great product", "de": "Ein großartiges Produkt" },
  price: 99.99,
  b2bPrice: 75.00,
  stockQuantity: 100,
  tags: ["electronics", "featured"],
  attributes: { color: "blue", size: "medium" },
  imageUrls: ["https://example.com/image1.jpg"],
  categoryId: "category-uuid",
  brandId: "brand-uuid"
};

const product = await catalogService.createProduct(productData);
```

### Backend Controller

```csharp
[HttpPost]
[ValidateModel]  // AOP filter
public async Task<ActionResult<ProductDto>> CreateProduct(
    [FromBody] CreateProductRequest request)
{
    var product = await _catalogService.CreateProductAsync(request);
    
    // Event is automatically published via event validation
    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}
```

### Search via Elasticsearch

```csharp
var results = await _searchService.SearchProductsAsync("laptop", tenantId);
// Returns products matching "laptop" with relevance score
```

## Event Integration

When a product is created/updated/deleted, domain events are published:

```csharp
// ProductCreatedEvent
public record ProductCreatedEvent(
    Guid ProductId,
    string Sku,
    LocalizedContent Name,
    decimal Price,
    // ... other fields
) : DomainEvent;

// Automatically validated by EventValidationMiddleware
// See EVENT_VALIDATION_IMPLEMENTATION.md
```

## Frontend Integration

**Type-Safe Store (Pinia):**

```typescript
// frontend-admin/src/stores/catalog.ts
const catalog = useCatalogStore();

// Load products
await catalog.loadProducts(page: 1, pageSize: 20);

// Create product
await catalog.createProduct(productData);

// Filter/search
const filtered = catalog.filteredProducts; // computed property
```

**Components:**

- `ProductsView.vue` - Table with pagination and search
- `CategoriesView.vue` - Tree view with parent-child relationships
- `BrandsView.vue` - Grid layout with logos
- Form components for create/edit (see `frontend-admin/src/views/catalog/`)

## Configuration

### Enable Elasticsearch Indexing

```bash
export ELASTICSEARCH_ENABLED=true
```

Products are automatically indexed on create/update.

### Multi-Language Support

Set configured languages in database config:

```csharp
var supportedLanguages = new[] { "en", "de", "fr", "es" };
```

Product names must have translations for all configured languages.

## Testing

### Unit Tests

```bash
# Run only catalog tests
dotnet test --filter "CatalogService"
```

### Integration Tests

```bash
# Includes database, Elasticsearch, events
dotnet test --filter "Catalog.Integration"
```

### API Testing (Postman/Thunder Client)

```http
POST http://localhost:9001/api/v1/products
Content-Type: application/json

{
  "sku": "TEST-001",
  "name": { "en": "Test Product" },
  "price": 99.99,
  "stockQuantity": 50
}
```

## Troubleshooting

### Products not appearing in search
- Check `ELASTICSEARCH_ENABLED=true`
- Verify Elasticsearch is running
- Rebuild indexes: `POST /api/v1/catalog/reindex`

### Validation errors
- Check `.copilot-specs.md` Section 21 for all validation rules
- See `AOP_VALIDATION_IMPLEMENTATION.md` for validator details

### Circular parent categories
- ValidationException thrown
- ParentCategoryId must not create cycles

## References

- [.copilot-specs.md](../../.copilot-specs.md) Section 21 (Catalog API spec)
- [AOP_VALIDATION_IMPLEMENTATION.md](AOP_VALIDATION_IMPLEMENTATION.md) (Validators)
- [EVENT_VALIDATION_IMPLEMENTATION.md](EVENT_VALIDATION_IMPLEMENTATION.md) (Domain events)
- [ELASTICSEARCH_IMPLEMENTATION.md](ELASTICSEARCH_IMPLEMENTATION.md) (Search integration)
- [VSCODE_ASPIRE_CONFIG.md](../architecture/VSCODE_ASPIRE_CONFIG.md) (Debug Catalog Service)
