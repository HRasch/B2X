// <copyright file="CustomerCommandHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.Domain.ERP.Services;
using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;
using B2X.ERP.Commands;
using B2X.ERP.Events;
using Wolverine;

namespace B2X.ERP.Handlers;

/// <summary>
/// Wolverine command handlers for customer operations.
/// Uses the existing ErpService while providing CQRS separation.
/// Publishes events for side effects.
/// </summary>
public static class CustomerCommandHandlers
{
    /// <summary>
    /// Handle sync customers command.
    /// </summary>
    public static async Task<DeltaSyncResponse<CustomerDto>> Handle(
        SyncCustomersCommand command,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var response = await erpService.SyncCustomersAsync(command.TenantId, command.Request, ct);

        // Publish event for side effects
        await bus.PublishAsync(new CustomersSyncedEvent(command.TenantId, response, DateTime.UtcNow));

        return response;
    }

    /// <summary>
    /// Handle batch write customers command.
    /// </summary>
    public static async Task<BatchWriteResponse<CustomerDto>> Handle(
        BatchWriteCustomersCommand command,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var response = await erpService.BatchWriteCustomersAsync(command.TenantId, command.Customers, ct);

        // Publish event for side effects
        await bus.PublishAsync(new CustomersBatchWrittenEvent(command.TenantId, response, DateTime.UtcNow));

        return response;
    }
}
