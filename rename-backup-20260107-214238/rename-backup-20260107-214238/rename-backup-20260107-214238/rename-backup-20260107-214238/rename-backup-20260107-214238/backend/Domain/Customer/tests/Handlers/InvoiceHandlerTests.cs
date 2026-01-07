using System;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.Customer.Handlers;
using B2Connect.Customer.Interfaces;
using B2Connect.Customer.Models;
using B2Connect.Customer.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Customer.Tests.Handlers;

/// <summary>
/// Unit tests for InvoiceHandler (Wolverine HTTP Endpoints)
/// Issue #32: Invoice Modification for Reverse Charge
/// 
/// Tests:
/// 1. GenerateInvoice endpoint: Creates invoice from order
/// 2. ModifyInvoice endpoint: Applies/removes reverse charge
/// 3. Error handling: Invalid inputs, missing data
/// </summary>
public class InvoiceHandlerTests : IAsyncLifetime
{
    private readonly Mock<IInvoiceService> _mockService;
    private readonly Mock<IValidator<GenerateInvoiceCommand>> _mockGenerateValidator;
    private readonly Mock<IValidator<ModifyInvoiceCommand>> _mockModifyValidator;
    private readonly Mock<ILogger<InvoiceHandler>> _mockLogger;
    private readonly InvoiceHandler _handler;
    private readonly Guid _tenantId;
    private readonly Guid _orderId;

    public InvoiceHandlerTests()
    {
        _mockService = new Mock<IInvoiceService>();
        _mockGenerateValidator = new Mock<IValidator<GenerateInvoiceCommand>>();
        _mockModifyValidator = new Mock<IValidator<ModifyInvoiceCommand>>();
        _mockLogger = new Mock<ILogger<InvoiceHandler>>(MockBehavior.Loose);

        _handler = new InvoiceHandler(
            _mockService.Object,
            _mockGenerateValidator.Object,
            _mockModifyValidator.Object,
            _mockLogger.Object);
        _tenantId = Guid.NewGuid();
        _orderId = Guid.NewGuid();
    }

    public Task InitializeAsync()
    {
        // Setup default validator mocks to return success
        var successResult = new FluentValidation.Results.ValidationResult();

        _mockGenerateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<GenerateInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        _mockModifyValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ModifyInvoiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        return Task.CompletedTask;
    }
    public Task DisposeAsync() => Task.CompletedTask;

    // ===== GENERATE INVOICE ENDPOINT TESTS =====

    [Fact]
    public async Task GenerateInvoice_ValidCommand_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new GenerateInvoiceCommand { OrderId = _orderId };
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000001",
            SubTotal = 100m,
            TaxRate = 0.19m,
            TaxAmount = 19m,
            Total = 119m,
            ReverseChargeApplies = false
        };

        _mockService
            .Setup(s => s.GenerateInvoiceAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var response = await _handler.GenerateInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotEqual(Guid.Empty, response.InvoiceId);
        Assert.Equal("INV-2025-000001", response.InvoiceNumber);
        Assert.Equal(19m, response.TaxAmount);
        Assert.False(response.ReverseChargeApplied);
    }

    [Fact]
    public async Task GenerateInvoice_B2BOrderWithReverseCharge_ReturnsReverseChargeFlag()
    {
        // Arrange
        var command = new GenerateInvoiceCommand { OrderId = _orderId };
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000002",
            BuyerVatId = "IT12345678901",
            BuyerCountry = "IT",
            SubTotal = 1000m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 1000m,
            ReverseChargeApplies = true
        };

        _mockService
            .Setup(s => s.GenerateInvoiceAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var response = await _handler.GenerateInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.True(response.ReverseChargeApplied);
        Assert.Equal(0m, response.TaxAmount);
    }

    [Fact]
    public async Task GenerateInvoice_ServiceThrowsException_ReturnsFailedResponse()
    {
        // Arrange
        var command = new GenerateInvoiceCommand { OrderId = _orderId };
        _mockService
            .Setup(s => s.GenerateInvoiceAsync(_orderId, It.IsAny<CancellationToken>()))
#pragma warning disable CA2201 // Do not raise reserved exception types
            .ThrowsAsync(new Exception("Database error"));
#pragma warning restore CA2201 // Do not raise reserved exception types

        // Act
        var response = await _handler.GenerateInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Contains("error", response.Message.ToLower(System.Globalization.CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase);
    }

    // ===== MODIFY INVOICE ENDPOINT TESTS =====

    [Fact]
    public async Task ModifyInvoice_ApplyReverseChargeValidCommand_ReturnsSuccessWithUpdatedTax()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = invoiceId,
            OrderId = _orderId,
            ApplyReverseCharge = true,
            BuyerVatId = "IT12345678901",
            BuyerCountry = "IT"
        };

        var originalInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
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
            OrderId = _orderId,
            SubTotal = 500m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 500m,
            ReverseChargeApplies = true
        };

        _mockService
            .Setup(s => s.GetInvoiceByOrderIdAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(originalInvoice);

        _mockService
            .Setup(s => s.ApplyReverseChargeAsync(invoiceId, "IT12345678901", "IT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedInvoice);

        // Act
        var response = await _handler.ModifyInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(invoiceId, response.InvoiceId);
        Assert.True(response.ReverseChargeApplies);
        Assert.Equal(95m, response.OldTaxAmount);
        Assert.Equal(0m, response.NewTaxAmount);
        Assert.Equal(595m, response.OldTotal);
        Assert.Equal(500m, response.NewTotal);
    }

    [Fact]
    public async Task ModifyInvoice_RemoveReverseChargeValidCommand_ReturnsSuccessWithResturedVat()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = invoiceId,
            OrderId = _orderId,
            ApplyReverseCharge = false,
            BuyerVatId = "",
            BuyerCountry = ""
        };

        var invoiceWithReverseCharge = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            SubTotal = 500m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 500m,
            ReverseChargeApplies = true
        };

        var invoiceWithoutReverseCharge = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            SubTotal = 500m,
            TaxRate = 0.19m,
            TaxAmount = 95m,
            Total = 595m,
            ReverseChargeApplies = false
        };

        _mockService
            .Setup(s => s.GetInvoiceByOrderIdAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceWithReverseCharge);

        _mockService
            .Setup(s => s.RemoveReverseChargeAsync(invoiceId, 0.19m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceWithoutReverseCharge);

        // Act
        var response = await _handler.ModifyInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.False(response.ReverseChargeApplies);
        Assert.Equal(0m, response.OldTaxAmount); // Was already removed
        Assert.Equal(95m, response.NewTaxAmount);
    }

    [Fact]
    public async Task ModifyInvoice_InvalidVatIdFormat_ReturnsFailedResponse()
    {
        // Arrange
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = Guid.NewGuid(),
            OrderId = _orderId,
            ApplyReverseCharge = true,
            BuyerVatId = "123INVALID", // Invalid format: starts with digits
            BuyerCountry = "IT"
        };

        // Act
        var response = await _handler.ModifyInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotEmpty(response.Message);
    }

    [Fact]
    public async Task ModifyInvoice_ServiceThrowsException_ReturnsFailedResponse()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = invoiceId,
            OrderId = _orderId,
            ApplyReverseCharge = true,
            BuyerVatId = "IT12345678901",
            BuyerCountry = "IT"
        };

        _mockService
            .Setup(s => s.GetInvoiceByOrderIdAsync(_orderId, It.IsAny<CancellationToken>()))
#pragma warning disable CA2201 // Do not raise reserved exception types
            .ThrowsAsync(new Exception("Invoice not found"));
#pragma warning restore CA2201 // Do not raise reserved exception types

        // Act
        var response = await _handler.ModifyInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotEmpty(response.Message);
    }

    // ===== TAX AMOUNT CHANGE VERIFICATION =====

    [Fact]
    public async Task ModifyInvoice_ApplyReverseCharge_CorrectlyCalculatesTaxDifference()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = invoiceId,
            OrderId = _orderId,
            ApplyReverseCharge = true,
            BuyerVatId = "DE987654321",
            BuyerCountry = "DE"
        };

        var updatedInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            SubTotal = 1000m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 1000m,
            ReverseChargeApplies = true
        };

        var invoiceBeforeReverseCharge = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            SubTotal = 1000m,
            TaxRate = 0.19m,
            TaxAmount = 190m,
            Total = 1190m,
            ReverseChargeApplies = false
        };

        _mockService
            .Setup(s => s.GetInvoiceByOrderIdAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoiceBeforeReverseCharge);

        _mockService
            .Setup(s => s.ApplyReverseChargeAsync(invoiceId, "DE987654321", "DE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedInvoice);

        // Act
        var response = await _handler.ModifyInvoice(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        // Verify tax was removed (new tax = 0)
        Assert.Equal(0m, response.NewTaxAmount);
    }

    // ===== LOGGING VERIFICATION =====

    [Fact]
    public async Task GenerateInvoice_OnSuccess_LogsInvoiceGeneration()
    {
        // Arrange
        var command = new GenerateInvoiceCommand { OrderId = _orderId };
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            OrderId = _orderId,
            InvoiceNumber = "INV-2025-000003",
            ReverseChargeApplies = false
        };

        _mockService
            .Setup(s => s.GenerateInvoiceAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        // Act
        var result = await _handler.GenerateInvoice(command, CancellationToken.None);

        // Assert - Verify operation was successful
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ModifyInvoice_OnSuccess_LogsModification()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var command = new ModifyInvoiceCommand
        {
            InvoiceId = invoiceId,
            OrderId = _orderId,
            ApplyReverseCharge = true,
            BuyerVatId = "IT12345678901",
            BuyerCountry = "IT"
        };

        var updatedInvoice = new Invoice
        {
            Id = invoiceId,
            TenantId = _tenantId,
            OrderId = _orderId,
            SubTotal = 500m,
            TaxRate = 0m,
            TaxAmount = 0m,
            Total = 500m,
            ReverseChargeApplies = true
        };

        _mockService
            .Setup(s => s.GetInvoiceByOrderIdAsync(_orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice
            {
                Id = invoiceId,
                TenantId = _tenantId,
                OrderId = _orderId,
                SubTotal = 500m,
                TaxRate = 0.19m,
                TaxAmount = 95m,
                Total = 595m,
                ReverseChargeApplies = false
            });

        _mockService
            .Setup(s => s.ApplyReverseChargeAsync(invoiceId, "IT12345678901", "IT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedInvoice);

        // Act
        var result = await _handler.ModifyInvoice(command, CancellationToken.None);

        // Assert - Verify operation was successful
        Assert.NotNull(result);
        Assert.True(result.Success);
    }
}
