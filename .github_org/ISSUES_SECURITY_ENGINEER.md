# Security Engineer - Zugeordnete Issues

**Status**: 0/9 Assigned  
**Gesamtaufwand**: ~25 Story Points  
**Kritischer Pfad**: Sprint 1-4 (NIS2 + GDPR + Data Protection)

---

## Sprint 1 (P0.1 - Audit Foundation)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #30 | VAT-ID Validation (Security Review) | 3 | - |
| #31 | Reverse Charge Logic (Security) | 3 | - |

**Summe Sprint 1**: 6 Story Points (+ Infrastructure)

---

## Sprint 2 (P0.2 - Encryption & Compliance)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #32 | Invoice Encryption & Secure Storage | 5 | Backend #21 |
| #27 | Database Migrations (Encryption Schema) | 2 | - |

**Summe Sprint 2**: 7 Story Points

---

## Sprint 3 (P0.3 & P0.4 - Monitoring & Response)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #34 | Audit Logging für Invoice Changes | 5 | Backend #21, #32 |
| #35 | Payment Fraud Detection | 4 | Backend #20 |
| #36 | GDPR Right-to-Forget Compliance | 3 | Database Schema |

**Summe Sprint 3**: 12 Story Points

---

## Sprint 4 (P0.5 & Incident Response)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #37 | Encryption Key Rotation Policy | 3 | #32 (Encryption Service) |
| #38 | Incident Response Procedures | 2 | All Security |
| #39 | Security Audit Trail (Umfassend) | 3 | All Logging |

**Summe Sprint 4**: 8 Story Points

---

## Compliance-Anforderungen

### P0.1: Audit Logging (Immutable)
- ✅ All CRUD operations logged
- ✅ Timestamp, User ID, Action, Before/After values
- ✅ Soft deletes nur (keine Hard Deletes)
- ✅ Tenant ID immer dabei
- **Issues**: #34, #39

### P0.2: Encryption at Rest (AES-256-GCM)
- ✅ PII verschlüsselt: Email, Phone, Address, SSN, DOB, Names
- ✅ Produktkosten verschlüsselt
- ✅ Key Rotation (annual)
- **Issues**: #32, #37

### P0.3: Incident Response (< 24h Notification)
- ✅ Automated detection (brute force, data exfiltration, availability)
- ✅ NIS2 notification service
- ✅ Incident tracking dashboard
- **Issues**: #35, #38

### P0.4: Network Segmentation & DDoS
- ✅ VPC mit 3 Subnets
- ✅ DDoS Protection (AWS Shield / Azure DDoS)
- ✅ WAF Rules
- ✅ mTLS für Service-to-Service
- **Koordiniert mit**: DevOps Engineer

### P0.5: Key Management (Azure KeyVault)
- ✅ No hardcoded secrets
- ✅ Automated key rotation
- ✅ Access control & audit logging
- **Issues**: #32, #37

---

## Technische Anforderungen

### Encryption Service Implementation

```csharp
// Core/Services/IEncryptionService.cs
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    void RotateKeys();
}

// Infrastructure/Services/AesEncryptionService.cs
public class AesEncryptionService : IEncryptionService
{
    // AES-256-GCM implementation
    // Per-field encryption with random IV
    // Key rotation support
}

// Core/Services/IAuditService.cs
public interface IAuditService
{
    Task LogAsync(Guid tenantId, Guid userId, string entity, 
        string action, object? before = null, object? after = null);
}
```

### Audit Logging Implementation

```csharp
// Core/Entities/AuditLogEntry.cs
public class AuditLogEntry
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public string EntityType { get; init; }
    public string Action { get; init; } // CREATE, UPDATE, DELETE
    public DateTime CreatedAt { get; init; }
    public string? BeforeValues { get; init; } // JSON
    public string? AfterValues { get; init; }
}
```

---

## Abhängigkeiten

| Security Issue | Benötigt Backend | Status |
|----------------|-----------------|--------|
| #32 | #21 (Invoice Gen) | ⏳ Wartend |
| #34 | #21 (Invoice APIs) | ⏳ Wartend |
| #35 | #20 (Price Calc) | ⏳ Wartend |
| #37 | #32 (Encryption) | Sequential (#32 first) |

---

## Blockt folgende Issues

```
Backend #20, #21, #29, #30, #31 warten auf Security Review:
  - VAT-ID Validation (#30)
  - Reverse Charge (#31)
  - Price Calculation (#20)
  - Invoice API (#21)
  - Return Policy (#29)
```

---

## Nächste Schritte

1. **1 Security Engineer zuweisen** (9 Issues, 6-8 Wochen)
2. **Sprint 1 fokus**: Audit Foundation (#30, #31 Review)
3. **Parallel**: Key Management & Encryption Service Design (#32)
4. **Nach Sprint 2**: Incident Response (#35, #38)
