using B2X.Orders.Core.Entities;
using Shouldly;
using Xunit;

namespace B2X.Orders.Tests.Entities;

public class OrderTests
{
    #region Order Entity Tests

    [Fact]
    public void Order_NewInstance_ShouldHaveDefaultValues()
    {
        // Act
        var order = new Order();

        // Assert
        order.Id.ShouldNotBe(Guid.Empty);
        order.Status.ShouldBe("pending");
        order.Currency.ShouldBe("EUR");
        order.PaymentMethod.ShouldBe("card");
        order.PaymentStatus.ShouldBe("pending");
        order.ShippingCountry.ShouldBe("DE");
        order.CreatedAt.ShouldNotBe(default);
        order.UpdatedAt.ShouldNotBe(default);
    }

    [Fact]
    public void Order_WithTenantId_ShouldStoreCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var order = new Order
        {
            TenantId = tenantId
        };

        // Assert
        order.TenantId.ShouldBe(tenantId);
    }

    [Fact]
    public void Order_WithCustomer_ShouldStoreCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var order = new Order
        {
            CustomerId = customerId
        };

        // Assert
        order.CustomerId.ShouldBe(customerId);
    }

    [Fact]
    public void Order_WithoutCustomer_ShouldAllowNull()
    {
        // Act
        var order = new Order
        {
            CustomerId = null
        };

        // Assert
        order.CustomerId.ShouldBeNull();
    }

    [Fact]
    public void Order_WithOrderNumber_ShouldStoreCorrectly()
    {
        // Arrange
        var orderNumber = "ORD-2026-001";

        // Act
        var order = new Order
        {
            OrderNumber = orderNumber
        };

        // Assert
        order.OrderNumber.ShouldBe(orderNumber);
    }

    [Fact]
    public void Order_WithAmounts_ShouldCalculateCorrectly()
    {
        // Arrange
        var order = new Order
        {
            Subtotal = 100.00m,
            TaxAmount = 19.00m,
            ShippingAmount = 5.00m,
            TotalAmount = 124.00m
        };

        // Assert
        order.Subtotal.ShouldBe(100.00m);
        order.TaxAmount.ShouldBe(19.00m);
        order.ShippingAmount.ShouldBe(5.00m);
        order.TotalAmount.ShouldBe(124.00m);
    }

    [Fact]
    public void Order_WithShippingAddress_ShouldStoreCorrectly()
    {
        // Act
        var order = new Order
        {
            ShippingFirstName = "John",
            ShippingLastName = "Doe",
            ShippingStreet = "123 Main St",
            ShippingCity = "Berlin",
            ShippingPostalCode = "10115",
            ShippingCountry = "DE"
        };

        // Assert
        order.ShippingFirstName.ShouldBe("John");
        order.ShippingLastName.ShouldBe("Doe");
        order.ShippingStreet.ShouldBe("123 Main St");
        order.ShippingCity.ShouldBe("Berlin");
        order.ShippingPostalCode.ShouldBe("10115");
        order.ShippingCountry.ShouldBe("DE");
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("confirmed")]
    [InlineData("processing")]
    [InlineData("shipped")]
    [InlineData("delivered")]
    [InlineData("cancelled")]
    public void Order_WithValidStatus_ShouldStoreCorrectly(string status)
    {
        // Act
        var order = new Order
        {
            Status = status
        };

        // Assert
        order.Status.ShouldBe(status);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("authorized")]
    [InlineData("captured")]
    [InlineData("failed")]
    [InlineData("refunded")]
    public void Order_WithValidPaymentStatus_ShouldStoreCorrectly(string paymentStatus)
    {
        // Act
        var order = new Order
        {
            PaymentStatus = paymentStatus
        };

        // Assert
        order.PaymentStatus.ShouldBe(paymentStatus);
    }

    [Theory]
    [InlineData("card")]
    [InlineData("invoice")]
    [InlineData("paypal")]
    [InlineData("sepa")]
    public void Order_WithValidPaymentMethod_ShouldStoreCorrectly(string paymentMethod)
    {
        // Act
        var order = new Order
        {
            PaymentMethod = paymentMethod
        };

        // Assert
        order.PaymentMethod.ShouldBe(paymentMethod);
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("GBP")]
    [InlineData("CHF")]
    public void Order_WithValidCurrency_ShouldStoreCorrectly(string currency)
    {
        // Act
        var order = new Order
        {
            Currency = currency
        };

        // Assert
        order.Currency.ShouldBe(currency);
    }

    [Fact]
    public void Order_Items_ShouldBeInitializedAsEmptyList()
    {
        // Act
        var order = new Order();

        // Assert
        order.Items.ShouldNotBeNull();
        order.Items.ShouldBeEmpty();
    }

    #endregion
}
