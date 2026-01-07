namespace B2X.LayoutService.Models;

/// <summary>
/// Localization helper for getting localized values
/// </summary>
public static class LocalizationHelper
{
    /// <summary>
    /// Get localized value with fallback chain:
    /// 1. Requested language
    /// 2. Default language
    /// 3. English fallback
    /// 4. Base value
    /// </summary>
    public static string GetLocalizedValue(
        string baseValue,
        IDictionary<string, string>? translations,
        string languageCode,
        string defaultLanguage = "en")
    {
        if (translations == null || translations.Count == 0)
        {
            return baseValue;
        }

        // Try requested language
        if (translations.TryGetValue(languageCode, out var value) && !string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Try default language if different from requested
        if (!string.Equals(languageCode, defaultLanguage, StringComparison.Ordinal) &&
            translations.TryGetValue(defaultLanguage, out var defaultValue) &&
            !string.IsNullOrEmpty(defaultValue))
        {
            return defaultValue;
        }

        // Try English as final fallback
        if (!string.Equals(languageCode, "en", StringComparison.Ordinal) && !string.Equals(defaultLanguage, "en", StringComparison.Ordinal) &&
            translations.TryGetValue("en", out var englishValue) &&
            !string.IsNullOrEmpty(englishValue))
        {
            return englishValue;
        }

        // Return base value
        return baseValue;
    }
}

/// <summary>
/// CMS Page DTO - Localized representation of CmsPage for API responses
/// </summary>
public class CmsPageDto
{
    /// <summary>Unique identifier for the page</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant ID for multi-tenant isolation</summary>
    public Guid TenantId { get; set; }

    /// <summary>Language code of this localized response (e.g., 'de', 'en')</summary>
    public string Language { get; set; } = null!;

    /// <summary>Localized page title</summary>
    public string Title { get; set; } = null!;

    /// <summary>Localized URL slug</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Localized page description</summary>
    public string Description { get; set; } = null!;

    /// <summary>Sections within this page (localized)</summary>
    public IList<CmsSectionDto> Sections { get; set; } = new List<CmsSectionDto>();

    /// <summary>Page visibility status</summary>
    public PageVisibility Visibility { get; set; } = PageVisibility.Draft;

    /// <summary>When the page was published</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Version number for tracking changes</summary>
    public int Version { get; set; } = 1;

    /// <summary>When the page was created</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>When the page was last modified</summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CMS Section DTO - Localized representation of CmsSection for API responses
/// </summary>
public class CmsSectionDto
{
    /// <summary>Unique identifier for the section</summary>
    public Guid Id { get; set; }

    /// <summary>Parent page ID</summary>
    public Guid PageId { get; set; }

    /// <summary>Section type (hero, features, testimonials, etc.)</summary>
    public string Type { get; set; } = null!;

    /// <summary>Display order within the page</summary>
    public int Order { get; set; }

    /// <summary>Layout configuration (full-width, 2-column, 3-column, etc.)</summary>
    public SectionLayout Layout { get; set; } = SectionLayout.FullWidth;

    /// <summary>Components within this section (localized)</summary>
    public IList<CmsComponentDto> Components { get; set; } = new List<CmsComponentDto>();

    /// <summary>Whether the section is visible on the live site</summary>
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// CMS Component DTO - Localized representation of CmsComponent for API responses
/// </summary>
public class CmsComponentDto
{
    /// <summary>Unique identifier for the component</summary>
    public Guid Id { get; set; }

    /// <summary>Parent section ID</summary>
    public Guid SectionId { get; set; }

    /// <summary>Language code of this localized response (e.g., 'de', 'en')</summary>
    public string Language { get; set; } = null!;

    /// <summary>Component type (button, text, image, product-card, etc.)</summary>
    public string Type { get; set; } = null!;

    /// <summary>Localized component content (text, HTML, or data reference)</summary>
    public string Content { get; set; } = null!;

    /// <summary>Whether the component is visible</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Component display order within section</summary>
    public int Order { get; set; }

    /// <summary>When the component was created</summary>
    public DateTime CreatedAt { get; set; }
}


/// <summary>
/// Request DTO for creating pages with multi-language support
/// </summary>
public class CreatePageRequestDto
{
    /// <summary>Page title in default language</summary>
    public string Title { get; set; } = null!;

    /// <summary>URL slug in default language</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Page description in default language</summary>
    public string Description { get; set; } = null!;

    /// <summary>Additional translations (optional)</summary>
    public IDictionary<string, CreatePageTranslationDto> Translations { get; set; } = new Dictionary<string, CreatePageTranslationDto>(StringComparer.Ordinal);
}

/// <summary>
/// Translation data for page creation
/// </summary>
public class CreatePageTranslationDto
{
    /// <summary>Language code (e.g., 'de', 'fr')</summary>
    public string LanguageCode { get; set; } = null!;

    /// <summary>Localized title</summary>
    public string Title { get; set; } = null!;

    /// <summary>Localized slug</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Localized description</summary>
    public string Description { get; set; } = null!;
}

/// <summary>
/// Request DTO for updating pages with multi-language support
/// </summary>
public class UpdatePageRequestDto
{
    /// <summary>Updated title (if provided)</summary>
    public string? Title { get; set; }

    /// <summary>Updated slug (if provided)</summary>
    public string? Slug { get; set; }

    /// <summary>Updated description (if provided)</summary>
    public string? Description { get; set; }

    /// <summary>Translation updates (optional)</summary>
    public IDictionary<string, UpdatePageTranslationDto>? Translations { get; set; }

    /// <summary>Updated visibility</summary>
    public PageVisibility? Visibility { get; set; }
}

/// <summary>
/// Translation update data
/// </summary>
public class UpdatePageTranslationDto
{
    /// <summary>Language code</summary>
    public string LanguageCode { get; set; } = null!;

    /// <summary>Updated title</summary>
    public string? Title { get; set; }

    /// <summary>Updated slug</summary>
    public string? Slug { get; set; }

    /// <summary>Updated description</summary>
    public string? Description { get; set; }
}
