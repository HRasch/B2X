---
docid: KB-111
title: Microsoft.Extensions.ServiceDiscovery
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.ServiceDiscovery

**Version:** 10.1.0  
**Package:** [Microsoft.Extensions.ServiceDiscovery](https://www.nuget.org/packages/Microsoft.Extensions.ServiceDiscovery)  
**Documentation:** [Service discovery in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/service-discovery)

## Overview

Microsoft.Extensions.ServiceDiscovery provides a unified abstraction for service discovery in .NET applications. Service discovery allows applications to use logical service names instead of physical addresses (IP addresses and ports) to refer to external services. This is essential in distributed systems and microservices architectures where service instances can be dynamically created, destroyed, or relocated.

The library supports multiple resolution strategies including configuration-based endpoints, platform-provided service discovery (Azure Container Apps, Kubernetes), and DNS SRV records. It integrates seamlessly with IHttpClientFactory and provides change notifications when service endpoints are updated.

## Key Features

- **Logical Service Names**: Use service names instead of hardcoded IP addresses and ports
- **Multiple Resolution Strategies**: Configuration, DNS SRV, platform-provided discovery
- **HTTP Client Integration**: Seamless integration with IHttpClientFactory
- **Change Notifications**: Automatic updates when service endpoints change
- **Scheme Selection**: Support for HTTP/HTTPS fallback and scheme filtering
- **Named Endpoints**: Support for multiple endpoints per service (e.g., API vs dashboard)
- **Extensible Architecture**: Custom providers via IServiceEndpointProvider interface
- **Platform Integration**: Built-in support for Azure Container Apps, Kubernetes, .NET Aspire

## Core Components

### IServiceEndpointResolver Interface
- **GetEndpointsAsync()**: Resolves service names to collections of endpoints
- **Change Notifications**: IChangeToken for monitoring endpoint changes
- **Features**: Extensible IFeatureCollection for endpoint metadata

### ServiceEndpointProvider Architecture
- **IServiceEndpointProvider**: Interface for implementing custom resolution strategies
- **IServiceEndpointProviderFactory**: Factory for creating provider instances
- **Resolution Order**: Providers called in registration order, first successful wins

### ServiceDiscoveryOptions
- **AllowedSchemes**: Restrict permitted URI schemes (http, https, etc.)
- **AllowAllSchemes**: Enable/disable scheme filtering
- **Configuration**: Control resolution behavior

## B2X Integration

The B2X platform extensively uses Microsoft.Extensions.ServiceDiscovery for all inter-service communication in distributed deployments. This enables seamless service-to-service calls without hardcoded addresses, supporting dynamic scaling and multi-environment deployments.

### Store Gateway Service Discovery

```csharp
// src/backend/Store/Gateway/Program.cs
using Microsoft.Extensions.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

// Add service discovery with default providers
builder.Services.AddServiceDiscovery();

// Configure HTTP clients with service discovery
builder.Services.AddHttpClient<CatalogServiceClient>(client =>
{
    // Use logical service name instead of hardcoded URL
    client.BaseAddress = new Uri("https://catalog-service");
})
.AddServiceDiscovery();

builder.Services.AddHttpClient<InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://inventory-service");
})
.AddServiceDiscovery();

builder.Services.AddHttpClient<PricingServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://pricing-service");
})
.AddServiceDiscovery();

// Configure default service discovery for all HTTP clients
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddServiceDiscovery();
});

var app = builder.Build();
```

### Admin Gateway with Named Endpoints

```csharp
// src/backend/Admin/Gateway/Program.cs
using Microsoft.Extensions.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDiscovery();

// Configure clients for different service endpoints
builder.Services.AddHttpClient<ContentManagementClient>(client =>
{
    // Default API endpoint
    client.BaseAddress = new Uri("https://cms-service");
})
.AddServiceDiscovery();

builder.Services.AddHttpClient<ContentManagementAdminClient>(client =>
{
    // Admin dashboard endpoint
    client.BaseAddress = new Uri("https://_admin.cms-service");
})
.AddServiceDiscovery();

builder.Services.AddHttpClient<ContentManagementApiClient>(client =>
{
    // REST API endpoint
    client.BaseAddress = new Uri("https://_api.cms-service");
})
.AddServiceDiscovery();

var app = builder.Build();
```

### ERP Connector with Platform Discovery

```csharp
// src/backend/ERP/Connector/Program.cs
using Microsoft.Extensions.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

// Add service discovery for Kubernetes/AKS deployments
builder.Services.AddServiceDiscovery();

// Configure ERP service clients
builder.Services.AddHttpClient<ErpApiClient>(client =>
{
    client.BaseAddress = new Uri("https://erp-system");
})
.AddServiceDiscovery();

builder.Services.AddHttpClient<ErpWebhookClient>(client =>
{
    client.BaseAddress = new Uri("https://erp-webhooks");
})
.AddServiceDiscovery();

// Add DNS SRV provider for advanced service discovery
builder.Services.AddDnsSrvServiceEndpointProvider();

var app = builder.Build();
```

### Multitenant Service Discovery

```csharp
// src/backend/Shared/Infrastructure/ServiceDiscovery/MultitenantServiceDiscovery.cs
using Microsoft.Extensions.ServiceDiscovery;

public class MultitenantServiceDiscovery
{
    private readonly IServiceEndpointResolver _resolver;
    private readonly ITenantContext _tenantContext;

    public MultitenantServiceDiscovery(
        IServiceEndpointResolver resolver,
        ITenantContext tenantContext)
    {
        _resolver = resolver;
        _tenantContext = tenantContext;
    }

    public async Task<ServiceEndpointCollection> ResolveServiceAsync(string serviceName)
    {
        var tenantId = _tenantContext.Tenant?.Id ?? "default";
        var tenantServiceName = $"{serviceName}-{tenantId}";

        // Try tenant-specific service first, fall back to default
        var endpoints = await _resolver.GetEndpointsAsync(tenantServiceName);
        if (!endpoints.Endpoints.Any())
        {
            endpoints = await _resolver.GetEndpointsAsync(serviceName);
        }

        return endpoints;
    }

    public async Task<ServiceEndpoint> GetRandomEndpointAsync(string serviceName)
    {
        var endpoints = await ResolveServiceAsync(serviceName);
        return endpoints.Endpoints.ElementAt(Random.Shared.Next(endpoints.Endpoints.Count));
    }
}
```

### Configuration-Based Service Discovery

```csharp
// appsettings.Development.json
{
  "Services": {
    "catalog-service": {
      "https": [
        "localhost:8080",
        "localhost:8081"
      ]
    },
    "inventory-service": {
      "https": [
        "localhost:8082"
      ]
    },
    "pricing-service": {
      "https": [
        "localhost:8083",
        "localhost:8084"
      ]
    },
    "cms-service": {
      "https": "localhost:8085",
      "_admin": "localhost:8086",
      "_api": "localhost:8087"
    }
  }
}

// appsettings.Production.json
{
  "Services": {
    "catalog-service": {
      "https": [
        "catalog-service.prod.internal:443",
        "catalog-service-backup.prod.internal:443"
      ]
    },
    "inventory-service": {
      "https": [
        "inventory-service.prod.internal:443"
      ]
    }
  }
}
```

### Kubernetes Deployment Configuration

```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: catalog-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: catalog-service
  template:
    metadata:
      labels:
        app: catalog-service
    spec:
      containers:
      - name: catalog
        image: b2x/catalog:latest
        ports:
        - containerPort: 80
          name: http
        - containerPort: 443
          name: https

---
apiVersion: v1
kind: Service
metadata:
  name: catalog-service
spec:
  selector:
    app: catalog-service
  ports:
  - name: http
    port: 80
    targetPort: 80
  - name: https
    port: 443
    targetPort: 443
  type: ClusterIP
```

### Azure Container Apps Configuration

```csharp
// AppHost/Program.cs (for .NET Aspire)
var builder = DistributedApplication.CreateBuilder(args);

// Define services with service discovery
var catalogService = builder.AddProject<Projects.CatalogService>("catalog-service");
var inventoryService = builder.AddProject<Projects.InventoryService>("inventory-service");
var pricingService = builder.AddProject<Projects.PricingService>("pricing-service");

// Store gateway references the services
var storeGateway = builder.AddProject<Projects.StoreGateway>("store-gateway")
    .WithReference(catalogService)
    .WithReference(inventoryService)
    .WithReference(pricingService);

// Admin gateway with named endpoints
var cmsService = builder.AddProject<Projects.CmsService>("cms-service")
    .WithEndpoint(hostPort: 8080, scheme: "https", name: "api")
    .WithEndpoint(hostPort: 8081, scheme: "https", name: "admin");

var adminGateway = builder.AddProject<Projects.AdminGateway>("admin-gateway")
    .WithReference(cmsService.GetEndpoint("api"))
    .WithReference(cmsService.GetEndpoint("admin"));

builder.Build().Run();
```

### Custom Service Endpoint Provider

```csharp
// src/backend/Shared/Infrastructure/ServiceDiscovery/B2XServiceEndpointProvider.cs
using Microsoft.Extensions.ServiceDiscovery;

public class B2XServiceEndpointProvider : IServiceEndpointProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<B2XServiceEndpointProvider> _logger;

    public B2XServiceEndpointProvider(
        IConfiguration configuration,
        ILogger<B2XServiceEndpointProvider> _logger)
    {
        _configuration = configuration;
        this._logger = logger;
    }

    public async ValueTask PopulateAsync(
        IServiceEndpointProviderContext context,
        CancellationToken cancellationToken)
    {
        var serviceName = context.ServiceName;

        // Check for tenant-specific configuration
        var tenantId = context.GetTenantId();
        var tenantServiceKey = $"Services:{tenantId}:{serviceName}";
        var defaultServiceKey = $"Services:{serviceName}";

        var endpoints = _configuration.GetSection(tenantServiceKey).Exists()
            ? _configuration.GetSection(tenantServiceKey)
            : _configuration.GetSection(defaultServiceKey);

        if (!endpoints.Exists())
        {
            _logger.LogWarning("No endpoints configured for service {ServiceName}", serviceName);
            return;
        }

        // Parse endpoints from configuration
        foreach (var scheme in endpoints.GetChildren())
        {
            var schemeName = scheme.Key;
            var endpointValues = scheme.Get<string[]>();

            foreach (var endpointValue in endpointValues)
            {
                if (Uri.TryCreate($"{schemeName}://{endpointValue}", UriKind.Absolute, out var uri))
                {
                    var endpoint = ServiceEndpoint.Create(uri);
                    endpoint.Features.Set<IHostNameFeature>(new HostNameFeature(uri.Host));
                    context.AddEndpoint(endpoint);
                }
            }
        }
    }
}

// Registration
public static class B2XServiceDiscoveryExtensions
{
    public static IServiceCollection AddB2XServiceDiscovery(this IServiceCollection services)
    {
        services.AddServiceDiscoveryCore();
        services.AddSingleton<IServiceEndpointProviderFactory, B2XServiceEndpointProviderFactory>();

        return services;
    }
}
```

### Service Discovery with Change Notifications

```csharp
// src/backend/Store/Gateway/Services/ServiceHealthMonitor.cs
using Microsoft.Extensions.ServiceDiscovery;

public class ServiceHealthMonitor : IHostedService
{
    private readonly IServiceEndpointResolver _resolver;
    private readonly ILogger<ServiceHealthMonitor> _logger;
    private readonly Dictionary<string, IDisposable> _changeTokens = new();

    public ServiceHealthMonitor(
        IServiceEndpointResolver resolver,
        ILogger<ServiceHealthMonitor> logger)
    {
        _resolver = resolver;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var services = new[] { "catalog-service", "inventory-service", "pricing-service" };

        foreach (var serviceName in services)
        {
            await MonitorServiceAsync(serviceName);
        }
    }

    private async Task MonitorServiceAsync(string serviceName)
    {
        try
        {
            var endpoints = await _resolver.GetEndpointsAsync(serviceName);

            // Monitor for changes
            var changeToken = endpoints.ChangeToken;
            var registration = changeToken.RegisterChangeCallback(
                _ => OnServiceEndpointsChanged(serviceName),
                null);

            _changeTokens[serviceName] = registration;

            _logger.LogInformation("Monitoring {ServiceName} with {Count} endpoints",
                serviceName, endpoints.Endpoints.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to monitor service {ServiceName}", serviceName);
        }
    }

    private void OnServiceEndpointsChanged(string serviceName)
    {
        _logger.LogInformation("Service endpoints changed for {ServiceName}", serviceName);

        // Re-monitor the service
        Task.Run(() => MonitorServiceAsync(serviceName));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var token in _changeTokens.Values)
        {
            token.Dispose();
        }
        _changeTokens.Clear();

        return Task.CompletedTask;
    }
}
```

### Testing Service Discovery

```csharp
using Microsoft.Extensions.ServiceDiscovery;
using Moq;

public class CatalogServiceClientTests
{
    [Fact]
    public async Task GetProductAsync_UsesServiceDiscovery()
    {
        // Arrange
        var mockResolver = new Mock<IServiceEndpointResolver>();
        var endpoints = new ServiceEndpointCollection(
            new[] { ServiceEndpoint.Create(new Uri("https://catalog-service-1:443")) },
            Mock.Of<IChangeToken>());

        mockResolver
            .Setup(r => r.GetEndpointsAsync("catalog-service", It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var httpClient = new HttpClient(new Mock<HttpMessageHandler>().Object);
        var client = new CatalogServiceClient(httpClient, mockResolver.Object);

        // Act
        var result = await client.GetProductAsync("123");

        // Assert
        mockResolver.Verify(r => r.GetEndpointsAsync("catalog-service", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ServiceDiscovery_HandlesEndpointChanges()
    {
        // Arrange
        var mockResolver = new Mock<IServiceEndpointResolver>();
        var changeTokenSource = new CancellationTokenSource();

        var initialEndpoints = new ServiceEndpointCollection(
            new[] { ServiceEndpoint.Create(new Uri("https://catalog-1:443")) },
            new CancellationChangeToken(changeTokenSource.Token));

        var updatedEndpoints = new ServiceEndpointCollection(
            new[]
            {
                ServiceEndpoint.Create(new Uri("https://catalog-1:443")),
                ServiceEndpoint.Create(new Uri("https://catalog-2:443"))
            },
            Mock.Of<IChangeToken>());

        mockResolver
            .SetupSequence(r => r.GetEndpointsAsync("catalog-service", It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialEndpoints)
            .ReturnsAsync(updatedEndpoints);

        // Act - simulate endpoint change
        changeTokenSource.Cancel();

        // Assert - verify change notification
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            initialEndpoints.ChangeToken.WaitForChangedAsync());
    }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "Services": {
    "catalog-service": {
      "https": [
        "catalog.prod.internal:443",
        "catalog-backup.prod.internal:443"
      ],
      "http": [
        "catalog.prod.internal:80"
      ]
    },
    "inventory-service": {
      "https": "inventory.prod.internal:443"
    },
    "cms-service": {
      "https": "cms.prod.internal:443",
      "_admin": "cms-admin.prod.internal:443",
      "_api": "cms-api.prod.internal:443"
    }
  },
  "ServiceDiscovery": {
    "AllowAllSchemes": false,
    "AllowedSchemes": ["https", "http"]
  }
}
```

### Program.cs Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure service discovery options
builder.Services.Configure<ServiceDiscoveryOptions>(options =>
{
    options.AllowAllSchemes = false;
    options.AllowedSchemes = ["https", "http"];
});

// Add service discovery with custom configuration
builder.Services.AddServiceDiscovery();

// Configure configuration-based provider
builder.Services.Configure<ConfigurationServiceEndpointProviderOptions>(options =>
{
    options.SectionName = "Services";
    options.ShouldApplyHostNameMetadata = endpoint =>
        endpoint.Endpoint is DnsEndPoint dnsEp && dnsEp.Host.Contains("internal");
});

// Add DNS SRV provider for Kubernetes
builder.Services.AddDnsSrvServiceEndpointProvider();

// Configure HTTP clients with service discovery
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddServiceDiscovery();
});

var app = builder.Build();
```

## Advanced Patterns

### Load Balancing with Service Discovery

```csharp
public class LoadBalancedHttpClient
{
    private readonly IServiceEndpointResolver _resolver;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LoadBalancedHttpClient> _logger;

    public LoadBalancedHttpClient(
        IServiceEndpointResolver resolver,
        IHttpClientFactory httpClientFactory,
        ILogger<LoadBalancedHttpClient> logger)
    {
        _resolver = resolver;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(
        string serviceName,
        HttpMethod method,
        string path,
        HttpContent content = null)
    {
        var endpoints = await _resolver.GetEndpointsAsync(serviceName);
        var endpoint = SelectEndpoint(endpoints);

        var client = _httpClientFactory.CreateClient();
        var requestUri = new Uri(endpoint.Endpoint, path);

        var request = new HttpRequestMessage(method, requestUri)
        {
            Content = content
        };

        _logger.LogInformation("Sending {Method} request to {Uri}", method, requestUri);

        return await client.SendAsync(request);
    }

    private ServiceEndpoint SelectEndpoint(ServiceEndpointCollection endpoints)
    {
        // Round-robin load balancing
        var endpointList = endpoints.Endpoints.ToList();
        var index = Random.Shared.Next(endpointList.Count);
        return endpointList[index];
    }
}
```

### Circuit Breaker with Service Discovery

```csharp
using Polly;
using Polly.CircuitBreaker;

public class ResilientServiceDiscoveryClient
{
    private readonly IServiceEndpointResolver _resolver;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;

    public ResilientServiceDiscoveryClient(IServiceEndpointResolver resolver)
    {
        _resolver = resolver;

        _circuitBreaker = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }

    public async Task<ServiceEndpointCollection> GetEndpointsWithCircuitBreakerAsync(string serviceName)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            var endpoints = await _resolver.GetEndpointsAsync(serviceName);

            if (!endpoints.Endpoints.Any())
            {
                throw new ServiceDiscoveryException($"No endpoints found for service {serviceName}");
            }

            return endpoints;
        });
    }
}
```

### Service Discovery Health Checks

```csharp
public class ServiceDiscoveryHealthCheck : IHealthCheck
{
    private readonly IServiceEndpointResolver _resolver;
    private readonly IEnumerable<string> _criticalServices;

    public ServiceDiscoveryHealthCheck(
        IServiceEndpointResolver resolver,
        IConfiguration configuration)
    {
        _resolver = resolver;
        _criticalServices = configuration.GetSection("HealthChecks:CriticalServices").Get<string[]>();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var unhealthyServices = new List<string>();

        foreach (var serviceName in _criticalServices)
        {
            try
            {
                var endpoints = await _resolver.GetEndpointsAsync(serviceName, cancellationToken);

                if (!endpoints.Endpoints.Any())
                {
                    unhealthyServices.Add(serviceName);
                }
            }
            catch (Exception ex)
            {
                unhealthyServices.Add($"{serviceName} ({ex.Message})");
            }
        }

        if (unhealthyServices.Any())
        {
            return HealthCheckResult.Unhealthy(
                $"Services with no endpoints: {string.Join(", ", unhealthyServices)}");
        }

        return HealthCheckResult.Healthy("All critical services have endpoints");
    }
}

// Registration
builder.Services.AddHealthChecks()
    .AddCheck<ServiceDiscoveryHealthCheck>("Service Discovery");
```

## Performance Considerations

- **Endpoint Caching**: Service discovery results are cached to avoid repeated resolution
- **Change Notifications**: Use IChangeToken to monitor endpoint changes efficiently
- **Connection Pooling**: HTTP clients automatically pool connections per endpoint
- **DNS Resolution**: Built-in DNS caching prevents excessive DNS lookups
- **Load Balancing**: Distribute requests across multiple service instances

## Security Considerations

- **Endpoint Validation**: Always validate resolved endpoints before use
- **Certificate Validation**: Ensure proper SSL certificate validation for HTTPS endpoints
- **Access Control**: Service discovery should not expose sensitive service locations
- **Network Security**: Use appropriate network policies in containerized environments
- **Authentication**: Configure proper authentication for service-to-service calls

## Related Libraries

- **Microsoft.Extensions.Http**: HTTP client factory integration
- **Microsoft.Extensions.Configuration**: Configuration-based endpoint resolution
- **Microsoft.Extensions.Hosting**: Service lifetime management
- **Polly**: Resilience policies for service calls
- **.NET Aspire**: Development-time service discovery

This library forms the foundation of B2X's service-to-service communication layer, enabling reliable, scalable, and maintainable inter-service communication in distributed deployments across multiple cloud platforms and container orchestrators.