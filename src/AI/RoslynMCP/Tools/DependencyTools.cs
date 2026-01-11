using System.ComponentModel;
using B2X.Tools.RoslynMCP.Services;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace B2X.Tools.RoslynMCP.Tools;

/// <summary>
/// MCP tools for analyzing dependencies in C# solutions.
/// </summary>
[McpServerToolType]
public sealed class DependencyTools
{
    private readonly CodeAnalysisService _codeAnalysis;
    private readonly ILogger<DependencyTools> _logger;

    public DependencyTools(CodeAnalysisService codeAnalysis, ILogger<DependencyTools> logger)
    {
        _codeAnalysis = codeAnalysis;
        _logger = logger;
    }

    [McpServerTool, Description("Analyze project dependencies and find potential issues like circular references")]
    public async Task<string> AnalyzeDependenciesAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var results = new List<string>();
            var projectMap = solution.Projects.ToDictionary(p => p.Id, p => p.Name);

            foreach (var project in solution.Projects.OrderBy(p => p.Name))
            {
                var refs = project.ProjectReferences
                    .Select(r => projectMap.TryGetValue(r.ProjectId, out var name) ? name : "Unknown")
                    .OrderBy(n => n)
                    .ToList();

                if (refs.Count > 0)
                {
                    results.Add($"**{project.Name}** depends on:");
                    results.AddRange(refs.Select(r => $"  - {r}"));
                    results.Add("");
                }
                else
                {
                    results.Add($"**{project.Name}** has no project dependencies");
                    results.Add("");
                }
            }

            // Check for circular dependencies
            var circular = FindCircularDependencies(solution);
            if (circular.Count > 0)
            {
                results.Add("⚠️ **Circular Dependencies Detected:**");
                results.AddRange(circular.Select(c => $"  - {c}"));
            }

            return string.Join("\n", results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing dependencies");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Find which namespaces are used across projects to identify coupling")]
    public async Task<string> FindNamespaceUsagesAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath,
        [Description("Namespace pattern to search for (e.g., 'B2X.Domain')")] string namespacePattern)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var usages = new Dictionary<string, List<string>>();

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var text = await document.GetTextAsync();
                    var content = text.ToString();
                    var lines = content.Split('\n');

                    foreach (var line in lines)
                    {
                        if (line.TrimStart().StartsWith("using ") && line.Contains(namespacePattern))
                        {
                            var ns = line.Trim().TrimEnd(';').Replace("using ", "").Trim();

                            if (!usages.ContainsKey(ns))
                            {
                                usages[ns] = new List<string>();
                            }

                            if (!usages[ns].Contains(project.Name))
                            {
                                usages[ns].Add(project.Name);
                            }
                        }
                    }
                }
            }

            if (usages.Count == 0)
            {
                return $"No usages found for namespace pattern '{namespacePattern}'.";
            }

            var results = new List<string> { $"Namespace usages matching '{namespacePattern}':\n" };

            foreach (var (ns, projects) in usages.OrderBy(kv => kv.Key))
            {
                results.Add($"**{ns}** used by:");
                results.AddRange(projects.OrderBy(p => p).Select(p => $"  - {p}"));
                results.Add("");
            }

            return string.Join("\n", results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding namespace usages");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Check for architectural layer violations (e.g., Domain referencing Infrastructure)")]
    public async Task<string> AnalyzeLayerViolationsAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath)
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var violations = new List<string>();

            // Define layer rules (outer layer cannot be referenced by inner layer)
            var layerOrder = new[] { "Domain", "Application", "Infrastructure", "API", "Gateway", "Tests" };

            foreach (var project in solution.Projects)
            {
                var projectLayer = GetProjectLayer(project.Name, layerOrder);
                if (projectLayer < 0)
                    continue;

                foreach (var refProject in solution.Projects.Where(p =>
                    project.ProjectReferences.Any(r => r.ProjectId == p.Id)))
                {
                    var refLayer = GetProjectLayer(refProject.Name, layerOrder);
                    if (refLayer < 0)
                        continue;

                    // Check if referencing an outer layer from an inner layer
                    if (projectLayer < refLayer && !refProject.Name.Contains("Tests"))
                    {
                        violations.Add($"⚠️ {project.Name} ({layerOrder[projectLayer]}) → {refProject.Name} ({layerOrder[refLayer]})");
                    }
                }
            }

            if (violations.Count == 0)
            {
                return "✅ No layer violations detected.";
            }

            return $"Layer Violations ({violations.Count}):\n\n{string.Join("\n", violations)}\n\n" +
                   "Inner layers should not reference outer layers (Domain → Application → Infrastructure → API).";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing layer violations");
            return $"Error: {ex.Message}";
        }
    }

    private static List<string> FindCircularDependencies(Solution solution)
    {
        var circular = new List<string>();
        var projectMap = solution.Projects.ToDictionary(p => p.Id, p => p);

        foreach (var project in solution.Projects)
        {
            var visited = new HashSet<ProjectId>();
            var path = new Stack<string>();

            if (HasCircularDependency(project, projectMap, visited, path, project.Id))
            {
                circular.Add(string.Join(" → ", path.Reverse()) + " → " + project.Name);
            }
        }

        return circular.Distinct().ToList();
    }

    private static bool HasCircularDependency(
        Project current,
        Dictionary<ProjectId, Project> projectMap,
        HashSet<ProjectId> visited,
        Stack<string> path,
        ProjectId targetId)
    {
        if (visited.Contains(current.Id))
        {
            return current.Id == targetId && path.Count > 0;
        }

        visited.Add(current.Id);
        path.Push(current.Name);

        foreach (var refId in current.ProjectReferences.Select(r => r.ProjectId))
        {
            if (refId == targetId && path.Count > 0)
            {
                return true;
            }

            if (projectMap.TryGetValue(refId, out var refProject))
            {
                if (HasCircularDependency(refProject, projectMap, visited, path, targetId))
                {
                    return true;
                }
            }
        }

        path.Pop();
        return false;
    }

    [McpServerTool, Description("Analyze cross-domain dependencies and coupling between backend domains")]
    public async Task<string> AnalyzeCrossDomainDependenciesAsync(
        [Description("The solution file path (.sln or .slnx)")] string solutionPath,
        [Description("Comma-separated list of domain names to analyze")] string domains = "Catalog,CMS,Identity,Localization,Search")
    {
        try
        {
            var solution = await _codeAnalysis.GetSolutionAsync(solutionPath);
            var domainList = domains.Split(',').Select(d => d.Trim()).ToArray();
            var results = new List<string>();

            results.Add("# Cross-Domain Dependency Analysis");
            results.Add($"**Analyzed Domains:** {string.Join(", ", domainList)}");
            results.Add("");

            // Analyze namespace coupling between domains
            var domainNamespaces = new Dictionary<string, HashSet<string>>();
            var domainDependencies = new Dictionary<string, HashSet<string>>();

            foreach (var domain in domainList)
            {
                domainNamespaces[domain] = new HashSet<string>();
                domainDependencies[domain] = new HashSet<string>();
            }

            foreach (var project in solution.Projects)
            {
                var domainName = GetDomainFromProjectName(project.Name, domainList);
                if (domainName == null)
                    continue;

                // Collect namespaces used by this domain
                foreach (var document in project.Documents)
                {
                    var text = await document.GetTextAsync();
                    var content = text.ToString();
                    var usingStatements = ExtractUsingStatements(content);

                    foreach (var ns in usingStatements)
                    {
                        domainNamespaces[domainName].Add(ns);

                        // Check if this namespace belongs to another domain
                        var targetDomain = GetDomainFromNamespace(ns, domainList);
                        if (targetDomain != null && targetDomain != domainName)
                        {
                            domainDependencies[domainName].Add(targetDomain);
                        }
                    }
                }
            }

            // Generate dependency matrix
            results.Add("## Dependency Matrix");
            results.Add("| From \\ To | " + string.Join(" | ", domainList) + " |");
            results.Add("|" + string.Join("", domainList.Select(_ => "---|")) + "---|");

            foreach (var fromDomain in domainList)
            {
                var row = $"| {fromDomain} |";
                foreach (var toDomain in domainList)
                {
                    var hasDependency = domainDependencies[fromDomain].Contains(toDomain);
                    row += $" {(hasDependency ? "✅" : "❌")} |";
                }
                results.Add(row);
            }

            results.Add("");

            // Detailed coupling analysis
            results.Add("## Coupling Details");
            foreach (var domain in domainList)
            {
                var dependencies = domainDependencies[domain];
                var dependents = domainList.Where(d => domainDependencies[d].Contains(domain)).ToList();

                results.Add($"### {domain} Domain");
                results.Add($"**Depends on:** {string.Join(", ", dependencies.OrderBy(d => d))}");
                results.Add($"**Used by:** {string.Join(", ", dependents.OrderBy(d => d))}");
                results.Add($"**Coupling Score:** {dependencies.Count + dependents.Count}/{(domainList.Length - 1) * 2}");
                results.Add("");
            }

            // Identify tightly coupled domains
            var tightCouplings = new List<string>();
            foreach (var domain in domainList)
            {
                var couplingScore = domainDependencies[domain].Count +
                    domainList.Count(d => domainDependencies[d].Contains(domain));

                if (couplingScore >= domainList.Length - 1)
                {
                    tightCouplings.Add($"{domain} (score: {couplingScore})");
                }
            }

            if (tightCouplings.Count > 0)
            {
                results.Add("## ⚠️ High Coupling Alert");
                results.Add("The following domains show high coupling that may indicate architectural issues:");
                results.AddRange(tightCouplings.Select(c => $"- {c}"));
                results.Add("");
            }

            return string.Join("\n", results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing cross-domain dependencies");
            return $"Error: {ex.Message}";
        }
    }

    private static string? GetDomainFromProjectName(string projectName, string[] domains)
    {
        foreach (var domain in domains)
        {
            if (projectName.Contains(domain, StringComparison.OrdinalIgnoreCase))
            {
                return domain;
            }
        }
        return null;
    }

    private static string? GetDomainFromNamespace(string namespaceName, string[] domains)
    {
        foreach (var domain in domains)
        {
            if (namespaceName.Contains($"B2X.Domain.{domain}", StringComparison.OrdinalIgnoreCase))
            {
                return domain;
            }
        }
        return null;
    }

    private static List<string> ExtractUsingStatements(string content)
    {
        var usingStatements = new List<string>();
        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("using ") && trimmed.EndsWith(";"))
            {
                var ns = trimmed.Substring(6, trimmed.Length - 7).Trim();
                usingStatements.Add(ns);
            }
        }

        return usingStatements;
    }
