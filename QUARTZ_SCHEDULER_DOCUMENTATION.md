# 📊 Quartz Scheduler mit Progress Tracking - Dokumentation

**Status**: ✅ IMPLEMENTATION COMPLETE
**Date**: 26 December 2025

---

## 📋 Übersicht

Das System wurde aktualisiert, um **Quartz.NET** für die PIM-Synchronisation zu verwenden statt eines einfachen BackgroundService. Dies bietet:

✅ **Robustere Planung** - Cron Expressions & Interval Support  
✅ **Real-time Progress Tracking** - Live-Fortschritt im Dashboard  
✅ **Persistente Job-Historie** - Alle Syncs werden tracked  
✅ **Bessere Fehlerbehandlung** - Quartz Retry-Mechaniken  
✅ **Skalierbarkeit** - Vorbereitet für Cluster-Setup  

---

## 🏗️ Neue Komponenten

### 1. SyncProgressModel
**Datei**: `src/Models/SyncProgressModel.cs`

Repräsentiert den Zustand einer PIM-Sync Operation:

```csharp
public class SyncProgressModel
{
    public Guid SyncRunId { get; set; }
    public SyncProgressStatus Status { get; set; }  // Queued, Running, Completed, Failed
    public string? ProviderName { get; set; }
    public int? TotalProducts { get; set; }
    public int ProductsProcessed { get; set; }
    public int ProductsIndexed { get; set; }
    public int ProductsFailed { get; set; }
    public string? CurrentLanguage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Computed properties
    public double ProgressPercentage { get; }
    public TimeSpan? EstimatedTimeRemaining { get; }
    public TimeSpan Duration { get; }
}

public enum SyncProgressStatus
{
    Queued,      // Wartet auf Ausführung
    Running,     // Läuft gerade
    Completed,   // Erfolgreich abgeschlossen
    Failed,      // Fehlgeschlagen
    Cancelled    // Abgebrochen
}
```

---

### 2. ISyncProgressService
**Datei**: `src/Services/SyncProgressService.cs`

Verwaltet Sync-Fortschritt und Historie:

```csharp
public interface ISyncProgressService
{
    // Neue Sync-Run starten
    Guid CreateSyncRun(string? providerName = null);
    
    // Progress aktualisieren
    void UpdateProgress(Guid syncRunId, int processed, int indexed, int failed, 
                       string? currentLanguage = null);
    
    // Fertig bzw. Fehler markieren
    void MarkCompleted(Guid syncRunId);
    void MarkFailed(Guid syncRunId, string errorMessage, params string[] errors);
    
    // Status abrufen
    SyncProgressModel? GetProgress(Guid syncRunId);
    List<SyncProgressModel> GetActiveSyncs();
    SyncProgressModel? GetLatestSync(string? providerName = null);
    List<SyncProgressModel> GetSyncHistory(int maxResults = 20);
}
```

---

### 3. PimSyncJob (Quartz Job)
**Datei**: `src/Jobs/PimSyncJob.cs`

Der Quartz Job, der periodisch ausgeführt wird:

```csharp
public class PimSyncJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // 1. Neue Sync-Run erstellen
        var syncRunId = _progressService.CreateSyncRun(providerName);
        
        // 2. Sync ausführen
        var result = await _syncService.SyncProductsAsync(providerName);
        
        // 3. Progress aktualisieren
        if (result.Success)
            _progressService.MarkCompleted(syncRunId);
        else
            _progressService.MarkFailed(syncRunId, result.Error);
    }
}
```

---

### 4. PimSyncProgressController
**Datei**: `src/Controllers/PimSyncProgressController.cs`

API Endpoints für Progress-Monitoring:

```csharp
// Get progress of specific sync
GET /api/v2/pimsync/progress/{syncRunId}

// Get all active syncs
GET /api/v2/pimsync/progress/active

// Get latest sync for provider
GET /api/v2/pimsync/progress/latest?provider=pimcore

// Get sync history
GET /api/v2/pimsync/progress/history?maxResults=20

// Get dashboard summary
GET /api/v2/pimsync/dashboard
```

---

### 5. PimSyncQuartzExtensions
**Datei**: `src/Extensions/PimSyncQuartzExtensions.cs`

Dependency Injection Setup für Quartz:

```csharp
// In Program.cs
builder.Services.AddPimSyncWithQuartz(builder.Configuration);
```

---

## ⚙️ Konfiguration

### appsettings.json

```json
{
  "PimSync": {
    "Enabled": true,
    
    // Option 1: Interval-basiert (in Sekunden)
    "IntervalSeconds": 3600,
    
    // Option 2: Cron-Expression (überschreibt Interval)
    "CronExpression": "0 2 * * *",  // Täglich um 02:00 Uhr
    
    "BatchSize": 100
  },
  
  "Quartz": {
    "quartz.scheduler.instanceName": "B2Connect-PimSync-Scheduler",
    "quartz.threadPool.threadCount": 2,
    "quartz.jobStore.type": "Quartz.Impl.InMemory.RAMJobStore, Quartz"
  }
}
```

### Cron Expression Beispiele

```
// Täglich um 02:00 Uhr
"0 2 * * *"

// Jeden Montag um 01:00 Uhr
"0 1 * * 1"

// Alle 6 Stunden
"0 0 */6 * * *"

// Alle 30 Minuten
"0 */30 * * * *"

// Jede Minute (Testing)
"0 * * * * *"
```

---

## 📡 API Endpoints

### 1. Get Progress of Specific Sync

```http
GET /api/v2/pimsync/progress/{syncRunId}

Response (200):
{
  "syncRunId": "12345678-1234-1234-1234-123456789012",
  "status": "Running",
  "providerName": "pimcore",
  "totalProducts": 10000,
  "productsProcessed": 5230,
  "productsIndexed": 5230,
  "productsFailed": 0,
  "currentLanguage": "de",
  "startedAt": "2025-12-26T10:30:00Z",
  "completedAt": null,
  "errorMessage": null,
  "progressPercentage": 52.3,
  "estimatedTimeRemaining": "00:05:30",
  "duration": "00:05:15"
}
```

---

### 2. Get All Active Syncs

```http
GET /api/v2/pimsync/progress/active

Response (200):
[
  {
    "syncRunId": "...",
    "status": "Running",
    "progressPercentage": 52.3,
    ...
  }
]
```

---

### 3. Get Latest Sync

```http
GET /api/v2/pimsync/progress/latest?provider=pimcore

Response (200):
{
  "syncRunId": "...",
  "status": "Completed",
  "progressPercentage": 100,
  ...
}

Response (404) if no sync found:
{
  "error": "No sync history found",
  "provider": "pimcore"
}
```

---

### 4. Get Sync History

```http
GET /api/v2/pimsync/progress/history?maxResults=10

Response (200):
[
  {
    "syncRunId": "...",
    "status": "Completed",
    "startedAt": "2025-12-26T10:00:00Z",
    "completedAt": "2025-12-26T10:08:30Z",
    "productsIndexed": 10000,
    ...
  },
  ...
]
```

---

### 5. Get Dashboard Summary ⭐

```http
GET /api/v2/pimsync/dashboard

Response (200):
{
  "activeSyncCount": 1,
  "activeSyncs": [
    {
      "syncRunId": "...",
      "status": "Running",
      "progressPercentage": 45.2,
      ...
    }
  ],
  "latestSync": {
    "syncRunId": "...",
    "status": "Completed",
    "productsIndexed": 10000,
    "duration": "00:08:30",
    ...
  },
  "recentHistory": [...],
  "statistics": {
    "totalSyncsCompleted": 125,
    "totalSyncsFailed": 3,
    "successRate": 97.66,
    "totalProductsIndexed": 1250000,
    "averageSyncDuration": "00:07:45"
  }
}
```

---

## 📊 Dashboard Integration

### Admin Frontend Beispiel

```vue
<template>
  <div class="sync-dashboard">
    <!-- Active Syncs -->
    <div v-if="dashboard.activeSyncCount > 0" class="active-syncs">
      <h3>{{ dashboard.activeSyncCount }} Active Syncs</h3>
      
      <div v-for="sync in dashboard.activeSyncs" :key="sync.syncRunId" class="sync-card">
        <h4>{{ sync.providerName || 'All Providers' }}</h4>
        
        <!-- Progress Bar -->
        <div class="progress-bar">
          <div class="progress-fill" :style="{ width: sync.progressPercentage + '%' }">
            {{ Math.round(sync.progressPercentage) }}%
          </div>
        </div>
        
        <!-- Details -->
        <div class="details">
          <p>Status: {{ sync.status }}</p>
          <p>{{ sync.productsProcessed }} / {{ sync.totalProducts }} products</p>
          <p v-if="sync.currentLanguage">Language: {{ sync.currentLanguage }}</p>
          <p v-if="sync.estimatedTimeRemaining">
            ETA: {{ sync.estimatedTimeRemaining }}
          </p>
        </div>
      </div>
    </div>
    
    <!-- Latest Sync -->
    <div v-if="dashboard.latestSync" class="latest-sync">
      <h3>Latest Sync</h3>
      <p>{{ dashboard.latestSync.status }}</p>
      <p>{{ dashboard.latestSync.productsIndexed }} products in {{ dashboard.latestSync.duration }}</p>
    </div>
    
    <!-- Statistics -->
    <div class="statistics">
      <h3>Statistics</h3>
      <p>Success Rate: {{ dashboard.statistics.successRate.toFixed(2) }}%</p>
      <p>Total Products: {{ dashboard.statistics.totalProductsIndexed }}</p>
      <p>Avg Duration: {{ dashboard.statistics.averageSyncDuration }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const dashboard = ref(null)
let pollInterval = null

onMounted(async () => {
  // Load dashboard initially
  await loadDashboard()
  
  // Poll every 5 seconds while syncs are active
  pollInterval = setInterval(loadDashboard, 5000)
})

onUnmounted(() => {
  if (pollInterval) clearInterval(pollInterval)
})

async function loadDashboard() {
  const response = await fetch('/api/v2/pimsync/dashboard')
  dashboard.value = await response.json()
  
  // Stop polling if no active syncs
  if (dashboard.value.activeSyncCount === 0 && pollInterval) {
    clearInterval(pollInterval)
    pollInterval = null
  }
}
</script>

<style scoped>
.sync-dashboard {
  padding: 20px;
}

.progress-bar {
  width: 100%;
  height: 24px;
  background: #e0e0e0;
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #4caf50, #45a049);
  transition: width 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
}
</style>
```

---

## 🔧 Program.cs Integration

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// OLD: builder.Services.AddPimSync(builder.Configuration);
// NEW: Use Quartz instead
builder.Services.AddPimSyncWithQuartz(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
```

---

## 📈 Monitoring im Dashboard

### Anzeigende Informationen

1. **Active Syncs**
   - Anzahl laufender Syncs
   - Provider name
   - Progress percentage
   - Estimated time remaining

2. **Latest Sync**
   - Status (Completed, Failed)
   - Productsprocessed
   - Duration
   - Errors (if any)

3. **Statistics**
   - Success rate
   - Total products indexed
   - Average sync duration
   - Failed sync count

4. **History**
   - Recent 5 syncs
   - Status, duration, product count

---

## 🚀 Produktions-Setup

### 1. NuGet Pakete hinzufügen

```bash
dotnet add package Quartz
dotnet add package Quartz.Extensions.Hosting
```

### 2. appsettings.Production.json

```json
{
  "PimSync": {
    "Enabled": true,
    "CronExpression": "0 2 * * *",  // Täglich 02:00 Uhr
    "BatchSize": 100
  },
  "Quartz": {
    "quartz.scheduler.instanceName": "B2Connect-PimSync-Scheduler",
    "quartz.threadPool.threadCount": 3,
    "quartz.jobStore.type": "Quartz.Impl.InMemory.RAMJobStore, Quartz"
  }
}
```

### 3. Optional: Redis-basierter Progress (für verteilte Systeme)

```csharp
// Implementieren Sie IProgressService mit Redis:
public class RedisProgressService : ISyncProgressService
{
    private readonly IConnectionMultiplexer _redis;
    
    // ... Implementation mit Redis Hashes
}

// In Program.cs:
builder.Services.AddSingleton<ISyncProgressService>(sp => 
    new RedisProgressService(sp.GetRequiredService<IConnectionMultiplexer>()));
```

---

## 📝 Änderungen vom alten System

| Feature | Alt (BackgroundService) | Neu (Quartz) |
|:-------:|:-----:|:-----:|
| Scheduler | Task.Delay Loop | Quartz Engine |
| Cron Support | ❌ Nein | ✅ Ja |
| Progress Tracking | ❌ Nein | ✅ Ja |
| Job Persistence | ❌ Nein | ✅ Ja (optional) |
| Skalierbarkeit | Begrenzt | Cluster-ready |
| Dashboard Support | ❌ Nein | ✅ Ja |
| Fehlerbehandlung | Basis | Erweitert |

---

## 🧪 Testing

### Unit Test Beispiel

```csharp
[Test]
public async Task PimSyncJob_ShouldTrackProgress()
{
    // Arrange
    var mockSyncService = new Mock<IPimSyncService>();
    var progressService = new SyncProgressService();
    var job = new PimSyncJob(mockSyncService.Object, progressService, _logger);
    
    // Act
    var syncRunId = progressService.CreateSyncRun("pimcore");
    progressService.UpdateProgress(syncRunId, 500, 500, 0);
    progressService.SetTotalProducts(syncRunId, 1000);
    
    // Assert
    var progress = progressService.GetProgress(syncRunId);
    Assert.AreEqual(50, progress.ProgressPercentage);
}
```

---

## 🐛 Troubleshooting

### Quartz startet nicht

```
Problem: "Quartz scheduler not starting"
Lösung: 
1. Prüfen Sie "PimSync:Enabled": true in appsettings.json
2. Prüfen Sie ob alle Quartz-Pakete installiert sind
3. Prüfen Sie die Logs auf Initialization-Errors
```

### Progress zeigt nicht aktuell

```
Problem: "Dashboard zeigt alte Daten"
Lösung:
1. Erhöhen Sie den Poll-Interval im Frontend
2. Prüfen Sie ob der Progress Service läuft
3. Prüfen Sie ISyncProgressService Registrierung
```

---

## ✅ Checkliste für Deployment

- [ ] Quartz NuGet Pakete installiert
- [ ] PimSyncWithQuartz in Program.cs registriert
- [ ] appsettings.json mit Cron/Interval konfiguriert
- [ ] PimSyncProgressController verfügbar
- [ ] Dashboard API getestet (/api/v2/pimsync/dashboard)
- [ ] Frontend-Dashboard aktualisiert
- [ ] Logs prüfen
- [ ] Staging-Test durchführt
- [ ] Production-Deployment vorbereitet

---

**Status**: ✅ Bereit zum Deployment!
