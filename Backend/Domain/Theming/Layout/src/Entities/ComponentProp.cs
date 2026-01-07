namespace B2Connect.LayoutService.Models;

public class ComponentProp
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public object DefaultValue { get; set; } = null!;
    public IList<string> Options { get; set; } = new List<string>();
    public ComponentPropValidation? Validation { get; set; }
    public string Description { get; set; } = null!;
    public bool IsRequired { get; set; }
}
