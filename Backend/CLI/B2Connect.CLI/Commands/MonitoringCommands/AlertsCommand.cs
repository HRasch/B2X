using System.CommandLine;
using B2Connect.CLI.Services;
using Spectre.Console;

namespace B2Connect.CLI.Commands.MonitoringCommands;

/// <summary>
/// CLI command to display resource alerts.
/// </summary>
public static class AlertsCommand
{
    public static Command Create()
    {
        var command = new Command("alerts", "Display resource alerts");

        var serviceIdArg = new Argument<string?>("service-id", () => null, "Optional service ID to filter alerts");
        var tenantIdOption = new Option<string>(["--tenant-id", "-t"], "Tenant ID (required)") { IsRequired = true };
        var thresholdOption = new Option<int>(["--threshold", "-th"], () => 0, "Minimum alert severity threshold (0=All, 1=High, 2=Critical)");

        command.AddArgument(serviceIdArg);
        command.AddOption(tenantIdOption);
        command.AddOption(thresholdOption);

        command.SetHandler(async (serviceId, tenantId, threshold) =>
        {
            await DisplayAlerts(serviceId, tenantId, threshold);
        }, serviceIdArg, tenantIdOption, thresholdOption);

        return command;
    }

    private static async Task DisplayAlerts(string? serviceId, string tenantId, int threshold)
    {
        try
        {
            var client = new MonitoringServiceClient();

            var filters = new Dictionary<string, object>
            {
                { "tenantId", tenantId },
                { "severity", threshold }
            };

            if (!string.IsNullOrEmpty(serviceId) && Guid.TryParse(serviceId, out var serviceGuid))
            {
                filters["serviceId"] = serviceGuid;
            }

            AnsiConsole.MarkupLine($"[yellow]Fetching resource alerts...[/]");

            var alerts = await client.GetResourceAlertsAsync(filters);

            if (!alerts.Any())
            {
                AnsiConsole.MarkupLine($"[green]✓ No active alerts[/]");
                return;
            }

            DisplayAlerts(alerts);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error: {ex.Message}[/]");
        }
    }

    private static void DisplayAlerts(IEnumerable<dynamic> alerts)
    {
        var table = new Table();
        table.Title = new TableTitle("[bold]Resource Alerts[/]");
        table.AddColumn("Service");
        table.AddColumn("Type");
        table.AddColumn("Severity");
        table.AddColumn("Current Value");
        table.AddColumn("Threshold");
        table.AddColumn("Triggered");
        table.AddColumn("Message");

        foreach (var alert in alerts)
        {
            var severity = DetermineSeverity((double)alert.CurrentValue, (double)alert.Threshold);
            var severityColor = severity switch
            {
                "Critical" => "[red]",
                "High" => "[yellow]",
                _ => "[gray]"
            };

            table.AddRow(
                (string)(alert.ServiceName ?? "Unknown"),
                (string)(alert.AlertType?.ToString() ?? "Unknown"),
                $"{severityColor}{severity}[/]",
                $"{(double)alert.CurrentValue:F2}",
                $"{(double)alert.Threshold:F2}",
                ((DateTime)alert.TriggeredAt).ToString("yyyy-MM-dd HH:mm:ss UTC"),
                (string)(alert.Message ?? ""));
        }

        AnsiConsole.Write(table);
    }

    private static string DetermineSeverity(double currentValue, double threshold)
    {
        var percentage = (currentValue / threshold) * 100;

        return percentage switch
        {
            >= 120 => "Critical",
            >= 100 => "High",
            >= 80 => "Medium",
            _ => "Low"
        };
    }
}
