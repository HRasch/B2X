using System;
using System.Collections.Generic;

namespace B2Connect.CMS.Core.Domain.Widgets
{
    /// <summary>
    /// Widget definition - blueprint for widgets
    /// Defines what settings a widget accepts
    /// </summary>
    public class WidgetDefinition
    {
        public string Id { get; set; } = null!; // 'hero-banner', 'product-grid', 'testimonials', etc.
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ComponentPath { get; set; } = null!; // Frontend component path
        public string Category { get; set; } = null!; // 'media', 'content', 'products', 'forms'
        public string ThumbnailUrl { get; set; } = null!;
        public int PreviewWidth { get; set; } = 400;
        public int PreviewHeight { get; set; } = 300;
        public List<WidgetSetting> DefaultSettings { get; set; } = new();
        public List<string> AllowedPageTypes { get; set; } = new(); // Empty = all
        public bool IsEnabled { get; set; } = true;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public WidgetDefinition()
        {
        }

        public WidgetDefinition(
            string id,
            string displayName,
            string componentPath,
            string category)
        {
            Id = id;
            DisplayName = displayName;
            ComponentPath = componentPath;
            Category = category;
            CreatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Definition of a setting that a widget accepts
    /// </summary>
    public class WidgetSetting
    {
        public string Key { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public WidgetSettingType Type { get; set; } // text, number, select, textarea, etc.
        public object? DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new(StringComparer.Ordinal); // Validation rules, options, etc.

        public WidgetSetting()
        {
        }

        public WidgetSetting(string key, string displayName, WidgetSettingType type)
        {
            Key = key;
            DisplayName = displayName;
            Type = type;
        }
    }

    public enum WidgetSettingType
    {
        Text,
        Number,
        Textarea,
        RichText,
        Select,
        MultiSelect,
        Toggle,
        Date,
        DateTime,
        Image,
        Video,
        Color,
        Json
    }
}
