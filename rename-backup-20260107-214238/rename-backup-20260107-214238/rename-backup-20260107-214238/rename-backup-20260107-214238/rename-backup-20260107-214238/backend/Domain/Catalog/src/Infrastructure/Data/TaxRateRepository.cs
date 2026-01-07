
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using B2Connect.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Infrastructure.Data;

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
            .Where(x => x.CountryCode.Equals(countryCode, StringComparison.CurrentCultureIgnoreCase) &&
                       x.EffectiveDate <= DateTime.UtcNow &&
                       (!x.EndDate.HasValue || x.EndDate > DateTime.UtcNow))
            .FirstOrDefaultAsync(ct).ConfigureAwait(false);

        if (result == null)
        {
            _logger.LogWarning("No active tax rate found for country: {CountryCode}", countryCode);
        }

        return result;
    }

    public async Task<IEnumerable<TaxRate>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.TaxRates
            .Where(x => x.EffectiveDate <= DateTime.UtcNow &&
                       (!x.EndDate.HasValue || x.EndDate > DateTime.UtcNow))
            .OrderBy(x => x.CountryCode)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task AddAsync(TaxRate taxRate, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(taxRate);

        await _context.TaxRates.AddAsync(taxRate, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation("Tax rate added: {CountryCode} - {Rate}%",
            taxRate.CountryCode, taxRate.StandardVatRate);
    }

    public async Task UpdateAsync(TaxRate taxRate, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(taxRate);

        taxRate.UpdatedAt = DateTime.UtcNow;
        _context.TaxRates.Update(taxRate);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogInformation("Tax rate updated: {CountryCode} - {Rate}%",
            taxRate.CountryCode, taxRate.StandardVatRate);
    }
}

