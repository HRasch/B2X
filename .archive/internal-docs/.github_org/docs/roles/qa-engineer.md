# 🧪 QA Engineer Quick Start

**Role Focus:** Test automation, compliance verification, test reporting  
**Time to Productivity:** 2 weeks  
**Critical Components:** P0.6, P0.7, P0.8, P0.9 (52 total tests)

---

## ⚡ Week 1: Testing Framework Setup

### Day 1-2: Understand Test Structure
```
Backend Testing:
  - xUnit for unit/integration tests
  - Moq for mocking dependencies
  - InMemory database for isolation
  - FluentAssertions for readable assertions

Frontend Testing:
  - Vitest for unit tests
  - Playwright for E2E tests
  - axe-core for accessibility testing
  - Lighthouse for performance testing
```

### Day 3: Run Existing Tests
```bash
# Run ALL backend tests
dotnet test B2Connect.slnx -v minimal

# Run specific test category
dotnet test --filter "Category=Security"
dotnet test --filter "Category=Compliance"

# Run specific service tests
dotnet test backend/Domain/Identity/tests/B2Connect.Identity.Tests.csproj

# Frontend tests
cd Frontend/Store && npm run test

# E2E tests
cd Frontend/Admin && npm run test:e2e
```

### Day 4: Test Patterns
```csharp
// AAA Pattern: Arrange, Act, Assert
[Fact]
public async Task CreateProduct_ValidInput_SavesSuccessfully()
{
    // Arrange: Setup test data
    var command = new CreateProductCommand 
    { 
        Sku = "TEST-001", 
        Name = "Test Product" 
    };
    var handler = new CreateProductHandler(_repository.Object);
    
    // Act: Perform action
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert: Verify expectations
    Assert.NotNull(result);
    Assert.Equal("TEST-001", result.Sku);
    _repository.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
}
```

### Day 5: Coverage Tools
```bash
# Measure code coverage
dotnet test --collect:"XPlat Code Coverage"

# View coverage report
# Files: **/coverage.cobertura.xml
# Use tool: ReportGenerator or Codecov

# Target: 80%+ coverage
```

---

## 📋 Week 2: Compliance Tests (52 Total)

### Day 1-2: Test Specifications
```
P0.6 E-Commerce (15 tests)
  - VAT calculation (B2C + B2B)
  - VIES validation
  - Invoice generation
  - Withdrawal period enforcement
  - Price display compliance
  - Impressum visibility

P0.7 AI Act (15 tests)
  - Risk register documented
  - Decision logging working
  - Bias testing framework
  - Transparency logs available
  - User explanation API

P0.8 BITV Accessibility (12 tests)
  - Keyboard navigation
  - Screen reader support
  - Color contrast
  - Text resize
  - Video captions
  - Alt text for images

P0.9 E-Rechnung (10 tests)
  - ZUGFeRD 3.0 validation
  - UBL format support
  - Digital signature
  - 10-year archival
  - ERP webhook integration
```

### Day 3: Test Implementation Example
```csharp
// P0.6: E-Commerce VAT Test
[Theory]
[InlineData("DE", 0.19)]  // Germany: 19% VAT
[InlineData("AT", 0.20)]  // Austria: 20% VAT
[InlineData("CH", 0.077)] // Switzerland: 7.7% VAT
public void CalculatePrice_AppliesCorrectVatRate(string country, decimal expectedRate)
{
    // Arrange
    var service = new PriceCalculationService();
    var productPrice = 100m;
    
    // Act
    var breakdown = service.CalculateFinalPrice(
        productPrice, 
        country, 
        ShippingMethod.Standard
    );
    
    // Assert
    Assert.Equal(expectedRate, breakdown.VatRate);
    Assert.Equal(productPrice * (1 + expectedRate), breakdown.FinalPrice);
}

// P0.7: AI Act Decision Logging Test
[Fact]
public async Task FraudDetection_LogsDecision()
{
    // Arrange
    var transaction = new Transaction { Amount = 5000, Country = "DE" };
    var service = new FraudDetectionService(_logger, _logs);
    
    // Act
    var result = await service.CheckTransactionAsync(Guid.NewGuid(), transaction);
    
    // Assert
    var log = await _logs.GetAsync(x => x.DecisionType == "FraudCheck");
    Assert.NotNull(log);
    Assert.True(log.WasHumanReviewed || log.ConfidenceScore < 0.9);
}

// P0.8: Accessibility Test
[Fact]
public async Task ProductCard_HasAccessibleLabel()
{
    // Use Playwright to check accessibility
    await page.GotoAsync("http://localhost:5173");
    
    // Check heading
    var heading = await page.GetByRole("heading", new() { Name = "Test Product" });
    Assert.True(await heading.IsVisibleAsync());
    
    // Check button aria-label
    var button = await page.GetByRole("button", new() { Name = /Add to cart/i });
    var ariaLabel = await button.GetAttributeAsync("aria-label");
    Assert.NotNull(ariaLabel);
}
```

### Day 4-5: Test Automation
```bash
# Run compliance tests
dotnet test --filter "Category=Compliance" -v minimal

# Generate test report
dotnet test --logger "trx;LogFileName=test-results.trx"

# Accessibility automated tests
npx @axe-core/cli http://localhost:5173 --exit

# E2E tests with Playwright
cd Frontend/Admin && npm run test:e2e

# Performance baseline
npm run test:performance
```

---

## ⚡ Quick Commands

```bash
# Backend tests
dotnet test B2Connect.slnx -v minimal           # All tests
dotnet test --filter "FullyQualifiedName~P0.6" # P0.6 tests only
dotnet test --filter "Category=Compliance"     # Compliance tests

# Frontend tests
npm run test                                     # Unit tests
npm run test:e2e                                # E2E tests
npx @axe-core/cli http://localhost:5173        # Accessibility

# Coverage
dotnet test --collect:"XPlat Code Coverage"    # Generate coverage
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage"

# Test reporting
dotnet test --logger "console;verbosity=detailed"
dotnet test --logger "trx;LogFileName=results.trx"
```

---

## 📚 Critical Resources

| Topic | File | Time |
|-------|------|------|
| Testing Framework | `docs/TESTING_FRAMEWORK_GUIDE.md` | 30 min |
| Testing Guide | `docs/guides/TESTING_GUIDE.md` | 20 min |
| Compliance Tests | `docs/COMPLIANCE_TESTING_EXAMPLES.md` | 30 min |
| P0.6 E-Commerce | `docs/P0.6_ECOMMERCE_LEGAL_TESTS.md` | 30 min |
| P0.7 AI Act | `docs/P0.7_AI_ACT_TESTS.md` | 30 min |
| P0.8 BITV | `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | 30 min |
| P0.9 E-Rechnung | `docs/P0.9_ERECHNUNG_TESTS.md` | 20 min |

---

## 🎯 Test Execution Matrix

| Component | Type | Count | Status |
|-----------|------|-------|--------|
| P0.6 E-Commerce | Unit + Integration | 15 | ⏳ Planned |
| P0.7 AI Act | Unit | 15 | ⏳ Planned |
| P0.8 BITV | E2E + Manual | 12 | ⏳ Planned |
| P0.9 E-Rechnung | Unit + Integration | 10 | ⏳ Planned |
| **TOTAL** | **Mixed** | **52** | **⏳** |

---

## 📊 Test Report Template

```markdown
# Test Report - [DATE]

## Summary
| Metric | Value |
|--------|-------|
| Total Tests | 52 |
| Passed | 45 |
| Failed | 2 |
| Skipped | 5 |
| Coverage | 82% |

## P0 Component Status
| Component | Tests | Passed | Coverage |
|-----------|-------|--------|----------|
| P0.6 | 15 | 14 | 85% |
| P0.7 | 15 | 13 | 78% |
| P0.8 | 12 | 11 | 88% |
| P0.9 | 10 | 7 | 72% |

## Failed Tests
| Test | Error | Assigned | ETA |
|------|-------|----------|-----|
| VAT_Calculation_B2B | ReverseCharge logic | Backend | [Date] |
| BiasTest_Gender | Disparity detected | Backend | [Date] |

## Next Steps
- [ ] Fix reverse charge VAT calculation
- [ ] Investigate gender bias in fraud detection
- [ ] Increase coverage for P0.9 to 80%+
```

---

## ✅ Phase 0 Testing Gate

Before deploying Phase 1 features:

```
✅ Audit Logging: 5 tests passing
✅ Encryption: 5 tests passing
✅ Incident Response: 6 tests passing
✅ E-Commerce: 15 tests passing
✅ AI Act: 15 tests passing
✅ BITV: 12 tests passing
✅ E-Rechnung: 10 tests passing
✅ Coverage: >= 80%
✅ No critical bugs open
```

If ANY ❌, HOLD deployment.

---

## 🚨 Critical Compliance Tests (Priority!)

| Priority | Test | Deadline | Penalty |
|----------|------|----------|---------|
| 🔴 CRITICAL | BITV Accessibility | 28. Juni 2025 | €5K-100K |
| 🔴 CRITICAL | E-Commerce Legal | Active | €5K-300K |
| 🔴 CRITICAL | AI Act Compliance | 12. Mai 2026 | €30M |

---

**Key Reminders:**
- All 52 compliance tests must pass before production
- Coverage target: 80%+ for all code
- Test failures = blocking issues
- Accessibility tests are non-optional (BITV law)
- Every feature = unit test + integration test + E2E test
