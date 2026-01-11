using Xunit;
using Moq;
using FluentValidation;
using B2X.Returns.Core.Entities;
using B2X.Returns.Application.Commands;
using B2X.Returns.Application.Services;
using B2X.Returns.Application.Validators;
using Microsoft.Extensions.Logging;

namespace B2X.Returns.Tests;

/// <summary>
/// Unit tests for ReturnManagementService (14-day VVVG compliance).
/// 
/// Test scenarios:
/// 1. Valid return within 14-day window → Success
/// 2. Return outside 14-day window → Rejected
/// 3. Invalid input (missing fields) → Validation error
/// 4. Refund amount exceeds original → Validation error
/// 5. Tenant isolation (different tenant cannot access) → Blocked
/// </summary>
public class ReturnManagementServiceTests : IAsyncLifetime
{
    private readonly Mock<ILogger<ReturnManagementService>> _mockLogger;
    private readonly CreateReturnValidator _validator;
    private readonly ReturnManagementService _service;

    // Test data
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _orderId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public ReturnManagementServiceTests()
    {
        _mockLogger = new Mock<ILogger<ReturnManagementService>>();
        _validator = new CreateReturnValidator();
        _service = new ReturnManagementService(_validator, _mockLogger.Object);
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    // ═══════════════════════════════════════════════════════════════════════════════
    // ✅ POSITIVE TESTS (Should Succeed)
    // ═══════════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateReturn_WithinDeadline_SuccessfullyInitiates()
    {
        // Arrange: Order delivered 5 days ago (within 14-day window)
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Product defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal("INITIATED", response.Status);
        Assert.NotEqual(Guid.Empty, response.ReturnId);
        Assert.Equal(99.99m, response.RefundAmount);
        Assert.True(response.DaysRemaining > 0);
        Assert.True(response.DaysRemaining <= 14);
    }

    [Fact]
    public async Task CreateReturn_FirstDayOfReturn_SuccessfullyInitiates()
    {
        // Arrange: Order delivered today (day 0 of 14)
        var deliveryDate = DateTime.UtcNow.Date;
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 3,
            Reason = "Wrong items sent",
            OriginalOrderAmount = 250.00m,
            RefundAmount = 250.00m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(14, response.DaysRemaining);  // 14 days remaining on day 0
    }

    [Fact]
    public async Task CreateReturn_LastDayOfReturn_SuccessfullyInitiates()
    {
        // Arrange: Order delivered 13 days ago (last valid day is tomorrow)
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-13);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Size doesn't fit",
            OriginalOrderAmount = 49.99m,
            RefundAmount = 49.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(1, response.DaysRemaining);  // 1 day remaining
    }

    [Fact]
    public async Task CreateReturn_WithShippingDeduction_CalculatesCorrectly()
    {
        // Arrange: Full refund minus shipping cost
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-3);
        decimal originalAmount = 99.99m;
        decimal refundAmount = 85.00m;  // After shipping deduction
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Changed mind",
            OriginalOrderAmount = originalAmount,
            RefundAmount = refundAmount,
            ShippingDeduction = originalAmount - refundAmount
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(refundAmount, response.RefundAmount);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // ❌ NEGATIVE TESTS (Should Fail)
    // ═══════════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateReturn_OutsideDeadline_Rejected()
    {
        // Arrange: Order delivered 15 days ago (OUTSIDE 14-day window)
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-15);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("REJECTED", response.Status);
        Assert.Contains("VVVG requires returns within 14 days", response.Message);
        Assert.Equal(0, response.DaysRemaining);
    }

    [Fact]
    public async Task CreateReturn_MissingReason_ValidationFails()
    {
        // Arrange: Empty reason field
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = string.Empty,  // INVALID
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("Validation failed", response.Message);
    }

    [Fact]
    public async Task CreateReturn_ZeroItems_ValidationFails()
    {
        // Arrange: No items being returned
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 0,  // INVALID
            Reason = "Defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("At least 1 item", response.Message);
    }

    [Fact]
    public async Task CreateReturn_RefundExceedsOriginal_ValidationFails()
    {
        // Arrange: Refund amount > original order amount
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 150.00m,  // INVALID: exceeds original
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("cannot exceed original order amount", response.Message);
    }

    [Fact]
    public async Task CreateReturn_NegativeShippingDeduction_ValidationFails()
    {
        // Arrange: Negative shipping deduction (should be >= 0)
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = -10.00m  // INVALID
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("cannot be negative", response.Message);
    }

    // ═══════════════════════════════════════════════════════════════════════════════
    // 🔐 SECURITY TESTS
    // ═══════════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateReturn_MultiTenantIsolation_TenantIdRequired()
    {
        // Arrange: Missing TenantId
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = Guid.Empty,  // INVALID: Missing tenant
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = _userId,
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("Tenant ID is required", response.Message);
    }

    [Fact]
    public async Task CreateReturn_AuditTrail_CreatedByRequired()
    {
        // Arrange: Missing user audit trail
        var deliveryDate = DateTime.UtcNow.Date.AddDays(-5);
        var cmd = new CreateReturnCommand
        {
            TenantId = _tenantId,
            OrderId = _orderId,
            CustomerId = _customerId,
            CreatedBy = Guid.Empty,  // INVALID: No audit trail
            DeliveryDate = deliveryDate,
            ItemsCount = 1,
            Reason = "Defective",
            OriginalOrderAmount = 99.99m,
            RefundAmount = 99.99m,
            ShippingDeduction = 0
        };

        // Act
        var response = await _service.CreateReturn(cmd, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("Creator ID is required", response.Message);
    }
}
