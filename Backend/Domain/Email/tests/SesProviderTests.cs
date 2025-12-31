using B2Connect.Email.Models;
using B2Connect.Email.Services.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Email.Tests.Services.Providers;

public class SesProviderTests
{
    private readonly Mock<ILogger<SesProvider>> _loggerMock;
    private readonly EmailProviderConfig _validConfig;

    public SesProviderTests()
    {
        _loggerMock = new Mock<ILogger<SesProvider>>();
        _validConfig = new EmailProviderConfig
        {
            Type = EmailProviderType.AmazonSes,
            AwsRegion = "us-east-1"
        };
    }

    [Fact]
    public void Constructor_ValidConfig_CreatesInstance()
    {
        // Act
        var provider = new SesProvider(_validConfig, _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
        Assert.Equal("Amazon SES", provider.ProviderName);
    }

    [Fact]
    public async Task SendAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new SesProvider(_validConfig, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            provider.SendAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task IsAvailableAsync_ReturnsTrue()
    {
        // Arrange
        var provider = new SesProvider(_validConfig, _loggerMock.Object);

        // Act
        var result = await provider.IsAvailableAsync();

        // Assert
        Assert.True(result);
    }
}