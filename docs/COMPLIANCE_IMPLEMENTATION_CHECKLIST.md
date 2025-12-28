# Compliance Implementation Checklist

**Version:** 1.0 | **Last Updated:** 28. Dezember 2025 | **Status:** Active

---

## Overview

This checklist ensures **every feature deployed to production meets EU compliance requirements** (NIS2, GDPR, DORA, AI Act, BITV).

**Use this checklist:**
- Before starting a feature (acceptance criteria)
- During development (design verification)
- Before PR submission (final verification)
- Before production deployment (gate)

---

## Phase 0: Compliance Foundation (Required before Phase 1)

### P0.1: Audit Logging ✅

**[✅ IN PROGRESS]** - Status: Core entity and interceptor implemented

- [ ] **Entity Created**
  - [ ] AuditLogEntry with TenantId, UserId, Action, OldValues, NewValues
  - [ ] Soft delete support (IsArchived, ArchivedAt)
  - [ ] Immutable on retrieval (no updates after creation)

- [ ] **EF Core Integration**
  - [ ] SaveChangesInterceptor logs all CRUD
  - [ ] JSON serialization of values (NewtonsoftJson)
  - [ ] Performance < 10ms overhead per operation

- [ ] **Testing**
  - [ ] Unit tests: Entity creation, mapping, soft delete
  - [ ] Integration tests: Product CRUD logging, User CRUD logging
  - [ ] Multi-tenant isolation test (cross-tenant leak impossible)
  - [ ] Audit tests: Verify logs created for CREATE, UPDATE, DELETE

- [ ] **Regulatory Compliance**
  - [ ] NIS2 Art. 21: Audit trail ✅
  - [ ] GDPR Art. 32(c): Accountability ✅

**Owner:** Security Engineer  
**Deadline:** Week 1-2

---

### P0.2: Encryption at Rest (AES-256-GCM) ✅

**[✅ IN PROGRESS]** - Status: Service and EF converters implemented

- [ ] **Encryption Service**
  - [ ] IEncryptionService interface (no Framework dependencies)
  - [ ] AES-256-GCM implementation
  - [ ] Random IV per encryption
  - [ ] Base64 output format
  - [ ] Performance < 5ms per operation

- [ ] **PII Fields Encrypted**
  - [ ] User.Email
  - [ ] User.Phone
  - [ ] User.FirstName
  - [ ] User.LastName
  - [ ] User.DateOfBirth
  - [ ] User.Address
  - [ ] Product.CostPrice
  - [ ] Product.SupplierName

- [ ] **EF Core Integration**
  - [ ] Value converters for all PII fields
  - [ ] Automatic encryption on SaveChanges
  - [ ] Automatic decryption on retrieval

- [ ] **Key Rotation**
  - [ ] KeyRotationService checks yearly (365 days)
  - [ ] Monthly automation (PeriodicTimer)
  - [ ] Old key stored for decryption of old ciphertexts
  - [ ] No service downtime during rotation

- [ ] **Testing**
  - [ ] Encryption/decryption round-trip
  - [ ] Different IVs produce different ciphertexts
  - [ ] EF Core integration: Save encrypted, load decrypted
  - [ ] Key rotation: Old and new keys work correctly

- [ ] **Regulatory Compliance**
  - [ ] GDPR Art. 32(1)(b): Encryption ✅
  - [ ] NIS2 Art. 21(4): Key management ✅

**Owner:** Security Engineer  
**Deadline:** Week 2-3

---

### P0.3: Incident Response (< 24h Notification) ✅

**[✅ IN PROGRESS]** - Status: Detection rules and NIS2 service implemented

- [ ] **Incident Detection**
  - [ ] Brute force detection (5+ failed logins in 10 min)
  - [ ] Data exfiltration detection (3x normal download volume)
  - [ ] Service availability detection (> 5 min down)
  - [ ] Performance degradation detection (2x baseline latency)

- [ ] **SecurityIncident Entity**
  - [ ] Type, Severity, Description, DetectedAt
  - [ ] TenantId (tenant isolation)
  - [ ] Context (JSON for incident details)

- [ ] **NIS2 Notification Service**
  - [ ] Authority lookup by country (DE → BSI, AT → NICS, FR → CERT)
  - [ ] Notification sent < 24h after detection
  - [ ] Reference number generated
  - [ ] Documentation created

- [ ] **Testing**
  - [ ] Brute force rule triggers correctly
  - [ ] Data exfiltration detection works
  - [ ] Authority lookup returns correct email
  - [ ] Notification sent within 24h timer

- [ ] **Regulatory Compliance**
  - [ ] NIS2 Art. 21: Detect incidents ✅
  - [ ] NIS2 Art. 23: Notify within 24h ✅

**Owner:** Security Engineer + DevOps  
**Deadline:** Week 4-5

---

### P0.4: Network Segmentation ✅

**[✅ IN PROGRESS]** - Status: VPC and security groups configured

- [ ] **VPC Architecture**
  - [ ] CIDR: 10.0.0.0/16
  - [ ] Public subnet: 10.0.1.0/24 (Load Balancer only)
  - [ ] Private Services: 10.0.2.0/24 (Microservices)
  - [ ] Private Databases: 10.0.3.0/24 (PostgreSQL, Redis, ES)

- [ ] **Security Groups**
  - [ ] ALB: Inbound HTTPS/HTTP from 0.0.0.0/0
  - [ ] Services: Inbound only from ALB (port 8080)
  - [ ] Databases: Inbound only from Services (no outbound)
  - [ ] Principle of least privilege verified

- [ ] **DDoS & WAF Protection**
  - [ ] AWS Shield enabled (automatic)
  - [ ] WAF rules deployed (SQL injection, XSS, large body, geo-blocking)
  - [ ] Rate limiting: 2000 req/5min per IP
  - [ ] Geo-blocking: Non-EU countries blocked

- [ ] **mTLS Between Services**
  - [ ] Service-to-service communication encrypted
  - [ ] Client certificate validation
  - [ ] No plaintext inter-service traffic

- [ ] **Testing**
  - [ ] Database not reachable from public internet
  - [ ] Services not reachable except through ALB
  - [ ] WAF blocks SQL injection patterns
  - [ ] Rate limiting enforced

- [ ] **Regulatory Compliance**
  - [ ] NIS2 Art. 21(3): Network segmentation ✅

**Owner:** DevOps Engineer  
**Deadline:** Week 5-6

---

### P0.5: Key Management (Azure KeyVault) ✅

**[✅ IN PROGRESS]** - Status: KeyVault configured and secrets migrated

- [ ] **Azure KeyVault Setup**
  - [ ] KeyVault provisioned (Vault name, location)
  - [ ] Access policies configured per service
  - [ ] Audit logging enabled (who accessed what key)

- [ ] **Secrets Migrated**
  - [ ] Encryption keys (for P0.2)
  - [ ] Database connection string
  - [ ] JWT secret
  - [ ] API keys (Stripe, etc.)
  - [ ] Certificate passwords

- [ ] **No Hardcoded Secrets**
  - [ ] Code review: Zero hardcoded secrets found
  - [ ] Configuration: All secrets from KeyVault/environment
  - [ ] Local dev: User Secrets only (never committed)

- [ ] **Key Rotation Automation**
  - [ ] Annual rotation policy documented
  - [ ] Rotation trigger configured (365 days)
  - [ ] No service downtime during rotation

- [ ] **Testing**
  - [ ] Secret retrieval works (KeyVault → App)
  - [ ] Access denied for unauthorized service
  - [ ] Audit logs show secret access

- [ ] **Regulatory Compliance**
  - [ ] GDPR Art. 32(1)(a): Key management ✅
  - [ ] NIS2 Art. 21(4): Secure key storage ✅

**Owner:** DevOps Engineer  
**Deadline:** Week 6-7

---

## Phase 1: MVP with Compliance Integration

### All Features: Compliance Integration Template

**[TO START]** - Use this for EVERY feature in Phase 1

- [ ] **Acceptance Criteria Defined**
  - [ ] Regulatory requirement identified (if any)
  - [ ] Business requirement clear
  - [ ] Technical acceptance criteria listed

- [ ] **Audit Logging Integrated**
  - [ ] All CRUD operations logged (via P0.1 interceptor)
  - [ ] AuditLog entry created automatically
  - [ ] Tenant isolation verified

- [ ] **Encryption Integrated (if PII)**
  - [ ] PII fields identified
  - [ ] EF Core value converters configured
  - [ ] Encryption/decryption round-trip tested

- [ ] **Input Validation (FluentValidation)**
  - [ ] Validator created for command
  - [ ] All required fields validated
  - [ ] Length limits enforced
  - [ ] Pattern matching (email, phone, etc.)

- [ ] **Error Handling**
  - [ ] No stack traces in API responses
  - [ ] User-friendly error messages
  - [ ] Validation errors clearly described

- [ ] **Testing**
  - [ ] Unit tests: Business logic
  - [ ] Integration tests: DB operations
  - [ ] Authorization tests: Tenant isolation
  - [ ] Coverage: >= 80%

- [ ] **Documentation**
  - [ ] API endpoint documented
  - [ ] Request/response examples
  - [ ] Error codes documented
  - [ ] Code comments for complex logic

---

## Phase 1 Feature: F1.1 Multi-Tenant Authentication

**[TO START]** - Week 7-8

- [ ] **JWT Token Generation**
  - [ ] 1h access token expiration
  - [ ] 7d refresh token expiration
  - [ ] Claims: userId, tenantId, roles
  - [ ] Signature validation (HMAC-SHA256)

- [ ] **Compliance: Audit Logging**
  - [ ] Login attempt logged (success/failure)
  - [ ] Token generation logged
  - [ ] Failed login from same IP tracked

- [ ] **Compliance: Suspicious Activity Detection**
  - [ ] 5+ failed logins from same IP → 10min lockout
  - [ ] Password change → invalidate all sessions
  - [ ] Session timeout: 15min inactivity (configurable)

- [ ] **Compliance: Password Policy**
  - [ ] Minimum 12 characters (configurable)
  - [ ] Uppercase + lowercase + numbers + symbols
  - [ ] Hashing: Argon2 (not MD5/SHA1)

- [ ] **Compliance: Email Verification**
  - [ ] Email verification link sent on registration
  - [ ] Token expires in 24 hours
  - [ ] Account disabled until verified

- [ ] **Testing**
  - [ ] Token generation: 10+ test cases
  - [ ] Token validation: 5+ test cases
  - [ ] Failed login lockout: 3+ test cases
  - [ ] Session timeout: 2+ test cases

**Acceptance Gate:** All tests passing, audit logs working, password policy enforced

---

## Phase 1 Feature: F1.2 Product Catalog

**[TO START]** - Week 9-10

- [ ] **CRUD Operations**
  - [ ] Create product: SKU, Name, Description, Price
  - [ ] Read: Filter by category, search by SKU
  - [ ] Update: Name, Price, Description
  - [ ] Delete: Soft delete (IsDeleted flag)

- [ ] **Compliance: Audit Logging**
  - [ ] Every create/update/delete logged
  - [ ] Before/after values captured in JSON
  - [ ] User and timestamp recorded

- [ ] **Compliance: Encryption**
  - [ ] Supplier names encrypted
  - [ ] Cost prices encrypted
  - [ ] Sensitive descriptions encrypted

- [ ] **Compliance: Tenant Isolation**
  - [ ] Product visible only to own tenant
  - [ ] Query always filters by TenantId
  - [ ] No cross-tenant leaks possible

- [ ] **Performance: Caching**
  - [ ] Redis cache: 5-min TTL for listings
  - [ ] Cache invalidation on product change
  - [ ] Measure: Cache hit rate > 80%

- [ ] **Testing**
  - [ ] CRUD: 15+ test cases
  - [ ] Encryption round-trip: 3+ test cases
  - [ ] Tenant isolation: 3+ test cases
  - [ ] Cache hit: 2+ test cases

**Acceptance Gate:** All tests passing, no cross-tenant leaks, audit logs working

---

## P0.6: E-Commerce Legal Compliance (B2C & B2B)

**[TO START]** - Week 11-12

- [ ] **B2C Legal Requirements**
  - [ ] 14-day withdrawal right (VVVG §357)
  - [ ] Price display: Always "incl. MwSt"
  - [ ] Shipping cost visible before checkout
  - [ ] AGB checkbox before order
  - [ ] Privacy statement linked
  - [ ] Impressum displayed

- [ ] **B2B Legal Requirements**
  - [ ] VAT-ID validation (VIES API)
  - [ ] Reverse charge (no VAT if valid VAT-ID)
  - [ ] Payment terms configurable (Net 30, Net 60)
  - [ ] INCOTERMS support (DDP, DDU, CIF)

- [ ] **Invoice Management**
  - [ ] Invoice generation (PDF)
  - [ ] 10-year archival (immutable, encrypted)
  - [ ] Unique invoice number per shop
  - [ ] Correct VAT calculation

- [ ] **Acceptance Criteria (B2C)**
  - [ ] [ ] Withdrawal form available
  - [ ] [ ] 14-day counter working
  - [ ] [ ] Return label generation working
  - [ ] [ ] Refund within 14 days (automated)
  - [ ] [ ] Audit log for each withdrawal

- [ ] **Acceptance Criteria (B2B)**
  - [ ] VAT-ID validation working
  - [ ] Reverse charge applied correctly
  - [ ] Invoices in UBL/ZUGFeRD format
  - [ ] Payment terms honored

- [ ] **Testing**
  - [ ] 15+ test cases covering legal requirements
  - [ ] Manual test: Withdrawal within 14 days
  - [ ] Manual test: Withdrawal after 14 days (error)
  - [ ] Manual test: B2B VAT-ID validation

- [ ] **Regulatory Compliance**
  - [ ] VVVG §357: 14-day withdrawal ✅
  - [ ] PAngV: Price transparency ✅
  - [ ] AStV: VAT-ID validation ✅
  - [ ] TMG: Impressum + Privacy ✅

**Acceptance Gate:** Legal review passed, all tests passing, manual testing complete

---

## P0.7: AI Act Compliance

**[TO START]** - Week 13-14

**Only required if using AI systems (fraud detection, recommendations, etc.)**

- [ ] **AI Risk Register**
  - [ ] All AI systems documented
  - [ ] Risk classification (HIGH, LIMITED, MINIMAL)
  - [ ] Responsible person assigned per system
  - [ ] Limitations documented

- [ ] **High-Risk AI (if applicable)**
  - [ ] Training data documented
  - [ ] Validation results (accuracy, precision)
  - [ ] Bias testing framework
  - [ ] Performance monitoring job (monthly)
  - [ ] User explanation API (Art. 22)

- [ ] **Bias Testing**
  - [ ] Test across demographic groups (gender, age, region)
  - [ ] Acceptance rate disparity < 5%
  - [ ] Alert if drift detected

- [ ] **Decision Logging**
  - [ ] Every AI decision logged (for transparency)
  - [ ] Confidence score recorded
  - [ ] Human override tracking
  - [ ] User can request explanation

- [ ] **Testing**
  - [ ] 15+ test cases covering AI risk scenarios
  - [ ] Bias testing framework validated
  - [ ] Performance monitoring triggers correctly

- [ ] **Regulatory Compliance**
  - [ ] EU AI Act Art. 6: Risk classification ✅
  - [ ] EU AI Act Art. 22: Responsible person ✅
  - [ ] EU AI Act Art. 35: Audit trail ✅

**Acceptance Gate:** Risk register complete, bias tests passing, decision logging working

---

## P0.8: Accessibility (WCAG 2.1 Level AA) ⚠️ CRITICAL

**[TO START]** - Week 15-16 | **Deadline: 28. Juni 2025!**

- [ ] **Keyboard Navigation**
  - [ ] All interactive elements focusable (TAB)
  - [ ] Focus order matches visual order
  - [ ] No keyboard traps
  - [ ] Escape closes modals

- [ ] **Screen Reader Support**
  - [ ] All images have alt-text
  - [ ] Form fields have labels
  - [ ] Headings in hierarchy (H1 → H2 → H3)
  - [ ] Error messages use role="alert"

- [ ] **Color & Contrast**
  - [ ] Text contrast >= 4.5:1 (WCAG AA)
  - [ ] Large text >= 3:1
  - [ ] Not relying on color alone

- [ ] **Responsive & Resizable**
  - [ ] Responsive design (320px - 1920px)
  - [ ] Text resizable to 200% without breaking
  - [ ] No horizontal scrolling at 200%

- [ ] **Media**
  - [ ] Videos have captions (DE + EN)
  - [ ] Audio has transcript

- [ ] **Testing**
  - [ ] Lighthouse Accessibility >= 90
  - [ ] axe DevTools: 0 critical issues
  - [ ] NVDA/VoiceOver manual test
  - [ ] Keyboard-only navigation test
  - [ ] 12+ accessibility test cases

- [ ] **Regulatory Compliance**
  - [ ] WCAG 2.1 Level AA ✅
  - [ ] BFSG (Barrierefreiheitsgesetz) ✅
  - [ ] Legal deadline: 28. Juni 2025 ✅

**Acceptance Gate:** Lighthouse >= 90, axe 0 critical, manual testing passed

---

## P0.9: E-Rechnung (ZUGFeRD & UBL)

**[TO START]** - Week 17-18

- [ ] **ZUGFeRD 3.0 Format**
  - [ ] XML generation per EN 16931
  - [ ] Hybrid PDF (ZUGFeRD + visual)
  - [ ] Schema validation: 0 errors

- [ ] **UBL 2.3 Alternative**
  - [ ] UBL format supported
  - [ ] XML generation
  - [ ] Compatibility with ERP systems

- [ ] **Invoice Archival**
  - [ ] 10-year storage (immutable)
  - [ ] Encrypted at rest (AES-256)
  - [ ] Queryable by invoice number

- [ ] **Digital Signatures**
  - [ ] XAdES signature (for legal validity)
  - [ ] Signature verification working
  - [ ] Non-repudiation

- [ ] **Testing**
  - [ ] 10+ test cases
  - [ ] ZUGFeRD schema validates
  - [ ] UBL format valid
  - [ ] Signature verification works

- [ ] **Regulatory Compliance**
  - [ ] ERechnungsverordnung §4 ✅
  - [ ] EN 16931 standard ✅

**Acceptance Gate:** ZUGFeRD schema validates, all tests passing, archival working

---

## Pre-Production Gate (Week 18)

**Before ANY feature goes to production:**

```
Phase 0 Compliance Foundation:
├─ [ ] P0.1: Audit Logging ✅
├─ [ ] P0.2: Encryption ✅
├─ [ ] P0.3: Incident Response ✅
├─ [ ] P0.4: Network Segmentation ✅
└─ [ ] P0.5: Key Management ✅

Phase 1 MVP Features:
├─ [ ] F1.1: Authentication ✅
├─ [ ] F1.2: Catalog ✅
├─ [ ] F1.3: Checkout ✅
└─ [ ] F1.4: Admin Dashboard ✅

Compliance Integration:
├─ [ ] P0.6: E-Commerce Legal ✅
├─ [ ] P0.7: AI Act ✅
├─ [ ] P0.8: BITV Accessibility ✅
└─ [ ] P0.9: E-Rechnung ✅

Testing Gate:
├─ [ ] 52+ compliance tests passing
├─ [ ] Coverage >= 80%
├─ [ ] 0 critical bugs
└─ [ ] Security review approved

IF ALL ✅: APPROVED FOR PRODUCTION
IF ANY ❌: HOLD DEPLOYMENT
```

---

**Document Owner:** Compliance & QA Team  
**Last Updated:** 28. Dezember 2025  
**Next Review:** 15. Januar 2026
