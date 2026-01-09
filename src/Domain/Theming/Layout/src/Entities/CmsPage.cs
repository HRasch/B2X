namespace B2X.LayoutService.Models;

/// <summary>
/// CMS Page - Represents a single customizable page in the storefront
/// </summary>
public class CmsPage
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = null!;
    public IDictionary<string, string> TitleTranslations { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public string Slug { get; set; } = null!;
    public IDictionary<string, string> SlugTranslations { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public string Description { get; set; } = null!;
    public IDictionary<string, string> DescriptionTranslations { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public IList<CmsSection> Sections { get; set; } = new List<CmsSection>();
    public PageVisibility Visibility { get; set; } = PageVisibility.Draft;
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
