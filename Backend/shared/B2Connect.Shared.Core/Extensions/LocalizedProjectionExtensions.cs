using System.Linq.Expressions;
using System.Reflection;
using B2Connect.Shared.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace B2Connect.Shared.Core.Extensions;

/// <summary>
/// Extension methods for projecting EF Core queries to localized DTOs.
/// Integrates with multi-tenancy interceptors and localization patterns.
/// Supports both attribute-based ([Localizable]) and convention-based projections.
/// </summary>
public static class LocalizedProjectionExtensions
{
    /// <summary>
    /// Cache for compiled projection expressions to avoid repeated reflection costs.
    /// Key: "EntityType:DtoType:locale" -> Expression tree
    /// </summary>
    private static readonly ConcurrentDictionary<string, object> _projectionCache
        = new ConcurrentDictionary<string, object>();

    /// <summary>
    /// Checks if a property type is LocalizedContent or LocalizedContent?
    /// </summary>
    private static bool IsLocalizedContentType(Type propertyType)
    {
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        return underlyingType == typeof(LocalizedContent);
    }

    /// <summary>
    /// Automatically projects an EF Core query to a localized DTO using attribute-based configuration or convention-over-configuration.
    /// Detects [Localizable] attributes on DTO properties first, then falls back to conventions (e.g., Name + NameTranslations).
    /// If both exist, uses EF.Functions.JsonExtract with fallback to default value.
    ///
    /// Attribute-based example:
    /// <code>
    /// public class ProductDto
    /// {
    ///     [Localizable("Name", "NameTranslations")]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    ///
    /// Convention-based example (still supported):
    /// DTO property "Name" maps to entity properties "Name" and "NameTranslations".
    ///
    /// Usage:
    /// <code>
    /// var dto = await dbContext.Products
    ///     .SelectLocalized&lt;LocalizedProductDto&gt;("de")
    ///     .FirstOrDefaultAsync();
    /// </code>
    /// </summary>
    /// <typeparam name="TEntity">The entity type (must implement ITenantEntity)</typeparam>
    /// <typeparam name="TDto">The DTO type</typeparam>
    /// <param name="query">The source query</param>
    /// <param name="locale">The locale code (e.g., "en", "de")</param>
    /// <returns>A queryable with automatic localized DTO projection</returns>
    public static IQueryable<TDto> SelectLocalized<TEntity, TDto>(this IQueryable<TEntity> query, string locale)
        where TEntity : class, ITenantEntity
    {
        var projection = GetCachedProjection<TEntity, TDto>(locale);
        return query.Select(projection);
    }

    /// <summary>
    /// Gets a cached projection expression, building it if not already cached.
    /// </summary>
    private static Expression<Func<TEntity, TDto>> GetCachedProjection<TEntity, TDto>(string locale)
        where TEntity : class, ITenantEntity
    {
        var key = $"{typeof(TEntity).FullName}:{typeof(TDto).FullName}:{locale}";

        var cached = _projectionCache.GetOrAdd(key, _ =>
        {
            // Build the projection expression (expensive operation, done only once per type combination)
            return (object)BuildLocalizedProjection<TEntity, TDto>(locale);
        });

        return (Expression<Func<TEntity, TDto>>)cached;
    }

    /// <summary>
    /// Builds an expression tree for automatic localized projection using [Localizable] attributes or conventions.
    /// Priority: [Localizable] attribute > Convention-based (Property + PropertyTranslations)
    /// </summary>
    private static Expression<Func<TEntity, TDto>> BuildLocalizedProjection<TEntity, TDto>(string locale)
    {
        var entityType = typeof(TEntity);
        var dtoType = typeof(TDto);

        var entityParam = Expression.Parameter(entityType, "entity");
        var dtoProperties = dtoType.GetProperties();

        var bindings = new List<MemberBinding>();

        foreach (var dtoProperty in dtoProperties)
        {
            // Check for [Localizable] attribute first
            var localizableAttribute = dtoProperty.GetCustomAttribute<LocalizableAttribute>();
            if (localizableAttribute != null)
            {
                // Use attribute configuration
                var defaultPropertyName = localizableAttribute.DefaultProperty ?? dtoProperty.Name;
                var translationPropertyName = localizableAttribute.TranslationProperty ?? (defaultPropertyName + "Translations");

                var propertyExpression = BuildLocalizedPropertyExpression(
                    entityParam, entityType, defaultPropertyName, translationPropertyName, locale);

                bindings.Add(Expression.Bind(dtoProperty, propertyExpression));
                continue;
            }

            // Fall back to convention-based approach
            var entityProperty = entityType.GetProperty(dtoProperty.Name);
            if (entityProperty == null)
                continue;

            var translationProperty = entityType.GetProperty(dtoProperty.Name + "Translations");
            var hasTranslations = translationProperty != null && IsLocalizedContentType(translationProperty.PropertyType);

            Expression conventionPropertyExpression;

            if (hasTranslations && translationProperty != null)
            {
                // Build: translationProperty != null ? EF.Functions.JsonExtract(translationProperty, locale) ?? entityProperty : entityProperty
                var translationAccess = Expression.Property(entityParam, translationProperty);
                var entityPropertyAccess = Expression.Property(entityParam, entityProperty);

                // EF.Functions.JsonExtract(translationProperty, "$.Translations." + locale)
                var functionsProperty = typeof(EF).GetProperty("Functions");
                if (functionsProperty == null)
                    throw new InvalidOperationException("EF.Functions property not found");

                var jsonExtractMethod = functionsProperty.PropertyType.GetMethod("JsonExtract", new[] { typeof(object), typeof(string) });
                if (jsonExtractMethod == null)
                    throw new InvalidOperationException("EF.Functions.JsonExtract method not found");

                var jsonExtractCall = Expression.Call(
                    Expression.Property(null, functionsProperty),
                    jsonExtractMethod,
                    translationAccess,
                    Expression.Constant("$.Translations." + locale)
                );

                // translationProperty != null
                var nullCheck = Expression.NotEqual(translationAccess, Expression.Constant(null, translationProperty.PropertyType));

                // EF.Functions.JsonExtract(...) ?? entityProperty
                var coalesce = Expression.Coalesce(jsonExtractCall, entityPropertyAccess);

                // translationProperty != null ? coalesce : entityProperty
                conventionPropertyExpression = Expression.Condition(nullCheck, coalesce, entityPropertyAccess);
            }
            else
            {
                // Direct property mapping
                conventionPropertyExpression = Expression.Property(entityParam, entityProperty);
            }

            bindings.Add(Expression.Bind(dtoProperty, conventionPropertyExpression));
        }

        var newExpression = Expression.New(dtoType);
        var memberInit = Expression.MemberInit(newExpression, bindings);

        return Expression.Lambda<Func<TEntity, TDto>>(memberInit, entityParam);
    }

    /// <summary>
    /// Builds the expression for a single localized property using the specified property names.
    /// </summary>
    private static Expression BuildLocalizedPropertyExpression(
        ParameterExpression entityParam,
        Type entityType,
        string defaultPropertyName,
        string translationPropertyName,
        string locale)
    {
        var defaultProperty = entityType.GetProperty(defaultPropertyName);
        if (defaultProperty == null)
            throw new InvalidOperationException($"Entity property '{defaultPropertyName}' not found on type '{entityType.Name}'");

        var translationProperty = entityType.GetProperty(translationPropertyName);
        if (translationProperty == null)
            throw new InvalidOperationException($"Entity property '{translationPropertyName}' not found on type '{entityType.Name}'");

        if (!IsLocalizedContentType(translationProperty.PropertyType))
            throw new InvalidOperationException($"Translation property '{translationPropertyName}' must be of type 'LocalizedContent' or 'LocalizedContent?'");

        // Build: translationProperty != null ? EF.Functions.JsonExtract(translationProperty, locale) ?? defaultProperty : defaultProperty
        var translationAccess = Expression.Property(entityParam, translationProperty);
        var defaultPropertyAccess = Expression.Property(entityParam, defaultProperty);

        // EF.Functions.JsonExtract(translationProperty, "$.Translations." + locale)
        var functionsProperty = typeof(EF).GetProperty("Functions");
        if (functionsProperty == null)
            throw new InvalidOperationException("EF.Functions property not found");

        var jsonExtractMethod = functionsProperty.PropertyType.GetMethod("JsonExtract", new[] { typeof(object), typeof(string) });
        if (jsonExtractMethod == null)
            throw new InvalidOperationException("EF.Functions.JsonExtract method not found");

        var jsonExtractCall = Expression.Call(
            Expression.Property(null, functionsProperty),
            jsonExtractMethod,
            translationAccess,
            Expression.Constant("$.Translations." + locale)
        );

        // translationProperty != null
        var nullCheck = Expression.NotEqual(translationAccess, Expression.Constant(null, translationProperty.PropertyType));

        // EF.Functions.JsonExtract(...) ?? defaultProperty
        var coalesce = Expression.Coalesce(jsonExtractCall, defaultPropertyAccess);

        // translationProperty != null ? coalesce : defaultProperty
        return Expression.Condition(nullCheck, coalesce, defaultPropertyAccess);
    }

    /// <summary>
    /// Projects an EF Core query to a localized DTO with automatic tenant isolation.
    /// The projection expression should handle localization logic using EF Core JSON functions.
    ///
    /// Example usage:
    /// <code>
    /// var dto = await dbContext.Products
    ///     .ProjectToLocalized("de", p => new LocalizedProductDto
    ///     {
    ///         Id = p.Id,
    ///         Name = p.NameTranslations != null
    ///             ? EF.Functions.JsonExtract(p.NameTranslations, "de") ?? p.Name
    ///             : p.Name,
    ///         Price = p.Price
    ///     })
    ///     .FirstOrDefaultAsync();
    /// </code>
    /// </summary>
    /// <typeparam name="TEntity">The entity type (must implement ITenantEntity)</typeparam>
    /// <typeparam name="TDto">The DTO type</typeparam>
    /// <param name="query">The source query</param>
    /// <param name="languageCode">The language code (e.g., "en", "de")</param>
    /// <param name="projection">The projection expression that handles localization</param>
    /// <returns>A queryable with the DTO projection</returns>
    public static IQueryable<TDto> ProjectToLocalized<TEntity, TDto>(
        this IQueryable<TEntity> query,
        string languageCode,
        Expression<Func<TEntity, TDto>> projection)
        where TEntity : class, ITenantEntity
    {
        // Tenant isolation is automatically applied by TenantQueryInterceptor
        // Language localization is handled within the projection expression
        return query.Select(projection);
    }

    /// <summary>
    /// Helper method to get localized value with fallback to default.
    /// This is a convenience method for building projection expressions.
    ///
    /// Note: This method is not SQL-translatable. Use EF.Functions.JsonExtract
    /// directly in projection expressions for database-level localization.
    /// </summary>
    /// <param name="defaultValue">The default value</param>
    /// <param name="translations">The translations object</param>
    /// <param name="languageCode">The language code</param>
    /// <returns>The localized value or default</returns>
    public static string GetLocalizedValue(
        string defaultValue,
        LocalizedContent? translations,
        string languageCode)
    {
        return translations?.GetValue(languageCode) ?? defaultValue;
    }

    /// <summary>
    /// Extension method for LocalizedContent to support fluent chaining.
    /// Example: content.GetValue("de").IsNullOrEmpty()
    /// </summary>
    /// <param name="content">The localized content</param>
    /// <param name="languageCode">The language code</param>
    /// <returns>The localized value or null</returns>
    public static string? GetValueOrDefault(
        this LocalizedContent? content,
        string languageCode)
    {
        return content?.GetValue(languageCode);
    }
}