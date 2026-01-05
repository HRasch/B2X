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
        var url = $"{registryUrl}/find-compatible?erpType={erpType}&version={erpVersion}";

        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ConnectorInfo>();
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not find compatible connector: {ex.Message}[/]");
        }

        return null;
    }

    private static async Task ListAvailableConnectorsAsync(string registryUrl, string erpType)
    {
        using var client = new HttpClient();
        var url = $"{registryUrl}/list?erpType={erpType}";

        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var connectors = await response.Content.ReadFromJsonAsync<List<ConnectorInfo>>();
                if (connectors != null && connectors.Any())
                {
                    var table = new Table();
                    table.AddColumn("Name");
                    table.AddColumn("Version");
                    table.AddColumn("Supported Versions");

                    foreach (var connector in connectors.Take(5))
                    {
                        table.AddRow(connector.Name, connector.Version, string.Join(", ", connector.SupportedVersions ?? new List<string>()));
                    }

                    AnsiConsole.Write(table);
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not list available connectors: {ex.Message}[/]");
        }
    }

    private static async Task<bool> ConnectorExistsAsync(string tenantId, string connectorId)
    {
        var connectorDir = GetTenantConnectorDirectory(tenantId);
        var connectorPath = Path.Combine(connectorDir, $"{connectorId}.dll");
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
        var connectorDir = GetTenantConnectorDirectory(tenantId);
        Directory.CreateDirectory(connectorDir);

        var connectorPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.dll");
        await File.WriteAllBytesAsync(connectorPath, connectorBytes);

        // Create metadata file
        var metadataPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.metadata.json");
        var metadata = new
        {
            Id = connectorInfo.Id,
            Name = connectorInfo.Name,
            Version = connectorInfo.Version,
            ErpType = connectorInfo.ErpType,
            SupportedVersions = connectorInfo.SupportedVersions,
            InstalledAt = DateTime.UtcNow,
            DownloadSize = connectorBytes.Length
        };

        var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(metadataPath, json);
    }

    private static async Task<bool> VerifyConnectorInstallationAsync(string tenantId, ConnectorInfo connectorInfo)
    {
        try
        {
            var connectorDir = GetTenantConnectorDirectory(tenantId);
            var connectorPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.dll");
            var metadataPath = Path.Combine(connectorDir, $"{connectorInfo.Id}.metadata.json");

            return File.Exists(connectorPath) && File.Exists(metadataPath);
        }
        catch
        {
            return false;
        }
    }

    private static string GetTenantConnectorDirectory(string tenantId)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, ".b2connect", "connectors", tenantId);
    }
}

public class ConnectorInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ErpType { get; set; } = string.Empty;
    public List<string>? SupportedVersions { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public long DownloadSizeBytes { get; set; }
    public string? Requirements { get; set; }
}

public class VersionDetectionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? DetectedVersion { get; set; }
    public string? ErpType { get; set; }
    public bool IsCompatible { get; set; }
    public bool ConnectionSuccessful { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Suggestion { get; set; }
}