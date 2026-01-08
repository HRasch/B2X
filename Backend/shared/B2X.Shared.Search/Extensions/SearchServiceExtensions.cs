using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.Shared.Search.Extensions;

public static class SearchServiceExtensions
{
    /// <summary>
    /// Registriert Elasticsearch Client
    /// </summary>
    public static IServiceCollection AddElasticsearchClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var elasticsearchUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";

        // CA2000: ElasticsearchClientSettings is passed to ElasticsearchClient which takes ownership.
        // The client is registered as Singleton and lives for the application lifetime.
        // Settings does not implement IDisposable, so no dispose is needed.
#pragma warning disable CA2000 // Dispose objects before losing scope
        var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri));
#pragma warning restore CA2000
        var client = new ElasticsearchClient(settings);
        services.AddSingleton(client);

        return services;
    }
}
