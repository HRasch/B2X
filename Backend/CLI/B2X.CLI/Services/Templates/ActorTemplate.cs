using System.Text;
using System.Threading.Channels;

namespace B2X.CLI.Services;

public class ActorTemplate : ITemplateProvider
{
    public Template GenerateTemplate(string name, bool tenantAware = false)
    {
        var actorName = $"{name}Actor";
        var operationName = $"{name}Operation";
        var resultName = $"{name}Result";

        var content = new StringBuilder();
        content.AppendLine("using System.Threading.Channels;");
        content.AppendLine("using Microsoft.Extensions.Logging;");
        content.AppendLine("using Microsoft.Extensions.Hosting;");
        content.AppendLine("using Polly;");
        content.AppendLine("using Polly.Retry;");
        content.AppendLine();
        content.AppendLine($"namespace B2X.{GetNamespaceSuffix(name)}.Infrastructure.Actor;");
        content.AppendLine();
        content.AppendLine($"// Base operation interface");
        content.AppendLine($"public interface I{name}Operation");
        content.AppendLine("{");
        content.AppendLine("    Task ExecuteAsync(CancellationToken cancellationToken = default);");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Operation result");
        content.AppendLine($"public class {resultName}<T>");
        content.AppendLine("{");
        content.AppendLine("    public bool Success { get; set; }");
        content.AppendLine("    public T? Data { get; set; }");
        content.AppendLine("    public string? Error { get; set; }");
        content.AppendLine("    public TimeSpan Duration { get; set; }");
        content.AppendLine();
        content.AppendLine($"    public static {resultName}<T> SuccessResult(T data, TimeSpan duration)");
        content.AppendLine("    {");
        content.AppendLine($"        return new {resultName}<T> {{ Success = true, Data = data, Duration = duration }};");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public static {resultName}<T> ErrorResult(string error, TimeSpan duration)");
        content.AppendLine("    {");
        content.AppendLine($"        return new {resultName}<T> {{ Success = false, Error = error, Duration = duration }};");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Base operation class");
        content.AppendLine($"public abstract class {operationName}<T> : I{name}Operation");
        content.AppendLine("{");
        content.AppendLine($"    private readonly TaskCompletionSource<{resultName}<T>> _completionSource = new();");
        content.AppendLine();
        content.AppendLine($"    public Task<{resultName}<T>> Result => _completionSource.Task;");
        content.AppendLine();
        content.AppendLine("    protected abstract Task<T> ExecuteCoreAsync(CancellationToken cancellationToken);");
        content.AppendLine();
        content.AppendLine("    public async Task ExecuteAsync(CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        var startTime = DateTimeOffset.UtcNow;");
        content.AppendLine("        try");
        content.AppendLine("        {");
        content.AppendLine("            var result = await ExecuteCoreAsync(cancellationToken);");
        content.AppendLine("            var duration = DateTimeOffset.UtcNow - startTime;");
        content.AppendLine($"            _completionSource.SetResult({resultName}<T>.SuccessResult(result, duration));");
        content.AppendLine("        }");
        content.AppendLine("        catch (Exception ex)");
        content.AppendLine("        {");
        content.AppendLine("            var duration = DateTimeOffset.UtcNow - startTime;");
        content.AppendLine($"            _completionSource.SetResult({resultName}<T>.ErrorResult(ex.Message, duration));");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Actor implementation with Channel-based message queue");
        content.AppendLine($"public class {actorName} : IHostedService, IDisposable");
        content.AppendLine("{");
        content.AppendLine("    private readonly Channel<I{name}Operation> _operationQueue;");
        content.AppendLine("    private readonly ILogger<{actorName}> _logger;");
        content.AppendLine("    private readonly ResiliencePipeline _resiliencePipeline;");
        content.AppendLine("    private readonly CancellationTokenSource _cts = new();");
        content.AppendLine("    private Task? _processingTask;");
        content.AppendLine("    private bool _disposed;");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine("    // Per-tenant actor instance");
            content.AppendLine("    private readonly Guid _tenantId;");
            content.AppendLine();
        }
        content.AppendLine($"    public {actorName}(");
        content.AppendLine("        ILogger<{actorName}> logger,");
        content.AppendLine("        ResiliencePipeline resiliencePipeline");
        if (tenantAware)
        {
            content.AppendLine("        , Guid tenantId");
        }
        content.AppendLine("    )");
        content.AppendLine("    {");
        content.AppendLine("        _operationQueue = Channel.CreateBounded<I{name}Operation>(");
        content.AppendLine("            new BoundedChannelOptions(100) // Buffer size");
        content.AppendLine("            {");
        content.AppendLine("                FullMode = BoundedChannelFullMode.Wait,");
        content.AppendLine("                SingleReader = true, // Single processing task");
        content.AppendLine("                SingleWriter = false");
        content.AppendLine("            });");
        content.AppendLine();
        content.AppendLine("        _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
        content.AppendLine("        _resiliencePipeline = resiliencePipeline ?? throw new ArgumentNullException(nameof(resiliencePipeline));");
        if (tenantAware)
        {
            content.AppendLine("        _tenantId = tenantId;");
        }
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    public async Task StartAsync(CancellationToken cancellationToken)");
        content.AppendLine("    {");
        content.AppendLine($"        _logger.LogInformation(\"Starting {actorName}" + (tenantAware ? " for tenant {{TenantId}}\"" : "\"") + (tenantAware ? ", _tenantId);" : ";"));
        content.AppendLine("        _processingTask = ProcessOperationsAsync(_cts.Token);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    public async Task StopAsync(CancellationToken cancellationToken)");
        content.AppendLine("    {");
        content.AppendLine($"        _logger.LogInformation(\"Stopping {actorName}\");");
        content.AppendLine("        _cts.Cancel();");
        content.AppendLine();
        content.AppendLine("        if (_processingTask != null)");
        content.AppendLine("        {");
        content.AppendLine("            try");
        content.AppendLine("            {");
        content.AppendLine("                await _processingTask.WaitAsync(cancellationToken);");
        content.AppendLine("            }");
        content.AppendLine("            catch (OperationCanceledException)");
        content.AppendLine("            {");
        content.AppendLine("                // Expected during shutdown");
        content.AppendLine("            }");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine($"    public async Task<{resultName}<T>> EnqueueAsync<T>({operationName}<T> operation)");
        content.AppendLine("    {");
        content.AppendLine("        if (_disposed)");
        content.AppendLine("        {");
        content.AppendLine($"            throw new ObjectDisposedException(nameof({actorName}));");
        content.AppendLine("        }");
        content.AppendLine();
        content.AppendLine("        await _operationQueue.Writer.WriteAsync(operation, _cts.Token);");
        content.AppendLine("        return await operation.Result;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    private async Task ProcessOperationsAsync(CancellationToken cancellationToken)");
        content.AppendLine("    {");
        content.AppendLine("        try");
        content.AppendLine("        {");
        content.AppendLine("            await foreach (var operation in _operationQueue.Reader.ReadAllAsync(cancellationToken))");
        content.AppendLine("            {");
        content.AppendLine("                await _resiliencePipeline.ExecuteAsync(");
        content.AppendLine("                    async ctx => await operation.ExecuteAsync(ctx.CancellationToken),");
        content.AppendLine("                    cancellationToken);");
        content.AppendLine("            }");
        content.AppendLine("        }");
        content.AppendLine("        catch (OperationCanceledException)");
        content.AppendLine("        {");
        content.AppendLine($"            _logger.LogInformation(\"{actorName} processing cancelled\");");
        content.AppendLine("        }");
        content.AppendLine("        catch (Exception ex)");
        content.AppendLine("        {");
        content.AppendLine($"            _logger.LogError(ex, \"Unexpected error in {actorName} processing loop\");");
        content.AppendLine("        }");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    public void Dispose()");
        content.AppendLine("    {");
        content.AppendLine("        if (_disposed) return;");
        content.AppendLine();
        content.AppendLine("        _disposed = true;");
        content.AppendLine("        _cts.Cancel();");
        content.AppendLine("        _cts.Dispose();");
        content.AppendLine("        _operationQueue.Writer.Complete();");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        if (tenantAware)
        {
            content.AppendLine($"// Actor pool for managing per-tenant actors");
            content.AppendLine($"public class {actorName}Pool");
            content.AppendLine("{");
            content.AppendLine($"    private readonly ConcurrentDictionary<Guid, {actorName}> _actors = new();");
            content.AppendLine("    private readonly ILoggerFactory _loggerFactory;");
            content.AppendLine("    private readonly ResiliencePipeline _resiliencePipeline;");
            content.AppendLine();
            content.AppendLine($"    public {actorName}Pool(");
            content.AppendLine("        ILoggerFactory loggerFactory,");
            content.AppendLine("        ResiliencePipeline resiliencePipeline)");
            content.AppendLine("    {");
            content.AppendLine("        _loggerFactory = loggerFactory;");
            content.AppendLine("        _resiliencePipeline = resiliencePipeline;");
            content.AppendLine("    }");
            content.AppendLine();
            content.AppendLine($"    public {actorName} GetOrCreateActor(Guid tenantId)");
            content.AppendLine("    {");
            content.AppendLine($"        return _actors.GetOrAdd(tenantId, id =>");
            content.AppendLine("        {");
            content.AppendLine($"            var logger = _loggerFactory.CreateLogger<{actorName}>();");
            content.AppendLine($"            return new {actorName}(logger, _resiliencePipeline, id);");
            content.AppendLine("        });");
            content.AppendLine("    }");
            content.AppendLine();
            content.AppendLine("    public void RemoveActor(Guid tenantId)");
            content.AppendLine("    {");
            content.AppendLine("        if (_actors.TryRemove(tenantId, out var actor))");
            content.AppendLine("        {");
            content.AppendLine("            actor.Dispose();");
            content.AppendLine("        }");
            content.AppendLine("    }");
            content.AppendLine("}");
        }

        var warnings = new List<string>();
        warnings.Add("Implement concrete operation classes inheriting from {operationName}<T>");
        warnings.Add("Ensure ResiliencePipeline is configured for retry and circuit breaker policies");
        warnings.Add("Register actor as IHostedService in DI container");
        if (tenantAware)
        {
            warnings.Add("Register {actorName}Pool as singleton in DI container");
            warnings.Add("Implement actor lifecycle management (creation/disposal)");
        }
        warnings.Add("Configure appropriate channel buffer size based on expected load");
        warnings.Add("Add health checks for actor queue depth and processing status");

        return new Template
        {
            FileName = $"{actorName}.cs",
            Content = content.ToString(),
            Warnings = warnings
        };
    }

    private string GetNamespaceSuffix(string name)
    {
        if (name.Contains("Erp") || name.Contains("Enventa")) return "ERP";
        if (name.Contains("Catalog")) return "Catalog";
        if (name.Contains("Cms")) return "CMS";
        return "Shared";
    }
}