using B2X.Search.Models;

namespace B2X.Search.Services;

public interface IElasticService
{
    Task<SearchResponseDto> SearchAsync(SearchRequestDto request);
    Task<ProductDocument?> GetByIdAsync(string id);
    Task EnsureIndexAsync();
}
