using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2Connect.LocalizationService.Services;

namespace B2Connect.LocalizationService.Controllers;

/// <summary>
/// REST API endpoints for localization and translations
/// </summary>
[ApiController]
[Route("api/localization")]
[Produces("application/json")]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
    }

    /// <summary>
    /// Gets a translated string by key and category
    /// </summary>
    /// <param name="category">The translation category (e.g., "auth", "ui", "errors")</param>
    /// <param name="key">The translation key (e.g., "login", "required")</param>
    /// <param name="language">The language code (e.g., "en", "de"). Defaults to "en"</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The translated string for the given language, or English fallback</returns>
    /// <response code="200">Returns the translated string</response>
    [HttpGet("{category}/{key}")]
    public async Task<IActionResult> GetString(
        [FromRoute] string category,
        [FromRoute] string key,
        [FromQuery] string language = "en",
        CancellationToken cancellationToken = default)
    {
        var value = await _localizationService.GetStringAsync(key, category, language, cancellationToken);
        return Ok(new LocalizedStringResponse
        {
            Key = $"{category}.{key}",
            Value = value,
            Language = language
        });
    }

    /// <summary>
    /// Gets all translations for a category and language
    /// </summary>
    /// <param name="category">The translation category</param>
    /// <param name="language">The language code. Defaults to "en"</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of all translations in the category</returns>
    /// <response code="200">Returns all translations for the category</response>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetCategory(
        [FromRoute] string category,
        [FromQuery] string language = "en",
        CancellationToken cancellationToken = default)
    {
        var translations = await _localizationService.GetCategoryAsync(category, language, cancellationToken);
        return Ok(new CategoryResponse
        {
            Category = category,
            Language = language,
            Translations = translations
        });
    }

    /// <summary>
    /// Gets all supported language codes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of supported language codes</returns>
    /// <response code="200">Returns list of supported languages</response>
    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages(CancellationToken cancellationToken = default)
    {
        var languages = await _localizationService.GetSupportedLanguagesAsync(cancellationToken);
        return Ok(new LanguagesResponse { Languages = languages.ToList() });
    }

    /// <summary>
    /// Sets or updates translations for a key (Admin only)
    /// </summary>
    /// <param name="category">The translation category</param>
    /// <param name="key">The translation key</param>
    /// <param name="request">Dictionary of language codes to translated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Translations updated successfully</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="403">Forbidden - admin role required</response>
    [HttpPost("{category}/{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetString(
        [FromRoute] string category,
        [FromRoute] string key,
        [FromBody] Dictionary<string, string> request,
        CancellationToken cancellationToken = default)
    {
        // Check admin role (attribute should prevent this, but being explicit)
        if (!User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var tenantId = ExtractTenantId();
        await _localizationService.SetStringAsync(tenantId, key, category, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Extracts tenant ID from user claims if available
    /// </summary>
    private Guid? ExtractTenantId()
    {
        var tenantClaim = User.FindFirst("tenant");
        if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
        {
            return tenantId;
        }
        return null;
    }
}

/// <summary>
/// Response model for a single localized string
/// </summary>
public class LocalizedStringResponse
{
    /// <summary>Gets or sets the full key (category.key)</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Gets or sets the translated value</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>Gets or sets the language code used</summary>
    public string Language { get; set; } = string.Empty;
}

/// <summary>
/// Response model for category translations
/// </summary>
public class CategoryResponse
{
    /// <summary>Gets or sets the category name</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets the language code</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the translations dictionary</summary>
    public Dictionary<string, string> Translations { get; set; } = new();
}

/// <summary>
/// Response model for supported languages
/// </summary>
public class LanguagesResponse
{
    /// <summary>Gets or sets the list of supported language codes</summary>
    public List<string> Languages { get; set; } = new();
}
