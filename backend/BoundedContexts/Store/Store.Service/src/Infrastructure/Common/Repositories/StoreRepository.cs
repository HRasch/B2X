using Microsoft.EntityFrameworkCore;
using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Common.Interfaces;
using B2Connect.Store.Infrastructure.Common.Data;

namespace B2Connect.Store.Infrastructure.Common.Repositories;

/// <summary>
/// Repository implementation for Shop entity
/// Common repository used by Store and Admin domains
/// </summary>
public class ShopRepository : Repository<Shop>, IShopRepository
{
    public ShopRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<Shop?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<Shop?> GetMainShopAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.IsMainShop, cancellationToken);
    }

    public async Task<ICollection<Shop>> GetActiveShopsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Shop>> GetShopsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.CountryId == countryId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }
}

