gousing Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.MCP.Services;
using B2Connect.Admin.MCP.Middleware;
using System.Text.Json;

namespace B2Connect.Admin.MCP;

/// <summary>
/// Extension methods for mapping MCP endpoints
/// </summary>
public static class McpEndpointExtensions
{
    public static WebApplication MapMcpEndpoints(this WebApplication app)
    {
        // MCP Protocol endpoints - stubbed for now
        app.MapPost("/mcp/initialize", async (
            [FromBody] InitializeRequest request,
            IMcpServer mcpServer) =>
        {
            var result = await mcpServer.InitializeAsync(request);
            return Results.Ok(result);
        });

        app.MapPost("/mcp/tools/list", async (
            [FromBody] ListToolsRequest request,
            IMcpServer mcpServer) =>
        {
            var result = await mcpServer.ListToolsAsync();
            return Results.Ok(result);
        });

        app.MapPost("/mcp/tools/call", async (
            [FromBody] CallToolRequest request,
            IMcpServer mcpServer) =>
        {
            var result = await mcpServer.CallToolAsync(request);
            return Results.Ok(result);
        });

        // AI Consumption monitoring endpoints
        app.MapGet("/api/admin/ai-consumption/metrics", async (
            AiConsumptionGateway aiGateway,
            TenantContext tenantContext) =>
        {
            var metrics = await aiGateway.GetConsumptionMetricsAsync(tenantContext.TenantId);
            return Results.Ok(metrics);
        }).RequireAuthorization("TenantAdmin");

        // System prompt management endpoints
        app.MapGet("/api/admin/prompts", async (TenantContext tenantContext) =>
        {
            // TODO: Implement prompt retrieval from database
            return Results.Ok(new { prompts = new[] { "placeholder" } });
        }).RequireAuthorization("TenantAdmin");

        app.MapPost("/api/admin/prompts", async (
            [FromBody] PromptUpdateRequest request,
            TenantContext tenantContext) =>
        {
            // TODO: Implement prompt update with validation
            return Results.Created($"/api/admin/prompts/{request.Key}", new { status = "created" });
        }).RequireAuthorization("TenantAdmin");

        app.MapPut("/api/admin/prompts/{key}", async (
            string key,
            [FromBody] PromptUpdateRequest request,
            TenantContext tenantContext) =>
        {
            // TODO: Implement prompt update with versioning
            return Results.Ok(new { status = "updated" });
        }).RequireAuthorization("TenantAdmin");

        return app;
    }
}

// Request/Response models - simplified stubs
public class InitializeRequest
{
    public string ProtocolVersion { get; set; } = string.Empty;
    public ClientInfo? ClientInfo { get; set; }
}

public class ClientInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public class InitializeResult
{
    public string ProtocolVersion { get; set; } = string.Empty;
    public ServerInfo ServerInfo { get; set; } = new();
    public ServerCapabilities Capabilities { get; set; } = new();
}

public class ServerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public class ServerCapabilities
{
    public ToolsCapability? Tools { get; set; }
}

public class ToolsCapability
{
    public bool? ListChanged { get; set; }
}

public class ListToolsRequest
{
    // Empty for now
}

public class ListToolsResult
{
    public List<Tool> Tools { get; set; } = new();
}

public class Tool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public JsonSchema InputSchema { get; set; } = new();
}

public class JsonSchema
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, JsonSchema>? Properties { get; set; }
    public string? Description { get; set; }
    public JsonSchema? Items { get; set; }
    public string[]? Required { get; set; }
}

public class CallToolRequest
{
    public string Name { get; set; } = string.Empty;
    public JsonElement? Arguments { get; set; }
}

public class CallToolResult
{
    public List<ContentItem> Content { get; set; } = new();
    public bool IsError { get; set; }
}

public abstract class ContentItem
{
    public string Type { get; set; } = string.Empty;
}

public class TextContent : ContentItem
{
    public string Text { get; set; } = string.Empty;
}

public class PromptUpdateRequest
{
    public string Key { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
}