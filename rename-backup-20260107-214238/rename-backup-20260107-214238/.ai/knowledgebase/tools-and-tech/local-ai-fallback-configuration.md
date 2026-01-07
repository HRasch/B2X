---
docid: KB-025
title: Global Local AI Mode Configuration
owner: GitHub Copilot
status: Active
---

# Global Local AI Mode Configuration

**Version**: 1.2  
**Last Updated**: 5. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Active

---

## Overview

The B2X MCP server supports **three AI modes**:

1. **Network Mode**: Uses network-hosted Ollama servers for all AI requests
2. **Local Fallback Mode**: Forces all agents to use local Ollama when external providers are rate limited
3. **Normal Mode**: Uses preferred providers with automatic fallback options

### Network AI Mode

When `AI:EnableNetworkMode` is enabled, **ALL agents use network-hosted Ollama servers exclusively**:

- **Network-first approach**: All AI requests go to configured network Ollama endpoints
- **No external API dependency**: Keeps processing within your network infrastructure
- **Centralized AI resources**: Leverage shared, powerful Ollama servers
- **Cost control**: Avoid cloud AI charges while maintaining performance

### Local Fallback Mode (Existing)

When `AI:EnableLocalFallback` is enabled, **ALL agents use local Ollama exclusively** during development or when external APIs are unavailable.

### Behavior When Enabled

#### Network Mode
- **ALL agents** use network-hosted Ollama regardless of their configured preferences
- **Network endpoints** are used based on tenant configuration
- **No fallback logic** - direct network Ollama usage from the start
- **Model mapping** is bypassed - agents can request any model name

#### Local Fallback Mode
- **ALL agents** use Ollama regardless of their configured preferences
- **No fallback logic** - direct Ollama usage from the start
- **Model mapping** is bypassed - agents can request any model name
- **Rate limiting protection** as secondary safeguard

### When to Enable

#### Network Mode
- **Enterprise environments** with centralized AI infrastructure
- **High-performance requirements** needing powerful shared GPU resources
- **Cost optimization** for large teams using shared network resources
- **Compliance requirements** keeping AI processing within corporate networks
- **Development teams** wanting consistent AI performance across all developers

#### Local Fallback Mode
- **Development environments** to avoid API costs
- **Offline development** when internet is unavailable
- **High-usage periods** when external APIs are rate limited
- **Cost-sensitive scenarios** where local processing is preferred

## Configuration

### Enable Network AI Mode

When `AI:EnableNetworkMode` is set to `true`, **ALL agents will use network-hosted Ollama servers exclusively**:

```json
{
  "AI": {
    "EnableNetworkMode": true,
    "Ollama": {
      "DefaultEndpoint": "http://ollama-server.company.com:11434",
      "Endpoints": {
        "tenant-1": "http://ollama-server-1.company.com:11434",
        "tenant-2": "http://ollama-server-2.company.com:11434"
      }
    }
  }
}
```

**Environment Variable:**
```bash
export AI__EnableNetworkMode=true
```

**Result:** Every AI request from any agent goes directly to the configured network Ollama endpoints.

### Enable Global Local Mode

When `AI:EnableLocalFallback` is set to `true`, **ALL agents will use Ollama exclusively**:

```json
{
  "AI": {
    "EnableLocalFallback": true,
    "Ollama": {
      "DefaultEndpoint": "http://localhost:11434"
    }
  }
}
```

**Environment Variable:**
```bash
export AI__EnableLocalFallback=true
```

**Result:** Every AI request from any agent goes directly to Ollama, bypassing all external providers.

### Ollama Configuration

Ensure Ollama is running with appropriate models:

```bash
# Pull recommended models
ollama pull deepseek-coder:33b
ollama pull qwen2.5-coder:32b
ollama pull qwen3:30b

# Start Ollama server
ollama serve
```

### Network Ollama Setup

For network-based Ollama instances, configure endpoints per tenant:

```json
{
  "AI": {
    "Ollama": {
      "DefaultEndpoint": "http://192.168.1.100:11434",
      "Endpoints": {
        "tenant-1": "http://ollama-server-1:11434",
        "tenant-2": "http://ollama-server-2:11434"
      }
    }
  }
}
```

## Runtime Mode Switching

**New Feature**: Switch between AI modes at runtime without restarting services using the MCP `ai_mode_switching` tool.

### Available Modes

1. **Normal**: Uses preferred AI providers with automatic fallback (default)
2. **Local**: Forces all agents to use local Ollama instance
3. **Network**: Forces all agents to use network-hosted Ollama servers

### Using the AI Mode Switching Tool

```json
{
  "method": "tools/call",
  "params": {
    "name": "ai_mode_switching",
    "arguments": {
      "targetMode": "Network",
      "reason": "Switching to network mode for better performance"
    }
  }
}
```

**Valid Target Modes**: `Normal`, `Local`, `Network` (case-insensitive)

### Response Format

**Success Response:**
```
✅ **AI Mode Switched Successfully**

**Previous Mode**: Normal
**New Mode**: Network
**Changed At**: 2026-01-05 14:30:22 UTC

🔄 **Normal Mode**: Uses preferred AI providers (OpenAI, Anthropic, etc.)
   - Cost: Variable (based on API usage)
   - Speed: Fast (cloud infrastructure)
   - Reliability: High (managed services)
   - Privacy: Data sent to external providers

**Next Steps:**
- Test AI functionality with a sample request
- Monitor performance and costs
- Switch back if needed using the same tool
```

**Error Response:**
```
❌ **Error**: Invalid mode 'invalid'. Valid modes are: Normal, Local, Network
```

### Thread Safety

The mode switching operation is **thread-safe** and handles concurrent requests properly. Multiple simultaneous mode switch requests are serialized to prevent race conditions.

### Use Cases

- **Development**: Switch to Local mode for cost-free development
- **Production**: Switch to Network mode for enterprise-grade performance
- **Testing**: Switch between modes to test different configurations
- **Cost Control**: Switch to Local/Network modes during high-usage periods
- **Compliance**: Switch to Network mode to keep data within corporate boundaries

## How It Works

### Network AI Mode

When `AI:EnableNetworkMode = true`:

1. **Provider Selection**: `GetProviderForTenantAsync()` always returns `OllamaProvider` regardless of requested provider
2. **Endpoint Resolution**: Uses network endpoints configured in `AI:Ollama:Endpoints:{tenantId}` or `AI:Ollama:DefaultEndpoint`
3. **Direct Usage**: Agents use network Ollama immediately without external API calls
4. **Tenant Isolation**: Each tenant can have its own network Ollama endpoint

### Global Local Mode

When `AI:EnableLocalFallback = true`:

1. **Provider Selection**: `GetProviderForTenantAsync()` always returns `OllamaProvider` regardless of requested provider
2. **Direct Usage**: Agents use Ollama immediately without external API calls
3. **No Model Mapping**: Original model names are passed through (Ollama handles compatibility)
4. **Tenant Isolation**: Each tenant can have its own Ollama endpoint

### Flow Diagram

```
Agent Request → MCP Server → AiProviderSelector
    ↓
Network Mode Enabled?
    ├── YES → Return OllamaProvider (network endpoints)
    └── NO → Local Fallback Enabled?
        ├── YES → Return OllamaProvider (local)
        └── NO → Return requested provider (normal operation)
```

### Rate Limiting Detection (Secondary)

As additional protection, the system still detects rate limiting:

- **429 Status Codes** (Too Many Requests)
- **Rate limit messages** in error responses
- **Quota exceeded** errors
- **HTTP timeout exceptions** from rate limiting

When detected, switches to Ollama even if global fallback is disabled.

1. **HTTP Status Codes**: 429 (Too Many Requests)
2. **Error Messages**: Containing "rate limit", "too many requests", "quota exceeded"
3. **Exception Types**: `HttpRequestException` with status 429

### Fallback Logic

```
User Request → Primary Provider (GitHub/OpenAI/etc.)
    ↓ (Rate Limited?)
    ├── No → Return Response
    └── Yes → Network Mode Enabled?
        ├── Yes → Network Ollama Fallback
        └── No → Local Mode Enabled?
            ├── No → Throw Exception
            └── Yes → Local Ollama Fallback
                ├── Map Model → Equivalent Ollama Model
                ├── Execute with Ollama
                └── Return Response
```

### Model Mapping

| Original Provider/Model | Ollama Equivalent | Reasoning |
|------------------------|-------------------|-----------|
| GitHub Models GPT-4 | `deepseek-coder:33b` | Best code generation |
| OpenAI GPT-4 | `deepseek-coder:33b` | Equivalent capabilities |
| OpenAI GPT-3.5 | `qwen2.5-coder:14b` | Good balance |
| Anthropic Claude-3.5 | `qwen3:30b` | Advanced reasoning |
| Anthropic Claude-3 | `qwen2.5:14b` | Solid performance |
| Azure OpenAI | `deepseek-coder:33b` | Default mapping |

## Usage Examples

### Development Workflow

```csharp
// In your MCP-enabled application
var selector = new AiProviderSelector(/* injected dependencies */);

// This will automatically fallback if rate limited
var response = await selector.ExecuteChatCompletionAsync(
    tenantId: "dev-tenant",
    model: "gpt-4",
    prompt: "You are a C# expert",
    userMessage: "Help me implement dependency injection",
    preferredProvider: "github-models" // Will fallback to Ollama if rate limited
);
```

### Testing Rate Limiting

For testing the fallback behavior:

```csharp
// Force rate limit simulation
_configuration["AI:EnableLocalFallback"] = "true";

// The selector will detect rate limiting and switch to Ollama
```

## Monitoring & Logging

### Log Messages

- **Rate Limit Detection**: `Rate limit detected for provider {Provider} and local fallback enabled. Switching to Ollama for tenant {TenantId}`
- **Fallback Success**: `Successfully fell back to Ollama for tenant {TenantId}`
- **Fallback Failure**: `Both primary provider {Provider} and Ollama fallback failed`

### Metrics

Track fallback usage via application metrics:

- `ai_fallback_total`: Total fallback operations
- `ai_fallback_duration`: Time spent in fallback execution
- `ai_fallback_success_rate`: Percentage of successful fallbacks

## Security Considerations

### Data Sanitization

All requests going to Ollama are subject to the same sanitization rules as external providers:

- **Sensitive Data Detection**: Blocks requests with PII, credentials, etc.
- **Content Filtering**: Prevents harmful prompts
- **Tenant Isolation**: Maintains per-tenant data separation

### Network Security

- **Endpoint Validation**: Only configured Ollama endpoints are allowed
- **TLS**: Use HTTPS for network Ollama instances
- **Authentication**: Configure API keys for cloud Ollama services

## Troubleshooting

### Common Issues

#### Ollama Not Available
```
Error: Both primary provider github-models and Ollama fallback failed
```
**Solution**: Ensure Ollama is running and accessible:
```bash
curl http://localhost:11434/api/tags
```

#### Model Not Found
```
Error: model 'deepseek-coder:33b' not found
```
**Solution**: Pull the required model:
```bash
ollama pull deepseek-coder:33b
```

#### Configuration Issues
```
Error: API key not found for tenant
```
**Solution**: Configure Ollama endpoint properly in `appsettings.json`

### Performance Considerations

- **Model Size**: Larger models (33B) provide better quality but slower response
- **Network Latency**: Network Ollama adds latency vs local instances
- **Resource Usage**: Monitor CPU/GPU usage during fallback operations

## Best Practices

### Network Mode
1. **Network Infrastructure**: Ensure reliable, high-bandwidth network connections to Ollama servers
2. **Load Balancing**: Distribute requests across multiple Ollama servers for high availability
3. **Security**: Use HTTPS and authentication for network Ollama endpoints
4. **Monitoring**: Track network latency and server performance
5. **Backup Planning**: Have local Ollama instances as backup for network failures

### Local Mode
1. **Pre-pull Models**: Ensure required Ollama models are available
2. **Resource Monitoring**: Track CPU/GPU usage during AI operations
3. **Model Updates**: Keep Ollama models updated for best performance
4. **Disk Space**: Ensure adequate storage for multiple models

### General
1. **Monitor Usage**: Track AI mode usage patterns and performance
2. **Test Regularly**: Validate both network and local modes work correctly
3. **Cost Analysis**: Compare network vs local vs cloud AI costs
4. **Compliance**: Ensure chosen mode meets data residency requirements

## Integration with VS Code

When using this with GitHub Copilot in VS Code:

### Network Mode Setup
1. Configure network Ollama endpoints in your application settings
2. Enable network mode: `AI:EnableNetworkMode=true`
3. Ensure network connectivity to Ollama servers
4. The MCP server will route all AI requests to network Ollama

### Local Mode Setup
1. Enable local fallback in your application configuration
2. Ensure Ollama is running locally with appropriate models
3. The MCP server will automatically handle fallbacks during rate limiting

### General
1. Monitor application logs for AI mode activity
2. Test both network and local modes during development
3. Configure appropriate timeouts for network requests

This provides flexible AI assistance with options for network infrastructure, local processing, or cloud services.

---

**Related Documentation**:
- [KB-018: Local LLM Models 2025](../tools-and-tech/local-llm-models-2025.md)
- [KB-024: Microsoft.Extensions.AI](../dependency-updates/Microsoft.Extensions.AI.md)
- [ADR-029: Microsoft.Extensions.AI Adoption](../decisions/ADR-029-microsoft-extensions-ai-adoption.md)</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/knowledgebase/tools-and-tech/local-ai-fallback-configuration.md