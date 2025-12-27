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

        return Results.Ok(product);
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
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        // TODO: Implement GetAllAsync method in IProductService
        return Results.Ok(new { products = new object[0], total = 0, page, pageSize });
    }
}
