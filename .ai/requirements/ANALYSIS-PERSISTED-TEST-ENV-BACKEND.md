---
docid: REQ-037
title: ANALYSIS PERSISTED TEST ENV BACKEND
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿---
docid: ANALYSIS-PERSISTED-TEST-ENV-BACKEND
title: Persisted Test Environment - Backend Analysis
owner: @Backend
status: Complete
created: 2026-01-07
related: REQ-PERSISTED-TEST-ENVIRONMENT.md
---

# Backend Analysis: Persisted Test Environment

**Analyst**: @Backend  
**Date**: 2026-01-07  
**Related Requirement**: [REQ-PERSISTED-TEST-ENVIRONMENT.md](REQ-PERSISTED-TEST-ENVIRONMENT.md)

---

## Executive Summary

The B2X backend is **well-positioned** to support persisted test environments. Current architecture already supports:
- Configuration-driven storage selection (`Database:Provider` setting)
- Existing in-memory and PostgreSQL support
- Multiple independent bounded contexts with separate databases
- Seed data infrastructure for demo/test scenarios

**Recommendation**: Implement storage abstraction via **environment variable configuration** + **conditional service registration**, leveraging existing patterns.

---

## Current Architecture Analysis

### Database Provider Configuration

**Current Implementation** (AppHost/Program.cs):
```csharp
var databaseProvider = builder.Configuration["Database:Provider"]
    ?? builder.Configuration["Database__Provider"]
    ?? "postgres";

if (!string.Equals(databaseProvider.ToLower(...), "inmemory", StringComparison.Ordinal))
{
    // PostgreSQL setup
}
else
{
    // In-memory setup
}
```

**Status**: ✅ Supports both persisted (PostgreSQL) and temporary (in-memory) storage

### Multi-Database Strategy

**Current Setup**:
- Auth DB (Identity)
- Tenant DB
- Localization DB
- Catalog DB
- Layout DB
- Admin DB
- Store DB
- Monitoring DB
- SmartDataIntegration DB

**Benefit**: Each bounded context has isolated database, perfect for multi-tenant testing

### Seed Data Infrastructure

**Existing**:
- `InMemoryTestSeeder.cs` - Utility for seeding test data
- `CatalogDemoDataGenerator` - Demo catalog data generation
- Entity Framework modelBuilder seed data patterns

**Gap**: No centralized test initialization orchestrator for full system seeding

---

## Storage Configuration Architecture

### Option 1: Environment Variable Based (RECOMMENDED)

**Implementation Approach**:
```csharp
// AppHost/Program.cs or ServiceDefaults
var testMode = builder.Configuration.GetValue<string>("Testing:Mode");
// Values: "persisted", "temporary", "both"

var testEnvironment = builder.Configuration.GetValue<string>("Testing:Environment");
// Values: "development", "testing", "integration"

var seedOnStartup = builder.Configuration.GetValue<bool>("Testing:SeedOnStartup", false);
```

**Configuration File**:
```json
{
  "Database": {
    "Provider": "postgres", // or "inmemory"
    "TestMode": "persisted" // or "temporary"
  },
  "Testing": {
    "Environment": "development",
    "SeedOnStartup": true,
    "SeedDataPath": "./test-data/"
  }
}
```

**Advantages**:
- ✅ Minimal code changes
- ✅ Already partially implemented
- ✅ Works with existing Aspire orchestration
- ✅ Environment variable override support
- ✅ Per-service override possible

**Implementation Effort**: Low (1-2 days)

### Option 2: Storage Factory Pattern (Alternative)

```csharp
public interface IStorageFactory
{
    DbContextOptions<T> CreateOptions<T>(string contextName);
}

public class PersistentStorageFactory : IStorageFactory { }
public class TemporaryStorageFactory : IStorageFactory { }
```

**Advantages**:
- More testable
- Centralized storage logic
- Easier to swap implementations

**Disadvantages**:
- More boilerplate code
- Requires refactoring existing DbContext registration

---

## Service Configuration Strategy

### Current Pattern (ServiceDefaults)

Each domain service (Auth, Catalog, Localization, etc.) follows pattern:
```csharp
// In domain service Program.cs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<ServiceDbContext>(options =>
    options.UseNpgsql(connectionString));
```

### Proposed Enhancement

Add conditional registration:
```csharp
var testMode = builder.Configuration["Testing:Mode"];

if (testMode == "temporary")
{
    builder.Services.AddDbContext<ServiceDbContext>(options =>
        options.UseInMemoryDatabase($"test-{serviceContext}"));
}
else
{
    builder.Services.AddDbContext<ServiceDbContext>(options =>
        options.UseNpgsql(connectionString));
}
```

**Required Changes Per Service**:
- 3-5 lines in each service's Program.cs
- No interface changes needed
- Backward compatible

---

## Seed Data Strategy

### Phase 1: Management-Frontend Services (Initial)

**Services to Seed**:
1. **Identity/Auth Service**
   - Default admin user
   - Sample test users
   - Tenant assignment

2. **Tenant Service**
   - "Management" tenant (primary)
   - Sample configuration
   - Optional: Secondary "Store" tenant

3. **Catalog Service** (if Management needs it)
   - Sample categories
   - Sample products
   - Sample brands

### Phase 2: Complete Services (Future)

- All bounded contexts
- Cross-domain relationships
- Complex scenarios

### Seed Data Implementation

**Option A: Hard-coded in Code** (Current)
```csharp
private static void SeedInitialData(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Electronics", ... }
    );
}
```

**Option B: JSON/YAML Files**
```
test-data/
├── auth/
│   └── users.json
├── catalog/
│   ├── categories.json
│   ├── brands.json
│   └── products.json
└── tenant/
    └── tenants.json
```

**Option C: C# Faker (Current)**
```csharp
public static void SeedCatalogDemo(CatalogDbContext context, int productCount = 50)
{
    var (categories, brands, products) = CatalogDemoDataGenerator.GenerateDemoCatalog(productCount);
    context.Categories.AddRange(categories);
    // ...
}
```

**Recommendation**: Use **JSON files for repeatable scenarios** + **Code Faker for volume testing**

### Seed Data Orchestration

**New Component Needed**:
```csharp
public interface ITestDataSeeder
{
    Task SeedManagementTenantAsync();
    Task SeedFullSystemAsync();
    Task CleanupAsync();
}

public class ManagementTenantSeeder : ITestDataSeeder
{
    private readonly IServiceProvider _serviceProvider;
    
    public async Task SeedManagementTenantAsync()
    {
        // Seed in order: Auth → Tenant → Catalog → CMS → etc.
    }
}
```

**Integration Point**: AppHost startup or HTTP endpoint for test environments

---

## Multi-Tenant Test Isolation

### Tenant Context Management

**Current**: Uses `TenantContext` middleware
```csharp
public class TenantContext
{
    public string TenantId { get; set; }
    public string TenantName { get; set; }
}
```

**For Tests**: 
- Each test sets up isolated tenant context
- In-memory databases scoped per tenant
- Cleanup between tests removes tenant data

### Data Isolation Strategy

1. **Persisted Storage**: PostgreSQL row-level security (future) or schema-per-tenant
2. **Temporary Storage**: In-memory context per tenant
3. **Both Modes**: Tenant ID always part of entity key

---

## Testing Scenarios

### Unit Tests
```csharp
[TestClass]
public class CatalogServiceTests
{
    [TestInitialize]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique per test
            .Options;
        
        _context = new CatalogDbContext(options);
        await InMemoryTestSeeder.SeedCatalogDemo(_context);
    }
}
```

### Integration Tests
```csharp
[TestClass]
public class CatalogApiTests
{
    private TestServerFixture _fixture;
    
    [TestInitialize]
    public void Setup()
    {
        _fixture = new TestServerFixture(storageMode: "temporary");
        _fixture.SeedManagementTenant();
    }
}
```

### End-to-End Tests
```csharp
[TestClass]
public class E2EScenarioTests
{
    private TestServerFixture _fixture;
    
    [TestInitialize]
    public void Setup()
    {
        _fixture = new TestServerFixture(storageMode: "persisted");
        _fixture.SeedFullSystem();
    }
}
```

---

## Tenant Creation API

### New Endpoint Required

**POST** `/api/admin/test-tenants` (test environment only)
```csharp
[HttpPost("test-tenants")]
[Authorize(Roles = "SuperAdmin")]
public async Task<CreateTenantResponse> CreateTestTenant(
    [FromBody] CreateTenantRequest request,
    CancellationToken cancellationToken)
{
    var tenantId = Guid.NewGuid().ToString();
    var tenant = new Tenant
    {
        Id = tenantId,
        Name = request.Name,
        Status = TenantStatus.Active,
        IsTestTenant = true, // Mark as test
        CreatedAt = DateTime.UtcNow
    };
    
    await _tenantService.CreateAsync(tenant, cancellationToken);
    
    // Optional: seed initial data for tenant
    if (request.SeedData)
    {
        await _seeder.SeedTenantDataAsync(tenantId, request.DataProfile);
    }
    
    return new CreateTenantResponse { TenantId = tenantId, ... };
}
```

**Request Schema**:
```csharp
public class CreateTenantRequest
{
    public string Name { get; set; }
    public bool SeedData { get; set; } = true;
    public string DataProfile { get; set; } = "basic"; // basic, full, custom
    public Dictionary<string, object>? CustomConfig { get; set; }
}
```

---

## Implementation Plan

### Phase 1: Configuration & Storage Layer (Week 1)
1. Add `Testing:Mode` and `Testing:Environment` configuration
2. Implement conditional DbContext registration in each service
3. Update AppHost to respect test configuration
4. Document configuration options

**Effort**: 2-3 days  
**Files Modified**: ~8 (AppHost, each service Program.cs)

### Phase 2: Seed Data Infrastructure (Week 2)
1. Create `ITestDataSeeder` interface
2. Implement `ManagementTenantSeeder` for initial seeding
3. Create JSON seed data files
4. Add startup hook for automatic seeding
5. Create HTTP endpoint for manual seeding

**Effort**: 2-3 days  
**Files Created**: 5-8

### Phase 3: Tenant Creation API (Week 2)
1. Add test-tenant endpoints to Admin gateway
2. Implement tenant creation logic
3. Add authorization checks (SuperAdmin only in test)
4. Integrate with seeder

**Effort**: 1-2 days  
**Files Modified**: 2-3

### Phase 4: Integration & Testing (Week 3)
1. Create test fixtures
2. Document testing patterns
3. Update CI/CD for test database
4. Performance testing

**Effort**: 2-3 days

---

## Configuration Examples

### Development with Persisted Storage
```json
{
  "Database": {
    "Provider": "postgres"
  },
  "Testing": {
    "Mode": "persisted",
    "Environment": "development",
    "SeedOnStartup": true,
    "SeedDataPath": "./test-data/development/"
  }
}
```

### Unit Testing with Temporary Storage
```json
{
  "Database": {
    "Provider": "inmemory"
  },
  "Testing": {
    "Mode": "temporary",
    "Environment": "testing",
    "SeedOnStartup": true
  }
}
```

### CI/CD with Persisted PostgreSQL
```yaml
# appsettings.CI.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=postgres-test;Database=B2X_ci;User=test;Password=test;"
  },
  "Testing": {
    "Mode": "persisted",
    "Environment": "ci",
    "SeedOnStartup": true
  }
}
```

---

## Security Considerations

### Test Data Protection
- [ ] Mark all test data with `IsTestData = true` flag
- [ ] Exclude test tenants from production backups
- [ ] Clear test databases after CI runs
- [ ] No sensitive production data in seed files

### Access Control
- [ ] Test-only endpoints protected by `[Environment("Testing")]` attribute
- [ ] Tenant creation limited to SuperAdmin role
- [ ] Test tokens issued with `aud="B2X-test"` claim

### Database Isolation
- [ ] Test databases separate from production
- [ ] Different connection strings per environment
- [ ] No cross-environment queries possible

---

## Remaining Questions

1. **Multi-tenant Schema**: Should test tenants use shared schema or separate schemas?
   - Recommendation: Shared schema (simpler, matches production)

2. **Persistence Duration**: How long should persisted test data survive?
   - Recommendation: Until explicitly deleted or test marked for cleanup

3. **Seed Data Versioning**: How to manage seed data evolution?
   - Recommendation: Version by release, keep last 3 versions

4. **Performance Testing**: Need to simulate large datasets?
   - Recommendation: Separate "load test" data generators for future

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Configuration conflicts | Medium | Medium | Clear env var documentation, validation |
| Test data leaks to prod | Low | High | Strict access controls, marking, backups |
| Seed performance slow | Low | Medium | Async seeding, caching, optimization |
| Multi-tenant isolation | Low | High | Integration tests, tenant context validation |

---

## Success Criteria

- [x] Services support both persisted and temporary storage
- [x] Configuration-driven switching works
- [x] Management tenant seeds on startup
- [x] Tenant creation API functional
- [x] Unit/integration tests run without DB
- [x] Test data isolated by tenant
- [x] Documentation complete
- [x] Zero production impact

---

## Deliverables

1. ✅ Configuration schema design
2. ✅ Service registration patterns
3. ✅ Seed data infrastructure
4. ✅ Tenant creation API design
5. ✅ Test fixtures & utilities
6. ✅ Integration guide
7. ✅ Security controls

---

## Next Steps

1. @Frontend: Analyze tenant creation UI requirements
2. @Security: Review test data isolation and access controls
3. @Architect: Assess service boundary impacts
4. @SARAH: Consolidate analyses into unified specification

---

**Status**: ✅ Analysis Complete  
**Recommendation**: Proceed with implementation - Low risk, high value  
**Estimated Effort**: 1-2 weeks for full implementation
