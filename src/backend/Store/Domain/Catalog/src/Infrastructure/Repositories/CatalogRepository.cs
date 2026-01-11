using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Infrastructure.Data;
using B2X.Catalog.Models;
using B2X.Types.DTOs;
using Microsoft.EntityFrameworkCore;

namespace B2X.Catalog.Infrastructure.Repositories;

/// <summary>
/// Unified catalog repository implementation
/// Consolidates data access for Products, Categories, and Variants
/// </summary>
public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _context;

    public CatalogRepository(CatalogDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Product operations
    public async Task<Product?> GetProductByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
    }

    public async Task<IEnumerable<Product>> GetProductsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 50)
    {
        return await _context.Products
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId, Guid tenantId, int page = 1, int pageSize = 50)
    {
        return await _context.ProductCategories
            .Where(pc => pc.CategoryId == categoryId)
            .Join(_context.Products,
                pc => pc.ProductId,
                p => p.Id,
                (pc, p) => p)
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid id, Guid tenantId)
    {
        var product = await GetProductByIdAsync(id, tenantId);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    // Category operations
    public async Task<Category?> GetCategoryByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
    }

    public async Task<Category?> GetCategoryBySlugAsync(string slug, Guid tenantId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug && c.TenantId == tenantId);
    }

    public async Task<PagedResult<Category>> GetCategoriesPagedAsync(
        Guid tenantId,
        string? searchTerm = null,
        Guid? parentId = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 50,
        string? sortBy = "DisplayOrder",
        bool sortDescending = false)
    {
        var query = _context.Categories.Where(c => c.TenantId == tenantId);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm));
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

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Category>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<List<Category>> GetCategoryHierarchyAsync(Guid tenantId, Guid? rootCategoryId = null)
    {
        var query = _context.Categories.Where(c => c.TenantId == tenantId);

        if (rootCategoryId.HasValue)
        {
            query = query.Where(c => c.Id == rootCategoryId.Value || c.ParentId == rootCategoryId.Value);
        }

        // Build hierarchy (simplified - in real implementation, use recursive loading)
        return await query.ToListAsync();
    }

    public async Task<List<Category>> GetCategoryChildrenAsync(Guid parentId, Guid tenantId)
    {
        return await _context.Categories
            .Where(c => c.TenantId == tenantId && c.ParentId == parentId)
            .ToListAsync();
    }

    public async Task AddCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid id, Guid tenantId)
    {
        var category = await GetCategoryByIdAsync(id, tenantId);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CategoryExistsAsync(Guid id, Guid tenantId)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id && c.TenantId == tenantId);
    }

    // Variant operations
    public async Task<Variant?> GetVariantByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Variants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == id && v.Product.TenantId == tenantId);
    }

    public async Task<Variant?> GetVariantBySkuAsync(string sku, Guid tenantId)
    {
        return await _context.Variants
            .Join(_context.Products,
                v => v.ProductId,
                p => p.Id,
                (v, p) => new { Variant = v, Product = p })
            .Where(x => x.Product.TenantId == tenantId && x.Variant.Sku == sku)
            .Select(x => x.Variant)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Variant>> GetVariantsByProductIdAsync(Guid productId)
    {
        return await _context.Variants
            .Where(v => v.ProductId == productId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Variant> Items, int TotalCount)> GetVariantsByProductIdPagedAsync(
        Guid productId,
        Guid tenantId,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Variants
            .Include(v => v.Product)
            .Where(v => v.ProductId == productId && v.Product.TenantId == tenantId);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Variant> Items, int TotalCount)> GetVariantsPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Variants
            .Join(_context.Products,
                v => v.ProductId,
                p => p.Id,
                (v, p) => new { Variant = v, Product = p })
            .Where(x => x.Product.TenantId == tenantId)
            .Select(x => x.Variant);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Variant> Items, int TotalCount)> SearchVariantsAsync(
        Guid tenantId,
        string query,
        int pageNumber,
        int pageSize)
    {
        var variantsQuery = _context.Variants
            .Join(_context.Products,
                v => v.ProductId,
                p => p.Id,
                (v, p) => new { Variant = v, Product = p })
            .Where(x => x.Product.TenantId == tenantId)
            .Where(x => x.Variant.Name.Contains(query) || x.Variant.Sku.Contains(query))
            .Select(x => x.Variant);

        var totalCount = await variantsQuery.CountAsync();
        var items = await variantsQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddVariantAsync(Variant variant)
    {
        await _context.Variants.AddAsync(variant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVariantAsync(Variant variant)
    {
        _context.Variants.Update(variant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVariantAsync(Guid id, Guid tenantId)
    {
        var variant = await GetVariantByIdAsync(id, tenantId);
        if (variant != null)
        {
            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();
        }
    }

    // Cross-entity operations
    public async Task<IEnumerable<Product>> GetProductsByCategoryHierarchyAsync(Guid categoryId, Guid tenantId, int page = 1, int pageSize = 50)
    {
        // Get all descendant category IDs using recursive CTE
        var categoryIds = await GetDescendantCategoryIdsAsync(categoryId, tenantId);

        return await _context.ProductCategories
            .Where(pc => categoryIds.Contains(pc.CategoryId))
            .Join(_context.Products,
                pc => pc.ProductId,
                p => p.Id,
                (pc, p) => p)
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> IsProductInCategoryAsync(Guid productId, Guid categoryId, Guid tenantId)
    {
        return await _context.ProductCategories
            .AnyAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId) &&
               await _context.Products
                   .AnyAsync(p => p.Id == productId && p.TenantId == tenantId);
    }

    private async Task<HashSet<Guid>> GetDescendantCategoryIdsAsync(Guid categoryId, Guid tenantId)
    {
        // Use recursive CTE to get all descendant categories
        var categoryIds = new HashSet<Guid> { categoryId };

        // This is a simplified implementation. In a real scenario, you'd use a recursive CTE
        // or store the hierarchy in a way that allows efficient querying
        var descendants = await _context.Categories
            .FromSqlRaw(@"
                WITH RECURSIVE CategoryHierarchy AS (
                    SELECT Id FROM Categories WHERE Id = {0} AND TenantId = {1}
                    UNION ALL
                    SELECT c.Id FROM Categories c
                    INNER JOIN CategoryHierarchy ch ON c.ParentId = ch.Id
                    WHERE c.TenantId = {1}
                )
                SELECT Id FROM CategoryHierarchy", categoryId, tenantId)
            .Select(c => c.Id)
            .ToListAsync();

        categoryIds.UnionWith(descendants);
        return categoryIds;
    }
}
