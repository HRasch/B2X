# B2Connect API Reference

> **Version**: 1.0  
> **Base URL**: `https://api.b2connect.de`  
> **Authentication**: Bearer Token (JWT)

---

## Overview

The B2Connect API follows RESTful conventions and uses JSON for request/response bodies. All endpoints require authentication unless specified otherwise.

## Quick Links

| Section | Description |
|---------|-------------|
| [Authentication](#authentication) | Login, Logout, Token Refresh |
| [Products](#products) | Product catalog management |
| [Categories](#categories) | Category hierarchy |
| [Brands](#brands) | Brand management |
| [Users](#users) | User administration |

---

## Authentication

All API requests require a Bearer token in the Authorization header:

```http
Authorization: Bearer <access_token>
```

### Multi-Tenant Support

For Admin API endpoints, include the Tenant ID header:

```http
X-Tenant-ID: <tenant-uuid>
```

---

### POST /api/auth/login

Authenticate a user and receive access/refresh tokens.

**Request**

```http
POST /api/auth/login
Content-Type: application/json
```

```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Response** `200 OK`

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2...",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

**Errors**

| Status | Description |
|--------|-------------|
| `400` | Invalid request body |
| `401` | Invalid credentials |
| `423` | Account locked |

---

### POST /api/auth/refresh

Refresh an expired access token.

**Request**

```http
POST /api/auth/refresh
Content-Type: application/json
```

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2..."
}
```

**Response** `200 OK`

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "bmV3IHJlZnJlc2ggdG9rZW4...",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

---

### POST /api/auth/logout

Invalidate the current session.

**Request**

```http
POST /api/auth/logout
Authorization: Bearer <access_token>
```

**Response** `204 No Content`

---

### GET /api/auth/me

Get the current authenticated user's profile.

**Request**

```http
GET /api/auth/me
Authorization: Bearer <access_token>
```

**Response** `200 OK`

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "firstName": "Max",
  "lastName": "Mustermann",
  "roles": ["Admin", "ProductManager"],
  "tenantId": "tenant-uuid"
}
```

---

## Products

### GET /api/products

Get all products.

**Request**

```http
GET /api/products
Authorization: Bearer <access_token>
```

**Response** `200 OK`

```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "name": "Dell XPS 15 Laptop",
      "sku": "DELL-XPS-15-2024",
      "slug": "dell-xps-15-laptop",
      "description": "High-performance laptop...",
      "price": 1499.00,
      "currency": "EUR",
      "stock": 50,
      "isActive": true,
      "categoryId": "category-uuid",
      "brandId": "brand-uuid",
      "images": ["https://..."],
      "createdAt": "2025-01-15T10:30:00Z",
      "updatedAt": "2025-01-15T10:30:00Z"
    }
  ],
  "success": true
}
```

---

### GET /api/products/paged

Get products with pagination.

**Query Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `pageNumber` | int | 1 | Page number (1-based) |
| `pageSize` | int | 10 | Items per page (max 100) |

**Request**

```http
GET /api/products/paged?pageNumber=1&pageSize=20
Authorization: Bearer <access_token>
```

**Response** `200 OK`

```json
{
  "data": {
    "items": [...],
    "total": 150,
    "pageNumber": 1,
    "pageSize": 20
  },
  "success": true
}
```

---

### GET /api/products/{id}

Get a product by ID.

**Request**

```http
GET /api/products/550e8400-e29b-41d4-a716-446655440001
Authorization: Bearer <access_token>
```

**Response** `200 OK`

```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "Dell XPS 15 Laptop",
    "sku": "DELL-XPS-15-2024",
    ...
  },
  "success": true
}
```

**Errors**

| Status | Description |
|--------|-------------|
| `404` | Product not found |

---

### GET /api/products/sku/{sku}

Get a product by SKU.

**Request**

```http
GET /api/products/sku/DELL-XPS-15-2024
Authorization: Bearer <access_token>
```

---

### GET /api/products/slug/{slug}

Get a product by URL slug.

**Request**

```http
GET /api/products/slug/dell-xps-15-laptop
Authorization: Bearer <access_token>
```

---

### GET /api/products/category/{categoryId}

Get products by category.

**Request**

```http
GET /api/products/category/550e8400-e29b-41d4-a716-446655440010
Authorization: Bearer <access_token>
```

---

### GET /api/products/brand/{brandId}

Get products by brand.

**Request**

```http
GET /api/products/brand/550e8400-e29b-41d4-a716-446655440020
Authorization: Bearer <access_token>
```

---

### GET /api/products/featured

Get featured products.

**Query Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `take` | int | 10 | Number of products to return |

**Request**

```http
GET /api/products/featured?take=5
Authorization: Bearer <access_token>
```

---

### GET /api/products/new

Get newest products.

**Query Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `take` | int | 10 | Number of products to return |

**Request**

```http
GET /api/products/new?take=10
Authorization: Bearer <access_token>
```

---

### GET /api/products/search

Search products.

**Query Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `q` | string | required | Search term |
| `pageNumber` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |

**Request**

```http
GET /api/products/search?q=laptop&pageNumber=1&pageSize=20
Authorization: Bearer <access_token>
```

---

## Categories

### GET /api/categories

Get all active categories.

**Request**

```http
GET /api/categories
Authorization: Bearer <access_token>
```

**Response** `200 OK`

```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440010",
      "name": "Electronics",
      "slug": "electronics",
      "description": "Electronic devices and accessories",
      "parentId": null,
      "isActive": true
    }
  ],
  "success": true
}
```

---

### GET /api/categories/{id}

Get a category by ID.

**Request**

```http
GET /api/categories/550e8400-e29b-41d4-a716-446655440010
Authorization: Bearer <access_token>
```

---

### GET /api/categories/slug/{slug}

Get a category by URL slug.

**Request**

```http
GET /api/categories/slug/electronics
Authorization: Bearer <access_token>
```

---

### GET /api/categories/root

Get root categories (no parent).

**Request**

```http
GET /api/categories/root
Authorization: Bearer <access_token>
```

---

### GET /api/categories/{parentId}/children

Get child categories of a parent.

**Request**

```http
GET /api/categories/550e8400-e29b-41d4-a716-446655440010/children
Authorization: Bearer <access_token>
```

---

### GET /api/categories/hierarchy

Get the complete category tree.

**Request**

```http
GET /api/categories/hierarchy
Authorization: Bearer <access_token>
```

**Response** `200 OK`

```json
{
  "data": [
    {
      "id": "...",
      "name": "Electronics",
      "slug": "electronics",
      "children": [
        {
          "id": "...",
          "name": "Laptops",
          "slug": "laptops",
          "children": []
        }
      ]
    }
  ],
  "success": true
}
```

---

### POST /api/categories

Create a new category.

**Authorization**: Admin role required

**Request**

```http
POST /api/categories
Authorization: Bearer <access_token>
Content-Type: application/json
```

```json
{
  "name": "Smartphones",
  "slug": "smartphones",
  "description": "Mobile phones and smartphones",
  "parentId": "550e8400-e29b-41d4-a716-446655440010"
}
```

**Response** `201 Created`

```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440011",
    "name": "Smartphones",
    "slug": "smartphones",
    ...
  },
  "success": true
}
```

---

### PUT /api/categories/{id}

Update a category.

**Authorization**: Admin role required

**Request**

```http
PUT /api/categories/550e8400-e29b-41d4-a716-446655440011
Authorization: Bearer <access_token>
Content-Type: application/json
```

```json
{
  "name": "Smartphones & Tablets",
  "slug": "smartphones-tablets",
  "description": "Mobile devices",
  "parentId": null
}
```

---

### DELETE /api/categories/{id}

Delete a category.

**Authorization**: Admin role required

**Request**

```http
DELETE /api/categories/550e8400-e29b-41d4-a716-446655440011
Authorization: Bearer <access_token>
```

**Response** `204 No Content`

**Errors**

| Status | Description |
|--------|-------------|
| `404` | Category not found |
| `409` | Category has products or children |

---

## Users (Admin API)

All User endpoints require:
- `Authorization: Bearer <token>`
- `X-Tenant-ID: <tenant-uuid>`

### GET /api/admin/users

Get all users for the tenant.

**Request**

```http
GET /api/admin/users
Authorization: Bearer <access_token>
X-Tenant-ID: <tenant-uuid>
```

**Response** `200 OK`

```json
{
  "data": {
    "users": [
      {
        "id": "user-uuid",
        "email": "user@example.com",
        "firstName": "Max",
        "lastName": "Mustermann",
        "isActive": true,
        "roles": ["Admin"],
        "createdAt": "2025-01-15T10:30:00Z"
      }
    ],
    "total": 25
  },
  "success": true,
  "message": "Users retrieved successfully"
}
```

---

### GET /api/admin/users/{userId}

Get a specific user.

**Request**

```http
GET /api/admin/users/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <access_token>
X-Tenant-ID: <tenant-uuid>
```

---

### POST /api/admin/users

Create a new user.

**Request**

```http
POST /api/admin/users
Authorization: Bearer <access_token>
X-Tenant-ID: <tenant-uuid>
Content-Type: application/json
```

```json
{
  "email": "newuser@example.com",
  "firstName": "Max",
  "lastName": "Mustermann",
  "password": "securePassword123",
  "roles": ["ProductManager"]
}
```

**Response** `201 Created`

```json
{
  "data": {
    "id": "new-user-uuid",
    "email": "newuser@example.com",
    ...
  },
  "success": true
}
```

---

### PUT /api/admin/users/{userId}

Update a user.

**Request**

```http
PUT /api/admin/users/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <access_token>
X-Tenant-ID: <tenant-uuid>
Content-Type: application/json
```

```json
{
  "email": "updated@example.com",
  "firstName": "Maximilian",
  "lastName": "Mustermann",
  "isActive": true,
  "roles": ["Admin", "ProductManager"]
}
```

---

### DELETE /api/admin/users/{userId}

Delete a user.

**Request**

```http
DELETE /api/admin/users/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <access_token>
X-Tenant-ID: <tenant-uuid>
```

**Response** `204 No Content`

---

## Error Responses

All error responses follow this format:

```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

### HTTP Status Codes

| Status | Description |
|--------|-------------|
| `200` | Success |
| `201` | Created |
| `204` | No Content (successful delete) |
| `400` | Bad Request (validation error) |
| `401` | Unauthorized (invalid/missing token) |
| `403` | Forbidden (insufficient permissions) |
| `404` | Not Found |
| `409` | Conflict (duplicate, referenced entity) |
| `422` | Unprocessable Entity |
| `500` | Internal Server Error |
| `503` | Service Unavailable |

---

## Rate Limiting

API requests are rate limited:

| Tier | Requests/Minute | Requests/Hour |
|------|-----------------|---------------|
| Standard | 60 | 1000 |
| Premium | 300 | 5000 |

Rate limit headers:
```http
X-RateLimit-Limit: 60
X-RateLimit-Remaining: 45
X-RateLimit-Reset: 1642345678
```

---

## Versioning

The API uses URL versioning (future):

```
https://api.b2connect.de/v1/products
https://api.b2connect.de/v2/products
```

Currently, all endpoints use v1 (implied).

---

## SDK & Examples

### cURL

```bash
# Login
curl -X POST https://api.b2connect.de/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"secret"}'

# Get Products
curl https://api.b2connect.de/api/products \
  -H "Authorization: Bearer <token>"
```

### JavaScript/TypeScript

```typescript
// Using fetch
const response = await fetch('https://api.b2connect.de/api/products', {
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  }
});
const data = await response.json();
```

### C# / .NET

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", accessToken);

var response = await client.GetAsync("https://api.b2connect.de/api/products");
var products = await response.Content.ReadFromJsonAsync<ProductsResponse>();
```

---

## Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-01 | Initial release |

---

*For questions or support, contact: api-support@b2connect.de*
