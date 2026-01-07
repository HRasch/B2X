namespace B2Connect.LayoutService.Data;

/// <summary>
/// Request to add a new component to a section
/// </summary>
public class AddComponentRequest
{
    /// <summary>
    /// The type of component (e.g., "button", "text", "image")
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// The content/data for the component
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Translations for the content
    /// </summary>
    public Dictionary<string, string>? ContentTranslations { get; set; }

    /// <summary>
    /// The order position of the component
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Whether the component is visible
    /// </summary>
    public bool? IsVisible { get; set; }
}
