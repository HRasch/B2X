using Xunit;
using Shouldly;
using Microsoft.Extensions.Logging;
using Moq;
using B2X.CMS.Application.Widgets;
using B2X.CMS.Core.Domain.Widgets;

namespace B2X.CMS.Tests;

/// <summary>
/// Unit tests for WidgetRegistry service
/// </summary>
public class WidgetRegistryTests
{
    private readonly IWidgetRegistry _registry;
    private readonly Mock<ILogger<WidgetRegistry>> _loggerMock;

    public WidgetRegistryTests()
    {
        _loggerMock = new Mock<ILogger<WidgetRegistry>>();
        _registry = new WidgetRegistry(_loggerMock.Object);
    }

    [Fact]
    public void RegisterWidget_WithValidWidget_ShouldRegisterSuccessfully()
    {
        // Arrange
        var widget = new WidgetDefinition(
            "test-widget",
            "Test Widget",
            "widgets/TestWidget.vue",
            "content");

        // Act
        _registry.RegisterWidget(widget);

        // Assert
        var retrieved = _registry.GetWidget("test-widget");
        retrieved.ShouldNotBeNull();
        retrieved.Id.ShouldBe("test-widget");
        retrieved.DisplayName.ShouldBe("Test Widget");
    }

    [Fact]
    public void RegisterWidget_WithEmptyId_ShouldThrowArgumentException()
    {
        // Arrange
        var widget = new WidgetDefinition(
            "",
            "Test Widget",
            "widgets/TestWidget.vue",
            "content");

        // Act & Assert
        Should.Throw<ArgumentException>(() => _registry.RegisterWidget(widget));
    }

    [Fact]
    public void GetWidget_WithNonExistentId_ShouldThrowException()
    {
        // Act & Assert
        Should.Throw<InvalidOperationException>(() => _registry.GetWidget("non-existent"));
    }

    [Fact]
    public void GetAllWidgets_WithMultipleWidgets_ShouldReturnAll()
    {
        // Arrange
        var widget1 = new WidgetDefinition("widget-1", "Widget 1", "widgets/W1.vue", "media");
        var widget2 = new WidgetDefinition("widget-2", "Widget 2", "widgets/W2.vue", "content");
        var widget3 = new WidgetDefinition("widget-3", "Widget 3", "widgets/W3.vue", "products");

        _registry.RegisterWidget(widget1);
        _registry.RegisterWidget(widget2);
        _registry.RegisterWidget(widget3);

        // Act
        var widgets = _registry.GetAllWidgets().ToList();

        // Assert
        widgets.Count.ShouldBe(3);
    }

    [Fact]
    public void GetWidgetsByCategory_ShouldReturnOnlyWidgetsInCategory()
    {
        // Arrange
        _registry.RegisterWidget(new WidgetDefinition("media-1", "Media 1", "widgets/M1.vue", "media"));
        _registry.RegisterWidget(new WidgetDefinition("media-2", "Media 2", "widgets/M2.vue", "media"));
        _registry.RegisterWidget(new WidgetDefinition("content-1", "Content 1", "widgets/C1.vue", "content"));

        // Act
        var mediaWidgets = _registry.GetWidgetsByCategory("media").ToList();
        var contentWidgets = _registry.GetWidgetsByCategory("content").ToList();

        // Assert
        mediaWidgets.Count.ShouldBe(2);
        contentWidgets.Count.ShouldBe(1);
    }

    [Fact]
    public void GetWidgetsForPageType_WithoutPageTypeRestriction_ShouldReturnAllWidgets()
    {
        // Arrange
        var homeOnlyWidget = new WidgetDefinition("home-widget", "Home", "widgets/H.vue", "content")
        {
            AllowedPageTypes = new List<string> { "home" }
        };
        var universalWidget = new WidgetDefinition("universal-widget", "Universal", "widgets/U.vue", "content")
        {
            AllowedPageTypes = new List<string>()
        };

        _registry.RegisterWidget(homeOnlyWidget);
        _registry.RegisterWidget(universalWidget);

        // Act
        var homeWidgets = _registry.GetWidgetsForPageType("home").ToList();
        var aboutWidgets = _registry.GetWidgetsForPageType("about").ToList();

        // Assert
        homeWidgets.Count.ShouldBe(2); // Both home-specific and universal
        aboutWidgets.Count.ShouldBe(1); // Only universal
    }

    [Fact]
    public void IsWidgetAvailable_WithEnabledWidget_ShouldReturnTrue()
    {
        // Arrange
        var widget = new WidgetDefinition("enabled-widget", "Enabled", "widgets/E.vue", "content")
        {
            IsEnabled = true
        };
        _registry.RegisterWidget(widget);

        // Act
        var isAvailable = _registry.IsWidgetAvailable("enabled-widget");

        // Assert
        isAvailable.ShouldBeTrue();
    }

    [Fact]
    public void IsWidgetAvailable_WithDisabledWidget_ShouldReturnFalse()
    {
        // Arrange
        var widget = new WidgetDefinition("disabled-widget", "Disabled", "widgets/D.vue", "content")
        {
            IsEnabled = false
        };
        _registry.RegisterWidget(widget);

        // Act
        var isAvailable = _registry.IsWidgetAvailable("disabled-widget");

        // Assert
        isAvailable.ShouldBeFalse();
    }

    [Fact]
    public void RegisterWidget_ShouldOverwriteDuplicateId()
    {
        // Arrange
        var widget1 = new WidgetDefinition("duplicate", "Widget 1", "widgets/W1.vue", "content");
        var widget2 = new WidgetDefinition("duplicate", "Widget 2", "widgets/W2.vue", "media");

        // Act
        _registry.RegisterWidget(widget1);
        _registry.RegisterWidget(widget2);

        var retrieved = _registry.GetWidget("duplicate");

        // Assert
        retrieved.DisplayName.ShouldBe("Widget 2");
        retrieved.Category.ShouldBe("media");
    }

    [Fact]
    public void GetAllWidgets_OnlyIncludesEnabledWidgets()
    {
        // Arrange
        var enabled = new WidgetDefinition("enabled", "Enabled", "widgets/E.vue", "content") { IsEnabled = true };
        var disabled = new WidgetDefinition("disabled", "Disabled", "widgets/D.vue", "content") { IsEnabled = false };

        _registry.RegisterWidget(enabled);
        _registry.RegisterWidget(disabled);

        // Act
        var widgets = _registry.GetAllWidgets().ToList();

        // Assert
        widgets.Count.ShouldBe(1);
        widgets[0].Id.ShouldBe("enabled");
    }

    [Fact]
    public void GetAllWidgets_ShouldReturnSortedBySortOrder()
    {
        // Arrange
        var widget1 = new WidgetDefinition("widget-1", "Widget 1", "widgets/W1.vue", "content") { SortOrder = 3 };
        var widget2 = new WidgetDefinition("widget-2", "Widget 2", "widgets/W2.vue", "content") { SortOrder = 1 };
        var widget3 = new WidgetDefinition("widget-3", "Widget 3", "widgets/W3.vue", "content") { SortOrder = 2 };

        _registry.RegisterWidget(widget1);
        _registry.RegisterWidget(widget2);
        _registry.RegisterWidget(widget3);

        // Act
        var widgets = _registry.GetAllWidgets().ToList();

        // Assert
        widgets[0].SortOrder.ShouldBe(1);
        widgets[1].SortOrder.ShouldBe(2);
        widgets[2].SortOrder.ShouldBe(3);
    }
}
