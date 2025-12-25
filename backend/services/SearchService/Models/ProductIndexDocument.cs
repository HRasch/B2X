using System;
using System.Collections.Generic;
using Elasticsearch.Net;

namespace B2Connect.SearchService.Models
{
    /// <summary>
    /// Product document for Elasticsearch index (immutable record)
    /// Maps to "products" index with document type "product"
    /// </summary>
    public record ProductIndexDocument(
        Guid ProductId,
        string Sku,
        string Name,
        string Description,
        string Category,
        decimal Price,
        decimal? B2bPrice,
        int StockQuantity,
        string[] Tags,
        string Brand,
        string Material,
        string[] Colors,
        string[] Sizes,
        Dictionary<string, string> CustomAttributes,
        string[] ImageUrls,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        Guid TenantId,
        double PopularityScore,
        int ReviewCount,
        double AverageRating)
    {
        // Computed property
        public bool IsAvailable => StockQuantity > 0;
    }

    /// <summary>
    /// Search request DTO for searching products (immutable record)
    /// </summary>
    public record ProductSearchQueryRequest(
        string Query,
        string? Category = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string[]? Tags = null,
        string? Brand = null,
        string[]? Colors = null,
        string[]? Sizes = null,
        int PageSize = 20,
        int PageNumber = 1,
        string SortBy = "relevance",
        Guid? TenantId = null,
        bool IncludeFacets = true)
    {
        public int GetOffset() => (PageNumber - 1) * PageSize;
    }

    /// <summary>
    /// Search result DTO (immutable record)
    /// </summary>
    public record ProductSearchResponseDto(
        int TotalCount,
        int PageNumber,
        int PageSize,
        List<ProductSearchResultItemDto> Results,
        Dictionary<string, FacetResultDto> Facets,
        int ElapsedMilliseconds)
    {
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }

    /// <summary>
    /// Individual search result item (immutable record)
    /// </summary>
    public record ProductSearchResultItemDto(
        Guid ProductId,
        string Sku,
        string Name,
        string Description,
        string Category,
        decimal Price,
        decimal? B2bPrice,
        int StockQuantity,
        bool IsAvailable,
        string[] Tags,
        string[] ImageUrls,
        double Relevance,
        int ReviewCount,
        double AverageRating);

    /// <summary>
    /// Search suggestion (autocomplete) result (immutable record)
    /// </summary>
    public record SearchSuggestionDto(
        string Suggestion,
        int PopularityCount,
        string Type,
        Guid? CategoryId = null);

    /// <summary>
    /// Facet result for filtering options (immutable record)
    /// </summary>
    public record FacetResultDto(
        string Field,
        List<FacetOptionDto> Options);

    /// <summary>
    /// Individual facet option (immutable record)
    /// </summary>
    public record FacetOptionDto(string Value, int Count);

    /// <summary>
    /// Aggregation result for price ranges, etc. (immutable record)
    /// </summary>
    public record AggregationResultDto(
        string Field,
        Dictionary<string, object> Data);
}
