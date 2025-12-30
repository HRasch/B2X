using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Customer.Interfaces;
using B2Connect.Customer.Models;
using Microsoft.Extensions.Logging;

namespace B2Connect.Customer.Services;

/// <summary>
/// Service for invoice generation and modification
/// Issue #32: Invoice Modification for Reverse Charge
/// 
/// Responsibilities:
/// 1. Generate invoice from order (called after order placed)
/// 2. Apply reverse charge when VAT-ID validation successful
/// 3. Modify invoice if reverse charge status changes
/// 4. Calculate correct VAT based on reverse charge flag
/// 
/// Business Logic:
/// - B2C orders: Normal VAT (19%, 7%, 20%, etc.)
/// - B2B with invalid VAT-ID: Normal VAT
/// - B2B with valid VAT-ID + different EU country: 0% VAT (reverse charge)
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        ILogger<InvoiceService> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<Invoice> GenerateInvoiceAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating invoice for order {OrderId}", orderId);

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            InvoiceNumber = GenerateInvoiceNumber(),
            IssuedAt = DateTime.UtcNow,
            DueAt = DateTime.UtcNow.AddDays(InvoiceConfig.DefaultPaymentTermsDays),
            Status = InvoiceStatus.Draft,

            SellerName = InvoiceConfig.DefaultSellerName,
            SellerVatId = InvoiceConfig.DefaultSellerVatId,
            SellerAddress = InvoiceConfig.DefaultSellerAddress,

            LineItems = new List<InvoiceLineItem>(),
        };

        // Set pricing based on reverse charge flag
        // (Order has ReverseChargeApplied flag set from checkout)
        invoice.SubTotal = 0m; // Will be calculated from line items
        invoice.TaxAmount = invoice.ReverseChargeApplies ? TaxConstants.NoVat : 0m; // Will calculate
        invoice.ShippingCost = 0m; // From order

        if (invoice.ReverseChargeApplies)
        {
            invoice.TaxRate = TaxConstants.NoVat;
            invoice.ReverseChargeNote = InvoiceConfig.ReverseChargeNote;
            _logger.LogInformation("Reverse charge applied to invoice {InvoiceId} for order {OrderId}",
                invoice.Id, orderId);
        }

        invoice.CreatedAt = DateTime.UtcNow;
        invoice.Status = InvoiceStatus.Issued;

        return await _invoiceRepository.AddAsync(invoice, cancellationToken);
    }

    public async Task<Invoice> ModifyInvoiceAsync(Guid invoiceId, Invoice updatedInvoice, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Modifying invoice {InvoiceId}", invoiceId);

        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning("Invoice {InvoiceId} not found", invoiceId);
            throw new InvalidOperationException($"Invoice {invoiceId} not found");
        }

        // Update changeable fields
        invoice.BuyerVatId = updatedInvoice.BuyerVatId ?? invoice.BuyerVatId;
        invoice.BuyerCountry = updatedInvoice.BuyerCountry ?? invoice.BuyerCountry;
        invoice.ReverseChargeApplies = updatedInvoice.ReverseChargeApplies;

        if (updatedInvoice.ReverseChargeApplies)
        {
            invoice.TaxRate = TaxConstants.NoVat;
            invoice.TaxAmount = TaxConstants.NoVat;
            invoice.ReverseChargeNote = InvoiceConfig.ReverseChargeNote;
            _logger.LogInformation("Reverse charge enabled on invoice {InvoiceId}", invoiceId);
        }
        else
        {
            invoice.TaxRate = updatedInvoice.TaxRate;
            invoice.TaxAmount = CalculateTaxAmount(invoice.SubTotal, updatedInvoice.TaxRate);
            invoice.ReverseChargeNote = string.Empty;
            _logger.LogInformation("Reverse charge disabled on invoice {InvoiceId}, tax rate: {TaxRate}%",
                invoiceId, updatedInvoice.TaxRate * 100);
        }

        invoice.ModifiedAt = DateTime.UtcNow;
        invoice.Total = invoice.SubTotal + invoice.TaxAmount + invoice.ShippingCost;

        return await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
    }

    public async Task<Invoice> GetInvoiceByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByOrderIdAsync(orderId, cancellationToken);
        if (invoice == null)
        {
            _logger.LogWarning("Invoice for order {OrderId} not found", orderId);
            throw new InvalidOperationException($"Invoice for order {orderId} not found");
        }

        return invoice;
    }

    public async Task<Invoice> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByInvoiceNumberAsync(invoiceNumber, cancellationToken);
        if (invoice == null)
        {
            _logger.LogWarning("Invoice with number {InvoiceNumber} not found", invoiceNumber);
            throw new InvalidOperationException($"Invoice with number {invoiceNumber} not found");
        }

        return invoice;
    }

    public async Task<Invoice> ApplyReverseChargeAsync(Guid invoiceId, string buyerVatId, string buyerCountry, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Applying reverse charge to invoice {InvoiceId} for VAT-ID {VatId}", invoiceId, buyerVatId);

        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice {invoiceId} not found");
        }

        var oldTaxAmount = invoice.TaxAmount;
        var oldTotal = invoice.Total;

        // Apply reverse charge
        invoice.BuyerVatId = buyerVatId;
        invoice.BuyerCountry = buyerCountry;
        invoice.ReverseChargeApplies = true;
        invoice.TaxRate = 0m;
        invoice.TaxAmount = 0m;
        invoice.ReverseChargeNote = "Reverse Charge: Art. 199a Directive 2006/112/EC";
        invoice.Total = invoice.SubTotal + invoice.ShippingCost; // No tax
        invoice.ModifiedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Reverse charge applied. Tax reduced from {OldTax:F2} to {NewTax:F2}, total from {OldTotal:F2} to {NewTotal:F2}",
            oldTaxAmount, invoice.TaxAmount, oldTotal, invoice.Total);

        return await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
    }

    public async Task<Invoice> RemoveReverseChargeAsync(Guid invoiceId, decimal correctTaxRate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing reverse charge from invoice {InvoiceId}, applying {TaxRate}% tax", invoiceId, correctTaxRate * 100);

        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice {invoiceId} not found");
        }

        var oldTaxAmount = invoice.TaxAmount;
        var oldTotal = invoice.Total;

        // Remove reverse charge and apply correct tax
        invoice.ReverseChargeApplies = false;
        invoice.TaxRate = correctTaxRate;
        invoice.TaxAmount = CalculateTaxAmount(invoice.SubTotal, correctTaxRate);
        invoice.ReverseChargeNote = string.Empty;
        invoice.Total = invoice.SubTotal + invoice.TaxAmount + invoice.ShippingCost;
        invoice.ModifiedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Reverse charge removed. Tax increased from {OldTax:F2} to {NewTax:F2}, total from {OldTotal:F2} to {NewTotal:F2}",
            oldTaxAmount, invoice.TaxAmount, oldTotal, invoice.Total);

        return await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement PDF generation (Issue #32)
        // Use iTextSharp or PdfSharp library
        await Task.Delay(10, cancellationToken);
        return Array.Empty<byte>();
    }

    public async Task<string> GenerateErechnungAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement E-Rechnung XML generation (Issue #33)
        // Generate ZUGFeRD 2.0 compliant XML
        await Task.Delay(10, cancellationToken);
        return string.Empty;
    }

    private string GenerateInvoiceNumber()
    {
        // Format: INV-YYYY-XXXXXX (e.g., INV-2025-001234)
        return $"INV-{DateTime.UtcNow:yyyy}-{new Random().Next(1000, 999999)}";
    }

    private decimal CalculateTaxAmount(decimal subtotal, decimal taxRate)
    {
        return Math.Round(subtotal * taxRate, 2);
    }
}
