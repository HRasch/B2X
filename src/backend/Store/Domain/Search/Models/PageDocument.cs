namespace B2X.Domain.Search.Models;

public class PageDocument
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string PageType { get; set; } = string.Empty;
    public string PagePath { get; set; } = string.Empty;
    public string PageTitle { get; set; } = string.Empty;
    public string PageDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;
    public string TemplateLayout { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Version { get; set; }
    public string Locale { get; set; } = "en";
}
