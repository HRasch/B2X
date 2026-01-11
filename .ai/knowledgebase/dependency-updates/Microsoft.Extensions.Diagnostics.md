---
docid: KB-100
title: Microsoft.Extensions.Diagnostics
owner: @Backend
status: Active
created: 2026-01-08
---

# Microsoft.Extensions.Diagnostics

**Overview**: Provides the default implementation of IMeterFactory and extension methods to easily register metrics collection with the dependency injection framework.

**Source**: [dotnet/extensions](https://github.com/dotnet/extensions)  
**NuGet**: `Microsoft.Extensions.Diagnostics`  
**Documentation**: [.NET Metrics](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics)  
**Current Version**: 10.0.1 (January 2026)

## Key Features

### Metrics Collection
- **IMeterFactory**: Factory for creating metric meters
- **Meter**: Core class for recording metrics
- **Counter**: Monotonically increasing values (requests served, errors)
- **Histogram**: Distribution of values (request duration, response size)
- **ObservableGauge**: Point-in-time measurements (current connections, memory usage)

### Dependency Injection Integration
- **AddMetrics()**: Registers metrics services with DI container
- **IMeterFactory**: Injectable service for creating meters
- **MeterOptions**: Configuration for meter behavior

## Basic Usage

### Setup
```csharp
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMetrics(); // Registers IMeterFactory

using IHost host = builder.Build();

IMeterFactory meterFactory = host.Services.GetRequiredService<IMeterFactory>();
```

### Creating Meters
```csharp
public class RequestMetrics
{
    private readonly Counter<long> _requestsTotal;
    private readonly Histogram<double> _requestDuration;
    private readonly ObservableGauge<long> _activeConnections;

    public RequestMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("B2X.Api");

        _requestsTotal = meter.CreateCounter<long>(
            "http_requests_total",
            description: "Total number of HTTP requests");

        _requestDuration = meter.CreateHistogram<double>(
            "http_request_duration_seconds",
            unit: "s",
            description: "HTTP request duration in seconds");

        _activeConnections = meter.CreateObservableGauge<long>(
            "http_active_connections",
            observeValues: () => GetActiveConnectionCount(),
            description: "Number of active HTTP connections");
    }

    public void RecordRequest(string method, string endpoint, double duration)
    {
        _requestsTotal.Add(1, new KeyValuePair<string, object?>("method", method),
                          new KeyValuePair<string, object?>("endpoint", endpoint));

        _requestDuration.Record(duration, new KeyValuePair<string, object?>("method", method),
                               new KeyValuePair<string, object?>("endpoint", endpoint));
    }

    private IEnumerable<Measurement<long>> GetActiveConnectionCount()
    {
        // Return current active connection count
        yield return new Measurement<long>(GetCurrentConnections());
    }
}
```

### Using Metrics in Services
```csharp
public class ApiService
{
    private readonly RequestMetrics _metrics;

    public ApiService(RequestMetrics metrics)
    {
        _metrics = metrics;
    }

    public async Task HandleRequestAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Process request
            await ProcessRequestAsync(context);

            _metrics.RecordRequest(
                context.Request.Method,
                context.Request.Path,
                stopwatch.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            _metrics.RecordRequest(
                context.Request.Method,
                context.Request.Path,
                stopwatch.Elapsed.TotalSeconds);

            throw;
        }
    }
}
```

## Integration in B2X Project

The B2X project uses Microsoft.Extensions.Diagnostics for:

- **API Performance Monitoring**: Track request rates, response times, error rates
- **Business Metrics**: Monitor order processing, catalog updates, user activity
- **System Health**: Track database connections, cache hit rates, service availability
- **Observability**: Provide metrics for monitoring dashboards and alerting

### Recommended Patterns

#### Service-Level Metrics
```csharp
public class OrderProcessingMetrics
{
    private readonly Counter<long> _ordersProcessed;
    private readonly Counter<long> _ordersFailed;
    private readonly Histogram<double> _processingTime;

    public OrderProcessingMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("B2X.Orders");

        _ordersProcessed = meter.CreateCounter<long>(
            "orders_processed_total",
            description: "Total number of orders processed successfully");

        _ordersFailed = meter.CreateCounter<long>(
            "orders_failed_total",
            description: "Total number of orders that failed processing");

        _processingTime = meter.CreateHistogram<double>(
            "order_processing_duration_seconds",
            unit: "s",
            description: "Time taken to process orders");
    }

    public void RecordOrderProcessed(string orderType, double duration)
    {
        _ordersProcessed.Add(1, new KeyValuePair<string, object?>("type", orderType));
        _processingTime.Record(duration, new KeyValuePair<string, object?>("type", orderType));
    }

    public void RecordOrderFailed(string orderType, string reason)
    {
        _ordersFailed.Add(1,
            new KeyValuePair<string, object?>("type", orderType),
            new KeyValuePair<string, object?>("reason", reason));
    }
}
```

#### Database Metrics
```csharp
public class DatabaseMetrics
{
    private readonly Counter<long> _queriesExecuted;
    private readonly Histogram<double> _queryDuration;
    private readonly ObservableGauge<long> _activeConnections;

    public DatabaseMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("B2X.Database");

        _queriesExecuted = meter.CreateCounter<long>(
            "db_queries_total",
            description: "Total number of database queries executed");

        _queryDuration = meter.CreateHistogram<double>(
            "db_query_duration_seconds",
            unit: "s",
            description: "Database query execution time");

        _activeConnections = meter.CreateObservableGauge<long>(
            "db_connections_active",
            observeValues: () => GetActiveConnections(),
            description: "Number of active database connections");
    }

    public void RecordQuery(string operation, double duration)
    {
        _queriesExecuted.Add(1, new KeyValuePair<string, object?>("operation", operation));
        _queryDuration.Record(duration, new KeyValuePair<string, object?>("operation", operation));
    }

    private IEnumerable<Measurement<long>> GetActiveConnections()
    {
        // Return current active connection count from connection pool
        yield return new Measurement<long>(GetCurrentActiveConnections());
    }
}
```

#### Cache Metrics
```csharp
public class CacheMetrics
{
    private readonly Counter<long> _cacheHits;
    private readonly Counter<long> _cacheMisses;
    private readonly ObservableGauge<long> _cacheSize;

    public CacheMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("B2X.Cache");

        _cacheHits = meter.CreateCounter<long>(
            "cache_hits_total",
            description: "Total number of cache hits");

        _cacheMisses = meter.CreateCounter<long>(
            "cache_misses_total",
            description: "Total number of cache misses");

        _cacheSize = meter.CreateObservableGauge<long>(
            "cache_entries_total",
            observeValues: () => GetCacheSize(),
            description: "Total number of entries in cache");
    }

    public void RecordCacheHit(string cacheName)
    {
        _cacheHits.Add(1, new KeyValuePair<string, object?>("cache", cacheName));
    }

    public void RecordCacheMiss(string cacheName)
    {
        _cacheMisses.Add(1, new KeyValuePair<string, object?>("cache", cacheName));
    }

    private IEnumerable<Measurement<long>> GetCacheSize()
    {
        // Return current cache size
        yield return new Measurement<long>(GetCurrentCacheSize());
    }
}
```

## Metrics Collection Setup

### OpenTelemetry Integration
```csharp
using OpenTelemetry.Metrics;
using Microsoft.Extensions.Diagnostics.Metrics;

builder.Services.AddMetrics();

// Configure OpenTelemetry metrics export
builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.AddMeter("B2X.*") // Collect all B2X meters
               .AddConsoleExporter() // For development
               .AddOtlpExporter(); // For production
    });
```

### Custom Meter Provider
```csharp
builder.Services.AddMetrics(options =>
{
    options.MeterFactory = new CustomMeterFactory();
});

public class CustomMeterFactory : IMeterFactory
{
    public Meter Create(MeterOptions options)
    {
        // Custom meter creation logic
        return new Meter(options);
    }
}
```

## Best Practices

### Metric Naming
- Use lowercase with underscores: `http_requests_total`
- Include units in names when applicable: `request_duration_seconds`
- Use prefixes for different components: `api_`, `db_`, `cache_`
- Follow OpenMetrics/Prometheus naming conventions

### Tag Usage
```csharp
// Good: Use consistent tag names and values
_requestsTotal.Add(1,
    new KeyValuePair<string, object?>("method", "GET"),
    new KeyValuePair<string, object?>("status", "200"));

// Avoid: High cardinality tags
// _requestsTotal.Add(1, new KeyValuePair<string, object?>("user_id", userId));
```

### Performance Considerations
- **Avoid high cardinality**: Don't use user IDs, email addresses, or timestamps as tag values
- **Batch recordings**: Record metrics in batches when possible
- **Use appropriate metric types**: Counters for monotonically increasing values, histograms for distributions
- **Consider memory usage**: Metrics are stored in memory until collected

### Testing Metrics
```csharp
public class MetricsTests
{
    [Fact]
    public void RequestMetrics_RecordsCorrectValues()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMetrics();
        var serviceProvider = services.BuildServiceProvider();

        var meterFactory = serviceProvider.GetRequiredService<IMeterFactory>();
        var metrics = new RequestMetrics(meterFactory);

        // Act
        metrics.RecordRequest("GET", "/api/products", 0.5);

        // Assert - Verify metrics were recorded
        // Note: Actual verification depends on your metrics collection setup
    }
}
```

## Version History

- **8.0.0**: Initial release with .NET 8 support
- **8.1.0**: Enhanced performance and memory management
- **8.2.0**: Added observable gauge support
- **9.0.0**: .NET 9 compatibility
- **10.0.0**: Latest stable (January 2026)

## References

- [Microsoft.Extensions.Diagnostics NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Diagnostics)
- [GitHub Repository](https://github.com/dotnet/extensions)
- [.NET Metrics Documentation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/net/)

---

**Next Review**: April 2026