
using B2Connect.CatalogService.Models;

namespace B2Connect.Catalog.Core.Interfaces;
public interface ITaxRateRepository
{
    Task<TaxRate?> GetByCountryCodeAsync(string countryCode, CancellationToken ct = default);
    Task<IEnumerable<TaxRate>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(TaxRate taxRate, CancellationToken ct = default);
    Task UpdateAsync(TaxRate taxRate, CancellationToken ct = default);
}
