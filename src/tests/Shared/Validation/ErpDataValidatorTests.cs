using B2X.Api.Models.Erp;
using B2X.Api.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2X.Api.Tests.Validation;

public class ErpDataValidatorTests
{
    private readonly Mock<ILogger<ErpDataValidator>> _loggerMock;
    private readonly Mock<IValidationRulesProvider> _rulesProviderMock;
    private readonly ErpDataValidator _validator;

    public ErpDataValidatorTests()
    {
        _loggerMock = new Mock<ILogger<ErpDataValidator>>();
        _rulesProviderMock = new Mock<IValidationRulesProvider>();
        _validator = new ErpDataValidator(_loggerMock.Object, _rulesProviderMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ValidData_ReturnsValidResult()
    {
        // Arrange
        var data = new ErpData
        {
            Id = "1",
            Sku = "TEST123",
            Email = "test@example.com",
            TenantId = "tenant1",
            CategoryId = "category1"
        };

        _rulesProviderMock.Setup(x => x.GetRulesForTenantAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<IValidationRule<ErpData>>());

        // Act
        var result = await _validator.ValidateAsync(data);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("VALIDATION_PASSED", result.Code);
    }

    [Fact]
    public async Task ValidateAsync_InvalidEmail_ReturnsError()
    {
        // Arrange
        var data = new ErpData
        {
            Id = "1",
            Sku = "TEST123",
            Email = "invalid-email",
            TenantId = "tenant1"
        };

        _rulesProviderMock.Setup(x => x.GetRulesForTenantAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<IValidationRule<ErpData>>());

        // Act
        var result = await _validator.ValidateAsync(data);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("VALIDATION_FAILED", result.Code);
    }
}
