namespace B2Connect.LayoutService.Models;

public class ComponentPreset
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IDictionary<string, object> Props { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);
    public string PreviewImage { get; set; } = null!;
}
