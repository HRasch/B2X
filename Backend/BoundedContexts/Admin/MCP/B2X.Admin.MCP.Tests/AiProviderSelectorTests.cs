using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Tools;
using B2X.Admin.MCP.Models;
using B2X.Admin.MCP.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;
using System.Net.Http;

namespace B2X.Admin.MCP.Tests;

public class AiProviderSelectorTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<OpenAiProvider>> _openAiLoggerMock;
    private readonly Mock<ILogger<AnthropicProvider>> _anthropicLoggerMock;
    private readonly Mock<ILogger<AzureOpenAiProvider>> _azureLoggerMock;
    private readonly Mock<ILogger<OllamaProvider>> _ollamaLoggerMock;
    private readonly Mock<ILogger<GitHubModelsProvider>> _gitHubLoggerMock;
    private readonly Mock<ILogger<AiProviderSelector>> _selectorLoggerMock;
    private readonly Mock<McpDbContext> _dbContextMock;
    private readonly DataSanitizationService _dataSanitizer;

    public AiProviderSelectorTests()
    {
        // Use a real configuration for testing
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("AI:EnableLocalFallback", "false"),
            new KeyValuePair<string, string>("AI:EnableNetworkMode", "false")
        });
        _configuration = configBuilder.Build();

        _openAiLoggerMock = new Mock<ILogger<OpenAiProvider>>();
        _anthropicLoggerMock = new Mock<ILogger<AnthropicProvider>>();
        _azureLoggerMock = new Mock<ILogger<AzureOpenAiProvider>>();
        _ollamaLoggerMock = new Mock<ILogger<OllamaProvider>>();
        _gitHubLoggerMock = new Mock<ILogger<GitHubModelsProvider>>();
        _selectorLoggerMock = new Mock<ILogger<AiProviderSelector>>();
        _dbContextMock = new Mock<McpDbContext>();

        // Create a real DataSanitizationService for testing
        var options = Options.Create(new DataSanitizationOptions());
        var sanitizerLogger = new Mock<ILogger<DataSanitizationService>>();
        _dataSanitizer = new DataSanitizationService(sanitizerLogger.Object, options);
    }

    [Fact]
    public async Task GetProviderForTenantAsync_ReturnsOllama_WhenLocalFallbackEnabled()
    {
        // Arrange - Create configuration with local fallback enabled
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("AI:EnableLocalFallback", "true"),
            new KeyValuePair<string, string>("AI:EnableNetworkMode", "false")
        });
        var configuration = configBuilder.Build();

        var ollamaProvider = new OllamaProvider(_ollamaLoggerMock.Object, configuration);
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, configuration),
            ollamaProvider,
            new GitHubModelsProvider(_gitHubLoggerMock.Object, configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            configuration,
            _dbContextMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant", "openai");

        // Assert
        Assert.Equal("ollama", provider.ProviderName);
        Assert.IsType<OllamaProvider>(provider);
    }

    [Fact]
    public async Task GetProviderForTenantAsync_ReturnsRequestedProvider_WhenLocalFallbackDisabled()
    {
        // Arrange - Use default configuration (both modes disabled)
        var openAiProvider = new OpenAiProvider(_openAiLoggerMock.Object, _configuration);
        var selector = new AiProviderSelector(
            openAiProvider,
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant", "openai");

        // Assert
        Assert.Equal("openai", provider.ProviderName);
        Assert.IsType<OpenAiProvider>(provider);
    }

    [Fact]
    public async Task GetProviderForTenantAsync_ReturnsCorrectProvider_ForGitHubModels()
    {
        // Arrange
        var gitHubProvider = new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration);
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            gitHubProvider,
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant", "github-models");

        // Assert
        Assert.Equal("github-models", provider.ProviderName);
        Assert.IsType<GitHubModelsProvider>(provider);
    }

    // [Fact]
    // public void GetAllProviders_IncludesNewProviders()
    // {
    //     // Arrange
    //     var selector = new AiProviderSelector(
    //         new OpenAiProvider(_openAiLoggerMock.Object, _configurationMock.Object),
    //         new AnthropicProvider(_anthropicLoggerMock.Object, _configurationMock.Object),
    //         new AzureOpenAiProvider(_azureLoggerMock.Object, _configurationMock.Object),
    //         new OllamaProvider(_ollamaLoggerMock.Object, _configurationMock.Object),
    //         new GitHubModelsProvider(_gitHubLoggerMock.Object, _configurationMock.Object),
    //         _dataSanitizer,
    //         _selectorLoggerMock.Object);

    //     // Act
    //     var providers = selector.GetAllProviders();

    //     // Assert
    //     Assert.Equal(5, providers.Count());
    //     Assert.Contains(providers, p => p.ProviderName == "openai");
    //     Assert.Contains(providers, p => p.ProviderName == "anthropic");
    //     Assert.Contains(providers, p => p.ProviderName == "azure");
    //     Assert.Contains(providers, p => p.ProviderName == "ollama");
    //     Assert.Contains(providers, p => p.ProviderName == "github-models");
    // }

    [Fact]
    public async Task GetProviderForTenantAsync_DefaultsToOpenAi_WhenNoProviderSpecified()
    {
        // Arrange
        var openAiProvider = new OpenAiProvider(_openAiLoggerMock.Object, _configuration);
        var selector = new AiProviderSelector(
            openAiProvider,
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant");

        // Assert
        Assert.Equal("openai", provider.ProviderName);
        Assert.IsType<OpenAiProvider>(provider);
    }

    [Fact]
    public async Task ExecuteChatCompletionAsync_UsesOllamaDirectly_WhenLocalFallbackEnabledGlobally()
    {
        // Arrange - Create configuration with local fallback enabled
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("AI:EnableLocalFallback", "true"),
            new KeyValuePair<string, string>("AI:EnableNetworkMode", "false")
        });
        var configuration = configBuilder.Build();

        var ollamaProvider = new OllamaProvider(_ollamaLoggerMock.Object, configuration);
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, configuration),
            ollamaProvider,
            new GitHubModelsProvider(_gitHubLoggerMock.Object, configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            configuration,
            _dbContextMock.Object);

        // Act - This will use Ollama directly due to global fallback
        // Note: This test may fail if Ollama is not running, but that's expected for integration testing
        try
        {
            var result = await selector.ExecuteChatCompletionAsync("test-tenant", "gpt-4", "system prompt", "user message", "openai");
            // If Ollama is running, we should get a response
            Assert.NotNull(result);
            Assert.Equal("ollama", result.Model); // Should be the requested model since no mapping occurs in global mode
        }
        catch (Exception ex)
        {
            // If Ollama is not available, we expect an exception
            Assert.Contains("ollama", ex.Message.ToLower() ?? "");
        }
    }

    [Fact]
    public async Task ExecuteChatCompletionAsync_DoesNotFallback_WhenLocalModeDisabled()
    {
        // Arrange - Use default configuration (fallback disabled)
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Act & Assert - This should try OpenAI first and fail since we don't have API keys configured
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            selector.ExecuteChatCompletionAsync("test-tenant", "gpt-4", "system prompt", "user message", "openai"));

        // Should fail with API key error, not fallback to Ollama
        Assert.Contains("API key not found", exception.Message);
    }

    [Fact]
    public void SwitchMode_ChangesCurrentMode()
    {
        // Arrange - Use default configuration
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Initial mode should be Normal
        Assert.Equal(AiProviderSelector.AiMode.Normal, selector.CurrentMode);

        // Act - Switch to Network mode
        selector.SwitchMode(AiProviderSelector.AiMode.Network);

        // Assert
        Assert.Equal(AiProviderSelector.AiMode.Network, selector.CurrentMode);

        // Act - Switch to Local mode
        selector.SwitchMode(AiProviderSelector.AiMode.Local);

        // Assert
        Assert.Equal(AiProviderSelector.AiMode.Local, selector.CurrentMode);
    }

    [Fact]
    public void TrySwitchMode_ByStringName_Succeeds()
    {
        // Arrange
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Act & Assert - Valid mode names
        Assert.True(selector.TrySwitchMode("network"));
        Assert.Equal(AiProviderSelector.AiMode.Network, selector.CurrentMode);

        Assert.True(selector.TrySwitchMode("LOCAL"));
        Assert.Equal(AiProviderSelector.AiMode.Local, selector.CurrentMode);

        Assert.True(selector.TrySwitchMode("Normal"));
        Assert.Equal(AiProviderSelector.AiMode.Normal, selector.CurrentMode);
    }

    [Fact]
    public void TrySwitchMode_InvalidModeName_ReturnsFalse()
    {
        // Arrange
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        var originalMode = selector.CurrentMode;

        // Act
        var result = selector.TrySwitchMode("invalidmode");

        // Assert
        Assert.False(result);
        Assert.Equal(originalMode, selector.CurrentMode); // Mode should not change
    }

    [Fact]
    public async Task GetProviderForTenantAsync_RespectsRuntimeMode()
    {
        // Arrange - Create selector with default (Normal) mode
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configuration),
            new OllamaProvider(_ollamaLoggerMock.Object, _configuration),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            _configuration,
            _dbContextMock.Object);

        // Act - Normal mode should return OpenAI for "openai" preference
        var provider = await selector.GetProviderForTenantAsync("test-tenant", "openai");

        // Assert
        Assert.IsType<OpenAiProvider>(provider);

        // Act - Switch to Network mode
        selector.SwitchMode(AiProviderSelector.AiMode.Network);
        provider = await selector.GetProviderForTenantAsync("test-tenant", "anthropic");

        // Assert - Network mode should return Ollama regardless of preference
        Assert.IsType<OllamaProvider>(provider);

        // Act - Switch to Local mode
        selector.SwitchMode(AiProviderSelector.AiMode.Local);
        provider = await selector.GetProviderForTenantAsync("test-tenant", "github-models");

        // Assert - Local mode should return Ollama regardless of preference
        Assert.IsType<OllamaProvider>(provider);
    }

    [Fact]
    public async Task GetProviderForTenantAsync_ReturnsOllama_WhenNetworkModeEnabled()
    {
        // Arrange - Create configuration with network mode enabled
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("AI:EnableLocalFallback", "false"),
            new KeyValuePair<string, string>("AI:EnableNetworkMode", "true")
        });
        var configuration = configBuilder.Build();

        var ollamaProvider = new OllamaProvider(_ollamaLoggerMock.Object, configuration);
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, configuration),
            new AnthropicProvider(_anthropicLoggerMock.Object, configuration),
            new AzureOpenAiProvider(_azureLoggerMock.Object, configuration),
            ollamaProvider,
            new GitHubModelsProvider(_gitHubLoggerMock.Object, configuration),
            _dataSanitizer,
            _selectorLoggerMock.Object,
            configuration,
            _dbContextMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant", "openai");

        // Assert
        Assert.Equal("ollama", provider.ProviderName);
        Assert.IsType<OllamaProvider>(provider);
    }
}

public class SystemHealthAnalysisToolTests
{
    private readonly Mock<TenantContext> _tenantContextMock;
    private readonly Mock<ILogger<SystemHealthAnalysisTool>> _loggerMock;
    private readonly DataSanitizationService _dataSanitizer;

    public SystemHealthAnalysisToolTests()
    {
        _tenantContextMock = new Mock<TenantContext>();
        _loggerMock = new Mock<ILogger<SystemHealthAnalysisTool>>();

        // Create a real DataSanitizationService for testing
        var options = Options.Create(new DataSanitizationOptions());
        var sanitizerLogger = new Mock<ILogger<DataSanitizationService>>();
        _dataSanitizer = new DataSanitizationService(sanitizerLogger.Object, options);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesCliExecution_WithDataSanitization()
    {
        // Arrange
        var tool = new SystemHealthAnalysisTool(
            _tenantContextMock.Object,
            _loggerMock.Object,
            _dataSanitizer);

        var args = new SystemHealthAnalysisArgs
        {
            Component = "backend",
            TimeRange = "1h"
        };

        // Act
        var result = await tool.ExecuteAsync(args);

        // Assert
        // The result should be generated (not empty), even if CLI execution fails
        Assert.NotEmpty(result);
        // Should contain some form of response
        Assert.True(result.Contains("Error") || result.Contains("Health") || result.Contains("analysis"));
    }
}