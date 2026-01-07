using System.CommandLine;
using B2Connect.CLI.Services;
using B2Connect.CLI.Shared;
using B2Connect.Shared.Monitoring;
using B2Connect.Shared.Monitoring.Models;
using Spectre.Console;

namespace B2Connect.CLI.Commands.MonitoringCommands;

/// <summary>
/// CLI command to display service status and health metrics.
/// </summary>
public static class ServiceStatusCommand
{
    public static Command Create()
    {
        var command = new Command("status", "Display service status and health metrics");

        var serviceIdArg = new Argument<string?>("service-id", () => null, "Optional service ID to check specific service");
        var tenantIdOption = new Option<string>(["--tenant-id", "-t"], "Tenant ID (required)") { IsRequired = true };
        var formatOption = new Option<string>(["--format", "-f"], () => "table", "Output format: table, json, or csv");

        command.AddArgument(serviceIdArg);
        command.AddOption(tenantIdOption);
        command.AddOption(formatOption);

        command.SetHandler(async (serviceId, tenantId, format) =>
        {
            await DisplayStatus(serviceId, tenantId, format);
        }, serviceIdArg, tenantIdOption, formatOption);

        return command;
    }

    private static async Task DisplayStatus(string? serviceId, string tenantId, string format)
    {
        try
        {
            var client = new MonitoringServiceClient();

            if (!string.IsNullOrEmpty(serviceId))
            {
                if (!Guid.TryParse(serviceId, out var serviceGuid))
                {
                    AnsiConsole.MarkupLine($"[red]Error: Invalid service ID format[/]");
                    return;
                }

                await DisplaySingleServiceStatus(client, serviceGuid, tenantId, format);
            }
            else
            {
                await DisplayAllServicesStatus(client, tenantId, format);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error: {ex.Message}[/]");
        }
    }

    private static async Task DisplaySingleServiceStatus(MonitoringServiceClient client, Guid serviceId, string tenantId, string format)
    {
        AnsiConsole.MarkupLine($"[yellow]Fetching status for service {serviceId}...[/]");

        var service = await client.GetServiceStatusAsync(serviceId, tenantId);

        if (service == null)
        {
            AnsiConsole.MarkupLine($"[yellow]Service not found[/]");
            return;
        }

        DisplayServiceDetails(service, format);
    }

    private static async Task DisplayAllServicesStatus(MonitoringServiceClient client, string tenantId, string format)
    {
        AnsiConsole.MarkupLine($"[yellow]Fetching status for all services...[/]");

        var services = await client.GetServicesAsync(tenantId);

        if (!services.Any())
        {
            AnsiConsole.MarkupLine($"[yellow]No services registered[/]");
            return;
        }

        if (format == "json")
        {
            DisplayAsJson(services);
        }
        else if (format == "csv")
        {
            DisplayAsCsv(services);
        }
        else
        {
            DisplayAsTable(services);
        }
    }

    private static void DisplayServiceDetails(ConnectedService service, string format)
    {
        if (format == "json")
        {
            var json = System.Text.Json.JsonSerializer.Serialize(service, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            AnsiConsole.Write(new Spectre.Console.Json.JsonText(json));
        }
        else
        {
            var panel = new Panel(
                $"[bold]{service.Name}[/]\n" +
                $"[dim]ID:[/] {service.Id}\n" +
                $"[dim]Type:[/] {service.Type}\n" +
                $"[dim]Endpoint:[/] {service.Endpoint}\n" +
                $"[dim]Status:[/] {FormatStatus(service.Status)}\n" +
                $"[dim]Last Checked:[/] {service.LastChecked:yyyy-MM-dd HH:mm:ss UTC}\n" +
                $"[dim]Latency:[/] {service.AverageLatencyMs:F2}ms\n" +
                $"[dim]Uptime:[/] {service.UptimePercent:F2}%")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(2, 1),
                Header = new PanelHeader("[bold cyan]Service Status[/]")
            };

            AnsiConsole.Write(panel);
        }
    }

    private static void DisplayAsTable(IEnumerable<ConnectedService> services)
    {
        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Type");
        table.AddColumn("Status");
        table.AddColumn("Latency (ms)");
        table.AddColumn("Uptime (%)");
        table.AddColumn("Last Checked");

        foreach (var service in services)
        {
            table.AddRow(
                service.Name,
                service.Type.ToString(),
                FormatStatus(service.Status),
                $"{service.AverageLatencyMs:F2}",
                $"{service.UptimePercent:F2}",
                service.LastChecked.ToString("yyyy-MM-dd HH:mm:ss UTC"));
        }

        AnsiConsole.Write(table);
    }

    private static void DisplayAsJson(IEnumerable<ConnectedService> services)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(services, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        AnsiConsole.Write(new Spectre.Console.Json.JsonText(json));
    }

    private static void DisplayAsCsv(IEnumerable<ConnectedService> services)
    {
        AnsiConsole.WriteLine("Name,Type,Status,Latency_ms,Uptime_Percent,Last_Checked");
        foreach (var service in services)
        {
            AnsiConsole.WriteLine($"{service.Name},{service.Type},{service.Status},{service.AverageLatencyMs:F2},{service.UptimePercent:F2},{service.LastChecked:yyyy-MM-dd HH:mm:ss UTC}");
        }
    }

    private static string FormatStatus(ServiceStatus status)
    {
        return status switch
        {
            ServiceStatus.Healthy => "[green]✓ Healthy[/]",
            ServiceStatus.Degraded => "[yellow]⚠ Degraded[/]",
            ServiceStatus.Unhealthy => "[red]✗ Unhealthy[/]",
            ServiceStatus.Unreachable => "[red]✗ Unreachable[/]",
            _ => "[gray]? Unknown[/]"
        };
    }
}
