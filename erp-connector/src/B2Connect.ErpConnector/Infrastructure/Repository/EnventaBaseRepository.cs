using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.ErpConnector.Infrastructure.Erp;

namespace B2Connect.ErpConnector.Infrastructure.Repository
{
    /// <summary>
    /// Base repository implementation for enventa entities.
    /// Based on eGate NVBaseRepository pattern.
    /// 
    /// Provides:
    /// - Mapping between enventa entities (TFSEntity) and DTOs (TDto)
    /// - Standard CRUD operations
    /// - Pagination and filtering
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    /// <typeparam name="TFSEntity">The enventa entity type (IcECArticle, etc.)</typeparam>
    public abstract class EnventaBaseRepository<TDto, TFSEntity> : IEnventaSelectRepository<TDto>
        where TDto : class, new()
        where TFSEntity : class, IDevFrameworkObject
    {
        protected readonly EnventaScope Scope;

        protected EnventaBaseRepository(EnventaScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <summary>
        /// The enventa entity type name (for logging).
        /// </summary>
        protected virtual string EntityTypeName => typeof(TFSEntity).Name;

        /// <summary>
        /// The DTO type name (for logging).
        /// </summary>
        protected virtual string DtoTypeName => typeof(TDto).Name;

        #region Abstract Methods - Must be implemented by derived classes

        /// <summary>
        /// Map enventa entity to DTO.
        /// Override to implement actual mapping logic.
        /// </summary>
        protected abstract TDto ToDto(TFSEntity entity);

        /// <summary>
        /// Map DTO to enventa entity.
        /// Override to implement actual mapping logic.
        /// </summary>
        protected abstract TFSEntity ToEntity(TDto dto);

        /// <summary>
        /// Load enventa entity by key.
        /// Override to implement entity-specific loading.
        /// </summary>
        protected abstract TFSEntity LoadEntity(string key);

        /// <summary>
        /// Get enventa entities by where/orderBy clause.
        /// Override to implement entity-specific query.
        /// </summary>
        protected abstract IEnumerable<TFSEntity> GetEntities(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null);

        /// <summary>
        /// Count entities matching where clause.
        /// Override to implement entity-specific count.
        /// </summary>
        protected abstract int CountEntities(string where = "");

        /// <summary>
        /// Check if entity with where clause exists.
        /// Override to implement entity-specific existence check.
        /// </summary>
        protected abstract bool ExistsEntity(string where = "");

        #endregion

        #region IEnventaReadRepository Implementation

        public virtual TDto Find(string key)
        {
            Console.WriteLine($"Find {DtoTypeName} by key: {key}");
            var entity = LoadEntity(key);
            return entity != null ? ToDto(entity) : null;
        }

        public virtual Task<TDto> FindAsync(string key)
        {
            return Task.FromResult(Find(key));
        }

        public virtual IEnumerable<TDto> Find(IEnumerable<string> keys)
        {
            if (keys == null) yield break;

            foreach (var key in keys)
            {
                var dto = Find(key);
                if (dto != null) yield return dto;
            }
        }

        public virtual bool Any(string where = "")
        {
            return ExistsEntity(where);
        }

        public virtual Task<bool> AnyAsync(string where = "")
        {
            return Task.FromResult(Any(where));
        }

        public virtual TDto FirstOrDefault(string where = "")
        {
            var entities = GetEntities(where, "", null, 1);
            var first = entities.FirstOrDefault();
            return first != null ? ToDto(first) : null;
        }

        public virtual Task<TDto> FirstOrDefaultAsync(string where = "")
        {
            return Task.FromResult(FirstOrDefault(where));
        }

        public virtual int Count(string where = "")
        {
            return CountEntities(where);
        }

        public virtual Task<int> CountAsync(string where = "")
        {
            return Task.FromResult(Count(where));
        }

        public virtual IEnumerable<TDto> GetAll()
        {
            return Select();
        }

        public virtual Task<IEnumerable<TDto>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        #endregion

        #region IEnventaSelectRepository Implementation

        public virtual IEnumerable<TDto> Select(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null,
            IProgress<int> progress = null,
            CancellationToken ct = default)
        {
            Console.WriteLine($"Select {DtoTypeName}: where='{where}', orderBy='{orderBy}', offset={offset}, limit={limit}");

            var entities = GetEntities(where, orderBy, offset, limit);
            var count = 0;

            foreach (var entity in entities)
            {
                if (ct.IsCancellationRequested) yield break;

                count++;
                progress?.Report(count);

                yield return ToDto(entity);
            }

            Console.WriteLine($"Select {DtoTypeName} completed: {count} records");
        }

        public virtual Task<IEnumerable<TDto>> SelectAsync(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null,
            IProgress<int> progress = null,
            CancellationToken ct = default)
        {
            return Task.FromResult(Select(where, orderBy, offset, limit, loadSize, progress, ct));
        }

        public virtual IQueryable<TDto> Queryable(string where = "", string orderBy = "")
        {
            // Note: This creates an in-memory queryable from the results
            // For true IQueryable support, would need LINQ provider like eGate's FSQueryable
            return Select(where, orderBy).AsQueryable();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Map multiple entities to DTOs.
        /// </summary>
        protected virtual IEnumerable<TDto> ToDtos(IEnumerable<TFSEntity> entities)
        {
            if (entities == null) yield break;

            foreach (var entity in entities)
            {
                if (entity != null)
                    yield return ToDto(entity);
            }
        }

        /// <summary>
        /// Create a new enventa entity instance using the scope.
        /// </summary>
        protected virtual TFSEntity CreateEntity()
        {
            return Scope.Create<TFSEntity>();
        }

        #endregion
    }

    /// <summary>
    /// Base CRUD repository implementation.
    /// Based on eGate NVCrudRepository pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    /// <typeparam name="TFSEntity">The enventa entity type</typeparam>
    public abstract class EnventaCrudRepository<TDto, TFSEntity> : EnventaBaseRepository<TDto, TFSEntity>, IEnventaCrudRepository<TDto>
        where TDto : class, new()
        where TFSEntity : class, IDevFrameworkDataObject
    {
        protected EnventaCrudRepository(EnventaScope scope) : base(scope)
        {
        }

        #region Abstract Methods for CRUD

        /// <summary>
        /// Save enventa entity.
        /// Override to implement entity-specific save.
        /// </summary>
        protected abstract TFSEntity SaveEntity(TFSEntity entity);

        /// <summary>
        /// Delete enventa entity.
        /// Override to implement entity-specific delete.
        /// </summary>
        protected abstract void DeleteEntity(TFSEntity entity);

        /// <summary>
        /// Get key from DTO to find corresponding entity.
        /// </summary>
        protected abstract string GetKey(TDto dto);

        #endregion

        #region IEnventaCrudRepository Implementation

        public virtual TDto Create()
        {
            var entity = CreateEntity();
            return ToDto(entity);
        }

        public virtual TDto Save(TDto dto)
        {
            Console.WriteLine($"Save {DtoTypeName}");

            var entity = ToEntity(dto);
            var saved = SaveEntity(entity);
            return ToDto(saved);
        }

        public virtual Task<TDto> SaveAsync(TDto dto)
        {
            return Task.FromResult(Save(dto));
        }

        public virtual void Delete(TDto dto)
        {
            Console.WriteLine($"Delete {DtoTypeName}");

            var key = GetKey(dto);
            var entity = LoadEntity(key);
            if (entity != null)
            {
                DeleteEntity(entity);
            }
        }

        public virtual Task DeleteAsync(TDto dto)
        {
            Delete(dto);
            return Task.CompletedTask;
        }

        public virtual bool HasEntityChanged(TDto dto)
        {
            // Default implementation - always assume changed
            // Override with actual change tracking if available
            return true;
        }

        public virtual TDto Reload(TDto dto)
        {
            var key = GetKey(dto);
            return Find(key);
        }

        #endregion
    }
}
