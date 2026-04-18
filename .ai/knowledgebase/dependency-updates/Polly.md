---
docid: KB-097
title: Polly Resilience Framework
owner: @Backend
status: Active
created: 2026-01-10
---

# Polly - .NET Resilience and Transient-Fault-Handling Library

**Latest Version**: 8.6.5 (Released: 2024-11-15)  
**NuGet Package**: [Polly](https://www.nuget.org/packages/Polly/)  
**GitHub Repository**: [App-vNext/Polly](https://github.com/App-vNext/Polly)  
**Documentation**: [Polly Docs](https://www.pollydocs.org/)

## Overview

Polly is a comprehensive .NET resilience and transient-fault-handling library that enables developers to express resilience strategies in a fluent and thread-safe manner. The library provides a unified API for combining multiple resilience strategies into composable pipelines that protect applications from transient failures, timeouts, and system overload.

Key features:
- **Retry**: Automatically retry failed operations with configurable delays and backoff strategies
- **Circuit Breaker**: Protect failing systems by temporarily blocking requests when failure thresholds are exceeded
- **Timeout**: Guarantee operations don't wait indefinitely by enforcing time limits
- **Fallback**: Provide alternative values or actions when operations fail
- **Hedging**: Execute parallel requests when operations are slow and use the fastest successful result
- **Rate Limiting**: Control the rate of execution to prevent system overload

## Core API (v8)

### Creating a Basic Resilience Pipeline

```csharp
using Polly;
using Polly.Retry;
using Polly.Timeout;

// Create a basic resilience pipeline
ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions())
    .AddTimeout(TimeSpan.FromSeconds(10))
    .Build();

// Execute synchronously
pipeline.Execute(() => {
    Console.WriteLine("Executing operation");
});

// Execute asynchronously with cancellation token
await pipeline.ExecuteAsync(async token => {
    await SomeAsyncOperation(token);
}, CancellationToken.None);
```

### Creating Generic Result-Based Pipelines

```csharp
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using System.Net;

// Create a generic pipeline for HttpResponseMessage
ResiliencePipeline<HttpResponseMessage> httpPipeline =
    new ResiliencePipelineBuilder<HttpResponseMessage>()
        .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
        {
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .HandleResult(r => r.StatusCode == HttpStatusCode.ServiceUnavailable),
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
        {
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .HandleResult(r => r.StatusCode == HttpStatusCode.InternalServerError),
            FailureRatio = 0.5,
            MinimumThroughput = 10,
            SamplingDuration = TimeSpan.FromSeconds(30),
            BreakDuration = TimeSpan.FromSeconds(30)
        })
        .Build();

// Execute the pipeline
HttpResponseMessage result = await httpPipeline.ExecuteAsync(async token =>
{
    using var httpClient = new HttpClient();
    return await httpClient.GetAsync("https://api.example.com/data", token);
}, CancellationToken.None);
```

## Key Strategies

### Retry Strategy

```csharp
var retryPipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        OnRetry = args =>
        {
            Console.WriteLine($"Retry attempt {args.AttemptNumber}");
            return default;
        }
    })
    .Build();
```

### Circuit Breaker Strategy

```csharp
var circuitBreakerPipeline = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 8,
        BreakDuration = TimeSpan.FromSeconds(30),
        OnOpened = args =>
        {
            Console.WriteLine("Circuit opened");
            return default;
        }
    })
    .Build();
```

### Timeout Strategy

```csharp
var timeoutPipeline = new ResiliencePipelineBuilder()
    .AddTimeout(TimeSpan.FromSeconds(10))
    .Build();
```

### Fallback Strategy

```csharp
var fallbackPipeline = new ResiliencePipelineBuilder<string>()
    .AddFallback(new FallbackStrategyOptions<string>
    {
        FallbackAction = args => Outcome.FromResultAsValueTask("default value")
    })
    .Build();
```

## Dependency Injection Integration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Polly;

// Configure services
var services = new ServiceCollection()
    .AddResiliencePipeline("my-pipeline", (builder, context) =>
    {
        builder
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 })
            .AddTimeout(TimeSpan.FromSeconds(10));
    });

// Retrieve pipeline
var pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
ResiliencePipeline pipeline = pipelineProvider.GetPipeline("my-pipeline");
```

## Combining Multiple Strategies

```csharp
var comprehensivePipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
    .AddFallback(new FallbackStrategyOptions<HttpResponseMessage>
    {
        FallbackAction = args => /* fallback logic */
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
    {
        /* circuit breaker config */
    })
    .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
    {
        /* retry config */
    })
    .AddTimeout(TimeSpan.FromSeconds(10))
    .Build();
```

## Release Information

### Current Stable Release
- **Version**: 8.6.5
- **Release Date**: November 2024
- **Target Frameworks**: .NET 6.0+, .NET Standard 2.0+, .NET Framework 4.6.2+
- **Package Size**: 749.36 KB
- **Downloads**: 2.8M (current version), 1.2B (total)

### Key Changes in v8.x
- Modern builder-based API centered around `ResiliencePipeline`
- Improved async/await support with `ValueTask`
- Better memory management through object pooling
- Enhanced telemetry and monitoring capabilities
- Unified API for all resilience strategies

### Previous Major Versions
- **v7.x**: Legacy Policy-based API (still supported but deprecated)
- **v6.x**: Added hedging strategy
- **v5.x**: Added rate limiting

## Usage in B2X Project

Polly is used extensively in the B2X project for:
- HTTP client resilience in API gateways
- Database connection handling
- External service integrations (ERP connectors)
- Message queue processing

### Integration with Microsoft.Extensions.Http.Resilience

The project uses `Microsoft.Extensions.Http.Resilience` which builds on Polly strategies:

```csharp
services.AddHttpClient("resilient-client")
    .AddResilienceHandler("retry-handler", builder =>
    {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2)
        });
    });
```

## Best Practices

1. **Test Resilience Strategies**: Always test retry and circuit breaker behavior under failure conditions
2. **Monitor Circuit State**: Track circuit breaker state for operational visibility
3. **Use Appropriate Timeouts**: Set timeouts based on SLA requirements
4. **Combine Strategies Wisely**: Order matters - fallback outermost, timeout innermost
5. **Handle Circuit Breaker Exceptions**: Implement fallback logic when circuits open

## Migration from v7 to v8

If upgrading from Polly v7:

```csharp
// Old v7 API
var policy = Policy
    .Handle<HttpRequestException>()
    .RetryAsync(3);

// New v8 API
var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder()
            .Handle<HttpRequestException>(),
        MaxRetryAttempts = 3
    })
    .Build();
```

## Troubleshooting

### Common Issues
- **Circuit stays open**: Check failure ratio and throughput settings
- **Timeouts not working**: Ensure cancellation tokens are properly propagated
- **Memory leaks**: Use `ResilienceContextPool` for context objects

### Debugging
Enable Polly logging to monitor strategy execution:

```csharp
services.AddLogging(builder => builder.AddConsole())
    .AddResiliencePipeline("debug-pipeline", builder =>
    {
        // Strategies will log execution details
    });
```

## Related KB Articles

- [KB-006] Wolverine Patterns (CQRS framework used alongside Polly)
- [KB-024] Microsoft.Extensions.AI (AI integration with resilience)
- [KB-055] Security MCP Best Practices (security considerations)

## References

- [Polly GitHub Repository](https://github.com/App-vNext/Polly)
- [Polly Documentation](https://www.pollydocs.org/)
- [Changelog](https://github.com/App-vNext/Polly/blob/main/CHANGELOG.md)
- [NuGet Package](https://www.nuget.org/packages/Polly/)
