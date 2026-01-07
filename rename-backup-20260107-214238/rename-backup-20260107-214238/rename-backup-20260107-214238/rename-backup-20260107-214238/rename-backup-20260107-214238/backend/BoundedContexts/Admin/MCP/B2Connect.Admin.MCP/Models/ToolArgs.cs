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

/// <summary>
/// Arguments for Tenant Management Tool
/// </summary>
public class TenantManagementArgs
{
    public string Action { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string? ResourceType { get; set; }
    public string? Configuration { get; set; }
}

/// <summary>
/// Arguments for Store Operations Tool
/// </summary>
public class StoreOperationsArgs
{
    public string Operation { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string? TimeRange { get; set; }
    public string? AnalysisType { get; set; }
}

/// <summary>
/// Arguments for Security & Compliance Tool
/// </summary>
public class SecurityComplianceArgs
{
    public string AssessmentType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string? SpecificComponent { get; set; }
    public bool IncludeRecommendations { get; set; } = true;
}

/// <summary>
/// Arguments for Performance Optimization Tool
/// </summary>
public class PerformanceOptimizationArgs
{
    public string Component { get; set; } = string.Empty;
    public string MetricType { get; set; } = string.Empty;
    public string? TimeRange { get; set; }
    public bool IncludeHistoricalData { get; set; } = true;
}

/// <summary>
/// Arguments for Integration Management Tool
/// </summary>
public class IntegrationManagementArgs
{
    public string IntegrationType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Endpoint { get; set; }
    public string? Configuration { get; set; }
}

/// <summary>
/// Arguments for Intelligent Assistant Tool
/// </summary>
public class IntelligentAssistantArgs
{
    public string Message { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
    public string? UserId { get; set; }
    public bool EnableLearning { get; set; } = true;
}

/// <summary>
/// Arguments for Template Validation Tool
/// </summary>
public class TemplateValidationArgs
{
    public string TemplateKey { get; set; } = string.Empty;
    public string TemplateContent { get; set; } = string.Empty;
}