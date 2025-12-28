# 🔐 Security Engineer - Documentation Guide

**Role:** Security Engineer | **P0 Components:** P0.1, P0.2, P0.3, P0.5, P0.7  
**Time to Read:** ~4 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als Security Engineer bist du verantwortlich für:
- **Encryption at Rest** (AES-256) - P0.2
- **Audit Logging** (Immutable, Tenant-isolated) - P0.1
- **Incident Response** (< 24h NIS2 notification) - P0.3
- **Key Management** (Azure KeyVault) - P0.5
- **AI Act Compliance** (Fraud Detection HIGH-RISK) - P0.7

---

## 📚 Required Reading (P0)

### Week 1: Security Foundation

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **Security Checklist** | [copilot-instructions.md §Security](../../.github/copilot-instructions.md) | 30 min |
| 2 | **Application Specs §Security** | [APPLICATION_SPECIFICATIONS.md](../APPLICATION_SPECIFICATIONS.md) | 45 min |
| 3 | **Pentester Review** | [PENTESTER_REVIEW.md](../PENTESTER_REVIEW.md) | 30 min |
| 4 | **Shared Authentication** | [architecture/SHARED_AUTHENTICATION.md](../architecture/SHARED_AUTHENTICATION.md) | 30 min |

### Week 2: Compliance Deep Dive

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 5 | **EU Compliance Roadmap** | [compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) | 90 min |
| 6 | **AI Act Tests (P0.7)** | [compliance/P0.7_AI_ACT_TESTS.md](../compliance/P0.7_AI_ACT_TESTS.md) | 45 min |
| 7 | **AI Act Overview** | [compliance/AI_ACT_OVERVIEW.md](../compliance/AI_ACT_OVERVIEW.md) | 30 min |
| 8 | **Audit Logging Guide** | [AUDIT_LOGGING_IMPLEMENTATION.md](../AUDIT_LOGGING_IMPLEMENTATION.md) | 30 min |

### Week 3: Testing & Verification

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 9 | **Compliance Testing** | [compliance/COMPLIANCE_TESTING_EXAMPLES.md](../compliance/COMPLIANCE_TESTING_EXAMPLES.md) | 30 min |
| 10 | **Quick Start Checklist** | [compliance/COMPLIANCE_QUICK_START_CHECKLIST.md](../compliance/COMPLIANCE_QUICK_START_CHECKLIST.md) | 15 min |

---

## 🔧 Your P0 Components

### P0.1: Audit Logging (Week 3-4)
```
Effort: 40 hours
Acceptance: 
  ✅ All CRUD operations logged
  ✅ Logs immutable (tamper detection)
  ✅ Tenant isolation enforced
  ✅ AES-256 encryption at rest
```

### P0.2: Encryption at Rest (Week 1-2)
```
Effort: 35 hours
Acceptance:
  ✅ All PII fields encrypted (Email, Phone, Address, DOB)
  ✅ AES-256-GCM with random IV
  ✅ Key rotation policy (annual)
  ✅ Azure KeyVault integration
```

### P0.3: Incident Response (Week 5-6)
```
Effort: 45 hours (with DevOps)
Acceptance:
  ✅ Detection rules: brute force, data exfiltration
  ✅ < 24h notification to authorities (NIS2 Art. 23)
  ✅ SIEM integration ready
```

### P0.5: Key Management (Week 7-8)
```
Effort: 20 hours (with DevOps)
Acceptance:
  ✅ Azure KeyVault configured
  ✅ No hardcoded secrets
  ✅ Key rotation automation
  ✅ Access audit logging
```

### P0.7: AI Act Compliance (Week 9-10)
```
Effort: 50 hours
Acceptance:
  ✅ AI Risk Register documented
  ✅ Fraud Detection classified as HIGH-RISK
  ✅ Decision logging implemented
  ✅ Bias testing framework
  ✅ User Right to Explanation API
```

---

## ⚡ Quick Commands

```bash
# Run security tests
dotnet test --filter "Category=Security"

# Check for hardcoded secrets
grep -r "password\|secret\|key" --include="*.cs" backend/

# Verify encryption
dotnet run --project backend/Domain/Identity/tests -- --filter "Encryption"

# Run compliance tests
dotnet test backend/Domain/Identity/tests -v minimal
```

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Security Incident | Security Lead → CTO | < 1h |
| Key Compromise | Security Lead → CEO | Immediate |
| Compliance Blocker | Legal Officer | < 24h |
| Architecture Question | Tech Lead | < 4h |

---

## ✅ Definition of Done (Security Engineer)

Before marking any P0 component as complete:

- [ ] All tests passing (xUnit)
- [ ] Code review by Tech Lead
- [ ] Penetration test passed (if applicable)
- [ ] Documentation updated
- [ ] No hardcoded secrets (verified)
- [ ] Encryption verified (AES-256)
- [ ] Audit logging active
- [ ] Legal review (if P0.7)

---

**Next:** Start with [copilot-instructions.md §Security](../../.github/copilot-instructions.md)
