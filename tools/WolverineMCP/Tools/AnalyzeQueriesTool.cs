using System.ComponentModel;
using McpSharp.Server;
using B2Connect.Tools.WolverineMCP.Services;

namespace B2Connect.Tools.WolverineMCP.Tools;

/// <summary>
/// MCP tools for analyzing queries.
/// </summary>
[McpServerToolType]
public class AnalyzeQueriesTool
{
    private readonly QueryOptimizationService _queryService;

    public AnalyzeQueriesTool(QueryOptimizationService queryService)
    {
        _queryService = queryService;
    }

    [McpServerTool, Description("Analyze PostgreSQL queries in Wolverine handlers")]
    public async Task<string> AnalyzeQueriesAsync(
        [Description("Workspace root directory")] string workspacePath)
    {
        try
        {
            var results = await _queryService.AnalyzeQueriesAsync(workspacePath);

            var output = "Query analysis results:\n";
            foreach (var result in results)
            {
                output += $"- {result.QueryType}: {result.Issue} ({result.FilePath}:{result.LineNumber})\n";
            }

            return output;
        }
        catch (Exception ex)
        {
            return $"Error analyzing queries: {ex.Message}";
        }
    }
}