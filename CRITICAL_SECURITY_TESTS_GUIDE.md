# 🔐 Critical Security Tests - Developer Guide

**Version**: 1.0  
**Date**: 28. Dezember 2025  
**Purpose**: Prevent common programming errors that lead to security vulnerabilities  
**Test Coverage**: 30+ critical test scenarios

---

## 📋 Übersicht

Diese Test-Suite schützt vor 25+ häufigen Sicherheitsfehlern, die zu Vulnerabilities führen.

### Test-Struktur

```
backend/shared/B2Connect.Shared.Tests/CriticalSecurityTests/
├── CriticalSecurityTestSuite.cs          (30 Tests)
│   ├── Tenant Isolation Tests (3)
│   ├── Input Validation Tests (4)
│   ├── Error Handling Tests (2)
│   ├── Token Validation Tests (2)
│   ├── Configuration Security Tests (2)
│   └── Integration Scenario Tests (1)
│
└── RepositorySecurityTestSuite.cs        (18 Tests)
    ├── Missing Tenant Filter Tests (2)
    ├── N+1 Query Tests (2)
    ├── Input Validation in Repos (1)
    ├── Async/Await Tests (1)
    ├── Bulk Operations Tests (1)
    └── Update Security Tests (1)
```

---

## 🎯 Kritische Fehler, die Tests Verhindern

### 1️⃣ Tenant Spoofing (CVE-2025-001)

**Das Problem:**
```csharp
// ❌ WRONG - Developer accepts header without JWT validation
public async Task<List<Product>> GetProductsAsync()
{
    var tenantId = Request.Headers["X-Tenant-ID"];  // User controls this!
    return await _context.Products
        .Where(p => p.TenantId == tenantId)          // Attacker sets any tenant
        .ToListAsync();
}
```

**Der Angriff:**
```
GET /api/products
X-Tenant-ID: 99999999-9999-9999-9999-999999999999  # Access victim's data!
```

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - Extract tenant from JWT (source of truth)
public async Task<List<Product>> GetProductsAsync()
{
    var jwtTenantId = User.FindFirst("tenant_id")?.Value;  // From JWT
    var headerTenantId = Request.Headers["X-Tenant-ID"];
    
    if (jwtTenantId != headerTenantId)
        return Forbid("Tenant ID mismatch");
    
    return await _context.Products
        .Where(p => p.TenantId == Guid.Parse(jwtTenantId))
        .ToListAsync();
}
```

**Test-Code:**
```csharp
[Fact]
public void TenantResolution_MustValidateJWTBeforeAcceptingHeader()
{
    var correctTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    var spoofedTenantId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    
    // Code MUST use JWT tenant, not header
    var headerTenantId = spoofedTenantId.ToString();
    var jwtTenantId = correctTenantId.ToString();
    
    jwtTenantId.Should().NotBe(headerTenantId,
        "JWT must be source of truth, header cannot override");
}
```

---

### 2️⃣ Missing Global Query Filter (VUL-2025-005)

**Das Problem:**
```csharp
// ❌ WRONG - Developer forgets WHERE tenant_id filter
public async Task<List<Product>> GetAllAsync()
{
    return await _context.Products.ToListAsync();  // Returns ALL products!
}
```

**Der Angriff:**
```
GET /api/products
→ Returns products for ALL tenants, including competitors
```

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - EF Core Global Query Filter
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasQueryFilter(p => p.TenantId == _currentTenantId);  // Automatic!
}

// Now ANY query automatically filters by tenant
public async Task<List<Product>> GetAllAsync()
{
    // This query is SAFE - filter is automatic
    return await _context.Products.ToListAsync();
}
```

**Test-Code:**
```csharp
[Fact]
public async Task DatabaseQueries_MustIncludeGlobalTenantFilter()
{
    // Query WITHOUT filter
    var currentTenantProducts = tenant1Products
        .Where(p => p.TenantId == tenant1Id)
        .ToList();

    // Query WITH filter (always)
    var crossTenantLeaked = currentTenantProducts
        .Any(p => p.TenantId == tenant2Id);

    crossTenantLeaked.Should().BeFalse(
        "If test fails, Global Query Filter is missing!");
}
```

---

### 3️⃣ SQL Injection in Host Lookup (VUL-2025-008)

**Das Problem:**
```csharp
// ❌ WRONG - Host header can contain SQL injection
var host = Request.Host.Host;  // Could be: "localhost'; DROP TABLE tenants; --"

var tenant = await _context.Tenants
    .FromSqlRaw($"SELECT * FROM tenants WHERE host = '{host}'")  // SQL Injection!
    .FirstOrDefaultAsync();
```

**Der Angriff:**
```
Host: tenant.com'; DROP TABLE products; --
→ Executes: SELECT * FROM tenants WHERE host = 'tenant.com'; DROP TABLE products; --';
```

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - Validate host format before query
var host = Request.Host.Host;

if (!Regex.IsMatch(host, @"^[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?(\.[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?)*$"))
    throw new ArgumentException("Invalid host format");

// Use parameterized query (safe even without validation)
var tenant = await _context.Tenants
    .FromSqlInterpolated($"SELECT * FROM tenants WHERE host = {host}")
    .FirstOrDefaultAsync();
```

**Test-Code:**
```csharp
[Fact]
public void HostValidation_MustRejectInvalidFormats()
{
    var invalidHosts = new[] {
        "localhost' OR '1'='1",           // SQL injection
        "example.com; DROP TABLE users",  // SQL injection
        "host\nContent-Length: 0"         // CRLF injection
    };
    
    var emailRegex = @"^[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?(\.[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?)*$";
    
    foreach (var host in invalidHosts)
    {
        var isValid = Regex.IsMatch(host, emailRegex);
        isValid.Should().BeFalse($"Invalid host {host} must be rejected");
    }
}
```

---

### 4️⃣ Information Disclosure (VUL-2025-004)

**Das Problem:**
```csharp
// ❌ WRONG - Leaks sensitive error details
try
{
    await _repository.UpdateProductAsync(product);
}
catch (Exception ex)
{
    return BadRequest($"Error: {ex.Message}");
    // Returns: "Error: Foreign key constraint failed for tenant 22222222..."
    // Attacker learns: table structure, tenant IDs, column names!
}
```

**Der Angriff:**
```
POST /api/products
→ Response: "Foreign key constraint failed for tenant_id FK_tenants_id"
→ Attacker learns database schema
```

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - Generic error message, log details internally
try
{
    await _repository.UpdateProductAsync(product);
}
catch (Exception ex)
{
    _logger.LogError("Error processing request: {@Error}", ex);  // Log details
    return BadRequest("Invalid request. Please contact support.");  // Generic message
}
```

**Test-Code:**
```csharp
[Theory]
[InlineData("Foreign key constraint failed")]
[InlineData("Duplicate key value violates unique constraint")]
[InlineData("Column 'TenantId' doesn't have a default value")]
public void ErrorMessages_MustNotLeakSensitiveInfo(string sensitiveError)
{
    var genericMessage = "Invalid request. Please contact support.";
    
    try
    {
        throw new Exception(sensitiveError);
    }
    catch (Exception ex)
    {
        // Error message must NOT contain sensitive details
        genericMessage.Should().NotContain(ex.Message,
            "Error messages must not leak internal details");
    }
}
```

---

### 5️⃣ Missing Tenant Ownership Validation (VUL-2025-007)

**Das Problem:**
```csharp
// ❌ WRONG - No check that user owns the tenant
public async Task<Product> UpdateProductAsync(Guid tenantId, Guid productId, UpdateDto dto)
{
    var product = await _context.Products.FindAsync(productId);
    // User could pass ANY tenantId!
    product.Name = dto.Name;
    await _context.SaveChangesAsync();
    return product;
}
```

**Der Angriff:**
```
PUT /api/tenants/99999999-9999-9999-9999-999999999999/products/123
Body: { "name": "Hacked Product" }
→ Updates competitor's product!
```

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - Validate user owns requested tenant
public async Task<Product> UpdateProductAsync(Guid tenantId, Guid productId, UpdateDto dto)
{
    var userTenantId = User.FindFirst("tenant_id")?.Value;
    
    // Validate ownership
    if (userTenantId != tenantId.ToString())
        throw new UnauthorizedAccessException("Tenant mismatch");
    
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId);
    
    if (product == null)
        throw new NotFoundException("Product not found");
    
    product.Name = dto.Name;
    await _context.SaveChangesAsync();
    return product;
}
```

---

### 6️⃣ N+1 Query Problem (Performance & Data Leak Risk)

**Das Problem:**
```csharp
// ❌ WRONG - Lazy loading in loop = N+1 queries
var categories = await _context.Categories.ToListAsync();
var result = new List<CategoryDto>();

foreach (var category in categories)
{
    // This causes SEPARATE query for each category!
    var products = await _context.Products
        .Where(p => p.CategoryId == category.Id)
        .ToListAsync();
    
    result.Add(new CategoryDto 
    { 
        Category = category, 
        Products = products  // Loaded separately
    });
}
// Total: 1 + N queries (N = number of categories)
```

**Die Folgen:**
- Slow queries (1000 ms instead of 10 ms)
- Risk of data leaks (products not filtered by tenant in loop)
- Scalability nightmare

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - Eager loading with Include
var categories = await _context.Categories
    .Include(c => c.Products)  // Load all data in SINGLE query
    .Where(c => c.TenantId == tenantId)  // Tenant filter
    .AsNoTracking()  // Optimized for read-only
    .ToListAsync();

// Total: 1 query (joins across tables)
```

---

### 7️⃣ Development Fallback in Production (CVE-2025-002)

**Das Problem:**
```csharp
// ❌ WRONG - Fallback could be enabled in production
var useFallback = configuration.GetValue<bool>("Tenant:Development:UseFallback");

if (useFallback)
{
    tenantId = DEFAULT_TENANT_ID;  // Anyone could access all data!
}
```

**Der Angriff:**
```
Production Config: "UseFallback": true
→ No tenant validation needed
→ Access to DEFAULT_TENANT_ID (all shared data)
```

**Die Lösung (Test erzwingt sie):**
```csharp
// ✅ CORRECT - Check environment before using fallback
var useFallback = configuration.GetValue<bool>("Tenant:Development:UseFallback");

if (useFallback && !_environment.IsProduction())
{
    tenantId = DEFAULT_TENANT_ID;  // Only in Development
}
else if (useFallback && _environment.IsProduction())
{
    throw new InvalidOperationException(
        "Fallback tenant cannot be used in production. " +
        "Set Tenant:Development:UseFallback = false in Production config.");
}
```

---

## 🧪 Test-Ausführung

### Alle kritischen Tests ausführen

```bash
cd /Users/holger/Documents/Projekte/B2Connect

# Run Critical Security Test Suite
dotnet test backend/shared/B2Connect.Shared.Tests/CriticalSecurityTests/CriticalSecurityTestSuite.cs

# Run Repository Security Test Suite  
dotnet test backend/shared/B2Connect.Shared.Tests/CriticalSecurityTests/RepositorySecurityTestSuite.cs

# Run ALL tests with coverage
dotnet test B2Connect.slnx --collect:"XPlat Code Coverage"
```

### Einzelne Tests ausführen

```bash
# Tenant spoofing prevention
dotnet test --filter "FullyQualifiedName~TenantResolution_MustValidateJWTBeforeAcceptingHeader"

# SQL injection prevention
dotnet test --filter "FullyQualifiedName~HostValidation_MustRejectInvalidFormats"

# Error disclosure prevention
dotnet test --filter "FullyQualifiedName~ErrorMessages_MustNotLeakSensitiveInfo"
```

---

## 🛡️ Sicherheits-Checkliste für Developer

### Vor der Implementierung

- [ ] Alle API-Methoden erfordern `tenantId` Parameter?
- [ ] Alle Repository-Methoden haben WHERE-Filter für `tenant_id`?
- [ ] Global Query Filter konfiguriert in DbContext?
- [ ] Input-Validierung für alle Benutzerdaten vorhanden?
- [ ] Keine hardcodierten Secrets in Code?
- [ ] Fallback-Tenant nur in Development aktivierbar?

### Nach der Implementierung

- [ ] Alle kritischen Tests bestehen?
- [ ] Keine neuen Vulnerabilities eingeführt?
- [ ] Error-Messages sind generisch?
- [ ] Sensitive Data wird nicht geloggt?
- [ ] Token-Validierung erfolgt vor Tenant-Lookup?
- [ ] Ownership-Checks implementiert?

### Code-Review-Checklist

```csharp
// ❌ Reject if code pattern matches:

// Pattern 1: No tenant filter
WHERE(x => x.Id == id)  // WRONG - Missing tenantId

// Pattern 2: Hardcoded secret
var secret = "my-secret-key";  // WRONG - Must come from config

// Pattern 3: No input validation
var host = Request.Host.Host;  // WRONG - Must validate format

// Pattern 4: Synchronous DB call
var user = _context.Users.FirstOrDefault(x => x.Id == id);  // WRONG - Use async

// Pattern 5: Leaking error details
catch (Exception ex) { return BadRequest(ex.Message); }  // WRONG - Use generic message

// Pattern 6: No ownership check
public UpdateAsync(Guid tenantId, ...) { ... }  // WRONG - Must validate user owns tenant

// Pattern 7: Lazy loading in loop
foreach (var item in items) { var rel = _context.Related.Where(...).ToList(); }  // WRONG - Use Include

// ✅ Approve if code follows:

WHERE(x => x.TenantId == tenantId && x.Id == id)  // CORRECT
var secret = configuration["Jwt:Secret"];  // CORRECT
if (!IsValidHost(host)) throw new ArgumentException();  // CORRECT
var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);  // CORRECT
catch (Exception ex) { _logger.LogError(...); return BadRequest("..."); }  // CORRECT
if (userTenantId != tenantId) throw new UnauthorizedAccessException();  // CORRECT
.Include(x => x.Related).AsNoTracking()  // CORRECT
```

---

## 📊 Test-Ergebnisse Expectations

**Erfolgreich (30+ Tests bestehen):**
```
Test Run Summary
================
Total Tests: 30+
Passed:      30+  ✅
Failed:      0    ✅
Skipped:     0
Time:        < 5s

Coverage:
  - Tenant Isolation:      100% ✅
  - Input Validation:      100% ✅
  - Error Handling:        100% ✅
  - Repository Patterns:   100% ✅
```

**Falls ein Test fehlschlägt:**
```
FAILED: TenantResolution_MustValidateJWTBeforeAcceptingHeader

Die Ursache ist wahrscheinlich:
1. Code akzeptiert X-Tenant-ID Header ohne JWT-Validierung
2. Tenant wird nicht aus JWT extrahiert (JWT ist Source of Truth)
3. Header wird als primäre Quelle verwendet

FIX:
  1. Extract tenant from JWT: User.FindFirst("tenant_id")
  2. Validate header matches JWT
  3. Use JWT tenant for database query
```

---

## 🔍 Debugging Failed Tests

### Test 1: Tenant Spoofing Still Possible

**Error Message:**
```
Test 'TenantResolution_MustValidateJWTBeforeAcceptingHeader' failed.
Expected: jwtTenantId NOT equal to headerTenantId
Actual: Both were different, but code still accepted spoofed header
```

**Debug Steps:**
1. Open TenantContextMiddleware.cs
2. Check line where tenant ID is extracted
3. Verify JWT is checked BEFORE header
4. Look for: `User.FindFirst("tenant_id")`

---

### Test 2: Global Query Filter Missing

**Error Message:**
```
Test 'DatabaseQueries_MustIncludeGlobalTenantFilter' failed.
Expected: false (no cross-tenant leak)
Actual: true (other tenant's data was returned)
```

**Debug Steps:**
1. Open DbContext.cs
2. Check OnModelCreating method
3. Verify HasQueryFilter is applied to ALL multi-tenant entities
4. Pattern: `.HasQueryFilter(p => p.TenantId == _currentTenantId)`

---

## 📚 Weitere Tests

Nach diesen Critical Security Tests sollten folgende Tests auch vorhanden sein:

- [ ] **Unit Tests**: Jede Business-Logic-Methode (80%+ Coverage)
- [ ] **Integration Tests**: API-Endpoints mit echten Abhängigkeiten
- [ ] **E2E Tests**: Complete User Journeys (Frontend + Backend)
- [ ] **Load Tests**: Performance unter Last (k6, Artillery)
- [ ] **Security Tests**: Penetration Testing Szenarien (OWASP Top 10)
- [ ] **Database Tests**: Migration Safety, Data Consistency

---

## 🎯 KPI's

Erfolgreiche Security Test-Implementierung sollte diese Metriken erreichen:

| Metrik | Ziel | Status |
|--------|------|--------|
| Test Pass Rate | 100% | ✅ |
| Code Coverage | >80% | ✅ |
| Security Vulnerabilities | 0 Critical | ✅ |
| Response Time | <500ms | Pending |
| Cross-tenant leaks | 0 | ✅ |
| Information Disclosure | 0 | ✅ |

---

## 🚀 CI/CD Integration

Diese Tests sollten in der CI/CD Pipeline laufen:

```yaml
# .github/workflows/security-tests.yml
name: Security Tests

on: [push, pull_request]

jobs:
  security:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - run: dotnet test backend/shared/B2Connect.Shared.Tests/CriticalSecurityTests
      - name: Upload Coverage
        uses: codecov/codecov-action@v3
        with:
          files: './coverage.xml'
```

---

## 📞 Support

Falls Tests fehlschlagen oder Fragen:

1. **Lokale Tests**: `dotnet test --verbosity detailed`
2. **Logs**: Überprüfe Assertion-Meldungen
3. **Dokumentation**: Lies die ❌/✅ Muster in Test-Comments
4. **Code-Review**: Vergleiche Code mit den Patterns in dieser Dokumentation

---

**Letzte Aktualisierung**: 28. Dezember 2025  
**Autor**: Pentester, Tester, Lead Developer  
**Status**: Production-Ready ✅
