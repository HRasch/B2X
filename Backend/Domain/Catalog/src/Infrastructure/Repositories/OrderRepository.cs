using Microsoft.EntityFrameworkCore;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;

namespace B2Connect.Catalog.Infrastructure.Repositories;

/// <summary>
/// EFCore implementation of IOrderRepository
/// VVVG §357 Order & Withdrawal Period Management
/// 
/// Compliance Focus:
/// - DeliveredAt timestamp crucial for 14-day window calculation
/// - B2B VAT-ID validation and Reverse Charge tracking
/// - Complete PII encryption via EF Core value converters
/// - Immutable audit trail (CreatedAt, CreatedBy never updated)
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(CatalogDbContext context, ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateOrderAsync(
        Guid tenantId,
        Order order,
        CancellationToken cancellationToken = default)
    {
        if (order.TenantId != tenantId)
            throw new InvalidOperationException("Tenant mismatch");

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order created: {OrderId} Customer: {CustomerId} Amount: {Amount} (Tenant: {TenantId})",
            order.Id, order.CustomerId, order.TotalAmount, tenantId
        );
    }

    public async Task<Order?> GetOrderAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == orderId && !x.IsDeleted,
                cancellationToken
            );
    }

    public async Task MarkAsDeliveredAsync(
        Guid tenantId,
        Guid orderId,
        DateTime deliveredAt,
        Guid updatedBy,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(tenantId, orderId, cancellationToken);

        if (order == null)
            throw new KeyNotFoundException($"Order {orderId} not found");

        order.DeliveredAt = deliveredAt;
        order.Status = "Delivered";
        order.ModifiedAt = DateTime.UtcNow;
        order.ModifiedBy = updatedBy;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order marked as delivered: {OrderId} DeliveredAt: {DeliveredAt} WithdrawalDaysRemaining: 14 (Tenant: {TenantId})",
            orderId, deliveredAt, tenantId
        );
    }

    public async Task<int> GetWithdrawalDaysRemainingAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(tenantId, orderId, cancellationToken);

        if (order == null)
            throw new KeyNotFoundException($"Order {orderId} not found");

        if (order.DeliveredAt == null)
            return 0;  // Not delivered yet

        var daysElapsed = (DateTime.UtcNow - order.DeliveredAt.Value).Days;
        var daysRemaining = 14 - daysElapsed;

        return Math.Max(0, daysRemaining);
    }

    public async Task<bool> IsWithinWithdrawalPeriodAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var daysRemaining = await GetWithdrawalDaysRemainingAsync(tenantId, orderId, cancellationToken);
        return daysRemaining > 0;
    }

    public async Task<List<Order>> GetCustomerOrdersAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateOrderStatusAsync(
        Guid tenantId,
        Guid orderId,
        string status,
        Guid updatedBy,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(tenantId, orderId, cancellationToken);

        if (order == null)
            throw new KeyNotFoundException($"Order {orderId} not found");

        var previousStatus = order.Status;
        order.Status = status;
        order.ModifiedAt = DateTime.UtcNow;
        order.ModifiedBy = updatedBy;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order status changed: {OrderId} {OldStatus} → {NewStatus} (Tenant: {TenantId})",
            orderId, previousStatus, status, tenantId
        );
    }
}
