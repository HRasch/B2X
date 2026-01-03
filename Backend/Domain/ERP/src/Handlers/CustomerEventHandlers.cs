// <copyright file="CustomerEventHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace B2Connect.ERP.Handlers;

/// <summary>
/// Event handlers for customer-related events.
/// Demonstrates Wolverine's event-driven capabilities.
/// </summary>
public static class CustomerEventHandlers
{
    /// <summary>
    /// Handle customers synced event - could update search indexes, cache, etc.
    /// </summary>
    public static void Handle(
        CustomersSyncedEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Customers synced for tenant {TenantId}: {SyncedCount} changes, watermark {Watermark}",
            @event.TenantId,
            @event.Response.Changes?.Count ?? 0,
            @event.Response.NewWatermark);

        // Could publish to search service, invalidate caches, etc.
        // For now, just log the event
    }

    /// <summary>
    /// Handle customers batch written event.
    /// </summary>
    public static void Handle(
        CustomersBatchWrittenEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Customers batch written for tenant {TenantId}: {SuccessCount} successful, {ErrorCount} failed",
            @event.TenantId,
            @event.Response.SuccessCount,
            @event.Response.ErrorCount);

        // Could trigger re-indexing, cache invalidation, etc.
    }

    /// <summary>
    /// Handle customer accessed event.
    /// </summary>
    public static void Handle(
        CustomerAccessedEvent @event,
        ILogger logger)
    {
        logger.LogInformation(
            "Customer {CustomerNumber} accessed in tenant {TenantId} at {Timestamp}",
            @event.CustomerNumber,
            @event.TenantId,
            @event.Timestamp);

        // Could update access analytics, cache warming, etc.
    }
}