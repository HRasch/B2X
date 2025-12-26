using B2Connect.LayoutService.Data;
using B2Connect.LayoutService.Models;

namespace B2Connect.LayoutService.Services;

/// <summary>
/// Layout Service - Business logic for CMS page building
/// Implements ILayoutService with minimal code to pass tests
/// </summary>
public class LayoutService : ILayoutService
{
    private readonly ILayoutRepository _repository;

    public LayoutService(ILayoutRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #region Page Operations

    public async Task<CmsPage> CreatePageAsync(Guid tenantId, CreatePageRequest request)
    {
        // Validate input (RED → GREEN: add minimal validation)
        if (string.IsNullOrWhiteSpace(request?.Title))
            throw new ArgumentException("Title is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request?.Slug))
            throw new ArgumentException("Slug is required", nameof(request));

        // Check for duplicate slug
        var slugExists = await _repository.PageSlugExistsAsync(tenantId, request.Slug);
        if (slugExists)
            throw new InvalidOperationException($"Page slug '{request.Slug}' already exists for this tenant");

        // Create page entity
        var page = new CmsPage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title,
            Slug = request.Slug,
            Description = request.Description,
            Sections = new List<CmsSection>(),
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Persist and return
        return await _repository.CreatePageAsync(tenantId, page);
    }

    public async Task<CmsPage> GetPageByIdAsync(Guid tenantId, Guid pageId)
    {
        return await _repository.GetPageByIdAsync(tenantId, pageId);
    }

    public async Task<List<CmsPage>> GetPagesByTenantAsync(Guid tenantId)
    {
        return await _repository.GetPagesByTenantAsync(tenantId);
    }

    public async Task<CmsPage> UpdatePageAsync(Guid tenantId, Guid pageId, UpdatePageRequest request)
    {
        // Get existing page
        var page = await _repository.GetPageByIdAsync(tenantId, pageId)
            ?? throw new KeyNotFoundException($"Page '{pageId}' not found");

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request?.Title))
            page.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request?.Description))
            page.Description = request.Description;

        if (request?.Visibility.HasValue == true)
            page.Visibility = request.Visibility.Value;

        // Increment version and update timestamp
        page.Version++;
        page.UpdatedAt = DateTime.UtcNow;

        // Persist and return
        return await _repository.UpdatePageAsync(tenantId, page);
    }

    public async Task<bool> DeletePageAsync(Guid tenantId, Guid pageId)
    {
        await _repository.DeletePageAsync(tenantId, pageId);
        return true;
    }

    #endregion

    #region Section Operations

    public async Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, AddSectionRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.Type))
            throw new ArgumentException("Section type is required", nameof(request));

        // Get current page to determine next order
        var page = await _repository.GetPageByIdAsync(tenantId, pageId)
            ?? throw new KeyNotFoundException($"Page '{pageId}' not found");

        // Create section
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = pageId,
            Type = request.Type,
            Layout = request.Layout,
            Order = page.Sections.Count, // Add to end
            Settings = request.Settings ?? new Dictionary<string, object>(),
            Components = new List<CmsComponent>(),
            IsVisible = true,
            CreatedAt = DateTime.UtcNow
        };

        // Persist and return
        return await _repository.AddSectionAsync(tenantId, pageId, section);
    }

    public async Task<bool> RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId)
    {
        await _repository.RemoveSectionAsync(tenantId, pageId, sectionId);
        return true;
    }

    public async Task<List<CmsSection>> ReorderSectionsAsync(Guid tenantId, Guid pageId, List<(Guid SectionId, int Order)> order)
    {
        return await _repository.ReorderSectionsAsync(tenantId, pageId, order);
    }

    #endregion

    #region Component Operations

    public async Task<CmsComponent> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, AddComponentRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.Type))
            throw new ArgumentException("Component type is required", nameof(request));

        // Create component
        var component = new CmsComponent
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Type = request.Type,
            Content = request.Content,
            Variables = request.Variables ?? new List<ComponentVariable>(),
            Styling = request.Styling ?? new Dictionary<string, string>(),
            IsVisible = true,
            Order = 0,
            CreatedAt = DateTime.UtcNow
        };

        // Persist and return
        return await _repository.AddComponentAsync(tenantId, pageId, sectionId, component);
    }

    public async Task<CmsComponent> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, UpdateComponentRequest request)
    {
        // Get existing component
        var page = await _repository.GetPageByIdAsync(tenantId, pageId)
            ?? throw new KeyNotFoundException($"Page '{pageId}' not found");

        var section = page.Sections.FirstOrDefault(s => s.Id == sectionId)
            ?? throw new KeyNotFoundException($"Section '{sectionId}' not found");

        var existingComponent = section.Components.FirstOrDefault(c => c.Id == componentId)
            ?? throw new KeyNotFoundException($"Component '{componentId}' not found");

        // Update properties
        existingComponent.Content = request?.Content ?? existingComponent.Content;
        existingComponent.Variables = request?.Variables ?? existingComponent.Variables;
        existingComponent.Styling = request?.Styling ?? existingComponent.Styling;
        existingComponent.IsVisible = request?.IsVisible ?? existingComponent.IsVisible;

        // Persist and return
        return await _repository.UpdateComponentAsync(tenantId, pageId, sectionId, existingComponent);
    }

    public async Task<bool> RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId)
    {
        await _repository.RemoveComponentAsync(tenantId, pageId, sectionId, componentId);
        return true;
    }

    #endregion

    #region Component Definitions

    public async Task<List<ComponentDefinition>> GetComponentDefinitionsAsync()
    {
        return await _repository.GetComponentDefinitionsAsync();
    }

    public async Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType)
    {
        return await _repository.GetComponentDefinitionAsync(componentType);
    }

    #endregion

    #region Preview

    public async Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId)
    {
        return await _repository.GeneratePreviewHtmlAsync(tenantId, pageId);
    }

    #endregion
}
