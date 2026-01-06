using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;
using Microsoft.Extensions.Logging;

namespace B2Connect.Domain.ERP.Services;

/// <summary>
/// HTTP client for communicating with Enventa ERP Connector (.NET Framework 4.8).
/// Handles streaming, pagination, delta sync, and batch operations over HTTP.
/// </summary>
public class ErpHttpClient : IErpClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ErpHttpClient> _logger;
    private readonly ErpHttpClientOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public ErpHttpClient(
        HttpClient httpClient,
        ILogger<ErpHttpClient> logger,
        ErpHttpClientOptions? options = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? new ErpHttpClientOptions();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }

    #region Health Check

    /// <summary>
    /// Check connector health.
    /// </summary>
    public async Task<ConnectorHealthResponse> GetHealthAsync(CancellationToken ct = default)
    {
        using var response = await _httpClient.GetAsync(ErpApiEndpoints.Health, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ConnectorHealthResponse>(_jsonOptions, ct)
            ?? throw new InvalidOperationException("Failed to deserialize health response");
    }

    #endregion

    #region Articles - Cursor Pagination

    /// <summary>
    /// Get articles with cursor-based pagination.
    /// </summary>
    public async Task<CursorPageResponse<ArticleDto>> GetArticlesAsync(
        string tenantId,
        CursorPageRequest request,
        CancellationToken ct = default)
    {
        var url = BuildUrl(ErpApiEndpoints.Articles, request.Cursor, request.PageSize);
        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);

        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CursorPageResponse<ArticleDto>>(_jsonOptions, ct)
            ?? new CursorPageResponse<ArticleDto>();
    }

    #endregion

    #region Articles - Delta Sync

    /// <summary>
    /// Delta sync articles since watermark.
    /// </summary>
    public async Task<DeltaSyncResponse<ArticleDto>> SyncArticlesAsync(
        string tenantId,
        DeltaSyncRequest request,
        CancellationToken ct = default)
    {
        var url = $"{ErpApiEndpoints.ArticlesSync}?watermark={request.Watermark}&batchSize={request.BatchSize}";
        if (!string.IsNullOrEmpty(request.ContinuationToken))
            url += $"&continuationToken={Uri.EscapeDataString(request.ContinuationToken)}";

        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);
        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DeltaSyncResponse<ArticleDto>>(_jsonOptions, ct)
            ?? new DeltaSyncResponse<ArticleDto>();
    }

    #endregion

    #region Articles - Streaming (JSON Lines)

    /// <summary>
    /// Stream articles using JSON Lines format for memory efficiency.
    /// </summary>
    public async IAsyncEnumerable<ArticleDto> StreamArticlesAsync(
        string tenantId,
        StreamRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var url = $"{ErpApiEndpoints.ArticlesStream}?chunkSize={request.ChunkSize}&format={(int)request.Format}";
        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);

        if (request.UseCompression)
            httpRequest.Headers.AcceptEncoding.ParseAdd("gzip");

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var article = JsonSerializer.Deserialize<ArticleDto>(line, _jsonOptions);
            if (article != null)
                yield return article;
        }
    }

    #endregion

    #region Articles - Batch Operations

    /// <summary>
    /// Batch write articles.
    /// </summary>
    public async Task<BatchWriteResponse<ArticleDto>> BatchWriteArticlesAsync(
        string tenantId,
        BatchWriteRequest<ArticleDto> request,
        CancellationToken ct = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Post, ErpApiEndpoints.ArticlesBatch, tenantId);
        httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);

        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BatchWriteResponse<ArticleDto>>(_jsonOptions, ct)
            ?? new BatchWriteResponse<ArticleDto>();
    }

    #endregion

    #region Customers - Cursor Pagination

    /// <summary>
    /// Get customers with cursor-based pagination.
    /// </summary>
    public async Task<CursorPageResponse<CustomerDto>> GetCustomersAsync(
        string tenantId,
        CursorPageRequest request,
        CancellationToken ct = default)
    {
        var url = BuildUrl(ErpApiEndpoints.Customers, request.Cursor, request.PageSize);
        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);

        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CursorPageResponse<CustomerDto>>(_jsonOptions, ct)
            ?? new CursorPageResponse<CustomerDto>();
    }

    #endregion

    #region Customers - Delta Sync

    /// <summary>
    /// Delta sync customers since watermark.
    /// </summary>
    public async Task<DeltaSyncResponse<CustomerDto>> SyncCustomersAsync(
        string tenantId,
        DeltaSyncRequest request,
        CancellationToken ct = default)
    {
        var url = $"{ErpApiEndpoints.CustomersSync}?watermark={request.Watermark}&batchSize={request.BatchSize}";
        if (!string.IsNullOrEmpty(request.ContinuationToken))
            url += $"&continuationToken={Uri.EscapeDataString(request.ContinuationToken)}";

        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);
        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DeltaSyncResponse<CustomerDto>>(_jsonOptions, ct)
            ?? new DeltaSyncResponse<CustomerDto>();
    }

    #endregion

    #region Customers - Streaming

    /// <summary>
    /// Stream customers using JSON Lines format.
    /// </summary>
    public async IAsyncEnumerable<CustomerDto> StreamCustomersAsync(
        string tenantId,
        StreamRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var url = $"{ErpApiEndpoints.CustomersStream}?chunkSize={request.ChunkSize}&format={(int)request.Format}";
        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);

        if (request.UseCompression)
            httpRequest.Headers.AcceptEncoding.ParseAdd("gzip");

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var customer = JsonSerializer.Deserialize<CustomerDto>(line, _jsonOptions);
            if (customer != null)
                yield return customer;
        }
    }

    /// <summary>
    /// Batch write customers.
    /// </summary>
    public async Task<BatchWriteResponse<CustomerDto>> BatchWriteCustomersAsync(
        string tenantId,
        BatchWriteRequest<CustomerDto> request,
        CancellationToken ct = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Post, ErpApiEndpoints.CustomersBatch, tenantId);
        httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);

        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BatchWriteResponse<CustomerDto>>(_jsonOptions, ct)
            ?? new BatchWriteResponse<CustomerDto>();
    }

    #endregion

    #region Orders - Cursor Pagination

    /// <summary>
    /// Get orders with cursor-based pagination.
    /// </summary>
    public async Task<CursorPageResponse<OrderDto>> GetOrdersAsync(
        string tenantId,
        CursorPageRequest request,
        CancellationToken ct = default)
    {
        var url = BuildUrl(ErpApiEndpoints.Orders, request.Cursor, request.PageSize);
        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);

        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CursorPageResponse<OrderDto>>(_jsonOptions, ct)
            ?? new CursorPageResponse<OrderDto>();
    }

    #endregion

    #region Orders - Delta Sync

    /// <summary>
    /// Delta sync orders since watermark.
    /// </summary>
    public async Task<DeltaSyncResponse<OrderDto>> SyncOrdersAsync(
        string tenantId,
        DeltaSyncRequest request,
        CancellationToken ct = default)
    {
        var url = $"{ErpApiEndpoints.OrdersSync}?watermark={request.Watermark}&batchSize={request.BatchSize}";
        if (!string.IsNullOrEmpty(request.ContinuationToken))
            url += $"&continuationToken={Uri.EscapeDataString(request.ContinuationToken)}";

        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);
        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DeltaSyncResponse<OrderDto>>(_jsonOptions, ct)
            ?? new DeltaSyncResponse<OrderDto>();
    }

    #endregion

    #region Orders - Streaming

    /// <summary>
    /// Stream orders using JSON Lines format.
    /// </summary>
    public async IAsyncEnumerable<OrderDto> StreamOrdersAsync(
        string tenantId,
        StreamRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var url = $"{ErpApiEndpoints.OrdersStream}?chunkSize={request.ChunkSize}&format={(int)request.Format}";
        using var httpRequest = CreateRequest(HttpMethod.Get, url, tenantId);

        if (request.UseCompression)
            httpRequest.Headers.AcceptEncoding.ParseAdd("gzip");

        using var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var order = JsonSerializer.Deserialize<OrderDto>(line, _jsonOptions);
            if (order != null)
                yield return order;
        }
    }

    /// <summary>
    /// Batch write orders.
    /// </summary>
    public async Task<BatchWriteResponse<OrderDto>> BatchWriteOrdersAsync(
        string tenantId,
        BatchWriteRequest<OrderDto> request,
        CancellationToken ct = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Post, ErpApiEndpoints.OrdersBatch, tenantId);
        httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);

        using var response = await _httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BatchWriteResponse<OrderDto>>(_jsonOptions, ct)
            ?? new BatchWriteResponse<OrderDto>();
    }

    #endregion

    #region Helper Methods

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, string tenantId)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add(ErpHttpHeaders.TenantId, tenantId);
        request.Headers.Add(ErpHttpHeaders.CorrelationId, Guid.NewGuid().ToString());
        request.Headers.Add(ErpHttpHeaders.ApiVersion, "1.0");
        return request;
    }

    private static string BuildUrl(string baseUrl, string? cursor, int pageSize)
    {
        var url = $"{baseUrl}?pageSize={pageSize}";
        if (!string.IsNullOrEmpty(cursor))
            url += $"&cursor={Uri.EscapeDataString(cursor)}";
        return url;
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        // HttpClient is typically managed by IHttpClientFactory, don't dispose
        GC.SuppressFinalize(this);
    }

    #endregion
}

/// <summary>
/// Configuration options for ErpHttpClient.
/// </summary>
public class ErpHttpClientOptions
{
    /// <summary>
    /// Base URL for the ERP Connector.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5050";

    /// <summary>
    /// Request timeout.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Enable automatic decompression.
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Default page size for pagination.
    /// </summary>
    public int DefaultPageSize { get; set; } = 1000;

    /// <summary>
    /// Default batch size for sync operations.
    /// </summary>
    public int DefaultBatchSize { get; set; } = 5000;

    /// <summary>
    /// Retry count for transient failures.
    /// </summary>
    public int RetryCount { get; set; } = 3;
}

/// <summary>
/// Interface for ERP client operations.
/// </summary>
public interface IErpClient
{
    // Health
    Task<ConnectorHealthResponse> GetHealthAsync(CancellationToken ct = default);

    // Articles
    Task<CursorPageResponse<ArticleDto>> GetArticlesAsync(string tenantId, CursorPageRequest request, CancellationToken ct = default);
    Task<DeltaSyncResponse<ArticleDto>> SyncArticlesAsync(string tenantId, DeltaSyncRequest request, CancellationToken ct = default);
    IAsyncEnumerable<ArticleDto> StreamArticlesAsync(string tenantId, StreamRequest request, CancellationToken ct = default);
    Task<BatchWriteResponse<ArticleDto>> BatchWriteArticlesAsync(string tenantId, BatchWriteRequest<ArticleDto> request, CancellationToken ct = default);

    // Customers
    Task<CursorPageResponse<CustomerDto>> GetCustomersAsync(string tenantId, CursorPageRequest request, CancellationToken ct = default);
    Task<DeltaSyncResponse<CustomerDto>> SyncCustomersAsync(string tenantId, DeltaSyncRequest request, CancellationToken ct = default);
    IAsyncEnumerable<CustomerDto> StreamCustomersAsync(string tenantId, StreamRequest request, CancellationToken ct = default);
    Task<BatchWriteResponse<CustomerDto>> BatchWriteCustomersAsync(string tenantId, BatchWriteRequest<CustomerDto> request, CancellationToken ct = default);

    // Orders
    Task<CursorPageResponse<OrderDto>> GetOrdersAsync(string tenantId, CursorPageRequest request, CancellationToken ct = default);
    Task<DeltaSyncResponse<OrderDto>> SyncOrdersAsync(string tenantId, DeltaSyncRequest request, CancellationToken ct = default);
    IAsyncEnumerable<OrderDto> StreamOrdersAsync(string tenantId, StreamRequest request, CancellationToken ct = default);
    Task<BatchWriteResponse<OrderDto>> BatchWriteOrdersAsync(string tenantId, BatchWriteRequest<OrderDto> request, CancellationToken ct = default);
}
