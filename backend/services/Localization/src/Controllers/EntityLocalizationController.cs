using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2Connect.LocalizationService.Services;
using B2Connect.Types.Localization;
using B2Connect.Types.Extensions;

namespace B2Connect.LocalizationService.Controllers;

/// <summary>
/// REST API endpoints for entity-based localization
/// Handles translations stored directly in entity records as JSON
/// </summary>
[ApiController]
[Route("api/entity-localization")]
[Produces("application/json")]
[Authorize]
public class EntityLocalizationController : ControllerBase
{
    private readonly IEntityLocalizationService _entityLocalizationService;
    private readonly ILogger<EntityLocalizationController> _logger;

    public EntityLocalizationController(
        IEntityLocalizationService entityLocalizationService,
        ILogger<EntityLocalizationController> logger)
    {
        _entityLocalizationService = entityLocalizationService
            ?? throw new ArgumentNullException(nameof(entityLocalizationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a translation for a specific entity property and language
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="propertyName">The name of the localized property</param>
    /// <param name="language">The language code (e.g., "en", "de")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The translated value for the property</returns>
    /// <response code="200">Returns the translated value</response>
    /// <response code="404">Entity or property not found</response>
    [HttpGet("{entityId}/{propertyName}/{language}")]
    public async Task<IActionResult> GetPropertyTranslation(
        [FromRoute] Guid entityId,
        [FromRoute] string propertyName,
        [FromRoute] string language,
        CancellationToken cancellationToken = default)
    {
        var tenantId = ExtractTenantId();
        if (!tenantId.HasValue)
        {
            return BadRequest("Tenant ID is required");
        }

        try
        {
            var value = await _entityLocalizationService.GetPropertyTranslationAsync(
                entityId,
                propertyName,
                language,
                tenantId.Value,
                cancellationToken);

            if (value is null)
            {
                return NotFound($"Translation not found for property '{propertyName}' in language '{language}'");
            }

            return Ok(new PropertyTranslationResponse
            {
                EntityId = entityId,
                PropertyName = propertyName,
                Language = language,
                Value = value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving translation for entity {EntityId}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the translation");
        }
    }

    /// <summary>
    /// Gets all translations for a specific entity property
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="propertyName">The name of the localized property</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of all language codes to translated values</returns>
    /// <response code="200">Returns all translations for the property</response>
    /// <response code="404">Entity or property not found</response>
    [HttpGet("{entityId}/{propertyName}")]
    public async Task<IActionResult> GetPropertyTranslations(
        [FromRoute] Guid entityId,
        [FromRoute] string propertyName,
        CancellationToken cancellationToken = default)
    {
        var tenantId = ExtractTenantId();
        if (!tenantId.HasValue)
        {
            return BadRequest("Tenant ID is required");
        }

        try
        {
            var translations = await _entityLocalizationService.GetPropertyTranslationsAsync(
                entityId,
                propertyName,
                tenantId.Value,
                cancellationToken);

            if (translations.Count == 0)
            {
                return NotFound($"No translations found for property '{propertyName}'");
            }

            return Ok(new PropertyTranslationsResponse
            {
                EntityId = entityId,
                PropertyName = propertyName,
                Translations = translations,
                AvailableLanguages = translations.Keys.ToList(),
                TotalTranslations = translations.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving translations for entity {EntityId}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving translations");
        }
    }

    /// <summary>
    /// Gets all localized properties for an entity
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of property names to their LocalizedContent objects</returns>
    /// <response code="200">Returns all localized properties</response>
    /// <response code="404">Entity not found</response>
    [HttpGet("{entityId}")]
    public async Task<IActionResult> GetEntityLocalizations(
        [FromRoute] Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = ExtractTenantId();
        if (!tenantId.HasValue)
        {
            return BadRequest("Tenant ID is required");
        }

        try
        {
            var localizations = await _entityLocalizationService.GetEntityLocalizationsAsync(
                entityId,
                tenantId.Value,
                cancellationToken);

            if (localizations.Count == 0)
            {
                return NotFound($"No localized properties found for entity {entityId}");
            }

            return Ok(new EntityLocalizationResponse
            {
                EntityId = entityId,
                Properties = localizations.Keys.ToList(),
                TotalProperties = localizations.Count,
                Localizations = localizations.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new PropertyLocalizationDetail
                    {
                        PropertyName = kvp.Key,
                        DefaultLanguage = kvp.Value.DefaultLanguage,
                        AvailableLanguages = kvp.Value.GetAvailableLanguages().ToList(),
                        TranslationCount = kvp.Value.Count
                    })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving localizations for entity {EntityId}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving localizations");
        }
    }

    /// <summary>
    /// Sets a translation for an entity property (Admin only)
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="propertyName">The name of the localized property</param>
    /// <param name="request">The translation request with language and value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Translation set successfully</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="403">Forbidden - admin role required</response>
    [HttpPost("{entityId}/{propertyName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetPropertyTranslation(
        [FromRoute] Guid entityId,
        [FromRoute] string propertyName,
        [FromBody] SetPropertyTranslationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Language) || string.IsNullOrWhiteSpace(request.Value))
        {
            return BadRequest("Language and Value are required");
        }

        var tenantId = ExtractTenantId();
        if (!tenantId.HasValue)
        {
            return BadRequest("Tenant ID is required");
        }

        try
        {
            var success = await _entityLocalizationService.SetPropertyTranslationAsync(
                entityId,
                propertyName,
                request.Language,
                request.Value,
                tenantId.Value,
                cancellationToken);

            if (!success)
            {
                return BadRequest($"Failed to set translation for property '{propertyName}'");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting translation for entity {EntityId}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while setting the translation");
        }
    }

    /// <summary>
    /// Sets multiple translations for an entity property at once (Admin only)
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="propertyName">The name of the localized property</param>
    /// <param name="request">Dictionary of language codes to translated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Translations set successfully</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="403">Forbidden - admin role required</response>
    [HttpPut("{entityId}/{propertyName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetPropertyTranslations(
        [FromRoute] Guid entityId,
        [FromRoute] string propertyName,
        [FromBody] Dictionary<string, string> request,
        CancellationToken cancellationToken = default)
    {
        if (request is null || request.Count == 0)
        {
            return BadRequest("At least one translation is required");
        }

        var tenantId = ExtractTenantId();
        if (!tenantId.HasValue)
        {
            return BadRequest("Tenant ID is required");
        }

        try
        {
            var success = await _entityLocalizationService.SetPropertyTranslationsAsync(
                entityId,
                propertyName,
                request,
                tenantId.Value,
                cancellationToken);

            if (!success)
            {
                return BadRequest($"Failed to set translations for property '{propertyName}'");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting translations for entity {EntityId}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while setting translations");
        }
    }

    /// <summary>
    /// Validates that required languages are present for an entity property
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="propertyName">The name of the localized property</param>
    /// <param name="request">Validation request with required languages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with missing languages</returns>
    /// <response code="200">Returns validation result</response>
    /// <response code="404">Entity or property not found</response>
    [HttpPost("{entityId}/{propertyName}/validate")]
    public async Task<IActionResult> ValidatePropertyLanguages(
        [FromRoute] Guid entityId,
        [FromRoute] string propertyName,
        [FromBody] ValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request?.RequiredLanguages is null || request.RequiredLanguages.Length == 0)
        {
            return BadRequest("At least one required language must be specified");
        }

        var tenantId = ExtractTenantId();
        if (!tenantId.HasValue)
        {
            return BadRequest("Tenant ID is required");
        }

        try
        {
            var isValid = await _entityLocalizationService.ValidatePropertyLanguagesAsync(
                entityId,
                propertyName,
                request.RequiredLanguages,
                tenantId.Value,
                cancellationToken);

            if (!isValid)
            {
                var missingLanguages = await _entityLocalizationService.GetMissingLanguagesAsync(
                    entityId,
                    propertyName,
                    request.RequiredLanguages,
                    tenantId.Value,
                    cancellationToken);

                return Ok(new ValidationResponse
                {
                    IsValid = false,
                    RequiredLanguages = request.RequiredLanguages.ToList(),
                    MissingLanguages = missingLanguages,
                    Message = $"Missing translations for languages: {string.Join(", ", missingLanguages)}"
                });
            }

            return Ok(new ValidationResponse
            {
                IsValid = true,
                RequiredLanguages = request.RequiredLanguages.ToList(),
                MissingLanguages = new List<string>(),
                Message = "All required language translations are present"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating translations for entity {EntityId}", entityId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while validating translations");
        }
    }

    /// <summary>
    /// Extracts tenant ID from user claims
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
/// Request model for setting a single property translation
/// </summary>
public class SetPropertyTranslationRequest
{
    /// <summary>Gets or sets the language code (e.g., "en", "de")</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the translated value</summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Request model for property language validation
/// </summary>
public class ValidationRequest
{
    /// <summary>Gets or sets the array of required language codes</summary>
    public string[] RequiredLanguages { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Response model for a single property translation
/// </summary>
public class PropertyTranslationResponse
{
    /// <summary>Gets or sets the entity ID</summary>
    public Guid EntityId { get; set; }

    /// <summary>Gets or sets the property name</summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>Gets or sets the language code</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>Gets or sets the translated value</summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Response model for multiple property translations
/// </summary>
public class PropertyTranslationsResponse
{
    /// <summary>Gets or sets the entity ID</summary>
    public Guid EntityId { get; set; }

    /// <summary>Gets or sets the property name</summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>Gets or sets the translations dictionary</summary>
    public Dictionary<string, string> Translations { get; set; } = new();

    /// <summary>Gets or sets the list of available languages</summary>
    public List<string> AvailableLanguages { get; set; } = new();

    /// <summary>Gets or sets the total number of translations</summary>
    public int TotalTranslations { get; set; }
}

/// <summary>
/// Response model for all entity localizations
/// </summary>
public class EntityLocalizationResponse
{
    /// <summary>Gets or sets the entity ID</summary>
    public Guid EntityId { get; set; }

    /// <summary>Gets or sets the list of localized property names</summary>
    public List<string> Properties { get; set; } = new();

    /// <summary>Gets or sets the total number of localized properties</summary>
    public int TotalProperties { get; set; }

    /// <summary>Gets or sets detailed localization information for each property</summary>
    public Dictionary<string, PropertyLocalizationDetail> Localizations { get; set; } = new();
}

/// <summary>
/// Detailed information about a property's localizations
/// </summary>
public class PropertyLocalizationDetail
{
    /// <summary>Gets or sets the property name</summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>Gets or sets the default language code</summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>Gets or sets the list of available languages</summary>
    public List<string> AvailableLanguages { get; set; } = new();

    /// <summary>Gets or sets the total number of translations</summary>
    public int TranslationCount { get; set; }
}

/// <summary>
/// Response model for translation validation
/// </summary>
public class ValidationResponse
{
    /// <summary>Gets or sets whether all required translations are present</summary>
    public bool IsValid { get; set; }

    /// <summary>Gets or sets the list of required languages</summary>
    public List<string> RequiredLanguages { get; set; } = new();

    /// <summary>Gets or sets the list of missing languages</summary>
    public List<string> MissingLanguages { get; set; } = new();

    /// <summary>Gets or sets a message describing the validation result</summary>
    public string Message { get; set; } = string.Empty;
}
