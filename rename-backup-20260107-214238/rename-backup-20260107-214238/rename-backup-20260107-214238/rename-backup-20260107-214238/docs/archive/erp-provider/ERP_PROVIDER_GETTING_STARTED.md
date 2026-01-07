# ERP Provider Integration - Getting Started

## 🚀 Immediate Integration (No Changes Required!)

The `CheckRegistrationTypeService` already works with the new provider pattern. No code changes needed!

### Current Flow (Still Works)

```csharp
// Program.cs - Register the provider
services.AddFakeErpProvider();  // Or your configuration-based setup

// Existing code - no changes
public class CheckRegistrationTypeService
{
    private readonly IErpCustomerService _erpService;
    
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        // This now uses FakeErpProvider with realistic mock data!
        var erpCustomer = await _erpService.GetCustomerByEmailAsync(
            request.Email,
            cancellationToken
        );
        
        if (erpCustomer != null)
        {
            // Customer found in ERP (now: in Faker database)
            return new CheckRegistrationTypeResponse
            {
                Success = true,
                RegistrationType = RegistrationType.ExistingCustomer,
                Data = new RegistrationTypeResponseDto
                {
                    RegistrationType = RegistrationType.ExistingCustomer,
                    IsExistingCustomer = true,
                    ErpCustomerId = erpCustomer.CustomerNumber,
                    ErpCustomerName = erpCustomer.CustomerName,
                    // ... rest of mapping
                }
            };
        }
        
        // New customer
        return new CheckRegistrationTypeResponse
        {
            Success = true,
            RegistrationType = RegistrationType.NewCustomer,
            Message = "Sie werden als Neukunde registriert."
        };
    }
}
```

## 📋 Setup in Program.cs

### Option 1: Development (Recommended)

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add ERP Provider - uses Faker for development
services.AddFakeErpProvider();

// ... rest of configuration
```

### Option 2: Configuration-Driven

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add ERP Provider based on environment configuration
services.AddErpProvider(builder.Configuration);

// ... rest of configuration
```

**appsettings.Development.json:**
```json
{
  "Erp": {
    "Provider": "Fake",
    "UseResilience": false
  }
}
```

### Option 3: With Fallback Support

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// For future SAP integration with Faker fallback
services.AddResilientErpProvider(
    primaryProviderName: "Fake",  // Change to "SAP" when implemented
    fallbackProviderName: "Fake"
);

// ... rest of configuration
```

## 🧪 Test Scenarios (Using Faker Data)

### B2C Existing Customer
```csharp
[Fact]
public async Task CheckRegistrationType_B2CExistingCustomer_ReturnsCorrectType()
{
    // Arrange
    var serviceProvider = new ServiceCollection()
        .AddFakeErpProvider()
        .AddLogging()
        .BuildServiceProvider();
    
    var erpService = serviceProvider.GetRequiredService<IErpCustomerService>();
    
    // Faker has: CUST-001 with email "max.mustermann@example.com"
    
    // Act
    var customer = await erpService.GetCustomerByEmailAsync("max.mustermann@example.com");
    
    // Assert
    Assert.NotNull(customer);
    Assert.Equal("CUST-001", customer.CustomerNumber);
    Assert.Equal("Max Mustermann", customer.CustomerName);
    Assert.Equal("PRIVATE", customer.BusinessType);
}
```

### B2B Existing Customer
```csharp
[Fact]
public async Task CheckRegistrationType_B2BExistingCustomer_ReturnsBusinessType()
{
    // Arrange
    var erpService = GetFakeErpService();
    
    // Faker has: CUST-100 = TechCorp GmbH (B2B, credit limit €50,000)
    
    // Act
    var customer = await erpService.GetCustomerByCompanyNameAsync("TechCorp GmbH");
    
    // Assert
    Assert.NotNull(customer);
    Assert.Equal("BUSINESS", customer.BusinessType);
    Assert.NotNull(customer.CreditLimit);
    Assert.True(customer.CreditLimit > 0);
}
```

### Lookup by Customer Number
```csharp
[Fact]
public async Task CheckRegistrationType_ByCustomerNumber_ReturnsFull Details()
{
    // Arrange
    var erpService = GetFakeErpService();
    
    // Faker has: CUST-101 = InnovateLabs AG
    
    // Act
    var customer = await erpService.GetCustomerByNumberAsync("CUST-101");
    
    // Assert
    Assert.NotNull(customer);
    Assert.Equal("InnovateLabs AG", customer.CustomerName);
    Assert.Equal("AT", customer.Country);  // Austria
    Assert.Equal("contact@innovatelabs.at", customer.Email);
}
```

## 🔍 Debug Logging

The provider system logs all operations. Enable debug logging to see what's happening:

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "B2X.Identity.Infrastructure": "Debug"
    }
  }
}
```

**Console output example:**
```
[FAKE ERP] GetCustomerByEmail: max.mustermann@example.com
[FAKE ERP] Found customer: Max Mustermann
[FAKE ERP] Lookup successful: CUST-001
```

## 📊 Available Test Customers

### B2C Customers (Private)

**CUST-001: Max Mustermann**
```
Email: max.mustermann@example.com
Phone: +49 30 123456
Address: Musterstraße 1, 10115 Berlin
Country: DE (Germany)
Type: PRIVATE (B2C)
Status: ACTIVE
```

**CUST-002: Erika Musterfrau**
```
Email: erika.musterfrau@example.com
Phone: +49 69 654321
Address: Frankfurter Allee 123, 60311 Frankfurt
Country: DE (Germany)
Type: PRIVATE (B2C)
Status: ACTIVE
```

### B2B Customers (Business)

**CUST-100: TechCorp GmbH**
```
Email: info@techcorp.de
Phone: +49 2xx 98765432
Address: Technologiepark 5, 80939 München
Country: DE (Germany)
Type: BUSINESS (B2B)
Status: ACTIVE
Credit Limit: €50,000
```

**CUST-101: InnovateLabs AG**
```
Email: contact@innovatelabs.at
Phone: +43 1 123456789
Address: Innovationstraße 42, 1020 Wien
Country: AT (Austria)
Type: BUSINESS (B2B)
Status: ACTIVE
Credit Limit: €75,000
```

**CUST-102: Global Solutions SA**
```
Email: sales@globalsolutions.ch
Phone: +41 44 123456
Address: Börsenstraße 7, 8001 Zürich
Country: CH (Switzerland)
Type: BUSINESS (B2B)
Status: ACTIVE
Credit Limit: €100,000
```

## ✅ No Breaking Changes

Your existing code continues to work unchanged:

**Before (with real ERP):**
```csharp
services.AddScoped<IErpCustomerService, RealErpService>();
```

**Now (with Faker):**
```csharp
services.AddFakeErpProvider();  // Maintains same interface!
```

The `IErpCustomerService` interface is still there, so all existing code works without modification.

## 🚀 Future: Switch to Real ERP

When you implement SAP provider:

```csharp
// Program.cs
if (env.IsProduction())
{
    // Use real SAP
    services.AddResilientErpProvider("SAP", "Fake");
}
else
{
    // Use Faker for development
    services.AddFakeErpProvider();
}
```

## 🎯 Benefits Now

✅ **No ERP Setup** - Develop without SAP, Oracle, etc.  
✅ **Fast Tests** - In-memory lookups < 1ms  
✅ **Realistic Data** - 5 pre-configured test customers  
✅ **Easy Integration** - Works with existing code  
✅ **Future Ready** - Switch to real ERP anytime  

## 📞 Support

For questions or to add more test customers:

1. Edit: `backend/Domain/Identity/src/Infrastructure/ExternalServices/FakeErpProvider.cs`
2. Add customers to `FakeCustomerDatabase` dictionary
3. Add lookups to `EmailToCustomerNumber` or `CompanyNameToCustomerNumber`
4. Run tests: `dotnet test`

---

**Ready to use!** Add `services.AddFakeErpProvider();` to Program.cs and start developing.
