using System.Collections.Concurrent;

namespace B2X.ThemeService.Models;

/// <summary>
/// In-memory implementation of IThemeRepository for development/testing
/// </summary>
public class InMemoryThemeRepository : IThemeRepository
{
    private readonly ConcurrentDictionary<Guid, Theme> _themes = new();
    private readonly ConcurrentDictionary<string, List<DesignVariable>> _variables = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, List<ThemeVariant>> _variants = new(StringComparer.Ordinal);

    #region Theme Operations

    public Task<Theme> CreateThemeAsync(Guid tenantId, Theme theme)
    {
        theme.Id = Guid.NewGuid();
        theme.TenantId = tenantId;
        theme.CreatedAt = DateTime.UtcNow;
        theme.UpdatedAt = DateTime.UtcNow;
        _themes[theme.Id] = theme;
        return Task.FromResult(theme);
    }

    public Task<Theme?> GetThemeByIdAsync(Guid tenantId, Guid themeId)
    {
        _themes.TryGetValue(themeId, out var theme);
        if (theme?.TenantId != tenantId)
        {
            return Task.FromResult<Theme?>(null);
        }

        return Task.FromResult<Theme?>(theme);
    }

    public Task<List<Theme>> GetThemesByTenantAsync(Guid tenantId)
    {
        var themes = _themes.Values.Where(t => t.TenantId == tenantId).ToList();
        return Task.FromResult(themes);
    }

    public Task<Theme?> GetActiveThemeAsync(Guid tenantId)
    {
        var theme = _themes.Values.FirstOrDefault(t => t.TenantId == tenantId && t.IsActive);
        return Task.FromResult(theme);
    }

    public Task<List<Theme>> GetPublishedThemesAsync(Guid tenantId)
    {
        var themes = _themes.Values.Where(t => t.TenantId == tenantId && t.PublishedAt != null).ToList();
        return Task.FromResult(themes);
    }

    public Task<bool> ThemeNameExistsAsync(Guid tenantId, string name)
    {
        var exists = _themes.Values.Any(t => t.TenantId == tenantId && string.Equals(t.Name, name, StringComparison.Ordinal));
        return Task.FromResult(exists);
    }

    public Task<Theme> UpdateThemeAsync(Guid tenantId, Guid themeId, Theme theme)
    {
        if (_themes.TryGetValue(themeId, out var existing) && existing.TenantId == tenantId)
        {
            theme.Id = themeId;
            theme.TenantId = tenantId;
            theme.UpdatedAt = DateTime.UtcNow;
            _themes[themeId] = theme;
            return Task.FromResult(theme);
        }
        throw new KeyNotFoundException($"Theme {themeId} not found");
    }

    public Task DeleteThemeAsync(Guid tenantId, Guid themeId)
    {
        if (_themes.TryGetValue(themeId, out var theme) && theme.TenantId == tenantId)
        {
            _themes.TryRemove(themeId, out _);
        }
        return Task.CompletedTask;
    }

    #endregion

    #region Design Variable Operations

    private static string GetVariableKey(Guid tenantId, Guid themeId) => $"{tenantId}:{themeId}";

    public Task<DesignVariable> AddDesignVariableAsync(Guid tenantId, Guid themeId, DesignVariable variable)
    {
        var key = GetVariableKey(tenantId, themeId);
        variable.Id = Guid.NewGuid();
        if (!_variables.ContainsKey(key))
        {
            _variables[key] = new List<DesignVariable>();
        }

        _variables[key].Add(variable);
        return Task.FromResult(variable);
    }

    public Task<DesignVariable> UpdateDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId, DesignVariable variable)
    {
        var key = GetVariableKey(tenantId, themeId);
        if (_variables.TryGetValue(key, out var list))
        {
            var index = list.FindIndex(v => v.Id == variableId);
            if (index >= 0)
            {
                variable.Id = variableId;
                list[index] = variable;
                return Task.FromResult(variable);
            }
        }
        throw new KeyNotFoundException($"Variable {variableId} not found");
    }

    public Task<List<DesignVariable>> GetDesignVariablesAsync(Guid tenantId, Guid themeId)
    {
        var key = GetVariableKey(tenantId, themeId);
        _variables.TryGetValue(key, out var list);
        return Task.FromResult(list ?? new List<DesignVariable>());
    }

    public Task RemoveDesignVariableAsync(Guid tenantId, Guid themeId, Guid variableId)
    {
        var key = GetVariableKey(tenantId, themeId);
        if (_variables.TryGetValue(key, out var list))
        {
            list.RemoveAll(v => v.Id == variableId);
        }
        return Task.CompletedTask;
    }

    #endregion

    #region Theme Variant Operations

    public Task<ThemeVariant> CreateThemeVariantAsync(Guid tenantId, Guid themeId, ThemeVariant variant)
    {
        var key = GetVariableKey(tenantId, themeId);
        variant.Id = Guid.NewGuid();
        if (!_variants.ContainsKey(key))
        {
            _variants[key] = new List<ThemeVariant>();
        }

        _variants[key].Add(variant);
        return Task.FromResult(variant);
    }

    public Task<List<ThemeVariant>> GetThemeVariantsAsync(Guid tenantId, Guid themeId)
    {
        var key = GetVariableKey(tenantId, themeId);
        _variants.TryGetValue(key, out var list);
        return Task.FromResult(list ?? new List<ThemeVariant>());
    }

    public Task<ThemeVariant> UpdateThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId, ThemeVariant variant)
    {
        var key = GetVariableKey(tenantId, themeId);
        if (_variants.TryGetValue(key, out var list))
        {
            var index = list.FindIndex(v => v.Id == variantId);
            if (index >= 0)
            {
                variant.Id = variantId;
                list[index] = variant;
                return Task.FromResult(variant);
            }
        }
        throw new KeyNotFoundException($"Variant {variantId} not found");
    }

    public Task RemoveThemeVariantAsync(Guid tenantId, Guid themeId, Guid variantId)
    {
        var key = GetVariableKey(tenantId, themeId);
        if (_variants.TryGetValue(key, out var list))
        {
            list.RemoveAll(v => v.Id == variantId);
        }
        return Task.CompletedTask;
    }

    #endregion

    #region CSS Generation & Export

    public async Task<string> GenerateCSSAsync(Guid tenantId, Guid themeId)
    {
        var variables = await GetDesignVariablesAsync(tenantId, themeId).ConfigureAwait(false);
        var css = ":root {\n";
        foreach (var v in variables)
        {
            css += $"  --{v.Name}: {v.Value};\n";
        }
        css += "}\n";
        return css;
    }

    public async Task<string> GenerateThemeJSONAsync(Guid tenantId, Guid themeId)
    {
        var theme = await GetThemeByIdAsync(tenantId, themeId).ConfigureAwait(false);
        var variables = await GetDesignVariablesAsync(tenantId, themeId).ConfigureAwait(false);
        var variants = await GetThemeVariantsAsync(tenantId, themeId).ConfigureAwait(false);

        return System.Text.Json.JsonSerializer.Serialize(new
        {
            theme,
            variables,
            variants
        });
    }

    #endregion

    #region Theme Publishing

    public async Task<Theme> PublishThemeAsync(Guid tenantId, Guid themeId)
    {
        var theme = await GetThemeByIdAsync(tenantId, themeId).ConfigureAwait(false);
        if (theme == null)
        {
            throw new KeyNotFoundException($"Theme {themeId} not found");
        }

        theme.PublishedAt = DateTime.UtcNow;
        return theme;
    }

    public async Task<Theme> UnpublishThemeAsync(Guid tenantId, Guid themeId)
    {
        var theme = await GetThemeByIdAsync(tenantId, themeId).ConfigureAwait(false);
        if (theme == null)
        {
            throw new KeyNotFoundException($"Theme {themeId} not found");
        }

        theme.PublishedAt = null;
        return theme;
    }

    public async Task<Theme> ActivateThemeAsync(Guid tenantId, Guid themeId)
    {
        // Deactivate all other themes first
        foreach (var t in _themes.Values.Where(t => t.TenantId == tenantId))
        {
            t.IsActive = false;
        }

        var theme = await GetThemeByIdAsync(tenantId, themeId).ConfigureAwait(false);
        if (theme == null)
        {
            throw new KeyNotFoundException($"Theme {themeId} not found");
        }

        theme.IsActive = true;
        return theme;
    }

    public async Task<Theme> DeactivateThemeAsync(Guid tenantId, Guid themeId)
    {
        var theme = await GetThemeByIdAsync(tenantId, themeId).ConfigureAwait(false);
        if (theme == null)
        {
            throw new KeyNotFoundException($"Theme {themeId} not found");
        }

        theme.IsActive = false;
        return theme;
    }

    #endregion
}
