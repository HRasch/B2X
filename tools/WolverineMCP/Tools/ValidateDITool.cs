using System.ComponentModel;
using McpSharp.Server;
using B2Connect.Tools.WolverineMCP.Services;

namespace B2Connect.Tools.WolverineMCP.Tools;

/// <summary>
/// MCP tools for validating dependency injection.
/// </summary>
[McpServerToolType]
public class ValidateDITool
{
    private readonly DependencyInjectionService _diService;

    public ValidateDITool(DependencyInjectionService diService)
    {
        _diService = diService;
    }

    [McpServerTool, Description("Validate dependency injection setup in Wolverine handlers")]
    public async Task<string> ValidateDIAsync(
        [Description("Workspace root directory")] string workspacePath)
    {
        try
        {
            var results = await _diService.ValidateDependencyInjectionAsync(workspacePath);

            var output = "DI validation results:\n";
            foreach (var result in results)
            {
                output += $"- {result.ServiceName}: {result.Status} ({result.Message})\n";
            }

            return output;
        }
        catch (Exception ex)
        {
            return $"Error validating DI: {ex.Message}";
        }
    }
}