using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using B2Connect.CatalogService.Events;

namespace B2Connect.CatalogService.Services
{
    /// <summary>
    /// Service for publishing domain events to RabbitMQ
    /// Used to notify other services (Search Service, etc.) of product changes
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, string routingKey) where T : DomainEvent;
    }

    /// <summary>
    /// RabbitMQ implementation of event publisher
    /// </summary>
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqEventPublisher> _logger;
        private IConnection _connection;
        private IModel _channel;

        private const string ExchangeName = "product-events";
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 1000;

        public RabbitMqEventPublisher(
            IConnectionFactory connectionFactory,
            ILogger<RabbitMqEventPublisher> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        /// <summary>
        /// Initialize RabbitMQ connection
        /// </summary>
        public void Initialize()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange (idempotent)
                _channel.ExchangeDeclare(
                    exchange: ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                _logger.LogInformation("Event Publisher initialized and connected to RabbitMQ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Event Publisher");
                throw;
            }
        }

        /// <summary>
        /// Publish a domain event to RabbitMQ
        /// </summary>
        public async Task PublishAsync<T>(T @event, string routingKey) where T : DomainEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if (string.IsNullOrWhiteSpace(routingKey))
                throw new ArgumentException("Routing key cannot be empty", nameof(routingKey));

            int retryCount = 0;
            Exception lastException = null;

            while (retryCount < MaxRetries)
            {
                try
                {
                    // Ensure connection is open
                    if (_connection == null || !_connection.IsOpen)
                    {
                        Initialize();
                    }

                    // Serialize event
                    var json = JsonSerializer.Serialize(@event);
                    var body = System.Text.Encoding.UTF8.GetBytes(json);

                    // Create properties with persistence
                    var properties = _channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.ContentType = "application/json";
                    properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    properties.MessageId = @event.EventId.ToString();
                    properties.Headers = new Dictionary<string, object>
                    {
                        { "x-event-type", @event.EventType },
                        { "x-aggregate-id", @event.AggregateId.ToString() },
                        { "x-aggregate-type", @event.AggregateType },
                        { "x-timestamp", @event.Timestamp.ToString("o") }
                    };

                    // Publish message
                    _channel.BasicPublish(
                        exchange: ExchangeName,
                        routingKey: routingKey,
                        basicProperties: properties,
                        body: body);

                    _logger.LogInformation(
                        $"Event published: {routingKey} - {@event.EventType} (Aggregate: {@event.AggregateId})");

                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    retryCount++;

                    _logger.LogWarning(
                        ex,
                        $"Failed to publish event (attempt {retryCount}/{MaxRetries}): {routingKey}");

                    if (retryCount < MaxRetries)
                    {
                        // Exponential backoff
                        await Task.Delay(RetryDelayMs * retryCount);
                    }
                }
            }

            // All retries failed
            _logger.LogError(
                lastException,
                $"Failed to publish event after {MaxRetries} attempts: {routingKey}");

            throw new InvalidOperationException(
                $"Failed to publish event '{routingKey}' after {MaxRetries} attempts",
                lastException);
        }

        /// <summary>
        /// Publish a ProductCreatedEvent
        /// </summary>
        public async Task PublishProductCreatedAsync(ProductCreatedEvent @event)
        {
            await PublishAsync(@event, "product.created");
        }

        /// <summary>
        /// Publish a ProductUpdatedEvent
        /// </summary>
        public async Task PublishProductUpdatedAsync(ProductUpdatedEvent @event)
        {
            await PublishAsync(@event, "product.updated");
        }

        /// <summary>
        /// Publish a ProductDeletedEvent
        /// </summary>
        public async Task PublishProductDeletedAsync(ProductDeletedEvent @event)
        {
            await PublishAsync(@event, "product.deleted");
        }

        /// <summary>
        /// Publish a ProductsBulkImportedEvent
        /// </summary>
        public async Task PublishProductsBulkImportedAsync(ProductsBulkImportedEvent @event)
        {
            await PublishAsync(@event, "products.bulk-imported");
        }

        /// <summary>
        /// Close RabbitMQ connection
        /// </summary>
        public void Close()
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
        }

        public void Dispose()
        {
            Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
