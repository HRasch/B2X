namespace B2X.LayoutService.Models;

public class ComponentDefinition
{
    public string ComponentType { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public IList<ComponentProp> Props { get; set; } = new List<ComponentProp>();
    public IList<ComponentSlot> Slots { get; set; } = new List<ComponentSlot>();
    public IList<ComponentPreset> PresetVariants { get; set; } = new List<ComponentPreset>();
}
