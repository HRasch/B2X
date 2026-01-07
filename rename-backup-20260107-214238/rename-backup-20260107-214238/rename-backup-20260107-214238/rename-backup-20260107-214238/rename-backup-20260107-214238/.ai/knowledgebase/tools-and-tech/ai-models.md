# AI Models and Providers Documentation

**Last Updated**: 2026-01-03
**Maintained By**: GitHub Copilot
**Status**: ✅ Current

---

## Microsoft.Extensions.AI Support Matrix

### Official Microsoft Packages (v10.1.1)

| Provider | Package | IChatClient | IEmbeddingGenerator | Status |
|----------|---------|-------------|-------------------|---------|
| OpenAI | `Microsoft.Extensions.AI.OpenAI` | ✅ | ✅ | Production |
| Anthropic | `Microsoft.Extensions.AI.Anthropic` | ✅ | ❌ | Production |
| Azure AI Inference | `Microsoft.Extensions.AI.AzureAIInference` | ✅ | ✅ | Production |
| Ollama | `Microsoft.Extensions.AI.Ollama` | ❌ Deprecated | ❌ | Use OllamaSharp |

### Community Packages

| Provider | Package | IChatClient | IEmbeddingGenerator | Status |
|----------|---------|-------------|-------------------|---------|
| Ollama | `OllamaSharp` v5.4.12 | ✅ | ✅ | Production |
| GitHub Models | `AI.Bridge` v0.1.0 | ✅ | ✅ | Beta |
| Multiple | `EasyAppDev.Blazor.AutoComplete.AI` | ✅ | ❌ | Beta |

---

## Ollama Support

### Official Recommendation: OllamaSharp

**Package**: `OllamaSharp` v5.4.12
**NuGet**: https://www.nuget.org/packages/OllamaSharp
**GitHub**: https://github.com/awaescher/OllamaSharp

#### Features
- ✅ Full Microsoft.Extensions.AI support (IChatClient, IEmbeddingGenerator)
- ✅ Local and network Ollama instances
- ✅ Streaming responses
- ✅ Model management (pull, list, delete)
- ✅ Vision model support
- ✅ Native AOT support
- ✅ Tool/function calling

#### Usage with Microsoft.Extensions.AI

```csharp
// For IChatClient
var ollama = new OllamaApiClient(uri, modelName);
IChatClient chatClient = ollama;

// For IEmbeddingGenerator
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = ollama;
```

#### Network Support
```csharp
// Local Ollama
var uri = new Uri("http://localhost:11434");

// Network Ollama
var uri = new Uri("http://192.168.1.100:11434");

// Cloud Ollama (with API key)
var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", "Bearer your-api-key");
var ollama = new OllamaApiClient(client);
```

#### Popular Models
- `llama3.2:3b` - Fast, lightweight
- `qwen2.5:7b` - Good balance
- `mistral:7b` - Popular open model
- `codellama:13b` - Code specialized
- `llava:7b` - Vision model

---

## GitHub Copilot Support

### No Public API Available

**Status**: ❌ Not supported
**Reason**: GitHub Copilot has no public API for external applications

#### Available Options
1. **GitHub Models** via OpenAI-compatible API
2. **Community Packages** like `AI.Bridge` that support GitHub Models
3. **Direct IDE Integration** (not for server applications)

#### GitHub Models (via OpenAI SDK)
```csharp
// GitHub Models are OpenAI-compatible
var client = new OpenAIClient(
    new ApiKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN")),
    new OpenAIClientOptions { Endpoint = new Uri("https://models.inference.ai.azure.com") }
);
```

---

## Docker Model Support

### Containerized AI Models

**Status**: ✅ Supported via Ollama containers

#### Ollama Docker Setup
```bash
# Run Ollama in Docker
docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama

# Pull a model
docker exec ollama ollama pull llama3.2:3b

# Use with OllamaSharp
var uri = new Uri("http://localhost:11434");
```

#### Other Container Options
- **vLLM**: High-performance inference server
- **Text Generation WebUI**: Gradio-based interface
- **LocalAI**: Drop-in OpenAI API replacement
- **Ollama**: Recommended for simplicity

---

## Implementation in B2X

### Current Providers (Microsoft.Extensions.AI)

```csharp
// OpenAI
var openaiClient = new OpenAIClient(apiKey).AsChatClient(model);

// Anthropic
var anthropicClient = new AnthropicClient(apiKey).AsChatClient(model);

// Azure OpenAI
var azureClient = new AzureOpenAIClient(endpoint, credential).AsChatClient(model);
```

### Adding Ollama Support

```csharp
// Add to Directory.Packages.props
<PackageReference Include="OllamaSharp" Version="5.4.12" />

// Implementation
public class OllamaProvider : IAIProvider
{
    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId, string model, string prompt, string userMessage, CancellationToken cancellationToken)
    {
        var uri = new Uri(_configuration["Ollama:Endpoint"] ?? "http://localhost:11434");
        var ollama = new OllamaApiClient(uri, model);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, prompt),
            new ChatMessage(ChatRole.User, userMessage)
        };

        var response = await ollama.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = response.Usage?.TotalTokenCount ?? 0,
            Model = model
        };
    }
}
```

### Adding GitHub Models Support

```csharp
// Via OpenAI SDK (GitHub Models are OpenAI-compatible)
public class GitHubModelsProvider : IAIProvider
{
    public async Task<AiResponse> ExecuteChatCompletionAsync(
        string tenantId, string model, string prompt, string userMessage, CancellationToken cancellationToken)
    {
        var token = await GetGitHubTokenAsync(tenantId);
        var client = new OpenAIClient(
            new ApiKeyCredential(token),
            new OpenAIClientOptions { Endpoint = new Uri("https://models.inference.ai.azure.com") }
        ).AsChatClient(model);

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, prompt),
            new ChatMessage(ChatRole.User, userMessage)
        };

        var response = await client.CompleteAsync(messages, new ChatOptions(), cancellationToken);

        return new AiResponse
        {
            Content = response.Message.Text ?? string.Empty,
            TokensUsed = response.Usage?.TotalTokenCount ?? 0,
            Model = model
        };
    }
}
```

---

## Security Considerations

### Ollama
- ✅ Local network isolation
- ✅ No external API keys required
- ✅ Models run locally/on-prem
- ⚠️ Network security for remote instances

### GitHub Models
- ✅ Uses GitHub tokens (scoped access)
- ✅ No separate API keys needed
- ✅ Integrated billing with GitHub
- ⚠️ Token management required

### Docker Models
- ✅ Container isolation
- ✅ Network policies applicable
- ✅ Resource limits enforceable
- ⚠️ Container security best practices required

---

## Performance Comparison

| Provider | Latency | Cost | Privacy | Setup Complexity |
|----------|---------|------|---------|------------------|
| OpenAI | Lowest | High | Low | Low |
| Anthropic | Low | High | Low | Low |
| Azure OpenAI | Low | Medium | Medium | Medium |
| Ollama Local | Medium | Free | High | Medium |
| Ollama Network | High | Free | Medium | Medium |
| GitHub Models | Low | Low | Medium | Low |

---

## Recommended Configurations

### Development
- **Primary**: Ollama (local, cost-free)
- **Fallback**: GitHub Models (easy setup)

### Production
- **Primary**: Azure OpenAI (managed, scalable)
- **Secondary**: Ollama (network, cost optimization)
- **Tertiary**: OpenAI/Anthropic (premium features)

---

## Migration Path

1. **Phase 1**: Add Ollama support (local/network)
2. **Phase 2**: Add GitHub Models support
3. **Phase 3**: Add Docker model orchestration
4. **Phase 4**: Implement provider failover logic

---

**Next Review**: 2026-04-03
**Update Trigger**: New Microsoft.Extensions.AI releases, OllamaSharp updates