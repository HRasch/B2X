---
docid: KB-102
title: Microsoft.Extensions.Logging
owner: @TechLead
status: Active
created: 2026-01-10
---

# Microsoft.Extensions.Logging

**Version:** 10.0.1  
**NuGet:** [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging)  
**Source:** [dotnet/runtime](https://github.com/dotnet/runtime)  
**Documentation:** [learn.microsoft.com/dotnet/core/extensions/logging](https://learn.microsoft.com/dotnet/core/extensions/logging)

## Overview

Microsoft.Extensions.Logging provides the core logging abstractions and implementations for .NET applications. It serves as the foundation for structured logging, supporting multiple providers and enabling consistent logging patterns across the .NET ecosystem.

## Key Features

- **ILogger Interface**: Core logging abstraction with structured logging support
- **ILoggerFactory**: Factory for creating logger instances with provider configuration
- **Built-in Providers**: Console, Debug, EventSource, EventLog, and Azure App Services
- **Filtering**: Configurable log level filtering by category and provider
- **Scopes**: Support for logical operation grouping with scope data
- **Source Generation**: Compile-time logging source generation for performance
- **Activity Tracking**: Integration with System.Diagnostics.Activity for distributed tracing

## Core Interfaces

### ILogger
The primary logging interface providing methods for different log levels:

```csharp
public interface ILogger
{
    void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter);
    bool IsEnabled(LogLevel logLevel);
    IDisposable? BeginScope<TState>(TState state);
}
```

### ILogger<TCategoryName>
Generic logger interface that automatically derives the category name from the type:

```csharp
public interface ILogger<T> : ILogger
{
    // Inherits all ILogger methods
}
```

### ILoggerFactory
Factory interface for creating logger instances:

```csharp
public interface ILoggerFactory : IDisposable
{
    ILogger CreateLogger(string categoryName);
    void AddProvider(ILoggerProvider provider);
}
```

## Log Levels

The framework defines six log levels in order of increasing severity:

```csharp
public enum LogLevel
{
    Trace = 0,      // Most detailed messages, disabled by default
    Debug = 1,      // Debugging information
    Information = 2, // General flow information
    Warning = 3,    // Abnormal events that don't cause failure
    Error = 4,     // Errors and exceptions
    Critical = 5,  // Failures requiring immediate attention
    None = 6       // Disables logging
}
```

## Basic Usage

### Simple Console Logging

```csharp
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("Program");

logger.LogInformation("Hello World! Logging is {Description}.", "fun");
```

### With Dependency Injection

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(); // Automatically added by host builders

var app = builder.Build();

// In service classes
public class OrderService(ILogger<OrderService> logger)
{
    public void ProcessOrder(Order order)
    {
        logger.LogInformation("Processing order {OrderId}", order.Id);
        
        try
        {
            // Process order logic
            logger.LogInformation("Order {OrderId} processed successfully", order.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process order {OrderId}", order.Id);
        }
    }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Console": {
      "IncludeScopes": true,
      "LogLevel": {
        "Microsoft.Extensions.Logging": "Warning"
      }
    }
  }
}
```

### Programmatic Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => 
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});

builder.Logging.AddDebug();
builder.Logging.AddEventSource();

builder.Logging.SetMinimumLevel(LogLevel.Information);
```

## Structured Logging

### Message Templates

```csharp
logger.LogInformation("User {UserId} performed {Action} on {ResourceType} {ResourceId}", 
    userId, action, resourceType, resourceId);
// Output: User 123 performed Update on Product 456
```

### Event IDs

```csharp
public static class LoggingEvents
{
    public const int OrderCreated = 1000;
    public const int OrderUpdated = 1001;
    public const int OrderDeleted = 1002;
}

logger.LogInformation(LoggingEvents.OrderCreated, "Order {OrderId} created by user {UserId}", orderId, userId);
```

### Scopes

```csharp
using (logger.BeginScope("Processing order {OrderId}", orderId))
{
    logger.LogInformation("Validating order details");
    // Validation logic
    logger.LogInformation("Order validation completed");
    
    using (logger.BeginScope("Payment processing"))
    {
        logger.LogInformation("Processing payment of {Amount}", amount);
        // Payment logic
    }
}
```

## Source Generation (Performance)

### LoggerMessage Attribute

```csharp
public partial class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(
        EventId = LoggingEvents.OrderCreated,
        Level = LogLevel.Information,
        Message = "Order {OrderId} created by user {UserId}")]
    public partial void LogOrderCreated(int orderId, string userId);

    [LoggerMessage(
        EventId = LoggingEvents.OrderProcessingFailed,
        Level = LogLevel.Error,
        Message = "Failed to process order {OrderId}")]
    public partial void LogOrderProcessingFailed(int orderId, Exception ex);
}
```

## B2X Integration Patterns

### Multitenant Logging

```csharp
public class TenantAwareLogger
{
    private readonly ILogger _logger;
    private readonly ITenantContext _tenantContext;

    public TenantAwareLogger(ILogger logger, ITenantContext tenantContext)
    {
        _logger = logger;
        _tenantContext = tenantContext;
    }

    public void LogTenantActivity(string action, string details)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["TenantId"] = _tenantContext.TenantId,
            ["TenantName"] = _tenantContext.TenantName
        }))
        {
            _logger.LogInformation("Tenant activity: {Action} - {Details}", action, details);
        }
    }
}
```

### Wolverine CQRS Integration

```csharp
public class OrderHandler
{
    private readonly ILogger<OrderHandler> _logger;
    private readonly IOrderRepository _repository;

    public OrderHandler(ILogger<OrderHandler> logger, IOrderRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [WolverineHandler]
    public async Task Handle(CreateOrderCommand command)
    {
        _logger.LogInformation("Creating order for customer {CustomerId}", command.CustomerId);

        var order = new Order
        {
            CustomerId = command.CustomerId,
            Items = command.Items
        };

        await _repository.SaveAsync(order);

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
    }
}
```

### Elasticsearch Integration

```csharp
public class ElasticsearchLoggerConfiguration
{
    public string ElasticsearchUrl { get; set; }
    public string IndexFormat { get; set; } = "logs-{0:yyyy.MM.dd}";
    public bool IncludeTenantContext { get; set; } = true;
}

public static class ElasticsearchLoggingExtensions
{
    public static ILoggingBuilder AddElasticsearch(
        this ILoggingBuilder builder, 
        Action<ElasticsearchLoggerConfiguration> configure)
    {
        var config = new ElasticsearchLoggerConfiguration();
        configure(config);

        builder.AddProvider(new ElasticsearchLoggerProvider(config));
        return builder;
    }
}
```

### PostgreSQL Integration

```csharp
public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseLoggerProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(categoryName, _scopeFactory);
    }

    public void Dispose() { }
}

public class DatabaseLogger : ILogger
{
    private readonly string _categoryName;
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseLogger(string categoryName, IServiceScopeFactory scopeFactory)
    {
        _categoryName = categoryName;
        _scopeFactory = scopeFactory;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var logEntry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = logLevel.ToString(),
            Category = _categoryName,
            EventId = eventId.Id,
            Message = formatter(state, exception),
            Exception = exception?.ToString()
        };

        dbContext.LogEntries.Add(logEntry);
        dbContext.SaveChanges();
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;
    public IDisposable BeginScope<TState>(TState state) => null;
}
```

## Filtering and Providers

### Custom Filter Function

```csharp
builder.Logging.AddFilter((provider, category, logLevel) =>
{
    // Log all errors and critical messages
    if (logLevel >= LogLevel.Error) return true;
    
    // Log information and above for B2X services
    if (category.StartsWith("B2X.")) return logLevel >= LogLevel.Information;
    
    // Default to warning and above
    return logLevel >= LogLevel.Warning;
});
```

### Provider-Specific Configuration

```csharp
builder.Logging.AddConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.LogLevel = new Dictionary<string, LogLevel>
    {
        ["B2X.Store"] = LogLevel.Debug,
        ["B2X.Admin"] = LogLevel.Information,
        ["Microsoft"] = LogLevel.Warning
    };
});
```

## Activity Tracking

### Distributed Tracing Integration

```csharp
builder.Logging.Configure(options =>
{
    options.ActivityTrackingOptions = 
        ActivityTrackingOptions.SpanId |
        ActivityTrackingOptions.TraceId |
        ActivityTrackingOptions.ParentId |
        ActivityTrackingOptions.Tags |
        ActivityTrackingOptions.Baggage;
});
```

## Best Practices

### 1. Use Structured Logging
```csharp
// Good: Structured logging with named parameters
logger.LogInformation("Order {OrderId} processed for customer {CustomerId}", orderId, customerId);

// Bad: String concatenation
logger.LogInformation($"Order {orderId} processed for customer {customerId}");
```

### 2. Use Appropriate Log Levels
```csharp
// Information: Normal business flow
logger.LogInformation("Order {OrderId} created", orderId);

// Warning: Unexpected but recoverable
logger.LogWarning("Payment service timeout, retrying");

// Error: Operation failed
logger.LogError(ex, "Payment processing failed for order {OrderId}", orderId);

// Critical: System-wide failure
logger.LogCritical("Database connection lost, system unavailable");
```

### 3. Include Context with Scopes
```csharp
using (logger.BeginScope("Processing order {OrderId} for tenant {TenantId}", orderId, tenantId))
{
    // All logs in this scope will include order and tenant context
    logger.LogInformation("Validating order");
    logger.LogInformation("Processing payment");
    logger.LogInformation("Order completed");
}
```

### 4. Use Source Generation for Performance
```csharp
[LoggerMessage(EventId = 1001, Level = LogLevel.Information, 
    Message = "Order {OrderId} status changed to {Status}")]
public partial void LogOrderStatusChanged(int orderId, string status);
```

### 5. Avoid Sensitive Data Logging
```csharp
// Good: Log identifiers only
logger.LogInformation("User {UserId} authenticated", user.Id);

// Bad: Log sensitive data
logger.LogInformation("User {Email} authenticated with password hash {Hash}", user.Email, user.PasswordHash);
```

## Migration from .NET Core

### From .NET Core 3.1 to .NET 10
- No breaking changes in core logging APIs
- Enhanced source generation support
- Improved activity tracking options
- Better performance with compiled logging methods

### Configuration Changes
```json
// .NET Core 3.1 style (still supported)
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}

// .NET 10 enhanced style
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    },
    "Console": {
      "IncludeScopes": true,
      "FormatterName": "Simple"
    }
  }
}
```

## Troubleshooting

### Common Issues

1. **Logs not appearing**: Check minimum log level configuration
2. **Scope data missing**: Ensure `IncludeScopes: true` for console provider
3. **Performance issues**: Use source generation instead of string interpolation
4. **Memory leaks**: Dispose of logger factory and scopes properly

### Debug Logging
```csharp
// Enable debug logging for specific category
builder.Logging.AddFilter("B2X.Store", LogLevel.Debug);

// Enable all debug logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

## Related Libraries

- **Microsoft.Extensions.Logging.Abstractions**: Core interfaces
- **Microsoft.Extensions.Logging.Console**: Console provider
- **Microsoft.Extensions.Logging.Debug**: Debug output provider
- **Microsoft.Extensions.Logging.EventSource**: EventSource provider
- **Microsoft.Extensions.Logging.EventLog**: Windows Event Log provider
- **Serilog.Extensions.Logging**: Third-party provider
- **NLog.Extensions.Logging**: Third-party provider

## Version History

- **10.0.1**: Latest stable release with .NET 10 support
- **8.0.x**: .NET 8 LTS support
- **6.0.x**: Source generation, activity tracking enhancements
- **5.0.x**: Activity tracking support
- **3.1.x**: .NET Core 3.1 LTS support

---

*This documentation covers Microsoft.Extensions.Logging v10.0.1 integration patterns for the B2X project. For additional details, refer to the [official documentation](https://learn.microsoft.com/dotnet/core/extensions/logging).*"