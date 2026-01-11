using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;

namespace B2X.CLI.Commands.SystemCommands;

public static class StatusCommand
{
    public static Command Create()
    {
        var command = new Command("status", "Check service health status");

        var serviceOption = new Option<string?>(
            ["-s", "--service"], "Specific service to check (or 'all' for all services)");

        command.AddOption(serviceOption);
        command.SetHandler(ExecuteAsync, serviceOption);

        return command;
    }

    private static async Task ExecuteAsync(string? service)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header("Service Health Status");

            var services = string.IsNullOrEmpty(service) || service == "all"
                ? config.GetAllServices().ToList()
                : new List<(string, ServiceEndpoint)> { (service, config.GetService(service)) };

            var statuses = new List<ServiceStatus>();

            foreach ((string serviceName, ServiceEndpoint endpoint) in services)
            {
                var client = new CliHttpClient(endpoint.Url);
                var status = new ServiceStatus { Service = serviceName, Url = endpoint.Url };

                try
                {
                    var response = await client.GetAsync<object>("/health");
                    status.IsHealthy = response.Success;
                    status.Status = response.Success ? "✓ Healthy" : "✗ Unhealthy";
                }
                catch
                {
                    status.IsHealthy = false;
                    status.Status = "✗ Offline";
                }

                statuses.Add(status);
            }

            // Tabelle ausgeben
            var healthy = statuses.Count(s => s.IsHealthy);
            var total = statuses.Count;

            foreach (var status in statuses)
            {
                if (status.IsHealthy)
                {
                    console.Success($"{status.Service}: {status.Status}");
                }
                else
                {
                    console.Error($"{status.Service}: {status.Status}");
                }
            }

            console.Info($"\nSummary: {healthy.ToString()}/{total.ToString()} services healthy");

            if (healthy < total)
            {
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            console.Error($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private class ServiceStatus
    {
        public string Service { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = "Unknown";
    }
}
