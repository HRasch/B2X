using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.Application.Commands.Categories;
using B2Connect.Admin.Presentation.Filters;
using Wolverine;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// API Controller for Category operations - HTTP Layer Only (CQRS Pattern)
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
public class CategoriesController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public CategoriesController(IMessageBus messageBus, ILogger<CategoriesController> logger) : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    /// <summary>
    /// Gets a category by ID
    /// HTTP: GET /api/categories/{id}
    /// CQRS: GetCategoryQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResult>> GetCategory(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching category {CategoryId} for tenant {TenantId}", id, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetCategoryQuery(tenantId, id);
        var category = await _messageBus.InvokeAsync<CategoryResult?>(query, ct);

        if (category == null)
            return NotFoundResponse($"Category {id} not found");

        return OkResponse(category);
    }

    /// <summary>
    /// Gets a category by slug
    /// HTTP: GET /api/categories/slug/{slug}
    /// CQRS: GetCategoryBySlugQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryResult>> GetCategoryBySlug(string slug, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching category by slug '{Slug}' for tenant {TenantId}", slug, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetCategoryBySlugQuery(tenantId, slug);
        var category = await _messageBus.InvokeAsync<CategoryResult?>(query, ct);

        if (category == null)
            return NotFoundResponse($"Category with slug '{slug}' not found");

        return OkResponse(category);
    }

    /// <summary>
    /// Gets all root categories
    /// HTTP: GET /api/categories/root
    /// CQRS: GetRootCategoriesQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("root")]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetRootCategories(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching root categories for tenant {TenantId}", tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetRootCategoriesQuery(tenantId);
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Gets child categories of a parent
    /// HTTP: GET /api/categories/{parentId}/children
    /// CQRS: GetChildCategoriesQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("{parentId}/children")]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetChildCategories(Guid parentId, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching child categories for parent {ParentId} in tenant {TenantId}",
            parentId, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetChildCategoriesQuery(tenantId, parentId);
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Gets the complete category hierarchy
    /// HTTP: GET /api/categories/hierarchy
    /// CQRS: GetCategoryHierarchyQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("hierarchy")]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetHierarchy(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching category hierarchy for tenant {TenantId}", tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetCategoryHierarchyQuery(tenantId);
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Gets all active categories
    /// HTTP: GET /api/categories
    /// CQRS: GetActiveCategoriesQuery dispatched to Wolverine
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetActiveCategories(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching active categories for tenant {TenantId}", tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetActiveCategoriesQuery(tenantId);
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Creates a new category
    /// HTTP: POST /api/categories
    /// CQRS: CreateCategoryCommand dispatched to Wolverine
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResult>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating category for tenant {TenantId}", userId, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new CreateCategoryCommand(
            tenantId,
            request.Name,
            request.Slug,
            request.Description,
            request.ParentId);

        var category = await _messageBus.InvokeAsync<CategoryResult>(command, ct);

        return CreatedResponse(nameof(GetCategory), new { id = category.Id }, category);
    }

    /// <summary>
    /// Updates an existing category
    /// HTTP: PUT /api/categories/{id}
    /// CQRS: UpdateCategoryCommand dispatched to Wolverine
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResult>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} updating category {CategoryId} in tenant {TenantId}",
            userId, id, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new UpdateCategoryCommand(
            tenantId,
            id,
            request.Name,
            request.Slug,
            request.Description,
            request.ParentId);

        var category = await _messageBus.InvokeAsync<CategoryResult>(command, ct);

        return OkResponse(category, "Category updated successfully");
    }

    /// <summary>
    /// Deletes a category
    /// HTTP: DELETE /api/categories/{id}
    /// CQRS: DeleteCategoryCommand dispatched to Wolverine
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} deleting category {CategoryId} from tenant {TenantId}",
            userId, id, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new DeleteCategoryCommand(tenantId, id);
        var success = await _messageBus.InvokeAsync<bool>(command, ct);

        if (!success)
            return NotFoundResponse($"Category {id} not found");

        return NoContent();
    }
}

/// <summary>
/// Request DTOs für CreateCategory
/// </summary>
public record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null);

/// <summary>
/// Request DTOs für UpdateCategory
/// </summary>
public record UpdateCategoryRequest(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null);
