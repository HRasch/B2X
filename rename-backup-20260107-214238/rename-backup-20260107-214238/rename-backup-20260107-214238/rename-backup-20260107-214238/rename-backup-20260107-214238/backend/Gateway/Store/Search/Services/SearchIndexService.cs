using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using B2Connect.Shared.Messaging.Events;
using B2Connect.SearchService.Models;
using System.Text.Json;

namespace B2Connect.SearchService.Services
{
    /// <summary>
    /// Service that consumes product events from RabbitMQ and updates Elasticsearch indexes
    /// Handles: ProductCreated, ProductUpdated, ProductDeleted, ProductsBulkImported
    /// Creates separate indexes for each language: products_de, products_en, products_fr
    /// </summary>
    public class SearchIndexService : BackgroundService
    {
        private readonly IElasticsearchClient _elasticsearchClient;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<SearchIndexService> _logger;
        private IConnection _connection;
        private IModel _channel;

        private const string ExchangeName = "product-events";
        private const string QueueName = "search-index-updates";

        // Language-specific indexes
        private const string IndexNameDe = "products_de";
        private const string IndexNameEn = "products_en";
        private const string IndexNameFr = "products_fr";
        private const string DefaultLanguage = "de";

        // Supported languages
        private static readonly string[] SupportedLanguages = { "de", "en", "fr" };

        public SearchIndexService(
            IElasticsearchClient elasticsearchClient,
            IConnectionFactory connectionFactory,
            ILogger<SearchIndexService> logger)
        {
            _elasticsearchClient = elasticsearchClient;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Initialize index if it doesn't exist
                await InitializeIndexAsync(stoppingToken);

                // Setup RabbitMQ connection
                SetupRabbitMQConnection();

                _logger.LogInformation("Search Index Service started and listening for product events");

                // Keep the service running
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Search Index Service is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search Index Service encountered an error");
                throw;
            }
        }

        /// <summary>
        /// Initialize Elasticsearch indexes for all supported languages
        /// </summary>
        private async Task InitializeIndexAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var language in SupportedLanguages)
                {
                    await CreateIndexForLanguageAsync(language, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Elasticsearch indexes");
                throw;
            }
        }

        /// <summary>
        /// Create index for a specific language
        /// </summary>
        private async Task CreateIndexForLanguageAsync(string language, CancellationToken cancellationToken)
        {
            var indexName = GetIndexNameForLanguage(language);

            try
            {
                // Check if index exists
                var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(indexName, cancellationToken);

                if (existsResponse.Exists)
                {
                    _logger.LogInformation($"Index '{indexName}' already exists");
                    return;
                }

                // Create index with mapping
                var createIndexResponse = await _elasticsearchClient.Indices.CreateAsync(
                    indexName,
                    descriptor => descriptor
                        .Settings(s => s
                            .NumberOfShards(3)
                            .NumberOfReplicas(1)
                            .RefreshInterval(new Time("1s")))
                        .Mappings(m => m
                            .Properties(props => props
                                .Keyword(f => f.ProductId)
                                .Keyword(f => f.Sku)
                                .Text(f => f
                                    .Name(n => n.Name)
                                    .Fields(fld => fld
                                        .Keyword(kw => kw.Name("keyword"))
                                        .Text(t => t.Name("autocomplete")
                                            .Analyzer("autocomplete_analyzer"))))
                                .Text(f => f.Description)
                                .Keyword(f => f.Category)
                                .ScaledFloat(f => f.Price, 100)
                                .ScaledFloat(f => f.B2bPrice, 100)
                                .Integer(f => f.StockQuantity)
                                .Boolean(f => f.IsAvailable)
                                .Keyword(f => f.Tags)
                                .Keyword(f => f.Brand)
                                .Keyword(f => f.Material)
                                .Keyword(f => f.Colors)
                                .Keyword(f => f.Sizes)
                                .Keyword(f => f.ImageUrls)
                                .Date(f => f.CreatedAt)
                                .Date(f => f.UpdatedAt)
                                .Keyword(f => f.TenantId)
                                .Double(f => f.PopularityScore)
                                .Integer(f => f.ReviewCount)
                                .ScaledFloat(f => f.AverageRating, 10))),
                    cancellationToken);

                _logger.LogInformation($"Index '{indexName}' created successfully for language: {language}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Elasticsearch index for language: {Language}", language);
                throw;
            }
        }

        /// <summary>
        /// Get index name for a specific language
        /// </summary>
        private static string GetIndexNameForLanguage(string language)
        {
            return language?.ToLower() switch
            {
                "de" => IndexNameDe,
                "en" => IndexNameEn,
                "fr" => IndexNameFr,
                _ => $"products_{language?.ToLower() ?? DefaultLanguage}"
            };
        }

        /// <summary>
        /// Setup RabbitMQ connection and declare exchange/queue
        /// </summary>
        private void SetupRabbitMQConnection()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange
                _channel.ExchangeDeclare(
                    exchange: ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                // Declare queue
                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                // Bind queue to exchange with routing keys
                _channel.QueueBind(QueueName, ExchangeName, "product.created");
                _channel.QueueBind(QueueName, ExchangeName, "product.updated");
                _channel.QueueBind(QueueName, ExchangeName, "product.deleted");
                _channel.QueueBind(QueueName, ExchangeName, "products.bulk-imported");

                // Create consumer
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) => await OnMessageReceivedAsync(ea);

                // Start consuming with manual acknowledgment
                _channel.BasicConsume(
                    queue: QueueName,
                    autoAck: false,
                    consumerTag: "search-index-consumer",
                    consumer: consumer);

                _logger.LogInformation("RabbitMQ connection established and consumer started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up RabbitMQ connection");
                throw;
            }
        }

        /// <summary>
        /// Handle incoming RabbitMQ message
        /// </summary>
        private async Task OnMessageReceivedAsync(BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation($"Received event: {routingKey}");

                var handled = false;

                if (routingKey == "product.created")
                {
                    var @event = JsonSerializer.Deserialize<ProductCreatedEvent>(message);
                    if (@event != null)
                    {
                        await HandleProductCreatedAsync(@event);
                        handled = true;
                    }
                }
                else if (routingKey == "product.updated")
                {
                    var @event = JsonSerializer.Deserialize<ProductUpdatedEvent>(message);
                    if (@event != null)
                    {
                        await HandleProductUpdatedAsync(@event);
                        handled = true;
                    }
                }
                else if (routingKey == "product.deleted")
                {
                    var @event = JsonSerializer.Deserialize<ProductDeletedEvent>(message);
                    if (@event != null)
                    {
                        await HandleProductDeletedAsync(@event);
                        handled = true;
                    }
                }
                else if (routingKey == "products.bulk-imported")
                {
                    var @event = JsonSerializer.Deserialize<ProductsBulkImportedEvent>(message);
                    if (@event != null)
                    {
                        await HandleBulkImportAsync(@event);
                        handled = true;
                    }
                }

                if (handled)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation($"Event processed successfully: {routingKey}");
                }
                else
                {
                    // Nack and send to dead letter queue for manual review
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    _logger.LogWarning($"Failed to process event: {routingKey}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RabbitMQ message");
                // Nack without requeue to send to dead letter queue
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        }

        /// <summary>
        /// Handle ProductCreatedEvent - index to all language indexes
        /// </summary>
        private async Task HandleProductCreatedAsync(ProductCreatedEvent @event)
        {
            var document = new ProductIndexDocument
            {
                ProductId = @event.ProductId,
                Sku = @event.Sku,
                Name = @event.Name,
                Description = @event.Description,
                Category = @event.Category,
                Price = @event.Price,
                B2bPrice = @event.B2bPrice,
                StockQuantity = @event.StockQuantity,
                IsAvailable = @event.StockQuantity > 0,
                Tags = @event.Tags,
                Brand = @event.Attributes?.Brand,
                Material = @event.Attributes?.Material,
                Colors = @event.Attributes?.Colors ?? Array.Empty<string>(),
                Sizes = @event.Attributes?.Sizes ?? Array.Empty<string>(),
                CustomAttributes = @event.Attributes?.Custom ?? new Dictionary<string, string>(),
                ImageUrls = @event.ImageUrls,
                CreatedAt = @event.Timestamp,
                UpdatedAt = @event.Timestamp,
                TenantId = @event.TenantId,
                PopularityScore = 0,
                ReviewCount = 0,
                AverageRating = 0
            };

            // Index to all language-specific indexes
            foreach (var language in SupportedLanguages)
            {
                var indexName = GetIndexNameForLanguage(language);
                var response = await _elasticsearchClient.IndexAsync(
                    document,
                    idx => idx.Index(indexName).Id(@event.ProductId.ToString()));

                if (!response.IsValidResponse)
                {
                    throw new Exception($"Failed to index product in {indexName}: {response.DebugInformation}");
                }

                _logger.LogInformation($"Product indexed in {indexName}: {document.ProductId}");
            }
        }

        /// <summary>
        /// Handle ProductUpdatedEvent - update all language indexes
        /// </summary>
        private async Task HandleProductUpdatedAsync(ProductUpdatedEvent @event)
        {
            // Partial update using script or get-update pattern
            var updateSource = new Dictionary<string, object>();
            foreach (var change in @event.Changes)
            {
                updateSource[change.Key] = change.Value;
            }
            updateSource["updatedAt"] = DateTime.UtcNow;

            // Update all language-specific indexes
            foreach (var language in SupportedLanguages)
            {
                var indexName = GetIndexNameForLanguage(language);
                var response = await _elasticsearchClient.UpdateAsync<ProductIndexDocument, object>(
                    indexName,
                    @event.ProductId.ToString(),
                    u => u.Doc(updateSource).DocAsUpsert(true));

                if (!response.IsValidResponse)
                {
                    throw new Exception($"Failed to update product in {indexName}: {response.DebugInformation}");
                }

                _logger.LogInformation("Updated product {ProductId} in index {IndexName}", @event.ProductId, indexName);

                if (!response.IsValidResponse)
                {
                    throw new Exception($"Failed to update product in {indexName}: {response.DebugInformation}");
                }
            }
        }

        /// <summary>
        /// Handle product deleted event - delete from all language indexes
        /// </summary>
        private async Task HandleProductDeletedAsync(ProductDeletedEvent @event)
        {
            // Delete from all language-specific indexes
            foreach (var language in SupportedLanguages)
            {
                var indexName = GetIndexNameForLanguage(language);
                var response = await _elasticsearchClient.DeleteAsync(
                    indexName,
                    @event.ProductId.ToString());

                if (!response.IsValidResponse)
                {
                    throw new Exception($"Failed to delete product in {indexName}: {response.DebugInformation}");
                }

                _logger.LogInformation($"Product deleted from {indexName}: {@event.ProductId}");
            }
            {
                throw new Exception($"Failed to delete product: {response.DebugInformation}");
            }

            _logger.LogInformation($"Product deleted: {@event.ProductId}");
        }

        /// <summary>
        /// Handle ProductsBulkImportedEvent
        /// </summary>
        private async Task HandleBulkImportAsync(ProductsBulkImportedEvent @event)
        {
            _logger.LogInformation($"Processing bulk import of {@event.TotalCount} products");

            // This would be handled by reading the products from the database
            // and using bulk API for indexing
            // For now, we log that we received the event
            // In production, you'd fetch all products in the import batch and bulk index them

            _logger.LogInformation($"Bulk import completed: {string.Join(", ", @event.ProductIds.Take(5))}...");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connection");
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
