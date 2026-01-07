using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using B2Connect.CMS.Application.Widgets;
using B2Connect.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace B2Connect.CMS.Application.Pages
{
    /// <summary>
    /// Query to retrieve page definition from database
    /// Used by store frontend to render page with widgets
    /// </summary>
    public class GetPageDefinitionQuery
    {
        public string? TenantId { get; init; }
        public string? PagePath { get; init; }
    }

    public interface IPageRepository
    {
        Task<PageDefinition?> GetPageByPathAsync(
            string tenantId,
            string pagePath,
            CancellationToken cancellationToken);
    }

    public class GetPageDefinitionQueryHandler
    {
        private readonly IPageRepository _repository;
        private readonly IWidgetRegistry _widgetRegistry;
        private readonly ILogger<GetPageDefinitionQueryHandler> _logger;

        public GetPageDefinitionQueryHandler(
            IPageRepository repository,
            IWidgetRegistry widgetRegistry,
            ILogger<GetPageDefinitionQueryHandler> logger)
        {
            _repository = repository;
            _widgetRegistry = widgetRegistry;
            _logger = logger;
        }

        public async Task<PageDefinitionDto> Handle(
            GetPageDefinitionQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching page definition: TenantId={TenantId}, PagePath={PagePath}",
                request.TenantId,
                request.PagePath);

            if (string.IsNullOrWhiteSpace(request.TenantId) || string.IsNullOrWhiteSpace(request.PagePath))
            {
                throw new InvalidOperationException("TenantId and PagePath must be provided");
            }

            var page = await _repository.GetPageByPathAsync(
                request.TenantId!,
                request.PagePath!,
                cancellationToken).ConfigureAwait(false);

            if (page?.IsPublished != true)
            {
                throw new InvalidOperationException($"Page '{request.PagePath}' not found");
            }

            return MapToDto(page);
        }

        private PageDefinitionDto MapToDto(PageDefinition page)
        {
            return new PageDefinitionDto
            {
                Id = page.Id,
                TenantId = page.TenantId,
                PageType = page.PageType,
                PagePath = page.PagePath,
                PageTitle = page.PageTitle,
                PageDescription = page.PageDescription,
                MetaKeywords = page.MetaKeywords,
                TemplateLayout = page.TemplateLayout,
                GlobalSettings = page.GlobalSettings,
                Regions = page.Regions.Select(r => MapRegionToDto(r)).ToList(),
                IsPublished = page.IsPublished,
                PublishedAt = page.PublishedAt
            };
        }

        private RegionDto MapRegionToDto(PageRegion region)
        {
            return new RegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Order = region.Order,
                Settings = region.RegionSettings,
                Widgets = region.Widgets
                    .Where(w => w.IsEnabled)
                    .OrderBy(w => w.Order)
                    .Select(w => MapWidgetToDto(w))
                    .ToList()
            };
        }

        private WidgetInstanceDto MapWidgetToDto(WidgetInstance widget)
        {
            var widgetDef = _widgetRegistry.GetWidget(widget.WidgetTypeId);

            return new WidgetInstanceDto
            {
                Id = widget.Id,
                WidgetTypeId = widget.WidgetTypeId,
                ComponentPath = widgetDef.ComponentPath,
                Order = widget.Order,
                Settings = widget.Settings
            };
        }
    }

    public class PageDefinitionDto
    {
        public string Id { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string PageType { get; set; } = null!;
        public string PagePath { get; set; } = null!;
        public string PageTitle { get; set; } = null!;
        public string PageDescription { get; set; } = null!;
        public string MetaKeywords { get; set; } = null!;
        public string TemplateLayout { get; set; } = null!;
        public Dictionary<string, object> GlobalSettings { get; set; } = new(StringComparer.Ordinal);
        public List<RegionDto> Regions { get; set; } = new();
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class RegionDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Order { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new(StringComparer.Ordinal);
        public List<WidgetInstanceDto> Widgets { get; set; } = new();
    }

    public class WidgetInstanceDto
    {
        public string Id { get; set; } = null!;
        public string WidgetTypeId { get; set; } = null!;
        public string ComponentPath { get; set; } = null!;
        public int Order { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new(StringComparer.Ordinal);
    }
}
