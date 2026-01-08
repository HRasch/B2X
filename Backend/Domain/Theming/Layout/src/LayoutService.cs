using B2X.LayoutService.Data;
using B2X.LayoutService.Models;

namespace B2X.LayoutService.Services;

/// <summary>
/// Layout Service - Business logic for CMS page building
/// Implements localization support with language-aware DTOs
/// </summary>
public class LayoutService : ILayoutService
{
    private readonly ILayoutRepository _repository;
    private readonly string _defaultLanguage = "en";

    public LayoutService(ILayoutRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #region Page Operations

    public async Task<CmsPageDto> CreatePageAsync(Guid tenantId, CreatePageRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request?.Title))
        {
            throw new ArgumentException("Title is required", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request?.Slug))
        {
            throw new ArgumentException("Slug is required", nameof(request));
        }

        var slugExists = await _repository.PageSlugExistsAsync(tenantId, request.Slug, cancellationToken).ConfigureAwait(false);
        if (slugExists)
        {
            throw new InvalidOperationException($"Page slug '{request.Slug}' already exists");
        }

        var titleTranslations = new Dictionary<string, string>(StringComparer.Ordinal);
        var slugTranslations = new Dictionary<string, string>(StringComparer.Ordinal);
        var descriptionTranslations = new Dictionary<string, string>(StringComparer.Ordinal);

        if (request.Translations != null)
        {
            foreach (var kvp in request.Translations)
            {
                titleTranslations[kvp.Key] = kvp.Value.Title;
                slugTranslations[kvp.Key] = kvp.Value.Slug;
                descriptionTranslations[kvp.Key] = kvp.Value.Description;
            }
        }

        var page = new CmsPage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title,
            Slug = request.Slug,
            Description = request.Description,
            TitleTranslations = titleTranslations,
            SlugTranslations = slugTranslations,
            DescriptionTranslations = descriptionTranslations,
            Sections = new List<CmsSection>(),
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPage = await _repository.CreatePageAsync(tenantId, page, cancellationToken).ConfigureAwait(false);
        return MapPageToDto(createdPage, _defaultLanguage);
    }

    public async Task<CmsPageDto?> GetPageByIdAsync(Guid tenantId, Guid pageId, string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code is required", nameof(languageCode));
        }

        var page = await _repository.GetPageByIdAsync(tenantId, pageId, cancellationToken).ConfigureAwait(false);
        return page == null ? null : MapPageToDto(page, languageCode);
    }

    public async Task<IList<CmsPageDto>> GetPagesByTenantAsync(Guid tenantId, string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code is required", nameof(languageCode));
        }

        var pages = await _repository.GetPagesByTenantAsync(tenantId, cancellationToken).ConfigureAwait(false);
        return pages.Select(p => MapPageToDto(p, languageCode)).ToList();
    }

    public async Task<CmsPageDto> UpdatePageAsync(Guid tenantId, Guid pageId, UpdatePageRequest request, string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code is required", nameof(languageCode));
        }

        var page = await _repository.GetPageByIdAsync(tenantId, pageId, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException($"Page {pageId} not found");

        if (!string.IsNullOrWhiteSpace(request?.Title))
        {
            page.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request?.Slug))
        {
            page.Slug = request.Slug;
        }

        if (!string.IsNullOrWhiteSpace(request?.Description))
        {
            page.Description = request.Description;
        }

        if (request?.Translations != null)
        {
            page.TitleTranslations ??= new Dictionary<string, string>(StringComparer.Ordinal);
            page.SlugTranslations ??= new Dictionary<string, string>(StringComparer.Ordinal);
            page.DescriptionTranslations ??= new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var kvp in request.Translations)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value.Title))
                {
                    page.TitleTranslations[kvp.Key] = kvp.Value.Title;
                }

                if (!string.IsNullOrWhiteSpace(kvp.Value.Slug))
                {
                    page.SlugTranslations[kvp.Key] = kvp.Value.Slug;
                }

                if (!string.IsNullOrWhiteSpace(kvp.Value.Description))
                {
                    page.DescriptionTranslations[kvp.Key] = kvp.Value.Description;
                }
            }
        }

        if (request?.Visibility.HasValue == true)
        {
            page.Visibility = request.Visibility.Value;
        }

        page.UpdatedAt = DateTime.UtcNow;
        page.Version++;

        var updated = await _repository.UpdatePageAsync(tenantId, page, cancellationToken).ConfigureAwait(false);
        return MapPageToDto(updated, languageCode);
    }

    public Task<bool> DeletePageAsync(Guid tenantId, Guid pageId, CancellationToken cancellationToken = default)
    {
        return _repository.DeletePageAsync(tenantId, pageId, cancellationToken);
    }

    #endregion

    #region Section Operations

    public async Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, AddSectionRequest request, CancellationToken cancellationToken = default)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request?.Type))
        {
            throw new ArgumentException("Section type is required", nameof(request));
        }

        // Get current page to determine next order
        var page = await _repository.GetPageByIdAsync(tenantId, pageId, cancellationToken)
.ConfigureAwait(false) ?? throw new KeyNotFoundException($"Page '{pageId}' not found");

        // Create section
        var section = new CmsSection
        {
            Id = Guid.NewGuid(),
            PageId = pageId,
            Type = request.Type,
            Layout = request.Layout,
            Order = page.Sections.Count, // Add to end
            Settings = request.Settings ?? new Dictionary<string, object>(StringComparer.Ordinal),
            Components = new List<CmsComponent>(),
            IsVisible = true,
            CreatedAt = DateTime.UtcNow
        };

        // Persist and return
        return await _repository.AddSectionAsync(tenantId, pageId, section, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId, CancellationToken cancellationToken = default)
    {
        await _repository.RemoveSectionAsync(tenantId, pageId, sectionId, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public Task<bool> ReorderSectionsAsync(Guid tenantId, Guid pageId, IList<(Guid SectionId, int Order)> order, CancellationToken cancellationToken = default)
    {
        return _repository.ReorderSectionsAsync(tenantId, pageId, order, cancellationToken);
    }

    #endregion

    #region Component Operations

    public async Task<CmsComponentDto> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, AddComponentRequest request, string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code is required", nameof(languageCode));
        }

        if (string.IsNullOrWhiteSpace(request?.Type))
        {
            throw new ArgumentException("Component type is required", nameof(request));
        }

        var component = new CmsComponent
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Type = request.Type,
            Content = request.Content,
            ContentTranslations = request.ContentTranslations ?? new Dictionary<string, string>(StringComparer.Ordinal),
            Order = request.Order ?? 0,
            IsVisible = request.IsVisible ?? true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddComponentAsync(tenantId, pageId, sectionId, component, cancellationToken).ConfigureAwait(false);
        return MapComponentToDto(created, languageCode);
    }

    public async Task<CmsComponentDto> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, UpdateComponentRequest request, string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code is required", nameof(languageCode));
        }

        var page = await _repository.GetPageByIdAsync(tenantId, pageId, cancellationToken)
            .ConfigureAwait(false) ?? throw new KeyNotFoundException($"Page {pageId} not found");

        var section = page.Sections.FirstOrDefault(s => s.Id == sectionId)
            ?? throw new KeyNotFoundException($"Section {sectionId} not found");

        var component = section.Components.FirstOrDefault(c => c.Id == componentId)
            ?? throw new KeyNotFoundException($"Component {componentId} not found");

        if (!string.IsNullOrWhiteSpace(request?.Content))
        {
            component.Content = request.Content;
        }

        if (request?.ContentTranslations != null)
        {
            component.ContentTranslations ??= new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var kvp in request.ContentTranslations)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value))
                {
                    component.ContentTranslations[kvp.Key] = kvp.Value;
                }
            }
        }

        if (request?.Order.HasValue == true)
        {
            component.Order = request.Order.Value;
        }

        if (request?.IsVisible.HasValue == true)
        {
            component.IsVisible = request.IsVisible.Value;
        }

        var updated = await _repository.UpdateComponentAsync(tenantId, pageId, sectionId, componentId, component, cancellationToken).ConfigureAwait(false);
        return MapComponentToDto(updated, languageCode);
    }

    public Task<bool> RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId, CancellationToken cancellationToken = default)
    {
        return _repository.RemoveComponentAsync(tenantId, pageId, sectionId, componentId, cancellationToken);
    }

    #endregion

    #region Component Definitions

    public Task<IList<ComponentDefinition>> GetComponentDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetComponentDefinitionsAsync(cancellationToken);
    }

    public Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType, CancellationToken cancellationToken = default)
    {
        return _repository.GetComponentDefinitionAsync(componentType, cancellationToken);
    }

    #endregion

    #region Preview

    public async Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId, string languageCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            throw new ArgumentException("Language code is required", nameof(languageCode));
        }

        var page = await _repository.GetPageByIdAsync(tenantId, pageId, cancellationToken)
.ConfigureAwait(false) ?? throw new KeyNotFoundException($"Page {pageId} not found");

        return GenerateHtml(page, languageCode);
    }

    #endregion

    #region Helper Methods

    private CmsPageDto MapPageToDto(CmsPage page, string languageCode)
    {
        return new CmsPageDto
        {
            Id = page.Id,
            TenantId = page.TenantId,
            Language = languageCode,
            Title = LocalizationHelper.GetLocalizedValue(page.Title, page.TitleTranslations, languageCode, _defaultLanguage),
            Slug = LocalizationHelper.GetLocalizedValue(page.Slug, page.SlugTranslations, languageCode, _defaultLanguage),
            Description = LocalizationHelper.GetLocalizedValue(page.Description, page.DescriptionTranslations, languageCode, _defaultLanguage),
            Visibility = page.Visibility,
            PublishedAt = page.PublishedAt,
            Version = page.Version,
            CreatedAt = page.CreatedAt,
            UpdatedAt = page.UpdatedAt,
            Sections = page.Sections.OrderBy(s => s.Order).Select(s => MapSectionToDto(s, languageCode)).ToList()
        };
    }

    private CmsSectionDto MapSectionToDto(CmsSection section, string languageCode)
    {
        return new CmsSectionDto
        {
            Id = section.Id,
            PageId = section.PageId,
            Type = section.Type,
            Order = section.Order,
            Layout = section.Layout,
            IsVisible = section.IsVisible,
            Components = section.Components.OrderBy(c => c.Order).Select(c => MapComponentToDto(c, languageCode)).ToList()
        };
    }

    private CmsComponentDto MapComponentToDto(CmsComponent component, string languageCode)
    {
        return new CmsComponentDto
        {
            Id = component.Id,
            SectionId = component.SectionId,
            Language = languageCode,
            Type = component.Type,
            Content = LocalizationHelper.GetLocalizedValue(component.Content, component.ContentTranslations, languageCode, _defaultLanguage),
            IsVisible = component.IsVisible,
            Order = component.Order,
            CreatedAt = component.CreatedAt
        };
    }

    private string GenerateHtml(CmsPage page, string languageCode)
    {
        var html = new System.Text.StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine($"<title>{LocalizationHelper.GetLocalizedValue(page.Title, page.TitleTranslations, languageCode, _defaultLanguage)}</title>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        foreach (var section in page.Sections.OrderBy(s => s.Order))
        {
            html.AppendLine($"<section class=\"section-{section.Type}\">");
            foreach (var component in section.Components.OrderBy(c => c.Order))
            {
                if (component.IsVisible)
                {
                    var content = LocalizationHelper.GetLocalizedValue(component.Content, component.ContentTranslations, languageCode, _defaultLanguage);
                    html.AppendLine($"<div class=\"component-{component.Type}\">{content}</div>");
                }
            }
            html.AppendLine("</section>");
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");
        return html.ToString();
    }

    #endregion
}
