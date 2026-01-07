using B2X.Identity.Interfaces;

namespace B2X.Identity.Infrastructure.ExternalServices;

/// <summary>
/// Factory zur Erstellung von ERP-Providern basierend auf Konfiguration
///
/// Unterstützte Provider:
/// - "Fake" - Faker-Implementation mit Mock-Daten
/// - "SAP" - SAP-Integration (zu implementieren)
/// - "Oracle" - Oracle-Integration (zu implementieren)
/// - "Custom" - Benutzerdefinierte Implementation
/// </summary>
public interface IErpProviderFactory
{
    /// <summary>
    /// Erstellt einen ERP-Provider basierend auf dem Namen
    /// </summary>
    IErpProvider CreateProvider(string providerName);

    /// <summary>
    /// Gibt Liste aller verfügbaren Provider zurück
    /// </summary>
    IEnumerable<string> GetAvailableProviders();
}

/// <summary>
/// Standard-Implementation der ERP-Provider-Factory
/// </summary>
public class ErpProviderFactory : IErpProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ErpProviderFactory> _logger;

    public ErpProviderFactory(IServiceProvider serviceProvider, ILogger<ErpProviderFactory> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;
    }

    public IErpProvider CreateProvider(string providerName)
    {
        _logger.LogInformation("Creating ERP provider: {ProviderName}", providerName);

        return providerName?.ToLowerInvariant() switch
        {
            "fake" => CreateFakeProvider(),
            "sap" => CreateSapProvider(),
            "oracle" => CreateOracleProvider(),
            _ => throw new ArgumentException($"Unknown ERP provider: {providerName}", nameof(providerName))
        };
    }

    public IEnumerable<string> GetAvailableProviders()
    {
        return new[] { "Fake", "SAP", "Oracle" };
    }

    private FakeErpProvider CreateFakeProvider()
    {
        _logger.LogInformation("Creating Fake ERP provider");
        var logger = _serviceProvider.GetService(typeof(ILogger<FakeErpProvider>)) as ILogger<FakeErpProvider>;
        return new FakeErpProvider(logger ?? throw new InvalidOperationException("Logger not available"));
    }

    private FakeErpProvider CreateSapProvider()
    {
        // TODO: Implementiere SAP-Provider (zu 1. Produktivversion)
        _logger.LogWarning("SAP provider not yet implemented, using Fake provider instead");
        return CreateFakeProvider();
    }

    private FakeErpProvider CreateOracleProvider()
    {
        // TODO: Implementiere Oracle-Provider (zukünftige Erweiterung)
        _logger.LogWarning("Oracle provider not yet implemented, using Fake provider instead");
        return CreateFakeProvider();
    }
}
