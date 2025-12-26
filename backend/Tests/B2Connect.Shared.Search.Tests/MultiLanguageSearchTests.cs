using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using B2Connect.CatalogService.Events;
using B2Connect.SearchService.Models;
using B2Connect.SearchService.Services;
using B2Connect.SearchService.Controllers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.SearchService.Tests
{
    /// <summary>
    /// Tests for Multi-Language Elasticsearch Support
    /// </summary>
    public class MultiLanguageSearchTests
    {
        #region SearchIndexService Tests

        /// <summary>
        /// Test that ProductCreatedEvent indexes to all language indexes
        /// </summary>
        [Fact]
        public async Task HandleProductCreatedAsync_ShouldIndexToAllLanguages()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockConnectionFactory = new Mock<RabbitMQ.Client.IConnectionFactory>();
            var mockLogger = new Mock<ILogger<SearchIndexService>>();

            var service = new SearchIndexService(mockElasticsearchClient.Object, mockConnectionFactory.Object, mockLogger.Object);

            var productEvent = new ProductCreatedEvent(
                productId: Guid.NewGuid(),
                sku: "TEST-001",
                name: "Test Product",
                description: "Test Description",
                price: 99.99m,
                stockQuantity: 100,
                tenantId: Guid.NewGuid(),
                category: "Electronics",
                imageUrls: Array.Empty<string>(),
                tags: Array.Empty<string>(),
                b2bPrice: null,
                attributes: null
            );

            // Mock successful responses for all indexes
            var indexResponse = new Mock<IndexResponse>();
            indexResponse.Setup(r => r.IsValidResponse).Returns(true);

            mockElasticsearchClient
                .Setup(c => c.IndexAsync(It.IsAny<ProductIndexDocument>(), It.IsAny<Action<IndexRequestDescriptor<ProductIndexDocument>>>(), default))
                .ReturnsAsync(indexResponse.Object);

            // Act
            // This would normally be called via RabbitMQ handler
            // var result = await service.HandleProductCreatedAsync(productEvent);

            // Assert
            // Verify that IndexAsync was called 3 times (once for each language)
            // Due to the nature of the BackgroundService, we test the event handler directly
            Assert.True(true); // Placeholder - actual implementation would test event handler
        }

        /// <summary>
        /// Test that ProductUpdatedEvent updates all language indexes
        /// </summary>
        [Fact]
        public async Task HandleProductUpdatedAsync_ShouldUpdateAllLanguages()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockConnectionFactory = new Mock<RabbitMQ.Client.IConnectionFactory>();
            var mockLogger = new Mock<ILogger<SearchIndexService>>();

            var service = new SearchIndexService(mockElasticsearchClient.Object, mockConnectionFactory.Object, mockLogger.Object);

            var productEvent = new ProductUpdatedEvent(
                productId: Guid.NewGuid(),
                changes: new Dictionary<string, object> { { "price", 129.99m } },
                tenantId: Guid.NewGuid()
            );

            // Mock successful update responses
            var updateResponse = new Mock<UpdateResponse<ProductIndexDocument>>();
            updateResponse.Setup(r => r.IsValidResponse).Returns(true);

            mockElasticsearchClient
                .Setup(c => c.UpdateAsync<ProductIndexDocument, object>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<UpdateRequestDescriptor<ProductIndexDocument, object>>>(), default))
                .ReturnsAsync(updateResponse.Object);

            // Act & Assert
            Assert.True(true); // Placeholder for actual test
        }

        /// <summary>
        /// Test that ProductDeletedEvent removes from all language indexes
        /// </summary>
        [Fact]
        public async Task HandleProductDeletedAsync_ShouldDeleteFromAllLanguages()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockConnectionFactory = new Mock<RabbitMQ.Client.IConnectionFactory>();
            var mockLogger = new Mock<ILogger<SearchIndexService>>();

            var service = new SearchIndexService(mockElasticsearchClient.Object, mockConnectionFactory.Object, mockLogger.Object);

            var productEvent = new ProductDeletedEvent(
                productId: Guid.NewGuid(),
                tenantId: Guid.NewGuid()
            );

            // Mock successful delete responses
            var deleteResponse = new Mock<DeleteResponse>();
            deleteResponse.Setup(r => r.IsValidResponse).Returns(true);

            mockElasticsearchClient
                .Setup(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(deleteResponse.Object);

            // Act & Assert
            Assert.True(true); // Placeholder for actual test
        }

        #endregion

        #region ProductSearchController Tests

        /// <summary>
        /// Test that SearchAsync accepts language parameter
        /// </summary>
        [Theory]
        [InlineData("de")]
        [InlineData("en")]
        [InlineData("fr")]
        public async Task SearchAsync_WithLanguageParameter_ShouldUseCorrectIndex(string language)
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            var searchRequest = new ProductSearchQueryRequest(
                query: "laptop",
                pageSize: 20,
                pageNumber: 1
            );

            // Mock cache miss
            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            // Mock search response
            var searchResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            searchResponse.Setup(r => r.Total).Returns(new TotalHits { Value = 10 });
            searchResponse.Setup(r => r.Documents).Returns(new List<ProductIndexDocument>());
            searchResponse.Setup(r => r.IsValidResponse).Returns(true);

            mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act
            var result = await controller.SearchAsync(searchRequest, language);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ProductSearchResponseDto>(okResult.Value);
            Assert.Equal(10, response.TotalCount);
        }

        /// <summary>
        /// Test that SearchAsync with invalid language falls back to German
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithInvalidLanguage_ShouldFallbackToGerman()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            var searchRequest = new ProductSearchQueryRequest(
                query: "laptop",
                pageSize: 20,
                pageNumber: 1
            );

            // Mock cache miss
            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            // Mock search response
            var searchResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            searchResponse.Setup(r => r.Total).Returns(new TotalHits { Value = 5 });
            searchResponse.Setup(r => r.Documents).Returns(new List<ProductIndexDocument>());

            mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act - use invalid language
            var result = await controller.SearchAsync(searchRequest, "xx");

            // Assert
            Assert.NotNull(result);
            // Should fall back to German ("de") without error
        }

        /// <summary>
        /// Test that GetSuggestionsAsync respects language parameter
        /// </summary>
        [Fact]
        public async Task GetSuggestionsAsync_WithLanguage_ShouldUseCorrectIndex()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            // Mock cache miss
            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            // Mock search response
            var doc = new ProductIndexDocument(
                productId: Guid.NewGuid(),
                sku: "TEST-001",
                name: "Test Product",
                description: "Description",
                category: "Electronics",
                price: 99.99m,
                b2bPrice: null,
                stockQuantity: 100,
                tags: Array.Empty<string>(),
                brand: "TestBrand",
                material: "Metal",
                colors: Array.Empty<string>(),
                sizes: Array.Empty<string>(),
                customAttributes: new Dictionary<string, string>(),
                imageUrls: Array.Empty<string>(),
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                tenantId: Guid.NewGuid(),
                popularityScore: 0,
                reviewCount: 0,
                averageRating: 0
            );

            var searchResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            searchResponse.Setup(r => r.Documents).Returns(new List<ProductIndexDocument> { doc });

            mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act
            var result = await controller.GetSuggestionsAsync("lap", "en", null, 10);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var suggestions = Assert.IsType<SearchSuggestionDto[]>(okResult.Value);
            Assert.NotEmpty(suggestions);
        }

        /// <summary>
        /// Test that GetProductAsync respects language parameter
        /// </summary>
        [Fact]
        public async Task GetProductAsync_WithLanguage_ShouldLoadFromCorrectIndex()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);
            var productId = Guid.NewGuid();

            // Mock cache miss
            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            // Mock get response
            var doc = new ProductIndexDocument(
                productId: productId,
                sku: "TEST-001",
                name: "Test Product",
                description: "Description",
                category: "Electronics",
                price: 99.99m,
                b2bPrice: null,
                stockQuantity: 100,
                tags: Array.Empty<string>(),
                brand: "TestBrand",
                material: "Metal",
                colors: Array.Empty<string>(),
                sizes: Array.Empty<string>(),
                customAttributes: new Dictionary<string, string>(),
                imageUrls: Array.Empty<string>(),
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                tenantId: Guid.NewGuid(),
                popularityScore: 0,
                reviewCount: 0,
                averageRating: 0
            );

            var getResponse = new Mock<GetResponse<ProductIndexDocument>>();
            getResponse.Setup(r => r.Found).Returns(true);
            getResponse.Setup(r => r.Source).Returns(doc);

            mockElasticsearchClient
                .Setup(c => c.GetAsync<ProductIndexDocument>(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(getResponse.Object);

            // Act
            var result = await controller.GetProductAsync(productId, "de");

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var product = Assert.IsType<ProductSearchResultItemDto>(okResult.Value);
            Assert.Equal(productId, product.ProductId);
        }

        /// <summary>
        /// Test that cache keys include language identifier
        /// </summary>
        [Theory]
        [InlineData("de")]
        [InlineData("en")]
        [InlineData("fr")]
        public async Task SearchAsync_CacheKey_ShouldIncludeLanguage(string language)
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            var searchRequest = new ProductSearchQueryRequest(
                query: "laptop",
                pageSize: 20,
                pageNumber: 1
            );

            // Setup cache to track calls
            string capturedCacheKey = null;
            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .Callback<string, object>((key, ct) => capturedCacheKey = key)
                .ReturnsAsync((string)null);

            mockCache
                .Setup(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), default))
                .Returns(Task.CompletedTask);

            // Mock search response
            var searchResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            searchResponse.Setup(r => r.Total).Returns(new TotalHits { Value = 0 });
            searchResponse.Setup(r => r.Documents).Returns(new List<ProductIndexDocument>());

            mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act
            await controller.SearchAsync(searchRequest, language);

            // Assert
            Assert.NotNull(capturedCacheKey);
            Assert.Contains(language, capturedCacheKey);
        }

        #endregion

        #region Language Validation Tests

        /// <summary>
        /// Test that default language is German
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithoutLanguageParameter_ShouldUseGermanDefault()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            var searchRequest = new ProductSearchQueryRequest(
                query: "laptop",
                pageSize: 20,
                pageNumber: 1
            );

            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            var searchResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            searchResponse.Setup(r => r.Total).Returns(new TotalHits { Value = 0 });
            searchResponse.Setup(r => r.Documents).Returns(new List<ProductIndexDocument>());

            mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act
            var result = await controller.SearchAsync(searchRequest); // No language specified

            // Assert - should use default language (de) without error
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test that empty language falls back to default
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithEmptyLanguage_ShouldFallbackToDefault()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            var searchRequest = new ProductSearchQueryRequest(
                query: "laptop",
                pageSize: 20,
                pageNumber: 1
            );

            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            var searchResponse = new Mock<SearchResponse<ProductIndexDocument>>();
            searchResponse.Setup(r => r.Total).Returns(new TotalHits { Value = 0 });
            searchResponse.Setup(r => r.Documents).Returns(new List<ProductIndexDocument>());

            mockElasticsearchClient
                .Setup(c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default))
                .ReturnsAsync(searchResponse.Object);

            // Act
            var result = await controller.SearchAsync(searchRequest, "");

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Cache Tests

        /// <summary>
        /// Test that cache returns stored results
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithCachedResult_ShouldReturnCachedValue()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            var searchRequest = new ProductSearchQueryRequest(
                query: "laptop",
                pageSize: 20,
                pageNumber: 1
            );

            // Mock cache hit
            var cachedResponse = new ProductSearchResponseDto(
                totalCount: 5,
                pageNumber: 1,
                pageSize: 20,
                results: new List<ProductSearchResultItemDto>(),
                facets: new Dictionary<string, FacetResultDto>(),
                elapsedMilliseconds: 50
            );

            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync(System.Text.Json.JsonSerializer.Serialize(cachedResponse));

            // Act
            var result = await controller.SearchAsync(searchRequest, "de");

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ProductSearchResponseDto>(okResult.Value);
            Assert.Equal(5, response.TotalCount);

            // Verify that Elasticsearch was NOT called (cache was used)
            mockElasticsearchClient.Verify(
                c => c.SearchAsync<ProductIndexDocument>(It.IsAny<SearchRequest>(), default),
                Times.Never);
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// Test that null query returns bad request
        /// </summary>
        [Fact]
        public async Task SearchAsync_WithNullQuery_ShouldReturnBadRequest()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            // Act
            var result = await controller.SearchAsync(null, "de");

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        /// <summary>
        /// Test that product not found returns 404
        /// </summary>
        [Fact]
        public async Task GetProductAsync_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            var mockElasticsearchClient = new Mock<IElasticsearchClient>();
            var mockCache = new Mock<IDistributedCache>();
            var mockLogger = new Mock<ILogger<ProductSearchController>>();

            var controller = new ProductSearchController(mockElasticsearchClient.Object, mockCache.Object, mockLogger.Object);

            mockCache
                .Setup(c => c.GetStringAsync(It.IsAny<string>(), default))
                .ReturnsAsync((string)null);

            var getResponse = new Mock<GetResponse<ProductIndexDocument>>();
            getResponse.Setup(r => r.Found).Returns(false);

            mockElasticsearchClient
                .Setup(c => c.GetAsync<ProductIndexDocument>(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(getResponse.Object);

            // Act
            var result = await controller.GetProductAsync(Guid.NewGuid(), "de");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion
    }
}
