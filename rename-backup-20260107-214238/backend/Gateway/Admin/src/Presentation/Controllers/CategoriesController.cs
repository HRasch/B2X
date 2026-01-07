using B2X.Admin.Application.Commands.Categories;
using B2X.Admin.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Admin.Presentation.Controllers;

/// <summary>
/// API Controller for Category operations - HTTP Layer Only (CQRS Pattern)
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
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResult>> GetCategory(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Fetching category {CategoryId}", id);

        var query = new GetCategoryQuery(id);
        var category = await _messageBus.InvokeAsync<CategoryResult?>(query, ct);

        if (category == null)
        {
            return NotFoundResponse($"Category {id} not found");
        }

        return OkResponse(category);
    }

    /// <summary>
    /// Gets a category by slug
    /// HTTP: GET /api/categories/slug/{slug}
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryResult>> GetCategoryBySlug(string slug, CancellationToken ct)
    {
        _logger.LogInformation("Fetching category by slug '{Slug}'", slug);

        var query = new GetCategoryBySlugQuery(slug);
        var category = await _messageBus.InvokeAsync<CategoryResult?>(query, ct);

        if (category == null)
        {
            return NotFoundResponse($"Category with slug '{slug}' not found");
        }

        return OkResponse(category);
    }

    /// <summary>
    /// Gets all root categories
    /// HTTP: GET /api/categories/root
    /// </summary>
    [HttpGet("root")]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetRootCategories(CancellationToken ct)
    {
        _logger.LogInformation("Fetching root categories");

        var query = new GetRootCategoriesQuery();
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Gets child categories of a parent
    /// HTTP: GET /api/categories/{parentId}/children
    /// </summary>
    [HttpGet("{parentId}/children")]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetChildCategories(Guid parentId, CancellationToken ct)
    {
        _logger.LogInformation("Fetching child categories for parent {ParentId}", parentId);

        var query = new GetChildCategoriesQuery(parentId);
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Gets the complete category hierarchy
    /// HTTP: GET /api/categories/hierarchy
    /// </summary>
    [HttpGet("hierarchy")]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetHierarchy(CancellationToken ct)
    {
        _logger.LogInformation("Fetching category hierarchy");

        var query = new GetCategoryHierarchyQuery();
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Gets all active categories
    /// HTTP: GET /api/categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResult>>> GetActiveCategories(CancellationToken ct)
    {
        _logger.LogInformation("Fetching active categories");

        var query = new GetActiveCategoriesQuery();
        var categories = await _messageBus.InvokeAsync<IEnumerable<CategoryResult>>(query, ct);

        return OkResponse(categories);
    }

    /// <summary>
    /// Creates a new category
    /// HTTP: POST /api/categories
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResult>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating category", userId);

        var command = new CreateCategoryCommand(
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
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResult>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} updating category {CategoryId}", userId, id);

        var command = new UpdateCategoryCommand(
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
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} deleting category {CategoryId}", userId, id);

        var command = new DeleteCategoryCommand(id);
        var success = await _messageBus.InvokeAsync<bool>(command, ct);

        if (!success)
        {
            return NotFoundResponse($"Category {id} not found");
        }

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
