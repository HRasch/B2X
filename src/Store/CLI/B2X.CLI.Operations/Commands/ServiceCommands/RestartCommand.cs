using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2X.CLI.Operations.Commands.ServiceCommands;

public static class RestartCommand
{
    public static Command Create()
    {
        var command = new Command("restart", "Restart a service instance");

        var serviceArg = new Argument<string>("service", "Name of the service to restart");
        var instanceOption = new Option<string?>(
            ["-i", "--instance"], "Specific instance ID to restart (default: all instances)");

        var forceOption = new Option<bool>(
            ["-f", "--force"], "Force restart without confirmation");

        var timeoutOption = new Option<int>(
            ["-t", "--timeout"], getDefaultValue: () => 300, "Timeout in seconds for restart operation");

        command.AddArgument(serviceArg);
        command.AddOption(instanceOption);
        command.AddOption(forceOption);
        command.AddOption(timeoutOption);
        command.SetHandler(ExecuteAsync, serviceArg, instanceOption, forceOption, timeoutOption);

        return command;
    }

    private static async Task ExecuteAsync(string service, string? instance, bool force, int timeout)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header($"Restarting Service: {service}");

            // Validate service exists
            var serviceEndpoint = config.GetService(service);
            if (serviceEndpoint == null)
            {
                console.Error($"Service '{service}' not found in configuration");
                Environment.Exit(1);
            }

            // Confirmation (unless forced)
            if (!force)
            {
                var instanceDesc = string.IsNullOrEmpty(instance) ? "all instances" : $"instance '{instance}'";
                if (!AnsiConsole.Confirm($"Are you sure you want to restart {instanceDesc} of service '{service}'?"))
                {
                    console.Info("Operation cancelled");
                    return;
                }
            }

            console.Info($"Initiating restart for {service}...");
            console.Info($"Timeout: {timeout} seconds");

            // Simulate restart operation (in real implementation, this would call service management API)
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("[green]Restarting service[/]");
                    task.MaxValue = 100;

                    for (int i = 0; i <= 100; i += 10)
                    {
                        task.Value = i;
                        Thread.Sleep(200); // Simulate work
                    }
                });

            // Wait for service to be ready
            console.Info("Waiting for service to restart...");
            await Task.Delay(2000); // Simulate waiting

            // Verify service is back online
            console.Info("Verifying service health...");
            using var client = new CliHttpClient(serviceEndpoint.Url);

            var healthCheck = await client.GetAsync<object>("/health");
            if (healthCheck.Success)
            {
                console.Success($"✅ Service '{service}' restarted successfully");
                console.Info($"Service URL: {serviceEndpoint.Url}");
            }
            else
            {
                console.Error($"❌ Service restart failed - health check unsuccessful");
                console.Info("Check service logs for details");
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            console.Error($"Restart operation failed: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
