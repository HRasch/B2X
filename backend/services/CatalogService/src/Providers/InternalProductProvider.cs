using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using Microsoft.Extensions.Logging;

namespace B2Connect.CatalogService.Providers;

/// <summary>
/// Internal Product Provider
/// Uses the internal CatalogService read model (CatalogReadDbContext)
/// Source of truth for products managed within B2Connect
/// </summary>
public class InternalProductProvider : IProductProvider
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<InternalProductProvider> _logger;

    public string ProviderName => "internal";

    public InternalProductProvider(
        CatalogReadDbContext readDb,
        ILogger<InternalProductProvider> logger)
    {
        _readDb = readDb ?? throw new ArgumentNullException(nameof(readDb));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> IsEnabledAsync(CancellationToken cancellationToken)
    {
        // Internal provider is always enabled (it's the core system)
        return await Task.FromResult(true);
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _readDb.ProductsReadModel
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == productId &&
                         p.TenantId == tenantId &&
                         !p.IsDeleted,
                    cancellationToken);

            if (product == null)
                return null;

            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId} from internal provider", productId);
            throw;
        }
    }

    public async Task<PagedResult<ProductDto>> GetProductsPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _readDb.ProductsReadModel
                .AsNoTracking()
                .Where(p => p.TenantId == tenantId && !p.IsDeleted);

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(p =>
                    p.SearchText.Contains(searchLower) ||
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Sku.ToLower().Contains(searchLower));
            }

            // Apply category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // Apply price filters
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            // Apply pagination
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductDto>
            {
                Items = products.Select(MapToDto).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (totalCount + pageSize - 1) / pageSize,
                HasNextPage = page < ((totalCount + pageSize - 1) / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged products from internal provider");
            throw;
        }
    }

    public async Task<PagedResult<ProductDto>> SearchProductsAsync(
        Guid tenantId,
        string searchTerm,
        int page,
        int pageSize,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        // Internal provider delegates to paged query with search term
        return await GetProductsPagedAsync(
            tenantId, page, pageSize,
            searchTerm, category, minPrice, maxPrice, "relevance",
            cancellationToken);
    }

    public async Task<int> GetProductCountAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return await _readDb.ProductsReadModel
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Test database connection
            var count = await _readDb.ProductsReadModel
                .AsNoTracking()
                .CountAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal provider connectivity check failed");
            return false;
        }
    }

    public async Task<ProviderMetadata> GetMetadataAsync(CancellationToken cancellationToken)
    {
        return await Task.FromResult(new ProviderMetadata
        {
            Name = "Internal CatalogService",
            Version = "1.0.0",
            IsConnected = true,
            LastSyncTime = DateTime.UtcNow,
            Capabilities = new ProviderCapabilities
            {
                SupportsFullTextSearch = true,
                SupportsPriceFiltering = true,
                SupportsCategoryFiltering = true,
                SupportsSorting = true,
                SupportsPagination = true,
                SupportsMultiLanguage = true,
                SupportsSync = true
            }
        });
    }

    private static ProductDto MapToDto(Data.ReadModel.ProductReadModel product) =>
        new()
        {
            Id = product.Id,
            TenantId = product.TenantId,
            Sku = product.Sku,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Category = product.Category,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
}
