using B2Connect.CatalogService.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.CatalogService.Extensions;

/// <summary>
/// Extension methods for Product Provider registration
/// Configures multiple product data sources (internal, PimCore, nexPIM, Oxomi)
/// </summary>
public static class ProductProviderExtensions
{
    /// <summary>
    /// Register product providers based on configuration
    /// </summary>
    public static IServiceCollection AddProductProviders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register registry and resolver
        services.AddSingleton<IProductProviderRegistry, ProductProviderRegistry>();
        services.AddSingleton<IProductProviderResolver, ProductProviderResolver>();

        var providersSection = configuration.GetSection("ProductProviders");
        if (!providersSection.Exists())
        {
            // Default: just internal provider
            services.AddScoped<IProductProvider>(sp =>
            {
                var readDb = sp.GetRequiredService<CatalogReadDbContext>();
                var logger = sp.GetRequiredService<ILogger<InternalProductProvider>>();
                return new InternalProductProvider(readDb, logger);
            });

            // Register providers
            var registry = services.BuildServiceProvider().GetRequiredService<IProductProviderRegistry>();
            var internalProvider = services.BuildServiceProvider().GetRequiredService<IProductProvider>();
            registry.RegisterProvider(internalProvider, priority: 100);

            return services;
        }

        // Register Internal Provider (always enabled)
        services.AddScoped(sp =>
        {
            var readDb = sp.GetRequiredService<CatalogReadDbContext>();
            var logger = sp.GetRequiredService<ILogger<InternalProductProvider>>();
            return new InternalProductProvider(readDb, logger);
        });

        // Register PimCore Provider if configured
        var pimCoreConfig = GetProviderConfig(providersSection, "pimcore");
        if (pimCoreConfig?.Enabled == true)
        {
            services.AddHttpClient<PimCoreProductProvider>()
                .ConfigureHttpClient(client =>
                {
                    if (!string.IsNullOrEmpty(pimCoreConfig.BaseUrl))
                        client.BaseAddress = new Uri(pimCoreConfig.BaseUrl);
                    if (!string.IsNullOrEmpty(pimCoreConfig.ApiKey))
                        client.DefaultRequestHeaders.Add("X-API-Key", pimCoreConfig.ApiKey);
                    client.Timeout = TimeSpan.FromMilliseconds(pimCoreConfig.TimeoutMs);
                });

            services.AddScoped(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(PimCoreProductProvider));
                var logger = sp.GetRequiredService<ILogger<PimCoreProductProvider>>();
                return new PimCoreProductProvider(httpClient, pimCoreConfig, logger) as IProductProvider;
            });
        }

        // Register NexPIM Provider if configured
        var nexPimConfig = GetProviderConfig(providersSection, "nexpim");
        if (nexPimConfig?.Enabled == true)
        {
            services.AddHttpClient<NexPIMProductProvider>()
                .ConfigureHttpClient(client =>
                {
                    if (!string.IsNullOrEmpty(nexPimConfig.BaseUrl))
                        client.BaseAddress = new Uri(nexPimConfig.BaseUrl);
                    if (!string.IsNullOrEmpty(nexPimConfig.ApiKey))
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {nexPimConfig.ApiKey}");
                    client.Timeout = TimeSpan.FromMilliseconds(nexPimConfig.TimeoutMs);
                });

            services.AddScoped(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(NexPIMProductProvider));
                var logger = sp.GetRequiredService<ILogger<NexPIMProductProvider>>();
                return new NexPIMProductProvider(httpClient, nexPimConfig, logger) as IProductProvider;
            });
        }

        // Register Oxomi Provider if configured
        var oxomiConfig = GetProviderConfig(providersSection, "oxomi");
        if (oxomiConfig?.Enabled == true)
        {
            services.AddHttpClient<OxomiProductProvider>()
                .ConfigureHttpClient(client =>
                {
                    if (!string.IsNullOrEmpty(oxomiConfig.BaseUrl))
                        client.BaseAddress = new Uri(oxomiConfig.BaseUrl);
                    if (!string.IsNullOrEmpty(oxomiConfig.ApiKey))
                        client.DefaultRequestHeaders.Add("X-API-Key", oxomiConfig.ApiKey);
                    client.Timeout = TimeSpan.FromMilliseconds(oxomiConfig.TimeoutMs);
                });

            services.AddScoped(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(OxomiProductProvider));
                var logger = sp.GetRequiredService<ILogger<OxomiProductProvider>>();
                return new OxomiProductProvider(httpClient, oxomiConfig, logger) as IProductProvider;
            });
        }

        return services;
    }

    /// <summary>
    /// Initialize providers in the registry
    /// Should be called after services are built
    /// </summary>
    public static IApplicationBuilder UseProductProviders(
        this IApplicationBuilder app)
    {
        var registry = app.ApplicationServices.GetRequiredService<IProductProviderRegistry>();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<IProductProviderRegistry>>();

        // Register all discovered providers
        var providers = app.ApplicationServices.GetServices<IProductProvider>();
        foreach (var provider in providers)
        {
            registry.RegisterProvider(provider, GetPriorityForProvider(provider.ProviderName));
            logger.LogInformation("Registered provider: {ProviderName}", provider.ProviderName);
        }

        return app;
    }

    private static ProviderConfig? GetProviderConfig(IConfigurationSection section, string providerName)
    {
        var providerSection = section.GetSection(providerName);
        if (!providerSection.Exists())
            return null;

        var config = new ProviderConfig();
        providerSection.Bind(config);
        return config;
    }

    private static int GetPriorityForProvider(string providerName)
    {
        return providerName switch
        {
            "internal" => 100,      // Highest priority
            "pimcore" => 90,
            "nexpim" => 80,
            "oxomi" => 70,
            _ => 0
        };
    }
}
