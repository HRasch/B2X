using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.CQRS.Queries;

/// <summary>
/// Get a single product by ID
/// Synchronous query with caching support
/// Hits denormalized read model for < 1ms response
/// </summary>
public class GetProductByIdQuery : IQuery<ProductDto>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
}

/// <summary>
/// Get paginated list of products
/// Supports filtering by category, price range, and search term
/// Uses indexed read model for fast filtering on millions of products
/// </summary>
public class GetProductsPagedQuery : IQuery<PagedResult<ProductDto>>
{
    public Guid TenantId { get; set; }
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? AvailableOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "name"; // name, price, price_desc, created, updated
}

/// <summary>
/// Search products with full-text search
/// Uses read model search_text index for SQL search
/// Can escalate to ElasticSearch for advanced features (fuzzy, facets)
/// </summary>
public class SearchProductsQuery : IQuery<PagedResult<ProductDto>>
{
    public Guid TenantId { get; set; }
    public required string SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Get catalog statistics
/// Returns aggregated counts: total products, categories, average price, etc.
/// Cached for 30 minutes in production
/// </summary>
public class GetCatalogStatsQuery : IQuery<CatalogStats>
{
    public Guid TenantId { get; set; }
}

/// <summary>
/// Aggregated catalog statistics
/// Used as response DTO for GetCatalogStatsQuery
/// </summary>
public class CatalogStats
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalCategories { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public DateTime LastUpdated { get; set; }
}
