using Microsoft.AspNetCore.Mvc;
using B2Connect.IdsConnectAdapter.Models;
using B2Connect.Shared.Infrastructure.ServiceClients;
using B2Connect.Domain.Search.Services;

namespace B2Connect.IdsConnectAdapter.Controllers;

/// <summary>
/// IDS Connect 2.5 Warenkorb Controller
/// Implements Warenkorb senden/empfangen endpoints per ITEK specification
/// </summary>
[ApiController]
[Route("api/ids/warenkorb")]
[Produces("application/xml")]
public class IdsWarenkorbController : ControllerBase
{
    private readonly ICatalogServiceClient _catalogClient;
    private readonly ICustomerServiceClient _customerClient;
    private readonly ITenantResolver _tenantResolver;

    public IdsWarenkorbController(
        ICatalogServiceClient catalogClient,
        ICustomerServiceClient customerClient,
        ITenantResolver tenantResolver)
    {
        _catalogClient = catalogClient;
        _customerClient = customerClient;
        _tenantResolver = tenantResolver;
    }

    /// <summary>
    /// IDS Connect 2.5 Warenkorb Senden
    /// POST /api/ids/warenkorb/senden
    /// </summary>
    [HttpPost("senden")]
    [Consumes("application/xml")]
    public async Task<IActionResult> WarenkorbSenden([FromBody] IdsWarenkorbSenden request)
    {
        if (request == null || request.Positionen == null || !request.Positionen.Any())
        {
            return BadRequest("Invalid request: Warenkorb must contain positions");
        }

        var tenantId = _tenantResolver.ResolveTenantIdFromHost(Request.Host.Host);
        if (string.IsNullOrEmpty(tenantId) || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return BadRequest("Invalid tenant");
        }

        // Validate customer
        if (request.Kunde == null || string.IsNullOrEmpty(request.Kunde.Id))
        {
            return BadRequest("Invalid request: Customer information required");
        }

        var customer = await _customerClient.GetCustomerByErpIdAsync(request.Kunde.Id, tenantGuid);
        if (customer == null)
        {
            return NotFound($"Customer {request.Kunde.Id} not found");
        }

        // Validate and lookup products
        foreach (var position in request.Positionen)
        {
            if (string.IsNullOrEmpty(position.Artikelnummer))
            {
                return BadRequest("Invalid request: All positions must have Artikelnummer");
            }

            var product = await _catalogClient.GetProductBySkuAsync(position.Artikelnummer, tenantGuid);
            if (product == null)
            {
                return NotFound($"Product {position.Artikelnummer} not found");
            }

            // Update position with product data
            position.Bezeichnung = product.Name;
            position.Preis = product.Price;
        }

        // Generate order number (in real implementation, this would create an actual order)
        var bestellnummer = $"IDS-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";

        var response = new IdsWarenkorbEmpfangen
        {
            Version = "2.5",
            Bestellnummer = bestellnummer,
            Status = "OK"
        };

        return Ok(response);
    }

    /// <summary>
    /// IDS Connect 2.5 Warenkorb Empfangen
    /// GET /api/ids/warenkorb/empfangen/{bestellnummer}
    /// </summary>
    [HttpGet("empfangen/{bestellnummer}")]
    public async Task<IActionResult> WarenkorbEmpfangen(string bestellnummer)
    {
        if (string.IsNullOrEmpty(bestellnummer))
        {
            return BadRequest("Bestellnummer is required");
        }

        // In real implementation, this would retrieve order status from database
        var response = new IdsWarenkorbEmpfangen
        {
            Version = "2.5",
            Bestellnummer = bestellnummer,
            Status = "OK"
        };

        return Ok(response);
    }
}