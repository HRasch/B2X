using B2X.Orders.Core.Entities;

namespace B2X.Orders.Core.Interfaces;

/// <summary>
/// Repository interface for order operations
/// </summary>
public interface IOrdersRepository
{
    // Order operations
    Task<Order?> GetOrderByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersByTenantAsync(
        Guid tenantId,
        string? status = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<int> GetOrdersCountByTenantAsync(
        Guid tenantId,
        string? status = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, Guid tenantId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<int> GetOrdersCountByCustomerAsync(Guid customerId, Guid tenantId, CancellationToken cancellationToken = default);
    Task AddOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    // Order item operations
    Task<OrderItem?> GetOrderItemByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId, Guid tenantId, CancellationToken cancellationToken = default);
    Task AddOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task DeleteOrderItemAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    // Bulk operations
    Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);
}
