using B2X.Admin.MCP.Models;
using B2X.Admin.MCP.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Admin.MCP.Controllers;

[ApiController]
[Route("mcp")]
public class McpController : ControllerBase
{
    private readonly IMcpService _mcpService;

    public McpController(IMcpService mcpService)
    {
        _mcpService = mcpService;
    }

    [HttpPost]
    public async Task<IActionResult> HandleRequest([FromBody] JsonRpcRequest request)
    {
        try
        {
            JsonRpcResponse response = request.Method switch
            {
                "initialize" => await _mcpService.Initialize(request),
                "tools/list" => await _mcpService.ListTools(request),
                "tools/call" => await _mcpService.CallTool(request),
                _ => CreateErrorResponse(request.Id, -32601, $"Method '{request.Method}' not found")
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return Ok(CreateErrorResponse(request.Id, -32603, ex.Message));
        }
    }

    private JsonRpcResponse CreateErrorResponse(string id, int code, string message)
    {
        return new JsonRpcResponse
        {
            Id = id,
            Error = new JsonRpcError
            {
                Code = code,
                Message = message
            }
        };
    }
}
