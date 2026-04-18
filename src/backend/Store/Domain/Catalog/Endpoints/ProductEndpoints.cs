using B2X.Catalog.Services;
using B2X.Types.DTOs;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace B2X.Catalog.Endpoints;

// Service interfaces - use fully qualified types to avoid ambiguity with local ProductDto record
public interface IProductService
{
    Task<dynamic?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
    Task<B2X.Types.DTOs.PagedResult<B2X.Catalog.Models.ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
}

public interface ISearchIndexService
{
    Task<B2X.Types.DTOs.PagedResult<B2X.Catalog.Models.ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
}

/// <summary>
/// Wolverine HTTP Endpoints for Product queries
/// These replace traditional Controllers and use Wolverine's mediator pattern
/// </summary>
public static class ProductEndpoints
{
    /// <summary>
    /// GET /api/products/{sku}
    /// Gets a product by SKU with tenant isolation
    /// </summary>
    [WolverineGet("/api/products/sku/{sku}")]
    public static async Task<IResult> GetProductBySku(
        string sku,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IProductService productService,
        B2X.Search.ITenantResolver tenantResolver,
        HttpRequest request,
        CancellationToken ct)
    {
        var resolvedTenant = tenantId;
        if (resolvedTenant == Guid.Empty)
        {
            var host = request.Host.Host;
            var tid = tenantResolver.ResolveTenantIdFromHost(host);
            if (!string.IsNullOrWhiteSpace(tid) && Guid.TryParse(tid, out var g))
            {
                resolvedTenant = g;
            }
        }

        var product = await productService.GetBySkuAsync(resolvedTenant, sku, ct).ConfigureAwait(false);

        if (product == null)
        {
            return Results.NotFound(new { Message = $"Product with SKU '{sku}' not found" });
        }

        // Map dynamic / anonymous product to ProductDto when necessary
        if (product is B2X.Catalog.Models.ProductDto dto)
        {
            return Results.Ok(dto);
        }

        try
        {
            var mapped = new B2X.Catalog.Models.ProductDto
            {
                Id = Guid.TryParse(Convert.ToString(product.id ?? product.Id), out Guid idVal) ? idVal : Guid.NewGuid(),
                TenantId = Guid.TryParse(Convert.ToString(product.tenantId ?? product.TenantId), out Guid tVal) ? tVal : tenantId,
                Sku = Convert.ToString(product.sku ?? product.Sku) ?? sku,
                Name = Convert.ToString(product.name ?? product.Name) ?? string.Empty,
                Description = Convert.ToString(product.description ?? product.Description),
                Price = Convert.ToDecimal(product.price ?? product.Price ?? 0m),
                DiscountPrice = product.discountPrice != null ? (decimal?)Convert.ToDecimal(product.discountPrice) : null,
                StockQuantity = product.stockQuantity != null ? Convert.ToInt32(product.stockQuantity) : 0,
                IsActive = (bool?)product.isActive ?? true,
                CategoryIds = new List<Guid>(),
                BrandName = Convert.ToString(product.brandName ?? product.BrandName),
                Tags = new List<string>(),
                CreatedAt = product.createdAt != null ? Convert.ToDateTime(product.createdAt) : DateTime.UtcNow,
                UpdatedAt = product.updatedAt != null ? Convert.ToDateTime(product.updatedAt) : DateTime.UtcNow,
                IsAvailable = (bool?)product.isAvailable ?? true
            };

            return Results.Ok(mapped);
        }
        catch
        {
            return Results.Ok(product);
        }
    }

    /// <summary>
    /// GET /api/products/search?q={query}
    /// Searches products using Elasticsearch
    /// </summary>
    [WolverineGet("/api/products/search")]
    public static async Task<IResult> SearchProducts(
        [FromQuery] string q,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        ISearchIndexService searchService,
        B2X.Search.ITenantResolver tenantResolver,
        HttpRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.BadRequest(new { Message = "Search query cannot be empty" });
        }

        var resolvedTenant = tenantId;
        if (resolvedTenant == Guid.Empty)
        {
            var host = request.Host.Host;
            var tid = tenantResolver.ResolveTenantIdFromHost(host);
            if (!string.IsNullOrWhiteSpace(tid) && Guid.TryParse(tid, out var g))
            {
                resolvedTenant = g;
            }
        }

        var results = await searchService.SearchAsync(resolvedTenant, q, 1, 20, ct).ConfigureAwait(false);
        return Results.Ok(results);
    }

    /// <summary>
    /// GET /api/products
    /// Lists all products for a tenant with pagination
    /// </summary>
    [WolverineGet("/api/products")]
    public static async Task<IResult> ListProducts(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IProductService productService,
        B2X.Search.ITenantResolver tenantResolver,
        HttpRequest request,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1 || pageSize > 100)
        {
            pageSize = 20;
        }

        // If configured for demo product count, use in-memory demo store
        var demoCount = configuration.GetValue<int?>("CatalogService:DemoProductCount");
        var demoSector = configuration.GetValue<string?>("CatalogService:DemoSector");
        if (demoCount > 0)
        {
            // Lazy initialize demo store with configured count
            B2X.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
            var (items, total) = B2X.Catalog.Endpoints.Dev.DemoProductStore.GetPage(page, pageSize);
            // resolve tenant if header missing
            var resolvedTenant = tenantId;
            if (resolvedTenant == Guid.Empty)
            {
                var host = request.Host.Host;
                var tid = tenantResolver.ResolveTenantIdFromHost(host);
                if (!string.IsNullOrWhiteSpace(tid) && Guid.TryParse(tid, out var g))
                {
                    resolvedTenant = g;
                }
            }

            // filter by resolvedTenant if DemoProductStore items include tenantId
            var filtered = items.Where(p => Convert.ToString(p.tenantId) == resolvedTenant.ToString());
            return Results.Ok(new { products = filtered, total, page, pageSize });
        }

        // TODO: Implement GetAllAsync method in IProductService
        return Results.Ok(new { products = Array.Empty<object>(), total = 0, page, pageSize });
    }
}
