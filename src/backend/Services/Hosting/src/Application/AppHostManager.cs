using B2X.Hosting.Domain;

namespace B2X.Hosting.Application;

/// <summary>
/// Manager for AppHost operations.
/// </summary>
public class AppHostManager
{
    private readonly IAppHostService _appHostService;
    private readonly IAppHostRepository _appHostRepository;

    public AppHostManager(IAppHostService appHostService, IAppHostRepository appHostRepository)
    {
        _appHostService = appHostService;
        _appHostRepository = appHostRepository;
    }

    /// <summary>
    /// Starts all active AppHosts.
    /// </summary>
    public async Task StartAllAsync(CancellationToken cancellationToken = default)
    {
        var activeHosts = await _appHostRepository.GetActiveAsync(cancellationToken);
        foreach (var host in activeHosts)
        {
            await _appHostService.StartAsync(host.Id, cancellationToken);
        }
    }

    /// <summary>
    /// Stops all active AppHosts.
    /// </summary>
    public async Task StopAllAsync(CancellationToken cancellationToken = default)
    {
        var activeHosts = await _appHostRepository.GetActiveAsync(cancellationToken);
        foreach (var host in activeHosts)
        {
            await _appHostService.StopAsync(host.Id, cancellationToken);
        }
    }

    /// <summary>
    /// Performs health checks on all active AppHosts.
    /// </summary>
    public async Task<IDictionary<Guid, bool>> HealthCheckAllAsync(CancellationToken cancellationToken = default)
    {
        var activeHosts = await _appHostRepository.GetActiveAsync(cancellationToken);
        var results = new Dictionary<Guid, bool>();

        foreach (var host in activeHosts)
        {
            var isHealthy = await _appHostService.HealthCheckAsync(host.Id, cancellationToken);
            results[host.Id] = isHealthy;
        }

        return results;
    }
}