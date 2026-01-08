using B2X.Search.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace B2X.Search.Services;

public class ElasticService : IElasticService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "products";

    public ElasticService(string uri)
    {
        var settings = new ElasticsearchClientSettings(new Uri(uri)).DefaultIndex(IndexName);
        _client = new ElasticsearchClient(settings);
    }

    public async Task EnsureIndexAsync()
    {
        var exists = await _client.Indices.ExistsAsync(IndexName);
        if (!exists.Exists)
        {
            await _client.Indices.CreateAsync(IndexName);
        }
    }

    public async Task<SearchResponseDto> SearchAsync(SearchRequestDto request)
    {
        await EnsureIndexAsync();

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        Query? q;
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query == "*")
        {
            q = new MatchAllQuery();
        }
        else
        {
            q = new MultiMatchQuery
            {
                Query = request.Query,
                Fields = new[] { "title", "description" },
                Fuzziness = new Fuzziness("AUTO")
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Sector))
        {
            q = new BoolQuery
            {
                Must = new List<Query> { q },
                Filter = new List<Query> { new TermQuery { Field = "sector", Value = request.Sector } }
            };
        }

        var resp = await _client.SearchAsync<ProductDocument>(s => s
            .Index(IndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(m => q ?? new MatchAllQuery())
        );

        var result = new SearchResponseDto
        {
            Products = resp.Documents,
            Total = (int)(resp.Total?.Value ?? resp.Documents.Count()),
            Page = page,
            PageSize = pageSize
        };

        return result;
    }

    public async Task<ProductDocument?> GetByIdAsync(string id)
    {
        var resp = await _client.GetAsync<ProductDocument>(id, idx => idx.Index(IndexName));
        return resp.Found ? resp.Source : null;
    }
}
