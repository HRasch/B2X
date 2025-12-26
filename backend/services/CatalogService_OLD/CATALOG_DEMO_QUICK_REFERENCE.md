# Catalog Service - Demo Database Quick Reference

## 🚀 Get Started in 30 Seconds

```bash
cd backend/services/CatalogService
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

✅ Service will:
- Start on `http://localhost:5008`
- Create InMemory database
- Generate 50 realistic products
- Populate with demo data
- Ready for API testing

## 📡 Test the API

```bash
# Get all products
curl http://localhost:5008/api/v1/products | jq .

# Get featured products
curl http://localhost:5008/api/v1/products/featured | jq .

# Get categories
curl http://localhost:5008/api/v1/categories | jq .

# Get brands
curl http://localhost:5008/api/v1/brands | jq .

# Search products
curl http://localhost:5008/api/v1/products/search?query=gaming | jq .

# Health check
curl http://localhost:5008/health
```

## 🌐 Browse Swagger Documentation

Open in browser:
```
http://localhost:5008/swagger
```

All 26 endpoints with demo data examples

## ⚙️ Configuration

### Enable Demo Mode (Development)
```json
// appsettings.Development.json
{
  "CatalogService": {
    "UseInMemoryDemo": false,
    "UseDemoDataByDefault": true,      // ← Auto-enable in Development
    "DemoProductCount": 50
  }
}
```

### Force Demo Mode (Any Environment)
```json
{
  "CatalogService": {
    "UseInMemoryDemo": true            // ← Always use InMemory
  }
}
```

### Disable Demo Mode (Use PostgreSQL)
```json
{
  "CatalogService": {
    "UseInMemoryDemo": false,
    "UseDemoDataByDefault": false      // ← Use real database
  }
}
```

## 📊 Demo Data

| Entity | Count | Details |
|--------|-------|---------|
| **Products** | 50 | Gaming/Business laptops, peripherals |
| **Brands** | 10 | Apple, Dell, HP, Lenovo, ASUS, etc. |
| **Categories** | 7 | Electronics → Computers/Peripherals |
| **Variants** | 150+ | Colors, storage configurations |
| **Images** | 250+ | 2-6 per product |
| **Documents** | 50+ | Specs, manuals, datasheets |
| **Languages** | 3 | English, German, French |

## 🧪 Verify Demo Database

```bash
# Run verification script
./verify-demo-db.sh

# Or manually check:
curl http://localhost:5008/api/v1/products?limit=1 | jq .data[0]
```

## 🔧 Development Usage

### In Tests
```csharp
var factory = serviceProvider.GetRequiredService<ICatalogDbContextFactory>();
var demoContext = factory.CreateDemoContext(productCount: 100, seed: 42);

// Same data every time (seed = 42)
Assert.Equal(100, demoContext.Products.Count());
```

### Customize Data Generation
Edit `src/Data/CatalogDemoDataGenerator.cs`:
- Modify `GenerateCategories()` for different hierarchy
- Modify `GenerateBrands()` for different brands
- Adjust `productTypes` array for product names
- Customize pricing ranges, stock levels, etc.

## 📈 Scale Demo Data

```json
// Generate 200 products instead of 50
"DemoProductCount": 200

// Generate 1000 products for load testing
"DemoProductCount": 1000
```

## 🐛 Troubleshooting

### Products table empty?
1. Check `ASPNETCORE_ENVIRONMENT=Development`
2. Verify `appsettings.Development.json` settings
3. Clean PostgreSQL database if it exists:
   ```bash
   dotnet ef database drop -f
   dotnet run
   ```

### Service won't start?
1. Check Bogus package is installed:
   ```bash
   grep Bogus B2Connect.CatalogService.csproj
   ```
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Check logs for specific errors

### Performance issues?
1. Reduce `DemoProductCount` to 10-20
2. Check available RAM (demo data ~100MB per 1000 products)
3. Use indexed queries: `/products/sku/{sku}`

## 📚 Documentation

- **Full Guide:** [CATALOG_DEMO_DATABASE.md](./CATALOG_DEMO_DATABASE.md)
- **Implementation:** [CATALOG_DEMO_IMPLEMENTATION.md](./CATALOG_DEMO_IMPLEMENTATION.md)
- **API Reference:** [CATALOG_API_REFERENCE.md](../../CATALOG_API_REFERENCE.md)
- **Quick Start:** [CATALOG_QUICK_START.md](../../CATALOG_QUICK_START.md)

## 🎯 Common Tasks

### Get Single Product with Details
```bash
curl http://localhost:5008/api/v1/products/SKU-0001 \
  | jq '.brand, .variants, .images, .documents'
```

### Get Product by Slug
```bash
curl "http://localhost:5008/api/v1/products/slug/gaming-laptop-asus-rog" \
  | jq '.name, .price, .variants[0]'
```

### Get All Gaming Laptops
```bash
curl "http://localhost:5008/api/v1/products/search?query=gaming" \
  | jq '.data[].name'
```

### Get Products in Category
```bash
curl http://localhost:5008/api/v1/products/category/{categoryId} \
  | jq '.data | length'
```

### Get Category Hierarchy
```bash
curl http://localhost:5008/api/v1/categories/hierarchy \
  | jq '.data[] | select(.children != null) | {name, children}'
```

### Pagination
```bash
# Get page 2, 20 items per page
curl "http://localhost:5008/api/v1/products/paged?page=2&pageSize=20" \
  | jq '.pageNumber, .pageSize, .totalCount, .totalPages'
```

## 🔄 Startup in Development

```bash
# Terminal 1: Catalog Service (InMemory Demo)
cd backend/services/CatalogService
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Terminal 2: Full Aspire Backend
cd backend/services/AppHost
dotnet run

# Terminal 3: Frontend
cd frontend
npm run dev
```

Or use task:
```
✅ Full Startup (Backend + Frontend)
```

## 📋 Checklist

After starting the service, verify:

- [ ] Service starts without errors
- [ ] Logs show "✅ Demo database seeded successfully"
- [ ] Health endpoint returns OK: `curl http://localhost:5008/health`
- [ ] Products endpoint returns data: `curl http://localhost:5008/api/v1/products`
- [ ] Swagger loads: Open `http://localhost:5008/swagger`
- [ ] Search works: `curl http://localhost:5008/api/v1/products/search?query=laptop`
- [ ] Categories load: `curl http://localhost:5008/api/v1/categories`

## 🎓 Learning Path

1. **Basic Testing** → Use Swagger UI (http://localhost:5008/swagger)
2. **API Integration** → Test with curl or Postman
3. **Data Customization** → Modify `CatalogDemoDataGenerator.cs`
4. **Advanced Usage** → Write integration tests with `CreateDemoContext()`
5. **Production** → Switch to PostgreSQL in appsettings.json

## 📞 Support

For issues:
1. Check [CATALOG_DEMO_DATABASE.md - Troubleshooting](./CATALOG_DEMO_DATABASE.md#troubleshooting)
2. Review startup logs (enable Debug logging)
3. Check Bogus documentation: https://github.com/bchavez/Bogus
4. Review implementation: [CatalogDemoDataGenerator.cs](./src/Data/CatalogDemoDataGenerator.cs)

---

**Remember:** InMemory database data is **NOT persisted** between restarts. It's for development and testing only. Use PostgreSQL for production.
