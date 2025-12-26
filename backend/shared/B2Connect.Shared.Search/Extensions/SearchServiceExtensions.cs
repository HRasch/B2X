namespace B2Connect.Shared.Search.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Elastic.Clients.Elasticsearch;

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

        var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri));

        var client = new ElasticsearchClient(settings);
        services.AddSingleton(client);

        return services;
    }
}
