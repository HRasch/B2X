using System.Security.Cryptography;
using System.Text;
using B2Connect.ThemeService.Models;

namespace B2Connect.ThemeService.Repositories;

/// <summary>
/// In-Memory SCSS Module Repository for development/testing
/// Replace with PostgreSQL implementation for production
/// </summary>
public class InMemoryScssModuleRepository : IScssModuleRepository
{
    private readonly Dictionary<(Guid TenantId, Guid ThemeId, Guid ModuleId), ScssModule> _modules = new();
    private readonly Dictionary<(Guid TenantId, Guid ThemeId), CompiledTheme> _compiledThemes = new();
    private readonly object _lock = new();

    #region Module CRUD Operations

    public Task<ScssModule> CreateModuleAsync(Guid tenantId, Guid themeId, ScssModule module)
    {
        lock (_lock)
        {
            module.Id = Guid.NewGuid();
            module.TenantId = tenantId;
            module.ThemeId = themeId;
            module.CreatedAt = DateTime.UtcNow;
            module.UpdatedAt = DateTime.UtcNow;

            _modules[(tenantId, themeId, module.Id)] = module;
            return Task.FromResult(module);
        }
    }

    public Task<ScssModule?> GetModuleByIdAsync(Guid tenantId, Guid themeId, Guid moduleId)
    {
        lock (_lock)
        {
            _modules.TryGetValue((tenantId, themeId, moduleId), out var module);
            return Task.FromResult(module);
        }
    }

    public Task<List<ScssModule>> GetModulesByThemeAsync(Guid tenantId, Guid themeId)
    {
        lock (_lock)
        {
            var modules = _modules.Values
                .Where(m => m.TenantId == tenantId && m.ThemeId == themeId)
                .OrderBy(m => (int)m.Category)
                .ThenBy(m => m.SortOrder)
                .ThenBy(m => m.Name)
                .ToList();
            return Task.FromResult(modules);
        }
    }

    public Task<List<ScssModule>> GetEnabledModulesAsync(Guid tenantId, Guid themeId)
    {
        lock (_lock)
        {
            var modules = _modules.Values
                .Where(m => m.TenantId == tenantId && m.ThemeId == themeId && m.IsEnabled)
                .OrderBy(m => (int)m.Category)
                .ThenBy(m => m.SortOrder)
                .ThenBy(m => m.Name)
                .ToList();
            return Task.FromResult(modules);
        }
    }

    public Task<List<ScssModule>> GetModulesByCategoryAsync(Guid tenantId, Guid themeId, ScssModuleCategory category)
    {
        lock (_lock)
        {
            var modules = _modules.Values
                .Where(m => m.TenantId == tenantId && m.ThemeId == themeId && m.Category == category)
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.Name)
                .ToList();
            return Task.FromResult(modules);
        }
    }

    public Task<ScssModule> UpdateModuleAsync(Guid tenantId, Guid themeId, Guid moduleId, ScssModule module)
    {
        lock (_lock)
        {
            var key = (tenantId, themeId, moduleId);
            if (!_modules.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Module {moduleId} not found");
            }

            module.UpdatedAt = DateTime.UtcNow;
            _modules[key] = module;
            return Task.FromResult(module);
        }
    }

    public Task DeleteModuleAsync(Guid tenantId, Guid themeId, Guid moduleId)
    {
        lock (_lock)
        {
            _modules.Remove((tenantId, themeId, moduleId));
            return Task.CompletedTask;
        }
    }

    public Task<bool> ModuleNameExistsAsync(Guid tenantId, Guid themeId, string name)
    {
        lock (_lock)
        {
            var exists = _modules.Values.Any(m =>
                m.TenantId == tenantId &&
                m.ThemeId == themeId &&
                m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }

    #endregion

    #region Compiled Theme Cache

    public Task<CompiledTheme?> GetCompiledThemeAsync(Guid tenantId, Guid themeId)
    {
        lock (_lock)
        {
            _compiledThemes.TryGetValue((tenantId, themeId), out var compiled);
            return Task.FromResult(compiled);
        }
    }

    public Task<CompiledTheme> SaveCompiledThemeAsync(Guid tenantId, Guid themeId, CompiledTheme compiled)
    {
        lock (_lock)
        {
            compiled.TenantId = tenantId;
            compiled.ThemeId = themeId;
            _compiledThemes[(tenantId, themeId)] = compiled;
            return Task.FromResult(compiled);
        }
    }

    public Task DeleteCompiledThemeAsync(Guid tenantId, Guid themeId)
    {
        lock (_lock)
        {
            _compiledThemes.Remove((tenantId, themeId));
            return Task.CompletedTask;
        }
    }

    public Task<string> CalculateSourceHashAsync(Guid tenantId, Guid themeId)
    {
        lock (_lock)
        {
            var modules = _modules.Values
                .Where(m => m.TenantId == tenantId && m.ThemeId == themeId && m.IsEnabled)
                .OrderBy(m => (int)m.Category)
                .ThenBy(m => m.SortOrder);

            var combined = string.Join("|", modules.Select(m => $"{m.Name}:{m.ScssContent}:{m.UpdatedAt:O}"));
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = SHA256.HashData(bytes);
            return Task.FromResult(Convert.ToHexString(hash).ToLowerInvariant());
        }
    }

    #endregion

    #region Bulk Operations

    public async Task CloneModulesToThemeAsync(Guid tenantId, Guid sourceThemeId, Guid targetThemeId)
    {
        var sourceModules = await GetModulesByThemeAsync(tenantId, sourceThemeId);

        foreach (var source in sourceModules)
        {
            var clone = new ScssModule
            {
                Name = source.Name,
                Category = source.Category,
                ScssContent = source.ScssContent,
                SortOrder = source.SortOrder,
                IsEnabled = source.IsEnabled,
                IsSystem = source.IsSystem,
                Description = source.Description
            };

            await CreateModuleAsync(tenantId, targetThemeId, clone);
        }
    }

    public async Task ImportDefaultModulesAsync(Guid tenantId, Guid themeId)
    {
        // Import default SCSS framework modules
        var defaultModules = GetDefaultScssModules();

        foreach (var module in defaultModules)
        {
            module.TenantId = tenantId;
            module.ThemeId = themeId;
            module.IsSystem = true;
            await CreateModuleAsync(tenantId, themeId, module);
        }
    }

    /// <summary>
    /// Get default SCSS framework modules (Bootstrap-style)
    /// </summary>
    private static List<ScssModule> GetDefaultScssModules()
    {
        return new List<ScssModule>
        {
            // Variables
            new()
            {
                Name = "_variables",
                Category = ScssModuleCategory.Variables,
                SortOrder = 0,
                Description = "Design tokens and CSS custom properties",
                ScssContent = GetDefaultVariablesScss()
            },
            // Functions
            new()
            {
                Name = "_functions",
                Category = ScssModuleCategory.Functions,
                SortOrder = 0,
                Description = "Color manipulation and utility functions",
                ScssContent = GetDefaultFunctionsScss()
            },
            // Mixins
            new()
            {
                Name = "_mixins",
                Category = ScssModuleCategory.Mixins,
                SortOrder = 0,
                Description = "Reusable component patterns",
                ScssContent = GetDefaultMixinsScss()
            },
            // Base
            new()
            {
                Name = "_base",
                Category = ScssModuleCategory.Base,
                SortOrder = 0,
                Description = "Base styles and typography",
                ScssContent = GetDefaultBaseScss()
            }
        };
    }

    private static string GetDefaultVariablesScss() => """
        // ============================================
        // Design Tokens - CSS Custom Properties
        // ============================================

        // Brand Colors
        $primary: #0066cc;
        $primary-light: #3399ff;
        $primary-dark: #004499;
        $secondary: #6c757d;
        $success: #28a745;
        $warning: #ffc107;
        $danger: #dc3545;
        $info: #17a2b8;

        // Neutral Colors
        $white: #ffffff;
        $black: #000000;
        $gray-100: #f8f9fa;
        $gray-200: #e9ecef;
        $gray-300: #dee2e6;
        $gray-400: #ced4da;
        $gray-500: #adb5bd;
        $gray-600: #6c757d;
        $gray-700: #495057;
        $gray-800: #343a40;
        $gray-900: #212529;

        // Typography
        $font-family-base: system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
        $font-family-heading: $font-family-base;
        $font-family-mono: SFMono-Regular, Menlo, Monaco, Consolas, monospace;

        $font-size-base: 1rem;
        $font-size-sm: 0.875rem;
        $font-size-lg: 1.125rem;
        $font-size-xl: 1.25rem;

        $font-weight-light: 300;
        $font-weight-normal: 400;
        $font-weight-medium: 500;
        $font-weight-semibold: 600;
        $font-weight-bold: 700;

        $line-height-base: 1.5;
        $line-height-sm: 1.25;
        $line-height-lg: 2;

        // Spacing Scale (8px base)
        $spacing-1: 0.25rem;
        $spacing-2: 0.5rem;
        $spacing-3: 0.75rem;
        $spacing-4: 1rem;
        $spacing-5: 1.5rem;
        $spacing-6: 2rem;
        $spacing-8: 3rem;
        $spacing-10: 4rem;
        $spacing-12: 6rem;

        // Border Radius
        $radius-sm: 0.25rem;
        $radius-md: 0.375rem;
        $radius-lg: 0.5rem;
        $radius-xl: 1rem;
        $radius-full: 9999px;

        // Shadows
        $shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
        $shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1);
        $shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1);
        $shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.1);

        // Transitions
        $transition-fast: 150ms ease;
        $transition-base: 200ms ease;
        $transition-slow: 300ms ease;

        // Breakpoints
        $breakpoint-sm: 640px;
        $breakpoint-md: 768px;
        $breakpoint-lg: 1024px;
        $breakpoint-xl: 1280px;
        $breakpoint-2xl: 1536px;

        // Z-Index Scale
        $z-dropdown: 1000;
        $z-sticky: 1020;
        $z-fixed: 1030;
        $z-modal-backdrop: 1040;
        $z-modal: 1050;
        $z-popover: 1060;
        $z-tooltip: 1070;
        """;

    private static string GetDefaultFunctionsScss() => """
        // ============================================
        // SCSS Functions - Color Math & Utilities
        // ============================================

        // Note: These are SCSS functions that get compiled to CSS
        // For runtime, we use CSS custom properties

        // Color tint (lighten towards white)
        // Usage: tint-color($primary, 20%)
        // Compiles to calculated value

        // Color shade (darken towards black)
        // Usage: shade-color($primary, 20%)
        // Compiles to calculated value

        // Color contrast (choose text color based on background)
        // Returns black or white based on luminance

        // These functions require full Dart Sass support
        // The simplified compiler converts variables to CSS custom properties
        """;

    private static string GetDefaultMixinsScss() => """
        // ============================================
        // SCSS Mixins - Reusable Patterns
        // ============================================

        // Note: Mixins require full Dart Sass compilation
        // For now, we define CSS custom property patterns

        // Button variant colors are defined via CSS custom properties:
        // --btn-bg, --btn-color, --btn-border
        // --btn-hover-bg, --btn-hover-color, --btn-hover-border

        // Card styles use:
        // --card-bg, --card-border, --card-shadow, --card-radius

        // Form field styles use:
        // --input-bg, --input-border, --input-focus-border, --input-focus-ring
        """;

    private static string GetDefaultBaseScss() => """
        // ============================================
        // Base Styles
        // ============================================

        // Apply CSS custom properties to :root
        // These are generated from the variables above

        body {
            font-family: var(--font-family-base);
            font-size: var(--font-size-base);
            line-height: var(--line-height-base);
            color: var(--gray-900);
            background-color: var(--white);
        }

        h1, h2, h3, h4, h5, h6 {
            font-family: var(--font-family-heading);
            font-weight: var(--font-weight-semibold);
            line-height: var(--line-height-sm);
        }

        a {
            color: var(--primary);
            text-decoration: none;
            transition: color var(--transition-fast);
        }

        a:hover {
            color: var(--primary-dark);
        }
        """;

    #endregion
}
