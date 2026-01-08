using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2X.Store.Application.Store.Services;
using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Store.Entities;

namespace B2X.Store.Presentation.Controllers.Admin;

[ApiController]
[Route("api/payment-methods")]
[Authorize(Roles = "Admin")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly ILogger<PaymentMethodsController> _logger;

    public PaymentMethodsController(IPaymentMethodService paymentMethodService, ILogger<PaymentMethodsController> logger)
    {
        _paymentMethodService = paymentMethodService;
        _logger = logger;
    }

    [HttpGet("store/{storeId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<PaymentMethod>>> GetPaymentMethodsForStore(Guid storeId, CancellationToken cancellationToken)
    {
        var methods = await _paymentMethodService.GetActiveMethodsAsync(storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("store/{storeId}/currency/{currencyCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<PaymentMethod>>> GetPaymentMethodsByCurrency(Guid storeId, string currencyCode, CancellationToken cancellationToken)
    {
        var methods = await _paymentMethodService.GetByCurrencyAsync(currencyCode, storeId, cancellationToken);
        return Ok(methods);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentMethod>> GetPaymentMethod(Guid id, CancellationToken cancellationToken)
    {
        var method = await _paymentMethodService.GetPaymentMethodByIdAsync(id, cancellationToken);
        if (method == null)
            return NotFound($"Payment method with ID '{id}' not found");

        return Ok(method);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentMethod>> CreatePaymentMethod([FromBody] PaymentMethod method, CancellationToken cancellationToken)
    {
        var createdMethod = await _paymentMethodService.CreatePaymentMethodAsync(method, cancellationToken);
        return CreatedAtAction(nameof(GetPaymentMethod), new { id = createdMethod.Id }, createdMethod);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PaymentMethod>> UpdatePaymentMethod(Guid id, [FromBody] PaymentMethod method, CancellationToken cancellationToken)
    {
        if (id != method.Id)
            return BadRequest("ID mismatch");

        var updated = await _paymentMethodService.UpdatePaymentMethodAsync(method, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaymentMethod(Guid id, CancellationToken cancellationToken)
    {
        await _paymentMethodService.DeletePaymentMethodAsync(id, cancellationToken);
        return NoContent();
    }
}


