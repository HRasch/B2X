namespace B2Connect.LayoutService.Models;

public class CreatePageRequest
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IDictionary<string, PageTranslation> Translations { get; set; } = new Dictionary<string, PageTranslation>(StringComparer.Ordinal);
}
