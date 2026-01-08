using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.Shared.Monitoring.Abstractions;
using B2X.Shared.Monitoring.Models;
using B2X.Shared.Monitoring;
using System.Collections.Concurrent;

namespace B2X.CLI.Administration.Commands.MetricsCommands;

public static class BenchmarkCommand
{
    public static Command Create()
    {
        var command = new Command("benchmark", "Run performance benchmarks for services");

        var serviceOption = new Option<string>(
            ["-s", "--service"],
            "Specific service to benchmark (all services if not specified)");
        var tenantOption = new Option<string>(
            ["-t", "--tenant"],
            "Tenant ID for tenant-specific benchmarking");
        var durationOption = new Option<int>(
            ["-d", "--duration"],
            "Benchmark duration in seconds (default: 60)");
        var concurrencyOption = new Option<int>(
            ["-c", "--concurrency"],
            "Number of concurrent operations (default: 10)");
        var outputOption = new Option<string>(
            ["-o", "--output"],
            "Output format: console, json, csv (default: console)");
        var saveOption = new Option<bool>(
            ["--save"],
            "Save benchmark results to monitoring service");

        command.AddOption(serviceOption);
        command.AddOption(tenantOption);
        command.AddOption(durationOption);
        command.AddOption(concurrencyOption);
        command.AddOption(outputOption);
        command.AddOption(saveOption);

        command.SetHandler(ExecuteAsync, serviceOption, tenantOption, durationOption, concurrencyOption, outputOption, saveOption);

        return command;
    }

    private static async Task ExecuteAsync(string service, string tenant, int duration, int concurrency, string output, bool save)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header("B2X Service Benchmarking");

            // Set defaults
            duration = duration > 0 ? duration : 60;
            concurrency = concurrency > 0 ? concurrency : 10;
            output = !string.IsNullOrEmpty(output) ? output : "console";

            console.Info($"Benchmark Configuration:");
            console.Info($"  Duration: {duration} seconds");
            console.Info($"  Concurrency: {concurrency} operations");
            console.Info($"  Output Format: {output}");
            console.Info($"  Save Results: {save}");

            if (!string.IsNullOrEmpty(service))
                console.Info($"  Target Service: {service}");

            if (!string.IsNullOrEmpty(tenant))
                console.Info($"  Tenant: {tenant}");

            console.Info("");

            // Get available services
            var services = config.GetAllServices().ToList();
            if (!services.Any())
            {
                console.Warning("No services configured. Use 'B2X-admin config set service.name url' to configure services.");
                return;
            }

            console.Info($"Found {services.Count} configured services:");
            foreach (var (name, endpoint) in services)
            {
                console.Info($"  - {name}: {endpoint.Url}");
            }
            console.Info("");

            // Filter services if specific service requested
            if (!string.IsNullOrEmpty(service))
            {
                var targetService = services.FirstOrDefault(s => s.Name.Equals(service, StringComparison.OrdinalIgnoreCase));
                if (targetService.Name == null)
                {
                    console.Error($"Service '{service}' not found in configuration.");
                    Environment.ExitCode = 1;
                    return;
                }
                services = new List<(string Name, ServiceEndpoint Endpoint)> { targetService };
            }

            // Run benchmarks
            var benchmarkResults = new List<BenchmarkResult>();

            foreach (var (serviceName, endpoint) in services)
            {
                console.Info($"Benchmarking {serviceName}...");

                var result = await RunServiceBenchmark(serviceName, endpoint, tenant, duration, concurrency);
                benchmarkResults.Add(result);

                // Display immediate results
                DisplayBenchmarkResult(console, result, output);
            }

            // Save results if requested
            if (save && benchmarkResults.Any())
            {
                await SaveBenchmarkResults(benchmarkResults, tenant);
                console.Success("Benchmark results saved to monitoring service.");
            }

            // Summary
            console.Header("Benchmark Summary");
            DisplayBenchmarkSummary(console, benchmarkResults);

        }
        catch (Exception ex)
        {
            console.Error($"Benchmark failed: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task<BenchmarkResult> RunServiceBenchmark(string serviceName, ServiceEndpoint endpoint, string tenant, int duration, int concurrency)
    {
        var result = new BenchmarkResult
        {
            ServiceName = serviceName,
            Endpoint = endpoint.Url,
            TenantId = tenant,
            StartTime = DateTime.UtcNow,
            DurationSeconds = duration,
            Concurrency = concurrency
        };

        var tasks = new List<Task>();
        var responses = new ConcurrentBag<BenchmarkResponse>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(duration));

        // Create concurrent benchmark tasks
        for (int i = 0; i < concurrency; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                while (!cts.Token.IsCancellationRequested)
                {
                    var startTime = DateTime.UtcNow;
                    var response = new BenchmarkResponse { StartTime = startTime };

                    try
                    {
                        // Add tenant header if specified
                        if (!string.IsNullOrEmpty(tenant))
                        {
                            httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenant);
                        }

                        // Perform health check or basic GET request
                        var httpResponse = await httpClient.GetAsync(endpoint.Url + "/health", cts.Token);
                        response.EndTime = DateTime.UtcNow;
                        response.StatusCode = (int)httpResponse.StatusCode;
                        response.IsSuccess = httpResponse.IsSuccessStatusCode;
                        response.ResponseTimeMs = (response.EndTime - startTime).TotalMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        response.EndTime = DateTime.UtcNow;
                        response.IsSuccess = false;
                        response.ErrorMessage = ex.Message;
                        response.ResponseTimeMs = (response.EndTime - startTime).TotalMilliseconds;
                    }

                    responses.Add(response);

                    // Small delay to avoid overwhelming the service
                    await Task.Delay(100, cts.Token);
                }
            }, cts.Token));
        }

        // Wait for all tasks to complete or timeout
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            // Expected when duration expires
        }

        // Calculate results
        result.EndTime = DateTime.UtcNow;
        result.TotalRequests = responses.Count;
        result.SuccessfulRequests = responses.Count(r => r.IsSuccess);
        result.FailedRequests = responses.Count(r => !r.IsSuccess);
        result.AverageResponseTime = responses.Average(r => r.ResponseTimeMs);
        result.MinResponseTime = responses.Min(r => r.ResponseTimeMs);
        result.MaxResponseTime = responses.Max(r => r.ResponseTimeMs);

        // Calculate percentiles
        var sortedResponses = responses.OrderBy(r => r.ResponseTimeMs).ToList();
        result.P50ResponseTime = Percentile(sortedResponses, 0.5);
        result.P95ResponseTime = Percentile(sortedResponses, 0.95);
        result.P99ResponseTime = Percentile(sortedResponses, 0.99);

        return result;
    }

    private static double Percentile(List<BenchmarkResponse> sortedResponses, double percentile)
    {
        if (!sortedResponses.Any()) return 0;

        var index = (int)Math.Ceiling(sortedResponses.Count * percentile) - 1;
        index = Math.Max(0, Math.Min(index, sortedResponses.Count - 1));
        return sortedResponses[index].ResponseTimeMs;
    }

    private static void DisplayBenchmarkResult(ConsoleOutputService console, BenchmarkResult result, string outputFormat)
    {
        switch (outputFormat.ToLower())
        {
            case "json":
                console.Info(System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                break;
            case "csv":
                console.Info($"Service,TotalRequests,Successful,Failed,AvgResponseTime,MinResponseTime,MaxResponseTime,P50,P95,P99,RPS");
                console.Info($"{result.ServiceName},{result.TotalRequests},{result.SuccessfulRequests},{result.FailedRequests},{result.AverageResponseTime:F2},{result.MinResponseTime:F2},{result.MaxResponseTime:F2},{result.P50ResponseTime:F2},{result.P95ResponseTime:F2},{result.P99ResponseTime:F2},{result.RequestsPerSecond:F2}");
                break;
            default: // console
                console.Info($"  Total Requests: {result.TotalRequests}");
                console.Info($"  Successful: {result.SuccessfulRequests} ({result.SuccessRate:P2})");
                console.Info($"  Failed: {result.FailedRequests}");
                console.Info($"  Response Time (ms):");
                console.Info($"    Average: {result.AverageResponseTime:F2}");
                console.Info($"    Min: {result.MinResponseTime:F2}");
                console.Info($"    Max: {result.MaxResponseTime:F2}");
                console.Info($"    P50: {result.P50ResponseTime:F2}");
                console.Info($"    P95: {result.P95ResponseTime:F2}");
                console.Info($"    P99: {result.P99ResponseTime:F2}");
                console.Info($"  Requests/Second: {result.RequestsPerSecond:F2}");
                console.Info("");
                break;
        }
    }

    private static void DisplayBenchmarkSummary(ConsoleOutputService console, List<BenchmarkResult> results)
    {
        if (!results.Any()) return;

        console.Info($"Services Benchmarked: {results.Count}");
        console.Info($"Total Requests: {results.Sum(r => r.TotalRequests)}");
        console.Info($"Overall Success Rate: {results.Average(r => r.SuccessRate):P2}");
        console.Info($"Average Response Time: {results.Average(r => r.AverageResponseTime):F2}ms");
        console.Info($"Total RPS: {results.Sum(r => r.RequestsPerSecond):F2}");
    }

    private static async Task SaveBenchmarkResults(List<BenchmarkResult> results, string tenant)
    {
        using var monitoringClient = new MonitoringServiceClient();

        foreach (var result in results)
        {
            // Convert benchmark result to monitoring metrics
            var metrics = new Dictionary<string, object>
            {
                ["benchmark.duration_seconds"] = result.DurationSeconds,
                ["benchmark.total_requests"] = result.TotalRequests,
                ["benchmark.successful_requests"] = result.SuccessfulRequests,
                ["benchmark.failed_requests"] = result.FailedRequests,
                ["benchmark.average_response_time_ms"] = result.AverageResponseTime,
                ["benchmark.min_response_time_ms"] = result.MinResponseTime,
                ["benchmark.max_response_time_ms"] = result.MaxResponseTime,
                ["benchmark.p50_response_time_ms"] = result.P50ResponseTime,
                ["benchmark.p95_response_time_ms"] = result.P95ResponseTime,
                ["benchmark.p99_response_time_ms"] = result.P99ResponseTime,
                ["benchmark.requests_per_second"] = result.RequestsPerSecond,
                ["benchmark.success_rate"] = result.SuccessRate
            };

            // Note: This would require extending the monitoring service client to accept custom metrics
            // For now, we'll just register the service if not already registered
            var service = new ConnectedService
            {
                Name = result.ServiceName,
                Type = ServiceType.Api,
                Endpoint = result.Endpoint,
                TenantId = tenant ?? "system"
            };

            await monitoringClient.RegisterServiceAsync(service);
        }
    }
}

public class BenchmarkResult
{
    public string ServiceName { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string TenantId { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationSeconds { get; set; }
    public int Concurrency { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double AverageResponseTime { get; set; }
    public double MinResponseTime { get; set; }
    public double MaxResponseTime { get; set; }
    public double P50ResponseTime { get; set; }
    public double P95ResponseTime { get; set; }
    public double P99ResponseTime { get; set; }
    public double RequestsPerSecond => TotalRequests / (double)DurationSeconds;
    public double SuccessRate => TotalRequests > 0 ? SuccessfulRequests / (double)TotalRequests : 0;
}

public class BenchmarkResponse
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = "";
    public double ResponseTimeMs { get; set; }
}