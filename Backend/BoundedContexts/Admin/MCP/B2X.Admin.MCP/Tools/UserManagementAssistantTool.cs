using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// User Management Assistant Tool
/// </summary>
public class UserManagementAssistantTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<UserManagementAssistantTool> _logger;

    public UserManagementAssistantTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<UserManagementAssistantTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(UserManagementAssistantArgs args)
    {
        var prompt = await GetSystemPromptAsync("user_management");

        var userMessage = $@"
Analyze the following user management task and provide recommendations:

Task: {args.Task}
Context: {args.UserContext ?? "General user management"}

Please provide:
1. Task breakdown and steps
2. Security considerations
3. Best practices recommendations
4. Potential risks and mitigations
5. Implementation suggestions
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "user_management_assistant",
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

        return aiResponse.Response?.Content ?? "Failed to analyze user management task";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert in user management and identity administration. Provide secure, efficient solutions for user lifecycle management.

Guidelines:
- Prioritize security and compliance
- Follow principle of least privilege
- Ensure audit trails and accountability
- Consider user experience and ease of administration
- Recommend automation where appropriate
- Address GDPR and privacy requirements

Provide actionable recommendations with security considerations.
";
    }
}