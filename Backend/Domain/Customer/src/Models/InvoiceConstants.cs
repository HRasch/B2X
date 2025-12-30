namespace B2Connect.Customer.Models;

/// <summary>
/// Constants for invoice statuses and business logic
/// Issue #32: P0.6-US-002 Invoice Generation
/// </summary>
public static class InvoiceStatus
{
    /// <summary>Draft status - invoice not yet issued</summary>
    public const string Draft = "Draft";

    /// <summary>Issued status - invoice formally issued to customer</summary>
    public const string Issued = "Issued";

    /// <summary>Paid status - invoice payment received</summary>
    public const string Paid = "Paid";

    /// <summary>Cancelled status - invoice void and no longer valid</summary>
    public const string Cancelled = "Cancelled";

    /// <summary>Refunded status - invoice has been refunded partially or fully</summary>
    public const string Refunded = "Refunded";
}

/// <summary>
/// Constants for invoice configuration and notes
/// </summary>
public static class InvoiceConfig
{
    /// <summary>Default payment terms in days (Net 30)</summary>
    public const int DefaultPaymentTermsDays = 30;

    /// <summary>Standard EU reverse charge note (Art. 199a Directive 2006/112/EC)</summary>
    public const string ReverseChargeNote = "Reverse Charge: Art. 199a Directive 2006/112/EC";

    /// <summary>Default seller company name (should be loaded from tenant config in production)</summary>
    public const string DefaultSellerName = "B2Connect GmbH";

    /// <summary>Default seller VAT ID (should be loaded from tenant config in production)</summary>
    public const string DefaultSellerVatId = "DE123456789";

    /// <summary>Default seller address (should be loaded from tenant config in production)</summary>
    public const string DefaultSellerAddress = "Somestrasse 123, 10115 Berlin, Germany";
}

/// <summary>
/// Constants for VAT and tax calculations
/// </summary>
public static class TaxConstants
{
    /// <summary>No VAT - used for reverse charge and exempt items</summary>
    public const decimal NoVat = 0m;

    /// <summary>Standard VAT rate for most goods/services in Germany (19%)</summary>
    public const decimal StandardVatRateGermany = 19m;

    /// <summary>Reduced VAT rate for certain goods (e.g., books, food) in Germany (7%)</summary>
    public const decimal ReducedVatRateGermany = 7m;

    /// <summary>EU standard VAT rate varies by country (typically 17-27%)</summary>
    public const decimal EuStandardVatMinimum = 17m;
}
