using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;
using Spectre.Console;
using System.Net.Http.Json;
using System.Text.Json;

namespace B2Connect.CLI.Commands.ErpCommands;

public static class DownloadConnectorCommand
{
    public static Command Create()
    {
        var command = new Command("download-connector", "Download and install ERP connector for a tenant");

        var tenantIdArgument = new Argument<string>("tenant-id", "Tenant identifier");
        var erpTypeArgument = new Argument<string>("erp-type", "ERP system type (e.g., enventa, sap, oracle)");
        var erpVersionOption = new Option<string>(
            ["-v", "--erp-version"], "ERP system version (auto-detected if not provided)");
        var forceOption = new Option<bool>(
            ["-f", "--force"], "Force download even if connector already exists");
        var registryUrlOption = new Option<string>(
            ["-r", "--registry-url"], "Connector registry URL (uses default if not provided)");

        command.AddArgument(tenantIdArgument);
        command.AddArgument(erpTypeArgument);
        command.AddOption(erpVersionOption);
        command.AddOption(forceOption);
        command.AddOption(registryUrlOption);

        command.SetHandler(ExecuteAsync, tenantIdArgument, erpTypeArgument, erpVersionOption, forceOption, registryUrlOption);

        return command;
    }

    private static async Task ExecuteAsync(string tenantId, string erpType, string? erpVersion, bool force, string? registryUrl)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.Info($"Starting ERP connector download for tenant '{tenantId}'");

            // Get registry URL
            var registryBaseUrl = registryUrl ?? config.GetConnectorRegistryUrl() ?? "https://registry.b2connect.com/api/connectors";

            // Step 1: Detect ERP version if not provided
            if (string.IsNullOrEmpty(erpVersion))
            {
                console.Info("Detecting ERP system version...");
                erpVersion = await DetectErpVersionAsync(tenantId, erpType, config);
                if (string.IsNullOrEmpty(erpVersion))
                {
                    console.Error("Could not detect ERP version. Please specify --erp-version manually.");
                    return;
                }
                console.Success($"Detected ERP version: {erpVersion}");
            }

            // Step 2: Find compatible connector
            console.Info($"Finding compatible connector for {erpType} v{erpVersion}...");
            var connectorInfo = await FindCompatibleConnectorAsync(registryBaseUrl, erpType, erpVersion);

            if (connectorInfo == null)
            {
                console.Error($"No compatible connector found for {erpType} v{erpVersion}");
                console.Info("Available connectors:");
                await ListAvailableConnectorsAsync(registryBaseUrl, erpType);
                return;
            }

            console.Success($"Found compatible connector: {connectorInfo.Name} v{connectorInfo.Version}");

            // Step 3: Check if connector already exists
            if (!force && await ConnectorExistsAsync(tenantId, connectorInfo.Id))
            {
                console.Warning($"Connector {connectorInfo.Name} already exists for tenant {tenantId}");
                console.Info("Use --force to overwrite existing connector");
                return;
            }

            // Step 4: Download connector
            console.Info("Downloading connector...");
            var downloadUrl = $"{registryBaseUrl}/download/{connectorInfo.Id}";
            var connectorBytes = await DownloadConnectorAsync(downloadUrl);

            // Step 5: Install connector
            console.Info("Installing connector...");
            await InstallConnectorAsync(tenantId, connectorInfo, connectorBytes);

            console.Success($"Successfully installed {connectorInfo.Name} v{connectorInfo.Version} for tenant {tenantId}");

            // Step 6: Verify installation
            console.Info("Verifying installation...");
            var verificationResult = await VerifyConnectorInstallationAsync(tenantId, connectorInfo);
            if (verificationResult)
            {
                console.Success("Connector installation verified successfully");
            }
            else
            {
                console.Warning("Connector installation could not be verified. Please check logs.");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Failed to download connector: {ex.Message}");
            if (ex.InnerException != null)
            {
                console.Error($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    private static async Task<string?> DetectErpVersionAsync(string tenantId, string erpType, ConfigurationService config)
    {
        try
        {
            // Get ERP service URL
            var erpServiceUrl = config.GetServiceUrl("erp");
            if (string.IsNullOrEmpty(erpServiceUrl))
            {
                return null;
            }

            var client = new CliHttpClient(erpServiceUrl);
            var token = config.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.SetAuthorizationToken(token);
            }

            // Call version detection endpoint
            var response = await client.GetAsync<VersionDetectionResult>($"/api/erp/detect-version?tenantId={tenantId}&erpType={erpType}");

            if (response.Success)
            {
                return response.Data?.DetectedVersion;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not detect ERP version: {ex.Message}[/]");
        }

        return null;
    }

    private static async Task<ConnectorInfo?> FindCompatibleConnectorAsync(string registryUrl, string erpType, string erpVersion)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{registryUrl}/find-compatible?erpType={erpType}&erpVersion={erpVersion}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ConnectorInfo>();
        }

        return null;
    }

    private static async Task ListAvailableConnectorsAsync(string registryUrl, string erpType)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{registryUrl}/list?erpType={erpType}");

            if (response.IsSuccessStatusCode)
            {
                var connectors = await response.Content.ReadFromJsonAsync<List<ConnectorInfo>>();
                if (connectors != null && connectors.Any())
                {
                    var table = new Table();
                    table.AddColumn("Name");
                    table.AddColumn("Version");
                    table.AddColumn("Supported ERP Versions");

                    foreach (var connector in connectors)
                    {
                        table.AddRow(
                            connector.Name,
                            connector.Version,
                            string.Join(", ", connector.SupportedVersions ?? new List<string>())
                        );
                    }

                    AnsiConsole.Write(table);
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error listing connectors: {ex.Message}[/]");
        }
    }

    private static async Task<bool> ConnectorExistsAsync(string tenantId, string connectorId)
    {
        // Check local connector registry
        var connectorPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "connectors",
            tenantId,
            $"{connectorId}.dll"
        );

        return File.Exists(connectorPath);
    }

    private static async Task<byte[]> DownloadConnectorAsync(string downloadUrl)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(downloadUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Download failed: {response.StatusCode}");
        }

        return await response.Content.ReadAsByteArrayAsync();
    }

    private static async Task InstallConnectorAsync(string tenantId, ConnectorInfo connectorInfo, byte[] connectorBytes)
    {
        // Create tenant-specific connector directory
        var connectorDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "connectors",
            tenantId
        );

        Directory.CreateDirectory(connectorDir);

        // Save connector assembly
        var connectorPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.dll");
        await File.WriteAllBytesAsync(connectorPath, connectorBytes);

        // Save connector metadata
        var metadataPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.json");
        var metadata = new
        {
            connectorInfo.Id,
            connectorInfo.Name,
            connectorInfo.Version,
            connectorInfo.ErpType,
            connectorInfo.SupportedVersions,
            InstalledAt = DateTime.UtcNow,
            TenantId = tenantId
        };

        var metadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(metadataPath, metadataJson);
    }

    private static async Task<bool> VerifyConnectorInstallationAsync(string tenantId, ConnectorInfo connectorInfo)
    {
        try
        {
            var connectorDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "connectors",
                tenantId
            );

            var connectorPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.dll");
            var metadataPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.json");

            return File.Exists(connectorPath) && File.Exists(metadataPath);
        }
        catch
        {
            return false;
        }
    }

    private class VersionDetectionResult
    {
        public string? DetectedVersion { get; set; }
        public bool DetectionSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    private class ConnectorInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ErpType { get; set; } = string.Empty;
        public List<string>? SupportedVersions { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}