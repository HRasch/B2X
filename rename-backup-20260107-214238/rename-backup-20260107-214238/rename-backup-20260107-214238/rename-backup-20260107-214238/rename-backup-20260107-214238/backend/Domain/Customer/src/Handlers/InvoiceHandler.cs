using System;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Customer.Interfaces;
using B2Connect.Customer.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace B2Connect.Customer.Handlers;

/// <summary>
/// Wolverine HTTP Handler: Generate or modify invoices
/// Issue #32: Invoice Modification for Reverse Charge
///
/// Endpoints (auto-discovered by Wolverine):
/// - POST /generateinvoice → GenerateInvoiceAsync
/// - POST /modifyinvoice → ModifyInvoiceAsync
///
/// Called after:
/// - Order placed (generate initial invoice)
/// - VAT-ID validation completes (apply/remove reverse charge)
/// </summary>
public class InvoiceHandler
{
    private readonly IInvoiceService _invoiceService;
    private readonly IValidator<GenerateInvoiceCommand> _generateValidator;
    private readonly IValidator<ModifyInvoiceCommand> _modifyValidator;
    private readonly ILogger<InvoiceHandler> _logger;

    public InvoiceHandler(
        IInvoiceService invoiceService,
        IValidator<GenerateInvoiceCommand> generateValidator,
        IValidator<ModifyInvoiceCommand> modifyValidator,
        ILogger<InvoiceHandler> logger)
    {
        _invoiceService = invoiceService;
        _generateValidator = generateValidator;
        _modifyValidator = modifyValidator;
        _logger = logger;
    }

    /// <summary>
    /// Generate invoice from order
    /// POST /generateinvoice
    /// </summary>
    public async Task<GenerateInvoiceResponse> GenerateInvoice(
        GenerateInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating invoice for order {OrderId}", request.OrderId);

        try
        {
            // Validate input
            var validationResult = await _generateValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invoice generation validation failed for order {OrderId}", request.OrderId);
                return new GenerateInvoiceResponse
                {
                    Success = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            // Generate invoice
            var invoice = await _invoiceService.GenerateInvoiceAsync(request.OrderId, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Invoice {InvoiceNumber} generated for order {OrderId}, reverse charge: {ReverseCharge}",
                invoice.InvoiceNumber, request.OrderId, invoice.ReverseChargeApplies);

            return new GenerateInvoiceResponse
            {
                Success = true,
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                ReverseChargeApplied = invoice.ReverseChargeApplies,
                TaxAmount = invoice.TaxAmount,
                Message = $"Invoice {invoice.InvoiceNumber} generated"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating invoice for order {OrderId}", request.OrderId);
            return new GenerateInvoiceResponse
            {
                Success = false,
                Message = "Error generating invoice: " + ex.Message
            };
        }
    }

    /// <summary>
    /// Modify invoice (apply or remove reverse charge)
    /// POST /modifyinvoice
    ///
    /// Called when:
    /// - VAT-ID validation completes successfully → ApplyReverseCharge
    /// - VAT-ID validation fails → RemoveReverseCharge (revert to normal VAT)
    /// </summary>
    public async Task<ModifyInvoiceResponse> ModifyInvoice(
        ModifyInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Modifying invoice for order {OrderId}, apply reverse charge: {ApplyReverseCharge}",
            request.OrderId, request.ApplyReverseCharge);

        try
        {
            // Validate input
            var validationResult = await _modifyValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invoice modification validation failed for order {OrderId}", request.OrderId);
                return new ModifyInvoiceResponse
                {
                    Success = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            // Get invoice by order ID
            var invoice = await _invoiceService.GetInvoiceByOrderIdAsync(request.OrderId, cancellationToken).ConfigureAwait(false);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice not found for order {OrderId}", request.OrderId);
                return new ModifyInvoiceResponse
                {
                    Success = false,
                    Message = $"No invoice found for order {request.OrderId}"
                };
            }

            var oldTaxAmount = invoice.TaxAmount;
            var oldTotal = invoice.Total;

            // Apply or remove reverse charge
            if (request.ApplyReverseCharge)
            {
                invoice = await _invoiceService.ApplyReverseChargeAsync(
                    invoice.Id,
                    request.BuyerVatId,
                    request.BuyerCountry,
                    cancellationToken).ConfigureAwait(false);

                _logger.LogInformation(
                    "Reverse charge applied to invoice {InvoiceNumber}. Tax: {OldTax:F2} → {NewTax:F2}, Total: {OldTotal:F2} → {NewTotal:F2}",
                    invoice.InvoiceNumber, oldTaxAmount, invoice.TaxAmount, oldTotal, invoice.Total);
            }
            else
            {
                // Revert to normal VAT (default 19% for Germany)
                const decimal correctTaxRate = 0.19m; // TODO: Get from TaxRateService based on country
                invoice = await _invoiceService.RemoveReverseChargeAsync(invoice.Id, correctTaxRate, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation(
                    "Reverse charge removed from invoice {InvoiceNumber}. Tax: {OldTax:F2} → {NewTax:F2}, Total: {OldTotal:F2} → {NewTotal:F2}",
                    invoice.InvoiceNumber, oldTaxAmount, invoice.TaxAmount, oldTotal, invoice.Total);
            }

            return new ModifyInvoiceResponse
            {
                Success = true,
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                ReverseChargeApplies = invoice.ReverseChargeApplies,
                OldTaxAmount = oldTaxAmount,
                NewTaxAmount = invoice.TaxAmount,
                OldTotal = oldTotal,
                NewTotal = invoice.Total,
                Message = request.ApplyReverseCharge
                    ? $"Reverse charge applied. VAT: {oldTaxAmount:F2} → {invoice.TaxAmount:F2}"
                    : $"Reverse charge removed. VAT: {oldTaxAmount:F2} → {invoice.TaxAmount:F2}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error modifying invoice for order {OrderId}", request.OrderId);
            return new ModifyInvoiceResponse
            {
                Success = false,
                Message = "Error modifying invoice: " + ex.Message
            };
        }
    }
}
