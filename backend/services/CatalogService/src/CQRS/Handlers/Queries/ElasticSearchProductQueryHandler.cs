using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

using Microsoft.Extensions.Logging;

using WolverineFx;

using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
namespace B2Connect.CatalogService.CQRS.Handlers.Queries;

/// <summary>
/// ElasticSearch-based Product Query Handler
/// Provides advanced full-text search capabilities for the Store Frontend
/// Scales to millions of products efficiently
/// Features: fuzzy matching, facets, aggregations, relevance scoring
/// </summary>
public class ElasticSearchProductQueryHandler : IQueryHandler<SearchProductsElasticQuery, PagedResult<ProductDto>>
{
    private readonly IElasticsearchClient _elasticsearchClient;
    private readonly ILogger<ElasticSearchProductQueryHandler> _logger;
    private const string IndexPrefix = "products_";

    public ElasticSearchProductQueryHandler(
        IElasticsearchClient elasticsearchClient,
        ILogger<ElasticSearchProductQueryHandler> logger)
    {
        _elasticsearchClient = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedResult<ProductDto>> Handle(
        SearchProductsElasticQuery query,
        CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        if (string.IsNullOrWhiteSpace(query.SearchTerm))
            throw new ArgumentException("Search term is required", nameof(query));

        try
        {
            _logger.LogInformation(
                "Searching ElasticSearch for term '{SearchTerm}' (Tenant: {TenantId}, Page: {Page})",
                query.SearchTerm, query.TenantId, query.Page);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Determine index name based on language
            var indexName = GetIndexNameForLanguage(query.Language ?? "de");

            // Build ElasticSearch query
            var searchResponse = await _elasticsearchClient.SearchAsync<ProductElasticModel>(s => s
                .Index(indexName)
                .From((query.Page - 1) * query.PageSize)
                .Size(query.PageSize)
                .Query(q => BuildSearchQuery(query))
                .Sort(sort => sort
                    .Score(sd => sd.Order(SortOrder.Desc))
                    .Field(f => f.Field("_doc").Order(SortOrder.Asc)))
                .Highlight(h => h
                    .Fields(hf => hf
                        .Field("Name")
                        .Field("Description")))
                .TrackTotalHits(true),
                cancellationToken);

            sw.Stop();
            _logger.LogDebug("ElasticSearch query executed in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);

            if (!searchResponse.IsSuccess())
            {
                _logger.LogError("ElasticSearch query failed: {Error}", searchResponse.DebugInformation);
                throw new InvalidOperationException($"ElasticSearch query failed: {searchResponse.DebugInformation}");
            }

            // Map ElasticSearch results to ProductDto
            var products = new List<ProductDto>();
            foreach (var hit in searchResponse.Hits)
            {
                var product = MapToProductDto(hit.Source);
                product.RelevanceScore = (float?)hit.Score ?? 0;
                products.Add(product);
            }

            var totalCount = (int)(searchResponse.Total?.Value ?? 0);

            return new PagedResult<ProductDto>
            {
                Items = products,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (totalCount + query.PageSize - 1) / query.PageSize,
                HasNextPage = query.Page < ((totalCount + query.PageSize - 1) / query.PageSize),
                SearchMetadata = new SearchMetadata
                {
                    QueryExecutionTimeMs = sw.ElapsedMilliseconds,
                    HitCount = searchResponse.Hits.Count,
                    Source = "ElasticSearch"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products in ElasticSearch for term '{SearchTerm}'", query.SearchTerm);
            throw;
        }
    }

    /// <summary>
    /// Build the ElasticSearch query with multi-field search, filters, and scoring
    /// </summary>
    private Query BuildSearchQuery(SearchProductsElasticQuery query)
    {
        var tenantIdTerm = new TermQuery { Field = "TenantId", Value = query.TenantId };

        // Base multi-field search with fuzzy matching
        var multiFieldSearch = new MultiMatchQuery
        {
            Query = query.SearchTerm,
            Fields = new Field[] { "Name^3", "Description^2", "Category", "Sku", "Brand" },
            Fuzziness = new Fuzziness("AUTO"),
            Operator = Operator.Or,
            MinimumShouldMatch = 1
        };

        var searchQueries = new List<Query> { multiFieldSearch };

        // Add price range filter if specified
        if (query.MinPrice.HasValue || query.MaxPrice.HasValue)
        {
            var priceFilter = new RangeQuery
            {
                Field = "Price"
            };

            if (query.MinPrice.HasValue)
                priceFilter.Gte = query.MinPrice.Value;

            if (query.MaxPrice.HasValue)
                priceFilter.Lte = query.MaxPrice.Value;

            searchQueries.Add(priceFilter);
        }

        // Add category filter if specified
        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            searchQueries.Add(new TermQuery
            {
                Field = "Category.keyword",
                Value = query.Category
            });
        }

        // Add availability filter
        if (query.OnlyAvailable)
        {
            searchQueries.Add(new TermQuery
            {
                Field = "IsAvailable",
                Value = true
            });
        }

        // Combine all queries
        return new BoolQuery
        {
            Must = new Query[] { multiFieldSearch, tenantIdTerm },
            Filter = searchQueries.Skip(1).ToList()
        };
    }

    /// <summary>
    /// Map ElasticSearch document to ProductDto
    /// </summary>
    private ProductDto MapToProductDto(ProductElasticModel source)
    {
        if (source == null)
            return null;

        return new ProductDto
        {
            Id = source.ProductId,
            Sku = source.Sku,
            Name = source.Name,
            Description = source.Description,
            Price = source.Price,
            B2bPrice = source.B2bPrice,
            Category = source.Category,
            ImageUrl = source.ImageUrls?.FirstOrDefault(),
            StockQuantity = source.StockQuantity,
            IsAvailable = source.IsAvailable,
            Brand = source.Brand,
            Material = source.Material,
            Tags = string.Join(", ", source.Tags ?? Array.Empty<string>()),
            AverageRating = source.AverageRating,
            ReviewCount = source.ReviewCount,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    /// <summary>
    /// Get ElasticSearch index name for a specific language
    /// </summary>
    private string GetIndexNameForLanguage(string language)
    {
        return language?.ToLower() switch
        {
            "de" => "products_de",
            "en" => "products_en",
            "fr" => "products_fr",
            _ => "products_de"
        };
    }
}

/// <summary>
/// ElasticSearch model for products (matches index mapping)
/// </summary>
public class ProductElasticModel
{
    public Guid ProductId { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal B2bPrice { get; set; }
    public string Category { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public string[] Tags { get; set; }
    public string Brand { get; set; }
    public string Material { get; set; }
    public string[] Colors { get; set; }
    public string[] Sizes { get; set; }
    public string[] ImageUrls { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public double PopularityScore { get; set; }
    public int ReviewCount { get; set; }
    public float AverageRating { get; set; }
}

/// <summary>
/// Metadata about the search execution
/// </summary>
public class SearchMetadata
{
    public long QueryExecutionTimeMs { get; set; }
    public int HitCount { get; set; }
    public string Source { get; set; }
    public Dictionary<string, int> Facets { get; set; }
}
