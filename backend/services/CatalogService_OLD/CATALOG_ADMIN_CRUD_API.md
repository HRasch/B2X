# B2Connect Catalog Service - Admin CRUD API

## Overview

The Catalog Service includes **Admin-only CRUD endpoints** for managing the product catalog. These endpoints are:
- **Secured** with JWT Bearer token authentication
- **Role-protected** - require "Admin" role
- **Separate** from public API endpoints
- **Not accessible** from Store Frontend

## Admin Endpoints

All admin endpoints are under `/api/admin/` prefix and require `Authorization: Bearer {token}` header with Admin role.

### Products Administration

#### Get All Products (Admin)
```http
GET /api/admin/products
Authorization: Bearer {token}
```

**Response:** `200 OK`
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "sku": "SKU-0001",
    "slug": "gaming-laptop-asus",
    "name": {
      "en": "Gaming Laptop ASUS ROG",
      "de": "Gaming Laptop ASUS ROG",
      "fr": "Gaming Laptop ASUS ROG"
    },
    "price": 1599.99,
    "stockQuantity": 45,
    "isActive": true,
    "isFeatured": false
  }
]
```

#### Get Product by ID
```http
GET /api/admin/products/{id}
Authorization: Bearer {token}
```

**Parameters:**
- `id` (guid, required) - Product ID

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "sku": "SKU-0001",
  "name": { ... },
  "description": { ... },
  "price": 1599.99,
  "stockQuantity": 45,
  "isActive": true,
  "variants": [ ... ],
  "images": [ ... ],
  "documents": [ ... ]
}
```

#### Create Product
```http
POST /api/admin/products
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "sku": "SKU-NEW-001",
  "slug": "new-product-slug",
  "name": {
    "en": "New Product",
    "de": "Neues Produkt",
    "fr": "Nouveau Produit"
  },
  "shortDescription": {
    "en": "Short description",
    "de": "Kurzbeschreibung",
    "fr": "Courte description"
  },
  "description": {
    "en": "Full description...",
    "de": "Vollständige Beschreibung...",
    "fr": "Description complète..."
  },
  "price": 999.99,
  "specialPrice": null,
  "costPrice": 500.00,
  "stockQuantity": 100,
  "lowStockThreshold": 10,
  "isStockTracked": true,
  "isAvailable": true,
  "isActive": true,
  "isFeatured": false,
  "isNew": true,
  "brandId": "550e8400-e29b-41d4-a716-446655440001",
  "weight": 1.5,
  "weightUnit": "kg"
}
```

**Response:** `201 Created`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "sku": "SKU-NEW-001",
  "name": { ... },
  "price": 999.99,
  "isActive": true
}
```

#### Update Product
```http
PUT /api/admin/products/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

**Parameters:**
- `id` (guid, required) - Product ID

**Request Body:** Same as Create Product

**Response:** `200 OK` - Updated product

#### Delete Product
```http
DELETE /api/admin/products/{id}
Authorization: Bearer {token}
```

**Parameters:**
- `id` (guid, required) - Product ID

**Response:** `204 No Content`

#### Get Products (Paginated)
```http
GET /api/admin/products/paged?page=1&pageSize=20
Authorization: Bearer {token}
```

**Query Parameters:**
- `page` (int, default: 1) - Page number (1-indexed)
- `pageSize` (int, default: 20) - Items per page (max: 100)

**Response:** `200 OK`
```json
{
  "items": [ ... ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8
}
```

#### Batch Update Product Status
```http
PATCH /api/admin/products/batch/status
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "productIds": [
    "550e8400-e29b-41d4-a716-446655440000",
    "550e8400-e29b-41d4-a716-446655440001"
  ],
  "isActive": true
}
```

**Response:** `200 OK`
```json
{
  "message": "Updated 2 products"
}
```

### Categories Administration

#### Get All Categories (Admin)
```http
GET /api/admin/categories
Authorization: Bearer {token}
```

#### Get Category by ID
```http
GET /api/admin/categories/{id}
Authorization: Bearer {token}
```

#### Create Category
```http
POST /api/admin/categories
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "slug": "new-category",
  "name": {
    "en": "New Category",
    "de": "Neue Kategorie",
    "fr": "Nouvelle Catégorie"
  },
  "description": {
    "en": "Category description",
    "de": "Kategoriebeschreibung",
    "fr": "Description de la catégorie"
  },
  "parentCategoryId": null,
  "displayOrder": 1,
  "isActive": true,
  "imageUrl": "https://example.com/category.jpg",
  "imageAltText": "Category image"
}
```

#### Update Category
```http
PUT /api/admin/categories/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

#### Delete Category
```http
DELETE /api/admin/categories/{id}
Authorization: Bearer {token}
```

#### Get Category Hierarchy
```http
GET /api/admin/categories/hierarchy/full
Authorization: Bearer {token}
```

**Response:** `200 OK`
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440010",
    "name": { "en": "Electronics", ... },
    "slug": "electronics",
    "isActive": true,
    "children": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440011",
        "name": { "en": "Computers", ... },
        "slug": "computers",
        "children": [ ... ]
      }
    ]
  }
]
```

#### Get Child Categories
```http
GET /api/admin/categories/{parentId}/children
Authorization: Bearer {token}
```

#### Batch Update Category Status
```http
PATCH /api/admin/categories/batch/status
Authorization: Bearer {token}
Content-Type: application/json
```

### Brands Administration

#### Get All Brands (Admin)
```http
GET /api/admin/brands
Authorization: Bearer {token}
```

#### Get Brand by ID
```http
GET /api/admin/brands/{id}
Authorization: Bearer {token}
```

#### Create Brand
```http
POST /api/admin/brands
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "slug": "new-brand",
  "name": {
    "en": "New Brand",
    "de": "Neue Marke",
    "fr": "Nouvelle Marque"
  },
  "description": {
    "en": "Brand description",
    "de": "Markendesbreibung",
    "fr": "Description de la marque"
  },
  "logoUrl": "https://example.com/logo.png",
  "websiteUrl": "https://brand.example.com",
  "displayOrder": 1,
  "isActive": true
}
```

#### Update Brand
```http
PUT /api/admin/brands/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

#### Delete Brand
```http
DELETE /api/admin/brands/{id}
Authorization: Bearer {token}
```

#### Get Brands (Paginated)
```http
GET /api/admin/brands/paged?page=1&pageSize=20
Authorization: Bearer {token}
```

#### Batch Update Brand Status
```http
PATCH /api/admin/brands/batch/status
Authorization: Bearer {token}
Content-Type: application/json
```

### Product Attributes Administration

#### Get All Attributes
```http
GET /api/admin/attributes
Authorization: Bearer {token}
```

#### Get Attribute by ID
```http
GET /api/admin/attributes/{id}
Authorization: Bearer {token}
```

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440020",
  "code": "color",
  "name": {
    "en": "Color",
    "de": "Farbe",
    "fr": "Couleur"
  },
  "attributeType": "select",
  "isSearchable": true,
  "isFilterable": true,
  "displayOrder": 1,
  "isActive": true,
  "options": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440021",
      "value": {
        "en": "Red",
        "de": "Rot",
        "fr": "Rouge"
      },
      "colorValue": "#FF0000",
      "displayOrder": 0,
      "isActive": true
    }
  ]
}
```

#### Create Attribute
```http
POST /api/admin/attributes
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "code": "size",
  "name": {
    "en": "Size",
    "de": "Größe",
    "fr": "Taille"
  },
  "attributeType": "select",
  "isSearchable": true,
  "isFilterable": true,
  "displayOrder": 0,
  "options": [
    {
      "value": {
        "en": "Small",
        "de": "Klein",
        "fr": "Petit"
      },
      "displayOrder": 0
    },
    {
      "value": {
        "en": "Medium",
        "de": "Mittel",
        "fr": "Moyen"
      },
      "displayOrder": 1
    }
  ]
}
```

#### Update Attribute
```http
PUT /api/admin/attributes/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": {
    "en": "Product Size",
    "de": "Produktgröße",
    "fr": "Taille du Produit"
  },
  "isSearchable": true,
  "isFilterable": true,
  "displayOrder": 1,
  "isActive": true
}
```

#### Delete Attribute
```http
DELETE /api/admin/attributes/{id}
Authorization: Bearer {token}
```

#### Add Attribute Option
```http
POST /api/admin/attributes/{attributeId}/options
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "value": {
    "en": "Large",
    "de": "Groß",
    "fr": "Grand"
  },
  "colorValue": null,
  "displayOrder": 2
}
```

#### Update Attribute Option
```http
PUT /api/admin/attributes/{attributeId}/options/{optionId}
Authorization: Bearer {token}
Content-Type: application/json
```

#### Delete Attribute Option
```http
DELETE /api/admin/attributes/{attributeId}/options/{optionId}
Authorization: Bearer {token}
```

## Authentication

### JWT Bearer Token

All admin endpoints require a valid JWT Bearer token in the `Authorization` header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Requirements

The JWT token must contain:
- **Role Claim:** `role: "Admin"` or similar (role claim name configured in Identity Provider)
- **Valid signature:** Must be signed by the authorization server
- **Not expired:** `exp` claim must be in the future

### Swagger Integration

The Swagger UI at `/swagger` includes a "Authorize" button that allows you to:
1. Paste your JWT token
2. Test admin endpoints interactively

## Error Responses

### 401 Unauthorized
```json
{
  "error": "Unauthorized",
  "message": "Missing or invalid authentication token"
}
```

### 403 Forbidden
```json
{
  "error": "Forbidden",
  "message": "User does not have Admin role"
}
```

### 400 Bad Request
```json
{
  "error": "Validation error",
  "message": "Product SKU must be unique"
}
```

### 404 Not Found
```json
{
  "error": "Not found",
  "message": "Product with ID not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "Server error",
  "message": "An unexpected error occurred",
  "traceId": "0HN1GHRQH5V7S:00000001"
}
```

## Examples

### cURL - Create Product
```bash
curl -X POST http://localhost:5008/api/admin/products \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "NEW-001",
    "slug": "new-product",
    "name": {"en": "New Product", "de": "Neues Produkt", "fr": "Nouveau Produit"},
    "price": 99.99,
    "stockQuantity": 50,
    "brandId": "550e8400-e29b-41d4-a716-446655440001",
    "isActive": true
  }'
```

### cURL - Update Product Status
```bash
curl -X PUT http://localhost:5008/api/admin/products/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "SKU-001",
    "slug": "product",
    "name": {"en": "Product"},
    "price": 99.99,
    "isActive": false
  }'
```

### cURL - Batch Update
```bash
curl -X PATCH http://localhost:5008/api/admin/products/batch/status \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "productIds": ["550e8400-e29b-41d4-a716-446655440000", "550e8400-e29b-41d4-a716-446655440001"],
    "isActive": true
  }'
```

## Security Notes

### Public vs Admin Endpoints

| Endpoint | Type | Authentication | Available In |
|----------|------|-----------------|--------------|
| `/api/products` | GET | None | Public, Store Frontend |
| `/api/categories` | GET | None | Public, Store Frontend |
| `/api/brands` | GET | None | Public, Store Frontend |
| `/api/admin/products` | CRUD | JWT + Admin role | Admin only |
| `/api/admin/categories` | CRUD | JWT + Admin role | Admin only |
| `/api/admin/brands` | CRUD | JWT + Admin role | Admin only |
| `/api/admin/attributes` | CRUD | JWT + Admin role | Admin only |

### Access Control

1. **Authentication:** JWT Bearer token required
2. **Authorization:** Admin role required
3. **Logging:** All admin operations are logged with user context
4. **Validation:** All inputs validated server-side
5. **Audit Trail:** Operations tracked for compliance

## Configuration

### appsettings.json

```json
{
  "Auth": {
    "Authority": "https://your-auth-server.example.com",
    "Audience": "catalog-service"
  }
}
```

### JWT Claims

The authorization server must include:
```json
{
  "sub": "user-id",
  "email": "admin@example.com",
  "role": "Admin",
  "exp": 1704067200
}
```

## Related Documentation

- [CATALOG_IMPLEMENTATION_COMPLETE.md](./CATALOG_IMPLEMENTATION_COMPLETE.md)
- [CATALOG_API_REFERENCE.md](./CATALOG_API_REFERENCE.md)
- [CATALOG_QUICK_START.md](./CATALOG_QUICK_START.md)

## Support

For admin API issues:
1. Check authentication token is valid
2. Verify Admin role is present in token
3. Review application logs for detailed errors
4. Check network tab in browser (Swagger UI)
