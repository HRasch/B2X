using B2X.Orders.Core.Entities;
using Shouldly;
using Xunit;

namespace B2X.Orders.Tests.Entities;

public class OrderItemTests
{
    #region OrderItem Entity Tests

    [Fact]
    public void OrderItem_NewInstance_ShouldHaveDefaultValues()
    {
        // Act
        var orderItem = new OrderItem();

        // Assert
        orderItem.Id.ShouldNotBe(Guid.Empty);
        orderItem.ProductName.ShouldBe(string.Empty);
        orderItem.ProductSku.ShouldBe(string.Empty);
        orderItem.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public void OrderItem_WithOrderId_ShouldStoreCorrectly()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var orderItem = new OrderItem
        {
            OrderId = orderId
        };

        // Assert
        orderItem.OrderId.ShouldBe(orderId);
    }

    [Fact]
    public void OrderItem_WithProductId_ShouldStoreCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var orderItem = new OrderItem
        {
            ProductId = productId
        };

        // Assert
        orderItem.ProductId.ShouldBe(productId);
    }

    [Fact]
    public void OrderItem_WithProductDetails_ShouldStoreCorrectly()
    {
        // Act
        var orderItem = new OrderItem
        {
            ProductName = "Test Product",
            ProductSku = "SKU-001"
        };

        // Assert
        orderItem.ProductName.ShouldBe("Test Product");
        orderItem.ProductSku.ShouldBe("SKU-001");
    }

    [Fact]
    public void OrderItem_WithQuantityAndPrices_ShouldStoreCorrectly()
    {
        // Act
        var orderItem = new OrderItem
        {
            Quantity = 3,
            UnitPrice = 29.99m,
            TotalPrice = 89.97m
        };

        // Assert
        orderItem.Quantity.ShouldBe(3);
        orderItem.UnitPrice.ShouldBe(29.99m);
        orderItem.TotalPrice.ShouldBe(89.97m);
    }

    [Fact]
    public void OrderItem_TotalPrice_ShouldBeQuantityTimesUnitPrice()
    {
        // Arrange
        var quantity = 5;
        var unitPrice = 19.99m;
        var expectedTotal = quantity * unitPrice;

        // Act
        var orderItem = new OrderItem
        {
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = expectedTotal
        };

        // Assert
        orderItem.TotalPrice.ShouldBe(expectedTotal);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void OrderItem_WithVariousQuantities_ShouldStoreCorrectly(int quantity)
    {
        // Act
        var orderItem = new OrderItem
        {
            Quantity = quantity
        };

        // Assert
        orderItem.Quantity.ShouldBe(quantity);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1.00)]
    [InlineData(99.99)]
    [InlineData(9999.99)]
    public void OrderItem_WithVariousUnitPrices_ShouldStoreCorrectly(decimal unitPrice)
    {
        // Act
        var orderItem = new OrderItem
        {
            UnitPrice = unitPrice
        };

        // Assert
        orderItem.UnitPrice.ShouldBe(unitPrice);
    }

    #endregion

    #region Order-OrderItem Relationship Tests

    [Fact]
    public void Order_WithOrderItems_ShouldMaintainRelationship()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001"
        };

        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductName = "Test Product",
            Quantity = 2,
            UnitPrice = 25.00m,
            TotalPrice = 50.00m
        };

        // Act
        order.Items.Add(orderItem);

        // Assert
        order.Items.Count.ShouldBe(1);
        order.Items.First().OrderId.ShouldBe(order.Id);
    }

    [Fact]
    public void Order_WithMultipleItems_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid()
        };

        var item1 = new OrderItem
        {
            OrderId = order.Id,
            Quantity = 2,
            UnitPrice = 25.00m,
            TotalPrice = 50.00m
        };

        var item2 = new OrderItem
        {
            OrderId = order.Id,
            Quantity = 1,
            UnitPrice = 30.00m,
            TotalPrice = 30.00m
        };

        order.Items.Add(item1);
        order.Items.Add(item2);

        // Act
        var expectedSubtotal = order.Items.Sum(i => i.TotalPrice);

        // Assert
        expectedSubtotal.ShouldBe(80.00m);
        order.Items.Count.ShouldBe(2);
    }

    #endregion
}
