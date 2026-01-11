namespace B2X.Search;

public class SearchRequest
{
    public string Query { get; set; } = "*";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Sector { get; set; }
    public string? Locale { get; set; }
}
