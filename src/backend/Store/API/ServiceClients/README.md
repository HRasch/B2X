# Service Clients

This directory contains HTTP clients for inter-service communication using Aspire Service Discovery.

## Available Clients

- **`IIdentityServiceClient`** - Authentication & User Management (`http://auth-service`)
- **`ITenancyServiceClient`** - Multi-Tenant Management (`http://tenant-service`)
- **`ICatalogServiceClient`** - Product Catalog (`http://catalog-service`)
- **`ILocalizationServiceClient`** - i18n Translations (`http://localization-service`)

## Usage

### 1. Register Service Clients

```csharp
// In Program.cs
using B2X.Shared.Infrastructure.Extensions;

// Register all service clients
builder.Services.AddAllServiceClients();

// Or register individually
builder.Services.AddIdentityServiceClient();
builder.Services.AddCatalogServiceClient();
```

### 2. Inject and Use

```csharp
public class MyService
{
    private readonly ICatalogServiceClient _catalogClient;

    public MyService(ICatalogServiceClient catalogClient)
    {
        _catalogClient = catalogClient;
    }

    public async Task<ProductDto?> GetProductAsync(string sku, Guid tenantId)
    {
        // Service Discovery resolves "http://catalog-service" automatically
        return await _catalogClient.GetProductBySkuAsync(sku, tenantId);
    }
}
```

## How It Works

1. **Service Discovery** resolves `http://catalog-service` to actual endpoint (e.g., `http://localhost:7005` in dev)
2. **HttpClient Factory** manages connection pooling and lifetime
3. **Resilience Policies** (retry, circuit breaker) are applied automatically via `AddStandardResilienceHandler()`

## Creating New Service Clients

```csharp
// 1. Define interface
public interface IMyServiceClient
{
    Task<MyDto?> GetDataAsync(Guid id, CancellationToken ct = default);
}

// 2. Implement client
public class MyServiceClient : IMyServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyServiceClient> _logger;

    public MyServiceClient(HttpClient httpClient, ILogger<MyServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<MyDto?> GetDataAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/data/{id}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MyDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get data {Id}", id);
            return null;
        }
    }
}

// 3. Register in Extensions
public static IServiceCollection AddMyServiceClient(this IServiceCollection services)
{
    services.AddHttpClient<IMyServiceClient, MyServiceClient>(client =>
    {
        client.BaseAddress = new Uri("http://my-service"); // Service Discovery name
    });
    return services;
}
```

## Important Notes

- ? Always use **service names** (e.g., `http://auth-service`), not `localhost:port`
- ? Include `X-Tenant-ID` header for multi-tenant calls
- ? Use `CancellationToken` for all async operations
- ? Log errors but don't throw - return `null` or empty collections
- ? Don't use `.Result` or `.Wait()` - always `await`

## See Also

- [Service Discovery Documentation](../../../docs/SERVICE_DISCOVERY.md)
- [Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/service-discovery/overview)
