namespace B2X.Search;

public interface ISearchService
{
    Task<SearchResult<object>> SearchAsync(SearchRequest request);
}
