using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace B2Connect.Identity.Infrastructure;

/// <summary>
/// Resilience pattern extensions for HttpClient using Polly
/// Implements: Retry, Circuit Breaker, and Timeout policies
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds standardized resilience policies to HttpClientBuilder for ERP integrations
    /// </summary>
    public static IHttpClientBuilder AddErpResiliencePolicies(this IHttpClientBuilder builder)
    {
        // Configure standard resilience handler, then return the original builder so chaining continues
        builder.AddStandardResilienceHandler(options =>
        {
            // Retry Policy: Exponential backoff for transient failures
            options.Retry = new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode switch
                    {
                        // Retry on transient HTTP status codes
                        System.Net.HttpStatusCode.RequestTimeout => true,        // 408
                        System.Net.HttpStatusCode.TooManyRequests => true,       // 429
                        System.Net.HttpStatusCode.InternalServerError => true,   // 500
                        System.Net.HttpStatusCode.BadGateway => true,            // 502
                        System.Net.HttpStatusCode.ServiceUnavailable => true,    // 503
                        System.Net.HttpStatusCode.GatewayTimeout => true,        // 504
                        _ => args.Outcome.Exception is HttpRequestException or TimeoutException
                    }
                )
            };

            // Circuit Breaker: Fail-fast after threshold of failures
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,              // 50% failure rate triggers circuit break
                MinimumThroughput = 10,          // At least 10 requests before evaluating
                SamplingDuration = TimeSpan.FromSeconds(30), // 30-second evaluation window
                BreakDuration = TimeSpan.FromSeconds(30),    // 30-second break period
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode switch
                    {
                        System.Net.HttpStatusCode.BadGateway => true,
                        System.Net.HttpStatusCode.ServiceUnavailable => true,
                        System.Net.HttpStatusCode.GatewayTimeout => true,
                        _ => args.Outcome.Exception is HttpRequestException
                    }
                )
            };
        });

        return builder;
    }

    /// <summary>
    /// Adds resilience policies to HttpClientBuilder for slower third-party services
    /// Increased timeouts and break duration
    /// </summary>
    public static IHttpClientBuilder AddSlowServiceResiliencePolicies(this IHttpClientBuilder builder)
    {
        // Configure standard resilience handler for slower services, keep chaining by returning builder
        builder.AddStandardResilienceHandler(options =>
        {
            // Retry: More lenient for slow services
            options.Retry = new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 4,
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode switch
                    {
                        System.Net.HttpStatusCode.RequestTimeout => true,
                        System.Net.HttpStatusCode.TooManyRequests => true,
                        System.Net.HttpStatusCode.InternalServerError => true,
                        System.Net.HttpStatusCode.BadGateway => true,
                        System.Net.HttpStatusCode.ServiceUnavailable => true,
                        System.Net.HttpStatusCode.GatewayTimeout => true,
                        _ => args.Outcome.Exception is HttpRequestException or TimeoutException
                    }
                )
            };

            // Circuit Breaker: Longer break duration for slow services
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.6,              // 60% failure rate triggers circuit break
                MinimumThroughput = 8,           // At least 8 requests before evaluating
                SamplingDuration = TimeSpan.FromSeconds(45),
                BreakDuration = TimeSpan.FromSeconds(60), // Longer break for slower services
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode switch
                    {
                        System.Net.HttpStatusCode.BadGateway => true,
                        System.Net.HttpStatusCode.ServiceUnavailable => true,
                        System.Net.HttpStatusCode.GatewayTimeout => true,
                        _ => args.Outcome.Exception is HttpRequestException
                    }
                )
            };
        });

        return builder;
    }
}
