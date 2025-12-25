using System;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using B2Connect.SearchService.Models;
using System.Text.Json;
using System.Diagnostics;

namespace B2Connect.SearchService.Controllers
{
    /// <summary>
    /// API endpoints for product search and catalog browsing
    /// Used by StoreFront to search and filter products from Elasticsearch
    /// </summary>
    [ApiController]
    [Route("api/catalog/products")]
    public class ProductSearchController : ControllerBase
    {
        private readonly IElasticsearchClient _elasticsearchClient;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ProductSearchController> _logger;
        private const string IndexAlias = "products-alias";
        private const string CacheKeyPrefix = "product-search:";
        private const int CacheDurationSeconds = 300; // 5 minutes

        public ProductSearchController(
            IElasticsearchClient elasticsearchClient,
            IDistributedCache cache,
            ILogger<ProductSearchController> logger)
        {
            _elasticsearchClient = elasticsearchClient;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Search products with full text search and filtering
        /// GET: /api/catalog/products/search
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<ProductSearchResponseDto>> SearchAsync(
            [FromBody] ProductSearchQueryRequest request)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                // Validate request
                if (request == null || string.IsNullOrWhiteSpace(request.Query))
                {
                    return BadRequest("Search query is required");
                }

                // Generate cache key
                var cacheKey = GenerateCacheKey(request);

                // Try to get from cache
                var cachedResult = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedResult))
                {
                    _logger.LogInformation($"Cache hit for search query: {request.Query}");
                    var cached = JsonSerializer.Deserialize<ProductSearchResponseDto>(cachedResult);
                    if (cached != null)
                    {
                        cached.ElapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;
                        return Ok(cached);
                    }
                }

                // Execute search in Elasticsearch
                var response = await ExecuteSearchAsync(request);

                // Cache the result
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheDurationSeconds)
                };
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(response),
                    cacheOptions);

                stopwatch.Stop();
                response.ElapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;

                _logger.LogInformation($"Search executed in {stopwatch.ElapsedMilliseconds}ms for: {request.Query}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing search");
                return StatusCode(500, new { error = "Search failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Get search suggestions (autocomplete)
        /// GET: /api/catalog/products/suggestions
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<ActionResult<SearchSuggestionDto[]>> GetSuggestionsAsync(
            [FromQuery] string query,
            [FromQuery] Guid? tenantId = null,
            [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                {
                    return BadRequest("Query must be at least 2 characters");
                }

                // Check cache
                var cacheKey = $"{CacheKeyPrefix}suggestions:{query}:{tenantId}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                {
                    return Ok(JsonSerializer.Deserialize<SearchSuggestionDto[]>(cached));
                }

                // Execute suggestion query
                var searchRequest = new SearchRequest(IndexAlias);

                var filters = new List<Query>();
                if (tenantId.HasValue)
                {
                    filters.Add(new TermQuery { Field = "tenantId", Value = tenantId });
                }

                var boolQuery = new BoolQuery();

                // Multi-match query for suggestions
                boolQuery.Must = new List<Query>
                {
                    new MultiMatchQuery
                    {
                        Query = query,
                        Fields = new[] { "name.autocomplete^2", "category", "brand" },
                        Fuzziness = new Fuzziness("AUTO"),
                        Operator = Operator.Or
                    }
                };

                if (filters.Count > 0)
                {
                    boolQuery.Filter = filters;
                }

                searchRequest.Query = new Query(boolQuery);
                searchRequest.Size = limit;
                searchRequest.Source = new SourceConfig { Includes = new[] { "productId", "name", "category", "brand" } };

                var result = await _elasticsearchClient.SearchAsync<ProductIndexDocument>(searchRequest);

                var suggestions = result.Documents
                    .GroupBy(d => d.Name)
                    .Select(g => new SearchSuggestionDto
                    {
                        Suggestion = g.Key,
                        Type = "product_name",
                        PopularityCount = g.Count()
                    })
                    .OrderByDescending(s => s.PopularityCount)
                    .Take(limit)
                    .ToArray();

                // Cache suggestions
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheDurationSeconds)
                };
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(suggestions),
                    cacheOptions);

                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting suggestions");
                return StatusCode(500, new { error = "Failed to get suggestions", details = ex.Message });
            }
        }

        /// <summary>
        /// Get product details by ID
        /// GET: /api/catalog/products/{id}
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductSearchResultItemDto>> GetProductAsync(Guid id)
        {
            try
            {
                var cacheKey = $"{CacheKeyPrefix}product:{id}";
                var cached = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                {
                    return Ok(JsonSerializer.Deserialize<ProductSearchResultItemDto>(cached));
                }

                var result = await _elasticsearchClient.GetAsync<ProductIndexDocument>(IndexAlias, id.ToString());

                if (!result.Found)
                {
                    return NotFound();
                }

                var item = MapToSearchResultItem(result.Source, 1.0);

                // Cache product details
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheDurationSeconds)
                };
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(item),
                    cacheOptions);

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product");
                return StatusCode(500, new { error = "Failed to get product", details = ex.Message });
            }
        }

        /// <summary>
        /// Execute the actual Elasticsearch search
        /// </summary>
        private async Task<ProductSearchResponseDto> ExecuteSearchAsync(ProductSearchQueryRequest request)
        {
            var response = new ProductSearchResponseDto
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            // Build query
            var boolQuery = new BoolQuery();

            // Main text search
            boolQuery.Must = new List<Query>
            {
                new MultiMatchQuery
                {
                    Query = request.Query,
                    Fields = new[] { "name^2", "description", "tags", "brand" },
                    Fuzziness = new Fuzziness("AUTO"),
                    Operator = Operator.Or
                }
            };

            // Apply filters
            var filters = new List<Query> { new TermQuery { Field = "isAvailable", Value = true } };

            if (!string.IsNullOrEmpty(request.Category))
            {
                filters.Add(new TermQuery { Field = "category.keyword", Value = request.Category });
            }

            if (request.MinPrice.HasValue)
            {
                filters.Add(new RangeQuery { Field = "price", Gte = request.MinPrice.Value });
            }

            if (request.MaxPrice.HasValue)
            {
                filters.Add(new RangeQuery { Field = "price", Lte = request.MaxPrice.Value });
            }

            if (request.Tags?.Length > 0)
            {
                filters.Add(new TermsQuery { Field = "tags", Terms = request.Tags.Cast<object>().ToList() });
            }

            if (!string.IsNullOrEmpty(request.Brand))
            {
                filters.Add(new TermQuery { Field = "brand.keyword", Value = request.Brand });
            }

            if (request.Colors?.Length > 0)
            {
                filters.Add(new TermsQuery { Field = "colors", Terms = request.Colors.Cast<object>().ToList() });
            }

            if (request.Sizes?.Length > 0)
            {
                filters.Add(new TermsQuery { Field = "sizes", Terms = request.Sizes.Cast<object>().ToList() });
            }

            if (request.TenantId != Guid.Empty)
            {
                filters.Add(new TermQuery { Field = "tenantId", Value = request.TenantId });
            }

            boolQuery.Filter = filters;

            // Build search request
            var searchRequest = new SearchRequest(IndexAlias)
            {
                Query = new Query(boolQuery),
                Size = request.PageSize,
                From = request.GetOffset()
            };

            // Add sorting
            switch (request.SortBy?.ToLower())
            {
                case "price_asc":
                    searchRequest.Sort = new List<ISort> { new FieldSort { Field = "price", Order = SortOrder.Asc } };
                    break;
                case "price_desc":
                    searchRequest.Sort = new List<ISort> { new FieldSort { Field = "price", Order = SortOrder.Desc } };
                    break;
                case "newest":
                    searchRequest.Sort = new List<ISort> { new FieldSort { Field = "createdAt", Order = SortOrder.Desc } };
                    break;
                case "popular":
                    searchRequest.Sort = new List<ISort> { new FieldSort { Field = "popularityScore", Order = SortOrder.Desc } };
                    break;
                default: // relevance
                    // Default Elasticsearch relevance scoring
                    break;
            }

            // Add aggregations for facets
            if (request.IncludeFacets)
            {
                // Add facet aggregations (categories, brands, colors, etc.)
                // This would require additional configuration in the SearchRequest
            }

            var result = await _elasticsearchClient.SearchAsync<ProductIndexDocument>(searchRequest);

            response.TotalCount = (int)(result.Total?.Value ?? 0);
            response.Results = result.Documents
                .Select((doc, idx) => MapToSearchResultItem(doc, 1.0))
                .ToList();

            return response;
        }

        /// <summary>
        /// Map Elasticsearch document to search result item
        /// </summary>
        private ProductSearchResultItemDto MapToSearchResultItem(
            ProductIndexDocument doc,
            double relevance)
        {
            return new ProductSearchResultItemDto
            {
                ProductId = doc.ProductId,
                Sku = doc.Sku,
                Name = doc.Name,
                Description = doc.Description,
                Category = doc.Category,
                Price = doc.Price,
                B2bPrice = doc.B2bPrice,
                StockQuantity = doc.StockQuantity,
                IsAvailable = doc.IsAvailable,
                Tags = doc.Tags,
                ImageUrls = doc.ImageUrls,
                Relevance = relevance,
                ReviewCount = doc.ReviewCount,
                AverageRating = doc.AverageRating
            };
        }

        /// <summary>
        /// Generate cache key from search request
        /// </summary>
        private string GenerateCacheKey(ProductSearchQueryRequest request)
        {
            var key = $"{CacheKeyPrefix}search:{request.Query}:{request.PageNumber}:{request.PageSize}";

            if (!string.IsNullOrEmpty(request.Category))
                key += $":cat:{request.Category}";
            if (request.MinPrice.HasValue)
                key += $":min:{request.MinPrice}";
            if (request.MaxPrice.HasValue)
                key += $":max:{request.MaxPrice}";
            if (request.TenantId != Guid.Empty)
                key += $":tenant:{request.TenantId}";

            return key;
        }
    }
}
