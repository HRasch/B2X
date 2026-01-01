using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using System.Text.Json;

namespace B2Connect.Shared.Core;

/// <summary>
/// Configuration options for LocalizedContent in EF Core.
/// </summary>
public class LocalizedContentOptions
{
    /// <summary>
    /// Languages that get their own indexed column for fast queries.
    /// Default: German and English.
    /// </summary>
    public string[] IndexedLanguages { get; set; } = { "de", "en" };

    /// <summary>
    /// Whether to include a combined search text column for full-text search across all languages.
    /// </summary>
    public bool IncludeSearchColumn { get; set; } = false;

    /// <summary>
    /// Maximum length for each language column.
    /// </summary>
    public int MaxLength { get; set; } = 5000;
}

/// <summary>
/// Extension methods for configuring LocalizedContent as an owned entity type in EF Core.
/// 
/// Usage example in DbContext.OnModelCreating:
/// <code>
/// modelBuilder.Entity&lt;Product&gt;()
///     .ConfigureLocalizedContent(p => p.Name, "Name", new LocalizedContentOptions
///     {
///         IndexedLanguages = new[] { "de", "en", "fr" },
///         IncludeSearchColumn = true
///     });
/// </code>
/// 
/// Generated columns:
/// - Name_Default (always)
/// - Name_Translations (JSON, always)
/// - Name_de (indexed, if in IndexedLanguages)
/// - Name_en (indexed, if in IndexedLanguages)
/// - Name_SearchText (optional, for full-text search)
/// </summary>
public static class LocalizedContentConfiguration
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Configures a LocalizedContent property with language-specific indexed columns.
    /// </summary>
    public static EntityTypeBuilder<TEntity> ConfigureLocalizedContent<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, LocalizedContent?>> navigationExpression,
        string columnPrefix,
        LocalizedContentOptions? options = null)
        where TEntity : class
    {
        options ??= new LocalizedContentOptions();

        // Configure owned type for DefaultValue and Translations (JSON)
        builder.OwnsOne(navigationExpression, localizedContent =>
        {
            localizedContent.Property(lc => lc.DefaultValue)
                .HasColumnName($"{columnPrefix}_Default")
                .IsRequired()
                .HasMaxLength(options.MaxLength);

            localizedContent.Property(lc => lc.Translations)
                .HasColumnName($"{columnPrefix}_Translations")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonOptions) ?? new Dictionary<string, string>()
                )
                .Metadata.SetValueComparer(
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyDictionary<string, string>>(
                        (c1, c2) => (c1 ?? new Dictionary<string, string>()).SequenceEqual(c2 ?? new Dictionary<string, string>()),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => new Dictionary<string, string>(c ?? new Dictionary<string, string>())
                    )
                );
        });

        // Add indexed shadow properties for each configured language
        foreach (var lang in options.IndexedLanguages)
        {
            var propertyName = $"{columnPrefix}_{lang.ToUpperInvariant()}";

            builder.Property<string?>(propertyName)
                .HasColumnName(propertyName)
                .HasMaxLength(options.MaxLength)
                .IsRequired(false);

            // Create index for this language column
            builder.HasIndex(propertyName)
                .HasDatabaseName($"IX_{typeof(TEntity).Name}_{columnPrefix}_{lang.ToUpperInvariant()}");
        }

        // Optional: Combined search text column for full-text search
        if (options.IncludeSearchColumn)
        {
            builder.Property<string?>($"{columnPrefix}_SearchText")
                .HasColumnName($"{columnPrefix}_SearchText")
                .HasMaxLength(options.MaxLength * 2)
                .IsRequired(false);

            builder.HasIndex($"{columnPrefix}_SearchText")
                .HasDatabaseName($"IX_{typeof(TEntity).Name}_{columnPrefix}_Search");
        }

        return builder;
    }

    /// <summary>
    /// Configures a required LocalizedContent property with language-specific indexed columns.
    /// </summary>
    public static EntityTypeBuilder<TEntity> ConfigureRequiredLocalizedContent<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, LocalizedContent?>> navigationExpression,
        string columnPrefix,
        LocalizedContentOptions? options = null)
        where TEntity : class
    {
        builder.Navigation(navigationExpression).IsRequired();
        return builder.ConfigureLocalizedContent(navigationExpression, columnPrefix, options);
    }

    /// <summary>
    /// Updates all language-specific shadow properties for an entity's LocalizedContent field.
    /// Call this in SaveChanges override or via interceptor.
    /// </summary>
    public static void SyncLocalizedColumns<TEntity>(
        this DbContext context,
        TEntity entity,
        Func<TEntity, LocalizedContent?> propertyAccessor,
        string columnPrefix,
        LocalizedContentOptions? options = null) where TEntity : class
    {
        options ??= new LocalizedContentOptions();
        var localizedContent = propertyAccessor(entity);
        var entry = context.Entry(entity);

        // Sync each indexed language column
        foreach (var lang in options.IndexedLanguages)
        {
            var propertyName = $"{columnPrefix}_{lang.ToUpperInvariant()}";
            var value = localizedContent?.GetValue(lang);
            entry.Property(propertyName).CurrentValue = value;
        }

        // Sync search text column if enabled
        if (options.IncludeSearchColumn)
        {
            var searchText = localizedContent?.GetSearchableText();
            entry.Property($"{columnPrefix}_SearchText").CurrentValue = searchText;
        }
    }
}

/// <summary>
/// Extension methods for querying entities with LocalizedContent using LINQ.
/// Enables efficient searching by language using indexed columns.
/// </summary>
public static class LocalizedContentQueryExtensions
{
    /// <summary>
    /// Filters entities where the localized content for a specific language contains the search term.
    /// Uses the indexed language column for optimal performance.
    /// 
    /// Usage:
    /// <code>
    /// var products = await dbContext.Products
    ///     .WhereLocalizedContains("Name", "de", "Laptop")
    ///     .ToListAsync();
    /// </code>
    /// </summary>
    public static IQueryable<TEntity> WhereLocalizedContains<TEntity>(
        this IQueryable<TEntity> query,
        string columnPrefix,
        string language,
        string searchTerm) where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var propertyName = $"{columnPrefix}_{language.ToUpperInvariant()}";

        return query.Where(e =>
            EF.Property<string>(e, propertyName) != null &&
            EF.Property<string>(e, propertyName).Contains(searchTerm));
    }

    /// <summary>
    /// Filters entities where the localized content equals the search term exactly.
    /// Uses the indexed language column for optimal performance.
    /// </summary>
    public static IQueryable<TEntity> WhereLocalizedEquals<TEntity>(
        this IQueryable<TEntity> query,
        string columnPrefix,
        string language,
        string value) where TEntity : class
    {
        var propertyName = $"{columnPrefix}_{language.ToUpperInvariant()}";

        return query.Where(e => EF.Property<string>(e, propertyName) == value);
    }

    /// <summary>
    /// Filters entities where the localized content starts with the search term.
    /// Ideal for autocomplete scenarios.
    /// </summary>
    public static IQueryable<TEntity> WhereLocalizedStartsWith<TEntity>(
        this IQueryable<TEntity> query,
        string columnPrefix,
        string language,
        string searchTerm) where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var propertyName = $"{columnPrefix}_{language.ToUpperInvariant()}";

        return query.Where(e =>
            EF.Property<string>(e, propertyName) != null &&
            EF.Property<string>(e, propertyName).StartsWith(searchTerm));
    }

    /// <summary>
    /// Filters entities searching across all indexed languages.
    /// Falls back to SearchText column if available for non-indexed languages.
    /// </summary>
    public static IQueryable<TEntity> WhereAnyLanguageContains<TEntity>(
        this IQueryable<TEntity> query,
        string columnPrefix,
        string searchTerm,
        params string[] languages) where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || languages.Length == 0)
            return query;

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? combinedExpression = null;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        foreach (var lang in languages)
        {
            var propertyName = $"{columnPrefix}_{lang.ToUpperInvariant()}";

            var propertyAccess = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(string) },
                parameter,
                Expression.Constant(propertyName)
            );

            var notNullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var containsCall = Expression.Call(propertyAccess, containsMethod, Expression.Constant(searchTerm));
            var nullSafeContains = Expression.AndAlso(notNullCheck, containsCall);

            combinedExpression = combinedExpression == null
                ? nullSafeContains
                : Expression.OrElse(combinedExpression, nullSafeContains);
        }

        if (combinedExpression == null)
            return query;

        var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Orders entities by localized content in a specific language.
    /// </summary>
    public static IOrderedQueryable<TEntity> OrderByLocalized<TEntity>(
        this IQueryable<TEntity> query,
        string columnPrefix,
        string language) where TEntity : class
    {
        var propertyName = $"{columnPrefix}_{language.ToUpperInvariant()}";
        return query.OrderBy(e => EF.Property<string>(e, propertyName));
    }

    /// <summary>
    /// Orders entities by localized content in a specific language (descending).
    /// </summary>
    public static IOrderedQueryable<TEntity> OrderByLocalizedDescending<TEntity>(
        this IQueryable<TEntity> query,
        string columnPrefix,
        string language) where TEntity : class
    {
        var propertyName = $"{columnPrefix}_{language.ToUpperInvariant()}";
        return query.OrderByDescending(e => EF.Property<string>(e, propertyName));
    }
}
