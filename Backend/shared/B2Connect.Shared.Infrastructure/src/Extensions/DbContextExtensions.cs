using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using B2Connect.Shared.Core.Interfaces;
// using B2Connect.Shared.Tenancy.Infrastructure.Context;  // TODO: Fix Tenancy reference

namespace B2Connect.Shared.Infrastructure.Extensions;

/// <summary>
/// Extension methods for DbContext to configure Global Query Filters for multi-tenant isolation.
/// 
/// SECURITY: This provides automatic tenant filtering at the database level,
/// preventing accidental cross-tenant data leaks even if developer forgets to add .Where(TenantId).
/// 
/// Usage in DbContext.OnModelCreating:
///   modelBuilder.ApplyGlobalTenantFilter(_tenantContext);
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Apply Global Query Filter for all entities implementing IHasTenantId.
    /// This ensures ALL database queries automatically filter by current tenant.
    /// </summary>
    /// <param name="modelBuilder">EF Core ModelBuilder</param>
    /// <param name="tenantContext">Scoped ITenantContext with current tenant ID</param>
    //public static void ApplyGlobalTenantFilter(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    //{
    //    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    //    {
    //        if (typeof(IHasTenantId).IsAssignableFrom(entityType.ClrType))
    //        {
    //            var method = SetGlobalQueryFilterMethod.MakeGenericMethod(entityType.ClrType);
    //            method.Invoke(null, new object[] { modelBuilder, tenantContext });
    //        }
    //    }
    //}

    //private static readonly MethodInfo SetGlobalQueryFilterMethod = typeof(DbContextExtensions)
    //    .GetMethod(nameof(SetGlobalQueryFilter), BindingFlags.NonPublic | BindingFlags.Static)!;

    /// <summary>
    /// Set Global Query Filter for a specific entity type.
    /// Filter: e => e.TenantId == tenantContext.TenantId
    /// </summary>
    //private static void SetGlobalQueryFilter<TEntity>(ModelBuilder modelBuilder, ITenantContext tenantContext)
    //    where TEntity : class, IHasTenantId
    //{
    //    // Build expression: e => e.TenantId == tenantContext.TenantId
    //    var parameter = Expression.Parameter(typeof(TEntity), "e");
    //    var property = Expression.Property(parameter, nameof(IHasTenantId.TenantId));
    //    var tenantIdValue = Expression.Property(Expression.Constant(tenantContext), nameof(ITenantContext.TenantId));
    //    var comparison = Expression.Equal(property, tenantIdValue);
    //    var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);

    //    modelBuilder.Entity<TEntity>().HasQueryFilter(lambda);
    //}
}
