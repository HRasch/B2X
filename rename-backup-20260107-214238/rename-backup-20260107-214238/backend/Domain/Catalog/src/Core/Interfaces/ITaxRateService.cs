namespace B2X.Catalog.Core.Interfaces;

/// <summary>
/// Service contract for tax rate operations
/// Issue #30: B2C Price Transparency
/// </summary>
public interface ITaxRateService
{
    /// <summary>Gets active VAT rate for a country</summary>
    Task<decimal> GetVatRateAsync(string countryCode, CancellationToken ct = default);

    /// <summary>Gets all active tax rates</summary>
    Task<IEnumerable<TaxRateDto>> GetAllRatesAsync(CancellationToken ct = default);

    /// <summary>Creates new tax rate (admin only)</summary>
    Task<TaxRateDto> CreateRateAsync(CreateTaxRateCommand cmd, CancellationToken ct = default);
}

public record TaxRateDto(string CountryCode, string CountryName, decimal StandardVatRate, decimal? ReducedVatRate);
public record CreateTaxRateCommand(string CountryCode, string CountryName, decimal StandardVatRate, decimal? ReducedVatRate);
