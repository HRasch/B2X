namespace B2Connect.Domain.Search.Models;

public class ProductDocument
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Available { get; set; }
    public string Locale { get; set; } = "en";
}
