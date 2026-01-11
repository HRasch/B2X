using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace B2X.Tools.RoslynMCP.Services;

/// <summary>
/// Service for loading and caching Roslyn workspaces and solutions.
/// </summary>
public sealed class CodeAnalysisService : IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CodeAnalysisService> _logger;
    private MSBuildWorkspace? _workspace;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public CodeAnalysisService(IMemoryCache cache, ILogger<CodeAnalysisService> logger)
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
            // Double-check after acquiring lock
            if (_cache.TryGetValue<Solution>(cacheKey, out cachedSolution) && cachedSolution is not null)
            {
                return cachedSolution;
            }

            _workspace ??= MSBuildWorkspace.Create();

            _logger.LogInformation("Loading solution: {SolutionPath}", solutionPath);
            var solution = await _workspace.OpenSolutionAsync(solutionPath);

            // Cache for 5 minutes
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

        if (_cache.TryGetValue<Compilation>(cacheKey, out var cachedCompilation))
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

    public void InvalidateCache()
    {
        if (_cache is MemoryCache memoryCache)
        {
            memoryCache.Compact(1.0);
        }
    }

    public void Dispose()
    {
        _workspace?.Dispose();
        _lock.Dispose();
    }
}
