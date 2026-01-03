using Microsoft.Extensions.AI;
using OpenAI;
using B2Connect.Admin.MCP.Services;
using Microsoft.Extensions.Logging;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Anthropic;
using Anthropic.SDK;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI.OpenAI;
using Microsoft.Extensions.AI.Anthropic;
using Microsoft.Extensions.AI.AzureAIInference;
using System.ClientModel;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// AI Provider interface using Microsoft.Extensions.AI
/// </summary>
public interface IAIProvider
{
    string ProviderName { get; }
    Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        DataSanitizationService sanitizationService,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// OpenAI provider implementation using Microsoft.Extensions.AI
/// </summary>
public class OpenAiProvider : IAIProvider
{
    private readonly ILogger<OpenAiProvider> _logger;
    private readonly SecretClient? _secretClient;
    private readonly IConfiguration _configuration;

    public string ProviderName => "openai";

    public OpenAiProvider(
        ILogger<OpenAiProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

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
        DataSanitizationService sanitizationService,
        CancellationToken cancellationToken = default)
    {
        // Sanitize user input before sending to external AI provider
        var promptValidation = sanitizationService.ValidateContent(prompt, tenantId);
        var messageValidation = sanitizationService.ValidateContent(userMessage, tenantId);

        // Log sanitization results
        if (promptValidation.DetectedPatterns.Any() || messageValidation.DetectedPatterns.Any())
        {
            _logger.LogInformation("AI request sanitized for tenant {TenantId}: Prompt patterns: {PromptPatterns}, Message patterns: {MessagePatterns}",
                tenantId,
                string.Join(", ", promptValidation.DetectedPatterns),
                string.Join(", ", messageValidation.DetectedPatterns));
        }

        // Block high-risk requests
        if (promptValidation.RiskLevel == RiskLevel.High || messageValidation.RiskLevel == RiskLevel.High)
        {
            _logger.LogWarning("Blocked high-risk AI request for tenant {TenantId} due to sensitive data detection", tenantId);
            throw new InvalidOperationException("Request contains sensitive data that cannot be sent to external AI providers");
        }

        // Get sanitized content
        var sanitizedPrompt = sanitizationService.SanitizeContent(prompt, tenantId).SanitizedContent;
        var sanitizedMessage = sanitizationService.SanitizeContent(userMessage, tenantId).SanitizedContent;

        var apiKey = await GetApiKeyAsync(tenantId, ProviderName);

        // Create OpenAI client using Microsoft.Extensions.AI
        var client = new OpenAIClient(apiKey).AsChatClient(model);

        // Add tenant-specific system prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, sanitizedPrompt),
            new ChatMessage(ChatRole.User, sanitizedMessage)
        };

        var response = await client.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        // Extract token usage from response metadata if available
        var tokensUsed = 0;
        if (response.Usage?.TotalTokenCount is not null)
        {
            tokensUsed = response.Usage.TotalTokenCount.Value;
        }

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = tokensUsed,
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
/// Anthropic provider implementation using Microsoft.Extensions.AI
/// </summary>
public class AnthropicProvider : IAIProvider
{
    private readonly ILogger<AnthropicProvider> _logger;
    private readonly SecretClient? _secretClient;
    private readonly IConfiguration _configuration;

    public string ProviderName => "anthropic";

    public AnthropicProvider(
        ILogger<AnthropicProvider> logger,
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
        DataSanitizationService sanitizationService,
        CancellationToken cancellationToken = default)
    {
        // Sanitize user input before sending to external AI provider
        var promptValidation = sanitizationService.ValidateContent(prompt, tenantId);
        var messageValidation = sanitizationService.ValidateContent(userMessage, tenantId);

        // Log sanitization results
        if (promptValidation.DetectedPatterns.Any() || messageValidation.DetectedPatterns.Any())
        {
            _logger.LogInformation("AI request sanitized for tenant {TenantId}: Prompt patterns: {PromptPatterns}, Message patterns: {MessagePatterns}",
                tenantId,
                string.Join(", ", promptValidation.DetectedPatterns),
                string.Join(", ", messageValidation.DetectedPatterns));
        }

        // Block high-risk requests
        if (promptValidation.RiskLevel == RiskLevel.High || messageValidation.RiskLevel == RiskLevel.High)
        {
            _logger.LogWarning("Blocked high-risk AI request for tenant {TenantId} due to sensitive data detection", tenantId);
            throw new InvalidOperationException("Request contains sensitive data that cannot be sent to external AI providers");
        }

        // Get sanitized content
        var sanitizedPrompt = sanitizationService.SanitizeContent(prompt, tenantId).SanitizedContent;
        var sanitizedMessage = sanitizationService.SanitizeContent(userMessage, tenantId).SanitizedContent;

        var apiKey = await GetApiKeyAsync(tenantId, ProviderName);

        // Create Anthropic client using Microsoft.Extensions.AI
        var client = new AnthropicClient(apiKey).AsChatClient(model);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, sanitizedPrompt),
            new ChatMessage(ChatRole.User, sanitizedMessage)
        };

        var response = await client.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        var tokensUsed = 0;
        if (response.Usage?.TotalTokenCount is not null)
        {
            tokensUsed = response.Usage.TotalTokenCount.Value;
        }

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = tokensUsed,
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
/// Azure OpenAI provider implementation using Microsoft.Extensions.AI
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
        DataSanitizationService sanitizationService,
        CancellationToken cancellationToken = default)
    {
        // Sanitize user input before sending to external AI provider
        var promptValidation = sanitizationService.ValidateContent(prompt, tenantId);
        var messageValidation = sanitizationService.ValidateContent(userMessage, tenantId);

        // Log sanitization results
        if (promptValidation.DetectedPatterns.Any() || messageValidation.DetectedPatterns.Any())
        {
            _logger.LogInformation("AI request sanitized for tenant {TenantId}: Prompt patterns: {PromptPatterns}, Message patterns: {MessagePatterns}",
                tenantId,
                string.Join(", ", promptValidation.DetectedPatterns),
                string.Join(", ", messageValidation.DetectedPatterns));
        }

        // Block high-risk requests
        if (promptValidation.RiskLevel == RiskLevel.High || messageValidation.RiskLevel == RiskLevel.High)
        {
            _logger.LogWarning("Blocked high-risk AI request for tenant {TenantId} due to sensitive data detection", tenantId);
            throw new InvalidOperationException("Request contains sensitive data that cannot be sent to external AI providers");
        }

        // Get sanitized content
        var sanitizedPrompt = sanitizationService.SanitizeContent(prompt, tenantId).SanitizedContent;
        var sanitizedMessage = sanitizationService.SanitizeContent(userMessage, tenantId).SanitizedContent;

        var apiKey = await GetApiKeyAsync(tenantId, ProviderName);
        var endpoint = await GetEndpointAsync(tenantId);

        // Create Azure OpenAI client using Microsoft.Extensions.AI
        var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
            .AsChatClient(model);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, sanitizedPrompt),
            new ChatMessage(ChatRole.User, sanitizedMessage)
        };

        var response = await client.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        var tokensUsed = 0;
        if (response.Usage?.TotalTokenCount is not null)
        {
            tokensUsed = response.Usage.TotalTokenCount.Value;
        }

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = tokensUsed,
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

/// <summary>
/// Ollama provider implementation using OllamaSharp and Microsoft.Extensions.AI
/// Supports both local and network Ollama instances
/// </summary>
public class OllamaProvider : IAIProvider
{
    private readonly ILogger<OllamaProvider> _logger;
    private readonly IConfiguration _configuration;

    public string ProviderName => "ollama";

    public OllamaProvider(
        ILogger<OllamaProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        DataSanitizationService sanitizationService,
        CancellationToken cancellationToken = default)
    {
        // Sanitize user input before sending to external AI provider
        var promptValidation = sanitizationService.ValidateContent(prompt, tenantId);
        var messageValidation = sanitizationService.ValidateContent(userMessage, tenantId);

        // Log sanitization results
        if (promptValidation.DetectedPatterns.Any() || messageValidation.DetectedPatterns.Any())
        {
            _logger.LogInformation("AI request sanitized for tenant {TenantId}: Prompt patterns: {PromptPatterns}, Message patterns: {MessagePatterns}",
                tenantId,
                string.Join(", ", promptValidation.DetectedPatterns),
                string.Join(", ", messageValidation.DetectedPatterns));
        }

        // Block high-risk requests
        if (promptValidation.RiskLevel == RiskLevel.High || messageValidation.RiskLevel == RiskLevel.High)
        {
            _logger.LogWarning("Blocked high-risk AI request for tenant {TenantId} due to sensitive data detection", tenantId);
            throw new InvalidOperationException("Request contains sensitive data that cannot be sent to external AI providers");
        }

        // Get sanitized content
        var sanitizedPrompt = sanitizationService.SanitizeContent(prompt, tenantId).SanitizedContent;
        var sanitizedMessage = sanitizationService.SanitizeContent(userMessage, tenantId).SanitizedContent;

        var endpoint = GetEndpoint(tenantId);

        // Create Ollama client using OllamaSharp with Microsoft.Extensions.AI
        var ollamaClient = new OllamaSharp.OllamaApiClient(endpoint);
        var chatClient = ollamaClient.AsChatClient(model);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, sanitizedPrompt),
            new ChatMessage(ChatRole.User, sanitizedMessage)
        };

        var response = await chatClient.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        // Note: Ollama doesn't provide token usage in the same way as cloud providers
        // We estimate based on input/output length for consumption monitoring
        var estimatedTokens = EstimateTokenCount(sanitizedPrompt, sanitizedMessage, response.Message.Text ?? string.Empty);

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = estimatedTokens,
            Model = model
        };
    }

    private string GetEndpoint(string tenantId)
    {
        // Try tenant-specific Ollama endpoint
        var tenantEndpoint = _configuration[$"AI:Ollama:Endpoints:{tenantId}"];
        if (!string.IsNullOrEmpty(tenantEndpoint))
        {
            return tenantEndpoint;
        }

        // Try tenant-specific configuration section
        var tenantConfig = _configuration.GetSection($"AI:Ollama:Tenants:{tenantId}");
        if (tenantConfig.Exists())
        {
            var endpoint = tenantConfig["Endpoint"];
            if (!string.IsNullOrEmpty(endpoint))
            {
                return endpoint;
            }
        }

        // Default to localhost Ollama instance
        var defaultEndpoint = _configuration["AI:Ollama:DefaultEndpoint"] ?? "http://localhost:11434";
        return defaultEndpoint;
    }

    private int EstimateTokenCount(string systemPrompt, string userMessage, string response)
    {
        // Rough estimation: ~4 characters per token for English text
        // This is used for consumption monitoring when exact token counts aren't available
        var totalChars = systemPrompt.Length + userMessage.Length + response.Length;
        return (int)Math.Ceiling(totalChars / 4.0);
    }
}

/// <summary>
/// GitHub Models provider implementation using GitHub Models API (OpenAI-compatible)
/// Uses GitHub's AI models through their OpenAI-compatible endpoint
/// </summary>
public class GitHubModelsProvider : IAIProvider
{
    private readonly ILogger<GitHubModelsProvider> _logger;
    private readonly SecretClient? _secretClient;
    private readonly IConfiguration _configuration;

    public string ProviderName => "github-models";

    public GitHubModelsProvider(
        ILogger<GitHubModelsProvider> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

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
        DataSanitizationService sanitizationService,
        CancellationToken cancellationToken = default)
    {
        // Sanitize user input before sending to external AI provider
        var promptValidation = sanitizationService.ValidateContent(prompt, tenantId);
        var messageValidation = sanitizationService.ValidateContent(userMessage, tenantId);

        // Log sanitization results
        if (promptValidation.DetectedPatterns.Any() || messageValidation.DetectedPatterns.Any())
        {
            _logger.LogInformation("AI request sanitized for tenant {TenantId}: Prompt patterns: {PromptPatterns}, Message patterns: {MessagePatterns}",
                tenantId,
                string.Join(", ", promptValidation.DetectedPatterns),
                string.Join(", ", messageValidation.DetectedPatterns));
        }

        // Block high-risk requests
        if (promptValidation.RiskLevel == RiskLevel.High || messageValidation.RiskLevel == RiskLevel.High)
        {
            _logger.LogWarning("Blocked high-risk AI request for tenant {TenantId} due to sensitive data detection", tenantId);
            throw new InvalidOperationException("Request contains sensitive data that cannot be sent to external AI providers");
        }

        // Get sanitized content
        var sanitizedPrompt = sanitizationService.SanitizeContent(prompt, tenantId).SanitizedContent;
        var sanitizedMessage = sanitizationService.SanitizeContent(userMessage, tenantId).SanitizedContent;

        var apiKey = await GetApiKeyAsync(tenantId, ProviderName);

        // GitHub Models uses OpenAI-compatible API
        // Create OpenAI client pointing to GitHub Models endpoint
        var client = new OpenAIClient(
            new ApiKeyCredential(apiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://models.inference.ai.azure.com")
            }
        ).AsChatClient(model);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, sanitizedPrompt),
            new ChatMessage(ChatRole.User, sanitizedMessage)
        };

        var response = await client.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        var tokensUsed = 0;
        if (response.Usage?.TotalTokenCount is not null)
        {
            tokensUsed = response.Usage.TotalTokenCount.Value;
        }

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = tokensUsed,
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
        var envKey = Environment.GetEnvironmentVariable($"GITHUB_MODELS_API_KEY_{tenantId.ToUpper()}");
        if (!string.IsNullOrEmpty(envKey))
        {
            return envKey;
        }

        throw new InvalidOperationException($"API key not found for tenant {tenantId}, provider {provider}");
    }
}