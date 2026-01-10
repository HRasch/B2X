using B2X.Store.Core.Common.Entities;

namespace B2X.Store.Core.Common.Interfaces;

/// <summary>
/// Repository interface for Country operations
/// Common interface for Country entity management
/// </summary>
public interface ICountryRepository : IRepository<Country>
{
    /// <summary>Gets a country by ISO code</summary>
    Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Gets all active countries</summary>
    Task<ICollection<Country>> GetActiveCountriesAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets countries in a specific region</summary>
    Task<ICollection<Country>> GetCountriesByRegionAsync(string region, CancellationToken cancellationToken = default);

    /// <summary>Gets countries with shipping enabled</summary>
    Task<ICollection<Country>> GetShippingCountriesAsync(CancellationToken cancellationToken = default);
}

