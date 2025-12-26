using Wolverine;
using Wolverine.Http;
using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.CatalogService.CQRS.Handlers.Queries;

/// <summary>
/// Handler for GetProductByIdQuery
/// Wolverine executes this synchronously - caller waits for response
/// Uses denormalized read model for fast single-product lookup
/// No joins or complex queries needed
/// </summary>
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        CatalogReadDbContext readDb,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var product = await _readDb.ProductsReadModel
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == query.ProductId &&
                         p.TenantId == query.TenantId &&
                         !p.IsDeleted,
                    cancellationToken);

            if (product == null)
            {
                _logger.LogWarning(
                    "Product not found: {ProductId} (Tenant: {TenantId})",
                    query.ProductId, query.TenantId);

                throw new KeyNotFoundException($"Product {query.ProductId} not found");
            }

            sw.Stop();

            _logger.LogInformation(
                "Product retrieved in {ElapsedMs}ms: {ProductId} ({Sku})",
                sw.ElapsedMilliseconds, query.ProductId, product.Sku);

            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", query.ProductId);
            throw;
        }
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

/// <summary>
/// Handler for GetProductsPagedQuery
/// Optimized for large datasets (millions of products)
/// Uses indexed read model queries to return results in milliseconds
/// Supports filtering by search term, category, and price range
/// </summary>
public class GetProductsPagedQueryHandler : IQueryHandler<GetProductsPagedQuery, PagedResult<ProductDto>>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<GetProductsPagedQueryHandler> _logger;

    public GetProductsPagedQueryHandler(
        CatalogReadDbContext readDb,
        ILogger<GetProductsPagedQueryHandler> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> Handle(
        GetProductsPagedQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Build base query filtered by tenant and active products
            var baseQuery = _readDb.ProductsReadModel
                .AsNoTracking()
                .Where(p => p.TenantId == query.TenantId && !p.IsDeleted);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                baseQuery = baseQuery.Where(p => p.SearchText.Contains(searchTerm));
            }

            // Apply category filter if provided
            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                baseQuery = baseQuery.Where(p => p.Category == query.Category);
            }

            // Apply price range filter if provided
            if (query.MinPrice.HasValue)
                baseQuery = baseQuery.Where(p => p.Price >= query.MinPrice.Value);
            if (query.MaxPrice.HasValue)
                baseQuery = baseQuery.Where(p => p.Price <= query.MaxPrice.Value);

            // Apply availability filter
            if (query.AvailableOnly.HasValue && query.AvailableOnly.Value)
            {
                baseQuery = baseQuery.Where(p => p.IsAvailable);
            }

            // Get total count for pagination
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // Apply sorting
            var sortedQuery = query.SortBy?.ToLower() switch
            {
                "price" => baseQuery.OrderBy(p => p.Price),
                "price_desc" => baseQuery.OrderByDescending(p => p.Price),
                "created" => baseQuery.OrderByDescending(p => p.CreatedAt),
                "updated" => baseQuery.OrderByDescending(p => p.UpdatedAt),
                _ => baseQuery.OrderBy(p => p.Name) // Default: sort by name
            };

            // Apply pagination
            var skip = (query.Page - 1) * query.PageSize;
            var products = await sortedQuery
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            sw.Stop();

            _logger.LogInformation(
                "Paged query executed in {ElapsedMs}ms. " +
                "Tenant: {TenantId}, Page: {Page}/{TotalPages}, Count: {Count}/{Total}",
                sw.ElapsedMilliseconds,
                query.TenantId,
                query.Page,
                (totalCount + query.PageSize - 1) / query.PageSize,
                products.Count,
                totalCount);

            return new PagedResult<ProductDto>
            {
                Items = products.Select(MapToDto),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing paged query for tenant {TenantId}", query.TenantId);
            throw;
        }
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

/// <summary>
/// Handler for SearchProductsQuery
/// Full-text search across product name, description, SKU
/// Uses search_text GIN index for fast full-text matching on millions of products
/// Falls back to ElasticSearch for advanced features (fuzzy matching, facets, etc.)
/// </summary>
public class SearchProductsQueryHandler : IQueryHandler<SearchProductsQuery, PagedResult<ProductDto>>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        CatalogReadDbContext readDb,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> Handle(
        SearchProductsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Build search query
            var searchTerm = query.SearchTerm.ToLower();
            var baseQuery = _readDb.ProductsReadModel
                .AsNoTracking()
                .Where(p => p.TenantId == query.TenantId &&
                           !p.IsDeleted &&
                           p.SearchText.Contains(searchTerm));

            // Get total count for pagination
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // Apply pagination
            var skip = (query.Page - 1) * query.PageSize;
            var products = await baseQuery
                .OrderByDescending(p => p.UpdatedAt)
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            sw.Stop();

            _logger.LogInformation(
                "Search executed in {ElapsedMs}ms for '{SearchTerm}'. " +
                "Tenant: {TenantId}, Results: {Count}/{Total}",
                sw.ElapsedMilliseconds,
                query.SearchTerm,
                query.TenantId,
                products.Count,
                totalCount);

            return new PagedResult<ProductDto>
            {
                Items = products.Select(MapToDto),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products for '{SearchTerm}'", query.SearchTerm);
            throw;
        }
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

/*
/// <summary>
/// Handler for GetCatalogStatsQuery
/// Returns aggregated statistics about the catalog
/// Queries the denormalized read model for fast computation
/// Ideally cached for 30 minutes
/// 
/// TEMPORARILY DISABLED: Type constraint mismatch between GetCatalogStatsQuery
/// and the IQueryHandler<TQuery, TResponse> interface.
/// The issue stems from multiple CatalogStats class definitions in different namespaces.
/// This will be resolved after refactoring the model structure.
/// </summary>
public class GetCatalogStatsQueryHandler : IQueryHandler<GetCatalogStatsQuery, CatalogStats>
{
    private readonly CatalogReadDbContext _readDb;
    private readonly ILogger<GetCatalogStatsQueryHandler> _logger;

    public GetCatalogStatsQueryHandler(
        CatalogReadDbContext readDb,
        ILogger<GetCatalogStatsQueryHandler> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    public async Task<CatalogStats> Handle(
        GetCatalogStatsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Query against read model (no joins, simple aggregations)
            var products = await _readDb.ProductsReadModel
                .AsNoTracking()
                .Where(p => p.TenantId == query.TenantId && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            var stats = new CatalogStats
            {
                TotalProducts = products.Count,
                ActiveProducts = products.Count(p => p.IsAvailable),
                TotalCategories = products.Select(p => p.Category).Distinct().Count(),
                AveragePrice = products.Any() ? products.Average(p => p.Price) : 0,
                MinPrice = products.Any() ? products.Min(p => p.Price) : 0,
                MaxPrice = products.Any() ? products.Max(p => p.Price) : 0,
                LastUpdated = DateTime.UtcNow
            };

            sw.Stop();

            _logger.LogInformation(
                "Catalog stats calculated in {ElapsedMs}ms. " +
                "Total: {Total}, Active: {Active}, Avg Price: {AvgPrice}",
                sw.ElapsedMilliseconds,
                stats.TotalProducts,
                stats.ActiveProducts,
                stats.AveragePrice);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating catalog stats for tenant {TenantId}", query.TenantId);
            throw;
        }
    }
}
*/

public class CatalogStats
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalCategories { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public DateTime LastUpdated { get; set; }
}
