using B2Connect.Domain.Search.Models;

namespace B2Connect.Domain.Search.Services;

public interface IElasticService
{
    Task<SearchResponseDto> SearchAsync(SearchRequestDto request);
    Task<ProductDocument?> GetByIdAsync(string id);
    Task EnsureIndexAsync();
}
