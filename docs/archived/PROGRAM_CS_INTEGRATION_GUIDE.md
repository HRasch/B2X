# 🔧 Program.cs Integration Guide - PIM Sync Service

**Datum**: 26. Dezember 2025

---

## 📋 Übersicht

Diese Anleitung zeigt genau, wie die **PIM Sync Service** Komponenten in den Backend-Service integriert werden.

---

## 📁 Datei-Lokationen

```
backend/services/CatalogService/Program.cs
                                ↑
                       (zu aktualisieren)
```

---

## 🔧 Integration Steps

### Schritt 1: Identify Current Program.cs

**Datei**: `backend/services/CatalogService/Program.cs`

Aktuelle Struktur (Beispiel):
```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddScoped<ICatalogService, CatalogService>();
// ... andere Services

var app = builder.Build();

// Configure
app.UseRouting();
app.MapControllers();

app.Run();
```

---

### Schritt 2: Services Registrieren

**Hinzufügen nach existierenden Services**:

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// ===== BESTEHENDE SERVICES =====
builder.Services.AddScoped<ICatalogService, CatalogService>();
// ... andere Services

// ===== PHASE 2: MULTI-PROVIDER INTEGRATION =====
// Registriere alle Product Providers (PimCore, nexPIM, Oxomi, etc.)
builder.Services.AddProductProviders(builder.Configuration);

// ===== PHASE 3: PIM SYNC SERVICE ===== ← NEW
// Registriere Sync Service und Background Worker
builder.Services.AddPimSync(builder.Configuration);

// ===== CONTROLLER & MIDDLEWARE =====
var app = builder.Build();

// Configure middleware
app.UseProductProviders();  // Provider health checks
app.UseRouting();
app.MapControllers();

app.Run();
```

---

## 📝 Detaillierte Erklärung

### `AddProductProviders()`

```csharp
builder.Services.AddProductProviders(builder.Configuration);
```

**Was macht das?**
- Registriert `IProductProviderRegistry`
- Registriert `IProductProviderResolver`
- Für jeden **Enabled Provider** in Config:
  - Erstellt `HttpClient` mit API Key & Timeout
  - Registriert spezialisierten Provider (PimCore, nexPIM, etc.)

**Quelle**: `ProductProviderExtensions.cs`

**Abhängigkeiten**:
- `IConfiguration` (für appsettings.json)
- `HttpClientFactory` (intern)

---

### `AddPimSync()`

```csharp
builder.Services.AddPimSync(builder.Configuration);
```

**Was macht das?**
- Registriert `IPimSyncService` → `PimSyncService`
- Wenn `"PimSync:Enabled" == true`:
  - Registriert `PimSyncWorker` als `IHostedService`
  - Worker startet automatisch bei App-Start
  - Worker läuft im Hintergrund nach Interval

**Quelle**: `PimSyncExtensions.cs`

**Abhängigkeiten**:
- `IConfiguration` (für PimSync Settings)
- `IProductProviderResolver` (wird automatisch per DI injiziert)
- `IElasticsearchClient` (muss bereits registriert sein)

---

### `UseProductProviders()`

```csharp
app.UseProductProviders();
```

**Was macht das?**
- Konfiguriert `HttpClientFactory` für Provider
- Setzt API Keys aus `IConfiguration`
- Setzt Timeouts per Provider

**Optional**: Kann auch weggelassen werden, wenn HTTP-Clients in Extension bereits configured.

---

## 🔄 Vollständiges Beispiel

### Szenario A: Minimal Integration

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddElasticsearch(builder.Configuration);  // vorausgesetzt

// ← NEW: Add Product Providers
builder.Services.AddProductProviders(builder.Configuration);

// ← NEW: Add PIM Sync Service
builder.Services.AddPimSync(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();
```

---

### Szenario B: Komplette Integration mit Middleware

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddElasticsearch(builder.Configuration);
builder.Services.AddLogging(config => 
{
    config.AddConsole();
    config.AddDebug();
});

// ← NEW: Multi-Provider Integration
builder.Services.AddProductProviders(builder.Configuration);

// ← NEW: PIM Sync Service
builder.Services.AddPimSync(builder.Configuration);

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// ← NEW (optional): Provider HTTP client configuration
app.UseProductProviders();

// Endpoints
app.MapControllers();

// Health Checks
app.MapHealthChecks("/health");

app.Run();
```

---

### Szenario C: Mit ASP.NET Core Extensions

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Core Services
builder.Services
    .AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = 
            JsonNamingPolicy.CamelCase;
    });

// Data & Search
builder.Services.AddDbContext<CatalogDbContext>();
builder.Services.AddElasticsearch(builder.Configuration);

// Logging
builder.Services.AddLogging();

// ← NEW: Multi-Provider PIM Integration
builder.Services.AddProductProviders(builder.Configuration);

// ← NEW: PIM Sync Service
builder.Services.AddPimSync(builder.Configuration);

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseProductProviders();

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();
```

---

## ✅ Verificationschecks nach Integration

### 1. Kompilierung prüfen

```bash
cd backend/services/CatalogService
dotnet build
```

**Erwartet**: ✅ Build successful
**Falls Error**: 
- Missing using statements? (PimSyncExtensions, ProductProviderExtensions)
- Missing NuGet packages? (Elasticsearch.Net, etc.)

---

### 2. Service Startup testen

```bash
dotnet run
```

**Erwartet**:
```
info: Microsoft.Hosting.Lifetime[0]
      Application started
info: B2Connect.CatalogService.Workers.PimSyncWorker[0]
      PIM Sync Worker started
```

---

### 3. API Endpoints testen

```bash
# Provider Health
curl http://localhost:9001/api/v2/providers/health

# Sync Status
curl http://localhost:9001/api/v2/pimsync/status

# Sync Health
curl http://localhost:9001/api/v2/pimsync/health
```

**Erwartet**: HTTP 200 mit JSON Response

---

### 4. Logs prüfen

```bash
tail -f logs/CatalogService.log

# Suche nach:
# - "AddPimSync"
# - "PimSyncWorker"
# - "ProductProviderRegistry"
```

---

## 🛑 Häufige Fehler beim Integration

### ❌ Error: "The type or namespace name 'AddPimSync' does not exist"

**Ursache**: Using statement fehlt

**Lösung**:
```csharp
using B2Connect.CatalogService.Extensions;  // ← Add this

builder.Services.AddPimSync(builder.Configuration);
```

---

### ❌ Error: "No service for type 'IElasticsearchClient' has been registered"

**Ursache**: ElasticSearch nicht registriert

**Lösung**: Vor `AddPimSync()` registrieren:
```csharp
builder.Services.AddElasticsearchClient(builder.Configuration);
builder.Services.AddProductProviders(builder.Configuration);
builder.Services.AddPimSync(builder.Configuration);
```

---

### ❌ Error: "The configuration for 'PimSync' is missing"

**Ursache**: appsettings.json hat keine PimSync Section

**Lösung**: appsettings.json aktualisieren:
```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 3600
  }
}
```

---

### ❌ Error: "PimSyncWorker not starting"

**Ursache**: `"PimSync:Enabled": false` in Config

**Lösung**: In appsettings.json:
```json
{
  "PimSync": {
    "Enabled": true  // ← Change to true
  }
}
```

---

## 📋 Pre-Deployment Checklist

- [ ] Program.cs aktualisiert
- [ ] appsettings.json hat PimSync Section
- [ ] appsettings.json hat ProductProviders Section
- [ ] `dotnet build` erfolgreich
- [ ] `dotnet run` startet ohne Fehler
- [ ] `/api/v2/pimsync/health` returniert 200
- [ ] `/api/v2/providers/health` returniert 200
- [ ] Environment Variables gesetzt (PIMCORE_API_KEY, etc.)
- [ ] ElasticSearch Cluster erreichbar
- [ ] PIM Systeme erreichbar
- [ ] Logs können gelesen werden

---

## 🚀 Nach der Integration

### 1. Configuration anpassen

**Datei**: `appsettings.json` oder `appsettings.Production.json`

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 86400  // 24 hours für Production
  }
}
```

---

### 2. Environment Variables setzen

```bash
# Local Development
export PIMCORE_API_KEY="dev_api_key_123"
export NEXPIM_API_KEY="dev_api_key_456"
export OXOMI_API_KEY="dev_api_key_789"

# Production (z.B. in Kubernetes Secret)
kubectl create secret generic pim-keys \
  --from-literal=PIMCORE_API_KEY=prod_key \
  --from-literal=NEXPIM_API_KEY=prod_key \
  --from-literal=OXOMI_API_KEY=prod_key
```

---

### 3. Monitoring einrichten

```bash
# Health Check Endpoint
curl -s http://localhost:9001/api/v2/pimsync/health | jq .

# Set up Prometheus scraping
# Set up Grafana dashboard
# Configure alerts
```

---

### 4. Test durchführen

```bash
# Manueller Sync starten
curl -X POST http://localhost:9001/api/v2/pimsync/sync?provider=pimcore

# Status prüfen
curl http://localhost:9001/api/v2/pimsync/status

# ElasticSearch Indexes verifizieren
curl http://elasticsearch:9200/_cat/indices | grep products
```

---

## 📚 Referenzen

| Datei | Inhalt |
|:-----:|:------:|
| `ProductProviderExtensions.cs` | DI für Provider |
| `PimSyncExtensions.cs` | DI für Sync Service |
| `PimSyncService.cs` | Sync-Logik |
| `PimSyncWorker.cs` | Hintergrund-Worker |
| `PimSyncController.cs` | HTTP API |
| `PIM_SYNC_SERVICE_CONFIGURATION.md` | Konfiguration |

---

## ✅ Status

**Implementierung**: ✅ Complete  
**Integration**: 🔄 In Progress (Program.cs Step)  
**Testing**: ⏳ Pending  
**Deployment**: ⏳ Pending  

**Nächster Schritt**: Führe die oben angegebenen Integration Steps durch und teste!

---

**Zusammenfassung**: In wenigen Zeilen Code wird die gesamte PIM Synchronisation ins Backend integriert. Die Extension Methods kümmern sich um alle Details!
