using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;

namespace B2X.Tools.WolverineMCP.Services;

/// <summary>
/// Service for running architecture tests using Roslyn.
/// </summary>
public sealed class ArchitectureTestingService : IDisposable
{
    private readonly ILogger<ArchitectureTestingService> _logger;
    private MSBuildWorkspace? _workspace;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public ArchitectureTestingService(ILogger<ArchitectureTestingService> logger)
    {
        _logger = logger;
    }

    public async Task<ArchitectureTestResults> RunArchitectureTestsAsync(string solutionPath)
    {
        var results = new ArchitectureTestResults();

        try
        {
            var solution = await GetSolutionAsync(solutionPath);

            // Run layer dependency tests
            var layerResults = await RunLayerDependencyTestsAsync(solution);
            results.LayerViolations.AddRange(layerResults);

            // Run bounded context isolation tests
            var bcResults = await RunBoundedContextTestsAsync(solution);
            results.BoundedContextViolations.AddRange(bcResults);

            // Run Wolverine pattern tests
            var wolverineResults = await RunWolverinePatternTestsAsync(solution);
            results.WolverineViolations.AddRange(wolverineResults);

            // Run naming convention tests
            var namingResults = await RunNamingConventionTestsAsync(solution);
            results.NamingViolations.AddRange(namingResults);

            results.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running architecture tests");
            results.Success = false;
            results.ErrorMessage = ex.Message;
        }

        return results;
    }

    private async Task<Solution> GetSolutionAsync(string solutionPath)
    {
        await _lock.WaitAsync();
        try
        {
            _workspace ??= MSBuildWorkspace.Create();
            return await _workspace.OpenSolutionAsync(solutionPath);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<IEnumerable<ArchitectureViolation>> RunLayerDependencyTestsAsync(Solution solution)
    {
        var violations = new List<ArchitectureViolation>();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation is null)
                continue;

            var coreTypes = GetTypesInNamespace(compilation, "Core");
            var infrastructureTypes = GetTypesInNamespace(compilation, "Infrastructure");
            var applicationTypes = GetTypesInNamespace(compilation, "Application");

            // Check Core dependencies
            foreach (var coreType in coreTypes)
            {
                if (DependsOnNamespace(coreType, infrastructureTypes))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "LayerDependency",
                        Description = $"Domain Core type '{coreType.Name}' depends on Infrastructure",
                        ViolatingType = coreType.Name,
                        Severity = ViolationSeverity.Error
                    });
                }
            }

            // Check Application dependencies
            foreach (var appType in applicationTypes)
            {
                if (DependsOnNamespace(appType, infrastructureTypes))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "LayerDependency",
                        Description = $"Application type '{appType.Name}' depends directly on Infrastructure",
                        ViolatingType = appType.Name,
                        Severity = ViolationSeverity.Warning
                    });
                }
            }
        }

        return violations;
    }

    private async Task<IEnumerable<ArchitectureViolation>> RunBoundedContextTestsAsync(Solution solution)
    {
        var violations = new List<ArchitectureViolation>();

        // Simple bounded context check - ensure contexts don't cross-reference inappropriately
        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation is null)
                continue;

            var allTypes = GetAllTypes(compilation);
            var contextGroups = GroupTypesByBoundedContext(allTypes);

            foreach (var contextGroup in contextGroups)
            {
                var crossReferences = FindCrossContextReferences(contextGroup.Value, contextGroups.Keys);
                foreach (var reference in crossReferences)
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "BoundedContextIsolation",
                        Description = $"Type '{reference.fromType}' in context '{contextGroup.Key}' references '{reference.toType}' in different context",
                        ViolatingType = reference.fromType,
                        Severity = ViolationSeverity.Warning
                    });
                }
            }
        }

        return violations;
    }

    private async Task<IEnumerable<ArchitectureViolation>> RunWolverinePatternTestsAsync(Solution solution)
    {
        var violations = new List<ArchitectureViolation>();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation is null)
                continue;

            var handlers = FindHandlers(compilation);
            var commands = FindCommands(compilation);
            var queries = FindQueries(compilation);

            // Check that handlers implement correct interfaces
            foreach (var handler in handlers)
            {
                var hasCommandInterface = handler.AllInterfaces.Any(i =>
                    i.Name.Contains("ICommandHandler") || i.Name.Contains("IQueryHandler"));
                if (!hasCommandInterface)
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "WolverinePattern",
                        Description = $"Handler '{handler.Name}' does not implement ICommandHandler or IQueryHandler",
                        ViolatingType = handler.Name,
                        Severity = ViolationSeverity.Warning
                    });
                }
            }

            // Check command/query naming
            foreach (var command in commands)
            {
                if (!command.Name.EndsWith("Command"))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "WolverinePattern",
                        Description = $"Command type '{command.Name}' should end with 'Command'",
                        ViolatingType = command.Name,
                        Severity = ViolationSeverity.Info
                    });
                }
            }

            foreach (var query in queries)
            {
                if (!query.Name.EndsWith("Query"))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "WolverinePattern",
                        Description = $"Query type '{query.Name}' should end with 'Query'",
                        ViolatingType = query.Name,
                        Severity = ViolationSeverity.Info
                    });
                }
            }
        }

        return violations;
    }

    private async Task<IEnumerable<ArchitectureViolation>> RunNamingConventionTestsAsync(Solution solution)
    {
        var violations = new List<ArchitectureViolation>();

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation is null)
                continue;

            var allTypes = GetAllTypes(compilation);

            foreach (var type in allTypes)
            {
                // Check interface naming
                if (type.TypeKind == TypeKind.Interface && !type.Name.StartsWith('I'))
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "NamingConvention",
                        Description = $"Interface '{type.Name}' should start with 'I'",
                        ViolatingType = type.Name,
                        Severity = ViolationSeverity.Info
                    });
                }

                // Check async method naming
                var asyncMethods = type.GetMembers().OfType<IMethodSymbol>()
                    .Where(m => m.IsAsync && !m.Name.EndsWith("Async"));

                foreach (var method in asyncMethods)
                {
                    violations.Add(new ArchitectureViolation
                    {
                        Rule = "NamingConvention",
                        Description = $"Async method '{method.Name}' in type '{type.Name}' should end with 'Async'",
                        ViolatingType = $"{type.Name}.{method.Name}",
                        Severity = ViolationSeverity.Info
                    });
                }
            }
        }

        return violations;
    }

    private IEnumerable<INamedTypeSymbol> GetTypesInNamespace(Compilation compilation, string namespacePart)
    {
        return GetAllTypes(compilation)
            .Where(type => type.ContainingNamespace?.ToDisplayString().Contains(namespacePart) == true);
    }

    private IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation)
    {
        return compilation.GlobalNamespace.GetNamespaceMembers()
            .SelectMany(ns => GetTypesInNamespaceRecursive(ns));
    }

    private IEnumerable<INamedTypeSymbol> GetTypesInNamespaceRecursive(INamespaceSymbol namespaceSymbol)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            yield return type;
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var type in GetTypesInNamespaceRecursive(childNamespace))
            {
                yield return type;
            }
        }
    }

    private bool DependsOnNamespace(INamedTypeSymbol type, IEnumerable<INamedTypeSymbol> targetTypes)
    {
        var targetTypeNames = new HashSet<string>(targetTypes.Select(t => t.Name));
        return type.GetMembers().OfType<IMethodSymbol>()
            .SelectMany(m => m.Parameters.AsEnumerable().Cast<ITypeSymbol>().Concat(new[] { m.ReturnType }))
            .Any(p => p is INamedTypeSymbol namedType && targetTypeNames.Contains(namedType.Name));
    }

    private Dictionary<string, List<INamedTypeSymbol>> GroupTypesByBoundedContext(IEnumerable<INamedTypeSymbol> types)
    {
        var groups = new Dictionary<string, List<INamedTypeSymbol>>();

        foreach (var type in types)
        {
            var context = GetBoundedContext(type);
            if (!groups.ContainsKey(context))
                groups[context] = new List<INamedTypeSymbol>();
            groups[context].Add(type);
        }

        return groups;
    }

    private string GetBoundedContext(INamedTypeSymbol type)
    {
        var namespaceParts = type.ContainingNamespace?.ToDisplayString().Split('.') ?? Array.Empty<string>();
        return namespaceParts.FirstOrDefault(p => p.Contains("Context") || p.Contains("Domain")) ?? "Default";
    }

    private IEnumerable<(string fromType, string toType)> FindCrossContextReferences(
        List<INamedTypeSymbol> contextTypes, IEnumerable<string> allContexts)
    {
        var results = new List<(string, string)>();

        foreach (var type in contextTypes)
        {
            var referencedTypes = type.GetMembers().OfType<IMethodSymbol>()
                .SelectMany(m => m.Parameters.AsEnumerable().Cast<ITypeSymbol>().Concat(new[] { m.ReturnType }))
                .OfType<INamedTypeSymbol>();

            foreach (var referencedType in referencedTypes)
            {
                var referencedContext = GetBoundedContext(referencedType);
                if (referencedContext != GetBoundedContext(type) && allContexts.Contains(referencedContext))
                {
                    results.Add((type.Name, referencedType.Name));
                }
            }
        }

        return results;
    }

    private IEnumerable<INamedTypeSymbol> FindHandlers(Compilation compilation)
    {
        return GetAllTypes(compilation)
            .Where(type => type.TypeKind == TypeKind.Class &&
                          type.AllInterfaces.Any(i =>
                              i.Name.Contains("Handler") ||
                              i.Name.Contains("ICommandHandler") ||
                              i.Name.Contains("IQueryHandler") ||
                              i.Name.Contains("IEventHandler")));
    }

    private IEnumerable<INamedTypeSymbol> FindCommands(Compilation compilation)
    {
        return GetAllTypes(compilation)
            .Where(type => type.Name.EndsWith("Command") && type.TypeKind == TypeKind.Class);
    }

    private IEnumerable<INamedTypeSymbol> FindQueries(Compilation compilation)
    {
        return GetAllTypes(compilation)
            .Where(type => type.Name.EndsWith("Query") && type.TypeKind == TypeKind.Class);
    }

    public void Dispose()
    {
        _workspace?.Dispose();
    }
}

public class ArchitectureTestResults
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<ArchitectureViolation> LayerViolations { get; } = new();
    public List<ArchitectureViolation> BoundedContextViolations { get; } = new();
    public List<ArchitectureViolation> WolverineViolations { get; } = new();
    public List<ArchitectureViolation> NamingViolations { get; } = new();

    public int TotalViolations => LayerViolations.Count + BoundedContextViolations.Count +
                                 WolverineViolations.Count + NamingViolations.Count;
}

public class ArchitectureViolation
{
    public string Rule { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ViolatingType { get; set; } = string.Empty;
    public ViolationSeverity Severity { get; set; }
}

public enum ViolationSeverity
{
    Error,
    Warning,
    Info
}
