using B2Connect.Search.Models;

namespace B2Connect.Search.Services;

public interface IElasticService
{
    Task<SearchResponseDto> SearchAsync(SearchRequestDto request);
    Task<ProductDocument?> GetByIdAsync(string id);
    Task EnsureIndexAsync();
}
