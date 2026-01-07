using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Common.Interfaces;

namespace B2Connect.Store.Application.Store.Services;

public interface ICountryService
{
    Task<Country?> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<Country?> GetCountryByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetAllActiveCountriesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetCountriesByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetShippingCountriesAsync(CancellationToken cancellationToken = default);
    Task<Country> CreateCountryAsync(Country country, CancellationToken cancellationToken = default);
    Task<Country> UpdateCountryAsync(Country country, CancellationToken cancellationToken = default);
    Task DeleteCountryAsync(Guid countryId, CancellationToken cancellationToken = default);
}

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<Country?> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await _countryRepository.GetByIdAsync(countryId, cancellationToken);
    }

    public async Task<Country?> GetCountryByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _countryRepository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<ICollection<Country>> GetAllActiveCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _countryRepository.GetActiveCountriesAsync(cancellationToken);
    }

    public async Task<ICollection<Country>> GetCountriesByRegionAsync(string region, CancellationToken cancellationToken = default)
    {
        return await _countryRepository.GetCountriesByRegionAsync(region, cancellationToken);
    }

    public async Task<ICollection<Country>> GetShippingCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _countryRepository.GetShippingCountriesAsync(cancellationToken);
    }

    public async Task<Country> CreateCountryAsync(Country country, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(country.Code) || country.Code.Length != 2)
            throw new ArgumentException("Country code must be a 2-character ISO code", nameof(country.Code));

        var existing = await _countryRepository.GetByCodeAsync(country.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Country with code '{country.Code}' already exists");

        country.Id = Guid.NewGuid();
        country.CreatedAt = DateTime.UtcNow;
        country.UpdatedAt = DateTime.UtcNow;
        country.Code = country.Code.ToUpper();

        await _countryRepository.AddAsync(country, cancellationToken);
        return country;
    }

    public async Task<Country> UpdateCountryAsync(Country country, CancellationToken cancellationToken = default)
    {
        var existing = await _countryRepository.GetByIdAsync(country.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException($"Country with ID '{country.Id}' not found");

        country.UpdatedAt = DateTime.UtcNow;
        country.CreatedAt = existing.CreatedAt;
        country.Code = country.Code.ToUpper();

        await _countryRepository.UpdateAsync(country, cancellationToken);
        return country;
    }

    public async Task DeleteCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        var country = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
        if (country == null)
            throw new InvalidOperationException($"Country with ID '{countryId}' not found");

        await _countryRepository.DeleteAsync(country, cancellationToken);
    }
}


