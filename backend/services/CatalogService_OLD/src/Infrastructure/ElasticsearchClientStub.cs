namespace B2Connect.CatalogService.Infrastructure;

using Elastic.Clients.Elasticsearch;

/// <summary>
/// Elasticsearch client wrapper interface
/// Provides simplified access to Elasticsearch functionality
/// </summary>
public interface IElasticsearchClient
{
    /// <summary>
    /// Search for documents matching a query
    /// </summary>
    Task<SearchResponse<T>> SearchAsync<T>(
        Func<SearchRequestDescriptor<T>, SearchRequestDescriptor<T>> selector,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Index a single document
    /// </summary>
    Task<IndexResponse> IndexAsync<T>(
        T document,
        Func<IndexRequestDescriptor<T>, IndexRequestDescriptor<T>>? selector = null,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Bulk index documents
    /// </summary>
    Task<BulkResponse> BulkAsync(
        Func<BulkRequestDescriptor, BulkRequestDescriptor> selector,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete documents matching a query
    /// </summary>
    Task<DeleteByQueryResponse> DeleteByQueryAsync<T>(
        Action<DeleteByQueryRequestDescriptor<T>>? selector,
        CancellationToken cancellationToken = default)
        where T : class;
}

/// <summary>
/// Elasticsearch client wrapper implementation
/// Wraps Elastic.Clients.Elasticsearch.ElasticsearchClient
/// </summary>
public class ElasticsearchClientWrapper : IElasticsearchClient
{
    private readonly ElasticsearchClient _client;

    public ElasticsearchClientWrapper(ElasticsearchClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<SearchResponse<T>> SearchAsync<T>(
        Func<SearchRequestDescriptor<T>, SearchRequestDescriptor<T>> selector,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await _client.SearchAsync(selector, cancellationToken);
    }

    public async Task<IndexResponse> IndexAsync<T>(
        T document,
        Func<IndexRequestDescriptor<T>, IndexRequestDescriptor<T>>? selector = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (selector == null)
        {
            return await _client.IndexAsync(document, cancellationToken: cancellationToken);
        }
        return await _client.IndexAsync(document, selector, cancellationToken);
    }

    public async Task<BulkResponse> BulkAsync(
        Func<BulkRequestDescriptor, BulkRequestDescriptor> selector,
        CancellationToken cancellationToken = default)
    {
        return await _client.BulkAsync(selector, cancellationToken);
    }

    public async Task<DeleteByQueryResponse> DeleteByQueryAsync<T>(
        Action<DeleteByQueryRequestDescriptor<T>>? selector,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await _client.DeleteByQueryAsync(selector, cancellationToken);
    }
}
