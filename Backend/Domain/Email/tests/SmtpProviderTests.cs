using B2Connect.Email.Models;
using B2Connect.Email.Services.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Email.Tests.Services.Providers;

public class SmtpProviderTests
{
    private readonly Mock<ILogger<SmtpProvider>> _loggerMock;
    private readonly EmailProviderConfig _validConfig;

    public SmtpProviderTests()
    {
        _loggerMock = new Mock<ILogger<SmtpProvider>>();
        _validConfig = new EmailProviderConfig
        {
            Type = EmailProviderType.Smtp,
            SmtpHost = "smtp.example.com",
            SmtpPort = 587,
            Username = "test@example.com",
            Password = "password",
            UseSsl = true
        };
    }

    [Fact]
    public void Constructor_ValidConfig_CreatesInstance()
    {
        // Act
        var provider = new SmtpProvider(_validConfig, _loggerMock.Object);

        // Assert
        Assert.NotNull(provider);
        Assert.Equal("SMTP", provider.ProviderName);
    }

    [Fact]
    public void Constructor_NullSmtpHost_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.Smtp,
            SmtpHost = null,
            Username = "test@example.com",
            Password = "password"
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SmtpProvider(config, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_NullUsername_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.Smtp,
            SmtpHost = "smtp.example.com",
            Username = null,
            Password = "password"
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SmtpProvider(config, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_NullPassword_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.Smtp,
            SmtpHost = "smtp.example.com",
            Username = "test@example.com",
            Password = null
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new SmtpProvider(config, _loggerMock.Object));
    }

    [Fact]
    public async Task SendAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new SmtpProvider(_validConfig, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            provider.SendAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task IsAvailableAsync_ReturnsTrue()
    {
        // Arrange
        var provider = new SmtpProvider(_validConfig, _loggerMock.Object);

        // Act
        var result = await provider.IsAvailableAsync();

        // Assert
        Assert.True(result);
    }
}