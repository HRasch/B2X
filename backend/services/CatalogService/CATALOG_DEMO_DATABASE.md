# Catalog Service - In-Memory Demo Database Guide

## Overview

The Catalog Service includes a built-in **In-Memory Demo Database** with realistic, faker-generated product data. This feature enables rapid development and testing without requiring a PostgreSQL or SQL Server instance.

## Quick Start

### Enable Demo Mode (Development Only)

The demo database is **automatically enabled in Development environment**. To use it:

```bash
cd backend/services/CatalogService
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

The service will:
1. ✅ Create an in-memory database
2. ✅ Generate 50 sample products with Bogus
3. ✅ Create realistic categories, brands, variants, images, and documents
4. ✅ Seed all data on startup
5. ✅ Ready for API testing

### Disable Demo Mode

To use a real PostgreSQL database instead:

**Option 1: Modify appsettings.Development.json**
```json
{
  "CatalogService": {
    "UseDemoDataByDefault": false,
    "UseInMemoryDemo": false
  }
}
```

**Option 2: Environment Variable**
```bash
CATALOG_USE_INMEMORY_DEMO=false dotnet run
```

## Configuration

### appsettings.Development.json

```json
{
  "CatalogService": {
    "UseInMemoryDemo": false,              // Explicitly use in-memory (overrides auto-detect)
    "UseDemoDataByDefault": true,          // Auto-use demo in Development environment
    "DemoProductCount": 50,                // Number of products to generate
    "UsePostgres": true                    // Use PostgreSQL for real database
  }
}
```

### Configuration Priority

1. `UseInMemoryDemo: true` → **Always** use in-memory demo
2. `UseInMemoryDemo: false` AND `UseDemoDataByDefault: true` AND `IsDevelopment` → Use demo
3. `UseInMemoryDemo: false` AND `UseDemoDataByDefault: false` → Use configured database
4. Production environment → Always uses PostgreSQL/SQL Server

## Generated Demo Data

### Data Structure

The demo generator creates a complete, realistic catalog:

| Entity | Count | Details |
|--------|-------|---------|
| **Brands** | 10 | Apple, Dell, HP, Lenovo, ASUS, Acer, MSI, Razer, Corsair, Logitech |
| **Categories** | 7 | Electronics → Computers, Peripherals with subcategories |
| **Products** | 50 | Gaming/Business laptops, desktops, peripherals |
| **Variants** | 150-250 | Colors, storage configurations per product |
| **Images** | 250-500 | Product photos (2-6 per product) |
| **Documents** | 50-100 | Manuals, specifications, datasheets |

### Multilingual Content

All data supports **3 languages**:
- 🇬🇧 **English** (en)
- 🇩🇪 **German** (de)
- 🇫🇷 **French** (fr)

Example:
```csharp
// Automatically generated
Product.Name = new LocalizedContent()
    .Set("en", "Gaming Laptop ASUS ROG")
    .Set("de", "Gaming Laptop ASUS ROG")
    .Set("fr", "Gaming Laptop ASUS ROG");
```

### Data Generation (Bogus)

Uses the **Bogus** library to generate realistic data:

```csharp
// Example: Product Names
"Gaming Laptop Dell XPS", "Business Laptop Lenovo ThinkPad", "Ultrabook HP Spectre"

// Example: Prices
$599.99, $1,299.99, $2,499.99 (with variants ±20%)

// Example: Stock
0-500 units, low stock threshold 5-20

// Example: Specifications
Realistic weight (1.2-2.8 kg), realistic ratings (2.5-5.0 stars)
```

## API Testing

Once started, test the API with the demo data:

### Get All Products
```bash
curl http://localhost:5008/api/v1/products
```

### Get Featured Products
```bash
curl http://localhost:5008/api/v1/products/featured
```

### Search Products
```bash
curl http://localhost:5008/api/v1/products/search?query=gaming
```

### Get Categories
```bash
curl http://localhost:5008/api/v1/categories
```

### Get Category Hierarchy
```bash
curl http://localhost:5008/api/v1/categories/hierarchy
```

### Health Check
```bash
curl http://localhost:5008/health
```

## Swagger Documentation

Access the interactive API documentation:

```
http://localhost:5008/swagger
```

Browse all 26 endpoints with demo data examples.

## Startup Logs

When using the demo database, you'll see:

```
🔄 Using IN-MEMORY DEMO DATABASE with realistic test data
📊 Seeding demo database with sample products, categories, and brands...
✅ Demo database seeded successfully!
   📦 Products: 50
   🏷️  Categories: 7
   🏢 Brands: 10
   🖼️  Product Variants: 175
   📸 Product Images: 312
   📄 Product Documents: 68
✅ Catalog Service started successfully
```

## Usage Scenarios

### Development

```bash
# Start with demo data for rapid UI development
cd backend/services/CatalogService
dotnet run
# → In-memory database with 50 products, immediately available
```

### Testing

```bash
# Run integration tests with reproducible demo data
var context = demoFactory.CreateDemoContext(productCount: 100, seed: 42);
// → Same data every time (seed = 42)
```

### Presentation/Demo

```bash
# Customize demo data size
CatalogService:DemoProductCount: 200  // Generate 200 products instead of 50
```

### Load Testing

```csharp
// Generate large dataset for performance testing
var context = factory.CreateDemoContext(productCount: 10000, seed: 123);
```

## Implementation Details

### CatalogDemoDataGenerator

Located in `src/Data/CatalogDemoDataGenerator.cs`

**Key Methods:**
- `GenerateDemoCatalog(productCount, seed)` → Main entry point
- `GenerateCategories()` → 7-category hierarchy (Electronics → Computers/Peripherals)
- `GenerateBrands()` → 10 realistic tech brands
- `GenerateProducts()` → Products with variants, images, documents
- `GenerateVariants()` → Colors, storage configurations
- `GenerateImages()` → 2-6 images per product (placeholder URLs)
- `GenerateDocuments()` → Manuals, specs, datasheets (multilingual)

**Seed Parameter:**
- Fixed seed (e.g., `seed: 42`) = reproducible data
- No seed = new random data each run

### CatalogDbContextFactory

Located in `src/Data/CatalogDbContextFactory.cs`

**Interfaces:**
```csharp
// For DI registration
public interface ICatalogDbContextFactory
{
    CatalogDbContext CreateProductionContext();
    CatalogDbContext CreateDemoContext(int productCount = 100, int? seed = null);
}
```

**Usage in Tests:**
```csharp
var factory = serviceProvider.GetRequiredService<ICatalogDbContextFactory>();
var demoContext = factory.CreateDemoContext(productCount: 50, seed: 42);
```

### Program.cs Integration

```csharp
// Automatic detection
var useInMemoryDemo = builder.Configuration.GetValue<bool>("CatalogService:UseInMemoryDemo", false) 
    || (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("CatalogService:UseDemoDataByDefault", true));

// Conditional seeding
if (useInMemoryDemo)
{
    await dbContext.Database.EnsureCreatedAsync();
    if (!await dbContext.Products.AnyAsync())
    {
        var (categories, brands, products) = CatalogDemoDataGenerator.GenerateDemoCatalog(50);
        // Save to context
    }
}
```

## Limitations & Notes

| Aspect | Note |
|--------|------|
| **Persistence** | In-memory data is **NOT persisted** between restarts |
| **Performance** | In-memory suitable for development; use real DB for load testing |
| **Concurrency** | In-memory InMemoryDatabase is NOT thread-safe for concurrent writes |
| **Image URLs** | Use placeholder URLs (picsum.photos); configure CDN in production |
| **Search** | LIKE-based search works; full-text search requires PostgreSQL/SQL Server |

## Troubleshooting

### Demo Data Not Generating

**Problem:** Products table is empty despite `UseDemoDataByDefault: true`

**Solutions:**
1. Check `appsettings.Development.json` is not overridden by `appsettings.json`
2. Verify `ASPNETCORE_ENVIRONMENT=Development`
3. Clear any previous database:
   ```bash
   cd backend/services/CatalogService
   dotnet ef database drop -f  # Remove PostgreSQL database
   dotnet run  # Restart with fresh demo data
   ```

### Performance Issues

**Problem:** Slow queries or timeout errors with large demo dataset

**Solutions:**
1. Reduce `DemoProductCount` in appsettings
2. Use indexed fields for queries (SKU, Slug)
3. Use `.AsNoTracking()` for read-only queries

### Data Not Found

**Problem:** Generate 50 products but only 20 returned in API

**Potential Causes:**
- Products filtered by `IsActive = true` (some may be inactive)
- Products filtered by `TenantId` mismatch
- Pagination limits results

**Check:**
```bash
curl http://localhost:5008/api/v1/products?limit=1000
```

## Next Steps

1. **Test the API** → Use Swagger or curl to test with demo data
2. **Customize Data** → Modify `CatalogDemoDataGenerator` for your needs
3. **Create Tests** → Write integration tests with `CreateDemoContext()`
4. **Switch to Real DB** → Update appsettings to use PostgreSQL

## Related Files

- [CATALOG_QUICK_START.md](../../CATALOG_QUICK_START.md) → Getting started guide
- [CATALOG_API_REFERENCE.md](../../CATALOG_API_REFERENCE.md) → Complete API documentation
- [CATALOG_SUMMARY.md](../../CATALOG_SUMMARY.md) → Architecture overview
- [CatalogDemoDataGenerator.cs](./CatalogDemoDataGenerator.cs) → Data generation logic
- [CatalogDbContextFactory.cs](./CatalogDbContextFactory.cs) → Factory implementation
- [Program.cs](../Program.cs) → Startup configuration

## Support

For issues or customization requests:
1. Check the logs (enable Debug logging in development)
2. Review Bogus documentation: https://github.com/bchavez/Bogus
3. Modify `CatalogDemoDataGenerator` for custom data
