# Quartz Scheduler Integration - Complete Setup Guide

## ✅ What Has Been Delivered

### Backend Components (6 Files)
1. **SyncProgressModel.cs** - Data model for tracking sync state
2. **SyncProgressService.cs** - Thread-safe progress tracking service  
3. **PimSyncJob.cs** - Quartz job implementation
4. **PimSyncProgressController.cs** - REST API (6 endpoints)
5. **PimSyncQuartzExtensions.cs** - Dependency injection setup
6. **PimSyncServiceExtensions.cs** - Helper extension methods

### Frontend Components
1. **PimSyncDashboard.vue** - Complete Vue 3 dashboard component

### Documentation
1. **QUARTZ_SCHEDULER_DOCUMENTATION.md** - Complete reference
2. **QUARTZ_IMPLEMENTATION_SUMMARY.md** - Implementation checklist
3. **QUARTZ_QUICK_START.md** - Quick reference guide

---

## 🚀 Step-by-Step Integration

### Step 1: Install NuGet Packages
**Time: 2 minutes**

```bash
cd backend/services/CatalogService
dotnet add package Quartz
dotnet add package Quartz.Extensions.Hosting
```

### Step 2: Update Program.cs
**Time: 1 minute**

**File:** `backend/services/CatalogService/Program.cs`

Find this line:
```csharp
// OLD - Remove this:
// builder.Services.AddPimSync(builder.Configuration);
```

Replace with:
```csharp
// NEW - Add this:
builder.Services.AddPimSyncWithQuartz(builder.Configuration);
```

### Step 3: Configure appsettings.json
**Time: 2 minutes**

**File:** `backend/services/CatalogService/appsettings.json`

Add this section:
```json
{
  "PimSync": {
    "Enabled": true,
    "CronExpression": "0 2 * * *"
  }
}
```

**Cron Examples:**
- `0 2 * * *` - Daily at 2 AM
- `0 */4 * * *` - Every 4 hours
- `0 0 * * 0` - Every Sunday at midnight
- `*/15 * * * *` - Every 15 minutes (for testing)

**OR use Interval (in seconds):**
```json
{
  "PimSync": {
    "Enabled": true,
    "IntervalSeconds": 3600
  }
}
```

### Step 4: Verify Build and Run
**Time: 5 minutes**

```bash
cd backend/services/CatalogService

# Clean and build
dotnet clean
dotnet build

# Run
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:9001
```

### Step 5: Test Dashboard API
**Time: 2 minutes**

```bash
# In new terminal:
curl http://localhost:9001/api/v2/pimsync/dashboard | jq

# Expected response (example):
{
  "activeSyncCount": 0,
  "activeSyncs": [],
  "latestSync": null,
  "recentHistory": [],
  "statistics": {
    "totalSyncsCompleted": 0,
    "totalSyncsFailed": 0,
    "successRate": 0.0,
    "totalProductsIndexed": 0,
    "averageSyncDuration": "00:00:00"
  }
}
```

### Step 6: Integrate Frontend Dashboard
**Time: 30 minutes**

**File:** `frontend-admin/src/views/PimSyncDashboardPage.vue`

```vue
<template>
  <div class="page">
    <h1>PIM Sync Management</h1>
    <PimSyncDashboard />
  </div>
</template>

<script setup>
import PimSyncDashboard from '@/components/PimSyncDashboard.vue'
</script>
```

**Add Route (router/index.ts):**
```typescript
{
  path: '/pimsync-dashboard',
  name: 'PimSyncDashboard',
  component: () => import('@/views/PimSyncDashboardPage.vue'),
  meta: { requiresAuth: true, admin: true }
}
```

**Add Menu Item:**
```vue
<RouterLink to="/pimsync-dashboard" class="menu-item">
  📊 PIM Sync Dashboard
</RouterLink>
```

---

## 📊 API Endpoints Overview

### 1. Get Single Sync Progress
```
GET /api/v2/pimsync/progress/{syncRunId}
```
**Response:** Individual sync progress details

### 2. Get Active Syncs
```
GET /api/v2/pimsync/progress/active
```
**Response:** List of currently running syncs

### 3. Get Latest Sync
```
GET /api/v2/pimsync/progress/latest?provider=SAP
```
**Response:** Most recent sync for provider (optional filter)

### 4. Get Sync History
```
GET /api/v2/pimsync/progress/history?maxResults=20
```
**Response:** List of completed syncs (default: 20)

### 5. Get Dashboard Summary ⭐
```
GET /api/v2/pimsync/dashboard
```
**Response:** Complete dashboard data (recommended endpoint)

---

## 🎯 Key Features

### Progress Tracking
- ✅ Real-time percentage (0-100%)
- ✅ Products processed, indexed, failed
- ✅ Current language being synced
- ✅ Estimated time remaining (ETA)
- ✅ Total sync duration

### Scheduling Options
- ✅ Cron expressions (daily, hourly, custom)
- ✅ Interval-based (every X seconds)
- ✅ Multiple providers support
- ✅ Cluster-ready (Quartz distributed)

### Dashboard Features
- ✅ Real-time sync monitoring
- ✅ Progress bars with percentage
- ✅ Statistics (success rate, total indexed)
- ✅ Sync history table
- ✅ Error tracking and details
- ✅ Auto-refresh every 5 seconds (when syncing)

---

## 🔧 Configuration Reference

### PimSync Configuration Section

```json
{
  "PimSync": {
    "Enabled": true,
    "CronExpression": "0 2 * * *",
    "IntervalSeconds": 3600,
    "TimeZoneId": "Europe/Berlin",
    "MaxHistoryRecords": 100,
    "IncludeDetailedErrors": true
  }
}
```

**Parameters:**
- `Enabled` - Enable/disable Quartz scheduler
- `CronExpression` - Cron format schedule (takes precedence over IntervalSeconds)
- `IntervalSeconds` - Fallback interval in seconds
- `TimeZoneId` - Timezone for Cron (optional, defaults to UTC)
- `MaxHistoryRecords` - Maximum sync records to keep in memory
- `IncludeDetailedErrors` - Include detailed error arrays in response

---

## 🧪 Testing Checklist

### Unit Tests Example
```csharp
[TestClass]
public class PimSyncQuartzTests
{
    private SyncProgressService _progressService;
    
    [TestInitialize]
    public void Setup()
    {
        _progressService = new SyncProgressService();
    }
    
    [TestMethod]
    public void CreateSyncRun_ShouldReturnValidGuid()
    {
        var syncId = _progressService.CreateSyncRun("TestProvider");
        Assert.IsNotEqual(Guid.Empty, syncId);
    }
    
    [TestMethod]
    public void UpdateProgress_ShouldCalculatePercentageCorrectly()
    {
        var syncId = _progressService.CreateSyncRun("TestProvider");
        _progressService.SetTotalProducts(syncId, 100);
        _progressService.UpdateProgress(syncId, 50, 50, 0, "de");
        
        var progress = _progressService.GetProgress(syncId);
        Assert.AreEqual(50.0, progress.ProgressPercentage);
    }
    
    [TestMethod]
    public void Thread_SafeAccess_NoExceptions()
    {
        var tasks = new List<Task>();
        var syncId = _progressService.CreateSyncRun("TestProvider");
        _progressService.SetTotalProducts(syncId, 1000);
        
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _progressService.UpdateProgress(syncId, j, j, 0, "de");
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        Assert.IsNotNull(_progressService.GetProgress(syncId));
    }
}
```

### Manual Testing Checklist
- [ ] Dashboard API returns valid JSON
- [ ] Progress percentage updates correctly
- [ ] ETA calculation is accurate
- [ ] Sync history bounded at max records
- [ ] Concurrent API requests don't cause errors
- [ ] Frontend dashboard loads without errors
- [ ] Real-time progress updates every 5 seconds
- [ ] Error messages display correctly
- [ ] Cron schedule executes at correct times

---

## 🚨 Troubleshooting

### Problem: "Quartz not found"
**Solution:**
```bash
dotnet add package Quartz
dotnet add package Quartz.Extensions.Hosting
dotnet restore
```

### Problem: Dashboard shows empty/old data
**Solution:**
```bash
# Force refresh in browser
curl -X GET http://localhost:9001/api/v2/pimsync/dashboard
```

### Problem: Sync not executing on schedule
**Solution:**
1. Check appsettings.json has `"Enabled": true`
2. Verify Cron expression: https://crontab.guru
3. Check logs for Quartz errors
4. Restart application

### Problem: Frontend shows "Loading..." forever
**Solution:**
1. Check browser console for fetch errors
2. Verify API endpoint is accessible
3. Check CORS settings if cross-origin
4. Verify backend is running

### Problem: ETA always shows "Calculating..."
**Solution:**
1. This is normal for first sync (no history)
2. Wait for a few syncs to complete
3. Check that timestamps are recording correctly

---

## 📈 Performance Optimization

### For High-Volume Syncs (100K+ products)
1. Increase `MaxHistoryRecords` carefully
2. Consider Redis backend for distributed systems
3. Use interval scheduling instead of Cron (more efficient)

### For Low-Latency Dashboards
1. Reduce polling interval in Vue component (default 5s)
2. Consider WebSocket for real-time updates
3. Add caching layer for statistics

### Memory Management
- Default: Max 100 sync records in memory
- Adjust via `MaxHistoryRecords` in config
- Completed syncs move to history (not kept in active list)

---

## 🔐 Security Considerations

### API Endpoint Protection
Add to controller:
```csharp
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/v2/pimsync")]
public class PimSyncProgressController : ControllerBase
{
    // ...endpoints protected
}
```

### Dashboard Access Control
```vue
<template v-if="user.isAdmin">
  <PimSyncDashboard />
</template>
```

---

## 📱 Deployment

### Docker Build
```dockerfile
# In Dockerfile for CatalogService
RUN dotnet add package Quartz
RUN dotnet add package Quartz.Extensions.Hosting
```

### Kubernetes Deployment
Quartz works in Kubernetes with proper pod anti-affinity configuration:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: catalog-service
spec:
  replicas: 2
  affinity:
    podAntiAffinity:
      preferredDuringSchedulingIgnoredDuringExecution:
      - weight: 100
        podAffinityTerm:
          labelSelector:
            matchExpressions:
            - key: app
              operator: In
              values:
              - catalog-service
          topologyKey: kubernetes.io/hostname
```

---

## 📞 Support & Next Steps

### What's Included
✅ 6 backend code files (ready to use)  
✅ 1 Vue 3 dashboard component (ready to use)  
✅ 3 comprehensive documentation files  
✅ Cron configuration support  
✅ Progress tracking API  
✅ Thread-safe implementation  

### What You Need To Do
1. Install NuGet packages (2 min)
2. Update Program.cs (1 min)
3. Configure appsettings.json (2 min)
4. Integrate frontend dashboard (30 min)
5. Deploy and test (1 hour)

**Total Time: ~2 hours**

---

## 📚 Additional Resources

- **Cron Expression Helper:** https://crontab.guru
- **Quartz.NET Documentation:** https://www.quartz-scheduler.net/
- **Vue 3 Guide:** https://vuejs.org/guide/
- **Your Project Docs:** See QUARTZ_SCHEDULER_DOCUMENTATION.md

---

**Happy syncing! 🚀**
