using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Tenant Management Tool - Manages tenant lifecycle and resources
/// </summary>
public class TenantManagementTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<TenantManagementTool> _logger;

    public TenantManagementTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<TenantManagementTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(TenantManagementArgs args)
    {
        var prompt = await GetSystemPromptAsync("tenant_management");

        var userMessage = $@"
Analyze and provide recommendations for tenant management task:

Action: {args.Action}
Tenant ID: {args.TenantId}
Resource Type: {args.ResourceType ?? "General"}
Configuration: {args.Configuration ?? "Current configuration"}

Please provide:
1. Action assessment and feasibility analysis
2. Resource impact analysis
3. Security and compliance considerations
4. Implementation steps and timeline
5. Risk assessment and mitigation strategies
6. Monitoring and rollback procedures

Format as a comprehensive tenant management report.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "tenant_management",
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

        return aiResponse.Response?.Content ?? "Failed to analyze tenant management task";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert in multi-tenant SaaS architecture and tenant lifecycle management. Provide strategic guidance for tenant operations while ensuring system stability and security.

Guidelines:
- Prioritize system stability and performance
- Ensure proper resource isolation and limits
- Maintain security boundaries between tenants
- Consider scalability and cost implications
- Follow tenant management best practices
- Provide actionable, prioritized recommendations

Focus on operational excellence and risk mitigation in multi-tenant environments.
";
    }
}