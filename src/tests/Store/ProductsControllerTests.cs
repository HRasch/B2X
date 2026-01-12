using B2X.Catalog.Application.Commands;
using B2X.Catalog.Models;
using B2X.Store.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Wolverine;
using Xunit;

namespace B2X.Store.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<ILogger<ProductsController>> _loggerMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _messageBusMock = new Mock<IMessageBus>();
        _loggerMock = new Mock<ILogger<ProductsController>>();

        _controller = new ProductsController(
            _messageBusMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProducts()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = 1;
        var pageSize = 10;
        var expectedProducts = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Test Product", Sku = "TEST-001", Price = 99.99m }
        };

        _messageBusMock
            .Setup(x => x.InvokeAsync<IEnumerable<Product>>(It.Is<GetProductsByTenantQuery>(q =>
                q.TenantId == tenantId && q.Page == page && q.PageSize == pageSize), CancellationToken.None))
            .ReturnsAsync(expectedProducts);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Request.Headers["X-Tenant-ID"] = tenantId.ToString();

        // Act
        var result = await _controller.GetProducts(tenantId, page, pageSize);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var products = okResult.Value.ShouldBeOfType<List<Product>>();
        products.ShouldHaveSingleItem();
        products[0].Name.ShouldBe("Test Product");
    }

    [Fact]
    public async Task GetProducts_ShouldUseDefaultValues()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var expectedProducts = new List<Product>();

        _messageBusMock
            .Setup(x => x.InvokeAsync<IEnumerable<Product>>(It.Is<GetProductsByTenantQuery>(q =>
                q.TenantId == tenantId && q.Page == 1 && q.PageSize == 50), CancellationToken.None))
            .ReturnsAsync(expectedProducts);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Request.Headers["X-Tenant-ID"] = tenantId.ToString();

        // Act
        var result = await _controller.GetProducts(tenantId);

        // Assert
        _messageBusMock.Verify(x => x.InvokeAsync<IEnumerable<Product>>(
            It.Is<GetProductsByTenantQuery>(q => q.TenantId == tenantId && q.Page == 1 && q.PageSize == 50),
            CancellationToken.None), Times.Once);
    }
}
