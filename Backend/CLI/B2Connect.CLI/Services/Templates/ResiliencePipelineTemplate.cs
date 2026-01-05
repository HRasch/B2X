using System.Text;

namespace B2Connect.CLI.Services;

public class ResiliencePipelineTemplate : ITemplateProvider
{
    public Template GenerateTemplate(string name, bool tenantAware = false)
    {
        var pipelineName = $"{name}ResiliencePipeline";

        var content = new StringBuilder();
        content.AppendLine("using Polly;");
        content.AppendLine("using Polly.CircuitBreaker;");
        content.AppendLine("using Polly.Retry;");
        content.AppendLine("using Polly.Timeout;");
        content.AppendLine("using Microsoft.Extensions.Logging;");
        content.AppendLine("using Microsoft.Extensions.Configuration;");
        content.AppendLine();
        content.AppendLine($"namespace B2Connect.{GetNamespaceSuffix(name)}.Infrastructure.Resilience;");
        content.AppendLine();
        content.AppendLine($"// Resilience pipeline with circuit breaker, retry, and timeout policies");
        content.AppendLine($"public class {pipelineName}");
        content.AppendLine("{");
        content.AppendLine("    private readonly ResiliencePipeline _pipeline;");
        content.AppendLine("    private readonly ILogger<{pipelineName}> _logger;");
        content.AppendLine("    private readonly CircuitBreakerStateProvider _circuitBreakerState;");
        content.AppendLine();
        content.AppendLine($"    public {pipelineName}(");
        content.AppendLine("        ILogger<{pipelineName}> logger,");
        content.AppendLine("        IConfiguration configuration)");
        content.AppendLine("    {");
        content.AppendLine("        _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
        content.AppendLine("        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));");
        content.AppendLine();
        content.AppendLine("        // Load configuration with defaults");
        content.AppendLine("        var retryCount = config.GetValue(\"Resilience:RetryCount\", 3);");
        content.AppendLine("        var retryDelayMs = config.GetValue(\"Resilience:RetryDelayMs\", 1000);");
        content.AppendLine("        var circuitBreakerThreshold = config.GetValue(\"Resilience:CircuitBreakerThreshold\", 5);");
        content.AppendLine("        var circuitBreakerDurationSec = config.GetValue(\"Resilience:CircuitBreakerDurationSec\", 60);");
        content.AppendLine("        var timeoutSec = config.GetValue(\"Resilience:TimeoutSec\", 30);");
        content.AppendLine();
        content.AppendLine("        // Circuit breaker state provider for monitoring");
        content.AppendLine("        _circuitBreakerState = new CircuitBreakerStateProvider();");
        content.AppendLine();
        content.AppendLine("        // Build resilience pipeline");
        content.AppendLine("        _pipeline = new ResiliencePipelineBuilder()");
        content.AppendLine("            .AddRetry(new RetryStrategyOptions");
        content.AppendLine("            {");
        content.AppendLine("                ShouldHandle = new PredicateBuilder()");
        content.AppendLine("                    .Handle<HttpRequestException>()");
        content.AppendLine("                    .Handle<TimeoutException>()");
        content.AppendLine("                    .Handle<OperationCanceledException>()");
        content.AppendLine("                    .Handle<Exception>(ex =>");
        content.AppendLine("                        ex is not ValidationException &&");
        content.AppendLine("                        ex is not ArgumentException),");
        content.AppendLine("                MaxRetryAttempts = retryCount,");
        content.AppendLine("                Delay = TimeSpan.FromMilliseconds(retryDelayMs),");
        content.AppendLine("                BackoffType = DelayBackoffType.Exponential,");
        content.AppendLine("                OnRetry = args =>");
        content.AppendLine("                {");
        content.AppendLine("                    _logger.LogWarning(");
        content.AppendLine("                        \"Retry attempt {{Attempt}} for operation {{OperationKey}} after {{Duration}}. Exception: {{Exception}}\",");
        content.AppendLine("                        args.AttemptNumber,");
        content.AppendLine("                        args.OperationKey,");
        content.AppendLine("                        args.Duration,");
        content.AppendLine("                        args.Outcome.Exception?.Message);");
        content.AppendLine("                    return ValueTask.CompletedTask;");
        content.AppendLine("                }");
        content.AppendLine("            })");
        content.AppendLine("            .AddCircuitBreaker(new CircuitBreakerStrategyOptions");
        content.AppendLine("            {");
        content.AppendLine("                FailureRatio = 0.5,");
        content.AppendLine("                MinimumThroughput = circuitBreakerThreshold,");
        content.AppendLine("                SamplingDuration = TimeSpan.FromSeconds(30),");
        content.AppendLine("                BreakDuration = TimeSpan.FromSeconds(circuitBreakerDurationSec),");
        content.AppendLine("                ShouldHandle = new PredicateBuilder()");
        content.AppendLine("                    .Handle<HttpRequestException>()");
        content.AppendLine("                    .Handle<TimeoutException>()");
        content.AppendLine("                    .Handle<Exception>(ex =>");
        content.AppendLine("                        ex is not ValidationException &&");
        content.AppendLine("                        ex is not ArgumentException),");
        content.AppendLine("                OnOpened = args =>");
        content.AppendLine("                {");
        content.AppendLine("                    _logger.LogError(");
        content.AppendLine("                        \"Circuit breaker opened for {{OperationKey}} after {{FailureCount}} failures\",");
        content.AppendLine("                        args.OperationKey,");
        content.AppendLine("                        args.FailureCount);");
        content.AppendLine("                    return ValueTask.CompletedTask;");
        content.AppendLine("                },");
        content.AppendLine("                OnClosed = args =>");
        content.AppendLine("                {");
        content.AppendLine("                    _logger.LogInformation(");
        content.AppendLine("                        \"Circuit breaker closed for {{OperationKey}}\",");
        content.AppendLine("                        args.OperationKey);");
        content.AppendLine("                    return ValueTask.CompletedTask;");
        content.AppendLine("                },");
        content.AppendLine("                OnHalfOpened = args =>");
        content.AppendLine("                {");
        content.AppendLine("                    _logger.LogInformation(");
        content.AppendLine("                        \"Circuit breaker half-opened for {{OperationKey}}\",");
        content.AppendLine("                        args.OperationKey);");
        content.AppendLine("                    return ValueTask.CompletedTask;");
        content.AppendLine("                },");
        content.AppendLine("                StateProvider = _circuitBreakerState");
        content.AppendLine("            })");
        content.AppendLine("            .AddTimeout(new TimeoutStrategyOptions");
        content.AppendLine("            {");
        content.AppendLine("                Timeout = TimeSpan.FromSeconds(timeoutSec),");
        content.AppendLine("                OnTimeout = args =>");
        content.AppendLine("                {");
        content.AppendLine("                    _logger.LogWarning(");
        content.AppendLine("                        \"Operation {{OperationKey}} timed out after {{Timeout}}\",");
        content.AppendLine("                        args.OperationKey,");
        content.AppendLine("                        args.Timeout);");
        content.AppendLine("                    return ValueTask.CompletedTask;");
        content.AppendLine("                }");
        content.AppendLine("            })");
        content.AppendLine("            .Build();");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    // Execute operation with resilience");
        content.AppendLine("    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        await _pipeline.ExecuteAsync(");
        content.AppendLine("            async ctx => await operation(ctx.CancellationToken),");
        content.AppendLine("            cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    // Execute operation with result");
        content.AppendLine("    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)");
        content.AppendLine("    {");
        content.AppendLine("        return await _pipeline.ExecuteAsync(");
        content.AppendLine("            async ctx => await operation(ctx.CancellationToken),");
        content.AppendLine("            cancellationToken);");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    // Get circuit breaker state for monitoring");
        content.AppendLine("    public CircuitBreakerState GetCircuitBreakerState()");
        content.AppendLine("    {");
        content.AppendLine("        return _circuitBreakerState.CircuitState;");
        content.AppendLine("    }");
        content.AppendLine();
        content.AppendLine("    // Get resilience metrics");
        content.AppendLine("    public ResilienceMetrics GetMetrics()");
        content.AppendLine("    {");
        content.AppendLine("        return new ResilienceMetrics");
        content.AppendLine("        {");
        content.AppendLine("            CircuitBreakerState = _circuitBreakerState.CircuitState,");
        content.AppendLine("            // Add more metrics as needed");
        content.AppendLine("        };");
        content.AppendLine("    }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine("// Resilience metrics for monitoring");
        content.AppendLine("public class ResilienceMetrics");
        content.AppendLine("{");
        content.AppendLine("    public CircuitBreakerState CircuitBreakerState { get; set; }");
        content.AppendLine("    // Add additional metrics properties as needed");
        content.AppendLine("    // public int TotalRequests { get; set; }");
        content.AppendLine("    // public int FailedRequests { get; set; }");
        content.AppendLine("    // public TimeSpan AverageResponseTime { get; set; }");
        content.AppendLine("}");
        content.AppendLine();
        content.AppendLine($"// Extension methods for easy registration");
        content.AppendLine($"public static class {pipelineName}Extensions");
        content.AppendLine("{");
        content.AppendLine($"    public static IServiceCollection Add{pipelineName}(");
        content.AppendLine("        this IServiceCollection services,");
        content.AppendLine("        IConfiguration configuration)");
        content.AppendLine("    {");
        content.AppendLine($"        services.AddSingleton<{pipelineName}>();");
        content.AppendLine("        services.AddSingleton<ResiliencePipeline>(sp =>");
        content.AppendLine("        {");
        content.AppendLine($"            var pipeline = sp.GetRequiredService<{pipelineName}>();");
        content.AppendLine("            // Return the internal pipeline - this is a bit of a hack but works");
        content.AppendLine("            return (ResiliencePipeline)typeof({pipelineName})");
        content.AppendLine("                .GetField(\"_pipeline\", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)");
        content.AppendLine("                ?.GetValue(pipeline) ?? throw new InvalidOperationException(\"Cannot access internal pipeline\");");
        content.AppendLine("        });");
        content.AppendLine("        return services;");
        content.AppendLine("    }");
        content.AppendLine("}");

        var warnings = new List<string>();
        warnings.Add("Configure resilience settings in appsettings.json under 'Resilience' section");
        warnings.Add("Adjust retry count, delays, and circuit breaker thresholds based on your service requirements");
        warnings.Add("Consider adding bulkhead isolation for high-throughput scenarios");
        warnings.Add("Add health checks to monitor circuit breaker state");
        warnings.Add("Test failure scenarios to ensure circuit breaker activates correctly");
        warnings.Add("Consider different policies for different types of operations (read vs write)");

        return new Template
        {
            FileName = $"{pipelineName}.cs",
            Content = content.ToString(),
            Warnings = warnings
        };
    }

    private string GetNamespaceSuffix(string name)
    {
        if (name.Contains("Erp") || name.Contains("Enventa")) return "ERP";
        if (name.Contains("Catalog")) return "Catalog";
        if (name.Contains("Cms")) return "CMS";
        if (name.Contains("Identity")) return "Identity";
        if (name.Contains("Search")) return "Search";
        return "Shared";
    }
}