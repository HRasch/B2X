using B2Connect.Domain.Search.Models;
using Nest;

namespace B2Connect.Domain.Search.Services;

public class ElasticService : IElasticService
{
    private readonly IElasticClient _client;
    private const string IndexName = "products";

    public ElasticService(string uri, string? username = null, string? password = null)
    {
        var settings = new ConnectionSettings(new Uri(uri)).DefaultIndex(IndexName);
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            settings = settings.BasicAuthentication(username, password);
        }
        _client = new ElasticClient(settings);
    }

    public async Task EnsureIndexAsync()
    {
        var exists = await _client.Indices.ExistsAsync(IndexName);
        if (!exists.Exists)
        {
            await _client.Indices.CreateAsync(IndexName, c => c.Map<ProductDocument>(m => m.AutoMap()));
        }
    }

    public async Task<SearchResponseDto> SearchAsync(SearchRequestDto request)
    {
        await EnsureIndexAsync();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        QueryContainer q;
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query == "*")
        {
            q = new MatchAllQuery();
        }
        else
        {
            q = new MultiMatchQuery
            {
                Fields = Infer.Fields<ProductDocument>(p => p.Title, p => p.Description),
                Query = request.Query,
                Fuzziness = Fuzziness.Auto
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Sector))
        {
            q = q && new TermQuery { Field = "sector", Value = request.Sector };
        }

        var resp = await _client.SearchAsync<ProductDocument>(s => s
            .Query(q)
            .From((page - 1) * pageSize)
            .Size(pageSize)
        );

        return new SearchResponseDto
        {
            Products = resp.Documents,
            Total = (int)resp.Total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDocument?> GetByIdAsync(string id)
    {
        var resp = await _client.GetAsync<ProductDocument>(id, idx => idx.Index(IndexName));
        return resp.Found ? resp.Source : null;
    }
}
