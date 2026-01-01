using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;

namespace B2Connect.Shared.Core.Interceptors
{
    /// <summary>
    /// Entity Framework interceptor that automatically applies tenant filtering
    /// to all queries and sets tenant ID on all saves.
    /// </summary>
    public class TenantCommandInterceptor : SaveChangesInterceptor
    {
        private readonly ITenantContext _tenantContext;

        public TenantCommandInterceptor(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyTenantFiltering(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ApplyTenantFiltering(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyTenantFiltering(DbContext? context)
        {
            if (context == null) return;

            var tenantId = _tenantContext.GetCurrentTenantId();
            if (tenantId == Guid.Empty) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is ITenantEntity tenantEntity)
                {
                    // Set tenant ID for new entities
                    if (entry.State == EntityState.Added)
                    {
                        tenantEntity.TenantId = tenantId;
                    }
                    // Ensure existing entities belong to current tenant
                    else if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                    {
                        if (tenantEntity.TenantId != tenantId)
                        {
                            throw new InvalidOperationException(
                                $"Entity {entry.Entity.GetType().Name} with ID {entry.Property(context.Model.FindEntityType(entry.Entity.GetType())!.FindPrimaryKey()!.Properties.First().Name).CurrentValue} does not belong to current tenant.");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Entity Framework interceptor that automatically filters queries by tenant
    /// </summary>
    public class TenantQueryInterceptor : IQueryExpressionInterceptor
    {
        private readonly ITenantContext _tenantContext;

        public TenantQueryInterceptor(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        }

        public Expression ProcessExpression(Expression queryExpression)
        {
            var tenantId = _tenantContext.GetCurrentTenantId();
            if (tenantId == Guid.Empty) return queryExpression;

            return new TenantQueryVisitor(tenantId).Visit(queryExpression);
        }

        private class TenantQueryVisitor : ExpressionVisitor
        {
            private readonly Guid _tenantId;

            public TenantQueryVisitor(Guid tenantId)
            {
                _tenantId = tenantId;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                // Check if this is a query on ITenantEntity
                if (IsTenantEntityQuery(node))
                {
                    // Add tenant filter to the query
                    return AddTenantFilter(node);
                }

                return base.VisitMethodCall(node);
            }

            private bool IsTenantEntityQuery(MethodCallExpression node)
            {
                // Check if the query source implements ITenantEntity
                if (node.Arguments.Count > 0)
                {
                    var sourceType = GetElementType(node.Arguments[0]);
                    return typeof(ITenantEntity).IsAssignableFrom(sourceType);
                }
                return false;
            }

            private Type GetElementType(Expression expression)
            {
                if (expression is MethodCallExpression methodCall)
                {
                    return GetElementType(methodCall.Arguments[0]);
                }

                if (expression is ConstantExpression constant && constant.Value is IQueryable queryable)
                {
                    return queryable.ElementType;
                }

                // For DbSet properties
                if (expression is MemberExpression member && member.Member is System.Reflection.PropertyInfo property)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    {
                        return propertyType.GetGenericArguments()[0];
                    }
                }

                return typeof(object);
            }

            private Expression AddTenantFilter(MethodCallExpression node)
            {
                // Create parameter for the entity type
                var entityType = GetElementType(node.Arguments[0]);
                var parameter = Expression.Parameter(entityType, "e");

                // Create property access: e.TenantId
                var tenantProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

                // Create constant for tenant ID
                var tenantIdConstant = Expression.Constant(_tenantId);

                // Create equality: e.TenantId == tenantId
                var equality = Expression.Equal(tenantProperty, tenantIdConstant);

                // Create lambda: e => e.TenantId == tenantId
                var lambda = Expression.Lambda(equality, parameter);

                // Create Where call: .Where(e => e.TenantId == tenantId)
                var whereMethod = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .First()
                    .MakeGenericMethod(entityType);

                // Apply the filter to the original query
                return Expression.Call(whereMethod, node, lambda);
            }
        }
    }
}