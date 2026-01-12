using B2X.Orders.Application.Commands;
using B2X.Orders.Core.Entities;
using B2X.Orders.Core.Interfaces;
using Wolverine;

namespace B2X.Orders.Application.Handlers;

/// <summary>
/// Unified command handler for all order operations
/// Consolidates Order and OrderItem command handling
/// </summary>
public class OrdersCommandHandler
{
    private readonly IOrdersRepository _repository;

    public OrdersCommandHandler(IOrdersRepository repository)
    {
        _repository = repository;
    }

    // Order command handlers
    public async Task<Order> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // Calculate totals
        var subtotal = command.Items.Sum(item => item.Quantity * item.UnitPrice);
        var totalAmount = subtotal; // Add tax, shipping, discounts later

        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            CustomerId = command.CustomerId,
            CustomerEmail = command.CustomerEmail,
            Status = "pending",
            Currency = command.Currency,
            Subtotal = subtotal,
            TaxAmount = 0, // TODO: Calculate tax
            DiscountAmount = 0, // TODO: Apply discount code
            ShippingAmount = 0, // TODO: Calculate shipping
            TotalAmount = totalAmount,
            PaymentMethod = command.PaymentMethod,
            PaymentStatus = "pending",
            ShippingFirstName = command.ShippingAddress.FirstName,
            ShippingLastName = command.ShippingAddress.LastName,
            ShippingCompany = command.ShippingAddress.Company,
            ShippingStreet = command.ShippingAddress.Street,
            ShippingCity = command.ShippingAddress.City,
            ShippingPostalCode = command.ShippingAddress.PostalCode,
            ShippingCountry = command.ShippingAddress.Country,
            ShippingPhone = command.ShippingAddress.Phone,
            BillingFirstName = command.BillingAddress?.FirstName ?? command.ShippingAddress.FirstName,
            BillingLastName = command.BillingAddress?.LastName ?? command.ShippingAddress.LastName,
            BillingCompany = command.BillingAddress?.Company ?? command.ShippingAddress.Company,
            BillingStreet = command.BillingAddress?.Street ?? command.ShippingAddress.Street,
            BillingCity = command.BillingAddress?.City ?? command.ShippingAddress.City,
            BillingPostalCode = command.BillingAddress?.PostalCode ?? command.ShippingAddress.PostalCode,
            BillingCountry = command.BillingAddress?.Country ?? command.ShippingAddress.Country,
            BillingPhone = command.BillingAddress?.Phone ?? command.ShippingAddress.Phone,
            Notes = command.Notes,
            DiscountCode = command.DiscountCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create order items
        var orderItems = command.Items.Select(item => new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            ProductSku = item.ProductSku,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.Quantity * item.UnitPrice,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        // Save order and items
        await _repository.AddOrderAsync(order, cancellationToken);
        await _repository.AddOrderItemsAsync(orderItems, cancellationToken);

        // Reload order with items
        var createdOrder = await _repository.GetOrderByIdAsync(order.Id, command.TenantId, cancellationToken);
        return createdOrder!;
    }

    public async Task<Order> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderByIdAsync(command.Id, command.TenantId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.Id} not found");

        order.Status = command.Status;
        order.Notes = command.Notes;
        order.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOrderAsync(order, cancellationToken);
        return order;
    }

    public async Task<Order> Handle(UpdateOrderPaymentStatusCommand command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderByIdAsync(command.Id, command.TenantId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.Id} not found");

        order.PaymentStatus = command.PaymentStatus;
        order.TransactionId = command.TransactionId;
        order.PaymentProvider = command.PaymentProvider;
        order.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOrderAsync(order, cancellationToken);
        return order;
    }

    public async Task Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderByIdAsync(command.Id, command.TenantId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.Id} not found");

        if (order.Status == "shipped" || order.Status == "delivered")
            throw new InvalidOperationException("Cannot cancel order that has been shipped or delivered");

        order.Status = "cancelled";
        order.Notes = $"Cancelled: {command.Reason}";
        order.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOrderAsync(order, cancellationToken);
    }

    // Order item command handlers
    public async Task<OrderItem> Handle(AddOrderItemCommand command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderByIdAsync(command.OrderId, command.TenantId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.OrderId} not found");

        if (order.Status != "pending")
            throw new InvalidOperationException("Cannot add items to order that is not pending");

        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = command.OrderId,
            ProductId = command.Item.ProductId,
            ProductName = command.Item.ProductName,
            ProductSku = command.Item.ProductSku,
            Quantity = command.Item.Quantity,
            UnitPrice = command.Item.UnitPrice,
            TotalPrice = command.Item.Quantity * command.Item.UnitPrice,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddOrderItemAsync(orderItem, cancellationToken);

        // Update order totals
        await UpdateOrderTotals(order, cancellationToken);

        return orderItem;
    }

    public async Task<OrderItem> Handle(UpdateOrderItemCommand command, CancellationToken cancellationToken)
    {
        var orderItem = await _repository.GetOrderItemByIdAsync(command.OrderItemId, command.TenantId, cancellationToken);
        if (orderItem == null)
            throw new KeyNotFoundException($"Order item {command.OrderItemId} not found");

        var order = await _repository.GetOrderByIdAsync(command.OrderId, command.TenantId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.OrderId} not found");

        if (order.Status != "pending")
            throw new InvalidOperationException("Cannot update items in order that is not pending");

        orderItem.Quantity = command.Quantity;
        orderItem.UnitPrice = command.UnitPrice;
        orderItem.TotalPrice = command.Quantity * command.UnitPrice;

        await _repository.UpdateOrderItemAsync(orderItem, cancellationToken);

        // Update order totals
        await UpdateOrderTotals(order, cancellationToken);

        return orderItem;
    }

    public async Task Handle(RemoveOrderItemCommand command, CancellationToken cancellationToken)
    {
        var orderItem = await _repository.GetOrderItemByIdAsync(command.OrderItemId, command.TenantId, cancellationToken);
        if (orderItem == null)
            throw new KeyNotFoundException($"Order item {command.OrderItemId} not found");

        var order = await _repository.GetOrderByIdAsync(command.OrderId, command.TenantId, cancellationToken);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.OrderId} not found");

        if (order.Status != "pending")
            throw new InvalidOperationException("Cannot remove items from order that is not pending");

        await _repository.DeleteOrderItemAsync(command.OrderItemId, command.TenantId, cancellationToken);

        // Update order totals
        await UpdateOrderTotals(order, cancellationToken);
    }

    private async Task UpdateOrderTotals(Order order, CancellationToken cancellationToken)
    {
        var items = await _repository.GetOrderItemsByOrderIdAsync(order.Id, order.TenantId, cancellationToken);
        order.Subtotal = items.Sum(item => item.TotalPrice);
        order.TotalAmount = order.Subtotal + order.TaxAmount - order.DiscountAmount + order.ShippingAmount;
        order.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOrderAsync(order, cancellationToken);
    }
}
