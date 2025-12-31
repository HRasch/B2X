using B2Connect.Email.Models;
using B2Connect.Email.Services.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Email.Tests.Services.Providers;

public class SendGridProviderTests
{
    private readonly Mock<ILogger<SendGridProvider>> _loggerMock;
    private readonly EmailProviderConfig _validConfig;

    public SendGridProviderTests()
    {
        _loggerMock = new Mock<ILogger<SendGridProvider>>();
        _validConfig = new EmailProviderConfig
        {
            Type = EmailProviderType.SendGrid,
            ApiKey = "test-api-key"
        };
    }

    [Fact]
    public void Constructor_ValidConfig_CreatesInstance()
    {
        // Act
        var provider = new SendGridProvider(_validConfig, _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
        Assert.Equal("SendGrid", provider.ProviderName);
    }

    [Fact]
    public void Constructor_NullApiKey_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.SendGrid,
            ApiKey = null
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SendGridProvider(config, _loggerMock.Object));
    }

    [Fact]
    public async Task SendAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new SendGridProvider(_validConfig, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            provider.SendAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task IsAvailableAsync_ReturnsTrue()
    {
        // Arrange
        var provider = new SendGridProvider(_validConfig, _loggerMock.Object);

        // Act
        var result = await provider.IsAvailableAsync();

        // Assert
        Assert.True(result);
    }
}