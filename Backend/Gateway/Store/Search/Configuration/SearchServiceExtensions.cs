using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;
using B2Connect.SearchService.Services;

namespace B2Connect.SearchService.Configuration
{
    /// <summary>
    /// Extension methods for configuring Elasticsearch, RabbitMQ, and Redis
    /// </summary>
    public static class SearchServiceExtensions
    {
        /// <summary>
        /// Add Elasticsearch client and search services
        /// </summary>
        public static IServiceCollection AddSearchServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure Elasticsearch client
            var elasticsearchSettings = configuration.GetSection("Elasticsearch");
            var nodes = elasticsearchSettings.GetSection("Nodes").Get<string[]>()
                ?? new[] { "http://localhost:9200" };
            var username = elasticsearchSettings["Username"];
            var password = elasticsearchSettings["Password"];

            var settings = new ElasticsearchClientSettings(new Uri(nodes[0]));

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                settings = settings.Authentication(new BasicAuthentication(username, password));
            }

            settings = settings
                .DefaultIndex("products")
                .EnableDebugMode()
                .DisableDirectStreaming()
                .OnRequestCompleted(details =>
                {
                    // Log Elasticsearch requests for debugging
                    if (details.RequestBodyInBytes != null)
                    {
                        var body = System.Text.Encoding.UTF8.GetString(details.RequestBodyInBytes);
                    }
                });

            var client = new ElasticsearchClient(settings);
            services.AddSingleton<IElasticsearchClient>(client);

            // Configure RabbitMQ connection factory
            var rabbitSettings = configuration.GetSection("RabbitMQ");
            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitSettings["HostName"] ?? "localhost",
                Port = int.Parse(rabbitSettings["Port"] ?? "5672"),
                UserName = rabbitSettings["Username"] ?? "guest",
                Password = rabbitSettings["Password"] ?? "guest",
                VirtualHost = rabbitSettings["VirtualHost"] ?? "/",
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            services.AddSingleton<IConnectionFactory>(connectionFactory);

            // Configure Redis cache
            var redisSettings = configuration.GetSection("Redis");
            var redisConnection = redisSettings["Connection"] ?? "localhost:6379";
            var redis = ConnectionMultiplexer.Connect(redisConnection);
            services.AddSingleton(redis);
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(redis);
            });

            // Register background service for consuming events
            services.AddHostedService<SearchIndexService>();

            // Register search service
            services.AddScoped<ISearchService, ElasticsearchSearchService>();

            return services;
        }
    }

    /// <summary>
    /// Interface for search service abstraction
    /// </summary>
    public interface ISearchService
    {
        Task<ProductSearchResponseDto> SearchAsync(ProductSearchQueryRequest request);
        Task<ProductSearchResultItemDto> GetProductAsync(Guid id);
        Task<SearchSuggestionDto[]> GetSuggestionsAsync(string query, Guid? tenantId = null, int limit = 10);
    }

    /// <summary>
    /// Elasticsearch implementation of search service
    /// </summary>
    public class ElasticsearchSearchService : ISearchService
    {
        private readonly IElasticsearchClient _client;
        private readonly IDistributedCache _cache;

        public ElasticsearchSearchService(
            IElasticsearchClient client,
            IDistributedCache cache)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<ProductSearchResponseDto> SearchAsync(ProductSearchQueryRequest request)
        {
            // Implementation would be similar to ProductSearchController
            throw new NotImplementedException();
        }

        public async Task<ProductSearchResultItemDto> GetProductAsync(Guid id)
        {
            // Implementation to get single product
            throw new NotImplementedException();
        }

        public async Task<SearchSuggestionDto[]> GetSuggestionsAsync(string query, Guid? tenantId = null, int limit = 10)
        {
            // Implementation to get suggestions
            throw new NotImplementedException();
        }
    }
}
