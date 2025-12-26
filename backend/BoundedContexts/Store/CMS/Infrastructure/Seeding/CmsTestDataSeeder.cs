using System;
using System.Collections.Generic;
using System.Linq;
using B2Connect.CMS.Core.Domain.Pages;
using B2Connect.CMS.Core.Domain.Widgets;

namespace B2Connect.CMS.Infrastructure.Seeding;

/// <summary>
/// Seeding service for CMS test data
/// Provides factory methods for creating test pages and widgets
/// </summary>
public class CmsTestDataSeeder
{
    public static PageDefinition CreateHomePage(string tenantId = "default-tenant")
    {
        var page = new PageDefinition(
            tenantId,
            "home",
            "/",
            "Welcome to B2Connect",
            "full-width")
        {
            PageDescription = "Discover premium products from our carefully curated collection",
            MetaKeywords = "store, products, shopping, ecommerce"
        };

        // Header Region - Hero Banner
        var headerRegion = new PageRegion
        {
            Name = "header",
            Order = 1,
            RegionSettings = new Dictionary<string, object>
            {
                { "padding", "0" }
            }
        };

        headerRegion.AddWidget("hero-banner", new Dictionary<string, object>
        {
            { "title", "Welcome to B2Connect" },
            { "subtitle", "Discover amazing products at great prices" },
            { "backgroundImage", "/images/hero-home.jpg" },
            { "ctaText", "Shop Now" },
            { "ctaLink", "/products" },
            { "height", 600 },
            { "textColor", "#ffffff" }
        });

        page.AddRegion(headerRegion);

        // Main Region - Products + Features
        var mainRegion = new PageRegion
        {
            Name = "main",
            Order = 2,
            RegionSettings = new Dictionary<string, object>
            {
                { "padding", "2rem" }
            }
        };

        mainRegion.AddWidget("feature-grid", new Dictionary<string, object>
        {
            { "title", "Why Shop With Us?" },
            { "columns", "3" },
            {
                "features", new List<object>
                {
                    new Dictionary<string, object>
                    {
                        { "icon", "🚚" },
                        { "title", "Free Shipping" },
                        { "description", "On orders over $100" }
                    },
                    new Dictionary<string, object>
                    {
                        { "icon", "💳" },
                        { "title", "Secure Payment" },
                        { "description", "100% encrypted transactions" }
                    },
                    new Dictionary<string, object>
                    {
                        { "icon", "⭐" },
                        { "title", "Quality Guaranteed" },
                        { "description", "Premium products only" }
                    }
                }
            }
        });

        var productWidget1 = mainRegion.AddWidget("product-grid", new Dictionary<string, object>
        {
            { "title", "Featured Products" },
            { "columns", "3" },
            { "itemsPerPage", 9 },
            { "sortBy", "newest" },
            { "showFilters", true }
        });

        mainRegion.AddWidget("testimonials", new Dictionary<string, object>
        {
            { "title", "What Our Customers Say" },
            { "autoplay", true },
            { "autoplayInterval", 5000 },
            {
                "testimonials", new List<object>
                {
                    new Dictionary<string, object>
                    {
                        { "text", "Best shopping experience I've had! Fast shipping and excellent quality." },
                        { "author", "Sarah Johnson" },
                        { "title", "Verified Buyer" }
                    },
                    new Dictionary<string, object>
                    {
                        { "text", "Great products, friendly customer service, and fair prices." },
                        { "author", "Mike Chen" },
                        { "title", "Verified Buyer" }
                    },
                    new Dictionary<string, object>
                    {
                        { "text", "Highly recommend! The quality exceeded my expectations." },
                        { "author", "Emma Wilson" },
                        { "title", "Verified Buyer" }
                    }
                }
            }
        });

        page.AddRegion(mainRegion);

        // Footer Region - CTA + Newsletter
        var footerRegion = new PageRegion
        {
            Name = "footer",
            Order = 3,
            RegionSettings = new Dictionary<string, object>
            {
                { "padding", "2rem" }
            }
        };

        footerRegion.AddWidget("call-to-action", new Dictionary<string, object>
        {
            { "heading", "Ready to Start Shopping?" },
            { "description", "Browse our collection and find exactly what you're looking for." },
            { "buttonText", "Explore Products" },
            { "buttonLink", "/products" },
            { "backgroundColor", "#007bff" }
        });

        footerRegion.AddWidget("newsletter-signup", new Dictionary<string, object>
        {
            { "heading", "Subscribe to Our Newsletter" },
            { "placeholder", "Enter your email address" },
            { "buttonText", "Subscribe" }
        });

        page.AddRegion(footerRegion);

        page.PublishPage();
        return page;
    }

    public static PageDefinition CreateProductListingPage(string tenantId = "default-tenant")
    {
        var page = new PageDefinition(
            tenantId,
            "product-listing",
            "/products",
            "Products - B2Connect",
            "sidebar")
        {
            PageDescription = "Browse our complete product catalog",
            MetaKeywords = "products, catalog, shopping"
        };

        // Header Region
        var headerRegion = new PageRegion
        {
            Name = "header",
            Order = 1
        };

        headerRegion.AddWidget("hero-banner", new Dictionary<string, object>
        {
            { "title", "Our Products" },
            { "subtitle", "Find exactly what you need" },
            { "backgroundImage", "/images/products-header.jpg" },
            { "height", 300 }
        });

        page.AddRegion(headerRegion);

        // Main Region - Product Grid
        var mainRegion = new PageRegion
        {
            Name = "main",
            Order = 2
        };

        mainRegion.AddWidget("product-grid", new Dictionary<string, object>
        {
            { "title", "" },
            { "columns", "2" },
            { "itemsPerPage", 12 },
            { "showFilters", true },
            { "sortBy", "popular" }
        });

        page.AddRegion(mainRegion);

        // Sidebar Region - Additional Info
        var sidebarRegion = new PageRegion
        {
            Name = "sidebar",
            Order = 3,
            MaxWidgets = 2
        };

        sidebarRegion.AddWidget("call-to-action", new Dictionary<string, object>
        {
            { "heading", "Need Help?" },
            { "description", "Our customer service team is here to assist you." },
            { "buttonText", "Contact Us" },
            { "buttonLink", "/contact" },
            { "backgroundColor", "#28a745" }
        });

        page.AddRegion(sidebarRegion);

        page.PublishPage();
        return page;
    }

    public static PageDefinition CreateAboutPage(string tenantId = "default-tenant")
    {
        var page = new PageDefinition(
            tenantId,
            "about",
            "/about",
            "About B2Connect",
            "full-width")
        {
            PageDescription = "Learn more about B2Connect and our mission",
            MetaKeywords = "about, company, mission"
        };

        var mainRegion = new PageRegion
        {
            Name = "main",
            Order = 1
        };

        mainRegion.AddWidget("hero-banner", new Dictionary<string, object>
        {
            { "title", "About B2Connect" },
            { "subtitle", "Our Story and Vision" },
            { "backgroundImage", "/images/about-hero.jpg" },
            { "height", 400 }
        });

        mainRegion.AddWidget("text-block", new Dictionary<string, object>
        {
            {
                "content", @"<h2>Our Mission</h2>
<p>B2Connect is dedicated to providing high-quality products and exceptional customer service. 
We believe in creating value for our customers through innovation, reliability, and transparency.</p>

<h2>Our Values</h2>
<ul>
  <li><strong>Quality:</strong> We source only the best products</li>
  <li><strong>Customer First:</strong> Your satisfaction is our priority</li>
  <li><strong>Innovation:</strong> We constantly improve our service</li>
  <li><strong>Integrity:</strong> We do business with honesty and transparency</li>
</ul>"
            },
            { "maxWidth", 800 }
        });

        mainRegion.AddWidget("feature-grid", new Dictionary<string, object>
        {
            { "title", "Our Achievements" },
            { "columns", "3" },
            {
                "features", new List<object>
                {
                    new Dictionary<string, object>
                    {
                        { "icon", "👥" },
                        { "title", "50K+ Customers" },
                        { "description", "Trusted by thousands worldwide" }
                    },
                    new Dictionary<string, object>
                    {
                        { "icon", "📦" },
                        { "title", "100K+ Orders" },
                        { "description", "Successfully delivered" }
                    },
                    new Dictionary<string, object>
                    {
                        { "icon", "⭐" },
                        { "title", "4.8/5 Rating" },
                        { "description", "Average customer rating" }
                    }
                }
            }
        });

        page.AddRegion(mainRegion);

        page.PublishPage();
        return page;
    }

    public static PageDefinition CreateContactPage(string tenantId = "default-tenant")
    {
        var page = new PageDefinition(
            tenantId,
            "contact",
            "/contact",
            "Contact B2Connect",
            "full-width")
        {
            PageDescription = "Get in touch with our customer support team",
            MetaKeywords = "contact, support, customer service"
        };

        var mainRegion = new PageRegion
        {
            Name = "main",
            Order = 1
        };

        mainRegion.AddWidget("text-block", new Dictionary<string, object>
        {
            {
                "content", @"<h1>Contact Us</h1>
<p>We'd love to hear from you. Please fill out the form below and we'll respond within 24 hours.</p>

<h3>Other Ways to Reach Us</h3>
<p><strong>Email:</strong> support@b2connect.com</p>
<p><strong>Phone:</strong> 1-800-B2CONNECT</p>
<p><strong>Hours:</strong> Monday - Friday, 9 AM - 5 PM EST</p>"
            },
            { "maxWidth", 600 }
        });

        page.AddRegion(mainRegion);

        page.PublishPage();
        return page;
    }

    public static List<PageDefinition> CreateSamplePages(string tenantId = "default-tenant")
    {
        return new List<PageDefinition>
        {
            CreateHomePage(tenantId),
            CreateProductListingPage(tenantId),
            CreateAboutPage(tenantId),
            CreateContactPage(tenantId)
        };
    }
}

/// <summary>
/// Widget definition seeder for CMS
/// Provides factory methods for creating widget definitions
/// </summary>
public class CmsWidgetSeeder
{
    public static List<WidgetDefinition> GetDefaultWidgets()
    {
        return new List<WidgetDefinition>
        {
            CreateHeroBannerWidget(),
            CreateProductGridWidget(),
            CreateFeatureGridWidget(),
            CreateTestimonialsWidget(),
            CreateCallToActionWidget(),
            CreateTextBlockWidget(),
            CreateVideoWidget(),
            CreateNewsletterSignupWidget()
        };
    }

    private static WidgetDefinition CreateHeroBannerWidget()
    {
        var widget = new WidgetDefinition(
            "hero-banner",
            "Hero Banner",
            "widgets/HeroBanner.vue",
            "media")
        {
            Description = "Full-width hero section with background image and CTA button",
            ThumbnailUrl = "/images/widget-thumbnails/hero-banner.jpg",
            PreviewWidth = 800,
            PreviewHeight = 400,
            DefaultSettings = new List<WidgetSetting>
            {
                new WidgetSetting("title", "Title", WidgetSettingType.Text)
                {
                    DisplayOrder = 1,
                    IsRequired = true,
                    DefaultValue = "Hero Title"
                },
                new WidgetSetting("subtitle", "Subtitle", WidgetSettingType.Text)
                {
                    DisplayOrder = 2,
                    DefaultValue = "Hero Subtitle"
                },
                new WidgetSetting("backgroundImage", "Background Image", WidgetSettingType.Image)
                {
                    DisplayOrder = 3,
                    IsRequired = true
                },
                new WidgetSetting("ctaText", "CTA Button Text", WidgetSettingType.Text)
                {
                    DisplayOrder = 4,
                    DefaultValue = "Learn More"
                },
                new WidgetSetting("ctaLink", "CTA Button Link", WidgetSettingType.Text)
                {
                    DisplayOrder = 5,
                    DefaultValue = "#"
                },
                new WidgetSetting("height", "Height (px)", WidgetSettingType.Number)
                {
                    DisplayOrder = 6,
                    DefaultValue = 500
                }
            },
            SortOrder = 1
        };

        return widget;
    }

    private static WidgetDefinition CreateProductGridWidget()
    {
        var widget = new WidgetDefinition(
            "product-grid",
            "Product Grid",
            "widgets/ProductGrid.vue",
            "products")
        {
            Description = "Display products in a responsive grid with pagination",
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
                    {
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
                    {
                        { "options", new[] { "newest", "popular", "price-asc", "price-desc", "rating" } }
                    }
                }
            },
            SortOrder = 2
        };

        return widget;
    }

    private static WidgetDefinition CreateFeatureGridWidget()
    {
        var widget = new WidgetDefinition(
            "feature-grid",
            "Feature Grid",
            "widgets/FeatureGrid.vue",
            "content")
        {
            Description = "Display features or benefits in a grid layout",
            DefaultSettings = new List<WidgetSetting>
            {
                new WidgetSetting("title", "Section Title", WidgetSettingType.Text)
                {
                    DisplayOrder = 1,
                    DefaultValue = "Features"
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
                    {
                        { "options", new[] { "2", "3", "4" } }
                    }
                }
            },
            SortOrder = 3
        };

        return widget;
    }

    private static WidgetDefinition CreateTestimonialsWidget()
    {
        var widget = new WidgetDefinition(
            "testimonials",
            "Testimonials",
            "widgets/Testimonials.vue",
            "content")
        {
            Description = "Customer testimonials carousel",
            DefaultSettings = new List<WidgetSetting>
            {
                new WidgetSetting("title", "Section Title", WidgetSettingType.Text)
                {
                    DisplayOrder = 1,
                    DefaultValue = "What Customers Say"
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
            },
            SortOrder = 4
        };

        return widget;
    }

    private static WidgetDefinition CreateCallToActionWidget()
    {
        var widget = new WidgetDefinition(
            "call-to-action",
            "Call to Action",
            "widgets/CallToAction.vue",
            "content")
        {
            Description = "CTA section with background color and button",
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
            },
            SortOrder = 5
        };

        return widget;
    }

    private static WidgetDefinition CreateTextBlockWidget()
    {
        var widget = new WidgetDefinition(
            "text-block",
            "Text Block",
            "widgets/TextBlock.vue",
            "content")
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
            },
            SortOrder = 6
        };

        return widget;
    }

    private static WidgetDefinition CreateVideoWidget()
    {
        var widget = new WidgetDefinition(
            "video",
            "Video",
            "widgets/Video.vue",
            "media")
        {
            Description = "Embedded video player",
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
                    {
                        { "options", new[] { "16:9", "4:3", "1:1" } }
                    }
                }
            },
            SortOrder = 7
        };

        return widget;
    }

    private static WidgetDefinition CreateNewsletterSignupWidget()
    {
        var widget = new WidgetDefinition(
            "newsletter-signup",
            "Newsletter Signup",
            "widgets/NewsletterSignup.vue",
            "forms")
        {
            Description = "Email subscription form",
            DefaultSettings = new List<WidgetSetting>
            {
                new WidgetSetting("heading", "Heading", WidgetSettingType.Text)
                {
                    DisplayOrder = 1,
                    DefaultValue = "Subscribe"
                },
                new WidgetSetting("placeholder", "Email Placeholder", WidgetSettingType.Text)
                {
                    DisplayOrder = 2,
                    DefaultValue = "Enter email"
                },
                new WidgetSetting("buttonText", "Button Text", WidgetSettingType.Text)
                {
                    DisplayOrder = 3,
                    DefaultValue = "Subscribe"
                }
            },
            SortOrder = 8
        };

        return widget;
    }
}
