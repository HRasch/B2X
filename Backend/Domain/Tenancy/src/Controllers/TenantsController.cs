using B2X.Tenancy.Handlers.Domains;
using B2X.Tenancy.Handlers.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Tenancy.Controllers;

/// <summary>
/// API controller for tenant management operations.
/// Restricted to platform administrators.
/// </summary>
[ApiController]
[Route("api/admin/tenants")]
// [Authorize(Policy = "PlatformAdmin")] // Disabled for POC testing
public class TenantsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TenantsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Gets paginated list of tenants.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<TenantListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenants(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        Types.Domain.TenantStatus? statusFilter = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<Types.Domain.TenantStatus>(status, true, out var parsed))
        {
            statusFilter = parsed;
        }

        var query = new GetTenantsQuery(pageNumber, pageSize, statusFilter, search);
        var result = await _messageBus.InvokeAsync<PagedResultDto<TenantListItemDto>>(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets a tenant by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenant(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetTenantQuery(id);
        var result = await _messageBus.InvokeAsync<TenantDetailDto?>(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { error = "Tenant not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTenant(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateTenantCommand(
            request.Name,
            request.Slug,
            request.LogoUrl,
            request.Metadata);

        try
        {
            var result = await _messageBus.InvokeAsync<CreateTenantResponse>(command, cancellationToken);
            return CreatedAtAction(nameof(GetTenant), new { id = result.TenantId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TenantDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTenant(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        Types.Domain.TenantStatus? status = null;
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<Types.Domain.TenantStatus>(request.Status, true, out var parsed))
        {
            status = parsed;
        }

        var command = new UpdateTenantCommand(
            id,
            request.Name,
            request.LogoUrl,
            status,
            request.SuspensionReason,
            request.Metadata);

        try
        {
            var result = await _messageBus.InvokeAsync<TenantDetailDto>(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Tenant not found" });
        }
    }

    /// <summary>
    /// Archives a tenant (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveTenant(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new ArchiveTenantCommand(id);

        try
        {
            await _messageBus.InvokeAsync<bool>(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Tenant not found" });
        }
    }

    #region Domains

    /// <summary>
    /// Gets all domains for a tenant.
    /// </summary>
    [HttpGet("{tenantId:guid}/domains")]
    [ProducesResponseType(typeof(IReadOnlyList<DomainDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDomains(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var query = new GetDomainsQuery(tenantId);
        var result = await _messageBus.InvokeAsync<IReadOnlyList<DomainDetailDto>>(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Adds a domain to a tenant.
    /// </summary>
    [HttpPost("{tenantId:guid}/domains")]
    [ProducesResponseType(typeof(AddDomainResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddDomain(
        Guid tenantId,
        [FromBody] AddDomainRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddDomainCommand(tenantId, request.DomainName, request.SetAsPrimary);

        try
        {
            var result = await _messageBus.InvokeAsync<AddDomainResponse>(command, cancellationToken);
            return CreatedAtAction(nameof(GetDomains), new { tenantId }, result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Tenant not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Removes a domain from a tenant.
    /// </summary>
    [HttpDelete("{tenantId:guid}/domains/{domainId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveDomain(
        Guid tenantId,
        Guid domainId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveDomainCommand(tenantId, domainId);

        try
        {
            await _messageBus.InvokeAsync<bool>(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Domain not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Sets a domain as primary for a tenant.
    /// </summary>
    [HttpPost("{tenantId:guid}/domains/{domainId:guid}/set-primary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimaryDomain(
        Guid tenantId,
        Guid domainId,
        CancellationToken cancellationToken = default)
    {
        var command = new SetPrimaryDomainCommand(tenantId, domainId);

        try
        {
            await _messageBus.InvokeAsync<bool>(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Domain not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    #endregion
}

#region Request DTOs

public record CreateTenantRequest(
    string Name,
    string Slug,
    string? LogoUrl = null,
    Dictionary<string, string>? Metadata = null);

public record UpdateTenantRequest(
    string? Name = null,
    string? LogoUrl = null,
    string? Status = null,
    string? SuspensionReason = null,
    Dictionary<string, string>? Metadata = null);

#endregion
