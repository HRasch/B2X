using Microsoft.AspNetCore.Mvc;
using B2Connect.Store.Application.Store.ReadServices;
using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Store.Entities;

namespace B2Connect.Store.Presentation.Controllers.Public;

[ApiController]
[Route("api/public/shipping-methods")]
public class PublicShippingMethodsController : ControllerBase
{
    private readonly IShippingMethodReadService _shippingMethodReadService;
    private readonly ILogger<PublicShippingMethodsController> _logger;

    public PublicShippingMethodsController(IShippingMethodReadService shippingMethodReadService, ILogger<PublicShippingMethodsController> logger)
    {
        _shippingMethodReadService = shippingMethodReadService;
        _logger = logger;
    }

    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<ICollection<ShippingMethod>>> GetShippingMethodsForStore(Guid storeId, CancellationToken cancellationToken)
    {
        var methods = await _shippingMethodReadService.GetActiveMethodsAsync(storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/country/{countryCode}")]
    public async Task<ActionResult<ICollection<ShippingMethod>>> GetShippingMethodsForCountry(Guid storeId, string countryCode, CancellationToken cancellationToken)
    {
        var methods = await _shippingMethodReadService.GetMethodsForCountryAsync(storeId, countryCode, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/carrier/{carrier}")]
    public async Task<ActionResult<ICollection<ShippingMethod>>> GetShippingMethodsByCarrier(Guid storeId, string carrier, CancellationToken cancellationToken)
    {
        var methods = await _shippingMethodReadService.GetByCarrierAsync(carrier, storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/carriers")]
    public async Task<ActionResult<ICollection<string>>> GetAvailableCarriers(Guid storeId, CancellationToken cancellationToken)
    {
        var carriers = await _shippingMethodReadService.GetAvailableCarriersAsync(storeId, cancellationToken);
        return Ok(carriers);
    }

    [HttpGet("store/{storeId}/country/{countryCode}/weight/{weight}")]
    public async Task<ActionResult<ShippingMethod>> GetCheapestShippingMethod(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken)
    {
        var method = await _shippingMethodReadService.GetCheapestMethodAsync(storeId, countryCode, weight, cancellationToken);
        if (method == null)
            return NotFound($"No shipping methods found for {countryCode} with weight {weight}kg");

        return Ok(method);
    }

    [HttpGet("cost")]
    public async Task<ActionResult<decimal>> CalculateShippingCost([FromQuery] Guid storeId, [FromQuery] string countryCode, [FromQuery] Guid shippingMethodId, [FromQuery] decimal weight, CancellationToken cancellationToken)
    {
        var cost = await _shippingMethodReadService.CalculateShippingCostAsync(storeId, countryCode, shippingMethodId, weight, cancellationToken);
        return Ok(cost);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShippingMethod>> GetShippingMethod(Guid id, CancellationToken cancellationToken)
    {
        var method = await _shippingMethodReadService.GetShippingMethodByIdAsync(id, cancellationToken);
        if (method == null)
            return NotFound($"Shipping method with ID '{id}' not found");

        return Ok(method);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<ShippingMethod>> GetShippingMethodByCode(string code, CancellationToken cancellationToken)
    {
        var method = await _shippingMethodReadService.GetShippingMethodByCodeAsync(code, cancellationToken);
        if (method == null)
            return NotFound($"Shipping method with code '{code}' not found");

        return Ok(method);
    }

    [HttpGet("store/{storeId}/count")]
    public async Task<ActionResult<int>> GetShippingMethodCount(Guid storeId, CancellationToken cancellationToken)
    {
        var count = await _shippingMethodReadService.GetShippingMethodCountAsync(storeId, cancellationToken);
        return Ok(count);
    }
}


