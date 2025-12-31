using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace B2Connect.Gateway.Integration.Tests;

public class GatewayIntegrationTests
{
    [Fact]
    public async Task Gateway_ForwardsRequest_To_Catalog()
    {
        // Start a minimal Catalog-like HTTP host on a dynamic port
        using var catalogHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options => options.Listen(System.Net.IPAddress.Loopback, 0));
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/products", async ctx =>
                        {
                            var payload = new { products = new[] { new { id = "1", name = "Dummy Product" } }, total = 1, page = 1, pageSize = 10 };
                            ctx.Response.ContentType = "application/json";
                            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        });

                        endpoints.MapGet("/api/products/{id}", async ctx =>
                        {
                            var id = ctx.Request.RouteValues["id"]?.ToString();
                            var payload = new { id = id, name = $"Dummy {id}", price = 12.34 };
                            ctx.Response.ContentType = "application/json";
                            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        });
                    });
                });
            })
            .Build();

        await catalogHost.StartAsync();

        var addressesFeature = catalogHost.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
        var catalogAddress = addressesFeature.Addresses.First();
        if (!catalogAddress.EndsWith('/')) catalogAddress += '/';

        // Configure and start the Gateway app as a test server, overriding ReverseProxy config
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    var proxyConfig = new Dictionary<string, string?>
                    {
                        // Override the existing 'api-v1-route' defined in appsettings to point to our test cluster
                        ["ReverseProxy:Routes:api-v1-route:ClusterId"] = "test-cluster",
                        ["ReverseProxy:Routes:api-v1-route:Match:Path"] = "/api/v1/{**catch-all}",
                        ["ReverseProxy:Routes:api-v1-route:Transforms:0:PathRemovePrefix"] = "/api/v1",
                        ["ReverseProxy:Routes:api-v2-route:Transforms:1:PathPrefix"] = "/api",

                        // Add a cluster entry that points to our fake catalog
                        ["ReverseProxy:Clusters:test-cluster:Destinations:destination1:Address"] = catalogAddress
                    };

                    conf.AddInMemoryCollection(proxyConfig);
                });
            });

        using var client = factory.CreateClient();

        // Call the gateway route — gateway should accept /api/v1/products and forward to catalog /api/products
        var res = await client.GetAsync("/api/v1/products");
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await res.Content.ReadAsStringAsync();
        content.Should().Contain("Dummy Product");

        // Also verify detail route forwarded
        var res2 = await client.GetAsync("/api/v2/products/1");
        res2.StatusCode.Should().Be(HttpStatusCode.OK);
        var content2 = await res2.Content.ReadAsStringAsync();
        content2.Should().Contain("Dummy 1");

        await catalogHost.StopAsync();
    }
}

    [Fact]
    public async Task Gateway_Forwards_Custom_Headers_To_Catalog()
    {
        using var catalogHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options => options.Listen(System.Net.IPAddress.Loopback, 0));
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/products", async ctx =>
                        {
                            var header = ctx.Request.Headers["X-Test-Header"].FirstOrDefault() ?? "-";
                            var payload = new { header };
                            ctx.Response.ContentType = "application/json";
                            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        });
                    });
                });
            })
            .Build();

        await catalogHost.StartAsync();
        var addressesFeature = catalogHost.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
        var catalogAddress = addressesFeature.Addresses.First();
        if (!catalogAddress.EndsWith('/')) catalogAddress += '/';

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    var proxyConfig = new Dictionary<string, string?>
                    {
                        ["ReverseProxy:Routes:api-v2-route:ClusterId"] = "test-cluster",
                        ["ReverseProxy:Routes:api-v2-route:Match:Path"] = "/api/v2/{**catch-all}",
                        ["ReverseProxy:Routes:api-v2-route:Transforms:0:PathRemovePrefix"] = "/api/v2",
                        ["ReverseProxy:Routes:api-v2-route:Transforms:1:PathPrefix"] = "/api",
                        ["ReverseProxy:Clusters:test-cluster:Destinations:destination1:Address"] = catalogAddress
                    };

                    conf.AddInMemoryCollection(proxyConfig);
                });
            });

        using var client = factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v2/products");
        request.Headers.Add("X-Test-Header", "header-value-123");
        var res = await client.SendAsync(request);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await res.Content.ReadAsStringAsync();
        content.Should().Contain("header-value-123");

        await catalogHost.StopAsync();
    }

    [Fact]
    public async Task Gateway_Propagates_Upstream_Error_As_Server_Error()
    {
        using var catalogHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options => options.Listen(System.Net.IPAddress.Loopback, 0));
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/api/products", async ctx =>
                        {
                            ctx.Response.StatusCode = 500;
                            await ctx.Response.WriteAsync("upstream failure");
                        });
                    });
                });
            })
            .Build();

        await catalogHost.StartAsync();
        var addressesFeature = catalogHost.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
        var catalogAddress = addressesFeature.Addresses.First();
        if (!catalogAddress.EndsWith('/')) catalogAddress += '/';

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    var proxyConfig = new Dictionary<string, string?>
                    {
                        ["ReverseProxy:Routes:api-v2-route:ClusterId"] = "test-cluster",
                        ["ReverseProxy:Routes:api-v2-route:Match:Path"] = "/api/v2/{**catch-all}",
                        ["ReverseProxy:Routes:api-v2-route:Transforms:0:PathRemovePrefix"] = "/api/v2",
                        ["ReverseProxy:Routes:api-v2-route:Transforms:1:PathPrefix"] = "/api",
                        ["ReverseProxy:Clusters:test-cluster:Destinations:destination1:Address"] = catalogAddress
                    };

                    conf.AddInMemoryCollection(proxyConfig);
                });
            });

        using var client = factory.CreateClient();

        var res = await client.GetAsync("/api/v2/products");

        // Gateway should surface an error status; allow either 500 or 502 depending on config
        ((int)res.StatusCode).Should().BeGreaterOrEqualTo(500);

        await catalogHost.StopAsync();
    }
