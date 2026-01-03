using B2Connect.Admin.MCP.Models;

namespace B2Connect.Admin.MCP.Services;

public interface IMcpService
{
    Task<JsonRpcResponse> Initialize(JsonRpcRequest request);
    Task<JsonRpcResponse> ListTools(JsonRpcRequest request);
    Task<JsonRpcResponse> CallTool(JsonRpcRequest request);
}