using Microsoft.AspNetCore.Mvc;
using WolverineFx;
using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API Controller for Product READ operations (Queries)
/// Uses Wolverine message bus to invoke query handlers
/// Queries always return immediately (synchronous)
/// Responses are cached for performance at scale
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Produces("application/json")]
public class ProductsQueryController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ProductsQueryController> _logger;

    public ProductsQueryController(
        IMessageBus messageBus,
        ILogger<ProductsQueryController> logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get a single product by ID
    /// GET /api/v2/products/{id}
    /// Cached for 1 hour
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery
        {
            TenantId = GetTenantId(),
            ProductId = id
        };

        try
        {
            _logger.LogInformation("Fetching product {ProductId}", id);
            var product = await _messageBus.InvokeAsync<ProductDto>(query, cancellationToken);

            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get paginated list of products with optional filtering
    /// GET /api/v2/products?page=1&pageSize=20&searchTerm=&category=&minPrice=&maxPrice=&sortBy=name
    /// Cached for 5 minutes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string sortBy = "name",
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = new GetProductsPagedQuery
        {
            TenantId = GetTenantId(),
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy
        };

        try
        {
            _logger.LogInformation("Fetching products page {Page} with filter {SearchTerm}", page, searchTerm);
            var result = await _messageBus.InvokeAsync<PagedResult<ProductDto>>(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// Search products with full-text search
    /// GET /api/v2/products/search?term=&page=1&pageSize=20
    /// Uses ElasticSearch for millions of products
    /// Cached for 5 minutes
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest(new { error = "Search term is required" });
        }

        var query = new SearchProductsQuery
        {
            TenantId = GetTenantId(),
            SearchTerm = term,
            Page = page,
            PageSize = pageSize
        };

        try
        {
            _logger.LogInformation("Searching products for term '{SearchTerm}'", term);
            var result = await _messageBus.InvokeAsync<PagedResult<ProductDto>>(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// Search products using ElasticSearch (Store Frontend)
    /// GET /api/v2/products/elasticsearch?term=&page=1&pageSize=20&language=de&category=&minPrice=&maxPrice=
    /// 
    /// Features:
    /// - Full-text search with fuzzy matching (typo tolerance)
    /// - Multi-field search (Name, Description, Category, SKU, Brand)
    /// - Price range filtering
    /// - Category filtering
    /// - Availability filtering
    /// - Relevance-based ranking
    /// - Language-specific indexes (de, en, fr)
    /// 
    /// Response includes:
    /// - Search results with relevance scores
    /// - Pagination metadata
    /// - Query execution time
    /// 
    /// Use for: Store frontend product discovery
    /// Scales to millions of products efficiently
    /// Cached for 5 minutes
    /// </summary>
    [HttpGet("elasticsearch")]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProductsElastic(
        [FromQuery] string term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string language = "de",
        [FromQuery] string category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool onlyAvailable = true,
        [FromQuery] string sortBy = "relevance",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest(new { error = "Search term is required" });
        }

        // Validate pagination
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = new SearchProductsElasticQuery
        {
            TenantId = GetTenantId(),
            SearchTerm = term,
            Page = page,
            PageSize = pageSize,
            Language = language ?? "de",
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            OnlyAvailable = onlyAvailable,
            SortBy = sortBy
        };

        try
        {
            _logger.LogInformation(
                "Searching ElasticSearch for term '{SearchTerm}' with language '{Language}'",
                term, language);

            var result = await _messageBus.InvokeAsync<PagedResult<ProductDto>>(query, cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid search parameters");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products in ElasticSearch");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get catalog statistics and aggregated data
    /// GET /api/v2/products/stats
    /// Cached for 30 minutes
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(CatalogStats), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogStats(CancellationToken cancellationToken)
    {
        var query = new GetCatalogStatsQuery
        {
            TenantId = GetTenantId()
        };

        try
        {
            _logger.LogInformation("Fetching catalog statistics");
            var stats = await _messageBus.InvokeAsync<CatalogStats>(query, cancellationToken);

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog statistics");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = ex.Message });
        }
    }

    private Guid GetTenantId()
    {
        // Extract tenant from JWT claims (primary) or X-Tenant-ID header (fallback)
        try
        {
            // Check JWT claims for tenant_id
            var tenantClaim = HttpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantFromClaim))
            {
                _logger.LogDebug("Tenant extracted from JWT claim: {TenantId}", tenantFromClaim);
                return tenantFromClaim;
            }

            // Fallback: Check X-Tenant-ID header
            if (HttpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var headerValue))
            {
                if (Guid.TryParse(headerValue, out var tenantFromHeader))
                {
                    _logger.LogDebug("Tenant extracted from X-Tenant-ID header: {TenantId}", tenantFromHeader);
                    return tenantFromHeader;
                }
            }

            // No tenant found - unauthorized
            _logger.LogWarning("No tenant ID found in JWT claims or X-Tenant-ID header");
            throw new UnauthorizedAccessException("Tenant ID not found in request. Provide 'tenant_id' claim in JWT or 'X-Tenant-ID' header.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting tenant ID");
            throw;
        }
    }
}

/// <summary>
/// DTO for catalog statistics
/// Used by GetCatalogStatsQuery handler
/// </summary>
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
