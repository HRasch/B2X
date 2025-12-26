# Shared Authentication Strategy

## Überblick

Beide Gateways (Store und Admin) verwenden **denselben JWT-Authentifizierungsmechanismus**, sodass Tokens zwischen den Contexts wiederverwendet werden können.

## Architektur

```
┌─────────────────┐
│  Identity       │  <- Zentraler Auth-Service (Shared Context)
│  Service        │     - Login
│  (Port 5001)    │     - Refresh Token
└────────┬────────┘     - User Management
         │
         │ JWT Token
         │ (mit Secret: "B2Connect-Super-Secret...")
         │
    ┌────┴────┐
    │         │
┌───▼──┐  ┌──▼────┐
│Store │  │ Admin │
│API   │  │ API   │
│(6000)│  │(6100) │
└──────┘  └───────┘
```

## JWT-Konfiguration (Shared)

**Gemeinsame Werte** in beiden Gateways:

```json
{
  "Jwt": {
    "Secret": "B2Connect-Super-Secret-Key-For-Development-Only-32chars!",
    "Issuer": "B2Connect",
    "Audience": "B2Connect",  // <- Gleich für Store und Admin!
    "ExpirationMinutes": 60
  }
}
```

## Token-Nutzung

### Szenario 1: Login über Store

```bash
# 1. User loggt sich über Store Frontend ein
POST http://localhost:6000/api/auth/login
{
  "email": "user@example.com",
  "password": "password123"
}

# Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "123",
    "email": "user@example.com",
    "roles": ["User"]
  }
}

# 2. Token kann AUCH für Admin-API verwendet werden
GET http://localhost:6100/api/admin/settings
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

# ✅ Token wird akzeptiert (falls User Admin-Rolle hat)
```

### Szenario 2: Login über Admin

```bash
# 1. Admin loggt sich über Admin Frontend ein
POST http://localhost:6100/api/auth/login
{
  "email": "admin@example.com",
  "password": "admin123"
}

# 2. Token kann AUCH für Store-API verwendet werden
GET http://localhost:6000/api/catalog/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

# ✅ Token wird akzeptiert
```

## Rollen-basierte Autorisierung

Während **Authentication** (Token-Validierung) für beide Gateways gleich ist, ist **Authorization** (Zugriffskontrolle) unterschiedlich:

### Store API (Port 6000)

- **Öffentliche Endpunkte**: Kein Token benötigt
  - `GET /api/catalog/products`
  - `GET /api/cms/pages`
  - `GET /api/theming/themes`

- **Geschützte Endpunkte**: Token benötigt (beliebige Rolle)
  - `GET /api/auth/me` (eigenes Profil)
  - `POST /api/orders` (Bestellungen erstellen)

### Admin API (Port 6100)

- **Alle Endpunkte geschützt**: Token + Admin-Rolle benötigt
  - `GET /api/admin/products` → Benötigt `Role = "Admin"`
  - `POST /api/admin/users` → Benötigt `Role = "Admin"`

**Authorization Policy**:

```csharp
// Admin API Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// In Controllers:
[Authorize(Policy = "AdminOnly")]
public class AdminProductsController : ControllerBase
{
    // Nur für Admin-Rolle
}
```

## Gateway-Routing

### Store Gateway (YARP Reverse Proxy)

```json
{
  "ReverseProxy": {
    "Routes": {
      "auth-route": {
        "ClusterId": "identity-cluster",
        "Match": { "Path": "/api/auth/{**catch-all}" }
      },
      "catalog-route": {
        "ClusterId": "catalog-cluster",
        "Match": { "Path": "/api/catalog/{**catch-all}" }
      }
      // ... weitere Routes
    },
    "Clusters": {
      "identity-cluster": {
        "Destinations": {
          "identity": { "Address": "http://localhost:5001" }
        }
      }
    }
  }
}
```

### Admin Gateway (direkte Controller)

Admin-Gateway hat eigene Controller, die direkt auf Services zugreifen:

```csharp
[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // CRUD-Operationen
    }
}
```

## Vorteile dieser Architektur

### ✅ Single Sign-On (SSO)

- User loggt sich einmal ein → Token funktioniert überall
- Keine separate Authentifizierung für Store/Admin nötig

### ✅ Einfache Token-Verwaltung

- Ein JWT-Secret für alle Services
- Zentrale Token-Generierung im Identity Service

### ✅ Flexibilität

- Store-User kann bei Bedarf Admin-Funktionen nutzen (wenn Rolle vorhanden)
- Admin kann Store-Frontend testen mit eigenem Token

### ✅ Security durch Rollen

- Obwohl Token überall akzeptiert wird, kontrollieren Rollen den Zugriff
- Store-User ohne Admin-Rolle kann Admin-Endpunkte nicht nutzen

## Security Best Practices

### 1. JWT Secret (Production)

```csharp
// ❌ NICHT in appsettings.json hartcodieren!
// ✅ Verwende Environment Variables oder Azure Key Vault
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException("JWT Secret not configured");
```

### 2. Token Expiration

- **AccessToken**: 60 Minuten (kurz)
- **RefreshToken**: 7 Tage (lang)
- User muss sich nach Ablauf neu authentifizieren

### 3. HTTPS in Production

```csharp
// Nur HTTPS in Production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
```

### 4. CORS Konfiguration

```csharp
// Nur erlaubte Frontends
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStoreFrontend", policy =>
    {
        policy
            .WithOrigins("https://store.b2connect.com") // Production URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

## Testing

### Token-Generierung für Tests

```csharp
public static string GenerateTestToken(string userId, string[] roles)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes("B2Connect-Super-Secret-Key-For-Development-Only-32chars!");
    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, "test@example.com")
        }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)))),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = "B2Connect",
        Audience = "B2Connect",
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), 
            SecurityAlgorithms.HmacSha256Signature)
    };
    
    return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
}

// Usage:
var adminToken = GenerateTestToken("user-123", new[] { "Admin" });
var storeToken = GenerateTestToken("user-456", new[] { "User" });
```

## Migration Path

Wenn später Microservices getrennt deployed werden:

1. **Identity Service** bleibt zentral
2. **Store API** deployed auf `store.b2connect.com`
3. **Admin API** deployed auf `admin.b2connect.com`
4. **Gleicher JWT-Secret** über Environment Variables synchronisiert

```bash
# Alle Services erhalten denselben Secret
export JWT_SECRET="production-secret-key-from-key-vault"
```

## Zusammenfassung

| Aspekt | Store Gateway | Admin Gateway | Identity Service |
|--------|---------------|---------------|------------------|
| Port | 6000 | 6100 | 5001 |
| Auth | JWT (shared) | JWT (shared) | JWT Generator |
| Zugriff | Public + Auth | Admin-Only | Public Login |
| Token akzeptiert | Alle gültigen | Alle gültigen | Generiert Tokens |
| Audience | "B2Connect" | "B2Connect" | "B2Connect" |
| Rollen-Check | Optional | Required (Admin) | N/A |

**Kernprinzip**: **Eine Authentifizierung, mehrere Autorisierungen** ✅
