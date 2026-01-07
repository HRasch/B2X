using B2Connect.Domain.Search.Models;

namespace B2Connect.Domain.Search.Services;

public interface IElasticService
{
    Task<SearchResponseDto> SearchAsync(SearchRequestDto request, string? tenantId = null, string? language = null);
    Task<ProductDocument?> GetByIdAsync(string id, string? tenantId = null, string? language = null);
    Task EnsureIndexAsync(string? tenantId = null, string? language = null);
    Task IndexManyAsync(IEnumerable<ProductDocument> documents, string? tenantId = null, string? language = null);
    Task<bool> IsSeededAsync(string tenantId, string? language = null);
    Task MarkSeededAsync(string tenantId, int count, string? language = null);
}
