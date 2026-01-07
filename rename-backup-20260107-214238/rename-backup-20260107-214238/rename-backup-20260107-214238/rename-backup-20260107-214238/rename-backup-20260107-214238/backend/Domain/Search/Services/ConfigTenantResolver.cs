using Microsoft.Extensions.Configuration;

namespace B2Connect.Domain.Search.Services;

/// <summary>
/// Configuration-backed tenant resolver. Expects configuration section:
/// Tenants:Hosts:
///   shop1.example.com: tenant-1
///   shop2.example.com: tenant-2
/// </summary>
public class ConfigTenantResolver : ITenantResolver
{
    private readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase);

    public ConfigTenantResolver(IConfiguration config)
    {
        var section = config.GetSection("Tenants:Hosts");
        foreach (var child in section.GetChildren())
        {
            var host = child.Key?.Trim().ToLowerInvariant();
            var tenant = child.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(tenant))
            {
                _map[host] = tenant!;
            }
        }
    }

    public string? ResolveTenantIdFromHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        var h = host.Split(':')[0].Trim().ToLowerInvariant();
        if (!_map.TryGetValue(h, out var tenant))
        {
            return null;
        }

        return tenant;
    }
}
