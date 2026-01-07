using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2Connect.CLI.Operations.Commands.MonitoringCommands;

public static class MetricsCommand
{
    public static Command Create()
    {
        var command = new Command("metrics", "Display detailed service metrics");

        var serviceOption = new Option<string?>(
            ["-s", "--service"], "Specific service to show metrics for (default: all)");

        var timeRangeOption = new Option<string>(
            ["-t", "--time-range"], getDefaultValue: () => "1h", "Time range: 1h, 6h, 24h, 7d");

        var formatOption = new Option<string>(
            ["-f", "--format"], getDefaultValue: () => "table", "Output format: table, json, csv");

        command.AddOption(serviceOption);
        command.AddOption(timeRangeOption);
        command.AddOption(formatOption);
        command.SetHandler(ExecuteAsync, serviceOption, timeRangeOption, formatOption);

        return command;
    }

    private static async Task ExecuteAsync(string? service, string timeRange, string format)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header("Service Metrics");

            var services = string.IsNullOrEmpty(service) || service == "all"
                ? config.GetAllServices().ToList()
                : new List<(string, ServiceEndpoint)> { (service, config.GetService(service)) };

            var allMetrics = new List<ServiceMetrics>();

            foreach ((string serviceName, ServiceEndpoint endpoint) in services)
            {
                var metrics = await GetServiceMetrics(serviceName, endpoint, timeRange);
                allMetrics.Add(metrics);
            }

            // Display results
            switch (format.ToLower())
            {
                case "json":
                    DisplayJson(console, allMetrics);
                    break;
                case "csv":
                    DisplayCsv(console, allMetrics);
                    break;
                default:
                    DisplayTable(console, allMetrics);
                    break;
            }
        }
        catch (Exception ex)
        {
            console.Error($"Error retrieving metrics: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static async Task<ServiceMetrics> GetServiceMetrics(string serviceName, ServiceEndpoint endpoint, string timeRange)
    {
        var metrics = new ServiceMetrics
        {
            Service = serviceName,
            Url = endpoint.Url
        };

        try
        {
            using var client = new CliHttpClient(endpoint.Url);

            // Try to get metrics from /metrics endpoint
            var response = await client.GetAsync<object>("/metrics");

            if (response.Success && response.Data != null)
            {
                // Parse metrics data (simplified for now)
                metrics.Status = "Available";
                metrics.ResponseTime = 0; // Would be measured
                metrics.Uptime = 100.0; // Placeholder
                metrics.RequestCount = 0; // Placeholder
                metrics.ErrorRate = 0.0; // Placeholder
                metrics.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                metrics.Status = "Metrics Unavailable";
            }
        }
        catch (Exception ex)
        {
            metrics.Status = $"Error: {ex.Message}";
        }

        return metrics;
    }

    private static void DisplayTable(ConsoleOutputService console, List<ServiceMetrics> metrics)
    {
        var table = new Table();
        table.AddColumn("Service");
        table.AddColumn("Status");
        table.AddColumn("Uptime");
        table.AddColumn("Response Time");
        table.AddColumn("Requests");
        table.AddColumn("Error Rate");

        foreach (var metric in metrics)
        {
            var statusColor = metric.Status.Contains("Available") ? "[green]" :
                             metric.Status.Contains("Error") ? "[red]" : "[yellow]";

            table.AddRow(
                metric.Service,
                $"{statusColor}{metric.Status}[/]",
                metric.Uptime.HasValue ? $"{metric.Uptime:F1}%" : "-",
                metric.ResponseTime.HasValue ? $"{metric.ResponseTime}ms" : "-",
                metric.RequestCount?.ToString() ?? "-",
                metric.ErrorRate.HasValue ? $"{metric.ErrorRate:F2}%" : "-"
            );
        }

        AnsiConsole.Write(table);
    }

    private static void DisplayJson(ConsoleOutputService console, List<ServiceMetrics> metrics)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(metrics, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        console.Info(json);
    }

    private static void DisplayCsv(ConsoleOutputService console, List<ServiceMetrics> metrics)
    {
        console.Info("Service,Status,Uptime,ResponseTime,RequestCount,ErrorRate,LastUpdated");
        foreach (var metric in metrics)
        {
            console.Info($"{metric.Service},{metric.Status},{metric.Uptime},{metric.ResponseTime},{metric.RequestCount},{metric.ErrorRate},{metric.LastUpdated:o}");
        }
    }

    private class ServiceMetrics
    {
        public string Service { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Status { get; set; } = "Unknown";
        public double? Uptime { get; set; }
        public long? ResponseTime { get; set; }
        public long? RequestCount { get; set; }
        public double? ErrorRate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}