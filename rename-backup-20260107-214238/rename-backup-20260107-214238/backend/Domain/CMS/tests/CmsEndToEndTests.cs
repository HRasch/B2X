using B2X.CMS.Application.Pages;
using B2X.CMS.Application.Widgets;
using B2X.CMS.Core.Domain.Pages;
using B2X.CMS.Core.Domain.Widgets;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.CMS.Tests;

/// <summary>
/// End-to-End CMS tests
/// Tests complete workflow: Create page → Add regions → Add widgets → Query
/// </summary>
public class CmsEndToEndTests
{
    private readonly WidgetRegistry _registry;
    private readonly Mock<IPageRepository> _repositoryMock;
    private readonly GetPageDefinitionQueryHandler _queryHandler;

    public CmsEndToEndTests()
    {
        var loggerRegistry = new Mock<ILogger<WidgetRegistry>>().Object;
        var loggerHandler = new Mock<ILogger<GetPageDefinitionQueryHandler>>().Object;

        _registry = new WidgetRegistry(loggerRegistry);
        _repositoryMock = new Mock<IPageRepository>();
        _queryHandler = new GetPageDefinitionQueryHandler(_repositoryMock.Object, _registry, loggerHandler);

        RegisterDefaultWidgets();
    }

    [Fact]
    public async Task CompleteWorkflow_CreatePageWithMultipleWidgets_ShouldReturnFullPageDefinition()
    {
        // Arrange: Register widgets
        var heroWidget = new WidgetDefinition("hero-banner", "Hero Banner", "widgets/HeroBanner.vue", "media")
        {
            DefaultSettings = new List<WidgetSetting>
            {
                new WidgetSetting("title", "Title", WidgetSettingType.Text)
            }
        };

        var productWidget = new WidgetDefinition("product-grid", "Product Grid", "widgets/ProductGrid.vue", "products")
        {
            AllowedPageTypes = new List<string> { "home" },
            DefaultSettings = new List<WidgetSetting>
            {
                new WidgetSetting("columns", "Columns", WidgetSettingType.Select)
            }
        };

        _registry.RegisterWidget(heroWidget);
        _registry.RegisterWidget(productWidget);

        // Arrange: Create page definition
        var page = new PageDefinition("tenant-1", "home", "/", "Home Page", "full-width");
        page.PageDescription = "Welcome to our store";
        page.MetaKeywords = "store, shopping";

        // Add header region with hero banner
        var headerRegion = new PageRegion { Name = "header", Order = 1 };
        headerRegion.AddWidget("hero-banner", new Dictionary<string, object>
        {
            { "title", "Welcome!" },
            { "backgroundImage", "/images/hero.jpg" }
        });
        page.AddRegion(headerRegion);

        // Add main region with product grid
        var mainRegion = new PageRegion { Name = "main", Order = 2 };
        mainRegion.AddWidget("product-grid", new Dictionary<string, object>
        {
            { "columns", "3" }
        });
        page.AddRegion(mainRegion);

        // Publish page
        page.PublishPage();

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync("tenant-1", "/", It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        // Act: Query the page
        var query = new GetPageDefinitionQuery { TenantId = "tenant-1", PagePath = "/" };
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.PageTitle.ShouldBe("Home Page");
        result.PageDescription.ShouldBe("Welcome to our store");
        result.TemplateLayout.ShouldBe("full-width");
        result.IsPublished.ShouldBeTrue();

        result.Regions.Count.ShouldBe(2);
        result.Regions[0].Name.ShouldBe("header");
        result.Regions[1].Name.ShouldBe("main");

        result.Regions[0].Widgets.Count.ShouldBe(1);
        result.Regions[0].Widgets[0].WidgetTypeId.ShouldBe("hero-banner");

        result.Regions[1].Widgets.Count.ShouldBe(1);
        result.Regions[1].Widgets[0].WidgetTypeId.ShouldBe("product-grid");
    }

    [Fact]
    public async Task Workflow_CreateMultiplePages_ShouldHaveIndependentDefinitions()
    {
        // Arrange
        var homePage = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");
        var aboutPage = new PageDefinition("tenant-1", "about", "/about", "About Us", "sidebar");

        var region1 = new PageRegion { Name = "main", Order = 1 };
        region1.AddWidget("hero-banner", new());
        homePage.AddRegion(region1);
        homePage.PublishPage();

        var region2 = new PageRegion { Name = "main", Order = 1 };
        region2.AddWidget("text-block", new());
        aboutPage.AddRegion(region2);
        aboutPage.PublishPage();

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync("tenant-1", "/", It.IsAny<CancellationToken>()))
            .ReturnsAsync(homePage);

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync("tenant-1", "/about", It.IsAny<CancellationToken>()))
            .ReturnsAsync(aboutPage);

        // Act
        var homeQuery = new GetPageDefinitionQuery { TenantId = "tenant-1", PagePath = "/" };
        var aboutQuery = new GetPageDefinitionQuery { TenantId = "tenant-1", PagePath = "/about" };

        var homeResult = await _queryHandler.Handle(homeQuery, CancellationToken.None);
        var aboutResult = await _queryHandler.Handle(aboutQuery, CancellationToken.None);

        // Assert
        homeResult.TemplateLayout.ShouldBe("full-width");
        aboutResult.TemplateLayout.ShouldBe("sidebar");
    }

    [Fact]
    public async Task Workflow_DynamicWidgetLoading_ShouldResolvePaths()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");

        var region = new PageRegion { Name = "main", Order = 1 };
        var widget1 = region.AddWidget("hero-banner", new());
        var widget2 = region.AddWidget("testimonials", new());

        page.AddRegion(region);
        page.PublishPage();

        var heroWidget = new WidgetDefinition("hero-banner", "Hero", "widgets/HeroBanner.vue", "media");
        var testimonialWidget = new WidgetDefinition("testimonials", "Testimonials", "widgets/Testimonials.vue", "content");

        _registry.RegisterWidget(heroWidget);
        _registry.RegisterWidget(testimonialWidget);

        _repositoryMock
            .Setup(x => x.GetPageByPathAsync("tenant-1", "/", It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        // Act
        var query = new GetPageDefinitionQuery { TenantId = "tenant-1", PagePath = "/" };
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert - Component paths should be resolved from registry
        result.Regions[0].Widgets[0].ComponentPath.ShouldBe("widgets/HeroBanner.vue");
        result.Regions[0].Widgets[1].ComponentPath.ShouldBe("widgets/Testimonials.vue");
    }

    [Fact]
    public void Workflow_RegisterWidgetAndQueryByCategory_ShouldFilter()
    {
        // Arrange & Act
        var mediaWidgets = _registry.GetWidgetsByCategory("media").ToList();
        var contentWidgets = _registry.GetWidgetsByCategory("content").ToList();
        var productWidgets = _registry.GetWidgetsByCategory("products").ToList();
        var formWidgets = _registry.GetWidgetsByCategory("forms").ToList();

        // Assert - Should have registered default widgets
        mediaWidgets.Count.ShouldBeGreaterThan(0);
        contentWidgets.Count.ShouldBeGreaterThan(0);
        productWidgets.Count.ShouldBeGreaterThan(0);
        formWidgets.Count.ShouldBeGreaterThan(0);
    }

    private void RegisterDefaultWidgets()
    {
        // Register minimal default widgets for testing
        var widgets = new[]
        {
            new WidgetDefinition("hero-banner", "Hero Banner", "widgets/HeroBanner.vue", "media"),
            new WidgetDefinition("product-grid", "Product Grid", "widgets/ProductGrid.vue", "products") { AllowedPageTypes = new List<string> { "home" } },
            new WidgetDefinition("testimonials", "Testimonials", "widgets/Testimonials.vue", "content"),
            new WidgetDefinition("text-block", "Text Block", "widgets/TextBlock.vue", "content"),
            new WidgetDefinition("feature-grid", "Feature Grid", "widgets/FeatureGrid.vue", "content"),
            new WidgetDefinition("call-to-action", "CTA", "widgets/CallToAction.vue", "content"),
            new WidgetDefinition("video", "Video", "widgets/Video.vue", "media"),
            new WidgetDefinition("newsletter-signup", "Newsletter", "widgets/NewsletterSignup.vue", "forms")
        };

        foreach (var widget in widgets)
        {
            _registry.RegisterWidget(widget);
        }
    }
}
