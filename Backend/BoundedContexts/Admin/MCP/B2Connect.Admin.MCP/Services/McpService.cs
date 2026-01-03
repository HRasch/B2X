using B2Connect.Admin.MCP.Models;

namespace B2Connect.Admin.MCP.Services;

public class McpService : IMcpService
{
    public Task<JsonRpcResponse> Initialize(JsonRpcRequest request)
    {
        var result = new McpInitializeResult
        {
            Server = new Implementation
            {
                Name = "B2Connect Admin MCP Server",
                Version = "1.0.0"
            },
            Capabilities = new Capabilities
            {
                Tools = new ToolsCapability { ListChanged = false }
            }
        };

        return Task.FromResult(new JsonRpcResponse
        {
            Id = request.Id,
            Result = result
        });
    }

    public Task<JsonRpcResponse> ListTools(JsonRpcRequest request)
    {
        var tools = new[]
        {
            new Tool
            {
                Name = "get_tenant_info",
                Description = "Get information about a tenant",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        tenantId = new { type = "string", description = "The tenant ID" }
                    },
                    required = new[] { "tenantId" }
                }
            },
            new Tool
            {
                Name = "list_tenants",
                Description = "List all tenants",
                InputSchema = new
                {
                    type = "object",
                    properties = new { }
                }
            }
        };

        var result = new ListToolsResult { Tools = tools };

        return Task.FromResult(new JsonRpcResponse
        {
            Id = request.Id,
            Result = result
        });
    }

    public Task<JsonRpcResponse> CallTool(JsonRpcRequest request)
    {
        var callRequest = request.Params as CallToolRequest;
        if (callRequest == null)
        {
            return Task.FromResult(new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError { Code = -32602, Message = "Invalid params" }
            });
        }

        // For now, mock implementations
        object content = callRequest.Name switch
        {
            "get_tenant_info" => new { tenantId = callRequest.Arguments?.ToString(), name = "Mock Tenant" },
            "list_tenants" => new[] { new { id = "1", name = "Tenant 1" }, new { id = "2", name = "Tenant 2" } },
            _ => new { error = "Tool not found" }
        };

        var result = new CallToolResult { Content = content };

        return Task.FromResult(new JsonRpcResponse
        {
            Id = request.Id,
            Result = result
        });
    }
}