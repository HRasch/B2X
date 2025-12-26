using Microsoft.EntityFrameworkCore;
using B2Connect.Store.Core.Store.Entities;
using B2Connect.Store.Core.Store.Interfaces;
using B2Connect.Store.Infrastructure.Common.Data;

namespace B2Connect.Store.Infrastructure.Store.Repositories;

/// <summary>
/// Repository implementation for ShippingMethod entity
/// Store-specific repository for Shipping Method management
/// </summary>
public class ShippingMethodRepository : B2Connect.Store.Infrastructure.Common.Repositories.Repository<ShippingMethod>, IShippingMethodRepository
{
    public ShippingMethodRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<ShippingMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Code == code.ToUpper(), cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.StoreId == storeId && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetMethodsForCountryAsync(Guid storeId, string countryCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.StoreId == storeId
                && s.IsActive
                && (s.SupportedCountries == null || s.SupportedCountries.Contains(countryCode))
                && (s.ExcludedCountries == null || !s.ExcludedCountries.Contains(countryCode)))
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetByCarrierAsync(string carrier, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Carrier == carrier && s.StoreId == storeId && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShippingMethod?> GetCheapestMethodAsync(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken = default)
    {
        var methods = await GetMethodsForCountryAsync(storeId, countryCode, cancellationToken);

        return methods
            .Where(s => s.MaximumWeight == null || s.MaximumWeight >= weight)
            .OrderBy(s => s.BaseCost + (s.CostPerKg ?? 0) * weight)
            .FirstOrDefault();
    }
}

