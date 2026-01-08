---
docid: KB-115
title: Enventa Trade Erp
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# enventa Trade ERP - Integration Guide

**DocID**: `KB-021`  
**Owner**: @Enventa  
**Last Updated**: 2. Januar 2026  
**Status**: ✅ Active

---

## Overview

enventa Trade ERP ist ein etabliertes ERP-System für Großhandel und B2B-Vertrieb, entwickelt auf .NET Framework 4.8 mit proprietärem ORM.

## Technical Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET Framework 4.8 (Windows-only) |
| ORM | Proprietary FrameworkSystems ORM |
| Database | SQL Server / Oracle |
| API | Custom assemblies (not REST/gRPC native) |
| Threading | **Single-threaded only** (not thread-safe) |
| Multi-Tenancy | **BusinessUnit** (set during initialization) |
| Initialization | **>2 seconds** (expensive - requires connection pooling!) |

## Key Components

### Core Assemblies

```csharp
// Product Management
IcECArticle - Article/Product CRUD operations
NVArticleQueryBuilder - Query construction for products

// Customer Management
IcECCustomer - Customer/CRM data access
IcECContact - Contact information management

// Order Processing
IcECOrder - Order creation and management
IcECOrderPosition - Order line items

// Framework Utilities
FSUtil - Login, Session, Transaction scopes
```

### Proprietary ORM Patterns

```csharp
// Query Pattern with Yield
public IEnumerable<Product> QueryProducts(ProductFilter filter)
{
    var query = new NVArticleQueryBuilder()
        .ByCategory(filter.CategoryId)
        .WithPricing(filter.IncludePrices)
        .WithStock(filter.IncludeStock);
    
    // Yield to avoid loading all records into memory
    foreach (var article in query.Execute())
    {
        yield return MapToProduct(article);
    }
}

// Batch Processing with Transaction Scope
public void BulkUpdate(IEnumerable<Product> products)
{
    foreach (var batch in products.Chunk(1000))
    {
        using var scope = FSUtil.CreateScope();
        foreach (var product in batch)
        {
            _articleService.Update(MapToArticle(product));
        }
        scope.Commit(); // Commit per batch
    }
}

// Login Management
FSUtil.Login(connectionString);
try
{
    // Perform operations
}
finally
{
    FSUtil.Logout();
}
```

## Critical Constraints

### 🚨 Single-Threading Requirement

**enventa assemblies are NOT thread-safe!** All operations must be serialized.

### 🚨 Large Catalog Challenge

**Real-World Data Volumes**:
- **1.5 million articles** (some customers)
- **30 million attributes/Merkmale**
- **Images, references, cross-references**

**Implication**: Real-time queries impossible for full catalog operations.

**Solution** (eGate implementation pattern):
- **Tier 1: Synchronized Master Data** (eventual consistency)
  - Articles, attributes, images, references → Local database
  - Background sync jobs (hourly/nightly, incremental)
  - Acceptable time delay (minutes to hours)
- **Tier 2: Live Customer-Specific Queries** (strong consistency)
  - Kundenindividuelle Preise (customer prices)
  - Bestände (stock levels)
  - Vorgangsdaten (order status, order history)
  - Always query ERP directly via connection pool

```csharp
// ❌ WRONG - Querying 1.5M articles in real-time
var articles = await _erpProvider.GetAllArticlesAsync(); // NEVER DO THIS!

// ✅ CORRECT - Hybrid approach (eGate pattern)
// Step 1: Get master data from local DB (synced, fast)
var article = await _dbContext.Articles
    .Include(a => a.Attributes)
    .Include(a => a.Images)
    .FirstOrDefaultAsync(a => a.ArticleNo == articleNo);

// Step 2: Get customer-specific data from ERP (live, current)
var customerData = await _erpProvider.GetCustomerDataAsync(new
{
    ArticleNo = articleNo,
    CustomerId = customerId,
    BusinessUnit = tenantContext.BusinessUnit
});

// Step 3: Combine results
return new ArticleViewModel
{
    // From local DB (synced)
    ArticleNo = article.ArticleNo,
    Description = article.Description,
    Attributes = article.Attributes,
    Images = article.Images,
    
    // From ERP (live)
    Price = customerData.Price,
    Stock = customerData.Stock,
    Discount = customerData.Discount
};
```

### 🚨 Single-Threading Requirement (Technical)

```csharp
// ❌ WRONG - Concurrent access causes data corruption
await Task.WhenAll(
    GetProductAsync(id1),
    GetProductAsync(id2)
);

// ✅ CORRECT - Use Actor pattern for serialization
await _erpActor.ExecuteAsync(async () => {
    var product1 = await GetProductAsync(id1);
    var product2 = await GetProductAsync(id2);
    return (product1, product2);
});
```

### Login/Session Management & BusinessUnit

**Critical Performance Constraint:**
- **Initialization time**: >2 seconds per tenant (very expensive!)
- **BusinessUnit**: Tenant is selected during initialization via `FSUtil.SetBusinessUnit()`
- **Connection pooling**: MUST keep connections alive to avoid repeated >2s init
- **Idle timeout**: 30 minutes default (configurable)
- **One session per tenant**: Login requires exclusive lock (SemaphoreSlim)
- **Logout**: Must be called in finally block or on disposal

**Connection Lifecycle Strategy:**
```csharp
// ❌ WRONG - Re-initializing on every request (>2s each!)
foreach (var request in requests)
{
    FSUtil.Login(connectionString);
    FSUtil.SetBusinessUnit(tenantId);
    ProcessRequest(request);
    FSUtil.Logout();
}

// ✅ CORRECT - Keep connection alive, reuse across requests
public class EnventaConnectionPool
{
    private readonly ConcurrentDictionary<string, EnventaConnection> _connections;
    private readonly TimeSpan _idleTimeout = TimeSpan.FromMinutes(30);
    
    public async Task<EnventaConnection> GetOrCreateAsync(string businessUnit)
    {
        if (_connections.TryGetValue(businessUnit, out var conn))
        {
            if (!conn.IsStale(_idleTimeout))
            {
                conn.UpdateLastUsed();
                return conn; // Reuse existing connection
            }
            
            // Stale connection - dispose and recreate
            await DisposeConnectionAsync(businessUnit);
        }
        
        // Create new connection (expensive >2s operation!)
        var newConn = new EnventaConnection(businessUnit);
        await newConn.InitializeAsync(); // >2s!
        _connections[businessUnit] = newConn;
        return newConn;
    }
}
```

### Memory Constraints

- **Don't load all records**: Use yield-based iteration
- **Batch size**: 1000 records per transaction
- **Connection pooling**: 1 connection per tenant (single-threaded)

## B2X Integration Architecture

### Communication Strategy

```
┌──────────────┐       gRPC        ┌──────────────────────┐
│  B2X   │◄──────────────────►│  enventa Provider    │
│  (.NET 10)   │    Streaming       │  (.NET Fwk 4.8)      │
└──────────────┘                    └──────────────────────┘
       │                                      │
       │ ProviderManager                      │
       ├─ EnventaPimProvider                  ▼
       ├─ EnventaCrmProvider        ┌──────────────────────┐
       └─ EnventaErpProvider        │  enventa Assemblies  │
              │                     │  (Single-threaded)   │
              ▼                     └──────────────────────┘
       ┌──────────────┐
       │  ErpActor    │ ← Serializes all operations
       │  (per-tenant)│
       └──────────────┘
```

### gRPC Service Definitions

**Location**: `backend/Domain/ERP/src/Protos/erp_services.proto`

```protobuf
service PimService {
  // Single operations
  rpc GetProduct(GetProductRequest) returns (ProductResponse);
  rpc GetProducts(GetProductsRequest) returns (ProductsResponse);
  
  // Streaming (.NET Framework 4.8 compatible)
  rpc StreamProducts(StreamProductsRequest) returns (stream ProductResponse);
  rpc StreamProductChanges(StreamChangesRequest) returns (stream ProductChangeEvent);
  
  // Category & Pricing
  rpc GetCategories(GetCategoriesRequest) returns (CategoriesResponse);
  rpc GetProductPricing(GetPricingRequest) returns (PricingResponse);
  rpc GetBulkPricing(GetBulkPricingRequest) returns (BulkPricingResponse);
}

service CrmService {
  rpc GetCustomer(GetCustomerRequest) returns (CustomerResponse);
  rpc SearchCustomers(SearchCustomersRequest) returns (CustomersResponse);
  rpc StreamCustomers(StreamCustomersRequest) returns (stream CustomerResponse);
  rpc GetCustomerPricing(GetCustomerPricingRequest) returns (CustomerPricingResponse);
}

service ErpService {
  rpc CreateOrder(CreateOrderRequest) returns (OrderResponse);
  rpc GetOrders(GetOrdersRequest) returns (OrdersResponse);
  rpc GetDocuments(GetDocumentsRequest) returns (DocumentsResponse);
}
```

### Actor Pattern Implementation

See: `backend/Domain/ERP/src/Infrastructure/Actor/ErpActor.cs`

**Production Implementation** using Channel-based message queue:

```csharp
/// <summary>
/// Thread-safe Actor for serialized ERP access.
/// Ensures single-threaded execution of all ERP operations for a specific tenant.
/// Critical for enventa Trade ERP which is NOT thread-safe.
/// </summary>
/// <remarks>
/// This Actor pattern implementation uses a Channel{T} as a message queue.
/// A single background worker processes operations sequentially, ensuring
/// that the underlying ERP connection is never accessed concurrently.
/// </remarks>
public sealed class ErpActor : IAsyncDisposable
{
    private readonly Channel<IErpOperation> _operationQueue;
    private readonly Task _workerTask;
    private readonly CancellationTokenSource _shutdownCts;
    private readonly TenantContext _tenant;
    
    public ErpActor(TenantContext tenant, ILogger<ErpActor> logger, int queueCapacity = 1000)
    {
        _tenant = tenant;
        _operationQueue = Channel.CreateBounded<IErpOperation>(new BoundedChannelOptions(queueCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
        
        _shutdownCts = new CancellationTokenSource();
        _workerTask = Task.Run(() => ProcessOperationsAsync(_shutdownCts.Token));
    }
    
    public async Task<TResult> EnqueueAsync<TResult>(ErpOperation<TResult> operation)
    {
        await _operationQueue.Writer.WriteAsync(operation);
        return await operation.ResultSource.Task;
    }
    
    private async Task ProcessOperationsAsync(CancellationToken ct)
    {
        await foreach (var operation in _operationQueue.Reader.ReadAllAsync(ct))
        {
            // Execute sequentially - this is the critical single-threaded constraint
            await operation.ExecuteAsync(ct);
        }
    }
}
```

**Key Features:**
- **Channel-based queue**: Bounded channel with configurable capacity (default 1000)
- **Single background worker**: Processes all operations sequentially
- **Per-tenant Actor**: Each tenant gets its own Actor instance
- **Metrics**: Tracks queued, processed, and failed operations
- **Graceful shutdown**: Completes pending operations before disposal

### Resilience Pipeline Implementation

See: `backend/Domain/ERP/src/Infrastructure/Resilience/ErpResiliencePipeline.cs`

**Polly-based resilience patterns** for production ERP reliability:

```csharp
/// <summary>
/// Resilience pipeline for ERP operations using Polly.
/// Implements Circuit Breaker, Retry, and Timeout policies.
/// </summary>
public sealed class ErpResiliencePipeline
{
    private readonly ResiliencePipeline _pipeline;
    
    public ErpResiliencePipeline(IOptions<ErpResilienceOptions> options)
    {
        _pipeline = new ResiliencePipelineBuilder()
            .AddTimeout(options.Value.Timeout)
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.Value.MaxRetries,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = options.Value.FailureRatio, // 0.5 = 50%
                MinimumThroughput = options.Value.MinimumThroughput, // 10
                BreakDuration = options.Value.BreakDuration, // 1 minute
                ShouldHandle = static args => args.Outcome.Exception is ErpException
            })
            .Build();
    }
    
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        var result = await _pipeline.ExecuteAsync(
            static async (state, ct) => await state.operation(ct),
            (operation, cancellationToken),
            cancellationToken);
            
        return result;
    }
}
```

**Configuration Options:**
```csharp
public class ErpResilienceOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
    public double FailureRatio { get; set; } = 0.5; // 50%
    public int MinimumThroughput { get; set; } = 10;
    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromMinutes(1);
}
```

**Key Features:**
- **Circuit Breaker**: Opens after 50% failure ratio, breaks for 1 minute
- **Retry**: Up to 3 attempts with exponential backoff + jitter
- **Timeout**: 30-second timeout per operation
- **Exception Handling**: Only handles `ErpException` types
- **Metrics Integration**: Tracks circuit state and failure rates

### Transaction Scope Implementation

See: `backend/Domain/ERP/src/Infrastructure/Transactions/IErpTransactionScope.cs`

**Transaction scope abstraction** for enventa FSUtil compatibility:

```csharp
/// <summary>
/// Abstraction for enventa transaction scopes.
/// Compatible with FSUtil.CreateScope() pattern.
/// </summary>
public interface IErpTransactionScope : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Wraps ERP operations in transaction scope.
/// Ensures atomicity for multi-step operations.
/// </summary>
public sealed class TransactionalErpOperation : IErpOperation
{
    private readonly IErpTransactionScope _scope;
    private readonly Func<Task> _operation;
    
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _operation();
            await _scope.CommitAsync(cancellationToken);
        }
        catch
        {
            await _scope.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
```

**Usage Pattern:**
```csharp
// ✅ Transactional batch update
await _erpActor.ExecuteAsync(async () =>
{
    using var scope = _transactionScopeFactory.CreateScope();
    
    foreach (var batch in products.Chunk(1000))
    {
        foreach (var product in batch)
        {
            await _erpProvider.UpdateProductAsync(product);
        }
    }
    
    await scope.CommitAsync();
});
```

**Key Features:**
- **FSUtil Compatibility**: Adapts enventa's `FSUtil.CreateScope()` pattern
- **Async Support**: Full async/await support with cancellation
- **Exception Handling**: Automatic rollback on exceptions
- **Batch Processing**: Optimized for bulk operations
- **Resource Management**: Proper disposal and cleanup

## Data Model Mappings

| enventa Model | B2X Model | Notes |
|---------------|-----------------|-------|
| `IcECArticle` | `PimProduct` | Product master data |
| `NVKunde` | `CrmCustomer` | Customer information |
| `NVBestellung` | `ErpOrder` | Order with line items |
| `NVPreis` | `PriceInfo` | Pricing, discounts |
| `NVLager` | `StockInfo` | Inventory levels |

## Performance Best Practices

### Connection Pooling & Warm-up

```csharp
// ✅ Pre-warm connections for active tenants on startup
public async Task WarmupAsync(IEnumerable<string> activeBusinessUnits)
{
    var tasks = activeBusinessUnits.Select(async bu =>
    {
        var sw = Stopwatch.StartNew();
        var conn = await _connectionPool.GetOrCreateAsync(bu);
        _logger.LogInformation(
            "Warmed up connection for BusinessUnit {BusinessUnit} in {ElapsedMs}ms",
            bu, sw.ElapsedMilliseconds);
    });
    
    await Task.WhenAll(tasks);
}

// ✅ Health check maintains connection state
public async Task MaintainConnectionsAsync()
{
    foreach (var (businessUnit, connection) in _activeConnections)
    {
        if (connection.IsStale(TimeSpan.FromMinutes(25)))
        {
            await connection.PingAsync(); // Keep alive before timeout
        }
    }
}
```

**Recommendations:**
- **Pre-warm top 20 active tenants** on application startup
- **Idle timeout**: 30 minutes (balance memory vs. re-init cost)
- **Health check interval**: Every 15 minutes for active connections
- **Graceful degradation**: If init fails, retry with exponential backoff

## Performance Best Practices

### Connection Pooling & Warm-up

```csharp
// ✅ Pre-warm connections for active tenants on startup
public async Task WarmupAsync(IEnumerable<string> activeBusinessUnits)
{
    var tasks = activeBusinessUnits.Select(async bu =>
    {
        var sw = Stopwatch.StartNew();
        var conn = await _connectionPool.GetOrCreateAsync(bu);
        _logger.LogInformation(
            "Warmed up connection for BusinessUnit {BusinessUnit} in {ElapsedMs}ms",
            bu, sw.ElapsedMilliseconds);
    });
    
    await Task.WhenAll(tasks);
}

// ✅ Health check maintains connection state
public async Task MaintainConnectionsAsync()
{
    foreach (var (businessUnit, connection) in _activeConnections)
    {
        if (connection.IsStale(TimeSpan.FromMinutes(25)))
        {
            await connection.PingAsync(); // Keep alive before timeout
        }
    }
}
```

**Recommendations:**
- **Pre-warm top 20 active tenants** on application startup
- **Idle timeout**: 30 minutes (balance memory vs. re-init cost)
- **Health check interval**: Every 15 minutes for active connections
- **Graceful degradation**: If init fails, retry with exponential backoff

### Bulk Operations

```csharp
// ✅ Efficient - Batched with transaction scope
public async Task SyncProducts(IEnumerable<Product> products)
{
    foreach (var batch in products.Chunk(1000))
    {
        await _erpActor.ExecuteAsync(() => {
            using var scope = FSUtil.CreateScope();
            foreach (var product in batch)
                _articleService.Update(MapToArticle(product));
            scope.Commit();
            return Task.CompletedTask;
        });
    }
}

// ❌ Inefficient - Too many DB roundtrips
foreach (var product in products)
{
    await UpdateProduct(product); // Separate transaction each!
}
```

### Query Optimization

```csharp
// ✅ Efficient - Single query with filtering
var products = _queryBuilder
    .ByCategory(categoryId)
    .WithPricing(true)
    .WithStock(true)
    .Execute();

// ❌ Inefficient - Multiple queries
var products = GetAllProducts();
var filtered = products.Where(p => p.CategoryId == categoryId);
foreach (var product in filtered)
{
    product.Price = GetPrice(product.Id);  // N+1 problem!
    product.Stock = GetStock(product.Id);
}
```

### Provider Factory & Lifecycle

**EnventaProviderFactory** creates provider instances per tenant:

```csharp
public sealed class EnventaProviderFactory : IProviderFactory
{
    public async Task<ProviderInstance> CreateProviderAsync(
        TenantContext tenant,
        ProviderConfiguration configuration,
        ErpActor actor,
        CancellationToken cancellationToken = default)
    {
        // Create gRPC channel to enventa adapter service
        var channel = GrpcChannel.ForAddress(configuration.EndpointUrl);
        
        var pimProvider = new EnventaPimProvider(channel, actor, logger);
        var crmProvider = new EnventaCrmProvider(channel, actor, logger);
        var erpProvider = new EnventaErpProvider(channel, actor, logger);
        
        return new ProviderInstance
        {
            PimProvider = pimProvider,
            CrmProvider = crmProvider,
            ErpProvider = erpProvider,
            ConnectionState = ProviderConnectionState.Connected
        };
    }
    
    public bool SupportsCapability(ProviderCapability capability)
    {
        // enventa supports: Products, Customers, Orders, Pricing,
        // Stock, Categories, Contacts, Activities, Streaming, BatchOps
        return capability switch
        {
            ProviderCapability.Products => true,
            ProviderCapability.Customers => true,
            ProviderCapability.Orders => true,
            ProviderCapability.Streaming => true,
            ProviderCapability.BatchOperations => true,
            _ => false
        };
    }
}
```

**ProviderManager** orchestrates provider lifecycle:
- One provider instance per tenant
- Actor pool manages all tenant actors
- Health checks validate provider connectivity
- Automatic reconnection on failures

### TenantContext Structure

All ERP operations require `TenantContext`:

```csharp
public sealed record TenantContext
{
    public required string TenantId { get; init; }
    public required string TenantName { get; init; }
    public string? CorrelationId { get; init; }  // Request tracing
    public string? UserId { get; init; }          // Audit logging
    public DateTimeOffset Timestamp { get; init; }
    public IReadOnlyDictionary<string, string> Metadata { get; init; }
}
```

**Usage in all provider calls:**
```csharp
var tenant = new TenantContext
{
    TenantId = "tenant-123",
    TenantName = "ACME Corp",
    CorrelationId = Guid.NewGuid().ToString(),
    UserId = "user@example.com"
};

var product = await pimProvider.GetProductDetailsAsync("SKU-001", tenant, ct);
```

## Deployment

### Windows Container Requirements

```dockerfile
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-ltsc2022

# Install enventa assemblies
COPY enventa/ C:/Program Files/enventa/

# Copy provider implementation
COPY publish/ /app/

WORKDIR /app
ENTRYPOINT ["ErpProviderService.exe"]
```

### Configuration

```json
{
  "EnventaErp": {
    "ConnectionString": "Server=sql.example.com;Database=Enventa;...",
    "Provider": "SqlServer",
    "MaxBatchSize": 1000,
    "QueryTimeout": 30,
    "SessionTimeout": 1800
  }
}
```

## Troubleshooting

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| Data corruption | Concurrent access | Use ErpActor.EnqueueAsync() |
| Memory overflow | Loading all records | Use gRPC streaming |
| Queue full | Too many concurrent requests | Increase Actor queue capacity |
| Operation timeout | Default 30s exceeded | Adjust ErpOperation timeout |
| Login timeout | Session expired | Check provider health endpoint |
| Slow bulk sync | Too small batches | Increase to 1000 records/batch |
| gRPC channel error | Wrong endpoint URL | Verify ProviderConfiguration.EndpointUrl |

### Debug Logging

```csharp
// Enable enventa internal logging
FSUtil.EnableLogging("C:/logs/enventa.log");

// Log operation timing
_logger.LogInformation("ERP operation: {Operation} took {Duration}ms", 
    operation, duration);
```

## Implementation Status

**Current State** (as of 2. Januar 2026):

✅ **Completed:**
- Actor pattern infrastructure (`ErpActor`, `ErpOperation`)
- Resilience pipeline with Polly (Circuit Breaker, Retry, Timeout)
- Transaction scope abstraction (`IErpTransactionScope`, `TransactionalErpOperation`)
- Provider factory and lifecycle management
- gRPC proto definitions for all services
- Provider interfaces (`IErpProvider`, `ICrmProvider`, `IPimProvider`)
- Core data models (`TenantContext`, `ProviderResult`, `PagedResult`)
- Provider manager with health checks
- Service collection extensions for DI
- Status-based error tracking in operations
- Reflection elimination in ErpActor (production-ready)

🚧 **In Progress:**
- Actual gRPC client implementations (currently throw `NotImplementedException`)
- enventa adapter service (.NET Framework 4.8 container)
- Concrete data model mappings (enventa → B2X)

📋 **Planned:**
- Windows container deployment for .NET Framework 4.8 adapter
- Complete enventa ORM integration
- End-to-end integration tests
- Performance benchmarks

## eGate Reference Implementation

**Source**: [NissenVelten/eGate](https://github.com/NissenVelten/eGate) - Production enventa integration

### Architecture Overview

eGate is a **web shop application** that integrates with enventa Trade ERP. The **Anti-Corruption Layer** is implemented in the `Broker/` projects.

**enventa Development**:
- Built with **FrameworkStudio** (defines software architecture)
- FrameworkStudio dictates: `FrameworkSystems.*` / `FS.*` namespaces
- ERP interface: `NV.ERP.MM.ECommerce.ECComponents`
- Assemblies loaded from enventa installation directory (runtime dependency)

**eGate Broker Strategy** (Runtime Version Selection):
- Configuration at startup: [AppActivator.Broker.cs](https://github.com/NissenVelten/eGate/blob/master/NVShop.Web/App_Start/AppActivator.Broker.cs)
- Decides which enventa version to load (FS_45 or FS_47)
- Loads corresponding Anti-Corruption Layer implementation

**Three Anti-Corruption Layer implementations** in `Broker/`:

1. **Direct FS API** (`Broker/FS_45/`, `Broker/FS_47/`) — Shared Projects
   - Direct access to enventa assemblies
   - FS_45: Uses `FrameworkSystems.*` namespace (older FrameworkStudio)
   - FS_47: Uses `FS.*` namespace (newer FrameworkStudio)
   - Same API surface, only namespace changes

2. **OData Broker** (`Broker/eNVentaClient/`)
   - Remote access via OData service
   - No direct enventa assembly dependency
   - Platform-independent

3. **Web Services** (`Broker/FS_45/WS/`, `Broker/FS_47/WS/`)
   - Domain-specific services (Steel, Datanorm)
   - Extensions to Direct FS API

### enventa Type System & Anti-Corruption Layer

**enventa Components** (from enventa assemblies):

1. **enventa Native Entities** (`NV.ERP.MM.ECommerce.ECComponents`)
   - Example: `IcECArticle`, `IcECOrder`, `IcECCustomer`
   - The actual enventa ERP entities
   - Loaded from enventa installation directory

2. **FrameworkStudio Architecture** (`FrameworkSystems.*` or `FS.*`)
   - Example: `IDevFrameworkDataObject`, `IFSRepository<T>`, `FSUtil`
   - enventa's framework/ORM provided by FrameworkStudio
   - Namespace changed: `FrameworkSystems.*` (FS_45) → `FS.*` (FS_47)

**eGate Anti-Corruption Layer** (`Broker/` projects):

3. **ACL DTOs** (`NV...` types in `Broker/` projects)
   - Example: `NVArticle`, `NVOrder`, `NVCustomer`
   - Shield application from enventa's object model
   - Defined in Anti-Corruption Layer (not in enventa)

4. **ACL Repositories** (`Broker/` projects)
   - Example: `NVArticleRepository`, `NVOrderRepository`, `NVContext`
   - Wrap enventa's `IFSRepository<T>` with clean API
   - Part of Anti-Corruption Layer

5. **ACL Services** (`Broker/` projects)
   - Example: `ECPriceService`, `ECStockService`
   - Business logic orchestration
   - Part of Anti-Corruption Layer

**Integration Flow**:
```
eGate Web Application
    ↓ (Startup Configuration)
AppActivator.Broker.cs (decides FS_45 vs FS_47)
    ↓ (loads)
Anti-Corruption Layer (Broker/FS_45/ or Broker/FS_47/)
    ↓ (uses)
enventa Assemblies (from enventa installation directory)
    ↓ (FrameworkStudio Architecture: IFSRepository, FSUtil)
enventa Native Entities (NV.ERP.MM.ECommerce.ECComponents.IcEC*)
    ↓
enventa Trade ERP
```

### 1. Direct FS API Integration (Broker/FS_45, Broker/FS_47)

**Location**: `Broker/FS_45/`, `Broker/FS_47/` (Shared Projects)

**Purpose**: Anti-Corruption Layer with direct access to enventa assemblies

**Strategy**: Hybrid data approach for large catalogs
- **Synchronized Master Data** → Local database (eGate uses SQL Server)
  - Articles, attributes, images (1.5M+ articles, 30M+ attributes)
  - Background sync jobs (incremental)
  - Eventual consistency (minutes to hours delay acceptable)
- **Live Customer Queries** → enventa ERP (direct via FSUtil)
  - Customer prices (kundenindividuelle Preise)
  - Stock levels (Bestände)
  - Order status (Vorgangsdaten)
  - Strong consistency (real-time required)

**Core Components** (from Anti-Corruption Layer):
- `NVContext` — Repository aggregator (like DbContext)
- `NVArticleRepository`, `NVOrderRepository`, etc. — 60+ repositories
- `ECPriceService`, `ECStockService`, etc. — Business logic services
- `NVArticle`, `NVOrder`, etc. — DTOs for application layer

**Uses enventa Framework** (from enventa assemblies):
- `FSUtil` — FrameworkStudio utility (authentication, scoping, BusinessUnit)
- `IFSRepository<T>` — FrameworkStudio repository for native entities
- `IcECArticle`, `IcECOrder`, etc. — enventa native entities (`NV.ERP.MM.ECommerce.ECComponents`)

**FS_45 vs. FS_47** (Shared Projects):
- `FS_45` uses `FrameworkSystems.*` namespace (older FrameworkStudio version)
- `FS_47` uses `FS.*` namespace (newer FrameworkStudio version)
- Same functionality, only namespace change from FrameworkStudio update
- Loaded via [AppActivator.Broker.cs](https://github.com/NissenVelten/eGate/blob/master/NVShop.Web/App_Start/AppActivator.Broker.cs) at runtime

**Hybrid Data Flow**:

```csharp
// eGate Pattern: Combine synced master data + live customer data
public async Task<ArticleViewModel> GetArticleAsync(string articleNo, string customerId)
{
    // Step 1: Get master data from local SQL Server (synced, fast)
    var article = await _localDb.Articles
        .Include(a => a.Attributes)
        .Include(a => a.Images)
        .FirstOrDefaultAsync(a => a.ArticleNo == articleNo);
    
    if (article == null)
        throw new NotFoundException($"Article {articleNo} not found");
    
    // Step 2: Get customer-specific data from enventa (live via FSUtil)
    using (var scope = _util.Scope()) // ✅ ACL: NVContext wrapper
    {
        var priceService = new ECPriceService(_util); // ✅ ACL: Business logic
        var stockService = new ECStockService(_util); // ✅ ACL: Business logic
        
        var price = await priceService.GetCustomerPriceAsync(articleNo, customerId);
        var stock = await stockService.GetStockLevelAsync(articleNo);
        
        // Step 3: Combine results
        return new ArticleViewModel
        {
            // From local DB (synced, eventual consistency)
            ArticleNo = article.ArticleNo,
            Description = article.Description,
            LongDescription = article.LongDescription,
            Attributes = article.Attributes,
            Images = article.Images,
            CategoryPath = article.CategoryPath,
            
            // From ERP (live, strong consistency)
            Price = price.NetPrice,
            Discount = price.Discount,
            Stock = stock.Available,
            DeliveryTime = stock.EstimatedDelivery,
            
            // Metadata
            LastSynced = article.LastSyncDate,
            PriceAsOf = DateTime.UtcNow
        };
    }
}

// Background Sync Job (eGate pattern)
public class ArticleSyncJob
{
    public async Task SyncArticlesAsync()
    {
        var lastSync = await _watermark.GetLastSyncAsync("Articles");
        
        using (var scope = _util.Scope()) // ✅ enventa: FSUtil.Scope()
        {
            // Query only changed articles (incremental sync)
            var repo = _util.GetRepository<IcECArticle>(); // ✅ enventa: IFSRepository
            var changed = repo.GetAll()
                .Where(a => a.ModifiedDate > lastSync)
                .OrderBy(a => a.ModifiedDate);
            
            foreach (var batch in changed.Chunk(1000))
            {
                // Map enventa entities → ACL DTOs → Local DB
                var dtos = batch.Select(a => new NVArticle // ✅ ACL: DTO
                {
                    ArticleNo = a.ArticleNo,
                    Description = a.Description,
                    // ... map all fields
                });
                
                await _localDb.BulkInsertOrUpdateAsync(dtos);
                
                // Update watermark after each batch
                await _watermark.SetLastSyncAsync(
                    "Articles",
                    batch.Max(a => a.ModifiedDate)
                );
            }
        }
    }
}
```

**Key Implementation Patterns**:

```csharp
// FSUtil - FrameworkStudio utility (from enventa assemblies)
public partial class FSUtil : IDisposable
{
    private readonly INVIdentityProvider _provider;
    
    public FSUtil(INVIdentityProvider provider) { ... }
    
    // Authentication with BusinessUnit
    public static bool Authenticate(NVIdentity identity) { ... }
    
    // Scope pattern for enventa operations
    public FSScope Scope() { ... }
}

// Anti-Corruption Layer Repository (Broker/FS_47/)
public class NVArticleRepository : 
    NVReadRepository<NVArticle, IcECArticle>, INVArticleRepository
{
    // NVArticle = ACL DTO (defined in Broker/)
    // IcECArticle = enventa native entity (NV.ERP.MM.ECommerce.ECComponents)
    
    public NVArticleRepository(
        IFSRepository<IcECArticle> rep,  // FrameworkStudio repository
        FSUtil util                      // FrameworkStudio utility
    ) : base(rep, util) { }
}

// Anti-Corruption Layer Service (Broker/FS_47/)
public class ECPriceService : IECPriceService
{
    private readonly FSUtil _util;
    
    public ECPriceService(FSUtil util) { _util = util; }
    
    public NVPriceCondition GetPriceCondition(NVPriceConditionRequest request)
    {
        using (var scope = _util.Scope())  // FrameworkStudio scope
        {
            // Create enventa native service (NV.ERP.MM.ECommerce.ECComponents)
            var priceService = scope.Create<IcECPriceService>();
            
            // Work with enventa native entities
            var result = priceService.GetPriceCondition(...);
            
            // Map to ACL DTO before returning
            return result.MapTo<NVPriceCondition>();
        }
    }
}
```

**NVContext Architecture** (Anti-Corruption Layer):
- 60+ lazy-loaded repositories wrapping enventa native entities
- Articles, Catalog, Orders, Customers, Stock, Returns, etc.
- Each repository maps enventa entities → ACL DTOs
- Defined in `Broker/FS_45/`, `Broker/FS_47/` (Shared Projects)
- Dependency Injection with Unity container

**Runtime Loading** (eGate startup):
```csharp
// AppActivator.Broker.cs decides which version to load
public static class BrokerActivator
{
    public static void Start()
    {
        // Decide based on configuration
        if (UseFS47)
            LoadFS47Broker();  // Uses FS.* namespaces
        else
            LoadFS45Broker();  // Uses FrameworkSystems.* namespaces
            
        // Assemblies loaded from enventa installation directory
    }
}
```

### 2. OData Broker Integration (Broker/eNVentaClient)

**Location**: `Broker/eNVentaClient/`

**Purpose**: Anti-Corruption Layer via OData (no direct enventa assembly dependency)

**Core Components** (from Anti-Corruption Layer):
- `NVODataReadRepository<T>` — OData-based repositories
- `NVUtilProxy` — HTTP client for remote operations
- `Container` — OData service container (auto-generated from $metadata)
- Same `NV*` DTOs as Direct FS API

**Advantages**:
- No direct DLL dependency on enventa assemblies
- Can run on non-Windows platforms (if OData service is Windows-hosted)
- Scales horizontally (stateless HTTP calls)

**Disadvantages**:
- Requires separate OData service deployment
- Network overhead for each call
- Limited to OData query capabilities

**Authentication**:
```csharp
// OData authentication with BusinessUnit in username
protected Container Container
{
    get
    {
        var container = new Container(new Uri(BrokerSettings.Default.ServiceUrl))
        {
            Credentials = new NetworkCredential(
                $"{_provider.Get.Name}@{_provider.Get.BusinessUnit}", 
                _provider.Get.Password
            )
        };
        
        container.BuildingRequest += (sender, args) => {
            args.Headers.Add("Accept-Encoding", "gzip");
        };
        
        return container;
    }
}
```

**Data Access Pattern**:
```csharp
public class NVArticleRepository : 
    NVODataQueryReadRepository<Model.NVArticle, NVArticle, INVArticleQueryBuilder>
{
    public NVArticleRepository(INVIdentityProvider provider) : base(provider) { }
    
    // OData queries use Container (DataServiceContext)
    public override IQueryable<Model.NVArticle> Queryable(...)
    {
        return Container.NVArticle
            .AddQueryOption("$filter", where)
            .Select(MapToModel);
    }
}
```

**Advantages**:
- No direct DLL dependency on enventa assemblies
- Can run on non-Windows platforms (if OData service is Windows-hosted)
- Scales horizontally (stateless HTTP calls)

**Disadvantages**:
- Requires separate OData service deployment
- Network overhead for each call
- Limited to OData query capabilities

### 3. Web Services Integration (WS)

**Location**: `Broker/FS_45/NVShop.Data.eNVenta.WS.shared/`, `Broker/FS_47/NVShop.Data.eNVenta.WS.shared/`

**Purpose**: Domain-specific services (Steel processing, Datanorm, etc.)

**Components**:
- `ECSteelService` — Steel processing calculations (quantity, cutting, pricing)
- `NVWSContext` — Specialized context for WS-specific repositories

**Pattern**:
```csharp
public class ECSteelService : IECSteelService
{
    private readonly FSUtil _util;
    
    public ECSteelService(FSUtil util) { _util = util; }
    
    public NVQuantityResponse GetQuantity(NVQuantityRequest request)
    {
        using (var scope = _util.Scope())
        {
            // Steel-specific calculations
            var steelService = scope.Create<IcECSteelService>();
            return steelService.CalculateQuantity(request);
        }
    }
}
```

### Multi-Tenancy Implementation

**BusinessUnit** is the tenant identifier in enventa:

```csharp
// Authentication includes BusinessUnit
public static bool Authenticate(NVIdentity identity)
{
    // identity contains: Name, Password, BusinessUnit
    return FSUtil.Authenticate(identity);
}

// OData uses BusinessUnit in HTTP Basic Auth
var auth = Convert.ToBase64String(
    Encoding.ASCII.GetBytes($"{Name}@{BusinessUnit}:{Password}")
);

// Direct FS API sets BusinessUnit during initialization
using (var scope = _util.Scope())
{
    scope.SetBusinessUnit(identity.BusinessUnit);
    // All operations now scoped to this tenant
}
```

### Query Builder Pattern

eGate implements **fluent query builders** for complex queries:

```csharp
public interface INVArticleQueryBuilder : INVQueryBuilder<NVArticle>
{
    INVArticleQueryBuilder WhereArticleId(string articleId);
    INVArticleQueryBuilder WhereArticleGroup(string group);
    INVArticleQueryBuilder WithTexts();
    INVArticleQueryBuilder WithFeatures();
}

// Usage
var articles = context.Article
    .Query()
    .WhereArticleGroup("ABC")
    .WithTexts()
    .WithFeatures()
    .Select();
```

### Data Mapping Strategy

eGate uses **AutoMapper** for enventa native entities → eGate DTOs:

```csharp
// MapProfile.cs defines mappings
public class MapProfile : Profile
{
    public MapProfile()
    {
        // Map enventa native entity (NV.ERP.*) → eGate DTO (NV*)
        CreateMap<IcECArticle, NVArticle>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name1))
            // ... 50+ property mappings
            
        // IcECArticle = enventa native entity from NV.ERP.MM.ECommerce.ECComponents
        // NVArticle = eGate DTO for application layer
    }
}
```

**Type Mapping Overview**:

| Layer | Type Example | Namespace | Purpose |
|-------|--------------|-----------|---------|
| enventa Native | `IcECArticle` | `NV.ERP.MM.ECommerce.ECComponents` | Real ERP entities |
| enventa Framework | `IFSRepository<T>` | `FrameworkSystems.*`, `FS.*` | Data access, ORM |
| eGate DTO | `NVArticle` | (no namespace) | Application-layer DTOs |

### Testing Approach

eGate demonstrates **integration testing** with real enventa instances:

```csharp
[TestClass]
public class FSDataContextTests
{
    private static FSUtil fsUtil;
    private static IFSRepository<IcECArticle> fsArticleRepository;
    
    [ClassInitialize]
    public static async Task TestInitialize(TestContext context)
    {
        fsUtil = new FSUtil(Bootstrap.Identity);
        fsArticleRepository = FSRepositoryFactory.Create<IcECArticle>(Bootstrap.Identity);
        await GlobalWarmup(); // Pre-warm connections
    }
    
    [ClassCleanup]
    public static void AssemblyCleanup()
    {
        fsUtil.Dispose(); // Clean up connections
    }
}
```

### Repository Pattern Hierarchy

```
INVSelectRepository<T>           # Base: Select, Queryable
    ↓
NVBaseRepository<TDTO, TNative>  # TDTO = eGate DTO (e.g., NVArticle)
    ↓                            # TNative = enventa entity (e.g., IcECArticle)
NVReadRepository<TDTO, TNative>  # Adds: FSUtil, IFSRepository, Mapping
    ↓                            # Adds: GetById, Exists
NVQueryReadRepository<TDTO, TNative>  # Adds: Query builder support
    ↓
NVCrudRepository<TDTO, TNative>  # Adds: Insert, Update, Delete
    ↓
NVQueryCrudRepository<TDTO, TNative>  # Full CRUD + Query builder
```

**Example**:
```csharp
public class NVArticleRepository : 
    NVReadRepository<NVArticle, IcECArticle>,  // DTO, Native Entity
    INVArticleRepository
{
    // NVArticle = eGate DTO (application layer)
    // IcECArticle = enventa native entity (NV.ERP.MM.ECommerce.ECComponents)
    
    public NVArticleRepository(
        IFSRepository<IcECArticle> rep,  // enventa framework repository
        FSUtil util                      // enventa framework utility
    ) : base(rep, util) { }
}
```

### Dependency Injection Pattern

```csharp
public class DependencyRegistrar : IDependencyRegistrar
{
    public void Register(IUnityContainer container)
    {
        // Register FSUtil as scoped
        container.RegisterType<FSUtil>(new HierarchicalLifetimeManager());
        
        // Register all repositories
        container.RegisterType<INVArticleRepository, NVArticleRepository>();
        container.RegisterType<INVOrderRepository, NVOrderRepository>();
        // ... 60+ repositories
        
        // Register services
        container.RegisterType<IECPriceService, ECPriceService>();
        container.RegisterType<IECStockService, ECStockService>();
    }
}
```

### Key Lessons for B2X

1. **Hybrid Data Strategy for Large Catalogs**:
   - **Master Data** (1.5M articles, 30M attributes) → Synchronized to PostgreSQL + Elasticsearch
   - **Customer Data** (prices, stock, orders) → Live queries to ERP
   - **Incremental sync**: Only changed records, staged (Articles → Attributes → Images)
   - **Eventual consistency**: Acceptable delay (minutes to hours) for master data
   - **Strong consistency**: Real-time required for customer prices/stock

2. **Three Integration Options**:
   - **Direct FS API**: Best performance, Windows-only, requires enventa assemblies
   - **OData Broker**: Platform-independent, scalable, requires separate service
   - **Hybrid**: Use Direct for high-frequency, OData for low-frequency

3. **Type System Layers**:
   - **enventa Native** (`NV.ERP.*`) — The actual ERP entities (IcECArticle)
   - **enventa Framework** (`FrameworkSystems.*`, `FS.*`) — Architecture/ORM components
   - **Application DTOs** (`NV*` without namespace) — App-layer data transfer objects

4. **BusinessUnit = Tenant**: Always include in authentication, not separate API call

5. **Scope Pattern**: `using (var scope = _util.Scope())` ensures proper cleanup

6. **Connection Pooling**: Critical for >2s initialization cost (30min idle timeout)

7. **Repository Pattern**: Abstracts enventa native entities from application DTOs

8. **Query Builder**: Fluent API reduces complex query construction errors

9. **Lazy Loading**: NVContext uses `Lazy<T>` for all repositories (deferred instantiation)

10. **Testing**: Integration tests with real enventa instances (not mocks)

### Recommended Approach for B2X

**Hybrid Data Architecture** (eGate-proven pattern):

```
┌────────────────────────────────────────────────────────────────┐
│              User Request: "Show Article X with Price"         │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  Step 1: PostgreSQL + Elasticsearch (synced master data)       │
│  ────────────────────────────────────────────────────────      │
│  SELECT * FROM articles WHERE article_no = @articleNo          │
│  → Article details, attributes, images (eventual consistency)  │
│                                                                │
│  Step 2: ERP Provider (live query via connection pool)         │
│  ────────────────────────────────────────────────────────      │
│  GetCustomerDataAsync(articleNo, customerId, businessUnit)     │
│  → Customer price, stock, discount (strong consistency)        │
│                                                                │
│  Step 3: Combine Results                                       │
│  ────────────────────────────────────────────────────────      │
│  return { ...article, price, stock };                          │
│                                                                │
└────────────────────────────────────────────────────────────────┘
         ↑                                     ↑
         │                                     │
    Sync Job                          Connection Pool
   (Background)                        (Pre-warmed)
         │                                     │
         └──────────────┬──────────────────────┘
                        │
                enventa Trade ERP
            (1.5M Articles, 30M Attributes)
```

**Integration Architecture**:

```
┌────────────────────────────────────────────────────────┐
│  B2X .NET 10 (Linux Container)                   │
│                                                        │
│  ┌──────────────────────────────────────────────────┐ │
│  │  gRPC Client (enventa provider)                  │ │
│  │  + Sync orchestrator (background jobs)           │ │
│  └───────────────────────┬──────────────────────────┘ │
│                          │ gRPC over HTTP/2           │
└──────────────────────────┼────────────────────────────┘
                           │
┌──────────────────────────▼────────────────────────────┐
│  enventa Broker Service (Windows Container)           │
│  .NET Framework 4.8                                   │
│                                                        │
│  ┌──────────────────────────────────────────────────┐ │
│  │  Direct FS API Integration                       │ │
│  │  - FSUtil with Connection Pool                   │ │
│  │  - NVContext with all repositories               │ │
│  │  - Per-tenant Actor for thread safety            │ │
│  │  - Incremental sync support (ModifiedDate)       │ │
│  └──────────────────────────────────────────────────┘ │
│                                                        │
│  ┌──────────────────────────────────────────────────┐ │
│  │  enventa Trade ERP Assemblies (.NET Framework)   │ │
│  │  - IcECArticle, IcECOrder, IcECCustomer, ...     │ │
│  └──────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────┘
```

**Why Hybrid Strategy**:
- ✅ **Fast Catalog Browsing**: 1.5M articles in PostgreSQL + Elasticsearch (200ms search)
- ✅ **Current Prices/Stock**: Live queries for customer-specific data (<100ms)
- ✅ **Reduced ERP Load**: Only customer-specific queries hit ERP (not catalog browsing)
- ✅ **Scalable**: Horizontal scaling for PostgreSQL/Elasticsearch (read replicas)
- ✅ **Proven Pattern**: eGate uses this approach in production

**Why Not Pure OData**:
- OData adds network overhead for every query
- Requires maintaining separate OData service
- Limited query capabilities vs. direct FS API
- Still requires handling 1.5M articles (doesn't solve large catalog problem)

**Why Not Pure Direct FS**:
- B2X runs .NET 10 (not compatible with .NET Framework 4.8 assemblies)
- Cross-framework boundary requires gRPC or similar

**Final Pattern**:
- **Master Data Sync**: Background jobs → PostgreSQL + Elasticsearch
- **Live Customer Queries**: Windows Container with Direct FS API (eGate's FS_47 approach)
- **gRPC Bridge**: .NET 10 ↔ .NET Framework 4.8 (B2X's current approach)
- **Connection Pool**: Per-tenant actors with 30min idle timeout
- **Incremental Sync**: ModifiedDate watermarks for each entity type
- **Master Data Sync**: Background jobs → PostgreSQL + Elasticsearch
- **Live Customer Queries**: Windows Container with Direct FS API (eGate's FS_47 approach)
- **gRPC Bridge**: .NET 10 ↔ .NET Framework 4.8 (B2X's current approach)
- **Connection Pool**: Per-tenant actors with 30min idle timeout
- **Incremental Sync**: ModifiedDate watermarks for each entity type
- **enventa Native Entities**: `IcEC*` types from `NV.ERP.*` namespace
- **enventa Framework**: `FrameworkSystems.*`, `FS.*` for data access
- **B2X DTOs**: Map enventa entities → B2X domain models
- **Actor Pattern**: One actor per tenant (like eGate's architecture)
- **Resilience Pipeline**: Polly-based Circuit Breaker, Retry, Timeout
- **Transaction Scopes**: FSUtil-compatible transaction abstraction
- **Status Tracking**: Operation status and error counting

## References

- **eGate Reference**: [NissenVelten/eGate](https://github.com/NissenVelten/eGate) - Production implementation
- **ADR-023**: [ERP Plugin Architecture](../../.ai/decisions/ADR-023-erp-plugin-architecture.md)
- **Polly Documentation**: [Polly Resilience Library](https://github.com/App-vNext/Polly)
- **Implementation**: `backend/Domain/ERP/src/`
- **Tests**: `backend/Domain/ERP/tests/`
- **Proto Definitions**: `backend/Domain/ERP/src/Protos/`
- **Official Docs**: Contact enventa support for API documentation

## Quick Start for Development

```bash
# Build ERP domain
cd backend/Domain/ERP
dotnet build src/B2X.ERP.csproj

# Run tests
dotnet test tests/B2X.ERP.Tests.csproj --verbosity minimal

# Register provider in DI
services.AddErpServices();
services.AddEnventaProvider();

# Use provider
var providerManager = services.GetRequiredService<IProviderManager>();
var pimProvider = await providerManager.GetPimProviderAsync(tenant);
var product = await pimProvider.GetProductDetailsAsync(productId, tenant, ct);
```

---

**Maintained By**: @Enventa  
**Last Updated**: 2. Januar 2026  
**Next Review**: Quarterly (April 2026)
