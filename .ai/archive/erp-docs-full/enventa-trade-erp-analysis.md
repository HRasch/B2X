# enventa Trade ERP - Technische Analyse

**Status:** Draft  
**Date:** 2026-01-02  
**Owner:** @Architect, @Backend  
**Source:** eGate Broker Implementation (https://github.com/NissenVelten/eGate)

---

## Executive Summary

Diese Analyse dokumentiert die technischen Erkenntnisse aus der eGate Broker-Implementierung für enventa Trade ERP. Die wichtigste Erkenntnis ist, dass **enventa Trade ERP nicht multithreading-fähig** ist und alle Datenzugriffe serialisiert werden müssen.

---

## 1. Kritische Einschränkungen

### 1.1 Kein Multithreading Support

**⚠️ KRITISCH: enventa ERP Assemblies sind NICHT thread-safe!**

```csharp
// Aus eGate: Login muss serialisiert werden
lock (_lock) 
{ 
    global.Login(_identity); 
}
```

**Implikationen:**
- Alle ERP-Operationen müssen über einen Single-Thread laufen
- Parallele Anfragen müssen in einer Queue serialisiert werden
- Connection-Pooling muss thread-safe implementiert werden
- Pro Tenant ist nur EIN aktiver ERP-Zugriff gleichzeitig möglich

### 1.2 Proprietärer ORM (FrameworkSystems)

enventa nutzt einen eigenen ORM namens "FrameworkSystems" mit folgenden Eigenschaften:
- Keine Entity Framework Kompatibilität
- Eigene Query-Syntax (`FSQueryList<T>`)
- Scope-basiertes Transaktionsmanagement
- Statische Factory-Pattern für Connections

### 1.3 Windows-Only / .NET Framework 4.8

- Keine .NET Core/.NET 5+ Unterstützung
- Windows Server erforderlich
- COM-Interop Abhängigkeiten möglich

---

## 2. eGate Architektur-Analyse

### 2.1 Schichtenmodell

```
┌─────────────────────────────────────────────────────────────────┐
│                        eGate Application                         │
├─────────────────────────────────────────────────────────────────┤
│  NVContext (Repository Container)                                │
│  - 50+ Lazy-Loaded Repositories                                  │
│  - Pro Operation: NVArticleRepository, NVOrderRepository, etc.   │
├─────────────────────────────────────────────────────────────────┤
│  QueryBuilder Layer                                              │
│  - NVQueryBuilderBase<TNVEntity, TFSEntity>                     │
│  - Fluent API für Queries                                        │
│  - Type-safe Expressions                                         │
├─────────────────────────────────────────────────────────────────┤
│  FSRepository (Data Access)                                      │
│  - FSScope für Connection Management                             │
│  - FSUtil für Utility Operations                                 │
├─────────────────────────────────────────────────────────────────┤
│  FSGlobalPool (Connection Pool)                                  │
│  - ConcurrentDictionary<string, Lazy<FSGlobalPool>>             │
│  - EventWaitHandle für Thread-Synchronization                    │
│  - Pro Tenant-Identity separater Pool                            │
├─────────────────────────────────────────────────────────────────┤
│  FrameworkSystems.* (Proprietary ORM)                            │
│  - IcECArticle, IcECOrder, IcECCustomer                         │
│  - Direkte Assembly-Referenzen                                   │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 Connection Pool Pattern

```csharp
// eGate FSGlobalPool Implementation
public class FSGlobalPool
{
    private readonly ConcurrentBag<IFSGlobalObjects> _pool;
    private readonly EventWaitHandle _waitHandle;
    private readonly object _lock = new object();
    
    public IFSGlobalObjects Acquire()
    {
        IFSGlobalObjects global;
        
        // Try to get from pool
        if (_pool.TryTake(out global))
            return global;
            
        // Wait for available connection
        _waitHandle.WaitOne();
        
        // Create new if pool empty
        lock (_lock)
        {
            global = CreateNew();
            global.Login(_identity);  // NOT thread-safe!
        }
        
        return global;
    }
    
    public void Release(IFSGlobalObjects global)
    {
        global.Logout();  // Clean state before return
        _pool.Add(global);
        _waitHandle.Set();
    }
}
```

### 2.3 QueryBuilder Pattern

```csharp
// eGate NVArticleQueryBuilder
public class NVArticleQueryBuilder : NVQueryBuilderBase<NVArticle, IcECArticle>
{
    public NVArticleQueryBuilder ByCategory(string categoryId)
    {
        _conditions.Add(q => q.CategoryId.Eq(categoryId));
        return this;
    }
    
    public NVArticleQueryBuilder WithPricing(bool includePrices)
    {
        if (includePrices)
            _includes.Add(nameof(IcECArticle.Prices));
        return this;
    }
    
    public IEnumerable<NVArticle> Execute()
    {
        using var scope = FSUtil.CreateScope();
        var query = BuildQuery();
        
        // Streaming mit yield return
        foreach (var item in query.GetFetchNext())
        {
            yield return MapToNVArticle(item);
        }
    }
}
```

### 2.4 Scope-Pattern für Transaktionen

```csharp
// eGate FSScope Pattern
public class FSScope : IDisposable
{
    private readonly IFSGlobalObjects _global;
    private bool _committed = false;
    
    public FSScope(IFSGlobalObjects global)
    {
        _global = global;
        _global.BeginTransaction();
    }
    
    public void Commit()
    {
        _global.CommitTransaction();
        _committed = true;
    }
    
    public void Dispose()
    {
        if (!_committed)
            _global.RollbackTransaction();
        
        FSGlobalPool.Release(_global);
    }
}

// Verwendung
using (var scope = FSUtil.CreateScope())
{
    // Operations...
    scope.Commit();
}
```

### 2.5 Bulk Operations Pattern

```csharp
// eGate Batch Processing
public class SyncRecordStore
{
    private const int BatchSize = 1000;
    
    public async Task SyncProductsAsync(
        IEnumerable<Product> products,
        IProgress<int> progress,
        CancellationToken ct)
    {
        var batches = products.Batch(BatchSize);
        var processed = 0;
        
        foreach (var batch in batches)
        {
            ct.ThrowIfCancellationRequested();
            
            using var scope = FSUtil.CreateScope();
            
            foreach (var product in batch)
            {
                var article = MapToArticle(product);
                _articleService.Update(article);
            }
            
            scope.Commit();
            processed += batch.Count();
            progress?.Report(processed);
        }
    }
}
```

---

## 3. Threading-Strategie für B2Connect

### 3.1 Problem: Keine Parallelität möglich

Da enventa ERP nicht thread-safe ist, müssen wir eine Serialisierungsstrategie implementieren:

```
┌─────────────────────────────────────────────────────────────────┐
│                    B2Connect Requests                            │
│              (Multiple concurrent requests)                      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Request Queue                                 │
│              (Channel<T> / BlockingCollection)                   │
│              Per-Tenant Queue für Isolation                      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                Single-Threaded Worker                            │
│              (Dedicated Thread per Tenant)                       │
│              Serialisierte ERP-Zugriffe                         │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  enventa ERP Assembly                            │
│              (Thread-unsafe operations)                          │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 Empfohlene Implementierung: Actor Model

```csharp
/// <summary>
/// Thread-safe ERP access using Actor pattern.
/// All ERP operations are serialized through a single worker thread.
/// </summary>
public class EnventaErpActor : IAsyncDisposable
{
    private readonly Channel<ErpOperation> _operationQueue;
    private readonly Task _workerTask;
    private readonly IFSGlobalObjects _erpConnection;
    private readonly CancellationTokenSource _cts;
    
    public EnventaErpActor(NVIdentity identity)
    {
        _operationQueue = Channel.CreateBounded<ErpOperation>(
            new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,  // Critical: only one reader!
                SingleWriter = false
            });
        
        _cts = new CancellationTokenSource();
        _erpConnection = CreateAndLoginConnection(identity);
        _workerTask = Task.Run(() => ProcessOperationsAsync(_cts.Token));
    }
    
    /// <summary>
    /// Enqueue an operation and wait for result.
    /// Thread-safe: can be called from multiple threads.
    /// </summary>
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<IFSGlobalObjects, TResult> operation,
        CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<object>();
        var erpOp = new ErpOperation(
            conn => tcs.SetResult(operation(conn)),
            ex => tcs.SetException(ex));
        
        await _operationQueue.Writer.WriteAsync(erpOp, ct);
        return (TResult)await tcs.Task;
    }
    
    /// <summary>
    /// Single-threaded worker processing all ERP operations.
    /// </summary>
    private async Task ProcessOperationsAsync(CancellationToken ct)
    {
        await foreach (var operation in _operationQueue.Reader.ReadAllAsync(ct))
        {
            try
            {
                // Execute on single thread - thread-safe for enventa
                operation.Execute(_erpConnection);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        _operationQueue.Writer.Complete();
        _cts.Cancel();
        await _workerTask;
        _erpConnection.Logout();
    }
}
```

### 3.3 Tenant-Isolation mit Actor Pool

```csharp
/// <summary>
/// Manages per-tenant ERP actors with thread isolation.
/// </summary>
public class EnventaActorPool : IDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<EnventaErpActor>> _actors;
    private readonly IEnventaConfigProvider _configProvider;
    
    public EnventaActorPool(IEnventaConfigProvider configProvider)
    {
        _configProvider = configProvider;
        _actors = new ConcurrentDictionary<string, Lazy<EnventaErpActor>>();
    }
    
    /// <summary>
    /// Get or create actor for tenant. Thread-safe.
    /// </summary>
    public EnventaErpActor GetActor(string tenantId)
    {
        return _actors.GetOrAdd(
            tenantId,
            id => new Lazy<EnventaErpActor>(() => 
                CreateActor(_configProvider.GetIdentity(id)))
        ).Value;
    }
    
    private EnventaErpActor CreateActor(NVIdentity identity)
    {
        return new EnventaErpActor(identity);
    }
}
```

---

## 4. Datenzugriffs-Design für B2Connect

### 4.1 Repository-Pattern mit Thread-Safety

```csharp
/// <summary>
/// Thread-safe repository wrapping enventa operations.
/// All operations are automatically serialized via actor.
/// </summary>
public class EnventaProductRepository : IProductRepository
{
    private readonly EnventaErpActor _actor;
    private readonly IMapper _mapper;
    
    public EnventaProductRepository(
        EnventaActorPool actorPool,
        TenantContext tenant,
        IMapper mapper)
    {
        _actor = actorPool.GetActor(tenant.TenantId);
        _mapper = mapper;
    }
    
    public async Task<Product> GetByIdAsync(string productId, CancellationToken ct)
    {
        return await _actor.ExecuteAsync(conn =>
        {
            var query = new NVArticleQueryBuilder(conn)
                .ById(productId);
            
            var article = query.Execute().FirstOrDefault();
            return article != null ? _mapper.Map<Product>(article) : null;
        }, ct);
    }
    
    public async Task<IEnumerable<Product>> GetByIdsAsync(
        IEnumerable<string> productIds, 
        CancellationToken ct)
    {
        // Batch to avoid large IN clauses
        var batches = productIds.Batch(100);
        var results = new List<Product>();
        
        foreach (var batch in batches)
        {
            var batchResults = await _actor.ExecuteAsync(conn =>
            {
                var query = new NVArticleQueryBuilder(conn)
                    .ByIds(batch.ToList());
                
                return query.Execute()
                    .Select(a => _mapper.Map<Product>(a))
                    .ToList();
            }, ct);
            
            results.AddRange(batchResults);
        }
        
        return results;
    }
}
```

### 4.2 Streaming mit Backpressure

Da wir nicht parallel streamen können, implementieren wir Backpressure-fähiges Streaming:

```csharp
/// <summary>
/// Stream products with backpressure support.
/// Operations are serialized but results streamed efficiently.
/// </summary>
public async IAsyncEnumerable<Product> StreamProductsAsync(
    ProductFilter filter,
    [EnumeratorCancellation] CancellationToken ct = default)
{
    const int PageSize = 100;
    string continuationToken = null;
    
    do
    {
        // Execute paged query on actor thread
        var page = await _actor.ExecuteAsync(conn =>
        {
            var query = new NVArticleQueryBuilder(conn)
                .ApplyFilter(filter)
                .WithPaging(PageSize, continuationToken);
            
            var items = query.Execute().ToList();
            var nextToken = items.Count == PageSize 
                ? items.Last().Id 
                : null;
            
            return new PagedResult<IcECArticle>(items, nextToken);
        }, ct);
        
        // Stream results outside of actor
        foreach (var article in page.Items)
        {
            yield return _mapper.Map<Product>(article);
        }
        
        continuationToken = page.ContinuationToken;
        
    } while (continuationToken != null);
}
```

### 4.3 Bulk Sync mit Progress Reporting

```csharp
/// <summary>
/// Bulk sync with progress reporting and cancellation support.
/// Uses batching to manage memory and provide checkpoints.
/// </summary>
public async Task<SyncResult> BulkSyncAsync(
    IEnumerable<Product> products,
    IProgress<SyncProgress> progress,
    CancellationToken ct)
{
    var result = new SyncResult();
    var batches = products.Batch(1000);
    var totalProcessed = 0;
    
    foreach (var batch in batches)
    {
        ct.ThrowIfCancellationRequested();
        
        var batchResult = await _actor.ExecuteAsync(conn =>
        {
            using var scope = new FSScope(conn);
            var batchStats = new BatchStats();
            
            foreach (var product in batch)
            {
                try
                {
                    var article = _mapper.Map<IcECArticle>(product);
                    
                    if (ArticleExists(conn, product.Id))
                    {
                        conn.ArticleService.Update(article);
                        batchStats.Updated++;
                    }
                    else
                    {
                        conn.ArticleService.Insert(article);
                        batchStats.Created++;
                    }
                }
                catch (Exception ex)
                {
                    batchStats.Errors.Add(new SyncError(product.Id, ex.Message));
                }
            }
            
            scope.Commit();
            return batchStats;
        }, ct);
        
        result.Merge(batchResult);
        totalProcessed += batch.Count();
        
        progress?.Report(new SyncProgress(totalProcessed, result));
    }
    
    return result;
}
```

---

## 5. Performance-Optimierungen

### 5.1 Request Batching (Coalescing)

Da ERP-Zugriffe serialisiert sind, können wir mehrere Anfragen zu einer Operation zusammenfassen:

```csharp
/// <summary>
/// Batches multiple concurrent requests into single ERP operation.
/// Reduces ERP load when many requests arrive simultaneously.
/// </summary>
public class RequestCoalescer<TKey, TResult>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Dictionary<TKey, TaskCompletionSource<TResult>> _pending = new();
    private readonly TimeSpan _coalesceWindow = TimeSpan.FromMilliseconds(50);
    private readonly Func<IEnumerable<TKey>, Task<Dictionary<TKey, TResult>>> _batchFetcher;
    
    public async Task<TResult> GetAsync(TKey key, CancellationToken ct)
    {
        TaskCompletionSource<TResult> tcs;
        bool shouldFetch = false;
        
        await _semaphore.WaitAsync(ct);
        try
        {
            if (!_pending.TryGetValue(key, out tcs))
            {
                tcs = new TaskCompletionSource<TResult>();
                _pending[key] = tcs;
                shouldFetch = _pending.Count == 1;
            }
        }
        finally
        {
            _semaphore.Release();
        }
        
        if (shouldFetch)
        {
            // Wait for more requests to coalesce
            await Task.Delay(_coalesceWindow, ct);
            await FetchBatchAsync();
        }
        
        return await tcs.Task;
    }
    
    private async Task FetchBatchAsync()
    {
        Dictionary<TKey, TaskCompletionSource<TResult>> batch;
        
        await _semaphore.WaitAsync();
        try
        {
            batch = new Dictionary<TKey, TaskCompletionSource<TResult>>(_pending);
            _pending.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
        
        try
        {
            var results = await _batchFetcher(batch.Keys);
            foreach (var (key, tcs) in batch)
            {
                if (results.TryGetValue(key, out var result))
                    tcs.SetResult(result);
                else
                    tcs.SetException(new KeyNotFoundException($"Key {key} not found"));
            }
        }
        catch (Exception ex)
        {
            foreach (var tcs in batch.Values)
                tcs.SetException(ex);
        }
    }
}
```

### 5.2 Caching Layer

```csharp
/// <summary>
/// Multi-level cache for ERP data.
/// L1: In-Memory (fast, limited)
/// L2: Redis (distributed, larger)
/// </summary>
public class EnventaCacheLayer
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly EnventaErpActor _actor;
    
    public async Task<Product> GetProductAsync(
        string productId,
        CancellationToken ct)
    {
        var cacheKey = $"product:{productId}";
        
        // L1: Memory cache (< 1ms)
        if (_l1Cache.TryGetValue(cacheKey, out Product cached))
            return cached;
        
        // L2: Redis cache (< 10ms)
        var redisData = await _l2Cache.GetAsync(cacheKey, ct);
        if (redisData != null)
        {
            cached = JsonSerializer.Deserialize<Product>(redisData);
            _l1Cache.Set(cacheKey, cached, TimeSpan.FromMinutes(5));
            return cached;
        }
        
        // L3: ERP (serialized, 50-500ms)
        var product = await _actor.ExecuteAsync(conn =>
        {
            var article = new NVArticleQueryBuilder(conn)
                .ById(productId)
                .Execute()
                .FirstOrDefault();
            return article != null ? MapToProduct(article) : null;
        }, ct);
        
        if (product != null)
        {
            _l1Cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));
            await _l2Cache.SetAsync(
                cacheKey, 
                JsonSerializer.SerializeToUtf8Bytes(product),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) },
                ct);
        }
        
        return product;
    }
}
```

---

## 6. Empfehlungen für B2Connect

### 6.1 Must-Have

1. **Actor Pattern implementieren** für thread-safe ERP-Zugriffe
2. **Per-Tenant Actors** für Isolation
3. **Caching Layer** um ERP-Last zu reduzieren
4. **Batch APIs** um Netzwerk-Roundtrips zu minimieren
5. **Health Monitoring** für Actor-Queues

### 6.2 Should-Have

1. **Request Coalescing** für gleichzeitige Anfragen
2. **Circuit Breaker** für ERP-Ausfälle
3. **Retry Policies** mit exponential Backoff
4. **Metrics Collection** für Performance-Monitoring

### 6.3 Nice-to-Have

1. **Read Replicas** für Read-Heavy Workloads
2. **Event Sourcing** für Audit Trail
3. **CQRS Split** für optimierte Read/Write Paths

---

## 7. Zusammenfassung

| Aspekt | Erkenntnis | Lösung |
|--------|-----------|--------|
| **Threading** | Nicht thread-safe | Actor Pattern |
| **Parallelität** | Nicht möglich | Request Queue |
| **Performance** | Serialisierung = Bottleneck | Caching + Batching |
| **Skalierung** | Vertikal limitiert | Horizontal via Tenants |
| **Transaktionen** | Scope-basiert | FSScope Wrapper |
| **Bulk Ops** | Batch mit Checkpoints | Iterative Batches |

---

**Nächste Schritte:**
1. ADR-023 mit diesen Erkenntnissen aktualisieren
2. Proof-of-Concept für Actor Pattern
3. Performance-Benchmarks definieren

---

**Agents:** @Backend, @Architect | Owner: @Architect
