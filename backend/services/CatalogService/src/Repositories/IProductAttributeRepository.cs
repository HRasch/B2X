using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Repository interface for ProductAttribute operations
/// </summary>
public interface IProductAttributeRepository : IRepository<ProductAttribute>
{
    /// <summary>Gets attribute by code</summary>
    Task<ProductAttribute?> GetByCodeAsync(string code);

    /// <summary>Gets all active attributes</summary>
    Task<IEnumerable<ProductAttribute>> GetActiveAsync();

    /// <summary>Gets attribute with all options</summary>
    Task<ProductAttribute?> GetWithOptionsAsync(Guid id);

    /// <summary>Gets searchable attributes</summary>
    Task<IEnumerable<ProductAttribute>> GetSearchableAsync();

    /// <summary>Gets filterable attributes</summary>
    Task<IEnumerable<ProductAttribute>> GetFilterableAsync();
}
