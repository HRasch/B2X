# ⚡ Quartz Scheduler Quick Start

---

## 3-Schritt Installation

### 1️⃣ NuGet Pakete
```bash
dotnet add package Quartz
dotnet add package Quartz.Extensions.Hosting
```

### 2️⃣ Program.cs
```csharp
// ERSETZEN SIE:
// builder.Services.AddPimSync(builder.Configuration);

// MIT:
builder.Services.AddPimSyncWithQuartz(builder.Configuration);
```

### 3️⃣ appsettings.json
```json
{
  "PimSync": {
    "Enabled": true,
    "CronExpression": "0 2 * * *"
  }
}
```

**Fertig!** ✅

---

## 📊 Dashboard API

```bash
# Get complete dashboard
curl http://localhost:9001/api/v2/pimsync/dashboard

# Response zeigt:
# - Active syncs with progress bars
# - Latest sync status
# - Statistics (success rate, total products, etc)
```

---

## 🔄 Cron Expression Beispiele

```
"0 2 * * *"       → Täglich 02:00 Uhr
"0 1 * * 1"       → Montags 01:00 Uhr
"0 0 */6 * * *"   → Alle 6 Stunden
"0 */30 * * * *"  → Alle 30 Minuten
"0 * * * * *"     → Jede Minute (nur Test!)
```

---

## 📡 API Endpoints

```
GET  /api/v2/pimsync/progress/{syncRunId}      # Einzelnen Sync abrufen
GET  /api/v2/pimsync/progress/active          # Aktive Syncs
GET  /api/v2/pimsync/progress/latest          # Letzten Sync
GET  /api/v2/pimsync/progress/history         # Historie
GET  /api/v2/pimsync/dashboard                # Dashboard (WICHTIG!)
```

---

## 💡 Progress Properties

```csharp
SyncProgressModel {
  syncRunId,              // Unique ID
  status,                 // Queued, Running, Completed, Failed
  progressPercentage,     // 0-100
  productsProcessed,      // Count
  productsIndexed,        // Count
  productsFailed,         // Count
  estimatedTimeRemaining, // TimeSpan
  duration,               // TimeSpan
  currentLanguage,        // "de", "en", "fr"
}
```

---

## 🎯 Admin Dashboard (Vue/React)

```javascript
const loadDashboard = async () => {
  const res = await fetch('/api/v2/pimsync/dashboard')
  const dashboard = await res.json()
  
  // Display active syncs with progress
  dashboard.activeSyncs.forEach(sync => {
    console.log(`${sync.providerName}: ${sync.progressPercentage}%`)
    console.log(`ETA: ${sync.estimatedTimeRemaining}`)
  })
  
  // Display statistics
  console.log(`Success Rate: ${dashboard.statistics.successRate}%`)
}

// Poll every 5 seconds
setInterval(loadDashboard, 5000)
```

---

## ✅ Verification

```bash
dotnet build      # Sollte erfolgreich sein
dotnet run        # App sollte starten

# In neuem Terminal:
curl http://localhost:9001/api/v2/pimsync/dashboard

# Sollte JSON zurückgeben (even if empty)
```

---

## 🐛 Falls es nicht funktioniert

| Problem | Lösung |
|:-------:|:-------:|
| `Build failed` | `dotnet add package Quartz` |
| `scheduler not starting` | Check `"PimSync:Enabled": true` |
| `404 on /api/v2/pimsync/dashboard` | Restart app |
| `Old backgroudservice still running` | Remove old `AddPimSync()` |

---

## 📋 Files Erstellt/Geändert

```
✅ src/Models/SyncProgressModel.cs
✅ src/Services/SyncProgressService.cs
✅ src/Jobs/PimSyncJob.cs
✅ src/Controllers/PimSyncProgressController.cs
✅ src/Extensions/PimSyncQuartzExtensions.cs
✅ src/Extensions/PimSyncServiceExtensions.cs
```

---

## 🚀 Das ist alles!

**Quartz ist jetzt bereit!**

Nächster Schritt: Implementieren Sie das Admin Dashboard Frontend zur Anzeige des `/api/v2/pimsync/dashboard` Endpoints.

---

**Dokumentation**: Siehe `QUARTZ_SCHEDULER_DOCUMENTATION.md`
