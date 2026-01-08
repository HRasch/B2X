using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using B2X.LayoutService.Controllers;
using B2X.LayoutService.Models;
using B2X.LayoutService.Data;
using B2X.LayoutService.Services;

namespace B2X.LayoutService.Tests;

/// <summary>
/// Controller Tests für Layout Service - RESTful API
/// TDD: Tests für HTTP endpoints und responses
/// </summary>
public class LayoutControllerTests
{
    private readonly Mock<ILayoutService> _mockService;
    private readonly LayoutController _controller;
    private readonly Guid _tenantId = Guid.NewGuid();

    public LayoutControllerTests()
    {
        _mockService = new Mock<ILayoutService>();
        _controller = new LayoutController(_mockService.Object);
        // Simulate tenant context (in real app, from header or claims)
        _controller.TenantId = _tenantId;
    }

    #region Create Page Endpoint Tests

    [Fact]
    public async Task CreatePage_WithValidRequest_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var request = new CreatePageRequest
        {
            Title = "New Page",
            Slug = "new-page",
            Description = "Test page"
        };
        var createdPageDto = new CmsPageDto
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Title = request.Title,
            Slug = request.Slug,
            Language = "en",
            Version = 1
        };

        _mockService
            .Setup(s => s.CreatePageAsync(_tenantId, request))
            .ReturnsAsync(createdPageDto);

        // Act
        var result = await _controller.CreatePage(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(nameof(_controller.GetPage), createdResult.RouteName);
        Assert.Equal(createdPageDto.Id, ((CmsPageDto)createdResult.Value!).Id);
    }

    [Fact]
    public async Task CreatePage_WithNullTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreatePageRequest
        {
            Title = null!,
            Slug = "new-page"
        };

        _mockService
            .Setup(s => s.CreatePageAsync(_tenantId, request))
            .ThrowsAsync(new ArgumentException("Title is required"));

        // Act
        var result = await _controller.CreatePage(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    #endregion

    #region Get Page Endpoint Tests

    [Fact]
    public async Task GetPage_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var page = new CmsPage
        {
            Id = pageId,
            TenantId = _tenantId,
            Title = "Test Page",
            Slug = "test-page",
            Version = 1
        };

        _mockService
            .Setup(s => s.GetPageByIdAsync(_tenantId, pageId, It.IsAny<string>()))
            .ReturnsAsync(new CmsPageDto { Id = pageId, Title = "Test", Slug = "test", Language = "en" });

        // Act
        var result = await _controller.GetPage(pageId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(page.Id, ((CmsPage)okResult.Value!).Id);
    }

    [Fact]
    public async Task GetPage_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var pageId = Guid.NewGuid();

        _mockService
            .Setup(s => s.GetPageByIdAsync(_tenantId, pageId, It.IsAny<string>()))
            .ReturnsAsync((CmsPageDto?)null);

        // Act
        var result = await _controller.GetPage(pageId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    #endregion

    #region Get All Pages Endpoint Tests

    [Fact]
    public async Task GetPages_ShouldReturnListOfPages()
    {
        // Arrange
        var pages = new List<CmsPageDto>
        {
            new CmsPageDto { Id = Guid.NewGuid(), TenantId = _tenantId, Title = "Page 1", Slug = "page-1", Language = "en" },
            new CmsPageDto { Id = Guid.NewGuid(), TenantId = _tenantId, Title = "Page 2", Slug = "page-2", Language = "en" }
        };

        _mockService
            .Setup(s => s.GetPagesByTenantAsync(_tenantId, It.IsAny<string>()))
            .ReturnsAsync(pages);

        // Act
        var result = await _controller.GetPages();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPages = Assert.IsType<List<CmsPage>>(okResult.Value);
        Assert.Equal(2, returnedPages.Count);
    }

    [Fact]
    public async Task GetPages_WithNoPages_ShouldReturnEmptyList()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetPagesByTenantAsync(_tenantId, It.IsAny<string>()))
            .ReturnsAsync(new List<CmsPageDto>());

        // Act
        var result = await _controller.GetPages();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPages = Assert.IsType<List<CmsPage>>(okResult.Value);
        Assert.Empty(returnedPages);
    }

    #endregion

    #region Update Page Endpoint Tests

    [Fact]
    public async Task UpdatePage_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var request = new UpdatePageRequest
        {
            Title = "Updated Title",
            Slug = "updated-slug"
        };
        var updatedPage = new CmsPage
        {
            Id = pageId,
            TenantId = _tenantId,
            Title = request.Title,
            Slug = request.Slug,
            Version = 2
        };

        _mockService
            .Setup(s => s.UpdatePageAsync(_tenantId, pageId, request, It.IsAny<string>()))
            .ReturnsAsync(new CmsPageDto { Id = pageId, Title = "Updated", Slug = "updated", Language = "en", Version = 2 });

        // Act
        var result = await _controller.UpdatePage(pageId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(2, ((CmsPage)okResult.Value!).Version);
    }

    [Fact]
    public async Task UpdatePage_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var request = new UpdatePageRequest { Title = "Updated" };

        _mockService
            .Setup(s => s.UpdatePageAsync(_tenantId, pageId, request, It.IsAny<string>()))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.UpdatePage(pageId, request);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Delete Page Endpoint Tests

    [Fact]
    public async Task DeletePage_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var pageId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeletePageAsync(_tenantId, pageId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeletePage(pageId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeletePageAsync(_tenantId, pageId), Times.Once);
    }

    [Fact]
    public async Task DeletePage_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var pageId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeletePageAsync(_tenantId, pageId))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.DeletePage(pageId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Section Management Endpoint Tests

    [Fact]
    public async Task AddSection_WithValidData_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var request = new AddSectionRequest { Type = "Hero", Layout = SectionLayout.FullWidth };
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = pageId,
            Type = request.Type,
            Layout = request.Layout,
            Order = 0
        };

        _mockService
            .Setup(s => s.AddSectionAsync(_tenantId, pageId, request))
            .ReturnsAsync(section);

        // Act
        var result = await _controller.AddSection(pageId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(section.Id, ((CmsSection)createdResult.Value!).Id);
    }

    [Fact]
    public async Task RemoveSection_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();

        _mockService
            .Setup(s => s.RemoveSectionAsync(_tenantId, pageId, sectionId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.RemoveSection(pageId, sectionId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion

    #region Component Management Endpoint Tests

    [Fact]
    public async Task AddComponent_WithValidData_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var request = new AddComponentRequest { Type = "Button", Content = "Click Me" };
        var component = new CmsComponent
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Type = request.Type,
            Content = request.Content,
            IsVisible = true,
            Order = 0
        };

        _mockService
            .Setup(s => s.AddComponentAsync(_tenantId, pageId, sectionId, request, It.IsAny<string>()))
            .ReturnsAsync(new CmsComponentDto { Id = Guid.NewGuid(), Type = "text", Content = "test", Language = "en", SectionId = sectionId });

        // Act
        var result = await _controller.AddComponent(pageId, sectionId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(component.Id, ((CmsComponent)createdResult.Value!).Id);
    }

    [Fact]
    public async Task UpdateComponent_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var request = new UpdateComponentRequest { Content = "Updated Content" };
        var component = new CmsComponent
        {
            Id = componentId,
            SectionId = sectionId,
            Type = "Button",
            Content = request.Content,
            IsVisible = true
        };

        _mockService
            .Setup(s => s.UpdateComponentAsync(_tenantId, pageId, sectionId, componentId, request, It.IsAny<string>()))
            .ReturnsAsync(new CmsComponentDto { Id = componentId, Type = "text", Content = request.Content ?? "test", Language = "en", SectionId = sectionId });

        // Act
        var result = await _controller.UpdateComponent(pageId, sectionId, componentId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(request.Content, ((CmsComponent)okResult.Value!).Content);
    }

    #endregion

    #region Preview Endpoint Tests

    [Fact]
    public async Task GeneratePreview_WithValidPageId_ShouldReturnHtmlContent()
    {
        // Arrange
        var pageId = Guid.NewGuid();
        var htmlPreview = "<html><body><h1>Test Page</h1></body></html>";

        _mockService
            .Setup(s => s.GeneratePreviewHtmlAsync(_tenantId, pageId, It.IsAny<string>()))
            .ReturnsAsync(htmlPreview);

        // Act
        var result = await _controller.GeneratePreview(pageId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(htmlPreview, okResult.Value);
    }

    #endregion
}
