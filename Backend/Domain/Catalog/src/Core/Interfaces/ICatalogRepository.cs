using B2X.Catalog.Core.Entities;

namespace B2X.Catalog.Core.Interfaces;

/// <summary>
/// Repository interface for catalog import operations
/// </summary>
public interface ICatalogImportRepository
{
    Task<CatalogImport?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CatalogImport?> GetByCompositeKeyAsync(Guid tenantId, string supplierId, string catalogId, DateTime importTimestamp, CancellationToken ct = default);
    Task<IEnumerable<CatalogImport>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<CatalogImport>> GetByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(CatalogImport catalogImport, CancellationToken ct = default);
    Task UpdateAsync(CatalogImport catalogImport, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// Repository interface for catalog products
/// </summary>
public interface ICatalogProductRepository
{
    Task<CatalogProduct?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CatalogProduct>> GetByCatalogImportIdAsync(Guid catalogImportId, CancellationToken ct = default);
    Task<IEnumerable<CatalogProduct>> GetByImportIdAsync(Guid importId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByImportIdAsync(Guid importId, CancellationToken ct = default);
    Task<CatalogProduct?> GetByImportAndAidAsync(Guid importId, string supplierAid, CancellationToken ct = default);
    Task AddAsync(CatalogProduct product, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<CatalogProduct> products, CancellationToken ct = default);
    Task UpdateAsync(CatalogProduct product, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task DeleteByCatalogImportIdAsync(Guid catalogImportId, CancellationToken ct = default);
    Task DeleteByImportIdAsync(Guid importId, CancellationToken ct = default);
}
