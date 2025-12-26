# 🔌 Multi-Provider PIM Integration - B2Connect

**Datum**: 26. Dezember 2025  
**Status**: ✅ Implementierung abgeschlossen

---

## 📋 Übersicht

Das B2Connect Catalog Service unterstützt jetzt **mehrere externe PIM-Systeme** als Datenquellen für Produktinformationen. Dies ermöglicht:

- ✅ **Flexible Datenquellen**: Interne DB, PimCore, nexPIM, Oxomi
- ✅ **Fallback-Ketten**: Wenn Primary Provider fehlt, nutze Secondary
- ✅ **Priority-basierte Auswahl**: Konfigurierbare Prioritäten
- ✅ **Health Checks**: Diagnostics und Monitoring
- ✅ **Provider Switching**: Laufzeit-Provider-Wechsel ohne Restart

---

## 🏗️ Architektur

### Komponenten

```
API Request (ProductsQueryController)
    ↓
IProductProviderResolver
    ↓
Provider Priority Chain:
    ├─ Primary Provider (highest priority, enabled)
    ├─ Secondary Provider (fallback)
    ├─ Tertiary Provider (further fallback)
    └─ Internal Provider (always available)
    ↓
ProductDto Response
```

### Provider Interface

```csharp
public interface IProductProvider
{
    string ProviderName { get; }
    Task<bool> IsEnabledAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid tenantId, Guid productId);
    Task<PagedResult<ProductDto>> GetProductsPagedAsync(...);
    Task<PagedResult<ProductDto>> SearchProductsAsync(...);
    Task<bool> VerifyConnectivityAsync();
    Task<ProviderMetadata> GetMetadataAsync();
}
```

---

## 🔧 Implementierte Provider

### 1. Internal Provider
**Status**: ✅ Immer verfügbar  
**Quelle**: CatalogService ReadModel (PostgreSQL/SQL Server)  
**Features**:
- Full-Text Search via SearchText Index
- Price Range Filtering
- Category Filtering
- Pagination & Sorting
- Multi-Language Support

### 2. PimCore Provider
**Status**: ✅ REST API Integration  
**Quelle**: PimCore Object Management System  
**Features**:
- REST API Integration (`/webservice/rest/...`)
- Object Class Filtering
- Full-Text Search
- Price/Category Filtering
- Pagination

**Konfiguration**:
```json
{
  "ProductProviders": {
    "pimcore": {
      "Name": "pimcore",
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "https://pimcore.example.com",
      "ApiKey": "YOUR_API_KEY",
      "TimeoutMs": 30000,
      "Settings": {
        "ObjectClassName": "Product",
        "LanguageField": "language"
      }
    }
  }
}
```

### 3. NexPIM Provider
**Status**: ✅ REST API Integration  
**Quelle**: nexPIM Product Management System  
**Features**:
- REST API Integration (`/products`, `/health`)
- Search & Filter Queries
- Price Range Filtering
- Pagination
- Multi-Language Support

**Konfiguration**:
```json
{
  "ProductProviders": {
    "nexpim": {
      "Name": "nexpim",
      "Enabled": true,
      "Priority": 80,
      "BaseUrl": "https://nexpim.example.com/api",
      "ApiKey": "YOUR_API_KEY",
      "TimeoutMs": 30000,
      "Settings": {
        "DataSourceId": "b2connect-catalog"
      }
    }
  }
}
```

### 4. Oxomi Provider
**Status**: ✅ REST API Integration  
**Quelle**: Oxomi MDM/PIM Solution  
**Features**:
- REST API Integration (`/entities/search`)
- Entity-based Search
- Complex Filter Queries
- Pagination
- Multi-Language Support

**Konfiguration**:
```json
{
  "ProductProviders": {
    "oxomi": {
      "Name": "oxomi",
      "Enabled": true,
      "Priority": 70,
      "BaseUrl": "https://oxomi.example.com/api/v2",
      "ApiKey": "YOUR_API_KEY",
      "TimeoutMs": 30000,
      "Settings": {
        "EntityType": "Product",
        "Environment": "production"
      }
    }
  }
}
```

---

## 📊 Provider Priority & Fallback

### Priority System

| Provider | Default Priority | Notes |
|----------|:----------------:|-------|
| Internal | 100 | Highest - Core System |
| PimCore | 90 | Primary External |
| NexPIM | 80 | Secondary |
| Oxomi | 70 | Tertiary |

### Fallback Chain Beispiel

```
Request: GetProduct(ProductId)
    ↓
Try PimCore (Priority 90) → ✅ Found → Return Result
    
Request: GetProduct(UnknownId)
    ↓
Try PimCore (Priority 90) → ❌ Not Found
    ↓
Try NexPIM (Priority 80) → ❌ Not Found
    ↓
Try Oxomi (Priority 70) → ❌ Not Found
    ↓
Try Internal (Priority 100) → ✅ Fallback Success
    
Request: All Providers Down
    ↓
Error: No provider could resolve request
```

---

## 🚀 API-Verwendung

### 1. Produkte Abrufen (Multi-Provider)

```http
GET /api/v2/products?page=1&pageSize=20

Response:
{
  "items": [...],
  "page": 1,
  "pageSize": 20,
  "totalCount": 1000,
  "totalPages": 50,
  "hasNextPage": true,
  "searchMetadata": {
    "source": "pimcore"  // Zeigt welcher Provider verwendet wurde
  }
}
```

**Datenfluss**:
1. ProductsQueryController ruft IProductProviderResolver auf
2. Resolver versucht PimCore (Primary)
3. Falls PimCore Fehler → versucht NexPIM
4. Falls NexPIM Fehler → versucht Oxomi
5. Falls alle extern Fehler → verwendet Internal Provider

### 2. Produkt Suchen (Multi-Provider)

```http
GET /api/v2/products/elasticsearch?term=laptop&language=de

Response:
{
  "items": [...],
  "searchMetadata": {
    "source": "pimcore",
    "queryExecutionTimeMs": 125
  }
}
```

### 3. Provider Health Check

```http
GET /api/v2/providers/health

Response:
{
  "timestamp": "2025-12-26T10:30:00Z",
  "primaryProvider": "pimcore",
  "providers": [
    {
      "name": "pimcore",
      "isConnected": true,
      "version": "11.0.0",
      "lastSyncTime": "2025-12-26T10:25:00Z",
      "capabilities": {
        "supportsFullTextSearch": true,
        "supportsPriceFiltering": true,
        "supportsCategoryFiltering": true,
        "supportsSorting": true,
        "supportsPagination": true,
        "supportsMultiLanguage": true,
        "supportsSync": true
      }
    },
    {
      "name": "nexpim",
      "isConnected": false,
      "version": "Unknown"
    },
    {
      "name": "internal",
      "isConnected": true,
      "version": "1.0.0"
    }
  ]
}
```

### 4. Provider Diagnostics

```http
GET /api/v2/providers
Response: { "providers": ["pimcore", "nexpim", "internal"] }

GET /api/v2/providers/pimcore
Response: { "name": "pimcore", "isConnected": true, ... }

POST /api/v2/providers/pimcore/test
Response: { 
  "provider": "pimcore", 
  "isConnected": true, 
  "responseTimeMs": 145,
  "testedAt": "2025-12-26T10:30:00Z"
}
```

---

## 🔌 Integration in Program.cs

### Registrierung

```csharp
// In Program.cs

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add providers
builder.Services.AddProductProviders(builder.Configuration);

// ... andere services ...

var app = builder.Build();

// Initialize providers
app.UseProductProviders();

app.Run();
```

### appsettings.json (Vollständig)

```json
{
  "ProductProviders": {
    "pimcore": {
      "Name": "pimcore",
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "https://pimcore.example.com",
      "ApiKey": "${PIMCORE_API_KEY}",
      "TimeoutMs": 30000,
      "CacheDurationSeconds": 300,
      "Settings": {}
    },
    "nexpim": {
      "Name": "nexpim",
      "Enabled": false,
      "Priority": 80,
      "BaseUrl": "https://nexpim.example.com/api",
      "ApiKey": "${NEXPIM_API_KEY}",
      "TimeoutMs": 30000,
      "CacheDurationSeconds": 300,
      "Settings": {}
    },
    "oxomi": {
      "Name": "oxomi",
      "Enabled": false,
      "Priority": 70,
      "BaseUrl": "https://oxomi.example.com/api/v2",
      "ApiKey": "${OXOMI_API_KEY}",
      "TimeoutMs": 30000,
      "CacheDurationSeconds": 300,
      "Settings": {}
    }
  }
}
```

---

## 🎯 Use Cases

### 1. Nur Interne Datenbank
```json
{
  "ProductProviders": {}  // Nur Internal Provider wird verwendet
}
```

### 2. PimCore als Primary Source
```json
{
  "ProductProviders": {
    "pimcore": {
      "Enabled": true,
      "Priority": 90
    }
  }
}
```

### 3. Multi-Source mit Fallback Chain
```json
{
  "ProductProviders": {
    "pimcore": { "Enabled": true, "Priority": 90 },
    "nexpim": { "Enabled": true, "Priority": 80 },
    "oxomi": { "Enabled": true, "Priority": 70 }
  }
}
```

**Datenfluss**: PimCore → NexPIM → Oxomi → Internal

### 4. Dynamische Provider-Aktivierung

```csharp
// Zur Laufzeit Provider aktivieren/deaktivieren
// via Health Endpoint oder Admin Panel

if (providerDown) {
  config.ProductProviders.pimcore.Enabled = false;
  // System nutzt automatisch nächsten Provider
}
```

---

## 🛡️ Fehlerbehandlung

### Provider-Failover

```
Request → Try Primary
  ├─ ✅ Success → Return
  ├─ ❌ Timeout (30s default) → Try Secondary
  ├─ ❌ Authentication Error → Try Secondary
  ├─ ❌ Not Found → Try Secondary
  └─ ❌ Server Error → Try Secondary
```

### Logging

```csharp
// Jeder Provider-Aufruf wird geloggt
logger.LogInformation(
    "Product {ProductId} resolved from provider '{ProviderName}'",
    productId, provider.ProviderName);

logger.LogWarning(
    "Provider '{ProviderName}' failed. Trying next provider.",
    provider.ProviderName);
```

---

## 📈 Performance-Überlegungen

### Caching Strategy

```json
{
  "ProductProviders": {
    "pimcore": {
      "CacheDurationSeconds": 300,  // 5 Min Cache
      "TimeoutMs": 30000             // 30 Sek Timeout
    }
  }
}
```

### Parallel Requests

```csharp
// Nicht empfohlen: Parallel Requests zu mehreren Providern
// Besser: Sequential mit schnellem Failover
foreach (var provider in providersInPriority)
{
    try
    {
        return await provider.GetProductByIdAsync(...);
    }
    catch { /* try next */ }
}
```

---

## 🔍 Diagnostics & Monitoring

### Health Check API

```bash
# Alle Provider-Status abrufen
curl http://localhost:5000/api/v2/providers/health

# Spezifischen Provider testen
curl -X POST http://localhost:5000/api/v2/providers/pimcore/test

# Verfügbare Provider auflisten
curl http://localhost:5000/api/v2/providers
```

### Metriken

- **Provider Response Time**: Track per Provider
- **Fallback Rate**: Wie oft fällt System auf Secondary Provider?
- **Error Rate**: Prozentuale Fehlerquote pro Provider
- **Cache Hit Rate**: Wenn caching implementiert

---

## 🔐 Sicherheit

### API Key Management

```csharp
// Umgebungsvariablen verwenden
builder.Configuration
    .AddEnvironmentVariables("PIMCORE_API_KEY");

// Nie API Keys in appsettings.json speichern
// Verwende statt dessen: User Secrets, Azure KeyVault, etc.
```

### Authentication

```csharp
// Jeder Provider nutzt eigene Authentifizierung
_httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);    // PimCore
_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");  // NexPIM
```

### Tenant Isolation

```csharp
// Multi-Tenant Filter wird auf alle Provider angewendet
var products = await provider.GetProductsPagedAsync(
    tenantId: currentTenantId,  // ← Tenant-Filter
    ...
);
```

---

## 🚀 Deployment Checklist

- ✅ `ProductProviders` in appsettings.json konfiguriert
- ✅ `AddProductProviders()` in Program.cs
- ✅ `UseProductProviders()` middleware registriert
- ✅ Provider API Keys in Environment Variables
- ✅ Firewall Rules für externe APIs
- ✅ Network Connectivity zu externen PIMs
- ✅ Health Check Endpoint getestet
- ✅ Fallback Chain verifiziert
- ✅ Logging Review durchgeführt
- ✅ Performance unter Last getestet

---

## 📚 Weitere Ressourcen

- [IProductProvider.cs](backend/services/CatalogService/src/Providers/IProductProvider.cs)
- [InternalProductProvider.cs](backend/services/CatalogService/src/Providers/InternalProductProvider.cs)
- [PimCoreProductProvider.cs](backend/services/CatalogService/src/Providers/PimCoreProductProvider.cs)
- [NexPIMProductProvider.cs](backend/services/CatalogService/src/Providers/NexPIMProductProvider.cs)
- [OxomiProductProvider.cs](backend/services/CatalogService/src/Providers/OxomiProductProvider.cs)
- [ProductProviderRegistry.cs](backend/services/CatalogService/src/Providers/ProductProviderRegistry.cs)
- [ProductProviderExtensions.cs](backend/services/CatalogService/src/Extensions/ProductProviderExtensions.cs)
- [ProvidersController.cs](backend/services/CatalogService/src/Controllers/ProvidersController.cs)

---

**Zusammenfassung**: B2Connect unterstützt jetzt mehrere externe PIM-Systeme (PimCore, nexPIM, Oxomi) als Datenquellen mit intelligenter Fallback-Chain. Das System bleibt flexibel, performant und ausfallsicher!
