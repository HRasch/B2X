using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2X.CLI.Operations.Commands.MonitoringCommands;

public static class DashboardCommand
{
    public static Command Create()
    {
        var command = new Command("dashboard", "Display comprehensive monitoring dashboard");

        var refreshOption = new Option<int>(
            ["-r", "--refresh"], getDefaultValue: () => 0, "Auto-refresh interval in seconds (0 = no refresh)");

        var formatOption = new Option<string>(
            ["-f", "--format"], getDefaultValue: () => "full", "Display format: full, summary, minimal");

        command.AddOption(refreshOption);
        command.AddOption(formatOption);
        command.SetHandler(ExecuteAsync, refreshOption, formatOption);

        return command;
    }

    private static async Task ExecuteAsync(int refreshInterval, string format)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        do
        {
            try
            {
                console.Header("Platform Monitoring Dashboard");
                AnsiConsole.MarkupLine($"[dim]Last updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}[/]");

                // Get system overview
                var systemStatus = await GetSystemOverview();

                // Display based on format
                switch (format.ToLower())
                {
                    case "summary":
                        DisplaySummary(systemStatus);
                        break;
                    case "minimal":
                        DisplayMinimal(systemStatus);
                        break;
                    default:
                        DisplayFullDashboard(systemStatus);
                        break;
                }

                if (refreshInterval > 0)
                {
                    AnsiConsole.MarkupLine($"\n[dim]Refreshing in {refreshInterval} seconds... Press Ctrl+C to exit[/]");
                    await Task.Delay(TimeSpan.FromSeconds(refreshInterval));
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                console.Error($"Dashboard error: {ex.Message}");
                if (refreshInterval == 0)
                    Environment.Exit(1);
            }
        } while (refreshInterval > 0);
    }

    private static async Task<SystemOverview> GetSystemOverview()
    {
        var overview = new SystemOverview();
        var config = new ConfigurationService();

        // Get service statuses
        var services = config.GetAllServices().ToList();
        overview.TotalServices = services.Count;

        foreach ((string serviceName, ServiceEndpoint endpoint) in services)
        {
            try
            {
                using var client = new CliHttpClient(endpoint.Url);
                var response = await client.GetAsync<object>("/health");

                if (response.Success)
                {
                    overview.HealthyServices++;
                }
                else
                {
                    overview.UnhealthyServices++;
                }
            }
            catch
            {
                overview.UnhealthyServices++;
            }
        }

        // Get alerts (if monitoring service available)
        try
        {
            var monitoringClient = new MonitoringServiceClient();
            var alerts = await monitoringClient.GetResourceAlertsAsync(new Dictionary<string, object>());
            overview.ActiveAlerts = alerts?.Count() ?? 0;
        }
        catch
        {
            overview.ActiveAlerts = -1; // Indicates monitoring service unavailable
        }

        overview.LastUpdated = DateTime.UtcNow;
        return overview;
    }

    private static void DisplayFullDashboard(SystemOverview overview)
    {
        // System Health Panel
        var healthPanel = new Panel(GetHealthSummary(overview))
        {
            Header = new PanelHeader("System Health"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(healthPanel);

        // Services Table
        var servicesTable = new Table();
        servicesTable.AddColumn("Metric");
        servicesTable.AddColumn("Value");
        servicesTable.AddColumn("Status");

        servicesTable.AddRow("Total Services", overview.TotalServices.ToString(), "[blue]ℹ[/]");
        servicesTable.AddRow("Healthy Services", overview.HealthyServices.ToString(),
            overview.HealthyServices == overview.TotalServices ? "[green]✓[/]" : "[yellow]⚠[/]");
        servicesTable.AddRow("Unhealthy Services", overview.UnhealthyServices.ToString(),
            overview.UnhealthyServices == 0 ? "[green]✓[/]" : "[red]✗[/]");

        var alertsStatus = overview.ActiveAlerts == -1 ? "[yellow]N/A[/]" :
                          overview.ActiveAlerts == 0 ? "[green]✓[/]" : $"[red]{overview.ActiveAlerts}[/]";
        servicesTable.AddRow("Active Alerts", overview.ActiveAlerts == -1 ? "N/A" : overview.ActiveAlerts.ToString(), alertsStatus);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(servicesTable);

        // Performance metrics (placeholder for future)
        AnsiConsole.WriteLine();
        var perfPanel = new Panel("[dim]Performance metrics coming soon...[/]")
        {
            Header = new PanelHeader("Performance"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(perfPanel);
    }

    private static void DisplaySummary(SystemOverview overview)
    {
        var health = overview.UnhealthyServices == 0 ? "[green]HEALTHY[/]" : "[red]DEGRADED[/]";
        var services = $"{overview.HealthyServices}/{overview.TotalServices}";
        var alerts = overview.ActiveAlerts == -1 ? "N/A" : overview.ActiveAlerts.ToString();

        AnsiConsole.MarkupLine($"System Status: {health}");
        AnsiConsole.MarkupLine($"Services: {services} healthy");
        AnsiConsole.MarkupLine($"Active Alerts: {alerts}");
    }

    private static void DisplayMinimal(SystemOverview overview)
    {
        var status = overview.UnhealthyServices == 0 ? "✓" : "✗";
        AnsiConsole.MarkupLine($"{status} {overview.HealthyServices}/{overview.TotalServices} services healthy");
    }

    private static string GetHealthSummary(SystemOverview overview)
    {
        var healthPercentage = overview.TotalServices > 0 ?
            (overview.HealthyServices * 100.0 / overview.TotalServices) : 0;

        var status = healthPercentage >= 95 ? "Excellent" :
                    healthPercentage >= 80 ? "Good" :
                    healthPercentage >= 50 ? "Fair" : "Poor";

        return $"[bold]{status}[/]\n" +
               $"Health: {healthPercentage:F1}%\n" +
               $"Last Check: {overview.LastUpdated:HH:mm:ss}";
    }

    private class SystemOverview
    {
        public int TotalServices { get; set; }
        public int HealthyServices { get; set; }
        public int UnhealthyServices { get; set; }
        public int ActiveAlerts { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
