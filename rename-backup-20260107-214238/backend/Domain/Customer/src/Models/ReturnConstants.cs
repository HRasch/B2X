namespace B2X.Customer.Models;

/// <summary>
/// Constants for return request statuses and business logic
/// Issue #32: P0.6-US-003 Withdrawal & Returns (VVVG §357 ff)
/// </summary>
public static class ReturnStatus
{
    /// <summary>Return request created but not yet processed</summary>
    public const string Requested = "Requested";

    /// <summary>Return approved and return label provided</summary>
    public const string Approved = "Approved";

    /// <summary>Return shipped by customer</summary>
    public const string Shipped = "Shipped";

    /// <summary>Return received by seller</summary>
    public const string Received = "Received";

    /// <summary>Items inspected and accepted</summary>
    public const string Accepted = "Accepted";

    /// <summary>Items inspected but rejected (not eligible for return)</summary>
    public const string Rejected = "Rejected";

    /// <summary>Refund processed to customer</summary>
    public const string Refunded = "Refunded";

    /// <summary>Return request cancelled</summary>
    public const string Cancelled = "Cancelled";
}

/// <summary>
/// Constants for refund methods and processing
/// </summary>
public static class RefundMethod
{
    /// <summary>Refund to original payment method (credit card, bank transfer, etc.)</summary>
    public const string OriginalPaymentMethod = "OriginalPaymentMethod";

    /// <summary>Refund as store credit</summary>
    public const string StoreCredit = "StoreCredit";

    /// <summary>Refund by bank transfer</summary>
    public const string BankTransfer = "BankTransfer";
}

/// <summary>
/// Constants for return request validation
/// </summary>
public static class ReturnValidation
{
    /// <summary>VVVG §357: Standard withdrawal period in days (14 days from delivery)</summary>
    public const int WithdrawalPeriodDays = 14;

    /// <summary>Business rule: cannot return without order</summary>
    public const string ErrorOrderNotFound = "Order not found";

    /// <summary>Business rule: cannot return outside withdrawal period</summary>
    public const string ErrorWithdrawalPeriodExpired = "Withdrawal period has expired (14 days from delivery)";

    /// <summary>Business rule: cannot return items in damaged condition</summary>
    public const string ErrorItemsDamaged = "Items are in unacceptable condition for return";

    /// <summary>Business rule: return already exists for this order</summary>
    public const string ErrorReturnAlreadyExists = "Return request already exists for this order";
}

/// <summary>
/// Constants for return label generation
/// </summary>
public static class ReturnLabel
{
    /// <summary>Standard carrier for returns (DHL, DPD, GLS, etc.)</summary>
    public const string DefaultCarrier = "DHL";

    /// <summary>Default service level for return shipping</summary>
    public const string DefaultServiceLevel = "Standard";
}

/// <summary>
/// Constants for return shipping and logistics
/// </summary>
public static class ReturnShipping
{
    /// <summary>Standard return label format (PDF)</summary>
    public const string LabelFormatPdf = "pdf";

    /// <summary>Standard return label format (PNG for printing)</summary>
    public const string LabelFormatPng = "png";

    /// <summary>Standard return label format (ZPL for thermal printers)</summary>
    public const string LabelFormatZpl = "zpl";
}
