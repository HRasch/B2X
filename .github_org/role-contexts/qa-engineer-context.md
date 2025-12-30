# 🧪 QA Engineer Agent Context

> ⚠️ **DEPRECATED**: Core content merged into `.github/agents/QA.agent.md` on 30.12.2025
> 
> **This file contains DEEP REFERENCE material only**:
> - Full 52 compliance tests table with assertions
> - Complete test templates (xUnit, Integration, Playwright)
> - Test execution commands

**Version:** 1.0 | **Focus:** Compliance Test Automation & Verification | **Last Updated:** 28. Dezember 2025

---

## 🎯 Your Mission

**52 Compliance Tests = Gate for Production Deployment**

Your job: Ensure every feature passes compliance tests before it ships. These tests are **non-negotiable blockers**:
- ✅ ALL 52 tests passing = Go to production
- ❌ ANY test failing = HOLD deployment

**Test Distribution:**
- P0.6 (E-Commerce): 15 tests
- P0.7 (AI Act): 15 tests
- P0.8 (BITV Accessibility): 12 tests
- P0.9 (E-Rechnung): 10 tests

---

## 📚 ONLY READ THESE (Don't Read Anything Else)

### Critical Entry Points
1. **[docs/by-role/QA_ENGINEER.md](../../docs/by-role/QA_ENGINEER.md)** - Your guide (START HERE)
2. **[docs/TESTING_FRAMEWORK_GUIDE.md](../../docs/TESTING_FRAMEWORK_GUIDE.md)** - Test patterns & setup
3. **Test Specification Files (One Per P0 Component):**
   - [docs/P0.6_ECOMMERCE_LEGAL_TESTS.md](../../docs/P0.6_ECOMMERCE_LEGAL_TESTS.md) - 15 tests
   - [docs/P0.7_AI_ACT_TESTS.md](../../docs/P0.7_AI_ACT_TESTS.md) - 15 tests
   - [docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md](../../docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md) - 12 tests
   - [docs/P0.9_ERECHNUNG_TESTS.md](../../docs/P0.9_ERECHNUNG_TESTS.md) - 10 tests

### Reference Only (Use When Needed)
- `docs/TESTING_FRAMEWORK_GUIDE.md` - General testing patterns
- `docs/guides/TESTING_GUIDE.md` - Test execution guide
- `docs/COMPLIANCE_TESTING_EXAMPLES.md` - Code examples
- `docs/guides/DEBUGGING_GUIDE.md` - Debugging test failures

---

## 📊 The 52 Compliance Tests (Your Primary Responsibility)

### P0.6: E-Commerce Legal (15 Tests)

| # | Test Name | Type | Assertion |
|---|-----------|------|-----------|
| 1 | VAT_Calculation_B2C_IncludesCorrectRate | Unit | Verify 19% VAT on German orders |
| 2 | VAT_Calculation_B2B_ReverseCharge_NoVAT | Unit | Verify 0% VAT when B2B VAT-ID valid |
| 3 | VIES_VatIdValidation_ValidId_ReturnsTrue | Integration | Call VIES, verify valid ID accepted |
| 4 | VIES_VatIdValidation_InvalidId_ReturnsFalse | Integration | Call VIES, verify invalid ID rejected |
| 5 | Invoice_Generation_CreatesValidPDF | Unit | Verify PDF file created with content |
| 6 | Invoice_Archival_Stores10Years_Immutable | Integration | Verify 10-year storage, immutability |
| 7 | Withdrawal_Request_Within14Days_Accepted | Unit | VVVG §357 - Accept if within 14 days |
| 8 | Withdrawal_Request_After14Days_Rejected | Unit | VVVG §357 - Reject if after 14 days |
| 9 | Price_Display_ShowsInclVAT_Always | E2E | Verify price includes VAT on UI |
| 10 | Shipping_Cost_VisibleBeforeCheckout | E2E | Verify shipping visible before order |
| 11 | AGB_Checkbox_RequiredBeforeOrder | E2E | Verify checkbox mandatory |
| 12 | Impressum_Link_Accessible | E2E | Verify link works, content present |
| 13 | Datenschutz_Link_Accessible | E2E | Verify link works, GDPR-compliant |
| 14 | Order_Confirmation_Email_Sent | Integration | Verify email sent after order |
| 15 | Return_Label_Generation_Works | Integration | Verify return label created |

### P0.7: AI Act (15 Tests)

| # | Test Name | Type | Assertion |
|---|-----------|------|-----------|
| 1 | AiRiskRegister_AllSystemsDocumented | Unit | All AI systems in register |
| 2 | FraudDetection_ClassifiedAsHighRisk | Unit | Fraud detection = HIGH-RISK |
| 3 | AiDecisionLog_Created_ForEveryDecision | Integration | Decision logged per AI call |
| 4 | AiDecisionLog_IncludesExplanation | Unit | Log has user-friendly explanation |
| 5 | BiasTest_NoGenderDisparity | Unit | Test across genders, < 5% delta |
| 6 | BiasTest_NoAgeDisparity | Unit | Test across ages, < 5% delta |
| 7 | BiasTest_NoRegionDisparity | Unit | Test across regions, < 5% delta |
| 8 | PerformanceMonitor_DetectsDrift | Unit | Alert if accuracy drops > 5% |
| 9 | HighRiskDecision_FlaggedForReview | Integration | HIGH-RISK decisions queued for human review |
| 10 | HumanOverride_Logged | Integration | Human overrides recorded |
| 11 | UserExplanation_API_ReturnsDetails | Integration | User can request decision explanation |
| 12 | AiSystem_HasResponsiblePerson | Unit | Each system has assigned person |
| 13 | ModelVersion_Tracked | Unit | Version history maintained |
| 14 | TrainingData_Documented | Unit | Training data source documented |
| 15 | Limitations_Documented | Unit | Known limitations documented |

### P0.8: BITV Accessibility (12 Tests)

| # | Test Name | Type | Assertion |
|---|-----------|------|-----------|
| 1 | Keyboard_Navigation_AllFunctional | E2E | TAB, ENTER, Escape work |
| 2 | Keyboard_Shortcuts_Work | E2E | Keyboard shortcuts functional |
| 3 | Modal_ClosableWithEscape | E2E | Modal closes with Escape |
| 4 | ScreenReader_SemanticHTML | E2E | Valid ARIA labels present |
| 5 | ScreenReader_FormLabels | E2E | All inputs have labels |
| 6 | ScreenReader_ErrorAnnouncements | E2E | Errors announced to screen reader |
| 7 | ColorContrast_MeetsWCAG_AA | E2E | Contrast ratio >= 4.5:1 |
| 8 | TextResize_200Percent_NoBreakage | E2E | 200% zoom doesn't break layout |
| 9 | Video_HasCaptions_DE | E2E | German captions present |
| 10 | Video_HasCaptions_EN | E2E | English captions present |
| 11 | PageTitles_Unique | E2E | Each page has unique title |
| 12 | Images_HaveAltText | E2E | All images have descriptive alt text |

### P0.9: E-Rechnung (10 Tests)

| # | Test Name | Type | Assertion |
|---|-----------|------|-----------|
| 1 | ZUGFeRD_XML_ValidStructure | Unit | XML structure matches spec |
| 2 | ZUGFeRD_XML_SchemaValidation | Integration | Validate against EN 16931 schema |
| 3 | HybridPDF_ContainsEmbeddedXML | Unit | XML embedded in PDF |
| 4 | UBL_Format_Supported | Unit | UBL 2.3 format works |
| 5 | DigitalSignature_XAdES_Works | Unit | XAdES signature applied |
| 6 | SignatureVerification_Works | Integration | Signature verifies correctly |
| 7 | Invoice_Archived_10Years | Integration | Archive retention = 10 years |
| 8 | Invoice_Archive_Immutable | Integration | Cannot modify archived invoice |
| 9 | ERP_Import_ZUGFeRD_Works | Integration | SAP/Oracle can import |
| 10 | Webhook_Transmission_Works | Integration | ERP receives invoice via webhook |

---

## 🏗️ Test File Structure

Create one test file per feature:

```
backend/Domain/[Service]/tests/
├── P0.6_EcommerceTests.cs          # All 15 E-Commerce tests
├── P0.7_AiActTests.cs              # All 15 AI Act tests
├── P0.8_AccessibilityTests.cs      # All 12 BITV tests (Playwright)
├── P0.9_ERechnungTests.cs          # All 10 E-Rechnung tests
└── ComplianceTestBase.cs           # Shared setup
```

---

## ✅ Test Writing Template (Copy This)

### Unit Test (xUnit)

```csharp
public class P0_6_EcommerceTests
{
    private readonly IVatCalculationService _vatService;
    private readonly IInvoiceService _invoiceService;
    
    [Fact]
    public void VAT_Calculation_B2C_IncludesCorrectRate()
    {
        // Arrange
        decimal productPrice = 100m;
        string country = "DE";
        decimal expectedVatRate = 0.19m;  // German VAT
        
        // Act
        var vatAmount = _vatService.CalculateVat(productPrice, country);
        
        // Assert
        Assert.Equal(productPrice * expectedVatRate, vatAmount);
    }
    
    [Fact]
    public void Withdrawal_Request_Within14Days_Accepted()
    {
        // Arrange
        var deliveryDate = DateTime.UtcNow.AddDays(-10);  // 10 days ago
        var order = new Order { DeliveredAt = deliveryDate };
        
        // Act
        var result = order.IsWithinWithdrawalPeriod();
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Withdrawal_Request_After14Days_Rejected()
    {
        // Arrange
        var deliveryDate = DateTime.UtcNow.AddDays(-15);  // 15 days ago
        var order = new Order { DeliveredAt = deliveryDate };
        
        // Act
        var result = order.IsWithinWithdrawalPeriod();
        
        // Assert
        Assert.False(result);
    }
}
```

### Integration Test (Database)

```csharp
public class P0_6_InvoiceTests : IAsyncLifetime
{
    private readonly DbContext _context;
    private readonly IInvoiceService _invoiceService;
    
    public async Task InitializeAsync()
    {
        _context = new InMemoryDbContext();
        _invoiceService = new InvoiceService(_context);
    }
    
    [Fact]
    public async Task Invoice_Archival_Stores10Years_Immutable()
    {
        // Arrange
        var invoice = new Invoice { InvoiceNumber = "INV-001", Amount = 100m };
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
        
        // Act
        var storedInvoice = await _invoiceService.GetAsync("INV-001");
        
        // Assert
        Assert.NotNull(storedInvoice);
        Assert.Equal("INV-001", storedInvoice.InvoiceNumber);
        
        // Verify immutability
        storedInvoice.Amount = 200m;
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _context.SaveChangesAsync()
        );
        Assert.Contains("immutable", ex.Message);
    }
}
```

### E2E Test (Playwright)

```csharp
public class P0_8_AccessibilityTests : PageTest
{
    [Test]
    public async Task Keyboard_Navigation_AllFunctional()
    {
        // Navigate to checkout
        await Page.GotoAsync("http://localhost:5173/checkout");
        
        // TAB through form fields
        var firstField = Page.Locator("input[name='email']");
        await firstField.FocusAsync();
        
        // ENTER should proceed if all valid
        await firstField.Fill("test@example.com");
        await Page.Keyboard.PressAsync("Tab");
        
        // Verify focus moved to next field
        var secondField = Page.Locator("input[name='address']");
        Assert.NotNull(await secondField.EvaluateAsync("el => el === document.activeElement"));
    }
    
    [Test]
    public async Task ColorContrast_MeetsWCAG_AA()
    {
        // Install axe accessibility scanner
        await Page.AddScriptTagAsync(new() { Url = "https://cdnjs.cloudflare.com/ajax/libs/axe-core/4.7.2/axe.min.js" });
        
        // Scan page
        var results = await Page.EvaluateAsync<JObject>(@"
            async () => {
                const results = await new Promise(resolve => {
                    axe.run((error, results) => resolve(results));
                });
                return results;
            }
        ");
        
        // Assert no color contrast violations
        var violations = results["violations"];
        var contrastViolations = violations
            .Where(v => v["id"].ToString() == "color-contrast")
            .ToList();
        
        Assert.Empty(contrastViolations);
    }
}
```

---

## 🚀 Test Execution Commands

```bash
# Run ALL compliance tests
dotnet test B2Connect.slnx --filter "Category=Compliance"

# Run specific P0 component
dotnet test --filter "FullyQualifiedName~P0.6"
dotnet test --filter "FullyQualifiedName~P0.7"
dotnet test --filter "FullyQualifiedName~P0.8"
dotnet test --filter "FullyQualifiedName~P0.9"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage" B2Connect.slnx

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage"

# E2E tests (Playwright)
cd Frontend/Store && npm run test:e2e

# Accessibility tests (axe)
npx @axe-core/cli http://localhost:5173
npx lighthouse http://localhost:5173 --only-categories=accessibility
```

---

## 📋 Test Status Dashboard Template

```markdown
# Compliance Test Report - Week X

## Summary
| Metric | Value | Target |
|--------|-------|--------|
| Total Tests | 52 | 52 |
| Passing | XX | 52 |
| Failing | XX | 0 |
| Code Coverage | XX% | 80% |

## P0.6: E-Commerce (15 Tests)
| Test | Status | Notes |
|------|--------|-------|
| VAT_Calculation_B2C | ✅/❌ | |
| Withdrawal_Within14Days | ✅/❌ | |
| ... | | |

## P0.7: AI Act (15 Tests)
| Test | Status | Notes |
|------|--------|-------|
| AiRiskRegister | ✅/❌ | |
| BiasTest_GenderDisparity | ✅/❌ | |
| ... | | |

## P0.8: BITV (12 Tests)
| Test | Status | Notes |
|------|--------|-------|
| Keyboard_Navigation | ✅/❌ | |
| ColorContrast | ✅/❌ | |
| ... | | |

## P0.9: E-Rechnung (10 Tests)
| Test | Status | Notes |
|------|--------|-------|
| ZUGFeRD_XML | ✅/❌ | |
| DigitalSignature | ✅/❌ | |
| ... | | |

## Blockers
- [Blocker 1] - Assigned to [Dev]
```

---

## 🔧 Tools & Frameworks You'll Use

### Backend Testing
- **xUnit** - Unit & integration tests
- **Moq** - Mocking
- **FluentAssertions** - Assertions
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database

### Frontend/Accessibility Testing
- **Playwright** - E2E tests
- **axe DevTools** - Accessibility scanning
- **Lighthouse** - Performance & accessibility audit
- **NVDA/VoiceOver** - Screen reader manual testing

### Test Reporting
- **OpenCover** - Code coverage
- **ReportGenerator** - Coverage reports
- **xUnit.Xml.TestLogger** - XML reports for CI/CD

---

## ✅ Quality Standards

### Every Test MUST Have

- [ ] **Arrange** section (setup)
- [ ] **Act** section (execute)
- [ ] **Assert** section (verify)
- [ ] **Descriptive name** (TestName_Condition_ExpectedResult)
- [ ] **Single assertion** (one thing per test)
- [ ] **No hardcoded values** (use constants or factories)
- [ ] **Cleanup** (if needed)

### Tests to AVOID

- ❌ Tests that depend on each other
- ❌ Tests with hardcoded paths/URLs
- ❌ Tests that randomly fail (flaky tests)
- ❌ Tests that test multiple things
- ❌ Tests with no assertions
- ❌ Tests that sleep/wait (use WaitFor instead)

---

## 🚫 Red Flags (If You See These, Something's Wrong)

- ❌ Test passes locally but fails in CI/CD
- ❌ Test takes > 5 seconds to run
- ❌ Test requires manual setup
- ❌ Test uses Thread.Sleep
- ❌ Test depends on external service (mock it!)
- ❌ Test has hardcoded IP/port
- ❌ Test modifies shared state

---

## 📊 Phase Gate Criteria (Your Responsibility)

### Before Phase 1 Deployment (Week 14)
```
✅ P0.6: 15/15 tests passing
✅ P0.7: 15/15 tests passing  
✅ P0.8: 12/12 tests passing (Accessibility audit >= 90)
✅ P0.9: 10/10 tests passing
✅ Overall code coverage: >= 80%
✅ 0 critical bugs open
✅ 0 high bugs open

If ANY ❌ → HOLD Phase 1 deployment
```

---

## 🎯 Weekly Rhythm

| Day | Activity | Deliverable |
|-----|----------|-------------|
| Mon | Test Planning | Test spec for upcoming features |
| Tue-Wed | Test Implementation | Write tests as features complete |
| Thu | Test Execution | Run full suite, report results |
| Fri | Regression Testing | Verify no regressions, update report |

---

## 📞 When Tests Fail

1. **Read the error message** - What exactly failed?
2. **Check the test code** - Is the test correct?
3. **Check the feature code** - Did the developer implement correctly?
4. **Create a bug** - If feature code is wrong
5. **Update test** - If test expectation was wrong
6. **Document** - Why this test exists

---

**Ready? Start with:** [docs/by-role/QA_ENGINEER.md](../../docs/by-role/QA_ENGINEER.md)
