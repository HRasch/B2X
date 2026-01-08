using System.CommandLine;
using B2X.CLI.Services;
using B2X.CLI.Shared;
using B2X.Shared.Monitoring;
using B2X.Shared.Monitoring.Models;
using Spectre.Console;

namespace B2X.CLI.Commands.MonitoringCommands;

/// <summary>
/// CLI command to register a service for monitoring.
/// </summary>
public static class RegisterServiceCommand
{
    public static Command Create()
    {
        var command = new Command("register", "Register a service for monitoring");

        var nameArg = new Argument<string>("name", "Service name (ERP, PIM, CRM, etc.)");
        var typeArg = new Argument<string>("type", "Service type (ERP, PIM, CRM, Other)");
        var endpointArg = new Argument<string>("endpoint", "Service endpoint URL");
        var tenantIdOption = new Option<string>(["--tenant-id", "-t"], "Tenant ID (required)") { IsRequired = true };

        command.AddArgument(nameArg);
        command.AddArgument(typeArg);
        command.AddArgument(endpointArg);
        command.AddOption(tenantIdOption);

        command.SetHandler(async (name, type, endpoint, tenantId) =>
        {
            await RegisterService(name, type, endpoint, tenantId);
        }, nameArg, typeArg, endpointArg, tenantIdOption);

        return command;
    }

    private static async Task RegisterService(string name, string type, string endpoint, string tenantId)
    {
        try
        {
            var client = new MonitoringServiceClient();

            if (!Enum.TryParse<ServiceType>(type, ignoreCase: true, out var serviceType))
            {
                AnsiConsole.MarkupLine($"[red]Error: Invalid service type '{type}'. Valid types are: {string.Join(", ", Enum.GetNames<ServiceType>())}[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Registering service '{name}' ({serviceType}) at {endpoint}...[/]");

            var service = new ConnectedService
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = serviceType,
                Endpoint = endpoint,
                TenantId = tenantId,
                Status = ServiceStatus.Unknown,
                LastChecked = DateTime.UtcNow
            };

            var success = await client.RegisterServiceAsync(service);

            if (success)
            {
                AnsiConsole.MarkupLine($"[green]✓ Service '{name}' registered successfully[/]");
                AnsiConsole.MarkupLine($"[dim]Service ID: {service.Id}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]✗ Failed to register service[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error: {ex.Message}[/]");
        }
    }
}
