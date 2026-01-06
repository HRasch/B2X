
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using B2Connect.Catalog.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Application.Handlers;
/// <summary>
/// Tax rate service implementation with caching
/// Issue #30: B2C Price Transparency (PAngV)
/// </summary>
public class TaxRateService : ITaxRateService
{
    private readonly ITaxRateRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TaxRateService> _logger;
    private const string CACHE_KEY_PREFIX = "taxrate_";
    private const string CACHE_KEY_ALL = "taxrates_all";
    private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(24);

    public TaxRateService(
        ITaxRateRepository repository,
        IMemoryCache cache,
        ILogger<TaxRateService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Gets active VAT rate for a country with caching
    /// </summary>
    public async Task<decimal> GetVatRateAsync(string countryCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            _logger.LogWarning("Empty country code provided to GetVatRateAsync");
            throw new ArgumentException("Country code cannot be empty", nameof(countryCode));
        }

        var normalizedCode = countryCode.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
        var cacheKey = $"{CACHE_KEY_PREFIX}{normalizedCode}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
        {
            _logger.LogDebug("Tax rate retrieved from cache: {CountryCode} = {Rate}%", normalizedCode, cachedRate);
            return cachedRate;
        }

        // Get from database
        var taxRate = await _repository.GetByCountryCodeAsync(normalizedCode, ct);

        if (taxRate == null)
        {
            _logger.LogWarning("No tax rate found for country: {CountryCode}, using default 19%", normalizedCode);
            // Fallback to Germany's VAT rate (19%)
            const decimal defaultRate = 19.00m;
            _cache.Set(cacheKey, defaultRate, CACHE_DURATION);
            return defaultRate;
        }

        // Cache the rate
        _cache.Set(cacheKey, taxRate.StandardVatRate, CACHE_DURATION);
        _logger.LogInformation("Tax rate loaded and cached: {CountryCode} = {Rate}%", normalizedCode, taxRate.StandardVatRate);

        return taxRate.StandardVatRate;
    }

    /// <summary>
    /// Gets all active tax rates
    /// </summary>
    public async Task<IEnumerable<TaxRateDto>> GetAllRatesAsync(CancellationToken ct = default)
    {
        // Try to get from cache
        if (_cache.TryGetValue(CACHE_KEY_ALL, out IEnumerable<TaxRateDto>? cachedRates))
        {
            return cachedRates ?? Enumerable.Empty<TaxRateDto>();
        }

        // Get from database
        var rates = await _repository.GetAllActiveAsync(ct);

        var dtos = rates.Select(r => new TaxRateDto(
            r.CountryCode,
            r.CountryCode, // Using country code as name temporarily
            r.StandardVatRate,
            r.ReducedVatRate
        )).ToList();

        // Cache the results
        _cache.Set(CACHE_KEY_ALL, (IEnumerable<TaxRateDto>)dtos, CACHE_DURATION);
        _logger.LogInformation("Loaded {Count} active tax rates from database", dtos.Count);

        return dtos;
    }

    /// <summary>
    /// Creates a new tax rate (admin function)
    /// </summary>
    public async Task<TaxRateDto> CreateRateAsync(CreateTaxRateCommand cmd, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        // Create entity
        var taxRate = new B2Connect.Catalog.Core.Entities.TaxRate
        {
            CountryCode = cmd.CountryCode,
            StandardVatRate = cmd.StandardVatRate,
            ReducedVatRate = cmd.ReducedVatRate,
            EffectiveDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Save to database
        await _repository.AddAsync(taxRate, ct);

        // Invalidate cache
        InvalidateCache(cmd.CountryCode);

        _logger.LogInformation("New tax rate created: {CountryCode} - {Rate}%", cmd.CountryCode, cmd.StandardVatRate);

        return new TaxRateDto(
            taxRate.CountryCode,
            cmd.CountryName,
            taxRate.StandardVatRate,
            taxRate.ReducedVatRate
        );
    }

    /// <summary>
    /// Invalidates cache for a specific country
    /// </summary>
    private void InvalidateCache(string countryCode)
    {
        _cache.Remove($"{CACHE_KEY_PREFIX}{countryCode.ToUpper(System.Globalization.CultureInfo.CurrentCulture)}");
        _cache.Remove(CACHE_KEY_ALL);
        _logger.LogDebug("Cache invalidated for country: {CountryCode}", countryCode);
    }
}
