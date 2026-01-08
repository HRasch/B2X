namespace B2X.LayoutService.Models;

/// <summary>
/// CMS Component - Individual UI element within a section
/// </summary>
public class CmsComponent
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Type { get; set; } = null!;
    public string Content { get; set; } = null!;
    public IDictionary<string, string> ContentTranslations { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public IList<ComponentVariable> Variables { get; set; } = new List<ComponentVariable>();
    public IDictionary<string, string> Styling { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public ComponentDataBinding? DataBinding { get; set; }
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
