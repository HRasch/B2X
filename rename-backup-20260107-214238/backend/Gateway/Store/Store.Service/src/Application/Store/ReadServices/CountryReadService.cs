using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;

namespace B2X.Store.Application.Store.ReadServices;

/// <summary>
/// Read-only service for Country queries
/// Optimized for public API access with no write operations
/// </summary>
public interface ICountryReadService
{
    Task<Country?> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<Country?> GetCountryByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetCountriesByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<ICollection<Country>> GetShippingCountriesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<string>> GetAvailableRegionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountryCountAsync(CancellationToken cancellationToken = default);
}

public class CountryReadService : ICountryReadService
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<CountryReadService> _logger;

    public CountryReadService(ICountryRepository countryRepository, ILogger<CountryReadService> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<Country?> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching country by ID: {CountryId}", countryId);
        return await _countryRepository.GetByIdAsync(countryId, cancellationToken);
    }

    public async Task<Country?> GetCountryByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching country by code: {Code}", code);
        var country = await _countryRepository.GetByCodeAsync(code, cancellationToken);

        if (country == null)
            _logger.LogWarning("Country not found with code: {Code}", code);

        return country;
    }

    public async Task<ICollection<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all active countries");
        return await _countryRepository.GetActiveCountriesAsync(cancellationToken);
    }

    public async Task<ICollection<Country>> GetCountriesByRegionAsync(string region, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching countries in region: {Region}", region);
        return await _countryRepository.GetCountriesByRegionAsync(region, cancellationToken);
    }

    public async Task<ICollection<Country>> GetShippingCountriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching countries with shipping enabled");
        return await _countryRepository.GetShippingCountriesAsync(cancellationToken);
    }

    public async Task<ICollection<string>> GetAvailableRegionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching available regions");
        var countries = await _countryRepository.GetActiveCountriesAsync(cancellationToken);

        return countries
            .Where(c => !string.IsNullOrWhiteSpace(c.Region))
            .Select(c => c.Region!)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    public async Task<int> GetCountryCountAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Counting active countries");
        return await _countryRepository.CountAsync(cancellationToken);
    }
}


