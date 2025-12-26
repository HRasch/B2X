using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Wolverine;

using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Infrastructure;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

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
                        .Add("Sku")
                        .Add("ShortDescription")))
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
                products.Add(product);
            }

            var totalCount = (int)(searchResponse.Total?.Value ?? 0L);

            return new PagedResult<ProductDto>
            {
                Items = products,
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
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
    /// Uses Elasticsearch.QueryDsl API for 9.2.2 compatibility
    /// </summary>
    private Query BuildSearchQuery(SearchProductsElasticQuery query)
    {
        var mustQueries = new List<Query>();

        // Base multi-field search with fuzzy matching
        mustQueries.Add(new MultiMatchQuery
        {
            Query = query.SearchTerm,
            Fields = new[] { "Sku^3", "ShortDescription^2", "Name" },
            Fuzziness = new Fuzziness { Value = "AUTO" },
            Operator = Operator.Or
        });

        // Add tenant filter
        mustQueries.Add(new TermQuery
        {
            Field = "TenantId",
            Value = (FieldValue)query.TenantId.ToString()
        });

        // Add price range filter if specified
        if (query.MinPrice.HasValue || query.MaxPrice.HasValue)
        {
            var rangeDescriptor = new RangeQuery { Field = "Price" };

            if (query.MinPrice.HasValue)
                rangeDescriptor.Gte = (FieldValue)query.MinPrice.Value;

            if (query.MaxPrice.HasValue)
                rangeDescriptor.Lte = (FieldValue)query.MaxPrice.Value;

            mustQueries.Add(rangeDescriptor);
        }

        // Add category filter if specified
        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            mustQueries.Add(new TermQuery
            {
                Field = "Categories.keyword",
                Value = (FieldValue)query.Category
            });
        }

        // Add availability filter
        if (query.OnlyAvailable)
        {
            mustQueries.Add(new TermQuery
            {
                Field = "IsActive",
                Value = (FieldValue)true
            });
        }

        // Combine all queries with Bool query
        if (mustQueries.Count == 1)
            return mustQueries[0];

        return new BoolQuery { Must = mustQueries.ToArray() };
    }

    /// <summary>
    /// Map ElasticSearch document to ProductDto
    /// </summary>
    private ProductDto MapToProductDto(ProductElasticModel? source)
    {
        if (source == null)
            return new ProductDto();

        return new ProductDto
        {
            Id = source.ProductId,
            Sku = source.Sku ?? string.Empty,
            Slug = source.Slug ?? string.Empty,
            Name = source.Name ?? new Dictionary<string, string>(),
            ShortDescription = source.ShortDescription,
            Description = source.Description,
            Price = source.Price,
            SpecialPrice = source.SpecialPrice,
            StockQuantity = source.StockQuantity,
            IsActive = source.IsActive,
            IsFeatured = source.IsFeatured,
            IsNew = source.IsNew,
            BrandId = source.BrandId,
            BrandName = source.BrandName,
            VariantCount = source.VariantCount,
            ImageCount = source.ImageCount,
            Categories = source.Categories ?? new List<CategoryDto>(),
            Variants = source.Variants ?? new List<ProductVariantDto>()
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
    public string? Sku { get; set; }
    public string? Slug { get; set; }
    public Dictionary<string, string>? Name { get; set; }
    public Dictionary<string, string>? ShortDescription { get; set; }
    public Dictionary<string, string>? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? SpecialPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int VariantCount { get; set; }
    public int ImageCount { get; set; }
    public List<CategoryDto>? Categories { get; set; }
    public List<ProductVariantDto>? Variants { get; set; }
}
