namespace B2X.LayoutService.Models;

/// <summary>
/// CMS Section - A container for components within a page
/// </summary>
public class CmsSection
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Type { get; set; } = null!;
    public int Order { get; set; }
    public SectionLayout Layout { get; set; } = SectionLayout.FullWidth;
    public IList<CmsComponent> Components { get; set; } = new List<CmsComponent>();
    public IDictionary<string, object> Settings { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);
    public IDictionary<string, string> Styling { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
