using B2X.Variants.Models;

namespace B2X.Variants.Handlers;

/// <summary>
/// Repository interface for variant data access
/// </summary>
public interface IVariantRepository
{
    /// <summary>
    /// Get variant by ID
    /// </summary>
    Task<Variant?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get variants by product ID
    /// </summary>
    Task<IEnumerable<Variant>> GetByProductIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged variants
    /// </summary>
    Task<(IEnumerable<Variant> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search variants
    /// </summary>
    Task<(IEnumerable<Variant> Items, int TotalCount)> SearchAsync(
        Guid tenantId,
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new variant
    /// </summary>
    Task AddAsync(Variant variant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing variant
    /// </summary>
    Task UpdateAsync(Variant variant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a variant
    /// </summary>
    Task DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if variant exists
    /// </summary>
    Task<bool> ExistsAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if SKU is unique for tenant
    /// </summary>
    Task<bool> IsSkuUniqueAsync(Guid tenantId, string sku, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
