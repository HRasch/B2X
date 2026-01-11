---
docid: KB-108
title: Microsoft.Extensions.Compliance
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Compliance

**Version:** 10.1.0  
**Packages:** 
- [Microsoft.Extensions.Compliance.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Compliance.Abstractions)
- [Microsoft.Extensions.Compliance.Redaction](https://www.nuget.org/packages/Microsoft.Extensions.Compliance.Redaction)  
**Documentation:** [Compliance libraries in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/compliance)

## Overview

Microsoft.Extensions.Compliance provides foundational components for implementing compliance features in .NET applications, focusing on data classification and redaction. These libraries help developers create and manage data in a standardized way, ensuring sensitive information is properly categorized and protected.

The compliance libraries are essential for applications handling personal data, financial information, or other confidential data points, providing systematic approaches to data protection and privacy compliance.

## Key Features

- **Data Classification**: Structured categorization of data based on sensitivity and protection levels
- **Data Redaction**: Automated sanitization and masking of sensitive information in logs and outputs
- **Custom Taxonomies**: Flexible classification systems tailored to specific business needs
- **Configuration Binding**: Integration with .NET configuration system for dynamic classification rules
- **Multiple Redactors**: Built-in redactors (Erasing, HMAC) with support for custom implementations
- **Logging Integration**: Seamless integration with Microsoft.Extensions.Logging for automatic redaction

## Core Components

### Data Classification

#### DataClassification Structure
- **TaxonomyName**: Identifies the classification system (e.g., "MyTaxonomy")
- **Value**: Represents the specific label within the taxonomy (e.g., "PrivateInformation")
- **DataClassificationSet**: Composes multiple classifications (logical AND operation)

#### Built-in Classifications
- **DataClassification.None**: Explicitly marks data as having no classification
- **DataClassification.Unknown**: Marks data with unknown classification status

### Data Redaction

#### Built-in Redactors
- **ErasingRedactor**: Replaces any input with an empty string
- **HmacRedactor**: Uses HMACSHA256 to encode data (experimental, requires warning suppression)

#### Redactor Architecture
- **IRedactorProvider**: Interface for providing redactor instances based on classifications
- **IRedactionBuilder**: Configuration interface for setting up redactors
- **Redactor**: Base class for implementing custom redaction logic

## Basic Usage

### Data Classification

```csharp
using Microsoft.Extensions.Compliance.Classification;

// Define custom classification taxonomy
public static class B2XTaxonomyClassifications
{
    public static string Name => "B2XTaxonomy";

    public static DataClassification PersonalData => new(Name, nameof(PersonalData));
    public static DataClassification PaymentInfo => new(Name, nameof(PaymentInfo));
    public static DataClassification HealthData => new(Name, nameof(HealthData));
    public static DataClassification BusinessConfidential => new(Name, nameof(BusinessConfidential));

    // Composite classifications
    public static DataClassificationSet SensitivePersonalData => new(PersonalData, PaymentInfo);
}

// Create custom classification attributes
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class PersonalDataAttribute : DataClassificationAttribute
{
    public PersonalDataAttribute() : base(B2XTaxonomyClassifications.PersonalData) { }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class PaymentInfoAttribute : DataClassificationAttribute
{
    public PaymentInfoAttribute() : base(B2XTaxonomyClassifications.PaymentInfo) { }
}
```

### Data Redaction

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Compliance.Redaction;

var builder = WebApplication.CreateBuilder(args);

// Register redaction services
builder.Services.AddRedaction(redactionBuilder =>
{
    // Configure redactors for classifications
    redactionBuilder.SetRedactor<StarRedactor>(
        B2XTaxonomyClassifications.PersonalData,
        B2XTaxonomyClassifications.PaymentInfo);

    // Set fallback redactor
    redactionBuilder.SetFallbackRedactor<ErasingRedactor>();

    // Configure HMAC redactor for cryptographic redaction (experimental)
    redactionBuilder.SetHmacRedactor(options =>
    {
        options.KeyId = 1234567890;
        options.Key = Convert.ToBase64String("your-secret-key-here-min-44-chars"u8);
    }, B2XTaxonomyClassifications.HealthData);
});

var app = builder.Build();
```

### Custom Redactors

```csharp
using Microsoft.Extensions.Compliance.Redaction;

// Star redactor - replaces with asterisks
public sealed class StarRedactor : Redactor
{
    private const string Stars = "****";

    public override int GetRedactedLength(ReadOnlySpan<char> input) => Stars.Length;

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        Stars.CopyTo(destination);
        return Stars.Length;
    }
}

// Hash redactor - replaces with hash
public sealed class HashRedactor : Redactor
{
    public override int GetRedactedLength(ReadOnlySpan<char> input) => 64; // SHA256 hex length

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(source));
        var hex = Convert.ToHexString(hash);
        hex.CopyTo(destination);
        return hex.Length;
    }
}

// Custom redactor provider
public sealed class B2XRedactorProvider : IRedactorProvider
{
    private static readonly StarRedactor _starRedactor = new();
    private static readonly HashRedactor _hashRedactor = new();

    public Redactor GetRedactor(DataClassificationSet classifications)
    {
        // Return different redactors based on classification
        if (classifications.Contains(B2XTaxonomyClassifications.PaymentInfo))
        {
            return _hashRedactor; // Hash payment info
        }

        return _starRedactor; // Star other sensitive data
    }
}
```

## B2X Integration

The B2X platform leverages Microsoft.Extensions.Compliance extensively for handling sensitive customer data across its multi-tenant e-commerce architecture.

### Store API Compliance

```csharp
// src/backend/Store/API/Program.cs
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.Telemetry;

var builder = WebApplication.CreateBuilder(args);

// Configure data classifications for e-commerce
builder.Services.AddRedaction(redactionBuilder =>
{
    redactionBuilder.SetRedactor<StarRedactor>(
        B2XTaxonomyClassifications.PersonalData,
        B2XTaxonomyClassifications.PaymentInfo);

    // Use HMAC for health data (GDPR compliance)
    redactionBuilder.SetHmacRedactor(options =>
    {
        options.KeyId = builder.Configuration.GetValue<long>("Compliance:HmacKeyId");
        options.Key = builder.Configuration.GetValue<string>("Compliance:HmacKey");
    }, B2XTaxonomyClassifications.HealthData);
});

// Enable redaction in logging
builder.Logging.EnableRedaction();

var app = builder.Build();

// Middleware for automatic data classification
app.Use(async (context, next) =>
{
    // Classify request data based on tenant and endpoint
    var tenantId = context.GetTenantId();
    var endpoint = context.Request.Path;

    if (endpoint.StartsWithSegments("/api/orders"))
    {
        // Orders contain payment and personal data
        context.Items["DataClassification"] = B2XTaxonomyClassifications.SensitivePersonalData;
    }

    await next();
});

app.UseRequestLatencyTelemetry();
```

### Customer Data Models

```csharp
// src/backend/Shared/Domain/Models/Customer.cs
using Microsoft.Extensions.Compliance.Classification;

public class Customer
{
    public Guid Id { get; set; }

    [PersonalData]
    public string FirstName { get; set; }

    [PersonalData]
    public string LastName { get; set; }

    [PersonalData]
    public string Email { get; set; }

    [PersonalData]
    public string PhoneNumber { get; set; }

    [PaymentInfo]
    public string PaymentMethodToken { get; set; } // Tokenized payment data

    [HealthData]
    public string MedicalConditions { get; set; } // If applicable for specialized products

    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
}

public class Address
{
    [PersonalData]
    public string Street { get; set; }

    [PersonalData]
    public string City { get; set; }

    [PersonalData]
    public string PostalCode { get; set; }

    [PersonalData]
    public string Country { get; set; }
}
```

### Order Processing with Compliance

```csharp
// src/backend/Store/Services/OrderProcessingService.cs
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.Logging;

public class OrderProcessingService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly IRedactorProvider _redactorProvider;
    private readonly IOrderRepository _orderRepository;

    public OrderProcessingService(
        ILogger<OrderProcessingService> logger,
        IRedactorProvider redactorProvider,
        IOrderRepository orderRepository)
    {
        _logger = logger;
        _redactorProvider = redactorProvider;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResult> ProcessOrderAsync(CreateOrderRequest request)
    {
        var customerId = request.CustomerId;
        var redactor = _redactorProvider.GetRedactor(B2XTaxonomyClassifications.PersonalData);

        // Log customer interaction with redaction
        using var customerScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CustomerId"] = redactor.Redact(customerId.ToString()),
            ["TenantId"] = request.TenantId
        });

        _logger.LogInformation("Processing order for customer {CustomerId}", customerId);

        try
        {
            var order = await CreateOrderAsync(request);
            _logger.LogInformation("Order {OrderId} created successfully", order.Id);

            await ProcessPaymentAsync(order);
            _logger.LogInformation("Payment processed for order {OrderId}", order.Id);

            return OrderResult.Success(order);
        }
        catch (PaymentException ex)
        {
            // Redact payment details in error logs
            var redactedMessage = RedactPaymentError(ex.Message);
            _logger.LogError(ex, "Payment failed: {Error}", redactedMessage);
            return OrderResult.PaymentFailed();
        }
    }

    private string RedactPaymentError(string errorMessage)
    {
        var redactor = _redactorProvider.GetRedactor(B2XTaxonomyClassifications.PaymentInfo);
        // Redact card numbers, tokens, etc. from error messages
        return redactor.Redact(errorMessage);
    }
}
```

### Multitenant Compliance Configuration

```csharp
// src/backend/Shared/Infrastructure/Compliance/TenantComplianceService.cs
using Microsoft.Extensions.Options;

public class TenantComplianceService
{
    private readonly ITenantContext _tenantContext;
    private readonly IOptionsMonitor<TenantComplianceOptions> _options;

    public TenantComplianceService(
        ITenantContext tenantContext,
        IOptionsMonitor<TenantComplianceOptions> options)
    {
        _tenantContext = tenantContext;
        _options = options;
    }

    public DataClassificationSet GetTenantDataClassifications()
    {
        var tenantOptions = _options.Get(_tenantContext.Tenant.Id);

        var classifications = new List<DataClassification>();

        if (tenantOptions.EnablePersonalDataClassification)
        {
            classifications.Add(B2XTaxonomyClassifications.PersonalData);
        }

        if (tenantOptions.EnablePaymentDataClassification)
        {
            classifications.Add(B2XTaxonomyClassifications.PaymentInfo);
        }

        if (tenantOptions.EnableHealthDataClassification)
        {
            classifications.Add(B2XTaxonomyClassifications.HealthData);
        }

        return new DataClassificationSet(classifications);
    }

    public Redactor GetTenantRedactor()
    {
        var tenantOptions = _options.Get(_tenantContext.Tenant.Id);

        return tenantOptions.UseHmacRedaction
            ? new HmacRedactor(new HmacRedactorOptions
              {
                  KeyId = tenantOptions.HmacKeyId,
                  Key = tenantOptions.HmacKey
              })
            : new StarRedactor();
    }
}

public class TenantComplianceOptions
{
    public bool EnablePersonalDataClassification { get; set; } = true;
    public bool EnablePaymentDataClassification { get; set; } = true;
    public bool EnableHealthDataClassification { get; set; } = false;
    public bool UseHmacRedaction { get; set; } = false;
    public long HmacKeyId { get; set; }
    public string HmacKey { get; set; }
}
```

### ERP Integration Compliance

```csharp
// src/backend/ERP/Connectors/BaseErpConnector.cs
using Microsoft.Extensions.Compliance.Redaction;

public abstract class BaseErpConnector
{
    private readonly ILogger _logger;
    private readonly IRedactorProvider _redactorProvider;

    protected BaseErpConnector(
        ILogger logger,
        IRedactorProvider redactorProvider)
    {
        _logger = logger;
        _redactorProvider = redactorProvider;
    }

    protected async Task<T> ExecuteWithComplianceAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        DataClassificationSet classifications)
    {
        var redactor = _redactorProvider.GetRedactor(classifications);

        try
        {
            _logger.LogInformation("Starting ERP operation: {Operation}", operationName);
            var result = await operation();
            _logger.LogInformation("ERP operation completed: {Operation}", operationName);
            return result;
        }
        catch (ErpException ex)
        {
            // Redact sensitive ERP data from error messages
            var redactedError = redactor.Redact(ex.Message);
            _logger.LogError(ex, "ERP operation failed: {Operation}, Error: {Error}",
                operationName, redactedError);
            throw new ErpException($"Operation failed: {redactedError}");
        }
    }
}

// Usage in specific ERP connector
public class EnventaErpConnector : BaseErpConnector
{
    public async Task<OrderData> GetOrderAsync(string orderNumber)
    {
        return await ExecuteWithComplianceAsync(
            () => _enventaApi.GetOrderAsync(orderNumber),
            $"GetOrder-{orderNumber}",
            B2XTaxonomyClassifications.SensitivePersonalData);
    }
}
```

### Logging Integration with Redaction

```csharp
// src/backend/Shared/Infrastructure/Logging/ComplianceLoggerExtensions.cs
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Compliance.Redaction;

public static class ComplianceLoggerExtensions
{
    public static void LogCustomerAction(
        this ILogger logger,
        Guid customerId,
        string action,
        [B2XTaxonomyClassifications.PersonalData] string customerEmail = null)
    {
        // The [PersonalData] attribute ensures customerEmail gets redacted
        logger.LogInformation(
            "Customer {CustomerId} performed action {Action} with email {CustomerEmail}",
            customerId, action, customerEmail);
    }

    public static void LogPaymentProcessing(
        this ILogger logger,
        Guid orderId,
        [B2XTaxonomyClassifications.PaymentInfo] string paymentToken,
        decimal amount)
    {
        logger.LogInformation(
            "Processing payment for order {OrderId}, token: {PaymentToken}, amount: {Amount}",
            orderId, paymentToken, amount);
    }

    public static void LogAuditEvent(
        this ILogger logger,
        string eventType,
        Guid tenantId,
        Guid userId,
        object data)
    {
        // Create redacted copy of audit data
        var redactedData = RedactAuditData(data);
        logger.LogInformation(
            "Audit event {EventType} for tenant {TenantId}, user {UserId}: {Data}",
            eventType, tenantId, userId, redactedData);
    }

    private static object RedactAuditData(object data)
    {
        // Implement logic to redact sensitive fields in audit data
        // This could use reflection or serialization to identify and redact fields
        return data; // Placeholder
    }
}
```

### Configuration Binding

```csharp
// src/backend/Shared/Infrastructure/Configuration/ComplianceOptions.cs
using Microsoft.Extensions.Compliance.Classification;

public class ComplianceOptions
{
    public DataClassification? DefaultPersonalDataClassification { get; set; }
    public DataClassification? DefaultPaymentDataClassification { get; set; }
    public Dictionary<string, DataClassification> FieldClassifications { get; set; } = new();
    public HmacRedactorOptions HmacOptions { get; set; } = new();
}

public class HmacRedactorOptions
{
    public long KeyId { get; set; }
    public string Key { get; set; }
}
```

```json
// appsettings.json
{
  "Compliance": {
    "DefaultPersonalDataClassification": "B2XTaxonomy:PersonalData",
    "DefaultPaymentDataClassification": "B2XTaxonomy:PaymentInfo",
    "FieldClassifications": {
      "Email": "B2XTaxonomy:PersonalData",
      "CreditCard": "B2XTaxonomy:PaymentInfo",
      "SSN": "B2XTaxonomy:HealthData"
    },
    "HmacOptions": {
      "KeyId": 1234567890,
      "Key": "your-base64-encoded-key-here"
    }
  }
}
```

### Middleware for Request Classification

```csharp
// src/backend/Shared/Infrastructure/Middleware/ComplianceMiddleware.cs
using Microsoft.Extensions.Compliance.Classification;

public class ComplianceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ComplianceMiddleware> _logger;
    private readonly ITenantContext _tenantContext;

    public ComplianceMiddleware(
        RequestDelegate next,
        ILogger<ComplianceMiddleware> logger,
        ITenantContext tenantContext)
    {
        _next = next;
        _logger = logger;
        _tenantContext = tenantContext;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Classify request based on path and tenant
        var classifications = GetRequestClassifications(context);

        // Store classifications for use by other components
        context.Items["DataClassifications"] = classifications;

        // Add compliance headers
        if (classifications.Contains(B2XTaxonomyClassifications.PersonalData))
        {
            context.Response.Headers["X-Data-Classification"] = "Personal";
        }

        if (classifications.Contains(B2XTaxonomyClassifications.PaymentInfo))
        {
            context.Response.Headers["X-Data-Classification"] = "Payment";
        }

        await _next(context);
    }

    private DataClassificationSet GetRequestClassifications(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        var method = context.Request.Method;

        var classifications = new List<DataClassification>();

        // API endpoints with sensitive data
        if (path?.Contains("/api/customers") == true)
        {
            classifications.Add(B2XTaxonomyClassifications.PersonalData);
        }

        if (path?.Contains("/api/orders") == true || path?.Contains("/api/payments") == true)
        {
            classifications.Add(B2XTaxonomyClassifications.PaymentInfo);
        }

        // Admin endpoints
        if (path?.StartsWith("/admin") == true)
        {
            classifications.Add(B2XTaxonomyClassifications.BusinessConfidential);
        }

        return new DataClassificationSet(classifications);
    }
}
```

## Advanced Patterns

### Custom Classification Attributes

```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
public class TenantSensitiveAttribute : DataClassificationAttribute
{
    public TenantSensitiveAttribute() : base(B2XTaxonomyClassifications.BusinessConfidential) { }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
public class AuditLogAttribute : DataClassificationAttribute
{
    public AuditLogAttribute() : base(new DataClassification("AuditTaxonomy", "LogData")) { }
}
```

### Dynamic Classification Based on Context

```csharp
public class ContextualClassifier
{
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextualClassifier(
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public DataClassificationSet ClassifyData(object data, string context)
    {
        var classifications = new List<DataClassification>();

        // Base classifications
        classifications.Add(B2XTaxonomyClassifications.PersonalData);

        // Context-specific classifications
        if (context == "payment")
        {
            classifications.Add(B2XTaxonomyClassifications.PaymentInfo);
        }

        // Tenant-specific rules
        if (_tenantContext.Tenant.Industry == "Healthcare")
        {
            classifications.Add(B2XTaxonomyClassifications.HealthData);
        }

        // Request-specific rules
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request.Headers.ContainsKey("X-Compliance-Level") == true)
        {
            var complianceLevel = httpContext.Request.Headers["X-Compliance-Level"].ToString();
            if (complianceLevel == "high")
            {
                classifications.Add(new DataClassification("ComplianceTaxonomy", "HighSecurity"));
            }
        }

        return new DataClassificationSet(classifications);
    }
}
```

### Compliance-Aware Serialization

```csharp
public class CompliantJsonSerializer
{
    private readonly IRedactorProvider _redactorProvider;

    public CompliantJsonSerializer(IRedactorProvider redactorProvider)
    {
        _redactorProvider = redactorProvider;
    }

    public string SerializeWithCompliance<T>(T obj, DataClassificationSet classifications)
    {
        var redactor = _redactorProvider.GetRedactor(classifications);

        // Create a copy with sensitive fields redacted
        var compliantObj = RedactObject(obj, redactor);

        return JsonSerializer.Serialize(compliantObj);
    }

    private object RedactObject(object obj, Redactor redactor)
    {
        if (obj == null) return null;

        var type = obj.GetType();
        var result = Activator.CreateInstance(type);

        foreach (var prop in type.GetProperties())
        {
            var value = prop.GetValue(obj);

            // Check for classification attributes
            var classificationAttr = prop.GetCustomAttribute<DataClassificationAttribute>();
            if (classificationAttr != null)
            {
                // Redact classified properties
                var redactedValue = redactor.Redact(value?.ToString() ?? "");
                prop.SetValue(result, redactedValue);
            }
            else
            {
                prop.SetValue(result, value);
            }
        }

        return result;
    }
}
```

### Audit Trail with Compliance

```csharp
public class ComplianceAuditService
{
    private readonly ILogger<ComplianceAuditService> _logger;
    private readonly IRedactorProvider _redactorProvider;
    private readonly IAuditRepository _auditRepository;

    public ComplianceAuditService(
        ILogger<ComplianceAuditService> logger,
        IRedactorProvider redactorProvider,
        IAuditRepository auditRepository)
    {
        _logger = logger;
        _redactorProvider = redactorProvider;
        _auditRepository = auditRepository;
    }

    public async Task AuditDataAccessAsync(
        Guid userId,
        string action,
        object data,
        DataClassificationSet classifications)
    {
        var redactor = _redactorProvider.GetRedactor(classifications);

        // Create audit entry with redacted data
        var auditEntry = new AuditEntry
        {
            UserId = userId,
            Action = action,
            Timestamp = DateTime.UtcNow,
            RedactedData = RedactForAudit(data, redactor),
            Classifications = classifications
        };

        await _auditRepository.SaveAsync(auditEntry);

        _logger.LogInformation(
            "Audit: User {UserId} performed {Action} on classified data",
            userId, action);
    }

    private string RedactForAudit(object data, Redactor redactor)
    {
        // Implement audit-specific redaction logic
        var json = JsonSerializer.Serialize(data);
        return redactor.Redact(json);
    }
}
```

## Testing

### Testing Data Classification

```csharp
using Microsoft.Extensions.Compliance.Classification;
using Xunit;

public class DataClassificationTests
{
    [Fact]
    public void B2XTaxonomyClassifications_AreCorrectlyDefined()
    {
        // Test taxonomy name
        Assert.Equal("B2XTaxonomy", B2XTaxonomyClassifications.Name);

        // Test individual classifications
        Assert.Equal("B2XTaxonomy", B2XTaxonomyClassifications.PersonalData.TaxonomyName);
        Assert.Equal("PersonalData", B2XTaxonomyClassifications.PersonalData.Value);

        // Test composite classifications
        var sensitiveSet = B2XTaxonomyClassifications.SensitivePersonalData;
        Assert.True(sensitiveSet.Contains(B2XTaxonomyClassifications.PersonalData));
        Assert.True(sensitiveSet.Contains(B2XTaxonomyClassifications.PaymentInfo));
    }

    [Fact]
    public void CustomAttributes_InheritCorrectly()
    {
        var personalAttr = new PersonalDataAttribute();
        Assert.Equal(B2XTaxonomyClassifications.PersonalData, personalAttr.Classification);
    }
}
```

### Testing Redaction

```csharp
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class RedactionTests
{
    [Fact]
    public void StarRedactor_RedactsCorrectly()
    {
        var redactor = new StarRedactor();
        var input = "sensitive data";
        var output = new char[redactor.GetRedactedLength(input)];

        var length = redactor.Redact(input, output);

        Assert.Equal("****", new string(output));
        Assert.Equal(4, length);
    }

    [Fact]
    public async Task RedactionIntegration_WorksWithDI()
    {
        var services = new ServiceCollection();
        services.AddRedaction(builder =>
        {
            builder.SetRedactor<StarRedactor>(B2XTaxonomyClassifications.PersonalData);
        });

        var provider = services.BuildServiceProvider();
        var redactorProvider = provider.GetRequiredService<IRedactorProvider>();

        var redactor = redactorProvider.GetRedactor(
            new DataClassificationSet(B2XTaxonomyClassifications.PersonalData));

        Assert.IsType<StarRedactor>(redactor);
    }

    [Fact]
    public void HmacRedactor_RequiresConfiguration()
    {
        var options = new HmacRedactorOptions
        {
            KeyId = 12345,
            Key = Convert.ToBase64String("test-key-44-chars-minimum-length-here"u8)
        };

        // Test that HMAC redactor can be created with valid options
        var redactor = new HmacRedactor(options);
        Assert.NotNull(redactor);
    }
}
```

### Testing Compliance Integration

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class ComplianceIntegrationTests
{
    [Fact]
    public void Logging_WithRedaction_ProtectsSensitiveData()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.EnableRedaction());
        services.AddRedaction(builder =>
        {
            builder.SetRedactor<StarRedactor>(B2XTaxonomyClassifications.PersonalData);
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<ComplianceIntegrationTests>>();

        // This would normally log with redaction applied
        // In a real test, you'd capture the log output and verify redaction
        logger.LogInformation("User email: {Email}", "test@example.com");
    }

    [Fact]
    public void TenantComplianceService_RespectsTenantSettings()
    {
        var tenantContextMock = new Mock<ITenantContext>();
        tenantContextMock.Setup(t => t.Tenant.Id).Returns("tenant1");
        tenantContextMock.Setup(t => t.Tenant.Industry).Returns("Healthcare");

        var options = new TenantComplianceOptions
        {
            EnablePersonalDataClassification = true,
            EnablePaymentDataClassification = true,
            EnableHealthDataClassification = true
        };

        var service = new TenantComplianceService(tenantContextMock.Object, options);

        var classifications = service.GetTenantDataClassifications();

        Assert.True(classifications.Contains(B2XTaxonomyClassifications.PersonalData));
        Assert.True(classifications.Contains(B2XTaxonomyClassifications.PaymentInfo));
        Assert.True(classifications.Contains(B2XTaxonomyClassifications.HealthData));
    }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "Compliance": {
    "Redaction": {
      "DefaultRedactor": "StarRedactor",
      "HmacKeyId": 1234567890,
      "HmacKey": "your-base64-encoded-secret-key-here"
    },
    "Classifications": {
      "PersonalData": "B2XTaxonomy:PersonalData",
      "PaymentInfo": "B2XTaxonomy:PaymentInfo",
      "HealthData": "B2XTaxonomy:HealthData"
    }
  },
  "Logging": {
    "EnableRedaction": true
  }
}
```

### Program.cs Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure compliance
builder.Services.AddCompliance(options =>
{
    // Bind from configuration
    builder.Configuration.GetSection("Compliance").Bind(options);
});

// Configure redaction
builder.Services.AddRedaction(redactionBuilder =>
{
    // Configure based on compliance options
    var complianceOptions = builder.Configuration
        .GetSection("Compliance")
        .Get<ComplianceOptions>();

    redactionBuilder.SetRedactor<StarRedactor>(
        B2XTaxonomyClassifications.PersonalData,
        B2XTaxonomyClassifications.PaymentInfo);

    if (complianceOptions.UseHmacForHealthData)
    {
        redactionBuilder.SetHmacRedactor(options =>
        {
            options.KeyId = complianceOptions.HmacKeyId;
            options.Key = complianceOptions.HmacKey;
        }, B2XTaxonomyClassifications.HealthData);
    }
});

// Enable redaction in logging
builder.Logging.EnableRedaction();

var app = builder.Build();

// Add compliance middleware
app.UseMiddleware<ComplianceMiddleware>();
```

## Limitations

- HMAC redactor is experimental (EXTEXP0002 warning)
- Redaction only applies to logging when explicitly enabled
- Custom redactors must be thread-safe
- Classification attributes are not inherited by default
- No built-in support for dynamic classification rules
- Redaction performance impact on high-volume logging

## Related Libraries

- **Microsoft.Extensions.Logging**: Core logging infrastructure with redaction integration
- **Microsoft.Extensions.Telemetry**: Enhanced logging with redaction support
- **Microsoft.Extensions.Configuration**: Configuration binding for compliance settings
- **Microsoft.Extensions.Options**: Options pattern for compliance configuration

This library provides B2X with essential compliance capabilities, enabling systematic data protection and privacy compliance across the multi-tenant e-commerce platform while maintaining operational transparency through appropriate logging and audit trails.