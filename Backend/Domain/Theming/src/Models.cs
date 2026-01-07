namespace B2Connect.ThemeService.Models;

/// <summary>
/// Theme - Represents a design theme with colors, typography, and variables
/// </summary>
public class Theme
{
    /// <summary>Unique identifier for the theme</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant ID for multi-tenant isolation</summary>
    public Guid TenantId { get; set; }

    /// <summary>Theme name (unique per tenant)</summary>
    public string? Name { get; set; }

    /// <summary>Theme description</summary>
    public string? Description { get; set; }

    /// <summary>Primary brand color (hex format)</summary>
    public string? PrimaryColor { get; set; }

    /// <summary>Secondary brand color (hex format)</summary>
    public string? SecondaryColor { get; set; }

    /// <summary>Tertiary brand color (hex format)</summary>
    public string? TertiaryColor { get; set; }

    /// <summary>Design variables (colors, spacing, fonts, etc.)</summary>
    public List<DesignVariable> Variables { get; set; } = new();

    /// <summary>Theme variants (dark mode, etc.)</summary>
    public List<ThemeVariant> Variants { get; set; } = new();

    /// <summary>Whether this theme is currently active/published</summary>
    public bool IsActive { get; set; }

    /// <summary>When the theme was published</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Version number for tracking changes</summary>
    public int Version { get; set; } = 1;

    /// <summary>When the theme was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the theme was last updated</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>User ID who created the theme</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>User ID who last updated the theme</summary>
    public Guid? UpdatedBy { get; set; }
}

/// <summary>
/// Design Variable - CSS custom property or design token
/// </summary>
public class DesignVariable
{
    /// <summary>Unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Theme ID</summary>
    public Guid ThemeId { get; set; }

    /// <summary>Variable name (e.g., primary-color, spacing-unit)</summary>
    public string Name { get; set; } = null!;

    /// <summary>Variable value (hex, size, etc.)</summary>
    public string Value { get; set; } = null!;

    /// <summary>Variable category (Colors, Spacing, Typography, etc.)</summary>
    public string Category { get; set; } = null!;

    /// <summary>Variable description</summary>
    public string Description { get; set; } = null!;

    /// <summary>Data type of the value</summary>
    public VariableType Type { get; set; } = VariableType.String;

    /// <summary>When the variable was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the variable was last updated</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Theme Variant - Different variations of a theme (e.g., dark mode)
/// </summary>
public class ThemeVariant
{
    /// <summary>Unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Theme ID this variant belongs to</summary>
    public Guid ThemeId { get; set; }

    /// <summary>Variant name (e.g., Dark Mode, High Contrast)</summary>
    public string Name { get; set; } = null!;

    /// <summary>Variant description</summary>
    public string Description { get; set; } = null!;

    /// <summary>Overridden variables for this variant</summary>
    public Dictionary<string, string> VariableOverrides { get; set; } = new(StringComparer.Ordinal);

    /// <summary>Whether this variant is enabled</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>When the variant was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Design Token - Extracted from design systems (Figma, etc.)
/// </summary>
public class DesignToken
{
    /// <summary>Token name</summary>
    public string Name { get; set; } = null!;

    /// <summary>Token value</summary>
    public string Value { get; set; } = null!;

    /// <summary>Token category</summary>
    public string Category { get; set; } = null!;

    /// <summary>Token type</summary>
    public VariableType Type { get; set; } = VariableType.String;

    /// <summary>Source system (Figma, etc.)</summary>
    public string Source { get; set; } = null!;

    /// <summary>Path in design system</summary>
    public string Path { get; set; } = null!;
}

/// <summary>
/// Variable type enumeration
/// </summary>
public enum VariableType
{
    /// <summary>Text/string value</summary>
    String,

    /// <summary>Color value (hex, rgb, etc.)</summary>
    Color,

    /// <summary>Size value (px, rem, etc.)</summary>
    Size,

    /// <summary>Number value</summary>
    Number,

    /// <summary>Boolean value</summary>
    Boolean,

    /// <summary>JSON object</summary>
    JSON
}

#region Request/Response DTOs

/// <summary>Create Theme Request DTO</summary>
public class CreateThemeRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? TertiaryColor { get; set; }
}

/// <summary>Update Theme Request DTO</summary>
public class UpdateThemeRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? TertiaryColor { get; set; }
}

/// <summary>Add Design Variable Request DTO</summary>
public class AddDesignVariableRequest
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public VariableType Type { get; set; } = VariableType.String;
}

/// <summary>Update Design Variable Request DTO</summary>
public class UpdateDesignVariableRequest
{
    public string? Value { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public VariableType Type { get; set; }
}

/// <summary>Create Theme Variant Request DTO</summary>
public class CreateThemeVariantRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string>? VariableOverrides { get; set; }
}

/// <summary>Update Theme Variant Request DTO</summary>
public class UpdateThemeVariantRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string>? VariableOverrides { get; set; }
    public bool? IsEnabled { get; set; }
}

#endregion
