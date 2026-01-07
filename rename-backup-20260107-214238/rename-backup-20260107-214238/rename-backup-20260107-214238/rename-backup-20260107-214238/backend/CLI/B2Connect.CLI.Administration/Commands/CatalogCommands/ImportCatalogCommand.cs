using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2Connect.CLI.Administration.Commands.CatalogCommands;

public static class ImportCatalogCommand
{
    public static Command Create()
    {
        var command = new Command("import", "Import catalog data from file");

        var fileArgument = new Argument<string>("file", "Path to catalog file (BMEcat XML, CSV, etc.)");
        var formatOption = new Option<string>(
            ["-f", "--format"],
            getDefaultValue: () => "bmecat",
            description: "Catalog format (bmecat, csv, json)");
        var tenantIdOption = new Option<string?>(
            ["-t", "--tenant-id"], "Tenant ID (defaults to environment variable)");
        var dryRunOption = new Option<bool>(
            ["-d", "--dry-run"],
            getDefaultValue: () => false,
            description: "Validate catalog without importing");

        command.AddArgument(fileArgument);
        command.AddOption(formatOption);
        command.AddOption(tenantIdOption);
        command.AddOption(dryRunOption);

        command.SetHandler(ExecuteAsync, fileArgument, formatOption, tenantIdOption, dryRunOption);

        return command;
    }

    private static async Task ExecuteAsync(string filePath, string format, string? tenantId, bool dryRun)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            tenantId ??= config.GetTenantId();
            if (string.IsNullOrEmpty(tenantId))
            {
                console.Error("Tenant ID not provided. Set B2CONNECT_TENANT environment variable or use --tenant-id");
                Environment.Exit(1);
            }

            if (!File.Exists(filePath))
            {
                console.Error($"File not found: {filePath}");
                Environment.Exit(1);
            }

            console.Header($"Importing {format.ToUpper()} catalog");
            console.Info($"File: {filePath}");
            console.Info($"Tenant: {tenantId}");
            console.Info($"Dry run: {dryRun}");

            var catalogUrl = config.GetServiceUrl("catalog");
            var client = new CliHttpClient(catalogUrl);

            var token = config.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.SetAuthorizationToken(token);
            }

            // Read file content
            var fileContent = await File.ReadAllTextAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            var payload = new
            {
                fileName,
                content = fileContent,
                format,
                tenantId = Guid.Parse(tenantId),
                dryRun
            };

            var operation = dryRun ? "Validating" : "Importing";
            console.Spinner($"{operation} catalog...", async () =>
            {
                var endpoint = dryRun ? "/catalog/validate" : "/catalog/import";
                var response = await client.PostAsync<ImportResponse>(endpoint, payload);

                if (response.Success && response.Data != null)
                {
                    if (dryRun)
                    {
                        console.Success("✅ Catalog validation successful");
                        console.Info($"Products found: {response.Data.ProductCount}");
                        console.Info($"Categories found: {response.Data.CategoryCount}");
                        console.Info($"Validation warnings: {response.Data.WarningCount}");
                    }
                    else
                    {
                        console.Success("✅ Catalog imported successfully");
                        console.Info($"Import ID: {response.Data.ImportId}");
                        console.Info($"Products imported: {response.Data.ProductCount}");
                        console.Info($"Categories imported: {response.Data.CategoryCount}");
                        console.Info($"Processing time: {response.Data.ProcessingTimeMs}ms");
                    }
                }
                else
                {
                    console.Error(response.Error ?? "Catalog operation failed");
                }
            });
        }
        catch (Exception ex)
        {
            console.Error($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private class ImportResponse
    {
        public Guid ImportId { get; set; }
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int WarningCount { get; set; }
        public long ProcessingTimeMs { get; set; }
    }
}