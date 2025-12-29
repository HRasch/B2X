# Authentication Architecture Guide

**Last Updated**: 29 December 2025  
**Service**: B2Connect Identity Service  
**Status**: ✅ Production Ready

---

## 📋 Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Component Design](#component-design)
3. [Authentication Flow](#authentication-flow)
4. [Authorization Mechanism](#authorization-mechanism)
5. [Multi-Tenancy Implementation](#multi-tenancy-implementation)
6. [Security Architecture](#security-architecture)
7. [Database Schema](#database-schema)
8. [Implementation Details](#implementation-details)

---

## Architecture Overview

### System Context Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     Client Applications                         │
│              (Store Frontend, Admin Dashboard)                  │
└────────────────────────┬────────────────────────────────────────┘
                         │ HTTP/HTTPS
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                      API Gateways                               │
│         (Store Gateway, Admin Gateway)                          │
└────────────────┬────────────────────────────────┬───────────────┘
                 │                                │
                 ▼                                ▼
┌────────────────────────────┐    ┌──────────────────────────────┐
│  Catalog Service           │    │  Identity Service (7002)     │
│  Tenancy Service           │    │  - JWT Generation            │
│  CMS Service               │    │  - Token Validation          │
│  Search Service            │    │  - User Management           │
└────────────────────────────┘    │  - Role Management           │
         │                         └──────────────────────────────┘
         └─────────────────────────────┬──────────────────────────┘
                                       │
                                       ▼
                            ┌──────────────────────┐
                            │   PostgreSQL (DB)    │
                            │  - Users             │
                            │  - Roles             │
                            │  - Audit Logs        │
                            └──────────────────────┘
```

### Key Responsibilities

**Identity Service (Backend)**:
- ✅ User authentication (login/logout)
- ✅ JWT token generation and validation
- ✅ Multi-factor authentication
- ✅ User and role management
- ✅ Tenant isolation enforcement

**API Gateways**:
- ✅ Route requests to appropriate services
- ✅ Forward JWT tokens in headers
- ✅ Handle CORS and security headers

**Other Microservices**:
- ✅ Validate JWT tokens
- ✅ Extract claims (userId, tenantId, roles)
- ✅ Enforce authorization based on roles
- ✅ Implement tenant filtering

---

## Component Design

### Identity Service Architecture

```
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                     │
│  - AuthController (HTTP endpoints)                      │
│  - AuthEndpoints (Wolverine endpoints)                  │
│  - Exception handling & error responses                 │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Application Layer                         │
│  - IAuthService (interface)                             │
│  - AuthService (implementation)                         │
│  - Validators (FluentValidation)                        │
│  - Commands & DTOs                                      │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│          Infrastructure Layer                           │
│  - AuthDbContext (EF Core)                              │
│  - UserManager<AppUser> (ASP.NET Identity)              │
│  - RoleManager<AppRole>                                 │
│  - Token generation services                            │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Data Layer                                │
│  - SQLite (development)                                 │
│  - PostgreSQL (production)                              │
│  - User entities                                        │
│  - Role entities                                        │
└─────────────────────────────────────────────────────────┘
```

### Key Components

**1. AuthController** (HTTP Endpoints)
- Traditional ASP.NET Core controller
- Endpoints: login, logout, me, 2fa, user management
- Attributes: `[AllowAnonymous]`, `[Authorize]`

**2. AuthEndpoints** (Wolverine Endpoints)
- Modern Wolverine HTTP pattern
- Endpoints: login, refresh, register
- Auto-discovered and routed

**3. AuthService** (Business Logic)
- User authentication
- Token generation & validation
- User profile management
- Two-factor authentication

**4. AuthDbContext** (Entity Framework Core)
- User entities (ASP.NET Identity)
- Role entities
- Claims and logins
- Tenant data

**5. Middleware Stack**
```
Request
   ↓
[Routing] → Route to handler/controller
   ↓
[Authentication] → Extract JWT, validate signature
   ↓
[Authorization] → Check [Authorize] attributes
   ↓
[Handler] → Execute business logic
   ↓
[Response] → Return result
```

---

## Authentication Flow

### 1. User Login

```csharp
// Step 1: User submits credentials
POST /api/auth/login
{
  "email": "admin@example.com",
  "password": "password"
}

// Step 2: AuthService validates credentials
var user = await _userManager.FindByEmailAsync(request.Email);
if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
    return Result.Failure(ErrorCodes.InvalidCredentials);

// Step 3: Generate JWT tokens
var accessToken = await GenerateAccessToken(user);
var refreshToken = await GenerateRefreshToken(user);

// Step 4: Return tokens to client
{
  "user": { id, email, roles, ... },
  "accessToken": "eyJ...",
  "refreshToken": "eyJ...",
  "expiresIn": 3600
}
```

### 2. Token Validation (Protected Request)

```csharp
// Request with token
GET /api/auth/me
Authorization: Bearer eyJ...

// Middleware extracts and validates
[Middleware] Authentication
  ├─ Extract JWT from "Authorization" header
  ├─ Validate signature with secret key
  ├─ Validate expiration time
  └─ Extract claims (userId, roles, tenantId)

// [Authorize] attribute checks token is present
[Authorize]
public async Task<IActionResult> GetCurrentUser()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var tenantId = User.FindFirst("TenantId")?.Value;
    // ... execute logic
}
```

### 3. Role-Based Authorization

```csharp
// Request
GET /api/auth/users
Authorization: Bearer eyJ...

// Middleware extracts roles from JWT claims
var userRoles = User.Claims
    .Where(c => c.Type == ClaimTypes.Role)
    .Select(c => c.Value)
    .ToList();  // ["Admin"]

// Handler checks role
var isAdmin = userRoles.Any(r => 
    r.Equals("Admin", StringComparison.OrdinalIgnoreCase));

if (!isAdmin)
    return Forbid();  // 403 Forbidden
```

---

## Authorization Mechanism

### ASP.NET Core Authorization Pipeline

```
┌─────────────────────────────────────────────────────────┐
│  Request arrives with Authorization header              │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│  Authentication Middleware                              │
│  - Extract JWT token from "Bearer" scheme               │
│  - Validate JWT signature                               │
│  - Validate expiration                                  │
│  - Create ClaimsPrincipal with JWT claims              │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│  Authorization Middleware                               │
│  - FallbackPolicy = null (allow anonymous)              │
│  - Check [AllowAnonymous] → Skip authorization          │
│  - Check [Authorize] → Require valid token              │
│  - Check [Authorize(Roles="Admin")] → Require role      │
└────────────────────┬────────────────────────────────────┘
                     │
              ┌──────┴──────┐
              │             │
        ✅ Authorized  ❌ Denied
              │             │
              ▼             ▼
          [Handler]      401/403
```

### Policy Configuration

```csharp
// Program.cs - NO fallback policy
builder.Services.AddAuthorization();
// This means:
// - Anonymous access allowed by default
// - [AllowAnonymous] explicitly allows anonymous
// - [Authorize] explicitly requires authentication
```

### Role-Based Access Control (RBAC)

```csharp
// Claims in JWT
"roles": ["Admin", "User"]

// Endpoint authorization
[Authorize(Roles = "Admin")]
public IActionResult GetAllUsers() { ... }

// Runtime role check
var isAdmin = User.IsInRole("Admin");

// Custom authorization policy (advanced)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
```

---

## Multi-Tenancy Implementation

### Tenant Isolation Strategy

**Layer 1: JWT Claims**
```json
{
  "TenantId": "default",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "user-001"
}
```

**Layer 2: Query Filtering (EF Core)**
```csharp
// All user queries must filter by TenantId
var user = await _context.Users
    .Where(u => u.TenantId == tenantId && u.Id == userId)
    .FirstOrDefaultAsync();
```

**Layer 3: Application Logic**
```csharp
// Extract tenant from JWT
var tenantId = User.FindFirst("TenantId")?.Value ?? "default";

// Verify access belongs to tenant
if (user.TenantId != tenantId)
    return Forbid();  // 403 Forbidden
```

### Tenant Context Flow

```
Request + JWT Token
         │
         ▼
Extract TenantId from claims
         │
         ▼
Pass tenantId to business logic
         │
         ▼
Database queries filtered by TenantId
         │
         ▼
Return tenant-specific data only
```

---

## Security Architecture

### JWT Token Security

**Token Structure**:
```
Header.Payload.Signature

Secret: Stored in configuration (never hardcoded)
Algorithm: HS256 (HMAC-SHA256)
Expiration: 3600 seconds (1 hour)
```

**Token Generation**:
```csharp
var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.ASCII.GetBytes(jwtSecret);

var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("TenantId", user.TenantId),
        new Claim(ClaimTypes.Role, role)
    }),
    Expires = DateTime.UtcNow.AddSeconds(3600),
    Issuer = "B2Connect",
    Audience = "B2Connect.Admin",
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
};

var token = tokenHandler.CreateToken(tokenDescriptor);
return tokenHandler.WriteToken(token);
```

### Password Security

**Hashing**: ASP.NET Core Identity (PBKDF2)
```csharp
var hasher = new PasswordHasher<AppUser>();
var hashedPassword = hasher.HashPassword(user, rawPassword);
// Stored in database, never in plaintext
```

**Validation**:
```csharp
var passwordValid = await _userManager.CheckPasswordAsync(user, rawPassword);
```

### Error Response Security

**Principle**: Never reveal if user exists

```csharp
// ✅ SECURE: Same response for both scenarios
if (user == null || !validPassword)
    return Unauthorized("Invalid credentials");

// ❌ INSECURE: User enumeration possible
if (user == null)
    return NotFound("User does not exist");
else if (!validPassword)
    return Unauthorized("Password incorrect");
```

---

## Database Schema

### User Entity

```sql
CREATE TABLE AspNetUsers (
    Id NVARCHAR(128) PRIMARY KEY,
    Email NVARCHAR(256),
    UserName NVARCHAR(256),
    PasswordHash NVARCHAR(MAX),
    PhoneNumber NVARCHAR(20),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    TenantId NVARCHAR(128),
    IsActive BIT DEFAULT 1,
    IsTwoFactorRequired BIT DEFAULT 0,
    EmailConfirmed BIT DEFAULT 0,
    PhoneNumberConfirmed BIT DEFAULT 0,
    TwoFactorEnabled BIT DEFAULT 0,
    LockoutEnd DATETIME2,
    LockoutEnabled BIT DEFAULT 1,
    AccessFailedCount INT DEFAULT 0,
    ConcurrencyStamp NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);
```

### Role Entity

```sql
CREATE TABLE AspNetRoles (
    Id NVARCHAR(128) PRIMARY KEY,
    Name NVARCHAR(256),
    NormalizedName NVARCHAR(256),
    Description NVARCHAR(500),
    ConcurrencyStamp NVARCHAR(MAX)
);
```

### User-Role Mapping

```sql
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(128),
    RoleId NVARCHAR(128),
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);
```

---

## Implementation Details

### Critical Configuration (Program.cs)

```csharp
// 1. Add ASP.NET Core Identity
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// 2. Add JWT Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),
            ValidIssuer = "B2Connect",
            ValidAudience = "B2Connect.Admin",
            ValidateExpiration = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 3. Add Authorization (NO fallback policy)
builder.Services.AddAuthorization();

// 4. Add Controllers + Wolverine
builder.Services.AddControllers();
builder.Services.AddWolverineHttp();

// 5. Middleware Order (CRITICAL)
app.UseAuthentication();    // Extract JWT
app.UseAuthorization();     // Check [Authorize]
app.MapControllers();       // ASP.NET Core endpoints
app.MapWolverineEndpoints(); // Wolverine endpoints
```

### Middleware Chain (Execution Order)

```
1. Routing → Route to handler/controller
2. Authentication → Extract & validate JWT
3. Authorization → Check [Authorize] attributes
4. Handler/Controller → Execute business logic
5. Response Middleware → Format response
```

### Token Validation Parameters

```csharp
var parameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,  // Verify signature
    IssuerSigningKey = key,           // Secret key
    ValidIssuer = "B2Connect",        // Check iss claim
    ValidAudience = "B2Connect",      // Check aud claim
    ValidateExpiration = true,        // Check exp claim
    ClockSkew = TimeSpan.Zero         // No time tolerance
};
```

---

## Key Learnings

### ✅ What Works

1. **No Fallback Policy**
   - Allows anonymous by default
   - `[AllowAnonymous]` explicitly permits access
   - `[Authorize]` explicitly requires authentication

2. **ASP.NET Core Authorization**
   - Built-in, well-tested, secure
   - Plays well with JWT tokens
   - Supports roles and claims

3. **Wolverine + ASP.NET Core**
   - Can coexist in same service
   - Wolverine endpoints auto-discovered
   - Both honor authentication/authorization

### ❌ What to Avoid

1. **Setting FallbackPolicy to RequireAuthenticatedUser()**
   - Forces all endpoints to require auth
   - Breaks `[AllowAnonymous]` endpoints
   - Must be null or empty

2. **Hardcoding JWT Secret**
   - Always use configuration
   - Never commit secrets to repo
   - Use Azure KeyVault in production

3. **Missing TenantId Filtering**
   - Allows cross-tenant data access
   - Security breach for multi-tenant apps
   - Must filter ALL queries by tenant

---

**Last Updated**: 29 December 2025  
**Status**: ✅ Production Ready  
**Verified**: All authentication flows tested and working

