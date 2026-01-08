using B2X.LayoutService.Models;

namespace B2X.LayoutService.Data;

/// <summary>
/// Repository Interface für Layout Service
/// Definiert alle Daten-Operationen mit Unterstützung für mehrsprachige Lokalisierung
/// </summary>
public interface ILayoutRepository
{
    #region Page Operations

    /// <summary>Create a new page</summary>
    Task<CmsPage> CreatePageAsync(Guid tenantId, CmsPage page, CancellationToken cancellationToken = default);

    /// <summary>Get page by ID - returns entity with full translation dictionaries</summary>
    Task<CmsPage?> GetPageByIdAsync(Guid tenantId, Guid pageId, CancellationToken cancellationToken = default);

    /// <summary>Get all pages for a tenant - returns entities with full translation dictionaries</summary>
    Task<IList<CmsPage>> GetPagesByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Check if page slug already exists</summary>
    Task<bool> PageSlugExistsAsync(Guid tenantId, string slug, CancellationToken cancellationToken = default);

    /// <summary>Update an existing page</summary>
    Task<CmsPage> UpdatePageAsync(Guid tenantId, CmsPage page, CancellationToken cancellationToken = default);

    /// <summary>Delete a page</summary>
    Task<bool> DeletePageAsync(Guid tenantId, Guid pageId, CancellationToken cancellationToken = default);

    #endregion

    #region Section Operations

    /// <summary>Add a section to a page</summary>
    Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, CmsSection section, CancellationToken cancellationToken = default);

    /// <summary>Remove a section from a page</summary>
    Task<bool> RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId, CancellationToken cancellationToken = default);

    /// <summary>Update section order</summary>
    Task<bool> ReorderSectionsAsync(Guid tenantId, Guid pageId, IList<(Guid SectionId, int Order)> order, CancellationToken cancellationToken = default);

    #endregion

    #region Component Operations

    /// <summary>Add a component to a section</summary>
    Task<CmsComponent> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, CmsComponent component, CancellationToken cancellationToken = default);

    /// <summary>Update a component</summary>
    Task<CmsComponent> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, CmsComponent component, CancellationToken cancellationToken = default);

    /// <summary>Remove a component from a section</summary>
    Task<bool> RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, CancellationToken cancellationToken = default);

    #endregion

    #region Component Definitions

    /// <summary>Get all available component definitions</summary>
    Task<IList<ComponentDefinition>> GetComponentDefinitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>Get component definition by type</summary>
    Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType, CancellationToken cancellationToken = default);

    #endregion

    #region Preview Generation

    /// <summary>Generate HTML preview for a page</summary>
    Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Layout Service Interface
/// Business logic for page building with language-aware localization
/// </summary>
public interface ILayoutService
{
    #region Page Operations

    /// <summary>Create a new page with translations</summary>
    Task<CmsPageDto> CreatePageAsync(Guid tenantId, CreatePageRequest request, CancellationToken cancellationToken = default);

    /// <summary>Get page by ID in specific language</summary>
    Task<CmsPageDto?> GetPageByIdAsync(Guid tenantId, Guid pageId, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>Get all pages for tenant in specific language</summary>
    Task<IList<CmsPageDto>> GetPagesByTenantAsync(Guid tenantId, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>Update an existing page</summary>
    Task<CmsPageDto> UpdatePageAsync(Guid tenantId, Guid pageId, UpdatePageRequest request, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>Delete a page</summary>
    Task<bool> DeletePageAsync(Guid tenantId, Guid pageId, CancellationToken cancellationToken = default);

    #endregion

    #region Section Operations

    Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, AddSectionRequest request, CancellationToken cancellationToken = default);
    Task<bool> RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId, CancellationToken cancellationToken = default);
    Task<bool> ReorderSectionsAsync(Guid tenantId, Guid pageId, IList<(Guid SectionId, int Order)> order, CancellationToken cancellationToken = default);

    #endregion

    #region Component Operations

    Task<CmsComponentDto> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, AddComponentRequest request, string languageCode, CancellationToken cancellationToken = default);
    Task<CmsComponentDto> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, UpdateComponentRequest request, string languageCode, CancellationToken cancellationToken = default);
    Task<bool> RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, CancellationToken cancellationToken = default);

    #endregion

    #region Component Definitions

    Task<IList<ComponentDefinition>> GetComponentDefinitionsAsync(CancellationToken cancellationToken = default);
    Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType, CancellationToken cancellationToken = default);

    #endregion

    #region Preview

    Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId, string languageCode, CancellationToken cancellationToken = default);

    #endregion
}
