using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Common.Interfaces;

namespace B2Connect.Store.Application.Store.ReadServices;

/// <summary>
/// Read-only service for Shop queries
/// Optimized for public API access with no write operations
/// </summary>
public interface IShopReadService
{
    Task<Shop?> GetShopByIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<Shop?> GetShopByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Shop?> GetMainShopAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Shop>> GetAllShopsAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Shop>> GetShopsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<int> GetShopCountAsync(CancellationToken cancellationToken = default);
}

public class ShopReadService : IShopReadService
{
    private readonly IShopRepository _shopRepository;
    private readonly ILogger<ShopReadService> _logger;

    public ShopReadService(IShopRepository shopRepository, ILogger<ShopReadService> logger)
    {
        _shopRepository = shopRepository;
        _logger = logger;
    }

    public async Task<Shop?> GetShopByIdAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shop by ID: {ShopId}", shopId);
        var shop = await _shopRepository.GetByIdAsync(shopId, cancellationToken);

        if (shop == null)
            _logger.LogWarning("Shop not found: {ShopId}", shopId);

        return shop;
    }

    public async Task<Shop?> GetShopByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shop by code: {Code}", code);
        var shop = await _shopRepository.GetByCodeAsync(code, cancellationToken);

        if (shop == null)
            _logger.LogWarning("Shop not found with code: {Code}", code);

        return shop;
    }

    public async Task<Shop?> GetMainShopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching main shop");
        return await _shopRepository.GetMainShopAsync(cancellationToken);
    }

    public async Task<ICollection<Shop>> GetAllShopsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all active shops");
        return await _shopRepository.GetActiveShopsAsync(cancellationToken);
    }

    public async Task<ICollection<Shop>> GetShopsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shops for country: {CountryId}", countryId);
        return await _shopRepository.GetShopsByCountryAsync(countryId, cancellationToken);
    }

    public async Task<int> GetShopCountAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Counting all shops");
        var shops = await _shopRepository.GetActiveShopsAsync(cancellationToken);
        return shops.Count;
    }
}


