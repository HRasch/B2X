using Microsoft.Extensions.DependencyInjection;

namespace B2X.Shared.Core.Extensions;

/// <summary>
/// Core Extension Methods - einfache Patterns für alle Services
/// Komplexe Setup-Logik (Wolverine, Logging) gehört in service-spezifische Program.cs
/// </summary>
public static class CoreServiceExtensions
{
    /// <summary>
    /// Registriert Serilog Logging (minimal)
    /// </summary>
    public static IServiceCollection AddSharedLogging(this IServiceCollection services)
    {
        // Logging wird hauptsächlich in Program.cs konfiguriert
        // Diese Methode ist für zukünftige gemeinsame Logging-Patterns reserviert
        return services;
    }

    /// <summary>
    /// Aktiviert Service Discovery
    /// </summary>
    public static IServiceCollection AddServiceDiscovery(this IServiceCollection services)
    {
        // Service Discovery wird aktiviert
        services.AddServiceDiscovery();
        return services;
    }
}
