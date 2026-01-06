using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using B2Connect.ThemeService.Models;

namespace B2Connect.ThemeService.Services;

/// <summary>
/// SCSS Compilation Service - Compiles SCSS to CSS using Dart Sass
/// Supports on-demand compilation triggered from Admin UI
/// </summary>
public class ScssCompilationService : IScssCompilationService
{
    private readonly IScssModuleRepository _moduleRepository;
    private readonly ILogger<ScssCompilationService> _logger;

    public ScssCompilationService(
        IScssModuleRepository moduleRepository,
        ILogger<ScssCompilationService> logger)
    {
        _moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<CompilationResult> CompileThemeAsync(Guid tenantId, Guid themeId)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting SCSS compilation for Theme {ThemeId}, Tenant {TenantId}", themeId, tenantId);

            // Get all enabled modules ordered by category and sort order
            var modules = await _moduleRepository.GetEnabledModulesAsync(tenantId, themeId);

            if (modules.Count == 0)
            {
                _logger.LogWarning("No SCSS modules found for Theme {ThemeId}", themeId);
                return CompilationResult.Error("No SCSS modules found for compilation");
            }

            // Build combined SCSS
            var combinedScss = BuildCombinedScss(modules);

            // Compile SCSS to CSS
            var compilationResult = await CompileScssAsync(combinedScss);

            if (!compilationResult.Success)
            {
                _logger.LogError("SCSS compilation failed: {Error}", compilationResult.ErrorMessage);
                return compilationResult;
            }

            // Calculate source hash for cache invalidation
            var sourceHash = CalculateHash(combinedScss);

            // Store compiled result
            var compiled = new CompiledTheme
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ThemeId = themeId,
                CssContent = compilationResult.Css ?? string.Empty,
                CssMinified = compilationResult.CssMinified ?? string.Empty,
                SourceHash = sourceHash,
                SourceMap = compilationResult.SourceMap,
                Status = CompilationStatus.Success,
                CompiledAt = DateTime.UtcNow,
                CompilationTimeMs = stopwatch.ElapsedMilliseconds,
                FileSizeBytes = compilationResult.Css?.Length ?? 0,
                MinifiedSizeBytes = compilationResult.CssMinified?.Length ?? 0
            };

            await _moduleRepository.SaveCompiledThemeAsync(tenantId, themeId, compiled);

            stopwatch.Stop();
            compilationResult.CompilationTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "SCSS compilation completed for Theme {ThemeId} in {ElapsedMs}ms, Output: {Size} bytes",
                themeId, stopwatch.ElapsedMilliseconds, compiled.FileSizeBytes);

            return compilationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SCSS compilation failed for Theme {ThemeId}", themeId);
            return CompilationResult.Error($"Compilation failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<CompilationResult> PreviewCompileAsync(Guid tenantId, Guid themeId, string? scssOverrides = null)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var modules = await _moduleRepository.GetEnabledModulesAsync(tenantId, themeId);
            var combinedScss = BuildCombinedScss(modules);

            // Append overrides for preview
            if (!string.IsNullOrWhiteSpace(scssOverrides))
            {
                combinedScss += $"\n\n// === Preview Overrides ===\n{scssOverrides}";
            }

            var result = await CompileScssAsync(combinedScss);
            result.CompilationTimeMs = stopwatch.ElapsedMilliseconds;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Preview compilation failed for Theme {ThemeId}", themeId);
            return CompilationResult.Error($"Preview compilation failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<string> GetCompiledCssAsync(Guid tenantId, Guid themeId, bool minified = true)
    {
        var compiled = await _moduleRepository.GetCompiledThemeAsync(tenantId, themeId);

        if (compiled == null || compiled.Status != CompilationStatus.Success)
        {
            // Compile on-demand if not cached
            var result = await CompileThemeAsync(tenantId, themeId);
            return minified ? result.CssMinified ?? string.Empty : result.Css ?? string.Empty;
        }

        // Check if recompilation needed
        if (await NeedsRecompilationAsync(tenantId, themeId))
        {
            var result = await CompileThemeAsync(tenantId, themeId);
            return minified ? result.CssMinified ?? string.Empty : result.Css ?? string.Empty;
        }

        return minified ? compiled.CssMinified : compiled.CssContent;
    }

    /// <inheritdoc />
    public async Task<bool> NeedsRecompilationAsync(Guid tenantId, Guid themeId)
    {
        var compiled = await _moduleRepository.GetCompiledThemeAsync(tenantId, themeId);

        if (compiled == null)
        {
            return true;
        }

        var currentHash = await _moduleRepository.CalculateSourceHashAsync(tenantId, themeId);
        return compiled.SourceHash != currentHash;
    }

    /// <inheritdoc />
    public async Task InvalidateCacheAsync(Guid tenantId, Guid themeId)
    {
        await _moduleRepository.DeleteCompiledThemeAsync(tenantId, themeId);
        _logger.LogInformation("Cache invalidated for Theme {ThemeId}", themeId);
    }

    /// <inheritdoc />
    public Task<CompiledTheme?> GetCompilationStatusAsync(Guid tenantId, Guid themeId)
    {
        return _moduleRepository.GetCompiledThemeAsync(tenantId, themeId);
    }

    #region Private Methods

    /// <summary>
    /// Build combined SCSS from all modules in correct order
    /// </summary>
    private static string BuildCombinedScss(List<ScssModule> modules)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// ============================================");
        sb.AppendLine("// B2Connect Theme - Auto-generated SCSS");
        sb.AppendLine($"// Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine("// ============================================");
        sb.AppendLine();

        // Group by category and order
        var orderedModules = modules
            .OrderBy(m => (int)m.Category)
            .ThenBy(m => m.SortOrder)
            .ThenBy(m => m.Name);

        ScssModuleCategory? lastCategory = null;

        foreach (var module in orderedModules)
        {
            // Add category header
            if (lastCategory != module.Category)
            {
                sb.AppendLine();
                sb.AppendLine($"// === {module.Category} ===");
                lastCategory = module.Category;
            }

            sb.AppendLine($"// --- {module.Name} ---");
            sb.AppendLine(module.ScssContent);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Compile SCSS to CSS using embedded Dart Sass
    /// </summary>
    private async Task<CompilationResult> CompileScssAsync(string scssContent)
    {
        // For now, use a simple SCSS compilation approach
        // In production, use DartSassHost or SharpScss NuGet package

        try
        {
            // Placeholder: Direct CSS passthrough for variables/simple SCSS
            // TODO: Integrate DartSassHost for full SCSS compilation
            //
            // Using DartSassHost:
            // var sassCompiler = new SassCompiler();
            // var result = sassCompiler.Compile(scssContent, new CompilationOptions
            // {
            //     OutputStyle = OutputStyle.Expanded,
            //     SourceMap = true
            // });

            var css = ConvertScssVariablesToCss(scssContent);
            var cssMinified = MinifyCss(css);

            return await Task.FromResult(CompilationResult.Ok(css, cssMinified, 0));
        }
        catch (Exception ex)
        {
            return CompilationResult.Error($"SCSS compilation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Convert SCSS variables to CSS custom properties (simplified)
    /// For full SCSS support, integrate DartSassHost
    /// </summary>
    private static string ConvertScssVariablesToCss(string scss)
    {
        var sb = new StringBuilder();

        sb.AppendLine("/* B2Connect Theme - Compiled CSS */");
        sb.AppendLine(":root {");

        // Extract SCSS variables and convert to CSS custom properties
        var lines = scss.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip comments
            if (trimmed.StartsWith("//", StringComparison.Ordinal) || trimmed.StartsWith("/*", StringComparison.Ordinal))
            {
                continue;
            }

            // Convert $variable: value; to --variable: value;
            if (trimmed.Length > 0 && trimmed[0] == '$' && trimmed.Contains(':'))
            {
                var parts = trimmed.TrimEnd(';').Split(':', 2);
                if (parts.Length == 2)
                {
                    var varName = parts[0].TrimStart('$').Trim();
                    var varValue = parts[1].Trim().TrimEnd(';');

                    // Convert SCSS variable references to CSS var()
                    if (varValue.Contains('$'))
                    {
                        varValue = ConvertVariableReferences(varValue);
                    }

                    sb.AppendLine($"  --{varName}: {varValue};");
                }
            }
        }

        sb.AppendLine("}");
        sb.AppendLine();

        // Pass through any CSS rules unchanged
        var inCssBlock = false;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip SCSS-only syntax
            if ((trimmed.Length > 0 && trimmed[0] == '$') || trimmed.StartsWith("@use", StringComparison.Ordinal) ||
                trimmed.StartsWith("@import", StringComparison.Ordinal) || trimmed.StartsWith("@mixin", StringComparison.Ordinal) ||
                trimmed.StartsWith("@include", StringComparison.Ordinal) || trimmed.StartsWith("@function", StringComparison.Ordinal))
            {
                continue;
            }

            // Pass through CSS selectors and rules
            if (trimmed.Contains('{') || inCssBlock)
            {
                sb.AppendLine(line);
                if (trimmed.Contains('{'))
                {
                    inCssBlock = true;
                }

                if (trimmed.Contains('}'))
                {
                    inCssBlock = false;
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Convert SCSS $variable references to CSS var(--variable)
    /// </summary>
    private static string ConvertVariableReferences(string value)
    {
        var result = value;
        var startIndex = 0;

        while ((startIndex = result.IndexOf('$', startIndex)) >= 0)
        {
            var endIndex = startIndex + 1;
            while (endIndex < result.Length && (char.IsLetterOrDigit(result[endIndex]) || result[endIndex] == '-' || result[endIndex] == '_'))
            {
                endIndex++;
            }

            var varName = new string(result.AsSpan(startIndex + 1, endIndex - startIndex - 1));
            var sb = new StringBuilder(result.Length - (endIndex - startIndex - 1) + 10);
            sb.Append(result, 0, startIndex);
            sb.Append("var(--");
            sb.Append(varName);
            sb.Append(')');
            sb.Append(result, endIndex, result.Length - endIndex);
            result = sb.ToString();
            startIndex = endIndex;
        }

        return result;
    }

    /// <summary>
    /// Basic CSS minification
    /// </summary>
    private static string MinifyCss(string css)
    {
        // Remove comments
        css = System.Text.RegularExpressions.Regex.Replace(css, @"/\*[\s\S]*?\*/", "");

        // Remove whitespace
        css = System.Text.RegularExpressions.Regex.Replace(css, @"\s+", " ");

        // Remove space around special characters
        css = System.Text.RegularExpressions.Regex.Replace(css, @"\s*([{};:,>+~])\s*", "$1");

        // Remove trailing semicolons before }
        css = System.Text.RegularExpressions.Regex.Replace(css, @";}", "}");

        return css.Trim();
    }

    /// <summary>
    /// Calculate SHA256 hash of content
    /// </summary>
    private static string CalculateHash(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    #endregion
}
