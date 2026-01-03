// <copyright file="ErpCqrsUsageExample.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;
using B2Connect.ERP.Commands;
using B2Connect.ERP.Queries;
using Wolverine;

namespace B2Connect.ERP.Examples;

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
}