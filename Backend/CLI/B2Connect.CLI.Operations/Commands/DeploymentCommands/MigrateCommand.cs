using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2Connect.CLI.Operations.Commands.DeploymentCommands;

public static class MigrateCommand
{
    public static Command Create()
    {
        var command = new Command("migrate", "Run database migrations for services");

        var serviceOption = new Option<string?>(
            ["-s", "--service"], "Specific service to migrate (default: all services with migrations)");

        var environmentOption = new Option<string>(
            ["-e", "--environment"], getDefaultValue: () => "development", "Target environment: development, staging, production");

        var dryRunOption = new Option<bool>(
            ["--dry-run"], "Show what migrations would be applied without executing them");

        var timeoutOption = new Option<int>(
            ["-t", "--timeout"], getDefaultValue: () => 300, "Timeout in seconds for migration operations");

        command.AddOption(serviceOption);
        command.AddOption(environmentOption);
        command.AddOption(dryRunOption);
        command.AddOption(timeoutOption);
        command.SetHandler(ExecuteAsync, serviceOption, environmentOption, dryRunOption, timeoutOption);

        return command;
    }

    private static async Task ExecuteAsync(string? service, string environment, bool dryRun, int timeout)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            var operation = dryRun ? "DRY RUN - Database Migration Preview" : "Database Migration";
            console.Header($"{operation} - Environment: {environment}");

            if (dryRun)
            {
                console.Warning("⚠️  DRY RUN MODE - No actual changes will be made");
            }

            // Get services that need migration
            var servicesToMigrate = GetServicesWithMigrations(config, service);

            if (!servicesToMigrate.Any())
            {
                console.Info("No services found that require database migrations");
                return;
            }

            console.Info($"Found {servicesToMigrate.Count} service(s) with pending migrations:");
            foreach (var svc in servicesToMigrate)
            {
                console.Info($"  - {svc.Service} ({svc.Url})");
            }

            if (!dryRun && !AnsiConsole.Confirm($"Proceed with migration in {environment} environment?"))
            {
                console.Info("Migration cancelled");
                return;
            }

            var results = new List<MigrationResult>();

            foreach (var svc in servicesToMigrate)
            {
                console.Info($"\nMigrating {svc.Service}...");
                var result = await RunMigration(svc, environment, dryRun, timeout);
                results.Add(result);

                if (result.Success)
                {
                    console.Success($"✅ {svc.Service} migration completed");
                }
                else
                {
                    console.Error($"❌ {svc.Service} migration failed: {result.Error}");
                }
            }

            // Summary
            var successful = results.Count(r => r.Success);
            var failed = results.Count(r => !r.Success);

            console.Header("Migration Summary");
            console.Info($"Total services: {results.Count}");
            console.Info($"Successful: {successful}");
            console.Info($"Failed: {failed}");

            if (failed > 0)
            {
                console.Error("Some migrations failed - check logs for details");
                Environment.Exit(1);
            }
            else
            {
                console.Success("All migrations completed successfully");
            }

        }
        catch (Exception ex)
        {
            console.Error($"Migration operation failed: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static List<(string Service, string Url)> GetServicesWithMigrations(ConfigurationService config, string? specificService)
    {
        // In a real implementation, this would check which services have database migrations
        // For now, return a subset of services that typically have databases
        var allServices = config.GetAllServices().ToList();
        var dbServices = allServices.Where(s => s.Name.Contains("catalog") ||
                                               s.Name.Contains("identity") ||
                                               s.Name.Contains("order") ||
                                               s.Name.Contains("cms"))
                                   .Select(s => (Service: s.Name, Url: s.Endpoint.Url))
                                   .ToList();

        if (!string.IsNullOrEmpty(specificService))
        {
            return dbServices.Where(s => s.Service == specificService).ToList();
        }

        return dbServices;
    }

    private static async Task<MigrationResult> RunMigration((string Service, string Url) service, string environment, bool dryRun, int timeout)
    {
        var result = new MigrationResult { Service = service.Service };

        try
        {
            using var client = new CliHttpClient(service.Url);

            // Simulate migration endpoint call
            var endpoint = dryRun ? "/migrations/preview" : "/migrations/run";
            var response = await client.PostAsync(endpoint, new
            {
                environment,
                timeout
            });

            if (response.Success)
            {
                result.Success = true;
                result.Message = dryRun ? "Migration preview completed" : "Migration completed successfully";
            }
            else
            {
                result.Success = false;
                result.Error = $"Migration failed: {response.Error ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
        }

        return result;
    }

    private class MigrationResult
    {
        public string Service { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}