using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace B2X.Admin.MCP.Services;

/// <summary>
/// Interface for MCP Server implementation
/// </summary>
public interface IMcpServer
{
    Task<InitializeResult> InitializeAsync(InitializeRequest request);
    Task<ListToolsResult> ListToolsAsync();
    Task<CallToolResult> CallToolAsync(CallToolRequest request);
}