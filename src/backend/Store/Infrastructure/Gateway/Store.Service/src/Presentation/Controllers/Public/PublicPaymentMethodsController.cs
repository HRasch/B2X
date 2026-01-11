using Microsoft.AspNetCore.Mvc;
using B2X.Store.Application.Store.ReadServices;
using Microsoft.Extensions.Logging;
using B2X.Store.Core.Common.Entities;
using Microsoft.Extensions.Logging;
using B2X.Store.Core.Store.Entities;
using Microsoft.Extensions.Logging;

namespace B2X.Store.Presentation.Controllers.Public;

[ApiController]
[Route("api/public/payment-methods")]
public class PublicPaymentMethodsController : ControllerBase
{
    private readonly IPaymentMethodReadService _paymentMethodReadService;
    private readonly ILogger<PublicPaymentMethodsController> _logger;

    public PublicPaymentMethodsController(IPaymentMethodReadService paymentMethodReadService, ILogger<PublicPaymentMethodsController> logger)
    {
        _paymentMethodReadService = paymentMethodReadService;
        _logger = logger;
    }

    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<ICollection<PaymentMethod>>> GetPaymentMethodsForStore(Guid storeId, CancellationToken cancellationToken)
    {
        var methods = await _paymentMethodReadService.GetActiveMethodsAsync(storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/currency/{currencyCode}")]
    public async Task<ActionResult<ICollection<PaymentMethod>>> GetPaymentMethodsByCurrency(Guid storeId, string currencyCode, CancellationToken cancellationToken)
    {
        var methods = await _paymentMethodReadService.GetByCurrencyAsync(currencyCode, storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("provider/{provider}")]
    public async Task<ActionResult<ICollection<PaymentMethod>>> GetPaymentMethodsByProvider(string provider, CancellationToken cancellationToken)
    {
        var methods = await _paymentMethodReadService.GetByProviderAsync(provider, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("providers")]
    public async Task<ActionResult<ICollection<string>>> GetAvailableProviders(CancellationToken cancellationToken)
    {
        var providers = await _paymentMethodReadService.GetAvailableProvidersAsync(cancellationToken);
        return Ok(providers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentMethod>> GetPaymentMethod(Guid id, CancellationToken cancellationToken)
    {
        var method = await _paymentMethodReadService.GetPaymentMethodByIdAsync(id, cancellationToken);
        if (method == null)
            return NotFound($"Payment method with ID '{id}' not found");

        return Ok(method);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<PaymentMethod>> GetPaymentMethodByCode(string code, CancellationToken cancellationToken)
    {
        var method = await _paymentMethodReadService.GetPaymentMethodByCodeAsync(code, cancellationToken);
        if (method == null)
            return NotFound($"Payment method with code '{code}' not found");

        return Ok(method);
    }

    [HttpGet("store/{storeId}/count")]
    public async Task<ActionResult<int>> GetPaymentMethodCount(Guid storeId, CancellationToken cancellationToken)
    {
        var count = await _paymentMethodReadService.GetPaymentMethodCountAsync(storeId, cancellationToken);
        return Ok(count);
    }
}


