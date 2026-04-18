using B2X.Orders.Core.Entities;
using B2X.Orders.Core.Interfaces;
using B2X.Orders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2X.Orders.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for order operations using EF Core
/// </summary>
public class OrdersRepository : IOrdersRepository
{
    private readonly OrdersDbContext _context;

    public OrdersRepository(OrdersDbContext context)
    {
        _context = context;
    }

    // Order operations
    public async Task<Order?> GetOrderByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.TenantId == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByTenantAsync(
        Guid tenantId,
        string? status = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Where(o => o.TenantId == tenantId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        if (!string.IsNullOrEmpty(paymentStatus))
            query = query.Where(o => o.PaymentStatus == paymentStatus);

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.CreatedAt <= toDate.Value);

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetOrdersCountByTenantAsync(
        Guid tenantId,
        string? status = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.Where(o => o.TenantId == tenantId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        if (!string.IsNullOrEmpty(paymentStatus))
            query = query.Where(o => o.PaymentStatus == paymentStatus);

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.CreatedAt <= toDate.Value);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, Guid tenantId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId && o.TenantId == tenantId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetOrdersCountByCustomerAsync(Guid customerId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .CountAsync(o => o.CustomerId == customerId && o.TenantId == tenantId, cancellationToken);
    }

    public async Task AddOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrderAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var order = await GetOrderByIdAsync(id, tenantId, cancellationToken);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Order item operations
    public async Task<OrderItem?> GetOrderItemByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.Id == id && oi.Order!.TenantId == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.OrderId == orderId && oi.Order!.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        await _context.OrderItems.AddAsync(orderItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrderItemAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var orderItem = await GetOrderItemByIdAsync(id, tenantId, cancellationToken);
        if (orderItem != null)
        {
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Bulk operations
    public async Task AddOrderItemsAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        await _context.OrderItems.AddRangeAsync(orderItems, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
