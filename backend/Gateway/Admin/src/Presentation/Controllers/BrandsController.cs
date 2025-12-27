using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.Application.Commands.Brands;
using B2Connect.Admin.Presentation.Filters;
using Wolverine;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// API Controller for Brand operations - HTTP Layer Only (CQRS Pattern)
/// 
/// NOTE: TenantId wird automatisch im Handler via ITenantContextAccessor injiziert!
///
/// Filters Applied:
/// - ValidateTenantAttribute: Validates X-Tenant-ID header
/// - ApiExceptionHandlingFilter: Centralizes error handling
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ValidateTenant]
public class BrandsController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public BrandsController(IMessageBus messageBus, ILogger<BrandsController> logger) : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    /// <summary>
    /// Gets a brand by ID
    /// HTTP: GET /api/brands/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BrandResult>> GetBrand(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Fetching brand {BrandId}", id);

        var query = new GetBrandQuery(id);
        var brand = await _messageBus.InvokeAsync<BrandResult?>(query, ct);

        if (brand == null)
            return NotFoundResponse($"Brand {id} not found");

        return OkResponse(brand);
    }

    /// <summary>
    /// Gets a brand by slug
    /// HTTP: GET /api/brands/slug/{slug}
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<BrandResult>> GetBrandBySlug(string slug, CancellationToken ct)
    {
        _logger.LogInformation("Fetching brand by slug '{Slug}'", slug);

        var query = new GetBrandBySlugQuery(slug);
        var brand = await _messageBus.InvokeAsync<BrandResult?>(query, ct);

        if (brand == null)
            return NotFoundResponse($"Brand with slug '{slug}' not found");

        return OkResponse(brand);
    }

    /// <summary>
    /// Gets all active brands
    /// HTTP: GET /api/brands
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandResult>>> GetActiveBrands(CancellationToken ct)
    {
        _logger.LogInformation("Fetching active brands");

        var query = new GetActiveBrandsQuery();
        var brands = await _messageBus.InvokeAsync<IEnumerable<BrandResult>>(query, ct);

        return OkResponse(brands);
    }

    /// <summary>
    /// Gets brands with pagination
    /// HTTP: GET /api/brands/paged?pageNumber=1&pageSize=10
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult> GetBrandsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching brands (page {PageNumber}, size {PageSize})", pageNumber, pageSize);

        var query = new GetBrandsPagedQuery(pageNumber, pageSize);
        var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<BrandResult>, int)>(query, ct);

        return OkResponseDynamic(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Creates a new brand
    /// HTTP: POST /api/brands
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandResult>> CreateBrand([FromBody] CreateBrandRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating brand", userId);

        var command = new CreateBrandCommand(
            request.Name,
            request.Slug,
            request.Logo,
            request.Description);

        var brand = await _messageBus.InvokeAsync<BrandResult>(command, ct);

        return CreatedResponse(nameof(GetBrand), new { id = brand.Id }, brand);
    }

    /// <summary>
    /// Updates an existing brand
    /// HTTP: PUT /api/brands/{id}
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandResult>> UpdateBrand(Guid id, [FromBody] UpdateBrandRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} updating brand {BrandId}", userId, id);

        var command = new UpdateBrandCommand(
            id,
            request.Name,
            request.Slug,
            request.Logo,
            request.Description);

        var brand = await _messageBus.InvokeAsync<BrandResult>(command, ct);

        return OkResponse(brand, "Brand updated successfully");
    }

    /// <summary>
    /// Deletes a brand
    /// HTTP: DELETE /api/brands/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBrand(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} deleting brand {BrandId}", userId, id);

        var command = new DeleteBrandCommand(id);
        var success = await _messageBus.InvokeAsync<bool>(command, ct);

        if (!success)
            return NotFoundResponse($"Brand {id} not found");

        return NoContent();
    }
}

/// <summary>
/// Request DTOs für CreateBrand
/// </summary>
public record CreateBrandRequest(
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null);

/// <summary>
/// Request DTOs für UpdateBrand
/// </summary>
public record UpdateBrandRequest(
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null);
