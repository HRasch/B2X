namespace B2Connect.Email.Models;

/// <summary>
/// Configuration for email provider
/// Contains all necessary settings for different provider types
/// </summary>
public class EmailProviderConfig
{
    /// <summary>
    /// The type of email provider
    /// </summary>
    public EmailProviderType Type { get; set; }

    // API Key authentication (SendGrid, Mailgun, Postmark, Azure Communication)
    /// <summary>
    /// API key for providers that use API key authentication
    /// </summary>
    public string? ApiKey { get; set; }

    // OAuth2 authentication (Microsoft Graph, Gmail)
    /// <summary>
    /// OAuth2 client ID
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// OAuth2 client secret
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// OAuth2 tenant ID (for Microsoft Graph)
    /// </summary>
    public string? TenantId { get; set; }

    // SMTP configuration
    /// <summary>
    /// SMTP server hostname
    /// </summary>
    public string? SmtpHost { get; set; }

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int? SmtpPort { get; set; }

    /// <summary>
    /// SMTP username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// SMTP password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Whether to use SSL/TLS for SMTP
    /// </summary>
    public bool? UseSsl { get; set; }

    // AWS SES configuration
    /// <summary>
    /// AWS region for SES
    /// </summary>
    public string? AwsRegion { get; set; }

    /// <summary>
    /// AWS access key (optional, uses IAM role if not provided)
    /// </summary>
    public string? AwsAccessKey { get; set; }

    /// <summary>
    /// AWS secret key (optional, uses IAM role if not provided)
    /// </summary>
    public string? AwsSecretKey { get; set; }

    /// <summary>
    /// Validates the configuration for the specified provider type
    /// </summary>
    public void Validate()
    {
        switch (Type)
        {
            case EmailProviderType.SendGrid:
            case EmailProviderType.Mailgun:
            case EmailProviderType.Postmark:
                if (string.IsNullOrEmpty(ApiKey))
                    throw new ArgumentException($"API key is required for {Type}");
                break;

            case EmailProviderType.AmazonSes:
                // AWS SDK handles authentication, no specific validation needed
                break;

            case EmailProviderType.MicrosoftGraph:
            case EmailProviderType.Gmail:
                if (string.IsNullOrEmpty(ClientId))
                    throw new ArgumentException($"Client ID is required for {Type}");
                if (string.IsNullOrEmpty(ClientSecret))
                    throw new ArgumentException($"Client secret is required for {Type}");
                break;

            case EmailProviderType.Smtp:
                if (string.IsNullOrEmpty(SmtpHost))
                    throw new ArgumentException("SMTP host is required for SMTP provider");
                if (string.IsNullOrEmpty(Username))
                    throw new ArgumentException("Username is required for SMTP provider");
                if (string.IsNullOrEmpty(Password))
                    throw new ArgumentException("Password is required for SMTP provider");
                break;

            case EmailProviderType.AzureCommunication:
                if (string.IsNullOrEmpty(ApiKey))
                    throw new ArgumentException($"API key is required for {Type}");
                break;

            default:
                throw new NotSupportedException($"Provider type {Type} is not supported");
        }
    }
}