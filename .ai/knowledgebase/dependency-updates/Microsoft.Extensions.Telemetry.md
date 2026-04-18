---
docid: KB-107
title: Microsoft.Extensions.Telemetry
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Telemetry

**Version:** 10.1.0  
**Package:** [Microsoft.Extensions.Telemetry](https://www.nuget.org/packages/Microsoft.Extensions.Telemetry)  
**Documentation:** [Telemetry in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/telemetry)

## Overview

Microsoft.Extensions.Telemetry provides advanced logging and telemetry enrichment capabilities for .NET applications. It enhances observability by offering sophisticated log sampling, buffering, enrichment, and latency monitoring features. The library is designed for applications requiring detailed telemetry insights and optimized logging performance.

Built on top of Microsoft.Extensions.Logging, it provides production-ready telemetry features that help reduce log volume, capture critical information, and monitor application performance without compromising on observability.

## Key Features

- **Log Sampling**: Intelligent log filtering to reduce volume while preserving important information
- **Log Buffering**: Temporary in-memory buffering with conditional flushing capabilities
- **Application Enrichment**: Automatic addition of application metadata to logs
- **Latency Monitoring**: Request/response timing and performance data collection
- **Log Redaction**: Sensitive data protection in log entries
- **Stack Trace Enhancement**: Detailed error context with configurable depth
- **Dynamic Configuration**: Runtime configuration updates without application restart

## Core Components

### Log Sampling

#### RandomProbabilisticSampler
- Probability-based log filtering
- Configurable sampling rates by log level
- Dynamic configuration support

#### TraceBasedSampler
- Aligns log sampling with distributed tracing decisions
- Consistent sampling across logs and traces
- Integration with OpenTelemetry tracing

### Log Buffering

#### GlobalLogBuffer
- Application-wide log buffering
- Circular buffer with configurable size
- Conditional flushing based on events
- Preserves original timestamps

### Application Log Enrichment

#### ApplicationLogEnricher
- Adds application metadata to log entries
- Configurable enrichment fields (name, version, environment, etc.)
- Automatic metadata detection

### Latency Monitoring

#### LatencyDataExporter
- Request/response timing collection
- Checkpoint and measure tracking
- Custom exporter implementations
- Console and external system integration

### Logging Enhancements

#### Log Enrichment
- Stack trace capturing with configurable depth
- Exception message inclusion
- File information for stack traces

#### Log Redaction
- Sensitive data protection
- Discriminator-based redaction rules
- Configurable redaction patterns

## Basic Usage

### Log Sampling

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Telemetry.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Random probabilistic sampling - sample 10% of logs
builder.Logging.AddRandomProbabilisticSampler(0.1);

// Sampling with specific log levels
builder.Logging.AddRandomProbabilisticSampler(options =>
{
    options.Rules.Add(new RandomProbabilisticSamplerFilterRule(0.1, logLevel: LogLevel.Information));
    options.Rules.Add(new RandomProbabilisticSamplerFilterRule(1.0, logLevel: LogLevel.Error));
});

// Trace-based sampling for consistency with distributed tracing
builder.Logging.AddTraceBasedSampler();

var host = builder.Build();
```

### Log Buffering

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Telemetry.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Global log buffering for warnings and below
builder.Logging.AddGlobalBuffer(LogLevel.Warning);

// Advanced buffering configuration
builder.Logging.AddGlobalBuffer(options =>
{
    options.Rules.Add(new LogBufferingFilterRule(logLevel: LogLevel.Information));
    options.Rules.Add(new LogBufferingFilterRule(categoryName: "Microsoft.*"));
});

var host = builder.Build();

// Flush buffer on exceptions
public class ErrorHandler
{
    private readonly GlobalLogBuffer _buffer;
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(GlobalLogBuffer buffer, ILogger<ErrorHandler> logger)
    {
        _buffer = buffer;
        _logger = logger;
    }

    public void HandleError(Exception ex)
    {
        _logger.LogError(ex, "Critical error occurred");
        _buffer.Flush(); // Emit all buffered logs
    }
}
```

### Application Log Enrichment

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Telemetry.Enrichment;

var builder = Host.CreateApplicationBuilder(args);

// Basic application enrichment
builder.Services.AddApplicationLogEnricher();

// Configured enrichment
builder.Services.AddApplicationLogEnricher(options =>
{
    options.ApplicationName = true;
    options.BuildVersion = true;
    options.DeploymentRing = true;
    options.EnvironmentName = true;
});

var host = builder.Build();

// Logs will now include application metadata
// Example: "Application: B2X, Version: 1.2.3, Environment: Production"
```

### Latency Monitoring

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Telemetry.Latency;

var builder = Host.CreateApplicationBuilder(args);

// Add latency context
builder.Services.AddLatencyContext();

// Register checkpoints, measures, and tags
builder.Services.RegisterCheckpointNames("databaseQuery", "externalApiCall");
builder.Services.RegisterMeasureNames("responseTime", "processingTime");
builder.Services.RegisterTagNames("userId", "transactionId");

// Add console exporter for development
builder.Services.AddConsoleLatencyDataExporter(options =>
{
    options.OutputCheckpoints = true;
    options.OutputMeasures = true;
    options.OutputTags = true;
});

// Add custom exporter
builder.Services.AddSingleton<ILatencyDataExporter, CustomLatencyExporter>();

var app = builder.Build();

// For ASP.NET Core applications
app.UseRequestLatencyTelemetry(); // Automatically exports latency data
```

### Logging Enhancements

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Telemetry.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Enable log enrichment with stack traces
builder.Logging.EnableEnrichment(options =>
{
    options.CaptureStackTraces = true;
    options.IncludeExceptionMessage = true;
    options.MaxStackTraceLength = 500;
    options.UseFileInfoForStackTraces = true;
});

builder.Services.AddApplicationLogEnricher();

// Enable log redaction
builder.Logging.EnableRedaction(options =>
{
    options.ApplyDiscriminator = true;
});

builder.Services.AddRedaction();

var host = builder.Build();
```

## B2X Integration

The B2X project leverages Microsoft.Extensions.Telemetry for enhanced observability and performance monitoring across its distributed architecture.

### Store API Telemetry

```csharp
// src/backend/Store/API/Program.cs
using Microsoft.Extensions.Telemetry.Logging;
using Microsoft.Extensions.Telemetry.Enrichment;

var builder = WebApplication.CreateBuilder(args);

// Configure telemetry for high-traffic store API
builder.Logging.AddRandomProbabilisticSampler(options =>
{
    // Sample 50% of debug logs, 100% of warnings/errors
    options.Rules.Add(new RandomProbabilisticSamplerFilterRule(0.5, logLevel: LogLevel.Debug));
    options.Rules.Add(new RandomProbabilisticSamplerFilterRule(1.0, logLevel: LogLevel.Warning));
});

// Add application enrichment
builder.Services.AddApplicationLogEnricher(options =>
{
    options.ApplicationName = true;
    options.EnvironmentName = true;
    options.BuildVersion = true;
});

// Add latency monitoring for API performance
builder.Services.AddLatencyContext();
builder.Services.RegisterCheckpointNames("productSearch", "orderProcessing", "paymentValidation");
builder.Services.RegisterMeasureNames("dbQueryTime", "externalApiTime", "totalResponseTime");
builder.Services.RegisterTagNames("tenantId", "userId", "correlationId");

// Use Application Insights for production telemetry
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddApplicationInsightsTelemetry();
}

var app = builder.Build();

// Add latency telemetry middleware
app.UseRequestLatencyTelemetry();
```

### Admin Panel Monitoring

```csharp
// src/backend/Admin/Gateway/Program.cs
using Microsoft.Extensions.Telemetry.Logging;

var builder = WebApplication.CreateBuilder(args);

// Log buffering for admin operations
builder.Logging.AddGlobalBuffer(options =>
{
    // Buffer all info and below logs for potential error analysis
    options.Rules.Add(new LogBufferingFilterRule(logLevel: LogLevel.Information));
    options.Rules.Add(new LogBufferingFilterRule(categoryName: "B2X.Admin.*"));
});

// Enhanced error logging
builder.Logging.EnableEnrichment(options =>
{
    options.CaptureStackTraces = true;
    options.IncludeExceptionMessage = true;
    options.MaxStackTraceLength = 1000;
});

builder.Services.AddApplicationLogEnricher();

// Custom latency exporter for admin operations
builder.Services.AddSingleton<ILatencyDataExporter, AdminLatencyExporter>();

var app = builder.Build();

// Middleware to flush logs on admin errors
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var buffer = context.RequestServices.GetRequiredService<GlobalLogBuffer>();
        buffer.Flush(); // Include buffered logs in error reports
        
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Admin operation failed");
        
        throw;
    }
});
```

### Background Service Telemetry

```csharp
// src/backend/Store/Services/Background/OrderProcessingService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Telemetry.Latency;

public class OrderProcessingService : BackgroundService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly ILatencyContext _latencyContext;
    private readonly IOrderRepository _orderRepository;

    public OrderProcessingService(
        ILogger<OrderProcessingService> logger,
        ILatencyContext latencyContext,
        IOrderRepository orderRepository)
    {
        _logger = logger;
        _latencyContext = latencyContext;
        _orderRepository = orderRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var latencyScope = _latencyContext.CreateScope();
            
            latencyScope.AddCheckpoint("startProcessing");
            
            try
            {
                var pendingOrders = await _orderRepository.GetPendingOrdersAsync();
                latencyScope.AddMeasure("dbQueryTime", latencyScope.Elapsed);
                
                foreach (var order in pendingOrders)
                {
                    latencyScope.AddTag("orderId", order.Id.ToString());
                    
                    await ProcessOrderAsync(order, latencyScope);
                }
                
                latencyScope.AddCheckpoint("processingComplete");
                latencyScope.AddMeasure("totalProcessingTime", latencyScope.Elapsed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Order processing failed");
                latencyScope.AddTag("error", "true");
                throw;
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
    private async Task ProcessOrderAsync(Order order, ILatencyScope scope)
    {
        // Order processing logic with checkpoints
        scope.AddCheckpoint("validationStart");
        await ValidateOrderAsync(order);
        scope.AddMeasure("validationTime", scope.Elapsed);
        
        scope.AddCheckpoint("paymentStart");
        await ProcessPaymentAsync(order);
        scope.AddMeasure("paymentTime", scope.Elapsed);
        
        scope.AddCheckpoint("fulfillmentStart");
        await FulfillOrderAsync(order);
        scope.AddMeasure("fulfillmentTime", scope.Elapsed);
    }
}
```

### Multitenant Telemetry

```csharp
// src/backend/Shared/Infrastructure/Telemetry/MultitenantTelemetryExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Telemetry.Enrichment;

public static class MultitenantTelemetryExtensions
{
    public static IServiceCollection AddMultitenantTelemetry(this IServiceCollection services)
    {
        // Add tenant-aware log enrichment
        services.AddApplicationLogEnricher(options =>
        {
            options.ApplicationName = true;
            options.EnvironmentName = true;
            options.BuildVersion = true;
        });
        
        // Add tenant ID to latency tags
        services.RegisterTagNames("tenantId", "tenantName", "tenantPlan");
        
        // Configure sampling based on tenant plan
        services.AddSingleton<ITenantSamplingStrategy, TenantSamplingStrategy>();
        
        return services;
    }
}

public class TenantSamplingStrategy
{
    private readonly ITenantContext _tenantContext;
    
    public TenantSamplingStrategy(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }
    
    public double GetSamplingRate()
    {
        return _tenantContext.Tenant.Plan switch
        {
            TenantPlan.Basic => 0.1,    // 10% sampling for basic tenants
            TenantPlan.Pro => 0.5,      // 50% sampling for pro tenants
            TenantPlan.Enterprise => 1.0, // 100% sampling for enterprise
            _ => 0.1
        };
    }
}
```

### Error Response Enhancement

```csharp
// src/backend/Shared/Infrastructure/Middleware/TelemetryErrorMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Telemetry.Logging;

public class TelemetryErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TelemetryErrorMiddleware> _logger;
    private readonly GlobalLogBuffer _logBuffer;

    public TelemetryErrorMiddleware(
        RequestDelegate next,
        ILogger<TelemetryErrorMiddleware> _logger,
        GlobalLogBuffer logBuffer)
    {
        _next = next;
        _logger = logger;
        _logBuffer = logBuffer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Flush buffered logs for error context
            _logBuffer.Flush();
            
            // Enhanced error logging with telemetry
            _logger.LogError(ex, 
                "Unhandled exception in {Path} with tenant {TenantId}", 
                context.Request.Path,
                context.GetTenantId());
            
            // Add error details to response
            await WriteErrorResponseAsync(context, ex);
        }
    }
    
    private async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        var errorResponse = new
        {
            Error = "Internal Server Error",
            CorrelationId = context.GetCorrelationId(),
            Timestamp = DateTime.UtcNow
        };
        
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}
```

## Advanced Patterns

### Custom Latency Exporter

```csharp
using Microsoft.Extensions.Telemetry.Latency;

public class ElasticsearchLatencyExporter : ILatencyDataExporter
{
    private readonly ILogger<ElasticsearchLatencyExporter> _logger;
    private readonly IElasticClient _elasticClient;

    public ElasticsearchLatencyExporter(
        ILogger<ElasticsearchLatencyExporter> logger,
        IElasticClient elasticClient)
    {
        _logger = logger;
        _elasticClient = elasticClient;
    }

    public async Task ExportAsync(LatencyData latencyData, CancellationToken cancellationToken)
    {
        try
        {
            var document = new LatencyDocument
            {
                Timestamp = DateTime.UtcNow,
                RequestId = latencyData.RequestId,
                Checkpoints = latencyData.Checkpoints.ToDictionary(c => c.Name, c => c.Timestamp),
                Measures = latencyData.Measures.ToDictionary(m => m.Name, m => m.Value),
                Tags = latencyData.Tags
            };

            await _elasticClient.IndexDocumentAsync(document, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export latency data to Elasticsearch");
        }
    }
}
```

### Dynamic Sampling Configuration

```csharp
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Telemetry.Logging;

public class DynamicSamplingService
{
    private readonly IOptionsMonitor<RandomProbabilisticSamplerOptions> _optionsMonitor;
    private readonly ILogger<DynamicSamplingService> _logger;

    public DynamicSamplingService(
        IOptionsMonitor<RandomProbabilisticSamplerOptions> optionsMonitor,
        ILogger<DynamicSamplingService> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;
        
        _optionsMonitor.OnChange(UpdateSamplingRules);
    }

    private void UpdateSamplingRules(RandomProbabilisticSamplerOptions options)
    {
        _logger.LogInformation("Sampling rules updated: {RuleCount} rules", options.Rules.Count);
        
        foreach (var rule in options.Rules)
        {
            _logger.LogDebug("Rule: Level={Level}, Probability={Probability}", 
                rule.LogLevel, rule.Probability);
        }
    }
    
    public void AdjustSamplingForHighLoad()
    {
        // Reduce sampling during high load
        var options = _optionsMonitor.CurrentValue;
        foreach (var rule in options.Rules.Where(r => r.LogLevel <= LogLevel.Information))
        {
            rule.Probability = Math.Max(0.01, rule.Probability * 0.1); // Reduce to 10% or minimum 1%
        }
    }
}
```

### Log Redaction with Custom Rules

```csharp
using Microsoft.Extensions.Telemetry.Logging;

public class PiiRedactor : ILogRedactor
{
    private readonly Regex _emailRegex = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
    private readonly Regex _phoneRegex = new Regex(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b");
    private readonly Regex _ssnRegex = new Regex(@"\b\d{3}[-]?\d{2}[-]?\d{4}\b");

    public string Redact(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = input;
        
        // Redact emails
        result = _emailRegex.Replace(result, "[EMAIL_REDACTED]");
        
        // Redact phone numbers
        result = _phoneRegex.Replace(result, "[PHONE_REDACTED]");
        
        // Redact SSNs
        result = _ssnRegex.Replace(result, "[SSN_REDACTED]");
        
        return result;
    }
}
```

## Testing

### Testing with Telemetry

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Telemetry.Latency;
using Xunit;

public class OrderServiceTelemetryTests
{
    [Fact]
    public async Task ProcessOrder_RecordsLatencyMetrics()
    {
        // Arrange
        var services = new ServiceCollection();
        
        services.AddLogging();
        services.AddLatencyContext();
        services.AddSingleton<IOrderRepository, MockOrderRepository>();
        services.AddSingleton<OrderService>();
        
        // Add test latency exporter
        var latencyData = new List<LatencyData>();
        services.AddSingleton<ILatencyDataExporter>(new TestLatencyExporter(latencyData));
        
        var serviceProvider = services.BuildServiceProvider();
        var orderService = serviceProvider.GetRequiredService<OrderService>();
        
        // Act
        await orderService.ProcessOrderAsync(123);
        
        // Assert
        Assert.Single(latencyData);
        var data = latencyData.First();
        Assert.Contains(data.Checkpoints, c => c.Name == "validationStart");
        Assert.Contains(data.Checkpoints, c => c.Name == "paymentStart");
        Assert.Contains(data.Measures, m => m.Name == "totalProcessingTime");
    }
}

public class TestLatencyExporter : ILatencyDataExporter
{
    private readonly List<LatencyData> _capturedData;
    
    public TestLatencyExporter(List<LatencyData> capturedData)
    {
        _capturedData = capturedData;
    }
    
    public Task ExportAsync(LatencyData latencyData, CancellationToken cancellationToken)
    {
        _capturedData.Add(latencyData);
        return Task.CompletedTask;
    }
}
```

### Testing Log Buffering

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Telemetry.Logging;
using Microsoft.Extensions.DependencyInjection;

public class LogBufferingTests
{
    [Fact]
    public void GlobalBuffer_FlushesOnError()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => 
        {
            builder.AddGlobalBuffer(LogLevel.Information);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<LogBufferingTests>>();
        var buffer = serviceProvider.GetRequiredService<GlobalLogBuffer>();
        
        // Act - Log some messages
        logger.LogInformation("Info message 1");
        logger.LogInformation("Info message 2");
        logger.LogWarning("Warning message");
        
        // Assert - Messages should be buffered, not yet logged
        // (In real scenario, verify with test logger)
        
        // Flush buffer
        buffer.Flush();
        
        // Verify messages were flushed
        // (Check test logger output)
    }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "Logging": {
    "Sampling": {
      "Rules": [
        {
          "LogLevel": "Information",
          "Probability": 0.1
        },
        {
          "LogLevel": "Warning",
          "Probability": 1.0
        }
      ]
    },
    "Buffering": {
      "Rules": [
        {
          "LogLevel": "Information"
        },
        {
          "CategoryName": "Microsoft.*"
        }
      ]
    }
  },
  "Telemetry": {
    "Enrichment": {
      "CaptureStackTraces": true,
      "IncludeExceptionMessage": true,
      "MaxStackTraceLength": 500
    },
    "Redaction": {
      "ApplyDiscriminator": true
    }
  }
}
```

### Environment-Specific Setup

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Development: More verbose logging, console exporters
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddRandomProbabilisticSampler(1.0); // Sample everything
    builder.Services.AddConsoleLatencyDataExporter();
    builder.Logging.EnableEnrichment(options => options.CaptureStackTraces = true);
}

// Production: Optimized for performance
else
{
    builder.Logging.AddRandomProbabilisticSampler(0.05); // 5% sampling
    builder.Logging.AddGlobalBuffer(LogLevel.Warning);
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Logging.EnableRedaction();
}
```

## Limitations

- Log buffering doesn't preserve log record order (timestamps are preserved)
- No custom configuration per logger provider
- Log scopes are not supported in buffered records
- Some log record properties may be lost during buffering serialization
- Activity SpanId and TraceId not preserved in buffered records

## Related Libraries

- **Microsoft.Extensions.Logging**: Core logging infrastructure
- **Microsoft.Extensions.Diagnostics**: Additional diagnostic capabilities
- **Microsoft.Extensions.Caching**: Caching with telemetry integration
- **OpenTelemetry**: Distributed tracing integration

This library provides B2X with advanced observability capabilities, enabling intelligent log management, performance monitoring, and enhanced debugging across the distributed application architecture.