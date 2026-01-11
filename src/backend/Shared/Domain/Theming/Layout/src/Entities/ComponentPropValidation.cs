namespace B2X.LayoutService.Models;

public class ComponentPropValidation
{
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string Pattern { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
}
