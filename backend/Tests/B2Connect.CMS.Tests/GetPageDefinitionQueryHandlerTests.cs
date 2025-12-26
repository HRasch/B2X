using Xunit;
using Shouldly;
using Moq;
using Microsoft.Extensions.Logging;
using B2Connect.CMS.Application.Pages;
using B2Connect.CMS.Application.Widgets;
using B2Connect.CMS.Core.Domain.Pages;
using B2Connect.CMS.Core.Domain.Widgets;

namespace B2Connect.CMS.Tests;

/// <summary>
/// Unit tests for GetPageDefinitionQueryHandler
/// </summary>
public class GetPageDefinitionQueryHandlerTests
{
    private readonly Mock<IPageRepository> _repositoryMock;
    private readonly Mock<IWidgetRegistry> _registryMock;
    private readonly Mock<ILogger<GetPageDefinitionQueryHandler>> _loggerMock;
    private readonly GetPageDefinitionQueryHandler _handler;

    public GetPageDefinitionQueryHandlerTests()
    {
        _repositoryMock = new Mock<IPageRepository>();
        _registryMock = new Mock<IWidgetRegistry>();
        _loggerMock = new Mock<ILogger<GetPageDefinitionQueryHandler>>();

        _handler = new GetPageDefinitionQueryHandler(
            _repositoryMock.Object,
            _registryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithPublishedPage_ShouldReturnPageDefinitionDto()
    {
        // Arrange
        var tenantId = "tenant-1";
        var pagePath = "/";

        var page = CreatePublishedPage(tenantId, pagePath);
        var widgetDef = new WidgetDefinition("hero-banner", "Hero Banner", "widgets/HeroBanner.vue", "media");

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync(tenantId, pagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        _registryMock
            .Setup(x => x.GetWidget("hero-banner"))
            .Returns(widgetDef);

        var query = new GetPageDefinitionQuery { TenantId = tenantId, PagePath = pagePath };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.PagePath.ShouldBe(pagePath);
        result.PageTitle.ShouldBe("Home Page");
        result.IsPublished.ShouldBeTrue();
        result.Regions.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WithUnpublishedPage_ShouldThrowException()
    {
        // Arrange
        var tenantId = "tenant-1";
        var pagePath = "/";

        var page = CreatePublishedPage(tenantId, pagePath);
        page.UnpublishPage();

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync(tenantId, pagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        var query = new GetPageDefinitionQuery { TenantId = tenantId, PagePath = pagePath };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentPage_ShouldThrowException()
    {
        // Arrange
        var tenantId = "tenant-1";
        var pagePath = "/non-existent";

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync(tenantId, pagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PageDefinition)null);

        var query = new GetPageDefinitionQuery { TenantId = tenantId, PagePath = pagePath };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldMapWidgetComponentPath()
    {
        // Arrange
        var tenantId = "tenant-1";
        var pagePath = "/";

        var page = CreatePublishedPage(tenantId, pagePath);
        var widgetDef = new WidgetDefinition("hero-banner", "Hero Banner", "widgets/HeroBanner.vue", "media");

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync(tenantId, pagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        _registryMock
            .Setup(x => x.GetWidget("hero-banner"))
            .Returns(widgetDef);

        var query = new GetPageDefinitionQuery { TenantId = tenantId, PagePath = pagePath };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Regions[0].Widgets[0].ComponentPath.ShouldBe("widgets/HeroBanner.vue");
    }

    [Fact]
    public async Task Handle_OnlyIncludesEnabledWidgets()
    {
        // Arrange
        var tenantId = "tenant-1";
        var pagePath = "/";

        var page = new PageDefinition(tenantId, "home", pagePath, "Home Page", "full-width");
        var region = new PageRegion { Name = "main", Order = 1 };

        var widget1 = region.AddWidget("hero-banner", new());
        widget1.IsEnabled = true;

        var widget2 = region.AddWidget("product-grid", new());
        widget2.IsEnabled = false;

        page.AddRegion(region);
        page.PublishPage();

        var widgetDef = new WidgetDefinition("hero-banner", "Hero Banner", "widgets/HeroBanner.vue", "media");

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync(tenantId, pagePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        _registryMock
            .Setup(x => x.GetWidget("hero-banner"))
            .Returns(widgetDef);

        var query = new GetPageDefinitionQuery { TenantId = tenantId, PagePath = pagePath };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Regions[0].Widgets.Count.ShouldBe(1);
        result.Regions[0].Widgets[0].WidgetTypeId.ShouldBe("hero-banner");
    }

    private static PageDefinition CreatePublishedPage(string tenantId, string pagePath)
    {
        var page = new PageDefinition(tenantId, "home", pagePath, "Home Page", "full-width");

        var region = new PageRegion { Name = "main", Order = 1 };
        region.AddWidget("hero-banner", new());

        page.AddRegion(region);
        page.PublishPage();

        return page;
    }
}
