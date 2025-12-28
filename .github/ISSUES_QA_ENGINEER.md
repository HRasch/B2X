# QA Engineer - Zugeordnete Issues

**Status**: 0/3 Assigned  
**Gesamtaufwand**: ~30 Story Points (52 Test Cases)  
**Kritischer Pfad**: Sprint 2-4 (Compliance Testing)

---

## Compliance Test Suites (52 Total Tests)

### P0.6: E-Commerce Legal Compliance Tests (15 Tests)

| Test Suite | Coverage | Sprint |
|-----------|----------|--------|
| E-Commerce Legal Tests | VAT, Return Policy, Terms & Conditions | Sprint 2 |

**Test Cases**:
```
✓ VAT Calculation (Brutto = Netto + VAT)
✓ VAT Display (correct format for DE/AT/CH)
✓ Reverse Charge Calculation (B2B valid VAT-ID)
✓ Reverse Charge Rejection (invalid VAT-ID)
✓ 14-Day Return Process
✓ Return Shipping Costs
✓ Refund Timeline Enforcement
✓ Terms & Conditions Acceptance (checkbox)
✓ Impressum Link Presence
✓ Privacy Policy Link Presence
✓ Withdrawal Rights Display
✓ Product Description Accuracy
✓ Price Transparency (no hidden costs)
✓ Dispute Resolution Process
✓ Invoice Generation & Delivery
```

**Owner Issue**: #45

---

### P0.7: AI Act Compliance Tests (15 Tests)

| Test Suite | Coverage | Sprint |
|-----------|----------|--------|
| AI Act Tests | Fraud Detection, Transparency, Audit | Sprint 3 |

**Test Cases** (If applicable):
```
✓ Payment Fraud Detection logging
✓ Human Fallback available
✓ Decision explanation provided
✓ Audit trail completeness
✓ High-risk AI system disclosure
✓ User notification on AI decision
✓ Performance monitoring
✓ Bias detection
✓ Explainability validation
✓ Override capability
✓ Incident response
✓ Documentation completeness
✓ Training data quality
✓ Model versioning
✓ Impact assessment
```

**Owner Issue**: #46 (Automation Framework)

---

### P0.8: BITV 2.0 / BFSG Accessibility Tests (12 Tests)

| Test Suite | Coverage | Sprint |
|-----------|----------|--------|
| Accessibility (WCAG 2.1 AA) | Screen readers, Keyboard, Contrast | Sprint 3 |

**Test Cases**:
```
✓ Screen Reader Compatibility
✓ Keyboard Navigation (Tab order)
✓ Color Contrast Ratios (4.5:1 minimum)
✓ Focus Indicators
✓ Alt Text for Images
✓ Form Labels Present
✓ Error Messages Descriptive
✓ Page Structure (Headings hierarchy)
✓ Language Declaration
✓ Skip Navigation Links
✓ Responsive Design (Mobile)
✓ Text Resize (up to 200%)
```

**Deadline**: 28. Juni 2025 (BFSG)

**Owner Issue**: #43 (Legal Review) + QA Implementation

---

### P0.9: E-Rechnung (ZUGFeRD/UBL) Tests (10 Tests)

| Test Suite | Coverage | Sprint |
|-----------|----------|--------|
| E-Invoice Tests | XML Format, Validation, Delivery | Sprint 4 |

**Test Cases**:
```
✓ ZUGFeRD 2.0 XML Generation
✓ UBL Format Compliance
✓ Mandatory Fields Present (Seller, Buyer, Amount)
✓ Tax Calculation in Invoice
✓ Invoice Numbering Sequential
✓ Timestamp Accuracy
✓ Digital Signature Validation
✓ Recipient Routing (via B2GNow)
✓ Delivery Confirmation
✓ Archival Compliance (10 years)
```

**Owner Issue**: #46 (Framework) + Backend #21

---

## Issue Mapping

| QA Issue | Scope | Sprint | Effort |
|----------|-------|--------|--------|
| #45 | E-Commerce Legal Tests (15) | Sprint 2 | 20h |
| #46 | Test Automation Framework | Sprint 2-3 | 30h |
| #47 | Performance & Load Testing | Sprint 4 | 15h |

---

## Test Automation Framework (#46)

**Stack**:
- Test Runner: xUnit (.NET 10)
- Assertion Library: FluentAssertions
- API Testing: Spectre.Console + HTTP Client
- Database: TestContainers (PostgreSQL)
- Browser Testing: Playwright (for frontend)

**Structure**:
```
backend/Tests/
├── Compliance/
│   ├── ECommerceTests.cs (15 tests)
│   ├── AIActTests.cs (15 tests)
│   └── EInvoiceTests.cs (10 tests)
├── Fixtures/
│   ├── ComplianceTestFixture.cs
│   ├── DatabaseFixture.cs
│   └── ApiClientFixture.cs
└── README.md (Test Documentation)
```

**Example Test Structure**:
```csharp
[TestClass]
public class VatCalculationTests : IAsyncLifetime
{
    private readonly ComplianceTestFixture _fixture;
    
    [TestMethod]
    public async Task VatCalculation_ConsumerDE_IncludesTax()
    {
        // Arrange
        var product = new Product { Price = 100m, VatRate = 0.19m };
        
        // Act
        var calculation = await _service.CalculateAsync(product, "DE");
        
        // Assert
        calculation.GrossPrice.Should().Be(119m);
        calculation.VatAmount.Should().Be(19m);
    }
    
    [TestMethod]
    public async Task VatCalculation_B2B_ValidVatId_ReversedCharge()
    {
        // Arrange
        var buyer = new Business { VatId = "DE123456789" };
        var calculation = await _service.CalculateAsync(product, buyer);
        
        // Assert
        calculation.VatApplied.Should().BeFalse();
        calculation.ReverseChargeApplied.Should().BeTrue();
    }
}
```

---

## Performance Testing (#47)

**Scenarios**:
1. **Normal Load**: 1000 users × 100 shops concurrent
2. **Black Friday**: 5× normal load, sudden spike
3. **Sustained**: 24h continuous at 50% capacity

**Metrics to Validate**:
- API response time < 200ms (P95)
- Database queries < 100ms
- Invoice generation < 500ms
- No 5xx errors under load
- Memory usage < 80% capacity

---

## Nächste Schritte

1. **1 QA Engineer zuweisen** (3 Issues, 4-5 Wochen)
2. **Sprint 2**: Framework setup (#46) + E-Commerce tests (#45)
3. **Sprint 3**: Accessibility tests (#43) + AI Act tests
4. **Sprint 4**: E-Invoice tests (#47) + Performance testing
5. **Continuous**: Regression testing during development

---

## Test Execution Checklist

Vor jedem Release:

```bash
# Run all compliance tests
dotnet test backend/Tests/Compliance/ -v minimal --logger "console;verbosity=detailed"

# Run frontend accessibility tests
cd frontend-store && npm run test:a11y

# Run performance tests
dotnet test backend/Tests/Performance/ -v minimal

# Generate compliance report
./scripts/generate-compliance-report.sh
```

---

## Integration with Development

**QA responsibilities**:
- ✅ Create test specifications (based on legal requirements)
- ✅ Implement automated test cases
- ✅ Review Backend/Frontend PRs for test coverage
- ✅ Maintain compliance test suite
- ✅ Generate compliance reports

**Developer responsibilities**:
- ✅ Unit tests (80%+ coverage required)
- ✅ Integration tests (critical paths)
- ✅ Fix failing compliance tests
- ✅ No PR merge without passing tests

