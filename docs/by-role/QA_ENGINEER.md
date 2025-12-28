# 🧪 QA Engineer - Documentation Guide

**Role:** QA Engineer | **P0 Components:** ALL (Test Execution)  
**Time to Read:** ~4 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als QA Engineer bist du verantwortlich für:
- **52 Compliance Tests** (P0.6, P0.7, P0.8, P0.9)
- **Test Automation** (xUnit, Playwright, axe)
- **Test Reporting** (Coverage, Results)
- **Verification** (All acceptance criteria)
- **Regression Testing**

---

## 📚 Required Reading (P0)

### Week 1: Testing Foundation

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **Testing Framework** | [TESTING_FRAMEWORK_GUIDE.md](../TESTING_FRAMEWORK_GUIDE.md) | 45 min |
| 2 | **Testing Guide** | [guides/TESTING_GUIDE.md](../guides/TESTING_GUIDE.md) | 30 min |
| 3 | **Debugging Guide** | [guides/DEBUGGING_GUIDE.md](../guides/DEBUGGING_GUIDE.md) | 20 min |
| 4 | **Debug Quick Ref** | [guides/DEBUG_QUICK_REFERENCE.md](../guides/DEBUG_QUICK_REFERENCE.md) | 10 min |
| 5 | **Compliance Testing** | [compliance/COMPLIANCE_TESTING_EXAMPLES.md](../compliance/COMPLIANCE_TESTING_EXAMPLES.md) | 30 min |

### Week 2: Compliance Test Specifications (ALL!)

| # | Document | Path | Tests | Est. Time |
|---|----------|------|-------|-----------|
| 6 | **E-Commerce Tests (P0.6)** | [compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md](../compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md) | 15 | 45 min |
| 7 | **AI Act Tests (P0.7)** | [compliance/P0.7_AI_ACT_TESTS.md](../compliance/P0.7_AI_ACT_TESTS.md) | 15 | 45 min |
| 8 | **BITV Tests (P0.8)** | [compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md](../compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md) | 12 | 45 min |
| 9 | **E-Rechnung Tests (P0.9)** | [compliance/P0.9_ERECHNUNG_TESTS.md](../compliance/P0.9_ERECHNUNG_TESTS.md) | 10 | 30 min |

### Week 3: Verification

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 10 | **Verification Guide** | [guides/VERIFICATION.md](../guides/VERIFICATION.md) | 20 min |
| 11 | **Review Summary** | [REVIEW_COMPLETION_SUMMARY.md](../REVIEW_COMPLETION_SUMMARY.md) | 15 min |

---

## 📊 Test Execution Matrix (52 Tests Total)

### P0.6: E-Commerce Legal (15 Tests)

| # | Test Name | Type | Automation | Status |
|---|-----------|------|------------|--------|
| 1 | VAT_Calculation_B2C_IncludesCorrectRate | Unit | xUnit | ⏳ |
| 2 | VAT_Calculation_B2B_ReverseCharge_NoVAT | Unit | xUnit | ⏳ |
| 3 | VIES_VatIdValidation_ValidId_ReturnsTrue | Integration | xUnit | ⏳ |
| 4 | VIES_VatIdValidation_InvalidId_ReturnsFalse | Integration | xUnit | ⏳ |
| 5 | Invoice_Generation_CreatesValidPDF | Unit | xUnit | ⏳ |
| 6 | Invoice_Archival_Stores10Years_Immutable | Integration | xUnit | ⏳ |
| 7 | Withdrawal_Request_Within14Days_Accepted | Unit | xUnit | ⏳ |
| 8 | Withdrawal_Request_After14Days_Rejected | Unit | xUnit | ⏳ |
| 9 | Price_Display_ShowsInclVAT_Always | E2E | Playwright | ⏳ |
| 10 | Shipping_Cost_VisibleBeforeCheckout | E2E | Playwright | ⏳ |
| 11 | AGB_Checkbox_RequiredBeforeOrder | E2E | Playwright | ⏳ |
| 12 | Impressum_Link_Accessible | E2E | Playwright | ⏳ |
| 13 | Datenschutz_Link_Accessible | E2E | Playwright | ⏳ |
| 14 | Order_Confirmation_Email_Sent | Integration | xUnit | ⏳ |
| 15 | Return_Label_Generation_Works | Integration | xUnit | ⏳ |

### P0.7: AI Act (15 Tests)

| # | Test Name | Type | Automation | Status |
|---|-----------|------|------------|--------|
| 1 | AiRiskRegister_AllSystemsDocumented | Unit | xUnit | ⏳ |
| 2 | FraudDetection_ClassifiedAsHighRisk | Unit | xUnit | ⏳ |
| 3 | AiDecisionLog_Created_ForEveryDecision | Integration | xUnit | ⏳ |
| 4 | AiDecisionLog_IncludesExplanation | Unit | xUnit | ⏳ |
| 5 | BiasTest_NoGenderDisparity | Unit | xUnit | ⏳ |
| 6 | BiasTest_NoAgeDisparity | Unit | xUnit | ⏳ |
| 7 | BiasTest_NoRegionDisparity | Unit | xUnit | ⏳ |
| 8 | PerformanceMonitor_DetectsDrift | Unit | xUnit | ⏳ |
| 9 | HighRiskDecision_FlaggedForReview | Integration | xUnit | ⏳ |
| 10 | HumanOverride_Logged | Integration | xUnit | ⏳ |
| 11 | UserExplanation_API_ReturnsDetails | Integration | xUnit | ⏳ |
| 12 | AiSystem_HasResponsiblePerson | Unit | xUnit | ⏳ |
| 13 | ModelVersion_Tracked | Unit | xUnit | ⏳ |
| 14 | TrainingData_Documented | Unit | xUnit | ⏳ |
| 15 | Limitations_Documented | Unit | xUnit | ⏳ |

### P0.8: Barrierefreiheit / BITV (12 Tests)

| # | Test Name | Type | Automation | Status |
|---|-----------|------|------------|--------|
| 1 | Keyboard_Navigation_AllFunctional | E2E | Playwright | ⏳ |
| 2 | Keyboard_Shortcuts_Work | E2E | Playwright | ⏳ |
| 3 | Modal_ClosableWithEscape | E2E | Playwright | ⏳ |
| 4 | ScreenReader_SemanticHTML | E2E | axe | ⏳ |
| 5 | ScreenReader_FormLabels | E2E | axe | ⏳ |
| 6 | ScreenReader_ErrorAnnouncements | E2E | axe | ⏳ |
| 7 | ColorContrast_MeetsWCAG_AA | E2E | axe | ⏳ |
| 8 | TextResize_200Percent_NoBreakage | E2E | Playwright | ⏳ |
| 9 | Video_HasCaptions_DE | E2E | Manual | ⏳ |
| 10 | Video_HasCaptions_EN | E2E | Manual | ⏳ |
| 11 | PageTitles_Unique | E2E | axe | ⏳ |
| 12 | Images_HaveAltText | E2E | axe | ⏳ |

### P0.9: E-Rechnung (10 Tests)

| # | Test Name | Type | Automation | Status |
|---|-----------|------|------------|--------|
| 1 | ZUGFeRD_XML_ValidStructure | Unit | xUnit | ⏳ |
| 2 | ZUGFeRD_XML_SchemaValidation | Integration | xUnit | ⏳ |
| 3 | HybridPDF_ContainsEmbeddedXML | Unit | xUnit | ⏳ |
| 4 | UBL_Format_Supported | Unit | xUnit | ⏳ |
| 5 | DigitalSignature_XAdES_Works | Unit | xUnit | ⏳ |
| 6 | SignatureVerification_Works | Integration | xUnit | ⏳ |
| 7 | Invoice_Archived_10Years | Integration | xUnit | ⏳ |
| 8 | Invoice_Archive_Immutable | Integration | xUnit | ⏳ |
| 9 | ERP_Import_ZUGFeRD_Works | Integration | xUnit | ⏳ |
| 10 | Webhook_Transmission_Works | Integration | xUnit | ⏳ |

---

## ⚡ Quick Commands

```bash
# Run ALL backend tests
dotnet test B2Connect.slnx -v minimal

# Run specific test category
dotnet test --filter "Category=Compliance"
dotnet test --filter "Category=Security"
dotnet test --filter "Category=Integration"

# Run specific P0 component tests
dotnet test --filter "FullyQualifiedName~P0.6"
dotnet test --filter "FullyQualifiedName~P0.7"
dotnet test --filter "FullyQualifiedName~P0.8"
dotnet test --filter "FullyQualifiedName~P0.9"

# Run E2E tests (Playwright)
cd Frontend/Store && npm run test:e2e
cd Frontend/Admin && npm run test:e2e

# Run accessibility tests (axe)
npx @axe-core/cli http://localhost:5173
npx lighthouse http://localhost:5173 --only-categories=accessibility

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage"
```

---

## 📈 Test Reporting Template

### Daily Test Report

```markdown
# Test Report - [DATE]

## Summary
| Metric | Value |
|--------|-------|
| Total Tests | 52 |
| Passed | XX |
| Failed | XX |
| Skipped | XX |
| Coverage | XX% |

## P0 Component Status
| Component | Tests | Passed | Failed | Coverage |
|-----------|-------|--------|--------|----------|
| P0.6 E-Commerce | 15 | XX | XX | XX% |
| P0.7 AI Act | 15 | XX | XX | XX% |
| P0.8 BITV | 12 | XX | XX | XX% |
| P0.9 E-Rechnung | 10 | XX | XX | XX% |

## Failed Tests
| Test | Reason | Assigned To | ETA |
|------|--------|-------------|-----|
| [Name] | [Error] | [Dev] | [Date] |

## Blockers
- [List any blocking issues]

## Next Steps
- [Planned actions]
```

### Weekly Test Summary

```markdown
# Weekly Test Summary - Week [X]

## Progress
| Metric | Last Week | This Week | Target |
|--------|-----------|-----------|--------|
| Tests Implemented | XX | XX | 52 |
| Tests Passing | XX | XX | 52 |
| Coverage | XX% | XX% | 80% |
| Critical Bugs | XX | XX | 0 |

## Milestones
- [ ] P0.6 Tests Complete
- [ ] P0.7 Tests Complete
- [ ] P0.8 Tests Complete
- [ ] P0.9 Tests Complete
- [ ] Coverage >= 80%
- [ ] All Critical Fixed
```

---

## 🔧 Test Environment Setup

### Backend Testing
```bash
# Prerequisites
- .NET 10 SDK
- PostgreSQL 16 (or InMemory for unit tests)
- Redis (for integration tests)

# Setup
dotnet restore B2Connect.slnx
dotnet build B2Connect.slnx

# Run with test database
export Database__Provider=inmemory
dotnet test
```

### Frontend Testing (Accessibility)
```bash
# Prerequisites
- Node.js 20+
- Chrome (for Lighthouse)
- NVDA (Windows) or VoiceOver (macOS)

# Setup
cd Frontend/Store
npm install
npm run dev

# Run axe tests
npx @axe-core/cli http://localhost:5173 --exit
```

### E2E Testing (Playwright)
```bash
# Prerequisites
- Playwright installed

# Setup
npx playwright install

# Run
cd Frontend/Store
npm run test:e2e

# With specific browser
npx playwright test --project=chromium
```

---

## 📋 Acceptance Criteria Verification

### P0.6 E-Commerce Acceptance
- [ ] 15/15 tests passing
- [ ] VAT calculation verified (B2C + B2B)
- [ ] VIES integration working
- [ ] Invoice generation correct
- [ ] Withdrawal period enforced

### P0.7 AI Act Acceptance
- [ ] 15/15 tests passing
- [ ] All AI systems in risk register
- [ ] Decision logging functional
- [ ] Bias tests running
- [ ] Explanation API working

### P0.8 BITV Acceptance
- [ ] 12/12 tests passing
- [ ] Lighthouse Accessibility >= 90
- [ ] axe: 0 critical issues
- [ ] NVDA manual test passed
- [ ] All images have alt text

### P0.9 E-Rechnung Acceptance
- [ ] 10/10 tests passing
- [ ] ZUGFeRD schema validates
- [ ] UBL format works
- [ ] 10-year archive functional
- [ ] Webhook tested

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Test Environment Down | DevOps | < 1h |
| Test Blocker (Bug) | Backend/Frontend Dev | < 4h |
| Flaky Test | Assigned Dev | < 24h |
| Coverage Gap | Tech Lead | < 24h |
| Compliance Question | Legal Officer | < 24h |

---

## ✅ Definition of Done (QA)

Before Phase 0 Gate approval:

- [ ] All 52 tests implemented
- [ ] All 52 tests passing
- [ ] Coverage >= 80%
- [ ] 0 critical bugs open
- [ ] 0 high bugs open
- [ ] Test reports generated
- [ ] Regression suite stable
- [ ] Manual accessibility test passed

---

**Next:** Start with [TESTING_FRAMEWORK_GUIDE.md](../TESTING_FRAMEWORK_GUIDE.md)
