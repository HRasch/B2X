using System.Collections.Concurrent;
using System.Linq;
using B2X.Domain.Search.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;

namespace B2X.Domain.Search.Services;

public class ElasticService : IElasticService
{
    private const string IndexName = "products";

    private readonly string _defaultUri;
    private readonly string? _defaultUsername;
    private readonly string? _defaultPassword;
    private readonly ITenantCredentialProvider? _tenantCredentialProvider;
    private readonly ConcurrentDictionary<string, ElasticsearchClient> _tenantClients = new(StringComparer.Ordinal);

    public ElasticService(string uri, string? username = null, string? password = null, ITenantCredentialProvider? tenantCredentialProvider = null)
    {
        _defaultUri = uri;
        _defaultUsername = username;
        _defaultPassword = password;
        _tenantCredentialProvider = tenantCredentialProvider;
        _defaultClient = CreateClient(_defaultUri, _defaultUsername, _defaultPassword, IndexName);
    }

    private string GetIndexName(string? tenantId, string? language)
    {
        var lang = string.IsNullOrWhiteSpace(language) ? "en" : language;
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return $"{IndexName}-{lang}";
        }

        if (_tenantCredentialProvider != null)
        {
            var creds = _tenantCredentialProvider.GetCredentials(tenantId);
            if (creds != null && !string.IsNullOrWhiteSpace(creds.IndexName))
            {
                return $"{creds.IndexName}-{lang}";
            }
        }
        // default scheme: base index name + tenant suffix + language
        return $"{IndexName}-{tenantId}-{lang}";
    }

    private readonly ElasticsearchClient _defaultClient;

    private static ElasticsearchClient CreateClient(string uri, string? username, string? password, string defaultIndex)
    {
        // CA2000: ElasticsearchClientSettings is passed to ElasticsearchClient which takes ownership.
        // The client manages the settings lifetime. Settings does not implement IDisposable.
#pragma warning disable CA2000 // Dispose objects before losing scope
        var settings = new ElasticsearchClientSettings(new Uri(uri)).DefaultIndex(defaultIndex);
#pragma warning restore CA2000
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            settings = settings.Authentication(new BasicAuthentication(username, password));
        }
        return new ElasticsearchClient(settings);
    }

    private ElasticsearchClient GetClientForTenant(string? tenantId, string? language)
    {
        var lang = string.IsNullOrWhiteSpace(language) ? "en" : language;
        var key = $"{tenantId ?? "__default"}|{lang}";
        return _tenantClients.GetOrAdd(key, k =>
        {
            if (string.IsNullOrWhiteSpace(tenantId) || _tenantCredentialProvider == null)
            {
                var defaultIndex = GetIndexName(null, lang);
                return CreateClient(_defaultUri, _defaultUsername, _defaultPassword, defaultIndex);
            }

            var creds = _tenantCredentialProvider.GetCredentials(tenantId!);
            if (creds == null)
            {
                var defaultIndex = GetIndexName(tenantId, lang);
                return CreateClient(_defaultUri, _defaultUsername, _defaultPassword, defaultIndex);
            }

            var tenantIndex = !string.IsNullOrWhiteSpace(creds.IndexName) ? $"{creds.IndexName}-{lang}" : GetIndexName(tenantId, lang);
            return CreateClient(creds.Uri, creds.Username, creds.Password, tenantIndex);
        });
    }

    public async Task EnsureIndexAsync(string? tenantId = null, string? language = null)
    {
        var client = GetClientForTenant(tenantId, language);
        var index = GetIndexName(tenantId, language);
        var exists = await client.Indices.ExistsAsync(index).ConfigureAwait(false);
        if (exists.Exists)
        {
            return;
        }

        await client.Indices.CreateAsync(index).ConfigureAwait(false);
    }

    public async Task<SearchResponseDto> SearchAsync(SearchRequestDto request, string? tenantId = null, string? language = null)
    {
        await EnsureIndexAsync(tenantId, language).ConfigureAwait(false);

        var client = GetClientForTenant(tenantId, language);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        // Build query
        Query? query;
        if (string.IsNullOrWhiteSpace(request.Query) || string.Equals(request.Query, "*", StringComparison.Ordinal))
        {
            query = new MatchAllQuery();
        }
        else
        {
            query = new MultiMatchQuery
            {
                Query = request.Query,
                Fields = new[] { "title", "description" },
                Fuzziness = new Fuzziness("AUTO")
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Sector))
        {
            // wrap in bool with filter
            query = new BoolQuery
            {
                Must = new List<Query> { query },
                Filter = new List<Query> { new TermQuery { Field = "sector", Value = request.Sector } }
            };
        }

        var index = GetIndexName(tenantId, language);

        var searchRequest = new SearchRequest(index)
        {
            From = (page - 1) * pageSize,
            Size = pageSize,
            Query = query ?? new MatchAllQuery()
        };

        var resp = await client.SearchAsync<ProductDocument>(searchRequest).ConfigureAwait(false);

        var total = resp.Total;
        return new SearchResponseDto
        {
            Products = resp.Documents,
            Total = (int)(total > 0 ? total : resp.Documents.Count),
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDocument?> GetByIdAsync(string id, string? tenantId = null, string? language = null)
    {
        var client = GetClientForTenant(tenantId, language);
        var index = GetIndexName(tenantId, language);
        var resp = await client.GetAsync<ProductDocument>(id, g => g.Index(index)).ConfigureAwait(false);
        return resp.Found ? resp.Source : null;
    }

    public async Task IndexManyAsync(IEnumerable<ProductDocument> documents, string? tenantId = null, string? language = null)
    {
        var client = GetClientForTenant(tenantId, language);
        var index = GetIndexName(tenantId, language);
        // ensure index exists
        var exists = await client.Indices.ExistsAsync(index).ConfigureAwait(false);
        if (!exists.Exists)
        {
            await client.Indices.CreateAsync(index).ConfigureAwait(false);
        }

        var bulk = await client.BulkAsync(b => b.IndexMany(documents)).ConfigureAwait(false);
        if (bulk.Errors)
        {
            // TODO: handle per-item failures (log or throw)
        }
    }

    public async Task<bool> IsSeededAsync(string tenantId, string? language = null)
    {
        var client = GetClientForTenant(tenantId, language);
        var index = GetIndexName(tenantId, language) + "-meta";
        var resp = await client.GetAsync<dynamic>("seeded", g => g.Index(index)).ConfigureAwait(false);
        return resp.Found;
    }

    public async Task MarkSeededAsync(string tenantId, int count, string? language = null)
    {
        var client = GetClientForTenant(tenantId, language);
        var index = GetIndexName(tenantId, language) + "-meta";
        var doc = new { seededAt = DateTime.UtcNow, count };
        await client.IndexAsync(doc, i => i.Index(index).Id("seeded")).ConfigureAwait(false);
    }
}
