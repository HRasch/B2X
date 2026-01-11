using B2X.Hosting.Domain;

namespace B2X.Hosting.Application;

/// <summary>
/// Service interface for AppHost operations.
/// </summary>
public interface IAppHostService
{
    /// <summary>
    /// Starts an AppHost.
    /// </summary>
    Task StartAsync(Guid appHostId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops an AppHost.
    /// </summary>
    Task StopAsync(Guid appHostId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of an AppHost.
    /// </summary>
    Task<AppHostStatus> GetStatusAsync(Guid appHostId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a health check on an AppHost.
    /// </summary>
    Task<bool> HealthCheckAsync(Guid appHostId, CancellationToken cancellationToken = default);
}