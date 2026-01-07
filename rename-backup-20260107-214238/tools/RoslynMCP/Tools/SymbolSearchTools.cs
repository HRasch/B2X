using System.ComponentModel;
using System.Text.RegularExpressions;
using B2Connect.Tools.RoslynMCP.Services;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace B2Connect.Tools.RoslynMCP.Tools;

/// <summary>
/// MCP tools for searching symbols in C# solutions.
/// </summary>
[McpServerToolType]
public sealed class SymbolSearchTools
{
    private readonly CodeAnalysisService _codeAnalysis;
    private readonly ILogger<SymbolSearchTools> _logger;

    public SymbolSearchTools(CodeAnalysisService codeAnalysis, ILogger<SymbolSearchTools> logger)
    {
        _codeAnalysis = codeAnalysis;
        _logger = logger;
    }

    [McpServerTool, Description("Search for classes, methods, properties, and other symbols using wildcard patterns (e.g., '*Service', 'Get*User')")]
    public async Task<string> SearchSymbolsAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath,
        [Description("Wildcard pattern to match symbol names (e.g., '*Service', 'Get*')")] string pattern,
        [Description("Symbol types to search: class, interface, struct, enum, method, property, field (comma-separated)")] string symbolTypes = "class,interface,method,property",
        [Description("Ignore case when matching")] bool ignoreCase = true,
        [Description("Maximum number of results")] int maxResults = 50)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var typeFilter = ParseSymbolTypes(symbolTypes);
            var regex = CreateWildcardRegex(pattern, ignoreCase);

            var results = new List<SymbolResult>();

            foreach (var project in solution.Projects.Where(p => p.SupportsCompilation))
            {
                var compilation = await _codeAnalysis.GetCompilationAsync(project);
                if (compilation is null)
                    continue;

                var symbols = GetAllSymbols(compilation.GlobalNamespace)
                    .Where(s => typeFilter.Contains(s.Kind) &&
                               (regex.IsMatch(s.Name) || regex.IsMatch(s.ToDisplayString())));

                foreach (var symbol in symbols)
                {
                    results.Add(CreateSymbolResult(symbol, project));
                    if (results.Count >= maxResults)
                        break;
                }

                if (results.Count >= maxResults)
                    break;
            }

            return FormatResults(results.OrderBy(r => r.Name).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching symbols");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get detailed information about a specific symbol including its signature, parameters, and documentation")]
    public async Task<string> GetSymbolInfoAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath,
        [Description("The exact name of the symbol to find")] string symbolName)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);

            foreach (var project in solution.Projects.Where(p => p.SupportsCompilation))
            {
                var compilation = await _codeAnalysis.GetCompilationAsync(project);
                if (compilation is null)
                    continue;

                var symbol = GetAllSymbols(compilation.GlobalNamespace)
                    .FirstOrDefault(s => s.Name.Equals(symbolName, StringComparison.OrdinalIgnoreCase));

                if (symbol is not null)
                {
                    return FormatSymbolInfo(symbol);
                }
            }

            return $"Symbol '{symbolName}' not found in solution.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting symbol info");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Find methods with high cyclomatic complexity that may need refactoring")]
    public async Task<string> AnalyzeComplexityAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath,
        [Description("Minimum complexity threshold to report")] int threshold = 10,
        [Description("Maximum number of results")] int maxResults = 20)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var complexMethods = new List<(IMethodSymbol Method, int Complexity, string Project)>();

            foreach (var project in solution.Projects.Where(p => p.SupportsCompilation))
            {
                var compilation = await _codeAnalysis.GetCompilationAsync(project);
                if (compilation is null)
                    continue;

                var methods = GetAllSymbols(compilation.GlobalNamespace)
                    .OfType<IMethodSymbol>()
                    .Where(m => !m.IsImplicitlyDeclared);

                foreach (var method in methods)
                {
                    var complexity = EstimateComplexity(method);
                    if (complexity >= threshold)
                    {
                        complexMethods.Add((method, complexity, project.Name));
                    }
                }
            }

            var results = complexMethods
                .OrderByDescending(x => x.Complexity)
                .Take(maxResults)
                .Select(x => $"- {x.Method.ContainingType?.Name}.{x.Method.Name}: Complexity ~{x.Complexity} ({x.Project})")
                .ToList();

            if (results.Count == 0)
            {
                return $"No methods found with complexity >= {threshold}";
            }

            return $"Methods with complexity >= {threshold}:\n\n{string.Join("\n", results)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing complexity");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("List all projects in a solution with their types and references")]
    public async Task<string> ListProjectsAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var results = new List<string>();

            foreach (var project in solution.Projects.OrderBy(p => p.Name))
            {
                var refs = project.ProjectReferences.Count();
                var docs = project.Documents.Count();
                results.Add($"- {project.Name}: {docs} files, {refs} project references");
            }

            return $"Projects in solution ({results.Count}):\n\n{string.Join("\n", results)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing projects");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Find all usages of a type or member across the solution")]
    public async Task<string> FindUsagesAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath,
        [Description("The name of the symbol to find usages for")] string symbolName,
        [Description("Maximum number of results")] int maxResults = 30)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var usages = new List<string>();

            // Find the symbol definition first
            ISymbol? targetSymbol = null;
            foreach (var project in solution.Projects.Where(p => p.SupportsCompilation))
            {
                var compilation = await _codeAnalysis.GetCompilationAsync(project);
                if (compilation is null)
                    continue;

                targetSymbol = GetAllSymbols(compilation.GlobalNamespace)
                    .FirstOrDefault(s => s.Name.Equals(symbolName, StringComparison.OrdinalIgnoreCase));

                if (targetSymbol is not null)
                    break;
            }

            if (targetSymbol is null)
            {
                return $"Symbol '{symbolName}' not found.";
            }

            // Search for usages in all documents
            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var text = await document.GetTextAsync();
                    var content = text.ToString();

                    // Simple text-based search for the symbol name
                    var lines = content.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains(symbolName) && usages.Count < maxResults)
                        {
                            var fileName = Path.GetFileName(document.FilePath ?? "unknown");
                            usages.Add($"- {fileName}:{i + 1}: {lines[i].Trim().Substring(0, Math.Min(80, lines[i].Trim().Length))}");
                        }
                    }
                }
            }

            if (usages.Count == 0)
            {
                return $"No usages found for '{symbolName}'.";
            }

            return $"Usages of '{symbolName}' ({usages.Count}):\n\n{string.Join("\n", usages)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding usages");
            return $"Error: {ex.Message}";
        }
    }

    private static IEnumerable<ISymbol> GetAllSymbols(INamespaceSymbol namespaceSymbol)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            yield return member;

            if (member is INamespaceSymbol nestedNamespace)
            {
                foreach (var nested in GetAllSymbols(nestedNamespace))
                {
                    yield return nested;
                }
            }
            else if (member is INamedTypeSymbol namedType)
            {
                foreach (var typeMember in namedType.GetMembers())
                {
                    yield return typeMember;
                }
            }
        }
    }

    private static HashSet<SymbolKind> ParseSymbolTypes(string symbolTypes)
    {
        var types = new HashSet<SymbolKind>();

        foreach (var type in symbolTypes.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            switch (type.Trim().ToLowerInvariant())
            {
                case "class":
                case "interface":
                case "struct":
                case "enum":
                    types.Add(SymbolKind.NamedType);
                    break;
                case "method":
                    types.Add(SymbolKind.Method);
                    break;
                case "property":
                    types.Add(SymbolKind.Property);
                    break;
                case "field":
                    types.Add(SymbolKind.Field);
                    break;
            }
        }

        return types;
    }

    private static Regex CreateWildcardRegex(string pattern, bool ignoreCase)
    {
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        var options = RegexOptions.Compiled;
        if (ignoreCase)
            options |= RegexOptions.IgnoreCase;
        return new Regex(regexPattern, options);
    }

    private static SymbolResult CreateSymbolResult(ISymbol symbol, Project project)
    {
        var location = symbol.Locations.FirstOrDefault();
        var lineNumber = location?.GetLineSpan().StartLinePosition.Line + 1 ?? 0;

        return new SymbolResult
        {
            Name = symbol.Name,
            FullName = symbol.ToDisplayString(),
            Kind = GetSymbolKind(symbol),
            Project = project.Name,
            File = Path.GetFileName(location?.SourceTree?.FilePath ?? ""),
            Line = lineNumber
        };
    }

    private static string GetSymbolKind(ISymbol symbol)
    {
        return symbol switch
        {
            INamedTypeSymbol namedType => namedType.TypeKind.ToString(),
            IMethodSymbol => "Method",
            IPropertySymbol => "Property",
            IFieldSymbol => "Field",
            _ => symbol.Kind.ToString()
        };
    }

    private static string FormatResults(List<SymbolResult> results)
    {
        if (results.Count == 0)
        {
            return "No symbols found matching the pattern.";
        }

        var lines = results.Select(r => $"- {r.Kind}: {r.FullName} ({r.Project}/{r.File}:{r.Line})");
        return $"Found {results.Count} symbols:\n\n{string.Join("\n", lines)}";
    }

    private static string FormatSymbolInfo(ISymbol symbol)
    {
        var info = new List<string>
        {
            $"**{symbol.Name}**",
            $"- Kind: {GetSymbolKind(symbol)}",
            $"- Full Name: {symbol.ToDisplayString()}",
            $"- Accessibility: {symbol.DeclaredAccessibility}",
            $"- Namespace: {symbol.ContainingNamespace?.ToDisplayString() ?? "global"}"
        };

        if (symbol is IMethodSymbol method)
        {
            info.Add($"- Return Type: {method.ReturnType.Name}");
            info.Add($"- Parameters: ({string.Join(", ", method.Parameters.Select(p => $"{p.Type.Name} {p.Name}"))})");
            info.Add($"- Is Async: {method.IsAsync}");
            info.Add($"- Is Static: {method.IsStatic}");
        }
        else if (symbol is IPropertySymbol property)
        {
            info.Add($"- Type: {property.Type.Name}");
            info.Add($"- Has Getter: {property.GetMethod is not null}");
            info.Add($"- Has Setter: {property.SetMethod is not null}");
        }
        else if (symbol is INamedTypeSymbol type)
        {
            info.Add($"- Type Kind: {type.TypeKind}");
            info.Add($"- Members: {type.GetMembers().Length}");
            if (type.BaseType is not null && type.BaseType.Name != "Object")
            {
                info.Add($"- Base Type: {type.BaseType.Name}");
            }
            if (type.Interfaces.Any())
            {
                info.Add($"- Interfaces: {string.Join(", ", type.Interfaces.Select(i => i.Name))}");
            }
        }

        var location = symbol.Locations.FirstOrDefault();
        if (location?.SourceTree is not null)
        {
            var lineSpan = location.GetLineSpan();
            info.Add($"- Location: {Path.GetFileName(location.SourceTree.FilePath)}:{lineSpan.StartLinePosition.Line + 1}");
        }

        var doc = symbol.GetDocumentationCommentXml();
        if (!string.IsNullOrWhiteSpace(doc))
        {
            info.Add($"- Documentation: Available");
        }

        return string.Join("\n", info);
    }

    private static int EstimateComplexity(IMethodSymbol method)
    {
        // Simplified complexity estimation based on method structure
        // Real implementation would analyze the syntax tree
        var complexity = 1; // Base complexity

        var location = method.Locations.FirstOrDefault();
        if (location?.SourceTree is null)
            return complexity;

        var text = location.SourceTree.GetText().ToString();
        var span = location.SourceSpan;

        if (span.Start >= text.Length || span.End > text.Length)
            return complexity;

        var methodText = text.Substring(span.Start, Math.Min(span.Length, text.Length - span.Start));

        // Count decision points
        complexity += Regex.Matches(methodText, @"\bif\b").Count;
        complexity += Regex.Matches(methodText, @"\belse\b").Count;
        complexity += Regex.Matches(methodText, @"\bwhile\b").Count;
        complexity += Regex.Matches(methodText, @"\bfor\b").Count;
        complexity += Regex.Matches(methodText, @"\bforeach\b").Count;
        complexity += Regex.Matches(methodText, @"\bcase\b").Count;
        complexity += Regex.Matches(methodText, @"\bcatch\b").Count;
        complexity += Regex.Matches(methodText, @"\?\?").Count;
        complexity += Regex.Matches(methodText, @"\?\.").Count;
        complexity += Regex.Matches(methodText, @"&&").Count;
        complexity += Regex.Matches(methodText, @"\|\|").Count;

        return complexity;
    }

    private sealed record SymbolResult
    {
        public required string Name { get; init; }
        public required string FullName { get; init; }
        public required string Kind { get; init; }
        public required string Project { get; init; }
        public required string File { get; init; }
        public required int Line { get; init; }
    }
}
