using System.ComponentModel;
using McpSharp.Server;
using B2Connect.Tools.WolverineMCP.Services;

namespace B2Connect.Tools.WolverineMCP.Tools;

/// <summary>
/// MCP tools for analyzing Wolverine handlers.
/// </summary>
[McpServerToolType]
public class AnalyzeHandlersTool
{
    private readonly WolverineAnalysisService _analysisService;

    public AnalyzeHandlersTool(WolverineAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    [McpServerTool, Description("Analyze Wolverine message handlers for CQRS patterns")]
    public async Task<string> AnalyzeHandlersAsync(
        [Description("Workspace root directory")] string workspacePath)
    {
        try
        {
            var results = await _analysisService.AnalyzeHandlersAsync(workspacePath);

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