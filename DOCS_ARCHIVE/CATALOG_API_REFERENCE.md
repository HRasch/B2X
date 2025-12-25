# B2Connect Katalog-Service - API Quick Reference

## 🔗 BASE URL
```
https://localhost:5009/api
```

---

## 📦 PRODUCTS API

### GET /products
**Alle Produkte abrufen**
```bash
curl -X GET https://localhost:5009/api/products
```

**Response (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "sku": "LAPTOP-001",
    "slug": "gaming-laptop",
    "name": {
      "en": "Gaming Laptop",
      "de": "Gaming-Laptop"
    },
    "price": 1299.99,
    "specialPrice": 999.99,
    "stockQuantity": 50,
    "isActive": true,
    "brandId": "550e8400-e29b-41d4-a716-446655440001",
    "brandName": "TechCorp",
    "variants": 3,
    "imageCount": 5
  }
]
```

---

### GET /products/{id}
**Produkt nach ID abrufen**
```bash
curl -X GET https://localhost:5009/api/products/550e8400-e29b-41d4-a716-446655440000
```

---

### GET /products/sku/{sku}
**Produkt nach SKU abrufen**
```bash
curl -X GET https://localhost:5009/api/products/sku/LAPTOP-001
```

---

### GET /products/slug/{slug}
**Produkt nach Slug abrufen (z.B. aus URL)**
```bash
curl -X GET https://localhost:5009/api/products/slug/gaming-laptop
```

---

### GET /products/paged
**Produkte mit Pagination**
```bash
curl -X GET "https://localhost:5009/api/products/paged?pageNumber=1&pageSize=10"
```

**Response:**
```json
{
  "items": [...],
  "total": 250,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

### GET /products/category/{categoryId}
**Alle Produkte einer Kategorie**
```bash
curl -X GET https://localhost:5009/api/products/category/550e8400-e29b-41d4-a716-446655440002
```

---

### GET /products/brand/{brandId}
**Alle Produkte einer Marke**
```bash
curl -X GET https://localhost:5009/api/products/brand/550e8400-e29b-41d4-a716-446655440001
```

---

### GET /products/featured
**Hervorgehobene Produkte**
```bash
curl -X GET "https://localhost:5009/api/products/featured?take=10"
```

---

### GET /products/new
**Neue Produkte**
```bash
curl -X GET "https://localhost:5009/api/products/new?take=10"
```

---

### GET /products/search
**Produkte suchen**
```bash
curl -X GET "https://localhost:5009/api/products/search?q=gaming"
```

---

### POST /products
**Neues Produkt erstellen**
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
    "description": {
      "en": "A powerful laptop for gaming..."
    },
    "price": 1299.99,
    "specialPrice": 999.99,
    "stockQuantity": 50,
    "brandId": "550e8400-e29b-41d4-a716-446655440001",
    "categoryIds": [
      "550e8400-e29b-41d4-a716-446655440002",
      "550e8400-e29b-41d4-a716-446655440003"
    ]
  }'
```

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "sku": "LAPTOP-001",
  ...
}
```

---

### PUT /products/{id}
**Produkt aktualisieren**
```bash
curl -X PUT https://localhost:5009/api/products/550e8400-e29b-41d4-a716-446655440000 \
  -H "Content-Type: application/json" \
  -d '{
    "name": {
      "en": "Updated Gaming Laptop",
      "de": "Aktualisierter Gaming-Laptop"
    },
    "price": 1199.99,
    "isActive": true,
    "isFeatured": true
  }'
```

---

### DELETE /products/{id}
**Produkt löschen**
```bash
curl -X DELETE https://localhost:5009/api/products/550e8400-e29b-41d4-a716-446655440000
```

**Response (204 No Content)**

---

## 🏷️ CATEGORIES API

### GET /categories
**Alle aktiven Kategorien**
```bash
curl -X GET https://localhost:5009/api/categories
```

---

### GET /categories/root
**Nur Root-Kategorien (ohne Parent)**
```bash
curl -X GET https://localhost:5009/api/categories/root
```

---

### GET /categories/hierarchy
**Komplette Kategorien-Hierarchie**
```bash
curl -X GET https://localhost:5009/api/categories/hierarchy
```

---

### GET /categories/{id}/children
**Subcategories**
```bash
curl -X GET https://localhost:5009/api/categories/550e8400-e29b-41d4-a716-446655440002/children
```

---

### GET /categories/{id}
**Kategorie nach ID**
```bash
curl -X GET https://localhost:5009/api/categories/550e8400-e29b-41d4-a716-446655440002
```

---

### GET /categories/slug/{slug}
**Kategorie nach Slug**
```bash
curl -X GET https://localhost:5009/api/categories/slug/electronics
```

---

### POST /categories
**Neue Kategorie erstellen**
```bash
curl -X POST https://localhost:5009/api/categories \
  -H "Content-Type: application/json" \
  -d '{
    "slug": "laptops",
    "name": {
      "en": "Laptops",
      "de": "Laptops"
    },
    "description": {
      "en": "All laptops and notebooks"
    },
    "parentCategoryId": "550e8400-e29b-41d4-a716-446655440002",
    "isActive": true
  }'
```

---

### PUT /categories/{id}
**Kategorie aktualisieren**
```bash
curl -X PUT https://localhost:5009/api/categories/550e8400-e29b-41d4-a716-446655440002 \
  -H "Content-Type: application/json" \
  -d '{
    "name": {
      "en": "Updated Laptops",
      "de": "Aktualisierte Laptops"
    },
    "isActive": true
  }'
```

---

### DELETE /categories/{id}
**Kategorie löschen**
```bash
curl -X DELETE https://localhost:5009/api/categories/550e8400-e29b-41d4-a716-446655440002
```

---

## 🏢 BRANDS API

### GET /brands
**Alle aktiven Marken**
```bash
curl -X GET https://localhost:5009/api/brands
```

---

### GET /brands/paged
**Marken mit Pagination**
```bash
curl -X GET "https://localhost:5009/api/brands/paged?pageNumber=1&pageSize=10"
```

---

### GET /brands/{id}
**Marke nach ID**
```bash
curl -X GET https://localhost:5009/api/brands/550e8400-e29b-41d4-a716-446655440001
```

---

### GET /brands/slug/{slug}
**Marke nach Slug**
```bash
curl -X GET https://localhost:5009/api/brands/slug/techcorp
```

---

### POST /brands
**Neue Marke erstellen**
```bash
curl -X POST https://localhost:5009/api/brands \
  -H "Content-Type: application/json" \
  -d '{
    "slug": "techcorp",
    "name": {
      "en": "TechCorp",
      "de": "TechCorp",
      "fr": "TechCorp"
    },
    "description": {
      "en": "Leading technology brand"
    },
    "logoUrl": "https://example.com/logo.png",
    "websiteUrl": "https://techcorp.com",
    "isActive": true
  }'
```

---

### PUT /brands/{id}
**Marke aktualisieren**
```bash
curl -X PUT https://localhost:5009/api/brands/550e8400-e29b-41d4-a716-446655440001 \
  -H "Content-Type: application/json" \
  -d '{
    "name": {
      "en": "Updated TechCorp"
    },
    "logoUrl": "https://example.com/new-logo.png"
  }'
```

---

### DELETE /brands/{id}
**Marke löschen**
```bash
curl -X DELETE https://localhost:5009/api/brands/550e8400-e29b-41d4-a716-446655440001
```

---

## 📊 DATA FORMATS

### LocalizedContent (Mehrsprachigkeit)
```json
{
  "en": "English Text",
  "de": "Deutsche Text",
  "fr": "Texte français",
  "es": "Texto español"
}
```

### ProductVariant Structure
```json
{
  "id": "uuid",
  "sku": "PRODUCT-001-RED",
  "name": {
    "en": "Red Version",
    "de": "Rote Version"
  },
  "price": 99.99,
  "stockQuantity": 25,
  "isActive": true,
  "attributeValues": {
    "color": "Red",
    "size": "Large"
  }
}
```

### Category Structure
```json
{
  "id": "uuid",
  "slug": "electronics",
  "name": {
    "en": "Electronics",
    "de": "Elektronik"
  },
  "description": {
    "en": "All electronic devices",
    "de": "Alle elektronischen Geräte"
  },
  "productCount": 125,
  "isActive": true
}
```

---

## ⚠️ ERROR RESPONSES

### 400 Bad Request
```json
{
  "error": "Bad request",
  "message": "Invalid input parameters"
}
```

### 404 Not Found
```json
{
  "error": "Not found",
  "message": "Product not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "An internal server error occurred",
  "message": "Error details (development only)",
  "traceId": "0HN1GCS7HVC23:00000001"
}
```

---

## 🔑 COMMON QUERY PARAMETERS

| Parameter | Type | Example | Description |
|-----------|------|---------|-------------|
| `pageNumber` | int | `1` | Seite (ab 1) |
| `pageSize` | int | `10` | Einträge pro Seite |
| `q` | string | `gaming` | Suchbegriff |
| `take` | int | `10` | Maximale Anzahl |
| `skip` | int | `20` | Überspringen |

---

## 🌍 Sprachen-Codes

| Code | Sprache |
|------|---------|
| `en` | English |
| `de` | Deutsch |
| `fr` | Français |
| `es` | Español |
| `it` | Italiano |
| `nl` | Nederlands |

---

## 📝 Response Status Codes

| Code | Bedeutung |
|------|-----------|
| 200 | OK - Erfolgreich |
| 201 | Created - Ressource erstellt |
| 204 | No Content - Erfolgreich, keine Response |
| 400 | Bad Request - Ungültige Parameter |
| 404 | Not Found - Nicht gefunden |
| 500 | Internal Server Error - Serverfehler |

---

## 🧪 Beispiel-Workflow

### 1. Marke erstellen
```bash
BRAND_ID=$(curl -s -X POST https://localhost:5009/api/brands \
  -H "Content-Type: application/json" \
  -d '{"slug":"acme","name":{"en":"ACME Corp"}}' \
  | jq -r '.id')
```

### 2. Kategorie erstellen
```bash
CATEGORY_ID=$(curl -s -X POST https://localhost:5009/api/categories \
  -H "Content-Type: application/json" \
  -d '{"slug":"tools","name":{"en":"Tools"}}' \
  | jq -r '.id')
```

### 3. Produkt erstellen
```bash
curl -X POST https://localhost:5009/api/products \
  -H "Content-Type: application/json" \
  -d "{
    \"sku\":\"TOOL-001\",
    \"slug\":\"hammer\",
    \"name\":{\"en\":\"Hammer\"},
    \"price\":29.99,
    \"stockQuantity\":100,
    \"brandId\":\"$BRAND_ID\",
    \"categoryIds\":[\"$CATEGORY_ID\"]
  }"
```

### 4. Produkt abrufen
```bash
curl https://localhost:5009/api/products/slug/hammer | jq
```

---

*Letztes Update: 25. Dezember 2025*
