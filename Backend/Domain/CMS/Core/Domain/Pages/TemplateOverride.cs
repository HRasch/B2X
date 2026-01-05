namespace B2Connect.CMS.Core.Domain.Pages;

/// <summary>
/// Represents a tenant-specific template override (ADR-030)
/// Used to store customized versions of page templates for specific tenants
/// </summary>
public class TemplateOverride
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public string? BaseTemplateKey { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public Dictionary<string, string> OverrideSections { get; set; } = new();
    public int Version { get; set; } = 1;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }

    public TemplateOverride()
    {
    }

    public TemplateOverride(
        Guid tenantId,
        string templateKey,
        string? baseTemplateKey,
        string templateContent,
        Dictionary<string, string> overrideSections,
        string createdBy)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        TemplateKey = templateKey;
        BaseTemplateKey = baseTemplateKey;
        TemplateContent = templateContent;
        OverrideSections = overrideSections;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        Version = 1;
        IsPublished = false;
    }

    public void Update(string templateContent, Dictionary<string, string> overrideSections, string updatedBy)
    {
        Version++;
        TemplateContent = templateContent;
        OverrideSections = overrideSections;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
        IsPublished = false; // Unpublish on update
    }

    public void Publish(string publishedBy)
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        PublishedBy = publishedBy;
    }

    public void Revert(int targetVersion)
    {
        if (targetVersion >= Version)
            throw new InvalidOperationException("Cannot revert to a version equal to or greater than current version");
        // Implementation would load version from history
        Version++;
    }
}
