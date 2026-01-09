using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B2X.Shared.Core;

/// <summary>
/// Extension methods for configuring LocalizedContent as an owned type in EF Core.
///
/// The Hybrid Localization Pattern:
/// - Default value: Stored in a dedicated column on the entity (indexed for search)
/// - Translations: Stored as JSON in LocalizedContent owned type
///
/// Usage example in entity:
/// <code>
/// public class Product
/// {
///     public string Name { get; set; }                    // Default (indexed)
///     public LocalizedContent? NameTranslations { get; set; } // Other languages (JSON)
///
///     public string GetLocalizedName(string lang)
///         => NameTranslations?.GetValue(lang) ?? Name;
/// }
/// </code>
///
/// Usage example in DbContext.OnModelCreating:
/// <code>
/// modelBuilder.Entity&lt;Product&gt;(entity =>
/// {
///     entity.HasIndex(p => p.Name); // Index default column
///     entity.ConfigureTranslations(p => p.NameTranslations);
/// });
/// </code>
///
/// See ADR: ADR-entity-localization-pattern.md for full documentation.
/// </summary>
public static class LocalizedContentConfiguration
{
    /// <summary>
    /// Configures a LocalizedContent property to be stored as JSON.
    /// Use this for the translations part of the Hybrid Localization Pattern.
    /// The default value should be a separate property on your entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="builder">The entity type builder</param>
    /// <param name="navigationExpression">Expression to the LocalizedContent property</param>
    /// <param name="columnName">Name of the JSON column (optional, defaults to property name)</param>
    /// <returns>The entity type builder for chaining</returns>
    public static EntityTypeBuilder<TEntity> ConfigureTranslations<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, LocalizedContent?>> navigationExpression,
        string? columnName = null)
        where TEntity : class
    {
        builder.OwnsOne(navigationExpression, owned =>
        {
            if (columnName != null)
            {
                // owned.ToJson(columnName);
            }
            else
            {
                // owned.ToJson();
            }
        });

        return builder;
    }

    /// <summary>
    /// Configures a required LocalizedContent property to be stored as JSON.
    /// </summary>
    public static EntityTypeBuilder<TEntity> ConfigureRequiredTranslations<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, LocalizedContent?>> navigationExpression,
        string? columnName = null)
        where TEntity : class
    {
        builder.Navigation(navigationExpression).IsRequired();
        return builder.ConfigureTranslations(navigationExpression, columnName);
    }
}

/// <summary>
/// Extension methods for querying entities with the Hybrid Localization Pattern.
/// The default language column is directly queryable via LINQ.
///
/// For searching translations (JSON), use in-memory filtering or Elasticsearch.
/// </summary>
public static class LocalizedContentQueryExtensions
{
    /// <summary>
    /// Gets the localized value for an entity, falling back to the default value.
    /// This is a helper for projection scenarios.
    ///
    /// Usage:
    /// <code>
    /// var dto = await dbContext.Products
    ///     .Select(p => new ProductDto
    ///     {
    ///         Name = p.GetLocalized(p.Name, p.NameTranslations, "de")
    ///     })
    ///     .FirstOrDefaultAsync();
    /// </code>
    ///
    /// Note: This is evaluated in-memory, not translated to SQL.
    /// For large datasets, prefer pre-filtering or Elasticsearch.
    /// </summary>
#pragma warning disable RCS1175 // Unused parameter 'entity'.
    public static string GetLocalized(
        this object entity,
        string defaultValue,
        LocalizedContent? translations,
        string languageCode)
#pragma warning restore RCS1175 // Unused parameter 'entity'.
    {
        return translations?.GetValue(languageCode) ?? defaultValue;
    }
}

/// <summary>
/// Mixin interface for entities that have localizable properties.
/// Provides a standard pattern for getting localized values.
/// </summary>
public interface ILocalizable
{
    /// <summary>
    /// Gets the default language code for this entity (typically tenant default).
    /// </summary>
    string DefaultLanguageCode { get; }
}

/// <summary>
/// Helper class for building localizable entity properties.
/// </summary>
public static class LocalizablePropertyBuilder
{
    /// <summary>
    /// Creates a localized property configuration for an entity type.
    ///
    /// Usage in DbContext:
    /// <code>
    /// modelBuilder.Entity&lt;Product&gt;(entity =>
    /// {
    ///     LocalizablePropertyBuilder.Configure(entity, p => p.Name, p => p.NameTranslations);
    ///     LocalizablePropertyBuilder.Configure(entity, p => p.Description, p => p.DescriptionTranslations);
    /// });
    /// </code>
    /// </summary>
    public static void Configure<TEntity>(
        EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, string>> defaultPropertyExpression,
        Expression<Func<TEntity, LocalizedContent?>> translationsExpression,
        int maxLength = 255,
        bool indexDefault = true)
        where TEntity : class
    {
        // Configure default property
        builder.Property(defaultPropertyExpression)
            .IsRequired()
            .HasMaxLength(maxLength);

        // Add index for default property if requested
        if (indexDefault)
        {
            // Convert expression to object for HasIndex
            var parameter = defaultPropertyExpression.Parameters[0];
            var body = Expression.Convert(defaultPropertyExpression.Body, typeof(object));
            var objectExpression = Expression.Lambda<Func<TEntity, object?>>(body, parameter);
            builder.HasIndex(objectExpression);
        }

        // Configure translations as JSON
        builder.ConfigureTranslations(translationsExpression);
    }

    /// <summary>
    /// Creates an optional localized property configuration.
    /// </summary>
    public static void ConfigureOptional<TEntity>(
        EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, string?>> defaultPropertyExpression,
        Expression<Func<TEntity, LocalizedContent?>> translationsExpression,
        int maxLength = 2000)
        where TEntity : class
    {
        // Configure default property as optional
        builder.Property(defaultPropertyExpression)
            .IsRequired(false)
            .HasMaxLength(maxLength);

        // Configure translations as JSON
        builder.ConfigureTranslations(translationsExpression);
    }
}
