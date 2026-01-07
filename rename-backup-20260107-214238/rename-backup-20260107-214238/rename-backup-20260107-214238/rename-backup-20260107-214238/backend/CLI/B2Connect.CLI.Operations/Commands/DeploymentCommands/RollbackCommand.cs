using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2Connect.CLI.Operations.Commands.DeploymentCommands;

public static class RollbackCommand
{
    public static Command Create()
    {
        var command = new Command("rollback", "Rollback a deployment to previous version");

        var serviceArg = new Argument<string>("service", "Name of the service to rollback");
        var targetVersionOption = new Option<string?>(
            ["-v", "--version"], "Target version to rollback to (default: previous version)");

        var environmentOption = new Option<string>(
            ["-e", "--environment"], getDefaultValue: () => "development", "Target environment: development, staging, production");

        var reasonOption = new Option<string>(
            ["-r", "--reason"], "Reason for rollback (required for audit trail)")
        { IsRequired = true };

        var timeoutOption = new Option<int>(
            ["-t", "--timeout"], getDefaultValue: () => 600, "Timeout in seconds for rollback operation");

        command.AddArgument(serviceArg);
        command.AddOption(targetVersionOption);
        command.AddOption(environmentOption);
        command.AddOption(reasonOption);
        command.AddOption(timeoutOption);
        command.SetHandler(ExecuteAsync, serviceArg, targetVersionOption, environmentOption, reasonOption, timeoutOption);

        return command;
    }

    private static async Task ExecuteAsync(string service, string? targetVersion, string environment, string reason, int timeout)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Header($"Rolling Back Service: {service}");

            // Validate service exists
            var serviceEndpoint = config.GetService(service);
            if (serviceEndpoint == null)
            {
                console.Error($"Service '{service}' not found in configuration");
                Environment.Exit(1);
            }

            // Get current version info (simulated)
            var currentVersion = await GetCurrentVersion(service);
            var rollbackVersion = targetVersion ?? await GetPreviousVersion(service);

            if (string.IsNullOrEmpty(rollbackVersion))
            {
                console.Error("No previous version available for rollback");
                Environment.Exit(1);
            }

            console.Info($"Current version: {currentVersion}");
            console.Info($"Rollback target: {rollbackVersion}");
            console.Info($"Environment: {environment}");
            console.Info($"Reason: {reason}");

            // Safety confirmation
            console.Warning("⚠️  ROLLBACK OPERATION");
            console.Warning("This will temporarily take the service offline");
            console.Warning("Ensure you have tested the rollback version");

            if (!AnsiConsole.Confirm($"Are you sure you want to rollback '{service}' from {currentVersion} to {rollbackVersion}?"))
            {
                console.Info("Rollback cancelled");
                return;
            }

            // Create rollback record for audit trail
            console.Info("Creating rollback audit record...");
            await CreateRollbackRecord(service, currentVersion, rollbackVersion, reason, environment);

            // Execute rollback
            console.Info("Initiating rollback operation...");
            var rollbackResult = await ExecuteRollback(service, rollbackVersion, environment, timeout);

            if (rollbackResult.Success)
            {
                console.Success($"✅ Service '{service}' rolled back successfully");
                console.Info($"New version: {rollbackVersion}");
                console.Info($"Previous version: {currentVersion} (available for re-deployment)");
                console.Info($"Service URL: {serviceEndpoint.Url}");

                // Verify service health after rollback
                console.Info("Verifying service health after rollback...");
                using var client = new CliHttpClient(serviceEndpoint.Url);
                var healthCheck = await client.GetAsync<object>("/health");

                if (healthCheck.Success)
                {
                    console.Success("✅ Service health check passed");
                }
                else
                {
                    console.Warning("⚠️  Service health check failed - monitor closely");
                }

            }
            else
            {
                console.Error($"❌ Rollback failed: {rollbackResult.Error}");
                console.Info("Service may be in an inconsistent state - manual intervention required");
                Environment.Exit(1);
            }

        }
        catch (Exception ex)
        {
            console.Error($"Rollback operation failed: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static async Task<string> GetCurrentVersion(string service)
    {
        // In real implementation, query deployment service
        await Task.Delay(100);
        return "2.1.0"; // Simulated
    }

    private static async Task<string?> GetPreviousVersion(string service)
    {
        // In real implementation, query deployment history
        await Task.Delay(100);
        return "2.0.1"; // Simulated
    }

    private static async Task CreateRollbackRecord(string service, string fromVersion, string toVersion, string reason, string environment)
    {
        // In real implementation, store in audit database
        await Task.Delay(200);
        // Rollback record created for audit trail
    }

    private static async Task<RollbackResult> ExecuteRollback(string service, string targetVersion, string environment, int timeout)
    {
        // Simulate rollback operation
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask($"[yellow]Rolling back {service} to {targetVersion}[/]");
                task.MaxValue = 100;

                for (int i = 0; i <= 100; i += 5)
                {
                    task.Value = i;
                    Thread.Sleep(300); // Simulate rollback time
                }
            });

        await Task.Delay(5000); // Simulate actual rollback

        // Simulate success (90% success rate for demo)
        var success = new Random().Next(100) < 90;

        return new RollbackResult
        {
            Success = success,
            Error = success ? null : "Deployment service reported rollback failure"
        };
    }

    private class RollbackResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}