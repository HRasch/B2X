using System.CommandLine;
using B2Connect.CLI.Services;
using Spectre.Console;

namespace B2Connect.CLI.Commands.MonitoringCommands;

/// <summary>
/// CLI command to test service connectivity.
/// </summary>
public static class TestConnectivityCommand
{
    public static Command Create()
    {
        var command = new Command("test", "Test connectivity to a registered service");

        var serviceIdArg = new Argument<string>("service-id", "Service ID to test");
        var tenantIdOption = new Option<string>(["--tenant-id", "-t"], "Tenant ID (required)") { IsRequired = true };

        command.AddArgument(serviceIdArg);
        command.AddOption(tenantIdOption);

        command.SetHandler(async (serviceId, tenantId) =>
        {
            await TestConnectivity(serviceId, tenantId);
        }, serviceIdArg, tenantIdOption);

        return command;
    }

    private static async Task TestConnectivity(string serviceId, string tenantId)
    {
        try
        {
            if (!Guid.TryParse(serviceId, out var serviceGuid))
            {
                AnsiConsole.MarkupLine($"[red]Error: Invalid service ID format[/]");
                return;
            }

            var client = new MonitoringServiceClient();

            AnsiConsole.MarkupLine($"[yellow]Testing connectivity to service {serviceId}...[/]");

            var result = await client.TestServiceConnectivityAsync(serviceGuid, tenantId);

            if (result.IsSuccessful)
            {
                AnsiConsole.MarkupLine($"[green]✓ Service is reachable[/]");
                AnsiConsole.MarkupLine($"[dim]Latency: {result.LatencyMs:F2}ms[/]");

                if (result.Data != null && result.Data.Any())
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[bold]Response Details:[/]");
                    foreach (var (key, value) in result.Data)
                    {
                        AnsiConsole.MarkupLine($"[dim]{key}:[/] {value}");
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]✗ Service is unreachable[/]");
                AnsiConsole.MarkupLine($"[dim]Error: {result.ErrorMessage}[/]");

                if (result.Data != null && result.Data.Any())
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[bold]Error Details:[/]");
                    foreach (var (key, value) in result.Data)
                    {
                        AnsiConsole.MarkupLine($"[dim]{key}:[/] {value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error: {ex.Message}[/]");
        }
    }
}
