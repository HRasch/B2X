namespace B2Connect.LayoutService.Models;

public class ComponentDataBinding
{
    public string Service { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public IDictionary<string, string> Query { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public IDictionary<string, string> Mapping { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    public int CacheDurationSeconds { get; set; }
}
