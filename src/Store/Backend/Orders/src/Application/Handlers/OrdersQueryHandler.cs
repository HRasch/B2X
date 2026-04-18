using B2X.Orders.Application.Commands;
using B2X.Orders.Core.Entities;
using B2X.Orders.Core.Interfaces;
using Wolverine;

namespace B2X.Orders.Application.Handlers;

/// <summary>
/// Query handlers for order operations
/// Implements CQRS read side
/// </summary>
public class OrdersQueryHandler
{
    private readonly IOrdersRepository _repository;

    public OrdersQueryHandler(IOrdersRepository repository)
    {
        _repository = repository;
    }

    // Query handlers
    public async Task<Order?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetOrderByIdAsync(query.Id, query.TenantId, cancellationToken);
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> Handle(GetOrdersByTenantQuery query, CancellationToken cancellationToken)
    {
        var items = await _repository.GetOrdersByTenantAsync(
            query.TenantId,
            query.Status,
            query.PaymentStatus,
            query.CustomerId,
            query.FromDate,
            query.ToDate,
            query.Page,
            query.PageSize,
            cancellationToken);

        var totalCount = await _repository.GetOrdersCountByTenantAsync(
            query.TenantId,
            query.Status,
            query.PaymentStatus,
            query.CustomerId,
            query.FromDate,
            query.ToDate,
            cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
    {
        var items = await _repository.GetOrdersByCustomerAsync(
            query.CustomerId,
            query.TenantId,
            query.Page,
            query.PageSize,
            cancellationToken);

        var totalCount = await _repository.GetOrdersCountByCustomerAsync(
            query.CustomerId,
            query.TenantId,
            cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<OrderItem>> Handle(GetOrderItemsQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetOrderItemsByOrderIdAsync(query.OrderId, query.TenantId, cancellationToken);
    }
}
