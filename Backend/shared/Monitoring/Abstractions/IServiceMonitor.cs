using System.Threading;
using System.Threading.Tasks;
using B2Connect.Shared.Monitoring.Models;

namespace B2Connect.Shared.Monitoring.Abstractions;

/// <summary>
/// Interface for monitoring connected services.
/// </summary>
public interface IServiceMonitor
{
    /// <summary>
    /// Registers a service for monitoring.
    /// </summary>
    /// <param name="service">The service to register.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RegisterServiceAsync(ConnectedService service, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters a service from monitoring.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task UnregisterServiceAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of a service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="status">The new status.</param>
    /// <param name="latencyMs">The current latency in milliseconds.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task UpdateServiceStatusAsync(
        Guid serviceId,
        ServiceStatus status,
        double latencyMs = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records resource metrics for a service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="metrics">The resource metrics.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RecordResourceMetricsAsync(
        Guid serviceId,
        ResourceMetrics metrics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current status of a service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The service status.</returns>
    Task<ConnectedService?> GetServiceStatusAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all services for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="type">Optional service type filter.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of services.</returns>
    Task<IEnumerable<ConnectedService>> GetServicesAsync(
        string tenantId,
        ServiceType? type = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests connectivity to a service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The test result.</returns>
    Task<ServiceTestResult> TestServiceConnectivityAsync(
        Guid serviceId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a service connectivity test.
/// </summary>
public record ServiceTestResult
{
    /// <summary>
    /// Gets whether the test was successful.
    /// </summary>
    public bool IsSuccessful { get; init; }

    /// <summary>
    /// Gets the latency in milliseconds.
    /// </summary>
    public double LatencyMs { get; init; }

    /// <summary>
    /// Gets the error message if the test failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets additional test data.
    /// </summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();
}