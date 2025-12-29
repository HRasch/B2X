using System;

namespace B2Connect.Returns.Core.Entities;

/// <summary>
/// Return entity representing a customer's withdrawal right within 14-day VVVG period.
/// 
/// VVVG (Verbrauchervertrag-Verjährungsgesetz): German law requiring retailers to
/// accept returns within 14 days from delivery date (NOT order date).
/// 
/// Critical: delivery_date is used, not order_date!
/// </summary>
public class Return
{
    /// <summary>Unique identifier for return</summary>
    public Guid Id { get; set; }

    /// <summary>Multi-tenant isolation - REQUIRED on all queries</summary>
    public Guid TenantId { get; set; }

    /// <summary>Reference to original order</summary>
    public Guid OrderId { get; set; }

    /// <summary>Customer who initiated return</summary>
    public Guid CustomerId { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // Return Status Tracking
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Return status: INITIATED → APPROVED → SHIPPED → REFUNDED or REJECTED
    /// </summary>
    public ReturnStatus Status { get; set; } = ReturnStatus.Initiated;

    /// <summary>Customer's reason for return (e.g., "defective", "wrong size", "changed mind")</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>Number of items being returned from order</summary>
    public int ItemsCount { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // 14-Day Deadline Validation (CRITICAL FOR VVVG COMPLIANCE)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Delivery date of original order.
    /// CRITICAL: This is used to calculate the 14-day withdrawal period, NOT order date!
    /// </summary>
    public DateTime DeliveryDate { get; set; }

    /// <summary>Calculated deadline (DeliveryDate + 14 days). Generated column in database.</summary>
    public DateTime ReturnDeadline { get; set; }

    /// <summary>When customer initiated the return (for audit trail)</summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Is this return within the 14-day legal window?
    /// Generated column: CURRENT_DATE <= delivery_date + 14 days
    /// </summary>
    public bool IsWithinDeadline { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // Refund Processing
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>Amount to refund customer (after deductions if any)</summary>
    public decimal RefundAmount { get; set; }

    /// <summary>Original order amount (for audit trail and verification)</summary>
    public decimal OriginalOrderAmount { get; set; }

    /// <summary>Shipping cost deducted from refund (if applicable)</summary>
    public decimal ShippingDeduction { get; set; } = 0;

    // ═══════════════════════════════════════════════════════════════════════════════
    // Carrier Integration
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>URL to downloadable return shipping label (for customer)</summary>
    public string? ReturnLabelUrl { get; set; }

    /// <summary>Carrier reference/confirmation number for return shipment</summary>
    public string? CarrierReference { get; set; }

    /// <summary>Shipping carrier name (e.g., "DHL", "UPS", "DPD")</summary>
    public string? CarrierName { get; set; }

    /// <summary>Tracking number for return shipment</summary>
    public string? TrackingNumber { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // Audit Trail (REQUIRED for compliance logging)
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>When return record was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When return was completed (refunded/rejected)</summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>User who created the return (audit trail)</summary>
    public Guid CreatedBy { get; set; }

    // ═══════════════════════════════════════════════════════════════════════════════
    // Methods
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Validate return is within 14-day VVVG window.
    /// Throws InvalidOperationException if outside deadline.
    /// </summary>
    public void ValidateWithinDeadline()
    {
        var daysRemaining = (ReturnDeadline.Date - DateTime.UtcNow.Date).TotalDays;
        
        if (daysRemaining < 0)
        {
            throw new InvalidOperationException(
                $"Return period expired on {ReturnDeadline:yyyy-MM-dd}. " +
                $"VVVG requires returns within 14 days of delivery."
            );
        }
    }

    /// <summary>
    /// Calculate days remaining in return window.
    /// Returns 0 or negative if deadline passed.
    /// </summary>
    public int DaysRemaining => 
        (int)(ReturnDeadline.Date - DateTime.UtcNow.Date).TotalDays;

    /// <summary>
    /// Check if return is still eligible (within 14 days and not already processed).
    /// </summary>
    public bool IsEligible => 
        DaysRemaining >= 0 && 
        Status is ReturnStatus.Initiated or ReturnStatus.Approved;

    /// <summary>
    /// Mark return as approved and ready for shipment.
    /// </summary>
    public void Approve(string carrierName, string returnLabelUrl)
    {
        ValidateWithinDeadline();
        Status = ReturnStatus.Approved;
        CarrierName = carrierName;
        ReturnLabelUrl = returnLabelUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark return as received and processed refund.
    /// </summary>
    public void MarkAsRefunded(string carrierReference, string trackingNumber)
    {
        Status = ReturnStatus.Refunded;
        CarrierReference = carrierReference;
        TrackingNumber = trackingNumber;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reject return (e.g., outside deadline or condition check failed).
    /// </summary>
    public void Reject(string rejectionReason)
    {
        Status = ReturnStatus.Rejected;
        Reason = rejectionReason;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Return status enum for state machine tracking.
/// Follows strict progression: INITIATED → APPROVED → SHIPPED → REFUNDED or REJECTED
/// </summary>
public enum ReturnStatus
{
    /// <summary>Return initiated by customer, awaiting approval</summary>
    Initiated = 0,

    /// <summary>Return approved, label generated, awaiting shipment</summary>
    Approved = 1,

    /// <summary>Return shipped by customer (tracking confirmed)</summary>
    Shipped = 2,

    /// <summary>Return received and refund processed</summary>
    Refunded = 3,

    /// <summary>Return rejected (outside deadline or condition issue)</summary>
    Rejected = 4
}
