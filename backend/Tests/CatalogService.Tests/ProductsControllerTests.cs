using B2Connect.CatalogService.Controllers;
using B2Connect.CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.CatalogService.Tests;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IProductQueryHandler> _mockQueryHandler;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;
    private readonly Guid _tenantId = Guid.NewGuid();

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockQueryHandler = new Mock<IProductQueryHandler>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(
            _mockProductService.Object,
            _mockQueryHandler.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetById_WithExistingProduct_ReturnsOkResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new ProductDto
        {
            Id = productId,
            TenantId = _tenantId,
            Sku = "SKU-001",
            Name = "Test Product",
            Price = 99.99m,
            IsAvailable = true
        };

        _mockQueryHandler
            .Setup(x => x.GetByIdAsync(_tenantId, productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(productId, _tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(productId, returnedProduct.Id);
    }

    [Fact]
    public async Task GetById_WithNonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockQueryHandler
            .Setup(x => x.GetByIdAsync(_tenantId, productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetById(productId, _tenantId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetPaged_ReturnsPagedResult()
    {
        // Arrange
        var pagedResult = new PagedResult<ProductDto>
        {
            Items = new()
            {
                new ProductDto { Id = Guid.NewGuid(), Sku = "SKU-001", Name = "Product 1", Price = 10m },
                new ProductDto { Id = Guid.NewGuid(), Sku = "SKU-002", Name = "Product 2", Price = 20m }
            },
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 2
        };

        _mockQueryHandler
            .Setup(x => x.GetPagedAsync(_tenantId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetPaged(_tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPage = Assert.IsType<PagedResult<ProductDto>>(okResult.Value);
        Assert.Equal(2, returnedPage.Items.Count);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Sku = "SKU-001",
            Name = "New Product",
            Price = 99.99m,
            StockQuantity = 10
        };

        var createdProduct = new ProductDto
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = request.Sku,
            Name = request.Name,
            Price = request.Price
        };

        _mockProductService
            .Setup(x => x.CreateAsync(_tenantId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.Create(_tenantId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
        Assert.Equal(createdProduct.Id, ((ProductDto)createdResult.Value!).Id);
    }

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Price = 149.99m
        };

        var updatedProduct = new ProductDto
        {
            Id = productId,
            TenantId = _tenantId,
            Name = "Updated Name",
            Price = 149.99m
        };

        _mockProductService
            .Setup(x => x.UpdateAsync(_tenantId, productId, updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.Update(productId, _tenantId, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal("Updated Name", returnedProduct.Name);
    }

    [Fact]
    public async Task Delete_WithExistingProduct_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockProductService
            .Setup(x => x.DeleteAsync(_tenantId, productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(productId, _tenantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Search_WithSearchTerm_ReturnsResults()
    {
        // Arrange
        var searchResults = new PagedResult<ProductDto>
        {
            Items = new()
            {
                new ProductDto { Id = Guid.NewGuid(), Sku = "PHONE-001", Name = "Smartphone X", Price = 999m }
            },
            PageNumber = 1,
            PageSize = 20,
            TotalCount = 1
        };

        _mockQueryHandler
            .Setup(x => x.SearchAsync(_tenantId, "Smartphone", 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _controller.Search(_tenantId, "Smartphone");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResults = Assert.IsType<PagedResult<ProductDto>>(okResult.Value);
        Assert.Single(returnedResults.Items);
    }

    [Fact]
    public async Task Search_WithEmptySearchTerm_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.Search(_tenantId, "");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
