using B2X.Admin.Application.Commands.Categories;
using B2X.Categories.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Admin.Controllers;

/// <summary>
/// Admin Categories Management API
/// Provides full CRUD operations for product categories management
/// </summary>
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Policy = "CatalogManager")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly IMessageBus _messageBus;

    public CategoriesController(ILogger<CategoriesController> logger, IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryDto createDto,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var command = new CreateCategoryCommand(
                tenantId,
                createDto.Name,
                createDto.Description,
                createDto.Slug,
                createDto.ParentId,
                createDto.ImageUrl,
                createDto.Icon,
                createDto.DisplayOrder,
                createDto.MetaTitle,
                createDto.MetaDescription,
                createDto.IsActive,
                createDto.IsVisible
            );

            var result = await _messageBus.InvokeAsync<CategoryDto>(command);

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = result.Id },
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return BadRequest(new { Message = "Failed to create category", Error = ex.Message });
        }
    }

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategory(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetCategoryByIdQuery(id, tenantId);
            var result = await _messageBus.InvokeAsync<CategoryDto>(query);

            if (result == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {Id}", id);
            return NotFound(new { Message = "Category not found" });
        }
    }

    /// <summary>
    /// Gets a category by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryBySlug(string slug, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetCategoryBySlugQuery(slug, tenantId);
            var result = await _messageBus.InvokeAsync<CategoryDto>(query);

            if (result == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category by slug {Slug}", slug);
            return NotFound(new { Message = "Category not found" });
        }
    }

    /// <summary>
    /// Gets categories with optional parent filter and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryDto>), 200)]
    public async Task<IActionResult> GetCategories(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] Guid? parentId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetCategoriesQuery(tenantId, null, parentId, null, page, pageSize);

            var result = await _messageBus.InvokeAsync<PagedResult<CategoryDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return BadRequest(new { Message = "Failed to retrieve categories" });
        }
    }

    /// <summary>
    /// Gets root categories (no parent)
    /// </summary>
    [HttpGet("root")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
    public async Task<IActionResult> GetRootCategories([FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetRootCategoriesQuery(tenantId);
            var result = await _messageBus.InvokeAsync<IEnumerable<CategoryDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving root categories");
            return BadRequest(new { Message = "Failed to retrieve root categories" });
        }
    }

    /// <summary>
    /// Gets child categories for a parent
    /// </summary>
    [HttpGet("{parentId}/children")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
    public async Task<IActionResult> GetChildCategories(Guid parentId, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetChildCategoriesQuery(parentId, tenantId);
            var result = await _messageBus.InvokeAsync<IEnumerable<CategoryDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child categories for parent {ParentId}", parentId);
            return BadRequest(new { Message = "Failed to retrieve child categories" });
        }
    }

    /// <summary>
    /// Gets the complete category tree
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
    public async Task<IActionResult> GetCategoryTree([FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetCategoryTreeQuery(tenantId);
            var result = await _messageBus.InvokeAsync<IEnumerable<CategoryDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category tree");
            return BadRequest(new { Message = "Failed to retrieve category tree" });
        }
    }

    /// <summary>
    /// Updates a category
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryDto updateDto,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var command = new UpdateCategoryCommand(
                id,
                tenantId,
                updateDto.Name,
                updateDto.Description,
                updateDto.Slug,
                updateDto.ParentId,
                updateDto.ImageUrl,
                updateDto.Icon,
                updateDto.DisplayOrder,
                updateDto.MetaTitle,
                updateDto.MetaDescription,
                updateDto.IsActive,
                updateDto.IsVisible
            );

            var result = await _messageBus.InvokeAsync<CategoryDto>(command);

            if (result == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {Id}", id);
            return BadRequest(new { Message = "Failed to update category", Error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteCategory(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var command = new DeleteCategoryCommand(id, tenantId);
            await _messageBus.InvokeAsync<bool>(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {Id}", id);
            return NotFound(new { Message = "Category not found or could not be deleted" });
        }
    }

    /// <summary>
    /// Searches categories
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<CategoryDto>), 200)]
    public async Task<IActionResult> SearchCategories(
        [FromQuery] string q,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { Message = "Search query is required" });
        }

        try
        {
            var query = new SearchCategoriesQuery(q, tenantId, page, pageSize);

            var result = await _messageBus.InvokeAsync<PagedResult<CategoryDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching categories with query '{Query}'", q);
            return BadRequest(new { Message = "Search failed" });
        }
    }
}
