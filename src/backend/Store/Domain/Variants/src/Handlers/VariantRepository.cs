using B2X.Variants.Models;

namespace B2X.Variants.Handlers;

/// <summary>
/// In-memory repository implementation for variants
/// TODO: Replace with actual database implementation
/// </summary>
public class VariantRepository : IVariantRepository
{
    private readonly Dictionary<Guid, List<Variant>> _variantsByTenant = new();

    public Task<Variant?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.FromResult<Variant?>(null);
        }

        return Task.FromResult(variants.FirstOrDefault(v => v.Id == id));
    }

    public Task<IEnumerable<Variant>> GetByProductIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.FromResult(Enumerable.Empty<Variant>());
        }

        var result = variants.Where(v => v.ProductId == productId).ToList();
        return Task.FromResult<IEnumerable<Variant>>(result);
    }

    public Task<(IEnumerable<Variant> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.FromResult((Enumerable.Empty<Variant>(), 0));
        }

        var query = variants.AsQueryable();
        var totalCount = query.Count();

        var items = query
            .OrderBy(v => v.DisplayOrder)
            .ThenBy(v => v.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((items.AsEnumerable(), totalCount));
    }

    public Task<(IEnumerable<Variant> Items, int TotalCount)> SearchAsync(
        Guid tenantId,
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.FromResult((Enumerable.Empty<Variant>(), 0));
        }

        var searchQuery = query.ToLowerInvariant();
        var filteredVariants = variants.Where(v =>
            v.Name.ToLowerInvariant().Contains(searchQuery) ||
            v.Sku.ToLowerInvariant().Contains(searchQuery) ||
            (v.Description?.ToLowerInvariant().Contains(searchQuery) ?? false) ||
            (v.Barcode?.ToLowerInvariant().Contains(searchQuery) ?? false));

        var totalCount = filteredVariants.Count();

        var items = filteredVariants
            .OrderBy(v => v.DisplayOrder)
            .ThenBy(v => v.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((items.AsEnumerable(), totalCount));
    }

    public Task AddAsync(Variant variant, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.ContainsKey(variant.TenantId))
        {
            _variantsByTenant[variant.TenantId] = new List<Variant>();
        }

        _variantsByTenant[variant.TenantId].Add(variant);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Variant variant, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(variant.TenantId, out var variants))
        {
            return Task.CompletedTask;
        }

        var existingIndex = variants.FindIndex(v => v.Id == variant.Id);
        if (existingIndex >= 0)
        {
            variants[existingIndex] = variant;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.CompletedTask;
        }

        variants.RemoveAll(v => v.Id == id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(variants.Any(v => v.Id == id));
    }

    public Task<bool> IsSkuUniqueAsync(Guid tenantId, string sku, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (!_variantsByTenant.TryGetValue(tenantId, out var variants))
        {
            return Task.FromResult(true);
        }

        var existingVariant = variants.FirstOrDefault(v =>
            v.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase) &&
            (!excludeId.HasValue || v.Id != excludeId.Value));

        return Task.FromResult(existingVariant == null);
    }
}
