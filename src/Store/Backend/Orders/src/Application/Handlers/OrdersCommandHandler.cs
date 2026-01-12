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
    // Order command handlers
    public static async Task<Order> Handle(CreateOrderCommand command, IOrdersRepository repository)
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
        await repository.AddOrderAsync(order);
        await repository.AddOrderItemsAsync(orderItems);

        // Reload order with items
        var createdOrder = await repository.GetOrderByIdAsync(order.Id, command.TenantId);
        return createdOrder!;
    }

    public static async Task<Order> Handle(UpdateOrderStatusCommand command, IOrdersRepository repository)
    {
        var order = await repository.GetOrderByIdAsync(command.Id, command.TenantId);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.Id} not found");

        order.Status = command.Status;
        order.Notes = command.Notes;
        order.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOrderAsync(order);
        return order;
    }

    public static async Task<Order> Handle(UpdateOrderPaymentStatusCommand command, IOrdersRepository repository)
    {
        var order = await repository.GetOrderByIdAsync(command.Id, command.TenantId);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.Id} not found");

        order.PaymentStatus = command.PaymentStatus;
        order.TransactionId = command.TransactionId;
        order.PaymentProvider = command.PaymentProvider;
        order.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOrderAsync(order);
        return order;
    }

    public static async Task Handle(CancelOrderCommand command, IOrdersRepository repository)
    {
        var order = await repository.GetOrderByIdAsync(command.Id, command.TenantId);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.Id} not found");

        if (order.Status == "shipped" || order.Status == "delivered")
            throw new InvalidOperationException("Cannot cancel order that has been shipped or delivered");

        order.Status = "cancelled";
        order.Notes = $"Cancelled: {command.Reason}";
        order.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOrderAsync(order);
    }

    // Order item command handlers
    public static async Task<OrderItem> Handle(AddOrderItemCommand command, IOrdersRepository repository)
    {
        var order = await repository.GetOrderByIdAsync(command.OrderId, command.TenantId);
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

        await repository.AddOrderItemAsync(orderItem);

        // Update order totals
        await UpdateOrderTotals(order, repository);

        return orderItem;
    }

    public static async Task<OrderItem> Handle(UpdateOrderItemCommand command, IOrdersRepository repository)
    {
        var orderItem = await repository.GetOrderItemByIdAsync(command.OrderItemId, command.TenantId);
        if (orderItem == null)
            throw new KeyNotFoundException($"Order item {command.OrderItemId} not found");

        var order = await repository.GetOrderByIdAsync(command.OrderId, command.TenantId);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.OrderId} not found");

        if (order.Status != "pending")
            throw new InvalidOperationException("Cannot update items in order that is not pending");

        orderItem.Quantity = command.Quantity;
        orderItem.UnitPrice = command.UnitPrice;
        orderItem.TotalPrice = command.Quantity * command.UnitPrice;

        await repository.UpdateOrderItemAsync(orderItem);

        // Update order totals
        await UpdateOrderTotals(order, repository);

        return orderItem;
    }

    public static async Task Handle(RemoveOrderItemCommand command, IOrdersRepository repository)
    {
        var orderItem = await repository.GetOrderItemByIdAsync(command.OrderItemId, command.TenantId);
        if (orderItem == null)
            throw new KeyNotFoundException($"Order item {command.OrderItemId} not found");

        var order = await repository.GetOrderByIdAsync(command.OrderId, command.TenantId);
        if (order == null)
            throw new KeyNotFoundException($"Order {command.OrderId} not found");

        if (order.Status != "pending")
            throw new InvalidOperationException("Cannot remove items from order that is not pending");

        await repository.DeleteOrderItemAsync(command.OrderItemId, command.TenantId);

        // Update order totals
        await UpdateOrderTotals(order, repository);
    }

    private static async Task UpdateOrderTotals(Order order, IOrdersRepository repository)
    {
        var items = await repository.GetOrderItemsByOrderIdAsync(order.Id, order.TenantId);
        order.Subtotal = items.Sum(item => item.TotalPrice);
        order.TotalAmount = order.Subtotal + order.TaxAmount - order.DiscountAmount + order.ShippingAmount;
        order.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOrderAsync(order);
    }
}
