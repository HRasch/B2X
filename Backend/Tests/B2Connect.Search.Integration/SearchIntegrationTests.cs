using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
// Integration tests can run against a running ES instance specified by ES_INTEGRATION_URL
// or start a local Docker Elasticsearch when `USE_TESTCONTAINERS=true`.
using Elastic.Clients.Elasticsearch;

using Xunit;

namespace B2Connect.Search.Integration.Tests;

public class SearchIntegrationTests : IAsyncLifetime
{
    private ElasticsearchClient? _client;
    private string? _dockerContainerId;

    public async Task InitializeAsync()
    {
        var url = Environment.GetEnvironmentVariable("ES_INTEGRATION_URL");
        var useDocker = Environment.GetEnvironmentVariable("USE_TESTCONTAINERS");

        if (string.IsNullOrWhiteSpace(url) && string.Equals(useDocker, "true", StringComparison.OrdinalIgnoreCase))
        {
            // Start Elasticsearch via Docker CLI (simple fallback to Testcontainers behavior)
            var image = "docker.elastic.co/elasticsearch/elasticsearch:8.15.0";
            var args = $"run -d -p 9200:9200 -e discovery.type=single-node -e xpack.security.enabled=false {image}";
            var psi = new ProcessStartInfo("docker", args) { RedirectStandardOutput = true, RedirectStandardError = true };
            var p = Process.Start(psi);
            if (p == null) throw new InvalidOperationException("Failed to start docker process");

            var output = await p.StandardOutput.ReadToEndAsync();
            var err = await p.StandardError.ReadToEndAsync();
            p.WaitForExit();
            if (p.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
                throw new InvalidOperationException($"Failed to start Elasticsearch container: {err}");

            _dockerContainerId = output.Trim();

            // wait for HTTP on localhost:9200
            var http = new HttpClient();
            var ready = false;
            for (var i = 0; i < 60 && !ready; i++)
            {
                try
                {
                    await Task.Delay(1000);
                    var resp = await http.GetAsync("http://localhost:9200/");
                    ready = resp.IsSuccessStatusCode;
                }
                catch { }
            }

            if (!ready)
            {
                try { if (!string.IsNullOrWhiteSpace(_dockerContainerId)) Process.Start("docker", $"rm -f {_dockerContainerId}")?.WaitForExit(); } catch { }
                throw new InvalidOperationException("Elasticsearch did not become ready in time");
            }

            url = "http://localhost:9200";
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
        if (!string.IsNullOrWhiteSpace(_dockerContainerId))
        {
            try
            {
                var p = Process.Start(new ProcessStartInfo("docker", $"rm -f {_dockerContainerId}") { RedirectStandardOutput = true, RedirectStandardError = true });
                if (p != null) await p.WaitForExitAsync();
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
