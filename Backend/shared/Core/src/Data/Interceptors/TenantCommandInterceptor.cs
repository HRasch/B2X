using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using B2Connect.LocalizationService;

namespace B2Connect.Shared.Data.Interceptors

/// <summary>
/// Entity Framework interceptor that automatically applies tenant filtering
/// to all queries and sets tenant ID on all saves.
/// </summary>
public class TenantCommandInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Automatically sets tenant ID on entities being saved
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyTenantToEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Automatically sets tenant ID on entities being saved (async)
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyTenantToEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyTenantToEntities(DbContext? context)
    {
        if (context == null) return;

        var tenantId = TenantContext.CurrentTenantId;
        if (!tenantId.HasValue) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is ITenantEntity tenantEntity)
            {
                // Set tenant ID for new entities
                if (entry.State == EntityState.Added)
                {
                    tenantEntity.TenantId = tenantId.Value;
                }
                // Ensure tenant ID is correct for modified entities
                else if (entry.State == EntityState.Modified)
                {
                    // Prevent cross-tenant data modification
                    if (tenantEntity.TenantId != tenantId.Value)
                    {
                        throw new InvalidOperationException(
                            $"Cannot modify entity {entry.Entity.GetType().Name} with ID {entry.Property(context.Model.FindEntityType(entry.Entity.GetType())?.FindPrimaryKey()?.Properties.First()?.Name)?.CurrentValue} " +
                            $"from tenant {tenantEntity.TenantId} while authenticated as tenant {tenantId.Value}");
                    }
                }
            }
        }
    }
}

/// <summary>
/// Entity Framework interceptor that automatically filters queries by tenant
/// </summary>
public class TenantQueryInterceptor : IQueryCompilerInterceptor
{
    /// <summary>
    /// Intercepts query compilation to add tenant filtering
    /// </summary>
    public Expression ProcessedQueryExpression
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Modifies the query expression to include tenant filtering
    /// </summary>
    public Expression ProcessQueryExpression(
        Expression queryExpression,
        QueryExpressionEventData eventData)
    {
        var tenantId = TenantContext.CurrentTenantId;
        if (!tenantId.HasValue)
        {
            // If no tenant context, return original query (for system operations)
            return queryExpression;
        }

        return new TenantQueryVisitor(tenantId.Value).Visit(queryExpression);
    }
}

/// <summary>
/// Expression visitor that adds tenant filtering to queries
/// </summary>
public class TenantQueryVisitor : ExpressionVisitor
{
    private readonly Guid _tenantId;

    public TenantQueryVisitor(Guid tenantId)
    {
        _tenantId = tenantId;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        // Intercept calls to Where method
        if (node.Method.Name == "Where" && node.Arguments.Count == 2)
        {
            var source = node.Arguments[0];
            var predicate = node.Arguments[1];

            // Check if this is a query on ITenantEntity
            if (IsTenantEntityQuery(source))
            {
                // Combine existing predicate with tenant filter
                var tenantPredicate = CreateTenantPredicate(source);
                var combinedPredicate = CombinePredicates(predicate, tenantPredicate);

                return Expression.Call(
                    node.Object,
                    node.Method,
                    new[] { source, combinedPredicate });
            }
        }

        return base.VisitMethodCall(node);
    }

    private bool IsTenantEntityQuery(Expression source)
    {
        // Check if the query source implements ITenantEntity
        var elementType = GetElementType(source.Type);
        return typeof(ITenantEntity).IsAssignableFrom(elementType);
    }

    private Type GetElementType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
        {
            return type.GetGenericArguments()[0];
        }
        return type;
    }

    private Expression CreateTenantPredicate(Expression source)
    {
        var elementType = GetElementType(source.Type);
        var parameter = Expression.Parameter(elementType, "e");
        var tenantProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
        var tenantConstant = Expression.Constant(_tenantId);
        var tenantCondition = Expression.Equal(tenantProperty, tenantConstant);

        return Expression.Lambda(tenantCondition, parameter);
    }

    private Expression CombinePredicates(Expression existing, Expression tenantFilter)
    {
        // Combine existing predicate with tenant filter using AND
        var existingLambda = (LambdaExpression)existing;
        var tenantLambda = (LambdaExpression)tenantFilter;

        var parameter = existingLambda.Parameters[0];
        var combinedBody = Expression.AndAlso(
            existingLambda.Body,
            Expression.Invoke(tenantLambda, parameter));

        return Expression.Lambda(combinedBody, parameter);
    }
}

/// <summary>
/// Marker interface for entities that belong to a tenant
/// </summary>
public interface ITenantEntity
{
    /// <summary>The tenant ID this entity belongs to</summary>
    Guid TenantId { get; set; }
}</content>
<parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Shared / Core / src / Data / Interceptors / TenantCommandInterceptor.cs