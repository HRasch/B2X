using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using B2Connect.Email.Services.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Services;

/// <summary>
/// Factory for creating email provider instances
/// Supports all configured email providers with their authentication methods
/// </summary>
public class EmailProviderFactory : IEmailProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EmailProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public IEmailProvider CreateProvider(EmailProviderConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

        return config.Type switch
        {
            EmailProviderType.SendGrid => CreateSendGridProvider(config, loggerFactory),
            EmailProviderType.AmazonSes => CreateSesProvider(config, loggerFactory),
            EmailProviderType.Smtp => CreateSmtpProvider(config, loggerFactory),
            EmailProviderType.Mailgun => throw new NotSupportedException("Mailgun provider not yet implemented"),
            EmailProviderType.Postmark => throw new NotSupportedException("Postmark provider not yet implemented"),
            EmailProviderType.MicrosoftGraph => throw new NotSupportedException("Microsoft Graph provider not yet implemented"),
            EmailProviderType.Gmail => throw new NotSupportedException("Gmail provider not yet implemented"),
            EmailProviderType.AzureCommunication => throw new NotSupportedException("Azure Communication provider not yet implemented"),
            _ => throw new NotSupportedException($"Email provider type '{config.Type}' is not supported")
        };
    }

    private IEmailProvider CreateSendGridProvider(EmailProviderConfig config, ILoggerFactory loggerFactory)
    {
        ValidateSendGridConfig(config);
        var logger = loggerFactory.CreateLogger<SendGridProvider>();
        return new SendGridProvider(config, logger);
    }

    private IEmailProvider CreateSesProvider(EmailProviderConfig config, ILoggerFactory loggerFactory)
    {
        ValidateSesConfig(config);
        var logger = loggerFactory.CreateLogger<SesProvider>();
        return new SesProvider(config, logger);
    }

    private IEmailProvider CreateSmtpProvider(EmailProviderConfig config, ILoggerFactory loggerFactory)
    {
        ValidateSmtpConfig(config);
        var logger = loggerFactory.CreateLogger<SmtpProvider>();
        return new SmtpProvider(config, logger);
    }

    private void ValidateSendGridConfig(EmailProviderConfig config)
    {
        if (string.IsNullOrEmpty(config.ApiKey))
            throw new ArgumentException("SendGrid API key is required", nameof(config.ApiKey));
    }

    private void ValidateSesConfig(EmailProviderConfig config)
    {
        // AWS SDK handles authentication automatically via IAM roles or credentials
        // No specific validation needed here as AWS SDK will handle auth
    }

    private void ValidateSmtpConfig(EmailProviderConfig config)
    {
        if (string.IsNullOrEmpty(config.SmtpHost))
            throw new ArgumentException("SMTP host is required", nameof(config.SmtpHost));

        if (string.IsNullOrEmpty(config.Username))
            throw new ArgumentException("SMTP username is required", nameof(config.Username));

        if (string.IsNullOrEmpty(config.Password))
            throw new ArgumentException("SMTP password is required", nameof(config.Password));
    }
}