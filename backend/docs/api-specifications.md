# B2Connect API Specifications

## Overview
This document describes the REST API endpoints for B2Connect services.

## Base URL
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.b2connect.com/api`

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
Real-time updates via WebSocket at `wss://api.b2connect.com/ws`:
- `tenant.created`: New tenant created
- `tenant.updated`: Tenant updated
- `user.joined`: User joined tenant
- `user.left`: User left tenant
