using Microsoft.EntityFrameworkCore;
using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Common.Interfaces;
using B2Connect.Store.Infrastructure.Common.Data;

namespace B2Connect.Store.Infrastructure.Common.Repositories;

/// <summary>
/// Repository implementation for Language entity
/// Common repository used by Store and Admin domains
/// </summary>
public class LanguageRepository : Repository<Language>, ILanguageRepository
{
    public LanguageRepository(StoreDbContext context) : base(context)
    {
    }

    public async Task<Language?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Code == code.ToUpper(), cancellationToken);
    }

    public async Task<Language?> GetDefaultLanguageAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.IsDefault, cancellationToken);
    }

    public async Task<ICollection<Language>> GetActiveLanguagesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.IsActive)
            .OrderBy(l => l.DisplayOrder)
            .ThenBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Language>> GetLanguagesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Shops.Any(s => s.Id == storeId) && l.IsActive)
            .OrderBy(l => l.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}

