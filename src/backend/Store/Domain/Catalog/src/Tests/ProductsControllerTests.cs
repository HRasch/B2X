using Xunit;
using Moq;
using B2X.CatalogService.Controllers;
using B2X.CatalogService.Handlers;
using B2X.CatalogService.Models;
using B2X.CatalogService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace B2X.CatalogService.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IProductQueryHandler> _mockQueryHandler;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockQueryHandler = new Mock<IProductQueryHandler>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockProductService.Object, _mockQueryHandler.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetById_WithExistingProduct_ShouldReturn200WithProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productDto = new ProductDto
        {
            Id = productId,
            Sku = "PROD-001",
            Name = "Test Product",
            Price = 99.99m,
            IsAvailable = true
        };

        _mockQueryHandler.Setup(x => x.GetByIdAsync(tenantId, productId, default))
            .ReturnsAsync(productDto);

        // Act
        var result = await _controller.GetById(productId, tenantId);

        // Assert
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        var returnedDto = okResult.Value as ProductDto;
        Assert.NotNull(returnedDto);
        Assert.Equal(productId, returnedDto.Id);
    }

    [Fact]
    public async Task GetById_WithNonExistentProduct_ShouldReturn404()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _mockQueryHandler.Setup(x => x.GetByIdAsync(tenantId, productId, default))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetById(productId, tenantId);

        // Assert
        Assert.NotNull(result);
        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetPaged_ShouldReturn200WithPagedResults()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pagedResult = new PagedResult<ProductDto>
        {
            Items = new List<ProductDto>
            {
                new() { Id = Guid.NewGuid(), Sku = "PROD-001", Name = "Product 1", Price = 10m },
                new() { Id = Guid.NewGuid(), Sku = "PROD-002", Name = "Product 2", Price = 20m }
            },
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 2
        };

        _mockQueryHandler.Setup(x => x.GetPagedAsync(tenantId, 1, 20, default))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetPaged(tenantId, 1, 20);

        // Assert
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        var returnedResult = okResult.Value as PagedResult<ProductDto>;
        Assert.NotNull(returnedResult);
        Assert.Equal(2, returnedResult.Items.Count);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturn201WithCreatedProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-001",
            Name = "New Product",
            Price = 99.99m,
            StockQuantity = 10
        };

        var createdDto = new ProductDto
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku,
            Name = request.Name,
            Price = request.Price,
            IsAvailable = true
        };

        _mockProductService.Setup(x => x.CreateAsync(tenantId, request, default))
            .ReturnsAsync(createdDto);

        // Act
        var result = await _controller.Create(tenantId, request);

        // Assert
        Assert.NotNull(result);
        var createdResult = result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Create_WithNullRequest_ShouldReturn400()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var result = await _controller.Create(tenantId, null!);

        // Assert
        Assert.NotNull(result);
        // Result could be BadRequestResult or BadRequestObjectResult
        var isBadRequest = result is BadRequestResult || result is BadRequestObjectResult;
        Assert.True(isBadRequest, $"Expected BadRequest result but got {result.GetType().Name}");
    }

    [Fact]
    public async Task Update_WithExistingProduct_ShouldReturn200WithUpdatedProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new UpdateProductRequest
        {
            Name = "Updated Product",
            Price = 199.99m
        };

        var updatedDto = new ProductDto
        {
            Id = productId,
            Sku = "PROD-001",
            Name = request.Name,
            Price = request.Price ?? 99.99m,
            IsAvailable = true
        };

        _mockProductService.Setup(x => x.UpdateAsync(tenantId, productId, request, default))
            .ReturnsAsync(updatedDto);

        // Act
        var result = await _controller.Update(productId, tenantId, request);

        // Assert
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Update_WithNonExistentProduct_ShouldReturn404()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new UpdateProductRequest { Name = "Updated" };

        _mockProductService.Setup(x => x.UpdateAsync(tenantId, productId, request, default))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.Update(productId, tenantId, request);

        // Assert
        Assert.NotNull(result);
        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Delete_WithExistingProduct_ShouldReturn204()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _mockProductService.Setup(x => x.DeleteAsync(tenantId, productId, default))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(productId, tenantId);

        // Assert
        Assert.NotNull(result);
        var noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentProduct_ShouldReturn404()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _mockProductService.Setup(x => x.DeleteAsync(tenantId, productId, default))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(productId, tenantId);

        // Assert
        Assert.NotNull(result);
        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Search_WithSearchTerm_ShouldReturn200WithResults()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var searchResults = new PagedResult<ProductDto>
        {
            Items = new List<ProductDto>
            {
                new() { Id = Guid.NewGuid(), Sku = "PROD-001", Name = "Test Product", Price = 99.99m }
            },
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 1
        };

        _mockQueryHandler.Setup(x => x.SearchAsync(tenantId, "test", 1, 20, default))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _controller.Search(tenantId, "test", 1, 20);

        // Assert
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        var returnedResult = okResult.Value as PagedResult<ProductDto>;
        Assert.NotNull(returnedResult);
        Assert.Single(returnedResult.Items);
    }

    [Fact]
    public async Task Search_WithEmptySearchTerm_ShouldReturn400()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var result = await _controller.Search(tenantId, "", 1, 20);

        // Assert
        Assert.NotNull(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
}
