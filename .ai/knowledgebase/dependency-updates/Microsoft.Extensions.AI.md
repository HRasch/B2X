---
docid: KB-091
title: Microsoft.Extensions.AI
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Microsoft.Extensions.AI - Unified AI Abstractions

**Overview**: .NET developers need to integrate and interact with a growing variety of artificial intelligence (AI) services in their apps. The `Microsoft.Extensions.AI` libraries provide a unified approach for representing generative AI components, and enable seamless integration and interoperability with various AI services.

**DocID**: `KB-024`  
**Last Updated**: 11. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Package Versions (January 2026)

| Package | Latest Stable | Preview | Notes |
|---------|--------------|---------|-------|
| `Microsoft.Extensions.AI.Abstractions` | **10.1.1** | - | Core interfaces and types |
| `Microsoft.Extensions.AI` | **10.1.1** | - | High-level utilities and middleware |
| `Microsoft.Extensions.AI.Evaluation` | - | **10.1.0** | AI response evaluation and quality assessment |
| `Microsoft.Extensions.AI.OpenAI` | - | **10.1.1-preview.1.25612.2** | OpenAI integration |
| `Microsoft.Extensions.AI.AzureAIInference` | - | **10.0.0-preview.1.25559.3** | Azure AI Inference |
| `Microsoft.Extensions.AI.Ollama` | - | **10.1.1-preview.1.25612.2** | Ollama integration |

## Key Interfaces and Functionality

### IChatClient
The `IChatClient` interface defines a client abstraction responsible for interacting with AI services that provide chat capabilities. It includes methods for sending and receiving messages with multi-modal content (such as text, images, and audio), either as a complete set or streamed incrementally.

### IEmbeddingGenerator<TInput,TEmbedding>
The `IEmbeddingGenerator<TInput,TEmbedding>` interface represents a generic generator of embeddings. For the generic type parameters, `TInput` is the type of input values being embedded, and `TEmbedding` is the type of generated embedding, which inherits from the `Embedding` class.

### IImageGenerator (Experimental)
The `IImageGenerator` interface represents a generator for creating images from text prompts or other input. This interface enables applications to integrate image generation capabilities from various AI services through a consistent API.

### Evaluation Framework

The `Microsoft.Extensions.AI.Evaluation` package provides comprehensive evaluation capabilities for AI responses, including quality assessment, safety checks, and NLP-based similarity analysis.

Key components:
- **IEvaluator**: Interface for implementing custom evaluators
- **EvaluationResult**: Structured results with scores and metrics
- **EvaluationThresholds**: Configurable pass/fail criteria
- **Built-in Evaluators**: Quality, Safety, Relevance, Groundedness

Usage:
```csharp
var evaluator = new QualityEvaluator();
var result = await evaluator.EvaluateAsync(chatResponse, evaluationContext);
if (result.Score < threshold) { /* handle low quality */ }
```

This is used in B2X for automated testing of AI integrations and ensuring response quality across tenant configurations.

## Middleware and Pipeline Features

The `Microsoft.Extensions.AI` package enables easy integration of components such as automatic function tool invocation, telemetry, and caching into applications using familiar dependency injection and middleware patterns.

### ChatClientBuilder
A builder for creating pipelines of `IChatClient` with middleware:

```csharp
using Microsoft.Extensions.AI;

var chatClient = new ChatClientBuilder()
    .UseOpenTelemetry()           // Add OpenTelemetry support
    .UseDistributedCache()        // Add distributed caching
    .UseFunctionInvocation()      // Enable function calling
    .UseLogging()                 // Add logging
    .Use(new OpenAI.Chat.ChatClient("gpt-4", apiKey))
    .Build();

var response = await chatClient.GetResponseAsync("Hello, AI!");
```

### Function Calling and Tools
- **FunctionInvocationChatClient**: Automatically invokes functions defined on ChatOptions
- **AIFunction**: Represents a function that can be described to an AI service and invoked
- **Tool Reduction**: Strategies for selecting reduced sets of tools for chat requests

### Caching and Performance
- **Distributed Caching**: Store results in IDistributedCache
- **Memory Caching**: In-memory caching for embeddings and chat responses

### Telemetry Integration
- **OpenTelemetry**: Semantic conventions for Generative AI systems
- **Logging**: Structured logging for AI operations
- **Metrics**: Usage tracking and performance monitoring

### Telemetry Integration
- **OpenTelemetry**: Semantic conventions for Generative AI systems
- **Logging**: Structured logging for AI operations
- **Metrics**: Usage tracking and performance monitoring

## Structured Output and Response Formats

```csharp
using Microsoft.Extensions.AI;

// Request structured JSON output
var response = await chatClient.GetResponseAsync(
    messages,
    new ChatOptions
    {
        ResponseFormat = ChatResponseFormat.Json,
        ResponseSchema = typeof(MyResponseType) // For strongly-typed responses
    });

// Strongly-typed response
var typedResponse = await chatClient.GetResponseAsync<MyResponseType>(messages);
```

## Multi-Modal Content Support

```csharp
using Microsoft.Extensions.AI;

// Text and image content
var messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.User, new AIContent[]
    {
        new TextContent("What's in this image?"),
        new DataContent(imageBytes, "image/jpeg")
    })
};

var response = await chatClient.GetResponseAsync(messages);
```

## Function Calling and Tools

```csharp
using Microsoft.Extensions.AI;

// Define a function
var weatherFunction = AIFunctionFactory.Create(
    (string location) => GetWeatherAsync(location),
    "GetWeather",
    "Gets the current weather for a location");

// Add to chat options
var options = new ChatOptions
{
    Tools = [weatherFunction]
};

// The client will automatically invoke functions when needed
var response = await chatClient.GetResponseAsync(messages, options);
```

## Streaming Responses

```csharp
using Microsoft.Extensions.AI;

// Stream responses incrementally
await foreach (var update in chatClient.GetStreamingResponseAsync(messages))
{
    Console.Write(update.Text);
}
```

## Provider Implementations

### OpenAI
```csharp
IChatClient client = new OpenAI.Chat.ChatClient(model, apiKey).AsIChatClient();
```

### Azure AI Inference
```csharp
IChatClient client = new AzureAIInferenceChatClient(endpoint, apiKey).AsIChatClient();
```

### Ollama
```csharp
IChatClient client = new OllamaChatClient(endpoint, modelName).AsIChatClient();
```

## Integration in B2X Project

The B2X project extensively uses Microsoft.Extensions.AI for:

- **Multi-Provider AI Support**: OpenAI, Azure OpenAI, Anthropic, Ollama, GitHub Models
- **Function Calling**: Dynamic tool invocation for ERP integrations and data processing
- **Caching**: Performance optimization for repeated AI requests
- **Telemetry**: Comprehensive observability of AI operations
- **Structured Output**: Type-safe responses for API contracts
- **Multitenant AI Services**: Tenant-specific AI configurations and rate limiting

### Recommended Usage Patterns

1. **Always use abstractions**: Reference `Microsoft.Extensions.AI.Abstractions` for interfaces
2. **Build pipelines**: Use `ChatClientBuilder` for middleware composition
3. **Enable telemetry**: Add OpenTelemetry for observability
4. **Implement caching**: Use distributed caching for performance
5. **Handle structured output**: Use typed responses when possible

### B2X-Specific Patterns

#### Multitenant AI Service Factory
```csharp
public class TenantAiServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<TenantAiServiceFactory> _logger;

    public TenantAiServiceFactory(
        IServiceProvider serviceProvider,
        ITenantContext tenantContext,
        ILogger<TenantAiServiceFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public IChatClient CreateChatClient()
    {
        var tenantConfig = GetTenantAiConfiguration();

        return new ChatClientBuilder()
            .Use(new TenantRateLimitingMiddleware(tenantConfig.RateLimit))
            .UseDistributedCache()
            .UseOpenTelemetry()
            .Use(tenantConfig.Provider switch
            {
                AiProvider.OpenAI => CreateOpenAiClient(tenantConfig),
                AiProvider.Azure => CreateAzureClient(tenantConfig),
                AiProvider.Ollama => CreateOllamaClient(tenantConfig),
                _ => throw new NotSupportedException($"Provider {tenantConfig.Provider} not supported")
            })
            .Build();
    }

    private TenantAiConfiguration GetTenantAiConfiguration()
    {
        // Load tenant-specific AI configuration from database/cache
        return _tenantContext.GetConfiguration<TenantAiConfiguration>();
    }
}

public class TenantAiConfiguration
{
    public AiProvider Provider { get; set; }
    public string ApiKey { get; set; }
    public string Endpoint { get; set; }
    public string Model { get; set; }
    public int RateLimit { get; set; } // requests per minute
}

public enum AiProvider { OpenAI, Azure, Ollama }
```

#### ERP Integration with Function Calling
```csharp
public class ErpIntegrationService
{
    private readonly IChatClient _chatClient;
    private readonly IErpClient _erpClient;
    private readonly ILogger<ErpIntegrationService> _logger;

    public ErpIntegrationService(
        IChatClient chatClient,
        IErpClient erpClient,
        ILogger<ErpIntegrationService> logger)
    {
        _chatClient = chatClient;
        _erpClient = erpClient;
        _logger = logger;
    }

    public async Task<string> ProcessNaturalLanguageQuery(string userQuery)
    {
        var chatOptions = new ChatOptions
        {
            Tools = new[]
            {
                AIFunctionFactory.Create(GetProductInfoAsync, "Get product information from ERP"),
                AIFunctionFactory.Create(GetCustomerInfoAsync, "Get customer information from ERP"),
                AIFunctionFactory.Create(CreateOrderAsync, "Create a new order in ERP")
            }
        };

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an ERP assistant. Use the available tools to help users with ERP operations."),
            new ChatMessage(ChatRole.User, userQuery)
        };

        var response = await _chatClient.CompleteAsync(messages, chatOptions);

        return response.Message.Text ?? "I couldn't process your request.";
    }

    [Function("GetProductInfo")]
    private async Task<string> GetProductInfoAsync(string productId)
    {
        _logger.LogInformation("AI requested product info for {ProductId}", productId);
        var product = await _erpClient.GetProductAsync(productId);
        return JsonSerializer.Serialize(product);
    }

    [Function("GetCustomerInfo")]
    private async Task<string> GetCustomerInfoAsync(string customerId)
    {
        _logger.LogInformation("AI requested customer info for {CustomerId}", customerId);
        var customer = await _erpClient.GetCustomerAsync(customerId);
        return JsonSerializer.Serialize(customer);
    }

    [Function("CreateOrder")]
    private async Task<string> CreateOrderAsync(string orderJson)
    {
        _logger.LogInformation("AI requested order creation");
        var order = JsonSerializer.Deserialize<OrderRequest>(orderJson);
        var result = await _erpClient.CreateOrderAsync(order);
        return $"Order created with ID: {result.OrderId}";
    }
}
```

#### Structured Output for API Contracts
```csharp
public class AiContractGenerator
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<AiContractGenerator> _logger;

    public AiContractGenerator(
        IChatClient chatClient,
        ILogger<AiContractGenerator> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    public async Task<ApiContract> GenerateContractAsync(string requirements)
    {
        var contractSchema = new
        {
            type = "object",
            properties = new
            {
                name = new { type = "string" },
                description = new { type = "string" },
                endpoints = new
                {
                    type = "array",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            path = new { type = "string" },
                            method = new { type = "string", @enum = new[] { "GET", "POST", "PUT", "DELETE" } },
                            parameters = new { type = "array", items = new { type = "string" } },
                            response = new { type = "string" }
                        },
                        required = new[] { "path", "method" }
                    }
                }
            },
            required = new[] { "name", "description", "endpoints" }
        };

        var chatOptions = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json,
            ModelId = "gpt-4-turbo-preview" // Use structured output capable model
        };

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, $"Generate an API contract based on the requirements. Respond with valid JSON matching this schema: {JsonSerializer.Serialize(contractSchema)}"),
            new ChatMessage(ChatRole.User, requirements)
        };

        var response = await _chatClient.CompleteAsync(messages, chatOptions);

        _logger.LogInformation("Generated API contract using AI");

        return JsonSerializer.Deserialize<ApiContract>(
            response.Message.Text ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}

public class ApiContract
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ApiEndpoint> Endpoints { get; set; } = new();
}

public class ApiEndpoint
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public string Response { get; set; } = string.Empty;
}
```

#### AI-Powered Data Transformation
```csharp
public class DataTransformationService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<DataTransformationService> _logger;

    public DataTransformationService(
        IChatClient chatClient,
        ILogger<DataTransformationService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    public async Task<TTarget> TransformDataAsync<TSource, TTarget>(
        TSource sourceData,
        string transformationInstructions)
    {
        var sourceJson = JsonSerializer.Serialize(sourceData);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, $"Transform the following JSON data according to these instructions: {transformationInstructions}. Respond only with valid JSON that matches the target schema."),
            new ChatMessage(ChatRole.User, sourceJson)
        };

        var chatOptions = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };

        var response = await _chatClient.CompleteAsync(messages, chatOptions);

        _logger.LogInformation("AI transformed data from {SourceType} to {TargetType}",
            typeof(TSource).Name, typeof(TTarget).Name);

        return JsonSerializer.Deserialize<TTarget>(
            response.Message.Text ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}
```

#### Telemetry and Observability
```csharp
public static class AiServiceCollectionExtensions
{
    public static IServiceCollection AddAiObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource("Microsoft.Extensions.AI")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4317");
                    });
            })
            .WithMetrics(metricsProviderBuilder =>
            {
                metricsProviderBuilder
                    .AddMeter("Microsoft.Extensions.AI")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4317");
                    });
            });

        return services;
    }
}
```

## Version Compatibility

- **.NET Version**: Requires .NET 8.0+ for full features
- **Breaking Changes**: API evolved significantly from early versions
- **Preview Packages**: Most provider packages are in preview as of January 2026

## Official Resources

- [Microsoft.Extensions.AI Libraries](https://learn.microsoft.com/en-us/dotnet/ai/microsoft-extensions-ai)
- [Use the IChatClient interface](https://learn.microsoft.com/en-us/dotnet/ai/ichatclient)
- [Use the IEmbeddingGenerator interface](https://learn.microsoft.com/en-us/dotnet/ai/iembeddinggenerator)
- [.NET AI Apps for Developers](https://learn.microsoft.com/en-us/dotnet/ai/)
- [GitHub Repository](https://github.com/dotnet/extensions/tree/main/src/Libraries/Microsoft.Extensions.AI)

---

**Next Review**: April 2026 (check for stable releases of preview packages)
