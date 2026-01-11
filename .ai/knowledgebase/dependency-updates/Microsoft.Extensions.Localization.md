---
docid: KB-104
title: Microsoft.Extensions.Localization
owner: @TechLead
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Localization

**Version:** 10.0.1  
**Package:** [Microsoft.Extensions.Localization](https://www.nuget.org/packages/Microsoft.Extensions.Localization)  
**Documentation:** [Localization in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/localization)

## Overview

Microsoft.Extensions.Localization provides services and default implementations for localizing .NET applications. It enables strongly-typed access to localized strings through dependency injection, supporting resource files (.resx) and culture-specific fallback mechanisms.

The localization system separates localizable content from executable code, allowing applications to support multiple languages and cultures while maintaining clean architecture.

## Key Features

- **Strongly Typed Localization**: IStringLocalizer<T> for type-safe string access
- **Resource File Support**: .resx files with culture-specific fallbacks
- **Culture Fallback**: Automatic fallback to parent cultures and default resources
- **Dependency Injection**: Seamless integration with DI container
- **Parameterized Strings**: Support for formatted localized strings
- **Resource Manager Integration**: Uses ResourceManager for efficient resource loading
- **Root Namespace Support**: Handles assembly namespace mismatches

## Core Interfaces

### IStringLocalizer<T>
- Provides localized strings for a specific type T
- Uses the type name to locate corresponding resource files
- Supports parameterized string formatting
- Returns LocalizedString objects with culture information

### IStringLocalizerFactory
- Factory for creating IStringLocalizer instances
- Allows dynamic creation of localizers for any type
- Useful when the type isn't known at compile time

### IStringLocalizer (non-generic)
- Base interface for string localization
- Used when type-specific localization isn't needed
- Created via IStringLocalizerFactory

## Basic Usage

### Setting Up Localization

```csharp
// Program.cs
var builder = Host.CreateApplicationBuilder(args);

// Configure localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

// Set current culture (can be from configuration)
CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = 
    CultureInfo.GetCultureInfo("en-US");
```

### Creating Resource Files

Resource files follow the naming pattern: `<TypeName>.<Culture>.resx`

```
// Resources/MessageService.resx (default/fallback)
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="GreetingMessage" xml:space="preserve">
    <value>Hello, welcome to our application!</value>
  </data>
  <data name="ErrorMessage" xml:space="preserve">
    <value>An error occurred. Please try again.</value>
  </data>
</root>

// Resources/MessageService.de.resx (German)
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="GreetingMessage" xml:space="preserve">
    <value>Hallo, willkommen in unserer Anwendung!</value>
  </data>
  <data name="ErrorMessage" xml:space="preserve">
    <value>Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut.</value>
  </data>
</root>
```

### Using IStringLocalizer<T>

```csharp
public class MessageService
{
    private readonly IStringLocalizer<MessageService> _localizer;

    public MessageService(IStringLocalizer<MessageService> localizer)
    {
        _localizer = localizer;
    }

    public string GetGreetingMessage()
    {
        return _localizer["GreetingMessage"];
    }

    public string GetErrorMessage()
    {
        return _localizer["ErrorMessage"];
    }
}
```

### Parameterized Localization

```csharp
public class OrderService
{
    private readonly IStringLocalizer<OrderService> _localizer;

    public OrderService(IStringLocalizer<OrderService> localizer)
    {
        _localizer = localizer;
    }

    public string GetOrderConfirmationMessage(string customerName, int orderId)
    {
        return _localizer["OrderConfirmation", customerName, orderId];
    }
}

// Resources/OrderService.resx
<data name="OrderConfirmation" xml:space="preserve">
  <value>Dear {0}, your order #{1} has been confirmed.</value>
</data>

// Resources/OrderService.de.resx
<data name="OrderConfirmation" xml:space="preserve">
  <value>Lieber {0}, Ihre Bestellung #{1} wurde bestätigt.</value>
</data>
```

## B2X Integration Patterns

### Multitenant Localization

```csharp
// B2X tenant-specific localization
public class TenantLocalizationService
{
    private readonly IStringLocalizerFactory _localizerFactory;
    private readonly ITenantContext _tenantContext;

    public TenantLocalizationService(
        IStringLocalizerFactory localizerFactory,
        ITenantContext tenantContext)
    {
        _localizerFactory = localizerFactory;
        _tenantContext = tenantContext;
    }

    public string GetLocalizedString(string key, params object[] args)
    {
        // Create localizer for tenant-specific resources
        var localizer = _localizerFactory.Create(
            typeof(TenantLocalizationService), 
            _tenantContext.TenantId);

        return localizer[key, args];
    }
}

// Usage in services
public class CatalogService
{
    private readonly TenantLocalizationService _localization;

    public CatalogService(TenantLocalizationService localization)
    {
        _localization = localization;
    }

    public string GetProductNotFoundMessage(string productId)
    {
        return _localization.GetLocalizedString("ProductNotFound", productId);
    }
}
```

### Wolverine CQRS Message Localization

```csharp
// Localized message handlers
public class OrderPlacedHandler
{
    private readonly IStringLocalizer<OrderPlacedHandler> _localizer;
    private readonly ILogger<OrderPlacedHandler> _logger;

    public OrderPlacedHandler(
        IStringLocalizer<OrderPlacedHandler> localizer,
        ILogger<OrderPlacedHandler> logger)
    {
        _localizer = localizer;
        _logger = logger;
    }

    public async Task Handle(OrderPlaced message, IMessageContext context)
    {
        _logger.LogInformation(
            _localizer["ProcessingOrder", message.OrderId]);

        // Process order...

        await context.PublishAsync(new OrderProcessed 
        { 
            OrderId = message.OrderId,
            Message = _localizer["OrderProcessedSuccessfully"]
        });
    }
}
```

### API Response Localization

```csharp
// Localized API responses
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IStringLocalizer<ProductsController> _localizer;
    private readonly IProductService _productService;

    public ProductsController(
        IStringLocalizer<ProductsController> localizer,
        IProductService productService)
    {
        _localizer = localizer;
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);
        
        if (product == null)
        {
            return NotFound(new 
            {
                Error = _localizer["ProductNotFound", id]
            });
        }

        return Ok(new
        {
            product.Id,
            Name = _localizer[product.NameKey],
            Description = _localizer[product.DescriptionKey]
        });
    }
}
```

## Culture Management

### Dynamic Culture Switching

```csharp
public class CultureService
{
    public void SetCulture(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        // For ASP.NET Core requests
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    public string GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture.Name;
    }
}

// Middleware for culture detection
public class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cultureCode = context.Request.Headers["Accept-Language"].FirstOrDefault() 
                         ?? "en-US";
        
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(context);
    }
}
```

### Supported Cultures Configuration

```csharp
// Configure supported cultures
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
        new CultureInfo("es-ES"),
        new CultureInfo("it-IT"),
        new CultureInfo("pt-BR"),
        new CultureInfo("nl-NL"),
        new CultureInfo("pl-PL")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
```

## Resource File Organization

### B2X Resource Structure

```
Resources/
├── Shared/
│   ├── Shared.resx
│   ├── Shared.de.resx
│   └── Shared.fr.resx
├── Services/
│   ├── Catalog/
│   │   ├── CatalogService.resx
│   │   ├── CatalogService.de.resx
│   │   └── CatalogService.fr.resx
│   └── Order/
│       ├── OrderService.resx
│       └── OrderService.de.resx
└── UI/
    ├── ValidationMessages.resx
    ├── ValidationMessages.de.resx
    └── ValidationMessages.fr.resx
```

### Root Namespace Configuration

```csharp
// Handle namespace mismatches
[assembly: RootNamespace("B2X.Services")]

// In project file
<PropertyGroup>
    <RootNamespace>B2X.Services</RootNamespace>
</PropertyGroup>
```

## Advanced Patterns

### Custom String Localizer

```csharp
public class DatabaseStringLocalizer : IStringLocalizer
{
    private readonly IDatabaseService _database;

    public DatabaseStringLocalizer(IDatabaseService database)
    {
        _database = database;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = _database.GetLocalizedString(
                CultureInfo.CurrentCulture.Name, name);
            
            return new LocalizedString(name, value ?? name, 
                value == null, SearchedLocation: null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = this[name];
            return new LocalizedString(name, 
                string.Format(format.Value, arguments), 
                format.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _database.GetAllLocalizedStrings(
            CultureInfo.CurrentCulture.Name, includeParentCultures);
    }
}
```

### Localization with Pluralization

```csharp
public class PluralizationService
{
    private readonly IStringLocalizer<PluralizationService> _localizer;

    public PluralizationService(IStringLocalizer<PluralizationService> localizer)
    {
        _localizer = localizer;
    }

    public string GetItemCountMessage(int count)
    {
        var key = count == 1 ? "ItemCountSingular" : "ItemCountPlural";
        return _localizer[key, count];
    }
}

// Resources/PluralizationService.resx
<data name="ItemCountSingular" xml:space="preserve">
  <value>You have {0} item.</value>
</data>
<data name="ItemCountPlural" xml:space="preserve">
  <value>You have {0} items.</value>
</data>
```

## Testing Localized Applications

### Unit Testing with Localization

```csharp
public class MessageServiceTests
{
    [Fact]
    public void GetGreetingMessage_ReturnsLocalizedString()
    {
        // Arrange
        var localizerMock = new Mock<IStringLocalizer<MessageService>>();
        localizerMock.Setup(l => l["GreetingMessage"])
            .Returns(new LocalizedString("GreetingMessage", "Hello!", false));
        
        var service = new MessageService(localizerMock.Object);

        // Act
        var result = service.GetGreetingMessage();

        // Assert
        Assert.Equal("Hello!", result);
    }
}

public class LocalizationTestFixture
{
    public IStringLocalizer<T> CreateLocalizer<T>(Dictionary<string, string> resources)
    {
        var localizerMock = new Mock<IStringLocalizer<T>>();
        
        foreach (var resource in resources)
        {
            localizerMock.Setup(l => l[resource.Key])
                .Returns(new LocalizedString(resource.Key, resource.Value, false));
        }

        return localizerMock.Object;
    }
}
```

### Integration Testing with Cultures

```csharp
public class LocalizationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LocalizationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("en-US", "Hello, welcome!")]
    [InlineData("de-DE", "Hallo, willkommen!")]
    public async Task GetGreeting_ReturnsLocalizedMessage(string culture, string expected)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));

        var response = await client.GetAsync("/api/greeting");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(expected, content);
    }
}
```

## Best Practices

1. **Use IStringLocalizer<T>** for type-specific localization
2. **Organize resources** by feature/component
3. **Follow naming conventions** for resource files
4. **Handle missing resources** gracefully
5. **Test localization** in multiple cultures
6. **Use parameterized strings** for dynamic content
7. **Configure culture** early in the request pipeline

## Migration from .NET Core to .NET 10

- **Enhanced Resource Loading**: Improved performance for resource file access
- **Better Culture Support**: Enhanced culture fallback mechanisms
- **Source Generation**: Potential for compile-time resource validation
- **Improved DI Integration**: Better integration with dependency injection patterns

## Troubleshooting

### Common Issues

1. **Resources not found**: Check file naming and location
2. **Culture not applied**: Ensure culture is set before localization
3. **Namespace mismatch**: Use RootNamespace attribute
4. **Missing translations**: Implement fallback to default culture

### Debug Localization

```csharp
// Log localization details
public class DebugLocalizationService
{
    private readonly IStringLocalizer _localizer;

    public void LogLocalizationInfo(string key)
    {
        var localized = _localizer[key];
        
        Console.WriteLine($"Key: {key}");
        Console.WriteLine($"Value: {localized.Value}");
        Console.WriteLine($"Found: {!localized.ResourceNotFound}");
        Console.WriteLine($"Culture: {CultureInfo.CurrentCulture.Name}");
    }
}
```

## Related Libraries

- **Microsoft.Extensions.Configuration**: For culture configuration
- **Microsoft.Extensions.Logging**: For logging localization issues
- **Microsoft.Extensions.Options**: Often used together for localized options

This library provides essential internationalization support for B2X, enabling proper localization across all supported languages and cultures while maintaining clean, testable code architecture.</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\knowledgebase\dependency-updates\Microsoft.Extensions.Localization.md