using Microsoft.EntityFrameworkCore;
using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;
using B2X.Store.Infrastructure.Common.Data;

namespace B2X.Store.Infrastructure.Common.Repositories;

/// <summary>
/// Repository implementation for Country entity
/// Common repository used by Store and Admin domains
/// </summary>
public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Code == code.ToUpper(), cancellationToken);
    }

    public async Task<ICollection<Country>> GetActiveCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Country>> GetCountriesByRegionAsync(string region, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Region == region && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Country>> GetShippingCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsShippingEnabled && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}

