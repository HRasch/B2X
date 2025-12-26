using Microsoft.EntityFrameworkCore;
using B2Connect.LayoutService.Models;

namespace B2Connect.LayoutService.Data;

/// <summary>
/// Entity Framework Core implementation of ILayoutRepository
/// Handles all data access for the Layout Service
/// </summary>
public class LayoutRepository : ILayoutRepository
{
    private readonly LayoutDbContext _context;

    public LayoutRepository(LayoutDbContext context)
    {
        _context = context;
    }

    #region Page Operations

    public async Task<CmsPage> CreatePageAsync(Guid tenantId, CmsPage page)
    {
        page.TenantId = tenantId;
        page.CreatedAt = DateTime.UtcNow;
        page.UpdatedAt = DateTime.UtcNow;

        _context.Pages.Add(page);
        await _context.SaveChangesAsync();

        return page;
    }

    public async Task<CmsPage?> GetPageByIdAsync(Guid tenantId, Guid pageId)
    {
        return await _context.Pages
            .AsNoTracking()
            .Include(p => p.Sections)
            .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(p => p.Id == pageId && p.TenantId == tenantId);
    }

    public async Task<List<CmsPage>> GetPagesByTenantAsync(Guid tenantId)
    {
        return await _context.Pages
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .Include(p => p.Sections)
            .ThenInclude(s => s.Components)
            .ToListAsync();
    }

    public async Task<bool> PageSlugExistsAsync(Guid tenantId, string slug)
    {
        return await _context.Pages
            .AnyAsync(p => p.TenantId == tenantId && p.Slug == slug);
    }

    public async Task<CmsPage> UpdatePageAsync(Guid tenantId, CmsPage page)
    {
        var existingPage = await _context.Pages
            .FirstOrDefaultAsync(p => p.Id == page.Id && p.TenantId == tenantId)
            ?? throw new KeyNotFoundException($"Page {page.Id} not found for tenant {tenantId}");

        existingPage.Title = page.Title;
        existingPage.Slug = page.Slug;
        existingPage.Description = page.Description;
        existingPage.Visibility = page.Visibility;
        existingPage.PublishedAt = page.PublishedAt;
        existingPage.ScheduledPublishAt = page.ScheduledPublishAt;
        existingPage.Version = page.Version;
        existingPage.UpdatedAt = DateTime.UtcNow;
        existingPage.UpdatedBy = page.UpdatedBy;

        await _context.SaveChangesAsync();

        return existingPage;
    }

    public async Task DeletePageAsync(Guid tenantId, Guid pageId)
    {
        var page = await _context.Pages
            .FirstOrDefaultAsync(p => p.Id == pageId && p.TenantId == tenantId)
            ?? throw new KeyNotFoundException($"Page {pageId} not found for tenant {tenantId}");

        _context.Pages.Remove(page);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Section Operations

    public async Task<CmsSection> AddSectionAsync(Guid tenantId, Guid pageId, CmsSection section)
    {
        var page = await _context.Pages
            .FirstOrDefaultAsync(p => p.Id == pageId && p.TenantId == tenantId)
            ?? throw new KeyNotFoundException($"Page {pageId} not found for tenant {tenantId}");

        section.PageId = pageId;
        section.CreatedAt = DateTime.UtcNow;

        _context.Sections.Add(section);
        await _context.SaveChangesAsync();

        return section;
    }

    public async Task RemoveSectionAsync(Guid tenantId, Guid pageId, Guid sectionId)
    {
        var section = await _context.Sections
            .FirstOrDefaultAsync(s => s.Id == sectionId && s.PageId == pageId)
            ?? throw new KeyNotFoundException($"Section {sectionId} not found");

        _context.Sections.Remove(section);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CmsSection>> ReorderSectionsAsync(Guid tenantId, Guid pageId, List<(Guid SectionId, int Order)> sectionOrders)
    {
        var sections = await _context.Sections
            .Where(s => s.PageId == pageId && sectionOrders.Select(so => so.SectionId).Contains(s.Id))
            .ToListAsync();

        foreach (var (sectionId, order) in sectionOrders)
        {
            var section = sections.FirstOrDefault(s => s.Id == sectionId);
            if (section != null)
            {
                section.Order = order;
            }
        }

        await _context.SaveChangesAsync();

        return sections.OrderBy(s => s.Order).ToList();
    }

    #endregion

    #region Component Operations

    public async Task<CmsComponent> AddComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, CmsComponent component)
    {
        var section = await _context.Sections
            .FirstOrDefaultAsync(s => s.Id == sectionId && s.PageId == pageId)
            ?? throw new KeyNotFoundException($"Section {sectionId} not found");

        component.SectionId = sectionId;
        component.CreatedAt = DateTime.UtcNow;

        _context.Components.Add(component);
        await _context.SaveChangesAsync();

        return component;
    }

    public async Task<CmsComponent> UpdateComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, CmsComponent component)
    {
        var existingComponent = await _context.Components
            .FirstOrDefaultAsync(c => c.Id == component.Id && c.SectionId == sectionId)
            ?? throw new KeyNotFoundException($"Component {component.Id} not found");

        existingComponent.Type = component.Type;
        existingComponent.Content = component.Content;
        existingComponent.Variables = component.Variables;
        existingComponent.DataBinding = component.DataBinding;
        existingComponent.Styling = component.Styling;
        existingComponent.IsVisible = component.IsVisible;
        existingComponent.Order = component.Order;

        await _context.SaveChangesAsync();

        return existingComponent;
    }

    public async Task RemoveComponentAsync(Guid tenantId, Guid pageId, Guid sectionId, Guid componentId)
    {
        var component = await _context.Components
            .FirstOrDefaultAsync(c => c.Id == componentId && c.SectionId == sectionId)
            ?? throw new KeyNotFoundException($"Component {componentId} not found");

        _context.Components.Remove(component);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Component Definition Operations

    public async Task<List<ComponentDefinition>> GetComponentDefinitionsAsync()
    {
        return await _context.ComponentDefinitions
            .AsNoTracking()
            .OrderBy(c => c.DisplayName)
            .ToListAsync();
    }

    public async Task<ComponentDefinition?> GetComponentDefinitionAsync(string componentType)
    {
        return await _context.ComponentDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ComponentType == componentType);
    }

    #endregion

    #region Preview Generation

    public async Task<string> GeneratePreviewHtmlAsync(Guid tenantId, Guid pageId)
    {
        var page = await GetPageByIdAsync(tenantId, pageId);
        if (page == null)
            throw new KeyNotFoundException($"Page {pageId} not found");

        var html = new System.Text.StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine($"<title>{System.Web.HttpUtility.HtmlEncode(page.Title)}</title>");
        html.AppendLine("<meta charset=\"UTF-8\">");
        html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto; margin: 0; padding: 20px; }");
        html.AppendLine("section { margin: 20px 0; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine($"<h1>{System.Web.HttpUtility.HtmlEncode(page.Title)}</h1>");

        foreach (var section in page.Sections.OrderBy(s => s.Order))
        {
            if (!section.IsVisible)
                continue;

            html.AppendLine($"<section data-layout=\"{section.Layout}\">");
            html.AppendLine($"<h2>{System.Web.HttpUtility.HtmlEncode(section.Type)}</h2>");

            foreach (var component in section.Components.OrderBy(c => c.Order))
            {
                if (!component.IsVisible)
                    continue;

                html.AppendLine($"<div class=\"component\" data-type=\"{component.Type}\">");
                html.AppendLine($"<p>{System.Web.HttpUtility.HtmlEncode(component.Content ?? "")}</p>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</section>");
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return await Task.FromResult(html.ToString());
    }

    #endregion
}
