using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Performance Optimization Tool - Analyzes and improves system performance
/// </summary>
public class PerformanceOptimizationTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<PerformanceOptimizationTool> _logger;

    public PerformanceOptimizationTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<PerformanceOptimizationTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(PerformanceOptimizationArgs args)
    {
        var prompt = await GetSystemPromptAsync("performance_optimization");

        var userMessage = $@"
Analyze system performance and provide optimization recommendations:

Component: {args.Component}
Metric Type: {args.MetricType}
Time Range: {args.TimeRange ?? "Last 7 days"}
Include Historical Data: {args.IncludeHistoricalData}

Please provide:
1. Current performance baseline analysis
2. Bottleneck identification and root cause analysis
3. Optimization recommendations with expected impact
4. Implementation priority and effort estimation
5. Monitoring and alerting recommendations
6. Scalability and capacity planning insights

Format as a comprehensive performance optimization report with quantified improvement projections.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "performance_optimization",
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

        return aiResponse.Response?.Content ?? "Failed to analyze performance";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are a performance engineering expert specializing in distributed systems and cloud-native applications. Provide data-driven performance analysis and optimization strategies for high-throughput SaaS platforms.

Guidelines:
- Focus on measurable performance improvements
- Consider scalability, reliability, and cost efficiency
- Analyze bottlenecks across the entire stack
- Provide evidence-based recommendations
- Include monitoring and observability requirements
- Balance optimization with maintainability
- Consider both immediate fixes and architectural improvements

Deliver performance insights that drive tangible improvements in speed, reliability, and user experience.
";
    }
}