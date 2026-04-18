using B2X.Catalog.Application.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Application.BackgroundJobs;

/// <summary>
/// Background service for processing catalog import jobs
/// Runs periodically to process queued catalog imports
/// </summary>
#pragma warning disable VSTHRD110 // Observe result of async calls
public class CatalogImportJobProcessor : BackgroundService
#pragma warning restore VSTHRD110
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CatalogImportJobProcessor> _logger;
    private Timer? _timer;

    public CatalogImportJobProcessor(
        IServiceProvider serviceProvider,
        ILogger<CatalogImportJobProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Catalog Import Job Processor started");

        // Process jobs immediately on startup, then every 10 seconds
        _timer = new Timer(TimerCallback,
                          null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void TimerCallback(object? state)
    {
        // Fire and forget - errors are handled in ProcessJobsAsync
        var task = Task.Run(async () => await ProcessJobsAsync(CancellationToken.None));
        // Intentionally not awaiting - this is a fire-and-forget background operation
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Catalog Import Job Processor stopping");

        _timer?.Change(Timeout.Infinite, 0);

        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessJobsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<ICatalogImportJobService>();

            await jobService.ProcessPendingJobsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing catalog import jobs");
        }
    }
}
