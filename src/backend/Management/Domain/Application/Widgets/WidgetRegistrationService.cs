using System;
using System.Collections.Generic;
using B2X.CMS.Core.Domain.Widgets;
using Microsoft.Extensions.Logging;

namespace B2X.CMS.Application.Widgets
{
    /// <summary>
    /// Service to initialize and register all available widgets
    /// Called at application startup
    /// </summary>
    public class WidgetRegistrationService
    {
        private readonly IWidgetRegistry _registry;
        private readonly ILogger<WidgetRegistrationService> _logger;
        private static readonly string[] value = new[] { "left", "center", "right" };

        public WidgetRegistrationService(
            IWidgetRegistry registry,
            ILogger<WidgetRegistrationService> logger)
        {
            _registry = registry;
            _logger = logger;
        }

        public void RegisterDefaultWidgets()
        {
            _logger.LogInformation("Registering default widgets...");

            RegisterHeroBannerWidget();
            RegisterProductGridWidget();
            RegisterFeatureGridWidget();
            RegisterTestimonialsWidget();
            RegisterCtaWidget();
            RegisterTextBlockWidget();
            RegisterVideoWidget();
            RegisterNewsletterWidget();

            _logger.LogInformation("Default widgets registered successfully");
        }

        private void RegisterHeroBannerWidget()
        {
            var widget = new WidgetDefinition(
                id: "hero-banner",
                displayName: "Hero Banner",
                componentPath: "widgets/HeroBanner.vue",
                category: "media")
            {
                Description = "Full-width hero banner with background image and CTA",
                ThumbnailUrl = "/images/widget-thumbnails/hero-banner.jpg",
                PreviewWidth = 800,
                PreviewHeight = 400,
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("title", "Title", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        IsRequired = true,
                        DefaultValue = "Welcome to Our Store"
                    },
                    new WidgetSetting("subtitle", "Subtitle", WidgetSettingType.Text)
                    {
                        DisplayOrder = 2,
                        DefaultValue = "Discover amazing products"
                    },
                    new WidgetSetting("backgroundImage", "Background Image", WidgetSettingType.Image)
                    {
                        DisplayOrder = 3,
                        IsRequired = true
                    },
                    new WidgetSetting("ctaText", "CTA Button Text", WidgetSettingType.Text)
                    {
                        DisplayOrder = 4,
                        DefaultValue = "Shop Now"
                    },
                    new WidgetSetting("ctaLink", "CTA Button Link", WidgetSettingType.Text)
                    {
                        DisplayOrder = 5,
                        DefaultValue = "/products"
                    },
                    new WidgetSetting("textAlignment", "Text Alignment", WidgetSettingType.Select)
                    {
                        DisplayOrder = 6,
                        DefaultValue = "center",
                        Metadata = new Dictionary<string, object>
(StringComparer.Ordinal) {
                            { "options", value }
                        }
                    },
                    new WidgetSetting("textColor", "Text Color", WidgetSettingType.Color)
                    {
                        DisplayOrder = 7,
                        DefaultValue = "#ffffff"
                    },
                    new WidgetSetting("height", "Height (px)", WidgetSettingType.Number)
                    {
                        DisplayOrder = 8,
                        DefaultValue = 500
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterProductGridWidget()
        {
            var widget = new WidgetDefinition(
                id: "product-grid",
                displayName: "Product Grid",
                componentPath: "widgets/ProductGrid.vue",
                category: "products")
            {
                Description = "Display products in a responsive grid with filters",
                AllowedPageTypes = new List<string> { "home", "product-listing" },
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("title", "Widget Title", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        DefaultValue = "Featured Products"
                    },
                    new WidgetSetting("columns", "Columns", WidgetSettingType.Select)
                    {
                        DisplayOrder = 2,
                        DefaultValue = "3",
                        Metadata = new Dictionary<string, object>
(StringComparer.Ordinal) {
                            { "options", new[] { "1", "2", "3", "4", "6" } }
                        }
                    },
                    new WidgetSetting("itemsPerPage", "Items Per Page", WidgetSettingType.Number)
                    {
                        DisplayOrder = 3,
                        DefaultValue = 12
                    },
                    new WidgetSetting("sortBy", "Sort By", WidgetSettingType.Select)
                    {
                        DisplayOrder = 4,
                        DefaultValue = "newest",
                        Metadata = new Dictionary<string, object>
(StringComparer.Ordinal) {
                            { "options", new[] { "newest", "popular", "price-asc", "price-desc", "rating" } }
                        }
                    },
                    new WidgetSetting("showFilters", "Show Filters", WidgetSettingType.Toggle)
                    {
                        DisplayOrder = 5,
                        DefaultValue = true
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterFeatureGridWidget()
        {
            var widget = new WidgetDefinition(
                id: "feature-grid",
                displayName: "Feature Grid",
                componentPath: "widgets/FeatureGrid.vue",
                category: "content")
            {
                Description = "Display features/benefits in a grid layout",
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("title", "Section Title", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        DefaultValue = "Why Choose Us"
                    },
                    new WidgetSetting("features", "Features", WidgetSettingType.Json)
                    {
                        DisplayOrder = 2,
                        IsRequired = true
                    },
                    new WidgetSetting("columns", "Columns", WidgetSettingType.Select)
                    {
                        DisplayOrder = 3,
                        DefaultValue = "3",
                        Metadata = new Dictionary<string, object>
(StringComparer.Ordinal) {
                            { "options", new[] { "2", "3", "4" } }
                        }
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterTestimonialsWidget()
        {
            var widget = new WidgetDefinition(
                id: "testimonials",
                displayName: "Testimonials",
                componentPath: "widgets/Testimonials.vue",
                category: "content")
            {
                Description = "Customer testimonials carousel",
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("title", "Section Title", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        DefaultValue = "What Our Customers Say"
                    },
                    new WidgetSetting("testimonials", "Testimonials", WidgetSettingType.Json)
                    {
                        DisplayOrder = 2,
                        IsRequired = true
                    },
                    new WidgetSetting("autoplay", "Autoplay", WidgetSettingType.Toggle)
                    {
                        DisplayOrder = 3,
                        DefaultValue = true
                    },
                    new WidgetSetting("autoplayInterval", "Autoplay Interval (ms)", WidgetSettingType.Number)
                    {
                        DisplayOrder = 4,
                        DefaultValue = 5000
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterCtaWidget()
        {
            var widget = new WidgetDefinition(
                id: "call-to-action",
                displayName: "Call to Action",
                componentPath: "widgets/CallToAction.vue",
                category: "content")
            {
                Description = "Simple CTA section with background and button",
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("heading", "Heading", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        IsRequired = true
                    },
                    new WidgetSetting("description", "Description", WidgetSettingType.Textarea)
                    {
                        DisplayOrder = 2
                    },
                    new WidgetSetting("buttonText", "Button Text", WidgetSettingType.Text)
                    {
                        DisplayOrder = 3,
                        IsRequired = true
                    },
                    new WidgetSetting("buttonLink", "Button Link", WidgetSettingType.Text)
                    {
                        DisplayOrder = 4,
                        IsRequired = true
                    },
                    new WidgetSetting("backgroundColor", "Background Color", WidgetSettingType.Color)
                    {
                        DisplayOrder = 5,
                        DefaultValue = "#007bff"
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterTextBlockWidget()
        {
            var widget = new WidgetDefinition(
                id: "text-block",
                displayName: "Text Block",
                componentPath: "widgets/TextBlock.vue",
                category: "content")
            {
                Description = "Rich text content block",
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("content", "Content", WidgetSettingType.RichText)
                    {
                        DisplayOrder = 1,
                        IsRequired = true
                    },
                    new WidgetSetting("maxWidth", "Max Width (px)", WidgetSettingType.Number)
                    {
                        DisplayOrder = 2,
                        DefaultValue = 600
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterVideoWidget()
        {
            var widget = new WidgetDefinition(
                id: "video",
                displayName: "Video",
                componentPath: "widgets/Video.vue",
                category: "media")
            {
                Description = "Embed video from YouTube or Vimeo",
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("videoUrl", "Video URL", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        IsRequired = true
                    },
                    new WidgetSetting("autoplay", "Autoplay", WidgetSettingType.Toggle)
                    {
                        DisplayOrder = 2,
                        DefaultValue = false
                    },
                    new WidgetSetting("aspectRatio", "Aspect Ratio", WidgetSettingType.Select)
                    {
                        DisplayOrder = 3,
                        DefaultValue = "16:9",
                        Metadata = new Dictionary<string, object>
(StringComparer.Ordinal) {
                            { "options", new[] { "16:9", "4:3", "1:1" } }
                        }
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }

        private void RegisterNewsletterWidget()
        {
            var widget = new WidgetDefinition(
                id: "newsletter-signup",
                displayName: "Newsletter Signup",
                componentPath: "widgets/NewsletterSignup.vue",
                category: "forms")
            {
                Description = "Email signup form",
                DefaultSettings = new List<WidgetSetting>
                {
                    new WidgetSetting("heading", "Heading", WidgetSettingType.Text)
                    {
                        DisplayOrder = 1,
                        DefaultValue = "Subscribe to Our Newsletter"
                    },
                    new WidgetSetting("placeholder", "Email Placeholder", WidgetSettingType.Text)
                    {
                        DisplayOrder = 2,
                        DefaultValue = "Enter your email"
                    },
                    new WidgetSetting("buttonText", "Button Text", WidgetSettingType.Text)
                    {
                        DisplayOrder = 3,
                        DefaultValue = "Subscribe"
                    }
                }
            };

            _registry.RegisterWidget(widget);
        }
    }
}
