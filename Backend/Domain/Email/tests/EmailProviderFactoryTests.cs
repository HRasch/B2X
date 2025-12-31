using B2Connect.Email.Models;
using B2Connect.Email.Services;
using B2Connect.Email.Services.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Email.Tests.Services;

public class EmailProviderFactoryTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;

    public EmailProviderFactoryTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(ILoggerFactory)))
            .Returns(_loggerFactoryMock.Object);
    }

    [Fact]
    public void CreateProvider_SendGrid_ValidConfig_ReturnsSendGridProvider()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.SendGrid,
            ApiKey = "test-api-key"
        };

        var factory = new EmailProviderFactory(_serviceProviderMock.Object);

        // Act
        var provider = factory.CreateProvider(config);

        // Assert
        Assert.IsType<SendGridProvider>(provider);
        Assert.Equal("SendGrid", provider.ProviderName);
    }

    [Fact]
    public void CreateProvider_Ses_ValidConfig_ReturnsSesProvider()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.AmazonSes,
            AwsRegion = "us-east-1"
        };

        var factory = new EmailProviderFactory(_serviceProviderMock.Object);

        // Act
        var provider = factory.CreateProvider(config);

        // Assert
        Assert.IsType<SesProvider>(provider);
        Assert.Equal("Amazon SES", provider.ProviderName);
    }

    [Fact]
    public void CreateProvider_Smtp_ValidConfig_ReturnsSmtpProvider()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.Smtp,
            SmtpHost = "smtp.example.com",
            Username = "test@example.com",
            Password = "password"
        };

        var factory = new EmailProviderFactory(_serviceProviderMock.Object);

        // Act
        var provider = factory.CreateProvider(config);

        // Assert
        Assert.IsType<SmtpProvider>(provider);
        Assert.Equal("SMTP", provider.ProviderName);
    }

    [Theory]
    [InlineData(EmailProviderType.Mailgun)]
    [InlineData(EmailProviderType.Postmark)]
    [InlineData(EmailProviderType.MicrosoftGraph)]
    [InlineData(EmailProviderType.Gmail)]
    [InlineData(EmailProviderType.AzureCommunication)]
    public void CreateProvider_NotImplementedProvider_ThrowsNotSupportedException(EmailProviderType providerType)
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = providerType,
            ApiKey = "test-key"
        };

        var factory = new EmailProviderFactory(_serviceProviderMock.Object);

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() =>
            factory.CreateProvider(config));

        Assert.Contains("not yet implemented", exception.Message);
    }

    [Fact]
    public void CreateProvider_SendGrid_MissingApiKey_ThrowsArgumentException()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.SendGrid,
            ApiKey = null
        };

        var factory = new EmailProviderFactory(_serviceProviderMock.Object);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            factory.CreateProvider(config));

        Assert.Contains("API key is required", exception.Message);
    }

    [Fact]
    public void CreateProvider_Smtp_MissingHost_ThrowsArgumentException()
    {
        // Arrange
        var config = new EmailProviderConfig
        {
            Type = EmailProviderType.Smtp,
            SmtpHost = null,
            Username = "test@example.com",
            Password = "password"
        };

        var factory = new EmailProviderFactory(_serviceProviderMock.Object);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            factory.CreateProvider(config));

        Assert.Contains("SMTP host is required", exception.Message);
    }
}