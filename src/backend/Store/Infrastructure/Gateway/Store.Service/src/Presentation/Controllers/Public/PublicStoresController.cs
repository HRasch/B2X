using Microsoft.AspNetCore.Mvc;
using B2X.Store.Application.Store.ReadServices;
using Microsoft.Extensions.Logging;
using B2X.Store.Core.Common.Entities;
using Microsoft.Extensions.Logging;

namespace B2X.Store.Presentation.Controllers.Public;

[ApiController]
[Route("api/public/shops")]
public class PublicShopsController : ControllerBase
{
    private readonly IShopReadService _shopReadService;
    private readonly ILogger<PublicShopsController> _logger;

    public PublicShopsController(IShopReadService shopReadService, ILogger<PublicShopsController> logger)
    {
        _shopReadService = shopReadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<Shop>>> GetShops(CancellationToken cancellationToken)
    {
        var shops = await _shopReadService.GetAllShopsAsync(cancellationToken);
        return Ok(shops);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Shop>> GetShop(Guid id, CancellationToken cancellationToken)
    {
        var shop = await _shopReadService.GetShopByIdAsync(id, cancellationToken);
        if (shop == null)
            return NotFound($"Shop with ID '{id}' not found");

        return Ok(shop);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<Shop>> GetShopByCode(string code, CancellationToken cancellationToken)
    {
        var shop = await _shopReadService.GetShopByCodeAsync(code, cancellationToken);
        if (shop == null)
            return NotFound($"Shop with code '{code}' not found");

        return Ok(shop);
    }

    [HttpGet("main")]
    public async Task<ActionResult<Shop>> GetMainShop(CancellationToken cancellationToken)
    {
        var shop = await _shopReadService.GetMainShopAsync(cancellationToken);
        if (shop == null)
            return NotFound("Main shop not found");

        return Ok(shop);
    }

    [HttpGet("country/{countryId}")]
    public async Task<ActionResult<ICollection<Shop>>> GetShopsByCountry(Guid countryId, CancellationToken cancellationToken)
    {
        var shops = await _shopReadService.GetShopsByCountryAsync(countryId, cancellationToken);
        return Ok(shops);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetShopCount(CancellationToken cancellationToken)
    {
        var count = await _shopReadService.GetShopCountAsync(cancellationToken);
        return Ok(count);
    }
}


