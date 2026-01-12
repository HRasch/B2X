using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using B2X.Admin.MCP.Services;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// Security Compliance Tool - Conducts security and compliance assessments
/// </summary>
public class SecurityComplianceTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<SecurityComplianceTool> _logger;

    public SecurityComplianceTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<SecurityComplianceTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(SecurityComplianceArgs args)
    {
        var prompt = await GetSystemPromptAsync("security_compliance");

        var userMessage = $@"
Conduct security and compliance assessment:

Assessment Type: {args.AssessmentType}
Scope: {args.Scope}
Specific Component: {args.SpecificComponent ?? "All components"}
Include Recommendations: {args.IncludeRecommendations}

Please provide:
1. Current security posture analysis
2. Compliance gap identification
3. Risk assessment and prioritization
4. Remediation recommendations with timelines
5. Monitoring and auditing procedures
6. Regulatory compliance verification

Format as a comprehensive security and compliance report with actionable remediation steps.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "security_compliance",
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

        return aiResponse.Response?.Content ?? "Failed to conduct security assessment";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are a cybersecurity and compliance expert specializing in SaaS platforms and GDPR/NIS2 regulations. Provide comprehensive security assessments and compliance guidance for multi-tenant systems.

Guidelines:
- Follow OWASP Top 10 and industry security standards
- Ensure GDPR, NIS2, and BITV 2.0 compliance
- Prioritize critical vulnerabilities and data protection
- Provide practical, implementable security measures
- Consider both technical and organizational controls
- Balance security with usability and performance
- Include monitoring and incident response procedures

Deliver security recommendations that protect both the platform and tenant data while maintaining operational efficiency.
";
    }
}
