using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.ErpConnector.Infrastructure.Repository
{
    /// <summary>
    /// Base query builder interface.
    /// Based on eGate INVQueryBuilder pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    public interface IEnventaQueryBuilder<TDto> where TDto : class
    {
        /// <summary>
        /// Add where clause.
        /// </summary>
        IEnventaQueryBuilder<TDto> Where(string where);

        /// <summary>
        /// Add order by clause.
        /// </summary>
        IEnventaQueryBuilder<TDto> OrderBy(string orderBy);

        /// <summary>
        /// Skip N records.
        /// </summary>
        IEnventaQueryBuilder<TDto> Skip(int count);

        /// <summary>
        /// Take N records.
        /// </summary>
        IEnventaQueryBuilder<TDto> Take(int count);

        /// <summary>
        /// Set load size for batching.
        /// </summary>
        IEnventaQueryBuilder<TDto> LoadSize(int size);

        /// <summary>
        /// Execute query and return results.
        /// </summary>
        IEnumerable<TDto> Execute();

        /// <summary>
        /// Execute query and return results (async).
        /// </summary>
        Task<IEnumerable<TDto>> ExecuteAsync(CancellationToken ct = default);

        /// <summary>
        /// Execute and return first result or null.
        /// </summary>
        TDto FirstOrDefault();

        /// <summary>
        /// Execute and return first result or null (async).
        /// </summary>
        Task<TDto> FirstOrDefaultAsync(CancellationToken ct = default);

        /// <summary>
        /// Execute and return count.
        /// </summary>
        int Count();

        /// <summary>
        /// Execute and return count (async).
        /// </summary>
        Task<int> CountAsync(CancellationToken ct = default);

        /// <summary>
        /// Check if any results exist.
        /// </summary>
        bool Any();

        /// <summary>
        /// Check if any results exist (async).
        /// </summary>
        Task<bool> AnyAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Base query builder implementation.
    /// Based on eGate NVQueryBuilderBase pattern.
    /// </summary>
    /// <typeparam name="TDto">The domain DTO type</typeparam>
    public abstract class EnventaQueryBuilderBase<TDto> : IEnventaQueryBuilder<TDto> where TDto : class, new()
    {
        protected readonly IEnventaSelectRepository<TDto> Repository;
        protected string WhereClause = "";
        protected string OrderByClause = "";
        protected int? SkipCount;
        protected int? TakeCount;
        protected int? LoadSizeValue;

        protected EnventaQueryBuilderBase(IEnventaSelectRepository<TDto> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public virtual IEnventaQueryBuilder<TDto> Where(string where)
        {
            if (!string.IsNullOrWhiteSpace(where))
            {
                WhereClause = string.IsNullOrWhiteSpace(WhereClause)
                    ? where
                    : $"({WhereClause}) AND ({where})";
            }
            return this;
        }

        public virtual IEnventaQueryBuilder<TDto> OrderBy(string orderBy)
        {
            OrderByClause = orderBy;
            return this;
        }

        public virtual IEnventaQueryBuilder<TDto> Skip(int count)
        {
            SkipCount = count;
            return this;
        }

        public virtual IEnventaQueryBuilder<TDto> Take(int count)
        {
            TakeCount = count;
            return this;
        }

        public virtual IEnventaQueryBuilder<TDto> LoadSize(int size)
        {
            LoadSizeValue = size;
            return this;
        }

        public virtual IEnumerable<TDto> Execute()
        {
            return Repository.Select(WhereClause, OrderByClause, SkipCount, TakeCount, LoadSizeValue);
        }

        public virtual Task<IEnumerable<TDto>> ExecuteAsync(CancellationToken ct = default)
        {
            return Repository.SelectAsync(WhereClause, OrderByClause, SkipCount, TakeCount, LoadSizeValue, null, ct);
        }

        public virtual TDto FirstOrDefault()
        {
            return Repository.FirstOrDefault(WhereClause);
        }

        public virtual Task<TDto> FirstOrDefaultAsync(CancellationToken ct = default)
        {
            return Repository.FirstOrDefaultAsync(WhereClause);
        }

        public virtual int Count()
        {
            return Repository.Count(WhereClause);
        }

        public virtual Task<int> CountAsync(CancellationToken ct = default)
        {
            return Repository.CountAsync(WhereClause);
        }

        public virtual bool Any()
        {
            return Repository.Any(WhereClause);
        }

        public virtual Task<bool> AnyAsync(CancellationToken ct = default)
        {
            return Repository.AnyAsync(WhereClause);
        }

        /// <summary>
        /// Helper to add AND condition.
        /// </summary>
        protected void AddWhere(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return;

            WhereClause = string.IsNullOrWhiteSpace(WhereClause)
                ? condition
                : $"{WhereClause} AND {condition}";
        }

        /// <summary>
        /// Helper to escape string for SQL-like where clause.
        /// </summary>
        protected static string EscapeString(string value)
        {
            if (value == null) return "NULL";
            return $"'{value.Replace("'", "''")}'";
        }
    }
}
