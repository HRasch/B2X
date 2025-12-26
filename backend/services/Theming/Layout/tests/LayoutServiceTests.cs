using Xunit;
using Moq;
using B2Connect.LayoutService.Services;
using B2Connect.LayoutService.Models;
using B2Connect.LayoutService.Data;

namespace B2Connect.LayoutService.Tests;

/// <summary>
/// Test Suite für Layout Service - Page Management
/// TDD: Tests zuerst, dann Implementation
/// </summary>
public class LayoutServiceTests
{
    private readonly Mock<ILayoutRepository> _mockRepository;
    private readonly ILayoutService _service;

    public LayoutServiceTests()
    {
        _mockRepository = new Mock<ILayoutRepository>();
        _service = new LayoutService.Services.LayoutService(_mockRepository.Object);
    }

    #region Create Page Tests (RED → GREEN → REFACTOR)

    [Fact]
    public async Task CreatePage_WithValidPageData_ShouldReturnCreatedPage()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var createPageRequest = new CreatePageRequest
        {
            Title = "Home",
            Slug = "home",
            Description = "Homepage"
        };
        var expectedPageId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.CreatePageAsync(tenantId, It.IsAny<CmsPage>()))
            .ReturnsAsync(new CmsPage
            {
                Id = expectedPageId,
                TenantId = tenantId,
                Title = createPageRequest.Title,
                Slug = createPageRequest.Slug,
                Description = createPageRequest.Description,
                Sections = new List<CmsSection>(),
                Version = 1,
                CreatedAt = DateTime.UtcNow
            });

        // Act
        var result = await _service.CreatePageAsync(tenantId, createPageRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPageId, result.Id);
        Assert.Equal("Home", result.Title);
        Assert.Equal("home", result.Slug);
        Assert.Equal(1, result.Version);
        _mockRepository.Verify(r => r.CreatePageAsync(tenantId, It.IsAny<CmsPage>()), Times.Once);
    }

    [Fact]
    public async Task CreatePage_WithDuplicateSlug_ShouldThrowException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var createPageRequest = new CreatePageRequest
        {
            Title = "Home",
            Slug = "home",
            Description = "Homepage"
        };

        _mockRepository
            .Setup(r => r.PageSlugExistsAsync(tenantId, "home"))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreatePageAsync(tenantId, createPageRequest)
        );
    }

    [Fact]
    public async Task CreatePage_WithNullTitle_ShouldThrowException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var invalidPageRequest = new CreatePageRequest
        {
            Title = null!,
            Slug = "home",
            Description = "Homepage"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreatePageAsync(tenantId, invalidPageRequest)
        );
    }

    [Fact]
    public async Task CreatePage_WithEmptySlug_ShouldThrowException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var invalidPageRequest = new CreatePageRequest
        {
            Title = "Home",
            Slug = "",
            Description = "Homepage"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreatePageAsync(tenantId, invalidPageRequest)
        );
    }

    #endregion

    #region Get Page Tests

    [Fact]
    public async Task GetPageById_WithValidId_ShouldReturnPage()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var expectedPage = new CmsPage
        {
            Id = pageId,
            TenantId = tenantId,
            Title = "Home",
            Slug = "home",
            Description = "Homepage",
            Sections = new List<CmsSection>(),
            Version = 1,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetPageByIdAsync(tenantId, pageId))
            .ReturnsAsync(expectedPage);

        // Act
        var result = await _service.GetPageByIdAsync(tenantId, pageId, "en");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageId, result.Id);
        Assert.Equal("Home", result.Title);
        _mockRepository.Verify(r => r.GetPageByIdAsync(tenantId, pageId), Times.Once);
    }

    [Fact]
    public async Task GetPageById_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var invalidPageId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetPageByIdAsync(tenantId, invalidPageId))
            .ReturnsAsync((CmsPage?)null);

        // Act
        var result = await _service.GetPageByIdAsync(tenantId, invalidPageId, "en");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllPages_ShouldReturnPageList()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pages = new List<CmsPage>
        {
            new CmsPage { Id = Guid.NewGuid(), Title = "Home", Slug = "home", TenantId = tenantId },
            new CmsPage { Id = Guid.NewGuid(), Title = "About", Slug = "about", TenantId = tenantId }
        };

        _mockRepository
            .Setup(r => r.GetPagesByTenantAsync(tenantId))
            .ReturnsAsync(pages);

        // Act
        var result = await _service.GetPagesByTenantAsync(tenantId, "en");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(r => r.GetPagesByTenantAsync(tenantId), Times.Once);
    }

    #endregion

    #region Update Page Tests

    [Fact]
    public async Task UpdatePage_WithValidData_ShouldUpdateAndIncrementVersion()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var updateRequest = new UpdatePageRequest
        {
            Title = "Home Updated",
            Description = "Updated homepage"
        };
        var existingPage = new CmsPage
        {
            Id = pageId,
            TenantId = tenantId,
            Title = "Home",
            Version = 1
        };

        _mockRepository
            .Setup(r => r.GetPageByIdAsync(tenantId, pageId))
            .ReturnsAsync(existingPage);

        _mockRepository
            .Setup(r => r.UpdatePageAsync(tenantId, It.IsAny<CmsPage>()))
            .ReturnsAsync(new CmsPage
            {
                Id = pageId,
                TenantId = tenantId,
                Title = updateRequest.Title,
                Description = updateRequest.Description,
                Version = 2
            });

        // Act
        var result = await _service.UpdatePageAsync(tenantId, pageId, updateRequest, "en");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Home Updated", result.Title);
        Assert.Equal(2, result.Version);
        _mockRepository.Verify(r => r.UpdatePageAsync(tenantId, It.IsAny<CmsPage>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePage_WithNonExistentPage_ShouldThrowException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var updateRequest = new UpdatePageRequest
        {
            Title = "Updated"
        };

        _mockRepository
            .Setup(r => r.GetPageByIdAsync(tenantId, pageId))
            .ReturnsAsync((CmsPage?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdatePageAsync(tenantId, pageId, updateRequest, "en")
        );
    }

    #endregion

    #region Delete Page Tests

    [Fact]
    public async Task DeletePage_WithValidId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.DeletePageAsync(tenantId, pageId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeletePageAsync(tenantId, pageId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeletePageAsync(tenantId, pageId), Times.Once);
    }

    [Fact]
    public async Task DeletePage_WithNonExistentPage_ShouldReturnFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.DeletePageAsync(tenantId, pageId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeletePageAsync(tenantId, pageId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Section Management Tests

    [Fact]
    public async Task AddSection_WithValidData_ShouldReturnSectionWithOrder()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var addSectionRequest = new AddSectionRequest
        {
            Type = "hero",
            Layout = SectionLayout.FullWidth
        };
        var expectedSectionId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.AddSectionAsync(tenantId, pageId, It.IsAny<CmsSection>()))
            .ReturnsAsync(new CmsSection
            {
                Id = expectedSectionId,
                Type = "hero",
                Order = 0,
                Components = new List<CmsComponent>()
            });

        // Act
        var result = await _service.AddSectionAsync(tenantId, pageId, addSectionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("hero", result.Type);
        Assert.Equal(0, result.Order);
        _mockRepository.Verify(r => r.AddSectionAsync(tenantId, pageId, It.IsAny<CmsSection>()), Times.Once);
    }

    [Fact]
    public async Task RemoveSection_WithValidId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.RemoveSectionAsync(tenantId, pageId, sectionId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.RemoveSectionAsync(tenantId, pageId, sectionId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.RemoveSectionAsync(tenantId, pageId, sectionId), Times.Once);
    }

    [Fact]
    public async Task ReorderSections_WithValidOrder_ShouldUpdateOrder()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var sectionOrder = new List<(Guid SectionId, int Order)>
        {
            (Guid.NewGuid(), 0),
            (Guid.NewGuid(), 1),
            (Guid.NewGuid(), 2)
        };

        _mockRepository
            .Setup(r => r.ReorderSectionsAsync(tenantId, pageId, It.IsAny<List<(Guid, int)>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ReorderSectionsAsync(tenantId, pageId, sectionOrder);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.ReorderSectionsAsync(tenantId, pageId, sectionOrder), Times.Once);
    }

    #endregion

    #region Component Management Tests

    [Fact]
    public async Task AddComponent_WithValidData_ShouldReturnComponent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var addComponentRequest = new AddComponentRequest
        {
            Type = "button",
            Content = "Click Me"
        };
        var expectedComponentId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.AddComponentAsync(tenantId, pageId, sectionId, It.IsAny<CmsComponent>()))
            .ReturnsAsync(new CmsComponent
            {
                Id = expectedComponentId,
                Type = "button",
                Content = "Click Me",
                IsVisible = true
            });

        // Act
        var result = await _service.AddComponentAsync(tenantId, pageId, sectionId, addComponentRequest, "en");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("button", result.Type);
        Assert.Equal("Click Me", result.Content);
        Assert.True(result.IsVisible);
    }

    [Fact]
    public async Task UpdateComponent_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var updateRequest = new UpdateComponentRequest
        {
            Content = "Updated Button Text"
        };

        _mockRepository
            .Setup(r => r.UpdateComponentAsync(tenantId, pageId, sectionId, componentId, It.IsAny<CmsComponent>()))
            .ReturnsAsync(new CmsComponent
            {
                Id = componentId,
                Type = "button",
                Content = "Updated Button Text",
                IsVisible = true
            });

        // Act
        var result = await _service.UpdateComponentAsync(tenantId, pageId, sectionId, componentId, updateRequest, "en");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Button Text", result.Content);
    }

    [Fact]
    public async Task RemoveComponent_WithValidId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var componentId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.RemoveComponentAsync(tenantId, pageId, sectionId, componentId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.RemoveComponentAsync(tenantId, pageId, sectionId, componentId);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Preview Generation Tests

    [Fact]
    public async Task GeneratePreview_WithValidPageId_ShouldReturnHtmlPreview()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var expectedHtml = "<html><body><div class=\"hero\">Welcome</div></body></html>";

        _mockRepository
            .Setup(r => r.GeneratePreviewHtmlAsync(tenantId, pageId))
            .ReturnsAsync(expectedHtml);

        // Act
        var result = await _service.GeneratePreviewHtmlAsync(tenantId, pageId, "en");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<html>", result);
        Assert.Contains("hero", result);
        _mockRepository.Verify(r => r.GeneratePreviewHtmlAsync(tenantId, pageId), Times.Once);
    }

    #endregion

    #region Component Registry Tests

    [Fact]
    public async Task GetAvailableComponents_ShouldReturnComponentDefinitions()
    {
        // Arrange
        var expectedComponents = new List<ComponentDefinition>
        {
            new ComponentDefinition { ComponentType = "button", DisplayName = "Button" },
            new ComponentDefinition { ComponentType = "text", DisplayName = "Text" },
            new ComponentDefinition { ComponentType = "image", DisplayName = "Image" }
        };

        _mockRepository
            .Setup(r => r.GetComponentDefinitionsAsync())
            .ReturnsAsync(expectedComponents);

        // Act
        var result = await _service.GetComponentDefinitionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.ComponentType == "button");
    }

    #endregion
}
