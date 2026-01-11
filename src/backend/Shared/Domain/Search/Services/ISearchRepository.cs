namespace B2X.Search;

public interface ISearchRepository
{
    Task<SearchResult<T>> SearchAsync<T>(SearchQuery query);
    Task<T?> GetByIdAsync<T>(string id);
}
