namespace B2X.Search;

public class SearchQuery
{
    public string Query { get; set; } = "*";
    public string? Filters { get; set; }
    public string? Sort { get; set; }
}
