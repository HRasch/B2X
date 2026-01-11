namespace B2X.Search;

public interface ITenantResolver
{
    /// <summary>
    /// Resolve tenant id from a hostname (eg. shop1.example.com -> tenant id string).
    /// Returns null when no mapping is found.
    /// </summary>
    string? ResolveTenantIdFromHost(string host);
}
