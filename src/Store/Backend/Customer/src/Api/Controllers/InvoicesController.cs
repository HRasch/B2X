using B2X.Customer.Interfaces;
using B2X.Customer.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

#pragma warning disable S6960 // This controller has multiple responsibilities and could be split into 2 smaller controllers

namespace B2X.Customer.Api.Controllers;

/// <summary>
/// Customer API controller using CQRS pattern with Wolverine
/// Handles invoice CRUD operations and customer management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMessageBus _bus;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService invoiceService,
        IInvoiceRepository invoiceRepository,
        IMessageBus bus,
        ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets an invoice by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInvoice(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Invoice not found" });
            }

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", id);
            return BadRequest(new { Message = "Failed to retrieve invoice", Error = ex.Message });
        }
    }

    /// <summary>
    /// Gets invoices by tenant with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetInvoices(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] string? status,
        [FromQuery] Guid? orderId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var (items, totalCount) = await _invoiceService.GetInvoicesByTenantAsync(tenantId, page, pageSize);

            return Ok(new
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices for tenant {TenantId}", tenantId);
            return BadRequest(new { Message = "Failed to retrieve invoices", Error = ex.Message });
        }
    }

    /// <summary>
    /// Creates an invoice from an order
    /// </summary>
    [HttpPost("from-order/{orderId:guid}")]
    public async Task<IActionResult> CreateInvoiceFromOrder(
        Guid orderId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var invoice = await _invoiceService.GenerateInvoiceAsync(orderId);
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice from order {OrderId}", orderId);
            return BadRequest(new { Message = "Failed to create invoice", Error = ex.Message });
        }
    }

    /// <summary>
    /// Updates invoice status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateInvoiceStatus(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] UpdateInvoiceStatusRequest request)
    {
        try
        {
            var invoice = await _invoiceService.UpdateInvoiceStatusAsync(id, tenantId, request.Status);
            return Ok(invoice);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Invoice not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice status for invoice {InvoiceId}", id);
            return BadRequest(new { Message = "Failed to update invoice status", Error = ex.Message });
        }
    }

    /// <summary>
    /// Applies reverse charge to an invoice
    /// </summary>
    [HttpPost("{id:guid}/apply-reverse-charge")]
    public async Task<IActionResult> ApplyReverseCharge(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] ApplyReverseChargeRequest request)
    {
        try
        {
            var invoice = await _invoiceService.ApplyReverseChargeAsync(
                id, request.VatId, request.CountryCode);

            return Ok(invoice);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying reverse charge to invoice {InvoiceId}", id);
            return BadRequest(new { Message = "Failed to apply reverse charge", Error = ex.Message });
        }
    }

    /// <summary>
    /// Cancels an invoice
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelInvoice(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] CancelInvoiceRequest request)
    {
        try
        {
            var invoice = await _invoiceService.CancelInvoiceAsync(id, tenantId, request.Reason);
            return Ok(invoice);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Invoice not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling invoice {InvoiceId}", id);
            return BadRequest(new { Message = "Failed to cancel invoice", Error = ex.Message });
        }
    }

    /// <summary>
    /// Gets invoice PDF (placeholder - would integrate with PDF generation service)
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetInvoicePdf(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Invoice not found" });
            }

            // TODO: Integrate with PDF generation service
            // For now, return placeholder
            return Ok(new { Message = "PDF generation not yet implemented", InvoiceId = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF for invoice {InvoiceId}", id);
            return BadRequest(new { Message = "Failed to generate PDF", Error = ex.Message });
        }
    }

    /// <summary>
    /// Validates VAT ID for reverse charge eligibility
    /// </summary>
    [HttpPost("validate-vat-id")]
    public async Task<IActionResult> ValidateVatId([FromBody] ValidateVatIdRequest request)
    {
        try
        {
            var isValid = await _invoiceService.ValidateVatIdAsync(request.VatId, request.CountryCode);
            return Ok(new { IsValid = isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating VAT ID {VatId}", request.VatId);
            return BadRequest(new { Message = "Failed to validate VAT ID", Error = ex.Message });
        }
    }
}

/// <summary>
/// Request DTOs for invoice operations
/// </summary>
public record CreateInvoiceFromOrderRequest(bool ApplyReverseCharge = false);
public record UpdateInvoiceStatusRequest(string Status);
public record ApplyReverseChargeRequest(string VatId, string CountryCode);
public record CancelInvoiceRequest(string Reason);
public record ValidateVatIdRequest(string VatId, string CountryCode);
