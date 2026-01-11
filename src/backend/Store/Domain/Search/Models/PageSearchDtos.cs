namespace B2X.Domain.Search.Models;

public class PageSearchRequestDto
{
    public string? Query { get; set; }
    public string? PageType { get; set; }
    public bool? IsPublished { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PageSearchResponseDto
{
    public IEnumerable<PageDocument> Pages { get; set; } = new List<PageDocument>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
