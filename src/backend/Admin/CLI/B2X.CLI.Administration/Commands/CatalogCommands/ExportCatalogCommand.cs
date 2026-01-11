using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2X.CLI.Administration.Commands.CatalogCommands;

public static class ExportCatalogCommand
{
    public static Command Create()
    {
        var command = new Command("export", "Export catalog data to file");

        var outputArgument = new Argument<string>("output", "Output file path");
        var formatOption = new Option<string>(
            ["-f", "--format"],
            getDefaultValue: () => "bmecat",
            description: "Export format (bmecat, csv, json)");
        var tenantIdOption = new Option<string?>(
            ["-t", "--tenant-id"], "Tenant ID (defaults to environment variable)");
        var includeInactiveOption = new Option<bool>(
            ["-i", "--include-inactive"],
            getDefaultValue: () => false,
            description: "Include inactive products in export");

        command.AddArgument(outputArgument);
        command.AddOption(formatOption);
        command.AddOption(tenantIdOption);
        command.AddOption(includeInactiveOption);

        command.SetHandler(ExecuteAsync, outputArgument, formatOption, tenantIdOption, includeInactiveOption);

        return command;
    }

    private static async Task ExecuteAsync(string outputPath, string format, string? tenantId, bool includeInactive)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            tenantId ??= config.GetTenantId();
            if (string.IsNullOrEmpty(tenantId))
            {
                console.Error("Tenant ID not provided. Set B2X_TENANT environment variable or use --tenant-id");
                Environment.Exit(1);
            }

            console.Header($"Exporting {format.ToUpper()} catalog");
            console.Info($"Output: {outputPath}");
            console.Info($"Tenant: {tenantId}");
            console.Info($"Include inactive: {includeInactive}");

            var catalogUrl = config.GetServiceUrl("catalog");
            var client = new CliHttpClient(catalogUrl);

            var token = config.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.SetAuthorizationToken(token);
            }

            var payload = new
            {
                format,
                tenantId = Guid.Parse(tenantId),
                includeInactive
            };

            ExportResponse? response = null;

            console.Spinner("Exporting catalog...", async () =>
            {
                var result = await client.PostAsync<ExportResponse>("/catalog/export", payload);
                response = result.Data;

                if (!result.Success || response == null)
                {
                    console.Error(result.Error ?? "Export failed");
                    Environment.Exit(1);
                }
            });

            if (response != null)
            {
                // Write content to file
                await File.WriteAllTextAsync(outputPath, response.Content);
                console.Success("✅ Catalog exported successfully");
                console.Info($"File: {outputPath}");
                console.Info($"Products exported: {response.ProductCount}");
                console.Info($"Categories exported: {response.CategoryCount}");
                console.Info($"File size: {response.Content.Length} bytes");
                console.Info($"Processing time: {response.ProcessingTimeMs}ms");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private class ExportResponse
    {
        public string Content { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public long ProcessingTimeMs { get; set; }
    }
}
