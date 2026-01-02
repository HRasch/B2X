using System.Threading;
using System.Threading.Tasks;

namespace B2Connect.Shared.Monitoring.Abstractions;

/// <summary>
/// Represents a health check that can be executed to determine the health status of a component.
/// </summary>
public interface IHealthCheck
{
    /// <summary>
    /// Gets the name of the health check.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the tags associated with this health check.
    /// </summary>
    IEnumerable<string> Tags { get; }

    /// <summary>
    /// Runs the health check asynchronously.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the health check result.</returns>
    Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Context information for a health check execution.
/// </summary>
public record HealthCheckContext
{
    /// <summary>
    /// Gets the tenant identifier for multi-tenant scenarios.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Gets additional context data.
    /// </summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();
}

/// <summary>
/// Result of a health check execution.
/// </summary>
public record HealthCheckResult
{
    /// <summary>
    /// Gets the status of the health check.
    /// </summary>
    public HealthStatus Status { get; init; }

    /// <summary>
    /// Gets the description of the health check result.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the exception that occurred during the health check, if any.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets the duration of the health check execution.
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Gets additional data associated with the health check result.
    /// </summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Creates a healthy result.
    /// </summary>
    public static HealthCheckResult Healthy(string? description = null, TimeSpan? duration = null) =>
        new()
        {
            Status = HealthStatus.Healthy,
            Description = description,
            Duration = duration ?? TimeSpan.Zero
        };

    /// <summary>
    /// Creates a degraded result.
    /// </summary>
    public static HealthCheckResult Degraded(string? description = null, Exception? exception = null, TimeSpan? duration = null) =>
        new()
        {
            Status = HealthStatus.Degraded,
            Description = description,
            Exception = exception,
            Duration = duration ?? TimeSpan.Zero
        };

    /// <summary>
    /// Creates an unhealthy result.
    /// </summary>
    public static HealthCheckResult Unhealthy(string? description = null, Exception? exception = null, TimeSpan? duration = null) =>
        new()
        {
            Status = HealthStatus.Unhealthy,
            Description = description,
            Exception = exception,
            Duration = duration ?? TimeSpan.Zero
        };
}

/// <summary>
/// Represents the status of a health check.
/// </summary>
public enum HealthStatus
{
    /// <summary>
    /// The health check is healthy.
    /// </summary>
    Healthy,

    /// <summary>
    /// The health check is degraded.
    /// </summary>
    Degraded,

    /// <summary>
    /// The health check is unhealthy.
    /// </summary>
    Unhealthy
}