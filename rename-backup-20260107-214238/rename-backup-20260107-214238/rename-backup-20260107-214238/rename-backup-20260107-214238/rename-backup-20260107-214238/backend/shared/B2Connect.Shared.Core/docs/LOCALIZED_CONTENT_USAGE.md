# LocalizedContent Value Object - Usage Guide

## Overview

`LocalizedContent` is an immutable Value Object for embedding multi-language content directly in domain entities. It supports:

- **Admin Area**: Full CRUD with all translations (JSON stored)
- **Store Area**: DTOs with pre-translated properties for the requested culture
- **Search**: LINQ-searchable across indexed languages with B-tree database indexes

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Product Entity                          │
├─────────────────────────────────────────────────────────────────┤
│ Id: Guid                                                        │
│ TenantId: Guid                                                  │
│ Sku: string                                                     │
│ Name: LocalizedContent  ←─────┐                                 │
│ Description: LocalizedContent │ Value Objects                   │
│ Price: decimal                                                  │
└───────────────────────────────────────────────────────────────── │

Database Columns (Code-First Generated):
┌──────────────────────────────────────────────────────────────────┐
│ Products Table                                                   │
├──────────────────────────────────────────────────────────────────┤
│ Id (PK)                                                          │
│ TenantId (FK)                                                    │
│ Sku                                                              │
│ Name_Default         ← DefaultValue                              │
│ Name_Translations    ← JSON: {"de":"...","en":"...","fr":"..."}  │
│ Name_DE              ← Indexed shadow property                   │
│ Name_EN              ← Indexed shadow property                   │
│ Name_SearchText      ← Optional: all languages combined          │
│ Description_Default                                              │
│ Description_Translations                                         │
│ Description_DE                                                   │
│ Description_EN                                                   │
│ Price                                                            │
└──────────────────────────────────────────────────────────────────┘
```

## Step 1: Update Entity with LocalizedContent

```csharp
// Product.cs
using B2Connect.Shared.Core;

public class Product : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    
    public required string Sku { get; set; }
    
    // ✅ LocalizedContent for multi-language properties
    public required LocalizedContent Name { get; set; }
    public LocalizedContent? Description { get; set; }
    
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Factory method for creating products with localization
    public static Product Create(
        Guid tenantId, 
        string sku, 
        string defaultName,
        decimal price)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Sku = sku,
            Name = new LocalizedContent(defaultName),
            Price = price
        };
    }
    
    // Add translation (returns new Product due to immutability)
    public Product WithNameTranslation(string languageCode, string translation)
    {
        return this with { Name = Name.WithTranslation(languageCode, translation) };
    }
}
```

## Step 2: Configure DbContext

```csharp
// CatalogDbContext.cs
using B2Connect.Shared.Core;
using Microsoft.EntityFrameworkCore;

public class CatalogDbContext : TenantDbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Product with LocalizedContent
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Sku).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            
            // ✅ Configure Name with indexed language columns
            entity.ConfigureRequiredLocalizedContent(
                p => p.Name,
                "Name",
                new LocalizedContentOptions
                {
                    IndexedLanguages = new[] { "de", "en" },
                    IncludeSearchColumn = true,  // For cross-language search
                    MaxLength = 500
                });
            
            // ✅ Configure Description (optional)
            entity.ConfigureLocalizedContent(
                p => p.Description,
                "Description",
                new LocalizedContentOptions
                {
                    IndexedLanguages = new[] { "de", "en" },
                    IncludeSearchColumn = false,
                    MaxLength = 5000
                });
        });
    }
    
    // ✅ Sync shadow properties on save
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SyncLocalizedContentColumns();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    public override int SaveChanges()
    {
        SyncLocalizedContentColumns();
        return base.SaveChanges();
    }
    
    private void SyncLocalizedContentColumns()
    {
        var nameOptions = new LocalizedContentOptions 
        { 
            IndexedLanguages = new[] { "de", "en" },
            IncludeSearchColumn = true 
        };
        
        var descriptionOptions = new LocalizedContentOptions 
        { 
            IndexedLanguages = new[] { "de", "en" } 
        };
        
        var changedProducts = ChangeTracker.Entries<Product>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            
        foreach (var entry in changedProducts)
        {
            var product = entry.Entity;
            this.SyncLocalizedColumns(product, p => p.Name, "Name", nameOptions);
            this.SyncLocalizedColumns(product, p => p.Description, "Description", descriptionOptions);
        }
    }
}
```

## Step 3: Generate Migration

```bash
# Generate migration for the new columns
dotnet ef migrations add AddLocalizedProductColumns -p backend/Domain/Catalog

# Preview the SQL
dotnet ef migrations script -p backend/Domain/Catalog
```

Expected migration creates:
- `Name_Default` (varchar)
- `Name_Translations` (json/text)
- `Name_DE` (varchar, indexed)
- `Name_EN` (varchar, indexed)
- `Name_SearchText` (varchar, indexed)
- Same pattern for Description

## Step 4: Admin API (Full CRUD with All Translations)

```csharp
// AdminProductDto.cs
public record AdminProductDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = "";
    
    // Full localized content for admin editing
    public LocalizedContentDto Name { get; init; } = new();
    public LocalizedContentDto? Description { get; init; }
    
    public decimal Price { get; init; }
}

public record LocalizedContentDto
{
    public string DefaultValue { get; init; } = "";
    public Dictionary<string, string> Translations { get; init; } = new();
}

// AdminProductsController.cs
[ApiController]
[Route("api/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly CatalogDbContext _context;
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AdminProductDto>> GetProduct(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        
        return new AdminProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = new LocalizedContentDto
            {
                DefaultValue = product.Name.DefaultValue,
                Translations = new Dictionary<string, string>(product.Name.Translations)
            },
            Description = product.Description == null ? null : new LocalizedContentDto
            {
                DefaultValue = product.Description.DefaultValue,
                Translations = new Dictionary<string, string>(product.Description.Translations)
            },
            Price = product.Price
        };
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, AdminProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        
        // Rebuild LocalizedContent from DTO
        product.Name = new LocalizedContent(dto.Name.DefaultValue, dto.Name.Translations);
        product.Description = dto.Description == null ? null 
            : new LocalizedContent(dto.Description.DefaultValue, dto.Description.Translations);
        product.Price = dto.Price;
        
        await _context.SaveChangesAsync(); // Syncs shadow columns
        return NoContent();
    }
}
```

## Step 5: Store API (Pre-Translated DTOs)

```csharp
// StoreProductDto.cs - Culture-specific response
public record StoreProductDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = "";
    
    // Pre-translated values (no dictionary)
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    
    public decimal Price { get; init; }
}

// StoreProductsController.cs
[ApiController]
[Route("api/store/products")]
public class StoreProductsController : ControllerBase
{
    private readonly CatalogDbContext _context;
    
    [HttpGet]
    public async Task<ActionResult<List<StoreProductDto>>> GetProducts(
        [FromHeader(Name = "Accept-Language")] string? language = "en")
    {
        var lang = language?.Split(',').FirstOrDefault()?.Split('-').FirstOrDefault() ?? "en";
        
        var products = await _context.Products
            .Where(p => p.IsActive)
            .Select(p => new StoreProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name.GetValue(lang),           // Pre-translated
                Description = p.Description != null 
                    ? p.Description.GetValue(lang) 
                    : null,
                Price = p.Price
            })
            .ToListAsync();
            
        return products;
    }
}
```

## Step 6: LINQ Queries with Indexed Columns

```csharp
// Search products in German by name
var germanProducts = await _context.Products
    .WhereLocalizedContains("Name", "de", "Laptop")
    .OrderByLocalized("Name", "de")
    .ToListAsync();

// Exact match for English name
var exactMatch = await _context.Products
    .WhereLocalizedEquals("Name", "en", "Gaming Mouse Pro")
    .FirstOrDefaultAsync();

// Autocomplete (starts with)
var suggestions = await _context.Products
    .WhereLocalizedStartsWith("Name", "en", "Gam")
    .Take(10)
    .ToListAsync();

// Search across all indexed languages
var multiLangResults = await _context.Products
    .WhereAnyLanguageContains("Name", "Laptop", "de", "en")
    .ToListAsync();
```

## Performance Considerations

| Query Type | Index Used | Performance |
|------------|------------|-------------|
| `WhereLocalizedContains("Name", "de", ...)` | `IX_Product_Name_DE` | ⚡ Fast |
| `WhereLocalizedEquals("Name", "en", ...)` | `IX_Product_Name_EN` | ⚡ Fast |
| `WhereLocalizedStartsWith(...)` | B-tree index | ⚡ Fast |
| `WhereAnyLanguageContains(...)` | Multiple indexes | ✅ Good |
| JSON path queries | None | ❌ Slow (full scan) |

## Best Practices

1. **Index commonly searched languages** - Only include languages you actually query by
2. **Use SearchText for multi-language full-text** - When users search without specifying language
3. **Keep MaxLength reasonable** - Large columns impact index performance
4. **Use DTOs appropriately** - Admin gets full LocalizedContent, Store gets pre-translated strings
5. **Always sync on save** - Call `SyncLocalizedColumns` in SaveChanges override

## Migration Example (PostgreSQL)

```sql
-- Generated by EF Core migration
ALTER TABLE "Products" ADD "Name_Default" character varying(500) NOT NULL;
ALTER TABLE "Products" ADD "Name_Translations" text NOT NULL;
ALTER TABLE "Products" ADD "Name_DE" character varying(500);
ALTER TABLE "Products" ADD "Name_EN" character varying(500);
ALTER TABLE "Products" ADD "Name_SearchText" character varying(1000);

CREATE INDEX "IX_Product_Name_DE" ON "Products" ("Name_DE");
CREATE INDEX "IX_Product_Name_EN" ON "Products" ("Name_EN");
CREATE INDEX "IX_Product_Name_Search" ON "Products" ("Name_SearchText");
```

## Summary

| Aspect | Implementation |
|--------|----------------|
| Entity Property | `LocalizedContent Name { get; set; }` |
| Storage | JSON + indexed shadow columns |
| Admin API | Returns full `LocalizedContentDto` |
| Store API | Returns pre-translated `string` |
| Search | LINQ extensions with indexed columns |
| Index Strategy | Per-language B-tree indexes |
