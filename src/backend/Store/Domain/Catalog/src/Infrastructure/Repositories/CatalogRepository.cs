using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Models;
using B2X.Types.DTOs;

namespace B2X.Catalog.Infrastructure.Repositories;

/// <summary>
/// Unified catalog repository implementation
/// Consolidates data access for Products, Categories, and Variants
/// </summary>
public class CatalogRepository : ICatalogRepository
{
    // In-memory storage for demonstration - replace with actual database context
    private readonly Dictionary<Guid, Product> _products = new();
    private readonly Dictionary<Guid, Category> _categories = new();
    private readonly Dictionary<Guid, Variant> _variants = new();

    // Product operations
    public Task<Product?> GetProductByIdAsync(Guid id, Guid tenantId)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product?.TenantId == tenantId ? product : null);
    }

    public Task<IEnumerable<Product>> GetProductsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 50)
    {
        var products = _products.Values
            .Where(p => p.TenantId == tenantId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return Task.FromResult(products);
    }

    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId, Guid tenantId, int page = 1, int pageSize = 50)
    {
        var products = _products.Values
            .Where(p => p.TenantId == tenantId && p.CategoryIds.Contains(categoryId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return Task.FromResult(products);
    }

    public Task AddProductAsync(Product product)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task UpdateProductAsync(Product product)
    {
        // For in-memory repository, update checks existence but performs upsert
        // This differentiates from AddProductAsync which doesn't check existence
        if (!_products.ContainsKey(product.Id))
        {
            // Could throw an exception here, but for simplicity we allow upsert
        }
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task DeleteProductAsync(Guid id, Guid tenantId)
    {
        if (_products.TryGetValue(id, out var product) && product.TenantId == tenantId)
        {
            _products.Remove(id);
        }
        return Task.CompletedTask;
    }

    // Category operations
    public Task<Category?> GetCategoryByIdAsync(Guid id, Guid tenantId)
    {
        _categories.TryGetValue(id, out var category);
        return Task.FromResult(category?.TenantId == tenantId ? category : null);
    }

    public Task<Category?> GetCategoryBySlugAsync(string slug, Guid tenantId)
    {
        var category = _categories.Values.FirstOrDefault(c => c.Slug == slug && c.TenantId == tenantId);
        return Task.FromResult(category);
    }

    public Task<PagedResult<Category>> GetCategoriesPagedAsync(
        Guid tenantId,
        string? searchTerm = null,
        Guid? parentId = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 50,
        string? sortBy = "DisplayOrder",
        bool sortDescending = false)
    {
        var query = _categories.Values.Where(c => c.TenantId == tenantId);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (parentId.HasValue)
        {
            query = query.Where(c => c.ParentId == parentId);
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        // Sorting
        query = sortBy switch
        {
            "Name" => sortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "DisplayOrder" => sortDescending ? query.OrderByDescending(c => c.DisplayOrder) : query.OrderBy(c => c.DisplayOrder),
            _ => sortDescending ? query.OrderByDescending(c => c.DisplayOrder) : query.OrderBy(c => c.DisplayOrder)
        };

        var queryList = query.ToList();
        var totalCount = queryList.Count;
        var items = queryList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Category>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public Task<List<Category>> GetCategoryHierarchyAsync(Guid tenantId, Guid? rootCategoryId = null)
    {
        var categories = _categories.Values.Where(c => c.TenantId == tenantId).ToList();

        if (rootCategoryId.HasValue)
        {
            categories = categories.Where(c => c.Id == rootCategoryId.Value || c.ParentId == rootCategoryId.Value).ToList();
        }

        // Build hierarchy (simplified - in real implementation, use recursive loading)
        return Task.FromResult(categories);
    }

    public Task<List<Category>> GetCategoryChildrenAsync(Guid parentId, Guid tenantId)
    {
        var children = _categories.Values
            .Where(c => c.TenantId == tenantId && c.ParentId == parentId)
            .ToList();
        return Task.FromResult(children);
    }

    public Task AddCategoryAsync(Category category)
    {
        _categories[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task UpdateCategoryAsync(Category category)
    {
        // For in-memory repository, update checks existence but performs upsert
        // This differentiates from AddCategoryAsync which doesn't check existence
        if (!_categories.ContainsKey(category.Id))
        {
            // Could throw an exception here, but for simplicity we allow upsert
        }
        _categories[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task DeleteCategoryAsync(Guid id, Guid tenantId)
    {
        if (_categories.TryGetValue(id, out var category) && category.TenantId == tenantId)
        {
            _categories.Remove(id);
        }
        return Task.CompletedTask;
    }

    public Task<bool> CategoryExistsAsync(Guid id, Guid tenantId)
    {
        return Task.FromResult(_categories.TryGetValue(id, out var category) && category.TenantId == tenantId);
    }

    // Variant operations
    public Task<Variant?> GetVariantByIdAsync(Guid id)
    {
        _variants.TryGetValue(id, out var variant);
        return Task.FromResult(variant);
    }

    public Task<Variant?> GetVariantBySkuAsync(string sku, Guid tenantId)
    {
        var variant = _variants.Values.FirstOrDefault(v =>
            v.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase) &&
            _products.TryGetValue(v.ProductId, out var p) && p.TenantId == tenantId);
        return Task.FromResult(variant);
    }

    public Task<IEnumerable<Variant>> GetVariantsByProductIdAsync(Guid productId)
    {
        var variants = _variants.Values.Where(v => v.ProductId == productId);
        return Task.FromResult(variants);
    }

    public Task<(IEnumerable<Variant> Items, int TotalCount)> GetVariantsByProductIdPagedAsync(
        Guid productId,
        int pageNumber,
        int pageSize)
    {
        var variants = _variants.Values.Where(v => v.ProductId == productId);
        var totalCount = variants.Count();
        var items = variants
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult((items, totalCount));
    }

    public Task<(IEnumerable<Variant> Items, int TotalCount)> GetVariantsPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize)
    {
        // Note: Variants don't have tenantId directly, but we can get it from associated products
        var variants = _variants.Values
            .Where(v => _products.TryGetValue(v.ProductId, out var p) && p.TenantId == tenantId);

        var totalCount = variants.Count();
        var items = variants
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult((items, totalCount));
    }

    public Task<(IEnumerable<Variant> Items, int TotalCount)> SearchVariantsAsync(
        Guid tenantId,
        string query,
        int pageNumber,
        int pageSize)
    {
        var variants = _variants.Values
            .Where(v => _products.TryGetValue(v.ProductId, out var p) && p.TenantId == tenantId)
            .Where(v => v.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                       v.Sku.Contains(query, StringComparison.OrdinalIgnoreCase));

        var totalCount = variants.Count();
        var items = variants
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult((items, totalCount));
    }

    public Task AddVariantAsync(Variant variant)
    {
        _variants[variant.Id] = variant;
        return Task.CompletedTask;
    }

    public Task UpdateVariantAsync(Variant variant)
    {
        // For in-memory repository, update checks existence but performs upsert
        // This differentiates from AddVariantAsync which doesn't check existence
        if (!_variants.ContainsKey(variant.Id))
        {
            // Could throw an exception here, but for simplicity we allow upsert
        }
        _variants[variant.Id] = variant;
        return Task.CompletedTask;
    }

    public Task DeleteVariantAsync(Guid id)
    {
        _variants.Remove(id);
        return Task.CompletedTask;
    }

    // Cross-entity operations
    public Task<IEnumerable<Product>> GetProductsByCategoryHierarchyAsync(Guid categoryId, Guid tenantId, int page = 1, int pageSize = 50)
    {
        // Get all descendant category IDs (simplified - in real implementation, use recursive query)
        var categoryIds = new HashSet<Guid> { categoryId };
        var queue = new Queue<Guid>();
        queue.Enqueue(categoryId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = _categories.Values
                .Where(c => c.ParentId == currentId && c.TenantId == tenantId)
                .Select(c => c.Id);

            foreach (var childId in children)
            {
                if (categoryIds.Add(childId))
                {
                    queue.Enqueue(childId);
                }
            }
        }

        var products = _products.Values
            .Where(p => p.TenantId == tenantId && p.CategoryIds.Any(id => categoryIds.Contains(id)))
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult(products);
    }

    public Task<bool> IsProductInCategoryAsync(Guid productId, Guid categoryId, Guid tenantId)
    {
        return Task.FromResult(
            _products.TryGetValue(productId, out var product) &&
            product.TenantId == tenantId &&
            product.CategoryIds.Contains(categoryId));
    }
}
