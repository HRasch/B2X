using System;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Customer.Interfaces;
using B2Connect.Customer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2Connect.Customer.Data;

/// <summary>
/// Repository for Invoice persistence (EF Core)
/// Issue #32: Invoice Modification for Reverse Charge
/// </summary>
public class InvoiceRepository : IInvoiceRepository
{
    private readonly CustomerDbContext _context;
    private readonly ILogger<InvoiceRepository> _logger;

    public InvoiceRepository(CustomerDbContext context, ILogger<InvoiceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding invoice {InvoiceNumber} for order {OrderId}",
            invoice.InvoiceNumber, invoice.OrderId);

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return invoice;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating invoice {InvoiceNumber}", invoice.InvoiceNumber);

        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return invoice;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Nullable", "CS8613")]
    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.LineItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Nullable", "CS8613")]
    public async Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.LineItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.OrderId == orderId && !i.IsDeleted, cancellationToken);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Nullable", "CS8613")]
    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Include(i => i.LineItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting invoice {InvoiceId}", id);

        var invoice = await _context.Invoices.FindAsync(new object[] { id }, cancellationToken);
        if (invoice != null)
        {
            invoice.IsDeleted = true;
            invoice.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
