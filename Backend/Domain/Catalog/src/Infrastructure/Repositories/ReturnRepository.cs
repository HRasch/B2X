using Microsoft.EntityFrameworkCore;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;

namespace B2Connect.Catalog.Infrastructure.Repositories;

/// <summary>
/// EFCore implementation of IReturnRepository
/// VVVG §357-359 Withdrawal Period Management
/// 
/// Compliance Implementation:
/// - All queries automatically filter by TenantId (tenant isolation)
/// - Soft deletes handled via HasQueryFilter (IsDeleted check automatic)
/// - Encrypted PII accessed through EF Core value converters
/// - Immutable CreatedAt/CreatedBy preserved (no updates on audit fields)
/// - 10-year refund archival design with IsArchived flag
/// </summary>
public class ReturnRepository : IReturnRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<ReturnRepository> _logger;

    public ReturnRepository(CatalogDbContext context, ILogger<ReturnRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateReturnRequestAsync(
        Guid tenantId,
        ReturnRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.TenantId != tenantId)
            throw new InvalidOperationException("Tenant mismatch");

        await _context.ReturnRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "ReturnRequest created: {RequestId} for Order {OrderId} (Tenant: {TenantId})",
            request.Id, request.OrderId, tenantId
        );
    }

    public async Task<ReturnRequest?> GetReturnRequestAsync(
        Guid tenantId,
        Guid returnRequestId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReturnRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == returnRequestId && !x.IsDeleted,
                cancellationToken
            );
    }

    public async Task<List<ReturnRequest>> GetOrderReturnRequestsAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReturnRequests
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.OrderId == orderId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<ReturnRequest>> GetCustomerReturnRequestsAsync(
        Guid tenantId,
        Guid customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ReturnRequests
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId && !x.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ReturnRequest>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task UpdateReturnRequestStatusAsync(
        Guid tenantId,
        Guid returnRequestId,
        string newStatus,
        Guid updatedBy,
        CancellationToken cancellationToken = default)
    {
        var request = await GetReturnRequestAsync(tenantId, returnRequestId, cancellationToken);

        if (request == null)
            throw new KeyNotFoundException($"ReturnRequest {returnRequestId} not found");

        var previousStatus = request.Status;
        request.Status = newStatus;
        request.ModifiedAt = DateTime.UtcNow;
        request.ModifiedBy = updatedBy;

        _context.ReturnRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "ReturnRequest status changed: {RequestId} {OldStatus} → {NewStatus} (Tenant: {TenantId})",
            returnRequestId, previousStatus, newStatus, tenantId
        );
    }

    public async Task AttachReturnLabelAsync(
        Guid tenantId,
        Guid returnRequestId,
        string carrier,
        string trackingNumber,
        string labelUrl,
        Guid attachedBy,
        CancellationToken cancellationToken = default)
    {
        var request = await GetReturnRequestAsync(tenantId, returnRequestId, cancellationToken);

        if (request == null)
            throw new KeyNotFoundException($"ReturnRequest {returnRequestId} not found");

        request.ReturnCarrier = carrier;
        request.ReturnTrackingNumber = trackingNumber;
        request.ReturnLabelUrl = labelUrl;
        request.LabelGeneratedAt = DateTime.UtcNow;
        request.Status = "ReturnLabelSent";
        request.ModifiedAt = DateTime.UtcNow;
        request.ModifiedBy = attachedBy;

        _context.ReturnRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Return label attached: {RequestId} {Carrier} {Tracking} (Tenant: {TenantId})",
            returnRequestId, carrier, trackingNumber, tenantId
        );
    }

    public async Task CreateRefundAsync(
        Guid tenantId,
        Refund refund,
        CancellationToken cancellationToken = default)
    {
        if (refund.TenantId != tenantId)
            throw new InvalidOperationException("Tenant mismatch");

        await _context.Refunds.AddAsync(refund, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Refund created: {RefundId} for Order {OrderId} Amount: {Amount} (Tenant: {TenantId})",
            refund.Id, refund.OrderId, refund.RefundAmount, tenantId
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

    public async Task<List<Refund>> GetOrderRefundsAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.OrderId == orderId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateRefundStatusAsync(
        Guid tenantId,
        Guid refundId,
        string newStatus,
        DateTime? processedAt,
        Guid updatedBy,
        CancellationToken cancellationToken = default)
    {
        var refund = await GetRefundAsync(tenantId, refundId, cancellationToken);

        if (refund == null)
            throw new KeyNotFoundException($"Refund {refundId} not found");

        var previousStatus = refund.Status;
        refund.Status = newStatus;

        if (newStatus == "Processed" && processedAt.HasValue)
        {
            refund.ProcessedAt = processedAt.Value;
        }

        if (newStatus == "Completed")
        {
            refund.CompletedAt = DateTime.UtcNow;
        }

        refund.ModifiedAt = DateTime.UtcNow;
        refund.ModifiedBy = updatedBy;

        _context.Refunds.Update(refund);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Refund status changed: {RefundId} {OldStatus} → {NewStatus} (Tenant: {TenantId})",
            refundId, previousStatus, newStatus, tenantId
        );
    }

    public async Task ArchiveCompletedRefundsAsync(
        Guid tenantId,
        DateTime? beforeDate = null,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = beforeDate ?? DateTime.UtcNow.AddYears(-10);

        var refundsToArchive = await _context.Refunds
            .Where(x => x.TenantId == tenantId && 
                       x.Status == "Completed" && 
                       x.CompletedAt < cutoffDate &&
                       !x.IsArchived)
            .ToListAsync(cancellationToken);

        foreach (var refund in refundsToArchive)
        {
            refund.IsArchived = true;
            refund.ArchivedAt = DateTime.UtcNow;
        }

        if (refundsToArchive.Any())
        {
            _context.Refunds.UpdateRange(refundsToArchive);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Archived {Count} completed refunds for 10-year retention (Tenant: {TenantId})",
                refundsToArchive.Count, tenantId
            );
        }
    }

    public async Task<List<ReturnRequest>> GetReturnRequestsByStatusAsync(
        Guid tenantId,
        string status,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReturnRequests
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == status && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveReturnRequestAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReturnRequests
            .AnyAsync(
                x => x.TenantId == tenantId && 
                     x.OrderId == orderId && 
                     !x.IsDeleted &&
                     x.Status != "Cancelled" &&
                     x.Status != "Rejected",
                cancellationToken
            );
    }
}
