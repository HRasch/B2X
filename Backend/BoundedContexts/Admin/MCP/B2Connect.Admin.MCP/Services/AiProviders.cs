using OpenAI;
using OpenAI.Chat;
using B2Connect.Admin.MCP.Services;
using Microsoft.Extensions.Logging;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Anthropic;
using Anthropic.SDK;
using Azure.AI.OpenAI;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// AI Provider interface for consistent implementation
/// </summary>
public interface IAIProvider
{
    string ProviderName { get; }
    Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// OpenAI provider implementation
/// </summary>
public class OpenAiProvider : IAIProvider
{
    private readonly ILogger<OpenAiProvider> _logger;
    private readonly SecretClient? _secretClient;

    public string ProviderName => "openai";

    public OpenAiProvider(
        ILogger<OpenAiProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        // Initialize Key Vault client
        var keyVaultUrl = configuration["Azure:KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }
    }

    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement OpenAI integration
        return new AiResponse
        {
            Content = $"OpenAI response for tenant {tenantId}: {userMessage}",
            TokensUsed = 100,
            Model = model
        };
    }

    private async Task<string> GetApiKeyAsync(string tenantId, string provider)
    {
        // Try Key Vault first
        if (_secretClient != null)
        {
            try
            {
                var secretName = $"tenant-{tenantId}-{provider}-apikey";
                var secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get API key from Key Vault for tenant {TenantId}, provider {Provider}", tenantId, provider);
            }
        }

        // Fallback to environment variable (for development)
        var envKey = Environment.GetEnvironmentVariable($"OPENAI_API_KEY_{tenantId.ToUpper()}");
        if (!string.IsNullOrEmpty(envKey))
        {
            return envKey;
        }

        throw new InvalidOperationException($"API key not found for tenant {tenantId}, provider {provider}");
    }
}

/// <summary>
/// Anthropic provider implementation
/// </summary>
public class AnthropicProvider : IAIProvider
{
    private readonly ILogger<AnthropicProvider> _logger;
    private readonly SecretClient? _secretClient;

    public string ProviderName => "anthropic";

    public AnthropicProvider(
        ILogger<AnthropicProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        var keyVaultUrl = configuration["Azure:KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }
    }

    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement Anthropic integration
        return new AiResponse
        {
            Content = $"Anthropic response for tenant {tenantId}: {userMessage}",
            TokensUsed = 100,
            Model = model
        };
    }

    private async Task<string> GetApiKeyAsync(string tenantId, string provider)
    {
        if (_secretClient != null)
        {
            try
            {
                var secretName = $"tenant-{tenantId}-{provider}-apikey";
                var secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get API key from Key Vault for tenant {TenantId}, provider {Provider}", tenantId, provider);
            }
        }

        var envKey = Environment.GetEnvironmentVariable($"ANTHROPIC_API_KEY_{tenantId.ToUpper()}");
        if (!string.IsNullOrEmpty(envKey))
        {
            return envKey;
        }

        throw new InvalidOperationException($"API key not found for tenant {tenantId}, provider {provider}");
    }
}

/// <summary>
/// Azure OpenAI provider implementation
/// </summary>
public class AzureOpenAiProvider : IAIProvider
{
    private readonly ILogger<AzureOpenAiProvider> _logger;
    private readonly SecretClient? _secretClient;
    private readonly IConfiguration _configuration;

    public string ProviderName => "azure";

    public AzureOpenAiProvider(
        ILogger<AzureOpenAiProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var keyVaultUrl = configuration["Azure:KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }
    }

    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement Azure OpenAI integration
        return new AiResponse
        {
            Content = $"Azure OpenAI response for tenant {tenantId}: {userMessage}",
            TokensUsed = 100,
            Model = model
        };
    }

    private async Task<string> GetApiKeyAsync(string tenantId, string provider)
    {
        if (_secretClient != null)
        {
            try
            {
                var secretName = $"tenant-{tenantId}-{provider}-apikey";
                var secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get API key from Key Vault for tenant {TenantId}, provider {Provider}", tenantId, provider);
            }
        }

        var envKey = Environment.GetEnvironmentVariable($"AZURE_OPENAI_API_KEY_{tenantId.ToUpper()}");
        if (!string.IsNullOrEmpty(envKey))
        {
            return envKey;
        }

        throw new InvalidOperationException($"API key not found for tenant {tenantId}, provider {provider}");
    }

    private async Task<string> GetEndpointAsync(string tenantId)
    {
        // Try tenant-specific endpoint first
        if (_secretClient != null)
        {
            try
            {
                var secretName = $"tenant-{tenantId}-azure-endpoint";
                var secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch
            {
                // Fall back to default
            }
        }

        // Default endpoint from configuration
        var defaultEndpoint = _configuration["Azure:OpenAI:DefaultEndpoint"];
        if (!string.IsNullOrEmpty(defaultEndpoint))
        {
            return defaultEndpoint;
        }

        throw new InvalidOperationException($"Azure OpenAI endpoint not found for tenant {tenantId}");
    }
}