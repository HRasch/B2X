using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// Service interface for product operations
/// </summary>
public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> GetPagedAsync(Guid tenantId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(Guid tenantId, CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(Guid tenantId, Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

/// <summary>
/// In-memory implementation of IProductService for MVP
/// </summary>
public class ProductService : IProductService
{
    private static readonly Dictionary<Guid, List<Product>> _products = new();
    private readonly ILogger<ProductService> _logger;
    private readonly ISearchIndexService _searchIndex;

    public ProductService(ILogger<ProductService> logger, ISearchIndexService searchIndex)
    {
        _logger = logger;
        _searchIndex = searchIndex;
    }

    public Task<ProductDto?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        if (!_products.TryGetValue(tenantId, out var tenantProducts))
        {
            return Task.FromResult<ProductDto?>(null);
        }

        var product = tenantProducts.FirstOrDefault(p => p.Id == productId);
        return Task.FromResult(product?.ToDto());
    }

    public Task<PagedResult<ProductDto>> GetPagedAsync(Guid tenantId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (!_products.TryGetValue(tenantId, out var tenantProducts))
        {
            return Task.FromResult(new PagedResult<ProductDto>
            {
                Items = new(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0
            });
        }

        var skip = (pageNumber - 1) * pageSize;
        var items = tenantProducts
            .Skip(skip)
            .Take(pageSize)
            .Select(p => p.ToDto())
            .ToList();

        return Task.FromResult(new PagedResult<ProductDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = tenantProducts.Count
        });
    }

    public async Task<ProductDto> CreateAsync(Guid tenantId, CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            StockQuantity = request.StockQuantity,
            Categories = request.Categories ?? new(),
            BrandName = request.BrandName,
            Tags = request.Tags ?? new(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (!_products.TryGetValue(tenantId, out var value))
        {
            value = new();
            _products[tenantId] = value;
        }

        value.Add(product);

        _logger.LogInformation("Product created: {ProductId} for tenant {TenantId}", product.Id, tenantId);

        // Index in Elasticsearch
        await _searchIndex.IndexProductAsync(product, cancellationToken);

        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid tenantId, Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (!_products.TryGetValue(tenantId, out var tenantProducts))
        {
            throw new KeyNotFoundException($"Product {productId} not found");
        }

        var product = tenantProducts.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product {productId} not found");
        }

        // Update fields
        if (!string.IsNullOrEmpty(request.Name))
        {
            product.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            product.Description = request.Description;
        }

        if (request.Price.HasValue)
        {
            product.Price = request.Price.Value;
        }

        if (request.DiscountPrice.HasValue)
        {
            product.DiscountPrice = request.DiscountPrice;
        }

        if (request.StockQuantity.HasValue)
        {
            product.StockQuantity = request.StockQuantity.Value;
        }

        if (request.IsActive.HasValue)
        {
            product.IsActive = request.IsActive.Value;
        }

        if (request.Categories != null)
        {
            product.Categories = request.Categories;
        }

        if (!string.IsNullOrEmpty(request.BrandName))
        {
            product.BrandName = request.BrandName;
        }

        if (request.Tags != null)
        {
            product.Tags = request.Tags;
        }

        product.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Product updated: {ProductId} for tenant {TenantId}", product.Id, tenantId);

        // Update in Elasticsearch
        await _searchIndex.IndexProductAsync(product, cancellationToken);

        return product.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        if (!_products.TryGetValue(tenantId, out var tenantProducts))
        {
            return false;
        }

        var product = tenantProducts.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            return false;
        }

        tenantProducts.Remove(product);

        _logger.LogInformation("Product deleted: {ProductId} for tenant {TenantId}", product.Id, tenantId);

        // Delete from Elasticsearch
        await _searchIndex.DeleteProductAsync(product.Id, cancellationToken);

        return true;
    }

    public Task<PagedResult<ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (!_products.TryGetValue(tenantId, out var tenantProducts))
        {
            return Task.FromResult(new PagedResult<ProductDto>
            {
                Items = new(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0
            });
        }

        var filtered = tenantProducts
            .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                       p.Sku.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var skip = (pageNumber - 1) * pageSize;
        var items = filtered
            .Skip(skip)
            .Take(pageSize)
            .Select(p => p.ToDto())
            .ToList();

        return Task.FromResult(new PagedResult<ProductDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = filtered.Count
        });
    }
}

/// <summary>
/// Extension methods for Product
/// </summary>
internal static class ProductExtensions
{
    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id,
        TenantId = product.TenantId,
        Sku = product.Sku,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        DiscountPrice = product.DiscountPrice,
        StockQuantity = product.StockQuantity,
        IsActive = product.IsActive,
        Categories = product.Categories,
        BrandName = product.BrandName,
        Tags = product.Tags,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        IsAvailable = product.IsAvailable
    };
}
