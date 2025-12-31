using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using B2Connect.CatalogService.Models;

namespace B2Connect.Catalog.Endpoints
{
    // Minimal development implementations to allow local frontend work
    public class DevProductService : IProductService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public DevProductService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<dynamic?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default)
        {
            var demoCount = _configuration.GetValue<int?>("CatalogService:DemoProductCount");
            if (demoCount.HasValue && demoCount.Value > 0)
            {
                var demoSector = _configuration.GetValue<string?>("CatalogService:DemoSector");
                B2Connect.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
                var (items, total) = B2Connect.Catalog.Endpoints.Dev.DemoProductStore.GetPage(1, demoCount.Value);
                var found = items.FirstOrDefault(p => string.Equals((string)p.sku, sku, StringComparison.OrdinalIgnoreCase));
                return Task.FromResult<dynamic?>(found);
            }

            var product = new
            {
                Id = Guid.NewGuid(),
                Sku = sku,
                Name = "Demo Product",
                Price = 9.99m,
                TenantId = tenantId
            };
            return Task.FromResult<dynamic?>(product);
        }
    }

    public class DevSearchIndexService : ISearchIndexService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public DevSearchIndexService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<dynamic> SearchAsync(Guid tenantId, string query, CancellationToken ct = default)
        {
            var demoCount = _configuration.GetValue<int?>("CatalogService:DemoProductCount");
            if (demoCount.HasValue && demoCount.Value > 0)
            {
                var demoSector = _configuration.GetValue<string?>("CatalogService:DemoSector");
                B2Connect.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
                // naive search over generated products
                var (items, total) = B2Connect.Catalog.Endpoints.Dev.DemoProductStore.GetPage(1, demoCount.Value);
                var matches = items.Where(p => ((string)p.name).Contains(query, StringComparison.OrdinalIgnoreCase) || ((string)p.sku).Contains(query, StringComparison.OrdinalIgnoreCase)).Take(20).ToList();
                var result = new { Items = matches, Total = matches.Count };
                return Task.FromResult<dynamic>(result);
            }

            var resultDefault = new
            {
                items = new[] {
                    new { id = Guid.NewGuid(), name = "Demo Product 1", price = 9.99m }
                },
                total = 1
            };
            return Task.FromResult<dynamic>(resultDefault);
        }
    }
}
