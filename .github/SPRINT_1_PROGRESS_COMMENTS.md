# Sprint 1 - Issue Progress & Comments

**Date**: 28. Dezember 2025  
**Sprint Status**: ✅ PLANNED (starts 02.01.2026)  
**Team**: 9 Developer assigned  
**Total Issues**: 14 in Sprint 1  

---

## 📌 Issue Comments & Progress

### 🔴 CRITICAL P0 Issues

#### Issue #4 - Epic: Customer Registration Flow
```
Status: 📋 PLANNING (Architecture Phase)
Assignee: @HRasch (Tech Lead)
Priority: 🔴 P0
Sprint: Sprint 1 (28.12.2025 - 24.01.2026)
Started: 28.12.2025

COMMENT (28.12.2025 @ 14:30):
─────────────────────────────────────
@HRasch: Epic Kickoff started! 🚀

Architecture design in progress for:
  ✅ Wolverine HTTP handlers (NOT MediatR!)
  ✅ JWT token generation (1h access, 7d refresh)
  ✅ Multi-tenant isolation (@MaxMueller handling)
  ✅ Email verification flow
  ✅ Password security (Argon2)

Linked Stories:
  - #5: User Registration Handler
  - #6: Email Verification Logic
  - #7: JWT Token Generation
  - #9: Multi-Tenant Context
  - #10: Password Policy
  - #11: Failed Login Lockout
  - #12: Session Timeout
  - #41: AGB Acceptance (Legal)
  - #42: Privacy Policy (Legal)
  - #19: Vue Components

Architecture Review scheduled: 02.01.2026 09:00 CET
Target Completion: 17.01.2026

---

#### Issue #30 - VAT-ID Validation (VIES API)
```
Status: 🔄 IN PROGRESS (Architecture)
Assignee: @HRasch (Implementation) + @DavidKeller (Security)
Priority: 🔴 P0 (Blocking #20, #31)
Sprint: Sprint 1
Started: 28.12.2025

COMMENT (28.12.2025 @ 15:00):
─────────────────────────────────────
@HRasch: VAT-ID validation architecture started.

Current work:
  📝 VIES API integration design (EU official database)
  🔐 Security review checklist prepared
  ✅ Reverse Charge logic dependencies mapped
  ⚠️  Rate limiting strategy (VIES has limits)

Blockers: None
Blocks: #31 (Reverse Charge), #20 (Price Calc)

Next Steps:
  1. VIES API documentation review (02.01)
  2. Mock implementation for testing (02.01)
  3. Integration with #31 (05.01)
  4. Security review @DavidKeller (08.01)

Test Strategy:
  - Valid VAT-ID (Germany, Austria, France)
  - Invalid VAT-ID rejection
  - VIES API timeout handling
  - Duplicate validation detection

Estimated Complete: 10.01.2026
```

---

#### Issue #31 - Reverse Charge Logic (B2B)
```
Status: 📋 READY FOR SPRINT
Assignee: @HRasch
Priority: 🔴 P0
Dependencies: #30 (VAT-ID validation)
Sprint: Sprint 1

COMMENT (28.12.2025 @ 15:15):
─────────────────────────────────────
@HRasch: Reverse Charge logic blocked by #30 VAT-ID validation.

Business Rule:
  IF order is B2B
  AND buyer has valid VAT-ID
  AND buyer is different EU country
  THEN no VAT charged (seller reports as reverse charge)

Test Cases:
  ✅ B2C: VAT always applied (19% DE, 20% AT, etc.)
  ✅ B2B Germany: No VAT (domestic reverse charge)
  ✅ B2B Austria buyer from DE: No VAT (cross-border)
  ✅ B2B invalid VAT-ID: VAT applied (fallback)

Depends on: #30 (VIES validation must work first)
Ready to start: 10.01.2026 (after #30)
Est. Duration: 3-5 days
```

---

### 👤 Registration Flow Issues

#### Issue #5 - User Registration Handler
```
Status: 📋 READY FOR SPRINT
Assignee: @MaxMueller
Priority: 🔴 P0
Sprint: Sprint 1
Start Date: 02.01.2026

COMMENT (28.12.2025 @ 14:45):
─────────────────────────────────────
@MaxMueller: Registration handler assigned for Sprint 1! 🎯

Core Requirements:
  ✅ Wolverine HTTP Handler (POST /registeruser)
  ✅ Plain POCO command (RegisterUserCommand)
  ✅ No MediatR patterns!
  ✅ FluentValidation for input
  ✅ Multi-tenant context extraction
  ✅ Duplicate detection service integration

Architecture Pattern (Wolverine, NOT MediatR):
  
  // Step 1: Command (Plain POCO)
  public class RegisterUserCommand {
    public string Email { get; set; }
    public string Password { get; set; }
    public Guid TenantId { get; set; }
  }
  
  // Step 2: Handler Service
  public class RegistrationService {
    public async Task<RegisterUserResponse> RegisterUser(
      RegisterUserCommand request,
      CancellationToken ct) { }
  }
  
  // Step 3: DI Registration
  builder.Services.AddScoped<RegistrationService>();
  
  // Result: Wolverine auto-discovers endpoint
  // POST /registeruser

Reference Implementation:
  ↳ backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs

Dependencies:
  - #9: Multi-tenant context (will work in parallel)
  - Database schema ready

Start: 02.01.2026
Est. Duration: 3-4 days
Blocks: #6, #7, #9, #12
```

---

#### Issue #6 - Email Verification Logic
```
Status: 📋 READY FOR SPRINT
Assignee: @MaxMueller
Priority: 🔴 P0
Dependencies: #5 (User Registration)
Sprint: Sprint 1

COMMENT (28.12.2025 @ 14:50):
─────────────────────────────────────
@MaxMueller: Email verification flow assigned.

Features:
  ✅ Async/await for email sending
  ✅ Verification token (random, expires 24h)
  ✅ Hashed token storage (never plain)
  ✅ Email delivery confirmation logging
  ✅ Retry logic (5 attempts, exponential backoff)
  ✅ Audit trail for security

Database Schema:
  - EmailVerificationToken table
  - Token, ExpiresAt, IsUsed flags
  - UserEmail foreign key to User table

Start: 04.01.2026 (after #5 completion)
Est. Duration: 2-3 days
Test Strategy:
  - Valid email flow
  - Expired token rejection
  - Already verified skip
  - Email delivery failure handling
```

---

### 💰 Pricing & Invoicing Issues

#### Issue #20 - Price Calculation Service (VAT)
```
Status: 📋 READY FOR SPRINT
Assignee: @LisaSchmidt
Priority: 🔴 P0
Dependencies: #30 (VAT-ID), #31 (Reverse Charge)
Sprint: Sprint 1

COMMENT (28.12.2025 @ 15:05):
─────────────────────────────────────
@LisaSchmidt: Price calculation service ready!

Formula:
  B2C: GrossPrice = NetPrice × (1 + VatRate)
       GrossPrice = 100 × 1.19 = 119€
  
  B2B (with valid VAT-ID): GrossPrice = NetPrice (no VAT)
       GrossPrice = 100€

Country-Specific VAT Rates:
  - Germany: 19% (standard), 7% (reduced)
  - Austria: 20% (standard), 10% (reduced)
  - France: 20% (standard), 5.5% (reduced)
  - etc.

Shipping Cost:
  - Added AFTER tax calculation
  - Separate tax handling if needed
  - Includes by default (included in GrossPrice)

Depends on:
  - #30: VIES VAT-ID validation
  - #31: Reverse charge logic

Start: 05.01.2026 (after #30, #31 architecture)
Est. Duration: 4-5 days
Test Cases: 15+ (see #45 test framework)
```

---

#### Issue #29 - Invoice Generation & 10-Year Archival
```
Status: 📋 READY FOR SPRINT
Assignee: @LisaSchmidt
Co-Review: @JuliaHoffmann (Legal)
Priority: 🔴 P0
Dependencies: #20 (Price Calc), #32 (Encryption)
Sprint: Sprint 1-2

COMMENT (28.12.2025 @ 15:20):
─────────────────────────────────────
@LisaSchmidt + @JuliaHoffmann: Invoice generation assigned!

German Legal Requirements (10-year archival):
  ✅ Invoice number: Unique per shop
  ✅ Seller information: Company name, address, VAT-ID
  ✅ Buyer information: Name, address, email
  ✅ Invoice date: Issue date (not delivery date)
  ✅ Item description: Product name, SKU, quantity, price
  ✅ VAT: Amount shown separately (or "reverse charge")
  ✅ Payment terms: Net 30, Net 60, etc.
  ✅ Digital signature: Optional for B2C, required for B2G

Format:
  - PDF (primary): Human-readable
  - XML (ZUGFeRD 2.0): Machine-readable (for B2G)
  - UBL 2.3: Alternative format (for EU)

Storage:
  - Encrypted (AES-256) #32
  - Immutable (append-only)
  - 10-year retention
  - Searchable by invoice number

Start: 08.01.2026 (after #20, parallel with #32)
Est. Duration: 5-7 days
Legal Review: Required before deployment
```

---

### 📖 Legal & Compliance Issues

#### Issue #41 - AGB & Widerrufsbelehrung
```
Status: 📋 READY FOR SPRINT
Assignee: @AnnaWeber (Frontend) + @JuliaHoffmann (Content)
Priority: 🔴 P0
Sprint: Sprint 1

COMMENT (28.12.2025 @ 15:30):
─────────────────────────────────────
@AnnaWeber + @JuliaHoffmann: Legal terms assigned!

Content Requirement (German Law):
  ✅ Terms & Conditions (AGB)
  ✅ 14-day withdrawal notice (VVVG)
  ✅ Return process description
  ✅ Shipping cost responsibility
  ✅ Refund timeline

UI Requirements:
  ✅ Checkbox before order (mandatory)
  ✅ Link to full text (footer)
  ✅ Readable font (14px minimum)
  ✅ Clear language (not legal-ese)
  ✅ Accessible (WCAG AA)

Start: 02.01.2026 (parallel work)
Frontend UI: @AnnaWeber (3 days)
Legal Content: @JuliaHoffmann (2 days)
Legal Review: 08.01.2026 (before merge)
```

---

#### Issue #42 - Datenschutzerklärung & Impressum
```
Status: 📋 READY FOR SPRINT
Assignee: @AnnaWeber (Frontend) + @JuliaHoffmann (Content)
Priority: 🔴 P0
Dependencies: #41 (structure)
Sprint: Sprint 1

COMMENT (28.12.2025 @ 15:35):
─────────────────────────────────────
@AnnaWeber + @JuliaHoffmann: Privacy policy assigned!

Content Requirement (GDPR + German Law):
  ✅ Privacy statement (DSGVO)
  ✅ Data processing (what we collect)
  ✅ Rights (access, delete, export)
  ✅ Cookies (tracking disclosure)
  ✅ Contact information (DPO, support)

Impressum Content (TMG §5):
  ✅ Company name, address, phone
  ✅ Managing director / responsible person
  ✅ VAT-ID, trade register number
  ✅ Contact email
  ✅ Responsible for content

UI Requirements:
  ✅ Footer links
  ✅ Accessible (WCAG AA)
  ✅ No login required

Start: 04.01.2026 (after #41)
Frontend UI: @AnnaWeber (2 days)
Legal Content: @JuliaHoffmann (3 days)
Legal Review: 10.01.2026
```

---

### 🧪 Testing & QA

#### Issue #45 - E-Commerce Legal Tests (15 tests)
```
Status: 📋 READY FOR SPRINT
Assignee: @ThomasKrause
Priority: 🔴 P0
Sprint: Sprint 1-2
Start: 04.01.2026 (after features)

COMMENT (28.12.2025 @ 15:40):
─────────────────────────────────────
@ThomasKrause: Test framework assigned!

15 E-Commerce Compliance Tests:
  1. VAT Calculation B2C (19% included)
  2. VAT Display (correct format)
  3. Reverse Charge B2B (valid VAT-ID)
  4. Reverse Charge Rejection (invalid VAT-ID)
  5. 14-Day Return Process
  6. Return Shipping Costs
  7. Refund Timeline (max 30 days)
  8. Terms & Conditions Acceptance (checkbox)
  9. Impressum Link (footer visible)
  10. Privacy Policy Link (footer visible)
  11. Withdrawal Rights Display
  12. Product Descriptions (accurate)
  13. Price Transparency (no hidden costs)
  14. Dispute Resolution (contact provided)
  15. Invoice Generation & Delivery

Test Framework:
  - xUnit (.NET 10)
  - FluentAssertions
  - TestContainers (PostgreSQL)
  - Spectre.Console (reporting)

Start: 04.01.2026
Est. Duration: 2-3 weeks
Coverage Target: 100% of P0.6 requirements
```

---

## 📊 Sprint Status Summary

| Phase | Status | Progress |
|-------|--------|----------|
| **Planning** | ✅ COMPLETE | 14 issues assigned, team ready |
| **Architecture** | 🔄 IN PROGRESS | Epic #4, #30, #31 analysis |
| **Development** | ⏳ STARTS 02.01 | All developers ready |
| **Testing** | ⏳ STARTS 04.01 | Test framework prepared |
| **Review** | ⏳ STARTS 08.01 | Code review gates ready |
| **Deployment** | ⏳ WEEK 4 | Go/No-Go 24.01.2026 |

---

## 🎯 Next Steps (Timeline)

```
28.12.2025 (Today)
  ✅ Sprint planning complete
  ✅ Issues assigned to team
  ✅ Progress comments added
  → All team members notified via GitHub

29-31.12.2025 (Holiday)
  → Minimal activity (skeleton crew if needed)
  → Final prep for development start

02.01.2026 (Development Starts)
  ✅ Daily standup 09:00 CET
  ✅ @HRasch: Architecture review Epic #4
  ✅ @MaxMueller: #5 Implementation
  ✅ @LisaSchmidt: #20, #29 Analysis
  ✅ @AnnaWeber: #41, #42 UI Layout
  
08.01.2026 (First Review)
  → Code reviews begin
  → Security review checkpoint
  → Legal review checkpoint
  
15.01.2026 (Mid-Sprint)
  → Testing framework active
  → 40+ story points completed
  → Burn-down verification
  
24.01.2026 (Sprint End)
  → Sprint review + retrospective
  → Go/No-Go decision for Phase 1
  → Release preparation
```

---

## 📞 Questions & Support

**Blocked on something?** Comment in the issue with @mention
**Need to escalate?** Report in daily standup
**Architecture question?** Ask @HRasch
**Legal question?** Ask @JuliaHoffmann
**Security concern?** Ask @DavidKeller

---

**Last Updated**: 28. Dezember 2025 @ 16:00 CET  
**Board Owner**: @HRasch  
**Next Review**: 02. Januar 2026 (Sprint Start)

