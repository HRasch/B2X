namespace B2Connect.Store.Core.Common.Interfaces;

/// <summary>
/// Generic repository interface for database operations
/// Common interface used by all domains
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Gets all entities</summary>
    Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets an entity by ID</summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Deletes an entity</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Deletes an entity by ID</summary>
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Checks if an entity exists</summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets count of all entities</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves changes to the database</summary>
    Task SaveAsync(CancellationToken cancellationToken = default);
}

