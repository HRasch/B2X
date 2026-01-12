using Wolverine;

namespace B2X.Orders.Application.Commands;

/// <summary>
/// Common DTOs for order operations
/// </summary>
public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    int Quantity,
    decimal UnitPrice
);

public record ShippingAddressDto(
    string FirstName,
    string LastName,
    string Company,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string Phone
);

public record BillingAddressDto(
    string FirstName,
    string LastName,
    string Company,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string Phone
);

/// <summary>
/// Commands for order operations
/// </summary>
public record CreateOrderCommand(
    Guid TenantId,
    Guid CustomerId,
    string CustomerEmail,
    List<OrderItemDto> Items,
    ShippingAddressDto ShippingAddress,
    BillingAddressDto? BillingAddress,
    string Currency,
    string PaymentMethod,
    string? Notes,
    string? DiscountCode
);

public record UpdateOrderStatusCommand(
    Guid Id,
    Guid TenantId,
    string Status,
    string? Notes
);

public record UpdateOrderPaymentStatusCommand(
    Guid Id,
    Guid TenantId,
    string PaymentStatus,
    string? TransactionId,
    string? PaymentProvider
);

public record CancelOrderCommand(
    Guid Id,
    Guid TenantId,
    string Reason
);

public record AddOrderItemCommand(
    Guid OrderId,
    Guid TenantId,
    OrderItemDto Item
);

public record UpdateOrderItemCommand(
    Guid OrderId,
    Guid OrderItemId,
    Guid TenantId,
    int Quantity,
    decimal UnitPrice
);

public record RemoveOrderItemCommand(
    Guid OrderId,
    Guid OrderItemId,
    Guid TenantId
);

/// <summary>
/// Queries for order operations
/// </summary>
public record GetOrderByIdQuery(Guid Id, Guid TenantId);

public record GetOrdersByTenantQuery(
    Guid TenantId,
    string? Status = null,
    string? PaymentStatus = null,
    Guid? CustomerId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20
);

public record GetOrdersByCustomerQuery(
    Guid CustomerId,
    Guid TenantId,
    int Page = 1,
    int PageSize = 20
);

public record GetOrderItemsQuery(
    Guid OrderId,
    Guid TenantId
);
