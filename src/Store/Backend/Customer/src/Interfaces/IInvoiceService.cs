using System;
using System.Threading;
using System.Threading.Tasks;
using B2X.Customer.Models;

namespace B2X.Customer.Interfaces;

/// <summary>
/// Interface for invoice generation and modification
/// Issue #32: Invoice Modification for Reverse Charge
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Generate invoice from order
    /// Applies reverse charge if applicable (B2B + valid VAT-ID + different EU country)
    /// </summary>
    Task<Invoice> GenerateInvoiceAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modify existing invoice (for reverse charge adjustments)
    /// </summary>
    Task<Invoice> ModifyInvoiceAsync(Guid invoiceId, Invoice updatedInvoice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoice by order ID
    /// </summary>
    Task<Invoice> GetInvoiceByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoice by invoice number
    /// </summary>
    Task<Invoice> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Apply reverse charge to existing invoice
    /// Called after VAT-ID validation confirms reverse charge eligibility
    /// </summary>
    Task<Invoice> ApplyReverseChargeAsync(Guid invoiceId, string buyerVatId, string buyerCountry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove reverse charge from invoice
    /// Called if VAT-ID validation fails and reverse charge needs to be reverted
    /// </summary>
    Task<Invoice> RemoveReverseChargeAsync(Guid invoiceId, decimal correctTaxRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate invoice PDF
    /// </summary>
    Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate E-Rechnung (ZUGFeRD XML)
    /// Issue #33 requirement
    /// </summary>
    Task<string> GenerateErechnungAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for invoice persistence
/// </summary>
public interface IInvoiceRepository
{
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for invoice PDF generation
/// </summary>
public interface IInvoicePdfGenerator
{
    Task<byte[]> GeneratePdfAsync(Invoice invoice, InvoiceTemplate template, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for E-Rechnung (ZUGFeRD) generation
/// Issue #33
/// </summary>
public interface IErechnungGenerator
{
    Task<string> GenerateZugferdXmlAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
