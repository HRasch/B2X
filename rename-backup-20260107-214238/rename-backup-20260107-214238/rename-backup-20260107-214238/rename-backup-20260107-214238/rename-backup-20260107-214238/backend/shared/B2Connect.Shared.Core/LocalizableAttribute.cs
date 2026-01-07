using System;

namespace B2Connect.Shared.Core;

/// <summary>
/// Attribute to mark DTO properties as localizable.
/// Enables automatic projection from entity default values and LocalizedContent translations.
/// Used with the Hybrid Localization Pattern (default column + JSON translations).
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class LocalizableAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the entity property containing the default value.
    /// If not specified, defaults to the DTO property name.
    /// </summary>
    public string? DefaultProperty { get; set; }

    /// <summary>
    /// Gets or sets the name of the entity property containing the LocalizedContent translations.
    /// If not specified, defaults to DTO property name + "Translations".
    /// </summary>
    public string? TranslationProperty { get; set; }

    /// <summary>
    /// Initializes a new instance of the LocalizableAttribute class.
    /// </summary>
    public LocalizableAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance with the default property name.
    /// The translation property will default to DefaultProperty + "Translations".
    /// </summary>
    /// <param name="defaultProperty">The name of the entity property containing the default value.</param>
    public LocalizableAttribute(string defaultProperty)
    {
        DefaultProperty = defaultProperty;
        TranslationProperty = defaultProperty + "Translations";
    }

    /// <summary>
    /// Initializes a new instance with both property names.
    /// </summary>
    /// <param name="defaultProperty">The name of the entity property containing the default value.</param>
    /// <param name="param name="translationProperty">The name of the entity property containing the LocalizedContent translations.</param>
    public LocalizableAttribute(string defaultProperty, string translationProperty)
    {
        DefaultProperty = defaultProperty;
        TranslationProperty = translationProperty;
    }
}
