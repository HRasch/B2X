namespace B2Connect.LayoutService.Models;

/// <summary>
/// CMS Page - Represents a single customizable page in the storefront
/// </summary>
public class CmsPage
{
    /// <summary>Unique identifier for the page</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant ID for multi-tenant isolation</summary>
    public Guid TenantId { get; set; }

    /// <summary>Page title (displayed in browser tab)</summary>
    public string Title { get; set; }

    /// <summary>URL slug for the page</summary>
    public string Slug { get; set; }

    /// <summary>Page description</summary>
    public string Description { get; set; }

    /// <summary>Sections within this page</summary>
    public List<CmsSection> Sections { get; set; } = new();

    /// <summary>Page visibility status</summary>
    public PageVisibility Visibility { get; set; } = PageVisibility.Draft;

    /// <summary>When the page was published</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>When the page should be auto-published (if scheduled)</summary>
    public DateTime? ScheduledPublishAt { get; set; }

    /// <summary>Version number for tracking changes</summary>
    public int Version { get; set; } = 1;

    /// <summary>When the page was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the page was last modified</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>User ID who created the page</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>User ID who last updated the page</summary>
    public Guid? UpdatedBy { get; set; }
}

/// <summary>
/// CMS Section - A container for components within a page
/// Examples: hero section, features section, testimonials, CTA, etc.
/// </summary>
public class CmsSection
{
    /// <summary>Unique identifier for the section</summary>
    public Guid Id { get; set; }

    /// <summary>Parent page ID</summary>
    public Guid PageId { get; set; }

    /// <summary>Section type (hero, features, testimonials, etc.)</summary>
    public string Type { get; set; }

    /// <summary>Display order within the page</summary>
    public int Order { get; set; }

    /// <summary>Layout configuration (full-width, 2-column, 3-column, etc.)</summary>
    public SectionLayout Layout { get; set; } = SectionLayout.FullWidth;

    /// <summary>Components within this section</summary>
    public List<CmsComponent> Components { get; set; } = new();

    /// <summary>Custom settings/properties for the section (JSON)</summary>
    public Dictionary<string, object> Settings { get; set; } = new();

    /// <summary>Custom styling overrides (CSS)</summary>
    public Dictionary<string, string> Styling { get; set; } = new();

    /// <summary>Whether the section is visible on the live site</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>When the section was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// CMS Component - Individual UI element within a section
/// Examples: button, text, image, product card, etc.
/// </summary>
public class CmsComponent
{
    /// <summary>Unique identifier for the component</summary>
    public Guid Id { get; set; }

    /// <summary>Parent section ID</summary>
    public Guid SectionId { get; set; }

    /// <summary>Component type (button, text, image, product-card, etc.)</summary>
    public string Type { get; set; }

    /// <summary>Component content (text, HTML, or data reference)</summary>
    public string Content { get; set; }

    /// <summary>Component-specific props and configuration</summary>
    public List<ComponentVariable> Variables { get; set; } = new();

    /// <summary>Component styling overrides</summary>
    public Dictionary<string, string> Styling { get; set; } = new();

    /// <summary>Data binding (e.g., dynamic product data)</summary>
    public ComponentDataBinding DataBinding { get; set; }

    /// <summary>Whether the component is visible</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Component display order within section</summary>
    public int Order { get; set; }

    /// <summary>When the component was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Component Variable - Props for a component (e.g., button color, text size)
/// </summary>
public class ComponentVariable
{
    /// <summary>Variable name</summary>
    public string Name { get; set; }

    /// <summary>Variable value</summary>
    public object Value { get; set; }

    /// <summary>Variable type (string, number, boolean, color, etc.)</summary>
    public string Type { get; set; }

    /// <summary>Description of what this variable does</summary>
    public string Description { get; set; }
}

/// <summary>
/// Component Data Binding - Connects component to dynamic data sources
/// </summary>
public class ComponentDataBinding
{
    /// <summary>Data source service (e.g., "catalog-service")</summary>
    public string Service { get; set; }

    /// <summary>API endpoint to fetch data from</summary>
    public string Endpoint { get; set; }

    /// <summary>Query parameters</summary>
    public Dictionary<string, string> Query { get; set; } = new();

    /// <summary>Mapping of component props to data fields</summary>
    public Dictionary<string, string> Mapping { get; set; } = new();

    /// <summary>Cache duration in seconds (0 = no cache)</summary>
    public int CacheDurationSeconds { get; set; }
}

/// <summary>
/// Component Definition - Schema for available components in the library
/// </summary>
public class ComponentDefinition
{
    /// <summary>Unique component type identifier</summary>
    public string ComponentType { get; set; }

    /// <summary>Human-readable component name</summary>
    public string DisplayName { get; set; }

    /// <summary>Component description</summary>
    public string Description { get; set; }

    /// <summary>Component category (UI, Layout, E-Commerce)</summary>
    public string Category { get; set; }

    /// <summary>Icon identifier for UI display</summary>
    public string Icon { get; set; }

    /// <summary>Available component props</summary>
    public List<ComponentProp> Props { get; set; } = new();

    /// <summary>Content slots (e.g., "default", "header", "footer")</summary>
    public List<ComponentSlot> Slots { get; set; } = new();

    /// <summary>Preset variants (e.g., "primary button", "secondary button")</summary>
    public List<ComponentPreset> PresetVariants { get; set; } = new();
}

/// <summary>
/// Component Prop - Definition of a component property
/// </summary>
public class ComponentProp
{
    /// <summary>Property name</summary>
    public string Name { get; set; }

    /// <summary>Property data type (text, number, boolean, select, color, etc.)</summary>
    public string Type { get; set; }

    /// <summary>Default value if not specified</summary>
    public object DefaultValue { get; set; }

    /// <summary>Available options for select type</summary>
    public List<string> Options { get; set; } = new();

    /// <summary>Validation rules</summary>
    public ComponentPropValidation Validation { get; set; }

    /// <summary>Property description</summary>
    public string Description { get; set; }

    /// <summary>Is this property required</summary>
    public bool IsRequired { get; set; }
}

/// <summary>
/// Component Prop Validation - Rules for validating component properties
/// </summary>
public class ComponentPropValidation
{
    /// <summary>Minimum value (for numbers)</summary>
    public int? MinValue { get; set; }

    /// <summary>Maximum value (for numbers)</summary>
    public int? MaxValue { get; set; }

    /// <summary>Minimum string length</summary>
    public int? MinLength { get; set; }

    /// <summary>Maximum string length</summary>
    public int? MaxLength { get; set; }

    /// <summary>Regex pattern to validate against</summary>
    public string Pattern { get; set; }

    /// <summary>Custom error message</summary>
    public string ErrorMessage { get; set; }
}

/// <summary>
/// Component Slot - Named content slot in a component
/// </summary>
public class ComponentSlot
{
    /// <summary>Slot name</summary>
    public string Name { get; set; }

    /// <summary>Slot description</summary>
    public string Description { get; set; }

    /// <summary>Whether the slot can accept multiple components</summary>
    public bool IsRepeatable { get; set; }
}

/// <summary>
/// Component Preset - Pre-configured variant of a component
/// </summary>
public class ComponentPreset
{
    /// <summary>Preset name (e.g., "Primary Button")</summary>
    public string Name { get; set; }

    /// <summary>Preset description</summary>
    public string Description { get; set; }

    /// <summary>Pre-configured props</summary>
    public Dictionary<string, object> Props { get; set; } = new();

    /// <summary>Preview image URL</summary>
    public string PreviewImage { get; set; }
}

/// <summary>
/// Page visibility status
/// </summary>
public enum PageVisibility
{
    /// <summary>Draft - not published</summary>
    Draft,

    /// <summary>Published - visible to public</summary>
    Published,

    /// <summary>Scheduled - will be published at specific time</summary>
    Scheduled,

    /// <summary>Archived - hidden from public</summary>
    Archived
}

/// <summary>
/// Section layout options
/// </summary>
public enum SectionLayout
{
    /// <summary>Full width</summary>
    FullWidth,

    /// <summary>2-column layout</summary>
    TwoColumn,

    /// <summary>3-column layout</summary>
    ThreeColumn,

    /// <summary>4-column layout</summary>
    FourColumn,

    /// <summary>Grid layout</summary>
    Grid,

    /// <summary>Sidebar layout</summary>
    Sidebar
}

#region Request/Response DTOs

/// <summary>Create Page Request DTO</summary>
public class CreatePageRequest
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
}

/// <summary>Update Page Request DTO</summary>
public class UpdatePageRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public PageVisibility? Visibility { get; set; }
}

/// <summary>Add Section Request DTO</summary>
public class AddSectionRequest
{
    public string Type { get; set; }
    public SectionLayout Layout { get; set; }
    public Dictionary<string, object> Settings { get; set; }
}

/// <summary>Add Component Request DTO</summary>
public class AddComponentRequest
{
    public string Type { get; set; }
    public string Content { get; set; }
    public List<ComponentVariable> Variables { get; set; }
    public Dictionary<string, string> Styling { get; set; }
}

/// <summary>Update Component Request DTO</summary>
public class UpdateComponentRequest
{
    public string Content { get; set; }
    public List<ComponentVariable> Variables { get; set; }
    public Dictionary<string, string> Styling { get; set; }
    public bool? IsVisible { get; set; }
}

/// <summary>Section Order Request DTO for reordering</summary>
public class SectionOrderRequest
{
    public Guid SectionId { get; set; }
    public int Order { get; set; }
}

#endregion
