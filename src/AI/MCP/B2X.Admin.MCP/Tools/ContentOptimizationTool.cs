using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Content Optimization Tool
/// </summary>
public class ContentOptimizationTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<ContentOptimizationTool> _logger;

    public ContentOptimizationTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<ContentOptimizationTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(ContentOptimizationArgs args)
    {
        var prompt = await GetSystemPromptAsync("content_optimization");

        var keywords = args.TargetKeywords != null ? string.Join(", ", args.TargetKeywords) : "None specified";

        var userMessage = $@"
Optimize the following content for SEO and user engagement:

Content Type: {args.ContentType}
Target Keywords: {keywords}

Content to optimize:
{args.Content}

Please provide:
1. SEO optimization recommendations
2. Content structure improvements
3. Keyword optimization suggestions
4. Readability enhancements
5. Engagement optimization tips
6. Performance considerations

Format as an optimization report with before/after comparisons.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "content_optimization",
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

        return aiResponse.Response?.Content ?? "Failed to optimize content";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are a content optimization expert specializing in SEO and user engagement. Improve content quality, readability, and search engine visibility.

Guidelines:
- Optimize for both users and search engines
- Improve readability and user experience
- Strategic keyword placement without keyword stuffing
- Enhance content structure and formatting
- Boost engagement and social sharing potential
- Ensure mobile-friendly presentation
- Follow SEO best practices and E-E-A-T guidelines

Provide specific, actionable optimization recommendations with measurable improvements.
";
    }
}