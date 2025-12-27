using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.Application.Commands.Brands;
using B2Connect.Admin.Presentation.Filters;
using Wolverine;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// API Controller for Brand operations - HTTP Layer Only (CQRS Pattern)
/// 
/// 🏗️ Architektur:
/// HTTP Request 
///   ↓
/// Controller (HTTP Concerns nur!)
///   ↓
/// Wolverine Message Bus
///   ↓
/// Handler (Business Logic)
///   ↓
/// Response
///
/// Filters Applied:
/// - ValidateTenantAttribute: Validates X-Tenant-ID header
/// - ApiExceptionHandlingFilter: Centralizes error handling
/// - ValidateModelStateFilter: Validates request models
/// - ApiLoggingFilter: Logs all requests and responses
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
    /// CQRS: GetBrandQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BrandResult>> GetBrand(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching brand {BrandId} for tenant {TenantId}", id, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetBrandQuery(tenantId, id);
        var brand = await _messageBus.InvokeAsync<BrandResult?>(query, ct);

        if (brand == null)
            return NotFoundResponse($"Brand {id} not found");

        return OkResponse(brand);
    }

    /// <summary>
    /// Gets a brand by slug
    /// HTTP: GET /api/brands/slug/{slug}
    /// CQRS: GetBrandBySlugQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<BrandResult>> GetBrandBySlug(string slug, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching brand by slug '{Slug}' for tenant {TenantId}", slug, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetBrandBySlugQuery(tenantId, slug);
        var brand = await _messageBus.InvokeAsync<BrandResult?>(query, ct);

        if (brand == null)
            return NotFoundResponse($"Brand with slug '{slug}' not found");

        return OkResponse(brand);
    }

    /// <summary>
    /// Gets all active brands
    /// HTTP: GET /api/brands
    /// CQRS: GetActiveBrandsQuery dispatched to Wolverine
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandResult>>> GetActiveBrands(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching active brands for tenant {TenantId}", tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetActiveBrandsQuery(tenantId);
        var brands = await _messageBus.InvokeAsync<IEnumerable<BrandResult>>(query, ct);

        return OkResponse(brands);
    }

    /// <summary>
    /// Gets brands with pagination
    /// HTTP: GET /api/brands/paged?pageNumber=1&pageSize=10
    /// CQRS: GetBrandsPagedQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult> GetBrandsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching brands (page {PageNumber}, size {PageSize}) for tenant {TenantId}",
            pageNumber, pageSize, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetBrandsPagedQuery(tenantId, pageNumber, pageSize);
        var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<BrandResult>, int)>(query, ct);

        return OkResponse(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Creates a new brand
    /// HTTP: POST /api/brands
    /// CQRS: CreateBrandCommand dispatched to Wolverine
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandResult>> CreateBrand([FromBody] CreateBrandRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating brand for tenant {TenantId}", userId, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new CreateBrandCommand(
            tenantId,
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
    /// CQRS: UpdateBrandCommand dispatched to Wolverine
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandResult>> UpdateBrand(Guid id, [FromBody] UpdateBrandRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} updating brand {BrandId} in tenant {TenantId}",
            userId, id, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new UpdateBrandCommand(
            tenantId,
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
    /// CQRS: DeleteBrandCommand dispatched to Wolverine
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBrand(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} deleting brand {BrandId} from tenant {TenantId}",
            userId, id, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new DeleteBrandCommand(tenantId, id);
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
