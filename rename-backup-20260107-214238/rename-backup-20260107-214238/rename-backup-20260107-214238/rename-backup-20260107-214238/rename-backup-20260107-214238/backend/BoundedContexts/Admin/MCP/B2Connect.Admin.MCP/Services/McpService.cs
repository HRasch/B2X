using B2Connect.Admin.MCP.Models;

namespace B2Connect.Admin.MCP.Services;

public class McpService : IMcpService
{
    public Task<JsonRpcResponse> Initialize(JsonRpcRequest request)
    {
        // TODO: Implement proper MCP initialization
        return Task.FromResult(new JsonRpcResponse { Id = request.Id });
    }

    public Task<JsonRpcResponse> ListTools(JsonRpcRequest request)
    {
        // TODO: Implement tool listing
        return Task.FromResult(new JsonRpcResponse { Id = request.Id });
    }

    public Task<JsonRpcResponse> CallTool(JsonRpcRequest request)
    {
        // TODO: Implement tool calling
        return Task.FromResult(new JsonRpcResponse { Id = request.Id });
    }
}