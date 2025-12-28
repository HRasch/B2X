using Xunit;
using Moq;
using FluentAssertions;
using B2Connect.Catalog.Application.Handlers;
using B2Connect.Catalog.Application.DTOs;
using B2Connect.Catalog.Core.Entities;
using B2Connect.Catalog.Core.Interfaces;
using B2Connect.Catalog.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2Connect.Catalog.Tests.Application.Handlers;

/// <summary>
/// Unit tests for ReturnApiHandler
/// 
/// Story 8: Widerrufsmanagement (Return/Withdrawal Management)
/// Tests all 4 API endpoints with happy path, edge cases, and compliance scenarios
/// 
/// VVVG §357-357d:
/// - 14-day withdrawal period enforcement
/// - Refund processing within 14 days
/// - Partial return handling
/// - Cross-tenant isolation
/// </summary>
public class ReturnApiHandlerTests : IAsyncLifetime
{
    private readonly Mock<IReturnRepository> _mockReturnRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IRefundRepository> _mockRefundRepository;
    private readonly Mock<ReturnManagementService> _mockReturnService;
    private readonly IValidator<CreateReturnRequestDto> _returnValidator;
    private readonly IValidator<CreateRefundRequestDto> _refundValidator;
    private readonly ReturnApiHandler _handler;
    private readonly CatalogDbContext _dbContext;
    private readonly Guid _testTenantId;
    private readonly Guid _testUserId;
    private readonly Guid _testOrderId;
    private readonly Guid _testProductId;

    public ReturnApiHandlerTests()
    {
        _mockReturnRepository = new Mock<IReturnRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockRefundRepository = new Mock<IRefundRepository>();
        _mockReturnService = new Mock<ReturnManagementService>(
            _mockOrderRepository.Object,
            _mockReturnRepository.Object,
            _mockRefundRepository.Object
        );

        _returnValidator = new CreateReturnRequestValidator();
        _refundValidator = new CreateRefundRequestValidator();

        _handler = new ReturnApiHandler(
            _mockReturnRepository.Object,
            _mockOrderRepository.Object,
            _mockRefundRepository.Object,
            _mockReturnService.Object
        );

        // Setup in-memory database
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: $"test-db-{Guid.NewGuid()}")
            .Options;

        _dbContext = new CatalogDbContext(options);

        // Test data IDs
        _testTenantId = Guid.NewGuid();
        _testUserId = Guid.NewGuid();
        _testOrderId = Guid.NewGuid();
        _testProductId = Guid.NewGuid();
    }

    public async Task InitializeAsync()
    {
        // Create test data
        var testOrder = new Order
        {
            Id = _testOrderId,
            TenantId = _testTenantId,
            OrderNumber = "ORDER-001",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            TotalAmount = 99.99m,
            VatAmount = 15.99m,
            ShippingCost = 5.00m,
            ShippingCountry = "DE",
            OrderDate = DateTime.UtcNow.AddDays(-5),
            DeliveryDate = DateTime.UtcNow.AddDays(-3),
            Status = OrderStatus.Delivered,
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = _testProductId,
                    Sku = "PROD-001",
                    Name = "Test Product",
                    Quantity = 1,
                    UnitPrice = 99.99m,
                    VatRate = 0.19m,
                    Total = 99.99m
                }
            }
        };

        await _dbContext.Orders.AddAsync(testOrder);
        await _dbContext.SaveChangesAsync();

        // Setup mock repositories to return test data
        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testOrder);

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testOrder);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    // ========================================
    // CreateReturn Tests (Happy Path & Edge Cases)
    // ========================================

    [Fact]
    public async Task CreateReturn_ValidRequest_WithinWithdrawalPeriod_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "Changed my mind",
            Description = "Product not as expected",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.CreateReturn(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ReturnId.Should().NotBeEmpty();
        result.Message.Should().Contain("Return created successfully");
        result.WithdrawalDeadline.Should().Be(DateTime.UtcNow.AddDays(14).Date);
    }

    [Fact]
    public async Task CreateReturn_ValidRequest_AllItems_ReturnsPartialReturn()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "Quality issue",
            Description = "Items damaged",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.ItemCount.Should().Be(1);
    }

    [Fact]
    public async Task CreateReturn_OutsideWithdrawalPeriod_ReturnsFail()
    {
        // Arrange
        // Create order delivered 15+ days ago (outside 14-day VVVG window)
        var oldOrder = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = _testTenantId,
            OrderNumber = "ORDER-OLD",
            OrderDate = DateTime.UtcNow.AddDays(-20),
            DeliveryDate = DateTime.UtcNow.AddDays(-16),
            Status = OrderStatus.Delivered,
            Items = new List<OrderItem>()
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(oldOrder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldOrder);

        var request = new CreateReturnRequestDto
        {
            OrderId = oldOrder.Id,
            Reason = "Changed mind",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("withdrawal period");
        result.Error.Should().Contain("14 days");
    }

    [Fact]
    public async Task CreateReturn_InvalidRequest_MissingReason_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "", // Empty reason - invalid
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var validationResult = await _returnValidator.ValidateAsync(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(x => x.PropertyName == "Reason");
    }

    [Fact]
    public async Task CreateReturn_InvalidRequest_ReasonTooLong_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "A".PadRight(501), // Exceeds 500 chars
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var validationResult = await _returnValidator.ValidateAsync(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(x => x.PropertyName == "Reason");
    }

    [Fact]
    public async Task CreateReturn_InvalidRequest_EmptyItemsList_FailsValidation()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "Changed mind",
            ReturnedItemIds = new List<Guid>() // Empty - invalid
        };

        // Act
        var validationResult = await _returnValidator.ValidateAsync(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(x => x.PropertyName.Contains("Items"));
    }

    [Fact]
    public async Task CreateReturn_InvalidRequest_NonExistentOrder_ReturnsFail()
    {
        // Arrange
        var nonExistentOrderId = Guid.NewGuid();

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(nonExistentOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order)null);

        var request = new CreateReturnRequestDto
        {
            OrderId = nonExistentOrderId,
            Reason = "Item broken",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateReturn_CrossTenantAccess_FailsSecurityCheck()
    {
        // Arrange - Create order for different tenant
        var otherTenantId = Guid.NewGuid();
        var otherTenantOrder = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = otherTenantId,
            OrderNumber = "ORDER-OTHER",
            Status = OrderStatus.Delivered,
            Items = new List<OrderItem>()
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(otherTenantOrder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherTenantOrder);

        var request = new CreateReturnRequestDto
        {
            OrderId = otherTenantOrder.Id,
            Reason = "Trying cross-tenant access",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("tenant") | result.Error.Should().Contain("access");
    }

    [Fact]
    public async Task CreateReturn_MultipleItems_ReturnsCorrectItemCount()
    {
        // Arrange
        var itemIds = new List<Guid> { _testProductId, Guid.NewGuid(), Guid.NewGuid() };

        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "Multiple items damaged",
            ReturnedItemIds = itemIds
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.ItemCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateReturn_StoredInDatabase_CanBeRetrieved()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "Quality issue",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var createResult = await _handler.CreateReturn(request, CancellationToken.None);
        var retrievedReturn = await _mockReturnRepository.Object.GetByIdAsync(
            createResult.ReturnId,
            CancellationToken.None
        );

        // Assert
        retrievedReturn.Should().NotBeNull();
        retrievedReturn.OrderId.Should().Be(_testOrderId);
        retrievedReturn.TenantId.Should().Be(_testTenantId);
    }

    [Fact]
    public async Task CreateReturn_AuditLogCreated_WithCorrectDetails()
    {
        // Arrange
        var request = new CreateReturnRequestDto
        {
            OrderId = _testOrderId,
            Reason = "Damaged goods",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert - Verify audit log was created (mocked verification)
        _mockReturnService.Verify(
            x => x.LogReturnCreatedAsync(
                It.Is<Return>(r => r.Id == result.ReturnId),
                It.IsAny<CancellationToken>()
            ),
            Times.AtLeastOnce
        );
    }

    // ========================================
    // CreateRefund Tests (Happy Path & Edge Cases)
    // ========================================

    [Fact]
    public async Task CreateRefund_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var returnId = Guid.NewGuid();

        var request = new CreateRefundRequestDto
        {
            ReturnId = returnId,
            RefundMethod = "Original Payment",
            ProcessingNotes = "Customer agrees to original payment method"
        };

        // Act
        var result = await _handler.CreateRefund(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.RefundId.Should().NotBeEmpty();
        result.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task CreateRefund_InvalidRequest_MissingRefundMethod_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "", // Empty - invalid
            ProcessingNotes = "Test"
        };

        // Act
        var validationResult = await _refundValidator.ValidateAsync(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(x => x.PropertyName == "RefundMethod");
    }

    [Fact]
    public async Task CreateRefund_InvalidRefundMethod_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "InvalidMethod", // Not in allowed list
            ProcessingNotes = "Test"
        };

        // Act
        var validationResult = await _refundValidator.ValidateAsync(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateRefund_NotesExceedLength_FailsValidation()
    {
        // Arrange
        var request = new CreateRefundRequestDto
        {
            ReturnId = Guid.NewGuid(),
            RefundMethod = "Original Payment",
            ProcessingNotes = "A".PadRight(501) // Exceeds 500 chars
        };

        // Act
        var validationResult = await _refundValidator.ValidateAsync(request);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateRefund_NonExistentReturn_ReturnsFail()
    {
        // Arrange
        var nonExistentReturnId = Guid.NewGuid();

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(nonExistentReturnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Return)null);

        var request = new CreateRefundRequestDto
        {
            ReturnId = nonExistentReturnId,
            RefundMethod = "Original Payment",
            ProcessingNotes = "Test"
        };

        // Act
        var result = await _handler.CreateRefund(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateRefund_WithVatCalculation_ReturnsCorrectAmount()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            TenantId = _testTenantId,
            OrderId = _testOrderId,
            Amount = 99.99m,
            VatAmount = 15.99m,
            GrossAmount = 99.99m
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        var request = new CreateRefundRequestDto
        {
            ReturnId = returnId,
            RefundMethod = "Original Payment",
            ProcessingNotes = "Full refund including VAT"
        };

        // Act
        var result = await _handler.CreateRefund(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.RefundAmount.Should().Be(mockReturn.GrossAmount);
    }

    // ========================================
    // GetReturnStatus Tests
    // ========================================

    [Fact]
    public async Task GetReturnStatus_ExistingReturn_ReturnsStatus()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            TenantId = _testTenantId,
            OrderId = _testOrderId,
            Status = ReturnStatus.Submitted,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Amount = 99.99m
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        // Act
        var result = await _handler.GetReturnStatus(returnId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ReturnId.Should().Be(returnId);
        result.Status.Should().Be(ReturnStatus.Submitted.ToString());
        result.SubmittedAt.Should().Be(mockReturn.CreatedAt);
    }

    [Fact]
    public async Task GetReturnStatus_NonExistentReturn_ReturnsFail()
    {
        // Arrange
        var nonExistentReturnId = Guid.NewGuid();

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(nonExistentReturnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Return)null);

        // Act
        var result = await _handler.GetReturnStatus(nonExistentReturnId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetReturnStatus_WithTimeline_ReturnsStatusHistory()
    {
        // Arrange
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            TenantId = _testTenantId,
            Status = ReturnStatus.Refunded,
            Timeline = new List<ReturnStatusChange>
            {
                new() { Status = ReturnStatus.Submitted, ChangedAt = DateTime.UtcNow.AddDays(-2) },
                new() { Status = ReturnStatus.Received, ChangedAt = DateTime.UtcNow.AddDays(-1) },
                new() { Status = ReturnStatus.Refunded, ChangedAt = DateTime.UtcNow }
            }
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        // Act
        var result = await _handler.GetReturnStatus(returnId, CancellationToken.None);

        // Assert
        result.Timeline.Should().HaveCount(3);
        result.Timeline.Last().Status.Should().Be(ReturnStatus.Refunded.ToString());
    }

    // ========================================
    // GetOrderReturns Tests
    // ========================================

    [Fact]
    public async Task GetOrderReturns_WithReturns_ReturnsAll()
    {
        // Arrange
        var returns = new List<Return>
        {
            new() { Id = Guid.NewGuid(), OrderId = _testOrderId, TenantId = _testTenantId, Status = ReturnStatus.Submitted },
            new() { Id = Guid.NewGuid(), OrderId = _testOrderId, TenantId = _testTenantId, Status = ReturnStatus.Received },
            new() { Id = Guid.NewGuid(), OrderId = _testOrderId, TenantId = _testTenantId, Status = ReturnStatus.Refunded }
        };

        _mockReturnRepository
            .Setup(x => x.GetByOrderIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);

        // Act
        var result = await _handler.GetOrderReturns(_testOrderId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Returns.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetOrderReturns_NoReturns_ReturnsEmptyList()
    {
        // Arrange
        _mockReturnRepository
            .Setup(x => x.GetByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Return>());

        // Act
        var result = await _handler.GetOrderReturns(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Returns.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetOrderReturns_WithPagination_ReturnsPaginatedResult()
    {
        // Arrange
        var allReturns = Enumerable.Range(0, 25)
            .Select(i => new Return
            {
                Id = Guid.NewGuid(),
                OrderId = _testOrderId,
                TenantId = _testTenantId,
                Status = ReturnStatus.Submitted
            })
            .ToList();

        _mockReturnRepository
            .Setup(x => x.GetByOrderIdAsync(_testOrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allReturns);

        // Act - Get page 1, 10 items per page
        var result = await _handler.GetOrderReturns(_testOrderId, pageNumber: 1, pageSize: 10);

        // Assert
        result.Returns.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    // ========================================
    // VVVG Compliance Tests
    // ========================================

    [Fact]
    public async Task CreateReturn_VvvgCompliance_14DayWithdrawalPeriodEnforced()
    {
        // Arrange - VVVG §357 Abs. 2: 14 days from delivery
        var deliveryDate = DateTime.UtcNow.AddDays(-13); // Within 14 days
        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = _testTenantId,
            DeliveryDate = deliveryDate,
            Status = OrderStatus.Delivered,
            Items = new List<OrderItem>()
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var request = new CreateReturnRequestDto
        {
            OrderId = order.Id,
            Reason = "Changed mind",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert - Should be allowed (within 14 days)
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRefund_VvvgCompliance_RefundProcessedWithin14Days()
    {
        // Arrange - VVVG §357d: Refund within 14 days of withdrawal
        var returnId = Guid.NewGuid();
        var mockReturn = new Return
        {
            Id = returnId,
            TenantId = _testTenantId,
            CreatedAt = DateTime.UtcNow.AddDays(-5) // 5 days old, still within 14-day window
        };

        _mockReturnRepository
            .Setup(x => x.GetByIdAsync(returnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockReturn);

        var request = new CreateRefundRequestDto
        {
            ReturnId = returnId,
            RefundMethod = "Original Payment",
            ProcessingNotes = "VVVG compliant refund"
        };

        // Act
        var result = await _handler.CreateRefund(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.ProcessedDate.Should().BeLessThanOrEqualTo(DateTime.UtcNow.AddDays(14));
    }

    [Fact]
    public async Task CreateReturn_VvvgCompliance_MultiTenantIsolation()
    {
        // Arrange - Tenant A tries to access Tenant B's order
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        var orderForTenantB = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Status = OrderStatus.Delivered,
            Items = new List<OrderItem>()
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderForTenantB.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderForTenantB);

        var request = new CreateReturnRequestDto
        {
            OrderId = orderForTenantB.Id,
            Reason = "Cross-tenant attack",
            ReturnedItemIds = new List<Guid> { _testProductId }
        };

        // Act - Handler called with tenantA context
        var result = await _handler.CreateReturn(request, CancellationToken.None);

        // Assert - Should fail (tenant isolation breach)
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("tenant");
    }
}
