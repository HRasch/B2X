namespace B2Connect.Returns.Application.Commands;

/// <summary>
/// Command to initiate a customer return within the 14-day VVVG withdrawal period.
/// 
/// This is a plain POCO command (not IRequest) used by Wolverine handler pattern.
/// </summary>
public class CreateReturnCommand
{
    /// <summary>Multi-tenant isolation key</summary>
    public Guid TenantId { get; set; }

    /// <summary>Order being returned</summary>
    public Guid OrderId { get; set; }

    /// <summary>Customer initiating return</summary>
    public Guid CustomerId { get; set; }

    /// <summary>User making the request (audit trail)</summary>
    public Guid CreatedBy { get; set; }

    /// <summary>When the order was delivered (CRITICAL: NOT order date!)</summary>
    public DateTime DeliveryDate { get; set; }

    /// <summary>Number of items to return from this order</summary>
    public int ItemsCount { get; set; }

    /// <summary>Customer's reason for return (defective, wrong size, etc.)</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>Original order total amount (for audit/verification)</summary>
    public decimal OriginalOrderAmount { get; set; }

    /// <summary>Refund amount to process (after any deductions)</summary>
    public decimal RefundAmount { get; set; }

    /// <summary>Shipping cost to deduct from refund (if applicable)</summary>
    public decimal ShippingDeduction { get; set; } = 0;
}

/// <summary>
/// Response after successfully creating a return.
/// </summary>
public class CreateReturnResponse
{
    /// <summary>Unique identifier for the return</summary>
    public Guid ReturnId { get; set; }

    /// <summary>Current status of return</summary>
    public string Status { get; set; } = "INITIATED";

    /// <summary>When return can no longer be modified (14 days from delivery)</summary>
    public DateTime ReturnDeadline { get; set; }

    /// <summary>Days remaining to complete return (calculated field)</summary>
    public int DaysRemaining { get; set; }

    /// <summary>Refund amount that will be processed</summary>
    public decimal RefundAmount { get; set; }

    /// <summary>Message for customer</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Success indicator</summary>
    public bool Success { get; set; } = true;
}
