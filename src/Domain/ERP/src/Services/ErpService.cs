// <copyright file="ErpService.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.Domain.ERP.Services;

/// <summary>
/// ERP Service interface - consumed by B2X application.
/// Uses DTOs and the HTTP client for ERP operations.
/// </summary>
public interface IErpService
{
    Task<ArticleDto?> GetArticleAsync(string tenantId, string articleId, CancellationToken ct = default);
    IAsyncEnumerable<ArticleDto> QueryArticlesAsync(string tenantId, QueryRequest query, CancellationToken ct = default);
    Task<IEnumerable<ArticleDto>> GetArticlesByIdsAsync(string tenantId, IEnumerable<string> articleIds, CancellationToken ct = default);
    Task<DeltaSyncResponse<ArticleDto>> SyncArticlesAsync(string tenantId, DeltaSyncRequest request, CancellationToken ct = default);
    Task<BatchWriteResponse<ArticleDto>> BatchWriteArticlesAsync(string tenantId, IEnumerable<ArticleDto> articles, CancellationToken ct = default);

    Task<CustomerDto?> GetCustomerAsync(string tenantId, string customerId, CancellationToken ct = default);
    IAsyncEnumerable<CustomerDto> QueryCustomersAsync(string tenantId, QueryRequest query, CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> GetCustomersByIdsAsync(string tenantId, IEnumerable<string> customerIds, CancellationToken ct = default);
    Task<DeltaSyncResponse<CustomerDto>> SyncCustomersAsync(string tenantId, DeltaSyncRequest request, CancellationToken ct = default);
    Task<BatchWriteResponse<CustomerDto>> BatchWriteCustomersAsync(string tenantId, IEnumerable<CustomerDto> customers, CancellationToken ct = default);

    Task<OrderDto?> GetOrderAsync(string tenantId, string orderId, CancellationToken ct = default);
    IAsyncEnumerable<OrderDto> QueryOrdersAsync(string tenantId, QueryRequest query, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetOrdersByIdsAsync(string tenantId, IEnumerable<string> orderIds, CancellationToken ct = default);
    Task<OrderDto> CreateOrderAsync(string tenantId, OrderDto order, CancellationToken ct = default);
    Task<DeltaSyncResponse<OrderDto>> SyncOrdersAsync(string tenantId, DeltaSyncRequest request, CancellationToken ct = default);
    Task<BatchWriteResponse<OrderDto>> BatchWriteOrdersAsync(string tenantId, IEnumerable<OrderDto> orders, CancellationToken ct = default);
}

/// <summary>
/// ERP Service implementation - delegates to HTTP client.
/// </summary>
public class ErpService : IErpService
{
    private readonly IErpClient _client;

    public ErpService(IErpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    #region Articles

    public async Task<ArticleDto?> GetArticleAsync(string tenantId, string articleId, CancellationToken ct = default)
    {
        var request = new CursorPageRequest
        {
            TenantId = tenantId,
            PageSize = 1,
            Filters =
            [
                new QueryFilter { PropertyName = "articleNumber", Operator = FilterOperator.Equals, Value = articleId }
            ]
        };

        var response = await _client.GetArticlesAsync(tenantId, request, ct);
        return response.Items.FirstOrDefault();
    }

    public async IAsyncEnumerable<ArticleDto> QueryArticlesAsync(
        string tenantId,
        QueryRequest query,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var streamRequest = new StreamRequest
        {
            TenantId = tenantId,
            EntityType = EntityType.Article,
            Filter = query
        };

        await foreach (var article in _client.StreamArticlesAsync(tenantId, streamRequest, ct))
        {
            yield return article;
        }
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByIdsAsync(
        string tenantId,
        IEnumerable<string> articleIds,
        CancellationToken ct = default)
    {
        var idList = articleIds.ToList();
        var results = new List<ArticleDto>();
        string? cursor = null;

        do
        {
            var request = new CursorPageRequest
            {
                TenantId = tenantId,
                Cursor = cursor,
                PageSize = Math.Min(idList.Count, 1000),
                Filters =
                [
                    new QueryFilter
                    {
                        PropertyName = "articleNumber",
                        Operator = FilterOperator.In,
                        Value = string.Join(",", idList)
                    }
                ]
            };

            var response = await _client.GetArticlesAsync(tenantId, request, ct);
            results.AddRange(response.Items);
            cursor = response.NextCursor;
        }
        while (cursor != null);

        return results;
    }

    public Task<DeltaSyncResponse<ArticleDto>> SyncArticlesAsync(
        string tenantId,
        DeltaSyncRequest request,
        CancellationToken ct = default)
    {
        return _client.SyncArticlesAsync(tenantId, request, ct);
    }

    public Task<BatchWriteResponse<ArticleDto>> BatchWriteArticlesAsync(
        string tenantId,
        IEnumerable<ArticleDto> articles,
        CancellationToken ct = default)
    {
        var request = new BatchWriteRequest<ArticleDto>
        {
            TenantId = tenantId,
            Items = articles.ToList(),
            Mode = BatchWriteMode.Upsert
        };

        return _client.BatchWriteArticlesAsync(tenantId, request, ct);
    }

    #endregion

    #region Customers

    public async Task<CustomerDto?> GetCustomerAsync(string tenantId, string customerId, CancellationToken ct = default)
    {
        var request = new CursorPageRequest
        {
            TenantId = tenantId,
            PageSize = 1,
            Filters =
            [
                new QueryFilter { PropertyName = "customerId", Operator = FilterOperator.Equals, Value = customerId }
            ]
        };

        var response = await _client.GetCustomersAsync(tenantId, request, ct);
        return response.Items.FirstOrDefault();
    }

    public async IAsyncEnumerable<CustomerDto> QueryCustomersAsync(
        string tenantId,
        QueryRequest query,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var streamRequest = new StreamRequest
        {
            TenantId = tenantId,
            EntityType = EntityType.Customer,
            Filter = query
        };

        await foreach (var customer in _client.StreamCustomersAsync(tenantId, streamRequest, ct))
        {
            yield return customer;
        }
    }

    public Task<DeltaSyncResponse<CustomerDto>> SyncCustomersAsync(
        string tenantId,
        DeltaSyncRequest request,
        CancellationToken ct = default)
    {
        return _client.SyncCustomersAsync(tenantId, request, ct);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersByIdsAsync(
        string tenantId,
        IEnumerable<string> customerIds,
        CancellationToken ct = default)
    {
        var idList = customerIds.ToList();
        var results = new List<CustomerDto>();
        string? cursor = null;

        do
        {
            var request = new CursorPageRequest
            {
                TenantId = tenantId,
                Cursor = cursor,
                PageSize = Math.Min(idList.Count, 1000),
                Filters =
                [
                    new QueryFilter
                    {
                        PropertyName = "customerId",
                        Operator = FilterOperator.In,
                        Value = string.Join(",", idList)
                    }
                ]
            };

            var response = await _client.GetCustomersAsync(tenantId, request, ct);
            results.AddRange(response.Items);
            cursor = response.NextCursor;
        }
        while (cursor != null);

        return results;
    }

    public Task<BatchWriteResponse<CustomerDto>> BatchWriteCustomersAsync(
        string tenantId,
        IEnumerable<CustomerDto> customers,
        CancellationToken ct = default)
    {
        var request = new BatchWriteRequest<CustomerDto>
        {
            TenantId = tenantId,
            Items = customers.ToList(),
            Mode = BatchWriteMode.Upsert
        };

        return _client.BatchWriteCustomersAsync(tenantId, request, ct);
    }

    #endregion

    #region Orders

    public async Task<OrderDto?> GetOrderAsync(string tenantId, string orderId, CancellationToken ct = default)
    {
        var request = new CursorPageRequest
        {
            TenantId = tenantId,
            PageSize = 1,
            Filters =
            [
                new QueryFilter { PropertyName = "orderId", Operator = FilterOperator.Equals, Value = orderId }
            ]
        };

        var response = await _client.GetOrdersAsync(tenantId, request, ct);
        return response.Items.FirstOrDefault();
    }

    public async IAsyncEnumerable<OrderDto> QueryOrdersAsync(
        string tenantId,
        QueryRequest query,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var streamRequest = new StreamRequest
        {
            TenantId = tenantId,
            EntityType = EntityType.Order,
            Filter = query
        };

        await foreach (var order in _client.StreamOrdersAsync(tenantId, streamRequest, ct))
        {
            yield return order;
        }
    }

    public async Task<OrderDto> CreateOrderAsync(string tenantId, OrderDto order, CancellationToken ct = default)
    {
        // For now, we don't have a dedicated create endpoint, so we use batch write
        // In a real implementation, you might have a POST /orders endpoint
        throw new NotImplementedException("CreateOrderAsync requires a dedicated POST endpoint implementation");
    }

    public Task<DeltaSyncResponse<OrderDto>> SyncOrdersAsync(
        string tenantId,
        DeltaSyncRequest request,
        CancellationToken ct = default)
    {
        return _client.SyncOrdersAsync(tenantId, request, ct);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByIdsAsync(
        string tenantId,
        IEnumerable<string> orderIds,
        CancellationToken ct = default)
    {
        var idList = orderIds.ToList();
        var results = new List<OrderDto>();
        string? cursor = null;

        do
        {
            var request = new CursorPageRequest
            {
                TenantId = tenantId,
                Cursor = cursor,
                PageSize = Math.Min(idList.Count, 1000),
                Filters =
                [
                    new QueryFilter
                    {
                        PropertyName = "orderId",
                        Operator = FilterOperator.In,
                        Value = string.Join(",", idList)
                    }
                ]
            };

            var response = await _client.GetOrdersAsync(tenantId, request, ct);
            results.AddRange(response.Items);
            cursor = response.NextCursor;
        }
        while (cursor != null);

        return results;
    }

    public Task<BatchWriteResponse<OrderDto>> BatchWriteOrdersAsync(
        string tenantId,
        IEnumerable<OrderDto> orders,
        CancellationToken ct = default)
    {
        var request = new BatchWriteRequest<OrderDto>
        {
            TenantId = tenantId,
            Items = orders.ToList(),
            Mode = BatchWriteMode.Upsert
        };

        return _client.BatchWriteOrdersAsync(tenantId, request, ct);
    }

    #endregion
}
