// <copyright file="OrderCommandHandlers.cs" company="NissenVelten">
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
/// Wolverine command handlers for order operations.
/// Uses the existing ErpService while providing CQRS separation.
/// Publishes events for side effects.
/// </summary>
public static class OrderCommandHandlers
{
    /// <summary>
    /// Handle sync orders command.
    /// </summary>
    public static async Task<DeltaSyncResponse<OrderDto>> Handle(
        SyncOrdersCommand command,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var response = await erpService.SyncOrdersAsync(command.TenantId, command.Request, ct);

        // Publish event for side effects
        await bus.PublishAsync(new OrdersSyncedEvent(command.TenantId, response, DateTime.UtcNow));

        return response;
    }

    /// <summary>
    /// Handle batch write orders command.
    /// </summary>
    public static async Task<BatchWriteResponse<OrderDto>> Handle(
        BatchWriteOrdersCommand command,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var response = await erpService.BatchWriteOrdersAsync(command.TenantId, command.Orders, ct);

        // Publish event for side effects
        await bus.PublishAsync(new OrdersBatchWrittenEvent(command.TenantId, response, DateTime.UtcNow));

        return response;
    }
}