using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace B2Connect.Catalog.Endpoints;

// Service interfaces
public interface IProductService
{
    Task<dynamic?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
}

public interface ISearchIndexService
{
    Task<dynamic> SearchAsync(Guid tenantId, string query, CancellationToken ct = default);
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
        CancellationToken ct)
    {
        var product = await productService.GetBySkuAsync(tenantId, sku, ct);

        if (product == null)
        {
            return Results.NotFound(new { Message = $"Product with SKU '{sku}' not found" });
        }

        // Map dynamic / anonymous product to ProductDto when necessary
        if (product is B2Connect.CatalogService.Models.ProductDto dto)
        {
            return Results.Ok(dto);
        }

        try
        {
            var mapped = new B2Connect.CatalogService.Models.ProductDto
            {
                Id = Guid.TryParse(Convert.ToString(product.id ?? product.Id), out Guid idVal) ? idVal : Guid.NewGuid(),
                TenantId = Guid.TryParse(Convert.ToString(product.tenantId ?? product.TenantId), out Guid tVal) ? tVal : tenantId,
                Sku = Convert.ToString(product.sku ?? product.Sku) ?? sku,
                Name = Convert.ToString(product.name ?? product.Name) ?? string.Empty,
                Description = Convert.ToString(product.description ?? product.Description),
                Price = Convert.ToDecimal(product.price ?? product.Price ?? 0m),
                DiscountPrice = product.discountPrice != null ? (decimal?)Convert.ToDecimal(product.discountPrice) : null,
                StockQuantity = product.stockQuantity != null ? Convert.ToInt32(product.stockQuantity) : 0,
                IsActive = product.isActive != null ? Convert.ToBoolean(product.isActive) : true,
                Categories = new List<string>(),
                BrandName = Convert.ToString(product.brandName ?? product.BrandName),
                Tags = new List<string>(),
                CreatedAt = product.createdAt != null ? Convert.ToDateTime(product.createdAt) : DateTime.UtcNow,
                UpdatedAt = product.updatedAt != null ? Convert.ToDateTime(product.updatedAt) : DateTime.UtcNow,
                IsAvailable = product.isAvailable != null ? Convert.ToBoolean(product.isAvailable) : true
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
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.BadRequest(new { Message = "Search query cannot be empty" });
        }

        var results = await searchService.SearchAsync(tenantId, q, ct);
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
        CancellationToken ct,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        // If configured for demo product count, use in-memory demo store
        var demoCount = configuration.GetValue<int?>("CatalogService:DemoProductCount");
        var demoSector = configuration.GetValue<string?>("CatalogService:DemoSector");
        if (demoCount.HasValue && demoCount.Value > 0)
        {
            // Lazy initialize demo store with configured count
            B2Connect.Catalog.Endpoints.Dev.DemoProductStore.EnsureInitialized(demoCount.Value, demoSector);
            var (items, total) = B2Connect.Catalog.Endpoints.Dev.DemoProductStore.GetPage(page, pageSize);
            return Results.Ok(new { products = items, total, page, pageSize });
        }

        // TODO: Implement GetAllAsync method in IProductService
        return Results.Ok(new { products = new object[0], total = 0, page, pageSize });
    }
}
