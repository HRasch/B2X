using B2X.Catalog.Core.Entities;
using B2X.Catalog.Models;
using B2X.Types.DTOs;

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

/// <summary>
/// Unified repository interface for all catalog operations
/// Consolidates Product, Category, and Variant data access
/// </summary>
public interface ICatalogRepository
{
    // Product operations
    Task<Product?> GetProductByIdAsync(Guid id, Guid tenantId);
    Task<IEnumerable<Product>> GetProductsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 50);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId, Guid tenantId, int page = 1, int pageSize = 50);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id, Guid tenantId);

    // Category operations
    Task<Category?> GetCategoryByIdAsync(Guid id, Guid tenantId);
    Task<Category?> GetCategoryBySlugAsync(string slug, Guid tenantId);
    Task<PagedResult<Category>> GetCategoriesPagedAsync(
        Guid tenantId,
        string? searchTerm = null,
        Guid? parentId = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 50,
        string? sortBy = "DisplayOrder",
        bool sortDescending = false);
    Task<List<Category>> GetCategoryHierarchyAsync(Guid tenantId, Guid? rootCategoryId = null);
    Task<List<Category>> GetCategoryChildrenAsync(Guid parentId, Guid tenantId);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid id, Guid tenantId);
    Task<bool> CategoryExistsAsync(Guid id, Guid tenantId);

    // Variant operations
    Task<Variant?> GetVariantByIdAsync(Guid id, Guid tenantId);
    Task<Variant?> GetVariantBySkuAsync(string sku, Guid tenantId);
    Task<IEnumerable<Variant>> GetVariantsByProductIdAsync(Guid productId);
    Task<(IEnumerable<Variant> Items, int TotalCount)> GetVariantsByProductIdPagedAsync(
        Guid productId,
        Guid tenantId,
        int pageNumber,
        int pageSize);
    Task<(IEnumerable<Variant> Items, int TotalCount)> GetVariantsPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize);
    Task<(IEnumerable<Variant> Items, int TotalCount)> SearchVariantsAsync(
        Guid tenantId,
        string query,
        int pageNumber,
        int pageSize);
    Task AddVariantAsync(Variant variant);
    Task UpdateVariantAsync(Variant variant);
    Task DeleteVariantAsync(Guid id, Guid tenantId);

    // Cross-entity operations
    Task<IEnumerable<Product>> GetProductsByCategoryHierarchyAsync(Guid categoryId, Guid tenantId, int page = 1, int pageSize = 50);
    Task<bool> IsProductInCategoryAsync(Guid productId, Guid categoryId, Guid tenantId);
}
