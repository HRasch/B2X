namespace B2Connect.ThemeService.Models;

/// <summary>
/// SCSS Module - Represents a tenant-specific SCSS file stored in database
/// Allows runtime customization of themes via Admin UI
/// </summary>
public class ScssModule
{
    /// <summary>Unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant ID for multi-tenant isolation</summary>
    public Guid TenantId { get; set; }

    /// <summary>Theme ID this module belongs to</summary>
    public Guid ThemeId { get; set; }

    /// <summary>Module name (e.g., "_variables", "_buttons", "_custom")</summary>
    public string Name { get; set; } = null!;

    /// <summary>Module category for organization and import order</summary>
    public ScssModuleCategory Category { get; set; }

    /// <summary>SCSS source code content</summary>
    public string ScssContent { get; set; } = string.Empty;

    /// <summary>Sort order for @import sequence within category</summary>
    public int SortOrder { get; set; }

    /// <summary>Whether this module is included in compilation</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Whether this is a system module (not editable by tenant)</summary>
    public bool IsSystem { get; set; }

    /// <summary>Optional description for documentation</summary>
    public string? Description { get; set; }

    /// <summary>When the module was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the module was last updated</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>User who last modified this module</summary>
    public Guid? UpdatedBy { get; set; }
}

/// <summary>
/// SCSS Module Categories - Determines compilation order
/// </summary>
public enum ScssModuleCategory
{
    /// <summary>Design tokens and variables (_variables.scss)</summary>
    Variables = 0,

    /// <summary>SCSS functions for color math, etc. (_functions.scss)</summary>
    Functions = 1,

    /// <summary>Reusable mixin patterns (_mixins.scss)</summary>
    Mixins = 2,

    /// <summary>Base styles, reset, typography (_base.scss)</summary>
    Base = 3,

    /// <summary>Component styles (_buttons.scss, _cards.scss)</summary>
    Components = 4,

    /// <summary>Utility class generators</summary>
    Utilities = 5,

    /// <summary>Tenant-specific custom SCSS (loaded last)</summary>
    Custom = 6
}

/// <summary>
/// Compiled Theme Cache - Stores compiled CSS output
/// </summary>
public class CompiledTheme
{
    /// <summary>Unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant ID</summary>
    public Guid TenantId { get; set; }

    /// <summary>Theme ID this compilation belongs to</summary>
    public Guid ThemeId { get; set; }

    /// <summary>Full compiled CSS output</summary>
    public string CssContent { get; set; } = string.Empty;

    /// <summary>Minified CSS for production use</summary>
    public string CssMinified { get; set; } = string.Empty;

    /// <summary>Hash of SCSS sources for cache invalidation</summary>
    public string SourceHash { get; set; } = string.Empty;

    /// <summary>CSS Source Map for debugging (optional)</summary>
    public string? SourceMap { get; set; }

    /// <summary>Compilation status</summary>
    public CompilationStatus Status { get; set; }

    /// <summary>Error message if compilation failed</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Error line number if compilation failed</summary>
    public int? ErrorLine { get; set; }

    /// <summary>Error column if compilation failed</summary>
    public int? ErrorColumn { get; set; }

    /// <summary>When compilation was performed</summary>
    public DateTime CompiledAt { get; set; } = DateTime.UtcNow;

    /// <summary>File size in bytes</summary>
    public long FileSizeBytes { get; set; }

    /// <summary>Minified file size in bytes</summary>
    public long MinifiedSizeBytes { get; set; }

    /// <summary>Compilation duration in milliseconds</summary>
    public long CompilationTimeMs { get; set; }
}

/// <summary>
/// Compilation Status
/// </summary>
public enum CompilationStatus
{
    /// <summary>Compilation not yet started</summary>
    Pending,

    /// <summary>Compilation in progress</summary>
    Compiling,

    /// <summary>Compilation completed successfully</summary>
    Success,

    /// <summary>Compilation failed with errors</summary>
    Failed
}

#region SCSS Request/Response DTOs

/// <summary>Create SCSS Module Request</summary>
public class CreateScssModuleRequest
{
    public string? Name { get; set; }
    public ScssModuleCategory Category { get; set; }
    public string? ScssContent { get; set; }
    public int SortOrder { get; set; }
    public string? Description { get; set; }
}

/// <summary>Update SCSS Module Request</summary>
public class UpdateScssModuleRequest
{
    public string? Name { get; set; }
    public string? ScssContent { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsEnabled { get; set; }
    public string? Description { get; set; }
}

/// <summary>Preview Compile Request</summary>
public class PreviewCompileRequest
{
    /// <summary>Optional SCSS overrides for preview</summary>
    public string? ScssOverrides { get; set; }

    /// <summary>Only compile specific modules</summary>
    public List<Guid>? ModuleIds { get; set; }
}

/// <summary>Compilation Result</summary>
public class CompilationResult
{
    /// <summary>Whether compilation succeeded</summary>
    public bool Success { get; set; }

    /// <summary>Compiled CSS output</summary>
    public string? Css { get; set; }

    /// <summary>Minified CSS output</summary>
    public string? CssMinified { get; set; }

    /// <summary>Source map for debugging</summary>
    public string? SourceMap { get; set; }

    /// <summary>Error message if failed</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Error line number</summary>
    public int? ErrorLine { get; set; }

    /// <summary>Error column</summary>
    public int? ErrorColumn { get; set; }

    /// <summary>Compilation duration in milliseconds</summary>
    public long CompilationTimeMs { get; set; }

    /// <summary>Output file size in bytes</summary>
    public long FileSizeBytes { get; set; }

    /// <summary>List of warnings during compilation</summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>Create successful result</summary>
    public static CompilationResult Ok(string css, string cssMinified, long timeMs) => new()
    {
        Success = true,
        Css = css,
        CssMinified = cssMinified,
        CompilationTimeMs = timeMs,
        FileSizeBytes = css?.Length ?? 0
    };

    /// <summary>Create failed result</summary>
    public static CompilationResult Error(string message, int? line = null, int? column = null) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorLine = line,
        ErrorColumn = column
    };
}

#endregion
