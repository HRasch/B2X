# Requirements Summary

**Version:** 1.0  
**Last Updated:** 27. Dezember 2025  
**Status:** In Effect for P0 Week

---

## Executive Summary

B2Connect ist eine **Multi-Tenant SaaS E-Commerce Plattform** mit Microservices-Architektur. Diese Anforderungen decken die **kritischen P0-Probleme** ab, die bis Freitag (03.01.2026) behoben sein müssen.

**Kritische Metriken:**
- 🚨 4 P0 (Critical) Issues: Alle müssen diese Woche behoben sein
- 📊 < 5% Test Coverage: Wird auf > 40% erhöht (Woche 2-3)
- 🔒 0 Verschlüsselung: Implementierung Mittwoch-Donnerstag
- 📝 0 Audit Trail: Implementierung Donnerstag-Freitag

---

## 🎯 P0 Critical Requirements (This Week)

### P0.1: Remove Hardcoded JWT Secrets

**Requirement:** JWT Secrets dürfen nicht hardcoded sein

**Current State:**
```csharp
// ❌ BAD
var jwtSecret = "B2Connect-Super-Secret-Key-For-Development-Only-32chars!";
```

**Required State:**
```csharp
// ✅ GOOD
var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? (app.Environment.IsDevelopment() 
        ? "dev-only-minimum-32-characters-key!!!" 
        : throw new InvalidOperationException("JWT Secret not configured"));
```

**Acceptance Criteria:**
- [ ] No hardcoded secrets in Program.cs
- [ ] Secrets loaded from IConfiguration (appsettings, env vars)
- [ ] Production requires environment variable
- [ ] Minimum 32-character length enforced
- [ ] All services affected (Admin API, Store API, Identity)
- [ ] All tests passing
- [ ] Code review approved

**Files to Update:**
- `backend/BoundedContexts/Admin/API/src/Presentation/Program.cs`
- `backend/BoundedContexts/Store/API/Program.cs`
- `backend/BoundedContexts/Shared/Identity/Program.cs`
- `backend/ServiceDefaults/Extensions.cs`

**Effort:** 8 hours  
**Timeline:** Monday morning - Tuesday noon  
**Owner:** Dev 1

**Reference:** [CRITICAL_ISSUES_ROADMAP.md - P0.1](../CRITICAL_ISSUES_ROADMAP.md#p01)

---

### P0.2: Fix CORS Configuration

**Requirement:** CORS darf nicht auf localhost hardcoded sein

**Current State:**
```csharp
// ❌ BAD - hardcoded
.WithOrigins("http://localhost:5174", "https://localhost:5174")
```

**Required State:**
```csharp
// ✅ GOOD - config-based
var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

if (app.Environment.IsProduction() && corsOrigins.Length == 0)
    throw new InvalidOperationException("CORS origins not configured for production");

policy.WithOrigins(corsOrigins)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithMaxAge(TimeSpan.FromHours(24));
```

**appsettings.Development.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5174",
      "http://localhost:5173",
      "http://127.0.0.1:5174",
      "https://localhost:5174"
    ]
  }
}
```

**appsettings.Production.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://admin.b2connect.com",
      "https://store.b2connect.com"
    ]
  }
}
```

**Acceptance Criteria:**
- [ ] CORS origins loaded from configuration
- [ ] No hardcoded domains in code
- [ ] Separate config for Development/Production
- [ ] Production fails startup if not configured
- [ ] Preflight requests handled correctly
- [ ] WithMaxAge set to 24 hours
- [ ] All tests passing
- [ ] Code review approved

**Files to Update:**
- All `Program.cs` files with CORS configuration
- `appsettings.Development.json`
- Create `appsettings.Production.json`

**Effort:** 6 hours  
**Timeline:** Monday morning - Tuesday noon  
**Owner:** Dev 2

**Reference:** [CRITICAL_ISSUES_ROADMAP.md - P0.2](../CRITICAL_ISSUES_ROADMAP.md#p02)

---

### P0.3: Implement Encryption at Rest

**Requirement:** PII muss verschlüsselt werden

**Affected Fields:**
- Email address
- Phone number
- First name
- Last name
- Address
- SSN/Tax ID (if applicable)
- Date of birth

**Implementation Approach:**
1. Create `IEncryptionService` with AES-256
2. Register as singleton in DI
3. Use EF Core Value Converters for automatic encryption/decryption
4. Key management from configuration or Key Vault

**Required Code Structure:**

**EncryptionService.cs:**
```csharp
public interface IEncryptionService
{
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
}

public class AesEncryptionService : IEncryptionService
{
    // AES-256 encryption with IV
    // Key loaded from configuration or Key Vault
}
```

**DbContext Configuration:**
```csharp
modelBuilder.Entity<User>()
    .Property(u => u.Email)
    .HasConversion(
        v => encryptionService.Encrypt(v),
        v => encryptionService.Decrypt(v)
    );
```

**Acceptance Criteria:**
- [ ] AES-256 encryption implemented
- [ ] IV generated randomly for each encryption
- [ ] Key ≥ 256 bits
- [ ] Value converters configured for all PII fields
- [ ] Existing data migration planned (if applicable)
- [ ] Decryption works for existing data
- [ ] Key management (config/KeyVault)
- [ ] All tests passing
- [ ] Code review approved

**Files to Create/Update:**
- Create `backend/shared/Services/Encryption/IEncryptionService.cs`
- Create `backend/shared/Services/Encryption/AesEncryptionService.cs`
- Update all DbContext configurations
- Create database migration

**Effort:** 8 hours  
**Timeline:** Wednesday full day  
**Owner:** Dev 1 & Dev 2

**Reference:** [CRITICAL_ISSUES_ROADMAP.md - P0.3](../CRITICAL_ISSUES_ROADMAP.md#p03)

---

### P0.4: Implement Audit Logging

**Requirement:** Alle Datenmodifikationen müssen geloggt werden

**Implementation Approach:**
1. Add audit fields to `BaseEntity`
2. Create `AuditInterceptor` in EF Core
3. Implement soft deletes
4. Configure global query filters

**Required Fields in BaseEntity:**
```csharp
public DateTime CreatedAt { get; set; }
public Guid CreatedBy { get; set; }
public DateTime ModifiedAt { get; set; }
public Guid? ModifiedBy { get; set; }
public DateTime? DeletedAt { get; set; }
public Guid? DeletedBy { get; set; }
public bool IsDeleted { get; set; }
```

**AuditInterceptor Implementation:**
```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    // Sets CreatedAt/CreatedBy on Add
    // Sets ModifiedAt/ModifiedBy on Update
    // Sets DeletedAt/DeletedBy on Delete (soft delete)
    // Logs all changes
}
```

**Audit Log Table:**
```sql
CREATE TABLE AuditLogs (
    Id UUID PRIMARY KEY,
    TenantId UUID NOT NULL,
    UserId UUID,
    EntityType VARCHAR(255),
    EntityId UUID,
    Action VARCHAR(50), -- Create, Update, Delete
    OldValues JSONB,
    NewValues JSONB,
    CreatedAt TIMESTAMPTZ,
    IPAddress VARCHAR(45),
    UserAgent TEXT
);
```

**Acceptance Criteria:**
- [ ] BaseEntity updated with audit fields
- [ ] AuditInterceptor implemented
- [ ] CreatedAt/CreatedBy set on Create
- [ ] ModifiedAt/ModifiedBy set on Update
- [ ] Soft deletes implemented (IsDeleted = true)
- [ ] Global query filter excludes deleted
- [ ] AuditLogs table created
- [ ] Audit logging working end-to-end
- [ ] All tests passing
- [ ] Code review approved

**Files to Create/Update:**
- Update `backend/shared/Domain/BaseEntity.cs`
- Create `backend/shared/Infrastructure/AuditInterceptor.cs`
- Update all DbContext files
- Create database migration
- Update global query filters

**Effort:** 8 hours  
**Timeline:** Thursday full day  
**Owner:** Dev 2 & Dev 1

**Reference:** [CRITICAL_ISSUES_ROADMAP.md - P0.4](../CRITICAL_ISSUES_ROADMAP.md#p04)

---

## 📋 P1 High-Priority Requirements (Weeks 2-3)

### Testing Framework Implementation
- Unit test framework (xUnit)
- Integration tests (Testcontainers)
- E2E tests expansion
- Target: 40%+ coverage

### API Response Standardization
- Consistent response format across services
- Standard error format
- Pagination standardization
- Status code standardization

### Rate Limiting
- Global rate limit: 1000 req/min per IP
- Per-user limit: 100 req/min
- Burst handling
- Rate limit headers

---

## 🔐 Security Requirements

### Authentication
- [x] JWT-based authentication
- [x] Token expiration (1 hour access, 7 days refresh)
- [ ] Hardcoded secrets removed (P0.1)
- [ ] CORS properly configured (P0.2)

### Data Protection
- [ ] Encryption at rest (P0.3)
- [ ] Encryption in transit (TLS 1.2+)
- [ ] No PII in logs
- [ ] Secure key management

### Audit & Compliance
- [ ] All data modifications logged (P0.4)
- [ ] GDPR compliance (Articles 15, 17, 20, 21)
- [ ] SOC2 readiness
- [ ] Regular security assessments

---

## 📊 Database Requirements

### New Tables (P0.4)
```
AuditLogs
├── Id (UUID, PK)
├── TenantId (UUID, FK)
├── UserId (UUID, FK)
├── EntityType (VARCHAR)
├── EntityId (UUID)
├── Action (VARCHAR: Create, Update, Delete)
├── OldValues (JSONB)
├── NewValues (JSONB)
├── CreatedAt (TIMESTAMPTZ)
├── IPAddress (VARCHAR)
└── UserAgent (TEXT)
```

### Updated Base Entity
- CreatedAt, CreatedBy
- ModifiedAt, ModifiedBy
- DeletedAt, DeletedBy
- IsDeleted

### Encrypted Fields (P0.3)
- User.Email
- User.PhoneNumber
- User.FirstName
- User.LastName
- User.Address
- User.SSN (if applicable)

---

## 🧪 Testing Requirements

### Unit Tests (Minimum)
- Service layer logic
- Validation logic
- Encryption/decryption
- Audit logging

### Integration Tests
- Database operations
- API endpoints
- CORS validation
- JWT token validation

### Example Tests for P0 Fixes

**P0.1 Test (JWT Secrets):**
```csharp
[Fact]
public void JwtSecret_IsNotHardcoded_LoadsFromConfiguration()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new[] { 
            new KeyValuePair<string, string>("Jwt:Secret", "test-secret-minimum-32-chars!!!")
        })
        .Build();
    
    // Act
    var secret = config["Jwt:Secret"];
    
    // Assert
    Assert.NotEmpty(secret);
    Assert.True(secret.Length >= 32);
}
```

**P0.3 Test (Encryption):**
```csharp
[Fact]
public void Encrypt_DecryptRoundtrip_ReturnsOriginalValue()
{
    // Arrange
    var originalEmail = "test@example.com";
    var encryptionService = new AesEncryptionService(keyProvider);
    
    // Act
    var encrypted = encryptionService.Encrypt(originalEmail);
    var decrypted = encryptionService.Decrypt(encrypted);
    
    // Assert
    Assert.Equal(originalEmail, decrypted);
    Assert.NotEqual(originalEmail, encrypted);
}
```

**P0.4 Test (Audit Logging):**
```csharp
[Fact]
public async Task SaveChanges_SetsAuditFields_OnEntityModification()
{
    // Arrange
    var user = new User { Email = "test@example.com" };
    
    // Act
    context.Users.Add(user);
    await context.SaveChangesAsync();
    
    // Assert
    Assert.NotEqual(default, user.CreatedAt);
    Assert.NotEqual(default, user.CreatedBy);
}
```

---

## 📈 Success Metrics

### P0 Week (This Week)
| Metric | Target | Current | By Friday |
|--------|--------|---------|-----------|
| P0.1 - JWT Secrets | 100% removed | 0% | ✅ 100% |
| P0.2 - CORS Config | Environment-based | 0% | ✅ 100% |
| P0.3 - Encryption | PII encrypted | 0% | ✅ 100% |
| P0.4 - Audit Logs | All mods logged | 0% | ✅ 100% |
| Build Status | Passing | ? | ✅ Passing |
| Test Coverage | > 10% | 5% | ✅ > 10% |

### P1 Week (Weeks 2-3)
| Metric | Target | By End Week 3 |
|--------|--------|---------------|
| Test Coverage | > 40% | ✅ 40%+ |
| Rate Limiting | Implemented | ✅ Done |
| API Standardization | 100% consistent | ✅ Done |

---

## 📋 Implementation Checklist

### Monday (P0.1 + P0.2)
- [ ] 09:00 Kickoff meeting
- [ ] 10:00 Dev 1 starts P0.1 (JWT Secrets)
- [ ] 10:00 Dev 2 starts P0.2 (CORS Config)
- [ ] 17:00 First commit
- [ ] Daily standup recorded

### Tuesday (Testing)
- [ ] Unit tests for P0.1 + P0.2
- [ ] Integration tests
- [ ] Manual testing
- [ ] Code review
- [ ] Commit and PR merge

### Wednesday (P0.3 Encryption)
- [ ] Design AES encryption service
- [ ] Implement encryption service
- [ ] Configure value converters
- [ ] Create database migration
- [ ] Test encryption/decryption

### Thursday (P0.4 Audit Logging)
- [ ] Design audit fields
- [ ] Implement AuditInterceptor
- [ ] Configure soft deletes
- [ ] Create audit log table
- [ ] Test audit logging

### Friday (Final Testing & Merge)
- [ ] Full regression testing
- [ ] Performance testing
- [ ] Security review
- [ ] Code review approval
- [ ] Merge to main
- [ ] Team celebration 🎉

---

## 🚫 Non-Requirements (Not This Week)

### Will NOT Be Done This Week
- [ ] GDPR compliance features (Week 4-5)
- [ ] Event-driven architecture (Week 6)
- [ ] Load testing (Week 7)
- [ ] Architecture refactoring (Week 6+)
- [ ] Performance optimization (Week 7)

### Deferred to P1
- [ ] Comprehensive test coverage (40%+)
- [ ] API response standardization
- [ ] Rate limiting
- [ ] Frontend error handling improvements

---

## 📚 References

### Documentation
- [APPLICATION_SPECIFICATIONS.md](APPLICATION_SPECIFICATIONS.md) — Full system specs
- [GITHUB_WORKFLOWS.md](GITHUB_WORKFLOWS.md) — Development workflows
- [CRITICAL_ISSUES_ROADMAP.md](../CRITICAL_ISSUES_ROADMAP.md) — Day-by-day guide
- [SECURITY_HARDENING_GUIDE.md](../SECURITY_HARDENING_GUIDE.md) — Code examples
- [QUICK_START_P0.md](../QUICK_START_P0.md) — Quick reference

### GitHub Templates
- [.github/ISSUE_TEMPLATE/p0-security-issue.md](../.github/ISSUE_TEMPLATE/p0-security-issue.md)
- [.github/pull_request_template.md](../.github/pull_request_template.md)

### Tools & Commands
```bash
# Testing
dotnet test backend/B2Connect.slnx

# Build
dotnet build backend/B2Connect.slnx

# Code formatting
dotnet format backend/B2Connect.slnx
```

---

## 🎯 Sign-Off

**Requirements Created By:** Architecture & Security Team  
**Date:** 27. Dezember 2025  
**Valid Through:** 03. Januar 2026 (end of P0 week)  
**Next Review:** 06. Januar 2026 (start of P1 week)

**Team Commitment:**
- [ ] All requirements understood
- [ ] Effort estimates realistic
- [ ] Resources allocated
- [ ] Timeline committed
- [ ] Success criteria agreed

---

**Version Control:**
- v1.0 - Initial requirements (27.12.2025)
- Changes tracked in DOCUMENTATION_INDEX.md
