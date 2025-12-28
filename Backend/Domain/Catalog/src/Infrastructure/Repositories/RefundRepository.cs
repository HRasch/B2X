using Microsoft.EntityFrameworkCore;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;

namespace B2Connect.Catalog.Infrastructure.Repositories;

/// <summary>
/// EFCore implementation of IRefundRepository
/// VVVG §357-359 Refund Management with 10-Year Archival
/// 
/// Compliance Implementation:
/// - 10-year retention for tax/audit compliance
/// - Immutable audit trail (CreatedAt/CreatedBy never modified)
/// - Tenant isolation on all queries (critical for SaaS security)
/// - Soft deletes (IsDeleted flag prevents accidental permanent deletion)
/// - ProcessedAt/CompletedAt timestamps for refund status transitions
/// </summary>
public class RefundRepository : IRefundRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<RefundRepository> _logger;

    public RefundRepository(CatalogDbContext context, ILogger<RefundRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateRefundAsync(
        Guid tenantId,
        Refund refund,
        CancellationToken cancellationToken = default)
    {
        if (refund.TenantId != tenantId)
            throw new InvalidOperationException("Tenant mismatch");

        refund.RefundStatus = "Initiated";
        refund.InitiatedAt = DateTime.UtcNow;

        await _context.Refunds.AddAsync(refund, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Refund created: {RefundId} ReturnRequest: {ReturnRequestId} Amount: {Amount} (Tenant: {TenantId})",
            refund.Id, refund.ReturnRequestId, refund.RefundAmount, tenantId
        );
    }

    public async Task<Refund?> GetRefundAsync(
        Guid tenantId,
        Guid refundId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == refundId && !x.IsDeleted,
                cancellationToken
            );
    }

    public async Task UpdateRefundStatusAsync(
        Guid tenantId,
        Guid refundId,
        string newStatus,
        Guid updatedBy,
        CancellationToken cancellationToken = default)
    {
        var refund = await GetRefundAsync(tenantId, refundId, cancellationToken);

        if (refund == null)
            throw new KeyNotFoundException($"Refund {refundId} not found");

        var previousStatus = refund.RefundStatus;
        refund.RefundStatus = newStatus;
        refund.ModifiedAt = DateTime.UtcNow;
        refund.ModifiedBy = updatedBy;

        // Set ProcessedAt only on transition to "Processed"
        if (newStatus == "Processed" && previousStatus != "Processed")
            refund.ProcessedAt = DateTime.UtcNow;

        // Set CompletedAt only on transition to "Completed"
        if (newStatus == "Completed" && previousStatus != "Completed")
            refund.CompletedAt = DateTime.UtcNow;

        _context.Refunds.Update(refund);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Refund status changed: {RefundId} {OldStatus} → {NewStatus} (Tenant: {TenantId})",
            refundId, previousStatus, newStatus, tenantId
        );
    }

    public async Task<List<Refund>> GetOrderRefundsAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        // First get all return requests for the order
        var returnRequestIds = await _context.ReturnRequests
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.OrderId == orderId && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        // Then get refunds for those return requests
        return await _context.Refunds
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && returnRequestIds.Contains(x.ReturnRequestId) && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Refund>> GetRefundsByStatusAsync(
        Guid tenantId,
        string status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Refunds
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.RefundStatus == status && !x.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Refund>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task UpdateRefundPaymentAsync(
        Guid tenantId,
        Guid refundId,
        string bankAccountEncrypted,
        string bankNameEncrypted,
        string paymentReferenceId,
        Guid updatedBy,
        CancellationToken cancellationToken = default)
    {
        var refund = await GetRefundAsync(tenantId, refundId, cancellationToken);

        if (refund == null)
            throw new KeyNotFoundException($"Refund {refundId} not found");

        refund.BankAccountNumberEncrypted = bankAccountEncrypted;
        refund.BankNameEncrypted = bankNameEncrypted;
        refund.PaymentReferenceId = paymentReferenceId;
        refund.ModifiedAt = DateTime.UtcNow;
        refund.ModifiedBy = updatedBy;

        _context.Refunds.Update(refund);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Refund payment info updated: {RefundId} PaymentRef: {PaymentRef} (Tenant: {TenantId})",
            refundId, paymentReferenceId, tenantId
        );
    }

    public async Task ArchiveCompletedRefundsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddYears(-10);

        var refundsToArchive = await _context.Refunds
            .Where(x => x.TenantId == tenantId && 
                        x.RefundStatus == "Completed" && 
                        x.CompletedAt < cutoffDate && 
                        !x.IsArchived &&
                        !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (!refundsToArchive.Any())
        {
            _logger.LogInformation("No refunds to archive (Tenant: {TenantId})", tenantId);
            return;
        }

        foreach (var refund in refundsToArchive)
        {
            refund.IsArchived = true;
            refund.ArchivedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Archived {Count} completed refunds (older than 10 years) (Tenant: {TenantId})",
            refundsToArchive.Count, tenantId
        );
    }

    public async Task<List<Refund>> GetCustomerRefundHistoryAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        // Get all orders for customer
        var orderIds = await _context.Orders
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        // Get refunds for those orders
        return await _context.Refunds
            .AsNoTracking()
            .Join(
                _context.ReturnRequests.AsNoTracking().Where(r => !r.IsDeleted),
                refund => refund.ReturnRequestId,
                returnRequest => returnRequest.Id,
                (refund, returnRequest) => new { Refund = refund, ReturnRequest = returnRequest }
            )
            .Where(x => x.Refund.TenantId == tenantId && 
                        orderIds.Contains(x.ReturnRequest.OrderId) && 
                        !x.Refund.IsDeleted)
            .Select(x => x.Refund)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasRefundForReturnAsync(
        Guid tenantId,
        Guid returnRequestId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds
            .AsNoTracking()
            .AnyAsync(
                x => x.TenantId == tenantId && x.ReturnRequestId == returnRequestId && !x.IsDeleted,
                cancellationToken
            );
    }

    public async Task<decimal> GetTotalRefundAmountAsync(
        Guid tenantId,
        Guid customerId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        // Get all orders for customer
        var orderIds = await _context.Orders
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var query = _context.Refunds
            .AsNoTracking()
            .Join(
                _context.ReturnRequests.AsNoTracking().Where(r => !r.IsDeleted),
                refund => refund.ReturnRequestId,
                returnRequest => returnRequest.Id,
                (refund, returnRequest) => new { Refund = refund, ReturnRequest = returnRequest }
            )
            .Where(x => x.Refund.TenantId == tenantId && 
                        orderIds.Contains(x.ReturnRequest.OrderId) && 
                        x.Refund.RefundStatus == "Completed" && 
                        !x.Refund.IsDeleted);

        if (from.HasValue)
            query = query.Where(x => x.Refund.CompletedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.Refund.CompletedAt <= to.Value);

        return await query.SumAsync(x => x.Refund.RefundAmount, cancellationToken);
    }
}
