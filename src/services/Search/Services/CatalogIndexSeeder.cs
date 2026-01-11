using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace B2X.Services.Search.Services;

public class CatalogIndexSeeder : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<CatalogIndexSeeder> _logger;

    public CatalogIndexSeeder(IServiceProvider sp, ILogger<CatalogIndexSeeder> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _sp.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var autoSeed = config.GetValue<bool?>("Catalog:AutoSeed") ?? true;
        if (!autoSeed)
        {
            _logger.LogInformation("CatalogIndexSeeder: AutoSeed disabled, skipping startup seeding.");
            return;
        }

        var indexer = scope.ServiceProvider.GetRequiredService<ICatalogIndexer>();
        await indexer.SeedAsync(false, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
