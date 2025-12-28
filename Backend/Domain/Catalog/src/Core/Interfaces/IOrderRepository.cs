using B2Connect.Catalog.Core.Entities;

namespace B2Connect.Catalog.Core.Interfaces;

/// <summary>
/// VVVG §357-359 Order Management for Withdrawal Period Compliance
/// 
/// Order repository ensures:
/// - Accurate DeliveredAt tracking (needed for 14-day calculation)
/// - B2B VAT-ID and Reverse Charge status
/// - Encrypted PII fields (customer names, addresses, emails)
/// - Complete audit trail for compliance verification
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Create new order
    /// Used in checkout (F1.3 task)
    /// </summary>
    Task CreateOrderAsync(
        Guid tenantId,
        Order order,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get order by ID
    /// Includes tenant isolation check
    /// </summary>
    Task<Order?> GetOrderAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark order as delivered
    /// Sets DeliveredAt timestamp (VVVG §357 requirement)
    /// </summary>
    Task MarkAsDeliveredAsync(
        Guid tenantId,
        Guid orderId,
        DateTime deliveredAt,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get withdrawal period remaining for order
    /// Returns days remaining before 14-day window closes
    /// Used for return eligibility checks
    /// </summary>
    Task<int> GetWithdrawalDaysRemainingAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if order is within withdrawal period
    /// Used before accepting return requests
    /// </summary>
    Task<bool> IsWithinWithdrawalPeriodAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get orders by customer
    /// For return history and customer service
    /// </summary>
    Task<List<Order>> GetCustomerOrdersAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update order delivery status
    /// </summary>
    Task UpdateOrderStatusAsync(
        Guid tenantId,
        Guid orderId,
        string status,
        Guid updatedBy,
        CancellationToken cancellationToken = default);
}
