using B2Connect.Catalog.Core.Entities;

namespace B2Connect.Catalog.Core.Interfaces;

/// <summary>
/// VVVG §357-359 Withdrawal Period Management
/// Return requests are withdrawal of purchase contracts within 14-day period
/// 
/// Repository manages:
/// - 14-day withdrawal request creation (ReturnRequest lifecycle)
/// - Refund status tracking (10-year archival compliance)
/// - Cross-tenant isolation (TenantId filter on all queries)
/// 
/// Compliance Notes:
/// - All queries filter by TenantId (no cross-tenant leaks)
/// - Soft deletes respected (IsDeleted filter automatic)
/// - Immutable audit trail (CreatedAt never changes)
/// - Encrypted PII fields (handled by EF Core value converters)
/// </summary>
public interface IReturnRepository
{
    /// <summary>
    /// Create a new withdrawal request (VVVG §357)
    /// Only possible within 14 days of delivery
    /// </summary>
    Task CreateReturnRequestAsync(
        Guid tenantId,
        ReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get specific return request by ID
    /// Includes tenant isolation check
    /// </summary>
    Task<ReturnRequest?> GetReturnRequestAsync(
        Guid tenantId,
        Guid returnRequestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all return requests for an order
    /// Tenant-isolated query
    /// </summary>
    Task<List<ReturnRequest>> GetOrderReturnRequestsAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all withdrawal requests for a customer
    /// Tenant-isolated query with optional filtering
    /// </summary>
    Task<PagedResult<ReturnRequest>> GetCustomerReturnRequestsAsync(
        Guid tenantId,
        Guid customerId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update return request status
    /// Handles transitions: Requested → ReturnLabelSent → InTransit → Received → Refunded
    /// </summary>
    Task UpdateReturnRequestStatusAsync(
        Guid tenantId,
        Guid returnRequestId,
        string newStatus,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Store generated return label
    /// Called after carrier API integration (Task #4)
    /// </summary>
    Task AttachReturnLabelAsync(
        Guid tenantId,
        Guid returnRequestId,
        string carrier,
        string trackingNumber,
        string labelUrl,
        Guid attachedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create refund record (10-year archival)
    /// Called when return is received and verified
    /// </summary>
    Task CreateRefundAsync(
        Guid tenantId,
        Refund refund,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get refund by ID
    /// For refund status tracking and archival purposes
    /// </summary>
    Task<Refund?> GetRefundAsync(
        Guid tenantId,
        Guid refundId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all refunds for an order
    /// For financial reporting and archival
    /// </summary>
    Task<List<Refund>> GetOrderRefundsAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update refund status
    /// Tracks payment status: Initiated → InProgress → Processed → Completed
    /// </summary>
    Task UpdateRefundStatusAsync(
        Guid tenantId,
        Guid refundId,
        string newStatus,
        DateTime? processedAt,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Archive old refunds (10-year retention cleanup)
    /// Called annually to move completed refunds to archive
    /// </summary>
    Task ArchiveCompletedRefundsAsync(
        Guid tenantId,
        DateTime? beforeDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Query withdrawal requests by status (for workflow management)
    /// Example: Get all "InTransit" returns to follow up with customers
    /// </summary>
    Task<List<ReturnRequest>> GetReturnRequestsByStatusAsync(
        Guid tenantId,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if order has active return request
    /// Used to prevent duplicate returns or to show return status in checkout
    /// </summary>
    Task<bool> HasActiveReturnRequestAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Paged result wrapper for list queries
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
