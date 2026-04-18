namespace B2X.ThemeService.Models;

/// <summary>
/// SCSS Compilation Service Interface
/// Handles SCSS to CSS compilation with caching
/// </summary>
public interface IScssCompilationService
{
    /// <summary>
    /// Compile all SCSS modules for a theme and store result
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="themeId">Theme identifier</param>
    /// <returns>Compilation result with CSS or error</returns>
    Task<CompilationResult> CompileThemeAsync(Guid tenantId, Guid themeId);

    /// <summary>
    /// Preview compilation without saving (for live preview in Admin UI)
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="themeId">Theme identifier</param>
    /// <param name="scssOverrides">Optional SCSS to append for preview</param>
    /// <returns>Compilation result with CSS or error</returns>
    Task<CompilationResult> PreviewCompileAsync(Guid tenantId, Guid themeId, string? scssOverrides = null);

    /// <summary>
    /// Get compiled CSS (from cache or compile on-demand)
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="themeId">Theme identifier</param>
    /// <param name="minified">Return minified version</param>
    /// <returns>Compiled CSS string</returns>
    Task<string> GetCompiledCssAsync(Guid tenantId, Guid themeId, bool minified = true);

    /// <summary>
    /// Check if compilation is needed (source changed)
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="themeId">Theme identifier</param>
    /// <returns>True if recompilation is needed</returns>
    Task<bool> NeedsRecompilationAsync(Guid tenantId, Guid themeId);

    /// <summary>
    /// Invalidate CSS cache for a theme
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="themeId">Theme identifier</param>
    Task InvalidateCacheAsync(Guid tenantId, Guid themeId);

    /// <summary>
    /// Get compilation status for a theme
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="themeId">Theme identifier</param>
    /// <returns>Current compilation status or null if never compiled</returns>
    Task<CompiledTheme?> GetCompilationStatusAsync(Guid tenantId, Guid themeId);
}

/// <summary>
/// SCSS Module Repository Interface
/// Data access for SCSS modules stored in database
/// </summary>
public interface IScssModuleRepository
{
    #region Module CRUD Operations

    /// <summary>Create a new SCSS module</summary>
    Task<ScssModule> CreateModuleAsync(Guid tenantId, Guid themeId, ScssModule module);

    /// <summary>Get module by ID</summary>
    Task<ScssModule?> GetModuleByIdAsync(Guid tenantId, Guid themeId, Guid moduleId);

    /// <summary>Get all modules for a theme (ordered by category and sort order)</summary>
    Task<List<ScssModule>> GetModulesByThemeAsync(Guid tenantId, Guid themeId);

    /// <summary>Get enabled modules for a theme (for compilation)</summary>
    Task<List<ScssModule>> GetEnabledModulesAsync(Guid tenantId, Guid themeId);

    /// <summary>Get modules by category</summary>
    Task<List<ScssModule>> GetModulesByCategoryAsync(Guid tenantId, Guid themeId, ScssModuleCategory category);

    /// <summary>Update module</summary>
    Task<ScssModule> UpdateModuleAsync(Guid tenantId, Guid themeId, Guid moduleId, ScssModule module);

    /// <summary>Delete module</summary>
    Task DeleteModuleAsync(Guid tenantId, Guid themeId, Guid moduleId);

    /// <summary>Check if module name exists</summary>
    Task<bool> ModuleNameExistsAsync(Guid tenantId, Guid themeId, string name);

    #endregion

    #region Compiled Theme Cache

    /// <summary>Get cached compilation</summary>
    Task<CompiledTheme?> GetCompiledThemeAsync(Guid tenantId, Guid themeId);

    /// <summary>Store compiled theme</summary>
    Task<CompiledTheme> SaveCompiledThemeAsync(Guid tenantId, Guid themeId, CompiledTheme compiled);

    /// <summary>Delete compiled theme cache</summary>
    Task DeleteCompiledThemeAsync(Guid tenantId, Guid themeId);

    /// <summary>Calculate hash of all SCSS sources</summary>
    Task<string> CalculateSourceHashAsync(Guid tenantId, Guid themeId);

    #endregion

    #region Bulk Operations

    /// <summary>Clone all modules from one theme to another</summary>
    Task CloneModulesToThemeAsync(Guid tenantId, Guid sourceThemeId, Guid targetThemeId);

    /// <summary>Import default SCSS modules (bootstrap-style framework)</summary>
    Task ImportDefaultModulesAsync(Guid tenantId, Guid themeId);

    #endregion
}
