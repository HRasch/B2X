using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2X.CLI.Operations.Commands.HealthCommands;

public static class SystemStatusCommand
{
    public static Command Create()
    {
        var command = new Command("status", "Show system status and service overview");

        var serviceOption = new Option<string?>(
            ["-s", "--service"], "Specific service to check (or 'all' for all services)");

        var formatOption = new Option<string>(
            ["-f", "--format"], getDefaultValue: () => "table", "Output format: table, json, csv");

        command.AddOption(serviceOption);
        command.AddOption(formatOption);
        command.SetHandler(ExecuteAsync, serviceOption, formatOption);

        return command;
    }

    private static async Task ExecuteAsync(string? service, string format)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header("System Status Overview");

            var services = string.IsNullOrEmpty(service) || service == "all"
                ? config.GetAllServices().ToList()
                : new List<(string, ServiceEndpoint)> { (service, config.GetService(service)) };

            var statuses = new List<ServiceStatus>();

            console.Info($"Checking status of {services.Count} services...");

            foreach ((string serviceName, ServiceEndpoint endpoint) in services)
            {
                var status = await GetServiceStatus(serviceName, endpoint);
                statuses.Add(status);
            }

            // Display results based on format
            switch (format.ToLower())
            {
                case "json":
                    DisplayJson(console, statuses);
                    break;
                case "csv":
                    DisplayCsv(console, statuses);
                    break;
                default:
                    DisplayTable(console, statuses);
                    break;
            }

            // Summary
            var healthy = statuses.Count(s => s.IsHealthy);
            var total = statuses.Count;
            var healthPercentage = total > 0 ? (healthy * 100.0 / total) : 0;

            console.Info($"\nSystem Health: {healthy}/{total} services healthy ({healthPercentage:F1}%)");

            if (healthy < total)
            {
                console.Warning("⚠️  Some services are unhealthy - check logs for details");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Error retrieving system status: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static async Task<ServiceStatus> GetServiceStatus(string serviceName, ServiceEndpoint endpoint)
    {
        var status = new ServiceStatus
        {
            Service = serviceName,
            Url = endpoint.Url,
            Description = endpoint.Description
        };

        try
        {
            using var client = new CliHttpClient(endpoint.Url);
            var response = await client.GetAsync<object>("/health");

            status.IsHealthy = response.Success;
            status.LastChecked = DateTime.UtcNow;

            if (response.Success)
            {
                status.Status = "Healthy";
                status.Uptime = "Available";
            }
            else
            {
                status.Status = "Unhealthy";
                status.Uptime = "Degraded";
            }
        }
        catch (Exception ex)
        {
            status.IsHealthy = false;
            status.Status = "Offline";
            status.Uptime = "Unavailable";
            status.Error = ex.Message;
        }

        return status;
    }

    private static void DisplayTable(ConsoleOutputService console, List<ServiceStatus> statuses)
    {
        var table = new Table();
        table.AddColumn("Service");
        table.AddColumn("Status");
        table.AddColumn("URL");
        table.AddColumn("Description");

        foreach (var status in statuses)
        {
            var statusColor = status.IsHealthy ? "[green]" : "[red]";
            var statusIcon = status.IsHealthy ? "✓" : "✗";

            table.AddRow(
                status.Service,
                $"{statusColor}{statusIcon} {status.Status}[/]",
                status.Url,
                status.Description
            );
        }

        AnsiConsole.Write(table);
    }

    private static void DisplayJson(ConsoleOutputService console, List<ServiceStatus> statuses)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(statuses, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        console.Info(json);
    }

    private static void DisplayCsv(ConsoleOutputService console, List<ServiceStatus> statuses)
    {
        console.Info("Service,Status,URL,Description,IsHealthy,LastChecked");
        foreach (var status in statuses)
        {
            console.Info($"{status.Service},{status.Status},{status.Url},{status.Description},{status.IsHealthy},{status.LastChecked:o}");
        }
    }

    private class ServiceStatus
    {
        public string Service { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = "Unknown";
        public string Uptime { get; set; } = "Unknown";
        public int? StatusCode { get; set; }
        public DateTime? LastChecked { get; set; }
        public string? Error { get; set; }
    }
}
