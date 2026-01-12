using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using B2X.Admin.MCP.Services;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Store Operations Tool - Analyzes and optimizes store performance
/// </summary>
public class StoreOperationsTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<StoreOperationsTool> _logger;

    public StoreOperationsTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<StoreOperationsTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(StoreOperationsArgs args)
    {
        var prompt = await GetSystemPromptAsync("store_operations");

        var userMessage = $@"
Analyze store operations and provide optimization recommendations:

Operation: {args.Operation}
Store ID: {args.StoreId}
Time Range: {args.TimeRange ?? "Last 30 days"}
Analysis Type: {args.AnalysisType ?? "Performance"}

Please provide:
1. Current performance metrics analysis
2. Operational efficiency assessment
3. Inventory and sales optimization recommendations
4. Customer experience insights
5. Competitive positioning analysis
6. Actionable improvement roadmap

Format as a comprehensive store operations report with prioritized recommendations.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "store_operations",
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

        return aiResponse.Response?.Content ?? "Failed to analyze store operations";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert retail operations consultant specializing in e-commerce and B2B marketplace optimization. Provide data-driven insights and strategic recommendations for store performance improvement.

Guidelines:
- Focus on revenue growth and operational efficiency
- Analyze customer behavior and market trends
- Optimize inventory management and supply chain
- Enhance user experience and conversion rates
- Provide measurable KPIs and success metrics
- Consider seasonal and market dynamics
- Balance short-term gains with long-term strategy

Deliver actionable insights that drive tangible business results.
";
    }
}
