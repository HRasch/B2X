using B2Connect.Domain.Search.Models;
using B2Connect.Domain.Search.Services;
using Microsoft.Extensions.Configuration;

namespace B2Connect.Services.Search.Services;

public interface ICatalogIndexer
{
    Task SeedAsync(bool force = false, CancellationToken ct = default);
}

public class CatalogIndexer : ICatalogIndexer
{
    private readonly ICatalogProductProvider _provider;
    private readonly IElasticService _elastic;
    private readonly IConfiguration _config;
    private readonly ILogger<CatalogIndexer> _logger;

    public CatalogIndexer(ICatalogProductProvider provider, IElasticService elastic, IConfiguration config, ILogger<CatalogIndexer> logger)
    {
        _provider = provider;
        _elastic = elastic;
        _config = config;
        _logger = logger;
    }

    public async Task SeedAsync(bool force = false, CancellationToken ct = default)
    {
        _logger.LogInformation("CatalogIndexer: starting seed (force={Force})", force);
        var page = 1;
        var pageSize = _config.GetValue<int?>("Catalog:SeedPageSize") ?? 100;

        while (!ct.IsCancellationRequested)
        {
            var (items, total) = await _provider.GetPageAsync(page, pageSize, cancellationToken);
            var list = items.ToList();
            if (!list.Any()) break;

            // map items to documents first (assign locale default "en")
            var mapped = list.Select(p => new ProductDocument
            {
                Id = (p.id as Guid?)?.ToString() ?? Guid.NewGuid().ToString(),
                Title = p.name ?? string.Empty,
                Description = p.description ?? string.Empty,
                Price = Convert.ToDecimal(p.price ?? 0),
                Available = p.isAvailable ?? false,
                Sector = (p.categories as IEnumerable<string>)?.FirstOrDefault() ?? "general",
                Locale = (p.locale as string) ?? "en"
            }).ToList();

            // Because source items carried tenant id separately, regroup using original list pairing
            var docsByTenantLang = list.Select(p => new
            {
                Tenant = (p.tenantId as Guid?)?.ToString() ?? "default",
                Locale = (p.locale as string) ?? "en",
                Doc = new ProductDocument
                {
                    Id = (p.id as Guid?)?.ToString() ?? Guid.NewGuid().ToString(),
                    Title = p.name ?? string.Empty,
                    Description = p.description ?? string.Empty,
                    Price = Convert.ToDecimal(p.price ?? 0),
                    Available = p.isAvailable ?? false,
                    Sector = (p.categories as IEnumerable<string>)?.FirstOrDefault() ?? "general",
                    Locale = (p.locale as string) ?? "en"
                }
            }).GroupBy(x => new { x.Tenant, x.Locale });

            foreach (var g in docsByTenantLang)
            {
                var tenantId = g.Key.Tenant;
                var locale = g.Key.Locale;

                if (!force)
                {
                    var seeded = await _elastic.IsSeededAsync(tenantId, locale);
                    if (seeded)
                    {
                        _logger.LogInformation("Skipping tenant {Tenant} locale {Locale} (already seeded)", tenantId, locale);
                        continue;
                    }
                }

                var docs = g.Select(x => x.Doc).ToList();
                if (docs.Any())
                {
                    await _elastic.IndexManyAsync(docs, tenantId, locale);
                    await _elastic.MarkSeededAsync(tenantId, docs.Count, locale);
                    _logger.LogInformation("Indexed {Count} products for tenant {Tenant} locale {Locale}", docs.Count, tenantId, locale);
                }
            }

            if (list.Count < pageSize) break;
            page++;
        }

        _logger.LogInformation("CatalogIndexer: completed seeding.");
    }
}
