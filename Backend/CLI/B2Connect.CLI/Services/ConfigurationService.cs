using System.Text.Json;

namespace B2Connect.CLI.Services;

/// <summary>
/// Service für CLI Konfiguration und Service-Endpoints
/// </summary>
public class ConfigurationService
{
    private readonly string _configPath;
    private Dictionary<string, ServiceEndpoint> _endpoints = new();

    public ConfigurationService()
    {
        // Standardpfade für Configuration
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _configPath = Path.Combine(home, ".b2connect", "config.json");

        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        if (!File.Exists(_configPath))
        {
            CreateDefaultConfiguration();
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<ConfigurationRoot>(json);

            if (config?.Services != null)
            {
                _endpoints = config.Services;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to load configuration: {ex.Message}");
            _endpoints = GetDefaultEndpoints();
        }
    }

    private void CreateDefaultConfiguration()
    {
        var dir = Path.GetDirectoryName(_configPath);
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var config = new ConfigurationRoot
        {
            Services = GetDefaultEndpoints(),
            Environment = "development",
            Timeout = 30
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }

    private Dictionary<string, ServiceEndpoint> GetDefaultEndpoints()
    {
        return new()
        {
            ["identity"] = new ServiceEndpoint { Url = "http://localhost:7002", Description = "Identity Service" },
            ["tenancy"] = new ServiceEndpoint { Url = "http://localhost:7003", Description = "Tenancy Service" },
            ["localization"] = new ServiceEndpoint { Url = "http://localhost:7004", Description = "Localization Service" },
            ["catalog"] = new ServiceEndpoint { Url = "http://localhost:7005", Description = "Catalog Service" },
            ["cms"] = new ServiceEndpoint { Url = "http://localhost:7006", Description = "CMS Service" },
            ["theming"] = new ServiceEndpoint { Url = "http://localhost:7008", Description = "Theming Service" },
            ["search"] = new ServiceEndpoint { Url = "http://localhost:9300", Description = "Search Service" }
        };
    }

    public string GetServiceUrl(string serviceName)
    {
        if (_endpoints.TryGetValue(serviceName.ToLower(), out var endpoint))
        {
            return endpoint.Url;
        }

        throw new InvalidOperationException($"Service '{serviceName}' not configured");
    }

    public ServiceEndpoint GetService(string serviceName)
    {
        if (_endpoints.TryGetValue(serviceName.ToLower(), out var endpoint))
        {
            return endpoint;
        }

        throw new InvalidOperationException($"Service '{serviceName}' not configured");
    }

    public IEnumerable<(string Name, ServiceEndpoint Endpoint)> GetAllServices()
    {
        return _endpoints.Select(x => (x.Key, x.Value));
    }

    public string? GetToken()
    {
        return Environment.GetEnvironmentVariable("B2CONNECT_TOKEN");
    }

    public string? GetTenantId()
    {
        return Environment.GetEnvironmentVariable("B2CONNECT_TENANT");
    }
}

public class ConfigurationRoot
{
    public Dictionary<string, ServiceEndpoint> Services { get; set; } = new();
    public string Environment { get; set; } = "development";
    public int Timeout { get; set; } = 30;
}

public class ServiceEndpoint
{
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
}
