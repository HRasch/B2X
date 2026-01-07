using B2X.Shared.Infrastructure.ServiceClients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.Shared.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering service clients with Aspire Service Discovery
/// </summary>
public static class ServiceClientExtensions
{
    /// <summary>
    /// Registers the Identity Service HTTP client with service discovery
    /// Service name: "http://auth-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddIdentityServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
        {
            // Service Discovery will resolve "http://auth-service" to the actual endpoint
            client.BaseAddress = new Uri("http://auth-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers the Tenancy Service HTTP client with service discovery
    /// Service name: "http://tenant-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddTenancyServiceClient(this IServiceCollection services)
    {
        // Register memory cache if not already registered
        if (!services.Any(x => x.ServiceType == typeof(IMemoryCache)))
        {
            services.AddMemoryCache();
        }

        services.AddHttpClient<ITenancyServiceClient, TenancyServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://tenant-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers the Catalog Service HTTP client with service discovery
    /// Service name: "http://catalog-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddCatalogServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://catalog-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers the Localization Service HTTP client with service discovery
    /// Service name: "http://localization-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddLocalizationServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<ILocalizationServiceClient, LocalizationServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://localization-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers the Customer Service HTTP client with service discovery
    /// Service name: "http://customer-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddCustomerServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<ICustomerServiceClient, CustomerServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://customer-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers the Usage Service HTTP client with service discovery
    /// Service name: "http://usage-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddUsageServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<IUsageServiceClient, UsageServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://usage-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers the Access Service HTTP client with service discovery
    /// Service name: "http://access-service" (resolved by Aspire)
    /// </summary>
    public static IServiceCollection AddAccessServiceClient(this IServiceCollection services)
    {
        services.AddHttpClient<IAccessServiceClient, AccessServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://access-service");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    /// <summary>
    /// Registers ALL service clients at once
    /// Use this in API Gateway or services that need to communicate with multiple services
    /// </summary>
    public static IServiceCollection AddAllServiceClients(this IServiceCollection services)
    {
        services
            .AddIdentityServiceClient()
            .AddTenancyServiceClient()
            .AddCatalogServiceClient()
            .AddLocalizationServiceClient()
            .AddCustomerServiceClient()
            .AddUsageServiceClient()
            .AddAccessServiceClient();

        return services;
    }
}
