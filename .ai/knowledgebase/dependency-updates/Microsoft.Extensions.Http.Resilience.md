---
docid: KB-092
title: Microsoft.Extensions.Http.Resilience
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: KB-092
title: Microsoft.Extensions.Http.Resilience
owner: @Backend
status: Active
created: 2026-01-08
---

# Microsoft.Extensions.Http.Resilience

**Overview**: Provides pre-built resilience/hedging handlers for `HttpClient` built on Polly strategies, supporting retry, timeout, circuit breaker, concurrency limiter, and fallback strategies.

**Source**: [dotnet/extensions](https://github.com/dotnet/extensions)  
**NuGet**: `Microsoft.Extensions.Http.Resilience`  
**Documentation**: [HTTP Resilience in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/http-resilience)  
**Current Version**: 10.1.0 (January 2026)

## Key Features

### Standard Resilience Handler
- **AddStandardResilienceHandler()**: Pre-configured pipeline with retry, circuit breaker, and timeout
- **AddStandardHedgingHandler()**: Includes hedging (parallel requests) for improved latency

### Custom Pipeline Builder
- **AddResilienceHandler()**: Build custom resilience pipelines
- **HttpResiliencePipelineBuilder**: Fluent API for configuring strategies

### Strategies Available
- **Retry**: Exponential backoff, jitter, custom retry conditions
- **Circuit Breaker**: Prevent cascading failures
- **Timeout**: Per-request timeouts
- **Rate Limiting**: Control request frequency
- **Hedging**: Send parallel requests to multiple endpoints

## Basic Usage

### Standard Handler
```csharp
using Microsoft.Extensions.Http.Resilience;

services.AddHttpClient("resilient-api")
    .AddStandardResilienceHandler(options =>
    {
        options.CircuitBreaker.MinimumThroughput = 10;
        options.Retry.MaxRetryAttempts = 3;
        options.Timeout.Timeout = TimeSpan.FromSeconds(30);
    });
```

### Hedging Handler
```csharp
services.AddHttpClient("hedged-api")
    .AddStandardHedgingHandler(options =>
    {
        options.Hedging.MaxHedgedAttempts = 3;
        options.Hedging.Delay = TimeSpan.FromMilliseconds(100);
    });
```

### Custom Pipeline
```csharp
services.AddHttpClient("custom-api")
    .AddResilienceHandler("custom-pipeline", builder =>
    {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 5,
            BackoffType = DelayBackoffType.Exponential
        });
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.3,
            MinimumThroughput = 10
        });
        builder.AddTimeout(TimeSpan.FromSeconds(10));
    });
```

## Integration in B2X Project

The B2X project uses Microsoft.Extensions.Http.Resilience for:

- **External API Calls**: Protected calls to ERP systems, payment gateways, and third-party services
- **Service-to-Service Communication**: Reliable communication between microservices
- **Gateway Resilience**: Store and Admin gateways with automatic retry and circuit breaking

### Recommended Configuration

```csharp
// In Program.cs or service registration
builder.Services.AddHttpClient("erp-api")
    .AddStandardResilienceHandler(options =>
    {
        // Configure for ERP system characteristics
        options.Retry.MaxRetryAttempts = 3;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.Timeout.Timeout = TimeSpan.FromMinutes(2); // ERP operations can be slow
    });

builder.Services.AddHttpClient("payment-gateway")
    .AddStandardHedgingHandler(options =>
    {
        // Hedging for payment operations
        options.Hedging.MaxHedgedAttempts = 2;
        options.Hedging.Delay = TimeSpan.FromMilliseconds(50);
    });
```

## Compatibility Notes

### gRPC Compatibility
- **Issue**: `Grpc.Net.ClientFactory` <= 2.63.0 may throw when enabling standard handlers
- **Solution**: Upgrade to `Grpc.Net.ClientFactory` >= 2.64.0 or suppress build-time checks
- **Detection**: Runtime errors when using gRPC clients with resilience handlers

### Application Insights Integration
- **Issue**: Registering resilience before AI (<=2.22.0) can prevent telemetry
- **Solution**: Ensure AI >=2.23.0 or register AI services before resilience handlers
- **Verification**: Check that HTTP telemetry is still collected after enabling resilience

## Testing Considerations

### Unit Testing
```csharp
// Mock HttpMessageHandler for testing
var mockHandler = new Mock<HttpMessageHandler>();
mockHandler.Protected()
    .Setup<Task<HttpResponseMessage>>(
        "SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>())
    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

var client = new HttpClient(mockHandler.Object);
```

### Integration Testing
```csharp
// Test resilience behavior
[Fact]
public async Task HttpClient_Retries_On_Transient_Failure()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddHttpClient("test")
        .AddStandardResilienceHandler()
        .ConfigurePrimaryHttpMessageHandler(() => new TestHandler());

    var provider = services.BuildServiceProvider();
    var factory = provider.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("test");

    // Act
    var response = await client.GetAsync("/api/test");

    // Assert
    // Verify retry behavior
}
```

## Best Practices

1. **Use Named Clients**: Register resilience per client, not globally
2. **Configure Timeouts**: Set appropriate timeouts for your service SLAs
3. **Monitor Circuit Breakers**: Log when circuits open/close
4. **Test Resilience**: Include chaos engineering in testing
5. **Version Compatibility**: Keep Microsoft.Extensions.* packages aligned

## Version History

- **8.0.0**: Initial release with standard handlers
- **8.1.0**: Added hedging support
- **8.2.0**: Enhanced routing strategies
- **10.0.0**: .NET 9 compatibility, improved performance
- **10.1.0**: Latest stable (January 2026)

## References

- [Microsoft.Extensions.Http.Resilience NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Http.Resilience)
- [GitHub Repository](https://github.com/dotnet/extensions)
- [Polly Resilience Framework](https://github.com/App-vNext/Polly)
- [HTTP Client Resilience Patterns](https://learn.microsoft.com/en-us/dotnet/core/extensions/http-resilience)

---

**Next Review**: April 2026
