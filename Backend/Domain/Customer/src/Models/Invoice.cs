using System;
using System.Collections.Generic;

namespace B2X.Customer.Models;

/// <summary>
/// Invoice Entity - Document generated from order
/// Issue #32: Invoice Modification for Reverse Charge
///
/// When reverse charge applies (B2B, valid VAT-ID, different EU country):
/// - Invoice shows: "Reverse Charge: Art. 199a Directive 2006/112/EC"
/// - VAT line shows: 0% VAT
/// - Bottom note: "Reverse Charge - Customer responsible for VAT"
///
/// Legal reference: PAngV (B2C) + AStV (B2B Reverse Charge)
/// </summary>
public class Invoice
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    // Invoice Reference
    public string InvoiceNumber { get; set; } = string.Empty; // e.g., "INV-2025-001234"
    public DateTime IssuedAt { get; set; }
    public DateTime DueAt { get; set; } // Net 30 days

    // Invoice Status
    public string Status { get; set; } = "Draft"; // "Draft", "Issued", "Paid", "Cancelled"

    // Parties
    public string SellerName { get; set; } = string.Empty; // "B2X GmbH"
    public string SellerVatId { get; set; } = string.Empty; // "DE123456789"
    public string SellerAddress { get; set; } = string.Empty;

    public string BuyerName { get; set; } = string.Empty; // Customer/Company name (encrypted)
    public string BuyerNameEncrypted { get; set; } = string.Empty;
    public string BuyerVatId { get; set; } = string.Empty; // For B2B only
    public string BuyerAddress { get; set; } = string.Empty; // Encrypted
    public string BuyerAddressEncrypted { get; set; } = string.Empty;
    public string BuyerCountry { get; set; } = string.Empty; // For reverse charge determination

    // Order Summary
    public List<InvoiceLineItem> LineItems { get; set; } = new();

    // Pricing Breakdown
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; } // 0 if reverse charge applies
    public decimal TaxRate { get; set; } // 19%, 7%, 20%, etc. or 0% for reverse charge
    public decimal ShippingCost { get; set; }
    public decimal ShippingTaxAmount { get; set; }
    public decimal Total { get; set; }

    // Reverse Charge
    public bool ReverseChargeApplies { get; set; }
    public string ReverseChargeNote { get; set; } = string.Empty; // "Reverse Charge: Art. 199a Directive 2006/112/EC"

    // Payment Terms
    public string PaymentMethod { get; set; } = string.Empty; // "Credit Card", "Bank Transfer", "PayPal"
    public string PaymentStatus { get; set; } = string.Empty; // "Pending", "Received", "Overdue"
    public DateTime? PaidAt { get; set; }

    // Document
    public byte[]? PdfContent { get; set; } // Invoice PDF (encrypted at rest)
    public string PdfHash { get; set; } = string.Empty; // SHA256 for integrity check
    public DateTime? PdfGeneratedAt { get; set; }

    // E-Rechnung
    public byte[]? XmlContent { get; set; } // ZUGFeRD 2.0 XML (for E-Rechnung)
    public bool IsErechnung { get; set; } // Issue #33 requirement
    public DateTime? XmlGeneratedAt { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty; // User ID
    public DateTime? ModifiedAt { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class InvoiceLineItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    // Product Reference
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    // Line Item
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineSubTotal { get; set; } // Quantity × UnitPrice
    public decimal LineTaxAmount { get; set; } // 0 if reverse charge
    public decimal LineTaxRate { get; set; }
    public decimal LineTotal { get; set; } // SubTotal + Tax

    // Audit
    public DateTime CreatedAt { get; set; }
}

public class InvoiceTemplate
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    // Template Info
    public string Name { get; set; } = string.Empty; // "Standard B2C", "B2B with Reverse Charge"
    public bool IsDefault { get; set; }

    // Branding
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyVatId { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyLogo { get; set; } = string.Empty; // Base64 encoded
    public string CompanyPhone { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyWebsite { get; set; } = string.Empty;

    // Footer
    public string FooterText { get; set; } = string.Empty; // Bank details, legal info
    public string PaymentInstructions { get; set; } = string.Empty;
    public string DeliveryNotes { get; set; } = string.Empty;

    // Tax/Compliance
    public string TaxNoteB2c { get; set; } = string.Empty; // "incl. VAT"
    public string TaxNoteB2b { get; set; } = string.Empty; // "Reverse Charge: Art. 199a Directive 2006/112/EC"

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
