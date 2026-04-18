# Tenant Isolation in B2X

## Overview
B2X implements multi-layered tenant isolation to ensure data security and privacy in a multitenant SaaS environment.

## Isolation Strategies

### 1. Database-Level Isolation

#### Row-Level Security (RLS) - Recommended
PostgreSQL Row-Level Security provides database-enforced tenant isolation.

**Advantages**:
- Enforced at database level, independent of application code
- Prevents accidental data leaks
- Minimal performance overhead
- Native PostgreSQL feature

**Implementation**:
```sql
-- Create table with tenant_id
CREATE TABLE users (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL,
  email VARCHAR(255) NOT NULL,
  first_name VARCHAR(100),
  last_name VARCHAR(100),
  created_at TIMESTAMP,
  FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

-- Create index for performance
CREATE INDEX idx_users_tenant_id ON users(tenant_id);

-- Enable RLS
ALTER TABLE users ENABLE ROW LEVEL SECURITY;

-- Create isolation policy
CREATE POLICY users_tenant_isolation ON users
  USING (tenant_id = current_setting('app.current_tenant_id')::uuid)
  WITH CHECK (tenant_id = current_setting('app.current_tenant_id')::uuid);

-- Grant permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON users TO app_user;
```

**Setting Tenant Context in Application**:
```csharp
// In middleware or repository
await connection.ExecuteAsync(
    "SELECT set_config('app.current_tenant_id', $1, false)",
    tenantId.ToString()
);
```

#### Separate Schemas
Alternative approach using separate schemas per tenant.

**Advantages**:
- Complete schema isolation
- Easier data export/backup per tenant
- Schema-level performance tuning

**Disadvantages**:
- More complex to manage
- Higher resource usage
- Requires dynamic connection strings or schema switching

**Implementation**:
```csharp
public interface ISchemaNameProvider
{
    string GetSchemaName(Guid tenantId);
}

public class TenantSchemaNameProvider : ISchemaNameProvider
{
    public string GetSchemaName(Guid tenantId)
    {
        return $"tenant_{tenantId:N}"; // tenant_[uuid without dashes]
    }
}

// Usage in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var tenantId = _tenantContextAccessor.GetTenantId();
    var schema = _schemaNameProvider.GetSchemaName(tenantId);
    
    modelBuilder.HasDefaultSchema(schema);
}
```

### 2. Application-Level Isolation

#### Tenant Context Middleware
Every request must establish tenant context before accessing data.

```csharp
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContextAccessor tenantContextAccessor)
    {
        var tenantId = ExtractTenantId(context);
        
        if (tenantId == Guid.Empty)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Tenant ID not found");
            return;
        }

        tenantContextAccessor.SetTenantId(tenantId);
        await _next(context);
    }

    private Guid ExtractTenantId(HttpContext context)
    {
        // From JWT claims
        var claim = context.User.FindFirst("tenant_id");
        if (claim != null && Guid.TryParse(claim.Value, out var id))
            return id;

        // From header (fallback)
        if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var header))
            if (Guid.TryParse(header.ToString(), out var id))
                return id;

        return Guid.Empty;
    }
}
```

#### Repository Pattern with Tenant Context
```csharp
public class TenantAwareRepository<T> : IRepository<T> where T : Entity
{
    private readonly DbContext _context;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public TenantAwareRepository(DbContext context, ITenantContextAccessor tenantContextAccessor)
    {
        _context = context;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var tenantId = _tenantContextAccessor.GetTenantId();
        
        return await _context.Set<T>()
            .Where(x => x.TenantId == tenantId && x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var tenantId = _tenantContextAccessor.GetTenantId();
        
        return await _context.Set<T>()
            .Where(x => x.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        entity.TenantId = _tenantContextAccessor.GetTenantId();
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
    }
}
```

### 3. API-Level Isolation

#### Request Validation
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
    {
        var tenantId = _tenantContextAccessor.GetTenantId();
        
        // Verify tenant ownership
        if (id != tenantId)
        {
            return Forbid("Access denied to this tenant");
        }

        var tenant = await _tenantService.GetByIdAsync(id);
        return Ok(tenant);
    }
}
```

### 4. JWT Token-Based Isolation

#### Token Claims Structure
```json
{
  "sub": "user-uuid",
  "email": "user@example.com",
  "tenant_id": "tenant-uuid",
  "roles": ["user", "admin"],
  "permissions": ["read:documents", "write:documents"],
  "iat": 1234567890,
  "exp": 1234571490
}
```

#### Token Generation
```csharp
public class TokenService
{
    private readonly IConfiguration _configuration;

    public string GenerateToken(User user, Tenant tenant)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("tenant_id", tenant.Id.ToString()),
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

## Testing Tenant Isolation

### Unit Tests
```csharp
[TestClass]
public class TenantIsolationTests
{
    private ITenantContextAccessor _tenantContextAccessor;
    private DbContext _context;

    [TestMethod]
    public async Task GetUser_ReturnOnlyCurrentTenantUsers()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid();
        var tenantId2 = Guid.NewGuid();
        
        var user1 = new User { Id = Guid.NewGuid(), TenantId = tenantId1, Email = "user1@t1.com" };
        var user2 = new User { Id = Guid.NewGuid(), TenantId = tenantId2, Email = "user2@t2.com" };

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        _tenantContextAccessor.SetTenantId(tenantId1);
        var users = await _context.Users.ToListAsync();

        // Assert
        Assert.AreEqual(1, users.Count);
        Assert.AreEqual(user1.Id, users.First().Id);
    }

    [TestMethod]
    public async Task InsertUser_EnforcesCurrentTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "user@example.com",
            // Note: TenantId intentionally not set to current tenant
            TenantId = Guid.NewGuid()
        };

        _tenantContextAccessor.SetTenantId(tenantId);

        // Act & Assert
        // RLS policy should prevent insertion of record with mismatched tenant_id
        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        });
    }
}
```

### Integration Tests
```csharp
[TestClass]
public class TenantIsolationIntegrationTests
{
    [TestMethod]
    public async Task MultipleRequests_DoNotLeakDataBetweenTenants()
    {
        // Simulate two concurrent requests from different tenants
        var tenant1Id = Guid.NewGuid();
        var tenant2Id = Guid.NewGuid();

        var task1 = SimulateRequest(tenant1Id);
        var task2 = SimulateRequest(tenant2Id);

        var results = await Task.WhenAll(task1, task2);

        // Verify no data cross-contamination
        Assert.AreEqual(tenant1Id, results[0].TenantId);
        Assert.AreEqual(tenant2Id, results[1].TenantId);
    }
}
```

## Monitoring Isolation Breaches

### Audit Logging
```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
    public string Changes { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
}

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuditService _auditService;

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = context.Items["TenantId"] as Guid?;
        var userId = context.User.GetUserId();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        await _next(context);

        if (IsAuditableRequest(context.Request.Method, context.Request.Path))
        {
            await _auditService.LogAsync(new AuditLog
            {
                TenantId = tenantId.Value,
                UserId = userId,
                Action = context.Request.Method,
                Resource = context.Request.Path,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress
            });
        }
    }
}
```

## Best Practices

1. **Always validate tenant context** before accessing data
2. **Use RLS** as primary isolation mechanism
3. **Implement authorization** in addition to authentication
4. **Log access attempts** to sensitive data
5. **Test isolation regularly** with automated tests
6. **Monitor for isolation breaches** with alerting
7. **Use parameterized queries** to prevent SQL injection
8. **Encrypt sensitive data** at rest and in transit
9. **Implement rate limiting** per tenant
10. **Regular security audits** of isolation implementation
