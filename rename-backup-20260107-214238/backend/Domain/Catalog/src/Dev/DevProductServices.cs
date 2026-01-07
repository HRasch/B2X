using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.Catalog.Endpoints;

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
        if (demoCount > 0)
        {
            var demoSector = _configuration.GetValue<string?>("CatalogService:DemoSector");
            B2X.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
            var (items, total) = B2X.Catalog.Endpoints.Dev.DemoProductStore.GetPage(1, demoCount.Value);
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

    public Task<B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var demoCount = _configuration.GetValue<int?>("CatalogService:DemoProductCount");
        if (demoCount > 0)
        {
            var demoSector = _configuration.GetValue<string?>("CatalogService:DemoSector");
            B2X.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
            var (items, total) = B2X.Catalog.Endpoints.Dev.DemoProductStore.GetPage(pageNumber, pageSize);
            var matches = items.Where(p => ((string)p.name).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || ((string)p.sku).Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            return Task.FromResult(new B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>
            {
                Items = matches.Select(p => new B2X.Catalog.Models.ProductDto
                {
                    Id = Guid.TryParse(p.id?.ToString(), out Guid id) ? id : Guid.NewGuid(),
                    TenantId = tenantId,
                    Sku = p.sku?.ToString() ?? "",
                    Name = p.name?.ToString() ?? "",
                    Price = Convert.ToDecimal(p.price ?? 0),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = matches.Count
            });
        }

        var products = new List<B2X.Catalog.Models.ProductDto>
        {
            new B2X.Catalog.Models.ProductDto
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Sku = "DEMO-001",
                Name = "Demo Product 1",
                Price = 9.99m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        return Task.FromResult(new B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>
        {
            Items = products,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = products.Count
        });
    }
}

public class DevSearchIndexService : ISearchIndexService
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public DevSearchIndexService(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var demoCount = _configuration.GetValue<int?>("CatalogService:DemoProductCount");
        if (demoCount > 0)
        {
            var demoSector = _configuration.GetValue<string?>("CatalogService:DemoSector");
            B2X.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
            var (items, total) = B2X.Catalog.Endpoints.Dev.DemoProductStore.GetPage(pageNumber, pageSize);
            var matches = items.Where(p => ((string)p.name).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || ((string)p.sku).Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            return Task.FromResult(new B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>
            {
                Items = matches.Select(p => new B2X.Catalog.Models.ProductDto
                {
                    Id = Guid.TryParse(p.id?.ToString(), out Guid id) ? id : Guid.NewGuid(),
                    TenantId = tenantId,
                    Sku = p.sku?.ToString() ?? "",
                    Name = p.name?.ToString() ?? "",
                    Price = Convert.ToDecimal(p.price ?? 0),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = matches.Count
            });
        }

        var products = new List<B2X.Catalog.Models.ProductDto>
        {
            new B2X.Catalog.Models.ProductDto
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Sku = "DEMO-001",
                Name = "Demo Product 1",
                Price = 9.99m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        return Task.FromResult(new B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>
        {
            Items = products,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = products.Count
        });
    }
}
