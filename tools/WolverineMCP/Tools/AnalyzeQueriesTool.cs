using System.ComponentModel;
using McpDotNet.Server;
using B2Connect.Tools.WolverineMCP.Services;

namespace B2Connect.Tools.WolverineMCP.Tools;

/// <summary>
/// MCP tools for analyzing queries.
/// </summary>
[McpToolType]
public class AnalyzeQueriesTool(QueryOptimizationService queryService)
{
    [McpTool, Description("Analyzes PostgreSQL queries in Wolverine handlers for optimization opportunities.")]
    public async Task<string> AnalyzeQueriesAsync(
        [Description("Workspace root directory")] string workspacePath)
    {
        try
        {
            var results = await queryService.AnalyzeQueriesAsync(workspacePath);

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