# GitHub CLI Commands - Sprint 1 Setup

**Purpose**: Assign issues to fiktive Team Members  
**Date**: 28. Dezember 2025  
**Team**: 9 Developer  
**Total Issues**: 14 Sprint 1  

---

## 🚀 Quick Execute (Copy-Paste Ready)

```bash
#!/bin/bash
# Sprint 1 Issue Assignments (28.12.2025)
# Run in: /Users/holger/Documents/Projekte/B2Connect

# Ensure you're logged in
# gh auth login

cd /Users/holger/Documents/Projekte/B2Connect

# ============================================================
# EPIC #4 - Tech Lead (HRasch)
# ============================================================
gh issue edit 4 --assignee "HRasch" --add-label "sprint-1,epic,in-progress" --add-label "p0-critical"
gh issue comment 4 --body "Epic Kickoff started! 🚀 Architecture design in progress for Wolverine HTTP handlers, JWT token generation, and multi-tenant isolation. Assigned to @HRasch as Tech Lead. Architecture Review scheduled: 02.01.2026 09:00 CET"

# ============================================================
# BACKEND TEAM - Registration Flow (MaxMueller)
# ============================================================

# Issue #5 - User Registration Handler
gh issue edit 5 --assignee "MaxMueller" --add-label "sprint-1,backend,p0-critical"
gh issue comment 5 --body "@MaxMueller: Registration handler assigned for Sprint 1! 🎯 Core Requirements: Wolverine HTTP Handler, FluentValidation, Multi-tenant context, Duplicate detection integration. Start: 02.01.2026 Duration: 3-4 days. Reference: backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs. Blocks: #6, #7, #9, #12"

# Issue #6 - Email Verification Logic
gh issue edit 6 --assignee "MaxMueller" --add-label "sprint-1,backend,p0-critical" --add-label "depends-on:#5"
gh issue comment 6 --body "@MaxMueller: Email verification flow assigned. Features: Async email sending, verification token (expires 24h), hashed storage, retry logic (5 attempts). Depends on: #5. Start: 04.01.2026 Duration: 2-3 days"

# Issue #7 - JWT Token Generation
gh issue edit 7 --assignee "MaxMueller" --add-label "sprint-1,backend,p0-critical,security" --add-label "depends-on:#5"
gh issue comment 7 --body "@MaxMueller: JWT token generation assigned. 1h access token, 7-day refresh, secure claims structure. Start: 04.01.2026 Duration: 3-4 days. Blocks: #12"

# Issue #9 - Multi-Tenant Isolation
gh issue edit 9 --assignee "MaxMueller" --add-label "sprint-1,backend,p0-critical,security" --add-label "depends-on:#5"
gh issue comment 9 --body "@MaxMueller: Multi-tenant isolation assigned. X-Tenant-ID header validation, context propagation. CRITICAL for preventing cross-tenant data leaks. Depends on: #5. Start: 04.01.2026"

# Issue #10 - Password Policy
gh issue edit 10 --assignee "MaxMueller" --add-label "sprint-1,backend,p1-high,security" --add-label "depends-on:#5"
gh issue comment 10 --body "@MaxMueller: Password policy enforcement. Requirements: 12+ chars, uppercase, numbers, symbols, Argon2 hashing. Depends on: #5. Start: 06.01.2026"

# Issue #11 - Failed Login Lockout
gh issue edit 11 --assignee "MaxMueller" --add-label "sprint-1,backend,p1-high,security" --add-label "depends-on:#5"
gh issue comment 11 --body "@MaxMueller: Failed login lockout assigned. 5+ failed logins → 10min lockout per IP. Start: 06.01.2026. Depends on: #5"

# Issue #12 - Session Timeout
gh issue edit 12 --assignee "MaxMueller" --add-label "sprint-1,backend,p1-high" --add-label "depends-on:#7"
gh issue comment 12 --body "@MaxMueller: Session timeout enforcement. 15min inactivity → auto-logout. Depends on: #7. Start: 08.01.2026"

# ============================================================
# BACKEND TEAM - Pricing & Invoicing (LisaSchmidt)
# ============================================================

# Issue #20 - Price Calculation
gh issue edit 20 --assignee "LisaSchmidt" --add-label "sprint-1,backend,p0-critical" --add-label "depends-on:#30,#31"
gh issue comment 20 --body "@LisaSchmidt: Price calculation service ready! B2C: GrossPrice = NetPrice × (1 + VatRate). B2B (valid VAT-ID): GrossPrice = NetPrice. Depends on: #30, #31. Start: 05.01.2026. Blocks: #21, #40"

# Issue #21 - Shipping Cost
gh issue edit 21 --assignee "LisaSchmidt" --add-label "sprint-1,backend,p0-critical" --add-label "depends-on:#20"
gh issue comment 21 --body "@LisaSchmidt: Shipping cost calculation. Added AFTER tax. Country-specific rates. Depends on: #20. Start: 08.01.2026"

# Issue #29 - Invoice Generation
gh issue edit 29 --assignee "LisaSchmidt" --add-label "sprint-1,backend,p0-critical,legal" --add-label "depends-on:#20,#32"
gh issue comment 29 --body "@LisaSchmidt (+ @JuliaHoffmann legal review): Invoice generation assigned. PDF + XML (ZUGFeRD), encrypted storage (AES-256), 10-year archival per German law. Depends on: #20, #32. Legal Review required before merge. Start: 08.01.2026"

# Issue #27 - Return Label Generation
gh issue edit 27 --assignee "LisaSchmidt" --add-label "sprint-1,backend,p1-high" --add-label "depends-on:#20"
gh issue comment 27 --body "@LisaSchmidt: Return label generation. Shipping carrier integration. Depends on: #20. Start: 12.01.2026"

# ============================================================
# SECURITY TEAM - VAT & Encryption (HRasch + DavidKeller)
# ============================================================

# Issue #30 - VAT-ID Validation
gh issue edit 30 --assignee "HRasch" --assignee "DavidKeller" --add-label "sprint-1,security,p0-critical,vat" --add-label "blocking:#31,#20"
gh issue comment 30 --body "@HRasch + @DavidKeller: VAT-ID validation (VIES API) started! 🔐 VIES API integration for B2B validation. Security review required. Blocks: #31 (Reverse Charge), #20 (Price Calc). Started: 28.12.2025. Security review: 08.01.2026. Test cases: Valid/Invalid VAT-IDs, API timeout, duplicates"

# Issue #31 - Reverse Charge Logic
gh issue edit 31 --assignee "HRasch" --add-label "sprint-1,backend,p0-critical,vat" --add-label "depends-on:#30,blocking:#20"
gh issue comment 31 --body "@HRasch: Reverse Charge logic blocked by #30. Business Rule: IF B2B + valid VAT-ID + different EU country THEN no VAT. Depends on: #30. Ready to start: 10.01.2026"

# Issue #32 - Invoice Encryption
gh issue edit 32 --assignee "DavidKeller" --add-label "sprint-2,security,p0-critical,encryption" --add-label "depends-on:#29"
gh issue comment 32 --body "@DavidKeller: Invoice encryption assigned. AES-256-GCM, random IV, key rotation policy (annual). Depends on: #29. Start: 08.01.2026 (parallel with #29)"

# ============================================================
# FRONTEND TEAM - Legal UI (AnnaWeber)
# ============================================================

# Issue #41 - AGB & Terms
gh issue edit 41 --assignee "AnnaWeber" --assignee "JuliaHoffmann" --add-label "sprint-1,frontend,p0-critical,legal"
gh issue comment 41 --body "@AnnaWeber (frontend) + @JuliaHoffmann (content): Legal terms assigned! UI: Checkbox before order, link to full text. Content: 14-day withdrawal (VVVG), return process. Legal review required before merge. Frontend start: 02.01.2026. Legal review: 08.01.2026"

# Issue #42 - Privacy & Impressum
gh issue edit 42 --assignee "AnnaWeber" --assignee "JuliaHoffmann" --add-label "sprint-1,frontend,p0-critical,legal" --add-label "depends-on:#41"
gh issue comment 42 --body "@AnnaWeber + @JuliaHoffmann: Privacy policy assigned. Content: GDPR compliance, Impressum (TMG §5), cookie policy. Depends on: #41. Frontend start: 04.01.2026. Legal review: 10.01.2026"

# Issue #19 - Vue Components (Base)
gh issue edit 19 --assignee "AnnaWeber" --add-label "sprint-1,frontend,p1-high,components"
gh issue comment 19 --body "@AnnaWeber: Base button component (accessible). WCAG 2.1 AA compliance. Start: 02.01.2026"

# ============================================================
# QA TEAM - Testing Framework (ThomasKrause)
# ============================================================

# Issue #45 - E-Commerce Legal Tests
gh issue edit 45 --assignee "ThomasKrause" --add-label "sprint-2,qa,p0-critical,testing"
gh issue comment 45 --body "@ThomasKrause: Test framework assigned! 15 E-Commerce compliance tests. xUnit + FluentAssertions + TestContainers. Start: 04.01.2026 (after features). Duration: 2-3 weeks. 100% coverage target for P0.6 requirements"

# ============================================================
# LEGAL TEAM - Compliance Review (JuliaHoffmann)
# ============================================================

# (Already assigned above with Frontend for #41, #42, #29)
# JuliaHoffmann handles legal content review for:
# - #29 (Invoice legal requirements)
# - #41 (AGB content)
# - #42 (Privacy policy content)

echo "✅ All Sprint 1 issues assigned to team members!"
echo "📊 14 issues assigned"
echo "👥 9 team members active"
echo "🎯 Sprint starts: 02.01.2026"
```

---

## 📋 Manual Assignment Checklist

If you prefer clicking UI instead of CLI, here's the assignment list:

### Backend Developer #1 - Max Müller (@MaxMueller)
- [ ] #5: User Registration Handler (05 pts)
- [ ] #6: Email Verification (03 pts)
- [ ] #7: JWT Token Gen (05 pts)
- [ ] #9: Multi-Tenant (03 pts)
- [ ] #10: Password Policy (02 pts)
- [ ] #11: Login Lockout (03 pts)
- [ ] #12: Session Timeout (02 pts)
**Total**: 23 story points

### Backend Developer #2 - Lisa Schmidt (@LisaSchmidt)
- [ ] #20: Price Calculation (08 pts)
- [ ] #21: Shipping Cost (05 pts)
- [ ] #27: Return Labels (05 pts)
- [ ] #29: Invoice Generation (08 pts)
**Total**: 26 story points

### Tech Lead - Holger Rasch (@HRasch)
- [ ] #4: Epic - Registration Flow
- [ ] #30: VAT-ID Validation (08 pts)
- [ ] #31: Reverse Charge (05 pts)
**Total**: 13 story points + Epic oversight

### Frontend Developer #1 - Anna Weber (@AnnaWeber)
- [ ] #41: AGB & Terms (05 pts)
- [ ] #42: Privacy Policy (03 pts)
- [ ] #19: Vue Components (02 pts)
**Total**: 10 story points

### Security Engineer - David Keller (@DavidKeller)
- [ ] #30: VAT-ID Review (co-assigned with HRasch)
- [ ] #32: Invoice Encryption (08 pts)
**Total**: 8 story points

### Legal Officer - Julia Hoffmann (@JuliaHoffmann)
- [ ] #29: Invoice legal review (co-assigned)
- [ ] #41: AGB content (co-assigned)
- [ ] #42: Privacy policy (co-assigned)
**Total**: Shared effort (20 hours estimated)

### QA Engineer - Thomas Krause (@ThomasKrause)
- [ ] #45: E-Commerce Tests (15 test cases)
**Total**: Starts Week 2

---

## 🔍 Verification Commands

After assignments, verify they're correct:

```bash
# List all Sprint 1 issues
gh issue list --label "sprint-1" --state open

# Check assignments
gh issue list --assignee "MaxMueller" --state open
gh issue list --assignee "HRasch" --state open

# See full board status
gh project view 1 --owner b2connect-dev --repo b2connect-platform
```

---

## 📞 Notification

Notify team members they're assigned:

```bash
# Send welcome message to each assigned person
gh issue comment 4 --body "Welcome to Sprint 1! @HRasch, @MaxMueller, @LisaSchmidt, @AnnaWeber, @DavidKeller, @JuliaHoffmann, @ThomasKrause. Check the .github/ISSUES_[YOUR_ROLE].md file for detailed requirements. Start date: 02.01.2026. Daily standup: 09:00 CET"
```

---

## ✅ Verification Checklist

After running all commands:

- [ ] 14 issues assigned to team
- [ ] Labels added (sprint-1, p0-critical, etc.)
- [ ] Comments added with progress notes
- [ ] Dependencies documented
- [ ] Team members notified
- [ ] Board reflects assignments
- [ ] All team members can see their issues

---

## 🚨 Important Notes

**For Local Testing** (if not using real GitHub):
- Create test accounts with GitHub usernames (MaxMueller, LisaSchmidt, etc.)
- Issues are fiction but structure is production-ready
- All commands are real GitHub CLI syntax

**In Production Use**:
- Replace fictional names with real GitHub IDs
- Run commands in your actual B2Connect repo
- Update dates to current timeline
- Adjust story points based on team capacity

---

**Last Updated**: 28. Dezember 2025  
**Ready to Execute**: Yes ✅  
**Environment**: Production-Ready  

