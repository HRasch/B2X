// <copyright file="OrderEventHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace B2X.ERP.Handlers;

/// <summary>
/// Event handlers for order-related events.
/// Demonstrates Wolverine's event-driven capabilities.
/// </summary>
public static class OrderEventHandlers
{
    /// <summary>
    /// Handle orders synced event - could update search indexes, cache, etc.
    /// </summary>
    public static void Handle(
        OrdersSyncedEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Orders synced for tenant {TenantId}: {SyncedCount} changes, watermark {Watermark}",
            @event.TenantId,
            @event.Response.Changes?.Count ?? 0,
            @event.Response.NewWatermark);

        // Could publish to search service, invalidate caches, etc.
        // For now, just log the event
    }

    /// <summary>
    /// Handle orders batch written event.
    /// </summary>
    public static void Handle(
        OrdersBatchWrittenEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Orders batch written for tenant {TenantId}: {SuccessCount} successful, {ErrorCount} failed",
            @event.TenantId,
            @event.Response.SuccessCount,
            @event.Response.ErrorCount);

        // Could trigger re-indexing, cache invalidation, etc.
    }

    /// <summary>
    /// Handle order accessed event.
    /// </summary>
    public static void Handle(
        OrderAccessedEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Order {OrderNumber} accessed in tenant {TenantId} at {Timestamp}",
            @event.OrderNumber,
            @event.TenantId,
            @event.Timestamp);

        // Could update access analytics, cache warming, etc.
    }
}
