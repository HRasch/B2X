namespace B2X.AppHost.Services;

/// <summary>
/// Thread-safe context for tracking test data across seeding operations.
/// Maintains relationships between seeded entities for consistent test data.
/// </summary>
public class TestDataContext
{
    private static readonly AsyncLocal<TestDataContext> _current = new();
    private readonly Dictionary<string, Guid> _tenantSlugs = new(StringComparer.Ordinal);
    private readonly Dictionary<Guid, List<Guid>> _tenantUsers = new();
    private readonly Dictionary<Guid, List<Guid>> _tenantProducts = new();
    private readonly Dictionary<Guid, List<Guid>> _tenantPages = new();

    /// <summary>
    /// Gets the current test data context for this async operation.
    /// </summary>
    public static TestDataContext Current
    {
        get
        {
            if (_current.Value == null)
            {
                _current.Value = new TestDataContext();
            }
            return _current.Value;
        }
    }

    /// <summary>
    /// Adds a tenant to the context.
    /// </summary>
    public void AddTenant(Guid tenantId, string slug)
    {
        _tenantSlugs[slug] = tenantId;
        _tenantUsers[tenantId] = new List<Guid>();
        _tenantProducts[tenantId] = new List<Guid>();
        _tenantPages[tenantId] = new List<Guid>();
    }

    /// <summary>
    /// Gets a tenant ID by slug.
    /// </summary>
    public Guid? GetTenantId(string slug)
    {
        return _tenantSlugs.TryGetValue(slug, out var tenantId) ? tenantId : null;
    }

    /// <summary>
    /// Gets all tenant IDs.
    /// </summary>
    public IEnumerable<Guid> GetAllTenantIds()
    {
        return _tenantSlugs.Values;
    }

    /// <summary>
    /// Gets all tenant slugs.
    /// </summary>
    public IEnumerable<string> GetAllTenantSlugs()
    {
        return _tenantSlugs.Keys;
    }

    /// <summary>
    /// Adds a user to a tenant.
    /// </summary>
    public void AddUserToTenant(Guid tenantId, Guid userId)
    {
        if (_tenantUsers.ContainsKey(tenantId))
        {
            _tenantUsers[tenantId].Add(userId);
        }
    }

    /// <summary>
    /// Gets users for a tenant.
    /// </summary>
    public IEnumerable<Guid> GetTenantUsers(Guid tenantId)
    {
        return _tenantUsers.TryGetValue(tenantId, out var users) ? users : Enumerable.Empty<Guid>();
    }

    /// <summary>
    /// Adds a product to a tenant.
    /// </summary>
    public void AddProductToTenant(Guid tenantId, Guid productId)
    {
        if (_tenantProducts.ContainsKey(tenantId))
        {
            _tenantProducts[tenantId].Add(productId);
        }
    }

    /// <summary>
    /// Gets products for a tenant.
    /// </summary>
    public IEnumerable<Guid> GetTenantProducts(Guid tenantId)
    {
        return _tenantProducts.TryGetValue(tenantId, out var products) ? products : Enumerable.Empty<Guid>();
    }

    /// <summary>
    /// Adds a page to a tenant.
    /// </summary>
    public void AddPageToTenant(Guid tenantId, Guid pageId)
    {
        if (_tenantPages.ContainsKey(tenantId))
        {
            _tenantPages[tenantId].Add(pageId);
        }
    }

    /// <summary>
    /// Gets pages for a tenant.
    /// </summary>
    public IEnumerable<Guid> GetTenantPages(Guid tenantId)
    {
        return _tenantPages.TryGetValue(tenantId, out var pages) ? pages : Enumerable.Empty<Guid>();
    }

    /// <summary>
    /// Clears all tenant data.
    /// </summary>
    public void ClearTenants()
    {
        _tenantSlugs.Clear();
        _tenantUsers.Clear();
        _tenantProducts.Clear();
        _tenantPages.Clear();
    }

    /// <summary>
    /// Gets seeding statistics.
    /// </summary>
    public TestDataStatistics GetStatistics()
    {
        return new TestDataStatistics
        {
            TenantCount = _tenantSlugs.Count,
            TotalUsers = _tenantUsers.Values.Sum(users => users.Count),
            TotalProducts = _tenantProducts.Values.Sum(products => products.Count),
            TotalPages = _tenantPages.Values.Sum(pages => pages.Count),
            TenantsWithUsers = _tenantUsers.Count(kvp => kvp.Value.Any()),
            TenantsWithProducts = _tenantProducts.Count(kvp => kvp.Value.Any()),
            TenantsWithPages = _tenantPages.Count(kvp => kvp.Value.Any())
        };
    }
}

/// <summary>
/// Statistics about seeded test data.
/// </summary>
public class TestDataStatistics
{
    /// <summary>
    /// Number of tenants seeded.
    /// </summary>
    public int TenantCount { get; set; }

    /// <summary>
    /// Total number of users across all tenants.
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Total number of products across all tenants.
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Total number of pages across all tenants.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Number of tenants that have users.
    /// </summary>
    public int TenantsWithUsers { get; set; }

    /// <summary>
    /// Number of tenants that have products.
    /// </summary>
    public int TenantsWithProducts { get; set; }

    /// <summary>
    /// Number of tenants that have pages.
    /// </summary>
    public int TenantsWithPages { get; set; }

    public override string ToString()
    {
        return $"{TenantCount} tenants, {TotalUsers} users, {TotalProducts} products, {TotalPages} pages";
    }
}
