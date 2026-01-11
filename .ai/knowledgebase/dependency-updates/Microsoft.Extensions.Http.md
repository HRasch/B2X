---
docid: KB-110
title: Microsoft.Extensions.Http
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Http

**Version:** 10.0.1  
**Package:** [Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http)  
**Documentation:** [Make HTTP requests using IHttpClientFactory in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests)

## Overview

Microsoft.Extensions.Http provides the `IHttpClientFactory` interface and extension methods for configuring and creating `HttpClient` instances in ASP.NET Core applications. The library enables proper management of `HttpClient` lifetimes, handler pooling, and outgoing request middleware through delegating handlers.

The factory pattern addresses common issues with manual `HttpClient` management, including socket exhaustion, DNS staleness, and resource leaks. It provides a central location for configuring logical `HttpClient` instances with different settings for various external services.

## Key Features

- **HttpClient Factory**: Creates and manages `HttpClient` instances with proper lifetime management
- **Named Clients**: Configure multiple `HttpClient` instances with different settings
- **Typed Clients**: Strongly-typed wrapper classes for specific API endpoints
- **Handler Pooling**: Automatic pooling and recycling of `HttpMessageHandler` instances
- **Outgoing Middleware**: Support for delegating handlers to modify requests/responses
- **Polly Integration**: Built-in support for resilience policies (retry, circuit breaker, etc.)
- **Logging**: Configurable logging for all HTTP requests
- **Header Propagation**: Automatic propagation of headers from incoming to outgoing requests

## Core Components

### IHttpClientFactory Interface
- **CreateClient()**: Creates a new `HttpClient` instance with default configuration
- **CreateClient(name)**: Creates a named `HttpClient` with specific configuration
- **Handler Management**: Automatic pooling and lifetime management of handlers

### HttpClientBuilder Extensions
- **AddHttpClient()**: Registers `IHttpClientFactory` in DI container
- **ConfigureHttpClient()**: Configures `HttpClient` properties (base address, headers, etc.)
- **AddHttpMessageHandler()**: Adds delegating handlers for middleware
- **ConfigurePrimaryHttpMessageHandler()**: Configures the underlying `HttpMessageHandler`
- **SetHandlerLifetime()**: Controls handler recycling interval

### Consumption Patterns

#### Basic Usage
```csharp
public class BasicHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BasicHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetDataAsync(string url)
    {
        var client = _httpClientFactory.CreateClient();
        return await client.GetStringAsync(url);
    }
}

// Registration
builder.Services.AddHttpClient();
```

#### Named Clients
```csharp
// Registration with configuration
builder.Services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    client.DefaultRequestHeaders.Add("User-Agent", "B2X-Application");
});

// Usage
public class GitHubService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GitHubService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<GitHubBranch>> GetBranchesAsync(string repo)
    {
        var client = _httpClientFactory.CreateClient("GitHub");
        return await client.GetFromJsonAsync<IEnumerable<GitHubBranch>>($"repos/{repo}/branches");
    }
}
```

#### Typed Clients
```csharp
public class GitHubApiClient
{
    private readonly HttpClient _httpClient;

    public GitHubApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Configuration can be done here or in registration
    }

    public async Task<IEnumerable<GitHubBranch>> GetBranchesAsync(string repo)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>($"repos/{repo}/branches");
    }

    public async Task<GitHubUser> GetUserAsync(string username)
    {
        return await _httpClient.GetFromJsonAsync<GitHubUser>($"users/{username}");
    }
}

// Registration
builder.Services.AddHttpClient<GitHubApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    client.DefaultRequestHeaders.Add("User-Agent", "B2X-Application");
});

// Usage
public class UserProfileService
{
    private readonly GitHubApiClient _gitHubClient;

    public UserProfileService(GitHubApiClient gitHubClient)
    {
        _gitHubClient = gitHubClient;
    }

    public async Task<GitHubUser> GetUserProfileAsync(string username)
    {
        return await _gitHubClient.GetUserAsync(username);
    }
}
```

## B2X Integration

The B2X platform extensively uses Microsoft.Extensions.Http for all external HTTP communications, providing consistent configuration, logging, and resilience patterns across the distributed architecture.

### ERP Connector HTTP Client

```csharp
// src/backend/Shared/Domain/ERP/src/Services/ErpHttpClient.cs
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

public class ErpHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ErpHttpClient> _logger;

    public ErpHttpClient(HttpClient httpClient, ILogger<ErpHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ErpProductResponse> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ErpProductResponse>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve product {ProductId} from ERP", productId);
            throw new ErpCommunicationException($"ERP communication failed for product {productId}", ex);
        }
    }

    public async Task UpdateProductStockAsync(string productId, int newStock, CancellationToken cancellationToken = default)
    {
        var updateRequest = new { ProductId = productId, Stock = newStock };
        var content = JsonContent.Create(updateRequest);

        var response = await _httpClient.PutAsync($"api/products/{productId}/stock", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Updated stock for product {ProductId} to {Stock}", productId, newStock);
    }
}

// Registration in ERP module
public static class ErpServiceCollectionExtensions
{
    public static IServiceCollection AddErpHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var erpConfig = configuration.GetSection("ERP");

        services.AddHttpClient<ErpHttpClient>(client =>
        {
            client.BaseAddress = new Uri(erpConfig["BaseUrl"]);
            client.Timeout = TimeSpan.FromSeconds(30);

            // ERP-specific headers
            client.DefaultRequestHeaders.Add("X-API-Key", erpConfig["ApiKey"]);
            client.DefaultRequestHeaders.Add("X-Tenant-Id", erpConfig["TenantId"]);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            // ERP might require specific TLS settings
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        .AddHttpMessageHandler<ErpAuthenticationHandler>() // Custom auth handler
        .AddHttpMessageHandler<ErpLoggingHandler>(); // Custom logging handler

        return services;
    }
}
```

### Store Gateway External API Client

```csharp
// src/backend/Store/Gateway/Services/ExternalApiClient.cs
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

public class ExternalApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalApiClient> _logger;

    public ExternalApiClient(IHttpClientFactory httpClientFactory, ILogger<ExternalApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("PaymentGateway");

        var paymentData = new
        {
            amount = request.Amount,
            currency = request.Currency,
            cardToken = request.CardToken,
            orderId = request.OrderId
        };

        var content = JsonContent.Create(paymentData);
        var response = await client.PostAsync("api/payments", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PaymentResult>(cancellationToken: cancellationToken);

        _logger.LogInformation("Processed payment for order {OrderId}, result: {Status}",
            request.OrderId, result.Status);

        return result;
    }

    public async Task<ShippingQuote> GetShippingQuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("ShippingService");

        var queryParams = new Dictionary<string, string>
        {
            ["origin"] = request.OriginPostalCode,
            ["destination"] = request.DestinationPostalCode,
            ["weight"] = request.WeightKg.ToString(),
            ["service"] = request.ServiceType
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var response = await client.GetAsync($"api/shipping/quote?{queryString}", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ShippingQuote>(cancellationToken: cancellationToken);
    }
}

// Registration in Store Gateway
public static class StoreGatewayServiceCollectionExtensions
{
    public static IServiceCollection AddExternalApiClients(this IServiceCollection services, IConfiguration configuration)
    {
        // Payment Gateway Client
        services.AddHttpClient("PaymentGateway", client =>
        {
            client.BaseAddress = new Uri(configuration["PaymentGateway:BaseUrl"]);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["PaymentGateway:ApiKey"]}");
            client.DefaultRequestHeaders.Add("X-Merchant-Id", configuration["PaymentGateway:MerchantId"]);
        })
        .AddHttpMessageHandler<PaymentGatewayAuthHandler>()
        .AddHttpMessageHandler<PaymentGatewayLoggingHandler>();

        // Shipping Service Client
        services.AddHttpClient("ShippingService", client =>
        {
            client.BaseAddress = new Uri(configuration["ShippingService:BaseUrl"]);
            client.DefaultRequestHeaders.Add("X-API-Key", configuration["ShippingService:ApiKey"]);
        })
        .AddHttpMessageHandler<ShippingServiceAuthHandler>();

        // Register typed client
        services.AddHttpClient<ExternalApiClient>();

        return services;
    }
}
```

### Admin Gateway Third-Party Integration

```csharp
// src/backend/Admin/Gateway/Services/ThirdPartyIntegrationClient.cs
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

public class ThirdPartyIntegrationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ThirdPartyIntegrationClient> _logger;

    public ThirdPartyIntegrationClient(HttpClient httpClient, ILogger<ThirdPartyIntegrationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AnalyticsData> GetAnalyticsDataAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["start_date"] = startDate.ToString("yyyy-MM-dd"),
            ["end_date"] = endDate.ToString("yyyy-MM-dd"),
            ["metrics"] = "page_views,sessions,bounce_rate"
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var response = await _httpClient.GetAsync($"api/analytics?{queryString}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _logger.LogWarning("Rate limited by analytics service, backing off");
            throw new RateLimitException("Analytics service rate limit exceeded");
        }

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<AnalyticsData>(cancellationToken: cancellationToken);
        _logger.LogInformation("Retrieved analytics data for period {Start} to {End}", startDate, endDate);

        return data;
    }

    public async Task SendWebhookAsync(WebhookPayload payload, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(payload);
        var response = await _httpClient.PostAsync("api/webhooks", content, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Webhook endpoint not found, webhook may be disabled");
            return; // Non-critical failure
        }

        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Successfully sent webhook for event {EventType}", payload.EventType);
    }
}

// Registration in Admin Gateway
public static class AdminGatewayServiceCollectionExtensions
{
    public static IServiceCollection AddThirdPartyIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        // Analytics Service
        services.AddHttpClient("AnalyticsService", client =>
        {
            client.BaseAddress = new Uri(configuration["Analytics:BaseUrl"]);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Analytics:ApiKey"]}");
            client.Timeout = TimeSpan.FromMinutes(2); // Analytics queries can be slow
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        // Webhook Service
        services.AddHttpClient("WebhookService", client =>
        {
            client.BaseAddress = new Uri(configuration["Webhooks:BaseUrl"]);
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        });

        // Typed client for third-party integrations
        services.AddHttpClient<ThirdPartyIntegrationClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ThirdParty:BaseUrl"]);
            client.DefaultRequestHeaders.Add("X-Client-Version", "B2X-1.0");
        })
        .AddHttpMessageHandler<ThirdPartyAuthHandler>()
        .AddHttpMessageHandler<ThirdPartyRetryHandler>();

        return services;
    }
}
```

### Custom Delegating Handlers

```csharp
// src/backend/Shared/Infrastructure/Http/Handlers/RequestLoggingHandler.cs
using Microsoft.Extensions.Logging;

public class RequestLoggingHandler : DelegatingHandler
{
    private readonly ILogger<RequestLoggingHandler> _logger;

    public RequestLoggingHandler(ILogger<RequestLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;

        // Log outgoing request
        _logger.LogInformation("HTTP Request {RequestId}: {Method} {Uri}",
            requestId, request.Method, request.RequestUri);

        if (request.Content != null)
        {
            _logger.LogDebug("HTTP Request {RequestId} Content-Type: {ContentType}, Length: {Length}",
                requestId, request.Content.Headers.ContentType, request.Content.Headers.ContentLength);
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            var duration = DateTime.UtcNow - startTime;

            // Log response
            _logger.LogInformation("HTTP Response {RequestId}: {StatusCode} in {Duration}ms",
                requestId, response.StatusCode, duration.TotalMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "HTTP Request {RequestId} failed after {Duration}ms: {Message}",
                requestId, duration.TotalMilliseconds, ex.Message);
            throw;
        }
    }
}

// src/backend/Shared/Infrastructure/Http/Handlers/TenantHeaderHandler.cs
using Microsoft.AspNetCore.Http;

public class TenantHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Propagate tenant information to external services
        var tenantId = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            request.Headers.Add("X-Tenant-Id", tenantId);
        }

        // Add correlation ID for distributed tracing
        var correlationId = _httpContextAccessor.HttpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(correlationId))
        {
            request.Headers.Add("X-Correlation-Id", correlationId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

// src/backend/Shared/Infrastructure/Http/Handlers/RetryHandler.cs
using Polly;
using Polly.Retry;

public class CustomRetryHandler : DelegatingHandler
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public CustomRetryHandler()
    {
        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => response.StatusCode >= HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Exponential backoff
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
            await base.SendAsync(request, cancellationToken));
    }
}
```

### Service Discovery Integration

```csharp
// src/backend/Infrastructure/Hosting/ServiceDefaults/Extensions.cs
using Microsoft.Extensions.ServiceDiscovery;

public static class ServiceDefaultsExtensions
{
    public static IServiceCollection AddServiceDefaults(this IServiceCollection services)
    {
        // Add service discovery
        services.AddServiceDiscovery();

        // Configure HTTP client with service discovery
        services.AddHttpClient("InternalService", static client =>
        {
            // Use service name instead of hardcoded URL
            client.BaseAddress = new Uri("https://internal-service");
        })
        .AddServiceDiscovery(); // Enable service discovery for this client

        return services;
    }
}
```

### Multitenant HTTP Client Configuration

```csharp
// src/backend/Shared/Infrastructure/Http/MultitenantHttpClientFactory.cs
using Microsoft.Extensions.Http;

public class MultitenantHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITenantContext _tenantContext;

    public MultitenantHttpClientFactory(IHttpClientFactory httpClientFactory, ITenantContext tenantContext)
    {
        _httpClientFactory = httpClientFactory;
        _tenantContext = tenantContext;
    }

    public HttpClient CreateClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);

        // Add tenant-specific headers
        if (_tenantContext.Tenant != null)
        {
            client.DefaultRequestHeaders.Add("X-Tenant-Id", _tenantContext.Tenant.Id);
            client.DefaultRequestHeaders.Add("X-Tenant-Name", _tenantContext.Tenant.Name);
        }

        return client;
    }

    public HttpClient CreateClient(string name, string tenantId)
    {
        var client = _httpClientFactory.CreateClient(name);
        client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);

        return client;
    }
}
```

## Advanced Patterns

### HTTP Client with Circuit Breaker

```csharp
using Polly;
using Polly.Extensions.Http;

public static class HttpClientResilienceExtensions
{
    public static IHttpClientBuilder AddResiliencePipeline(this IHttpClientBuilder builder)
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

        return builder.AddPolicyHandler(retryPolicy)
                     .AddPolicyHandler(circuitBreakerPolicy)
                     .AddPolicyHandler(timeoutPolicy);
    }
}

// Usage
builder.Services.AddHttpClient<ExternalApiClient>()
    .AddResiliencePipeline();
```

### Custom Message Handler Pipeline

```csharp
public class HttpClientPipelineBuilder
{
    private readonly IServiceCollection _services;

    public HttpClientPipelineBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IHttpClientBuilder AddStandardPipeline(string clientName)
    {
        return _services.AddHttpClient(clientName)
            .AddHttpMessageHandler<RequestLoggingHandler>()
            .AddHttpMessageHandler<TenantHeaderHandler>()
            .AddHttpMessageHandler<CustomRetryHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false // Disable cookies for API clients
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(10)); // Custom lifetime
    }
}

// Usage in Program.cs
var pipelineBuilder = new HttpClientPipelineBuilder(builder.Services);
pipelineBuilder.AddStandardPipeline("ExternalApi");
```

### Typed Client with Interface

```csharp
public interface IExternalApiService
{
    Task<UserProfile> GetUserProfileAsync(string userId);
    Task<OrderStatus> GetOrderStatusAsync(string orderId);
    Task<bool> UpdateUserPreferencesAsync(string userId, UserPreferences preferences);
}

public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiService> _logger;

    public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserProfile> GetUserProfileAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"users/{userId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserProfile>();
    }

    public async Task<OrderStatus> GetOrderStatusAsync(string orderId)
    {
        var response = await _httpClient.GetAsync($"orders/{orderId}/status");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderStatus>();
    }

    public async Task<bool> UpdateUserPreferencesAsync(string userId, UserPreferences preferences)
    {
        var content = JsonContent.Create(preferences);
        var response = await _httpClient.PutAsync($"users/{userId}/preferences", content);
        return response.IsSuccessStatusCode;
    }
}

// Registration
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.external-service.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<AuthHandler>();
```

### HTTP Client Testing

```csharp
using Microsoft.Extensions.Http;
using Moq;

public class ExternalApiServiceTests
{
    [Fact]
    public async Task GetUserProfileAsync_ReturnsUserProfile()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var userProfile = new UserProfile { Id = "123", Name = "John Doe" };

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(userProfile)
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://api.test.com/")
        };

        var logger = new Mock<ILogger<ExternalApiService>>();
        var service = new ExternalApiService(httpClient, logger.Object);

        // Act
        var result = await service.GetUserProfileAsync("123");

        // Assert
        Assert.Equal("123", result.Id);
        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task GetUserProfileAsync_HandlesHttpError()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var logger = new Mock<ILogger<ExternalApiService>>();
        var service = new ExternalApiService(httpClient, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            service.GetUserProfileAsync("999"));
    }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "HttpClients": {
    "ERP": {
      "BaseUrl": "https://erp-api.company.com",
      "Timeout": "00:00:30",
      "ApiKey": "your-erp-api-key"
    },
    "PaymentGateway": {
      "BaseUrl": "https://payments.stripe.com",
      "ApiKey": "sk_test_...",
      "MerchantId": "merchant_123"
    },
    "ShippingService": {
      "BaseUrl": "https://shipping.fedex.com",
      "ApiKey": "fedex_api_key"
    }
  }
}
```

### Program.cs Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure HTTP clients
builder.Services.AddHttpClient();

// Configure ERP client
builder.Services.AddHttpClient("ERP", (serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var erpConfig = config.GetSection("HttpClients:ERP");

    client.BaseAddress = new Uri(erpConfig["BaseUrl"]);
    client.Timeout = TimeSpan.Parse(erpConfig["Timeout"]);
    client.DefaultRequestHeaders.Add("X-API-Key", erpConfig["ApiKey"]);
})
.AddHttpMessageHandler<ErpAuthHandler>();

// Configure Payment Gateway client
builder.Services.AddHttpClient("PaymentGateway", (serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var paymentConfig = config.GetSection("HttpClients:PaymentGateway");

    client.BaseAddress = new Uri(paymentConfig["BaseUrl"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {paymentConfig["ApiKey"]}");
    client.DefaultRequestHeaders.Add("Stripe-Version", "2023-10-16");
});

// Register typed clients
builder.Services.AddHttpClient<ErpHttpClient>();
builder.Services.AddHttpClient<ExternalApiClient>();

var app = builder.Build();
```

## Performance Considerations

- **Handler Pooling**: Reuse `HttpMessageHandler` instances to avoid socket exhaustion
- **DNS Refresh**: Default handler lifetime (2 minutes) prevents stale DNS issues
- **Connection Limits**: Configure `HttpClientHandler.MaxConnectionsPerServer` for high-throughput scenarios
- **Timeouts**: Set appropriate timeouts to prevent hanging requests
- **Compression**: Enable gzip/deflate compression for reduced bandwidth
- **Logging Level**: Use appropriate log levels to avoid performance impact

## Security Considerations

- **Certificate Validation**: Always validate SSL certificates in production
- **Header Sanitization**: Avoid logging sensitive headers (authorization, API keys)
- **Timeout Configuration**: Prevent DoS through long-running requests
- **Rate Limiting**: Implement client-side rate limiting for external APIs
- **Secrets Management**: Store API keys securely, not in configuration files

## Related Libraries

- **Microsoft.Extensions.Http.Resilience**: Adds Polly-based resilience policies
- **Microsoft.Extensions.Http.Telemetry**: Adds OpenTelemetry instrumentation
- **Microsoft.Extensions.ServiceDiscovery**: Dynamic service endpoint resolution
- **Polly**: Comprehensive resilience and transient fault-handling library

This library forms the foundation of B2X's HTTP communication layer, enabling reliable, configurable, and maintainable external service integrations across the distributed multi-tenant architecture.