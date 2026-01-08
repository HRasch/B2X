using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2X.CLI.Operations.Commands.HealthCommands;

public static class CheckHealthCommand
{
    public static Command Create()
    {
        var command = new Command("check", "Perform comprehensive health check of all platform services");

        var detailedOption = new Option<bool>(
            ["-d", "--detailed"], "Show detailed health information");

        var timeoutOption = new Option<int>(
            ["-t", "--timeout"], getDefaultValue: () => 30, "Timeout in seconds for health checks");

        command.AddOption(detailedOption);
        command.AddOption(timeoutOption);
        command.SetHandler(ExecuteAsync, detailedOption, timeoutOption);

        return command;
    }

    private static async Task ExecuteAsync(bool detailed, int timeout)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header("Platform Health Check");

            var services = config.GetAllServices().ToList();
            var statuses = new List<ServiceHealthStatus>();

            console.Info($"Checking {services.Count} services (timeout: {timeout}s)...");

            var tasks = services.Select(async service =>
            {
                var (serviceName, endpoint) = service;
                var status = await CheckServiceHealth(serviceName, endpoint, timeout, detailed);
                return status;
            });

            var results = await Task.WhenAll(tasks);
            statuses.AddRange(results);

            // Display results
            DisplayResults(console, statuses, detailed);

            // Exit with appropriate code
            var unhealthyCount = statuses.Count(s => !s.IsHealthy);
            if (unhealthyCount > 0)
            {
                console.Error($"\n❌ {unhealthyCount} service(s) unhealthy");
                Environment.Exit(1);
            }
            else
            {
                console.Success($"\n✅ All {statuses.Count} services healthy");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Health check failed: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static async Task<ServiceHealthStatus> CheckServiceHealth(
        string serviceName,
        ServiceEndpoint endpoint,
        int timeout,
        bool detailed)
    {
        var status = new ServiceHealthStatus
        {
            Service = serviceName,
            Url = endpoint.Url,
            Description = endpoint.Description
        };

        using var client = new CliHttpClient(endpoint.Url);

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await client.GetAsync<object>("/health");
            stopwatch.Stop();

            status.IsHealthy = response.Success;
            status.ResponseTime = stopwatch.ElapsedMilliseconds;

            if (response.Success)
            {
                status.Status = "✓ Healthy";
                status.Details = $"Response time: {status.ResponseTime}ms";
            }
            else
            {
                status.Status = "✗ Unhealthy";
                status.Details = $"Error: {response.Error ?? "Unknown error"}";
            }

            if (detailed && response.Data != null)
            {
                status.Details += $"\nData: {System.Text.Json.JsonSerializer.Serialize(response.Data)}";
            }
        }
        catch (TaskCanceledException)
        {
            status.IsHealthy = false;
            status.Status = "✗ Timeout";
            status.Details = $"Timeout after {timeout}s";
        }
        catch (Exception ex)
        {
            status.IsHealthy = false;
            status.Status = "✗ Error";
            status.Details = ex.Message;
        }

        return status;
    }

    private static void DisplayResults(ConsoleOutputService console, List<ServiceHealthStatus> statuses, bool detailed)
    {
        var table = new Table();
        table.AddColumn("Service");
        table.AddColumn("Status");
        table.AddColumn("URL");

        if (detailed)
        {
            table.AddColumn("Response Time");
            table.AddColumn("Details");
        }

        foreach (var status in statuses)
        {
            var statusColor = status.IsHealthy ? "[green]" : "[red]";
            var row = new List<string>
            {
                status.Service,
                $"{statusColor}{status.Status}[/]",
                status.Url
            };

            if (detailed)
            {
                row.Add(status.ResponseTime.HasValue ? $"{status.ResponseTime}ms" : "-");
                row.Add(status.Details ?? "-");
            }

            table.AddRow(row.ToArray());
        }

        console.Info("Health Check Results:");
        AnsiConsole.Write(table);
    }

    private class ServiceHealthStatus
    {
        public string Service { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = "Unknown";
        public long? ResponseTime { get; set; }
        public int? StatusCode { get; set; }
        public string? Details { get; set; }
    }
}