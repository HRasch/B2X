using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.ErpConnector.Infrastructure.Repository
{
    /// <summary>
    /// Base repository interface for read operations.
    /// Based on eGate INVReadRepository pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    public interface IEnventaReadRepository<TDto> where TDto : class, new()
    {
        /// <summary>
        /// Find entity by primary key.
        /// </summary>
        TDto Find(string key);

        /// <summary>
        /// Find entity by primary key (async).
        /// </summary>
        Task<TDto> FindAsync(string key);

        /// <summary>
        /// Find multiple entities by keys.
        /// </summary>
        IEnumerable<TDto> Find(IEnumerable<string> keys);

        /// <summary>
        /// Check if any entity matches the where clause.
        /// </summary>
        bool Any(string where = "");

        /// <summary>
        /// Check if any entity matches (async).
        /// </summary>
        Task<bool> AnyAsync(string where = "");

        /// <summary>
        /// Get first entity matching where clause or null.
        /// </summary>
        TDto FirstOrDefault(string where = "");

        /// <summary>
        /// Get first entity matching where clause or null (async).
        /// </summary>
        Task<TDto> FirstOrDefaultAsync(string where = "");

        /// <summary>
        /// Count entities matching where clause.
        /// </summary>
        int Count(string where = "");

        /// <summary>
        /// Count entities matching where clause (async).
        /// </summary>
        Task<int> CountAsync(string where = "");

        /// <summary>
        /// Get all entities.
        /// </summary>
        IEnumerable<TDto> GetAll();

        /// <summary>
        /// Get all entities (async).
        /// </summary>
        Task<IEnumerable<TDto>> GetAllAsync();
    }

    /// <summary>
    /// Extended repository interface with select/pagination.
    /// Based on eGate INVSelectRepository pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    public interface IEnventaSelectRepository<TDto> : IEnventaReadRepository<TDto> where TDto : class, new()
    {
        /// <summary>
        /// Select entities with filtering, ordering, and pagination.
        /// </summary>
        IEnumerable<TDto> Select(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null,
            IProgress<int> progress = null,
            CancellationToken ct = default);

        /// <summary>
        /// Select entities (async).
        /// </summary>
        Task<IEnumerable<TDto>> SelectAsync(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null,
            IProgress<int> progress = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get IQueryable for LINQ queries.
        /// </summary>
        IQueryable<TDto> Queryable(string where = "", string orderBy = "");
    }

    /// <summary>
    /// Full CRUD repository interface.
    /// Based on eGate INVCrudRepository pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    public interface IEnventaCrudRepository<TDto> : IEnventaSelectRepository<TDto> where TDto : class, new()
    {
        /// <summary>
        /// Create a new entity instance.
        /// </summary>
        TDto Create();

        /// <summary>
        /// Save entity (insert or update).
        /// </summary>
        TDto Save(TDto entity);

        /// <summary>
        /// Save entity (async).
        /// </summary>
        Task<TDto> SaveAsync(TDto entity);

        /// <summary>
        /// Delete entity.
        /// </summary>
        void Delete(TDto entity);

        /// <summary>
        /// Delete entity (async).
        /// </summary>
        Task DeleteAsync(TDto entity);

        /// <summary>
        /// Check if entity has been modified.
        /// </summary>
        bool HasEntityChanged(TDto entity);

        /// <summary>
        /// Reload entity from data source.
        /// </summary>
        TDto Reload(TDto entity);
    }

    /// <summary>
    /// Repository with query builder support.
    /// Based on eGate INVQueryRepository pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    /// <typeparam name="TQueryBuilder">The query builder type</typeparam>
    public interface IEnventaQueryRepository<TDto, out TQueryBuilder> : IEnventaSelectRepository<TDto>
        where TDto : class, new()
        where TQueryBuilder : IEnventaQueryBuilder<TDto>
    {
        /// <summary>
        /// Create a new query builder for type-safe queries.
        /// </summary>
        TQueryBuilder Query();
    }
}
