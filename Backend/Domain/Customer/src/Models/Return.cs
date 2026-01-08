using System;

namespace B2X.Customer.Models;

/// <summary>
/// ReturnRequest Entity - Represents a customer return/withdrawal request
/// VVVG §357 ff: 14-day withdrawal right for B2C customers
/// </summary>
public class ReturnRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }

    // Return Reference
    public string? ReturnNumber { get; set; } // e.g., "RET-2025-001234"
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    // Return Status
    public string? Status { get; set; } // "Requested", "ReturnLabelSent", "InTransit", "Received", "Refunded", "Rejected"

    // Return Reason
    public string? Reason { get; set; } // User's reason for return

    // Return Items (which items being returned)
    public bool ReturnAllItems { get; set; } = true;
    public string? ReturnedItemsJson { get; set; } // JSON array of item IDs if partial return

    // Return Shipping
    public string? ReturnCarrier { get; set; } // e.g., "DHL", "DPD", "Deutsche Post"
    public string? ReturnTrackingNumber { get; set; }
    public string? ReturnLabelUrl { get; set; }
    public DateTime? ReturnLabelGeneratedAt { get; set; }
    public DateTime? ReturnReceivedAt { get; set; }

    // Refund Information
    public decimal RefundAmount { get; set; } // Amount to be refunded
    public string? RefundStatus { get; set; } // "Pending", "Processed", "Failed"
    public string? RefundTransactionId { get; set; }
    public DateTime? RefundProcessedAt { get; set; }
    public string? RefundMethod { get; set; } // "OriginalPaymentMethod", "CreditNote", "BankTransfer"

    // Compliance & Audit
    public bool IsWithinWithdrawalPeriod { get; set; } // VVVG compliance check
    public int DaysAfterDelivery { get; set; } // How many days after delivery
    public string? AuditNotes { get; set; } // Internal notes for compliance

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Audit Trail
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
}

/// <summary>
/// Refund Entity - Tracks refund transactions for audit/legal purposes (10-year archival)
/// </summary>
public class Refund
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ReturnRequestId { get; set; }
    public Guid OrderId { get; set; }

    // Refund Details
    public decimal RefundAmount { get; set; }
    public string? RefundMethod { get; set; } // "OriginalPaymentMethod", "CreditNote", "BankTransfer"
    public string? RefundTransactionId { get; set; } // Payment gateway reference
    public string? Status { get; set; } // "Pending", "Processed", "Failed", "Cancelled"

    // Refund Reason
    public string? Reason { get; set; } // "Withdrawal", "Return", "Cancellation", "Error"

    // Bank Details (if refund via bank transfer)
    public string? BankAccountEncrypted { get; set; } // Encrypted for security
    public string? BankNameEncrypted { get; set; }

    // Timestamps
    public DateTime ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Audit Trail
    public Guid ProcessedBy { get; set; }
    public string? AuditLog { get; set; } // JSON with all state changes

    // Archival (10-year retention per German law)
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}
