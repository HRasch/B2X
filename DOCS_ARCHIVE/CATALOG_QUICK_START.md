# B2Connect Katalog-Service - Quick Start Guide

## 🚀 Schnelleinstieg

### 1. Projektstruktur überprüfen

```bash
cd backend/services/CatalogService
ls -la
```

Erwartete Dateien:
- ✅ `Program.cs`
- ✅ `appsettings.json`
- ✅ `appsettings.Development.json`
- ✅ `B2Connect.CatalogService.csproj`
- ✅ `src/Models/` (9 Entity-Dateien)
- ✅ `src/Data/CatalogDbContext.cs`
- ✅ `src/Repositories/` (10 Dateien)
- ✅ `src/Services/` (6 Dateien)
- ✅ `src/Controllers/` (3 Dateien)

### 2. Abhängigkeiten installieren

```bash
cd backend/services/CatalogService
dotnet restore
```

### 3. Datenbank vorbereiten

```bash
# Stelle sicher, dass PostgreSQL läuft
# Standard-Credentials in appsettings.Development.json:
# Host: localhost:5432
# Database: b2connect_catalog_dev
# User: postgres
# Password: postgres

# Datenbank erstellen (falls nicht vorhanden)
createdb -h localhost -U postgres b2connect_catalog_dev
```

### 4. EF Core Migrations erstellen

```bash
cd backend/services/CatalogService

# Migration erstellen
dotnet ef migrations add InitialCatalogCreate

# Migrations durchführen
dotnet ef database update
```

### 5. Service starten

```bash
# Entwicklung
dotnet run --launch-profile "https"

# Oder mit watch-Mode
dotnet watch run
```

Service läuft unter:
- **HTTP**: http://localhost:5008
- **HTTPS**: https://localhost:5009
- **Swagger**: https://localhost:5009/swagger

### 6. API testen

#### Swagger UI
```
https://localhost:5009/swagger
```

#### cURL-Beispiele

**Alle Produkte abrufen:**
```bash
curl -X GET https://localhost:5009/api/products \
  -H "accept: application/json"
```

**Produkt erstellen:**
```bash
curl -X POST https://localhost:5009/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "LAPTOP-001",
    "slug": "gaming-laptop",
    "name": {
      "en": "Gaming Laptop",
      "de": "Gaming-Laptop",
      "fr": "Ordinateur de jeu"
    },
    "shortDescription": {
      "en": "High-performance gaming laptop"
    },
    "price": 1299.99,
    "specialPrice": 999.99,
    "stockQuantity": 50,
    "categoryIds": ["00000000-0000-0000-0000-000000000001"]
  }'
```

**Kategorien abrufen:**
```bash
curl -X GET https://localhost:5009/api/categories \
  -H "accept: application/json"
```

**Marke erstellen:**
```bash
curl -X POST https://localhost:5009/api/brands \
  -H "Content-Type: application/json" \
  -d '{
    "slug": "techcorp",
    "name": {
      "en": "TechCorp",
      "de": "TechCorp"
    },
    "logoUrl": "https://example.com/logo.png"
  }'
```

---

## 📊 Datenbank-Schema

Nach `dotnet ef database update` werden diese Tabellen erstellt:

```
┌─ catalog_products
│  ├── id (UUID)
│  ├── sku (VARCHAR UNIQUE)
│  ├── slug (VARCHAR UNIQUE)
│  ├── name (JSONB) - {"en": "...", "de": "...", "fr": "..."}
│  ├── description (JSONB)
│  ├── price (DECIMAL)
│  ├── special_price (DECIMAL)
│  ├── stock_quantity (INTEGER)
│  ├── is_active (BOOLEAN)
│  ├── brand_id (UUID FK)
│  ├── created_at (TIMESTAMP)
│  └── updated_at (TIMESTAMP)
│
├─ catalog_categories
│  ├── id (UUID)
│  ├── slug (VARCHAR UNIQUE)
│  ├── name (JSONB)
│  ├── parent_category_id (UUID FK)
│  ├── is_active (BOOLEAN)
│  └── ...
│
├─ catalog_brands
│  ├── id (UUID)
│  ├── slug (VARCHAR UNIQUE)
│  ├── name (JSONB)
│  ├── logo_url (VARCHAR)
│  ├── is_active (BOOLEAN)
│  └── ...
│
├─ catalog_product_variants
│  ├── id (UUID)
│  ├── product_id (UUID FK)
│  ├── sku (VARCHAR UNIQUE)
│  ├── name (JSONB)
│  ├── price (DECIMAL)
│  ├── stock_quantity (INTEGER)
│  └── ...
│
├─ catalog_product_attributes
│  ├── id (UUID)
│  ├── code (VARCHAR UNIQUE)
│  ├── name (JSONB)
│  ├── attribute_type (VARCHAR)
│  ├── is_searchable (BOOLEAN)
│  └── ...
│
├─ catalog_product_images
│  ├── id (UUID)
│  ├── product_id (UUID FK)
│  ├── url (VARCHAR)
│  ├── thumbnail_url (VARCHAR)
│  ├── is_primary (BOOLEAN)
│  └── ...
│
├─ catalog_product_documents
│  ├── id (UUID)
│  ├── product_id (UUID FK)
│  ├── name (JSONB)
│  ├── document_type (VARCHAR)
│  ├── url (VARCHAR)
│  └── ...
│
└─ catalog_product_categories (Junction)
   ├── product_id (UUID FK)
   ├── category_id (UUID FK)
   └── is_primary (BOOLEAN)
```

---

## 🧪 Integration mit anderen Services

### Mit LocalizationService
```csharp
// Automatisches Fallback auf Standard-Sprache
var germanName = product.Name.Get("de");
```

### Mit LayoutService
- Ähnliches Muster für JSONB-Eigenschaften
- Gleiche DbContext-Konfiguration

### Mit AuthService
- JWT-Token für geschützte Endpoints (zu implementieren)

### Mit TenantService
- Automatische Mandanten-Isolierung über TenantId-Filter

---

## 📝 Häufige Aufgaben

### Produktvariante hinzufügen

```csharp
var variant = new ProductVariant
{
    ProductId = productId,
    Sku = "LAPTOP-001-RED-L",
    Name = new LocalizedContent()
        .Set("en", "Red, Size L")
        .Set("de", "Rot, Größe L"),
    Price = 1099.99m,
    StockQuantity = 10,
    IsActive = true
};

product.Variants.Add(variant);
await dbContext.SaveChangesAsync();
```

### Produktattribute definieren

```csharp
var colorAttribute = new ProductAttribute
{
    Code = "color",
    Name = new LocalizedContent()
        .Set("en", "Color")
        .Set("de", "Farbe"),
    AttributeType = "select",
    IsSearchable = true,
    IsFilterable = true
};

var redOption = new ProductAttributeOption
{
    Code = "red",
    Label = new LocalizedContent()
        .Set("en", "Red")
        .Set("de", "Rot"),
    ColorValue = "#FF0000"
};

colorAttribute.Options.Add(redOption);
```

### Produkt mit Kategorie verknüpfen

```csharp
var productCategory = new ProductCategory
{
    ProductId = productId,
    CategoryId = categoryId,
    IsPrimary = true,
    DisplayOrder = 1
};

await dbContext.ProductCategories.AddAsync(productCategory);
await dbContext.SaveChangesAsync();
```

---

## 🔍 Query-Beispiele

### Produkte in Kategorie filtern
```csharp
var products = await _productRepository.GetByCategoryAsync(categoryId);
```

### Mit Pagination
```csharp
var (items, total) = await _productRepository.GetPagedAsync(pageNumber: 1, pageSize: 20);
```

### Featured Products
```csharp
var featured = await _productRepository.GetFeaturedAsync(take: 10);
```

### Suche
```csharp
var results = await _productRepository.SearchAsync("gaming");
```

### Mit allen Details laden
```csharp
var product = await _productRepository.GetWithDetailsAsync(productId);
// Includes: Brand, Categories, Variants, Images, Documents, Attributes
```

---

## ⚠️ Troubleshooting

### "No database provider has been configured for this DbContext"

**Lösung**: Stelle sicher, dass `Program.cs` korrekt ist und `appsettings.json` eine gültige Verbindungszeichenfolge hat.

### Migration fehlgeschlagen

```bash
# Migrations zurücksetzen
dotnet ef database update 0

# Migrationen löschen
rm -rf Migrations

# Neu erstellen
dotnet ef migrations add InitialCatalogCreate
dotnet ef database update
```

### Port bereits in Verwendung

```bash
# Ändere den Port in appsettings.Development.json
# Oder:
dotnet run --urls "https://localhost:6009;http://localhost:6008"
```

### "System.NullReferenceException" bei DbContext

- Stelle sicher, dass alle Navigation Properties initialisiert sind
- Verwende `.Include()` für related data

---

## 📚 Dokumentation

- [CATALOG_IMPLEMENTATION.md](../../../CATALOG_IMPLEMENTATION.md) - Detaillierte Dokuentation
- [Swagger API Docs](https://localhost:5009/swagger) - Interactive API Explorer
- [Entity Localization Guide](../../../backend/docs/ENTITY_LOCALIZATION_GUIDE.md) - Mehrsprachigkeit

---

## 🚀 Nächste Schritte

1. **Frontend Integration** - React/Vue Komponenten für Produktliste
2. **Search Index** - Elasticsearch Integration
3. **Image Processing** - CDN und Thumbnails
4. **Reviews & Ratings** - Kundenbewertungen
5. **Inventory Management** - Lagerbestands-API
6. **Product Recommendations** - AI-basierte Vorschläge

---

*Letzte Aktualisierung: 25. Dezember 2025*
