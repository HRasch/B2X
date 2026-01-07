using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using B2Connect.Admin.MCP.Services;

namespace B2Connect.Admin.MCP.Middleware;

/// <summary>
/// Middleware that enforces EXCLUSIVE AI access through MCP server
/// NO other system components can access AI services directly
/// </summary>
public class AiConsumptionControlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AiConsumptionControlMiddleware> _logger;
    private readonly AiConsumptionGateway _aiGateway;

    public AiConsumptionControlMiddleware(
        RequestDelegate next,
        ILogger<AiConsumptionControlMiddleware> logger,
        AiConsumptionGateway aiGateway)
    {
        _next = next;
        _logger = logger;
        _aiGateway = aiGateway;
    }

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        // Only allow AI requests through MCP endpoints
        if (IsAiRelatedEndpoint(context.Request.Path))
        {
            // Verify this is an MCP server request (not external access)
            if (!IsMcpServerRequest(context))
            {
                _logger.LogWarning("Blocked direct AI access attempt from {Path} by tenant {TenantId}",
                    context.Request.Path, tenantContext.TenantId);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Direct AI access forbidden",
                    message = "AI services are exclusively accessible through MCP server",
                    code = "EXCLUSIVE_AI_ACCESS_VIOLATION"
                });
                return;
            }

            // Log AI consumption for monitoring
            _logger.LogInformation("AI request authorized: Tenant={TenantId}, Endpoint={Endpoint}, User={UserId}",
                tenantContext.TenantId, context.Request.Path, tenantContext.UserId);
        }

        await _next(context);
    }

    private bool IsAiRelatedEndpoint(PathString path)
    {
        // Define AI-related endpoints that should only be accessible via MCP
        var aiEndpoints = new[]
        {
            "/api/ai/",
            "/api/openai/",
            "/api/anthropic/",
            "/api/azure-ai/",
            "/api/completions",
            "/api/chat/completions",
            "/api/embeddings"
        };

        return aiEndpoints.Any(endpoint => path.StartsWithSegments(endpoint));
    }

    private bool IsMcpServerRequest(HttpContext context)
    {
        // Check if request comes from MCP server components
        // This could be enhanced with API keys, certificates, or internal routing

        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var referer = context.Request.Headers["Referer"].ToString();

        // Allow requests from MCP server itself
        if (userAgent.Contains("MCP-Server") || referer.Contains("/mcp/"))
        {
            return true;
        }

        // Allow requests with valid MCP authorization header
        var mcpAuth = context.Request.Headers["X-MCP-Authorization"].ToString();
        if (!string.IsNullOrEmpty(mcpAuth) && mcpAuth.StartsWith("MCP ", StringComparison.OrdinalIgnoreCase))
        {
            // TODO: Validate MCP authorization token
            return true;
        }

        // For development, allow localhost requests (can be removed in production)
        if (context.Connection.LocalIpAddress?.ToString() == "127.0.0.1" ||
            context.Connection.LocalIpAddress?.ToString() == "::1")
        {
            return true;
        }

        return false;
    }
}