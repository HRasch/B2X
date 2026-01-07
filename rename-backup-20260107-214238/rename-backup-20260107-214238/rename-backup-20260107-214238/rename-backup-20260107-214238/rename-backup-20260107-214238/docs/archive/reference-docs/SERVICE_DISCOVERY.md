# Service Discovery Configuration

## �bersicht

B2X nutzt **Aspire Service Discovery** f�r die Inter-Service-Kommunikation. Services kommunizieren �ber **Service-Namen** statt hardcodierter URLs/Ports.

## Service-Namen (Aspire Registry)

| Service | Service-Name | Port (lokal) | Beschreibung |
|---------|-------------|--------------|--------------|
| Identity/Auth | `http://auth-service` | 7002 | JWT Authentication, Passkeys |
| Tenancy | `http://tenant-service` | 7003 | Multi-Tenancy Management |
| Localization | `http://localization-service` | 7004 | i18n Translations |
| Catalog | `http://catalog-service` | 7005 | Products, Categories |
| Theming | `http://theming-service` | 7008 | UI Themes, Layouts |
| Store Gateway | `http://store-gateway` | 8000 | Public API (YARP) |
| Admin Gateway | `http://admin-gateway` | 8080 | Protected API (YARP) |

## Wie es funktioniert

### 1. Aspire Orchestration registriert Services

```csharp
// backend/Orchestration/Program.cs
var authService = builder
    .AddProject("auth-service", "../BoundedContexts/Shared/Identity/B2X.Identity.API.csproj")
    .WithHttpEndpoint(port: 7002, targetPort: 7002, name: "auth-service");
```

### 2. ServiceDefaults aktiviert Service Discovery

```csharp
// backend/ServiceDefaults/Extensions.cs
builder.ConfigureServices((context, services) =>
{
    // Enable Service Discovery for all services
    services.AddServiceDiscovery();

    // Configure HttpClient defaults with Service Discovery
    services.ConfigureHttpClientDefaults(http =>
    {
        http.AddServiceDiscovery();
        http.AddStandardResilienceHandler();
    });
});
```

### 3. Services registrieren HttpClients mit Service-Namen

```csharp
// backend/shared/B2X.Shared.Infrastructure/Extensions/ServiceClientExtensions.cs
services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
{
    // Service Discovery resolves "http://auth-service" automatically
    client.BaseAddress = new Uri("http://auth-service");
});
```

### 4. YARP Gateways nutzen Service-Namen

```json
// backend/BoundedContexts/Store/API/appsettings.json
{
  "ReverseProxy": {
    "Clusters": {
      "identity-cluster": {
        "Destinations": {
          "identity": {
            "Address": "http://auth-service"
          }
        }
      }
    }
  }
}
```

## Verwendung in Services

### Service Clients registrieren

```csharp
// In Program.cs
using B2X.Shared.Infrastructure.Extensions;

// Alle Service Clients registrieren
builder.Services.AddAllServiceClients();

// Oder einzeln
builder.Services.AddIdentityServiceClient();
builder.Services.AddCatalogServiceClient();
```

### Service Clients verwenden

```csharp
public class ProductController : ControllerBase
{
    private readonly ICatalogServiceClient _catalogService;

    public ProductController(ICatalogServiceClient catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("{sku}")]
    public async Task<IActionResult> GetProduct(string sku, Guid tenantId)
    {
        // Calls http://catalog-service/api/products/sku/{sku}
        var product = await _catalogService.GetProductBySkuAsync(sku, tenantId);
        return Ok(product);
    }
}
```

## Vorteile

? **Keine hardcodierten URLs** - Services finden sich automatisch  
? **Umgebungs-unabh�ngig** - Funktioniert in Dev, Docker, Kubernetes  
? **Automatisches Load Balancing** - Aspire verteilt Last  
? **Resilience** - Circuit Breaker, Retry-Policies integriert  
? **Health Checks** - Nur gesunde Services erhalten Traffic  

## Lokale Entwicklung

### Mit Aspire (empfohlen)

```bash
cd backend/Orchestration
dotnet run

# Aspire Dashboard: http://localhost:15500
# Alle Services werden automatisch gestartet
```

### Ohne Aspire (manuell)

Falls du Services einzeln startest, musst du Umgebungsvariablen setzen:

```bash
# �berschreibt Service Discovery mit localhost
export SERVICES__AUTH__0=http://localhost:7002
export SERVICES__CATALOG__0=http://localhost:7005

dotnet run --project backend/BoundedContexts/Store/API
```

## Troubleshooting

### Service nicht gefunden

```
System.InvalidOperationException: No service endpoints found for 'http://auth-service'
```

**L�sung**: Aspire Orchestration l�uft nicht. Starte:
```bash
cd backend/Orchestration && dotnet run
```

### Service Discovery deaktiviert

Falls Service Discovery nicht funktioniert, pr�fe `Program.cs`:

```csharp
// Muss vorhanden sein
builder.Host.AddServiceDefaults();
```

### Hardcodierte URLs in Produktion

In `appsettings.Production.json` sollten **keine** `localhost`-URLs sein:

```json
{
  "ReverseProxy": {
    "Clusters": {
      "identity-cluster": {
        "Destinations": {
          "identity": {
            "Address": "http://auth-service"  // ? Service-Name
            // "Address": "http://localhost:7002"  // ? Hardcoded
          }
        }
      }
    }
  }
}
```

## Weitere Informationen

- [Aspire Service Discovery Docs](https://learn.microsoft.com/en-us/dotnet/aspire/service-discovery/overview)
- [YARP Reverse Proxy](https://microsoft.github.io/reverse-proxy/)
- [HttpClient Resilience](https://learn.microsoft.com/en-us/dotnet/core/resilience/)
