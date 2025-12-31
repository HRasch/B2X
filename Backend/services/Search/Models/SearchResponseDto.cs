namespace B2Connect.Search.Models;

public class SearchResponseDto
{
    public IEnumerable<ProductDocument> Products { get; set; } = Array.Empty<ProductDocument>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
