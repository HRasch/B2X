// <copyright file="ArticleQueryHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.Domain.ERP.Services;
using B2X.ERP.Abstractions;
using B2X.ERP.Events;
using B2X.ERP.Queries;
using Wolverine;

namespace B2X.ERP.Handlers;

/// <summary>
/// Wolverine query handlers for article operations.
/// Uses the existing ErpService while providing CQRS separation.
/// Publishes access events for analytics.
/// </summary>
public static class ArticleQueryHandlers
{
    /// <summary>
    /// Handle get article query.
    /// </summary>
    public static async Task<ArticleDto?> Handle(
        GetArticleQuery query,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var result = await erpService.GetArticleAsync(query.TenantId, query.ArticleId.ToString(), ct);

        // Publish access event for analytics
        if (result != null)
        {
            await bus.PublishAsync(new ArticleAccessedEvent(query.TenantId, query.ArticleId.ToString(), DateTime.UtcNow));
        }

        return result;
    }

    /// <summary>
    /// Handle get articles by IDs query.
    /// </summary>
    public static async Task<IEnumerable<ArticleDto>> Handle(
        GetArticlesByIdsQuery query,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var articleIds = query.ArticleIds.Select(id => id.ToString());
        var results = await erpService.GetArticlesByIdsAsync(query.TenantId, articleIds, ct);

        // Publish access events for each found article
        foreach (var article in results)
        {
            await bus.PublishAsync(new ArticleAccessedEvent(query.TenantId, article.ArticleId.ToString(), DateTime.UtcNow));
        }

        return results;
    }

    /// <summary>
    /// Handle query articles query.
    /// </summary>
    public static async IAsyncEnumerable<ArticleDto> Handle(
        QueryArticlesQuery query,
        IErpService erpService,
        IMessageBus bus)
    {
        await foreach (var article in erpService.QueryArticlesAsync(query.TenantId, query.Query))
        {
            // Publish access event for each article
            await bus.PublishAsync(new ArticleAccessedEvent(query.TenantId, article.ArticleId.ToString(), DateTime.UtcNow));
            yield return article;
        }
    }
}
