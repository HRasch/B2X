namespace B2Connect.ThemeService.Models;

/// <summary>
/// Theme Repository Interface - Data access layer for themes
/// </summary>
public interface IThemeRepository
{
    #region Theme Operations

    Task<Theme> CreateThemeAsync(Guid tenantId, Theme theme);
    Task<Theme?> GetThemeByIdAsync(Guid tenantId, Guid themeId);
    Task<List<Theme>> GetThemesByTenantAsync(Guid tenantId);
    Task<Theme?> GetActiveThemeAsync(Guid tenantId);
    Task<List<Theme>> GetPublishedThemesAsync(Guid tenantId);
    Task<bool> ThemeNameExistsAsync(Guid tenantId, string name);
    Task<Theme> UpdateThemeAsync(Guid tenantId, Guid themeId, Theme theme);
    Task DeleteThemeAsync(Guid tenantId, Guid themeId);

    #endregion

    #region Design Variable Operations

    Task<DesignVariable> AddDesignVariableAsync(Guid tenantId, Guid themeId, DesignVariable variable);
    Task<DesignVariable> UpdateDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId, DesignVariable variable);
    Task<List<DesignVariable>> GetDesignVariablesAsync(Guid tenantId, Guid themeId);
    Task RemoveDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId);

    #endregion

    #region Theme Variant Operations

    Task<ThemeVariant> CreateThemeVariantAsync(Guid tenantId, Guid themeId, ThemeVariant variant);
    Task<List<ThemeVariant>> GetThemeVariantsAsync(Guid tenantId, Guid themeId);
    Task<ThemeVariant> UpdateThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId, ThemeVariant variant);
    Task RemoveThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId);

    #endregion

    #region SCSS File Operations

    Task<ScssFile> CreateScssFileAsync(Guid tenantId, Guid themeId, ScssFile file);
    Task<ScssFile?> GetScssFileByIdAsync(Guid tenantId, Guid themeId, Guid fileId);
    Task<List<ScssFile>> GetScssFilesAsync(Guid tenantId, Guid themeId);
    Task<ScssFile> UpdateScssFileAsync(Guid tenantId, Guid themeId, Guid fileId, ScssFile file);
    Task DeleteScssFileAsync(Guid tenantId, Guid themeId, Guid fileId);

    #endregion

    #region CSS Generation & Export

    Task<string> GenerateCSSAsync(Guid tenantId, Guid themeId);
    Task<string> GenerateThemeJSONAsync(Guid tenantId, Guid themeId);

    #endregion

    #region Theme Publishing

    Task<Theme> PublishThemeAsync(Guid tenantId, Guid themeId);
    Task<Theme> UnpublishThemeAsync(Guid tenantId, Guid themeId);

    #endregion
}

/// <summary>
/// Theme Service Interface - Business logic layer for themes
/// </summary>
public interface IThemeService
{
    #region Theme Operations

    Task<Theme> CreateThemeAsync(Guid tenantId, CreateThemeRequest request);
    Task<Theme?> GetThemeByIdAsync(Guid tenantId, Guid themeId);
    Task<List<Theme>> GetThemesByTenantAsync(Guid tenantId);
    Task<Theme?> GetActiveThemeAsync(Guid tenantId);
    Task<List<Theme>> GetPublishedThemesAsync(Guid tenantId);
    Task<Theme> UpdateThemeAsync(Guid tenantId, Guid themeId, UpdateThemeRequest request);
    Task DeleteThemeAsync(Guid tenantId, Guid themeId);

    #endregion

    #region Design Variable Operations

    Task<DesignVariable> AddDesignVariableAsync(Guid tenantId, Guid themeId, AddDesignVariableRequest request);
    Task<DesignVariable> UpdateDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId, UpdateDesignVariableRequest request);
    Task<List<DesignVariable>> GetDesignVariablesAsync(Guid tenantId, Guid themeId);
    Task RemoveDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId);

    #endregion

    #region Theme Variant Operations

    Task<ThemeVariant> CreateThemeVariantAsync(Guid tenantId, Guid themeId, CreateThemeVariantRequest request);
    Task<List<ThemeVariant>> GetThemeVariantsAsync(Guid tenantId, Guid themeId);
    Task<ThemeVariant> UpdateThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId, UpdateThemeVariantRequest request);
    Task RemoveThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId);

    #endregion

    #region SCSS File Operations

    Task<ScssFile> CreateScssFileAsync(Guid tenantId, Guid themeId, CreateScssFileRequest request);
    Task<ScssFile?> GetScssFileByIdAsync(Guid tenantId, Guid themeId, Guid fileId);
    Task<List<ScssFile>> GetScssFilesAsync(Guid tenantId, Guid themeId);
    Task<ScssFile> UpdateScssFileAsync(Guid tenantId, Guid themeId, Guid fileId, UpdateScssFileRequest request);
    Task DeleteScssFileAsync(Guid tenantId, Guid themeId, Guid fileId);

    #endregion

    #region CSS Generation & Export

    Task<string> GenerateCSSAsync(Guid tenantId, Guid themeId);
    Task<string> GenerateThemeJSONAsync(Guid tenantId, Guid themeId);

    #endregion

    #region Theme Publishing

    Task<Theme> PublishThemeAsync(Guid tenantId, Guid themeId);
    Task<Theme> UnpublishThemeAsync(Guid tenantId, Guid themeId);

    #endregion
}
