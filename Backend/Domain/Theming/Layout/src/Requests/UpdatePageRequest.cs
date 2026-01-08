namespace B2X.LayoutService.Models;

public class UpdatePageRequest
{
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public PageVisibility? Visibility { get; set; }
    public IDictionary<string, PageTranslation>? Translations { get; set; }
}
