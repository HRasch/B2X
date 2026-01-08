---
docid: REQ-039
title: ANALYSIS PERSISTED TEST ENV SECURITY
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿---
docid: ANALYSIS-PERSISTED-TEST-ENV-SECURITY
title: Persisted Test Environment - Security Analysis
owner: @Security
status: Complete
created: 2026-01-07
related: REQ-PERSISTED-TEST-ENVIRONMENT.md
---

# Security Analysis: Persisted Test Environment

**Analyst**: @Security  
**Date**: 2026-01-07  
**Related Requirement**: [REQ-PERSISTED-TEST-ENVIRONMENT.md](REQ-PERSISTED-TEST-ENVIRONMENT.md)

---

## Executive Summary

The Persisted Test Environment feature requires **careful security controls** to prevent test data leaks, unauthorized access, and production contamination. 

**Key Findings**:
- ✅ Multi-tenant isolation framework supports test isolation
- ⚠️ Test endpoints must be production-only disabled
- ⚠️ Test data must be clearly marked and segregated
- ⚠️ Seed data must not contain sensitive information
- ✅ Existing authentication can enforce access controls

**Risk Level**: **MEDIUM** (manageable with proper controls)

**Recommendation**: Implement **four-layer security approach**: Environment-based gating, role-based access control (RBAC), data marking, and strict separation of concerns.

---

## Threat Analysis

### 1. Production Contamination

**Threat**: Test endpoints accidentally called in production, creating fake test tenants/data

**Probability**: Medium  
**Impact**: High - Corrupts production environment

**Mitigation**:
- [x] Environment-based endpoint gating (test mode only)
- [x] Compile-time endpoint removal in production
- [x] Configuration validation on startup
- [x] Separate test controllers with `[EnvironmentRestriction("Testing")]`
- [x] Logging of all test operations

**Implementation**:
```csharp
[ApiController]
[Route("api/admin")]
public class TestTenantController : ControllerBase
{
    // Only compiled/active when environment != "Production"
    [Conditional("DEBUG_TESTS")]
    public TestTenantController() { }

    [HttpPost("test-tenants")]
    [Environment("Testing", "Development")] // Custom attribute
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> CreateTestTenant(...)
    {
        // Implementation
    }
}
```

**Custom Attribute**:
```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EnvironmentRestrictionAttribute : Attribute
{
    private readonly string[] _allowedEnvironments;

    public EnvironmentRestrictionAttribute(params string[] allowedEnvironments)
    {
        _allowedEnvironments = allowedEnvironments;
    }
}

public class EnvironmentRestrictionFilter : IAsyncActionFilter
{
    private readonly IHostEnvironment _hostEnvironment;

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var restriction = endpoint?.Metadata.GetMetadata<EnvironmentRestrictionAttribute>();

        if (restriction != null && !restriction.AllowedEnvironments.Contains(_hostEnvironment.EnvironmentName))
        {
            context.Result = new NotFoundResult();
            return;
        }

        await next();
    }
}
```

### 2. Unauthorized Access to Test Tenants

**Threat**: Non-admin users creating, modifying, or accessing test tenants

**Probability**: High  
**Impact**: Medium - Unauthorized data access, test manipulation

**Mitigation**:
- [x] Enforce SuperAdmin role requirement
- [x] Multi-factor authentication (MFA) for admin operations
- [x] API key validation for programmatic access
- [x] Audit logging of all operations
- [x] Rate limiting on tenant creation

**Implementation**:
```csharp
[HttpPost("test-tenants")]
[Authorize(Roles = "SuperAdmin")]
[RequireMfa] // Custom attribute
[RateLimit(Requests = 10, Window = TimeSpan.FromHours(1))]
public async Task<IActionResult> CreateTestTenant(
    [FromBody] CreateTenantRequest request,
    CancellationToken cancellationToken)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var tenantId = User.FindFirst("tenant_id")?.Value;

    // Log operation
    _auditService.LogOperation(
        operationType: "TestTenantCreate",
        userId: userId,
        tenantId: tenantId,
        details: new { request.Name, request.StorageMode }
    );

    // Proceed with creation
    return Ok(await _tenantService.CreateTestTenantAsync(request, cancellationToken));
}
```

### 3. Test Data Leakage to Production

**Threat**: Sensitive test data (PII, credentials) copied to or visible in production

**Probability**: Medium  
**Impact**: High - Data breach

**Mitigation**:
- [x] Explicit test data flagging
- [x] Exclusion from production backups
- [x] Separate test databases
- [x] No real PII in seed data
- [x] Data retention policies
- [x] Regular audit trails

**Implementation**:

**Entity Flagging**:
```csharp
public abstract class Entity
{
    public bool IsTestData { get; set; }
    public bool IsSensitive { get; set; }
}

public class User : Entity
{
    public string Email { get; set; }
    public string Name { get; set; }
    // IsTestData = true for all test users
}
```

**Backup Exclusion**:
```csharp
public class BackupService
{
    public async Task BackupProductionDataAsync()
    {
        // Exclude test data from backups
        var dataToBackup = await _context.Users
            .Where(u => !u.IsTestData) // Only production data
            .Where(u => !u.IsSensitive)
            .ToListAsync();

        // Perform backup
    }
}
```

**Seed Data Guidelines**:
```json
// test-data/auth/users.json - Use ONLY fake data
{
  "users": [
    {
      "email": "test.admin@B2X.local",
      "name": "Test Admin",
      "password": "TestPassword123!",
      "isTestData": true,
      "isSensitive": false
    }
  ]
}

// ❌ NEVER include:
// - Real employee emails
// - Production credentials
// - Real customer data
// - PII (personally identifiable information)
// - Financial data
```

### 4. Tenant Isolation Breach

**Threat**: Test user accessing another tenant's data due to improper isolation

**Probability**: Low  
**Impact**: High - Data breach

**Mitigation**:
- [x] Enforce TenantContext in all queries
- [x] Database-level row security
- [x] Integration tests validating isolation
- [x] Regular security audits
- [x] Multi-tenant query validation

**Implementation**:

**Query Validation**:
```csharp
public class TenantIsolationFilter : IAsyncPageFilter
{
    private readonly ITenantContext _tenantContext;

    public async Task OnPageHandlerExecutionAsync(
        PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next)
    {
        // Verify tenant context is set for all DB operations
        if (_tenantContext.TenantId == null)
        {
            throw new InvalidOperationException("TenantContext not set");
        }

        await next();
    }
}

// All queries MUST include tenant filter
public class CatalogService
{
    public async Task<List<Product>> GetProductsAsync(CancellationToken ct)
    {
        var tenantId = _tenantContext.TenantId;
        
        return await _context.Products
            .Where(p => p.TenantId == tenantId) // REQUIRED
            .ToListAsync(ct);
    }
}
```

**Integration Test**:
```csharp
[TestMethod]
public async Task TestTenantIsolation()
{
    // Create two test tenants
    var tenant1 = await _tenantService.CreateAsync("Tenant1", "persisted");
    var tenant2 = await _tenantService.CreateAsync("Tenant2", "persisted");

    // Create product in tenant1
    var product = await _catalogService.CreateProductAsync(
        "Product",
        tenantId: tenant1.Id
    );

    // Try to access from tenant2 context
    _tenantContext.TenantId = tenant2.Id;
    var result = await _catalogService.GetProductAsync(product.Id);

    // Should NOT find product (different tenant)
    Assert.IsNull(result);

    // Restore tenant1 context
    _tenantContext.TenantId = tenant1.Id;
    result = await _catalogService.GetProductAsync(product.Id);

    // Should find product (same tenant)
    Assert.IsNotNull(result);
}
```

### 5. API Key/Credential Exposure

**Threat**: Test environment credentials hardcoded in code or exposed in version control

**Probability**: Medium  
**Impact**: High - System compromise

**Mitigation**:
- [x] Use environment variables exclusively
- [x] Secret management service (e.g., Azure Key Vault)
- [x] No credentials in config files
- [x] Pre-commit hooks scanning
- [x] Regular credential rotation

**Implementation**:
```csharp
// ❌ WRONG - Credentials in code
public const string TestApiKey = "secret-key-12345";

// ✅ CORRECT - Environment variables
var testApiKey = builder.Configuration["Testing:ApiKey"]
    ?? Environment.GetEnvironmentVariable("TESTING_API_KEY");

// ✅ BETTER - Secret management
var client = new SecretClient(
    vaultUri: new Uri("https://B2X-kv.vault.azure.net/"),
    credential: new DefaultAzureCredential()
);

var secret = await client.GetSecretAsync("test-api-key");
var testApiKey = secret.Value.Value;
```

**Pre-commit Hook** (.husky/pre-commit):
```bash
#!/bin/sh

# Scan for hardcoded secrets
git diff --cached | grep -E '(password|secret|key|token)\s*[:=]' && {
    echo "❌ ERROR: Possible hardcoded credentials detected"
    exit 1
}

exit 0
```

---

## Access Control Framework

### Role-Based Access Control (RBAC)

**Roles for Test Operations**:

| Role | Permissions | Requirement |
|------|-------------|-------------|
| SuperAdmin | All test operations | MFA required |
| TenantAdmin | View own tenant, create tenants | MFA required |
| Developer | View test tenants (read-only) | API key |
| DevOps | Manage test infrastructure | MFA + IP whitelist |

**Implementation**:
```csharp
public enum TestOperationRole
{
    SuperAdmin,        // Create/delete tenants
    TenantAdmin,       // Manage own tenant
    Developer,         // Read-only access
    DevOps             // Infrastructure access
}

[Authorize(Roles = "SuperAdmin")]
[HttpPost("test-tenants")]
public async Task<IActionResult> CreateTestTenant(...) { }

[Authorize(Roles = "SuperAdmin,TenantAdmin")]
[HttpGet("test-tenants/{id}")]
public async Task<IActionResult> GetTenantDetails(string id) { }

[Authorize]
[HttpGet("test-tenants")]
public async Task<IActionResult> ListTestTenants() { } // Read-only for all
```

### API Key Management

**For CI/CD & Automated Tests**:
```csharp
public class ApiKeyValidator : IAsyncAuthorizationFilter
{
    private readonly IApiKeyService _apiKeyService;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers["X-API-Key"].ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var key = await _apiKeyService.ValidateAsync(apiKey);
        if (key == null || !key.IsValid || !key.HasTestAccess)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["ApiKey"] = key;
    }
}
```

---

## Data Marking & Segregation

### Test Data Identification

**Properties Every Entity Must Have**:
```csharp
public abstract class AuditableEntity
{
    public bool IsTestData { get; set; }
    public bool IsSensitive { get; set; }
    public string DataProfile { get; set; } // "basic", "full", "custom"
    public string CreatedByEnvironment { get; set; } // "testing", "development"
    public DateTime CreatedAt { get; set; }
}
```

**Flagging Convention**:
- ✅ All entities created in test environment must have `IsTestData = true`
- ✅ Seed data must have `DataProfile` set
- ✅ Must track `CreatedByEnvironment` for audit trail

### Backup & Recovery

**Backup Policy**:
```
Production Backup:
├── Exclude: IsTestData = true
├── Exclude: IsSensitive = true
├── Include: All production data
└── Encryption: AES-256

Test Backup:
├── Include: All test data
├── Retention: 7 days only
├── Encryption: Standard
└── Access: Admin only
```

---

## Audit Logging

### Required Logging

**All test operations must log**:
```csharp
public class AuditLog
{
    public DateTime Timestamp { get; set; }
    public string OperationType { get; set; } // "TestTenantCreate", "TestTenantDelete"
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public string Environment { get; set; }
    public Dictionary<string, object> Details { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
}
```

**Logged Operations**:
- ✅ Test tenant created
- ✅ Test tenant deleted
- ✅ Test tenant modified
- ✅ Test data reset
- ✅ Unauthorized access attempts
- ✅ Bulk operations
- ✅ Configuration changes

**Audit Log Retention**: 90 days minimum

---

## Environment-Based Gating

### Compile-Time Exclusion

**appsettings.Production.json**:
```json
{
  "Testing": {
    "Enabled": false
  }
}
```

**Startup Validation**:
```csharp
var testingEnabled = builder.Configuration.GetValue<bool>("Testing:Enabled", false);

if (app.Environment.IsProduction() && testingEnabled)
{
    throw new InvalidOperationException(
        "❌ CRITICAL: Testing endpoints enabled in production!");
}

if (!app.Environment.IsProduction() && !testingEnabled)
{
    logger.LogWarning("⚠️ Testing endpoints disabled in non-production environment");
}
```

### Runtime Validation

**Middleware Validation**:
```csharp
public class TestingOnlyMiddleware
{
    private readonly RequestDelegate _next;

    public TestingOnlyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IHostEnvironment env)
    {
        if (context.Request.Path.StartsWithSegments("/api/admin/test-tenants"))
        {
            if (env.IsProduction())
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        await _next(context);
    }
}

// In Program.cs
app.UseMiddleware<TestingOnlyMiddleware>();
```

---

## Compliance & Standards

### Data Protection Regulations

**GDPR Compliance**:
- [ ] Test data must not include real personal data
- [ ] Test tenants must be marked as "non-production"
- [ ] Test data must be deleted after retention period
- [ ] Right to erasure tested with test tenants only

**Data Retention Policy**:
```
Test Data Retention:
├── Temporary (In-Memory): 0 days (session only)
├── Persisted (PostgreSQL): 30 days max
├── Archived: Keep 1 backup for 7 days
└── Deletion: Automatic after retention period
```

### Security Standards

**OWASP Top Ten Controls**:
1. ✅ Broken Access Control - RBAC + endpoint gating
2. ✅ Cryptographic Failures - Encrypted storage + TLS
3. ✅ Injection - Parameterized queries (EF Core)
4. ✅ Insecure Design - Multi-tenant isolation
5. ✅ Security Misconfiguration - Environment gating
6. ✅ Vulnerable Components - Regular updates
7. ✅ Auth Failures - MFA enforcement
8. ✅ Data Integrity Failures - Audit logging
9. ✅ Logging Failures - Comprehensive audit logs
10. ✅ SSRF - Input validation

---

## Implementation Checklist

### Phase 1: Gating & Access Control
- [ ] Create `EnvironmentRestrictionAttribute`
- [ ] Apply to all test endpoints
- [ ] Add startup validation
- [ ] Implement RBAC authorization

**Effort**: 1 day

### Phase 2: Data Protection
- [ ] Add `IsTestData` and `IsSensitive` to entities
- [ ] Create test data seed files (no real data)
- [ ] Implement backup exclusion logic
- [ ] Add data retention policies

**Effort**: 1.5 days

### Phase 3: Audit & Monitoring
- [ ] Create `AuditLog` entity
- [ ] Log all test operations
- [ ] Create audit log views
- [ ] Set up alerting for suspicious activity

**Effort**: 1 day

### Phase 4: Testing & Validation
- [ ] Write integration tests for tenant isolation
- [ ] Test unauthorized access scenarios
- [ ] Validate production build excludes test code
- [ ] Security review and pen testing

**Effort**: 1.5 days

---

## Security Review Checklist

- [ ] All test endpoints protected by `[EnvironmentRestriction]`
- [ ] Startup validation fails if testing enabled in production
- [ ] All test operations logged to audit trail
- [ ] Test data marked with `IsTestData = true`
- [ ] No real PII in seed data
- [ ] Backup exclusion working for test data
- [ ] Tenant isolation verified in tests
- [ ] MFA enforced for admin operations
- [ ] API keys properly rotated
- [ ] No credentials in version control
- [ ] OWASP compliance verified
- [ ] Rate limiting on sensitive endpoints

---

## Risks & Mitigation

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Test code in production | Medium | Critical | Compile-time exclusion, validation |
| Test data leak | Medium | High | Explicit flagging, backup exclusion |
| Tenant isolation breach | Low | High | Integration tests, query validation |
| Unauthorized creation | Medium | Medium | RBAC, MFA, audit logging |
| Credential exposure | Medium | High | Environment variables, secret mgmt |
| Compliance violations | Low | High | Data retention, audit logs |

---

## Next Steps

1. @Backend: Implement environment gating and data protection
2. @Frontend: Add MFA enforcement for admin operations
3. @Architect: Review service boundary security impacts
4. @SARAH: Consolidate analyses into unified spec

---

**Status**: ✅ Analysis Complete  
**Risk Level**: MEDIUM (manageable with controls)  
**Recommendation**: Implement all security controls before production deployment
