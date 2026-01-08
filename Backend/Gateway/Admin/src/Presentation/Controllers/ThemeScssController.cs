using B2X.ThemeService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Admin.Presentation.Controllers;

/// <summary>
/// Theme SCSS Controller - Manage SCSS modules and trigger compilation
/// Enables runtime theme customization via Admin UI
/// </summary>
[Route("api/admin/themes")]
public class ThemeScssController : ApiControllerBase
{
    private readonly IScssCompilationService _compilationService;
    private readonly IScssModuleRepository _moduleRepository;
    private readonly IThemeService _themeService;

    public ThemeScssController(
        ILogger<ThemeScssController> logger,
        IScssCompilationService compilationService,
        IScssModuleRepository moduleRepository,
        IThemeService themeService) : base(logger)
    {
        _compilationService = compilationService;
        _moduleRepository = moduleRepository;
        _themeService = themeService;
    }

    #region SCSS Module Endpoints

    /// <summary>
    /// Get all SCSS modules for a theme
    /// </summary>
    [HttpGet("{themeId:guid}/scss-modules")]
    [ProducesResponseType(typeof(List<ScssModule>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ScssModule>>> GetScssModules(Guid themeId)
    {
        var tenantId = GetTenantId();

        // Verify theme exists
        var theme = await _themeService.GetThemeByIdAsync(tenantId, themeId);
        if (theme == null)
        {
            return NotFound(new { message = $"Theme {themeId} not found" });
        }

        var modules = await _moduleRepository.GetModulesByThemeAsync(tenantId, themeId);
        return Ok(modules);
    }

    /// <summary>
    /// Get a specific SCSS module
    /// </summary>
    [HttpGet("{themeId:guid}/scss-modules/{moduleId:guid}")]
    [ProducesResponseType(typeof(ScssModule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScssModule>> GetScssModule(Guid themeId, Guid moduleId)
    {
        var tenantId = GetTenantId();

        var module = await _moduleRepository.GetModuleByIdAsync(tenantId, themeId, moduleId);
        if (module == null)
        {
            return NotFound(new { message = $"Module {moduleId} not found" });
        }

        return Ok(module);
    }

    /// <summary>
    /// Create a new SCSS module
    /// </summary>
    [HttpPost("{themeId:guid}/scss-modules")]
    [ProducesResponseType(typeof(ScssModule), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScssModule>> CreateScssModule(
        Guid themeId,
        [FromBody] CreateScssModuleRequest request)
    {
        var tenantId = GetTenantId();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Module name is required" });
        }

        // Check for duplicate name
        if (await _moduleRepository.ModuleNameExistsAsync(tenantId, themeId, request.Name))
        {
            return BadRequest(new { message = $"Module '{request.Name}' already exists" });
        }

        var module = new ScssModule
        {
            Name = request.Name,
            Category = request.Category,
            ScssContent = request.ScssContent ?? string.Empty,
            SortOrder = request.SortOrder,
            Description = request.Description,
            IsEnabled = true,
            IsSystem = false
        };

        var created = await _moduleRepository.CreateModuleAsync(tenantId, themeId, module);

        // Invalidate cache since source changed
        await _compilationService.InvalidateCacheAsync(tenantId, themeId);

        _logger.LogInformation("Created SCSS module {ModuleName} for Theme {ThemeId}", request.Name, themeId);

        return CreatedAtAction(
            nameof(GetScssModule),
            new { themeId, moduleId = created.Id },
            created);
    }

    /// <summary>
    /// Update an SCSS module
    /// </summary>
    [HttpPut("{themeId:guid}/scss-modules/{moduleId:guid}")]
    [ProducesResponseType(typeof(ScssModule), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScssModule>> UpdateScssModule(
        Guid themeId,
        Guid moduleId,
        [FromBody] UpdateScssModuleRequest request)
    {
        var tenantId = GetTenantId();

        var existing = await _moduleRepository.GetModuleByIdAsync(tenantId, themeId, moduleId);
        if (existing == null)
        {
            return NotFound(new { message = $"Module {moduleId} not found" });
        }

        // Prevent editing system modules
        if (existing.IsSystem)
        {
            return BadRequest(new { message = "Cannot modify system modules" });
        }

        // Update fields
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            existing.Name = request.Name;
        }

        if (request.ScssContent != null)
        {
            existing.ScssContent = request.ScssContent;
        }

        if (request.SortOrder.HasValue)
        {
            existing.SortOrder = request.SortOrder.Value;
        }

        if (request.IsEnabled.HasValue)
        {
            existing.IsEnabled = request.IsEnabled.Value;
        }

        if (request.Description != null)
        {
            existing.Description = request.Description;
        }

        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetUserId();

        var updated = await _moduleRepository.UpdateModuleAsync(tenantId, themeId, moduleId, existing);

        // Invalidate cache since source changed
        await _compilationService.InvalidateCacheAsync(tenantId, themeId);

        _logger.LogInformation("Updated SCSS module {ModuleId} for Theme {ThemeId}", moduleId, themeId);

        return Ok(updated);
    }

    /// <summary>
    /// Delete an SCSS module
    /// </summary>
    [HttpDelete("{themeId:guid}/scss-modules/{moduleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteScssModule(Guid themeId, Guid moduleId)
    {
        var tenantId = GetTenantId();

        var existing = await _moduleRepository.GetModuleByIdAsync(tenantId, themeId, moduleId);
        if (existing == null)
        {
            return NotFound(new { message = $"Module {moduleId} not found" });
        }

        // Prevent deleting system modules
        if (existing.IsSystem)
        {
            return BadRequest(new { message = "Cannot delete system modules" });
        }

        await _moduleRepository.DeleteModuleAsync(tenantId, themeId, moduleId);

        // Invalidate cache
        await _compilationService.InvalidateCacheAsync(tenantId, themeId);

        _logger.LogInformation("Deleted SCSS module {ModuleId} from Theme {ThemeId}", moduleId, themeId);

        return NoContent();
    }

    #endregion

    #region Compilation Endpoints

    /// <summary>
    /// Compile SCSS to CSS and store result
    /// Called from Admin UI "Compile & Save" button
    /// </summary>
    [HttpPost("{themeId:guid}/compile")]
    [ProducesResponseType(typeof(CompilationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompilationResult>> CompileTheme(Guid themeId)
    {
        var tenantId = GetTenantId();

        // Verify theme exists
        var theme = await _themeService.GetThemeByIdAsync(tenantId, themeId);
        if (theme == null)
        {
            return NotFound(new { message = $"Theme {themeId} not found" });
        }

        _logger.LogInformation("Starting SCSS compilation for Theme {ThemeId}, Tenant {TenantId}", themeId, tenantId);

        var result = await _compilationService.CompileThemeAsync(tenantId, themeId);

        if (result.Success)
        {
            _logger.LogInformation(
                "SCSS compilation successful for Theme {ThemeId} in {ElapsedMs}ms",
                themeId, result.CompilationTimeMs);
        }
        else
        {
            _logger.LogWarning(
                "SCSS compilation failed for Theme {ThemeId}: {Error}",
                themeId, result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Preview SCSS compilation without saving
    /// For live preview in Admin UI editor
    /// </summary>
    [HttpPost("{themeId:guid}/preview")]
    [ProducesResponseType(typeof(CompilationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompilationResult>> PreviewCompile(
        Guid themeId,
        [FromBody] PreviewCompileRequest? request)
    {
        var tenantId = GetTenantId();

        // Verify theme exists
        var theme = await _themeService.GetThemeByIdAsync(tenantId, themeId);
        if (theme == null)
        {
            return NotFound(new { message = $"Theme {themeId} not found" });
        }

        var result = await _compilationService.PreviewCompileAsync(tenantId, themeId, request?.ScssOverrides);

        return Ok(result);
    }

    /// <summary>
    /// Get compilation status for a theme
    /// </summary>
    [HttpGet("{themeId:guid}/compilation-status")]
    [ProducesResponseType(typeof(CompiledTheme), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompiledTheme>> GetCompilationStatus(Guid themeId)
    {
        var tenantId = GetTenantId();

        var status = await _compilationService.GetCompilationStatusAsync(tenantId, themeId);
        if (status == null)
        {
            return NotFound(new { message = "Theme has not been compiled yet" });
        }

        return Ok(status);
    }

    /// <summary>
    /// Check if theme needs recompilation
    /// </summary>
    [HttpGet("{themeId:guid}/needs-recompilation")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> CheckNeedsRecompilation(Guid themeId)
    {
        var tenantId = GetTenantId();

        var needsRecompilation = await _compilationService.NeedsRecompilationAsync(tenantId, themeId);

        return Ok(new { needsRecompilation });
    }

    /// <summary>
    /// Invalidate CSS cache for a theme
    /// </summary>
    [HttpPost("{themeId:guid}/invalidate-cache")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> InvalidateCache(Guid themeId)
    {
        var tenantId = GetTenantId();

        await _compilationService.InvalidateCacheAsync(tenantId, themeId);

        _logger.LogInformation("Cache invalidated for Theme {ThemeId}", themeId);

        return NoContent();
    }

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Import default SCSS framework modules into a theme
    /// </summary>
    [HttpPost("{themeId:guid}/import-defaults")]
    [ProducesResponseType(typeof(List<ScssModule>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ScssModule>>> ImportDefaultModules(Guid themeId)
    {
        var tenantId = GetTenantId();

        // Verify theme exists
        var theme = await _themeService.GetThemeByIdAsync(tenantId, themeId);
        if (theme == null)
        {
            return NotFound(new { message = $"Theme {themeId} not found" });
        }

        await _moduleRepository.ImportDefaultModulesAsync(tenantId, themeId);

        var modules = await _moduleRepository.GetModulesByThemeAsync(tenantId, themeId);

        _logger.LogInformation("Imported default SCSS modules for Theme {ThemeId}", themeId);

        return Ok(modules);
    }

    /// <summary>
    /// Clone SCSS modules from another theme
    /// </summary>
    [HttpPost("{themeId:guid}/clone-from/{sourceThemeId:guid}")]
    [ProducesResponseType(typeof(List<ScssModule>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ScssModule>>> CloneFromTheme(Guid themeId, Guid sourceThemeId)
    {
        var tenantId = GetTenantId();

        // Verify both themes exist
        var targetTheme = await _themeService.GetThemeByIdAsync(tenantId, themeId);
        var sourceTheme = await _themeService.GetThemeByIdAsync(tenantId, sourceThemeId);

        if (targetTheme == null)
        {
            return NotFound(new { message = $"Target theme {themeId} not found" });
        }

        if (sourceTheme == null)
        {
            return NotFound(new { message = $"Source theme {sourceThemeId} not found" });
        }

        await _moduleRepository.CloneModulesToThemeAsync(tenantId, sourceThemeId, themeId);

        var modules = await _moduleRepository.GetModulesByThemeAsync(tenantId, themeId);

        _logger.LogInformation(
            "Cloned SCSS modules from Theme {SourceThemeId} to Theme {ThemeId}",
            sourceThemeId, themeId);

        return Ok(modules);
    }

    #endregion
}

/// <summary>
/// Public Theme CSS Controller - Serve compiled CSS to Store Frontend
/// No authentication required
/// </summary>
[Route("api/themes")]
[ApiController]
[AllowAnonymous]
public class ThemeCssController : ControllerBase
{
    private readonly IScssCompilationService _compilationService;
    private readonly ILogger<ThemeCssController> _logger;

    public ThemeCssController(
        IScssCompilationService compilationService,
        ILogger<ThemeCssController> logger)
    {
        _compilationService = compilationService;
        _logger = logger;
    }

    /// <summary>
    /// Get compiled CSS for a tenant's active theme
    /// Called by Store Frontend: &lt;link href="/api/themes/{tenantId}/theme.css" /&gt;
    /// </summary>
    [HttpGet("{tenantId:guid}/theme.css")]
    [Produces("text/css")]
    [ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "v" })]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/css")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetThemeCss(Guid tenantId, [FromQuery] Guid? themeId = null)
    {
        try
        {
            // If no specific theme requested, we'd need to get the active theme
            // For now, require themeId
            if (!themeId.HasValue)
            {
                return NotFound("Theme ID required");
            }

            var css = await _compilationService.GetCompiledCssAsync(tenantId, themeId.Value, minified: true);

            if (string.IsNullOrEmpty(css))
            {
                return NotFound("Theme CSS not found");
            }

            return Content(css, "text/css");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to serve theme CSS for Tenant {TenantId}", tenantId);
            return StatusCode(500, "Failed to load theme CSS");
        }
    }

    /// <summary>
    /// Get unminified CSS (for debugging)
    /// </summary>
    [HttpGet("{tenantId:guid}/theme.debug.css")]
    [Produces("text/css")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK, "text/css")]
    public async Task<IActionResult> GetThemeCssDebug(Guid tenantId, [FromQuery] Guid themeId)
    {
        var css = await _compilationService.GetCompiledCssAsync(tenantId, themeId, minified: false);

        if (string.IsNullOrEmpty(css))
        {
            return NotFound("Theme CSS not found");
        }

        return Content(css, "text/css");
    }
}
