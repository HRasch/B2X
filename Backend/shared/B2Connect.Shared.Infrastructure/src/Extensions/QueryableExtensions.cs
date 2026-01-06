using System.Linq.Expressions;
using B2Connect.Shared.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Shared.Infrastructure.Extensions;

/// <summary>
/// IQueryable extension methods for tenant filtering.
/// These methods make tenant filtering explicit and easy to use.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies automatic tenant filtering to any BaseEntity-derived queryable.
    /// Used when you want explicit, readable tenant filtering in repository queries.
    ///
    /// Example:
    ///   return await _context.Products
    ///       .ForTenant(tenantId)
    ///       .Where(p => p.IsActive)
    ///       .ToListAsync(ct);
    /// </summary>
    /// <typeparam name="T">Entity type (must inherit from BaseEntity)</typeparam>
    /// <param name="query">IQueryable to filter</param>
    /// <param name="tenantId">Tenant ID to filter by</param>
    /// <returns>Filtered IQueryable</returns>
    public static IQueryable<T> ForTenant<T>(this IQueryable<T> query, Guid tenantId)
        where T : BaseEntity
    {
        return query.Where(x => x.TenantId == tenantId);
    }

    /// <summary>
    /// Includes navigation properties while maintaining tenant isolation.
    ///
    /// Example:
    ///   return await _context.Products
    ///       .ForTenant(tenantId)
    ///       .WithInclude(p => p.Category)
    ///       .FirstOrDefaultAsync(ct);
    /// </summary>
    public static IQueryable<T> WithInclude<T, TProperty>(
        this IQueryable<T> query,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : BaseEntity
    {
        return query.Include(navigationPropertyPath);
    }

    /// <summary>
    /// Applies pagination to query while maintaining tenant isolation.
    ///
    /// Example:
    ///   return await _context.Products
    ///       .ForTenant(tenantId)
    ///       .Paginate(pageNumber: 1, pageSize: 20)
    ///       .ToListAsync(ct);
    /// </summary>
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize) where T : BaseEntity
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Excludes soft-deleted items from query.
    /// Useful when combined with ForTenant for complete filtering.
    ///
    /// Example:
    ///   return await _context.Products
    ///       .ForTenant(tenantId)
    ///       .ExcludeDeleted()
    ///       .ToListAsync(ct);
    /// </summary>
    public static IQueryable<T> ExcludeDeleted<T>(this IQueryable<T> query)
        where T : BaseEntity
    {
        return query.Where(x => !x.IsDeleted);
    }
}
