using System.ComponentModel;
using McpDotNet.Server;
using B2X.Tools.WolverineMCP.Services;

namespace B2X.Tools.WolverineMCP.Tools;

/// <summary>
/// MCP tools for analyzing Wolverine handlers.
/// </summary>
[McpToolType]
public class AnalyzeHandlersTool(WolverineAnalysisService analysisService)
{
    [McpTool, Description("Analyzes Wolverine message handlers in the codebase to identify CQRS patterns and potential issues.")]
    public async Task<string> AnalyzeHandlersAsync(
        [Description("Workspace root directory")] string workspacePath)
    {
        try
        {
            var results = await analysisService.AnalyzeHandlersAsync(workspacePath);

            var output = $"Found {results.Count()} handlers:\n";
            foreach (var result in results)
            {
                output += $"- {result.HandlerName}: {result.Pattern} ({result.FilePath})\n";
            }

            return output;
        }
        catch (Exception ex)
        {
            return $"Error analyzing handlers: {ex.Message}";
        }
    }
}