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
    private readonly ILogger<AiProviderSelector> _logger;

    public AiProviderSelector(
        OpenAiProvider openAiProvider,
        AnthropicProvider anthropicProvider,
        AzureOpenAiProvider azureOpenAiProvider,
        ILogger<AiProviderSelector> logger)
    {
        _openAiProvider = openAiProvider;
        _anthropicProvider = anthropicProvider;
        _azureOpenAiProvider = azureOpenAiProvider;
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
            _ => _openAiProvider // Default fallback
        };
    }

    /// <summary>
    /// Get all available providers
    /// </summary>
    public IEnumerable<IAIProvider> GetAllProviders()
    {
        return new IAIProvider[] { _openAiProvider, _anthropicProvider, _azureOpenAiProvider };
    }
}