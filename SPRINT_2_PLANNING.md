# 🎯 Sprint 2 Planning Session - 29. Dezember 2025

**Duration:** 30 minutes  
**Attendees:** Backend Developer, Frontend Developer, DevOps Engineer, QA Engineer, Security Engineer, Tech Lead  
**Status:** ACTIVE PLANNING  
**Velocity Target:** 40 story points  

---

## 📊 Current Project Status

### ✅ Completed (P0.6 Phase A)
- Architecture foundation (DDD + Wolverine)
- Build pipeline optimized (7.1 seconds)
- Test framework in place (50+ tests passing)
- Sprint 1 Issues created and ready (#30, #31)
- All GitHub compliance issues created (#30-#39)
- Documentation complete (10,580+ lines)

### 🚀 What's Next: Sprint 2 Focus
**P0.6 Store Legal Compliance - Phase B**
- Issue #32: 14-Day Withdrawal Right (VVVG)
- Issue #33: Order Confirmation Email (TMG)
- Issue #34: Invoice Generation & 10-Year Archival

---

## 📅 Sprint 2 Goals (3 Primary Objectives)

### Goal 1: Order Withdrawal & Returns (Legal)
**Issue #32 - 14-Day Withdrawal Right**
- Owner: Backend Developer
- Effort: 16 hours
- Regulatory: VVVG (German consumer protection)
- Dependencies: Order service from Sprint 1

**Tasks:**
- [ ] Create `WithdrawalRequestHandler` (Wolverine service)
- [ ] Calculate 14-day countdown from order
- [ ] Support partial withdrawals
- [ ] Trigger refund process
- [ ] Audit withdrawal records
- [ ] Frontend withdrawal button + confirmation modal
- [ ] Unit tests (5+ scenarios)
- [ ] QA validation + edge cases

**Done Criteria:**
- ✓ API endpoint: `POST /withdrawals` working
- ✓ Countdown stops on day 15
- ✓ Partial withdrawals supported
- ✓ Refunds trigger automatically
- ✓ Tests: 100% pass rate

---

### Goal 2: Email & Order Confirmation (Communication)
**Issue #33 - Order Confirmation Email**
- Owner: Backend Developer
- Effort: 14 hours
- Regulatory: TMG §7a (legal order confirmation)
- Dependencies: Email service integration

**Tasks:**
- [ ] Create `OrderConfirmationEmailService` (Wolverine handler)
- [ ] Template: Order details + withdrawal notice
- [ ] Include mandatory legal text (withdrawal right, contact)
- [ ] Support multi-language (German, English)
- [ ] Add PDF attachment capability
- [ ] Async email delivery with retry logic
- [ ] Logging & audit trail
- [ ] Unit tests (3+ scenarios)

**Done Criteria:**
- ✓ Email sent within 5 seconds of order creation
- ✓ Contains all legal requirements
- ✓ Supports German decimal format (1,99€)
- ✓ Retry on failure
- ✓ Audit logs created

---

### Goal 3: Invoice Management & Compliance (Tax/Legal)
**Issue #34 - Invoice Generation & 10-Year Archival**
- Owner: Backend Developer + QA Engineer
- Effort: 15 hours
- Regulatory: German Tax Code (10-year retention)
- Dependencies: Invoice format ZUGFeRD (P0.9 future link)

**Tasks:**
- [ ] Create `InvoiceGenerationService` (Wolverine handler)
- [ ] Generate invoice on order completion
- [ ] Include all mandatory fields (seller, buyer, VAT, terms)
- [ ] Create encrypted archive table (immutable)
- [ ] Implement 10-year soft-delete policy
- [ ] Add hash verification (tamper detection)
- [ ] Audit logging for all invoice access
- [ ] QA: Invoice validation tests (8+ scenarios)
- [ ] QA: Archival retention tests

**Done Criteria:**
- ✓ Invoice PDF generated within 30s of order completion
- ✓ All 16 mandatory fields present
- ✓ Stored encrypted (AES-256)
- ✓ Hash verification prevents tampering
- ✓ 10-year retention enforced
- ✓ Audit trail for all access

---

## 👥 Sprint 2 Assignments

### Backend Developer
**Primary:** #32 (Withdrawal), #33 (Email), #34 (Invoice)
**Capacity:** 40 hours
**Allocation:**
- #32 Withdrawal: 16h
- #33 Email: 14h
- #34 Invoice: 10h (shared with QA)
**Total:** 40h ✅ Perfect fit

**Success Criteria:**
- All 3 services built (WithdrawalService, EmailService, InvoiceService)
- Wolverine handlers working
- Unit tests written
- Code review ready by Friday

### Frontend Developer
**Primary:** #32 (Withdrawal UI), #34 (Invoice display)
**Capacity:** 20 hours
**Allocation:**
- #32 Withdrawal modal: 8h
- #34 Invoice download: 4h
- Testing & refinement: 8h
**Total:** 20h

**Success Criteria:**
- Withdrawal button in order detail
- Confirmation modal with legal text
- Invoice download functionality
- All Vue 3 components built

### QA Engineer
**Primary:** #32, #33, #34 validation
**Capacity:** 30 hours
**Allocation:**
- #32 Withdrawal E2E tests: 8h
- #33 Email delivery tests: 6h
- #34 Invoice validation tests: 12h
- Compliance verification: 4h
**Total:** 30h

**Test Plan:**
- Withdrawal scenarios: Happy path, partial, failed refund, replay attacks
- Email tests: Template validation, locale handling, retry logic
- Invoice tests: Format validation, archival, hash verification, retention policy

### DevOps Engineer
**Support role:** Port management, test environment
**Capacity:** 10 hours
**Allocation:**
- Environment setup: 4h
- Health check implementation: 3h
- Database migration scripts: 3h
**Total:** 10h

### Security Engineer
**Primary:** #34 (Encryption/Archival)
**Capacity:** 15 hours
**Allocation:**
- Encryption implementation review: 5h
- Audit logging setup: 5h
- Compliance verification: 5h
**Total:** 15h

**Focus:**
- PII encryption in invoice (IEncryptionService)
- Immutable audit log for invoice access
- Tamper detection (hash verification)
- GDPR compliance (right to be forgotten)

---

## 🔗 Dependencies & Blockers

### Critical Path
```
Sprint 1 Complete (Orders created)
    ↓
Sprint 2 #32 (Withdrawal logic)
    ↓
Sprint 2 #33 (Email notification)
    ↓
Sprint 2 #34 (Invoice generation)
```

### Potential Blockers
| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Email service integration delay | Medium | High | Start email service setup immediately (Wed) |
| Invoice format changes (P0.9 conflict) | Low | Medium | Align with tax team before design (Tuesday) |
| Encryption key management | Low | High | Use existing IEncryptionService, security review early |
| QA environment instability | Low | Medium | Run environment tests before Sprint 2 start |

---

## 📈 Velocity & Capacity

### Planned Capacity
```
Backend Developer:  40h (assigned 40h)      ✅ 100%
Frontend Developer: 20h (assigned 20h)      ✅ 100%
QA Engineer:        30h (assigned 30h)      ✅ 100%
DevOps Engineer:    10h (assigned 10h)      ✅ 100%
Security Engineer:  15h (assigned 15h)      ✅ 100%
───────────────────────
TOTAL:             115h

Sprint Capacity:    40h per developer × 5 = 200h available
Utilization:       115h / 200h = 57.5% ✅ Healthy
```

### Why Only 57.5% Utilization?
- Code review & Q&A (10%)
- Environment troubleshooting (10%)
- Documentation & knowledge transfer (10%)
- Buffer for unknowns (12.5%)

---

## 🚀 Sprint Execution Plan

### Monday 29. Dezember - Kickoff
- [ ] **9:00** Sprint planning standup (all agents)
- [ ] **9:15** Review issue details (#32, #33, #34)
- [ ] **9:30** Assign tasks to agents
- [ ] **10:00** Backend: Start service design
- [ ] **10:00** Security: Review encryption approach
- [ ] **10:30** QA: Write test plans

### Tuesday 30. Dezember - Design & Spec
- [ ] Backend: Complete `WithdrawalService.CreateAsync()` signature
- [ ] Backend: Design `OrderConfirmationEmailService`
- [ ] Backend: Design `InvoiceGenerationService`
- [ ] Security: Encryption & audit logging review
- [ ] Frontend: Withdrawal modal wireframe
- [ ] QA: E2E test scenarios review

### Wednesday 31. Dezember - Implementation Start
- [ ] Backend: Implement withdrawal logic
- [ ] Backend: Email template design
- [ ] Backend: Invoice generation (core logic)
- [ ] Frontend: Withdrawal button component
- [ ] QA: Setup test data & automation

### Thursday 1. Januar - Mid-Sprint Check
- [ ] **Standup at 10:00** - Progress update
- [ ] Backend: Integration testing begins
- [ ] Frontend: Connect to backend APIs
- [ ] QA: Start E2E test execution
- [ ] Security: Code review for encryption

### Friday 2. Januar - Testing & Refinement
- [ ] Backend: Code review & fixes
- [ ] Frontend: Bug fixes & edge cases
- [ ] QA: Full regression testing
- [ ] DevOps: Production environment prep

### Friday 2. Januar - Sprint Review
- [ ] **14:00** Sprint review demo
- [ ] Show #32, #33, #34 working end-to-end
- [ ] QA report: All tests passing
- [ ] Security: Compliance checklist review
- [ ] Sprint retrospective

---

## ✅ Acceptance Criteria (Gate for Merge)

### #32 - Withdrawal (Definition of Done)
- [ ] `WithdrawalService` public methods: `CreateAsync()`, `GetAsync()`, `ListAsync()`
- [ ] Wolverine handler `POST /withdrawals` working
- [ ] Database: `Withdrawals` table with soft delete
- [ ] Audit: All withdrawal actions logged
- [ ] Frontend: Modal component + button
- [ ] Tests: 100% pass (5+ scenarios)
- [ ] Security: Tenant isolation verified
- [ ] Code review: ✅ Approved

### #33 - Email (Definition of Done)
- [ ] `OrderConfirmationEmailService` public method: `SendAsync(orderId, cancellationToken)`
- [ ] Wolverine handler `POST /send-confirmation-email` working
- [ ] Email template with mandatory legal text
- [ ] Localization: German + English
- [ ] Retry logic: Exponential backoff (3 attempts)
- [ ] Audit: Email delivery logged
- [ ] Tests: 100% pass (SMTP mock, locale scenarios)
- [ ] Code review: ✅ Approved

### #34 - Invoice (Definition of Done)
- [ ] `InvoiceGenerationService` public method: `GenerateAsync(orderId, cancellationToken)`
- [ ] PDF generation (16 mandatory fields)
- [ ] Database: `Invoices` table (encrypted, immutable)
- [ ] Archival: 10-year soft delete enforced
- [ ] Hash verification: Tamper detection working
- [ ] Audit: All invoice access logged
- [ ] Frontend: Download button in order detail
- [ ] Tests: 100% pass (format validation, retention policy)
- [ ] Security review: ✅ Encryption verified
- [ ] Code review: ✅ Approved

---

## 📋 Daily Standup Template

```
## Daily Standup - [Date]

### Backend Developer
- ✅ Completed: [Tasks]
- 🚧 In Progress: [Current tasks]
- 🔴 Blocked By: [Issues]
- 📅 Next: [Tomorrow's focus]

### Frontend Developer
- ✅ Completed: [Tasks]
- 🚧 In Progress: [Current tasks]
- 🔴 Blocked By: [Issues]
- 📅 Next: [Tomorrow's focus]

### QA Engineer
- ✅ Completed: [Tests]
- 🚧 In Progress: [Test scenarios]
- 🔴 Blocked By: [Issues]
- 📅 Next: [Tomorrow's focus]

### DevOps Engineer
- ✅ Completed: [Infrastructure]
- 🚧 In Progress: [Deployments]
- 🔴 Blocked By: [Issues]
- 📅 Next: [Tomorrow's focus]

### Security Engineer
- ✅ Completed: [Reviews]
- 🚧 In Progress: [Validation]
- 🔴 Blocked By: [Issues]
- 📅 Next: [Tomorrow's focus]
```

---

## 🎯 Success Metrics (End of Sprint)

| Metric | Target | How to Measure |
|--------|--------|----------------|
| **Story Completion** | 3/3 issues | All #32, #33, #34 merged |
| **Test Coverage** | 80%+ | `dotnet test` coverage report |
| **Build Time** | < 10s | Task timing from CI/CD |
| **Code Quality** | 0 critical issues | ESLint + Roslyn static analysis |
| **Security Review** | ✅ Passed | Security engineer sign-off |
| **E2E Tests** | 100% pass | QA final report |
| **Team Satisfaction** | 4/5+ | Retrospective feedback |

---

## 📞 Communication Plan

- **Standup:** Daily at 10:00 (5 min sync)
- **Blockers:** Post in Slack immediately (don't wait)
- **Code Review:** Turn-around < 4 hours (high priority)
- **Questions:** Tag specific agent (e.g., @backend-developer)
- **Escalation:** Tech Lead if blocked > 2 hours

---

## 🚀 Ready to Start?

**Action Items Before Friday Sprint Review:**

1. **Backend Developer:**
   - [ ] Create feature branches: `feature/withdrawal`, `feature/email`, `feature/invoice`
   - [ ] Build service stubs with Wolverine handlers
   - [ ] Set up unit test projects

2. **Frontend Developer:**
   - [ ] Create `WithdrawalModal.vue` component
   - [ ] Create `InvoiceDownloadButton.vue` component
   - [ ] Connect to backend endpoints

3. **QA Engineer:**
   - [ ] Write E2E test scenarios (Playwright)
   - [ ] Create test data factories
   - [ ] Setup test environment

4. **DevOps Engineer:**
   - [ ] Verify test environment ports (7002, 7005, 7006)
   - [ ] Prepare database migrations script
   - [ ] Health check monitoring

5. **Security Engineer:**
   - [ ] Review encryption approach
   - [ ] Prepare audit logging templates
   - [ ] Compliance checklist ready

---

**Sprint Start:** Monday 29. Dezember, 9:00  
**Sprint End:** Friday 2. Januar, 17:00  
**Sprint Length:** 5 working days  
**Planned Velocity:** 40 story points  

✅ **PLANNING COMPLETE - READY TO EXECUTE**

