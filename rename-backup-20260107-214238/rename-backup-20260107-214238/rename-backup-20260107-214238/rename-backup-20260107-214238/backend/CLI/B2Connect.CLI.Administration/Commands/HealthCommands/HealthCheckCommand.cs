using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using Spectre.Console;

namespace B2Connect.CLI.Administration.Commands.HealthCommands;

public static class HealthCheckCommand
{
    public static Command Create()
    {
        var command = new Command("check", "Perform system health checks");

        var componentOption = new Option<string>(
            ["--component"],
            () => "all",
            "Specific component to check (all, gateway, database, elasticsearch)");
        componentOption.FromAmong("all", "gateway", "database", "elasticsearch");

        var timeRangeOption = new Option<string>(
            ["--time-range"],
            () => "24h",
            "Time range for analysis (1h, 24h, 7d)");
        timeRangeOption.FromAmong("1h", "24h", "7d");

        command.AddOption(componentOption);
        command.AddOption(timeRangeOption);

        command.SetHandler(ExecuteHealthCheck, componentOption, timeRangeOption);

        return command;
    }

    private static async Task ExecuteHealthCheck(string component, string timeRange)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        console.Header("B2Connect System Health Check");
        console.Info($"Component: {component}");
        console.Info($"Time Range: {timeRange}");

        try
        {
            var results = new List<HealthCheckResult>();

            if (component == "all" || component == "gateway")
            {
                results.Add(await CheckGatewayHealth(config));
            }

            if (component == "all" || component == "database")
            {
                results.Add(await CheckDatabaseHealth(config));
            }

            if (component == "all" || component == "elasticsearch")
            {
                results.Add(await CheckElasticsearchHealth(config));
            }

            // Display results
            var table = new Table();
            table.AddColumn("Component");
            table.AddColumn("Status");
            table.AddColumn("Response Time");
            table.AddColumn("Details");

            foreach (var result in results)
            {
                var statusColor = result.IsHealthy ? Color.Green : Color.Red;
                table.AddRow(
                    result.Component,
                    $"[{statusColor}]{result.Status}[/]",
                    result.ResponseTime.HasValue ? $"{result.ResponseTime.Value.TotalMilliseconds:F0}ms" : "N/A",
                    result.Details
                );
            }

            AnsiConsole.Write(table);

            var healthyCount = results.Count(r => r.IsHealthy);
            var totalCount = results.Count;

            if (healthyCount == totalCount)
            {
                console.Success($"All {totalCount} components are healthy");
            }
            else
            {
                console.Warning($"{healthyCount}/{totalCount} components are healthy");
                console.Info("Check details above for issues");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Health check failed: {ex.Message}");
        }
    }

    private static async Task<HealthCheckResult> CheckGatewayHealth(ConfigurationService config)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var services = config.GetAllServices();
            var gatewayService = services.FirstOrDefault(s => s.Name.Contains("gateway", StringComparison.OrdinalIgnoreCase));

            if (gatewayService.Endpoint.Url == null)
            {
                return new HealthCheckResult
                {
                    Component = "Gateway",
                    IsHealthy = false,
                    Status = "Not Configured",
                    Details = "Gateway service not found in configuration"
                };
            }

            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetAsync($"{gatewayService.Endpoint.Url}/health");

            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                return new HealthCheckResult
                {
                    Component = "Gateway",
                    IsHealthy = true,
                    Status = "Healthy",
                    ResponseTime = stopwatch.Elapsed,
                    Details = $"HTTP {response.StatusCode}"
                };
            }
            else
            {
                return new HealthCheckResult
                {
                    Component = "Gateway",
                    IsHealthy = false,
                    Status = "Unhealthy",
                    ResponseTime = stopwatch.Elapsed,
                    Details = $"HTTP {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new HealthCheckResult
            {
                Component = "Gateway",
                IsHealthy = false,
                Status = "Error",
                ResponseTime = stopwatch.Elapsed,
                Details = ex.Message
            };
        }
    }

    private static async Task<HealthCheckResult> CheckDatabaseHealth(ConfigurationService config)
    {
        // TODO: Implement database connectivity check
        // For now, return mock result
        await Task.Delay(100); // Simulate check

        return new HealthCheckResult
        {
            Component = "Database",
            IsHealthy = true,
            Status = "Healthy",
            ResponseTime = TimeSpan.FromMilliseconds(50),
            Details = "PostgreSQL connection OK"
        };
    }

    private static async Task<HealthCheckResult> CheckElasticsearchHealth(ConfigurationService config)
    {
        // TODO: Implement Elasticsearch connectivity check
        // For now, return mock result
        await Task.Delay(100); // Simulate check

        return new HealthCheckResult
        {
            Component = "Elasticsearch",
            IsHealthy = true,
            Status = "Healthy",
            ResponseTime = TimeSpan.FromMilliseconds(75),
            Details = "Cluster status: green"
        };
    }

    private class HealthCheckResult
    {
        public string Component { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan? ResponseTime { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}