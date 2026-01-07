using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace B2Connect.Domain.Search.Services;

/// <summary>
/// Simple IConfiguration-backed tenant credential provider.
/// Expects configuration under "Elasticsearch:Tenants" with child sections per-tenant id:
/// "Elasticsearch:Tenants:{tenantId}:Uri", ":Username", ":Password".
/// </summary>
public class ConfigTenantCredentialProvider : ITenantCredentialProvider
{
    private readonly ConcurrentDictionary<string, TenantElasticCredentials> _map = new(StringComparer.Ordinal);

    public ConfigTenantCredentialProvider(IConfiguration config)
    {
        var section = config.GetSection("Elasticsearch:Tenants");
        foreach (var child in section.GetChildren())
        {
            var id = child.Key;
            var creds = new TenantElasticCredentials
            {
                Uri = child["Uri"] ?? string.Empty,
                Username = child["Username"],
                Password = child["Password"],
                IndexName = child["Index"] ?? child["IndexName"] ?? child["IndexPrefix"]
            };

            if (!string.IsNullOrWhiteSpace(creds.Uri))
            {
                _map[id] = creds;
            }
        }
    }

    public TenantElasticCredentials? GetCredentials(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return null;
        }

        return _map.TryGetValue(tenantId, out var creds) ? creds : null;
    }
}
