# B2X Authentication API Guide

**Last Updated**: 29 December 2025  
**Service**: Identity Service (Port 7002)  
**Framework**: ASP.NET Core 10.0 + Wolverine  
**Pattern**: JWT Bearer Tokens + Multi-tenant

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [API Endpoints](#api-endpoints)
3. [Authentication Flow](#authentication-flow)
4. [Error Handling](#error-handling)
5. [Security](#security)
6. [Examples](#examples)
7. [Testing](#testing)

---

## Overview

The B2X Identity Service provides JWT-based authentication for all microservices. It implements:

- **User Authentication**: Login with email/password
- **Token Management**: JWT access and refresh tokens
- **Multi-Factor Authentication**: TOTP-based 2FA
- **Multi-Tenancy**: Tenant isolation via JWT claims
- **Role-Based Access Control**: Admin, User roles
- **Password Management**: Secure hashing (ASP.NET Core Identity)

### Key Features

✅ JWT Bearer tokens with 1-hour expiration  
✅ Refresh token rotation for token renewal  
✅ Multi-factor authentication support  
✅ Role-based access control (RBAC)  
✅ Tenant isolation at application level  
✅ User enumeration protection  
✅ Secure error handling  

---

## API Endpoints

### Authentication Endpoints

#### 1. **Login** `POST /api/auth/login`

Authenticates a user and returns JWT tokens.

**Access**: `[AllowAnonymous]`

**Request**:
```json
{
  "email": "admin@example.com",
  "password": "password"
}
```

**Success Response (200 OK)**:
```json
{
  "user": {
    "id": "admin-001",
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "tenantId": "default",
    "roles": ["Admin"],
    "permissions": ["*"],
    "twoFactorEnabled": false
  },
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "requires2FA": false,
  "tempUserId": null,
  "twoFactorEnabled": false
}
```

**Error Responses**:
- `400 Bad Request`: Missing email or password
- `401 Unauthorized`: Invalid credentials or inactive user

**cURL Example**:
```bash
curl -X POST http://localhost:7002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'
```

---

#### 2. **Refresh Token** `POST /api/auth/refresh`

Refreshes an expired access token using a refresh token.

**Access**: `[AllowAnonymous]`

**Request**:
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Success Response (200 OK)**:
```json
{
  "user": {
    "id": "admin-001",
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "tenantId": "default",
    "roles": ["Admin"],
    "permissions": ["*"]
  },
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

**Error Responses**:
- `400 Bad Request`: Missing or invalid refresh token
- `401 Unauthorized`: Expired or tampered refresh token

---

#### 3. **Get Current User** `GET /api/auth/me`

Returns the authenticated user's information.

**Access**: `[Authorize]` - Requires valid JWT

**Success Response (200 OK)**:
```json
{
  "data": {
    "id": "admin-001",
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "tenantId": "default",
    "roles": [],
    "permissions": ["*"],
    "twoFactorEnabled": false
  },
  "message": "User loaded successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Missing or invalid JWT token

**cURL Example**:
```bash
curl -X GET http://localhost:7002/api/auth/me \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

#### 4. **Logout** `POST /api/auth/logout`

Logs out the authenticated user.

**Access**: `[Authorize]` - Requires valid JWT

**Success Response (200 OK)**:
```json
{
  "message": "Logged out successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Missing or invalid JWT token

---

#### 5. **Enable 2FA** `POST /api/auth/2fa/enable`

Enables two-factor authentication for the user.

**Access**: `[Authorize]` - Requires valid JWT

**Success Response (200 OK)**:
```json
{
  "data": {
    "secret": "JBSWY3DPEBLW64TMMQ======",
    "qrCodeUrl": "https://...",
    "backupCodes": ["123456", "234567", ...]
  },
  "message": "2FA enabled successfully"
}
```

**Error Responses**:
- `400 Bad Request`: 2FA already enabled
- `401 Unauthorized`: Missing or invalid JWT token

---

#### 6. **Verify 2FA Code** `POST /api/auth/2fa/verify`

Verifies a 2FA code during login.

**Access**: `[AllowAnonymous]`

**Request**:
```json
{
  "tempUserId": "user-123",
  "code": "123456"
}
```

**Success Response (200 OK)**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

**Error Responses**:
- `400 Bad Request`: Invalid or expired code
- `401 Unauthorized`: Invalid tempUserId

---

### User Management Endpoints (Admin Only)

#### 7. **List All Users** `GET /api/auth/users`

Lists all users in the system.

**Access**: `[Authorize + Admin]` - Requires Admin role

**Query Parameters**:
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 50)
- `search` (string): Search by email or name

**Success Response (200 OK)**:
```json
{
  "data": [
    {
      "id": "admin-001",
      "email": "admin@example.com",
      "firstName": "Admin",
      "lastName": "User",
      "tenantId": "default",
      "isActive": true,
      "isTwoFactorEnabled": false,
      "roles": ["Admin"]
    }
  ],
  "message": "Users retrieved successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Missing or invalid JWT token
- `403 Forbidden`: User does not have Admin role

**cURL Example**:
```bash
curl -X GET "http://localhost:7002/api/auth/users?pageSize=10" \
  -H "Authorization: Bearer $TOKEN"
```

---

#### 8. **Get User by ID** `GET /api/auth/users/{id}`

Gets a specific user's details.

**Access**: `[Authorize + Admin]` - Requires Admin role

**Path Parameters**:
- `id` (string): User ID

**Success Response (200 OK)**:
```json
{
  "data": {
    "id": "admin-001",
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "tenantId": "default",
    "isActive": true,
    "isTwoFactorEnabled": false,
    "roles": ["Admin"]
  },
  "message": "User retrieved successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Missing or invalid JWT token
- `403 Forbidden`: User does not have Admin role
- `404 Not Found`: User not found

---

#### 9. **Toggle User Status** `PATCH /api/auth/users/{id}/status`

Activates or deactivates a user account.

**Access**: `[Authorize + Admin]` - Requires Admin role

**Path Parameters**:
- `id` (string): User ID

**Request**:
```json
{
  "isActive": false
}
```

**Success Response (200 OK)**:
```json
{
  "data": {
    "id": "admin-001",
    "email": "admin@example.com",
    "isActive": false
  },
  "message": "User status updated successfully"
}
```

**Error Responses**:
- `401 Unauthorized`: Missing or invalid JWT token
- `403 Forbidden`: User does not have Admin role
- `404 Not Found`: User not found

---

### Health Endpoints

#### 10. **Health Check** `GET /health`

Service health status.

**Access**: `[AllowAnonymous]`

**Success Response (200 OK)**:
```json
{
  "status": "healthy"
}
```

---

## Client Library Integration

### JavaScript / TypeScript (axios)

#### Setup

```javascript
// services/authService.ts
import axios from 'axios';

const API_URL = 'http://localhost:7002/api/auth';

const apiClient = axios.create({
  baseURL: API_URL,
  headers: { 'Content-Type': 'application/json' }
});

// Add token to every request
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle token expiration
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const { data } = await axios.post(`${API_URL}/refresh`, {
            refreshToken
          });
          localStorage.setItem('accessToken', data.accessToken);
          localStorage.setItem('refreshToken', data.refreshToken);
          error.config.headers.Authorization = `Bearer ${data.accessToken}`;
          return apiClient(error.config);
        } catch {
          localStorage.clear();
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);

export default apiClient;
```

#### Login Example

```javascript
// services/authService.ts
export const login = async (email, password) => {
  const response = await apiClient.post('/login', { email, password });
  const { accessToken, refreshToken, user } = response.data;
  
  localStorage.setItem('accessToken', accessToken);
  localStorage.setItem('refreshToken', refreshToken);
  localStorage.setItem('user', JSON.stringify(user));
  
  return { accessToken, user };
};

export const logout = async () => {
  await apiClient.post('/logout');
  localStorage.clear();
};

export const getCurrentUser = async () => {
  const response = await apiClient.get('/me');
  return response.data.data;
};
```

#### Vue 3 Component Example

```vue
<script setup lang="ts">
import { ref } from 'vue'
import { login, logout } from '@/services/authService'

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  try {
    const { user } = await login(email.value, password.value)
    console.log('Logged in as:', user.email)
    // Navigate to dashboard
  } catch (err) {
    error.value = err.response?.data?.error?.message || 'Login failed'
  } finally {
    loading.value = false
  }
}

const handleLogout = async () => {
  await logout()
  // Navigate to login
}
</script>

<template>
  <form @submit.prevent="handleLogin">
    <input v-model="email" type="email" placeholder="Email" />
    <input v-model="password" type="password" placeholder="Password" />
    <button :disabled="loading">{{ loading ? 'Logging in...' : 'Login' }}</button>
    <div v-if="error" class="error">{{ error }}</div>
  </form>
</template>
```

### C# / .NET (HttpClient)

#### Setup

```csharp
// Services/AuthServiceClient.cs
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class AuthServiceClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:7002/api/auth";

    public AuthServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/login", 
            request, 
            cancellationToken: ct);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<LoginResponse>(cancellationToken: ct);
    }

    public async Task<RefreshResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/refresh",
            new { refreshToken },
            cancellationToken: ct);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<RefreshResponse>(cancellationToken: ct);
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync(
            $"{BaseUrl}/logout",
            null,
            cancellationToken: ct);
        
        response.EnsureSuccessStatusCode();
    }
}

// Response Models
public record LoginRequest(string Email, string Password);

public record LoginResponse(
    [property: JsonPropertyName("user")] UserDto User,
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("expiresIn")] int ExpiresIn,
    [property: JsonPropertyName("requires2FA")] bool Requires2FA = false);

public record RefreshResponse(
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("expiresIn")] int ExpiresIn);
```

#### Usage in Startup

```csharp
// Program.cs
builder.Services.AddHttpClient<AuthServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7002");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Usage in controller/handler
public class HomeController
{
    private readonly AuthServiceClient _authClient;

    public HomeController(AuthServiceClient authClient)
    {
        _authClient = authClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _authClient.LoginAsync(request, ct);
            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

---

## Authentication Flow

### Login Flow Diagram

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ 1. POST /api/auth/login
       │    { email, password }
       ▼
┌─────────────────────────────────┐
│   Identity Service              │
│  - Validate credentials         │
│  - Check if user active         │
│  - Generate JWT tokens          │
└──────┬──────────────────────────┘
       │ 2. Return JWT tokens
       │    + user info
       ▼
┌─────────────┐
│   Client    │
│  - Store    │
│    tokens   │
└─────────────┘
```

### Protected Request Flow

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ 1. GET /api/auth/me
       │    Authorization: Bearer <token>
       ▼
┌─────────────────────────────────┐
│   Middleware                    │
│  - Extract JWT from header      │
│  - Validate signature           │
│  - Validate expiration          │
│  - Extract claims               │
└──────┬──────────────────────────┘
       │ 2. Token valid?
       ├─ Yes: Continue to handler
       ├─ No: Return 401
       ▼
┌──────────────────────────┐
│  Authorization Filter    │
│  - Check [Authorize]     │
│  - Check [Authorize(Role)]
│  - Check role claims     │
└──────┬───────────────────┘
       │ 3. Authorized?
       ├─ Yes: Handler executes
       ├─ No: Return 403
       ▼
┌──────────────────────┐
│   Response to Client │
└──────────────────────┘
```

### Token Refresh Flow

```
Client has expired access token
         │
         ▼
POST /api/auth/refresh
{ refreshToken }
         │
         ▼
Validate refresh token
  ├─ Check signature
  ├─ Check expiration
  └─ Extract user ID
         │
         ▼
Generate new tokens
  ├─ New access token
  ├─ New refresh token
  └─ Send response
         │
         ▼
Client stores new tokens
```

---

## Error Handling

### HTTP Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success | Login successful, token refresh successful |
| 400 | Bad Request | Missing required fields, invalid format |
| 401 | Unauthorized | Invalid credentials, expired/invalid token |
| 403 | Forbidden | User lacks required role or permissions |
| 404 | Not Found | User or resource not found |
| 500 | Server Error | Internal server error |

### Error Response Format

```json
{
  "error": {
    "code": "InvalidCredentials",
    "message": "Email or password is incorrect"
  }
}
```

### Common Error Codes

| Code | HTTP | Description |
|------|------|-------------|
| `InvalidCredentials` | 401 | Email/password mismatch |
| `UserInactive` | 401 | User account is inactive |
| `UserNotFound` | 401 | User does not exist |
| `InvalidToken` | 401 | Token is invalid or tampered |
| `TokenExpired` | 401 | Token has expired |
| `TwoFactorRequired` | 200 | 2FA code needed (check `requires2FA` flag) |
| `AccessDenied` | 403 | User lacks required role |

### Detailed Error Scenarios

#### Scenario 1: Expired Access Token

**Problem**: Client has valid refresh token but expired access token

```json
// Request with expired token
GET /api/auth/me
Authorization: Bearer eyJ...expired...

// Response
{
  "error": {
    "code": "TokenExpired",
    "message": "Token has expired"
  }
}
// Status: 401
```

**Solution**: Use refresh token to get new access token

```bash
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "eyJ...valid..."
}
```

#### Scenario 2: Invalid Token Signature

**Problem**: Token has been tampered with or signature doesn't match

```json
// Request with tampered token
GET /api/auth/me
Authorization: Bearer eyJ...tampered...

// Response
{
  "error": {
    "code": "InvalidToken",
    "message": "Token is invalid or has been tampered"
  }
}
// Status: 401
```

**Solution**: Force re-login, obtain new tokens

#### Scenario 3: Missing Authorization Header

**Problem**: Client forgot to include Authorization header

```bash
# Request
GET /api/auth/me
# No Authorization header

# Response
{
  "error": {
    "code": "Unauthorized",
    "message": "Authorization header is missing"
  }
}
// Status: 401
```

**Solution**: Always include Authorization header on protected endpoints

```bash
GET /api/auth/me
Authorization: Bearer <your_token_here>
```

#### Scenario 4: Insufficient Permissions

**Problem**: User lacks required role for endpoint

```bash
# Request (User role trying to access Admin endpoint)
GET /api/auth/users
Authorization: Bearer eyJ...user_token...

# Response
{
  "error": {
    "code": "AccessDenied",
    "message": "You do not have permission to access this resource"
  }
}
// Status: 403
```

**Solution**: Either use admin account or request elevated permissions

#### Scenario 5: Rate Limiting (Brute Force Protection)

**Problem**: Too many failed login attempts from same IP

```bash
# After 5+ failed attempts in 10 minutes
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "wrong_password"
}

# Response
{
  "error": {
    "code": "TooManyAttempts",
    "message": "Too many failed login attempts. Please try again later."
  }
}
// Status: 429
```

**Solution**: Wait 10 minutes or contact support

---

## Troubleshooting Guide

### "Invalid Credentials" When Login Should Work

**Possible Causes**:
1. User account doesn't exist
2. User account is inactive
3. Password is incorrect
4. Email spelling is wrong
5. User was deleted

**Debugging Steps**:
```bash
# 1. Check if user exists (requires Admin access)
curl -X GET http://localhost:7002/api/auth/users \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq '.data[] | select(.email == "user@example.com")'

# 2. Check user status
# Look for "isActive": true in response

# 3. Try reset password flow if account exists but password is unknown
```

### Token Not Being Accepted After Refresh

**Possible Causes**:
1. New token not stored properly
2. Old token still being used
3. Server restarted (new signing key)
4. Token was invalidated

**Debugging Steps**:
```bash
# 1. Verify token is being stored
localStorage.getItem('accessToken')  // Should return token string

# 2. Decode token to check expiration
# Use jwt.io to decode token
# Check "exp" field is in future: new Date(exp * 1000) > new Date()

# 3. Verify Authorization header format
# Should be: "Authorization: Bearer eyJ..."
# NOT "Authorization: eyJ..." (missing "Bearer ")
```

### 403 Forbidden When Accessing Admin Endpoints

**Possible Causes**:
1. User doesn't have Admin role
2. Role claim is missing from token
3. Admin role requirement changed

**Debugging Steps**:
```bash
# 1. Check user's roles
curl -X GET http://localhost:7002/api/auth/me \
  -H "Authorization: Bearer $TOKEN" | jq '.data.roles'

# 2. Promote user to Admin (requires existing Admin)
curl -X PATCH http://localhost:7002/api/auth/users/user-id/role \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"role": "Admin"}'

# 3. Get new token after role change
# Old token won't have new role, must re-login
```

---

## Security

### JWT Token Structure

```
Header.Payload.Signature

Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "admin-001",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "admin@example.com",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "Admin User",
  "TenantId": "default",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin",
  "exp": 1767005194,
  "iss": "B2X",
  "aud": "B2X.Admin"
}

Signature: HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
```

### Security Best Practices

✅ **Always include Authorization header**:
```bash
Authorization: Bearer <token>
```

✅ **Store tokens securely** (client-side):
- Use httpOnly cookies for sensitive apps
- Or secure localStorage with HTTPS only

✅ **Never expose secrets**:
- JWT secret stored in Azure KeyVault (production)
- appsettings.json (development only)

✅ **Handle token expiration**:
- Access tokens: 3600 seconds (1 hour)
- Refresh tokens: 7 days
- Implement automatic refresh before expiration

✅ **Validate tokens before use**:
- Check expiration
- Verify signature
- Validate claims

---

## Examples

### JavaScript/TypeScript Example

```typescript
// 1. Login
async function login(email: string, password: string) {
  const response = await fetch('http://localhost:7002/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  
  if (!response.ok) throw new Error('Login failed');
  
  const data = await response.json();
  localStorage.setItem('accessToken', data.accessToken);
  localStorage.setItem('refreshToken', data.refreshToken);
  return data;
}

// 2. Protected request
async function getCurrentUser(token: string) {
  const response = await fetch('http://localhost:7002/api/auth/me', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  
  if (!response.ok) throw new Error('Unauthorized');
  return response.json();
}

// 3. Refresh token
async function refreshToken(refreshToken: string) {
  const response = await fetch('http://localhost:7002/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  });
  
  if (!response.ok) throw new Error('Refresh failed');
  const data = await response.json();
  localStorage.setItem('accessToken', data.accessToken);
  return data;
}
```

### C# Example (HttpClient)

```csharp
using HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:7002") };

// 1. Login
var loginRequest = new { email = "admin@example.com", password = "password" };
var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
var result = await response.Content.ReadAsAsync<AuthResponse>();
string token = result.AccessToken;

// 2. Protected request
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
var userResponse = await client.GetAsync("/api/auth/me");
var user = await userResponse.Content.ReadAsAsync<UserInfo>();

// 3. Refresh token
var refreshRequest = new { refreshToken = result.RefreshToken };
var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);
var newResult = await refreshResponse.Content.ReadAsAsync<AuthResponse>();
```

### cURL Examples

```bash
# 1. Login
curl -X POST http://localhost:7002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}' | jq .

# 2. Extract token (bash)
TOKEN=$(curl -s -X POST http://localhost:7002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}' | jq -r '.accessToken')

# 3. Protected request
curl -X GET http://localhost:7002/api/auth/me \
  -H "Authorization: Bearer $TOKEN" | jq .

# 4. Refresh token
curl -X POST http://localhost:7002/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"'$REFRESH_TOKEN'"}' | jq .

# 5. List users (admin)
curl -X GET "http://localhost:7002/api/auth/users?pageSize=10" \
  -H "Authorization: Bearer $TOKEN" | jq .
```

---

## Testing

### Manual Testing Checklist

- [ ] Login with valid credentials → returns JWT
- [ ] Login with invalid password → 401
- [ ] Login with non-existent email → 401
- [ ] Access protected endpoint without token → 401
- [ ] Access protected endpoint with valid token → 200
- [ ] Access protected endpoint with expired token → 401
- [ ] Refresh token with valid refreshToken → new tokens
- [ ] List users as admin → 200 with user list
- [ ] List users as regular user → 403
- [ ] Health check → 200 healthy

### Automated Testing

Run unit tests:
```bash
dotnet test backend/Domain/Identity/tests
```

Run integration tests:
```bash
dotnet test --filter "Category=Integration"
```

### Performance Testing

Baseline response times (local):
- Login: ~20ms
- Protected endpoint: ~5ms
- Token refresh: ~15ms
- User list: ~25ms

---

## Troubleshooting

### "401 Unauthorized" on protected endpoint

1. ✅ Token is expired → Use refresh token
2. ✅ Token format incorrect → Should be `Bearer <token>`
3. ✅ Token is invalid → Check signature and claims
4. ✅ Headers missing → Must include `Authorization` header

### "403 Forbidden" on admin endpoint

1. ✅ User lacks Admin role → Check `roles` claim in token
2. ✅ TenantId mismatch → Verify tenant isolation
3. ✅ Permission check failing → Verify endpoint requirements

### Tokens not persisting

1. ✅ Store in localStorage or session
2. ✅ Include in Authorization header on subsequent requests
3. ✅ Implement automatic refresh before expiration

---

**Last Updated**: 29 December 2025  
**Status**: ✅ Production Ready  
**Support**: Contact Backend Team

