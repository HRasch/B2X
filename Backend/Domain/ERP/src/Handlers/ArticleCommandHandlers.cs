// <copyright file="ArticleCommandHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.Domain.ERP.Services;
using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;
using B2Connect.ERP.Commands;
using B2Connect.ERP.Events;
using Wolverine;

namespace B2Connect.ERP.Handlers;

/// <summary>
/// Wolverine command handlers for article operations.
/// Uses the existing ErpService while providing CQRS separation.
/// Publishes events for side effects.
/// </summary>
public static class ArticleCommandHandlers
{
    /// <summary>
    /// Handle sync articles command.
    /// </summary>
    public static async Task<DeltaSyncResponse<ArticleDto>> Handle(
        SyncArticlesCommand command,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var response = await erpService.SyncArticlesAsync(command.TenantId, command.Request, ct);

        // Publish event for side effects
        await bus.PublishAsync(new ArticlesSyncedEvent(command.TenantId, response, DateTime.UtcNow));

        return response;
    }

    /// <summary>
    /// Handle batch write articles command.
    /// </summary>
    public static async Task<BatchWriteResponse<ArticleDto>> Handle(
        BatchWriteArticlesCommand command,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var response = await erpService.BatchWriteArticlesAsync(command.TenantId, command.Articles, ct);

        // Publish event for side effects
        await bus.PublishAsync(new ArticlesBatchWrittenEvent(command.TenantId, response, DateTime.UtcNow));

        return response;
    }
}
