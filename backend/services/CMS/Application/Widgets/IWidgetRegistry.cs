using System;
using System.Collections.Generic;
using B2Connect.CMS.Core.Domain.Widgets;
using Microsoft.Extensions.Logging;

namespace B2Connect.CMS.Application.Widgets
{
    /// <summary>
    /// Registry of all available widgets
    /// Used by admin frontend to display widget picker
    /// </summary>
    public interface IWidgetRegistry
    {
        void RegisterWidget(WidgetDefinition definition);
        WidgetDefinition GetWidget(string widgetId);
        IEnumerable<WidgetDefinition> GetAllWidgets();
        IEnumerable<WidgetDefinition> GetWidgetsByCategory(string category);
        IEnumerable<WidgetDefinition> GetWidgetsForPageType(string pageType);
        bool IsWidgetAvailable(string widgetId);
    }

    public class WidgetRegistry : IWidgetRegistry
    {
        private readonly Dictionary<string, WidgetDefinition> _widgets = new();
        private readonly ILogger<WidgetRegistry> _logger;

        public WidgetRegistry(ILogger<WidgetRegistry> logger)
        {
            _logger = logger;
        }

        public void RegisterWidget(WidgetDefinition definition)
        {
            if (string.IsNullOrEmpty(definition.Id))
            {
                throw new ArgumentException("Widget ID cannot be empty");
            }

            _widgets[definition.Id] = definition;
            _logger.LogInformation("Registered widget: {WidgetId}", definition.Id);
        }

        public WidgetDefinition GetWidget(string widgetId)
        {
            if (!_widgets.TryGetValue(widgetId, out var widget))
            {
                throw new InvalidOperationException($"Widget '{widgetId}' not found");
            }

            return widget;
        }

        public IEnumerable<WidgetDefinition> GetAllWidgets()
        {
            return _widgets.Values
                .Where(w => w.IsEnabled)
                .OrderBy(w => w.SortOrder);
        }

        public IEnumerable<WidgetDefinition> GetWidgetsByCategory(string category)
        {
            return GetAllWidgets()
                .Where(w => w.Category == category)
                .OrderBy(w => w.SortOrder);
        }

        public IEnumerable<WidgetDefinition> GetWidgetsForPageType(string pageType)
        {
            return GetAllWidgets()
                .Where(w => w.AllowedPageTypes.Count == 0 || w.AllowedPageTypes.Contains(pageType))
                .OrderBy(w => w.SortOrder);
        }

        public bool IsWidgetAvailable(string widgetId)
        {
            return _widgets.ContainsKey(widgetId) && _widgets[widgetId].IsEnabled;
        }
    }
}
