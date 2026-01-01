# Architecture Decision Record: Email Provider Authentication Methods

**Date:** December 31, 2025
**Author:** @Architect
**Status:** Proposed
**Scope:** Email Domain Service - Provider Authentication

---

## Decision

**Implement comprehensive authentication methods** for all major email providers supporting modern security standards including OAuth2, API Keys, IAM roles, and certificate-based authentication.

---

## Context

### Current State
EmailService uses IEmailProvider abstraction but lacks concrete implementations for different email providers and their authentication methods.

### Problem
- **Security gaps:** No support for modern authentication methods
- **Provider limitations:** Cannot use major email services (SendGrid, AWS SES, Microsoft Graph, Gmail)
- **Compliance issues:** Missing OAuth2 support for regulated environments
- **Scalability:** No support for provider-specific optimizations

### Goals
1. Support all major email providers with their native authentication
2. Implement modern security standards (OAuth2, API Keys, IAM)
3. Enable provider failover and load balancing
4. Maintain tenant-specific configuration

---

## Decision

### Supported Authentication Methods

#### 1. API Key Authentication
```csharp
public class SendGridProvider : IEmailProvider
{
    private readonly string _apiKey;

    public SendGridProvider(EmailProviderConfig config)
    {
        _apiKey = config.ApiKey ?? throw new ArgumentNullException(nameof(config.ApiKey));
    }

    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken ct)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        // SendGrid API call
        var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", content, ct);
        return new EmailProviderResult { Success = response.IsSuccessStatusCode };
    }
}
```

#### 2. OAuth2 Authentication
```csharp
public class MicrosoftGraphProvider : IEmailProvider
{
    private readonly ITokenProvider _tokenProvider;

    public MicrosoftGraphProvider(EmailProviderConfig config, ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken ct)
    {
        var token = await _tokenProvider.GetAccessTokenAsync();
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Microsoft Graph API call
        var response = await client.PostAsync("https://graph.microsoft.com/v1.0/me/sendMail", content, ct);
        return new EmailProviderResult { Success = response.IsSuccessStatusCode };
    }
}
```

#### 3. AWS IAM/Signature Authentication
```csharp
public class SesProvider : IEmailProvider
{
    private readonly IAmazonSimpleEmailService _sesClient;

    public SesProvider(EmailProviderConfig config)
    {
        // AWS SDK handles IAM authentication automatically
        _sesClient = new AmazonSimpleEmailServiceClient();
    }

    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken ct)
    {
        var request = new SendEmailRequest
        {
            Source = message.From,
            Destination = new Destination { ToAddresses = message.To },
            Message = new Amazon.SimpleEmail.Model.Message { /* ... */ }
        };

        var response = await _sesClient.SendEmailAsync(request, ct);
        return new EmailProviderResult { Success = true, ExternalMessageId = response.MessageId };
    }
}
```

#### 4. SMTP Authentication
```csharp
public class SmtpProvider : IEmailProvider
{
    private readonly SmtpConfig _config;

    public SmtpProvider(EmailProviderConfig config)
    {
        _config = new SmtpConfig
        {
            Host = config.SmtpHost,
            Port = config.SmtpPort ?? 587,
            Username = config.Username,
            Password = config.Password,
            UseSsl = config.UseSsl ?? true
        };
    }

    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken ct)
    {
        using var client = new SmtpClient(_config.Host, _config.Port);
        client.Credentials = new NetworkCredential(_config.Username, _config.Password);
        client.EnableSsl = _config.UseSsl;

        var mailMessage = new MailMessage(message.From, message.To.First())
        {
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        await client.SendMailAsync(mailMessage, ct);
        return new EmailProviderResult { Success = true };
    }
}
```

### Provider Configuration Model

```csharp
public class EmailProviderConfig
{
    public EmailProviderType Type { get; set; }

    // API Key auth
    public string? ApiKey { get; set; }

    // OAuth2
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }

    // SMTP
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool? UseSsl { get; set; }

    // AWS
    public string? AwsRegion { get; set; }
    public string? AwsAccessKey { get; set; }
    public string? AwsSecretKey { get; set; }
}

public enum EmailProviderType
{
    SendGrid,
    Mailgun,
    Postmark,
    AmazonSes,
    MicrosoftGraph,
    Gmail,
    Smtp,
    AzureCommunication
}
```

### Token Provider Abstraction

```csharp
public interface ITokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    Task RefreshTokenAsync(CancellationToken cancellationToken = default);
}

public class OAuth2TokenProvider : ITokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly OAuth2Config _config;
    private string? _accessToken;
    private DateTime _expiresAt;

    public async Task<string> GetAccessTokenAsync(CancellationToken ct)
    {
        if (_accessToken == null || DateTime.UtcNow >= _expiresAt)
        {
            await RefreshTokenAsync(ct);
        }
        return _accessToken!;
    }

    public async Task RefreshTokenAsync(CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _config.TokenEndpoint);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _config.ClientId),
            new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
            new KeyValuePair<string, string>("scope", _config.Scope)
        });

        var response = await _httpClient.SendAsync(request, ct);
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(ct);

        _accessToken = tokenResponse!.AccessToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // 1min buffer
    }
}
```

### Provider Factory Pattern

```csharp
public interface IEmailProviderFactory
{
    IEmailProvider CreateProvider(EmailProviderConfig config);
}

public class EmailProviderFactory : IEmailProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EmailProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEmailProvider CreateProvider(EmailProviderConfig config)
    {
        return config.Type switch
        {
            EmailProviderType.SendGrid => new SendGridProvider(config),
            EmailProviderType.AmazonSes => new SesProvider(config),
            EmailProviderType.MicrosoftGraph => new MicrosoftGraphProvider(config, _serviceProvider.GetRequiredService<ITokenProvider>()),
            EmailProviderType.Smtp => new SmtpProvider(config),
            _ => throw new NotSupportedException($"Provider type {config.Type} not supported")
        };
    }
}
```

---

## Implementation Plan

### Phase 1: Core Providers (Priority: High)
1. **SendGrid** - API Key authentication
2. **Amazon SES** - IAM/Signature authentication
3. **SMTP** - Basic authentication with TLS

### Phase 2: OAuth2 Providers (Priority: Medium)
1. **Microsoft Graph** - OAuth2 client credentials
2. **Gmail API** - OAuth2 with refresh tokens
3. **Azure Communication Services** - API Key + OAuth2

### Phase 3: Advanced Features (Priority: Low)
1. **Provider failover** - Automatic switching between providers
2. **Load balancing** - Distribute load across multiple provider instances
3. **Rate limiting** - Respect provider quotas
4. **Certificate authentication** - For on-premise SMTP servers

---

## Security Considerations

### Credential Management
- **Never store credentials in code** - Use Azure Key Vault, AWS Secrets Manager, or similar
- **Rotate credentials regularly** - Implement credential rotation policies
- **Audit credential access** - Log all credential retrievals

### OAuth2 Security
- **PKCE for public clients** - Use Proof Key for Code Exchange
- **State parameter** - Prevent CSRF attacks
- **Token refresh** - Automatic token renewal before expiration
- **Scope limitation** - Request minimal required permissions

### Network Security
- **TLS 1.3** - Use latest TLS version for all connections
- **Certificate pinning** - Pin provider certificates to prevent MITM
- **IP whitelisting** - Restrict outgoing connections where possible

---

## Testing Strategy

### Unit Tests
```csharp
[Fact]
public async Task SendGridProvider_ApiKeyAuth_SendsEmail()
{
    // Arrange
    var config = new EmailProviderConfig
    {
        Type = EmailProviderType.SendGrid,
        ApiKey = "test-api-key"
    };
    var provider = new SendGridProvider(config);

    // Mock HTTP client and test
}
```

### Integration Tests
```csharp
[Fact]
public async Task OAuth2Provider_TokenRefresh_Works()
{
    // Test complete OAuth2 flow with mock token endpoint
}
```

### Security Tests
- **Credential leakage** - Ensure no credentials in logs or error messages
- **Token expiration** - Test automatic token refresh
- **Rate limit handling** - Test provider quota management

---

## Consequences

### Positive
- **Provider flexibility** - Support for all major email services
- **Security compliance** - Modern authentication standards
- **Scalability** - Multiple providers with failover
- **Future-proof** - Easy to add new providers

### Negative
- **Complexity** - Multiple authentication methods to maintain
- **Dependencies** - Additional NuGet packages for each provider
- **Configuration** - Complex tenant-specific provider setup

### Risks
- **API changes** - Provider APIs may change authentication methods
- **Rate limits** - Different providers have different quotas
- **Cost** - Some providers charge per email sent

---

## Alternatives Considered

### Single Provider Approach
- **Pros:** Simpler implementation, consistent API
- **Cons:** Vendor lock-in, limited scalability
- **Decision:** Rejected due to business requirements for multiple providers

### Custom SMTP Only
- **Pros:** Full control, no external dependencies
- **Cons:** Deliverability issues, infrastructure overhead
- **Decision:** Rejected due to compliance and deliverability requirements

---

## Compliance

This ADR aligns with:
- ADR_DOMAIN_SERVICES_STATELESS: Stateless provider implementations
- Security Instructions: Modern authentication methods
- GDPR: Secure credential handling and audit logging

---

## Status: Proposed

**Next Steps:**
1. Implement core providers (SendGrid, SES, SMTP)
2. Add OAuth2 token management
3. Create provider factory and configuration
4. Add comprehensive tests
5. Document provider setup per tenant