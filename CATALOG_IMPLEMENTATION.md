# B2Connect Katalog-Funktionalität - Implementierungsübersicht

## 📋 Überblick

Die Katalog-Funktionalität wurde vollständig implementiert mit Unterstützung für:
- ✅ **Produkte** mit mehrsprachigen Eigenschaften
- ✅ **Varianten** für Größen, Farben, etc.
- ✅ **Merkmale/Attribute** mit Optionen
- ✅ **Kategorien** mit hierarchischer Struktur
- ✅ **Marken** mit Produktkatalog
- ✅ **Bilder** mit mehreren Auflösungen
- ✅ **Dokumente** (Spezifikationen, Handbücher, Zertifikationen)
- ✅ **Mehrsprachigkeit** über LocalizedContent

---

## 🏗️ Architektur

### Projekt-Struktur

```
backend/services/CatalogService/
├── src/
│   ├── Models/
│   │   ├── Product.cs                  # Hauptprodukt-Entity
│   │   ├── ProductVariant.cs           # Produkt-Varianten
│   │   ├── ProductAttribute.cs         # Attribute & Optionen
│   │   ├── Category.cs                 # Kategorien (hierarchisch)
│   │   ├── Brand.cs                    # Marken
│   │   ├── ProductImage.cs             # Produktbilder
│   │   ├── ProductDocument.cs          # Produktdokumente
│   │   ├── ProductCategory.cs          # Junction (M:N)
│   │   ├── ProductAttributeValue.cs    # Junction (M:N)
│   │   └── VariantAttributeValue.cs    # Variant-Attribute
│   │
│   ├── Data/
│   │   └── CatalogDbContext.cs         # EF Core Context
│   │
│   ├── Repositories/
│   │   ├── IRepository.cs              # Generisches Interface
│   │   ├── Repository.cs               # Basis-Implementierung
│   │   ├── IProductRepository.cs       # Product-spezifisch
│   │   ├── ProductRepository.cs
│   │   ├── ICategoryRepository.cs      # Category-spezifisch
│   │   ├── CategoryRepository.cs
│   │   ├── IBrandRepository.cs         # Brand-spezifisch
│   │   ├── BrandRepository.cs
│   │   ├── IProductAttributeRepository.cs
│   │   └── ProductAttributeRepository.cs
│   │
│   ├── Services/
│   │   ├── IProductService.cs          # Service Interfaces
│   │   ├── ProductService.cs
│   │   ├── ICategoryService.cs
│   │   ├── CategoryService.cs
│   │   ├── IBrandService.cs
│   │   └── BrandService.cs
│   │
│   └── Controllers/
│       ├── ProductsController.cs       # REST API
│       ├── CategoriesController.cs
│       └── BrandsController.cs
│
├── Program.cs                          # ASP.NET Setup (zu erstellen)
├── appsettings.json                    # Konfiguration (zu erstellen)
└── B2Connect.CatalogService.csproj     # Projekt-Datei (zu erstellen)
```

---

## 📊 Datenmodell

### Hauptentitäten

#### 1. **Product** (Hauptprodukt)
```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; }                    // Eindeutige Produktnummer
    public string Slug { get; set; }                   // URL-freundlicher Name
    public LocalizedContent Name { get; set; }        // Mehrsprachig
    public LocalizedContent ShortDescription { get; set; }
    public LocalizedContent Description { get; set; }
    public decimal Price { get; set; }                // Basis-Preis
    public decimal? SpecialPrice { get; set; }        // Aktionspreis
    public int StockQuantity { get; set; }            // Verfügbarkeit
    public bool IsActive { get; set; }                // Veröffentlicht
    public bool IsFeatured { get; set; }              // Hervorgehobenes Produkt
    public bool IsNew { get; set; }                   // Neues Produkt
    
    // Beziehungen
    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public ICollection<ProductVariant> Variants { get; set; }
    public ICollection<ProductImage> Images { get; set; }
    public ICollection<ProductDocument> Documents { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; }
    public ICollection<ProductAttributeValue> AttributeValues { get; set; }
}
```

#### 2. **ProductVariant** (Größe, Farbe, etc.)
```csharp
public class ProductVariant
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; }                    // Variant-spezifische SKU
    public LocalizedContent Name { get; set; }        // z.B. "Rot, Größe M"
    public decimal? Price { get; set; }               // Optional: Andere Preis
    public int StockQuantity { get; set; }
    public bool IsDefault { get; set; }               // Standard-Variante
    
    // Beziehungen
    public ICollection<VariantAttributeValue> AttributeValues { get; set; }
}
```

#### 3. **ProductAttribute** & **ProductAttributeOption**
```csharp
public class ProductAttribute
{
    public Guid Id { get; set; }
    public string Code { get; set; }                  // z.B. "color", "size"
    public LocalizedContent Name { get; set; }        // Mehrsprachig
    public string AttributeType { get; set; }         // "select", "text", "date"
    public bool IsSearchable { get; set; }            // In Suche nutzbar
    public bool IsFilterable { get; set; }            // In Filter nutzbar
    
    public ICollection<ProductAttributeOption> Options { get; set; }
}

public class ProductAttributeOption
{
    public Guid Id { get; set; }
    public Guid AttributeId { get; set; }
    public string Code { get; set; }                  // z.B. "red", "large"
    public LocalizedContent Label { get; set; }       // "Rot", "Groß"
    public string? ColorValue { get; set; }           // "#FF0000" für Farben
}
```

#### 4. **Category** (Hierarchische Kategorien)
```csharp
public class Category
{
    public Guid Id { get; set; }
    public string Slug { get; set; }
    public LocalizedContent Name { get; set; }
    public LocalizedContent? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }       // Für Hierarchie
    
    // Beziehungen
    public Category? ParentCategory { get; set; }
    public ICollection<Category> ChildCategories { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; }
}
```

#### 5. **Brand** (Marken)
```csharp
public class Brand
{
    public Guid Id { get; set; }
    public string Slug { get; set; }
    public LocalizedContent Name { get; set; }
    public LocalizedContent? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    
    public ICollection<Product> Products { get; set; }
}
```

#### 6. **ProductImage** (Bilder mit mehreren Größen)
```csharp
public class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Url { get; set; }                   // Original-URL
    public string? ThumbnailUrl { get; set; }         // Vorschau (z.B. 100x100)
    public string? MediumUrl { get; set; }            // Mittel (z.B. 300x300)
    public string? LargeUrl { get; set; }             // Groß (z.B. 800x800)
    public bool IsPrimary { get; set; }               // Hauptbild
    
    // Metadaten
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? MimeType { get; set; }             // image/jpeg, etc.
}
```

#### 7. **ProductDocument** (PDFs, Spezifikationen, etc.)
```csharp
public class ProductDocument
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public LocalizedContent Name { get; set; }        // Mehrsprachig
    public LocalizedContent? Description { get; set; }
    public string DocumentType { get; set; }          // "specification", "manual", "certification"
    public string Url { get; set; }
    public string? Language { get; set; }             // "en", "de", "fr"
    public string? Version { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
```

---

## 🔌 API-Endpunkte

### Products
```
GET    /api/products                     # Alle Produkte
GET    /api/products/{id}                # Nach ID
GET    /api/products/sku/{sku}           # Nach SKU
GET    /api/products/slug/{slug}         # Nach Slug
GET    /api/products/paged?page=1&size=10
GET    /api/products/category/{categoryId}
GET    /api/products/brand/{brandId}
GET    /api/products/featured?take=10
GET    /api/products/new?take=10
GET    /api/products/search?q=laptop
POST   /api/products                     # Neu erstellen
PUT    /api/products/{id}                # Aktualisieren
DELETE /api/products/{id}                # Löschen
```

### Categories
```
GET    /api/categories                   # Alle aktiven Kategorien
GET    /api/categories/{id}              # Nach ID
GET    /api/categories/slug/{slug}       # Nach Slug
GET    /api/categories/root              # Nur Root-Kategorien
GET    /api/categories/hierarchy         # Komplette Hierarchie
GET    /api/categories/{parentId}/children
POST   /api/categories                   # Neue Kategorie
PUT    /api/categories/{id}              # Aktualisieren
DELETE /api/categories/{id}              # Löschen
```

### Brands
```
GET    /api/brands                       # Alle Marken
GET    /api/brands/{id}                  # Nach ID
GET    /api/brands/slug/{slug}           # Nach Slug
GET    /api/brands/paged?page=1&size=10
POST   /api/brands                       # Neue Marke
PUT    /api/brands/{id}                  # Aktualisieren
DELETE /api/brands/{id}                  # Löschen
```

---

## 🌍 Mehrsprachige Eigenschaften

Die Katalog-Entitäten nutzen `LocalizedContent` aus der `shared/types` Library:

### Beispiel: Produkt mit Übersetzungen
```csharp
// Erstellen
var product = new Product
{
    Sku = "LAPTOP-001",
    Slug = "gaming-laptop",
    Name = new LocalizedContent()
        .Set("en", "Gaming Laptop")
        .Set("de", "Gaming-Laptop")
        .Set("fr", "Ordinateur de jeu"),
    
    Description = new LocalizedContent()
        .Set("en", "High-performance gaming laptop...")
        .Set("de", "Hochleistungs-Gaming-Laptop...")
        .Set("fr", "Ordinateur de jeu haute performance...")
};

// Abrufen
string germanName = product.Name.Get("de");        // "Gaming-Laptop"
string germanDesc = product.Description.Get("de"); // "Hochleistungs-Gaming-Laptop..."

// Fallback
string spanishName = product.Name.Get("es");  // Fallback auf Standard (en)
```

### Im Datenbank-Schema
- Alle `LocalizedContent`-Felder werden als **JSONB** in PostgreSQL gespeichert
- Format: `{"en": "English value", "de": "Deutsche Wert", "fr": "Valeur française"}`

---

## 💾 Repositories & Services

### Repository-Pattern
```csharp
// Generisches Interface
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<int> SaveChangesAsync();
}

// Spezialisierte Interfaces
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku);
    Task<Product?> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetFeaturedAsync(int take = 10);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize);
}
```

### Service-Layer
```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Sku = dto.Sku,
            Slug = dto.Slug,
            Name = LocalizedContent.FromDictionary(dto.Name),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity
        };

        await _productRepository.CreateAsync(product);
        await _productRepository.SaveChangesAsync();
        
        return MapToDto(product);
    }
}
```

---

## 📦 DTOs (Data Transfer Objects)

### ProductDto
```csharp
public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    public Dictionary<string, string> Name { get; set; }  // Alle Sprachen
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public List<ProductVariantDto> Variants { get; set; }
    public List<CategoryDto> Categories { get; set; }
}
```

### CreateProductDto
```csharp
public class CreateProductDto
{
    public string Sku { get; set; }
    public Dictionary<string, string> Name { get; set; }
    public decimal Price { get; set; }
    public List<Guid> CategoryIds { get; set; }
    public Guid? BrandId { get; set; }
}
```

---

## 🗄️ Datenbank-Indizes

Optimierte Indizes für häufige Anfragen:

```
Products
├── Sku (UNIQUE)
├── Slug (UNIQUE)
├── TenantId
├── IsActive
└── CreatedAt

Categories
├── Slug (UNIQUE)
├── TenantId
├── IsActive
└── ParentCategoryId

ProductCategory
├── (ProductId, CategoryId) - PRIMARY KEY
├── ProductId
├── CategoryId
└── IsPrimary

ProductAttributeValue
├── (ProductId, AttributeId) - UNIQUE
├── ProductId
└── AttributeId
```

---

## 🚀 Verwendungsbeispiele

### Produkt mit allen Details erstellen
```csharp
var createDto = new CreateProductDto
{
    Sku = "LAPTOP-GAMING-001",
    Slug = "gaming-laptop-pro",
    Name = new Dictionary<string, string>
    {
        { "en", "Gaming Laptop Pro" },
        { "de", "Gaming-Laptop Pro" },
        { "fr", "Ordinateur de jeu Pro" }
    },
    ShortDescription = new Dictionary<string, string>
    {
        { "en", "High-performance gaming laptop" },
        { "de", "Hochleistungs-Gaming-Laptop" }
    },
    Price = 1299.99m,
    SpecialPrice = 999.99m,
    StockQuantity = 50,
    BrandId = brandId,
    CategoryIds = new List<Guid> { electronicsCategory, laptopsCategory }
};

var product = await _productService.CreateProductAsync(createDto);
```

### Produkt mit Varianten abrufen
```csharp
var product = await _productService.GetProductAsync(productId);

foreach (var variant in product.Variants)
{
    Console.WriteLine($"Variant: {variant.Name["de"]}"); // "Gaming-Laptop Pro - Rot"
    Console.WriteLine($"Price: {variant.Price}");
    Console.WriteLine($"Stock: {variant.StockQuantity}");
}
```

### Kategorien hierarchisch abrufen
```csharp
var hierarchy = await _categoryService.GetCategoryHierarchyAsync();

foreach (var category in hierarchy)
{
    Console.WriteLine($"Category: {category.Name["de"]}");
    var children = await _categoryService.GetChildCategoriesAsync(category.Id);
    foreach (var child in children)
    {
        Console.WriteLine($"  └─ {child.Name["de"]}");
    }
}
```

### Produkte suchen
```csharp
// Nach SKU
var product = await _productService.GetProductBySkuAsync("LAPTOP-001");

// Nach Slug (z.B. aus URL)
var productByUrl = await _productService.GetProductBySlugAsync("gaming-laptop-pro");

// Volltextsuche
var results = await _productService.SearchProductsAsync("gaming");

// Kategoriebasiert
var electronicsProducts = await _productService.GetProductsByCategoryAsync(electronicsId);

// Mit Paginierung
var (items, total) = await _productService.GetProductsPagedAsync(pageNumber: 1, pageSize: 20);
```

---

## 🔄 Multi-Tenancy

Alle Entitäten unterstützen Multi-Tenancy durch `TenantId`:

```csharp
public class Product
{
    // ...
    public Guid? TenantId { get; set; }  // Isolierung pro Mandant
}
```

**In Repository-Queries automatisch filtern:**
```csharp
var query = _dbSet.Where(p => p.IsActive && p.TenantId == tenantId);
```

---

## 📝 Audit-Trail

Alle Entitäten speichern:
```csharp
public DateTime CreatedAt { get; set; }        // Erstellungszeit
public string? CreatedBy { get; set; }         // Ersteller-ID
public DateTime UpdatedAt { get; set; }        // Änderungszeit
public string? UpdatedBy { get; set; }         // Bearbeiter-ID
```

---

## ✅ Implementierungs-Checkliste

- [x] Model-Klassen für alle Entitäten
- [x] CatalogDbContext mit JSONB-Konvertierung
- [x] Repository-Pattern (generisch + spezifisch)
- [x] Service-Layer mit DTOs
- [x] REST-Controller
- [x] Mehrsprachige Unterstützung
- [ ] **Program.cs** - Service-Registrierung (zu erstellen)
- [ ] **appsettings.json** - Konfiguration (zu erstellen)
- [ ] **Datenbank-Migrations** (zu erstellen)
- [ ] Unit Tests
- [ ] Integration Tests
- [ ] API-Dokumentation (Swagger)

---

## 🔌 Integration mit bestehenden Services

### LocalizationService
- `LocalizedContent` Klasse aus `backend/shared/types/`
- Automatisches Fallback auf Standard-Sprache
- JSON-Serialisierung für Datenbank

### LayoutService
- Ähnliches Pattern für JSONB-Eigenschaften
- Gleicher DbContext-Aufbau

### TenantService
- Automatische Mandanten-Isolierung
- TenantId in allen Queries

---

## 📚 Nächste Schritte

1. **Program.cs erstellen** - Dependency Injection setup
2. **appsettings.json** - Datenbankverbindung konfigurieren
3. **EF Core Migrations** - Datenbanktabellen erstellen
4. **Swagger/OpenAPI** - API-Dokumentation
5. **Unit Tests** - Repository/Service Tests
6. **Frontend Integration** - API-Consumer implementieren

---

*Implementiert: 25. Dezember 2025*
*Services: 9 Dateien Models | 10 Dateien Repositories | 6 Dateien Services | 3 Dateien Controllers*
