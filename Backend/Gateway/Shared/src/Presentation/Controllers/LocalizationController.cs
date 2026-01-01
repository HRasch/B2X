using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.LocalizationService.Services;
using B2Connect.LocalizationService;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;

namespace B2Connect.Gateway.Shared.Presentation.Controllers;

/// <summary>
/// Localization Controller - Demonstrates ambient tenant context pattern
///
/// This controller shows how the TenantContext automatically provides
/// the current tenant ID without explicit parameter passing.
/// </summary>
[ApiController]
[Route("api/localization")]
[Produces("application/json")]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _localizationService;
    private readonly IEntityLocalizationService _entityLocalizationService;
    private readonly LocalizationDbContext _dbContext;
    private readonly ILogger<LocalizationController> _logger;

    public LocalizationController(
        ILocalizationService localizationService,
        IEntityLocalizationService entityLocalizationService,
        LocalizationDbContext dbContext,
        ILogger<LocalizationController> logger)
    {
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        _entityLocalizationService = entityLocalizationService ?? throw new ArgumentNullException(nameof(entityLocalizationService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a localized string using ambient tenant context
    /// </summary>
    /// <param name="key">Translation key</param>
    /// <param name="category">Translation category</param>
    /// <returns>Localized string</returns>
    [HttpGet("string/{key}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IActionResult> GetLocalizedString(string key, [FromQuery] string category = "ui")
    {
        try
        {
            // Ambient pattern: tenant ID comes from TenantContext automatically
            var localizedString = await _localizationService.GetStringAsync(key, category);

            // Log current tenant context for demonstration
            var tenantId = TenantContext.CurrentTenantId;
            _logger.LogInformation("Retrieved localized string '{Key}' for tenant {TenantId}", key, tenantId);

            return Ok(new
            {
                Key = key,
                Category = category,
                Value = localizedString,
                TenantId = tenantId,
                Language = _localizationService.GetCurrentLanguage()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving localized string for key '{Key}'", key);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Sets a localized string using ambient tenant context
    /// </summary>
    /// <param name="key">Translation key</param>
    /// <param name="request">Translation data</param>
    /// <returns>Success status</returns>
    [HttpPost("string/{key}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SetLocalizedString(string key, [FromBody] SetTranslationRequest request)
    {
        try
        {
            // Ambient pattern: tenant ID comes from TenantContext automatically
            await _localizationService.SetStringAsync(key, request.Category, request.Translations);

            // Log current tenant context for demonstration
            var tenantId = TenantContext.CurrentTenantId;
            _logger.LogInformation("Set localized string '{Key}' for tenant {TenantId}", key, tenantId);

            return Ok(new
            {
                Key = key,
                Category = request.Category,
                TenantId = tenantId,
                Languages = request.Translations.Keys.ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting localized string for key '{Key}'", key);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets entity property translation using ambient tenant context
    /// </summary>
    /// <param name="entityId">Entity ID</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>Localized property value</returns>
    [HttpGet("entity/{entityId}/property/{propertyName}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IActionResult> GetEntityPropertyTranslation(Guid entityId, string propertyName)
    {
        try
        {
            // Ambient pattern: tenant ID comes from TenantContext automatically
            var translation = await _entityLocalizationService.GetPropertyTranslationAsync(
                entityId, propertyName, _localizationService.GetCurrentLanguage());

            // Log current tenant context for demonstration
            var tenantId = TenantContext.CurrentTenantId;
            _logger.LogInformation("Retrieved entity property translation for entity {EntityId}, property '{Property}' for tenant {TenantId}",
                entityId, propertyName, tenantId);

            return Ok(new
            {
                EntityId = entityId,
                PropertyName = propertyName,
                Value = translation,
                TenantId = tenantId,
                Language = _localizationService.GetCurrentLanguage()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity property translation for entity {EntityId}", entityId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Demonstrates automatic tenant filtering in EF queries
    /// </summary>
    /// <returns>All localized strings for current tenant</returns>
    [HttpGet("tenant-strings")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetTenantStrings()
    {
        try
        {
            // This demonstrates automatic tenant filtering!
            // The query is automatically filtered by TenantContext.CurrentTenantId
            // No need to manually add .Where(x => x.TenantId == tenantId)

            var strings = await _dbContext.LocalizedStrings
                .Where(x => x.Category == "ui")
                .OrderBy(x => x.Key)
                .Take(10)
                .Select(x => new
                {
                    x.Key,
                    x.Category,
                    x.DefaultValue,
                    Translations = x.Translations,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                TenantId = TenantContext.CurrentTenantId,
                Count = strings.Count,
                Strings = strings
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant strings");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Demonstrates automatic tenant assignment on save
    /// </summary>
    /// <param name="request">Translation to create</param>
    /// <returns>Created translation info</returns>
    [HttpPost("tenant-strings")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> CreateTenantString([FromBody] CreateTranslationRequest request)
    {
        try
        {
            var localizedString = new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = request.Key,
                Category = request.Category,
                Translations = request.Translations,
                DefaultValue = request.Translations.GetValueOrDefault("en") ?? request.Key,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
                // TenantId is automatically set by the interceptor!
            };

            _dbContext.LocalizedStrings.Add(localizedString);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Id = localizedString.Id,
                Key = localizedString.Key,
                Category = localizedString.Category,
                TenantId = localizedString.TenantId, // Automatically set!
                Translations = localizedString.Translations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant string");
            return StatusCode(500, "Internal server error");
        }
    }
}

/// <summary>
/// Request model for setting translations
/// </summary>
public class SetTranslationRequest
{
    /// <summary>Translation category</summary>
    public string Category { get; set; } = "ui";

    /// <summary>Language code to translation value mappings</summary>
    public Dictionary<string, string> Translations { get; set; } = new();
}

/// <summary>
/// Request model for creating translations
/// </summary>
public class CreateTranslationRequest
{
    /// <summary>Translation key</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Translation category</summary>
    public string Category { get; set; } = "ui";

    /// <summary>Language code to translation value mappings</summary>
    public Dictionary<string, string> Translations { get; set; } = new();
}</content>
<parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Gateway / Shared / src / Presentation / Controllers / LocalizationController.cs