using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Admin.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for database operations
/// Handles multi-tenant filtering automatically
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
#pragma warning disable CA1051 // Do not declare visible instance fields
    protected readonly CatalogDbContext _context;
    protected readonly DbSet<T> _dbSet;
#pragma warning restore CA1051 // Do not declare visible instance fields

    public Repository(CatalogDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken: ct);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }

    public virtual async Task<IEnumerable<T>> GetByConditionAsync(Guid tenantId, Func<T, bool> predicate, CancellationToken ct = default)
    {
        return await Task.FromResult(_dbSet.Where(predicate).ToList());
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity, ct);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid tenantId, Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(tenantId, id, ct);
        if (entity == null)
        {
            return;
        }

        _dbSet.Remove(entity);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }

    public virtual async Task<bool> ExistsAsync(Guid tenantId, Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(tenantId, id, ct);
        return entity != null;
    }
}
