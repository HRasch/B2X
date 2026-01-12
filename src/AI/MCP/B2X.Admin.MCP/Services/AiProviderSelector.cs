using B2X.Admin.MCP.Data;
using B2X.Admin.MCP.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Services;

/// <summary>
/// AI Provider Selector - Selects the appropriate AI provider based on tenant configuration
/// Supports automatic fallback to local Ollama when rate limited and local mode is enabled
/// Supports network mode for using network-hosted Ollama servers
/// Supports runtime mode switching between normal, local, and network modes
/// PRIORITIZES CLIENT-SIDE AI SERVICES: If tenant has configured client-side AI (Ollama/LM Studio), ALL traffic routes there
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
    private readonly IConfiguration _configuration;
    private readonly McpDbContext _dbContext;

    // Runtime mode switching support
    private AiMode _currentMode = AiMode.Normal;
    private readonly object _modeLock = new();

    public AiProviderSelector(
        OpenAiProvider openAiProvider,
        AnthropicProvider anthropicProvider,
        AzureOpenAiProvider azureOpenAiProvider,
        OllamaProvider ollamaProvider,
        GitHubModelsProvider gitHubModelsProvider,
        DataSanitizationService sanitizationService,
        ILogger<AiProviderSelector> logger,
        IConfiguration configuration,
        McpDbContext dbContext)
    {
        _openAiProvider = openAiProvider;
        _anthropicProvider = anthropicProvider;
        _azureOpenAiProvider = azureOpenAiProvider;
        _ollamaProvider = ollamaProvider;
        _gitHubModelsProvider = gitHubModelsProvider;
        _sanitizationService = sanitizationService;
        _logger = logger;
        _configuration = configuration;
        _dbContext = dbContext;

        // Initialize mode from configuration
        InitializeModeFromConfiguration();
    }

    /// <summary>
    /// AI mode enumeration
    /// </summary>
    public enum AiMode
    {
        Normal,     // Use preferred providers (OpenAI, Anthropic, etc.)
        Local,      // Force all agents to use local Ollama
        Network     // Force all agents to use network-hosted Ollama
    }

    /// <summary>
    /// Get the current AI mode
    /// </summary>
    public AiMode CurrentMode
    {
        get
        {
            lock (_modeLock)
            {
                return _currentMode;
            }
        }
    }

    /// <summary>
    /// Initialize the mode from configuration on startup
    /// </summary>
    private void InitializeModeFromConfiguration()
    {
        var networkModeEnabled = _configuration.GetValue<bool>("AI:EnableNetworkMode", false);
        var localModeEnabled = _configuration.GetValue<bool>("AI:EnableLocalFallback", false);

        lock (_modeLock)
        {
            if (networkModeEnabled)
            {
                _currentMode = AiMode.Network;
            }
            else if (localModeEnabled)
            {
                _currentMode = AiMode.Local;
            }
            else
            {
                _currentMode = AiMode.Normal;
            }
        }

        _logger.LogInformation("AI Provider Selector initialized with mode: {Mode}", _currentMode);
    }

    /// <summary>
    /// Switch to a different AI mode at runtime
    /// </summary>
    public void SwitchMode(AiMode newMode)
    {
        lock (_modeLock)
        {
            var oldMode = _currentMode;
            _currentMode = newMode;
            _logger.LogInformation("AI mode switched from {OldMode} to {NewMode}", oldMode, newMode);
        }
    }

    /// <summary>
    /// Switch mode by string name (for API compatibility)
    /// </summary>
    public bool TrySwitchMode(string modeName)
    {
        if (Enum.TryParse<AiMode>(modeName, true, out var mode))
        {
            SwitchMode(mode);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get the AI provider for a tenant based on their configuration
    /// PRIORITY: Client-side AI services (Ollama, LM Studio) if configured by tenant
    /// When local fallback is enabled, ALL agents use Ollama regardless of preference
    /// When network mode is enabled, ALL agents use network-hosted Ollama servers
    /// </summary>
    public async Task<IAIProvider> GetProviderForTenantAsync(string tenantId, string? preferredProvider = null)
    {
        AiMode currentMode;
        lock (_modeLock)
        {
            currentMode = _currentMode;
        }

        // Check current runtime mode first (overrides tenant preferences)
        if (currentMode == AiMode.Network)
        {
            _logger.LogInformation("Network AI mode active for tenant {TenantId}. All agents will use network-hosted Ollama provider.", tenantId);
            return _ollamaProvider;
        }

        if (currentMode == AiMode.Local)
        {
            _logger.LogInformation("Local AI mode active for tenant {TenantId}. All agents will use Ollama provider.", tenantId);
            return _ollamaProvider;
        }

        // CRITICAL: Check if tenant has configured client-side AI services
        // If tenant has configured Ollama or LM Studio, ALL their AI traffic must use client-side AI
        var tenantAiConfigs = await _dbContext.TenantAiConfigurations
            .Where(t => t.TenantId == tenantId && t.IsActive)
            .ToListAsync();

        // Check for client-side AI configurations (Ollama, LM Studio)
        var clientSideConfig = tenantAiConfigs.FirstOrDefault(t =>
            t.Provider.ToLower() is "ollama" or "lmstudio");

        if (clientSideConfig != null)
        {
            _logger.LogInformation("Tenant {TenantId} has configured client-side AI ({Provider}). All AI traffic will use client-side services.",
                tenantId, clientSideConfig.Provider);

            return clientSideConfig.Provider.ToLower() switch
            {
                "ollama" => _ollamaProvider,
                "lmstudio" => _ollamaProvider, // LM Studio also uses Ollama protocol
                _ => _ollamaProvider
            };
        }

        // No client-side AI configured, use tenant's preferred provider or fallback
        var tenantConfig = tenantAiConfigs.FirstOrDefault();
        if (tenantConfig != null)
        {
            _logger.LogInformation("Using tenant {TenantId} configured provider: {Provider}", tenantId, tenantConfig.Provider);
            return tenantConfig.Provider.ToLower() switch
            {
                "openai" => _openAiProvider,
                "anthropic" => _anthropicProvider,
                "azure" => _azureOpenAiProvider,
                "ollama" => _ollamaProvider,
                "github-models" => _gitHubModelsProvider,
                _ => _openAiProvider // Default fallback
            };
        }

        // No tenant configuration found, use preferredProvider parameter or default to OpenAI
        var providerName = preferredProvider ?? "openai";
        _logger.LogInformation("No tenant AI configuration found for {TenantId}, using provider: {Provider}", tenantId, providerName);

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
    /// When network mode is enabled globally, ALL agents automatically use network-hosted Ollama
    /// When local fallback is enabled globally, ALL agents automatically use Ollama
    /// Additional rate limiting fallback as secondary protection for edge cases
    /// </summary>
    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId,
        string model,
        string prompt,
        string userMessage,
        string? preferredProvider = null,
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
            // Check if current mode allows fallback and this is a rate limiting error
            var isRateLimited = IsRateLimitingException(ex);
            AiMode currentMode;
            lock (_modeLock)
            {
                currentMode = _currentMode;
            }

            // Allow fallback to Ollama if we're not already in Local or Network mode
            var fallbackAllowed = currentMode == AiMode.Normal && (isRateLimited || currentMode != AiMode.Local);

            if (fallbackAllowed && provider.ProviderName != "ollama")
            {
                _logger.LogWarning(ex, "Rate limit detected for provider {Provider} or fallback requested. Switching to Ollama for tenant {TenantId}",
                    provider.ProviderName, tenantId);

                try
                {
                    // Fallback to Ollama with equivalent model mapping
                    var ollamaModel = MapToOllamaModel(model, provider.ProviderName);
                    return await _ollamaProvider.ExecuteChatCompletionAsync(
                        tenantId,
                        ollamaModel,
                        prompt,
                        userMessage,
                        _sanitizationService,
                        cancellationToken);
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Ollama fallback also failed for tenant {TenantId}", tenantId);
                    throw new InvalidOperationException($"Both primary provider {provider.ProviderName} and Ollama fallback failed. Primary error: {ex.Message}", ex);
                }
            }

            _logger.LogError(ex, "AI provider {Provider} failed for tenant {TenantId}",
                provider.ProviderName, tenantId);
            throw new InvalidOperationException($"AI provider {provider.ProviderName} failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Determines if an exception indicates rate limiting
    /// </summary>
    private bool IsRateLimitingException(Exception ex)
    {
        // Check for common rate limiting indicators
        var message = ex.Message.ToLowerInvariant();
        return message.Contains("rate limit") ||
               message.Contains("too many requests") ||
               message.Contains("429") ||
               message.Contains("quota exceeded") ||
               ex is HttpRequestException httpEx && httpEx.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
    }

    /// <summary>
    /// Maps external AI models to equivalent Ollama models
    /// </summary>
    private string MapToOllamaModel(string originalModel, string providerName)
    {
        // Model mapping based on capabilities and size
        return (providerName.ToLower(), originalModel.ToLower()) switch
        {
            // GitHub Models / OpenAI GPT-4 level -> DeepSeek Coder
            ("github-models" or "openai", _) when originalModel.Contains("gpt-4") => "deepseek-coder:33b",
            ("github-models" or "openai", _) when originalModel.Contains("gpt-3.5") => "qwen2.5-coder:14b",

            // Anthropic Claude -> Qwen (good reasoning)
            ("anthropic", _) when originalModel.Contains("claude-3-5") => "qwen3:30b",
            ("anthropic", _) when originalModel.Contains("claude-3") => "qwen2.5:14b",

            // Azure OpenAI -> DeepSeek Coder
            ("azure", _) => "deepseek-coder:33b",

            // Default fallback
            _ => "deepseek-coder:33b"
        };
    }
}
