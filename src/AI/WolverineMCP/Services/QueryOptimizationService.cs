using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace B2X.Tools.WolverineMCP.Services;

/// <summary>
/// Result of query analysis.
/// </summary>
public record QueryAnalysisResult
{
    public required string QueryType { get; init; }
    public required string Issue { get; init; }
    public required string FilePath { get; init; }
    public required int LineNumber { get; init; }
}

/// <summary>
/// Service for analyzing and optimizing PostgreSQL queries in EF Core.
/// </summary>
public sealed class QueryOptimizationService
{
    private readonly ILogger<QueryOptimizationService> _logger;

    public QueryOptimizationService(ILogger<QueryOptimizationService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<QueryIssue> AnalyzeQueries(Compilation compilation)
    {
        var issues = new List<QueryIssue>();

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            // Find LINQ queries
            var linqQueries = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(inv => IsLinqQuery(inv, semanticModel));

            foreach (var query in linqQueries)
            {
                var issuesForQuery = AnalyzeLinqQuery(query, semanticModel);
                issues.AddRange(issuesForQuery);
            }

            // Find raw SQL queries
            var rawSqlQueries = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(inv => IsRawSqlQuery(inv, semanticModel));

            foreach (var query in rawSqlQueries)
            {
                var issuesForQuery = AnalyzeRawSqlQuery(query, semanticModel);
                issues.AddRange(issuesForQuery);
            }
        }

        return issues;
    }

    private bool IsLinqQuery(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        if (semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol)
            return false;

        var methodName = symbol.Name;
        return methodName is "Where" or "Select" or "FirstOrDefault" or "SingleOrDefault" or "ToList" or "Count" or "Any";
    }

    private bool IsRawSqlQuery(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        if (semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol)
            return false;

        var methodName = symbol.Name;
        return methodName.Contains("FromSql") || methodName.Contains("ExecuteSql");
    }

    private IEnumerable<QueryIssue> AnalyzeLinqQuery(InvocationExpressionSyntax query, SemanticModel semanticModel)
    {
        var issues = new List<QueryIssue>();

        // Check for N+1 query patterns
        if (HasPotentialNPlusOne(query, semanticModel))
        {
            issues.Add(new QueryIssue
            {
                Type = QueryIssueType.NPlusOne,
                Message = "Potential N+1 query detected. Consider using Include() or Select() for eager loading.",
                Location = query.GetLocation(),
                Severity = QuerySeverity.Warning
            });
        }

        // Check for inefficient operations
        if (HasInefficientOperation(query))
        {
            issues.Add(new QueryIssue
            {
                Type = QueryIssueType.Inefficient,
                Message = "Inefficient query operation detected. Consider optimizing the query.",
                Location = query.GetLocation(),
                Severity = QuerySeverity.Info
            });
        }

        return issues;
    }

    private IEnumerable<QueryIssue> AnalyzeRawSqlQuery(InvocationExpressionSyntax query, SemanticModel semanticModel)
    {
        var issues = new List<QueryIssue>();

        // Check for parameterized queries
        if (!IsParameterizedQuery(query))
        {
            issues.Add(new QueryIssue
            {
                Type = QueryIssueType.SqlInjection,
                Message = "Raw SQL query may be vulnerable to SQL injection. Use parameterized queries.",
                Location = query.GetLocation(),
                Severity = QuerySeverity.Error
            });
        }

        return issues;
    }

    public async Task<IEnumerable<QueryAnalysisResult>> AnalyzeQueriesAsync(string workspacePath)
    {
        var solutionPath = Path.Combine(workspacePath, "B2X.slnx");
        if (!File.Exists(solutionPath))
        {
            throw new FileNotFoundException("Solution file not found", solutionPath);
        }

        // For now, return sample results - full implementation would analyze the solution
        return new List<QueryAnalysisResult>
        {
            new() { QueryType = "LINQ", Issue = "Potential N+1 query", FilePath = "SampleHandler.cs", LineNumber = 42 }
        };
    }

    private bool HasPotentialNPlusOne(InvocationExpressionSyntax query, SemanticModel semanticModel)
    {
        // Simple heuristic: check if there are property accesses in loops after queries
        var containingMethod = query.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (containingMethod is null)
            return false;

        var propertyAccesses = containingMethod.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Where(ma => ma.Kind() == SyntaxKind.SimpleMemberAccessExpression);

        // This is a simplified check - in practice, you'd need more sophisticated analysis
        return propertyAccesses.Any();
    }

    private bool HasInefficientOperation(InvocationExpressionSyntax query)
    {
        // Check for ToList() followed by Count() or similar
        var methodName = query.Expression.ToString();
        return methodName.Contains("ToList") || methodName.Contains("ToArray");
    }

    private bool IsParameterizedQuery(InvocationExpressionSyntax query)
    {
        // Check if the query uses parameters
        return query.ArgumentList.Arguments.Count > 1;
    }
}

public enum QueryIssueType
{
    NPlusOne,
    Inefficient,
    SqlInjection
}

public enum QuerySeverity
{
    Error,
    Warning,
    Info
}

public class QueryIssue
{
    public QueryIssueType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public Location Location { get; set; } = Location.None;
    public QuerySeverity Severity { get; set; }
}
