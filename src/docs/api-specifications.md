# B2X API Specifications

## Overview
This document describes the REST API endpoints for B2X services, including the e-commerce shop platform and procurement gateway.

## Base URL
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.B2X.com/api`

## API Endpoints Structure

The API is organized into major resource categories:
- **Auth**: Authentication and authorization
- **Tenants**: Tenant management
- **Shop**: E-commerce core (carts, customers, pricing)
- **Catalog**: Product management and search
- **Orders**: Order management and fulfillment
- **Inventory**: Stock and warehouse management
- **Payments**: Payment processing
- **Procurement**: Procurement platform integration
- **Suppliers**: Supplier management
- **Notifications**: Multi-channel notifications

## Authentication
All protected endpoints require a Bearer token in the Authorization header:
```
Authorization: Bearer <jwt-token>
```

## Common Headers
```
X-Tenant-ID: <tenant-uuid>
Content-Type: application/json
```

## Response Format
All responses follow the standard envelope format:
```json
{
  "data": {},
  "success": true,
  "message": "Optional message",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Auth Service Endpoints

### POST /auth/login
Login with email and password.

**Request**:
```json
{
  "email": "user@example.com",
  "password": "password",
  "tenantId": "optional-tenant-uuid"
}
```

**Response** (200 OK):
```json
{
  "data": {
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "expiresIn": 3600,
    "user": {
      "id": "user-uuid",
      "tenantId": "tenant-uuid",
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "avatar": "https://...",
      "status": "Active",
      "emailConfirmed": true
    }
  },
  "success": true,
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### POST /auth/refresh
Refresh JWT token.

**Request**:
```json
{
  "refreshToken": "refresh-token"
}
```

**Response** (200 OK):
```json
{
  "data": {
    "accessToken": "new-jwt-token",
    "expiresIn": 3600
  },
  "success": true
}
```

### POST /auth/logout
Logout current user.

**Authentication**: Required (Bearer token)

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

## Tenant Service Endpoints

### GET /tenants
Get all tenants for current user.

**Authentication**: Required
**Query Parameters**:
- `pageNumber` (default: 1)
- `pageSize` (default: 10)

**Response** (200 OK):
```json
{
  "data": {
    "items": [
      {
        "id": "tenant-uuid",
        "name": "Acme Corp",
        "slug": "acme-corp",
        "description": "Description",
        "logoUrl": "https://...",
        "status": "Active"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "success": true
}
```

### POST /tenants
Create a new tenant.

**Authentication**: Required
**Request**:
```json
{
  "name": "New Company",
  "slug": "new-company",
  "description": "Company description",
  "logoUrl": "https://..."
}
```

**Response** (201 Created):
```json
{
  "data": {
    "id": "tenant-uuid",
    "name": "New Company",
    "slug": "new-company",
    "status": "Active"
  },
  "success": true,
  "message": "Tenant created successfully"
}
```

### GET /tenants/{id}
Get tenant details.

**Authentication**: Required
**Tenant Context**: Required (must match authenticated tenant)

**Response** (200 OK):
```json
{
  "data": {
    "id": "tenant-uuid",
    "name": "Acme Corp",
    "slug": "acme-corp",
    "description": "Description",
    "logoUrl": "https://...",
    "status": "Active",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-02T00:00:00Z"
  },
  "success": true
}
```

### PUT /tenants/{id}
Update tenant.

**Authentication**: Required
**Tenant Context**: Required (must match authenticated tenant)

**Request**:
```json
{
  "name": "Updated Name",
  "description": "Updated description",
  "logoUrl": "https://..."
}
```

**Response** (200 OK):
```json
{
  "data": {
    "id": "tenant-uuid",
    "name": "Updated Name",
    "status": "Active"
  },
  "success": true,
  "message": "Tenant updated successfully"
}
```

### DELETE /tenants/{id}
Delete tenant (admin only).

**Authentication**: Required
**Authorization**: Admin role required

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Tenant deleted successfully"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "email": "Invalid email format",
    "password": "Password must be at least 8 characters"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Unauthorized",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "You do not have permission to access this resource",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred processing your request",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Rate Limiting
- Limit: 1000 requests per hour per IP
- Headers: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

## Pagination
All list endpoints support pagination:
- `pageNumber`: Current page (default: 1)
- `pageSize`: Items per page (default: 10, max: 100)

Response includes:
- `totalCount`: Total number of items
- `totalPages`: Total number of pages
- `hasNextPage`: Whether more pages exist
- `hasPreviousPage`: Whether previous page exists

## Versioning
API versioning is managed through URL paths:
- Current: `/api/v1/...`
- Legacy: `/api/v0/...` (deprecated)

## WebSocket Events
Real-time updates via WebSocket at `wss://api.B2X.com/ws`:
- `tenant.created`: New tenant created
- `tenant.updated`: Tenant updated
- `user.joined`: User joined tenant
- `user.left`: User left tenant

---

# Shop Platform API Endpoints

## Shopping Cart Endpoints

### POST /shop/carts
Create a new shopping cart.

**Authentication**: Optional (creates guest cart if not authenticated)
**Request**:
```json
{
  "customerId": "customer-uuid",
  "cartType": "b2b|b2c"
}
```

**Response** (201 Created):
```json
{
  "data": {
    "cartId": "cart-uuid",
    "customerId": "customer-uuid",
    "items": [],
    "subtotal": 0,
    "tax": 0,
    "total": 0,
    "currency": "EUR",
    "cartType": "b2c"
  },
  "success": true
}
```

### GET /shop/carts/{cartId}
Get cart details.

**Response** (200 OK):
```json
{
  "data": {
    "cartId": "cart-uuid",
    "items": [
      {
        "productId": "product-uuid",
        "quantity": 2,
        "price": 29.99,
        "discount": 0,
        "lineTotal": 59.98
      }
    ],
    "subtotal": 59.98,
    "tax": 9.60,
    "total": 69.58
  },
  "success": true
}
```

### POST /shop/carts/{cartId}/items
Add item to cart.

**Request**:
```json
{
  "productId": "product-uuid",
  "quantity": 1,
  "variantId": "variant-uuid"
}
```

**Response** (200 OK):
```json
{
  "data": {
    "cartId": "cart-uuid",
    "total": 69.58
  },
  "success": true,
  "message": "Item added to cart"
}
```

## Catalog Endpoints

### GET /catalog/products
Get products with filtering and search.

**Query Parameters**:
- `search`: Search term
- `categoryId`: Filter by category
- `pageNumber`: Page number (default: 1)
- `pageSize`: Items per page (default: 10, max: 100)
- `sortBy`: Sort field (name, price, popularity, newest)
- `minPrice`, `maxPrice`: Price range filter
- `inStock`: Only in-stock items (default: true)

**Response** (200 OK):
```json
{
  "data": {
    "items": [
      {
        "id": "product-uuid",
        "name": "Product Name",
        "description": "Product description",
        "price": 29.99,
        "b2bPrice": 24.99,
        "currency": "EUR",
        "images": ["https://...", "https://..."],
        "category": "Electronics",
        "inStock": true,
        "stockQuantity": 150,
        "rating": 4.5,
        "reviews": 234
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1250,
    "totalPages": 125
  },
  "success": true
}
```

## Order Endpoints

### POST /orders
Create a new order.

**Authentication**: Required
**Request**:
```json
{
  "cartId": "cart-uuid",
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Berlin",
    "postalCode": "10115",
    "country": "DE"
  },
  "paymentMethod": "credit_card|bank_transfer|invoice",
  "shippingMethod": "standard|express|overnight"
}
```

**Response** (201 Created):
```json
{
  "data": {
    "orderId": "order-uuid",
    "orderNumber": "ORD-2024-001234",
    "status": "confirmed",
    "total": 69.58,
    "items": [
      {
        "productId": "product-uuid",
        "quantity": 2,
        "price": 29.99,
        "lineTotal": 59.98
      }
    ],
    "createdAt": "2024-01-01T12:00:00Z",
    "estimatedDelivery": "2024-01-05T12:00:00Z"
  },
  "success": true,
  "message": "Order created successfully"
}
```

### GET /orders/{orderId}
Get order details.

**Authentication**: Required (must be order owner or admin)
**Response** (200 OK):
```json
{
  "data": {
    "orderId": "order-uuid",
    "orderNumber": "ORD-2024-001234",
    "status": "shipped",
    "items": [...],
    "total": 69.58,
    "trackingNumber": "DHL-123456789",
    "timeline": [
      {
        "status": "confirmed",
        "timestamp": "2024-01-01T12:00:00Z"
      },
      {
        "status": "shipped",
        "timestamp": "2024-01-02T10:30:00Z"
      }
    ]
  },
  "success": true
}
```

---

# Procurement Gateway API

## Procurement Order Synchronization

### POST /procurement/orders/sync
Synchronize order from procurement platform.

**Authentication**: Required + API Key for platform
**Request**:
```json
{
  "platformOrderId": "PO-12345",
  "platformType": "coupa|ariba|jaggr",
  "orderData": {
    "items": [
      {
        "sku": "SKU-123",
        "quantity": 100,
        "unitPrice": 10.50
      }
    ],
    "deliveryDate": "2024-01-15",
    "buyer": {
      "email": "buyer@company.com"
    }
  }
}
```

**Response** (200 OK):
```json
{
  "data": {
    "B2XOrderId": "order-uuid",
    "platformOrderId": "PO-12345",
    "status": "synchronized",
    "mappedItems": 5,
    "unmappedItems": 0
  },
  "success": true,
  "message": "Order synchronized successfully"
}
```

### GET /procurement/orders/{orderId}/status
Get order status for procurement platform.

**Response** (200 OK):
```json
{
  "data": {
    "platformOrderId": "PO-12345",
    "B2XOrderId": "order-uuid",
    "status": "shipped",
    "lastUpdated": "2024-01-02T10:30:00Z",
    "items": [
      {
        "sku": "SKU-123",
        "quantityOrdered": 100,
        "quantityShipped": 100,
        "quantityReceived": 95
      }
    ],
    "shipments": [
      {
        "shipmentId": "shipment-uuid",
        "trackingNumber": "DHL-123456789",
        "carrier": "DHL",
        "status": "in_transit"
      }
    ]
  },
  "success": true
}
```

## Inventory Synchronization

### POST /procurement/inventory/update
Update inventory for procurement platform visibility.

**Authentication**: Required
**Request**:
```json
{
  "updates": [
    {
      "sku": "SKU-123",
      "quantity": 500,
      "locationId": "warehouse-1",
      "lastUpdated": "2024-01-02T10:00:00Z"
    }
  ]
}
```

**Response** (200 OK):
```json
{
  "data": {
    "updated": 3,
    "failed": 0,
    "timestamp": "2024-01-02T10:15:00Z"
  },
  "success": true
}
```

## Supplier Management

### GET /suppliers
Get list of suppliers.

**Query Parameters**:
- `status`: active|inactive|pending
- `pageNumber`: Page number
- `pageSize`: Items per page

**Response** (200 OK):
```json
{
  "data": {
    "items": [
      {
        "supplierId": "supplier-uuid",
        "name": "Supplier Inc.",
        "email": "contact@supplier.com",
        "status": "active",
        "performanceScore": 4.8,
        "documentationStatus": "compliant"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 45
  },
  "success": true
}
```
