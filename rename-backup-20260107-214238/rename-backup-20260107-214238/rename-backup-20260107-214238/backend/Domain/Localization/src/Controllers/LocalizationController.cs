using B2X.LocalizationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.LocalizationService.Controllers;

/// <summary>
/// REST API endpoints for localization and translations
/// </summary>
[ApiController]
[Route("api/v1/localization")]
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
    /// Gets merged translations for a tenant and language (ADR-039)
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code (e.g., "en", "de")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Merged translations (global + tenant overrides)</returns>
    /// <response code="200">Returns merged translations</response>
    [HttpGet("tenant/{tenantId:guid}/{languageCode}")]
    public async Task<IActionResult> GetTenantTranslations(
        [FromRoute] Guid tenantId,
        [FromRoute] string languageCode,
        CancellationToken cancellationToken = default)
    {
        var translations = await _localizationService.GetMergedTranslationsAsync(tenantId, languageCode, cancellationToken);
        return Ok(new TenantTranslationsResponse
        {
            TenantId = tenantId,
            LanguageCode = languageCode,
            Translations = translations
        });
    }

    /// <summary>
    /// Gets SSR-optimized translations for a tenant and language (ADR-039)
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code (e.g., "en", "de")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SSR-optimized translations payload</returns>
    /// <response code="200">Returns SSR-optimized translations</response>
    [HttpGet("tenant/ssr/{tenantId:guid}/{languageCode}")]
    public async Task<IActionResult> GetTenantTranslationsSsr(
        [FromRoute] Guid tenantId,
        [FromRoute] string languageCode,
        CancellationToken cancellationToken = default)
    {
        var translations = await _localizationService.GetMergedTranslationsAsync(tenantId, languageCode, cancellationToken);
        return Ok(new SsrTranslationsResponse
        {
            TenantId = tenantId,
            LanguageCode = languageCode,
            Translations = translations,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Bulk upserts tenant translations (ADR-039)
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code</param>
    /// <param name="request">Bulk translation updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Translations updated successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - tenant admin required</response>
    [HttpPost("tenant/{tenantId:guid}/{languageCode}")]
    [Authorize]
    public async Task<IActionResult> UpsertTenantTranslations(
        [FromRoute] Guid tenantId,
        [FromRoute] string languageCode,
        [FromBody] BulkTranslationUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        // Verify tenant access
        var userTenantId = ExtractTenantId();
        if (userTenantId != tenantId && !User.IsInRole("SuperAdmin"))
        {
            return Forbid();
        }

        var userId = ExtractUserId();
        await _localizationService.BulkUpsertTranslationsAsync(
            tenantId, languageCode, request.Translations, userId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Resets a tenant translation key to global default (ADR-039)
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code</param>
    /// <param name="keyPath">The translation key path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Translation reset to default</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - tenant admin required</response>
    [HttpDelete("tenant/{tenantId:guid}/{languageCode}/{keyPath}")]
    [Authorize]
    public async Task<IActionResult> ResetTenantTranslation(
        [FromRoute] Guid tenantId,
        [FromRoute] string languageCode,
        [FromRoute] string keyPath,
        CancellationToken cancellationToken = default)
    {
        // Verify tenant access
        var userTenantId = ExtractTenantId();
        if (userTenantId != tenantId && !User.IsInRole("SuperAdmin"))
        {
            return Forbid();
        }

        await _localizationService.ResetTranslationToDefaultAsync(
            tenantId, languageCode, keyPath, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Extracts tenant ID from user claims if available
    /// </summary>
    private Guid? ExtractTenantId()
    {
        var tenantClaim = User.FindFirst("tenant");
        if (tenantClaim == null || !Guid.TryParse(tenantClaim.Value, out var tenantId))
        {
            return null;
        }

        return tenantId;
    }

    /// <summary>
    /// Extracts user ID from claims
    /// </summary>
    private Guid? ExtractUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
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
    public Dictionary<string, string> Translations { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Response model for tenant translations
/// </summary>
public class TenantTranslationsResponse
{
    /// <summary>Gets or sets the tenant ID</summary>
    public Guid TenantId { get; set; }

    /// <summary>Gets or sets the language code</summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the merged translations</summary>
    public Dictionary<string, string> Translations { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Response model for SSR translations
/// </summary>
public class SsrTranslationsResponse
{
    /// <summary>Gets or sets the tenant ID</summary>
    public Guid TenantId { get; set; }

    /// <summary>Gets or sets the language code</summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the merged translations</summary>
    public Dictionary<string, string> Translations { get; set; } = new(StringComparer.Ordinal);

    /// <summary>Gets or sets the response timestamp</summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Request model for bulk translation updates
/// </summary>
public class BulkTranslationUpdateRequest
{
    /// <summary>Gets or sets the translations to update</summary>
    public Dictionary<string, string> Translations { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Response model for supported languages
/// </summary>
public class LanguagesResponse
{
    /// <summary>Gets or sets the list of supported language codes</summary>
    public List<string> Languages { get; set; } = new();
}
