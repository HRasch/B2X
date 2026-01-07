namespace B2Connect.LayoutService.Data;

/// <summary>
/// Request to specify section ordering
/// </summary>
public class SectionOrderRequest
{
    /// <summary>
    /// The ID of the section to reorder
    /// </summary>
    public required Guid SectionId { get; set; }

    /// <summary>
    /// The new order position for the section
    /// </summary>
    public required int Order { get; set; }
}
