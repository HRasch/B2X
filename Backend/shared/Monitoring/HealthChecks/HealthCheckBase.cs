using System.Threading;
using System.Threading.Tasks;
using B2Connect.Shared.Monitoring.Abstractions;

namespace B2Connect.Shared.Monitoring.HealthChecks;

/// <summary>
/// Base class for health check implementations.
/// </summary>
public abstract class HealthCheckBase : IHealthCheck
{
    /// <summary>
    /// Gets the name of the health check.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the tags associated with this health check.
    /// </summary>
    public virtual IEnumerable<string> Tags => Array.Empty<string>();

    /// <summary>
    /// Runs the health check asynchronously.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the health check result.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await CheckHealthCoreAsync(context, cancellationToken);
            stopwatch.Stop();

            return result with { Duration = stopwatch.Elapsed };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return HealthCheckResult.Unhealthy(
                $"Health check failed with exception: {ex.Message}",
                ex,
                stopwatch.Elapsed);
        }
    }

    /// <summary>
    /// Core implementation of the health check logic.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the health check result.</returns>
    protected abstract Task<HealthCheckResult> CheckHealthCoreAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken);
}

/// <summary>
/// Base class for service-specific health checks.
/// </summary>
public abstract class ServiceHealthCheckBase : HealthCheckBase
{
    /// <summary>
    /// Gets the service type this health check is for.
    /// </summary>
    protected abstract ServiceType ServiceType { get; }

    /// <summary>
    /// Gets the tags for this service health check.
    /// </summary>
    public override IEnumerable<string> Tags => new[] { "service", ServiceType.ToString().ToLowerInvariant() };
}

/// <summary>
/// Base class for database health checks.
/// </summary>
public abstract class DatabaseHealthCheckBase : ServiceHealthCheckBase
{
    /// <summary>
    /// Gets the service type (Database).
    /// </summary>
    protected override ServiceType ServiceType => ServiceType.Database;

    /// <summary>
    /// Gets the connection string for the database.
    /// </summary>
    protected abstract string ConnectionString { get; }

    /// <summary>
    /// Tests the database connection.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the connection is successful.</returns>
    protected abstract Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Base class for API health checks.
/// </summary>
public abstract class ApiHealthCheckBase : ServiceHealthCheckBase
{
    /// <summary>
    /// Gets the service type (Api).
    /// </summary>
    protected override ServiceType ServiceType => ServiceType.Api;

    /// <summary>
    /// Gets the base URL of the API.
    /// </summary>
    protected abstract string BaseUrl { get; }

    /// <summary>
    /// Gets the health check endpoint path.
    /// </summary>
    protected virtual string HealthEndpoint => "/health";

    /// <summary>
    /// Gets the timeout for the health check request.
    /// </summary>
    protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(30);

    /// <summary>
    /// Tests the API health endpoint.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the API is healthy.</returns>
    protected abstract Task<bool> TestApiHealthAsync(CancellationToken cancellationToken);
}