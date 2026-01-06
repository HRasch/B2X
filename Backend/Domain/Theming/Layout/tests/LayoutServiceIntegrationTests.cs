using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using B2Connect.LayoutService.Data;
using B2Connect.LayoutService.Models;

namespace B2Connect.LayoutService.Tests;

/// <summary>
/// Integration Tests für Layout Service mit InMemory Database
/// Tests die tatsächliche Datenbankoperationen verwenden
/// </summary>
public class LayoutServiceIntegrationTests : IAsyncLifetime
{
    private readonly LayoutDbContext _dbContext;
    private readonly LayoutRepository _repository;

    public LayoutServiceIntegrationTests()
    {
        // Setup InMemory Database für Tests
        var options = new DbContextOptionsBuilder<LayoutDbContext>()
            .UseInMemoryDatabase(databaseName: $"LayoutTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new LayoutDbContext(options);
        _repository = new LayoutRepository(_dbContext);
    }

    // IAsyncLifetime für async Setup/Teardown
    public async Task InitializeAsync()
    {
        // Ensure database is created
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Cleanup
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    #region Page Repository Tests

    [Fact]
    public async Task CreatePageAsync_WithValidPage_ShouldPersistAndReturn()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = "Integration Test Page",
            Slug = "integration-test-page",
            Description = "Test Description",
            Visibility = PageVisibility.Draft,
            Version = 1
        };

        // Act
        var result = await _repository.CreatePageAsync(tenantId, page);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page.Id, result.Id);
        Assert.Equal(tenantId, result.TenantId);

        // Verify it's actually in the database
        var retrievedPage = await _dbContext.Pages.FirstOrDefaultAsync(p => p.Id == page.Id);
        Assert.NotNull(retrievedPage);
        Assert.Equal(page.Title, retrievedPage.Title);
    }

    [Fact]
    public async Task GetPageByIdAsync_WithValidId_ShouldReturnPage()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var page = new CmsPage
        {
            Id = pageId,
            TenantId = tenantId,
            Title = "Test Page",
            Slug = "test-page",
            Version = 1
        };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetPageByIdAsync(tenantId, pageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageId, result.Id);
        Assert.Equal("Test Page", result.Title);
    }

    [Fact]
    public async Task GetPageByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var invalidPageId = Guid.NewGuid();

        // Act
        var result = await _repository.GetPageByIdAsync(tenantId, invalidPageId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task PageSlugExistsAsync_WithExistingSlug_ShouldReturnTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = "Existing Page",
            Slug = "existing-page",
            Version = 1
        };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _repository.PageSlugExistsAsync(tenantId, "existing-page");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task PageSlugExistsAsync_WithNonExistentSlug_ShouldReturnFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var exists = await _repository.PageSlugExistsAsync(tenantId, "non-existent-slug");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task UpdatePageAsync_WithValidPage_ShouldUpdateVersion()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = "Original Title",
            Slug = "original-slug",
            Version = 1
        };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        page.Title = "Updated Title";
        page.Version = 2;
        var result = await _repository.UpdatePageAsync(tenantId, page);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Version);

        var retrievedPage = await _dbContext.Pages.FirstOrDefaultAsync(p => p.Id == page.Id);
        Assert.Equal("Updated Title", retrievedPage?.Title);
    }

    [Fact]
    public async Task DeletePageAsync_WithValidId_ShouldRemoveFromDatabase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var page = new CmsPage
        {
            Id = pageId,
            TenantId = tenantId,
            Title = "Page to Delete",
            Slug = "page-to-delete",
            Version = 1
        };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.DeletePageAsync(tenantId, pageId);

        // Assert
        var deletedPage = await _dbContext.Pages.FirstOrDefaultAsync(p => p.Id == pageId);
        Assert.Null(deletedPage);
    }

    [Fact]
    public async Task GetPagesByTenantAsync_ShouldReturnOnlyTenantPages()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid();
        var tenantId2 = Guid.NewGuid();

        var page1 = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId1, Title = "Page 1", Slug = "page-1", Version = 1 };
        var page2 = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId1, Title = "Page 2", Slug = "page-2", Version = 1 };
        var page3 = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId2, Title = "Page 3", Slug = "page-3", Version = 1 };

        _dbContext.Pages.AddRange(page1, page2, page3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetPagesByTenantAsync(tenantId1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal(tenantId1, p.TenantId));
    }

    #endregion

    #region Section Repository Tests

    [Fact]
    public async Task AddSectionAsync_WithValidSection_ShouldPersist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId, Title = "Test", Slug = "test", Version = 1 };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = page.Id,
            Type = "Hero",
            Order = 0,
            Layout = SectionLayout.FullWidth
        };

        // Act
        var result = await _repository.AddSectionAsync(tenantId, page.Id, section);

        // Assert
        Assert.NotNull(result);
        var retrievedSection = await _dbContext.Sections.FirstOrDefaultAsync(s => s.Id == section.Id);
        Assert.NotNull(retrievedSection);
    }

    [Fact]
    public async Task RemoveSectionAsync_WithValidId_ShouldDelete()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId, Title = "Test", Slug = "test", Version = 1 };
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = page.Id,
            Type = "Hero",
            Order = 0,
            Layout = SectionLayout.FullWidth
        };
        page.Sections = new List<CmsSection> { section };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.RemoveSectionAsync(tenantId, page.Id, section.Id);

        // Assert
        var deletedSection = await _dbContext.Sections.FirstOrDefaultAsync(s => s.Id == section.Id);
        Assert.Null(deletedSection);
    }

    #endregion

    #region Component Repository Tests

    [Fact]
    public async Task AddComponentAsync_WithValidComponent_ShouldPersist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId, Title = "Test", Slug = "test", Version = 1 };
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = page.Id,
            Type = "Hero",
            Order = 0,
            Layout = SectionLayout.FullWidth
        };
        page.Sections = new List<CmsSection> { section };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        var component = new CmsComponent
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            Type = "Button",
            Content = "Click Me",
            IsVisible = true,
            Order = 0
        };

        // Act
        var result = await _repository.AddComponentAsync(tenantId, page.Id, section.Id, component);

        // Assert
        Assert.NotNull(result);
        var retrievedComponent = await _dbContext.Components.FirstOrDefaultAsync(c => c.Id == component.Id);
        Assert.NotNull(retrievedComponent);
    }

    [Fact]
    public async Task UpdateComponentAsync_WithValidComponent_ShouldUpdate()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId, Title = "Test", Slug = "test", Version = 1 };
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = page.Id,
            Type = "Hero",
            Order = 0,
            Layout = SectionLayout.FullWidth
        };
        var component = new CmsComponent
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            Type = "Button",
            Content = "Original Content",
            IsVisible = true,
            Order = 0
        };
        section.Components = new List<CmsComponent> { component };
        page.Sections = new List<CmsSection> { section };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        component.Content = "Updated Content";
        var result = await _repository.UpdateComponentAsync(tenantId, page.Id, section.Id, component.Id, component);

        // Assert
        Assert.Equal("Updated Content", result.Content);
    }

    [Fact]
    public async Task RemoveComponentAsync_WithValidId_ShouldDelete()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page = new CmsPage { Id = Guid.NewGuid(), TenantId = tenantId, Title = "Test", Slug = "test", Version = 1 };
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = page.Id,
            Type = "Hero",
            Order = 0,
            Layout = SectionLayout.FullWidth
        };
        var component = new CmsComponent
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            Type = "Button",
            Content = "Content",
            IsVisible = true,
            Order = 0
        };
        section.Components = new List<CmsComponent> { component };
        page.Sections = new List<CmsSection> { section };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.RemoveComponentAsync(tenantId, page.Id, section.Id, component.Id);

        // Assert
        var deletedComponent = await _dbContext.Components.FirstOrDefaultAsync(c => c.Id == component.Id);
        Assert.Null(deletedComponent);
    }

    #endregion

    #region Component Definition Tests

    [Fact]
    public async Task GetComponentDefinitionsAsync_ShouldReturnAllDefinitions()
    {
        // Act
        var result = await _repository.GetComponentDefinitionsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, c => c.ComponentType == "Button");
        Assert.Contains(result, c => c.ComponentType == "TextBlock");
    }

    [Fact]
    public async Task GetComponentDefinitionAsync_WithValidType_ShouldReturnDefinition()
    {
        // Act
        var result = await _repository.GetComponentDefinitionAsync("Button");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Button", result.ComponentType);
        Assert.Equal("Button", result.DisplayName);
    }

    #endregion
}
