using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using B2X.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2X.CLI.Commands.ErpCommands;

public static class DetectVersionCommand
{
    public static Command Create()
    {
        var command = new Command("detect-version", "Detect ERP system version for a tenant");

        var tenantIdArgument = new Argument<string>("tenant-id", "Tenant identifier");
        var erpTypeArgument = new Argument<string>("erp-type", "ERP system type (e.g., enventa, sap, oracle)");
        var connectionStringOption = new Option<string>(
            ["-c", "--connection-string"], "ERP connection string (if not stored in configuration)");
        var timeoutOption = new Option<int>(
            ["-t", "--timeout"], "Detection timeout in seconds (default: 30)");

        command.AddArgument(tenantIdArgument);
        command.AddArgument(erpTypeArgument);
        command.AddOption(connectionStringOption);
        command.AddOption(timeoutOption);

        command.SetHandler(ExecuteAsync, tenantIdArgument, erpTypeArgument, connectionStringOption, timeoutOption);

        return command;
    }

    private static async Task ExecuteAsync(string tenantId, string erpType, string? connectionString, int timeoutSeconds = 30)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Info($"Detecting ERP version for tenant '{tenantId}' ({erpType})");

            // Get ERP service URL
            var erpServiceUrl = config.GetServiceUrl("erp");
            if (string.IsNullOrEmpty(erpServiceUrl))
            {
                console.Error("ERP service URL not configured. Please set the ERP service URL in configuration.");
                return;
            }

            var client = new CliHttpClient(erpServiceUrl);
            var token = config.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.SetAuthorizationToken(token);
            }

            // Prepare detection request
            var detectionRequest = new
            {
                TenantId = tenantId,
                ErpType = erpType,
                ConnectionString = connectionString,
                TimeoutSeconds = timeoutSeconds
            };

            console.Info("Connecting to ERP system for version detection...");

            // Call version detection endpoint
            var response = await client.PostAsync<VersionDetectionResult>("/api/erp/detect-version", new
            {
                TenantId = tenantId,
                ErpType = erpType,
                ConnectionString = connectionString,
                TimeoutSeconds = timeoutSeconds
            });

            if (response.Success)
            {
                var result = response.Data;

                if (result == null)
                {
                    console.Error("Invalid response from version detection service");
                    return;
                }

                if (!result.Success)
                {
                    console.Error($"Version detection failed: {result.ErrorMessage}");
                    if (!string.IsNullOrEmpty(result.Suggestion))
                    {
                        console.Info($"Suggestion: {result.Suggestion}");
                    }
                    return;
                }

                // Display results
                var table = new Table();
                table.Title = new TableTitle($"[green]ERP Version Detection Results[/]");
                table.AddColumn("Property");
                table.AddColumn("Value");

                table.AddRow("ERP Type", result.ErpType ?? "Unknown");
                table.AddRow("Detected Version", result.DetectedVersion ?? "Unknown");
                table.AddRow("API Version", result.ApiVersion ?? "Unknown");
                table.AddRow("System Name", result.SystemName ?? "Unknown");
                table.AddRow("Detection Method", result.DetectionMethod ?? "Unknown");
                table.AddRow("Confidence", $"{result.Confidence:P0}");

                if (!string.IsNullOrEmpty(result.AdditionalInfo))
                {
                    table.AddRow("Additional Info", result.AdditionalInfo);
                }

                AnsiConsole.Write(table);

                console.Success($"Successfully detected {result.ErpType} version {result.DetectedVersion}");

                // Suggest next steps
                console.Info("\nNext steps:");
                console.Info($"1. Download connector: b2c erp download-connector {tenantId} {erpType} --erp-version {result.DetectedVersion}");
                console.Info($"2. Configure tenant: b2c tenant configure-erp {tenantId} {erpType} {result.DetectedVersion}");
            }
            else
            {
                console.Error($"Version detection failed: {response.Error}");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Failed to detect ERP version: {ex.Message}");
            if (ex.InnerException != null)
            {
                console.Error($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    private class VersionDetectionResult
    {
        public bool Success { get; set; }
        public string? ErpType { get; set; }
        public string? DetectedVersion { get; set; }
        public string? ApiVersion { get; set; }
        public string? SystemName { get; set; }
        public string? DetectionMethod { get; set; }
        public double Confidence { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Suggestion { get; set; }
    }
}