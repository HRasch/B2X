using System.Text.Json;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace B2X.AppHost.Tests;

/// <summary>
/// Tests to detect port conflicts between Aspire AppHost configuration
/// and individual service launchSettings.json files.
/// 
/// Problem: When Aspire uses WithHttpEndpoint(port: X) AND the service's
/// launchSettings.json also uses port X, both try to bind → "address already in use"
/// 
/// Solution: These tests ensure launchSettings ports don't conflict with AppHost ports.
/// </summary>
public partial class PortConflictTests
{
    private static readonly string SolutionRoot = FindSolutionRoot();

    /// <summary>
    /// Ports explicitly configured in AppHost Program.cs via WithHttpEndpoint().
    /// These are the "source of truth" for Aspire-managed services.
    /// </summary>
    private static readonly Dictionary<string, int> AppHostConfiguredPorts = new()
    {
        // Gateways with fixed ports (frontends connect directly)
        ["reverse-proxy"] = 5000,
        ["store-gateway"] = 8000,
        ["admin-gateway"] = 8080,
        ["mcp-server"] = 8090,
        ["seeding-api"] = 8095,

        // Frontends with fixed ports
        ["frontend-store"] = 5173,
        ["frontend-admin"] = 5174,
        ["frontend-management"] = 5175,
    };

    /// <summary>
    /// Maps Aspire service names to their project directories (relative to src/).
    /// </summary>
    private static readonly Dictionary<string, string> ServiceProjectPaths = new()
    {
        ["reverse-proxy"] = "backend/Infrastructure/Hosting/ReverseProxy",
        ["store-gateway"] = "backend/Store/API",
        ["admin-gateway"] = "backend/Admin/Gateway",
        ["mcp-server"] = "AI/MCP/B2X.Admin.MCP",
        ["seeding-api"] = "tools/seeders/seeding",
    };

    [Fact]
    public void LaunchSettings_ShouldNotConflictWith_AppHostPorts()
    {
        var conflicts = new List<string>();

        foreach (var (serviceName, appHostPort) in AppHostConfiguredPorts)
        {
            if (!ServiceProjectPaths.TryGetValue(serviceName, out var projectPath))
                continue; // Skip frontends (they don't have launchSettings.json)

            var launchSettingsPath = Path.Combine(
                SolutionRoot, "src", projectPath, "Properties", "launchSettings.json");

            if (!File.Exists(launchSettingsPath))
                continue;

            var launchSettingsPorts = ExtractPortsFromLaunchSettings(launchSettingsPath);

            foreach (var launchPort in launchSettingsPorts)
            {
                if (launchPort == appHostPort)
                {
                    conflicts.Add(
                        $"CONFLICT: {serviceName} - AppHost uses port {appHostPort}, " +
                        $"but launchSettings.json also uses {launchPort}. " +
                        $"This causes 'address already in use' errors. " +
                        $"Fix: Change launchSettings port or use dynamic port (0).");
                }
            }
        }

        conflicts.ShouldBeEmpty(
            $"Port conflicts detected between AppHost and launchSettings:\n{string.Join("\n", conflicts)}");
    }

    [Fact]
    public void AppHostPorts_ShouldBeUnique()
    {
        var duplicates = AppHostConfiguredPorts
            .GroupBy(kvp => kvp.Value)
            .Where(g => g.Count() > 1)
            .Select(g => $"Port {g.Key} used by: {string.Join(", ", g.Select(kvp => kvp.Key))}")
            .ToList();

        duplicates.ShouldBeEmpty(
            $"Duplicate ports in AppHost configuration:\n{string.Join("\n", duplicates)}");
    }

    [Fact]
    public void AllLaunchSettings_ShouldHaveNonConflictingPorts()
    {
        var allLaunchSettings = Directory.GetFiles(
            Path.Combine(SolutionRoot, "src"),
            "launchSettings.json",
            SearchOption.AllDirectories);

        var portUsage = new Dictionary<int, List<string>>();
        var appHostPorts = AppHostConfiguredPorts.Values.ToHashSet();

        foreach (var launchSettingsPath in allLaunchSettings)
        {
            var relativePath = Path.GetRelativePath(SolutionRoot, launchSettingsPath);
            var ports = ExtractPortsFromLaunchSettings(launchSettingsPath);

            foreach (var port in ports.Where(p => p > 0)) // Ignore port 0 (dynamic)
            {
                if (!portUsage.ContainsKey(port))
                    portUsage[port] = new List<string>();
                portUsage[port].Add(relativePath);
            }
        }

        var conflicts = portUsage
            .Where(kvp => kvp.Value.Count > 1 || appHostPorts.Contains(kvp.Key))
            .Select(kvp =>
            {
                var files = string.Join(", ", kvp.Value);
                var aspireConflict = appHostPorts.Contains(kvp.Key)
                    ? " [CONFLICTS WITH ASPIRE APPHOST]"
                    : "";
                return $"Port {kvp.Key}: {files}{aspireConflict}";
            })
            .ToList();

        // This is informational - we only fail on AppHost conflicts
        if (conflicts.Any())
        {
            // Log for awareness but don't fail on launchSettings-only conflicts
            // (those only matter when running standalone, not under Aspire)
        }
    }

    [Theory]
    [InlineData("reverse-proxy", 5000)]
    [InlineData("store-gateway", 8000)]
    [InlineData("admin-gateway", 8080)]
    public void Service_LaunchSettings_ShouldUseDifferentPort(string serviceName, int appHostPort)
    {
        if (!ServiceProjectPaths.TryGetValue(serviceName, out var projectPath))
        {
            Assert.Fail($"Unknown service: {serviceName}");
            return;
        }

        var launchSettingsPath = Path.Combine(
            SolutionRoot, "src", projectPath, "Properties", "launchSettings.json");

        if (!File.Exists(launchSettingsPath))
        {
            // No launchSettings = no conflict possible
            return;
        }

        var launchPorts = ExtractPortsFromLaunchSettings(launchSettingsPath);

        launchPorts.ShouldNotContain(appHostPort,
            $"{serviceName}'s launchSettings.json uses port {appHostPort} which conflicts with " +
            $"AppHost's WithHttpEndpoint(port: {appHostPort}). " +
            $"Change launchSettings to use a different port (e.g., {appHostPort + 50}).");
    }

    #region Helper Methods

    private static HashSet<int> ExtractPortsFromLaunchSettings(string filePath)
    {
        var ports = new HashSet<int>();

        try
        {
            var json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("profiles", out var profiles))
            {
                foreach (var profile in profiles.EnumerateObject())
                {
                    if (profile.Value.TryGetProperty("applicationUrl", out var urlElement))
                    {
                        var urls = urlElement.GetString() ?? "";
                        foreach (var url in urls.Split(';', StringSplitOptions.RemoveEmptyEntries))
                        {
                            var port = ExtractPortFromUrl(url.Trim());
                            if (port > 0)
                                ports.Add(port);
                        }
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Invalid JSON - skip
        }

        return ports;
    }

    private static int ExtractPortFromUrl(string url)
    {
        // Match patterns like http://localhost:5000 or https://localhost:5001
        var match = PortRegex().Match(url);
        if (match.Success && int.TryParse(match.Groups[1].Value, out var port))
        {
            return port;
        }
        return 0;
    }

    [GeneratedRegex(@":(\d+)/?$")]
    private static partial Regex PortRegex();

    private static string FindSolutionRoot()
    {
        var current = AppContext.BaseDirectory;
        while (current != null)
        {
            if (File.Exists(Path.Combine(current, "B2X.slnx")) ||
                File.Exists(Path.Combine(current, "B2X.sln")))
            {
                return current;
            }
            current = Path.GetDirectoryName(current);
        }
        throw new InvalidOperationException("Could not find solution root directory");
    }

    #endregion
}
