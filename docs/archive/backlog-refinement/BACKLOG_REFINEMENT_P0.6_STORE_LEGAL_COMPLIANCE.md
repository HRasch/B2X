# Backlog Refinement: P0.6 Store Legal Compliance (Issue #15)

**Date:** 28. Dezember 2025  
**Product Owner:** [Name]  
**Tech Lead:** [Name]  
**Status:** Ready for Refinement  

---

## 📊 Epic Overview

**Epic:** P0.6 - Store Legal Compliance Implementation (EU E-Commerce)  
**GitHub Issue:** #15  
**Total Effort:** 60 hours  
**Target Duration:** 1.5 weeks (2 Backend Devs)  
**Phase:** Phase 0 (Compliance Foundation)  

---

## 🎯 Business Goals

1. **Legal Risk Mitigation**: Eliminate €5,000-€300,000 fines for non-compliance
2. **Enterprise Sales**: Enable contracts with large customers requiring legal compliance
3. **Customer Trust**: Transparent pricing, clear terms, reliable returns
4. **GDPR Compliance**: Proper data handling and privacy disclosures
5. **Market Expansion**: Support EU-wide operations (DACH first, then EU)

---

## 📋 User Stories (Broken Down)

### Story 1: B2C Price Transparency (PAngV)
**ID:** P0.6-US-001  
**Effort:** 12 hours  
**Sprint:** Sprint 1  

**As a** shop owner  
**I want** customers to always see final prices including VAT  
**So that** I comply with PAngV and customers know actual cost before checkout

**Acceptance Criteria:**
- [ ] Product listing displays price as "€99,99 inkl. MwSt"
- [ ] Product detail page shows VAT breakdown
- [ ] Cart displays "Subtotal + VAT = Total"
- [ ] Checkout confirms final price before payment
- [ ] Invoice shows VAT breakdown per line item
- [ ] All prices dynamically update when VAT rate changes
- [ ] Mobile responsive (tested at 320px+)

**Definition of Done:**
- [ ] Code reviewed
- [ ] 3 unit tests + 2 E2E tests passing
- [ ] No warnings in build
- [ ] Performance: Page load < 2s

**Subtasks:**
1. Create `PriceCalculationService` (backend)
2. Implement VAT rate lookup per country/product
3. Update product component to show VAT
4. Update cart component to show breakdown
5. Create E2E tests (Playwright)
6. Documentation updated

**Dependencies:** VAT master data must be configured in system

---

### Story 2: B2C Shipping Cost Display
**ID:** P0.6-US-002  
**Effort:** 8 hours  
**Sprint:** Sprint 1  

**As a** customer  
**I want** to see shipping costs before confirming my order  
**So that** I can decide if the total cost is acceptable

**Acceptance Criteria:**
- [ ] Shipping cost calculated based on country selection
- [ ] Displayed on cart page before checkout
- [ ] Updates dynamically when changing shipping country
- [ ] Displayed as "Shipping: €X.XX to [Country]"
- [ ] Free shipping option available if configured
- [ ] Maximum order weight validation
- [ ] Shipping not applicable for digital products

**Definition of Done:**
- [ ] 2 unit tests + 1 E2E test passing
- [ ] Performance: Calculation < 100ms

**Subtasks:**
1. Implement `ShippingCostService`
2. Update cart component to show shipping
3. Add country selector with dynamic recalculation
4. Create tests

**Dependencies:** Shipping rates must be configured per country

---

### Story 3: 14-Day Withdrawal Period Enforcement
**ID:** P0.6-US-003  
**Effort:** 16 hours  
**Sprint:** Sprint 1-2  

**As a** customer  
**I want** to return items within 14 days of delivery  
**So that** I can get a refund if the product doesn't meet my needs

**Acceptance Criteria:**
- [ ] System calculates 14-day window from delivery date (not order date)
- [ ] Customer can request return within 14 days via web UI
- [ ] Return requests after 14 days are rejected with clear message
- [ ] Return label automatically generated with tracking
- [ ] Refund processed within 14 days of withdrawal request
- [ ] Customer receives email confirmation of return
- [ ] Return status visible in customer account
- [ ] Audit log entry for each return process

**Definition of Done:**
- [ ] 4 unit tests + 2 E2E tests passing
- [ ] Return label generated successfully (test with mock carrier API)
- [ ] Refund processes to original payment method

**Subtasks:**
1. Create `ReturnManagementService`
2. Add return request UI to customer account
3. Implement return label API integration (DHL/DPD)
4. Create refund processing logic
5. Email templates (return confirmation, refund notice)
6. Create tests

**Dependencies:** Carrier integration (DHL API or equivalent)

---

### Story 4: Legal Documents Management (Impressum, Privacy, Terms)
**ID:** P0.6-US-004  
**Effort:** 20 hours  
**Sprint:** Sprint 2  

**As a** shop admin  
**I want** to edit my shop's legal documents (Impressum, Privacy Policy, Terms & Conditions)  
**So that** my shop can operate legally in the EU

**Acceptance Criteria:**
- [ ] Admin UI to edit Impressum (company name, address, contact, VAT-ID)
- [ ] Admin UI to edit Privacy Policy (GDPR-compliant template provided)
- [ ] Admin UI to edit Terms & Conditions (B2C/B2B variants)
- [ ] Version control: each change creates new version
- [ ] Document history visible (when changed, by whom, what changed)
- [ ] Preview before publishing
- [ ] Activation timestamp recorded
- [ ] Customers notified when major terms change
- [ ] Old versions archived (immutable)
- [ ] Footer links automatically generated

**Definition of Done:**
- [ ] 5 unit tests + 2 E2E tests passing
- [ ] Admin UI validated by UX review
- [ ] Legal review approved by compliance officer

**Subtasks:**
1. Create `LegalDocumentsService` (backend)
2. Create database schema for legal docs + versions
3. Build admin UI component
4. Implement version history & audit trail
5. Create footer links component
6. Email notification for terms changes
7. Create tests

**Dependencies:** Legal templates must be provided

---

### Story 5: Mandatory Terms & Conditions Acceptance
**ID:** P0.6-US-005  
**Effort:** 8 hours  
**Sprint:** Sprint 2  

**As a** shop owner  
**I want** customers to explicitly accept my Terms & Conditions  
**So that** I have proof of acceptance for legal protection

**Acceptance Criteria:**
- [ ] Checkout page displays checkbox "I accept Terms & Conditions"
- [ ] Checkbox is **required** (cannot uncheck during checkout)
- [ ] Link to full Terms & Conditions
- [ ] Timestamp and document version recorded with acceptance
- [ ] Customer cannot proceed without accepting
- [ ] Acceptance logged in audit trail (who, when, which version)
- [ ] Error message if user tries to skip

**Definition of Done:**
- [ ] 2 unit tests + 1 E2E test passing
- [ ] Accessibility: keyboard navigable, screen reader friendly

**Subtasks:**
1. Update checkout component
2. Add Terms acceptance service
3. Store acceptance in order record
4. Create tests

**Dependencies:** Story P0.6-US-004 (legal docs management)

---

### Story 6: B2B VAT-ID Validation (VIES)
**ID:** P0.6-US-006  
**Effort:** 12 hours  
**Sprint:** Sprint 2-3  

**As a** B2B customer  
**I want** my VAT-ID validated against the EU VIES database  
**So that** I can get correct pricing (with reverse charge if applicable)

**Acceptance Criteria:**
- [ ] B2B checkout shows VAT-ID input field (optional for B2C)
- [ ] When VAT-ID entered, validate against VIES API
- [ ] Valid VAT-ID shows company name & address
- [ ] Store validation result with expiry date (1 year)
- [ ] Invalid VAT-ID shows error, allow continue as B2C
- [ ] API timeout handled gracefully (fallback to B2C pricing)
- [ ] Rate limiting: max 1 request per 5 seconds per IP
- [ ] Audit log: VAT-ID validation attempts

**Definition of Done:**
- [ ] 3 unit tests + 1 E2E test passing
- [ ] Mock VIES API for testing
- [ ] Error scenarios tested (invalid ID, timeout, rate limit)

**Subtasks:**
1. Create `VatIdValidationService` (calls VIES API)
2. Add VAT-ID input to checkout
3. Implement validation error handling
4. Create mock VIES API for tests
5. Implement caching (valid IDs cached 1 year)
6. Create tests

**Dependencies:** VIES API access (free, EU-provided)

---

### Story 7: B2B Reverse Charge Logic
**ID:** P0.6-US-007  
**Effort:** 10 hours  
**Sprint:** Sprint 3  

**As a** B2B customer  
**I want** VAT not charged when I provide a valid VAT-ID  
**So that** I don't pay VAT on intra-EU purchases (reverse charge applies)

**Acceptance Criteria:**
- [ ] IF buyer has valid VAT-ID AND buyer country ≠ seller country THEN no VAT
- [ ] Invoice shows "Reverse Charge (Art. 199a Directive 2006/112/EC)"
- [ ] VAT Amount on invoice = €0
- [ ] Buyer's VAT-ID displayed on invoice
- [ ] Seller's VAT-ID displayed on invoice
- [ ] B2C customers (no VAT-ID) always charged VAT
- [ ] Price calculation correct: no VAT added to total

**Definition of Done:**
- [ ] 3 unit tests + 1 E2E test passing
- [ ] All edge cases tested (same country, invalid ID, etc.)

**Subtasks:**
1. Create `ReverseChargeService`
2. Update `PriceCalculationService` to use reverse charge logic
3. Update invoice template to show reverse charge note
4. Create tests

**Dependencies:** Story P0.6-US-006 (VAT-ID validation)

---

### Story 8: Invoice Generation & Encryption
**ID:** P0.6-US-008  
**Effort:** 18 hours  
**Sprint:** Sprint 3  

**As a** customer  
**I want** invoices automatically generated and securely stored  
**So that** I have a record of my purchase and can download it anytime

**Acceptance Criteria:**
- [ ] Invoice PDF generated automatically after payment confirmation
- [ ] Invoice encrypted with AES-256 before storage
- [ ] Stored for minimum 10 years (German tax law)
- [ ] Invoice number unique per shop
- [ ] Invoice includes: items, VAT breakdown, customer details (encrypted), delivery address
- [ ] Shop legal entity info (Impressum) included on invoice
- [ ] Payment method displayed
- [ ] Signature placeholder ready for eIDAS compliance
- [ ] Customer can download invoice from account
- [ ] Invoice sent via email immediately
- [ ] Immutability: once created, cannot be modified
- [ ] Audit log: who accessed which invoice, when

**Definition of Done:**
- [ ] 4 unit tests + 1 E2E test passing
- [ ] Invoice PDF format validated (readable, correct data)
- [ ] Encryption/decryption working correctly
- [ ] Performance: invoice generation < 2 seconds

**Subtasks:**
1. Create `InvoiceGenerationService` (PDF creation)
2. Create `InvoiceStorageService` (encrypted storage)
3. Create `InvoiceRetrievalService` (customer download)
4. Implement invoice number generation (unique per shop)
5. Create invoice template (HTML → PDF)
6. Integrate with email service
7. Create archival process (10-year retention)
8. Create tests

**Dependencies:** PDF generation library (iText, SelectPdf, etc.), encryption service (P0.2)

---

### Story 9: Invoice Archival & Retention Policy
**ID:** P0.6-US-009  
**Effort:** 6 hours  
**Sprint:** Sprint 3  

**As a** system administrator  
**I want** old invoices automatically archived after 10 years  
**So that** we comply with German tax law and manage storage efficiently

**Acceptance Criteria:**
- [ ] Background job runs daily to archive invoices > 10 years old
- [ ] Archived invoices marked as `IsArchived = true`
- [ ] Archived invoices still accessible to customer (read-only)
- [ ] Audit log: when invoice was archived
- [ ] Retention policy configurable per tenant
- [ ] Alert if deletion date approaching (90 days before)

**Definition of Done:**
- [ ] 2 unit tests passing
- [ ] Job runs successfully on test data
- [ ] No customer-facing impact

**Subtasks:**
1. Create `InvoiceArchivalService`
2. Implement background job (HostedService)
3. Create admin monitoring view
4. Create tests

**Dependencies:** Story P0.6-US-008 (invoice generation)

---

### Story 10: Admin Dashboard - Legal Documents Overview
**ID:** P0.6-US-010  
**Effort:** 8 hours  
**Sprint:** Sprint 3  

**As a** shop admin  
**I want** to see an overview of legal documents and their versions  
**So that** I can manage compliance easily

**Acceptance Criteria:**
- [ ] Dashboard shows current Impressum, Privacy Policy, Terms & Conditions
- [ ] Display last updated date and by whom
- [ ] Quick links to edit each document
- [ ] Show version history (click to see old versions)
- [ ] Activation status (active/draft)
- [ ] Warning if critical documents missing
- [ ] Link to compliance checklist

**Definition of Done:**
- [ ] UI approved by Product Owner
- [ ] 1 E2E test passing

**Subtasks:**
1. Design dashboard layout
2. Create admin component
3. Create tests

**Dependencies:** Story P0.6-US-004 (legal docs management)

---

## 📅 Sprint Plan

### Sprint 1 (5 days) - Price & Shipping
| Story | ID | Hours | Owner | Status |
|-------|-----|-------|-------|--------|
| B2C Price Transparency | P0.6-US-001 | 12 | Backend Dev 1 | Ready |
| B2C Shipping Costs | P0.6-US-002 | 8 | Backend Dev 1 | Ready |
| **Sprint 1 Total** | | **20** | | |

**Sprint Goals:**
- ✅ Customers see final prices with VAT
- ✅ Shipping costs displayed before checkout
- ✅ PAngV compliance achieved

---

### Sprint 2 (5 days) - Returns & Legal Docs
| Story | ID | Hours | Owner | Status |
|-------|-----|-------|-------|--------|
| 14-Day Withdrawal | P0.6-US-003 | 16 | Backend Dev 1 | Ready |
| Legal Docs Management | P0.6-US-004 | 20 | Backend Dev 2 | Ready |
| Acceptance Checkbox | P0.6-US-005 | 8 | Frontend Dev 1 | Ready |
| **Sprint 2 Total** | | **44** | | |

⚠️ **Note:** Sprint 2 is overloaded (44 hours / 40 capacity). Recommend moving P0.6-US-004 to Sprint 3 OR reducing scope.

**Revised Sprint 2 (5 days):**
| Story | ID | Hours | Owner | Status |
|-------|-----|-------|-------|--------|
| 14-Day Withdrawal | P0.6-US-003 | 16 | Backend Dev 1 | Ready |
| Terms Acceptance | P0.6-US-005 | 8 | Frontend Dev 1 | Ready |
| B2B VAT-ID Validation | P0.6-US-006 | 12 | Backend Dev 2 | Ready |
| **Sprint 2 Revised Total** | | **36** | | ✅ Fits |

---

### Sprint 3 (5 days) - B2B & Invoicing
| Story | ID | Hours | Owner | Status |
|-------|-----|-------|-------|--------|
| Legal Docs Management | P0.6-US-004 | 20 | Backend Dev 1 | Ready |
| Reverse Charge Logic | P0.6-US-007 | 10 | Backend Dev 2 | Ready |
| Invoice Generation | P0.6-US-008 | 18 | Backend Dev 1 | Ready |
| Invoice Archival | P0.6-US-009 | 6 | Backend Dev 2 | Ready |
| Dashboard Overview | P0.6-US-010 | 8 | Frontend Dev 1 | Ready |
| **Sprint 3 Total** | | **62** | | |

⚠️ **Sprint 3 is overloaded (62 hours / 40 capacity).**

**Recommendation:** Reduce P0.6-US-010 to next phase (nice-to-have, not critical for compliance).

**Revised Sprint 3 (5 days):**
| Story | ID | Hours | Owner | Status |
|-------|-----|-------|-------|--------|
| Legal Docs Management | P0.6-US-004 | 20 | Backend Dev 1 | Ready |
| Reverse Charge Logic | P0.6-US-007 | 10 | Backend Dev 2 | Ready |
| Invoice Generation | P0.6-US-008 | 18 | Backend Dev 1 | Ready |
| Invoice Archival | P0.6-US-009 | 6 | Backend Dev 2 | Ready |
| **Sprint 3 Revised Total** | | **54** | | |

---

## 🔗 Dependencies

```
P0.6-US-001 (Price) ────────────┐
                                ├─► P0.6-US-002 (Shipping)
                                │
P0.6-US-003 (Returns) ──────────┤
                                │
P0.6-US-004 (Legal Docs) ───────┼─► P0.6-US-005 (Acceptance)
                                │
P0.6-US-006 (VAT-ID) ──────────┐│
                               ││
                              ││
                               └┴─► P0.6-US-007 (Reverse Charge)
                                     │
                                     ├─► P0.6-US-008 (Invoicing)
                                     │
                                     └─► P0.6-US-009 (Archival)
```

**Critical Path:**
1. P0.6-US-001, P0.6-US-002 (can start immediately)
2. P0.6-US-003, P0.6-US-004 (no dependencies, can start immediately)
3. P0.6-US-005 (depends on P0.6-US-004)
4. P0.6-US-006 (independent)
5. P0.6-US-007 (depends on P0.6-US-006)
6. P0.6-US-008, P0.6-US-009 (depends on P0.6-US-001 & P0.6-US-007)

---

## ⚠️ Risks & Blockers

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| VIES API unreliable | Medium | High | Implement fallback to B2C pricing |
| PDF generation complexity | Medium | Medium | Use proven library (iText, SelectPdf) |
| Legal doc templates non-standard | High | Medium | Get legal review early (first 2 days) |
| Carrier API integration issues | Medium | Medium | Use mock API for MVP, real integration later |
| Performance: invoice generation slow | Low | Medium | Cache, optimize SQL queries |
| Frontend complexity: many new components | Medium | Medium | Reuse existing components, keep simple |

---

## 🧪 Testing Strategy

### Unit Tests (Backend)
- Price calculation: 8 test cases
- VAT-ID validation: 5 test cases
- Reverse charge logic: 4 test cases
- Invoice generation: 4 test cases
- Return management: 4 test cases
- Legal docs versioning: 3 test cases
- **Total Unit Tests:** 28 cases

### E2E Tests (Frontend)
- Checkout flow with prices: 2 tests
- Return request: 2 tests
- Legal doc acceptance: 1 test
- B2B VAT-ID entry: 1 test
- Invoice download: 1 test
- **Total E2E Tests:** 7 cases

### Manual Testing
- Regulatory compliance review (Legal Officer)
- Tax calculation review (Accountant)
- UI/UX review (Product Owner)

---

## 📊 Effort Estimate Summary

| Sprint | Stories | Hours | Capacity | Status |
|--------|---------|-------|----------|--------|
| Sprint 1 | 2 | 20 | 40 | ✅ Fits |
| Sprint 2 | 3 | 36 | 40 | ✅ Fits |
| Sprint 3 | 4 | 54 | 40 | ⚠️ Overflow +14h |
| **Total** | **10** | **110** | **120** | ⚠️ Over by 10h |

**Mitigation Options:**
1. **Extend to 3.5 weeks** (allow 50h/week per dev)
2. **Move P0.6-US-010 to Phase 1** (nice-to-have, not critical)
3. **Reduce P0.6-US-008 scope** (basic invoicing only, skip archival)
4. **Add 3rd developer** for Sprint 3

**Recommendation:** Move P0.6-US-010 to Phase 1, keep 3 weeks delivery.

---

## ✅ Definition of Done (Epic Level)

Before closing the epic, verify:

- [ ] **Price Transparency**: All prices show VAT, shipping visible before checkout
- [ ] **Returns**: 14-day window enforced, labels generated, refunds processed
- [ ] **Legal Docs**: Impressum, Privacy, Terms editable by shop admin, versioned
- [ ] **B2B Support**: VAT-ID validation, reverse charge logic working
- [ ] **Invoicing**: PDFs generated, encrypted, stored 10 years, downloadable
- [ ] **Terms Acceptance**: Logged with timestamp, version, customer ID
- [ ] **Tests**: All 35 test cases passing (28 unit + 7 E2E)
- [ ] **Code Review**: Tech Lead approved
- [ ] **Legal Review**: Compliance Officer approved
- [ ] **Documentation**: Admin guide, user guide updated
- [ ] **Performance**: Invoice generation < 2s, API response < 200ms

---

## 📞 Team Assignment

| Role | Person | Responsibility | Availability |
|------|--------|-----------------|--------------|
| **Backend Dev 1** | [Name] | P0.6-US-001, P0.6-US-003, P0.6-US-004, P0.6-US-008 | 100% |
| **Backend Dev 2** | [Name] | P0.6-US-002, P0.6-US-006, P0.6-US-007, P0.6-US-009 | 100% |
| **Frontend Dev** | [Name] | P0.6-US-005, P0.6-US-010 | 50% (shared) |
| **QA Engineer** | [Name] | Test case creation, test automation | 100% |
| **Tech Lead** | [Name] | Code review, architecture, Go/No-Go | On-demand |
| **Compliance Officer** | [Name] | Legal template review, compliance sign-off | 10 hours |

---

## 📋 Next Steps

1. **Day 1**: Backlog refinement meeting
   - Review effort estimates with team
   - Clarify acceptance criteria
   - Assign team members
   - Get legal review of templates

2. **Day 2**: Sprint planning
   - Commit to Sprint 1 stories
   - Create tasks in Jira/GitHub
   - Set up development environment

3. **Day 3-14**: Execution
   - Daily standup (15 min)
   - Sprint reviews (Friday)
   - Merge & deploy to staging

---

## 📚 Related Documents

- [P0.6_ECOMMERCE_LEGAL_TESTS.md](docs/P0.6_ECOMMERCE_LEGAL_TESTS.md) - Full test specifications
- [ECOMMERCE_LEGAL_OVERVIEW.md](docs/ECOMMERCE_LEGAL_OVERVIEW.md) - Regulatory details
- [EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) - Phase 0 roadmap
- [APPLICATION_SPECIFICATIONS.md](docs/APPLICATION_SPECIFICATIONS.md) - Full requirements

---

**Approved By:** [Tech Lead]  
**Date:** [Date]
