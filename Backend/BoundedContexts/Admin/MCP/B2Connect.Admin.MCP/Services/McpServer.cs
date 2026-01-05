using ModelContextProtocol;
using ModelContextProtocol.Server;
using B2Connect.Admin.MCP.Services;
using B2Connect.Admin.MCP.Tools;
using B2Connect.Admin.MCP.Middleware;
using B2Connect.Admin.MCP.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// MCP Server implementation for tenant administration
/// </summary>
public class McpServer : IMcpServer
{
    private readonly ILogger<McpServer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public McpServer(
        ILogger<McpServer> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task<InitializeResult> InitializeAsync(InitializeRequest request)
    {
        var result = new InitializeResult
        {
            ProtocolVersion = "2024-11-05",
            ServerInfo = new ServerInfo
            {
                Name = "B2Connect Admin MCP Server",
                Version = "1.0.0"
            },
            Capabilities = new ServerCapabilities
            {
                Tools = new ToolsCapability
                {
                    ListChanged = true
                }
            }
        };

        // TenantContext is scoped - get it from service provider when needed
        using var scope = _serviceProvider.CreateScope();
        var tenantContext = scope.ServiceProvider.GetService<TenantContext>();
        _logger.LogInformation("MCP Server initialized for tenant {TenantId}", tenantContext?.TenantId ?? "unknown");
        return Task.FromResult(result);
    }

    public Task<ListToolsResult> ListToolsAsync()
    {
        var tools = new List<Tool>
        {
            CreateToolDefinition("intelligent_assistant", "AI-powered intelligent assistant that analyzes your request and routes to the most appropriate tool",
                typeof(IntelligentAssistantArgs)),
            CreateToolDefinition("cms_page_design", "Design and optimize CMS page layouts",
                typeof(CmsPageDesignArgs)),
            CreateToolDefinition("email_template_design", "Design professional email templates",
                typeof(EmailTemplateDesignArgs)),
            CreateToolDefinition("system_health_analysis", "Analyze system health and performance",
                typeof(SystemHealthAnalysisArgs)),
            CreateToolDefinition("user_management_assistant", "Assist with user management tasks",
                typeof(UserManagementAssistantArgs)),
            CreateToolDefinition("content_optimization", "Optimize content for SEO and engagement",
                typeof(ContentOptimizationArgs)),
            CreateToolDefinition("tenant_management", "Manage tenant lifecycle and resources",
                typeof(TenantManagementArgs)),
            CreateToolDefinition("store_operations", "Analyze and optimize store performance",
                typeof(StoreOperationsArgs)),
            CreateToolDefinition("security_compliance", "Ensure security standards and compliance",
                typeof(SecurityComplianceArgs)),
            CreateToolDefinition("performance_optimization", "Analyze and improve system performance",
                typeof(PerformanceOptimizationArgs)),
            CreateToolDefinition("integration_management", "Manage external integrations and APIs",
                typeof(IntegrationManagementArgs)),
            CreateToolDefinition("template_validation", "Validate CMS templates for syntax, security, and best practices (ADR-030)",
                typeof(TemplateValidationArgs))
        };

        return Task.FromResult(new ListToolsResult { Tools = tools });
    }

    public async Task<CallToolResult> CallToolAsync(CallToolRequest request)
    {
        try
        {
            // Check if this is an intelligent routing request
            if (request.Name == "intelligent_assistant" || string.IsNullOrEmpty(request.Name))
            {
                return await ExecuteIntelligentToolAsync(request);
            }

            var result = await ExecuteToolAsync(request.Name, request.Arguments);
            return new CallToolResult
            {
                Content = new List<ContentItem> { new TextContent { Text = result } }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {ToolName}", request.Name);
            return new CallToolResult
            {
                Content = new List<ContentItem> {
                    new TextContent { Text = $"Error executing tool: {ex.Message}" }
                },
                IsError = true
            };
        }
    }

    private async Task<CallToolResult> ExecuteIntelligentToolAsync(CallToolRequest request)
    {
        try
        {
            // Extract user message from arguments
            string userMessage = "";
            int? conversationId = null;

            if (request.Arguments.HasValue)
            {
                var args = JsonSerializer.Deserialize<IntelligentAssistantArgs>(
                    request.Arguments.Value.GetRawText());
                if (args != null)
                {
                    userMessage = args.Message ?? "";
                    conversationId = args.ConversationId;
                }
            }

            if (string.IsNullOrEmpty(userMessage))
            {
                return new CallToolResult
                {
                    Content = new List<ContentItem> {
                        new TextContent { Text = "Please provide a message to analyze." }
                    },
                    IsError = true
                };
            }

            // Create scope for scoped services
            using var scope = _serviceProvider.CreateScope();
            var nlpService = scope.ServiceProvider.GetRequiredService<AdvancedNlpService>();
            var conversationService = scope.ServiceProvider.GetRequiredService<ConversationService>();

            // Analyze intent using NLP service
            var analysis = await nlpService.AnalyzeIntentAsync(userMessage, conversationId);

            _logger.LogInformation("NLP Analysis - Tool: {Tool}, Confidence: {Confidence}",
                analysis.ToolName, analysis.Confidence);

            // Store the user message in conversation if conversationId provided
            if (conversationId.HasValue)
            {
                await conversationService.AddMessageAsync(
                    conversationId.Value,
                    "user",
                    userMessage,
                    analysis.ToolName);
            }

            // Execute the recommended tool
            var toolResult = await ExecuteToolAsync(analysis.ToolName, request.Arguments);

            // Store the assistant response
            if (conversationId.HasValue)
            {
                await conversationService.AddMessageAsync(
                    conversationId.Value,
                    "assistant",
                    toolResult,
                    analysis.ToolName);
            }

            // Return result with analysis metadata
            var content = new List<ContentItem>
            {
                new TextContent { Text = toolResult }
            };

            // Add analysis metadata if confidence is below threshold
            if (analysis.Confidence < 0.7)
            {
                var metadata = $"\\n\\n*Analysis: Used tool '{analysis.ToolName}' with {analysis.Confidence:P1} confidence*";
                if (analysis.AlternativeTools.Any())
                {
                    var alternatives = string.Join(", ",
                        analysis.AlternativeTools.Take(2).Select(a => $"{a.Key} ({a.Value:P1})"));
                    metadata += $"\\n*Alternatives considered: {alternatives}*";
                }
                content.Add(new TextContent { Text = metadata });
            }

            return new CallToolResult
            {
                Content = content
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in intelligent tool execution");
            return new CallToolResult
            {
                Content = new List<ContentItem> {
                    new TextContent { Text = $"Error in intelligent analysis: {ex.Message}" }
                },
                IsError = true
            };
        }
    }

    private Tool CreateToolDefinition(string name, string description, Type argsType)
    {
        var properties = new Dictionary<string, JsonSchema>();
        var required = new List<string>();

        foreach (var prop in argsType.GetProperties())
        {
            var propSchema = new JsonSchema { Type = GetJsonType(prop.PropertyType) };

            if (!string.IsNullOrEmpty(prop.Name))
            {
                propSchema.Description = prop.Name;
            }

            // Check if property is required (not nullable)
            if (prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) == null)
            {
                required.Add(prop.Name.ToLower());
            }
            else if (!prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) != null)
            {
                // Optional properties
            }
            else
            {
                required.Add(prop.Name.ToLower());
            }

            properties[prop.Name.ToLower()] = propSchema;
        }

        return new Tool
        {
            Name = name,
            Description = description,
            InputSchema = new JsonSchema
            {
                Type = "object",
                Properties = properties,
                Required = required.ToArray()
            }
        };
    }

    private string GetJsonType(Type type)
    {
        if (type == typeof(string))
            return "string";
        if (type == typeof(int) || type == typeof(long))
            return "integer";
        if (type == typeof(float) || type == typeof(double))
            return "number";
        if (type == typeof(bool))
            return "boolean";
        if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            return "array";
        return "object";
    }

    private async Task<string> ExecuteToolAsync(string toolName, JsonElement? arguments)
    {
        if (arguments == null)
            throw new ArgumentException("Arguments are required");

        return toolName switch
        {
            "intelligent_assistant" => await ExecuteIntelligentAssistantTool(arguments.Value),
            "cms_page_design" => await ExecuteCmsPageDesignTool(arguments.Value),
            "email_template_design" => await ExecuteEmailTemplateDesignTool(arguments.Value),
            "system_health_analysis" => await ExecuteSystemHealthAnalysisTool(arguments.Value),
            "user_management_assistant" => await ExecuteUserManagementAssistantTool(arguments.Value),
            "content_optimization" => await ExecuteContentOptimizationTool(arguments.Value),
            "tenant_management" => await ExecuteTenantManagementTool(arguments.Value),
            "store_operations" => await ExecuteStoreOperationsTool(arguments.Value),
            "security_compliance" => await ExecuteSecurityComplianceTool(arguments.Value),
            "performance_optimization" => await ExecutePerformanceOptimizationTool(arguments.Value),
            "integration_management" => await ExecuteIntegrationManagementTool(arguments.Value),
            "template_validation" => await ExecuteTemplateValidationTool(arguments.Value),
            _ => throw new ArgumentException($"Unknown tool: {toolName}")
        };
    }

    private async Task<string> ExecuteIntelligentAssistantTool(JsonElement args)
    {
        // This method is handled by ExecuteIntelligentToolAsync in CallToolAsync
        // This is just a fallback for direct calls
        var toolArgs = JsonSerializer.Deserialize<IntelligentAssistantArgs>(args.GetRawText());
        return $"Intelligent assistant received: {toolArgs?.Message ?? "no message"}";
    }

    private async Task<string> ExecuteCmsPageDesignTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<CmsPageDesignArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<CmsPageDesignTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteEmailTemplateDesignTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<EmailTemplateDesignArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<EmailTemplateDesignTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteSystemHealthAnalysisTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<SystemHealthAnalysisArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<SystemHealthAnalysisTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteUserManagementAssistantTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<UserManagementAssistantArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<UserManagementAssistantTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteContentOptimizationTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<ContentOptimizationArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<ContentOptimizationTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteTenantManagementTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<TenantManagementArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<TenantManagementTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteStoreOperationsTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<StoreOperationsArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<StoreOperationsTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteSecurityComplianceTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<SecurityComplianceArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<SecurityComplianceTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecutePerformanceOptimizationTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<PerformanceOptimizationArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<PerformanceOptimizationTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteIntegrationManagementTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<IntegrationManagementArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<IntegrationManagementTool>();
        return await tool.ExecuteAsync(toolArgs!);
    }

    private async Task<string> ExecuteTemplateValidationTool(JsonElement args)
    {
        var toolArgs = JsonSerializer.Deserialize<TemplateValidationArgs>(args.GetRawText());
        var tool = _serviceProvider.GetRequiredService<TemplateValidationTool>();
        var result = await tool.ExecuteAsync(toolArgs!);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }
}