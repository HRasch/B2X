using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Email Template Design Tool
/// </summary>
public class EmailTemplateDesignTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<EmailTemplateDesignTool> _logger;

    public EmailTemplateDesignTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<EmailTemplateDesignTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(EmailTemplateDesignArgs args)
    {
        var prompt = await GetSystemPromptAsync("email_template_design");

        var userMessage = $@"
Design an email template for the following requirements:

Email Type: {args.EmailType}
Content Purpose: {args.ContentPurpose}
Brand Guidelines: {args.BrandGuidelines ?? "Standard professional guidelines"}

Please provide:
1. Email structure and layout
2. Subject line suggestions
3. Content sections and hierarchy
4. Call-to-action placement
5. Mobile optimization
6. Deliverability considerations
7. HTML/CSS implementation notes

Format as a complete email template specification.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "email_template_design",
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

        return aiResponse.Response?.Content ?? "Failed to generate email template";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert email marketing designer. Create professional, effective email templates that drive engagement and conversions.

Guidelines:
- Optimize for mobile devices first
- Ensure high deliverability and inbox placement
- Use clear, compelling calls-to-action
- Maintain brand consistency
- Follow email marketing best practices
- Consider accessibility and readability
- Include unsubscribe and contact information

Provide complete template specifications with HTML structure recommendations.
";
    }
}