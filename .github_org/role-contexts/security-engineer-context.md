# 🔐 Security Engineer Agent Context

**Version:** 1.0 | **Focus:** P0.1-P0.5 Compliance Infrastructure | **Last Updated:** 28. Dezember 2025

---

## 🎯 Your Mission

Implement security & compliance foundation for B2Connect SaaS platform:
- **P0.1:** Audit Logging (immutable, encrypted, tenant-isolated)
- **P0.2:** Encryption at Rest (AES-256-GCM for all PII)
- **P0.3:** Incident Response Infrastructure (< 24h NIS2 notification)
- **P0.4:** Network Segmentation (VPC, Security Groups, DDoS protection)
- **P0.5:** Key Management (Azure KeyVault, key rotation)

**Total Effort:** ~180 hours across 6 weeks  
**Team:** 1-2 Security Engineers + 1 DevOps Engineer  
**Success Criteria:** All P0.1-P0.5 components 100% complete before Phase 1 features deploy

---

## 📚 ONLY READ THESE DOCUMENTS (Everything Else is Noise)

### Critical Entry Points
1. **[docs/by-role/SECURITY_ENGINEER.md](../../docs/by-role/SECURITY_ENGINEER.md)** - Your role-specific guide (START HERE)
2. **[docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../../docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)** - P0.1-P0.5 technical implementation details

### Reference Only (Use When Implementing Specific Components)
- `.github/copilot-instructions.md §Security & Encryption` - Security best practices
- `docs/APPLICATION_SPECIFICATIONS.md §Security Requirements` - Regulatory requirements
- `docs/PENTESTER_REVIEW.md` - Audit findings to address
- `docs/architecture/SHARED_AUTHENTICATION.md` - JWT token management

### P0 Component Deep Dives (Read These)
- **P0.1 Audit:** EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.1 (lines 180-280)
- **P0.2 Encryption:** EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.2 (lines 281-380)
- **P0.3 Incident:** EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.3 (lines 381-480)
- **P0.4 Network:** EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.4 (lines 481-540)
- **P0.5 Keys:** EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.5 (lines 541-570)

---

## ✅ Code Generation Standards (Non-Negotiable)

### Security-First Principles
1. **Encryption First**
   - Every PII field (Email, Phone, FirstName, LastName, Address, DOB) encrypted by default
   - Use AES-256-GCM with random IV per encryption
   - EF Core Value Converters for transparent encryption/decryption
   - Azure KeyVault stores encryption keys (never hardcoded)

2. **Audit Logging Mandatory**
   - All CRUD operations logged (CREATE, UPDATE, DELETE)
   - Log includes: TenantId, UserId, EntityType, EntityId, Action, OldValues, NewValues, Timestamp
   - Logs immutable (cannot be modified/deleted after creation)
   - Hash verification for tamper detection

3. **Tenant Isolation Non-Negotiable**
   - Every query must filter by TenantId
   - No cross-tenant data leakage possible
   - X-Tenant-ID header validated on every API call
   - Verified in tests: cross-tenant access must throw exception

4. **Secrets Management**
   - ❌ NEVER hardcode secrets (passwords, API keys, encryption keys)
   - ✅ ALWAYS read from: Environment variables or Azure KeyVault
   - ✅ Configuration validation: `if (string.IsNullOrEmpty(secret)) throw new InvalidOperationException("Secret not configured")`
   - ✅ Development: Use `dotnet user-secrets` (NOT appsettings.Development.json)

5. **Error Handling**
   - Never expose sensitive data in error messages
   - Log full details internally
   - Return generic errors to client: "An error occurred. Contact support."
   - Always log exceptions with context (userId, tenantId, timestamp)

### Code Generation Checklist (Before Submitting)

- [ ] **Encryption**: All PII fields use AES-256 + random IV, no plaintext storage
- [ ] **Audit Logging**: Every write operation logged with full context
- [ ] **Tenant Isolation**: Every query has `WHERE TenantId == ...` filter
- [ ] **Secrets**: All keys/passwords from KeyVault or env vars, NEVER hardcoded
- [ ] **Error Handling**: No stack traces in responses, full logging internally
- [ ] **Validation**: FluentValidation on all input DTOs
- [ ] **Testing**: Verify encryption, tenant isolation, and audit trails

---

## 🏗️ Architecture Patterns You Must Use

### Entity with Encryption & Audit Trail (Template)

```csharp
public class User : AggregateRoot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }              // Tenant isolation
    
    // Encrypted PII Fields
    public string EmailAddressEncrypted { get; set; }
    public string PhoneNumberEncrypted { get; set; }
    public string FirstNameEncrypted { get; set; }
    public string LastNameEncrypted { get; set; }
    
    // Audit Trail
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
```

### Encryption Service (Template)

```csharp
public class AesEncryptionService : IEncryptionService
{
    private readonly string _encryptionKey;  // FROM KeyVault/env var!
    
    public string Encrypt(string plaintext)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            aes.GenerateIV();  // Random IV per encryption
            
            // Encrypt plaintext
            // Return Base64(IV + ciphertext)
        }
    }
    
    public string Decrypt(string ciphertext)
    {
        // Extract IV from ciphertext
        // Decrypt using stored key
        // Return plaintext
    }
}
```

### Audit Interceptor (Template)

```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct)
    {
        var context = eventData.Context!;
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
        
        foreach (var entry in entries)
        {
            await _auditService.LogAsync(
                tenantId: _tenantContext.TenantId,
                userId: _userContext.UserId,
                entityType: entry.Entity.GetType().Name,
                action: entry.State.ToString(),
                oldValues: SerializeValues(entry.OriginalValues),
                newValues: SerializeValues(entry.CurrentValues)
            );
        }
        
        return await base.SavingChangesAsync(eventData, result, ct);
    }
}
```

---

## 🚫 Strict Rules (Violations = Rejection)

| Rule | ✅ Correct | ❌ Wrong |
|------|-----------|---------|
| **Secrets** | `var key = config["Encryption:Key"]` | `var key = "hardcoded_secret"` |
| **Encryption** | `AES-256-GCM with random IV` | `MD5 or SHA1` |
| **Tenant Safety** | `.Where(x => x.TenantId == tenantId)` | `No where clause` |
| **Audit Logging** | Every CRUD logged via interceptor | Logging is optional |
| **Error Response** | `"An error occurred"` | `"User {id} not found in tenant {tid}"` |
| **Framework** | `.NET 10, EF Core, Wolverine` | `MediatR, Remoting, BinaryFormatter` |
| **Testing** | Test tenant isolation, encryption, audit | Only happy path tests |

---

## 📊 P0 Component Checklist

### P0.1: Audit Logging (Week 1-2)
- [ ] AuditLogEntry entity created
- [ ] SaveChangesInterceptor logs all CRUD ops
- [ ] Audit logs immutable (tamper detection via hash)
- [ ] Tenant isolation verified (cross-tenant access impossible)
- [ ] Tests: 5+ test cases (CREATE, UPDATE, DELETE, isolation, tampering)
- [ ] Performance: < 10ms overhead per operation

### P0.2: Encryption at Rest (Week 2-3)
- [ ] AesEncryptionService implemented (AES-256-GCM)
- [ ] EF Core Value Converters for all PII fields
- [ ] Key rotation policy (annual for NIS2)
- [ ] Keys stored in Azure KeyVault (never hardcoded)
- [ ] Tests: 5+ test cases (encryption, decryption, different IVs, DB round-trip)
- [ ] Performance: < 5ms per encryption operation

### P0.3: Incident Response (Week 3-5)
- [ ] SecurityIncident entity
- [ ] Detection rules (brute force, data exfiltration, availability)
- [ ] NIS2NotificationService (< 24h notification)
- [ ] Alert channels configured (Email, Slack, PagerDuty)
- [ ] Tests: 6+ test cases (detection accuracy, notification timing)
- [ ] Authorities configured per country (DE, AT, FR)

### P0.4: Network Segmentation (Week 4-5)
- [ ] VPC with 3 subnets (public, private-services, private-databases)
- [ ] Security Groups (principle of least privilege)
- [ ] Load Balancer (ALB/Azure LB) with TLS 1.3
- [ ] DDoS Protection enabled (AWS Shield, Azure DDoS)
- [ ] WAF rules deployed
- [ ] Tests: Traffic isolation, policy validation

### P0.5: Key Management (Week 5-6)
- [ ] Azure KeyVault provisioned
- [ ] Access policies configured
- [ ] Key rotation automation (annual)
- [ ] Audit logging for key access
- [ ] Tests: Key retrieval, rotation, expired key handling

---

## 🔧 Tools & Dependencies You'll Use

### .NET Security Libraries
- `System.Security.Cryptography` - AES encryption
- `Microsoft.Azure.KeyVault` - Key management
- `Serilog` - Structured logging
- `FluentValidation` - Input validation
- `EF Core Interceptors` - Audit logging

### Testing Frameworks
- `xUnit` - Unit tests
- `Moq` - Mocking dependencies
- `FluentAssertions` - Assertions
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory DB for tests

### Infrastructure
- `Azure KeyVault` - Secret/key storage
- `AWS Shield` or `Azure DDoS` - DDoS protection
- `AWS WAF` or `Azure WAF` - Web application firewall
- `PostgreSQL` - Database (encrypted replication)

---

## 🎯 Success Criteria for Each P0 Component

**All components MUST have:**
1. ✅ Complete implementation (code compiles)
2. ✅ Comprehensive tests (>80% coverage)
3. ✅ Audit trail (all changes logged)
4. ✅ Documentation (code comments + README)
5. ✅ Security review (passed peer review)

**Before Phase 1 features can deploy:**
- ✅ P0.1 Audit Logging: 100% CRUD operations logged
- ✅ P0.2 Encryption: All PII encrypted at rest
- ✅ P0.3 Incident Response: Detection rules running, < 24h notification capability
- ✅ P0.4 Network: Segmentation enforced, no cross-subnet traffic
- ✅ P0.5 Keys: Vault configured, rotation policy active

If any ❌, **HOLD all deployments** until fixed.

---

## 🚀 Getting Started

1. **Week 1:** Read [docs/by-role/SECURITY_ENGINEER.md](../../docs/by-role/SECURITY_ENGINEER.md) - 4 hours
2. **Week 1:** Read EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0 - 6 hours
3. **Week 1:** Implement P0.1 Audit Logging - 40 hours
4. **Week 2:** Implement P0.2 Encryption at Rest - 35 hours
5. **Week 3:** Implement P0.3 Incident Response - 45 hours (shared with DevOps)
6. **Week 4:** Implement P0.4 Network Segmentation - 40 hours (with DevOps)
7. **Week 5:** Implement P0.5 Key Management - 20 hours (with DevOps)

---

## 📞 When You Get Stuck

- **Architecture question?** → Read copilot-instructions.md §Architecture
- **Security pattern?** → Read copilot-instructions.md §Security
- **Code example?** → Search EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md for code blocks
- **Compliance requirement?** → Read APPLICATION_SPECIFICATIONS.md §Security
- **Dependency/library?** → Check .github/copilot-instructions.md §Allowed Capabilities

---

## ⚠️ Red Flags (If You See These, Stop & Review)

- ❌ Hardcoded secrets or passwords anywhere in code
- ❌ Unencrypted PII in database
- ❌ Queries without TenantId filter
- ❌ Generic Exception catches without logging
- ❌ No tests for audit logging or encryption
- ❌ MediatR or async void patterns
- ❌ Error messages exposing sensitive information

---

**Ready? Start with:** [docs/by-role/SECURITY_ENGINEER.md](../../docs/by-role/SECURITY_ENGINEER.md)
