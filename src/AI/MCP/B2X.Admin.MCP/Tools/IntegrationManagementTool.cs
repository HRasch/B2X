using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using B2X.Admin.MCP.Services;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Integration Management Tool - Manages external integrations and APIs
/// </summary>
public class IntegrationManagementTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<IntegrationManagementTool> _logger;

    public IntegrationManagementTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<IntegrationManagementTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(IntegrationManagementArgs args)
    {
        var prompt = await GetSystemPromptAsync("integration_management");

        var userMessage = $@"
Analyze and optimize external integration:

Integration Type: {args.IntegrationType}
Action: {args.Action}
Endpoint: {args.Endpoint ?? "N/A"}
Configuration: {args.Configuration ?? "Default"}

Please provide:
1. Integration health assessment
2. Performance and reliability analysis
3. Security and data protection evaluation
4. Optimization recommendations
5. Monitoring and alerting setup
6. Troubleshooting procedures and best practices

Format as a comprehensive integration management report with implementation guidance.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "integration_management",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to analyze integration";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an integration architecture expert specializing in API design, microservices communication, and enterprise system integration. Provide strategic guidance for building robust, scalable integration solutions.

Guidelines:
- Ensure reliable, secure, and performant integrations
- Follow API design best practices and standards
- Implement proper error handling and resilience patterns
- Consider monitoring, logging, and observability
- Balance integration complexity with business value
- Provide migration and modernization strategies
- Include testing and validation procedures

Deliver integration solutions that are maintainable, scalable, and business-critical reliable.
";
    }
}
