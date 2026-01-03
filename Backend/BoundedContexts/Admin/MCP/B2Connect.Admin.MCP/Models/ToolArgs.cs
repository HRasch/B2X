namespace B2Connect.Admin.MCP.Models;

/// <summary>
/// Arguments for CMS Page Design Tool
/// </summary>
public class CmsPageDesignArgs
{
    public string PageType { get; set; } = string.Empty;
    public string ContentRequirements { get; set; } = string.Empty;
    public string? TargetAudience { get; set; }
}

/// <summary>
/// Arguments for Email Template Design Tool
/// </summary>
public class EmailTemplateDesignArgs
{
    public string TemplateType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? BrandGuidelines { get; set; }
    public string? EmailType { get; set; }
    public string? ContentPurpose { get; set; }
}

/// <summary>
/// Arguments for System Health Analysis Tool
/// </summary>
public class SystemHealthAnalysisArgs
{
    public string Component { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
}

/// <summary>
/// Arguments for User Management Assistant Tool
/// </summary>
public class UserManagementAssistantArgs
{
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? AdditionalData { get; set; }
    public string? Task { get; set; }
    public string? UserContext { get; set; }
}

/// <summary>
/// Arguments for Content Optimization Tool
/// </summary>
public class ContentOptimizationArgs
{
    public string ContentType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TargetKeywords { get; set; }
}