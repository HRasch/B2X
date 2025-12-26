using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolverine;
using Microsoft.Extensions.Logging;
using B2Connect.CMS.Core.Domain.Pages;
using B2Connect.CMS.Application.Widgets;

namespace B2Connect.CMS.Application.Pages
{
    /// <summary>
    /// Query to retrieve page definition from database
    /// Used by store frontend to render page with widgets
    /// </summary>
    public class GetPageDefinitionQuery
    {
        public string TenantId { get; init; }
        public string PagePath { get; init; }
    }

    public interface IPageRepository
    {
        Task<PageDefinition> GetPageByPathAsync(
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

            var page = await _repository.GetPageByPathAsync(
                request.TenantId,
                request.PagePath,
                cancellationToken);

            if (page == null || !page.IsPublished)
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
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string PageType { get; set; }
        public string PagePath { get; set; }
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string TemplateLayout { get; set; }
        public Dictionary<string, object> GlobalSettings { get; set; }
        public List<RegionDto> Regions { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class RegionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public Dictionary<string, object> Settings { get; set; }
        public List<WidgetInstanceDto> Widgets { get; set; }
    }

    public class WidgetInstanceDto
    {
        public string Id { get; set; }
        public string WidgetTypeId { get; set; }
        public string ComponentPath { get; set; }
        public int Order { get; set; }
        public Dictionary<string, object> Settings { get; set; }
    }
}
