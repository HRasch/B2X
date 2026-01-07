# Microsoft.Extensions.AI - Unified AI Abstractions

**DocID**: `KB-SW-MSAI`  
**Last Updated**: 4. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Package Versions (January 2026)

| Package | Latest Stable | Preview | Notes |
|---------|--------------|---------|-------|
| `Microsoft.Extensions.AI` | **10.1.1** | - | Core abstractions (IChatClient, ChatMessage, ChatRole) |
| `Microsoft.Extensions.AI.Abstractions` | **10.1.1** | - | Interfaces only |
| `Microsoft.Extensions.AI.OpenAI` | - | **10.1.1-preview.1.25612.2** | Requires OpenAI >= 2.8.0 |
| `Microsoft.Extensions.AI.AzureAIInference` | - | **10.0.0-preview.1.25559.3** | Azure AI Inference integration |
| `Microsoft.Extensions.AI.Ollama` | - | **10.1.1-preview.1.25612.2** | Ollama integration |

## Related SDK Versions

| Package | Version | Notes |
|---------|---------|-------|
| `OpenAI` | **2.8.0** | Required by Microsoft.Extensions.AI.OpenAI |
| `Azure.AI.OpenAI` | **2.1.0** | Latest stable (no 2.2.0 exists!) |
| `Azure.AI.Inference` | **1.0.0-beta.3** | For GitHub Models / Azure AI |
| `Anthropic.SDK` | **5.8.0** | Native SDK, no M.E.AI adapter |
| `OllamaSharp` | **5.4.12** | Implements IChatClient directly |

---

## API Changes from Earlier Versions

### ❌ OLD API (Pre-10.x)
```csharp
// OLD - No longer works
using OpenAI.Chat;
var client = new ChatClient(model, apiKey).AsChatClient();
var response = await client.CompleteAsync(messages, options, ct);
var text = response.Message.Text;
```

### ✅ NEW API (10.1.x)
```csharp
// NEW - Current pattern
using Microsoft.Extensions.AI;
IChatClient client = new OpenAI.Chat.ChatClient(model, apiKey).AsIChatClient();
var response = await client.GetResponseAsync(messages, options, ct);
var text = response.Text;
```

---

## Provider Implementation Patterns

### OpenAI
```csharp
using Microsoft.Extensions.AI;

IChatClient client = new OpenAI.Chat.ChatClient(model, apiKey).AsIChatClient();

var messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, systemPrompt),
    new ChatMessage(ChatRole.User, userMessage)
};

var response = await client.GetResponseAsync(messages, null, cancellationToken);
var content = response.Text ?? string.Empty;
var tokens = response.Usage?.TotalTokenCount ?? 0;
```

### Azure OpenAI
```csharp
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using System.ClientModel;

IChatClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
    .GetChatClient(deploymentName)
    .AsIChatClient();

var response = await client.GetResponseAsync(messages, null, cancellationToken);
```

### Anthropic (Native SDK - No M.E.AI Adapter)
```csharp
using Anthropic.SDK;
using Anthropic.SDK.Messaging;

var client = new AnthropicClient(apiKey);
var request = new MessageParameters
{
    Model = model,
    MaxTokens = 4096,
    System = [new SystemMessage(systemPrompt)],
    Messages = [new Message { Role = RoleType.User, Content = userMessage }]
};

var response = await client.Messages.GetClaudeMessageAsync(request, cancellationToken);
var content = response.Content?.FirstOrDefault()?.ToString() ?? string.Empty;
var tokens = response.Usage.InputTokens + response.Usage.OutputTokens;
```

### Ollama (Implements IChatClient Directly)
```csharp
using Microsoft.Extensions.AI;
using OllamaSharp;

var ollamaClient = new OllamaApiClient(new Uri(endpoint), model);
IChatClient chatClient = ollamaClient; // Direct cast - implements IChatClient

var response = await chatClient.GetResponseAsync(messages, cancellationToken);
```

### GitHub Models (Azure AI Inference)
```csharp
using Microsoft.Extensions.AI;
using Azure.AI.Inference;

IChatClient client = new ChatCompletionsClient(
    new Uri("https://models.inference.ai.azure.com"),
    new AzureKeyCredential(apiKey)
).AsIChatClient(model);

var response = await client.GetResponseAsync(messages, cancellationToken);
```

---

## Type Aliasing (Avoid Ambiguity)

When using Microsoft.Extensions.AI with provider SDKs that have their own `ChatMessage`/`ChatRole` types:

```csharp
// Add at top of file to resolve ambiguity
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;
using AiChatRole = Microsoft.Extensions.AI.ChatRole;

// Then use aliases in code
var messages = new List<AiChatMessage>
{
    new AiChatMessage(AiChatRole.System, prompt),
    new AiChatMessage(AiChatRole.User, message)
};
```

**Conflicting namespaces to avoid importing together:**
- `OpenAI.Chat` (has ChatMessage, ChatRole)
- `Azure.AI.OpenAI` (re-exports OpenAI types)
- `Azure.AI.Inference` (has ChatRequestMessage)

---

## Token Usage

```csharp
// New API returns long, not int
var response = await client.GetResponseAsync(messages, null, ct);

int tokensUsed = 0;
if (response.Usage?.TotalTokenCount is not null)
{
    tokensUsed = (int)response.Usage.TotalTokenCount.Value; // Cast long to int
}
```

---

## Directory.Packages.props Configuration

```xml
<!-- Microsoft.Extensions.AI ecosystem -->
<PackageVersion Include="Microsoft.Extensions.AI" Version="10.1.1" />
<PackageVersion Include="Microsoft.Extensions.AI.Abstractions" Version="10.1.1" />
<PackageVersion Include="Microsoft.Extensions.AI.OpenAI" Version="10.1.1-preview.1.25612.2" />
<PackageVersion Include="Microsoft.Extensions.AI.AzureAIInference" Version="10.0.0-preview.1.25559.3" />

<!-- Provider SDKs -->
<PackageVersion Include="OpenAI" Version="2.8.0" />
<PackageVersion Include="Azure.AI.OpenAI" Version="2.1.0" />
<PackageVersion Include="Azure.AI.Inference" Version="1.0.0-beta.3" />
<PackageVersion Include="Anthropic.SDK" Version="5.8.0" />
<PackageVersion Include="OllamaSharp" Version="5.4.12" />
```

---

## Breaking Changes Summary

| Change | Old | New |
|--------|-----|-----|
| Extension method | `AsChatClient()` | `AsIChatClient()` |
| Completion method | `CompleteAsync()` | `GetResponseAsync()` |
| Response text | `response.Message.Text` | `response.Text` |
| Token count type | `int` | `long` (requires cast) |
| Interface name | `IChatClient` | `IChatClient` (unchanged) |

---

## Official Resources

- [Microsoft.Extensions.AI NuGet](https://www.nuget.org/packages/Microsoft.Extensions.AI)
- [Microsoft.Extensions.AI.OpenAI NuGet](https://www.nuget.org/packages/Microsoft.Extensions.AI.OpenAI)
- [OpenAI .NET SDK](https://www.nuget.org/packages/OpenAI)
- [Azure.AI.OpenAI NuGet](https://www.nuget.org/packages/Azure.AI.OpenAI)
- [Anthropic.SDK NuGet](https://www.nuget.org/packages/Anthropic.SDK)
- [OllamaSharp NuGet](https://www.nuget.org/packages/OllamaSharp)

---

## B2Connect Implementation

See [AiProviders.cs](../../../backend/BoundedContexts/Admin/MCP/B2Connect.Admin.MCP/Services/AiProviders.cs) for our multi-provider implementation using Microsoft.Extensions.AI patterns.

**Supported Providers:**
- OpenAI (GPT-4, GPT-4o, etc.)
- Azure OpenAI (tenant-specific deployments)
- Anthropic (Claude models - native SDK)
- Ollama (local/network LLM instances)
- GitHub Models (Azure AI Inference)

---

**Next Review**: April 2026 (check for stable releases of preview packages)
