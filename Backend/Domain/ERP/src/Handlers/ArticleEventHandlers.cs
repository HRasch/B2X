// <copyright file="ArticleEventHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace B2X.ERP.Handlers;

/// <summary>
/// Event handlers for article-related events.
/// Demonstrates Wolverine's event-driven capabilities.
/// </summary>
public static class ArticleEventHandlers
{
    /// <summary>
    /// Handle articles synced event - could update search indexes, cache, etc.
    /// </summary>
    public static void Handle(
        ArticlesSyncedEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Articles synced for tenant {TenantId}: {SyncedCount} changes, watermark {Watermark}",
            @event.TenantId,
            @event.Response.Changes?.Count ?? 0,
            @event.Response.NewWatermark);

        // Could publish to search service, invalidate caches, etc.
        // For now, just log the event
    }

    /// <summary>
    /// Handle articles batch written event.
    /// </summary>
    public static void Handle(
        ArticlesBatchWrittenEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Articles batch written for tenant {TenantId}: {SuccessCount} successful, {ErrorCount} failed",
            @event.TenantId,
            @event.Response.SuccessCount,
            @event.Response.ErrorCount);

        // Could trigger re-indexing, cache invalidation, etc.
    }

    /// <summary>
    /// Handle article accessed event - could update access statistics.
    /// </summary>
    public static void Handle(
        ArticleAccessedEvent @event,
        ILogger logger)
    {
        logger.LogDebug(
            "Article {ArticleId} accessed for tenant {TenantId}",
            @event.ArticleId,
            @event.TenantId);

        // Could update access statistics, popularity metrics, etc.
    }
}
