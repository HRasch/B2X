using B2X.CMS.Core.Domain.Pages;
using Shouldly;
using Xunit;

namespace B2X.CMS.Tests;

/// <summary>
/// Unit tests for PageDefinition aggregate root
/// </summary>
public class PageDefinitionTests
{
    [Fact]
    public void CreatePageDefinition_WithValidData_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var page = new PageDefinition(
            "tenant-1",
            "home",
            "/",
            "Home Page",
            "full-width");

        // Assert
        page.TenantId.ShouldBe("tenant-1");
        page.PageType.ShouldBe("home");
        page.PagePath.ShouldBe("/");
        page.PageTitle.ShouldBe("Home Page");
        page.TemplateLayout.ShouldBe("full-width");
        page.IsPublished.ShouldBeFalse();
        page.Version.ShouldBe(1);
        page.Regions.ShouldBeEmpty();
    }

    [Fact]
    public void AddRegion_WithUniqueRegionName_ShouldAddSuccessfully()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");
        var region = new PageRegion { Name = "header", Order = 1 };

        // Act
        page.AddRegion(region);

        // Assert
        page.Regions.Count.ShouldBe(1);
        page.Regions[0].Name.ShouldBe("header");
        page.Regions[0].PageDefinitionId.ShouldBe(page.Id);
    }

    [Fact]
    public void AddRegion_WithDuplicateRegionName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");
        var region1 = new PageRegion { Name = "header", Order = 1 };
        var region2 = new PageRegion { Name = "header", Order = 2 };

        page.AddRegion(region1);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => page.AddRegion(region2));
    }

    [Fact]
    public void RemoveRegion_WithExistingRegion_ShouldRemoveSuccessfully()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");
        page.AddRegion(new PageRegion { Name = "header", Order = 1 });
        page.AddRegion(new PageRegion { Name = "main", Order = 2 });

        // Act
        page.RemoveRegion("header");

        // Assert
        page.Regions.Count.ShouldBe(1);
        page.Regions[0].Name.ShouldBe("main");
    }

    [Fact]
    public void RemoveRegion_WithNonExistentRegion_ShouldNotThrow()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");
        page.AddRegion(new PageRegion { Name = "header", Order = 1 });

        // Act & Assert - should not throw
        page.RemoveRegion("non-existent");
        page.Regions.Count.ShouldBe(1);
    }

    [Fact]
    public void PublishPage_ShouldSetIsPublishedAndPublishedAt()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");

        // Act
        page.PublishPage();

        // Assert
        page.IsPublished.ShouldBeTrue();
        page.PublishedAt.ShouldNotBe(default(DateTime));
    }

    [Fact]
    public void UnpublishPage_ShouldSetIsPublishedToFalse()
    {
        // Arrange
        var page = new PageDefinition("tenant-1", "home", "/", "Home", "full-width");
        page.PublishPage();

        // Act
        page.UnpublishPage();

        // Assert
        page.IsPublished.ShouldBeFalse();
    }
}

/// <summary>
/// Unit tests for PageRegion entity
/// </summary>
public class PageRegionTests
{
    [Fact]
    public void AddWidget_WithValidData_ShouldAddSuccessfully()
    {
        // Arrange
        var region = new PageRegion { Name = "main", Order = 1 };
        var settings = new Dictionary<string, object> { { "title", "Widget Title" } };

        // Act
        var widget = region.AddWidget("hero-banner", settings);

        // Assert
        region.Widgets.Count.ShouldBe(1);
        widget.WidgetTypeId.ShouldBe("hero-banner");
        widget.Order.ShouldBe(1);
        widget.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void AddWidget_MultipleWidgets_ShouldIncrementOrder()
    {
        // Arrange
        var region = new PageRegion { Name = "main", Order = 1 };

        // Act
        var widget1 = region.AddWidget("hero-banner", new());
        var widget2 = region.AddWidget("product-grid", new());
        var widget3 = region.AddWidget("testimonials", new());

        // Assert
        widget1.Order.ShouldBe(1);
        widget2.Order.ShouldBe(2);
        widget3.Order.ShouldBe(3);
    }

    [Fact]
    public void AddWidget_ExceedingMaxWidgets_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var region = new PageRegion { Name = "main", Order = 1, MaxWidgets = 2 };
        region.AddWidget("hero-banner", new());
        region.AddWidget("product-grid", new());

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            region.AddWidget("testimonials", new()));
    }

    [Fact]
    public void RemoveWidget_WithExistingWidget_ShouldRemoveAndReorder()
    {
        // Arrange
        var region = new PageRegion { Name = "main", Order = 1 };
        var widget1 = region.AddWidget("hero-banner", new());
        var widget2 = region.AddWidget("product-grid", new());
        var widget3 = region.AddWidget("testimonials", new());

        // Act
        region.RemoveWidget(widget2.Id);

        // Assert
        region.Widgets.Count.ShouldBe(2);
        region.Widgets[0].Order.ShouldBe(1);
        region.Widgets[1].Order.ShouldBe(2); // Reordered
    }

    [Fact]
    public void RemoveWidget_NonExistentWidget_ShouldNotThrow()
    {
        // Arrange
        var region = new PageRegion { Name = "main", Order = 1 };
        region.AddWidget("hero-banner", new());

        // Act & Assert - should not throw
        region.RemoveWidget("non-existent-id");
        region.Widgets.Count.ShouldBe(1);
    }

    [Fact]
    public void ReorderWidgets_ShouldUpdateAllOrders()
    {
        // Arrange
        var region = new PageRegion { Name = "main", Order = 1 };
        region.AddWidget("hero-banner", new());
        region.AddWidget("product-grid", new());
        region.AddWidget("testimonials", new());

        // Manually mess up the order
        region.Widgets[0].Order = 5;
        region.Widgets[1].Order = 10;
        region.Widgets[2].Order = 3;

        // Act
        region.ReorderWidgets();

        // Assert
        region.Widgets[0].Order.ShouldBe(1);
        region.Widgets[1].Order.ShouldBe(2);
        region.Widgets[2].Order.ShouldBe(3);
    }
}

/// <summary>
/// Unit tests for WidgetInstance entity
/// </summary>
public class WidgetInstanceTests
{
    [Fact]
    public void CreateWidgetInstance_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var widget = new WidgetInstance();

        // Assert
        widget.Id.ShouldNotBeNullOrEmpty();
        widget.IsEnabled.ShouldBeTrue();
        widget.CreatedAt.ShouldNotBe(default(DateTime));
    }

    [Fact]
    public void WidgetInstance_WithSettings_ShouldStoreSettings()
    {
        // Arrange & Act
        var widget = new WidgetInstance
        {
            Settings = new Dictionary<string, object>
            {
                { "title", "Hero Title" },
                { "backgroundColor", "#000000" }
            }
        };

        // Assert
        widget.Settings["title"].ShouldBe("Hero Title");
        widget.Settings["backgroundColor"].ShouldBe("#000000");
    }
}
