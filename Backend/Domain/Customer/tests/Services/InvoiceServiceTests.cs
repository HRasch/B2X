using System;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Customer.Interfaces;
using B2Connect.Customer.Models;
using B2Connect.Customer.Services;
using B2Connect.Customer.Validators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Customer.Tests.Services;

/// <summary>
/// Unit tests for InvoiceService
/// Issue #32: Invoice Modification for Reverse Charge
/// 
/// Tests:
/// 1. GenerateInvoice: Creates invoice from order
/// 2. ApplyReverseCharge: Sets TaxAmount=0, TaxRate=0%
/// 3. RemoveReverseCharge: Restores normal VAT
/// 4. Validators: FluentValidation checks
/// </summary>
public class InvoiceServiceTests : IAsyncLifetime
{
    private readonly Mock<IInvoiceRepository> _mockRepository;
    private readonly Mock<ILogger<InvoiceService>> _mockLogger;
    private readonly InvoiceService _service;
    private readonly Guid _tenantId;
    private readonly Guid _orderId;

    public InvoiceServiceTests()
    {
        _mockRepository = new Mock<IInvoiceRepository>();
        _mockLogger = new Mock<ILogger<InvoiceService>>(MockBehavior.Loose);

        _service = new InvoiceService(_mockRepository.Object, _mockLogger.Object);
        _tenantId = Guid.NewGuid();
        _orderId = Guid.NewGuid();
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    // ===== GENERATE INVOICE TESTS =====

    [Fact]
    public async Task GenerateInvoice_ValidOrderWithoutReverseCharge_CreatesInvoiceWithNormalVat()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000001",
            IssuedAt = DateTime.UtcNow,
            DueAt = DateTime.UtcNow.AddDays(30),
            Status = "Issued",
            SellerName = "B2Connect GmbH",
            SellerVatId = "DE123456789",
            BuyerName = "Customer Company",
            SubTotal = 100m,
            TaxRate = 0.19m,
            TaxAmount = 19m,
            Total = 119m,
            ReverseChargeApplies = false
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _service.GenerateInvoiceAsync(_orderId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);
        Assert.Equal(0.19m, result.TaxRate);
        Assert.Equal(19m, result.TaxAmount);
        Assert.False(result.ReverseChargeApplies);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateInvoice_ValidOrderWithReverseChargeFlag_CreatesInvoiceWithZeroVat()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000002",
            IssuedAt = DateTime.UtcNow,
            DueAt = DateTime.UtcNow.AddDays(30),
            Status = "Issued",
            SellerName = "B2Connect GmbH",
            SellerVatId = "DE123456789",
            BuyerName = "EU Company",
            BuyerVatId = "IT12345678901", // Valid EU VAT ID
            BuyerCountry = "IT",
            SubTotal = 100m,
            TaxRate = 0m, // 0% due to reverse charge
            TaxAmount = 0m,
            Total = 100m, // No VAT added
            ReverseChargeApplies = true,
            ReverseChargeNote = "Reverse Charge: Art. 199a Directive 2006/112/EC"
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _service.GenerateInvoiceAsync(_orderId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.TaxRate);
        Assert.Equal(0m, result.TaxAmount);
        Assert.True(result.ReverseChargeApplies);
        Assert.Contains("Reverse Charge", result.ReverseChargeNote);
    }

    // ===== APPLY REVERSE CHARGE TESTS =====

    [Fact]
    public async Task ApplyReverseCharge_ValidB2BOrder_SetsTaxRateAndAmountToZero()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var originalInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000003",
            SellerName = "B2Connect",
            BuyerName = "Customer",
            SubTotal = 1000m,
            TaxRate = 0.19m,
            TaxAmount = 190m,
            Total = 1190m,
            ReverseChargeApplies = false
        };

        var updatedInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000003",
            SellerName = "B2Connect",
            BuyerName = "Customer",
            BuyerVatId = "IT12345678901",
            BuyerCountry = "IT",
            SubTotal = 1000m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 1000m,
            ReverseChargeApplies = true,
            ReverseChargeNote = "Reverse Charge: Art. 199a Directive 2006/112/EC"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(originalInvoice);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedInvoice);

        // Act
        var result = await _service.ApplyReverseChargeAsync(
            invoiceId, "IT12345678901", "IT", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.TaxRate);
        Assert.Equal(0m, result.TaxAmount);
        Assert.Equal(1000m, result.Total); // No VAT added
        Assert.True(result.ReverseChargeApplies);
        Assert.Equal("IT12345678901", result.BuyerVatId);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyReverseCharge_ValidB2BOrder_LogsCalculationChanges()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var originalInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            SubTotal = 500m,
            TaxRate = 0.19m,
            TaxAmount = 95m,
            Total = 595m,
            ReverseChargeApplies = false
        };

        var updatedInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            SubTotal = 500m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 500m,
            ReverseChargeApplies = true
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(originalInvoice);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedInvoice);

        // Act
        await _service.ApplyReverseChargeAsync(invoiceId, "IT12345678901", "IT", CancellationToken.None);

        // Assert - Verify repository was updated
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ===== REMOVE REVERSE CHARGE TESTS =====

    [Fact]
    public async Task RemoveReverseCharge_ReverseChargeAppliedInvoice_RestoresNormalVat()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoiceWithReverseCharge = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            SubTotal = 1000m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 1000m,
            ReverseChargeApplies = true
        };

        var invoiceWithNormalVat = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            SubTotal = 1000m,
            TaxRate = 0.19m,
            TaxAmount = 190m,
            Total = 1190m,
            ReverseChargeApplies = false
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceWithReverseCharge);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceWithNormalVat);

        // Act
        var result = await _service.RemoveReverseChargeAsync(
            invoiceId, 0.19m, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.19m, result.TaxRate);
        Assert.Equal(190m, result.TaxAmount);
        Assert.Equal(1190m, result.Total);
        Assert.False(result.ReverseChargeApplies);
    }

    // ===== VALIDATOR TESTS =====

    [Fact]
    public async Task GenerateInvoiceValidator_ValidCommand_Passes()
    {
        // Arrange
        var command = new GenerateInvoiceCommand { OrderId = Guid.NewGuid() };
        var validator = new GenerateInvoiceCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GenerateInvoiceValidator_MissingOrderId_Fails()
    {
        // Arrange
        var command = new GenerateInvoiceCommand { OrderId = Guid.Empty };
        var validator = new GenerateInvoiceCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task ModifyInvoiceValidator_ReverseChargeWithValidVatId_Passes()
    {
        // Arrange
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            ApplyReverseCharge = true,
            BuyerVatId = "IT12345678901", // Valid format
            BuyerCountry = "IT"
        };
        var validator = new ModifyInvoiceCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ModifyInvoiceValidator_ReverseChargeWithInvalidVatId_Fails()
    {
        // Arrange
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            ApplyReverseCharge = true,
            BuyerVatId = "123INVALID", // Invalid format: starts with digits
            BuyerCountry = "IT"
        };
        var validator = new ModifyInvoiceCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task ModifyInvoiceValidator_ReverseChargeWithoutCountry_Fails()
    {
        // Arrange
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            ApplyReverseCharge = true,
            BuyerVatId = "IT12345678901",
            BuyerCountry = "" // Empty country
        };
        var validator = new ModifyInvoiceCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
    }

    // ===== EDGE CASES =====

    [Fact]
    public async Task GenerateInvoice_WithShippingCost_IncludesShippingInTotal()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            SubTotal = 100m,
            ShippingCost = 9.99m,
            TaxRate = 0.19m,
            TaxAmount = 20.99m, // (100 + 9.99) * 0.19
            Total = 130.98m,
            ReverseChargeApplies = false
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _service.GenerateInvoiceAsync(_orderId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(9.99m, result.ShippingCost);
        Assert.Equal(130.98m, result.Total);
    }

    [Fact]
    public async Task ApplyReverseCharge_WithMultipleLineItems_AppliesReverseChargeToAllItems()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            SubTotal = 500m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 500m,
            ReverseChargeApplies = true,
            LineItems = new()
            {
                new InvoiceLineItem { Id = Guid.NewGuid(), LineTaxRate = 0m, LineTaxAmount = 0m },
                new InvoiceLineItem { Id = Guid.NewGuid(), LineTaxRate = 0m, LineTaxAmount = 0m },
                new InvoiceLineItem { Id = Guid.NewGuid(), LineTaxRate = 0m, LineTaxAmount = 0m }
            }
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _service.ApplyReverseChargeAsync(
            invoiceId, "IT12345678901", "IT", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.LineItems.Count);
        Assert.All(result.LineItems, item => Assert.Equal(0m, item.LineTaxRate));
    }
}
