# 🔧 PIM Sync Service - Integrations- & Konfigurationshandbuch

**Datum**: 26. Dezember 2025  
**Status**: ✅ Fertig zur Integration

---

## 📋 Schnelleinstieg

### 1. DI in Program.cs Registrieren

```csharp
// Program.cs
var builder = WebApplicationBuilder.CreateBuilder(args);

// SCHRITT 1: Product Providers (MultiProvider PIM Integration)
builder.Services.AddProductProviders(builder.Configuration);

// SCHRITT 2: PIM Sync Service (Daten zu ElasticSearch)
builder.Services.AddPimSync(builder.Configuration);

// ... rest der Configuration
var app = builder.Build();

// ... Configure middleware
app.UseProductProviders();
app.MapControllers();
app.Run();
```

### 2. appsettings.json Konfigurieren

**Beispiel** (Development):

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 3600,
    "BatchSize": 100,
    "Timeout": 300000
  },
  
  "ProductProviders": {
    "internal": {
      "Name": "internal",
      "Enabled": true,
      "Priority": 100
    },
    
    "pimcore": {
      "Name": "pimcore",
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "https://pimcore.dev.example.com",
      "ApiKey": "${PIMCORE_API_KEY}",
      "TimeoutMs": 30000
    },
    
    "nexpim": {
      "Name": "nexpim",
      "Enabled": false,
      "Priority": 80,
      "BaseUrl": "https://nexpim.example.com",
      "ApiKey": "${NEXPIM_API_KEY}",
      "TimeoutMs": 30000
    },
    
    "oxomi": {
      "Name": "oxomi",
      "Enabled": false,
      "Priority": 70,
      "BaseUrl": "https://oxomi.example.com",
      "ApiKey": "${OXOMI_API_KEY}",
      "TimeoutMs": 30000
    }
  }
}
```

**Beispiel** (Production):

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 86400,  // 24 Stunden
    "BatchSize": 100,
    "Timeout": 300000
  },
  
  "ProductProviders": {
    "internal": {
      "Name": "internal",
      "Enabled": true,
      "Priority": 100
    },
    
    "pimcore": {
      "Name": "pimcore",
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "https://pimcore.prod.example.com",
      "ApiKey": "${PIMCORE_API_KEY}",
      "TimeoutMs": 30000
    },
    
    "nexpim": {
      "Name": "nexpim",
      "Enabled": true,
      "Priority": 80,
      "BaseUrl": "https://nexpim.prod.example.com",
      "ApiKey": "${NEXPIM_API_KEY}",
      "TimeoutMs": 30000
    }
  }
}
```

### 3. Environment Variables Setzen

```bash
# Development
export PIMCORE_API_KEY="dev_key_12345"
export NEXPIM_API_KEY="dev_key_67890"
export OXOMI_API_KEY="dev_key_11111"

# Oder in .env file:
PIMCORE_API_KEY=dev_key_12345
NEXPIM_API_KEY=dev_key_67890
OXOMI_API_KEY=dev_key_11111
```

---

## 🚀 Konfigurationsszenarien

### Szenario 1: Entwicklung mit lokalem PimCore

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 600,  // 10 Minuten (häufiges Testing)
    "BatchSize": 50
  },
  
  "ProductProviders": {
    "pimcore": {
      "Name": "pimcore",
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "http://localhost:8080",
      "ApiKey": "demo_key_123",
      "TimeoutMs": 60000
    }
  }
}
```

**Workflow**:
1. PimCore läuft auf http://localhost:8080
2. Service lädt alle 10 Minuten Produkte
3. Ideal für schnelle Entwicklung & Testing

---

### Szenario 2: Production mit Failover

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 86400,  // 24 Stunden
    "BatchSize": 100
  },
  
  "ProductProviders": {
    "internal": {
      "Name": "internal",
      "Enabled": true,
      "Priority": 100  // Primary fallback
    },
    
    "pimcore": {
      "Name": "pimcore",
      "Enabled": true,
      "Priority": 90,  // Primary source
      "BaseUrl": "https://pimcore-primary.prod.example.com",
      "ApiKey": "${PIMCORE_PRIMARY_KEY}",
      "TimeoutMs": 30000
    },
    
    "nexpim": {
      "Name": "nexpim",
      "Enabled": true,
      "Priority": 80,  // Secondary fallback
      "BaseUrl": "https://nexpim.prod.example.com",
      "ApiKey": "${NEXPIM_KEY}",
      "TimeoutMs": 30000
    }
  }
}
```

**Failover-Chain**:
1. **Primary**: PimCore (90)
2. **Fallback 1**: NexPIM (80)
3. **Fallback 2**: Internal (100)

Wenn PimCore nicht antwortet, wird automatisch nexPIM versucht, dann Internal.

---

### Szenario 3: Multi-PIM mit unterschiedlichen Kategorien

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 3600  // Stündlich
  },
  
  "ProductProviders": {
    "pimcore": {
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "https://pimcore.example.com",
      "ApiKey": "${PIMCORE_KEY}"
    },
    
    "nexpim": {
      "Enabled": true,
      "Priority": 85,
      "BaseUrl": "https://nexpim.example.com",
      "ApiKey": "${NEXPIM_KEY}"
    }
  }
}
```

**Verwendung**:
- Alle Produkte aus **PimCore** werden geladen (Primary)
- Produkte aus **NexPIM** werden hinzugefügt wenn nicht in PimCore
- Ermöglicht Multibranding / Multi-Tenant Setup

---

## 🎯 Häufige Konfigurationen

### Nur Manueller Sync (Kein Scheduler)

```json
{
  "PimSync": {
    "Enabled": false  // ← Scheduler deaktiviert
  },
  
  "ProductProviders": {
    "pimcore": {
      "Enabled": true,
      "Priority": 90,
      "BaseUrl": "https://pimcore.example.com"
    }
  }
}
```

**API Calls**:
```bash
# Manuell triggern
curl -X POST http://localhost:9001/api/v2/pimsync/sync?provider=pimcore

# Status prüfen
curl http://localhost:9001/api/v2/pimsync/status
```

---

### Sehr häufige Syncs (Echtzeit-ish)

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 300,  // 5 Minuten
    "BatchSize": 50
  }
}
```

**Nutzung**: E-Commerce mit sehr häufigen Inventaränderungen

---

### Nächtliche Batch-Syncs

```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 86400,  // 24 Stunden
    "BatchSize": 1000
  }
}
```

Scheduler startet täglich zu `Application Start + 24h`.

---

## 🔍 Debugging & Troubleshooting

### 1. Check ob Service läuft

```bash
curl http://localhost:9001/api/v2/pimsync/health

# Expected Response:
# {
#   "isHealthy": true,
#   "status": "OK",
#   "recommendations": ["Sync is healthy"]
# }
```

### 2. Letzten Sync Status sehen

```bash
curl http://localhost:9001/api/v2/pimsync/status

# Shows:
# - Last sync time
# - If successful
# - Product count synced
# - Any errors
```

### 3. Test-Sync durchführen

```bash
# Mit allen Providern
curl -X POST http://localhost:9001/api/v2/pimsync/sync

# Oder nur PimCore
curl -X POST http://localhost:9001/api/v2/pimsync/sync?provider=pimcore

# Logs beobachten
tail -f logs/sync.log
```

### 4. Provider Connectivity testen

```bash
# Alle Provider prüfen
curl http://localhost:9001/api/v2/providers/health

# Spezifischen Provider testen
curl -X POST http://localhost:9001/api/v2/providers/pimcore/test
```

---

## 🛠️ Häufige Fehler & Lösungen

### Problem: "Connection refused"

```json
Problem:
  curl: (7) Failed to connect to pimcore.example.com port 443

Ursachen:
  1. PimCore URL falsch in appsettings.json
  2. PimCore Service läuft nicht
  3. Firewall blockiert Verbindung
  4. DNS nicht erreichbar

Lösung:
  1. URL verifizieren: https://pimcore.example.com/admin
  2. curl -I https://pimcore.example.com
  3. Firewall-Regeln prüfen
  4. DNS resolver testen
```

### Problem: "401 Unauthorized"

```json
Problem:
  API Key ist falsch oder abgelaufen

Lösungen:
  1. API Key in PimCore regenerieren
  2. Environment Variable korrekt setzen:
     export PIMCORE_API_KEY="new_key"
  3. appsettings.json neu laden
  4. Container/Service neustarten
```

### Problem: "Request timeout"

```json
Problem:
  PimCore antwortet zu langsam

Lösungen:
  1. TimeoutMs erhöhen in appsettings.json
  2. Netzwerk-Latenz prüfen (ping)
  3. PimCore API Performance prüfen
  4. Batch-Größe reduzieren
```

### Problem: "Sync is already in progress"

```json
Problem:
  HTTP 409 error beim Starten neuer Sync

Grund:
  Eine andere Sync läuft noch

Lösung:
  1. Warten bis aktuelle Sync fertig
  2. Status prüfen: GET /api/v2/pimsync/status
  3. Bei Deadlock: Service neu starten
```

---

## 📊 Monitoring Setup

### Prometheus Integration

```csharp
// Custom Metrics
public class PimSyncMetrics
{
    private static readonly Counter SyncCounter = Counter
        .Create("pimsync_total", "Total sync operations");
    
    private static readonly Gauge ProductsGauge = Gauge
        .Create("pimsync_products_total", "Products indexed");
    
    private static readonly Histogram SyncDuration = Histogram
        .Create("pimsync_duration_seconds", "Sync duration");
}
```

### ElasticSearch Cluster Health

```bash
# Index Status
curl http://elasticsearch:9200/_cluster/health

# Document Count
curl http://elasticsearch:9200/products_de/_count
curl http://elasticsearch:9200/products_en/_count
curl http://elasticsearch:9200/products_fr/_count
```

### Logging Aggregation

```bash
# Alle Sync-Logs anschauen
grep "PimSyncService" logs/*.log

# Nur Fehler
grep "ERROR.*PimSync" logs/*.log

# Mit Timestamps
tail -f logs/sync.log | grep "ERROR"
```

---

## 🚀 Deployment Steps

### 1. Code-Integration

```bash
cd backend/services/CatalogService
# Services sollten bereits erstellt sein
ls -la src/Services/PimSyncService.cs
ls -la src/Workers/PimSyncWorker.cs
ls -la src/Controllers/PimSyncController.cs
```

### 2. Program.cs Update

```csharp
// In Startup.cs oder Program.cs
builder.Services.AddProductProviders(builder.Configuration);
builder.Services.AddPimSync(builder.Configuration);  // ← Add this line
```

### 3. appsettings.json Update

```bash
# Backup erstellen
cp appsettings.json appsettings.json.backup

# Neue Settings hinzufügen
# (siehe Szenarien oben)
```

### 4. Environment Variables

```bash
# .env datei oder CloudFormation/K8s ConfigMap
PIMCORE_API_KEY=xxx
NEXPIM_API_KEY=yyy
OXOMI_API_KEY=zzz
```

### 5. Build & Test

```bash
# Build
dotnet build

# Test
dotnet test

# Run
dotnet run

# Test Endpoints
curl http://localhost:9001/api/v2/pimsync/health
```

### 6. Health Checks

```bash
# Service Status
curl http://localhost:9001/api/v2/pimsync/health

# Sync Status
curl http://localhost:9001/api/v2/pimsync/status

# Provider Health
curl http://localhost:9001/api/v2/providers/health
```

---

## 📚 Best Practices

✅ **DO**:
- Regelmäßige Health Checks durchführen
- API Keys in Environment Variables halten
- Monitoring & Alerting einrichten
- Sync-Zeiten für große Kataloge ausreichend dimensionieren
- Error Logs regelmäßig prüfen
- Provider Priorities vernünftig setzen

❌ **DON'T**:
- API Keys in Code/Config files hardcoden
- Sync Interval zu kurz setzen (< 5 Minuten)
- Sehr große BatchSize (max 100-200)
- Sync während Peak-Zeiten starten
- Provider Prioritäten willkürlich ändern

---

**Status**: ✅ Bereit für Produktion!
