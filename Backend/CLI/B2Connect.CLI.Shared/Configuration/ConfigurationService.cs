using System.Text.Json;

namespace B2Connect.CLI.Shared.Configuration;

public interface IConfigurationSection : IEnumerable<KeyValuePair<string, string>>
{
    string? this[string key] { get; }
}

// Simple implementation of IConfigurationSection
public class SimpleConfigurationSection : IConfigurationSection
{
    private readonly Dictionary<string, string> _data;

    public SimpleConfigurationSection(Dictionary<string, string> data)
    {
        _data = data ?? new Dictionary<string, string>();
    }

    public string? this[string key] => _data.TryGetValue(key, out var value) ? value : null;

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

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

    public string GetServiceUrl(string serviceName, string environment)
    {
        // For now, environment-specific URLs are not implemented
        // This could be extended to support different URLs per environment
        return GetServiceUrl(serviceName);
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

    public Dictionary<string, string> GetAllServiceUrls(string environment = null)
    {
        return _endpoints.ToDictionary(
            x => x.Key,
            x => x.Value.Url,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public string? GetToken()
    {
        return Environment.GetEnvironmentVariable("B2CONNECT_TOKEN");
    }

    public string? GetTenantId()
    {
        return Environment.GetEnvironmentVariable("B2CONNECT_TENANT");
    }

    public IConfigurationSection? GetSection(string sectionName)
    {
        switch (sectionName.ToLower())
        {
            case "ai":
                return new SimpleConfigurationSection(new Dictionary<string, string>
                {
                    ["Enabled"] = "true",
                    ["PreferredProvider"] = "ollama",
                    ["Ollama:Endpoint"] = "http://localhost:11434",
                    ["LMStudio:Endpoint"] = "http://localhost:1234"
                });
            default:
                return null;
        }
    }

    public string? GetValue(string key, string environment = null)
    {
        // Simple key-value lookup - could be extended
        switch (key.ToLower())
        {
            case "environment.current":
                return environment ?? "default";
            default:
                return null;
        }
    }

    public void SetGlobalValue(string key, string value)
    {
        // For now, this is a placeholder
        // Could be extended to support global configuration values
        throw new NotImplementedException("Global configuration setting not yet implemented");
    }

    public void SetEnvironmentValue(string key, string value, string environment)
    {
        // For now, this is a placeholder
        // Could be extended to support environment-specific configuration
        throw new NotImplementedException("Environment-specific configuration setting not yet implemented");
    }

    public async Task SaveAsync()
    {
        // For now, this is a placeholder
        // Could be extended to save configuration changes
        await Task.CompletedTask;
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
