# 🚀 Sprint 1 Implementation Kickoff

**Status:** ✅ READY TO START  
**Sprint Duration:** 2 weeks (32 hours)  
**Assigned Developer:** @HRasch  
**Start Date:** Today  
**End Date:** +2 weeks

---

## Assignment Status

| Issue | Title | Hours | Status | Link |
|-------|-------|-------|--------|------|
| #30 | B2C Price Transparency (PAngV) | 12 | ✅ **Assigned** | [View](https://github.com/HRasch/B2Connect/issues/30) |
| #31 | B2B VAT-ID Validation (Reverse Charge) | 20 | ✅ **Assigned** | [View](https://github.com/HRasch/B2Connect/issues/31) |

---

## Development Environment Status

### ✅ Build Verification
```
Command: dotnet build B2Connect.slnx
Result: SUCCESS
Duration: 7.1 seconds
Errors: 0
Warnings: 77 (Moq framework - expected)
```

### ✅ Test Verification
```
Command: dotnet test B2Connect.slnx -v minimal
Result: ALL PASSING (50+)
Duration: ~4 seconds
Test Projects:
  ✅ Identity.Tests (0.9s)
  ✅ Catalog.Tests (0.9s)
  ✅ CMS.Tests (0.9s)
  ✅ Localization.Tests (0.9s)
  ✅ Tenancy.Tests (0.9s)
  ✅ Search.Tests
  ✅ Integration.Tests
```

**Conclusion:** Environment is clean and ready for feature development.

---

## Sprint 1 Feature Branches

Create feature branches for each issue:

```bash
# Branch 1: B2C Price Transparency
git checkout -b feature/us-001-price-transparency

# Branch 2: B2B VAT-ID Validation
git checkout -b feature/us-002-vat-validation
```

---

## Issue #30: B2C Price Transparency (PAngV Compliance)

### 🎯 Objective
Implement price calculation with transparent VAT breakdown for B2C customers to comply with German PAngV (Price Indication Ordinance).

### 📋 Key Requirements
- **Display VAT explicitly** on all product pages
- **Price breakdown** before checkout
- **Multiple currency support** (EUR minimum)
- **Dynamic VAT rates** per country
- **Audit trail** of all price calculations

### 🔧 Implementation Plan

#### Step 1: Database Schema
```sql
-- Create tax_rates table
CREATE TABLE tax_rates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    country_code VARCHAR(2) NOT NULL,
    country_name VARCHAR(100),
    standard_vat_rate DECIMAL(5,2) NOT NULL,
    reduced_vat_rate DECIMAL(5,2),
    effective_date DATE NOT NULL,
    end_date DATE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(country_code, effective_date)
);

-- Sample data
INSERT INTO tax_rates (country_code, country_name, standard_vat_rate, effective_date)
VALUES 
    ('DE', 'Germany', 19.00, '2025-01-01'),
    ('AT', 'Austria', 20.00, '2025-01-01'),
    ('CH', 'Switzerland', 8.10, '2025-01-01'),
    ('FR', 'France', 20.00, '2025-01-01');
```

#### Step 2: Create Service
```csharp
// File: backend/BoundedContexts/Store/Catalog/src/Application/Services/PriceCalculationService.cs

namespace B2Connect.Store.Catalog.Application.Services;

public record PriceBreakdown(
    decimal ProductPrice,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalWithVat,
    string CurrencyCode = "EUR",
    decimal ShippingCost = 0,
    decimal FinalTotal = 0
);

public interface ITaxRateService
{
    Task<decimal> GetVatRateAsync(string countryCode);
}

public class PriceCalculationService
{
    private readonly ITaxRateService _taxService;
    private readonly ILogger<PriceCalculationService> _logger;

    public async Task<PriceBreakdown> CalculatePriceAsync(
        decimal productPrice,
        string destinationCountry,
        CancellationToken ct)
    {
        // Get VAT rate for destination
        var vatRate = await _taxService.GetVatRateAsync(destinationCountry);
        
        var vatAmount = productPrice * (vatRate / 100);
        var totalWithVat = productPrice + vatAmount;
        
        var breakdown = new PriceBreakdown(
            ProductPrice: productPrice,
            VatRate: vatRate,
            VatAmount: vatAmount,
            TotalWithVat: totalWithVat,
            CurrencyCode: "EUR"
        );
        
        _logger.LogInformation(
            "Price calculated for {Country}: {Price} EUR + {VatAmount} VAT = {Total}",
            destinationCountry, productPrice, vatAmount, totalWithVat
        );
        
        return breakdown;
    }
}
```

#### Step 3: Create Tests
```csharp
// File: backend/BoundedContexts/Store/Catalog/tests/Application/PriceCalculationServiceTests.cs

public class PriceCalculationServiceTests
{
    [Fact]
    public async Task CalculatePrice_Germany_Applies19PercentVat()
    {
        // Arrange
        var service = new PriceCalculationService(_mockTaxService, _logger);
        decimal productPrice = 100m;
        
        // Act
        var result = await service.CalculatePriceAsync(productPrice, "DE", CancellationToken.None);
        
        // Assert
        Assert.Equal(19m, result.VatAmount);  // 100 * 19%
        Assert.Equal(119m, result.TotalWithVat);
    }
    
    [Fact]
    public async Task CalculatePrice_Austria_Applies20PercentVat()
    {
        // Arrange
        var service = new PriceCalculationService(_mockTaxService, _logger);
        decimal productPrice = 100m;
        
        // Act
        var result = await service.CalculatePriceAsync(productPrice, "AT", CancellationToken.None);
        
        // Assert
        Assert.Equal(20m, result.VatAmount);
        Assert.Equal(120m, result.TotalWithVat);
    }
    
    [Theory]
    [InlineData("DE", 19.00, 100, 119)]
    [InlineData("AT", 20.00, 100, 120)]
    [InlineData("FR", 20.00, 50, 60)]
    public async Task CalculatePrice_MultipleCountries(
        string country, decimal expectedVat, decimal price, decimal expectedTotal)
    {
        // Arrange
        var service = new PriceCalculationService(_mockTaxService, _logger);
        
        // Act
        var result = await service.CalculatePriceAsync(price, country, CancellationToken.None);
        
        // Assert
        Assert.Equal(expectedTotal, result.TotalWithVat);
    }
}
```

#### Step 4: Update Product Listing
- Display final price with "incl. VAT" notation
- Show VAT breakdown on detail pages
- Implement in Store frontend (Vue.js)

### ✅ Acceptance Criteria (Issue #30)
- [ ] `PriceCalculationService` implemented
- [ ] `ITaxRateService` interface defined
- [ ] Tax rates table created with sample data
- [ ] 5+ unit tests (Germany, Austria, France, etc.)
- [ ] Store frontend displays "incl. VAT" on prices
- [ ] VAT breakdown visible on detail pages
- [ ] Audit logs capture all calculations
- [ ] API response time < 100ms
- [ ] Code review approved
- [ ] All tests passing

### 📊 Acceptance Metric
- **Done when:** Store displays prices with VAT correctly for all supported countries

---

## Issue #31: B2B VAT-ID Validation (Reverse Charge)

### 🎯 Objective
Implement VIES VAT-ID validation for B2B customers to enable reverse charge (no VAT when buyer is valid EU business).

### 📋 Key Requirements
- **VIES API integration** (EU VAT validation)
- **Reverse charge logic** (no VAT if valid VAT-ID)
- **1-year caching** of validations
- **Failure handling** (graceful degradation)
- **Audit trail** for all validations

### 🔧 Implementation Plan

#### Step 1: Database Schema
```sql
-- Create vat_id_validations table
CREATE TABLE vat_id_validations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    vat_id VARCHAR(20) NOT NULL,
    country_code VARCHAR(2) NOT NULL,
    is_valid BOOLEAN NOT NULL,
    company_name VARCHAR(255),
    company_address VARCHAR(500),
    validated_at TIMESTAMPTZ NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    api_response_code VARCHAR(10),
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(tenant_id, vat_id),
    INDEX idx_expires (expires_at)
);
```

#### Step 2: Create Service
```csharp
// File: backend/BoundedContexts/Store/Catalog/src/Application/Services/VatIdValidationService.cs

namespace B2Connect.Store.Catalog.Application.Services;

public record VatValidationResult(
    bool IsValid,
    string VatId,
    string CompanyName,
    string CompanyAddress,
    DateTime ValidatedAt,
    DateTime ExpiresAt
);

public interface IViesApiClient
{
    Task<VatValidationResult> ValidateVatIdAsync(
        string countryCode,
        string vatNumber);
}

public class VatIdValidationService
{
    private readonly IViesApiClient _viesClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<VatIdValidationService> _logger;
    
    public async Task<VatValidationResult> ValidateVatIdAsync(
        string countryCode,
        string vatId)
    {
        // Check cache first (1-year TTL)
        var cacheKey = $"vat:{countryCode}:{vatId}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<VatValidationResult>(cached);
        }
        
        try
        {
            // Call VIES API
            var result = await _viesClient.ValidateVatIdAsync(countryCode, vatId);
            
            // Cache for 1 year
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
                }
            );
            
            _logger.LogInformation(
                "VAT-ID validated: {CountryCode}{VatId} = {IsValid}",
                countryCode, vatId, result.IsValid
            );
            
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "VIES API error for {CountryCode}{VatId} - using default (invalid)",
                countryCode, vatId
            );
            
            // Fail safe: treat as invalid if API unavailable
            return new VatValidationResult(
                IsValid: false,
                VatId: vatId,
                CompanyName: null,
                CompanyAddress: null,
                ValidatedAt: DateTime.UtcNow,
                ExpiresAt: DateTime.UtcNow.AddHours(24)  // Retry tomorrow
            );
        }
    }
    
    public bool ShouldApplyReverseCharge(
        VatValidationResult validation,
        string buyerCountry,
        string sellerCountry)
    {
        // Reverse charge applies when:
        // 1. VAT-ID is valid
        // 2. Buyer and seller in different EU countries
        
        return validation.IsValid &&
               buyerCountry != sellerCountry &&
               IsEuCountry(buyerCountry);
    }
    
    private bool IsEuCountry(string countryCode)
    {
        var euCountries = new[]
        {
            "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE",
            "FI", "FR", "DE", "GR", "HU", "IE", "IT", "LV",
            "LT", "LU", "MT", "NL", "PL", "PT", "RO", "SK",
            "SI", "ES", "SE"
        };
        
        return euCountries.Contains(countryCode);
    }
}
```

#### Step 3: Create Tests
```csharp
// File: backend/BoundedContexts/Store/Catalog/tests/Application/VatIdValidationServiceTests.cs

public class VatIdValidationServiceTests
{
    [Fact]
    public async Task ValidateVatId_ValidGermanVatId_ReturnsValid()
    {
        // Arrange
        var mockVies = new Mock<IViesApiClient>();
        mockVies.Setup(x => x.ValidateVatIdAsync("DE", "123456789"))
            .ReturnsAsync(new VatValidationResult(
                IsValid: true,
                VatId: "DE123456789",
                CompanyName: "Example GmbH",
                CompanyAddress: "Main Street 123, Berlin",
                ValidatedAt: DateTime.UtcNow,
                ExpiresAt: DateTime.UtcNow.AddDays(365)
            ));
        
        var service = new VatIdValidationService(mockVies.Object, _cache, _logger);
        
        // Act
        var result = await service.ValidateVatIdAsync("DE", "123456789");
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("Example GmbH", result.CompanyName);
    }
    
    [Fact]
    public async Task ShouldApplyReverseCharge_ValidVatIdDifferentCountries_ReturnsTrue()
    {
        // Arrange
        var validation = new VatValidationResult(true, "DE123", "Test GmbH", "Address", DateTime.UtcNow, DateTime.UtcNow.AddDays(365));
        var service = new VatIdValidationService(_mockVies.Object, _cache, _logger);
        
        // Act
        var result = service.ShouldApplyReverseCharge(validation, buyerCountry: "DE", sellerCountry: "AT");
        
        // Assert
        Assert.True(result);  // Different EU countries, valid VAT-ID
    }
    
    [Fact]
    public async Task ShouldApplyReverseCharge_SameCountry_ReturnsFalse()
    {
        // Arrange
        var validation = new VatValidationResult(true, "DE123", "Test GmbH", "Address", DateTime.UtcNow, DateTime.UtcNow.AddDays(365));
        var service = new VatIdValidationService(_mockVies.Object, _cache, _logger);
        
        // Act
        var result = service.ShouldApplyReverseCharge(validation, buyerCountry: "DE", sellerCountry: "DE");
        
        // Assert
        Assert.False(result);  // Same country - no reverse charge
    }
}
```

#### Step 4: Integrate into Checkout
- Validate VAT-ID during B2B checkout
- Apply reverse charge if valid
- Store validation in audit log
- Update order with VAT calculation

### ✅ Acceptance Criteria (Issue #31)
- [ ] `VatIdValidationService` implemented
- [ ] `IViesApiClient` interface defined
- [ ] VIES API integration working
- [ ] Cache configured (365-day TTL)
- [ ] Reverse charge logic working
- [ ] 5+ unit tests (valid/invalid VAT-IDs, reverse charge scenarios)
- [ ] B2B checkout flow updated
- [ ] Failure handling graceful (API down → safe default)
- [ ] Audit trail captures all validations
- [ ] API response time < 500ms (with cache)
- [ ] Code review approved
- [ ] All tests passing

### 📊 Acceptance Metric
- **Done when:** B2B checkout applies reverse charge for valid EU VAT-IDs from different countries

---

## Daily Standup Template

### Format
```
Yesterday:
  - [Task 1]: Completed / In Progress / Blocked
  - [Task 2]: Completed / In Progress / Blocked

Today:
  - [Next task]: Planning to...
  - [Next task]: Planning to...

Blockers:
  - [If any]

Questions:
  - [If any]
```

### Example
```
Yesterday:
  - Issue #30 database schema: ✅ Completed
  - PriceCalculationService: ✅ Implemented

Today:
  - Unit tests for #30: Starting
  - Frontend integration: Planning

Blockers: None

Questions: Should VAT rates be configured per shop or global?
```

---

## Resources & References

### Documentation
- [Application Specifications](./docs/APPLICATION_SPECIFICATIONS.md)
- [P0.6 Backlog](./P0.6_COMPLETION_REPORT.md)
- [Copilot Instructions](./\.github/copilot-instructions.md)
- [Onion Architecture](./docs/ONION_ARCHITECTURE.md)

### Code Examples
- [CheckRegistrationTypeService](./backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs) - Wolverine pattern reference
- [Entity Extensions Implementation](./docs/ENTITY_EXTENSIONS_IMPLEMENTATION.md)

### Key Technologies
- **Framework**: .NET 10 with Wolverine HTTP
- **Testing**: xUnit with Moq
- **ORM**: Entity Framework Core (InMemory for tests)
- **Validation**: FluentValidation
- **API Integration**: HttpClient with IHttpClientFactory

---

## Commits & Pull Requests

### Commit Message Format
```
[#30] P0.6-US-001: Add PriceCalculationService

- Implement PriceCalculationService for VAT calculation
- Create tax_rates table with EU country rates
- Add unit tests (Germany, Austria, France)
- Wire up to Store API endpoint

Closes #30
```

### Pull Request Template
```markdown
## Issue
Closes #30 (P0.6-US-001: B2C Price Transparency)

## Changes
- [ ] Database schema created
- [ ] Service implemented
- [ ] Unit tests added
- [ ] Integration tests added
- [ ] Frontend updated

## Testing
- [ ] All tests passing
- [ ] Manual testing completed
- [ ] Performance validated

## Checklist
- [ ] Code follows project conventions
- [ ] Documentation updated
- [ ] No breaking changes
- [ ] Audit logging added
```

---

## Success Criteria (Sprint 1 Complete)

### Issue #30 Success
- ✅ B2C customers see "incl. VAT" on all prices
- ✅ VAT breakdown visible on detail pages
- ✅ Works for 10+ EU countries
- ✅ API response < 100ms
- ✅ All tests passing

### Issue #31 Success
- ✅ B2B customers can enter VAT-ID
- ✅ VIES validation working
- ✅ Reverse charge applied correctly
- ✅ Cache working (365 days)
- ✅ All tests passing

### Overall Sprint Success
- ✅ 32 hours delivered on schedule
- ✅ 0 critical bugs
- ✅ Code review approved
- ✅ Regulations compliance verified (PAngV, AStV)
- ✅ Ready for Phase 1 production deployment

---

## Contact & Support

| Item | Contact |
|------|---------|
| **Questions about #30 (Price)** | @HRasch - See Issue #30 |
| **Questions about #31 (VAT-ID)** | @HRasch - See Issue #31 |
| **Architecture questions** | Review [DDD_BOUNDED_CONTEXTS.md](./docs/architecture/DDD_BOUNDED_CONTEXTS.md) |
| **Database questions** | Review [Onion Architecture](./docs/ONION_ARCHITECTURE.md) |
| **Regulatory questions** | See [P0.6_ECOMMERCE_LEGAL_TESTS.md](./docs/P0.6_ECOMMERCE_LEGAL_TESTS.md) |

---

**🎯 You're all set! Start with Issue #30 (12 hours) or #31 (20 hours) - both are ready to begin.**

**Good luck with Sprint 1! 🚀**
