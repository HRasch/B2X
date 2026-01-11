using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace B2X.Tools.WolverineMCP.Services;

/// <summary>
/// Result of handler analysis.
/// </summary>
public record HandlerAnalysisResult
{
    public required string HandlerName { get; init; }
    public required string Pattern { get; init; }
    public required string FilePath { get; init; }
}

/// <summary>
/// Service for analyzing Wolverine CQRS patterns in .NET solutions.
/// </summary>
public sealed class WolverineAnalysisService : IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<WolverineAnalysisService> _logger;
    private MSBuildWorkspace? _workspace;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public WolverineAnalysisService(IMemoryCache cache, ILogger<WolverineAnalysisService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Solution> GetSolutionAsync(string solutionPath)
    {
        var cacheKey = $"solution:{solutionPath}";

        if (_cache.TryGetValue<Solution>(cacheKey, out var cachedSolution) && cachedSolution is not null)
        {
            return cachedSolution;
        }

        await _lock.WaitAsync();
        try
        {
            if (_cache.TryGetValue<Solution>(cacheKey, out cachedSolution) && cachedSolution is not null)
            {
                return cachedSolution;
            }

            _workspace ??= MSBuildWorkspace.Create();

            _logger.LogInformation("Loading solution: {SolutionPath}", solutionPath);
            var solution = await _workspace.OpenSolutionAsync(solutionPath);

            _cache.Set(cacheKey, solution, TimeSpan.FromMinutes(5));

            return solution;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Compilation?> GetCompilationAsync(Project project)
    {
        var cacheKey = $"compilation:{project.FilePath}";

        if (_cache.TryGetValue<Compilation>(cacheKey, out var cachedCompilation) && cachedCompilation is not null)
        {
            return cachedCompilation;
        }

        var compilation = await project.GetCompilationAsync();
        if (compilation is not null)
        {
            _cache.Set(cacheKey, compilation, TimeSpan.FromMinutes(5));
        }

        return compilation;
    }

    public async Task<IEnumerable<HandlerAnalysisResult>> AnalyzeHandlersAsync(string workspacePath)
    {
        var solutionPath = Path.Combine(workspacePath, "B2X.slnx");
        if (!File.Exists(solutionPath))
        {
            throw new FileNotFoundException("Solution file not found", solutionPath);
        }

        var solution = await GetSolutionAsync(solutionPath);
        var results = new List<HandlerAnalysisResult>();

        foreach (var project in solution.Projects)
        {
            var compilation = await GetCompilationAsync(project);
            if (compilation is null)
                continue;

            var handlers = FindHandlers(compilation);
            var commands = FindCommands(compilation).ToHashSet(SymbolEqualityComparer.Default);
            var queries = FindQueries(compilation).ToHashSet(SymbolEqualityComparer.Default);
            var events = FindEvents(compilation).ToHashSet(SymbolEqualityComparer.Default);

            foreach (var handler in handlers)
            {
                var pattern = DeterminePattern(handler, commands, queries, events);
                results.Add(new HandlerAnalysisResult
                {
                    HandlerName = handler.Name,
                    Pattern = pattern,
                    FilePath = handler.Locations.FirstOrDefault()?.GetLineSpan().Path ?? "Unknown"
                });
            }
        }

        return results;
    }

    private string DeterminePattern(INamedTypeSymbol handler, HashSet<INamedTypeSymbol> commands, HashSet<INamedTypeSymbol> queries, HashSet<INamedTypeSymbol> events)
    {
        // Check handler interfaces
        foreach (var interfaceType in handler.AllInterfaces)
        {
            var genericType = interfaceType as INamedTypeSymbol;
            if (genericType?.TypeArguments.Length == 1)
            {
                var messageType = genericType.TypeArguments[0] as INamedTypeSymbol;
                if (messageType is not null)
                {
                    if (commands.Contains(messageType))
                        return "Command Handler";
                    if (queries.Contains(messageType))
                        return "Query Handler";
                    if (events.Contains(messageType))
                        return "Event Handler";
                }
            }
        }

        return "Unknown Pattern";
    }

    public IEnumerable<INamedTypeSymbol> FindHandlers(Compilation compilation)
    {
        return GetAllTypes(compilation.GlobalNamespace)
            .Where(type => type.TypeKind == TypeKind.Class &&
                          type.AllInterfaces.Any(i =>
                              i.Name.Contains("Handler") ||
                              i.Name.Contains("ICommandHandler") ||
                              i.Name.Contains("IQueryHandler") ||
                              i.Name.Contains("IEventHandler")));
    }

    public IEnumerable<INamedTypeSymbol> FindCommands(Compilation compilation)
    {
        return GetAllTypes(compilation.GlobalNamespace)
            .Where(type => type.Name.EndsWith("Command") && type.TypeKind == TypeKind.Class);
    }

    public IEnumerable<INamedTypeSymbol> FindQueries(Compilation compilation)
    {
        return GetAllTypes(compilation.GlobalNamespace)
            .Where(type => type.Name.EndsWith("Query") && type.TypeKind == TypeKind.Class);
    }

    public IEnumerable<INamedTypeSymbol> FindEvents(Compilation compilation)
    {
        return GetAllTypes(compilation.GlobalNamespace)
            .Where(type => type.Name.EndsWith("Event") && type.TypeKind == TypeKind.Class);
    }

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            yield return type;
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var type in GetAllTypes(childNamespace))
            {
                yield return type;
            }
        }
    }

    public void Dispose()
    {
        _workspace?.Dispose();
    }
}
