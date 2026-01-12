using B2X.Orders.Application.Commands;
using B2X.Orders.Core.Entities;
using B2X.Orders.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace B2X.Orders.Api.Controllers;

/// <summary>
/// Orders API Controller
/// Endpoints for order CRUD operations and management
/// </summary>
[ApiController]
[Route("api/v1/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersRepository _repository;
    private readonly IMessageBus _bus;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrdersRepository repository,
        IMessageBus bus,
        ILogger<OrdersController> logger)
    {
        _repository = repository;
        _bus = bus;
        _logger = logger;
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetOrderByIdAsync(id, tenantId);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Get paged orders for tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] string? status = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetOrdersByTenantAsync(
            tenantId, status, paymentStatus, customerId, fromDate, toDate, page, pageSize);

        var totalCount = await _repository.GetOrdersCountByTenantAsync(
            tenantId, status, paymentStatus, customerId, fromDate, toDate);

        return Ok(new
        {
            Items = orders,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    /// <summary>
    /// Get orders by customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(
        Guid customerId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetOrdersByCustomerAsync(customerId, tenantId, page, pageSize);
        var totalCount = await _repository.GetOrdersCountByCustomerAsync(customerId, tenantId);

        return Ok(new
        {
            Items = orders,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    /// <summary>
    /// Get order items
    /// </summary>
    [HttpGet("{orderId}/items")]
    public async Task<IActionResult> GetOrderItems(
        Guid orderId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetOrderItemsByOrderIdAsync(orderId, tenantId);
        return Ok(items);
    }

    /// <summary>
    /// Create new order
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest("Request body is required");
        }

        var command = new CreateOrderCommand(
            TenantId: tenantId,
            CustomerId: request.CustomerId,
            CustomerEmail: request.CustomerEmail,
            Items: request.Items.Select(i => new OrderItemDto(
                ProductId: i.ProductId,
                ProductName: i.ProductName,
                ProductSku: i.ProductSku,
                Quantity: i.Quantity,
                UnitPrice: i.UnitPrice)).ToList(),
            ShippingAddress: new ShippingAddressDto(
                FirstName: request.ShippingAddress.FirstName,
                LastName: request.ShippingAddress.LastName,
                Company: request.ShippingAddress.Company,
                Street: request.ShippingAddress.Street,
                City: request.ShippingAddress.City,
                PostalCode: request.ShippingAddress.PostalCode,
                Country: request.ShippingAddress.Country,
                Phone: request.ShippingAddress.Phone),
            BillingAddress: request.BillingAddress != null ? new BillingAddressDto(
                FirstName: request.BillingAddress.FirstName,
                LastName: request.BillingAddress.LastName,
                Company: request.BillingAddress.Company,
                Street: request.BillingAddress.Street,
                City: request.BillingAddress.City,
                PostalCode: request.BillingAddress.PostalCode,
                Country: request.BillingAddress.Country,
                Phone: request.BillingAddress.Phone) : null,
            Currency: request.Currency,
            PaymentMethod: request.PaymentMethod,
            Notes: request.Notes,
            DiscountCode: request.DiscountCode);

        var order = await _bus.InvokeAsync<Order>(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOrderStatusCommand(
            Id: id,
            TenantId: tenantId,
            Status: request.Status,
            Notes: request.Notes);

        var order = await _bus.InvokeAsync<Order>(command, cancellationToken);
        return Ok(order);
    }

    /// <summary>
    /// Update payment status
    /// </summary>
    [HttpPatch("{id}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] UpdatePaymentStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOrderPaymentStatusCommand(
            Id: id,
            TenantId: tenantId,
            PaymentStatus: request.PaymentStatus,
            TransactionId: request.TransactionId,
            PaymentProvider: request.PaymentProvider);

        var order = await _bus.InvokeAsync<Order>(command, cancellationToken);
        return Ok(order);
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CancelOrderCommand(
            Id: id,
            TenantId: tenantId,
            Reason: request.Reason);

        await _bus.InvokeAsync(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Add item to order
    /// </summary>
    [HttpPost("{orderId}/items")]
    public async Task<IActionResult> AddItem(
        Guid orderId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] AddOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddOrderItemCommand(
            OrderId: orderId,
            TenantId: tenantId,
            Item: new OrderItemDto(
                ProductId: request.ProductId,
                ProductName: request.ProductName,
                ProductSku: request.ProductSku,
                Quantity: request.Quantity,
                UnitPrice: request.UnitPrice));

        var orderItem = await _bus.InvokeAsync<OrderItem>(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrderItems), new { orderId }, orderItem);
    }

    /// <summary>
    /// Update order item
    /// </summary>
    [HttpPut("{orderId}/items/{itemId}")]
    public async Task<IActionResult> UpdateItem(
        Guid orderId,
        Guid itemId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] UpdateOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOrderItemCommand(
            OrderId: orderId,
            OrderItemId: itemId,
            TenantId: tenantId,
            Quantity: request.Quantity,
            UnitPrice: request.UnitPrice);

        var orderItem = await _bus.InvokeAsync<OrderItem>(command, cancellationToken);
        return Ok(orderItem);
    }

    /// <summary>
    /// Remove item from order
    /// </summary>
    [HttpDelete("{orderId}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(
        Guid orderId,
        Guid itemId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveOrderItemCommand(
            OrderId: orderId,
            OrderItemId: itemId,
            TenantId: tenantId);

        await _bus.InvokeAsync(command, cancellationToken);
        return NoContent();
    }
}

// Request/Response DTOs
public record CreateOrderRequest(
    Guid CustomerId,
    string CustomerEmail,
    List<OrderItemRequest> Items,
    ShippingAddressRequest ShippingAddress,
    BillingAddressRequest? BillingAddress,
    string Currency = "EUR",
    string PaymentMethod = "card",
    string? Notes = null,
    string? DiscountCode = null);

public record OrderItemRequest(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice);

public record ShippingAddressRequest(
    string FirstName,
    string LastName,
    string Company,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string Phone);

public record BillingAddressRequest(
    string FirstName,
    string LastName,
    string Company,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string Phone);

public record UpdateOrderStatusRequest(string Status, string? Notes = null);

public record UpdatePaymentStatusRequest(
    string PaymentStatus,
    string? TransactionId = null,
    string? PaymentProvider = null);

public record CancelOrderRequest(string Reason);

public record AddOrderItemRequest(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice);

public record UpdateOrderItemRequest(int Quantity, decimal UnitPrice);
