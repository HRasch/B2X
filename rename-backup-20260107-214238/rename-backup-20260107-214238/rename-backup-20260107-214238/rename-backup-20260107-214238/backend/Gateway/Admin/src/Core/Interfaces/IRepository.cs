using B2X.Admin.Core.Entities;

namespace B2X.Admin.Core.Interfaces;

/// <summary>
/// Generic repository interface for database operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Gets an entity by ID</summary>
    Task<T?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken ct = default);

    /// <summary>Gets all entities</summary>
    Task<IEnumerable<T>> GetAllAsync(Guid tenantId, CancellationToken ct = default);

    /// <summary>Gets entities by a predicate condition</summary>
    Task<IEnumerable<T>> GetByConditionAsync(Guid tenantId, Func<T, bool> predicate, CancellationToken ct = default);

    /// <summary>Creates a new entity</summary>
    Task AddAsync(T entity, CancellationToken ct = default);

    /// <summary>Updates an existing entity</summary>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>Deletes an entity</summary>
    Task DeleteAsync(Guid tenantId, Guid id, CancellationToken ct = default);

    /// <summary>Saves all pending changes</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>Checks if an entity exists</summary>
    Task<bool> ExistsAsync(Guid tenantId, Guid id, CancellationToken ct = default);
}
