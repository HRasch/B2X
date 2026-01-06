using System.ComponentModel;
using McpDotNet.Server;
using B2Connect.Tools.WolverineMCP.Services;

namespace B2Connect.Tools.WolverineMCP.Tools;

/// <summary>
/// MCP tools for validating dependency injection.
/// </summary>
[McpToolType]
public class ValidateDITool(DependencyInjectionService diService)
{
    [McpTool, Description("Validates dependency injection setup in Wolverine handlers and services.")]
    public async Task<string> ValidateDIAsync(
        [Description("Workspace root directory")] string workspacePath)
    {
        try
        {
            var results = await diService.ValidateDependencyInjectionAsync(workspacePath);

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