using B2X.Admin.Presentation.Controllers;

namespace B2X.Admin.Application.Services;

/// <summary>
/// Service for managing CLI tool downloads and metadata
/// Handles Administration-CLI and ERP-Connector distribution to tenants
/// </summary>
public interface ICliToolsService
{
    /// <summary>
    /// Get available CLI tools for a tenant
    /// </summary>
    Task<IEnumerable<CliToolInfo>> GetAvailableToolsAsync(Guid tenantId);

    /// <summary>
    /// Get Administration-CLI file for download
    /// </summary>
    Task<CliToolFileInfo?> GetAdministrationCliAsync(Guid tenantId, string version, string osType);

    /// <summary>
    /// Get ERP-Connector file for download
    /// </summary>
    Task<CliToolFileInfo?> GetErpConnectorAsync(Guid tenantId, string erpType, string version);

    /// <summary>
    /// Get installation instructions for a tool
    /// </summary>
    Task<CliInstallationInstructions?> GetInstallationInstructionsAsync(
        Guid tenantId, string toolType, string osType);

    /// <summary>
    /// Get available versions for a tool
    /// </summary>
    Task<IEnumerable<CliVersionInfo>> GetAvailableVersionsAsync(Guid tenantId, string toolType);
}

/// <summary>
/// Default implementation of CLI tools service
/// Manages CLI distribution with tenant isolation
/// </summary>
public class CliToolsService : ICliToolsService
{
    private readonly ILogger<CliToolsService> _logger;
    private readonly IConfiguration _configuration;

    public CliToolsService(ILogger<CliToolsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IEnumerable<CliToolInfo>> GetAvailableToolsAsync(Guid tenantId)
    {
        try
        {
            // Validate tenant can access tools
            await ValidateTenantAccessAsync(tenantId).ConfigureAwait(false);

            var tools = new List<CliToolInfo>
            {
                new CliToolInfo(
                    ToolType: "administration-cli",
                    Name: "B2X Administration CLI",
                    Description: "Tenant administration and management tool for B2X",
                    LatestVersion: GetLatestVersion("administration-cli"),
                    IsAvailable: true,
                    SupportedOperatingSystems: new[] { "win", "linux", "osx" },
                    LastUpdated: DateTime.UtcNow),
                new CliToolInfo(
                    ToolType: "erp-connector",
                    Name: "ERP Connector",
                    Description: "ERP system integration connector",
                    LatestVersion: GetLatestVersion("erp-connector"),
                    IsAvailable: true,
                    SupportedOperatingSystems: new[] { "win", "linux" },
                    LastUpdated: DateTime.UtcNow)
            };

            _logger.LogInformation("Retrieved available tools for tenant {TenantId}", tenantId);
            return await Task.FromResult(tools).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available tools for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<CliToolFileInfo?> GetAdministrationCliAsync(Guid tenantId, string version, string osType)
    {
        try
        {
            // Validate tenant access
            await ValidateTenantAccessAsync(tenantId).ConfigureAwait(false);

            // Validate version and OS
            if (!await ValidateVersionAsync(version).ConfigureAwait(false))
            {
                _logger.LogWarning("Invalid version {Version} requested for administration-cli", version);
                return null;
            }

            if (!IsSupportedOperatingSystem(osType, new[] { "win", "linux", "osx" }))
            {
                _logger.LogWarning("Unsupported OS type {OSType} for administration-cli", osType);
                return null;
            }

            // Get file path from configuration or artifact repository
            var filePath = ResolveCliFilePath("administration-cli", version, osType);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("CLI file not found at {FilePath}", filePath);
                return null;
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = Path.GetFileName(filePath);

            _logger.LogInformation("Prepared Administration-CLI download for tenant {TenantId}", tenantId);

            return new CliToolFileInfo(
                FileStream: fileStream,
                ContentType: GetContentType(osType),
                FileName: fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Administration-CLI for download");
            throw;
        }
    }

    public async Task<CliToolFileInfo?> GetErpConnectorAsync(Guid tenantId, string erpType, string version)
    {
        try
        {
            // Validate tenant access
            await ValidateTenantAccessAsync(tenantId).ConfigureAwait(false);

            // Validate ERP type
            var supportedErpTypes = new[] { "enventa", "craft" };
            if (!supportedErpTypes.Contains(erpType.ToLowerInvariant()))
            {
                _logger.LogWarning("Unsupported ERP type {ERP} requested", erpType);
                return null;
            }

            // Validate version
            if (!await ValidateVersionAsync(version).ConfigureAwait(false))
            {
                _logger.LogWarning("Invalid version {Version} for ERP-Connector {ERP}", version, erpType);
                return null;
            }

            // Get file path
            var filePath = ResolveErpConnectorFilePath(erpType, version);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("ERP-Connector file not found at {FilePath}", filePath);
                return null;
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = Path.GetFileName(filePath);

            _logger.LogInformation("Prepared ERP-Connector ({ERP}) download for tenant {TenantId}",
                erpType, tenantId);

            return new CliToolFileInfo(
                FileStream: fileStream,
                ContentType: "application/octet-stream",
                FileName: fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing ERP-Connector for download");
            throw;
        }
    }

    public async Task<CliInstallationInstructions?> GetInstallationInstructionsAsync(
        Guid tenantId, string toolType, string osType)
    {
        try
        {
            await ValidateTenantAccessAsync(tenantId).ConfigureAwait(false);

            return toolType.ToLowerInvariant() switch
            {
                "administration-cli" => GetAdministrationCliInstructions(osType),
                "erp-connector" => GetErpConnectorInstructions(osType),
                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving installation instructions");
            throw;
        }
    }

    public async Task<IEnumerable<CliVersionInfo>> GetAvailableVersionsAsync(Guid tenantId, string toolType)
    {
        try
        {
            await ValidateTenantAccessAsync(tenantId).ConfigureAwait(false);

            // Return mock versions - in production, this would query artifact repository
            var versions = new List<CliVersionInfo>
            {
                new CliVersionInfo(
                    Version: "1.0.0",
                    ReleaseDate: DateTime.UtcNow.AddDays(-30),
                    ReleaseNotes: "Initial release with basic tenant management capabilities",
                    IsPrerelease: false,
                    BreakingChanges: Array.Empty<string>()),
                new CliVersionInfo(
                    Version: "1.1.0",
                    ReleaseDate: DateTime.UtcNow.AddDays(-15),
                    ReleaseNotes: "Added ERP-Connector management commands",
                    IsPrerelease: false,
                    BreakingChanges: Array.Empty<string>()),
                new CliVersionInfo(
                    Version: "1.2.0",
                    ReleaseDate: DateTime.UtcNow,
                    ReleaseNotes: "Enhanced configuration and improved error handling",
                    IsPrerelease: false,
                    BreakingChanges: Array.Empty<string>())
            };

            return await Task.FromResult(versions.OrderByDescending(v => v.ReleaseDate)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available versions");
            throw;
        }
    }

    // Private helper methods

    private async Task ValidateTenantAccessAsync(Guid tenantId)
    {
        // TODO: Implement actual tenant validation
        // For now, just log the access
        await Task.CompletedTask.ConfigureAwait(false);
        _logger.LogDebug("Validating tenant {TenantId} access", tenantId);
    }

    private string GetLatestVersion(string toolType)
    {
        return toolType switch
        {
            "administration-cli" => "1.2.0",
            "erp-connector" => "1.1.0",
            _ => "1.0.0"
        };
    }

    private async Task<bool> ValidateVersionAsync(string version)
    {
        if (version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Simple semantic version validation
        return System.Text.RegularExpressions.Regex.IsMatch(version, @"^\d+\.\d+\.\d+$");
    }

    private bool IsSupportedOperatingSystem(string osType, string[] supportedTypes)
    {
        return supportedTypes.Contains(osType.ToLowerInvariant());
    }

    private string ResolveCliFilePath(string toolType, string version, string osType)
    {
        // TODO: Implement actual file path resolution from artifact storage
        var artifactDir = _configuration["ArtifactStorage:CliDirectory"] ?? "artifacts/cli";
        var actualVersion = version.Equals("latest", StringComparison.OrdinalIgnoreCase)
            ? GetLatestVersion(toolType)
            : version;

        return Path.Combine(artifactDir, toolType, actualVersion, $"B2X-cli-{osType}");
    }

    private string ResolveErpConnectorFilePath(string erpType, string version)
    {
        // TODO: Implement actual file path resolution from artifact storage
        var artifactDir = _configuration["ArtifactStorage:ConnectorDirectory"] ?? "artifacts/connectors";
        var actualVersion = version.Equals("latest", StringComparison.OrdinalIgnoreCase)
            ? GetLatestVersion("erp-connector")
            : version;

        return Path.Combine(artifactDir, erpType, actualVersion, $"connector-{erpType}.exe");
    }

    private string GetContentType(string osType)
    {
        return osType.ToLowerInvariant() switch
        {
            "win" => "application/x-msdownload",
            "linux" or "osx" => "application/octet-stream",
            _ => "application/octet-stream"
        };
    }

    private CliInstallationInstructions GetAdministrationCliInstructions(string osType)
    {
        return osType.ToLowerInvariant() switch
        {
            "win" => new CliInstallationInstructions(
                ToolType: "administration-cli",
                OperatingSystem: "Windows",
                Steps: new[]
                {
                    "1. Download the Administration-CLI executable",
                    "2. Extract to a folder (e.g., C:\\B2X-cli)",
                    "3. Add folder to PATH environment variable",
                    "4. Open new Command Prompt",
                    "5. Run: B2X info (to verify installation)",
                    "6. Set environment variable: set B2X_TENANT_TOKEN=your-api-key",
                    "7. Run: B2X tenant list (to test connectivity)"
                },
                Prerequisites: new[] { ".NET Runtime 8.0+", "Windows 7 or later" },
                ConfigurationTemplate: "# B2X-config.json\n{\n  \"endpoint\": \"https://api.B2X.local\",\n  \"tenantId\": \"your-tenant-id\",\n  \"apiKey\": \"your-api-key\"\n}",
                TroubleshootingTips: new[]
                {
                    "If command not recognized, verify PATH contains CLI folder",
                    "If connection fails, check firewall and endpoint URL",
                    "For API key issues, regenerate in admin dashboard"
                }),

            "linux" => new CliInstallationInstructions(
                ToolType: "administration-cli",
                OperatingSystem: "Linux",
                Steps: new[]
                {
                    "1. Download the Administration-CLI binary",
                    "2. chmod +x ./B2X-cli",
                    "3. sudo mv ./B2X-cli /usr/local/bin/",
                    "4. Run: B2X info (to verify installation)",
                    "5. Export API key: export B2X_TENANT_TOKEN=your-api-key",
                    "6. Run: B2X tenant list (to test connectivity)"
                },
                Prerequisites: new[] { ".NET Runtime 8.0+", "Ubuntu 20.04 LTS or later" },
                ConfigurationTemplate: "#!/bin/bash\nexport B2X_ENDPOINT=\"https://api.B2X.local\"\nexport B2X_TENANT_ID=\"your-tenant-id\"\nexport B2X_TENANT_TOKEN=\"your-api-key\"",
                TroubleshootingTips: new[]
                {
                    "If .so not found error, install: libssl-dev libkrb5-dev",
                    "Verify execution permissions: ls -la /usr/local/bin/B2X-cli",
                    "Check runtime: dotnet --version"
                }),

            "osx" => new CliInstallationInstructions(
                ToolType: "administration-cli",
                OperatingSystem: "macOS",
                Steps: new[]
                {
                    "1. Download the Administration-CLI binary",
                    "2. chmod +x ./B2X-cli",
                    "3. sudo mv ./B2X-cli /usr/local/bin/",
                    "4. Run: B2X info (to verify installation)",
                    "5. Export API key: export B2X_TENANT_TOKEN=your-api-key",
                    "6. Run: B2X tenant list (to test connectivity)"
                },
                Prerequisites: new[] { ".NET Runtime 8.0+", "macOS 12 or later", "Xcode Command Line Tools" },
                ConfigurationTemplate: "#!/bin/bash\nexport B2X_ENDPOINT=\"https://api.B2X.local\"\nexport B2X_TENANT_ID=\"your-tenant-id\"\nexport B2X_TENANT_TOKEN=\"your-api-key\"",
                TroubleshootingTips: new[]
                {
                    "If 'cannot be opened', run: xattr -d com.apple.quarantine ./B2X-cli",
                    "Verify installation: which B2X-cli",
                    "Check .NET runtime: dotnet --version"
                }),

            _ => throw new ArgumentException($"Unsupported OS: {osType}")
        };
    }

    private CliInstallationInstructions GetErpConnectorInstructions(string osType)
    {
        return osType.ToLowerInvariant() switch
        {
            "win" => new CliInstallationInstructions(
                ToolType: "erp-connector",
                OperatingSystem: "Windows",
                Steps: new[]
                {
                    "1. Download the ERP-Connector installer",
                    "2. Run the installer and follow the setup wizard",
                    "3. Select your ERP system type during installation",
                    "4. Configure connection parameters to your ERP",
                    "5. Set tenant API key for B2X integration",
                    "6. Start the connector service",
                    "7. Verify connection in Administration Dashboard"
                },
                Prerequisites: new[] { ".NET Framework 4.8+", "Windows Server 2016 or later", "ERP system access credentials" },
                ConfigurationTemplate: "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<configuration>\n  <erp type=\"enventa\">\n    <connection>your-erp-connection-string</connection>\n  </erp>\n  <B2X>\n    <endpoint>https://api.B2X.local</endpoint>\n    <tenantId>your-tenant-id</tenantId>\n    <apiKey>your-api-key</apiKey>\n  </B2X>\n</configuration>",
                TroubleshootingTips: new[]
                {
                    "If service won't start, check Event Viewer for details",
                    "Verify ERP credentials and connectivity",
                    "Ensure firewall allows outbound HTTPS connections",
                    "Check logs in: C:\\Program Files\\B2X\\ERP-Connector\\logs"
                }),

            "linux" => new CliInstallationInstructions(
                ToolType: "erp-connector",
                OperatingSystem: "Linux",
                Steps: new[]
                {
                    "1. Download the ERP-Connector package",
                    "2. tar xzf B2X-erp-connector.tar.gz",
                    "3. cd B2X-erp-connector",
                    "4. sudo ./install.sh",
                    "5. Edit configuration file at /etc/B2X/erp-connector.conf",
                    "6. systemctl start B2X-erp-connector",
                    "7. systemctl enable B2X-erp-connector (to auto-start)"
                },
                Prerequisites: new[] { ".NET Runtime 8.0+", "Ubuntu 20.04 LTS or later", "ERP system access", "systemd" },
                ConfigurationTemplate: "[erp]\ntype=enventa\nconnection_string=your-erp-connection\n\n[B2X]\nendpoint=https://api.B2X.local\ntenant_id=your-tenant-id\napi_key=your-api-key",
                TroubleshootingTips: new[]
                {
                    "Check service status: systemctl status B2X-erp-connector",
                    "View logs: journalctl -u B2X-erp-connector -f",
                    "Verify permissions: ls -la /etc/B2X/",
                    "Test connectivity: curl -X GET http://localhost:9091/health"
                }),

            _ => throw new ArgumentException($"Unsupported OS: {osType}")
        };
    }
}
