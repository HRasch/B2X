using B2X.Orders.Core.Entities;

namespace B2X.Orders.Core.Interfaces;

/// <summary>
/// Repository interface for order operations
/// </summary>
public interface IOrdersRepository
{
    // Order operations
    Task<Order?> GetOrderByIdAsync(Guid id, Guid tenantId);
    Task<IEnumerable<Order>> GetOrdersByTenantAsync(
        Guid tenantId,
        string? status = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20);
    Task<int> GetOrdersCountByTenantAsync(
        Guid tenantId,
        string? status = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, Guid tenantId, int page = 1, int pageSize = 20);
    Task<int> GetOrdersCountByCustomerAsync(Guid customerId, Guid tenantId);
    Task AddOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Guid id, Guid tenantId);

    // Order item operations
    Task<OrderItem?> GetOrderItemByIdAsync(Guid id, Guid tenantId);
    Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId, Guid tenantId);
    Task AddOrderItemAsync(OrderItem orderItem);
    Task UpdateOrderItemAsync(OrderItem orderItem);
    Task DeleteOrderItemAsync(Guid id, Guid tenantId);

    // Bulk operations
    Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems);
}
