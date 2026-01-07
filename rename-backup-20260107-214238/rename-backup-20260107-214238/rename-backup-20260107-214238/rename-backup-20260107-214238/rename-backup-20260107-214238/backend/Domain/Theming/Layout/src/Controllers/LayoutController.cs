using B2Connect.LayoutService.Data;
using B2Connect.LayoutService.Models;
using B2Connect.LayoutService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace B2Connect.LayoutService.Controllers;

/// <summary>
/// REST API Controller for Layout Service
/// Manages CMS pages, sections, and components
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LayoutController : ControllerBase
{
    private readonly ILayoutService _layoutService;
    private readonly ILogger<LayoutController> _logger;

    /// <summary>
    /// Tenant ID from request context (header or claims)
    /// In production, extract from X-Tenant-ID header or JWT claims
    /// </summary>
    public Guid TenantId { get; set; } = Guid.Empty;

    public LayoutController(ILayoutService layoutService, ILogger<LayoutController>? logger = null)
    {
        _layoutService = layoutService;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<LayoutController>.Instance;
    }

    #region Page Management Endpoints

    /// <summary>
    /// Create a new CMS page
    /// POST /api/layout/pages
    /// </summary>
    [HttpPost("pages")]
    [ProducesResponseType(typeof(CmsPage), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CmsPage>> CreatePage([FromBody] CreatePageRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new page with slug: {Slug}", request.Slug);

            var page = await _layoutService.CreatePageAsync(TenantId, request);

            return CreatedAtRoute(nameof(GetPage), new { id = page.Id }, page);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error creating page: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Business logic error creating page: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a page by ID
    /// GET /api/layout/pages/{id}
    /// </summary>
    [HttpGet("pages/{id}", Name = nameof(GetPage))]
    [ProducesResponseType(typeof(CmsPage), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CmsPageDto>> GetPage(Guid id, [FromQuery] string lang = "en")
    {
        _logger.LogInformation("Getting page with ID: {PageId}", id);

        var page = await _layoutService.GetPageByIdAsync(TenantId, id, lang);

        if (page == null)
        {
            _logger.LogWarning("Page not found: {PageId}", id);
            return NotFound();
        }

        return Ok(page);
    }

    /// <summary>
    /// Get all pages for the current tenant
    /// GET /api/layout/pages
    /// </summary>
    [HttpGet("pages")]
    [ProducesResponseType(typeof(List<CmsPageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CmsPageDto>>> GetPages([FromQuery] string lang = "en")
    {
        _logger.LogInformation("Getting all pages for tenant: {TenantId}", TenantId);

        var pages = await _layoutService.GetPagesByTenantAsync(TenantId, lang);

        return Ok(pages);
    }

    /// <summary>
    /// Update an existing page
    /// PUT /api/layout/pages/{id}
    /// </summary>
    [HttpPut("pages/{id}")]
    [ProducesResponseType(typeof(CmsPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePage(Guid id, [FromBody] UpdatePageRequest request, [FromQuery] string lang = "en")
    {
        try
        {
            _logger.LogInformation("Updating page with ID: {PageId}", id);

            var page = await _layoutService.UpdatePageAsync(TenantId, id, request, lang);

            return Ok(page);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Page not found for update: {PageId}", id);
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error updating page: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a page
    /// DELETE /api/layout/pages/{id}
    /// </summary>
    [HttpDelete("pages/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePage(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting page with ID: {PageId}", id);

            await _layoutService.DeletePageAsync(TenantId, id);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Page not found for deletion: {PageId}", id);
            return NotFound();
        }
    }

    #endregion

    #region Section Management Endpoints

    /// <summary>
    /// Add a section to a page
    /// POST /api/layout/pages/{pageId}/sections
    /// </summary>
    [HttpPost("pages/{pageId}/sections")]
    [ProducesResponseType(typeof(CmsSection), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CmsSection>> AddSection(Guid pageId, [FromBody] AddSectionRequest request)
    {
        try
        {
            _logger.LogInformation("Adding section to page: {PageId}", pageId);

            var section = await _layoutService.AddSectionAsync(TenantId, pageId, request);

            return CreatedAtRoute(nameof(GetPage), new { id = pageId }, section);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Page not found for adding section: {PageId}", pageId);
            return NotFound();
        }
    }

    /// <summary>
    /// Remove a section from a page
    /// DELETE /api/layout/pages/{pageId}/sections/{sectionId}
    /// </summary>
    [HttpDelete("pages/{pageId}/sections/{sectionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSection(Guid pageId, Guid sectionId)
    {
        try
        {
            _logger.LogInformation("Removing section: {SectionId} from page: {PageId}", sectionId, pageId);

            await _layoutService.RemoveSectionAsync(TenantId, pageId, sectionId);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Section not found for removal: {SectionId}", sectionId);
            return NotFound();
        }
    }

    /// <summary>
    /// Reorder sections in a page
    /// POST /api/layout/pages/{pageId}/sections/reorder
    /// </summary>
    [HttpPost("pages/{pageId}/sections/reorder")]
    [ProducesResponseType(typeof(List<CmsSection>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CmsSection>>> ReorderSections(Guid pageId, [FromBody] List<SectionOrderRequest> orders)
    {
        try
        {
            _logger.LogInformation("Reordering {Count} sections in page: {PageId}", orders.Count, pageId);

            var sectionOrders = orders.Select(o => (o.SectionId, o.Order)).ToList();
            var sections = await _layoutService.ReorderSectionsAsync(TenantId, pageId, sectionOrders);

            return Ok(sections);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Page not found for section reordering: {PageId}", pageId);
            return NotFound();
        }
    }

    #endregion

    #region Component Management Endpoints

    /// <summary>
    /// Add a component to a section
    /// POST /api/layout/pages/{pageId}/sections/{sectionId}/components
    /// </summary>
    [HttpPost("pages/{pageId}/sections/{sectionId}/components")]
    [ProducesResponseType(typeof(CmsComponent), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CmsComponentDto>> AddComponent(Guid pageId, Guid sectionId, [FromBody] AddComponentRequest request, [FromQuery] string lang = "en")
    {
        try
        {
            _logger.LogInformation("Adding component to section: {SectionId}", sectionId);

            var component = await _layoutService.AddComponentAsync(TenantId, pageId, sectionId, request, lang);

            return CreatedAtRoute(nameof(GetPage), new { id = pageId }, component);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Section not found for adding component: {SectionId}", sectionId);
            return NotFound();
        }
    }

    /// <summary>
    /// Update a component
    /// PUT /api/layout/pages/{pageId}/sections/{sectionId}/components/{componentId}
    /// </summary>
    [HttpPut("pages/{pageId}/sections/{sectionId}/components/{componentId}")]
    [ProducesResponseType(typeof(CmsComponentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComponent(Guid pageId, Guid sectionId, Guid componentId, [FromBody] UpdateComponentRequest request, [FromQuery] string lang = "en")
    {
        try
        {
            _logger.LogInformation("Updating component: {ComponentId}", componentId);

            var component = await _layoutService.UpdateComponentAsync(TenantId, pageId, sectionId, componentId, request, lang);

            return Ok(component);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Component not found for update: {ComponentId}", componentId);
            return NotFound();
        }
    }

    /// <summary>
    /// Remove a component from a section
    /// DELETE /api/layout/pages/{pageId}/sections/{sectionId}/components/{componentId}
    /// </summary>
    [HttpDelete("pages/{pageId}/sections/{sectionId}/components/{componentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveComponent(Guid pageId, Guid sectionId, Guid componentId)
    {
        try
        {
            _logger.LogInformation("Removing component: {ComponentId}", componentId);

            await _layoutService.RemoveComponentAsync(TenantId, pageId, sectionId, componentId);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Component not found for removal: {ComponentId}", componentId);
            return NotFound();
        }
    }

    #endregion

    #region Component Definitions Endpoints

    /// <summary>
    /// Get all available component definitions
    /// GET /api/layout/components/definitions
    /// </summary>
    [HttpGet("components/definitions")]
    [ProducesResponseType(typeof(List<ComponentDefinition>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ComponentDefinition>>> GetComponentDefinitions()
    {
        _logger.LogInformation("Getting all component definitions");

        var definitions = await _layoutService.GetComponentDefinitionsAsync();

        return Ok(definitions);
    }

    /// <summary>
    /// Get a specific component definition
    /// GET /api/layout/components/definitions/{componentType}
    /// </summary>
    [HttpGet("components/definitions/{componentType}")]
    [ProducesResponseType(typeof(ComponentDefinition), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComponentDefinition>> GetComponentDefinition(string componentType)
    {
        _logger.LogInformation("Getting component definition: {ComponentType}", componentType);

        var definition = await _layoutService.GetComponentDefinitionAsync(componentType);

        if (definition == null)
        {
            _logger.LogWarning("Component definition not found: {ComponentType}", componentType);
            return NotFound();
        }

        return Ok(definition);
    }

    #endregion

    #region Preview Endpoints

    /// <summary>
    /// Generate HTML preview of a page
    /// GET /api/layout/pages/{pageId}/preview
    /// </summary>
    [HttpGet("pages/{pageId}/preview")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("text/html")]
    public async Task<ActionResult<string>> GeneratePreview(Guid pageId, [FromQuery] string lang = "en")
    {
        try
        {
            _logger.LogInformation("Generating preview for page: {PageId}", pageId);

            var html = await _layoutService.GeneratePreviewHtmlAsync(TenantId, pageId, lang);

            return Ok(html);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Page not found for preview generation: {PageId}", pageId);
            return NotFound();
        }
    }

    #endregion
}
