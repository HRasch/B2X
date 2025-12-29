# Authentication Quick Reference Card

**Last Updated**: 29 December 2025 | **Print-Friendly**: Yes | **Laminate for Desk**: Recommended

---

## 🔐 JWT Token Structure

```
eyJ[Header].[Payload].[Signature]

Header:   {"alg":"HS256","typ":"JWT"}
Payload:  {"sub":"user-001","email":"test@example.com","TenantId":"default",...}
Signature: HMAC(base64(header)+"."+base64(payload), secret)
```

---

## 📝 Common HTTP Headers

### Request Headers (Client → Server)

```http
Authorization: Bearer eyJ...                    ← JWT token
X-Tenant-ID: default                          ← Tenant identifier
Content-Type: application/json                ← Body format
Accept: application/json                      ← Expected response
```

### Response Headers (Server → Client)

```http
Content-Type: application/json                ← Response format
X-Correlation-ID: abc123                      ← Request tracking
Set-Cookie: refreshToken=...                  ← Token storage
Cache-Control: no-store, no-cache             ← Token security
```

---

## 🔓 Public (Anonymous) Endpoints

| Method | Endpoint | Purpose | Returns |
|--------|----------|---------|---------|
| POST | `/api/auth/login` | Authenticate user | JWT + user info |
| POST | `/api/auth/refresh` | Get new access token | New JWT |
| GET | `/health` | Service status | 200 OK |

### Login Request

```csharp
POST /api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

**Success (200)**:
```json
{
  "success": true,
  "user": {
    "id": "user-001",
    "email": "test@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"]
  },
  "accessToken": "eyJ...",
  "refreshToken": "eyJ...",
  "expiresIn": 3600
}
```

**Failure (401)**:
```json
{
  "success": false,
  "error": "INVALID_CREDENTIALS",
  "message": "Invalid credentials"
}
```

---

## 🔒 Protected Endpoints

| Method | Endpoint | Auth | Purpose | Returns |
|--------|----------|------|---------|---------|
| GET | `/api/auth/me` | JWT | Current user profile | User data |
| POST | `/api/auth/logout` | JWT | Logout user | 200 OK |
| GET | `/api/auth/users` | JWT + Admin | List all users | User array |
| GET | `/api/auth/users/:id` | JWT + Admin | Get user by ID | User data |
| PUT | `/api/auth/users/:id` | JWT + Admin | Update user | Updated user |
| DELETE | `/api/auth/users/:id` | JWT + Admin | Delete user | 204 No Content |

### Protected Request

```csharp
GET /api/auth/me
Authorization: Bearer eyJ...
```

**Success (200)**:
```json
{
  "id": "user-001",
  "email": "test@example.com",
  "firstName": "John",
  "roles": ["User"],
  "tenantId": "default",
  "createdAt": "2025-01-01T00:00:00Z"
}
```

**Failure (401)** - No token:
```json
{
  "error": "UNAUTHORIZED",
  "message": "Invalid token"
}
```

**Failure (403)** - Insufficient permissions:
```json
{
  "error": "FORBIDDEN",
  "message": "Access denied"
}
```

---

## 🎯 HTTP Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| **200** | Success | Login successful, data retrieved |
| **201** | Created | User registered |
| **204** | No Content | User deleted, logout |
| **400** | Bad Request | Invalid JSON, missing fields |
| **401** | Unauthorized | Invalid token, expired token |
| **403** | Forbidden | Role required (e.g., Admin) |
| **404** | Not Found | User doesn't exist |
| **429** | Too Many Requests | Rate limited (brute force protection) |
| **500** | Server Error | Unexpected error |

---

## 🔑 Authentication Attributes (C#)

```csharp
[AllowAnonymous]              // Anyone can access
public IActionResult Public() { }

[Authorize]                   // Must be logged in
public IActionResult Protected() { }

[Authorize(Roles = "Admin")]  // Must have Admin role
public IActionResult AdminOnly() { }

[Authorize(Roles = "Admin,Manager")]  // Admin OR Manager
public IActionResult Management() { }
```

---

## 📋 User Claims in JWT

```csharp
User.FindFirst(ClaimTypes.NameIdentifier)    // NameId: user-001
User.FindFirst(ClaimTypes.Email)              // Email: test@example.com
User.FindFirst(ClaimTypes.GivenName)          // FirstName: John
User.FindFirst(ClaimTypes.Surname)            // LastName: Doe
User.FindFirst(ClaimTypes.Role)               // Role: User, Admin
User.FindFirst("TenantId")                    // TenantId: default
User.IsInRole("Admin")                        // Is user admin?
```

---

## 🧪 Common Test Patterns

### Login Success Test

```csharp
[Fact]
public async Task Login_ValidCredentials_Returns200WithToken()
{
    var request = new LoginRequest 
    { 
        Email = "test@example.com", 
        Password = "password123" 
    };
    var response = await _client.PostAsync("/api/auth/login", JsonContent.Create(request));
    
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadAsAsync<LoginResponse>();
    result.AccessToken.Should().NotBeNullOrEmpty();
}
```

### Protected Endpoint Test

```csharp
[Fact]
public async Task GetProfile_WithValidToken_Returns200()
{
    // Add token to header
    _client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", validToken);
    
    var response = await _client.GetAsync("/api/auth/me");
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### Role-Based Test

```csharp
[Fact]
public async Task AdminEndpoint_UserRole_Returns403()
{
    _client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", userToken);
    
    var response = await _client.GetAsync("/api/auth/users");
    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
}
```

---

## ❌ Common Mistakes

| Mistake | Fix |
|---------|-----|
| Hardcoded JWT secret | Use `IConfiguration["Jwt:Secret"]` |
| Storing plaintext password | Use `UserManager.CreateAsync(user, password)` |
| Querying without TenantId | Add `.Where(u => u.TenantId == tenantId)` |
| [AllowAnonymous] not working | Remove fallback policy from `AddAuthorization()` |
| Token not sent in header | Use `Authorization: Bearer {token}` |
| Missing [Authorize] attribute | Add to protected endpoints |
| User enumeration in errors | Return same error for "user not found" + "password wrong" |
| No error handling | Wrap with try/catch, return 500 error |

---

## 🚀 Quick Curl Examples

### Login

```bash
curl -X POST http://localhost:7002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123"}'
```

### Protected Endpoint (with token)

```bash
curl -X GET http://localhost:7002/api/auth/me \
  -H "Authorization: Bearer eyJ..."
```

### Refresh Token

```bash
curl -X POST http://localhost:7002/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"eyJ..."}'
```

### Admin Endpoint

```bash
curl -X GET http://localhost:7002/api/auth/users \
  -H "Authorization: Bearer {admin-token}"
```

---

## 🎯 JavaScript Examples

### Login

```javascript
const response = await fetch('http://localhost:7002/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'test@example.com',
    password: 'password123'
  })
});

const data = await response.json();
localStorage.setItem('token', data.accessToken);
localStorage.setItem('refreshToken', data.refreshToken);
```

### Protected Request

```javascript
const response = await fetch('http://localhost:7002/api/auth/me', {
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('token')}`
  }
});

const userData = await response.json();
```

### Auto-Refresh Logic

```javascript
async function refreshToken() {
  const response = await fetch('http://localhost:7002/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      refreshToken: localStorage.getItem('refreshToken')
    })
  });
  
  const data = await response.json();
  localStorage.setItem('token', data.accessToken);
  return data.accessToken;
}

// Call before token expires
setInterval(refreshToken, 3300 * 1000); // 55 minutes for 1-hour tokens
```

---

## 🔐 Security Checklist

- [ ] JWT secret stored in `appsettings.json` (dev) / Azure KeyVault (prod)
- [ ] All passwords hashed with PBKDF2 (ASP.NET Core Identity)
- [ ] HTTPS enforced in production (`app.UseHttpsRedirection()`)
- [ ] Token expiration set (default: 3600 seconds)
- [ ] Refresh token rotation enabled
- [ ] CORS configured for frontend domains only
- [ ] Rate limiting enabled (prevent brute force)
- [ ] Audit logging enabled (all auth events)
- [ ] Multi-tenancy enforced (`TenantId` filter on all queries)
- [ ] [AllowAnonymous] only on public endpoints

---

## 📊 Token Lifespan

| Token | Lifetime | Refresh | Revoke |
|-------|----------|---------|--------|
| Access Token | 1 hour (3600s) | Use refresh token | Logout |
| Refresh Token | 30 days | Rotate on use | Logout |

### Timeline

```
T=0s        T=3600s (1h)      T=7200s (2h)
│           │                 │
├───────────┼─────────────────┤
│Access Token expires          │
│           └──Refresh─────────┤
│                    New access token (1h)
│                              │
│                    Logout/Revoke
```

---

## 🔧 Troubleshooting

### "401 Unauthorized"

```bash
# Check token format
curl -H "Authorization: Bearer token-here" http://localhost:7002/api/auth/me

# Check if token is valid JWT (3 parts)
echo $TOKEN | cut -d '.' -f1-3 | wc -w  # Should be 3

# Decode token to verify claims
# Use https://jwt.io (development only!)
```

### "403 Forbidden"

```bash
# Check if user has required role
# Inspect JWT claims at https://jwt.io

# Verify [Authorize(Roles="Admin")] attribute exists on endpoint

# Check user assignment to role
SELECT * FROM AspNetUserRoles WHERE UserId = 'user-id'
```

### "[AllowAnonymous] not working"

```csharp
// In Program.cs, verify:
builder.Services.AddAuthorization();  // ✅ No fallback policy

// NOT:
// builder.Services.AddAuthorization(options =>
//     options.FallbackPolicy = new AuthorizationPolicyBuilder()
//         .RequireAuthenticatedUser()
//         .Build());
```

---

## 📚 Files to Review

| File | Purpose |
|------|---------|
| `backend/Domain/Identity/Program.cs` | Configuration |
| `backend/Domain/Identity/Endpoints/AuthEndpoints.cs` | Wolverine endpoints |
| `backend/Domain/Identity/Controllers/AuthController.cs` | ASP.NET controller |
| `docs/AUTHENTICATION_API_GUIDE.md` | Complete API reference |
| `docs/AUTHENTICATION_ARCHITECTURE.md` | Architecture details |
| `docs/AUTHENTICATION_IMPLEMENTATION_GUIDE.md` | Implementation patterns |
| `docs/AUTHENTICATION_TESTING_GUIDE.md` | Testing examples |

---

**Keep this handy!** Laminate for your desk or bookmark in your browser.

