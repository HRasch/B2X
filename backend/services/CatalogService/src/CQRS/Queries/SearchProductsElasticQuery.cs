using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.CQRS.Queries;

/// <summary>
/// Query for ElasticSearch-based product search
/// Used by Store Frontend for advanced, scalable product discovery
/// Features: full-text search, filtering, pagination, relevance scoring
/// </summary>
public class SearchProductsElasticQuery : IQuery<PagedResult<ProductDto>>
{
    /// <summary>
    /// Tenant identifier for multi-tenant isolation
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Search term (will use fuzzy matching for typo tolerance)
    /// Searches: Name, Description, Category, SKU, Brand
    /// </summary>
    public string SearchTerm { get; set; }

    /// <summary>
    /// Language for index selection (de, en, fr)
    /// </summary>
    public string Language { get; set; } = "de";

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Items per page (1-100)
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Optional category filter
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Minimum price filter
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Maximum price filter
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Only return available products
    /// </summary>
    public bool OnlyAvailable { get; set; } = true;

    /// <summary>
    /// Sort field (relevance, price, popularity, rating)
    /// </summary>
    public string SortBy { get; set; } = "relevance";
}

/// <summary>
/// Extended PagedResult with search metadata
/// </summary>
public class SearchMetadata
{
    public long QueryExecutionTimeMs { get; set; }
    public int HitCount { get; set; }
    public string Source { get; set; }
    public Dictionary<string, int> Facets { get; set; }
}
