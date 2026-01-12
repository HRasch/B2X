using B2X.Shared.Monitoring;
using B2X.Shared.Monitoring.Abstractions;
using B2X.Shared.Monitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace B2X.Monitoring.Endpoints;

/// <summary>
/// Endpoints for monitoring connected services.
/// </summary>
public static class ServiceMonitoringEndpoints
{
    /// <summary>
    /// Get all services for a tenant.
    /// </summary>
    [WolverineGet("/api/v1/monitoring/services")]
    public static async Task<IResult> GetServices(
        [FromQuery] string tenantId,
        [FromQuery] ServiceType? type,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        var services = await monitor.GetServicesAsync(tenantId, type, cancellationToken);
        return Results.Ok(new { services = services.ToArray() });
    }

    /// <summary>
    /// Get a specific service status.
    /// </summary>
    [WolverineGet("/api/v1/monitoring/services/{serviceId}")]
    public static async Task<IResult> GetServiceStatus(
        Guid serviceId,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        var service = await monitor.GetServiceStatusAsync(serviceId, cancellationToken);
        return service != null ? Results.Ok(service) : Results.NotFound();
    }

    /// <summary>
    /// Register a new service for monitoring.
    /// </summary>
    [WolverinePost("/api/v1/monitoring/services")]
    public static async Task<IResult> RegisterService(
        ConnectedService service,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        await monitor.RegisterServiceAsync(service, cancellationToken);
        return Results.Created($"/api/v1/monitoring/services/{service.Id}", service);
    }

    /// <summary>
    /// Unregister a service from monitoring.
    /// </summary>
    [WolverineDelete("/api/v1/monitoring/services/{serviceId}")]
    public static async Task<IResult> UnregisterService(
        Guid serviceId,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        await monitor.UnregisterServiceAsync(serviceId, cancellationToken);
        return Results.NoContent();
    }

    /// <summary>
    /// Test connectivity to a service.
    /// </summary>
    [WolverinePost("/api/v1/monitoring/services/{serviceId}/test")]
    public static async Task<IResult> TestServiceConnectivity(
        Guid serviceId,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        var result = await monitor.TestServiceConnectivityAsync(serviceId, cancellationToken);
        return Results.Ok(result);
    }

    /// <summary>
    /// Update service status (typically called by health checks).
    /// </summary>
    [WolverinePut("/api/v1/monitoring/services/{serviceId}/status")]
    public static async Task<IResult> UpdateServiceStatus(
        Guid serviceId,
        [FromBody] UpdateServiceStatusRequest request,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        await monitor.UpdateServiceStatusAsync(
            serviceId,
            request.Status,
            request.LatencyMs,
            cancellationToken);

        return Results.NoContent();
    }

    /// <summary>
    /// Record resource metrics for a service.
    /// </summary>
    [WolverinePost("/api/v1/monitoring/services/{serviceId}/metrics")]
    public static async Task<IResult> RecordResourceMetrics(
        Guid serviceId,
        ResourceMetrics metrics,
        IServiceMonitor monitor,
        CancellationToken cancellationToken)
    {
        await monitor.RecordResourceMetricsAsync(serviceId, metrics, cancellationToken);
        return Results.NoContent();
    }
}

/// <summary>
/// Request model for updating service status.
/// </summary>
public record UpdateServiceStatusRequest
{
    /// <summary>
    /// The new service status.
    /// </summary>
    public required ServiceStatus Status { get; init; }

    /// <summary>
    /// The latency in milliseconds.
    /// </summary>
    public double LatencyMs { get; init; }
}
