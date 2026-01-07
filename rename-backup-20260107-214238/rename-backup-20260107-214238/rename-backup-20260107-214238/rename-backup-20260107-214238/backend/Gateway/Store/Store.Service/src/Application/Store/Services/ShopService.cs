using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Common.Interfaces;

namespace B2Connect.Store.Application.Store.Services;

/// <summary>
/// Interface for Shop service operations
/// </summary>
public interface IShopService
{
    Task<Shop?> GetShopByIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<Shop?> GetShopByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Shop?> GetMainShopAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Shop>> GetAllActiveShopsAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Shop>> GetShopsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default);
    Task<Shop> CreateShopAsync(Shop shop, CancellationToken cancellationToken = default);
    Task<Shop> UpdateShopAsync(Shop shop, CancellationToken cancellationToken = default);
    Task DeleteShopAsync(Guid shopId, CancellationToken cancellationToken = default);
}

public class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;

    public ShopService(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }

    public async Task<Shop?> GetShopByIdAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await _shopRepository.GetByIdAsync(shopId, cancellationToken);
    }

    public async Task<Shop?> GetShopByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _shopRepository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<Shop?> GetMainShopAsync(CancellationToken cancellationToken = default)
    {
        return await _shopRepository.GetMainShopAsync(cancellationToken);
    }

    public async Task<ICollection<Shop>> GetAllActiveShopsAsync(CancellationToken cancellationToken = default)
    {
        return await _shopRepository.GetActiveShopsAsync(cancellationToken);
    }

    public async Task<ICollection<Shop>> GetShopsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await _shopRepository.GetShopsByCountryAsync(countryId, cancellationToken);
    }

    public async Task<Shop> CreateShopAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(shop.Code))
            throw new ArgumentException("Shop code is required", nameof(shop.Code));

        var existing = await _shopRepository.GetByCodeAsync(shop.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Shop with code '{shop.Code}' already exists");

        shop.Id = Guid.NewGuid();
        shop.CreatedAt = DateTime.UtcNow;
        shop.UpdatedAt = DateTime.UtcNow;

        await _shopRepository.AddAsync(shop, cancellationToken);
        return shop;
    }

    public async Task<Shop> UpdateShopAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        var existing = await _shopRepository.GetByIdAsync(shop.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException($"Shop with ID '{shop.Id}' not found");

        shop.UpdatedAt = DateTime.UtcNow;
        shop.CreatedAt = existing.CreatedAt;
        shop.CreatedBy = existing.CreatedBy;

        await _shopRepository.UpdateAsync(shop, cancellationToken);
        return shop;
    }

    public async Task DeleteShopAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        var shop = await _shopRepository.GetByIdAsync(shopId, cancellationToken);
        if (shop == null)
            throw new InvalidOperationException($"Shop with ID '{shopId}' not found");

        await _shopRepository.DeleteAsync(shop, cancellationToken);
    }
}
