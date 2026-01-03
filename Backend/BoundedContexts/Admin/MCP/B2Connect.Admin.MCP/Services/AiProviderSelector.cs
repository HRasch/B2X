using B2Connect.Admin.MCP.Services;
using Microsoft.Extensions.Logging;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// AI Provider Selector - Selects the appropriate AI provider based on tenant configuration
/// </summary>
public class AiProviderSelector
{
    private readonly OpenAiProvider _openAiProvider;
    private readonly AnthropicProvider _anthropicProvider;
    private readonly AzureOpenAiProvider _azureOpenAiProvider;
    private readonly OllamaProvider _ollamaProvider;
    private readonly GitHubModelsProvider _gitHubModelsProvider;
    private readonly DataSanitizationService _sanitizationService;
    private readonly ILogger<AiProviderSelector> _logger;

    public AiProviderSelector(
        OpenAiProvider openAiProvider,
        AnthropicProvider anthropicProvider,
        AzureOpenAiProvider azureOpenAiProvider,
        OllamaProvider ollamaProvider,
        GitHubModelsProvider gitHubModelsProvider,
        DataSanitizationService sanitizationService,
        ILogger<AiProviderSelector> logger)
    {
        _openAiProvider = openAiProvider;
        _anthropicProvider = anthropicProvider;
        _azureOpenAiProvider = azureOpenAiProvider;
        _ollamaProvider = ollamaProvider;
        _gitHubModelsProvider = gitHubModelsProvider;
        _sanitizationService = sanitizationService;
        _logger = logger;
    }

    /// <summary>
    /// Get the AI provider for a tenant based on their configuration
    /// </summary>
    public async Task<IAIProvider> GetProviderForTenantAsync(string tenantId, string preferredProvider = null)
    {
        // TODO: Retrieve tenant's preferred provider from database
        // For now, use preferredProvider parameter or default to OpenAI

        var providerName = preferredProvider ?? "openai";

        return providerName.ToLower() switch
        {
            "openai" => _openAiProvider,
            "anthropic" => _anthropicProvider,
            "azure" => _azureOpenAiProvider,
            "ollama" => _ollamaProvider,
            "github-models" => _gitHubModelsProvider,
            _ => _openAiProvider // Default fallback
        };
    }

    /// <summary>
    /// Execute AI chat completion with automatic sanitization and provider selection
    /// </summary>
    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        string preferredProvider = null,
        CancellationToken cancellationToken = default)
    {
        var provider = await GetProviderForTenantAsync(tenantId, preferredProvider);

        try
        {
            return await provider.ExecuteChatCompletionAsync(
                tenantId,
                model,
                prompt,
                userMessage,
                _sanitizationService,
                cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("sensitive data"))
        {
            // Re-throw sanitization-related exceptions
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI provider {Provider} failed for tenant {TenantId}",
                provider.ProviderName, tenantId);
            throw new InvalidOperationException($"AI provider {provider.ProviderName} failed: {ex.Message}", ex);
        }
    }
}