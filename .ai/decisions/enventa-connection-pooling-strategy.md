# enventa Connection Pooling Strategy

**Created**: 2. Januar 2026  
**Authors**: @Architect, @Enventa  
**Related**: ADR-023, KB-021  
**Status**: Recommendation

---

## Problem Statement

enventa Trade ERP has a **very expensive initialization** (>2 seconds per tenant):
- BusinessUnit must be set during initialization (`FSUtil.SetBusinessUnit()`)
- Re-initializing on every request causes unacceptable latency (>2s per request)
- enventa is multi-tenant via BusinessUnit (similar to B2Connect's tenant isolation)
- Connection state must be maintained for performance

## Architecture Decision

Implement **Connection Pooling with Keep-Alive Strategy** for enventa ERP connections.

### Design Principles

1. **Per-Tenant Connection Pool**
   - One connection per BusinessUnit (tenant)
   - Connection initialized once, reused for all requests from that tenant
   - Idle timeout: 30 minutes (configurable)

2. **Lazy Initialization with Pre-Warming**
   - Create connection on first request (lazy)
   - Pre-warm top N active tenants on startup (eager)
   - Track usage metrics to identify candidates for pre-warming

3. **Keep-Alive & Health Checks**
   - Periodic health check (every 15 minutes)
   - Prevent timeout by pinging before idle timeout expires
   - Graceful reconnection on connection loss

4. **Resource Management**
   - Release stale connections (idle > timeout)
   - Limit total connections (prevent memory exhaustion)
   - Monitor connection lifetime and statistics

## Implementation Strategy

### Phase 1: Connection Pool (Current Sprint)

```csharp
/// <summary>
/// Connection pool for enventa ERP connections.
/// Maintains one connection per BusinessUnit with keep-alive.
/// </summary>
public sealed class EnventaConnectionPool : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, EnventaConnection> _connections;
    private readonly ILogger<EnventaConnectionPool> _logger;
    private readonly Timer _healthCheckTimer;
    private readonly TimeSpan _idleTimeout;
    private readonly int _maxConnections;
    
    public EnventaConnectionPool(
        ILogger<EnventaConnectionPool> logger,
        TimeSpan? idleTimeout = null,
        int maxConnections = 50)
    {
        _connections = new ConcurrentDictionary<string, EnventaConnection>();
        _logger = logger;
        _idleTimeout = idleTimeout ?? TimeSpan.FromMinutes(30);
        _maxConnections = maxConnections;
        
        // Start health check timer (every 15 minutes)
        _healthCheckTimer = new Timer(
            HealthCheckCallback, 
            null, 
            TimeSpan.FromMinutes(15), 
            TimeSpan.FromMinutes(15));
    }
    
    public async Task<EnventaConnection> GetOrCreateAsync(
        string businessUnit, 
        CancellationToken ct = default)
    {
        // Try get existing connection
        if (_connections.TryGetValue(businessUnit, out var connection))
        {
            if (!connection.IsStale(_idleTimeout))
            {
                connection.UpdateLastUsed();
                _logger.LogDebug("Reusing connection for BusinessUnit {BusinessUnit}", businessUnit);
                return connection;
            }
            
            // Stale - dispose and recreate
            _logger.LogInformation("Connection stale for BusinessUnit {BusinessUnit}, recreating", businessUnit);
            await DisposeConnectionAsync(businessUnit);
        }
        
        // Create new connection
        return await CreateConnectionAsync(businessUnit, ct);
    }
    
    private async Task<EnventaConnection> CreateConnectionAsync(
        string businessUnit, 
        CancellationToken ct)
    {
        // Enforce connection limit
        if (_connections.Count >= _maxConnections)
        {
            await EvictLeastRecentlyUsedAsync();
        }
        
        var sw = Stopwatch.StartNew();
        var connection = new EnventaConnection(businessUnit);
        
        try
        {
            await connection.InitializeAsync(ct); // Expensive >2s operation!
            _connections[businessUnit] = connection;
            
            _logger.LogInformation(
                "Created connection for BusinessUnit {BusinessUnit} in {ElapsedMs}ms",
                businessUnit, sw.ElapsedMilliseconds);
            
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to initialize connection for BusinessUnit {BusinessUnit} after {ElapsedMs}ms",
                businessUnit, sw.ElapsedMilliseconds);
            throw;
        }
    }
    
    private async Task EvictLeastRecentlyUsedAsync()
    {
        var lru = _connections
            .OrderBy(kvp => kvp.Value.LastUsed)
            .FirstOrDefault();
        
        if (lru.Value != null)
        {
            _logger.LogInformation(
                "Evicting LRU connection for BusinessUnit {BusinessUnit}",
                lru.Key);
            await DisposeConnectionAsync(lru.Key);
        }
    }
    
    private async void HealthCheckCallback(object? state)
    {
        foreach (var (businessUnit, connection) in _connections)
        {
            try
            {
                // Check if approaching idle timeout
                if (connection.IdleTime > _idleTimeout - TimeSpan.FromMinutes(5))
                {
                    await connection.PingAsync(); // Keep alive
                    _logger.LogDebug("Pinged connection for BusinessUnit {BusinessUnit}", businessUnit);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, 
                    "Health check failed for BusinessUnit {BusinessUnit}, disposing",
                    businessUnit);
                await DisposeConnectionAsync(businessUnit);
            }
        }
    }
    
    private async Task DisposeConnectionAsync(string businessUnit)
    {
        if (_connections.TryRemove(businessUnit, out var connection))
        {
            await connection.DisposeAsync();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        _healthCheckTimer?.Dispose();
        
        foreach (var connection in _connections.Values)
        {
            await connection.DisposeAsync();
        }
        
        _connections.Clear();
    }
}
```

### Phase 2: Pre-Warming (Next Sprint)

```csharp
public class EnventaProviderWarmup : IHostedService
{
    private readonly EnventaConnectionPool _connectionPool;
    private readonly ILogger<EnventaProviderWarmup> _logger;
    
    public async Task StartAsync(CancellationToken ct)
    {
        // Load top active tenants from configuration or analytics
        var activeBusinessUnits = await GetTopActiveBusinessUnitsAsync(ct);
        
        _logger.LogInformation("Pre-warming {Count} enventa connections", activeBusinessUnits.Count);
        
        var tasks = activeBusinessUnits.Select(bu => 
            _connectionPool.GetOrCreateAsync(bu, ct));
        
        await Task.WhenAll(tasks);
        
        _logger.LogInformation("Pre-warming completed");
    }
    
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
```

### Phase 3: Metrics & Monitoring (Future)

```csharp
public class EnventaConnectionMetrics
{
    public int TotalConnections { get; set; }
    public int ActiveConnections { get; set; }
    public int IdleConnections { get; set; }
    public TimeSpan AverageInitTime { get; set; }
    public TimeSpan AverageConnectionLifetime { get; set; }
    public int TotalInitializations { get; set; }
    public int FailedInitializations { get; set; }
    public Dictionary<string, ConnectionStats> PerTenantStats { get; set; }
}
```

## Configuration

```json
{
  "EnventaConnectionPool": {
    "IdleTimeout": "00:30:00",
    "HealthCheckInterval": "00:15:00",
    "MaxConnections": 50,
    "PreWarmingEnabled": true,
    "PreWarmTopN": 20,
    "EnableMetrics": true
  }
}
```

## Impact Analysis

### Performance Improvement
- **Before**: >2s latency on every request (new connection per request)
- **After**: <50ms latency for cached connections (connection reused)
- **Improvement**: ~40x faster for repeat requests to same tenant

### Resource Usage
- **Memory**: ~10-20 MB per connection (50 connections = ~0.5-1 GB)
- **CPU**: Minimal (only during init and health checks)
- **I/O**: Reduced database connections (one per tenant vs. per request)

### Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| Memory exhaustion | High | Connection limit + LRU eviction |
| Stale connections | Medium | Health checks + idle timeout |
| Init failures | Medium | Retry with exponential backoff |
| Connection leaks | Low | Proper disposal + monitoring |

## Testing Strategy

1. **Unit Tests**
   - Connection pool creation/disposal
   - LRU eviction logic
   - Health check behavior

2. **Integration Tests**
   - Actual enventa initialization
   - Connection reuse verification
   - Timeout and reconnection scenarios

3. **Load Tests**
   - 100 concurrent tenants
   - Connection pool saturation
   - Memory usage under load

4. **Chaos Tests**
   - enventa connection failures
   - Network interruptions
   - Database restarts

## Rollout Plan

### Week 1 (Current)
- [x] Document strategy (this document)
- [ ] Implement `EnventaConnectionPool`
- [ ] Implement `EnventaConnection` with lifecycle management
- [ ] Unit tests for connection pool

### Week 2
- [ ] Integration tests with actual enventa
- [ ] Wire up to `ProviderManager`
- [ ] Health check implementation
- [ ] Monitoring/logging

### Week 3
- [ ] Pre-warming implementation
- [ ] Load testing
- [ ] Performance benchmarks
- [ ] Documentation updates

### Week 4
- [ ] Deploy to staging
- [ ] Monitor metrics
- [ ] Production deployment
- [ ] Post-deployment validation

## Success Metrics

- **Initialization latency**: <2.5s (95th percentile)
- **Cached request latency**: <50ms (95th percentile)
- **Connection reuse rate**: >80% of requests
- **Failed init rate**: <1% of connection attempts
- **Memory usage**: <2 GB for 100 active tenants

---

**Next Steps:**
1. @Enventa: Implement `EnventaConnectionPool` class
2. @Backend: Wire up connection pool in `ProviderManager`
3. @DevOps: Configure monitoring for connection metrics
4. @QA: Create integration test suite for connection lifecycle

**Approval Required**: @Architect, @TechLead  
**Implementation Owner**: @Enventa, @Backend
