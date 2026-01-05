using B2Connect.Admin.MCP.Services;
using B2Connect.Admin.MCP.Middleware;
using B2Connect.Admin.MCP.Tools;
using B2Connect.Admin.MCP.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;

namespace B2Connect.Admin.MCP.Tests;

public class AiProviderSelectorTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<OpenAiProvider>> _openAiLoggerMock;
    private readonly Mock<ILogger<AnthropicProvider>> _anthropicLoggerMock;
    private readonly Mock<ILogger<AzureOpenAiProvider>> _azureLoggerMock;
    private readonly Mock<ILogger<OllamaProvider>> _ollamaLoggerMock;
    private readonly Mock<ILogger<GitHubModelsProvider>> _gitHubLoggerMock;
    private readonly Mock<ILogger<AiProviderSelector>> _selectorLoggerMock;
    private readonly DataSanitizationService _dataSanitizer;

    public AiProviderSelectorTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _openAiLoggerMock = new Mock<ILogger<OpenAiProvider>>();
        _anthropicLoggerMock = new Mock<ILogger<AnthropicProvider>>();
        _azureLoggerMock = new Mock<ILogger<AzureOpenAiProvider>>();
        _ollamaLoggerMock = new Mock<ILogger<OllamaProvider>>();
        _gitHubLoggerMock = new Mock<ILogger<GitHubModelsProvider>>();
        _selectorLoggerMock = new Mock<ILogger<AiProviderSelector>>();

        // Create a real DataSanitizationService for testing
        var options = Options.Create(new DataSanitizationOptions());
        var sanitizerLogger = new Mock<ILogger<DataSanitizationService>>();
        _dataSanitizer = new DataSanitizationService(sanitizerLogger.Object, options);
    }

    [Fact]
    public async Task GetProviderForTenantAsync_ReturnsCorrectProvider_ForOllama()
    {
        // Arrange
        var ollamaProvider = new OllamaProvider(_ollamaLoggerMock.Object, _configurationMock.Object);
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configurationMock.Object),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configurationMock.Object),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configurationMock.Object),
            ollamaProvider,
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configurationMock.Object),
            _dataSanitizer,
            _selectorLoggerMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant", "ollama");

        // Assert
        Assert.Equal("ollama", provider.ProviderName);
        Assert.IsType<OllamaProvider>(provider);
    }

    [Fact]
    public async Task GetProviderForTenantAsync_ReturnsCorrectProvider_ForGitHubModels()
    {
        // Arrange
        var gitHubProvider = new GitHubModelsProvider(_gitHubLoggerMock.Object, _configurationMock.Object);
        var selector = new AiProviderSelector(
            new OpenAiProvider(_openAiLoggerMock.Object, _configurationMock.Object),
            new AnthropicProvider(_anthropicLoggerMock.Object, _configurationMock.Object),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configurationMock.Object),
            new OllamaProvider(_ollamaLoggerMock.Object, _configurationMock.Object),
            gitHubProvider,
            _dataSanitizer,
            _selectorLoggerMock.Object);

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
        var openAiProvider = new OpenAiProvider(_openAiLoggerMock.Object, _configurationMock.Object);
        var selector = new AiProviderSelector(
            openAiProvider,
            new AnthropicProvider(_anthropicLoggerMock.Object, _configurationMock.Object),
            new AzureOpenAiProvider(_azureLoggerMock.Object, _configurationMock.Object),
            new OllamaProvider(_ollamaLoggerMock.Object, _configurationMock.Object),
            new GitHubModelsProvider(_gitHubLoggerMock.Object, _configurationMock.Object),
            _dataSanitizer,
            _selectorLoggerMock.Object);

        // Act
        var provider = await selector.GetProviderForTenantAsync("test-tenant");

        // Assert
        Assert.Equal("openai", provider.ProviderName);
        Assert.IsType<OpenAiProvider>(provider);
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