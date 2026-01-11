using B2X.Shared.Infrastructure.ServiceClients;
using B2X.Store.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Store.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<ICatalogServiceClient> _catalogServiceMock;
    private readonly Mock<ILocalizationServiceClient> _localizationServiceMock;
    private readonly Mock<ISearchServiceClient> _searchServiceMock;
    private readonly Mock<ILogger<ProductsController>> _loggerMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _catalogServiceMock = new Mock<ICatalogServiceClient>();
        _localizationServiceMock = new Mock<ILocalizationServiceClient>();
        _searchServiceMock = new Mock<ISearchServiceClient>();
        _loggerMock = new Mock<ILogger<ProductsController>>();

        _controller = new ProductsController(
            _catalogServiceMock.Object,
            _localizationServiceMock.Object,
            _searchServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SearchProducts_ShouldReturnSearchResponse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new SearchRequestDto(
            Query: "test",
            Page: 1,
            PageSize: 10,
            Locale: "en");
        var expectedResponse = new SearchResponseDto(
            Products: new[] { new ProductDocument(
                Id: "1",
                Title: "Test Product",
                Description: "",
                Sector: "",
                Price: 0,
                Available: true,
                Locale: "en") },
            Total: 1,
            Page: 1,
            PageSize: 10);

        _searchServiceMock
            .Setup(x => x.SearchProductsAsync(request, tenantId, default))
            .ReturnsAsync(expectedResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Request.Headers["X-Tenant-ID"] = tenantId.ToString();
        _controller.HttpContext.Request.Headers["Accept-Language"] = "en";

        // Act
        var result = await _controller.SearchProducts(tenantId, "test", 1, 10, null);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<SearchResponseDto>();
        response.Total.ShouldBe(1);
        response.Products.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task SearchProducts_ShouldUseDefaultValues()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var expectedRequest = new SearchRequestDto(
            Query: "*",
            Page: 1,
            PageSize: 20,
            Locale: "en");

        _searchServiceMock
            .Setup(x => x.SearchProductsAsync(It.Is<SearchRequestDto>(r =>
                r.Query == "*" && r.Page == 1 && r.PageSize == 20 && r.Locale == "en"), tenantId, default))
            .ReturnsAsync(new SearchResponseDto(
                Products: Array.Empty<ProductDocument>(),
                Total: 0,
                Page: 1,
                PageSize: 20));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Request.Headers["X-Tenant-ID"] = tenantId.ToString();
        _controller.HttpContext.Request.Headers["Accept-Language"] = "en";

        // Act
        await _controller.SearchProducts(tenantId);

        // Assert
        _searchServiceMock.Verify(x => x.SearchProductsAsync(
            It.Is<SearchRequestDto>(r => r.Query == "*" && r.Page == 1 && r.PageSize == 20),
            tenantId, default), Times.Once);
    }
}
