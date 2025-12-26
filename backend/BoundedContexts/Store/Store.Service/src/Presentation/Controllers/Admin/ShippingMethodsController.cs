using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2Connect.Store.Application.Store.Services;
using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Store.Entities;

namespace B2Connect.Store.Presentation.Controllers.Admin;

[ApiController]
[Route("api/shipping-methods")]
[Authorize(Roles = "Admin")]
public class ShippingMethodsController : ControllerBase
{
    private readonly IShippingMethodService _shippingMethodService;
    private readonly ILogger<ShippingMethodsController> _logger;

    public ShippingMethodsController(IShippingMethodService shippingMethodService, ILogger<ShippingMethodsController> logger)
    {
        _shippingMethodService = shippingMethodService;
        _logger = logger;
    }

    [HttpGet("store/{storeId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<ShippingMethod>>> GetShippingMethodsForStore(Guid storeId, CancellationToken cancellationToken)
    {
        var methods = await _shippingMethodService.GetActiveMethodsAsync(storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/country/{countryCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<ShippingMethod>>> GetShippingMethodsForCountry(Guid storeId, string countryCode, CancellationToken cancellationToken)
    {
        var methods = await _shippingMethodService.GetMethodsForCountryAsync(storeId, countryCode, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/country/{countryCode}/weight/{weight}")]
    [AllowAnonymous]
    public async Task<ActionResult<ShippingMethod>> GetCheapestShippingMethod(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken)
    {
        var method = await _shippingMethodService.GetCheapestMethodAsync(storeId, countryCode, weight, cancellationToken);
        if (method == null)
            return NotFound($"No shipping methods found for {countryCode} with weight {weight}kg");

        return Ok(method);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShippingMethod>> GetShippingMethod(Guid id, CancellationToken cancellationToken)
    {
        var method = await _shippingMethodService.GetShippingMethodByIdAsync(id, cancellationToken);
        if (method == null)
            return NotFound($"Shipping method with ID '{id}' not found");

        return Ok(method);
    }

    [HttpPost]
    public async Task<ActionResult<ShippingMethod>> CreateShippingMethod([FromBody] ShippingMethod method, CancellationToken cancellationToken)
    {
        var createdMethod = await _shippingMethodService.CreateShippingMethodAsync(method, cancellationToken);
        return CreatedAtAction(nameof(GetShippingMethod), new { id = createdMethod.Id }, createdMethod);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ShippingMethod>> UpdateShippingMethod(Guid id, [FromBody] ShippingMethod method, CancellationToken cancellationToken)
    {
        if (id != method.Id)
            return BadRequest("ID mismatch");

        var updated = await _shippingMethodService.UpdateShippingMethodAsync(method, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShippingMethod(Guid id, CancellationToken cancellationToken)
    {
        await _shippingMethodService.DeleteShippingMethodAsync(id, cancellationToken);
        return NoContent();
    }
}


