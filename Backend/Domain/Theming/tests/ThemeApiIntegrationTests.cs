using System.Net;
using System.Net.Http.Json;
using B2Connect.ThemeService.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Http;
using Xunit;

namespace B2Connect.Theming.Tests.Integration;

public class ThemeApiIntegrationTests : IClassFixture<ThemingApiFactory>
{
    private readonly HttpClient _client;
    private readonly ThemingApiFactory _factory;

    public ThemeApiIntegrationTests(ThemingApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        // Set default tenant header for all requests
        _client.DefaultRequestHeaders.Add("X-Tenant-ID", "00000000-0000-0000-0000-000000000001");
    }

    // Clear repository before each test to ensure isolation
    private void ClearRepository()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IThemeRepository>();
        if (repository is InMemoryThemeRepository inMemoryRepo)
        {
            inMemoryRepo.Clear();
        }
    }

    [Fact]
    public async Task GetThemes_WhenNoThemesExist_ReturnsEmptyList()
    {
        // Arrange
        ClearRepository();

        // Act
        var response = await _client.GetAsync("/api/themes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var themes = await response.Content.ReadFromJsonAsync<List<Theme>>();
        themes.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateTheme_WithValidData_ReturnsCreatedTheme()
    {
        // Arrange
        var createRequest = new CreateThemeRequest
        {
            Name = "Test Theme",
            Description = "A test theme",
            PrimaryColor = "#007bff",
            SecondaryColor = "#6c757d",
            TertiaryColor = "#28a745"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/themes", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var theme = await response.Content.ReadFromJsonAsync<Theme>();
        theme.Should().NotBeNull();
        theme!.Name.Should().Be("Test Theme");
        theme.Description.Should().Be("A test theme");
        theme.PrimaryColor.Should().Be("#007bff");
        theme.Id.Should().NotBeEmpty();
        theme.TenantId.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }

    [Fact]
    public async Task GetTheme_ById_ReturnsTheme()
    {
        // Arrange
        ClearRepository();
        var createRequest = new CreateThemeRequest
        {
            Name = "Test Theme",
            Description = "A test theme"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/themes", createRequest);
        var createdTheme = await createResponse.Content.ReadFromJsonAsync<Theme>();

        // Act
        var response = await _client.GetAsync($"/api/themes/{createdTheme!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var theme = await response.Content.ReadFromJsonAsync<Theme>();
        theme.Should().NotBeNull();
        theme!.Id.Should().Be(createdTheme.Id);
        theme.Name.Should().Be("Test Theme");
    }

    [Fact]
    public async Task UpdateTheme_WithValidData_ReturnsUpdatedTheme()
    {
        // Arrange
        ClearRepository();
        var createRequest = new CreateThemeRequest
        {
            Name = "Original Theme",
            Description = "Original description"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/themes", createRequest);
        var createdTheme = await createResponse.Content.ReadFromJsonAsync<Theme>();

        var updateRequest = new UpdateThemeRequest
        {
            Name = "Updated Theme",
            Description = "Updated description",
            PrimaryColor = "#ff0000"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/themes/{createdTheme!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTheme = await response.Content.ReadFromJsonAsync<Theme>();
        updatedTheme.Should().NotBeNull();
        updatedTheme!.Name.Should().Be("Updated Theme");
        updatedTheme.Description.Should().Be("Updated description");
        updatedTheme.PrimaryColor.Should().Be("#ff0000");
    }

    [Fact]
    public async Task DeleteTheme_WhenThemeExists_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new CreateThemeRequest
        {
            Name = "Theme to Delete"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/themes", createRequest);
        var createdTheme = await createResponse.Content.ReadFromJsonAsync<Theme>();

        // Act
        var response = await _client.DeleteAsync($"/api/themes/{createdTheme!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify theme is deleted
        var getResponse = await _client.GetAsync($"/api/themes/{createdTheme.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PublishTheme_MakesThemeActive()
    {
        // Arrange
        ClearRepository();
        var createRequest = new CreateThemeRequest
        {
            Name = "Theme to Publish"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/themes", createRequest);
        var createdTheme = await createResponse.Content.ReadFromJsonAsync<Theme>();

        // Act
        var response = await _client.PostAsync($"/api/themes/{createdTheme!.Id}/publish", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var publishedTheme = await response.Content.ReadFromJsonAsync<Theme>();
        publishedTheme.Should().NotBeNull();
        publishedTheme!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UnpublishTheme_MakesThemeInactive()
    {
        // Arrange
        ClearRepository();
        var createRequest = new CreateThemeRequest
        {
            Name = "Theme to Unpublish"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/themes", createRequest);
        var createdTheme = await createResponse.Content.ReadFromJsonAsync<Theme>();

        // First publish the theme to make it active
        await _client.PostAsync($"/api/themes/{createdTheme!.Id}/publish", null);

        // Act
        var response = await _client.PostAsync($"/api/themes/{createdTheme!.Id}/unpublish", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var unpublishedTheme = await response.Content.ReadFromJsonAsync<Theme>();
        unpublishedTheme.Should().NotBeNull();
        unpublishedTheme!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetActiveTheme_WhenNoActiveTheme_ReturnsNotFound()
    {
        // Arrange
        ClearRepository();

        // Act
        var response = await _client.GetAsync("/api/themes/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActiveTheme_WhenActiveThemeExists_ReturnsTheme()
    {
        // Arrange
        ClearRepository();
        var createRequest = new CreateThemeRequest
        {
            Name = "Active Theme"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/themes", createRequest);
        var createdTheme = await createResponse.Content.ReadFromJsonAsync<Theme>();
        await _client.PostAsync($"/api/themes/{createdTheme!.Id}/publish", null);

        // Act
        var response = await _client.GetAsync("/api/themes/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var activeTheme = await response.Content.ReadFromJsonAsync<Theme>();
        activeTheme.Should().NotBeNull();
        activeTheme!.Name.Should().Be("Active Theme");
        activeTheme.IsActive.Should().BeTrue();
    }
}