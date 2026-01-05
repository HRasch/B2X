using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.Shared.Monitoring.Abstractions;
using B2Connect.Shared.Monitoring.Models;
using B2Connect.Shared.Monitoring;

namespace B2Connect.CLI.Administration.Commands.MetricsCommands;

public static class MetricsViewCommand
{
    public static Command Create()
    {
        var command = new Command("view", "View current metrics and monitoring data");

        var serviceOption = new Option<string>(
            ["-s", "--service"],
            "Specific service to view metrics for (all services if not specified)");
        var tenantOption = new Option<string>(
            ["-t", "--tenant"],
            "Tenant ID to filter metrics");
        var timeRangeOption = new Option<string>(
            ["-r", "--time-range"],
            "Time range for metrics: 1h, 24h, 7d, 30d (default: 24h)");
        var formatOption = new Option<string>(
            ["-f", "--format"],
            "Output format: table, json, csv (default: table)");
        var liveOption = new Option<bool>(
            ["-l", "--live"],
            "Enable live monitoring mode (updates every 5 seconds)");
        var alertsOnlyOption = new Option<bool>(
            ["-a", "--alerts-only"],
            "Show only services with active alerts");

        command.AddOption(serviceOption);
        command.AddOption(tenantOption);
        command.AddOption(timeRangeOption);
        command.AddOption(formatOption);
        command.AddOption(liveOption);
        command.AddOption(alertsOnlyOption);

        command.SetHandler(ExecuteAsync, serviceOption, tenantOption, timeRangeOption, formatOption, liveOption, alertsOnlyOption);

        return command;
    }

    private static async Task ExecuteAsync(string service, string tenant, string timeRange, string format, bool live, bool alertsOnly)
    {
        var console = new ConsoleOutputService();

        try
        {
            // Set defaults
            timeRange = !string.IsNullOrEmpty(timeRange) ? timeRange : "24h";
            format = !string.IsNullOrEmpty(format) ? format : "table";

            if (live)
            {
                await RunLiveMonitoring(console, service, tenant, timeRange, format, alertsOnly);
            }
            else
            {
                await DisplayMetricsSnapshot(console, service, tenant, timeRange, format, alertsOnly);
            }

        }
        catch (Exception ex)
        {
            console.Error($"Failed to view metrics: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task DisplayMetricsSnapshot(ConsoleOutputService console, string service, string tenant, string timeRange, string format, bool alertsOnly)
    {
        console.Header($"Service Metrics ({timeRange})");

        using var monitoringClient = new MonitoringServiceClient();

        // Get services to monitor
        var services = await GetServicesToMonitor(monitoringClient, service, tenant);

        if (!services.Any())
        {
            console.Warning("No services found to monitor.");
            return;
        }

        var allMetrics = new List<ServiceMetrics>();

        foreach (var svc in services)
        {
            var metrics = await GetServiceMetrics(monitoringClient, svc, timeRange);
            if (metrics != null)
            {
                allMetrics.Add(metrics);
            }
        }

        // Filter for alerts only if requested
        if (alertsOnly)
        {
            allMetrics = allMetrics.Where(m => m.HasActiveAlerts).ToList();
        }

        if (!allMetrics.Any())
        {
            console.Info("No metrics data available for the specified criteria.");
            return;
        }

        // Display metrics based on format
        switch (format.ToLower())
        {
            case "json":
                DisplayMetricsJson(console, allMetrics);
                break;
            case "csv":
                DisplayMetricsCsv(console, allMetrics);
                break;
            default:
                DisplayMetricsTable(console, allMetrics);
                break;
        }
    }

    private static async Task RunLiveMonitoring(ConsoleOutputService console, string service, string tenant, string timeRange, string format, bool alertsOnly)
    {
        console.Info("Starting live monitoring mode... (Press Ctrl+C to stop)");
        console.Info("");

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                Console.Clear();
                await DisplayMetricsSnapshot(console, service, tenant, timeRange, format, alertsOnly);
                console.Info($"\nLast updated: {DateTime.Now:HH:mm:ss} | Next update in 5 seconds...");

                await Task.Delay(5000, cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            console.Info("\nLive monitoring stopped.");
        }
    }

    private static async Task<IEnumerable<ConnectedService>> GetServicesToMonitor(
        MonitoringServiceClient monitoringClient, string serviceFilter, string tenantFilter)
    {
        var config = new ConfigurationService();
        var configuredServices = config.GetAllServices();

        var monitoredServices = new List<ConnectedService>();

        foreach (var (name, endpoint) in configuredServices)
        {
            if (!string.IsNullOrEmpty(serviceFilter) && !name.Equals(serviceFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            // Get service details from monitoring service
            var services = await monitoringClient.GetServicesAsync(tenantFilter ?? "system");
            var monitoredService = services.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (monitoredService != null)
            {
                monitoredServices.Add(monitoredService);
            }
            else
            {
                // Service not registered with monitoring, create a basic entry
                monitoredServices.Add(new ConnectedService
                {
                    Name = name,
                    Type = ServiceType.Api,
                    Endpoint = endpoint.Url,
                    TenantId = tenantFilter ?? "system"
                });
            }
        }

        return monitoredServices;
    }

    private static async Task<ServiceMetrics?> GetServiceMetrics(
        MonitoringServiceClient monitoringClient, ConnectedService service, string timeRange)
    {
        try
        {
            // Test service connectivity
            var testResult = await monitoringClient.TestServiceConnectivityAsync(service.Id, service.TenantId);

            // Get resource alerts
            var alerts = await monitoringClient.GetResourceAlertsAsync(new Dictionary<string, object>
            {
                ["serviceId"] = service.Id,
                ["tenantId"] = service.TenantId,
                ["timeRange"] = timeRange
            });

            return new ServiceMetrics
            {
                Service = service,
                ConnectivityTest = testResult,
                ActiveAlerts = alerts?.ToList() ?? new List<dynamic>(),
                LastUpdated = DateTime.UtcNow
            };
        }
        catch
        {
            return null;
        }
    }

    private static void DisplayMetricsTable(ConsoleOutputService console, List<ServiceMetrics> metrics)
    {
        console.Info("┌─────────────────┬────────────┬────────────┬────────────┬────────────┬────────────┐");
        console.Info("│ Service         │ Status     │ Response   │ CPU        │ Memory     │ Alerts     │");
        console.Info("│                 │            │ Time (ms)  │ Usage (%)  │ Usage (%)  │ Count      │");
        console.Info("├─────────────────┼────────────┼────────────┼────────────┼────────────┼────────────┤");

        foreach (var metric in metrics)
        {
            var status = metric.ConnectivityTest?.IsSuccessful == true ? "✓ UP" : "✗ DOWN";
            var responseTime = metric.ConnectivityTest?.LatencyMs.ToString("F0") ?? "N/A";
            var cpuUsage = "N/A"; // Would come from actual metrics
            var memoryUsage = "N/A"; // Would come from actual metrics
            var alertCount = metric.ActiveAlerts.Count.ToString();

            console.Info($"│ {metric.Service.Name,-15} │ {status,-10} │ {responseTime,-10} │ {cpuUsage,-10} │ {memoryUsage,-10} │ {alertCount,-10} │");
        }

        console.Info("└─────────────────┴────────────┴────────────┴────────────┴────────────┴────────────┘");

        // Show alerts summary
        var totalAlerts = metrics.Sum(m => m.ActiveAlerts.Count);
        if (totalAlerts > 0)
        {
            console.Warning($"\nActive Alerts: {totalAlerts}");
            foreach (var metric in metrics.Where(m => m.ActiveAlerts.Any()))
            {
                console.Warning($"  {metric.Service.Name}: {metric.ActiveAlerts.Count} alerts");
            }
        }
    }

    private static void DisplayMetricsJson(ConsoleOutputService console, List<ServiceMetrics> metrics)
    {
        var jsonData = new
        {
            Timestamp = DateTime.UtcNow,
            Services = metrics.Select(m => new
            {
                m.Service.Name,
                m.Service.Endpoint,
                Status = m.ConnectivityTest?.IsSuccessful == true ? "UP" : "DOWN",
                ResponseTimeMs = m.ConnectivityTest?.LatencyMs,
                ActiveAlerts = m.ActiveAlerts.Count,
                LastUpdated = m.LastUpdated
            }).ToList()
        };

        console.Info(System.Text.Json.JsonSerializer.Serialize(jsonData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }

    private static void DisplayMetricsCsv(ConsoleOutputService console, List<ServiceMetrics> metrics)
    {
        console.Info("Service,Status,ResponseTimeMs,ActiveAlerts,LastUpdated");
        foreach (var metric in metrics)
        {
            var status = metric.ConnectivityTest?.IsSuccessful == true ? "UP" : "DOWN";
            var responseTime = metric.ConnectivityTest?.LatencyMs.ToString() ?? "";
            var alerts = metric.ActiveAlerts.Count.ToString();
            var lastUpdated = metric.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss");

            console.Info($"{metric.Service.Name},{status},{responseTime},{alerts},{lastUpdated}");
        }
    }
}

public class ServiceMetrics
{
    public required ConnectedService Service { get; set; }
    public ServiceTestResult? ConnectivityTest { get; set; }
    public List<dynamic> ActiveAlerts { get; set; } = new();
    public DateTime LastUpdated { get; set; }

    public bool HasActiveAlerts => ActiveAlerts.Any();
}