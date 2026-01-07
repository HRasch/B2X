namespace B2X.Domain.Search.Services;

public class TenantElasticCredentials
{
    public string Uri { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    /// <summary>
    /// Optional index name to use for this tenant (e.g. "products_tenant123").
    /// If not set, a default index naming scheme will be used.
    /// </summary>
    public string? IndexName { get; set; }
}

public interface ITenantCredentialProvider
{
    /// <summary>
    /// Returns tenant-specific Elasticsearch credentials or null if none configured.
    /// </summary>
    TenantElasticCredentials? GetCredentials(string tenantId);
}
