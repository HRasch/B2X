# 🔐 Security Engineer Quick Start

**Role Focus:** Encryption, Audit logging, Incident response, Key management  
**Time to Productivity:** 3 weeks  
**Critical Components:** P0.1, P0.2, P0.3, P0.5, P0.7

---

## ⚡ Week 1: Compliance Foundation

### Day 1-2: Read Regulatory Framework
```
1. copilot-instructions.md (§Security Checklist) - 30 min
2. docs/APPLICATION_SPECIFICATIONS.md (§Security Requirements) - 45 min
3. docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.1-P0.5) - 90 min
```

**Key Regulations:**
- **NIS2**: Incident notification < 24 hours (Art. 23)
- **GDPR**: Encryption at rest (Art. 32), breach notification (Art. 33)
- **DORA**: Operational resilience (Art. 6, 10, 19)
- **AI Act**: Risk management, transparency (Art. 6, 22, 35)

### Day 3-4: Review Current Security Status
```bash
# Check what's implemented:
dotnet build B2Connect.slnx -v minimal

# Search for security patterns:
grep -r "Encrypt\|AuditLog\|Incident" backend/Domain --include="*.cs" | head -20

# Check for hardcoded secrets:
grep -r "password\|secret\|key" backend --include="*.cs" | grep -v "config\|environment"
```

### Day 5: Map P0 Components
- **P0.1**: Audit Logging (immutable, encrypted)
- **P0.2**: Encryption at Rest (AES-256)
- **P0.3**: Incident Response (< 24h notification)
- **P0.4**: Network Segmentation (DevOps)
- **P0.5**: Key Management (Azure KeyVault)
- **P0.7**: AI Act Compliance (transparency, bias testing)

---

## 🔒 Week 2: Implementation

### Day 1-2: Audit Logging System

```csharp
// 1. Create AuditLogEntry entity
public class AuditLogEntry
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }        // Tenant isolation
    public Guid UserId { get; set; }           // WHO changed it
    public string EntityType { get; set; }     // Product, User, Order
    public string EntityId { get; set; }       // Which record
    public string Action { get; set; }         // CREATE, UPDATE, DELETE
    public string OldValues { get; set; }      // JSON before
    public string NewValues { get; set; }      // JSON after
    public DateTime CreatedAt { get; set; }    // IMMUTABLE timestamp
    public string Hash { get; set; }           // SHA-256 for tamper detection
}

// 2. Create EF Core interceptor
public class AuditInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        // Log all CRUD operations before SaveAsync
    }
}

// 3. Register in DI
builder.Services.AddScoped<AuditInterceptor>();
```

**Acceptance Criteria:**
- All CRUD operations logged ✅
- Logs encrypted at rest ✅
- Logs immutable (tamper detection) ✅
- Tenant isolation verified ✅
- Performance < 10ms overhead ✅

### Day 3-4: Encryption at Rest (AES-256)

```csharp
// 1. Encryption service
public class AesEncryptionService : IEncryptionService
{
    public string Encrypt(string plaintext)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_key);
            aes.GenerateIV();  // Random per encryption
            
            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, ...))
                using (var sw = new StreamWriter(cs))
                    sw.Write(plaintext);
                
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}

// 2. EF Core value converters
modelBuilder.Entity<User>()
    .Property(u => u.EmailAddressEncrypted)
    .HasConversion(
        v => _encryptionService.Encrypt(v),
        v => _encryptionService.Decrypt(v)
    );

// 3. Key rotation policy
// Annual rotation required by NIS2
```

**PII Fields to Encrypt:**
- Email address
- Phone number
- First name, last name
- Date of birth
- Address
- Supplier names (B2B)

### Day 5: Incident Detection Rules

```csharp
// 1. Brute force detection
if (failedLogins > 5 in 10 minutes from IP) {
    await _incidents.CreateAsync(new SecurityIncident {
        Type = "BruteForceAttack",
        Severity = "High"
    });
}

// 2. Data exfiltration detection
if (dailyDataVolume > 3x baseline) {
    await _incidents.CreateAsync(new SecurityIncident {
        Type = "DataExfiltration",
        Severity = "Critical"
    });
}

// 3. NIS2 notification (< 24 hours)
await _nis2Service.NotifyAuthoritiesAsync(incident);
```

---

## 🛡️ Week 3: Testing & Verification

### Day 1-2: Write Security Tests
```bash
dotnet test --filter "Category=Security"

# Test requirements:
# 1. Encryption/Decryption round-trip
# 2. Different IVs produce different ciphertexts
# 3. Audit logs created for all CRUD ops
# 4. Logs immutable (tamper detection works)
# 5. Brute force detection fires correctly
# 6. NIS2 notification within 24h
# 7. Tenant isolation (cross-tenant access blocked)
# 8. No hardcoded secrets found
```

### Day 3-4: Compliance Audit
```bash
# Check for compliance issues:
1. Search for hardcoded secrets
   grep -r "password\|secret" backend --include="*.cs"

2. Verify encryption
   grep -r "Encrypt\|AES" backend --include="*.cs"

3. Verify audit logging
   grep -r "AuditLog" backend --include="*.cs"

4. Check tenant isolation
   grep -r "TenantId" backend/Domain/*/src --include="*.cs"
```

### Day 5: Documentation & Handover
- [ ] All P0 components documented
- [ ] Encryption keys configured (Azure KeyVault)
- [ ] NIS2 notification procedures documented
- [ ] Incident response runbook created
- [ ] Security review checklist published

---

## ⚡ Quick Commands

```bash
# Build and test security
dotnet build B2Connect.slnx
dotnet test --filter "Category=Security"

# Search for vulnerabilities
grep -r "password\|secret\|hardcoded" backend --include="*.cs" | grep -v "config"

# Check dependencies for vulnerabilities
dotnet list B2Connect.slnx package --vulnerable

# Run OWASP dependency check (if installed)
dotnet tool install -g DependencyCheck
dependency-check --scan backend/ --out .
```

---

## 📚 Critical Resources

| Topic | File | Time |
|-------|------|------|
| Security Checklist | `.github/copilot-instructions.md` | 30 min |
| Compliance Roadmap | `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | 60 min |
| Audit Logging | `docs/AUDIT_LOGGING_IMPLEMENTATION.md` | 20 min |
| AI Act Compliance | `docs/P0.7_AI_ACT_TESTS.md` | 30 min |
| Pentester Review | `docs/PENTESTER_REVIEW.md` | 20 min |

---

## 🎯 First Task: Implement P0.1 (Audit Logging)

**Phase 1: Setup**
1. Create AuditLogEntry entity
2. Create EF Core interceptor
3. Register in DI
4. Verify compilation

**Phase 2: Testing**
1. Write test: CRUD operation creates log
2. Write test: Log is immutable
3. Write test: Tenant isolation enforced
4. Run tests (target: 3/3 passing)

**Phase 3: Verification**
1. Manual test: Create product → verify audit log
2. Check performance: < 10ms overhead
3. Code review: Security Engineer approval

**Time Estimate:** 2 weeks  
**Success Criteria:** All 5 tests passing + performance verified

---

## 🔐 P0 Components Timeline

```
Week 1:     P0.1 (Audit Logging) + P0.2 (Encryption)
Week 2-3:   P0.3 (Incident Response) + P0.5 (Key Management)
Week 4+:    P0.7 (AI Act Compliance)
```

---

## 🚨 Critical Compliance Deadlines

| Deadline | Component | Penalty |
|----------|-----------|---------|
| 28. Juni 2025 | BITV/Accessibility | €5K-100K |
| 17. Okt 2025 | NIS2 Incident Response | Business disruption |
| 12. Mai 2026 | AI Act Compliance | €30M max fine |

---

## 📞 Getting Help

- **Encryption questions:** Microsoft Cryptography docs
- **GDPR questions:** Your Data Protection Officer
- **NIS2 questions:** BSI (Germany) or competent authority
- **Architecture questions:** Tech Lead
- **Implementation questions:** Backend Developer

---

**Key Reminders:**
- No hardcoded secrets (use environment variables)
- All PII encrypted (email, phone, name, address)
- All data changes audited (immutable logs)
- Tenant isolation everywhere (TenantId in every query)
- NIS2 < 24h notification is non-negotiable
