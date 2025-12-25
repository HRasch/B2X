using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using RabbitMQ.Client;
using B2Connect.CatalogService.Events;
using B2Connect.SearchService.Services;
using B2Connect.SearchService.Models;

namespace B2Connect.Tests.SearchService
{
    /// <summary>
    /// Integration tests for Elasticsearch-based search functionality
    /// </summary>
    public class SearchIndexServiceTests
    {
        private readonly Mock<IElasticsearchClient> _mockElasticsearchClient;
        private readonly Mock<IConnectionFactory> _mockConnectionFactory;
        private readonly Mock<ILogger<SearchIndexService>> _mockLogger;
        private readonly SearchIndexService _searchIndexService;

        public SearchIndexServiceTests()
        {
            _mockElasticsearchClient = new Mock<IElasticsearchClient>();
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockLogger = new Mock<ILogger<SearchIndexService>>();

            _searchIndexService = new SearchIndexService(
                _mockElasticsearchClient.Object,
                _mockConnectionFactory.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleProductCreatedAsync_IndexesProductCorrectly()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var @event = new ProductCreatedEvent(
                productId: productId,
                sku: "JACKET-001",
                name: "Blue Leather Jacket",
                description: "Premium blue leather jacket",
                category: "Clothing",
                price: 199.99m,
                b2bPrice: 149.99m,
                stockQuantity: 50,
                tags: new[] { "leather", "jacket", "blue" },
                attributes: new ProductAttributesDto(
                    Brand: "Premium Brand",
                    Colors: new[] { "blue" },
                    Material: "Leather",
                    Sizes: new[] { "S", "M", "L", "XL" }),
                imageUrls: new[] { "https://example.com/jacket.jpg" },
                tenantId: tenantId);

            var mockResponse = new Mock<IndexResponse>();
            mockResponse.Setup(r => r.IsValidResponse).Returns(true);

            _mockElasticsearchClient
                .Setup(c => c.IndexAsync(
                    It.IsAny<ProductIndexDocument>(),
                    It.IsAny<Action<IndexRequestDescriptor<ProductIndexDocument>>>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            // Act
            await _searchIndexService.HandleProductCreatedAsync(@event);

            // Assert
            _mockElasticsearchClient.Verify(
                c => c.IndexAsync(
                    It.IsAny<ProductIndexDocument>(),
                    It.IsAny<Action<IndexRequestDescriptor<ProductIndexDocument>>>(),
                    default),
                Times.Once);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((s, _) => s.ToString().Contains("Product indexed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleProductUpdatedAsync_UpdatesProductInIndex()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var changes = new Dictionary<string, object>
            {
                { "price", 179.99m },
                { "stockQuantity", 45 }
            };

            var @event = new ProductUpdatedEvent(
                productId: productId,
                changes: changes,
                tenantId: tenantId);

            var mockResponse = new Mock<UpdateResponse<ProductIndexDocument>>();
            mockResponse.Setup(r => r.IsValidResponse).Returns(true);

            _mockElasticsearchClient
                .Setup(c => c.UpdateAsync<ProductIndexDocument, object>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<UpdateRequestDescriptor<ProductIndexDocument, object>>>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            // Act
            await _searchIndexService.HandleProductUpdatedAsync(@event);

            // Assert
            _mockElasticsearchClient.Verify(
                c => c.UpdateAsync<ProductIndexDocument, object>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Action<UpdateRequestDescriptor<ProductIndexDocument, object>>>(),
                    default),
                Times.Once);
        }

        [Fact]
        public async Task HandleProductDeletedAsync_RemovesProductFromIndex()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var @event = new ProductDeletedEvent(productId, tenantId);

            var mockResponse = new Mock<DeleteResponse>();
            mockResponse.Setup(r => r.IsValidResponse).Returns(true);

            _mockElasticsearchClient
                .Setup(c => c.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            // Act
            await _searchIndexService.HandleProductDeletedAsync(@event);

            // Assert
            _mockElasticsearchClient.Verify(
                c => c.DeleteAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    default),
                Times.Once);
        }
    }

    /// <summary>
    /// Tests for product search API endpoints
    /// </summary>
    public class ProductSearchControllerTests
    {
        private readonly Mock<IElasticsearchClient> _mockElasticsearchClient;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ILogger<ProductSearchController>> _mockLogger;

        public ProductSearchControllerTests()
        {
            _mockElasticsearchClient = new Mock<IElasticsearchClient>();
            _mockCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<ProductSearchController>>();
        }

        [Fact]
        public async Task SearchAsync_WithValidQuery_ReturnsResults()
        {
            // Arrange
            var request = new ProductSearchQueryRequest
            {
                Query = "blue jacket",
                PageNumber = 1,
                PageSize = 20,
                TenantId = Guid.NewGuid()
            };

            var testDocuments = new List<ProductIndexDocument>
            {
                new ProductIndexDocument
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Blue Leather Jacket",
                    Price = 199.99m,
                    Category = "Clothing"
                },
                new ProductIndexDocument
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Blue Denim Jacket",
                    Price = 89.99m,
                    Category = "Clothing"
                }
            };

            var mockResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            mockResponse.Setup(r => r.Documents).Returns(testDocuments);
            mockResponse.Setup(r => r.Total).Returns(new CountAggregate { Value = 2 });

            _mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            _mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(
                    It.IsAny<SearchRequest>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            _mockCache
                .Setup(c => c.SetStringAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    default))
                .ReturnsAsync(new ValueTask());

            // Act
            var controller = new ProductSearchController(
                _mockElasticsearchClient.Object,
                _mockCache.Object,
                _mockLogger.Object);

            var result = await controller.SearchAsync(request);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ProductSearchResponseDto>(okResult.Value);
            Assert.Equal(2, response.Results.Count);
            Assert.Equal(2, response.TotalCount);
        }

        [Fact]
        public async Task SearchAsync_CachesResults()
        {
            // Arrange
            var request = new ProductSearchQueryRequest
            {
                Query = "jacket",
                PageNumber = 1,
                PageSize = 20
            };

            // Simulate cached result
            var cachedResponse = new ProductSearchResponseDto
            {
                TotalCount = 10,
                PageNumber = 1,
                PageSize = 20,
                Results = new List<ProductSearchResultItemDto>()
            };

            _mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync(System.Text.Json.JsonSerializer.Serialize(cachedResponse));

            // Act
            var controller = new ProductSearchController(
                _mockElasticsearchClient.Object,
                _mockCache.Object,
                _mockLogger.Object);

            var result = await controller.SearchAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ProductSearchResponseDto>(okResult.Value);
            Assert.Equal(10, response.TotalCount);

            // Verify Elasticsearch was NOT called (cache hit)
            _mockElasticsearchClient.Verify(
                c => c.SearchAsync<ProductIndexDocument>(
                    It.IsAny<SearchRequest>(),
                    default),
                Times.Never);
        }

        [Fact]
        public async Task GetSuggestionsAsync_ReturnsAutocompleteSuggestions()
        {
            // Arrange
            var query = "bl";
            var testDocuments = new List<ProductIndexDocument>
            {
                new ProductIndexDocument { Name = "Blue Jacket" },
                new ProductIndexDocument { Name = "Black Shirt" },
                new ProductIndexDocument { Name = "Blue Jeans" }
            };

            var mockResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            mockResponse.Setup(r => r.Documents).Returns(testDocuments);

            _mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            _mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(
                    It.IsAny<SearchRequest>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

            _mockCache
                .Setup(c => c.SetStringAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    default))
                .ReturnsAsync(new ValueTask());

            // Act
            var controller = new ProductSearchController(
                _mockElasticsearchClient.Object,
                _mockCache.Object,
                _mockLogger.Object);

            var result = await controller.GetSuggestionsAsync(query, limit: 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var suggestions = Assert.IsType<SearchSuggestionDto[]>(okResult.Value);
            Assert.NotEmpty(suggestions);
            Assert.All(suggestions, s => Assert.Equal("product_name", s.Type));
        }
    }

    /// <summary>
    /// Tests for event publishing
    /// </summary>
    public class RabbitMqEventPublisherTests
    {
        [Fact]
        public async Task PublishAsync_PublishesEventSuccessfully()
        {
            // Arrange
            var mockConnectionFactory = new Mock<IConnectionFactory>();
            var mockConnection = new Mock<IConnection>();
            var mockChannel = new Mock<IModel>();
            var mockLogger = new Mock<ILogger<RabbitMqEventPublisher>>();

            mockConnectionFactory
                .Setup(f => f.CreateConnection())
                .Returns(mockConnection.Object);

            mockConnection
                .Setup(c => c.CreateModel())
                .Returns(mockChannel.Object);

            mockConnection.Setup(c => c.IsOpen).Returns(true);

            mockChannel
                .Setup(m => m.CreateBasicProperties())
                .Returns(new Mock<IBasicProperties>().Object);

            var publisher = new RabbitMqEventPublisher(mockConnectionFactory.Object, mockLogger.Object);
            publisher.Initialize();

            var @event = new ProductCreatedEvent(
                productId: Guid.NewGuid(),
                sku: "SKU-001",
                name: "Product",
                description: "Description",
                category: "Category",
                price: 99.99m,
                b2bPrice: null,
                stockQuantity: 10,
                tags: Array.Empty<string>(),
                attributes: new ProductAttributesDto(),
                imageUrls: Array.Empty<string>(),
                tenantId: Guid.NewGuid());

            // Act
            await publisher.PublishProductCreatedAsync(@event);

            // Assert
            mockChannel.Verify(
                m => m.BasicPublish(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IBasicProperties>(),
                    It.IsAny<ReadOnlyMemory<byte>>()),
                Times.Once);
        }
    }
}
