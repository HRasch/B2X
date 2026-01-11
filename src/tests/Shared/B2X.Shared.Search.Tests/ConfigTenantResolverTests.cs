using B2X.Search;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace B2X.Shared.Search.Tests;

public class ConfigTenantResolverTests
{
    [Fact]
    public void ResolveTenant_ReturnsMappedTenant_WhenHostConfigured()
    {
        var inMemory = new Dictionary<string, string?>
(StringComparer.Ordinal)
        {
            ["Tenants:Hosts:shop1.example.com"] = "tenant-1111-1111-1111-111111111111",
            ["Tenants:Hosts:shop2.example.com"] = "tenant-2222-2222-2222-222222222222",
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
        var resolver = new ConfigTenantResolver(config);

        var t1 = resolver.ResolveTenantIdFromHost("shop1.example.com");
        Assert.Equal("tenant-1111-1111-1111-111111111111", t1);

        var t2 = resolver.ResolveTenantIdFromHost("SHOP2.EXAMPLE.COM");
        Assert.Equal("tenant-2222-2222-2222-222222222222", t2);

        var t3 = resolver.ResolveTenantIdFromHost("unknown.example.com");
        Assert.Null(t3);
    }
}
