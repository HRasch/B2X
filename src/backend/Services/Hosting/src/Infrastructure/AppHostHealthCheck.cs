using B2X.Hosting.Application;
using B2X.Hosting.Domain;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace B2X.Hosting.Infrastructure;

/// <summary>
/// Health check for AppHost services.
/// </summary>
public class AppHostHealthCheck : IHealthCheck
{
    private readonly IAppHostService _appHostService;
    private readonly IAppHostRepository _appHostRepository;

    public AppHostHealthCheck(IAppHostService appHostService, IAppHostRepository appHostRepository)
    {
        _appHostService = appHostService;
        _appHostRepository = appHostRepository;
    }

    /// <summary>
    /// Checks the health of AppHost services.
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var activeHosts = await _appHostRepository.GetActiveAsync(cancellationToken);
            var unhealthyHosts = new List<string>();

            foreach (var host in activeHosts)
            {
                var isHealthy = await _appHostService.HealthCheckAsync(host.Id, cancellationToken);
                if (!isHealthy)
                {
                    unhealthyHosts.Add(host.Name);
                }
            }

            if (unhealthyHosts.Any())
            {
                return HealthCheckResult.Degraded($"Unhealthy AppHosts: {string.Join(", ", unhealthyHosts)}");
            }

            return HealthCheckResult.Healthy("All AppHosts are healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to check AppHost health", ex);
        }
    }
}