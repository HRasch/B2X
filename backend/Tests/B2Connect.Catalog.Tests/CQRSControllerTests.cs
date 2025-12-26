using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wolverine;
using B2Connect.CatalogService.Controllers;
using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Tests.Controllers;

/// <summary>
/// API Controller Integration Tests
/// Tests HTTP endpoints and CQRS command/query invocation
/// </summary>
public class ProductsCommandControllerTests
{
    private readonly Mock<IMessageBus> _mockMessageBus;
    private readonly Mock<ILogger<ProductsCommandController>> _mockLogger;
    private readonly ProductsCommandController _controller;

    public ProductsCommandControllerTests()
    {
        _mockMessageBus = new Mock<IMessageBus>();
        _mockLogger = new Mock<ILogger<ProductsCommandController>>();
        _controller = new ProductsCommandController(_mockMessageBus.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateProduct_WithValidCommand_Returns201Created()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Sku = "TEST-SKU-001",
            Name = "Test Product",
            Price = 99.99m,
            IsAvailable = true
        };

        var expectedId = Guid.NewGuid();
        var successResult = new CommandResult { Success = true, Id = expectedId };

        _mockMessageBus
            .Setup(m => m.InvokeAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.CreateProduct(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProductsQueryController.GetProductById), createdResult.ActionName);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(successResult, createdResult.Value);

        _mockMessageBus.Verify(
            m => m.InvokeAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateProduct_WithValidationException_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Sku = "DUPLICATE-SKU",
            Name = "Test",
            Price = 99.99m
        };

        _mockMessageBus
            .Setup(m => m.InvokeAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("SKU already exists"));

        // Act
        var result = await _controller.CreateProduct(command, CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithValidCommand_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand
        {
            Name = "Updated Name",
            Price = 149.99m
        };

        var successResult = new CommandResult { Success = true };

        _mockMessageBus
            .Setup(m => m.InvokeAsync(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.UpdateProduct(productId, command, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, ((NoContentResult)result).StatusCode);

        _mockMessageBus.Verify(
            m => m.InvokeAsync(It.Is<UpdateProductCommand>(c => c.ProductId == productId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var successResult = new CommandResult { Success = true };

        _mockMessageBus
            .Setup(m => m.InvokeAsync(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _mockMessageBus.Verify(
            m => m.InvokeAsync(It.Is<DeleteProductCommand>(c => c.ProductId == productId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_NotFound_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var notFoundResult = new CommandResult { Success = false, ErrorMessage = "Product not found" };

        _mockMessageBus
            .Setup(m => m.InvokeAsync(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notFoundResult);

        // Act
        var result = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}

public class ProductsQueryControllerTests
{
    private readonly Mock<IMessageBus> _mockMessageBus;
    private readonly Mock<ILogger<ProductsQueryController>> _mockLogger;
    private readonly ProductsQueryController _controller;

    public ProductsQueryControllerTests()
    {
        _mockMessageBus = new Mock<IMessageBus>();
        _mockLogger = new Mock<ILogger<ProductsQueryController>>();
        _controller = new ProductsQueryController(_mockMessageBus.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsOkWithProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new ProductDto
        {
            Id = productId,
            Sku = "TEST-SKU",
            Name = "Test Product",
            Price = 99.99m,
            IsAvailable = true
        };

        _mockMessageBus
            .Setup(m => m.InvokeAsync<ProductDto>(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductById(productId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(product, okResult.Value);

        _mockMessageBus.Verify(
            m => m.InvokeAsync<ProductDto>(It.Is<GetProductByIdQuery>(q => q.ProductId == productId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProductById_NotFound_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockMessageBus
            .Setup(m => m.InvokeAsync<ProductDto>(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.GetProductById(productId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<ProductDto>
        {
            Items = new[]
            {
                new ProductDto { Id = Guid.NewGuid(), Sku = "SKU-001", Name = "Product 1", Price = 99.99m },
                new ProductDto { Id = Guid.NewGuid(), Sku = "SKU-002", Name = "Product 2", Price = 149.99m }
            },
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 2
        };

        _mockMessageBus
            .Setup(m => m.InvokeAsync<PagedResult<ProductDto>>(It.IsAny<GetProductsPagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetProducts(page: 1, pageSize: 20, cancellationToken: CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<PagedResult<ProductDto>>(okResult.Value);
        Assert.Equal(2, returnedResult.Items.Count());
        Assert.Equal(2, returnedResult.TotalCount);
    }

    [Fact]
    public async Task GetProducts_WithFilters_IncludesFiltersInQuery()
    {
        // Arrange
        var pagedResult = new PagedResult<ProductDto>
        {
            Items = Array.Empty<ProductDto>(),
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 0
        };

        _mockMessageBus
            .Setup(m => m.InvokeAsync<PagedResult<ProductDto>>(It.IsAny<GetProductsPagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _controller.GetProducts(
            page: 1,
            pageSize: 20,
            searchTerm: "laptop",
            category: "Electronics",
            minPrice: 100m,
            maxPrice: 2000m,
            cancellationToken: CancellationToken.None);

        // Assert
        _mockMessageBus.Verify(
            m => m.InvokeAsync<PagedResult<ProductDto>>(
                It.Is<GetProductsPagedQuery>(q =>
                    q.SearchTerm == "laptop" &&
                    q.Category == "Electronics" &&
                    q.MinPrice == 100m &&
                    q.MaxPrice == 2000m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchProducts_WithValidTerm_ReturnsResults()
    {
        // Arrange
        var searchResult = new PagedResult<ProductDto>
        {
            Items = new[]
            {
                new ProductDto { Id = Guid.NewGuid(), Name = "Red Laptop", Price = 999.99m }
            },
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 1
        };

        _mockMessageBus
            .Setup(m => m.InvokeAsync<PagedResult<ProductDto>>(It.IsAny<SearchProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResult);

        // Act
        var result = await _controller.SearchProducts("laptop", cancellationToken: CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<PagedResult<ProductDto>>(okResult.Value);
        Assert.Single(returnedResult.Items);
    }

    [Fact]
    public async Task SearchProducts_WithEmptyTerm_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.SearchProducts("", cancellationToken: CancellationToken.None);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task GetCatalogStats_ReturnsStatistics()
    {
        // Arrange
        var stats = new CatalogStats
        {
            TotalProducts = 1000,
            ActiveProducts = 950,
            TotalCategories = 15,
            AveragePrice = 125.50m,
            MinPrice = 10m,
            MaxPrice = 5000m,
            LastUpdated = DateTime.UtcNow
        };

        _mockMessageBus
            .Setup(m => m.InvokeAsync<CatalogStats>(It.IsAny<GetCatalogStatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        // Act
        var result = await _controller.GetCatalogStats(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedStats = Assert.IsType<CatalogStats>(okResult.Value);
        Assert.Equal(1000, returnedStats.TotalProducts);
        Assert.Equal(950, returnedStats.ActiveProducts);
        Assert.Equal(125.50m, returnedStats.AveragePrice);
    }
}
