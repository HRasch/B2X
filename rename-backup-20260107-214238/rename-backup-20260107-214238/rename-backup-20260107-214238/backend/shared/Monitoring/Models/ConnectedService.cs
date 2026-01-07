using B2X.Shared.Monitoring;

namespace B2X.Shared.Monitoring.Models;

/// <summary>
/// Represents a connected service in the monitoring system.
/// </summary>
public record ConnectedService
{
    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of the service.
    /// </summary>
    public ServiceType Type { get; init; }

    /// <summary>
    /// Gets the endpoint URL or connection string of the service.
    /// </summary>
    public string Endpoint { get; init; } = string.Empty;

    /// <summary>
    /// Gets the tenant identifier for multi-tenant isolation.
    /// </summary>
    public string TenantId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the current status of the service.
    /// </summary>
    public ServiceStatus Status { get; init; }

    /// <summary>
    /// Gets the timestamp when the service was last checked.
    /// </summary>
    public DateTime LastChecked { get; init; }

    /// <summary>
    /// Gets the timestamp when the service was last successful.
    /// </summary>
    public DateTime? LastSuccessful { get; init; }

    /// <summary>
    /// Gets the average latency in milliseconds.
    /// </summary>
    public double AverageLatencyMs { get; init; }

    /// <summary>
    /// Gets the uptime percentage (0-100).
    /// </summary>
    public double UptimePercent { get; init; }

    /// <summary>
    /// Gets the current resource metrics.
    /// </summary>
    public ResourceMetrics Resources { get; init; } = new();

    /// <summary>
    /// Gets additional metadata associated with the service.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();
}
