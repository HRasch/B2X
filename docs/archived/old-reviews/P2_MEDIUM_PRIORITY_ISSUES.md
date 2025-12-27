# P2 - Medium Priority Security Issues
**Status**: 📋 Ready for Implementation  
**Total Issues**: 5  
**Recommended Start Order**: P2.1 → P2.2 → P2.3 → P2.4 → P2.5

---

## P2.1: Database Transparent Data Encryption (TDE)

**Severity**: HIGH  
**Risk**: Database files unencrypted on disk  
**Business Impact**: Data breach if server compromised  
**Implementation Effort**: 2-3 hours

### Current State:
- ❌ No database encryption
- ❌ EF Core using InMemory (dev) or SQL Server (prod)
- ❌ No disk-level encryption

### Target State:
- ✅ SQL Server Transparent Data Encryption (TDE) enabled
- ✅ Master Database Encryption Key secured
- ✅ Automated encryption of new objects
- ✅ Performance monitoring

### Implementation Steps:
1. Create Master Key in SQL Server
2. Create Database Encryption Key
3. Enable encryption on database
4. Monitor encryption status
5. Backup encryption keys

### Code Changes Needed:
```csharp
// In migration or setup script:
// CREATE MASTER KEY ENCRYPTION BY PASSWORD = '[Complex Password]'
// CREATE DATABASE ENCRYPTION KEY WITH ALGORITHM = AES_256 
//   ENCRYPTION BY SERVER CERTIFICATE [CertificateName];
// ALTER DATABASE [DatabaseName] SET ENCRYPTION ON;
```

### Configuration:
- Store Master Key password in Azure Key Vault
- Monitor: sys.dm_database_encryption_keys view
- Backup: Database and encryption keys regularly

### Testing:
- Verify encryption status in SSMS
- Test backup/restore with encrypted database
- Monitor performance impact

### Rollback Plan:
- Keep backup of unencrypted database
- TDE can be disabled if needed (decrypt offline)

---

## P2.2: API Versioning Strategy

**Severity**: MEDIUM  
**Risk**: Breaking changes breaking client applications  
**Business Impact**: Client integration complexity  
**Implementation Effort**: 1-2 hours

### Current State:
- ❌ No versioning strategy
- ❌ Single API endpoint versions
- ❌ No deprecation plan

### Target State:
- ✅ URL-based versioning (/api/v1/, /api/v2/)
- ✅ Deprecation policy documented
- ✅ Support windows defined
- ✅ Version-specific configurations

### Implementation Options:

**Option A: URL-Based (Recommended)**
```csharp
app.MapGroup("/api/v1")
   .WithTags("v1")
   .MapIdentityEndpoints();

app.MapGroup("/api/v2")
   .WithTags("v2")
   .MapIdentityEndpointsV2();
```

**Option B: Header-Based**
```csharp
if (context.Request.Headers.TryGetValue("api-version", out var version))
{
    // Route to appropriate handler
}
```

### Configuration:
- `appsettings.json`: `"ApiVersioning": { "DefaultVersion": "1.0" }`
- Support windows: v1 (1 year), v2 (2 years)
- Deprecation warnings in response headers

### Code Changes Needed:
1. Add ApiVersion to response headers
2. Create versioned endpoint groups
3. Implement version-specific DTOs
4. Document deprecation timeline

### Testing:
- Test both versions simultaneously
- Verify backward compatibility
- Test deprecation headers

### Migration Path:
- Support 2 versions at a time
- 6-month deprecation notice
- 1-year support window minimum

---

## P2.3: Request Tracing & Distributed Logging

**Severity**: MEDIUM  
**Risk**: Difficult debugging across microservices  
**Business Impact**: Slow incident response, poor observability  
**Implementation Effort**: 2-3 hours

### Current State:
- ❌ No distributed tracing
- ✅ Serilog logging only
- ❌ No correlation IDs
- ❌ No trace visualization

### Target State:
- ✅ OpenTelemetry integration
- ✅ Jaeger tracing setup
- ✅ Correlation IDs in all requests
- ✅ Distributed trace visualization

### Implementation:

**Step 1: Install Packages**
```bash
dotnet add package OpenTelemetry.Exporter.Jaeger
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore
```

**Step 2: Configure OpenTelemetry**
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddJaegerExporter(o =>
            {
                o.AgentHost = builder.Configuration["Jaeger:AgentHost"];
                o.AgentPort = int.Parse(builder.Configuration["Jaeger:AgentPort"]);
            });
    });
```

**Step 3: Add Correlation IDs**
```csharp
app.Use((context, next) =>
{
    var correlationId = context.Request.Headers
        .TryGetValue("X-Correlation-Id", out var id)
        ? id.ToString()
        : Guid.NewGuid().ToString();
    
    context.Items["CorrelationId"] = correlationId;
    context.Response.Headers.Add("X-Correlation-Id", correlationId);
    
    return next();
});
```

### Configuration:
```json
{
  "Jaeger": {
    "AgentHost": "localhost",
    "AgentPort": "6831"
  },
  "OpenTelemetry": {
    "SamplingRate": 0.1
  }
}
```

### Testing:
- Send request and check Jaeger UI
- Verify trace contains all services
- Check correlation ID propagation

### Docker Compose for Jaeger:
```yaml
jaeger:
  image: jaegertracing/all-in-one
  ports:
    - "6831:6831/udp"
    - "16686:16686"
```

---

## P2.4: Advanced Audit Features

**Severity**: MEDIUM  
**Risk**: Audit logs not queryable, no analytics  
**Business Impact**: Compliance issues, slow investigation  
**Implementation Effort**: 2-3 hours

### Current State:
- ✅ Audit interceptor (P0.4)
- ❌ No dedicated audit table
- ❌ No audit query API
- ❌ Limited audit analytics

### Target State:
- ✅ Dedicated AuditLog table/collection
- ✅ Audit query endpoints
- ✅ Audit dashboard
- ✅ Advanced filtering/searching

### Entity Design:

```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public string EntityName { get; set; }
    public Guid EntityId { get; set; }
    public string Action { get; set; } // Create, Update, Delete
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Changes { get; set; } // Old vs New
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}
```

### Implementation:
1. Create AuditLog entity
2. Write database migration
3. Update AuditInterceptor to save to table
4. Create AuditLog repository
5. Create query endpoints
6. Add filtering/sorting

### API Endpoints:
```csharp
[HttpGet("/audit/logs")]
public async Task<PaginatedResponse<AuditLogDto>> GetAuditLogs(
    [FromQuery] string? entityName,
    [FromQuery] string? action,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
{
    // Return paginated audit logs with filtering
}

[HttpGet("/audit/logs/{id}")]
public async Task<AuditLogDto> GetAuditDetail(Guid id)
{
    // Return specific audit entry with full change history
}
```

### Query Examples:
```csharp
// Find all user modifications
var logs = await context.AuditLogs
    .Where(l => l.EntityName == "AppUser" && l.UserId == userId)
    .OrderByDescending(l => l.Timestamp)
    .ToListAsync();

// Find recent deletions
var deletions = await context.AuditLogs
    .Where(l => l.Action == "Delete")
    .Where(l => l.Timestamp > DateTime.UtcNow.AddDays(-30))
    .ToListAsync();
```

### Testing:
- Create entity → verify audit log created
- Update entity → verify changes recorded
- Delete entity → verify soft delete tracked
- Query endpoints → verify filtering works

---

## P2.5: Cache Security & Invalidation

**Severity**: MEDIUM  
**Risk**: Sensitive data in cache, cache poisoning  
**Business Impact**: Data leakage, stale/incorrect data  
**Implementation Effort**: 1-2 hours

### Current State:
- ❌ No distributed cache
- ❌ No cache security
- ❌ No cache invalidation strategy
- ❌ No cache key patterns

### Target State:
- ✅ Redis with AUTH & TLS
- ✅ Encrypted cache values (sensitive data)
- ✅ Automatic cache invalidation
- ✅ Cache key naming patterns

### Implementation:

**Step 1: Install Redis Packages**
```bash
dotnet add package StackExchange.Redis
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

**Step 2: Configure Redis**
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.ConfigurationOptions.DefaultDatabase = 0;
    options.ConfigurationOptions.SSL = true; // Production
    options.ConfigurationOptions.Password = builder.Configuration["Redis:Password"];
});
```

**Step 3: Cache Key Patterns**
```csharp
public static class CacheKeys
{
    public static string Product(Guid id) => $"product:{id:N}";
    public static string Products(Guid categoryId) => $"products:category:{categoryId:N}";
    public static string User(string userId) => $"user:{userId}";
    public static string UserProfile(string userId) => $"user:profile:{userId}";
}
```

**Step 4: Cache Service**
```csharp
public class CacheService
{
    private readonly IDistributedCache _cache;
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json == null ? null : JsonSerializer.Deserialize<T>(json);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions 
        { 
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1) 
        });
    }
    
    public async Task InvalidateAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
```

**Step 5: Sensitive Data Encryption**
```csharp
public async Task SetSecureAsync<T>(string key, T value, IEncryptionService encryption)
{
    var json = JsonSerializer.Serialize(value);
    var encrypted = encryption.Encrypt(json);
    await _cache.SetStringAsync(key, encrypted);
}

public async Task<T?> GetSecureAsync<T>(string key, IEncryptionService encryption)
{
    var encrypted = await _cache.GetStringAsync(key);
    if (encrypted == null) return null;
    
    var json = encryption.Decrypt(encrypted);
    return JsonSerializer.Deserialize<T>(json);
}
```

### TTL Strategy:
- **User Data**: 5 minutes (sensitive)
- **Product Catalog**: 1 hour (static)
- **Search Results**: 15 minutes (frequently changing)
- **Configuration**: 24 hours (rarely changes)

### Cache Invalidation:
```csharp
// On product update
await _cacheService.InvalidateAsync(CacheKeys.Product(id));
await _cacheService.InvalidateAsync(CacheKeys.Products(categoryId));

// On user update
await _cacheService.InvalidateAsync(CacheKeys.User(userId));
await _cacheService.InvalidateAsync(CacheKeys.UserProfile(userId));
```

### Configuration:
```json
{
  "ConnectionStrings": {
    "Redis": "<configure-via-environment-variable-or-key-vault>"
    // Example format: redis://localhost:6379,password=<secure-random-password>,ssl=true
    // ⚠️ IMPORTANT: NEVER hardcode passwords! Use environment variables or Azure Key Vault
  },
  "CacheSettings": {
    "DefaultTTL": "01:00:00",
    "SensitiveDataTTL": "00:05:00"
  }
}
```

### Testing:
- Set cache value → verify retrieval
- Set encrypted value → verify decryption
- Invalidate cache → verify removal
- Verify TTL expiration
- Test cache miss handling

### Redis Security Commands:
```bash
# Require authentication
CONFIG SET requirepass "secure-password"

# Disable dangerous commands
CONFIG SET rename-commands '{"FLUSHDB":"","FLUSHALL":"","CONFIG":""}'

# Enable TLS
server.crt and server.key files required
```

---

## Implementation Priority Matrix

| Issue | Impact | Effort | Priority |
|-------|--------|--------|----------|
| P2.1 - TDE | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | 🔴 CRITICAL |
| P2.2 - Versioning | ⭐⭐⭐ | ⭐⭐ | 🟠 HIGH |
| P2.3 - Tracing | ⭐⭐⭐ | ⭐⭐⭐ | 🟠 HIGH |
| P2.4 - Audit API | ⭐⭐ | ⭐⭐⭐ | 🟡 MEDIUM |
| P2.5 - Cache | ⭐⭐ | ⭐⭐ | 🟡 MEDIUM |

---

## Recommended Implementation Order

### This Week:
1. ✅ **P2.1 - TDE** (Critical security)
2. ✅ **P2.2 - Versioning** (Prevents breaking changes)

### Next Week:
3. ✅ **P2.3 - Tracing** (Operational visibility)
4. ✅ **P2.4 - Audit API** (Compliance)

### Week After:
5. ✅ **P2.5 - Cache** (Performance optimization)

---

## Total Effort Estimate

| Phase | Duration | Output |
|-------|----------|--------|
| **P2.1 (TDE)** | 2-3 hours | 1 SQL script, docs |
| **P2.2 (Versioning)** | 1-2 hours | API changes, docs |
| **P2.3 (Tracing)** | 2-3 hours | Code changes, Jaeger setup |
| **P2.4 (Audit)** | 2-3 hours | Entity, API, migrations |
| **P2.5 (Cache)** | 1-2 hours | Service, configuration |
| **TOTAL** | **8-13 hours** | **Complete P2** |

---

## Success Criteria

### P2.1 ✅
- [ ] Database encrypted with TDE
- [ ] Encryption keys backed up
- [ ] No performance degradation
- [ ] Monitoring configured

### P2.2 ✅
- [ ] Two API versions running
- [ ] Deprecation headers in responses
- [ ] Documentation updated
- [ ] No breaking changes

### P2.3 ✅
- [ ] Traces visible in Jaeger
- [ ] Correlation IDs propagated
- [ ] All services instrumented
- [ ] Performance acceptable

### P2.4 ✅
- [ ] Audit logs queryable
- [ ] Query API working
- [ ] Filtering/sorting implemented
- [ ] Performance acceptable

### P2.5 ✅
- [ ] Cache reduces DB load
- [ ] Sensitive data encrypted
- [ ] TTL working correctly
- [ ] Invalidation working

---

## Next Action

**Option 1**: Start P2 implementation immediately  
**Option 2**: Conduct code review of P0/P1 first, then P2  
**Option 3**: Deploy P0/P1 to staging, parallel P2 implementation

**Recommendation**: Option 2 (Code review + staging test, then P2)

