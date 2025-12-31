using System;
using System.Net.Http;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace B2Connect.Search.Integration.Tests;

public class SearchIntegrationTests : IAsyncLifetime
{
    private ElasticsearchClient? _client;
    private DotNet.Testcontainers.Containers.IContainer? _esContainer;

    public async Task InitializeAsync()
    {
        var url = Environment.GetEnvironmentVariable("ES_INTEGRATION_URL");
        var useTestcontainers = Environment.GetEnvironmentVariable("USE_TESTCONTAINERS");

        if (string.IsNullOrWhiteSpace(url) && string.Equals(useTestcontainers, "true", StringComparison.OrdinalIgnoreCase))
        {
            var builder = new DotNet.Testcontainers.Builders.ContainerBuilder()
                .WithImage("docker.elastic.co/elasticsearch/elasticsearch:8.15.0")
                .WithEnvironment("discovery.type", "single-node")
                .WithEnvironment("xpack.security.enabled", "false")
                .WithEnvironment("ES_JAVA_OPTS", "-Xms256m -Xmx256m")
                .WithExposedPort(9200)
                .WithPortBinding(9200, true)
                .WithCleanUp(true);

            _esContainer = builder.Build();
            await _esContainer.StartAsync();

            var mappedPort = _esContainer.GetMappedPublicPort(9200);
            url = $"http://localhost:{mappedPort}";
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            _client = null; // treat as skipped
            return;
        }

        var uri = new Uri(url);
        var settings = new ElasticsearchClientSettings(uri).DefaultIndex("products-en");
        _client = new ElasticsearchClient(settings);

        var exists = await _client.Indices.ExistsAsync("products-en");
        if (!exists.Exists) await _client.Indices.CreateAsync("products-en");
    }

    public async Task DisposeAsync()
    {
        if (_esContainer != null)
        {
            try
            {
                await _esContainer.StopAsync();
            }
            catch { }
        }
    }

    [Fact]
    public async Task IndexAndSearch_Product_ReturnsResult()
    {
        if (_client == null) return; // no integration endpoint configured; treat as skipped

        var doc = new { id = "p1", title = "Integration test product", description = "Test search" };
        await _client.IndexAsync(doc, i => i.Index("products-en").Id("p1"));
        await _client.Indices.RefreshAsync("products-en");

        var resp = await _client.SearchAsync<dynamic>(s => s.Indices("products-en").Query(q => q.MatchAll()));
        Assert.True(resp.Total > 0, "expected at least one document in index");
    }
}
