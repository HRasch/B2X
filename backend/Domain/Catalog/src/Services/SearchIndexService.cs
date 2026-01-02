using B2Connect.CatalogService.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// Service interface for Elasticsearch indexing
/// </summary>
public interface ISearchIndexService
{
    Task IndexProductAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

/// <summary>
/// Search index implementation using Elasticsearch 9.2.2
/// </summary>
public class SearchIndexService : ISearchIndexService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<SearchIndexService> _logger;
    private const string IndexPrefix = "products_";
    private static readonly string[] value = new[] { "Name^2", "Description", "Sku" };

    public SearchIndexService(ElasticsearchClient client, ILogger<SearchIndexService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task IndexProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {
            var indexName = GetIndexName(product.TenantId);

            var response = await _client.IndexAsync(
                product,
                i => i
                    .Index(indexName)
                    .Id(product.Id.ToString()),
                cancellationToken);

            if (!response.IsSuccess())
            {
                _logger.LogError("Failed to index product {ProductId}: {Error}", product.Id, response.DebugInformation);
            }
            else
            {
                _logger.LogDebug("Product {ProductId} indexed successfully", product.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing product {ProductId}", product.Id);
        }
    }

    public async Task DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Search all tenant indices for the product and delete
            var response = await _client.DeleteAsync<Product>(
                productId.ToString(),
                d => d.Index("products_*"),
                cancellationToken);

            if (response.IsSuccess())
            {
                _logger.LogDebug("Product {ProductId} deleted from index", productId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId} from index", productId);
        }
    }

    public async Task<PagedResult<ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var indexName = GetIndexName(tenantId);
            var from = (pageNumber - 1) * pageSize;

            var searchResponse = await _client.SearchAsync<Product>(s => s
                .Indices(indexName)
                .From(from)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(searchTerm)
                        .Fields(value)
                        .Fuzziness("AUTO")
                    )
                ),
                cancellationToken);

            if (!searchResponse.IsSuccess())
            {
                _logger.LogError("Search failed: {Error}", searchResponse.DebugInformation);
                return new PagedResult<ProductDto> { Items = new(), PageNumber = pageNumber, PageSize = pageSize, TotalCount = 0 };
            }

            var total = (int)searchResponse.Total;
            var items = searchResponse.Documents.Select(p => p.ToDto()).ToList();

            return new PagedResult<ProductDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            return new PagedResult<ProductDto> { Items = new(), PageNumber = pageNumber, PageSize = pageSize, TotalCount = 0 };
        }
    }

    private static string GetIndexName(Guid tenantId) => $"{IndexPrefix}{tenantId:N}";
}
