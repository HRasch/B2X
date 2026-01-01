using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using B2Connect.ThemeService.Models;

namespace B2Connect.Theming.Endpoints;

/// <summary>
/// Wolverine HTTP Endpoints for Theme management
/// </summary>
public static class ThemeEndpoints
{
    #region Theme CRUD Endpoints

    /// <summary>
    /// POST /api/themes - Create a new theme
    /// </summary>
    [WolverinePost("/api/themes")]
    public static async Task<IResult> CreateTheme(
        [FromBody] CreateThemeRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var theme = await themeService.CreateThemeAsync(tenantId, request);
        return Results.Created($"/api/themes/{theme.Id}", theme);
    }

    /// <summary>
    /// GET /api/themes - Get all themes for tenant
    /// </summary>
    [WolverineGet("/api/themes")]
    public static async Task<IResult> GetThemes(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var themes = await themeService.GetThemesByTenantAsync(tenantId);
        return Results.Ok(themes);
    }

    /// <summary>
    /// GET /api/themes/{themeId} - Get theme by ID
    /// </summary>
    [WolverineGet("/api/themes/{themeId}")]
    public static async Task<IResult> GetTheme(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var theme = await themeService.GetThemeByIdAsync(tenantId, themeId);
        if (theme == null)
        {
            return Results.NotFound(new { Message = $"Theme '{themeId}' not found" });
        }
        return Results.Ok(theme);
    }

    /// <summary>
    /// PUT /api/themes/{themeId} - Update theme
    /// </summary>
    [WolverinePut("/api/themes/{themeId}")]
    public static async Task<IResult> UpdateTheme(
        Guid themeId,
        [FromBody] UpdateThemeRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var theme = await themeService.UpdateThemeAsync(tenantId, themeId, request);
        return Results.Ok(theme);
    }

    /// <summary>
    /// DELETE /api/themes/{themeId} - Delete theme
    /// </summary>
    [WolverineDelete("/api/themes/{themeId}")]
    public static async Task<IResult> DeleteTheme(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        await themeService.DeleteThemeAsync(tenantId, themeId);
        return Results.NoContent();
    }

    #endregion

    #region Theme Publishing Endpoints

    /// <summary>
    /// POST /api/themes/{themeId}/publish - Publish theme
    /// </summary>
    [WolverinePost("/api/themes/{themeId}/publish")]
    public static async Task<IResult> PublishTheme(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var theme = await themeService.PublishThemeAsync(tenantId, themeId);
        return Results.Ok(theme);
    }

    /// <summary>
    /// POST /api/themes/{themeId}/unpublish - Unpublish theme
    /// </summary>
    [WolverinePost("/api/themes/{themeId}/unpublish")]
    public static async Task<IResult> UnpublishTheme(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var theme = await themeService.UnpublishThemeAsync(tenantId, themeId);
        return Results.Ok(theme);
    }

    /// <summary>
    /// GET /api/themes/active - Get active theme
    /// </summary>
    [WolverineGet("/api/themes/active")]
    public static async Task<IResult> GetActiveTheme(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var theme = await themeService.GetActiveThemeAsync(tenantId);
        if (theme == null)
        {
            return Results.NotFound(new { Message = "No active theme found" });
        }
        return Results.Ok(theme);
    }

    #endregion

    #region Design Variable Endpoints

    /// <summary>
    /// POST /api/themes/{themeId}/variables - Add design variable
    /// </summary>
    [WolverinePost("/api/themes/{themeId}/variables")]
    public static async Task<IResult> AddDesignVariable(
        Guid themeId,
        [FromBody] AddDesignVariableRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var variable = await themeService.AddDesignVariableAsync(tenantId, themeId, request);
        return Results.Created($"/api/themes/{themeId}/variables/{variable.Id}", variable);
    }

    /// <summary>
    /// GET /api/themes/{themeId}/variables - Get design variables
    /// </summary>
    [WolverineGet("/api/themes/{themeId}/variables")]
    public static async Task<IResult> GetDesignVariables(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var variables = await themeService.GetDesignVariablesAsync(tenantId, themeId);
        return Results.Ok(variables);
    }

    /// <summary>
    /// PUT /api/themes/{themeId}/variables/{variableId} - Update design variable
    /// </summary>
    [WolverinePut("/api/themes/{themeId}/variables/{variableId}")]
    public static async Task<IResult> UpdateDesignVariable(
        Guid themeId,
        Guid variableId,
        [FromBody] UpdateDesignVariableRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var variable = await themeService.UpdateDesignVariableAsync(tenantId, themeId, variableId, request);
        return Results.Ok(variable);
    }

    /// <summary>
    /// DELETE /api/themes/{themeId}/variables/{variableId} - Remove design variable
    /// </summary>
    [WolverineDelete("/api/themes/{themeId}/variables/{variableId}")]
    public static async Task<IResult> RemoveDesignVariable(
        Guid themeId,
        Guid variableId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        await themeService.RemoveDesignVariableAsync(tenantId, themeId, variableId);
        return Results.NoContent();
    }

    #endregion

    #region Theme Variant Endpoints

    /// <summary>
    /// POST /api/themes/{themeId}/variants - Create theme variant
    /// </summary>
    [WolverinePost("/api/themes/{themeId}/variants")]
    public static async Task<IResult> CreateThemeVariant(
        Guid themeId,
        [FromBody] CreateThemeVariantRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var variant = await themeService.CreateThemeVariantAsync(tenantId, themeId, request);
        return Results.Created($"/api/themes/{themeId}/variants/{variant.Id}", variant);
    }

    /// <summary>
    /// GET /api/themes/{themeId}/variants - Get theme variants
    /// </summary>
    [WolverineGet("/api/themes/{themeId}/variants")]
    public static async Task<IResult> GetThemeVariants(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var variants = await themeService.GetThemeVariantsAsync(tenantId, themeId);
        return Results.Ok(variants);
    }

    /// <summary>
    /// PUT /api/themes/{themeId}/variants/{variantId} - Update theme variant
    /// </summary>
    [WolverinePut("/api/themes/{themeId}/variants/{variantId}")]
    public static async Task<IResult> UpdateThemeVariant(
        Guid themeId,
        Guid variantId,
        [FromBody] UpdateThemeVariantRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var variant = await themeService.UpdateThemeVariantAsync(tenantId, themeId, variantId, request);
        return Results.Ok(variant);
    }

    /// <summary>
    /// DELETE /api/themes/{themeId}/variants/{variantId} - Remove theme variant
    /// </summary>
    [WolverineDelete("/api/themes/{themeId}/variants/{variantId}")]
    public static async Task<IResult> RemoveThemeVariant(
        Guid themeId,
        Guid variantId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        await themeService.RemoveThemeVariantAsync(tenantId, themeId, variantId);
        return Results.NoContent();
    }

    #endregion

    #region SCSS File Endpoints

    /// <summary>
    /// POST /api/themes/{themeId}/scss-files - Create SCSS file
    /// </summary>
    [WolverinePost("/api/themes/{themeId}/scss-files")]
    public static async Task<IResult> CreateScssFile(
        Guid themeId,
        [FromBody] CreateScssFileRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var file = await themeService.CreateScssFileAsync(tenantId, themeId, request);
        return Results.Created($"/api/themes/{themeId}/scss-files/{file.Id}", file);
    }

    /// <summary>
    /// GET /api/themes/{themeId}/scss-files - Get SCSS files
    /// </summary>
    [WolverineGet("/api/themes/{themeId}/scss-files")]
    public static async Task<IResult> GetScssFiles(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var files = await themeService.GetScssFilesAsync(tenantId, themeId);
        return Results.Ok(files);
    }

    /// <summary>
    /// GET /api/themes/{themeId}/scss-files/{fileId} - Get SCSS file by ID
    /// </summary>
    [WolverineGet("/api/themes/{themeId}/scss-files/{fileId}")]
    public static async Task<IResult> GetScssFile(
        Guid themeId,
        Guid fileId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var file = await themeService.GetScssFileByIdAsync(tenantId, themeId, fileId);
        if (file == null)
        {
            return Results.NotFound(new { Message = $"SCSS file '{fileId}' not found" });
        }
        return Results.Ok(file);
    }

    /// <summary>
    /// PUT /api/themes/{themeId}/scss-files/{fileId} - Update SCSS file
    /// </summary>
    [WolverinePut("/api/themes/{themeId}/scss-files/{fileId}")]
    public static async Task<IResult> UpdateScssFile(
        Guid themeId,
        Guid fileId,
        [FromBody] UpdateScssFileRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var file = await themeService.UpdateScssFileAsync(tenantId, themeId, fileId, request);
        return Results.Ok(file);
    }

    /// <summary>
    /// DELETE /api/themes/{themeId}/scss-files/{fileId} - Delete SCSS file
    /// </summary>
    [WolverineDelete("/api/themes/{themeId}/scss-files/{fileId}")]
    public static async Task<IResult> DeleteScssFile(
        Guid themeId,
        Guid fileId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        await themeService.DeleteScssFileAsync(tenantId, themeId, fileId);
        return Results.NoContent();
    }

    #endregion

    #region CSS Generation Endpoints

    /// <summary>
    /// GET /api/themes/{themeId}/css - Generate CSS for theme
    /// </summary>
    [WolverineGet("/api/themes/{themeId}/css")]
    public static async Task<IResult> GenerateCSS(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var css = await themeService.GenerateCSSAsync(tenantId, themeId);
        return Results.Text(css, "text/css");
    }

    /// <summary>
    /// GET /api/themes/{themeId}/json - Export theme as JSON
    /// </summary>
    [WolverineGet("/api/themes/{themeId}/json")]
    public static async Task<IResult> GenerateThemeJSON(
        Guid themeId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IThemeService themeService,
        CancellationToken ct)
    {
        var json = await themeService.GenerateThemeJSONAsync(tenantId, themeId);
        return Results.Text(json, "application/json");
    }

    #endregion
}