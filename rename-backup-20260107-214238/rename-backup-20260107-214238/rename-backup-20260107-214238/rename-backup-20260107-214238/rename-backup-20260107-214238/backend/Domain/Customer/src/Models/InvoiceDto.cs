using System;
using System.Collections.Generic;

namespace B2X.Customer.Models;

/// <summary>
/// Command: Generate invoice from order
/// Issue #32: Invoice Modification for Reverse Charge
/// </summary>
public class GenerateInvoiceCommand
{
    public Guid OrderId { get; set; }
}

/// <summary>
/// Response: Invoice generated successfully
/// </summary>
public class GenerateInvoiceResponse
{
    public bool Success { get; set; }
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public bool ReverseChargeApplied { get; set; }
    public decimal TaxAmount { get; set; } // 0 if reverse charge
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Command: Modify invoice (apply/remove reverse charge)
/// </summary>
public class ModifyInvoiceCommand
{
    public Guid InvoiceId { get; set; }
    public Guid OrderId { get; set; }
    public bool ApplyReverseCharge { get; set; }
    public string BuyerVatId { get; set; } = string.Empty;
    public string BuyerCountry { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty; // "VAT-ID validation completed", etc.
}

/// <summary>
/// Response: Invoice modified
/// </summary>
public class ModifyInvoiceResponse
{
    public bool Success { get; set; }
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public bool ReverseChargeApplies { get; set; }
    public decimal OldTaxAmount { get; set; }
    public decimal NewTaxAmount { get; set; }
    public decimal OldTotal { get; set; }
    public decimal NewTotal { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// DTO: Invoice summary for display
/// </summary>
public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime DueAt { get; set; }

    public string BuyerName { get; set; } = string.Empty;
    public string BuyerVatId { get; set; } = string.Empty;
    public string BuyerCountry { get; set; } = string.Empty;

    public List<InvoiceLineItemDto> LineItems { get; set; } = new();

    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }

    public bool ReverseChargeApplies { get; set; }
    public string ReverseChargeNote { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public bool HasPdf { get; set; }
    public bool HasErechnungXml { get; set; }
}

/// <summary>
/// DTO: Invoice line item for display
/// </summary>
public class InvoiceLineItemDto
{
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineSubTotal { get; set; }
    public decimal LineTaxAmount { get; set; }
    public decimal LineTaxRate { get; set; }
    public decimal LineTotal { get; set; }
}
