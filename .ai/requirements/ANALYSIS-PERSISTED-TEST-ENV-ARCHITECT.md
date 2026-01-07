---
docid: ANALYSIS-PERSISTED-TEST-ENV-ARCHITECT
title: Persisted Test Environment - Architecture Analysis
owner: @Architect
status: Complete
created: 2026-01-07
related: REQ-PERSISTED-TEST-ENVIRONMENT.md
---

# Architecture Analysis: Persisted Test Environment

**Analyst**: @Architect  
**Date**: 2026-01-07  
**Related Requirement**: [REQ-PERSISTED-TEST-ENVIRONMENT.md](REQ-PERSISTED-TEST-ENVIRONMENT.md)

---

## Executive Summary

The Persisted Test Environment feature is **architecturally sound** and aligns with B2Connect's multi-tenant, service-oriented design. The feature:

✅ Follows existing architectural patterns (configuration-driven, multi-tenant aware)  
✅ Requires minimal service boundary changes  
✅ Leverages existing Aspire orchestration  
✅ Maintains separation of concerns  
✅ Enables horizontal scaling for testing  

**Recommendation**: Implement using **conditional service registration** pattern with **centralized seeding orchestrator**. No architectural changes needed; configuration enhancements only.

---

## Current Architecture Review

### Multi-Tenant Design

**Current State**:
```
┌─────────────────────────────────────────────────┐
│           Aspire Orchestration                  │
├─────────────────────────────────────────────────┤
│                                                 │
│  API Gateways                                   │
│  ├─ Store Gateway                               │
│  └─ Admin Gateway                               │
│                                                 │
│  Domain Services (Bounded Contexts)             │
│  ├─ Auth Service         → AuthDb               │
│  ├─ Tenant Service       → TenantDb             │
│  ├─ Localization Service → LocalizationDb      │
│  ├─ Catalog Service      → CatalogDb           │
│  ├─ CMS Service          → CmsDb               │
│  └─ ...more services...                         │
│                                                 │
│  Shared Infrastructure                          │
│  ├─ PostgreSQL (persistence)                    │
│  ├─ Redis (caching)                             │
│  ├─ Elasticsearch (search)                      │
│  ├─ RabbitMQ (messaging)                        │
│  └─ etc.                                        │
│                                                 │
└─────────────────────────────────────────────────┘

Each Service:
├─ Receives TenantContext via middleware
├─ Filters queries by TenantId
├─ Stores data in shared database (same schema)
└─ Scales independently
```

**Multi-Tenant Isolation**: Logical (row-level), not physical

### Service Boundaries

**Key Boundaries**:
1. **Auth Service**: User identity & authentication
2. **Tenant Service**: Tenant management & configuration
3. **Domain Services**: Business logic (Catalog, CMS, etc.)
4. **Gateway Services**: API aggregation & routing
5. **Shared Services**: Logging, localization, search

**Dependency Flow**:
```
Gateway Layer
    ↓
Domain Services (loosely coupled via messaging)
    ↓
Shared Infrastructure
    ↓
Data Layer (DbContexts)
```

---

## Testing Environment Architecture

### Proposed Design

```
┌──────────────────────────────────────────────────────┐
│  Persisted Test Environment Architecture             │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Configuration Layer                                 │
│  ├─ appsettings.Testing.json                         │
│  ├─ Testing:Mode (persisted|temporary)               │
│  ├─ Testing:Environment (dev|testing|ci)             │
│  └─ Testing:SeedOnStartup (true|false)               │
│                                                      │
│  Service Registration Layer                          │
│  ├─ Conditional DbContext setup                      │
│  ├─ Test-only repositories                           │
│  └─ Seeding orchestrator registration                │
│                                                      │
│  Data Layer                                          │
│  ├─ Persisted (PostgreSQL test database)             │
│  │   ├─ auth_test                                    │
│  │   ├─ tenant_test                                  │
│  │   ├─ catalog_test                                 │
│  │   └─ ...                                          │
│  │                                                   │
│  └─ Temporary (In-Memory)                            │
│      ├─ In-memory repositories                       │
│      ├─ Scoped per test                              │
│      └─ No persistence                               │
│                                                      │
│  Seeding Layer                                       │
│  ├─ ITestDataSeeder interface                        │
│  ├─ ManagementTenantSeeder (Phase 1)                 │
│  ├─ FullSystemSeeder (Phase 2)                       │
│  └─ Seed data files (JSON)                           │
│                                                      │
│  API Layer                                           │
│  ├─ Test-only endpoints (/api/admin/test-tenants)    │
│  ├─ Environment-restricted                           │
│  └─ Logging & audit trails                           │
│                                                      │
└──────────────────────────────────────────────────────┘
```

---

## Service Integration Points

### 1. Auth Service

**Current Architecture**:
- Manages user identity (AppUser)
- Issues JWT tokens
- Enforces tenant context
- Uses Identity framework

**Integration for Tests**:
```csharp
// Current
public class AuthDbContext : IdentityDbContext<AppUser>
{
    // Uses real PostgreSQL by default
}

// With Test Support
public class AuthDbContext : IdentityDbContext<AppUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) 
        : base(options) { }
    
    // Same schema, different backend
    // Configured based on Testing:Mode
}

// In Auth Service Program.cs
var testMode = builder.Configuration["Testing:Mode"];

if (testMode == "temporary")
{
    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseInMemoryDatabase("auth-test"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("AuthDb");
    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(connectionString));
}
```

**Test Users Creation**:
```csharp
public interface ITestAuthSeeder
{
    Task SeedDefaultAdminAsync(string tenantId);
    Task SeedTestUsersAsync(string tenantId, int count = 5);
    Task CleanupAsync();
}

public class TestAuthSeeder : ITestAuthSeeder
{
    private readonly UserManager<AppUser> _userManager;
    
    public async Task SeedDefaultAdminAsync(string tenantId)
    {
        var admin = new AppUser
        {
            UserName = "test.admin@b2connect.local",
            Email = "test.admin@b2connect.local",
            FirstName = "Test",
            LastName = "Admin",
            TenantId = tenantId,
            IsTestData = true,
            EmailConfirmed = true
        };
        
        await _userManager.CreateAsync(admin, "TestPassword123!");
        await _userManager.AddToRoleAsync(admin, "SuperAdmin");
    }
}
```

### 2. Tenant Service

**Current Architecture**:
- Creates and manages tenant records
- Stores configuration
- Maintains tenant-service relationships

**Integration for Tests**:
```csharp
public class Tenant : AuditableEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    public TenantStatus Status { get; set; }
    public bool IsTestTenant { get; set; }  // NEW
    public string StorageMode { get; set; }  // NEW: persisted|temporary
    public string DataProfile { get; set; }  // NEW: basic|full|custom
    public DateTime CreatedAt { get; set; }
}

// API Endpoint (test-only)
[HttpPost("test-tenants")]
[Environment("Testing", "Development")]
[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> CreateTestTenant(
    [FromBody] CreateTestTenantRequest request)
{
    var tenant = new Tenant
    {
        Id = Guid.NewGuid().ToString(),
        Name = request.Name,
        IsTestTenant = true,
        StorageMode = request.StorageMode,
        DataProfile = request.DataProfile,
        Status = TenantStatus.Active,
        CreatedAt = DateTime.UtcNow
    };
    
    await _tenantService.CreateAsync(tenant);
    
    // Seed initial data if requested
    if (request.SeedData)
    {
        await _seeder.SeedTenantAsync(tenant.Id, request.DataProfile);
    }
    
    return CreatedAtAction(nameof(GetTenant), tenant);
}
```

### 3. Catalog & Domain Services

**No Changes Required** - Services already support:
- ✅ Multi-tenant queries (TenantId filtering)
- ✅ Configurable storage (DbContext options)
- ✅ Seed data patterns (demo generators)

**Example: Catalog Service**
```csharp
// Existing code already works
public class CatalogService
{
    private readonly CatalogDbContext _context;
    private readonly ITenantContext _tenantContext;
    
    public async Task<List<Product>> GetProductsAsync()
    {
        // Automatically scoped to current tenant
        return await _context.Products
            .Where(p => p.TenantId == _tenantContext.TenantId)
            .ToListAsync();
    }
}
```

### 4. Gateway Services

**No Changes Required** - Gateways:
- ✅ Route requests based on TenantContext
- ✅ Aggregate domain service responses
- ✅ Enforce authentication

**New Test Endpoints** (Admin Gateway only):
```csharp
[ApiController]
[Route("api/admin")]
public class AdminGatewayTestController
{
    [HttpPost("test-tenants")]
    [Environment("Testing")]
    public async Task<IActionResult> CreateTestTenant(...) { }
    
    [HttpGet("test-tenants")]
    [Environment("Testing")]
    public async Task<IActionResult> ListTestTenants(...) { }
}
```

---

## Seeding Architecture

### Centralized Orchestrator Pattern

**Problem**: Seeding involves multiple services in specific order
- Auth first (create users)
- Tenant second (link to users)
- Catalog third (add products)
- CMS fourth (add content)
- etc.

**Solution**: Centralized orchestrator

```csharp
public interface ITestDataOrchestrator
{
    Task SeedManagementTenantAsync();
    Task SeedFullSystemAsync();
    Task SeedTenantAsync(string tenantId, string profile);
    Task CleanupAsync();
}

public class TestDataOrchestrator : ITestDataOrchestrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TestDataOrchestrator> _logger;
    
    public async Task SeedManagementTenantAsync()
    {
        var tenantId = Guid.NewGuid().ToString();
        
        try
        {
            // 1. Create tenant record
            using var scope = _serviceProvider.CreateScope();
            var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
            var tenant = new Tenant { Id = tenantId, Name = "Management", IsTestTenant = true };
            await tenantService.CreateAsync(tenant);
            
            // 2. Seed auth (users)
            var authSeeder = scope.ServiceProvider.GetRequiredService<ITestAuthSeeder>();
            await authSeeder.SeedDefaultAdminAsync(tenantId);
            
            // 3. Seed other services as needed
            var catalogSeeder = scope.ServiceProvider.GetRequiredService<ITestCatalogSeeder>();
            await catalogSeeder.SeedBasicAsync(tenantId);
            
            _logger.LogInformation("✅ Management tenant seeded: {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to seed management tenant");
            throw;
        }
    }
}
```

**Registration**:
```csharp
// In ServiceDefaults or each service
builder.Services.AddScoped<ITestDataOrchestrator, TestDataOrchestrator>();
builder.Services.AddScoped<ITestAuthSeeder, TestAuthSeeder>();
builder.Services.AddScoped<ITestCatalogSeeder, TestCatalogSeeder>();

// Seed on startup if configured
if (builder.Configuration.GetValue<bool>("Testing:SeedOnStartup"))
{
    var orchestrator = app.Services.GetRequiredService<ITestDataOrchestrator>();
    await orchestrator.SeedManagementTenantAsync();
}
```

---

## Configuration Strategy

### Hierarchical Configuration

```csharp
// 1. Defaults (code)
var testMode = "persisted";
var seedOnStartup = false;

// 2. appsettings.json
{
  "Testing": {
    "Mode": "persisted",
    "Environment": "development",
    "SeedOnStartup": false
  }
}

// 3. appsettings.Development.json
{
  "Testing": {
    "SeedOnStartup": true  // Override for dev
  }
}

// 4. appsettings.Testing.json
{
  "Database": {
    "Provider": "inmemory"  // Fast tests
  },
  "Testing": {
    "Mode": "temporary",
    "SeedOnStartup": true
  }
}

// 5. Environment variables (highest priority)
Testing__Mode=persisted
Testing__Environment=ci
```

### Configuration Schema

```csharp
public class TestingConfiguration
{
    public string Mode { get; set; } = "persisted"; // persisted|temporary|both
    public string Environment { get; set; } = "development"; // dev|testing|ci
    public bool SeedOnStartup { get; set; } = false;
    public string SeedDataPath { get; set; } = "./test-data/";
    public int MaxTenants { get; set; } = 100; // Limit for test environments
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);
}

// Bind in Program.cs
var testingConfig = new TestingConfiguration();
builder.Configuration.GetSection("Testing").Bind(testingConfig);
```

---

## Database Strategy

### Schema Design

**Same Schema for All Storage Modes**:
```
Option A: Single Schema (RECOMMENDED)
├─ Persisted: PostgreSQL table
├─ Temporary: In-memory entity collection
└─ Benefit: No mapping differences, EF handles both

Option B: Separate Schemas
├─ Persisted: Public schema
├─ Temporary: Test schema (prefixed with "test_")
└─ Drawback: Requires schema switching logic
```

**Recommendation**: Use Option A - EF Core handles both transparently

### Connection Strings

```json
{
  "ConnectionStrings": {
    "AuthDb": "Server=postgres;Database=auth;User=postgres;Password=***;",
    "AuthDbTest": "Server=postgres;Database=auth_test;User=postgres;Password=***;",
    "TenantDb": "Server=postgres;Database=tenant;User=postgres;Password=***;",
    "TenantDbTest": "Server=postgres;Database=tenant_test;User=postgres;Password=***;",
    "CatalogDb": "Server=postgres;Database=catalog;User=postgres;Password=***;",
    "CatalogDbTest": "Server=postgres;Database=catalog_test;User=postgres;Password=***;"
  }
}
```

**Selection Logic**:
```csharp
var baseConnection = builder.Configuration.GetConnectionString("AuthDb");
var testMode = builder.Configuration["Testing:Mode"];

var connectionString = testMode == "persisted"
    ? builder.Configuration.GetConnectionString("AuthDbTest") ?? baseConnection.Replace("auth", "auth_test")
    : null; // In-memory, no connection string

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    if (testMode == "temporary")
    {
        options.UseInMemoryDatabase("auth-test");
    }
    else
    {
        options.UseNpgsql(connectionString);
    }
});
```

---

## Messaging & Event Architecture

### Current State

**RabbitMQ for Async Communication**:
```
Service A → RabbitMQ → Service B
↓
Event handlers in each service
```

**With Testing**:
- ✅ In-memory mode: Use in-memory event bus
- ✅ Persisted mode: Use actual RabbitMQ
- ✅ No changes needed to event handlers

**Implementation**:
```csharp
// Conditional transport
if (testMode == "temporary")
{
    builder.AddInMemoryMessaging(); // In-memory transport
}
else
{
    builder.AddRabbitMQMessaging(config); // Real RabbitMQ
}

// Wolverine handlers work with both
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Handle(OrderCreated @event)
    {
        // Works with both in-memory and RabbitMQ
    }
}
```

---

## Caching Architecture

### Redis Handling

**Current**: Redis for caching across services

**With Testing**:
```csharp
if (testMode == "temporary")
{
    builder.Services.AddDistributedMemoryCache(); // In-memory cache
}
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
        options.Configuration = redisConnection);
}

// Code uses IDistributedCache - implementation swapped
public class CatalogService
{
    private readonly IDistributedCache _cache;
    
    public async Task<Product> GetProductAsync(string id)
    {
        var cached = await _cache.GetAsync(id);
        if (cached != null) return JsonSerializer.Deserialize<Product>(cached);
        
        // ... load from DB
    }
}
```

---

## Scaling Considerations

### Horizontal Scaling for Testing

```
Test Environment Scaling:
├─ Development
│   └─ Single instance with full stack
│
├─ Integration Testing
│   ├─ Multiple service instances
│   ├─ Isolated databases (one per test run)
│   └─ Parallel test execution
│
└─ CI/CD Pipeline
    ├─ Dockerized services
    ├─ Ephemeral databases
    └─ Cleanup on completion
```

### Service Replication

```csharp
// Each test can get isolated instance
public class TestServiceFactory
{
    public async Task<ServiceContext> CreateIsolatedServiceAsync()
    {
        var tenantId = Guid.NewGuid().ToString();
        
        // Create dedicated databases
        var authDb = await CreateDatabaseAsync($"auth-test-{tenantId}");
        var catalogDb = await CreateDatabaseAsync($"catalog-test-{tenantId}");
        
        // Return context with isolated connections
        return new ServiceContext
        {
            TenantId = tenantId,
            AuthConnection = authDb,
            CatalogConnection = catalogDb
        };
    }
}
```

---

## API Gateway Routing

### Current Routing

```
API Gateway
├─ /api/store/* → Store Gateway (public)
├─ /api/admin/* → Admin Gateway (protected)
└─ /api/management/* → Management Gateway (admin-only)
```

### Test Endpoints Routing

```
API Gateway
├─ /api/admin/
│   ├─ ... (existing)
│   └─ test-tenants/* (NEW, testing-only)
│
└─ Routing Rules:
    ├─ /api/admin/test-tenants → Admin Gateway
    ├─ Environment restriction: Testing|Development only
    └─ Authorization: SuperAdmin role required
```

**No Changes to Store Gateway** - Test endpoints admin-only

---

## Deployment Architecture

### Development
```
AppHost (Aspire)
├─ All services in-process
├─ PostgreSQL/Redis/RabbitMQ containerized
├─ Testing:Mode = persisted
└─ SeedOnStartup = true
```

### Testing (CI/CD)
```
Docker Compose
├─ Each service in container
├─ Ephemeral PostgreSQL instances
├─ Testing:Mode = temporary (or persisted with cleanup)
└─ SeedOnStartup = true
```

### Production
```
Kubernetes
├─ Services distributed
├─ No test endpoints compiled in
├─ Testing:Enabled = false
└─ SeedOnStartup = false
```

---

## Service Boundary Changes

**Summary**: **ZERO breaking changes to service boundaries**

| Boundary | Change | Impact |
|----------|--------|--------|
| Auth ↔ Tenant | Tenant creation API | Internal only |
| Tenant ↔ Domain | TenantId filtering | No API change |
| Domain ↔ Domain | Event handling | No change |
| Gateway ↔ Domain | Routing | Add test routes only |

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Storage mode mismatch | Low | Medium | Config validation |
| Seeding order problems | Medium | Low | Orchestrator pattern |
| Tenant isolation issues | Low | High | Integration tests |
| Performance degradation | Medium | Low | In-memory defaults |
| Deployment complexity | Low | Medium | Documentation |

---

## Implementation Approach

### Phase 1: Core Infrastructure (Week 1)
1. Add configuration schema
2. Implement conditional DbContext registration
3. Create seeding orchestrator interface
4. Add startup validation

**Effort**: 2 days  
**Services Modified**: All (5 lines each)

### Phase 2: Service Integration (Week 1-2)
1. Implement Auth seeder
2. Implement Catalog seeder (if needed for Phase 1)
3. Create seed data files
4. Wire up orchestrator

**Effort**: 2 days  
**Services Modified**: Auth, Catalog (if needed)

### Phase 3: API & Frontend (Week 2)
1. Add test tenant endpoints
2. Implement frontend UI
3. Add auth/admin gating
4. Documentation

**Effort**: 2 days

### Phase 4: Testing & Validation (Week 3)
1. Integration tests
2. Tenant isolation tests
3. Performance testing
4. Documentation

**Effort**: 2 days

---

## Success Criteria

- ✅ Services support both storage modes transparently
- ✅ Seeding follows deterministic order
- ✅ No service boundary breaking changes
- ✅ Tenant isolation enforced
- ✅ Configuration-driven (no code changes for mode switch)
- ✅ Production build excludes test code
- ✅ All tests pass (unit, integration, e2e)

---

## Next Steps

1. @Backend: Implement service registration patterns
2. @Frontend: Build tenant management UI
3. @Security: Add environment gating & audit logging
4. @SARAH: Consolidate analyses into unified spec

---

**Status**: ✅ Analysis Complete  
**Architectural Fit**: Excellent  
**Recommendation**: Proceed with implementation  
**Estimated Effort**: 1-2 weeks for full integration
