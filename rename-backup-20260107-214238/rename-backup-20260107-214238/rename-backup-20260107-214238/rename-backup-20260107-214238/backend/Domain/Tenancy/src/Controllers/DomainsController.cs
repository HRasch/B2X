using B2X.Tenancy.Handlers.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Tenancy.Controllers;

/// <summary>
/// Controller for domain management operations.
/// </summary>
[ApiController]
[Route("api/admin/domains")]
[Authorize(Policy = "PlatformAdmin")]
public class DomainsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public DomainsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Gets a domain by ID.
    /// </summary>
    [HttpGet("{domainId:guid}")]
    [ProducesResponseType(typeof(DomainDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDomain(Guid domainId, CancellationToken cancellationToken = default)
    {
        var query = new GetDomainQuery(domainId);
        var result = await _messageBus.InvokeAsync<DomainDetailDto?>(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { error = "Domain not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Verifies a domain's DNS configuration.
    /// </summary>
    [HttpPost("{domainId:guid}/verify")]
    [ProducesResponseType(typeof(VerifyDomainResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyDomain(Guid domainId, CancellationToken cancellationToken = default)
    {
        var command = new VerifyDomainCommand(domainId);

        try
        {
            var result = await _messageBus.InvokeAsync<VerifyDomainResponse>(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Domain not found" });
        }
    }
}
