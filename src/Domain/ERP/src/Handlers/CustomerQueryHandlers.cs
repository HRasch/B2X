// <copyright file="CustomerQueryHandlers.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.Domain.ERP.Services;
using B2X.ERP.Abstractions;
using B2X.ERP.Events;
using B2X.ERP.Queries;
using Wolverine;

namespace B2X.ERP.Handlers;

/// <summary>
/// Wolverine query handlers for customer operations.
/// Uses the existing ErpService while providing CQRS separation.
/// Publishes access events for analytics.
/// </summary>
public static class CustomerQueryHandlers
{
    /// <summary>
    /// Handle get customer query.
    /// </summary>
    public static async Task<CustomerDto?> Handle(
        GetCustomerQuery query,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var result = await erpService.GetCustomerAsync(query.TenantId, query.CustomerNumber, ct);

        // Publish access event for analytics
        if (result != null)
        {
            await bus.PublishAsync(new CustomerAccessedEvent(query.TenantId, query.CustomerNumber, DateTime.UtcNow));
        }

        return result;
    }

    /// <summary>
    /// Handle get customers by IDs query.
    /// </summary>
    public static async Task<IEnumerable<CustomerDto>> Handle(
        GetCustomersByIdsQuery query,
        IErpService erpService,
        IMessageBus bus,
        CancellationToken ct = default)
    {
        var results = await erpService.GetCustomersByIdsAsync(query.TenantId, query.CustomerNumbers, ct);

        // Publish access events for each found customer
        foreach (var customer in results)
        {
            await bus.PublishAsync(new CustomerAccessedEvent(query.TenantId, customer.CustomerNumber, DateTime.UtcNow));
        }

        return results;
    }

    /// <summary>
    /// Handle query customers query.
    /// </summary>
    public static async IAsyncEnumerable<CustomerDto> Handle(
        QueryCustomersQuery query,
        IErpService erpService,
        IMessageBus bus)
    {
        await foreach (var customer in erpService.QueryCustomersAsync(query.TenantId, query.Query))
        {
            // Publish access event for each customer
            await bus.PublishAsync(new CustomerAccessedEvent(query.TenantId, customer.CustomerNumber, DateTime.UtcNow));
            yield return customer;
        }
    }
}
