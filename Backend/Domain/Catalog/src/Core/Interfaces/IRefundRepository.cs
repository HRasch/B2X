using B2Connect.Catalog.Core.Entities;

namespace B2Connect.Catalog.Core.Interfaces;

/// <summary>
/// Repository contract for Refund entity persistence
/// VVVG §357-359 Refund Management with 10-Year Archival
/// 
/// Refund Lifecycle:
/// - Initiated: When customer initiates withdrawal (RefundStatus = "Initiated")
/// - Approved: When shop approves refund (RefundStatus = "Approved")
/// - Processed: When refund payment sent to customer (RefundStatus = "Processed", ProcessedAt set)
/// - Completed: Final state (RefundStatus = "Completed", CompletedAt set)
/// - Archived: After 10 years for compliance (IsArchived = true, ArchivedAt set)
/// 
/// Compliance Notes:
/// - German law (VVVG §357): 14-day withdrawal period → Refund within 14 days of return receipt
/// - Archival requirement: Keep 10 years for tax/audit purposes
/// - PII Encryption: All customer data encrypted at rest via EF Core value converters
/// - Tenant Isolation: TenantId filter on all queries (critical for SaaS security)
/// - Soft Deletes: IsDeleted flag prevents accidental permanent deletion
/// </summary>
public interface IRefundRepository
{
    /// <summary>
    /// Create a new refund record (Initiated status)
    /// Called when customer initiates withdrawal or return request approved
    /// 
    /// VVVG Compliance:
    /// - Sets InitiatedAt = now
    /// - RefundStatus = "Initiated"
    /// - Audit trail: CreatedAt, CreatedBy captured
    /// </summary>
    Task CreateRefundAsync(
        Guid tenantId,
        Refund refund,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve refund by ID with tenant isolation
    /// 
    /// Returns null if:
    /// - Refund not found
    /// - Tenant mismatch (security)
    /// - IsDeleted = true (soft delete)
    /// </summary>
    Task<Refund?> GetRefundAsync(
        Guid tenantId,
        Guid refundId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update refund status with audit trail
    /// Status flow: Initiated → Approved → Processed → Completed
    /// 
    /// Compliance:
    /// - ProcessedAt: Set only when status changes to "Processed"
    /// - CompletedAt: Set only when status changes to "Completed"
    /// - ModifiedAt, ModifiedBy: Always updated
    /// - Immutability: CreatedAt/CreatedBy never modified
    /// </summary>
    Task UpdateRefundStatusAsync(
        Guid tenantId,
        Guid refundId,
        string newStatus,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all refunds for an order
    /// Used for: Tracking multiple withdrawal requests for same order
    /// </summary>
    Task<List<Refund>> GetOrderRefundsAsync(
        Guid tenantId,
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get refunds by status with pagination
    /// Used for: Workflow queries (pending refunds, completed refunds, etc.)
    /// </summary>
    Task<PagedResult<Refund>> GetRefundsByStatusAsync(
        Guid tenantId,
        string status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update refund payment details (bank account, payment reference)
    /// Called after payment is processed via payment gateway
    /// 
    /// Data encrypted: BankAccountNumberEncrypted, BankNameEncrypted
    /// </summary>
    Task UpdateRefundPaymentAsync(
        Guid tenantId,
        Guid refundId,
        string bankAccountEncrypted,
        string bankNameEncrypted,
        string paymentReferenceId,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Archive completed refunds older than 10 years
    /// VVVG compliance: Keep refunds 10 years for tax/audit purposes
    /// Called nightly or via maintenance job
    /// 
    /// Archival Logic:
    /// - Find refunds where CompletedAt < (now - 10 years)
    /// - Set IsArchived = true, ArchivedAt = now
    /// - Do NOT delete (maintain audit trail)
    /// </summary>
    Task ArchiveCompletedRefundsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customer's refund history (for customer portal)
    /// Returns all refunds for customer's orders, non-deleted, sorted by date
    /// </summary>
    Task<List<Refund>> GetCustomerRefundHistoryAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if refund exists for specific return request
    /// Used to prevent duplicate refunds for same return
    /// </summary>
    Task<bool> HasRefundForReturnAsync(
        Guid tenantId,
        Guid returnRequestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate total refund amount for customer (for financial reporting)
    /// Sums RefundAmount for all non-deleted, completed refunds
    /// Filtered by DateRange if provided
    /// </summary>
    Task<decimal> GetTotalRefundAmountAsync(
        Guid tenantId,
        Guid customerId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Pagination helper for list queries
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
