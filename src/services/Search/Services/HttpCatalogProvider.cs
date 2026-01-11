using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace B2X.Services.Search.Services;

public class HttpCatalogProvider : ICatalogProductProvider
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public HttpCatalogProvider(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = config["Catalog:ApiUrl"]?.TrimEnd('/') ?? string.Empty;
    }

    public async Task<(IEnumerable<dynamic> Items, int Total)> GetPageAsync(int page, int pageSize, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl)) return (new object[0], 0);

        // Expecting API shape { products: [...], total: 123 }
        var url = $"{_baseUrl}/api/products?page={page}&pageSize={pageSize}";
        var resp = await _http.GetAsync(url, cancellationToken);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        var doc = await JsonSerializer.DeserializeAsync<JsonElement>(stream, cancellationToken: ct);
        if (doc.ValueKind == JsonValueKind.Object && doc.TryGetProperty("products", out var products))
        {
            var items = new List<dynamic>();
            foreach (var item in products.EnumerateArray())
            {
                items.Add(item);
            }
            var total = doc.TryGetProperty("total", out var t) && t.TryGetInt32(out var tv) ? tv : items.Count;
            return (items, total);
        }

        return (new object[0], 0);
    }
}
