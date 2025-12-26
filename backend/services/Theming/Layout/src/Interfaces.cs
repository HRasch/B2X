using B2Connect.LayoutService.Models;

namespace B2Connect.LayoutService.Data;

/// <summary>
/// Repository Interface für Layout Service
/// Definiert alle Daten-Operationen
/// </summary>
public interface ILayoutRepository
{
    #region Page Operations

    /// <summary>Create a new page</summary>
    Task<CmsPage> CreatePageAsync(Guid tenantId, CmsPage page);

    /// <summary>Get page by ID</summary>
    Task<CmsPage?> GetPageByIdAsync(Guid tenantId, Guid pageId);

    /// <summary>Get all pages for a tenant</summary>
    Task<List<CmsPage>> GetPagesByTenantAsync(Guid tenantId);

    /// <summary>Check if page slug already exists</summary>
    Task<bool> PageSlugExistsAsync(Guid tenantId, string slug);

    /// <summary>Update an existing page</summary>
    Task<CmsPage> UpdatePageAsync(Guid tenantId, CmsPage page);

    /// <summary>Delete a page</summary>
    Task<bool> DeletePageAsync(Guid tenantId, Guid pageId);

    #endregion

    #region Section Operations

    /// <summary>Add a section to a page</summary>
    Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, CmsSection section);

    /// <summary>Remove a section from a page</summary>
    Task<bool> RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId);

    /// <summary>Update section order</summary>
    Task<bool> ReorderSectionsAsync(Guid tenantId, Guid pageId, List<(Guid SectionId, int Order)> order);

    #endregion

    #region Component Operations

    /// <summary>Add a component to a section</summary>
    Task<CmsComponent> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, CmsComponent component);

    /// <summary>Update a component</summary>
    Task<CmsComponent> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, CmsComponent component);

    /// <summary>Remove a component</summary>
    Task<bool> RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId);

    #endregion

    #region Component Definitions

    /// <summary>Get all available component definitions</summary>
    Task<List<ComponentDefinition>> GetComponentDefinitionsAsync();

    /// <summary>Get component definition by type</summary>
    Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType);

    #endregion

    #region Preview Generation

    /// <summary>Generate HTML preview for a page</summary>
    Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId);

    #endregion
}

/// <summary>
/// Layout Service Interface
/// Business logic for page building
/// </summary>
public interface ILayoutService
{
    #region Page Operations

    Task<CmsPage> CreatePageAsync(Guid tenantId, CreatePageRequest request);
    Task<CmsPage?> GetPageByIdAsync(Guid tenantId, Guid pageId);
    Task<List<CmsPage>> GetPagesByTenantAsync(Guid tenantId);
    Task<CmsPage> UpdatePageAsync(Guid tenantId, Guid pageId, UpdatePageRequest request);
    Task<bool> DeletePageAsync(Guid tenantId, Guid pageId);

    #endregion

    #region Section Operations

    Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, AddSectionRequest request);
    Task<bool> RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId);
    Task<bool> ReorderSectionsAsync(Guid tenantId, Guid pageId, List<(Guid SectionId, int Order)> order);

    #endregion

    #region Component Operations

    Task<CmsComponent> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, AddComponentRequest request);
    Task<CmsComponent> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, UpdateComponentRequest request);
    Task<bool> RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId);

    #endregion

    #region Component Definitions

    Task<List<ComponentDefinition>> GetComponentDefinitionsAsync();
    Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType);

    #endregion

    #region Preview

    Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId);

    #endregion
}
