using B2X.Domain.Search.Models;

namespace B2X.Domain.Search.Services;

public interface IElasticService
{
    // Product operations
    Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request, string? tenantId = null, string? language = null);
    Task<ProductDocument?> GetProductByIdAsync(string id, string? tenantId = null, string? language = null);
    Task EnsureProductIndexAsync(string? tenantId = null, string? language = null);
    Task IndexProductsAsync(IEnumerable<ProductDocument> documents, string? tenantId = null, string? language = null);
    Task<bool> IsProductSeededAsync(string tenantId, string? language = null);
    Task MarkProductSeededAsync(string tenantId, int count, string? language = null);

    // Page operations
    Task<PageSearchResponseDto> SearchPagesAsync(PageSearchRequestDto request, string? tenantId = null, string? language = null);
    Task<PageDocument?> GetPageByIdAsync(string id, string? tenantId = null, string? language = null);
    Task EnsurePageIndexAsync(string? tenantId = null, string? language = null);
    Task IndexPagesAsync(IEnumerable<PageDocument> documents, string? tenantId = null, string? language = null);
    Task<bool> IsPageSeededAsync(string tenantId, string? language = null);
    Task MarkPageSeededAsync(string tenantId, int count, string? language = null);

    // Legacy methods for backward compatibility
    Task<SearchResponseDto> SearchAsync(SearchRequestDto request, string? tenantId = null, string? language = null) => SearchProductsAsync(request, tenantId, language);
    Task<ProductDocument?> GetByIdAsync(string id, string? tenantId = null, string? language = null) => GetProductByIdAsync(id, tenantId, language);
    Task EnsureIndexAsync(string? tenantId = null, string? language = null) => EnsureProductIndexAsync(tenantId, language);
    Task IndexManyAsync(IEnumerable<ProductDocument> documents, string? tenantId = null, string? language = null) => IndexProductsAsync(documents, tenantId, language);
    Task<bool> IsSeededAsync(string tenantId, string? language = null) => IsProductSeededAsync(tenantId, language);
    Task MarkSeededAsync(string tenantId, int count, string? language = null) => MarkProductSeededAsync(tenantId, count, language);
}
