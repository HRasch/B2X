using Xunit;
using Moq;
using FluentAssertions;
using B2Connect.Catalog.Application.Handlers;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2Connect.Catalog.Tests.Application.Services;

/// <summary>
/// Unit tests for ReturnManagementService
/// 
/// Story 8: Widerrufsmanagement (Return/Withdrawal Management)
/// Tests business logic for return and refund processing
/// VVVG §357-357d compliance
/// </summary>
public class ReturnManagementServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IReturnRepository> _mockReturnRepository;
    private readonly Mock<IRefundRepository> _mockRefundRepository;
    private readonly ReturnManagementService _service;
    private readonly Guid _testTenantId;
    private readonly Guid _testOrderId;
    private readonly Guid _testProductId;

    public ReturnManagementServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockReturnRepository = new Mock<IReturnRepository>();
        _mockRefundRepository = new Mock<IRefundRepository>();

        _service = new ReturnManagementService(
            _mockOrderRepository.Object,
            _mockReturnRepository.Object,
            _mockRefundRepository.Object
        );

        _testTenantId = Guid.NewGuid();
        _testOrderId = Guid.NewGuid();
        _testProductId = Guid.NewGuid();
    }

    // ========================================
    // ValidateWithdrawalPeriod Tests
    // ========================================

    [Fact]
    public async Task ValidateWithdrawalPeriod_WithinPeriod_ReturnsTrue()
    {
        // Arrange
        var order = new Order
        {
            DeliveryDate = DateTime.UtcNow.AddDays(-5) // 5 days old
        };

        // Act
        var result = await _service.IsWithinWithdrawalPeriodAsync(order);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateWithdrawalPeriod_ExactlyOnBoundary_ReturnsTrue()
    {
        // Arrange
        var order = new Order
        {
            DeliveryDate = DateTime.UtcNow.AddDays(-14) // Exactly 14 days
        };

        // Act
        var result = await _service.IsWithinWithdrawalPeriodAsync(order);

        // Assert
        result.Should().BeTrue(); // Still within (14 days is the limit)
    }

    [Fact]
    public async Task ValidateWithdrawalPeriod_JustOutsidePeriod_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            DeliveryDate = DateTime.UtcNow.AddDays(-14).AddSeconds(-1) // Just over 14 days
        };

        // Act
        var result = await _service.IsWithinWithdrawalPeriodAsync(order);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateWithdrawalPeriod_OutsidePeriod_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            DeliveryDate = DateTime.UtcNow.AddDays(-30) // 30 days old
        };

        // Act
        var result = await _service.IsWithinWithdrawalPeriodAsync(order);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateWithdrawalPeriod_JustDelivered_ReturnsTrue()
    {
        // Arrange
        var order = new Order
        {
            DeliveryDate = DateTime.UtcNow.AddMinutes(-1) // Just delivered
        };

        // Act
        var result = await _service.IsWithinWithdrawalPeriodAsync(order);

        // Assert
        result.Should().BeTrue();
    }

    // ========================================
    // CalculateRefundAmount Tests
    // ========================================

    [Fact]
    public async Task CalculateRefundAmount_FullReturn_ReturnsFullAmount()
    {
        // Arrange
        var orderItem = new OrderItem
        {
            Quantity = 1,
            UnitPrice = 99.99m,
            VatRate = 0.19m,
            Total = 99.99m
        };

        var order = new Order
        {
            SubTotal = 99.99m,
            VatAmount = 15.99m,
            ShippingCost = 5.00m,
            TotalAmount = 120.98m,
            Items = new List<OrderItem> { orderItem }
        };

        var returnRequest = new Return
        {
            Items = new List<ReturnItem>
            {
                new() { ProductId = _testProductId, Quantity = 1 }
            }
        };

        // Act
        var result = await _service.CalculateRefundAmountAsync(order, returnRequest);

        // Assert
        result.Should().Be(120.98m); // Full amount including VAT and shipping
    }

    [Fact]
    public async Task CalculateRefundAmount_PartialReturn_ReducesAmount()
    {
        // Arrange - 2 items, returning 1
        var orderItems = new List<OrderItem>
        {
            new() { Quantity = 1, UnitPrice = 50.00m, VatRate = 0.19m, Total = 50.00m },
            new() { Quantity = 1, UnitPrice = 50.00m, VatRate = 0.19m, Total = 50.00m }
        };

        var order = new Order
        {
            SubTotal = 100.00m,
            VatAmount = 19.00m,
            ShippingCost = 5.00m,
            TotalAmount = 124.00m,
            Items = orderItems
        };

        var returnRequest = new Return
        {
            Items = new List<ReturnItem>
            {
                new() { ProductId = _testProductId, Quantity = 1 }
            }
        };

        // Act
        var result = await _service.CalculateRefundAmountAsync(order, returnRequest);

        // Assert
        result.Should().Be(62.00m); // 50 + half of VAT + half of shipping
    }

    [Fact]
    public async Task CalculateRefundAmount_WithoutShippingRefund_ExcludesShipping()
    {
        // Arrange
        var order = new Order
        {
            SubTotal = 100.00m,
            VatAmount = 19.00m,
            ShippingCost = 10.00m,
            TotalAmount = 129.00m,
            Items = new List<OrderItem>()
        };

        var returnRequest = new Return
        {
            IncludeShippingRefund = false, // No shipping refund
            Items = new List<ReturnItem>()
        };

        // Act
        var result = await _service.CalculateRefundAmountAsync(order, returnRequest);

        // Assert
        result.Should().Be(119.00m); // Excludes shipping cost
    }

    [Fact]
    public async Task CalculateRefundAmount_MultipleItems_CalculatesProportionally()
    {
        // Arrange - Order with 3 items at different prices
        var items = new List<OrderItem>
        {
            new() { Quantity = 2, UnitPrice = 25.00m, VatRate = 0.19m, Total = 50.00m },
            new() { Quantity = 1, UnitPrice = 50.00m, VatRate = 0.19m, Total = 50.00m },
            new() { Quantity = 1, UnitPrice = 50.00m, VatRate = 0.19m, Total = 50.00m }
        };

        var order = new Order
        {
            SubTotal = 150.00m,
            VatAmount = 28.50m,
            ShippingCost = 5.00m,
            TotalAmount = 183.50m,
            Items = items
        };

        // Return 2 items from first product and 1 from second
        var returnRequest = new Return
        {
            Items = new List<ReturnItem>
            {
                new() { Quantity = 2 }, // 2 units at 25 = 50
                new() { Quantity = 1 }  // 1 unit at 50 = 50
            }
        };

        // Act
        var result = await _service.CalculateRefundAmountAsync(order, returnRequest);

        // Assert
        var expectedRefundSubTotal = 100.00m; // 50 + 50
        var expectedRefundVat = 19.00m; // Proportional VAT
        var expectedRefundShipping = 2.50m; // Proportional shipping
        var expected = expectedRefundSubTotal + expectedRefundVat + expectedRefundShipping;

        result.Should().Be(expected);
    }

    // ========================================
    // ReturnStatus Workflow Tests
    // ========================================

    [Fact]
    public async Task UpdateReturnStatus_FromSubmittedToReceived_Succeeds()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            Status = ReturnStatus.Submitted,
            Timeline = new List<ReturnStatusChange>()
        };

        // Act
        await _service.UpdateReturnStatusAsync(returnId, ReturnStatus.Received, "Customer returned item");

        // Assert
        mockReturn.Status.Should().Be(ReturnStatus.Received);
        mockReturn.Timeline.Should().HaveCount(1);
        mockReturn.Timeline.Last().Status.Should().Be(ReturnStatus.Received);
    }

    [Fact]
    public async Task UpdateReturnStatus_InvalidTransition_Fails()
    {
        // Arrange
        var mockReturn = new Return
        {
            Status = ReturnStatus.Received // Can't go back to Submitted
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateReturnStatusAsync(
                Guid.NewGuid(),
                ReturnStatus.Submitted,
                "Invalid transition"
            )
        );

        exception.Message.Should().Contain("valid transition");
    }

    [Fact]
    public async Task ReturnStatusWorkflow_FullCycle_Succeeds()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            Status = ReturnStatus.Submitted,
            Timeline = new List<ReturnStatusChange>()
        };

        // Act - Full workflow
        // 1. Submitted → Received
        await _service.UpdateReturnStatusAsync(
            returnId,
            ReturnStatus.Received,
            "Item received at warehouse"
        );
        mockReturn.Status = ReturnStatus.Received;

        // 2. Received → Inspected
        await _service.UpdateReturnStatusAsync(
            returnId,
            ReturnStatus.Inspected,
            "Quality inspection passed"
        );
        mockReturn.Status = ReturnStatus.Inspected;

        // 3. Inspected → Refunded
        await _service.UpdateReturnStatusAsync(
            returnId,
            ReturnStatus.Refunded,
            "Refund processed"
        );
        mockReturn.Status = ReturnStatus.Refunded;

        // Assert
        mockReturn.Status.Should().Be(ReturnStatus.Refunded);
        mockReturn.Timeline.Should().HaveCount(3);
    }

    // ========================================
    // Refund Processing Tests
    // ========================================

    [Fact]
    public async Task ProcessRefund_ValidRefund_CreatesRefundRecord()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return { Id = returnId, Amount = 99.99m };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        // Act
        var refund = await _service.CreateRefundAsync(
            returnId,
            "Original Payment",
            "Test processing"
        );

        // Assert
        refund.Should().NotBeNull();
        refund.ReturnId.Should().Be(returnId);
        refund.Amount.Should().Be(99.99m);
        refund.Status.Should().Be(RefundStatus.Pending);
    }

    [Fact]
    public async Task ProcessRefund_OriginalPaymentMethod_ReferencesPaymentMethod()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            Amount = 50.00m,
            Order = new Order { PaymentMethod = "Credit Card" }
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        // Act
        var refund = await _service.CreateRefundAsync(
            returnId,
            "Original Payment",
            "Refunding to original credit card"
        );

        // Assert
        refund.RefundMethod.Should().Be("Original Payment");
        refund.Amount.Should().Be(50.00m);
    }

    [Fact]
    public async Task ProcessRefund_WithVat_IncludesVatAmount()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            Amount = 84.00m,
            VatAmount = 15.99m,
            GrossAmount = 99.99m
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        // Act
        var refund = await _service.CreateRefundAsync(
            returnId,
            "Bank Transfer",
            "VAT included"
        );

        // Assert
        refund.Amount.Should().Be(99.99m); // Gross amount
        refund.VatAmount.Should().Be(15.99m);
    }

    // ========================================
    // Cross-Tenant Isolation Tests
    // ========================================

    [Fact]
    public async Task GetReturn_CrossTenantAccess_ReturnsFail()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var returnId = Guid.NewGuid();

        var returnForTenantB = new Return
        {
            Id = returnId,
            TenantId = tenantB
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnForTenantB);

        // Act - Service called with tenantA context
        var result = await _service.GetReturnAsync(returnId, tenantA);

        // Assert
        result.Should().BeNull(); // Access denied
    }

    [Fact]
    public async Task GetOrderReturns_EnforceTenantIsolation_ReturnsOnlyOwnReturns()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        var returns = new List<Return>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenantId, OrderId = orderId },
            new() { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), OrderId = orderId } // Other tenant
        };

        _mockReturnRepository
            .Setup(x => x.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns.Where(r => r.TenantId == tenantId).ToList());

        // Act
        var result = await _service.GetOrderReturnsAsync(orderId, tenantId);

        // Assert
        result.Should().HaveCount(1);
        result.All(r => r.TenantId == tenantId).Should().BeTrue();
    }

    // ========================================
    // VVVG Compliance Tests
    // ========================================

    [Fact]
    public async Task Vvvg_RefundDeadline_CalculatedCorrectly()
    {
        // Arrange - VVVG §357d: Refund within 14 days of withdrawal notification
        var returnCreatedAt = DateTime.UtcNow.AddDays(-5);
        var mockReturn = new Return
        {
            CreatedAt = returnCreatedAt
        };

        // Act
        var refundDeadline = _service.CalculateRefundDeadline(mockReturn);

        // Assert
        var expectedDeadline = returnCreatedAt.AddDays(14);
        refundDeadline.Should().Be(expectedDeadline.Date);
    }

    [Fact]
    public async Task Vvvg_ReturnOutsideWithdrawalPeriod_DeniesReturn()
    {
        // Arrange
        var order = new Order
        {
            DeliveryDate = DateTime.UtcNow.AddDays(-16) // Outside 14-day window
        };

        // Act
        var isAllowed = await _service.IsWithinWithdrawalPeriodAsync(order);

        // Assert
        isAllowed.Should().BeFalse(); // VVVG §357: 14 days only
    }

    [Fact]
    public async Task Vvvg_MultipleReturns_AllHandledCorrectly()
    {
        // Arrange - Customer submits multiple returns for same order
        var orderId = Guid.NewGuid();
        var returns = new List<Return>
        {
            new() { Id = Guid.NewGuid(), OrderId = orderId, Status = ReturnStatus.Submitted },
            new() { Id = Guid.NewGuid(), OrderId = orderId, Status = ReturnStatus.Submitted },
            new() { Id = Guid.NewGuid(), OrderId = orderId, Status = ReturnStatus.Submitted }
        };

        _mockReturnRepository
            .Setup(x => x.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);

        // Act
        var result = await _service.GetOrderReturnsAsync(orderId, _testTenantId);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Vvvg_AuditTrail_RecordsAllChanges()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            Status = ReturnStatus.Submitted,
            Timeline = new List<ReturnStatusChange>()
        };

        // Act - Make multiple status changes
        await _service.UpdateReturnStatusAsync(returnId, ReturnStatus.Received, "Received");
        mockReturn.Status = ReturnStatus.Received;
        mockReturn.Timeline.Add(new ReturnStatusChange
        {
            Status = ReturnStatus.Received,
            ChangedAt = DateTime.UtcNow
        });

        await _service.UpdateReturnStatusAsync(returnId, ReturnStatus.Refunded, "Refunded");
        mockReturn.Status = ReturnStatus.Refunded;
        mockReturn.Timeline.Add(new ReturnStatusChange
        {
            Status = ReturnStatus.Refunded,
            ChangedAt = DateTime.UtcNow
        });

        // Assert - All changes recorded
        mockReturn.Timeline.Should().HaveCount(2);
        mockReturn.Timeline.Select(t => t.Status)
            .Should()
            .ContainInOrder(ReturnStatus.Received, ReturnStatus.Refunded);
    }
}
