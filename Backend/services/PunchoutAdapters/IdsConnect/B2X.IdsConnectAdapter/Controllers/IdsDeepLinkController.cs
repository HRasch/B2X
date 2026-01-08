using B2X.Domain.Search.Services;
using B2X.IdsConnectAdapter.Models;
using B2X.Shared.Infrastructure.ServiceClients;
using Microsoft.AspNetCore.Mvc;

namespace B2X.IdsConnectAdapter.Controllers;

/// <summary>
/// IDS Connect 2.5 Deep-Link Controller
/// Implements Artikel-Deep-Link functionality per ITEK specification
/// </summary>
[ApiController]
[Route("api/ids/deeplink")]
[Produces("application/xml")]
public class IdsDeepLinkController : ControllerBase
{
    private readonly ICatalogServiceClient _catalogClient;
    private readonly ITenantResolver _tenantResolver;

    public IdsDeepLinkController(
        ICatalogServiceClient catalogClient,
        ITenantResolver tenantResolver)
    {
        _catalogClient = catalogClient;
        _tenantResolver = tenantResolver;
    }

    /// <summary>
    /// IDS Connect 2.5 Get Artikel Deep-Link
    /// GET /api/ids/deeplink/artikel/{artikelnummer}
    /// </summary>
    [HttpGet("artikel/{artikelnummer}")]
    public async Task<IActionResult> GetArtikelDeepLink(string artikelnummer)
    {
        if (string.IsNullOrEmpty(artikelnummer))
        {
            return BadRequest("Artikelnummer is required");
        }

        var tenantId = _tenantResolver.ResolveTenantIdFromHost(Request.Host.Host);
        if (string.IsNullOrEmpty(tenantId) || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return BadRequest("Invalid tenant");
        }

        // Lookup product
        var product = await _catalogClient.GetProductBySkuAsync(artikelnummer, tenantGuid);
        if (product == null)
        {
            return NotFound($"Product {artikelnummer} not found");
        }

        // Generate personalized deep-link
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var deepLink = $"{baseUrl}/store/product/{product.Id}?source=ids-connect";

        var response = new IdsDeepLinkResponse
        {
            Version = "2.5",
            DeepLink = deepLink,
            Status = "OK",
            ArtikelInfo = new IdsArtikelInfo
            {
                Artikelnummer = product.Sku,
                Bezeichnung = product.Name,
                Preis = product.Price,
                Waehrung = "EUR",
                Verfuegbar = product.StockLevel > 0
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// IDS Connect 2.5 Create Artikel Deep-Link
    /// POST /api/ids/deeplink/artikel
    /// </summary>
    [HttpPost("artikel")]
    [Consumes("application/xml")]
    public async Task<IActionResult> CreateArtikelDeepLink([FromBody] IdsDeepLinkRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Artikelnummer))
        {
            return BadRequest("Invalid request: Artikelnummer required");
        }

        var tenantId = _tenantResolver.ResolveTenantIdFromHost(Request.Host.Host);
        if (string.IsNullOrEmpty(tenantId) || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return BadRequest("Invalid tenant");
        }

        // Validate customer if provided
        if (request.Kunde != null && !string.IsNullOrEmpty(request.Kunde.Id))
        {
            // Customer validation could be added here if needed
        }

        // Lookup product
        var product = await _catalogClient.GetProductBySkuAsync(request.Artikelnummer, tenantGuid);
        if (product == null)
        {
            return NotFound($"Product {request.Artikelnummer} not found");
        }

        // Generate personalized deep-link
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var deepLink = $"{baseUrl}/store/product/{product.Id}?source=ids-connect";

        if (request.Kunde != null && !string.IsNullOrEmpty(request.Kunde.Id))
        {
            deepLink += $"&customer={request.Kunde.Id}";
        }

        var response = new IdsDeepLinkResponse
        {
            Version = "2.5",
            DeepLink = deepLink,
            Status = "OK",
            ArtikelInfo = new IdsArtikelInfo
            {
                Artikelnummer = product.Sku,
                Bezeichnung = product.Name,
                Preis = product.Price,
                Waehrung = "EUR",
                Verfuegbar = product.StockLevel > 0
            }
        };

        return Ok(response);
    }
}
