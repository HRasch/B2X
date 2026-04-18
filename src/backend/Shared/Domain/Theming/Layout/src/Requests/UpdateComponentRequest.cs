namespace B2X.LayoutService.Data;

/// <summary>
/// Request to update an existing component
/// </summary>
public class UpdateComponentRequest
{
    /// <summary>
    /// The updated content/data for the component
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Updated translations for the content
    /// </summary>
    public Dictionary<string, string>? ContentTranslations { get; set; }

    /// <summary>
    /// The updated order position of the component
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Whether the component is visible
    /// </summary>
    public bool? IsVisible { get; set; }
}
