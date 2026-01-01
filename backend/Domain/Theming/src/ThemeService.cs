using B2Connect.ThemeService.Models;

namespace B2Connect.ThemeService.Services;

/// <summary>
/// Theme Service - Business logic for theme management
/// Implements IThemeService with minimal code to pass tests
/// </summary>
public class ThemeService : IThemeService
{
    private readonly IThemeRepository _repository;

    public ThemeService(IThemeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #region Theme Operations

    public async Task<Theme> CreateThemeAsync(Guid tenantId, CreateThemeRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.Name))
            throw new ArgumentException("Theme name is required", nameof(request));

        // Check for duplicate name
        var nameExists = await _repository.ThemeNameExistsAsync(tenantId, request.Name);
        if (nameExists)
            throw new InvalidOperationException($"Theme '{request.Name}' already exists for this tenant");

        // Create theme entity
        var theme = new Theme
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name ?? string.Empty,
            Description = request.Description,
            PrimaryColor = request.PrimaryColor,
            SecondaryColor = request.SecondaryColor,
            TertiaryColor = request.TertiaryColor,
            Variables = new List<DesignVariable>(),
            Variants = new List<ThemeVariant>(),
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.CreateThemeAsync(tenantId, theme);
    }

    public async Task<Theme?> GetThemeByIdAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.GetThemeByIdAsync(tenantId, themeId);
    }

    public async Task<List<Theme>> GetThemesByTenantAsync(Guid tenantId)
    {
        return await _repository.GetThemesByTenantAsync(tenantId);
    }

    public async Task<Theme?> GetActiveThemeAsync(Guid tenantId)
    {
        return await _repository.GetActiveThemeAsync(tenantId);
    }

    public async Task<List<Theme>> GetPublishedThemesAsync(Guid tenantId)
    {
        return await _repository.GetPublishedThemesAsync(tenantId);
    }

    public async Task<Theme> UpdateThemeAsync(Guid tenantId, Guid themeId, UpdateThemeRequest request)
    {
        // Get existing theme
        var theme = await _repository.GetThemeByIdAsync(tenantId, themeId)
            ?? throw new KeyNotFoundException($"Theme '{themeId}' not found");

        // Update fields if provided (use locals to help the compiler infer non-null values)
        var reqName = request?.Name;
        if (!string.IsNullOrWhiteSpace(reqName))
            theme.Name = reqName;

        var reqDescription = request?.Description;
        if (!string.IsNullOrWhiteSpace(reqDescription))
            theme.Description = reqDescription;

        var reqPrimary = request?.PrimaryColor;
        if (!string.IsNullOrWhiteSpace(reqPrimary))
            theme.PrimaryColor = reqPrimary;

        var reqSecondary = request?.SecondaryColor;
        if (!string.IsNullOrWhiteSpace(reqSecondary))
            theme.SecondaryColor = reqSecondary;

        var reqTertiary = request?.TertiaryColor;
        if (!string.IsNullOrWhiteSpace(reqTertiary))
            theme.TertiaryColor = reqTertiary;

        // Increment version
        theme.Version++;
        theme.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateThemeAsync(tenantId, themeId, theme);
    }

    public async Task DeleteThemeAsync(Guid tenantId, Guid themeId)
    {
        await _repository.DeleteThemeAsync(tenantId, themeId);
    }

    #endregion

    #region Design Variable Operations

    public async Task<DesignVariable> AddDesignVariableAsync(Guid tenantId, Guid themeId, AddDesignVariableRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.Name))
            throw new ArgumentException("Variable name is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request?.Value))
            throw new ArgumentException("Variable value is required", nameof(request));

        // Create variable entity
        var variable = new DesignVariable
        {
            Id = Guid.NewGuid(),
            ThemeId = themeId,
            Name = request.Name ?? string.Empty,
            Value = request.Value ?? string.Empty,
            Category = request.Category ?? string.Empty,
            Description = request.Description ?? string.Empty,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.AddDesignVariableAsync(tenantId, themeId, variable);
    }

    public async Task<DesignVariable> UpdateDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId, UpdateDesignVariableRequest request)
    {
        // Get existing variable
        var variables = await _repository.GetDesignVariablesAsync(tenantId, themeId);
        var variable = variables.FirstOrDefault(v => v.Id == variableId)
            ?? throw new KeyNotFoundException($"Variable '{variableId}' not found");

        // Update fields if provided (use locals to ensure non-null values)
        var reqValue = request?.Value;
        if (!string.IsNullOrWhiteSpace(reqValue))
            variable.Value = reqValue;

        var reqCategory = request?.Category;
        if (!string.IsNullOrWhiteSpace(reqCategory))
            variable.Category = reqCategory;

        var reqDescriptionVar = request?.Description;
        if (!string.IsNullOrWhiteSpace(reqDescriptionVar))
            variable.Description = reqDescriptionVar;

        variable.Type = request?.Type ?? variable.Type;
        variable.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateDesignVariableAsync(tenantId, themeId, variableId, variable);
    }

    public async Task<List<DesignVariable>> GetDesignVariablesAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.GetDesignVariablesAsync(tenantId, themeId);
    }

    public async Task RemoveDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId)
    {
        await _repository.RemoveDesignVariableAsync(tenantId, themeId, variableId);
    }

    #endregion

    #region Theme Variant Operations

    public async Task<ThemeVariant> CreateThemeVariantAsync(Guid tenantId, Guid themeId, CreateThemeVariantRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.Name))
            throw new ArgumentException("Variant name is required", nameof(request));

        // Create variant entity
        var variant = new ThemeVariant
        {
            Id = Guid.NewGuid(),
            ThemeId = themeId,
            Name = request.Name ?? string.Empty,
            Description = request.Description ?? string.Empty,
            VariableOverrides = request.VariableOverrides ?? new Dictionary<string, string>(),
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.CreateThemeVariantAsync(tenantId, themeId, variant);
    }

    public async Task<List<ThemeVariant>> GetThemeVariantsAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.GetThemeVariantsAsync(tenantId, themeId);
    }

    public async Task<ThemeVariant> UpdateThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId, UpdateThemeVariantRequest request)
    {
        // Get existing variant
        var variants = await _repository.GetThemeVariantsAsync(tenantId, themeId);
        var variant = variants.FirstOrDefault(v => v.Id == variantId)
            ?? throw new KeyNotFoundException($"Variant '{variantId}' not found");

        // Update fields if provided
        var reqVarName = request?.Name;
        if (!string.IsNullOrWhiteSpace(reqVarName))
            variant.Name = reqVarName;

        var reqVarDescription = request?.Description;
        if (!string.IsNullOrWhiteSpace(reqVarDescription))
            variant.Description = reqVarDescription;

        if (request?.VariableOverrides != null)
            variant.VariableOverrides = request.VariableOverrides;

        if (request?.IsEnabled.HasValue == true)
            variant.IsEnabled = request.IsEnabled.Value;

        return await _repository.UpdateThemeVariantAsync(tenantId, themeId, variantId, variant);
    }

    public async Task RemoveThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId)
    {
        await _repository.RemoveThemeVariantAsync(tenantId, themeId, variantId);
    }

    #endregion

    #region SCSS File Operations

    public async Task<ScssFile> CreateScssFileAsync(Guid tenantId, Guid themeId, CreateScssFileRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.FileName))
            throw new ArgumentException("File name is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request?.Content))
            throw new ArgumentException("Content is required", nameof(request));

        // Create file entity
        var file = new ScssFile
        {
            Id = Guid.NewGuid(),
            ThemeId = themeId,
            FileName = request.FileName ?? string.Empty,
            Content = request.Content ?? string.Empty,
            Description = request.Description ?? string.Empty,
            Order = request.Order,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.CreateScssFileAsync(tenantId, themeId, file);
    }

    public async Task<ScssFile?> GetScssFileByIdAsync(Guid tenantId, Guid themeId, Guid fileId)
    {
        return await _repository.GetScssFileByIdAsync(tenantId, themeId, fileId);
    }

    public async Task<List<ScssFile>> GetScssFilesAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.GetScssFilesAsync(tenantId, themeId);
    }

    public async Task<ScssFile> UpdateScssFileAsync(Guid tenantId, Guid themeId, Guid fileId, UpdateScssFileRequest request)
    {
        // Get existing file
        var file = await _repository.GetScssFileByIdAsync(tenantId, themeId, fileId)
            ?? throw new KeyNotFoundException($"SCSS file '{fileId}' not found");

        // Update fields if provided
        var reqFileName = request?.FileName;
        if (!string.IsNullOrWhiteSpace(reqFileName))
            file.FileName = reqFileName;

        var reqContent = request?.Content;
        if (!string.IsNullOrWhiteSpace(reqContent))
            file.Content = reqContent;

        var reqDescription = request?.Description;
        if (!string.IsNullOrWhiteSpace(reqDescription))
            file.Description = reqDescription;

        if (request?.IsActive.HasValue == true)
            file.IsActive = request.IsActive.Value;

        if (request?.Order.HasValue == true)
            file.Order = request.Order.Value;

        file.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateScssFileAsync(tenantId, themeId, fileId, file);
    }

    public async Task DeleteScssFileAsync(Guid tenantId, Guid themeId, Guid fileId)
    {
        await _repository.DeleteScssFileAsync(tenantId, themeId, fileId);
    }

    #endregion

    #region CSS Generation & Export

    public async Task<string> GenerateCSSAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.GenerateCSSAsync(tenantId, themeId);
    }

    public async Task<string> GenerateThemeJSONAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.GenerateThemeJSONAsync(tenantId, themeId);
    }

    #endregion

    #region Theme Publishing

    public async Task<Theme> PublishThemeAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.PublishThemeAsync(tenantId, themeId);
    }

    public async Task<Theme> UnpublishThemeAsync(Guid tenantId, Guid themeId)
    {
        return await _repository.UnpublishThemeAsync(tenantId, themeId);
    }

    #endregion
}
