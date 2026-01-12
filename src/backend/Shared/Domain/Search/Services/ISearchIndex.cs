namespace B2X.Search;

public interface ISearchIndex
{
    Task IndexAsync(object document);
    Task DeleteAsync(string id);
    Task UpdateAsync(object document);
}
