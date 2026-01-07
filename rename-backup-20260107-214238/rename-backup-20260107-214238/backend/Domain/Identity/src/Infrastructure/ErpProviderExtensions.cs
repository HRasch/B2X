using B2X.Identity.Infrastructure.ExternalServices;
using B2X.Identity.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.Identity.Infrastructure;

/// <summary>
/// Extension-Methoden für ERP-Provider Dependency Injection
///
/// Verwendung in Program.cs:
///
/// // Option 1: Nur Fake-Provider (für Development/Testing)
/// services.AddFakeErpProvider();
///
/// // Option 2: Mit Fallback (Primary + Fallback)
/// services.AddResilientErpProvider("SAP", "Fake");
///
/// // Option 3: Konfigurationsbasiert
/// var erpConfig = configuration.GetSection("Erp:Provider").Value ?? "Fake";
/// services.AddErpProvider(erpConfig);
/// </summary>
public static class ErpProviderExtensions
{
    /// <summary>
    /// Registriert nur den Fake-ERP-Provider (ideal für Development/Testing)
    /// </summary>
    public static IServiceCollection AddFakeErpProvider(this IServiceCollection services)
    {
        services.AddScoped<IErpProvider, FakeErpProvider>();
        services.AddScoped<IErpCustomerService>(sp =>
        {
            var provider = sp.GetRequiredService<IErpProvider>();
            var logger = sp.GetRequiredService<ILogger<ErpProviderAdapter>>();
            return new ErpProviderAdapter(provider, logger);
        });

        return services;
    }

    /// <summary>
    /// Registriert einen Primary-Provider mit Fake-Provider als Fallback
    /// </summary>
    public static IServiceCollection AddResilientErpProvider(
        this IServiceCollection services,
        string primaryProviderName,
        string fallbackProviderName = "Fake")
    {
        services.AddScoped<IErpProviderFactory, ErpProviderFactory>();

        services.AddScoped<IErpProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IErpProviderFactory>();
            var logger = sp.GetRequiredService<ILogger<ResilientErpProviderDecorator>>();

            var primaryProvider = factory.CreateProvider(primaryProviderName);
            var fallbackProvider = factory.CreateProvider(fallbackProviderName);

            return new ResilientErpProviderDecorator(primaryProvider, fallbackProvider, logger);
        });

        services.AddScoped<IErpCustomerService>(sp =>
        {
            var provider = sp.GetRequiredService<IErpProvider>();
            var logger = sp.GetRequiredService<ILogger<ErpProviderAdapter>>();
            return new ErpProviderAdapter(provider, logger);
        });

        return services;
    }

    /// <summary>
    /// Registriert ERP-Provider basierend auf Konfiguration
    /// appsettings.json:
    /// {
    ///   "Erp": {
    ///     "Provider": "Fake",          // Primary provider
    ///     "FallbackProvider": "Fake",  // Fallback (optional)
    ///     "UseResilience": true        // Mit Fallback-Logik (optional)
    ///   }
    /// }
    /// </summary>
    public static IServiceCollection AddErpProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var erpSection = configuration.GetSection("Erp");
        var providerName = erpSection.GetValue<string>("Provider") ?? "Fake";
        var fallbackName = erpSection.GetValue<string>("FallbackProvider") ?? "Fake";
        var useResilience = erpSection.GetValue<bool>("UseResilience", true);

        if (useResilience && !string.Equals(providerName, fallbackName, StringComparison.Ordinal))
        {
            return services.AddResilientErpProvider(providerName, fallbackName);
        }

        return services.AddErpProvider(providerName);
    }

    /// <summary>
    /// Registriert einen spezifischen ERP-Provider nach Name
    /// </summary>
    public static IServiceCollection AddErpProvider(this IServiceCollection services, string providerName)
    {
        services.AddScoped<IErpProviderFactory, ErpProviderFactory>();

        services.AddScoped<IErpProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IErpProviderFactory>();
            return factory.CreateProvider(providerName);
        });

        services.AddScoped<IErpCustomerService>(sp =>
        {
            var provider = sp.GetRequiredService<IErpProvider>();
            var logger = sp.GetRequiredService<ILogger<ErpProviderAdapter>>();
            return new ErpProviderAdapter(provider, logger);
        });

        return services;
    }
}
