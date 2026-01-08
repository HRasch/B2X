// <copyright file="OrderQueryHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.Domain.ERP.Services;
using B2X.ERP.Abstractions;
using B2X.ERP.Events;
using B2X.ERP.Queries;
using Wolverine;

namespace B2X.ERP.Handlers;

/// <summary>
/// Wolverine query handlers for order operations.
/// Uses the existing ErpService while providing CQRS separation.
/// Publishes access events for analytics.
/// </summary>
public static class OrderQueryHandlers
{
    /// <summary>
    /// Handle get order query.
    /// </summary>
    public static async Task<OrderDto?> Handle(
        GetOrderQuery query,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var result = await erpService.GetOrderAsync(query.TenantId, query.OrderNumber, ct);

        // Publish access event for analytics
        if (result != null)
        {
            await bus.PublishAsync(new OrderAccessedEvent(query.TenantId, query.OrderNumber, DateTime.UtcNow));
        }

        return result;
    }

    /// <summary>
    /// Handle get orders by IDs query.
    /// </summary>
    public static async Task<IEnumerable<OrderDto>> Handle(
        GetOrdersByIdsQuery query,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var results = await erpService.GetOrdersByIdsAsync(query.TenantId, query.OrderNumbers, ct);

        // Publish access events for each found order
        foreach (var order in results)
        {
            await bus.PublishAsync(new OrderAccessedEvent(query.TenantId, order.OrderNumber, DateTime.UtcNow));
        }

        return results;
    }

    /// <summary>
    /// Handle query orders query.
    /// </summary>
    public static async IAsyncEnumerable<OrderDto> Handle(
        QueryOrdersQuery query,
        IErpService erpService,
        IMessageBus bus)
    {
        await foreach (var order in erpService.QueryOrdersAsync(query.TenantId, query.Query))
        {
            // Publish access event for each order
            await bus.PublishAsync(new OrderAccessedEvent(query.TenantId, order.OrderNumber, DateTime.UtcNow));
            yield return order;
        }
    }
}
