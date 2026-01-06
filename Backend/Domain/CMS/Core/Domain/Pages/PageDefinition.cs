using System;
using System.Collections.Generic;
using System.Linq;

namespace B2Connect.CMS.Core.Domain.Pages
{
    /// <summary>
    /// Page definition stored in database
    /// Contains layout and widget configuration
    /// </summary>
    public class PageDefinition : AggregateRoot
    {
        public string TenantId { get; set; } = null!;
        public string PageType { get; set; } = null!; // 'home', 'product-listing', 'about', etc.
        public string PagePath { get; set; } = null!; // '/home', '/about-us', etc.
        public string PageTitle { get; set; } = null!;
        public string PageDescription { get; set; } = null!;
        public string MetaKeywords { get; set; } = null!;
        public string TemplateLayout { get; set; } = null!; // 'sidebar', 'full-width', 'three-column'
        public List<PageRegion> Regions { get; set; } = new();
        public Dictionary<string, object> GlobalSettings { get; set; } = new();
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; }

        // Template Override Support (ADR-030)
        public bool IsTemplateOverride { get; set; } // True if this is a tenant-specific override
        public string? BaseTemplateKey { get; set; } // Reference to base template if this is an override
        public Dictionary<string, string> OverrideSections { get; set; } = new(); // Section-level overrides
        public TemplateOverrideMetadata? OverrideMetadata { get; set; }

        public PageDefinition()
        {
        }

        public PageDefinition(
            string tenantId,
            string pageType,
            string pagePath,
            string pageTitle,
            string templateLayout)
        {
            TenantId = tenantId;
            PageType = pageType;
            PagePath = pagePath;
            PageTitle = pageTitle;
            TemplateLayout = templateLayout;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Version = 1;
        }

        public void PublishPage()
        {
            IsPublished = true;
            PublishedAt = DateTime.UtcNow;
        }

        public void UnpublishPage()
        {
            IsPublished = false;
        }

        public void AddRegion(PageRegion region)
        {
            if (Regions.Any(r => r.Name == region.Name))
            {
                throw new InvalidOperationException($"Region '{region.Name}' already exists");
            }

            region.PageDefinitionId = Id;
            Regions.Add(region);
        }

        public void RemoveRegion(string regionName)
        {
            var region = Regions.FirstOrDefault(r => r.Name == regionName);
            if (region == null)
            {
                return;
            }

            Regions.Remove(region);
        }
    }

    /// <summary>
    /// Region/Slot within a page where widgets can be placed
    /// </summary>
    public class PageRegion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PageDefinitionId { get; set; } = null!;
        public string Name { get; set; } = null!; // 'header', 'sidebar', 'main', 'footer'
        public int Order { get; set; }
        public int MaxWidgets { get; set; } = -1; // -1 = unlimited
        public List<WidgetInstance> Widgets { get; set; } = new();
        public Dictionary<string, object> RegionSettings { get; set; } = new();

        public WidgetInstance AddWidget(string widgetTypeId, Dictionary<string, object> settings)
        {
            if (MaxWidgets > 0 && Widgets.Count >= MaxWidgets)
            {
                throw new InvalidOperationException(
                    $"Region '{Name}' has reached maximum widget limit of {MaxWidgets}");
            }

            var widget = new WidgetInstance
            {
                WidgetTypeId = widgetTypeId,
                Order = Widgets.Count + 1,
                Settings = settings,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            };

            Widgets.Add(widget);
            return widget;
        }

        public void RemoveWidget(string widgetId)
        {
            var widget = Widgets.FirstOrDefault(w => w.Id == widgetId);
            if (widget == null)
            {
                return;
            }

            Widgets.Remove(widget);
            ReorderWidgets();
        }

        public void ReorderWidgets()
        {
            for (int i = 0; i < Widgets.Count; i++)
            {
                Widgets[i].Order = i + 1;
            }
        }
    }

    /// <summary>
    /// Instance of a widget placed on a page
    /// </summary>
    public class WidgetInstance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string WidgetTypeId { get; set; } = null!; // Reference to widget definition
        public int Order { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
