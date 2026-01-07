using B2X.Identity.Infrastructure.ExternalServices;
using B2X.Identity.Interfaces;
using B2X.Identity.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2X.Identity.Tests.Infrastructure.ExternalServices;

public class FakeErpProviderTests
{
    private readonly Mock<ILogger<FakeErpProvider>> _mockLogger;
    private readonly FakeErpProvider _provider;

    public FakeErpProviderTests()
    {
        _mockLogger = new Mock<ILogger<FakeErpProvider>>();
        _provider = new FakeErpProvider(_mockLogger.Object);
    }

    [Fact]
    public async Task ProviderName_Returns_Fake()
    {
        // Act
        var name = _provider.ProviderName;

        // Assert
        Assert.Equal("Fake", name);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_ValidCustomerNumber_ReturnsCustomer()
    {
        // Act
        var result = await _provider.GetCustomerByNumberAsync("CUST-001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CUST-001", result.CustomerNumber);
        Assert.Equal("Max Mustermann", result.CustomerName);
        Assert.Equal("max.mustermann@example.com", result.Email);
        Assert.Equal("DE", result.Country);
        Assert.Equal("PRIVATE", result.BusinessType);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_B2BCustomer_ReturnsBusinessCustomer()
    {
        // Act
        var result = await _provider.GetCustomerByNumberAsync("CUST-100");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BUSINESS", result.BusinessType);
        Assert.Equal("TechCorp GmbH", result.CustomerName);
        Assert.NotNull(result.CreditLimit);
        Assert.True(result.CreditLimit > 0);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_InvalidCustomerNumber_ReturnsNull()
    {
        // Act
        var result = await _provider.GetCustomerByNumberAsync("INVALID-999");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_EmptyCustomerNumber_ReturnsNull()
    {
        // Act
        var result = await _provider.GetCustomerByNumberAsync("");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("max.mustermann@example.com", "CUST-001", "Max Mustermann")]
    [InlineData("info@techcorp.de", "CUST-100", "TechCorp GmbH")]
    [InlineData("contact@innovatelabs.at", "CUST-101", "InnovateLabs AG")]
    public async Task GetCustomerByEmailAsync_ValidEmail_ReturnsCustomer(string email, string expectedNumber, string expectedName)
    {
        // Act
        var result = await _provider.GetCustomerByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedNumber, result.CustomerNumber);
        Assert.Equal(expectedName, result.CustomerName);
    }

    [Fact]
    public async Task GetCustomerByEmailAsync_InvalidEmail_ReturnsNull()
    {
        // Act
        var result = await _provider.GetCustomerByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("TechCorp GmbH", "CUST-100")]
    [InlineData("techcorp gmbh", "CUST-100")] // Case-insensitive
    [InlineData("InnovateLabs AG", "CUST-101")]
    [InlineData("Global Solutions SA", "CUST-102")]
    public async Task GetCustomerByCompanyNameAsync_ValidCompanyName_ReturnsCustomer(string companyName, string expectedNumber)
    {
        // Act
        var result = await _provider.GetCustomerByCompanyNameAsync(companyName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedNumber, result.CustomerNumber);
    }

    [Fact]
    public async Task GetCustomerByCompanyNameAsync_InvalidCompanyName_ReturnsNull()
    {
        // Act
        var result = await _provider.GetCustomerByCompanyNameAsync("Nonexistent Corp");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsAvailableAsync_AlwaysReturnsTrue()
    {
        // Act
        var result = await _provider.IsAvailableAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetSyncStatusAsync_ReturnsSyncStatus()
    {
        // Act
        var result = await _provider.GetSyncStatusAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsConnected);
        Assert.Equal("Fake", result.ErpSystemType);
        Assert.True(result.CachedCustomerCount > 0);
        Assert.NotNull(result.LastSyncTime);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_ReturnsClonedData_DoesNotAffectInternalState()
    {
        // Arrange
        var customer1 = await _provider.GetCustomerByNumberAsync("CUST-001");

        // Act
        customer1!.CustomerName = "Modified Name";
        var customer2 = await _provider.GetCustomerByNumberAsync("CUST-001");

        // Assert
        Assert.NotEqual(customer1.CustomerName, customer2!.CustomerName);
        Assert.Equal("Max Mustermann", customer2.CustomerName);
    }

    [Fact]
    public async Task MultipleLookupsForSameCustomer_SucceedViaEmailAndNumber()
    {
        // Act
        var byNumber = await _provider.GetCustomerByNumberAsync("CUST-001");
        var byEmail = await _provider.GetCustomerByEmailAsync("max.mustermann@example.com");

        // Assert
        Assert.NotNull(byNumber);
        Assert.NotNull(byEmail);
        Assert.Equal(byNumber.CustomerNumber, byEmail.CustomerNumber);
        Assert.Equal(byNumber.Email, byEmail.Email);
    }
}

public class ResilientErpProviderDecoratorTests
{
    private readonly Mock<IErpProvider> _mockPrimary;
    private readonly Mock<IErpProvider> _mockFallback;
    private readonly Mock<ILogger<ResilientErpProviderDecorator>> _mockLogger;
    private readonly ResilientErpProviderDecorator _decorator;

    public ResilientErpProviderDecoratorTests()
    {
        _mockPrimary = new Mock<IErpProvider>();
        _mockFallback = new Mock<IErpProvider>();
        _mockLogger = new Mock<ILogger<ResilientErpProviderDecorator>>();

        _mockPrimary.Setup(p => p.ProviderName).Returns("Primary");
        _mockFallback.Setup(p => p.ProviderName).Returns("Fallback");

        _decorator = new ResilientErpProviderDecorator(_mockPrimary.Object, _mockFallback.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_PrimarySucceeds_ReturnsPrimaryResult()
    {
        // Arrange
        var customer = new ErpCustomerDto { CustomerNumber = "123", CustomerName = "Test" };
        _mockPrimary.Setup(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _decorator.GetCustomerByNumberAsync("123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.CustomerName);
        _mockPrimary.Verify(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()), Times.Once);
        _mockFallback.Verify(p => p.GetCustomerByNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_PrimaryFails_UsesFallback()
    {
        // Arrange
        var customer = new ErpCustomerDto { CustomerNumber = "123", CustomerName = "Fallback Test" };
        _mockPrimary.Setup(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Connection failed"));
        _mockFallback.Setup(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _decorator.GetCustomerByNumberAsync("123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Fallback Test", result.CustomerName);
        _mockFallback.Verify(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCustomerByNumberAsync_BothFail_ThrowsException()
    {
        // Arrange
        _mockPrimary.Setup(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Primary failed"));
        _mockFallback.Setup(p => p.GetCustomerByNumberAsync("123", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Fallback also failed"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _decorator.GetCustomerByNumberAsync("123"));
    }

    [Fact]
    public async Task IsAvailableAsync_PrimaryThrows_ReturnsFalse()
    {
        // Arrange
        _mockPrimary.Setup(p => p.IsAvailableAsync(It.IsAny<CancellationToken>()))
#pragma warning disable CA2201 // Do not raise reserved exception types
            .ThrowsAsync(new Exception("Connection error"));
#pragma warning restore CA2201 // Do not raise reserved exception types

        // Act
        var result = await _decorator.IsAvailableAsync();

        // Assert
        Assert.False(result);
    }
}

public class ErpProviderFactoryTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<ErpProviderFactory>> _mockLogger;
    private readonly ErpProviderFactory _factory;

    public ErpProviderFactoryTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<ErpProviderFactory>>();

        // Setup für Fake-Provider
        var fakeLogger = new Mock<ILogger<FakeErpProvider>>();
        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(ILogger<FakeErpProvider>)))
            .Returns(fakeLogger.Object);

        _factory = new ErpProviderFactory(_mockServiceProvider.Object, _mockLogger.Object);
    }

    [Fact]
    public void CreateProvider_Fake_ReturnsFakeProvider()
    {
        // Act
        var provider = _factory.CreateProvider("Fake");

        // Assert
        Assert.NotNull(provider);
        Assert.Equal("Fake", provider.ProviderName);
        Assert.IsType<FakeErpProvider>(provider);
    }

    [Fact]
    public void GetAvailableProviders_ReturnsProviderList()
    {
        // Act
        var providers = _factory.GetAvailableProviders();

        // Assert
        Assert.NotNull(providers);
        Assert.Contains("Fake", providers);
    }

    [Fact]
    public void CreateProvider_UnknownProvider_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateProvider("Unknown"));
    }
}
