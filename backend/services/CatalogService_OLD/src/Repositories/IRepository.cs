using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Generic repository interface for database operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Gets an entity by ID</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>Gets all entities</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Gets entities by a predicate condition</summary>
    Task<IEnumerable<T>> GetByConditionAsync(Func<T, bool> predicate);

    /// <summary>Creates a new entity</summary>
    Task<T> CreateAsync(T entity);

    /// <summary>Updates an existing entity</summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>Deletes an entity</summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>Saves all pending changes</summary>
    Task<int> SaveChangesAsync();

    /// <summary>Checks if an entity exists</summary>
    Task<bool> ExistsAsync(Guid id);
}
