// <copyright file="ErpCqrsUsageExample.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;
using B2X.ERP.Commands;
using B2X.ERP.Queries;
using Wolverine;

namespace B2X.ERP.Examples;

/// <summary>
/// Example usage of the ERP CQRS layer with Wolverine.
/// Demonstrates command/query separation while using existing ErpService.
/// </summary>
public class ErpCqrsUsageExample
{
    private readonly IMessageBus _bus;

    public ErpCqrsUsageExample(IMessageBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Example: Query a single article.
    /// </summary>
    public async Task<ArticleDto?> GetArticleExample(string tenantId, int articleId)
    {
        var query = new GetArticleQuery(tenantId, articleId);
        return await _bus.InvokeAsync<ArticleDto?>(query);
    }

    /// <summary>
    /// Example: Query articles with filtering.
    /// </summary>
    public async IAsyncEnumerable<ArticleDto> QueryArticlesExample(string tenantId, QueryRequest queryRequest)
    {
        var query = new QueryArticlesQuery(tenantId, queryRequest);
        var result = await _bus.InvokeAsync<IAsyncEnumerable<ArticleDto>>(query);
        await foreach (var article in result)
        {
            yield return article;
        }
    }

    /// <summary>
    /// Example: Sync articles (command - modifies state).
    /// </summary>
    public async Task<DeltaSyncResponse<ArticleDto>> SyncArticlesExample(string tenantId, DeltaSyncRequest request)
    {
        var command = new SyncArticlesCommand(tenantId, request);
        return await _bus.InvokeAsync<DeltaSyncResponse<ArticleDto>>(command);
    }

    /// <summary>
    /// Example: Batch write articles (command - modifies state).
    /// </summary>
    public async Task<BatchWriteResponse<ArticleDto>> BatchWriteArticlesExample(string tenantId, IEnumerable<ArticleDto> articles)
    {
        var command = new BatchWriteArticlesCommand(tenantId, articles);
        return await _bus.InvokeAsync<BatchWriteResponse<ArticleDto>>(command);
    }

    /// <summary>
    /// Example: Fire-and-forget command (no response expected).
    /// </summary>
    public async Task SyncArticlesFireAndForgetExample(string tenantId, DeltaSyncRequest request)
    {
        var command = new SyncArticlesCommand(tenantId, request);
        await _bus.SendAsync(command); // Fire and forget
    }

    /// <summary>
    /// Example: Query a single customer.
    /// </summary>
    public async Task<CustomerDto?> GetCustomerExample(string tenantId, string customerNumber)
    {
        var query = new GetCustomerQuery(tenantId, customerNumber);
        return await _bus.InvokeAsync<CustomerDto?>(query);
    }

    /// <summary>
    /// Example: Query customers with filtering.
    /// </summary>
    public async IAsyncEnumerable<CustomerDto> QueryCustomersExample(string tenantId, QueryRequest queryRequest)
    {
        var query = new QueryCustomersQuery(tenantId, queryRequest);
        var result = await _bus.InvokeAsync<IAsyncEnumerable<CustomerDto>>(query);
        await foreach (var customer in result)
        {
            yield return customer;
        }
    }

    /// <summary>
    /// Example: Sync customers (command - modifies state).
    /// </summary>
    public async Task<DeltaSyncResponse<CustomerDto>> SyncCustomersExample(string tenantId, DeltaSyncRequest request)
    {
        var command = new SyncCustomersCommand(tenantId, request);
        return await _bus.InvokeAsync<DeltaSyncResponse<CustomerDto>>(command);
    }

    /// <summary>
    /// Example: Batch write customers (command - modifies state).
    /// </summary>
    public async Task<BatchWriteResponse<CustomerDto>> BatchWriteCustomersExample(string tenantId, IEnumerable<CustomerDto> customers)
    {
        var command = new BatchWriteCustomersCommand(tenantId, customers);
        return await _bus.InvokeAsync<BatchWriteResponse<CustomerDto>>(command);
    }

    /// <summary>
    /// Example: Query a single order.
    /// </summary>
    public async Task<OrderDto?> GetOrderExample(string tenantId, string orderNumber)
    {
        var query = new GetOrderQuery(tenantId, orderNumber);
        return await _bus.InvokeAsync<OrderDto?>(query);
    }

    /// <summary>
    /// Example: Query orders with filtering.
    /// </summary>
    public async IAsyncEnumerable<OrderDto> QueryOrdersExample(string tenantId, QueryRequest queryRequest)
    {
        var query = new QueryOrdersQuery(tenantId, queryRequest);
        var result = await _bus.InvokeAsync<IAsyncEnumerable<OrderDto>>(query);
        await foreach (var order in result)
        {
            yield return order;
        }
    }

    /// <summary>
    /// Example: Sync orders (command - modifies state).
    /// </summary>
    public async Task<DeltaSyncResponse<OrderDto>> SyncOrdersExample(string tenantId, DeltaSyncRequest request)
    {
        var command = new SyncOrdersCommand(tenantId, request);
        return await _bus.InvokeAsync<DeltaSyncResponse<OrderDto>>(command);
    }

    /// <summary>
    /// Example: Batch write orders (command - modifies state).
    /// </summary>
    public async Task<BatchWriteResponse<OrderDto>> BatchWriteOrdersExample(string tenantId, IEnumerable<OrderDto> orders)
    {
        var command = new BatchWriteOrdersCommand(tenantId, orders);
        return await _bus.InvokeAsync<BatchWriteResponse<OrderDto>>(command);
    }
}
