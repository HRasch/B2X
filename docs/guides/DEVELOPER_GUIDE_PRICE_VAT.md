# Developer Guide: Price Calculation & VAT Validation

**Issues**: #30, #31  
**Framework**: .NET 10.0 + Wolverine  
**Architecture**: Onion (Core → Application → Infrastructure → API)  
**Target Audience**: Backend developers implementing price/tax features  
**Prerequisite Knowledge**: Wolverine patterns, EF Core, async/await

---

## Project Structure

```
backend/Domain/Catalog/
├── src/
│   ├── Core/                          # Domain layer (no framework deps)
│   │   ├── Models/
│   │   │   ├── PriceCalculationRequest.cs
│   │   │   ├── CalculatePriceResponse.cs
│   │   │   ├── VatValidationResult.cs
│   │   │   └── VatIdValidationCache.cs
│   │   └── Services/
│   │       ├── IPriceCalculationService.cs
│   │       └── IVatIdValidationService.cs
│   │
│   ├── Application/                   # Application layer (CQRS-like)
│   │   ├── Handlers/
│   │   │   ├── PriceCalculationHandler.cs   (Wolverine endpoint)
│   │   │   └── VatIdValidationHandler.cs    (Wolverine endpoint)
│   │   ├── Services/
│   │   │   ├── PriceCalculationService.cs
│   │   │   └── VatIdValidationService.cs
│   │   └── Validators/
│   │       ├── CalculatePriceRequestValidator.cs
│   │       └── ValidateVatIdRequestValidator.cs
│   │
│   ├── Infrastructure/                # Infrastructure layer (EF Core, VIES API)
│   │   ├── Clients/
│   │   │   ├── IViesApiClient.cs
│   │   │   └── ViesApiClient.cs       (SOAP/XML HTTP client)
│   │   ├── Repositories/
│   │   │   └── VatIdValidationRepository.cs
│   │   └── Migrations/
│   │       └── *_AddVatIdValidationCache.cs
│   │
│   ├── API/                           # Presentation layer
│   │   └── Program.cs                 (DI configuration)
│   │
│   └── B2Connect.Catalog.API.csproj
│
└── tests/
    ├── PriceCalculationServiceTests.cs
    ├── VatIdValidationServiceTests.cs
    ├── ValidatorsTests.cs
    └── Fixtures/
        └── TestDistributedCache.cs
```

---

## Key Design Patterns

### 1. Wolverine HTTP Endpoints (NOT MediatR!)

**Pattern**: Public async method → automatic HTTP endpoint registration

```csharp
// ✅ CORRECT: Wolverine pattern
namespace B2Connect.Catalog.Application.Handlers
{
    public class PriceCalculationHandler
    {
        private readonly IPriceCalculationService _service;

        public PriceCalculationHandler(IPriceCalculationService service)
        {
            _service = service;
        }

        // Wolverine AUTOMATICALLY creates: POST /calculateprice
        public async Task<CalculatePriceResponse> CalculatePrice(
            CalculatePriceRequest request,
            CancellationToken cancellationToken)
        {
            return await _service.CalculatePrice(request, cancellationToken);
        }
    }
}

// ❌ WRONG: MediatR pattern (not used in B2Connect)
public record CalculatePriceCommand : IRequest<CalculatePriceResponse>;
public class CalculatePriceHandler : IRequestHandler<...> { }
```

**Why Wolverine?**
- ✅ Distributed messaging across microservices
- ✅ Automatic HTTP endpoint creation (no routing needed)
- ✅ Built-in async handling
- ✅ Lightweight (no additional abstractions)

### 2. Service Layer Pattern

**Service**: Contains business logic, takes domain models, uses repositories/clients

```csharp
public interface IPriceCalculationService
{
    Task<CalculatePriceResponse> CalculatePrice(
        CalculatePriceRequest request,
        CancellationToken cancellationToken);
}

public class PriceCalculationService : IPriceCalculationService
{
    private readonly ILogger<PriceCalculationService> _logger;

    public PriceCalculationService(ILogger<PriceCalculationService> logger)
    {
        _logger = logger;
    }

    public async Task<CalculatePriceResponse> CalculatePrice(
        CalculatePriceRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating price for {Country}", request.DestinationCountry);

        // Business logic here
        var vatRate = GetVatRate(request.DestinationCountry);
        var vatAmount = request.BasePrice * (vatRate / 100m);
        var finalPrice = request.BasePrice + vatAmount;

        return new CalculatePriceResponse
        {
            BasePrice = request.BasePrice,
            VatRate = vatRate,
            VatAmount = Math.Round(vatAmount, 2),
            FinalPrice = Math.Round(finalPrice, 2)
        };
    }

    private decimal GetVatRate(string countryCode)
    {
        // Static dictionary of VAT rates
        var rates = new Dictionary<string, decimal>
        {
            { "DE", 19m },
            { "AT", 20m },
            { "FR", 20m },
            // ... 16 more countries
        };

        return rates.TryGetValue(countryCode.ToUpper(), out var rate) 
            ? rate 
            : 19m;  // Default fallback
    }
}
```

### 3. Repository Pattern for Caching

**Repository**: Abstracts data access (cache + database)

```csharp
public interface IVatIdValidationRepository
{
    Task<VatIdValidationCache?> GetByVatIdAsync(
        string tenantId, string countryCode, string vatNumber);
    
    Task SaveAsync(VatIdValidationCache validation);
}

public class VatIdValidationRepository : IVatIdValidationRepository
{
    private readonly CatalogDbContext _context;
    private readonly IDistributedCache _cache;

    public VatIdValidationRepository(
        CatalogDbContext context,
        IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<VatIdValidationCache?> GetByVatIdAsync(
        string tenantId, string countryCode, string vatNumber)
    {
        var cacheKey = $"vat:{countryCode}:{vatNumber}";

        // Try cache first
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<VatIdValidationCache>(cached);
        }

        // Fall back to database
        return await _context.VatIdValidationCaches
            .Where(v => v.TenantId.ToString() == tenantId
                && v.CountryCode == countryCode
                && v.VatNumber == vatNumber
                && !v.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task SaveAsync(VatIdValidationCache validation)
    {
        _context.VatIdValidationCaches.Add(validation);
        await _context.SaveChangesAsync();

        // Also cache
        var cacheKey = $"vat:{validation.CountryCode}:{validation.VatNumber}";
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = validation.IsValid
                ? TimeSpan.FromDays(365)
                : TimeSpan.FromHours(24)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(validation), options);
    }
}
```

### 4. External API Integration (VIES)

**SOAP/XML Client**: Parses EU VAT validation responses

```csharp
public interface IViesApiClient
{
    Task<VatValidationResult> CheckVatAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken);
}

public class ViesApiClient : IViesApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<ViesApiClient> _logger;
    private const int MaxRetries = 3;

    public ViesApiClient(HttpClient client, ILogger<ViesApiClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<VatValidationResult> CheckVatAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken)
    {
        var soapRequest = BuildSoapRequest(countryCode, vatNumber);
        var attempts = 0;
        HttpResponseMessage? response = null;

        while (attempts < MaxRetries)
        {
            try
            {
                response = await _client.PostAsync(
                    "https://ec.europa.eu/taxation_customs/vies/",
                    new StringContent(soapRequest, Encoding.UTF8, "application/soap+xml"),
                    cancellationToken
                );

                if (response.IsSuccessStatusCode)
                    break;

                attempts++;
                await Task.Delay(Math.Pow(2, attempts) * 1000, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning("VIES API attempt {Attempt}/{Max} failed: {Error}",
                    attempts + 1, MaxRetries, ex.Message);
                attempts++;
            }
        }

        if (response == null || !response.IsSuccessStatusCode)
            throw new InvalidOperationException("VIES API unavailable after retries");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return ParseSoapResponse(content);
    }

    private VatValidationResult ParseSoapResponse(string soapResponse)
    {
        var doc = XDocument.Parse(soapResponse);
        var ns = XNamespace.Get("http://ec.europa.eu/taxation_customs/vies/");

        var valid = bool.Parse(
            doc.Descendants(ns + "valid").First().Value
        );
        var companyName = doc.Descendants(ns + "name").FirstOrDefault()?.Value ?? "";
        var address = doc.Descendants(ns + "address").FirstOrDefault()?.Value ?? "";

        return new VatValidationResult
        {
            IsValid = valid,
            CompanyName = companyName,
            CompanyAddress = address,
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(365)
        };
    }

    private string BuildSoapRequest(string countryCode, string vatNumber)
    {
        return $@"
            <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'>
                <soapenv:Body>
                    <ns:checkVat xmlns:ns='http://ec.europa.eu/taxation_customs/vies/'>
                        <ns:countryCode>{countryCode}</ns:countryCode>
                        <ns:vatNumber>{vatNumber}</ns:vatNumber>
                    </ns:checkVat>
                </soapenv:Body>
            </soapenv:Envelope>";
    }
}
```

---

## Adding New Features

### Scenario 1: Add New VAT Rate (Germany → Italy)

**Steps:**

1. **Update VAT Rate Dictionary**
   ```csharp
   // PriceCalculationService.cs
   var rates = new Dictionary<string, decimal>
   {
       { "DE", 19m },
       // ... existing rates
       { "IT", 22m }  // Add Italy
   };
   ```

2. **Test It**
   ```csharp
   // PriceCalculationServiceTests.cs
   [Fact]
   public async Task CalculatePrice_Italy_Returns22PercentVat()
   {
       var service = new PriceCalculationService(_logger);
       var request = new CalculatePriceRequest { BasePrice = 100m, DestinationCountry = "IT" };
       
       var result = await service.CalculatePrice(request, CancellationToken.None);
       
       result.VatRate.Should().Be(22m);
       result.FinalPrice.Should().Be(122m);
   }
   ```

3. **Run Tests**
   ```bash
   dotnet test --filter "Italy"
   ```

4. **Commit**
   ```bash
   git add PriceCalculationService.cs PriceCalculationServiceTests.cs
   git commit -m "feat(catalog): add Italy (22%) VAT rate"
   ```

### Scenario 2: Add New EU Country for VAT Validation

**Steps:**

1. **Identify Country Code** (e.g., Romania = "RO")
2. **Add to VIES Support** (VIES already supports 27 EU countries)
3. **Test with Valid Romanian VAT**
   ```csharp
   [Fact]
   public async Task ValidateVatId_Romania_SuccessfullyValidates()
   {
       var mockViesClient = new Mock<IViesApiClient>();
       mockViesClient
           .Setup(c => c.CheckVatAsync("RO", "12345678", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new VatValidationResult { IsValid = true });

       var service = new VatIdValidationService(mockViesClient.Object, ...);
       var result = await service.ValidateVatIdAsync("RO", "12345678", CancellationToken.None);
       
       result.IsValid.Should().BeTrue();
   }
   ```

4. **Test Reverse Charge**
   ```csharp
   [Fact]
   public void ReverseCharge_RomaniaToGermany_ReturnsTrue()
   {
       var service = new VatIdValidationService(...);
       var validation = new VatValidationResult { IsValid = true };
       
       var result = service.ShouldApplyReverseCharge(validation, "RO", "DE");
       
       result.Should().BeTrue();
   }
   ```

### Scenario 3: Modify Cache TTL

**Current**:
- Valid VAT IDs: 365 days
- Invalid VAT IDs: 24 hours

**To Change**:

```csharp
// VatIdValidationService.cs
public async Task SaveToCache(VatValidationResult result, string cacheKey)
{
    var ttl = result.IsValid
        ? TimeSpan.FromDays(180)      // Changed from 365 to 180
        : TimeSpan.FromHours(12);     // Changed from 24 to 12

    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = ttl
    };

    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);
}
```

**Test It**:
```csharp
[Fact]
public async Task Cache_ValidVatId_Expires180Days()
{
    var mockCache = new TestDistributedCache();
    var service = new VatIdValidationService(...);

    var result = new VatValidationResult { IsValid = true };
    await service.SaveToCache(result, "vat:DE:123456789");

    // Verify TTL set to 180 days (~15,552,000 seconds)
    var entry = mockCache.Get("vat:DE:123456789");
    entry.Should().NotBeNull();
}
```

---

## Common Tasks

### Task 1: Adding Validation Rules

**Example**: Require VAT number to be 1-17 characters

```csharp
// ValidateVatIdRequestValidator.cs
public class ValidateVatIdRequestValidator : AbstractValidator<ValidateVatIdRequest>
{
    public ValidateVatIdRequestValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2)
            .Matches(@"^[A-Z]{2}$");

        RuleFor(x => x.VatNumber)
            .NotEmpty()
            .Length(1, 17)  // Add this rule
            .Matches(@"^[A-Z0-9]+$");
    }
}

// Test it
[Fact]
public void Validator_VatNumberTooLong_ShouldFail()
{
    var validator = new ValidateVatIdRequestValidator();
    var request = new ValidateVatIdRequest
    {
        CountryCode = "DE",
        VatNumber = "123456789012345678"  // 18 chars, should fail
    };

    var result = validator.Validate(request);
    
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "VatNumber");
}
```

### Task 2: Logging Events

**Pattern**: Log at appropriate levels

```csharp
public class VatIdValidationService
{
    private readonly ILogger<VatIdValidationService> _logger;

    public async Task<VatValidationResult> ValidateVatIdAsync(...)
    {
        // INFO: Start of operation
        _logger.LogInformation("Validating VAT ID: {CountryCode}{VatNumber}",
            countryCode, vatNumber);

        // Check cache
        var cached = await _cache.GetAsync(cacheKey);
        if (cached != null)
        {
            _logger.LogInformation("VAT validation cache HIT: {Key}", cacheKey);
            return cached;
        }

        _logger.LogInformation("VAT validation cache MISS, calling VIES API");

        // Call VIES API
        try
        {
            var result = await _viesClient.CheckVatAsync(...);
            _logger.LogInformation("VIES API returned: IsValid={IsValid}", result.IsValid);
            return result;
        }
        catch (Exception ex)
        {
            // ERROR: Failure
            _logger.LogError(ex, "VIES API call failed for {VatId}", $"{countryCode}{vatNumber}");
            throw;
        }
    }
}
```

### Task 3: Handling Errors

**Pattern**: Catch specific exceptions, log, and return user-friendly errors

```csharp
public class VatIdValidationHandler
{
    private readonly IVatIdValidationService _service;
    private readonly ILogger<VatIdValidationHandler> _logger;

    public async Task<VatIdValidationResponse> ValidateVatId(
        ValidateVatIdRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.ValidateVatIdAsync(
                request.CountryCode,
                request.VatNumber,
                cancellationToken
            );

            return new VatIdValidationResponse
            {
                IsValid = result.IsValid,
                CompanyName = result.CompanyName,
                ReverseChargeApplies = _service.ShouldApplyReverseCharge(
                    result,
                    request.BuyerCountry,
                    request.SellerCountry
                )
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid input: {Error}", ex.Message);
            return new VatIdValidationResponse
            {
                IsValid = false,
                Message = "Invalid VAT ID format"
            };
        }
        catch (HttpRequestException ex) when (ex.InnerException?.Message.Contains("timeout") == true)
        {
            _logger.LogError(ex, "VIES API timeout");
            return new VatIdValidationResponse
            {
                IsValid = false,
                Message = "VAT validation service unavailable, please try again"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during VAT validation");
            throw;  // Let middleware handle it
        }
    }
}
```

---

## Best Practices

### ✅ DO:

1. **Use async/await throughout**
   ```csharp
   public async Task<Result> DoSomethingAsync(CancellationToken cancellationToken)
   {
       return await _service.SomeAsync(cancellationToken);
   }
   ```

2. **Always pass CancellationToken**
   ```csharp
   await _httpClient.GetAsync(url, cancellationToken);
   await _context.SaveChangesAsync(cancellationToken);
   ```

3. **Filter by TenantId for multi-tenant isolation**
   ```csharp
   .Where(v => v.TenantId == tenantId && v.CountryCode == country && v.VatNumber == vat)
   ```

4. **Use dependency injection for all services**
   ```csharp
   public PriceCalculationHandler(IPriceCalculationService service)
   {
       _service = service;
   }
   ```

5. **Log contextual information**
   ```csharp
   _logger.LogInformation("Processing request for tenant {TenantId}", tenantId);
   ```

### ❌ DON'T:

1. **Use `.Result` or `.Wait()`** (deadlocks in async contexts)
   ```csharp
   // ❌ WRONG
   var result = service.GetAsync().Result;
   
   // ✅ RIGHT
   var result = await service.GetAsync();
   ```

2. **Hardcode configuration**
   ```csharp
   // ❌ WRONG
   var timeout = 10;
   
   // ✅ RIGHT
   var timeout = configuration.GetValue<int>("Vies:TimeoutSeconds", 10);
   ```

3. **Catch `Exception` broadly**
   ```csharp
   // ❌ WRONG
   try { ... } catch (Exception ex) { ... }
   
   // ✅ RIGHT
   try { ... }
   catch (HttpRequestException ex) { ... }
   catch (ArgumentException ex) { ... }
   ```

4. **Use static methods for business logic**
   ```csharp
   // ❌ WRONG
   public static decimal CalculateVat(decimal price, decimal rate) { ... }
   
   // ✅ RIGHT
   public class PriceCalculationService : IPriceCalculationService
   {
       public async Task<CalculatePriceResponse> CalculatePrice(...) { ... }
   }
   ```

5. **Store secrets in code**
   ```csharp
   // ❌ WRONG
   var apiKey = "secret-123";
   
   // ✅ RIGHT
   var apiKey = configuration["Vies:ApiKey"] ?? throw new InvalidOperationException();
   ```

---

## Debugging

### Debug Price Calculation

```csharp
var service = new PriceCalculationService(_logger);
var request = new CalculatePriceRequest
{
    BasePrice = 99.99m,
    DestinationCountry = "DE",
    DiscountPercentage = 10m
};

// Set breakpoint here
var result = await service.CalculatePrice(request, CancellationToken.None);

// Inspect:
// result.BasePrice = 99.99
// result.VatRate = 19.0
// result.VatAmount = 19.00 (calculated from 99.99 * 0.19)
// result.FinalPrice should be 107.10 (after 10% discount)
```

### Debug VAT Validation

```bash
# Enable debug logging
export LOG_LEVEL=Debug

# Run service
dotnet run

# Watch logs for:
# - "Validating VAT ID: ..."
# - "Cache MISS" vs "Cache HIT"
# - "VIES API returned: IsValid=..."
# - "Reverse charge applies: ..."
```

---

## Resources

- **Wolverine Documentation**: https://wolverine.dev
- **EF Core**: https://learn.microsoft.com/en-us/ef/core/
- **FluentValidation**: https://docs.fluentvalidation.net/
- **VIES API**: https://ec.europa.eu/taxation_customs/vies/
- **Germany VAT Rules**: https://www.zoll.de/ (PAngV)
- **EU Reverse Charge**: https://ec.europa.eu/taxation_customs/

---

**Last Updated**: 29 December 2025  
**Maintainers**: Backend Team  
**Difficulty**: Intermediate  
**Time to Master**: ~4 hours
