# 🏢 Tenant Resolution Guide - B2Connect

**Zuletzt aktualisiert:** 28. Dezember 2025

---

## Übersicht

B2Connect unterstützt **Multi-Tenancy** mit drei verschiedenen Strategien zur Tenant-Ermittlung:

1. **X-Tenant-ID Header** (höchste Priorität)
2. **Host-basierte Lookup** (Production)
3. **Development Fallback** (nur Development)

---

## Strategie-Details

### 1️⃣ X-Tenant-ID Header (Explizit)

**Wann verwendet:** 
- API-Calls von Admin-Frontends
- Mobile Apps
- Externe Integrationen

**Wie:**
```http
GET /api/products HTTP/1.1
Host: api.b2connect.com
X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer <jwt_token>
```

**Frontend Beispiel:**
```typescript
// Frontend/Admin/src/services/api/auth.ts
const tenantId = localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;

const response = await axios.post("/api/auth/login", credentials, {
  headers: {
    "X-Tenant-ID": tenantId
  }
});
```

---

### 2️⃣ Host-basierte Lookup (Production)

**Wann verwendet:**
- Public Storefront
- Subdomains für verschiedene Mandanten
- White-Label Deployment

**Wie es funktioniert:**
```
1. Request kommt von: https://kunde1.b2connect.com
2. Middleware extrahiert Host: "kunde1.b2connect.com"
3. TenancyServiceClient lookup: GET /api/tenants/by-domain/kunde1.b2connect.com
4. Response: { "id": "550e8400-...", "name": "Kunde 1", "isActive": true }
5. Tenant ID wird im Context gesetzt
```

**Konfiguration:**
```json
// Tenant Service speichert Domain-Mapping
{
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Kunde 1",
  "domain": "kunde1.b2connect.com",
  "isActive": true
}
```

---

### 3️⃣ Development Fallback (Local)

**Wann verwendet:**
- Lokale Entwicklung
- Testing ohne Domain-Setup
- CI/CD Pipelines

**Konfiguration:**
```json
// backend/Domain/Identity/appsettings.Development.json
{
  "Tenant": {
    "Development": {
      "UseFallback": true,
      "FallbackTenantId": "00000000-0000-0000-0000-000000000001",
      "Comment": "In Development wird immer diese Tenant-ID verwendet wenn kein X-Tenant-ID Header vorhanden ist"
    }
  }
}
```

**Verhalten:**
- Wenn `UseFallback: true` → Verwendet `FallbackTenantId`
- Nur in `Development` Environment aktiv
- Wird ignoriert wenn X-Tenant-ID Header vorhanden ist

---

## Middleware Implementation

### TenantContextMiddleware

**Datei:** `backend/Domain/Tenancy/src/Infrastructure/Middleware/TenantContextMiddleware.cs`

```csharp
public async Task InvokeAsync(
    HttpContext context,
    ITenantContext tenantContext,
    ITenancyServiceClient tenancyClient)
{
    // 1. Public Endpoints überspringen
    if (IsPublicEndpoint(path))
    {
        await _next(context);
        return;
    }

    Guid? tenantId = null;

    // 2. X-Tenant-ID Header (höchste Priorität)
    var tenantIdHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
    if (Guid.TryParse(tenantIdHeader, out var parsedTenantId))
    {
        tenantId = parsedTenantId;
    }

    // 3. Host-basierte Lookup
    if (!tenantId.HasValue)
    {
        var host = context.Request.Host.Host;
        var tenant = await tenancyClient.GetTenantByDomainAsync(host);
        if (tenant != null && tenant.IsActive)
        {
            tenantId = tenant.Id;
        }
    }

    // 4. Development Fallback
    if (!tenantId.HasValue && _config["Tenant:Development:UseFallback"] == "true")
    {
        tenantId = Guid.Parse(_config["Tenant:Development:FallbackTenantId"]);
    }

    // 5. Kein Tenant gefunden = 400 Bad Request
    if (!tenantId.HasValue)
    {
        return BadRequest("Tenant could not be resolved");
    }

    // 6. Tenant in Context setzen
    ((TenantContext)tenantContext).TenantId = tenantId.Value;
}
```

---

## Frontend Integration

### Admin Frontend (mit X-Tenant-ID Header)

```typescript
// Frontend/Admin/src/main.ts
const DEFAULT_TENANT_ID = "00000000-0000-0000-0000-000000000001";

if (!localStorage.getItem("tenantId")) {
  localStorage.setItem("tenantId", DEFAULT_TENANT_ID);
}
```

```typescript
// Frontend/Admin/src/services/api/auth.ts
async login(credentials: LoginRequest): Promise<LoginResponse> {
  const tenantId = localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;

  const response = await axios.post("/api/auth/login", credentials, {
    headers: {
      "Content-Type": "application/json",
      "X-Tenant-ID": tenantId
    }
  });

  // Store tenant ID from response
  if (response.data.user?.tenantId) {
    localStorage.setItem("tenantId", response.data.user.tenantId);
  }

  return response.data;
}
```

### Store Frontend (host-basiert, kein Header)

```typescript
// Frontend/Store/src/services/api.ts
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
});

// Kein X-Tenant-ID Header nötig!
// Middleware ermittelt Tenant automatisch aus Host
```

---

## Public Endpoints (ohne Tenant-Check)

Diese Endpoints benötigen **keinen** Tenant:

```csharp
private static bool IsPublicEndpoint(string path)
{
    var publicPaths = new[]
    {
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/refresh",
        "/api/auth/passkeys/registration/start",
        "/api/auth/passkeys/registration/complete",
        "/api/auth/passkeys/authentication/start",
        "/api/auth/passkeys/authentication/complete",
        "/health",
        "/healthz",
        "/live",
        "/ready",
        "/swagger",
        "/.well-known/",
        "/metrics"
    };

    return publicPaths.Any(p => path.StartsWith(p));
}
```

---

## Environment-spezifische Konfiguration

### Development (lokal)

```json
// appsettings.Development.json
{
  "Tenant": {
    "Development": {
      "UseFallback": true,
      "FallbackTenantId": "00000000-0000-0000-0000-000000000001"
    }
  }
}
```

**Verhalten:**
- Keine Domain-Lookup nötig
- Feste Tenant-ID für alle Requests
- Schnelles Testen ohne Setup

### Staging

```json
// appsettings.Staging.json
{
  "Tenant": {
    "Development": {
      "UseFallback": false
    }
  }
}
```

**Verhalten:**
- Host-basierte Lookup aktiv
- Testet Production-Verhalten
- Benötigt DNS-Setup

### Production

```json
// appsettings.Production.json
{
  "Tenant": {
    "Development": {
      "UseFallback": false
    }
  }
}
```

**Verhalten:**
- Nur X-Tenant-ID Header oder Host-Lookup
- Kein Fallback
- Maximale Sicherheit

---

## Testing

### Unit Tests (mit Mock)

```csharp
[Fact]
public async Task Middleware_ResolvesTenantFromHeader()
{
    var context = new DefaultHttpContext();
    context.Request.Headers["X-Tenant-ID"] = "550e8400-e29b-41d4-a716-446655440000";

    await _middleware.InvokeAsync(context, _tenantContext, _tenancyClient);

    Assert.Equal(Guid.Parse("550e8400-..."), _tenantContext.TenantId);
}
```

### Integration Tests (mit TestServer)

```csharp
[Fact]
public async Task API_RequiresTenantForProtectedEndpoints()
{
    var response = await _client.GetAsync("/api/products");
    
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    // "Tenant could not be resolved"
}

[Fact]
public async Task API_AcceptsXTenantIdHeader()
{
    _client.DefaultRequestHeaders.Add("X-Tenant-ID", "550e8400-...");
    var response = await _client.GetAsync("/api/products");
    
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

---

## Troubleshooting

### ❌ "Missing required header: X-Tenant-ID"

**Problem:** Alte Middleware-Version ohne Fallback  
**Lösung:** Update auf neue TenantContextMiddleware

### ❌ "Tenant could not be resolved"

**Mögliche Ursachen:**
1. Kein X-Tenant-ID Header
2. Host nicht in Tenant Service registriert
3. Development Fallback deaktiviert

**Lösung:**
```bash
# 1. Check Config
cat backend/Domain/Identity/appsettings.Development.json

# 2. Verify Tenant Service
curl http://localhost:7003/api/tenants/by-domain/localhost

# 3. Check Logs
docker logs tenant-service | grep "GetTenantByDomainAsync"
```

### ❌ Frontend bekommt 400 beim Login

**Problem:** Frontend sendet keinen X-Tenant-ID Header  
**Lösung:**
```typescript
// Füge Header hinzu
const response = await axios.post("/api/auth/login", credentials, {
  headers: {
    "X-Tenant-ID": "00000000-0000-0000-0000-000000000001"
  }
});
```

---

## Migration Guide (Alt → Neu)

### Alte Implementation
```csharp
// ❌ Alte Middleware: Immer X-Tenant-ID Header erforderlich
if (string.IsNullOrEmpty(tenantIdHeader))
{
    return BadRequest("Missing required header: X-Tenant-ID");
}
```

### Neue Implementation
```csharp
// ✅ Neue Middleware: Fallback-Strategien
1. X-Tenant-ID Header
2. Host-Lookup
3. Development Fallback
```

### Migration Steps

1. **Backend aktualisieren:**
   ```bash
   git pull origin main
   dotnet restore
   ```

2. **Config hinzufügen:**
   ```json
   // appsettings.Development.json
   {
     "Tenant": {
       "Development": {
         "UseFallback": true,
         "FallbackTenantId": "00000000-0000-0000-0000-000000000001"
       }
     }
   }
   ```

3. **Middleware registrieren:**
   ```csharp
   // Program.cs
   app.UseMiddleware<TenantContextMiddleware>();
   ```

4. **Testen:**
   ```bash
   # Ohne Header (sollte Development Fallback verwenden)
   curl http://localhost:7002/api/products

   # Mit Header
   curl -H "X-Tenant-ID: 550e8400-..." http://localhost:7002/api/products
   ```

---

## Best Practices

### ✅ DO
- Verwende X-Tenant-ID Header für Admin-APIs
- Verwende Host-Lookup für Public Storefronts
- Verwende Development Fallback nur in Development
- Logge Tenant-Resolution-Entscheidungen
- Validiere Tenant existiert im Tenant Service

### ❌ DON'T
- Verwende Development Fallback in Production
- Hardcode Tenant-IDs in Frontend-Code
- Überschreibe Tenant-ID nach Resolution
- Ignoriere Tenant-Validierung
- Verwende gleiche Tenant-ID für alle Environments

---

## Weiterführende Dokumentation

- [Multi-Tenant Architecture](docs/architecture/DDD_BOUNDED_CONTEXTS.md)
- [Application Specifications](docs/APPLICATION_SPECIFICATIONS.md)
- [Aspire Quick Start](ASPIRE_QUICK_START.md)
- [Frontend Tenant Setup](docs/FRONTEND_TENANT_SETUP.md)

---

**Fragen?** Siehe [TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) oder öffne ein GitHub Issue.
