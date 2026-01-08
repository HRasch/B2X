using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2X.Store.Application.Store.Services;
using B2X.Store.Core.Common.Entities;

namespace B2X.Store.Presentation.Controllers.Admin;

[ApiController]
[Route("api/shops")]
[Authorize(Roles = "Admin")]
public class ShopsController : ControllerBase
{
    private readonly IShopService _shopService;
    private readonly ILogger<ShopsController> _logger;

    public ShopsController(IShopService shopService, ILogger<ShopsController> logger)
    {
        _shopService = shopService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<Shop>>> GetShops(CancellationToken cancellationToken)
    {
        var shops = await _shopService.GetAllActiveShopsAsync(cancellationToken);
        return Ok(shops);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Shop>> GetShop(Guid id, CancellationToken cancellationToken)
    {
        var shop = await _shopService.GetShopByIdAsync(id, cancellationToken);
        if (shop == null)
            return NotFound($"Shop with ID '{id}' not found");

        return Ok(shop);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<Shop>> GetShopByCode(string code, CancellationToken cancellationToken)
    {
        var shop = await _shopService.GetShopByCodeAsync(code, cancellationToken);
        if (shop == null)
            return NotFound($"Shop with code '{code}' not found");

        return Ok(shop);
    }

    [HttpGet("main")]
    public async Task<ActionResult<Shop>> GetMainShop(CancellationToken cancellationToken)
    {
        var shop = await _shopService.GetMainShopAsync(cancellationToken);
        if (shop == null)
            return NotFound("Main shop not found");

        return Ok(shop);
    }

    [HttpPost]
    public async Task<ActionResult<Shop>> CreateShop([FromBody] Shop shop, CancellationToken cancellationToken)
    {
        var createdShop = await _shopService.CreateShopAsync(shop, cancellationToken);
        return CreatedAtAction(nameof(GetShop), new { id = createdShop.Id }, createdShop);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Shop>> UpdateShop(Guid id, [FromBody] Shop shop, CancellationToken cancellationToken)
    {
        if (id != shop.Id)
            return BadRequest("ID mismatch");

        var updated = await _shopService.UpdateShopAsync(shop, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShop(Guid id, CancellationToken cancellationToken)
    {
        await _shopService.DeleteShopAsync(id, cancellationToken);
        return NoContent();
    }
}


