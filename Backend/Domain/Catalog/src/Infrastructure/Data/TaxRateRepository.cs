namespace B2Connect.CatalogService.Infrastructure.Data;

using B2Connect.CatalogService.Models;
using B2Connect.Catalog.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class TaxRateRepository : ITaxRateRepository
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<TaxRateRepository> _logger;

    public TaxRateRepository(CatalogDbContext context, ILogger<TaxRateRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaxRate?> GetByCountryCodeAsync(string countryCode, CancellationToken ct = default)
    {
        var result = await _context.TaxRates
            .Where(x => x.CountryCode == countryCode.ToUpper() && x.IsActive)
            .FirstOrDefaultAsync(ct);

        if (result == null)
        {
            _logger.LogWarning("No active tax rate found for country: {CountryCode}", countryCode);
        }

        return result;
    }

    public async Task<IEnumerable<TaxRate>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.TaxRates
            .Where(x => x.IsActive)
            .OrderBy(x => x.CountryCode)
            .ToListAsync(ct);
    }

    public async Task AddAsync(TaxRate taxRate, CancellationToken ct = default)
    {
        if (taxRate == null) throw new ArgumentNullException(nameof(taxRate));

        await _context.TaxRates.AddAsync(taxRate, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Tax rate added: {CountryCode} - {Rate}%",
            taxRate.CountryCode, taxRate.StandardVatRate);
    }

    public async Task UpdateAsync(TaxRate taxRate, CancellationToken ct = default)
    {
        if (taxRate == null) throw new ArgumentNullException(nameof(taxRate));

        taxRate.UpdatedAt = DateTime.UtcNow;
        _context.TaxRates.Update(taxRate);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Tax rate updated: {CountryCode} - {Rate}%",
            taxRate.CountryCode, taxRate.StandardVatRate);
    }
}

