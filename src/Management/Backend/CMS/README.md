# CMS Widget System

## Overview

A widget-based Content Management System that allows dynamic page composition through database-defined page templates. Similar to partial views, widgets are self-contained Vue components that can be placed in editable regions/slots on any page.

## Features

- **Widget-Based Architecture**: Modular, reusable components
- **Multiple Layout Templates**: Full-width, sidebar, three-column support
- **Dynamic Region Management**: Multiple editable slots per page
- **SEO Support**: Meta tags from page definitions
- **Type-Safe**: Full TypeScript support for both backend and frontend
- **Admin-Friendly**: JSON-based page definitions
- **Extensible**: Easy to add new widgets

## Directory Structure

### Backend
```
backend/services/CMS/
├── Core/
│   └── Domain/
│       ├── Pages/
│       │   └── PageDefinition.cs          # Page & region domain models
│       └── Widgets/
│           └── WidgetDefinition.cs        # Widget blueprint definitions
├── Application/
│   ├── Widgets/
│   │   ├── IWidgetRegistry.cs            # Widget registry interface
│   │   └── WidgetRegistrationService.cs  # Widget initialization
│   └── Pages/
│       └── GetPageDefinitionQuery.cs     # CQRS query handler
└── Infrastructure/
    └── (Persistence layer - implement as needed)
```

### Frontend
```
frontend/src/
├── types/
│   └── cms.ts                           # TypeScript interfaces
├── services/api/
│   └── cms.ts                           # API client
├── composables/
│   └── useCms.ts                        # CMS composable hooks
└── components/
    ├── cms/
    │   ├── CmsPageLayout.vue            # Main page layout renderer
    │   ├── RegionRenderer.vue           # Region/slot renderer
    │   ├── WidgetRenderer.vue           # Widget dynamic loader
    │   └── WidgetNotFound.vue           # Fallback component
    └── widgets/                          # Widget components
        ├── HeroBanner.vue
        ├── ProductGrid.vue
        ├── FeatureGrid.vue
        ├── Testimonials.vue
        ├── CallToAction.vue
        ├── TextBlock.vue
        ├── Video.vue
        └── NewsletterSignup.vue
```

## Data Models

### Page Definition
```typescript
interface PageDefinition {
  id: string;
  tenantId: string;
  pageType: string;              // 'home', 'about', 'product-listing'
  pagePath: string;              // '/home', '/about-us'
  pageTitle: string;             // SEO title
  pageDescription: string;       // SEO meta description
  metaKeywords: string;
  templateLayout: string;        // 'full-width', 'sidebar', 'three-column'
  globalSettings: Record<string, any>;
  regions: PageRegion[];
  isPublished: boolean;
  publishedAt: string;
}

interface PageRegion {
  id: string;
  name: string;                  // 'header', 'main', 'sidebar', 'footer'
  order: number;
  settings: Record<string, any>; // Region-specific styling/config
  widgets: WidgetInstance[];
}

interface WidgetInstance {
  id: string;
  widgetTypeId: string;          // 'hero-banner', 'product-grid'
  componentPath: string;
  order: number;
  settings: Record<string, any>; // Widget-specific configuration
}
```

### Widget Definition
```typescript
interface WidgetDefinition {
  id: string;                    // Unique widget identifier
  displayName: string;           // Admin UI display name
  description: string;
  componentPath: string;         // Vue component import path
  category: string;              // 'media', 'content', 'products', 'forms'
  thumbnailUrl: string;
  defaultSettings: WidgetSetting[];
  allowedPageTypes: string[];    // Empty = all page types allowed
  isEnabled: boolean;
}

interface WidgetSetting {
  key: string;                   // Setting property name
  displayName: string;           // Admin UI label
  type: WidgetSettingType;       // 'text', 'number', 'select', 'json', etc.
  defaultValue: any;
  isRequired: boolean;
  displayOrder: number;
  metadata: Record<string, any>; // Validation rules, options for select, etc.
}
```

## Available Widgets

### 1. Hero Banner (`hero-banner`)
Full-width hero section with background image and CTA button.

**Settings**:
- `title`: Main heading
- `subtitle`: Subheading
- `backgroundImage`: Image URL
- `ctaText`: Button text
- `ctaLink`: Button link
- `textColor`: Text color (hex)
- `height`: Banner height in pixels

### 2. Product Grid (`product-grid`)
Responsive product display grid with pagination.

**Settings**:
- `title`: Widget title
- `columns`: Number of columns (1-6)
- `itemsPerPage`: Products per page
- `sortBy`: Sort option
- `showFilters`: Show/hide filters

### 3. Feature Grid (`feature-grid`)
Display features/benefits in a grid.

**Settings**:
- `title`: Section title
- `features`: JSON array of features
- `columns`: Grid columns (2-4)

### 4. Testimonials (`testimonials`)
Customer testimonials carousel.

**Settings**:
- `title`: Section title
- `testimonials`: JSON array of testimonials
- `autoplay`: Auto-rotate
- `autoplayInterval`: Milliseconds between rotations

### 5. Call to Action (`call-to-action`)
Simple CTA section with button.

**Settings**:
- `heading`: Section heading
- `description`: Description text
- `buttonText`: Button label
- `buttonLink`: Button URL
- `backgroundColor`: Background color (hex)

### 6. Text Block (`text-block`)
Rich text content area.

**Settings**:
- `content`: HTML/rich text content
- `maxWidth`: Maximum width in pixels

### 7. Video (`video`)
Embedded video player.

**Settings**:
- `videoUrl`: Video embed URL
- `autoplay`: Autoplay video
- `aspectRatio`: Video ratio (16:9, 4:3, 1:1)

### 8. Newsletter Signup (`newsletter-signup`)
Email subscription form.

**Settings**:
- `heading`: Form heading
- `placeholder`: Email input placeholder
- `buttonText`: Submit button text

## Backend Integration

### 1. Register Widgets at Startup

In your `Program.cs` or startup configuration:

```csharp
// Register CMS services
services.AddScoped<IWidgetRegistry, WidgetRegistry>();
services.AddScoped<WidgetRegistrationService>();

// Register CQRS handlers
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetPageDefinitionQuery).Assembly));

// Initialize widgets
app.MapGet("/api/cms/initialize", (WidgetRegistrationService service) =>
{
    service.RegisterDefaultWidgets();
    return Ok("Widgets registered");
});
```

### 2. Create CMS Controller

```csharp
[ApiController]
[Route("api/cms")]
public class CmsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("pages/by-path")]
    public async Task<IActionResult> GetPageByPath(
        [FromQuery] string path,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        CancellationToken ct)
    {
        var query = new GetPageDefinitionQuery
        {
            TenantId = tenantId,
            PagePath = path
        };

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("widgets")]
    public IActionResult GetWidgets(
        [FromServices] IWidgetRegistry registry,
        [FromQuery] string pageType = null)
    {
        var widgets = string.IsNullOrEmpty(pageType)
            ? registry.GetAllWidgets()
            : registry.GetWidgetsForPageType(pageType);

        return Ok(widgets);
    }
}
```

### 3. Implement Page Repository

Implement `IPageRepository` to load page definitions from your database:

```csharp
public class PageRepository : IPageRepository
{
    private readonly DbContext _context;

    public async Task<PageDefinition> GetPageByPathAsync(
        string tenantId,
        string pagePath,
        CancellationToken cancellationToken)
    {
        return await _context.PageDefinitions
            .Include(p => p.Regions)
            .ThenInclude(r => r.Widgets)
            .FirstOrDefaultAsync(p =>
                p.TenantId == tenantId &&
                p.PagePath == pagePath,
                cancellationToken);
    }
}
```

### 4. Create Sample Page Definition

```csharp
var homePage = new PageDefinition(
    tenantId: "tenant-1",
    pageType: "home",
    pagePath: "/",
    pageTitle: "Welcome to Our Store",
    templateLayout: "full-width")
{
    PageDescription = "Shop amazing products at great prices",
    MetaKeywords = "store, shopping, products"
};

// Add header region with hero banner
var headerRegion = new PageRegion
{
    Name = "header",
    Order = 1
};

headerRegion.AddWidget("hero-banner", new Dictionary<string, object>
{
    { "title", "Welcome!" },
    { "backgroundImage", "/images/hero.jpg" },
    { "ctaText", "Shop Now" },
    { "ctaLink", "/products" }
});

homePage.AddRegion(headerRegion);

// Add main region with product grid
var mainRegion = new PageRegion
{
    Name = "main",
    Order = 2
};

mainRegion.AddWidget("product-grid", new Dictionary<string, object>
{
    { "title", "Featured Products" },
    { "columns", "3" },
    { "itemsPerPage", 12 }
});

homePage.AddRegion(mainRegion);

// Publish
homePage.PublishPage();
```

## Frontend Usage

### In Your Router

Update your router to catch all unmatched routes and render CMS pages:

```typescript
// src/router/index.ts

const router = createRouter({
  history: createWebHistory(),
  routes: [
    // Your static routes first
    { path: '/products', component: () => import('@/views/Products.vue') },
    { path: '/cart', component: () => import('@/views/Cart.vue') },
    { path: '/checkout', component: () => import('@/views/Checkout.vue') },

    // CMS dynamic routes - MUST be last
    {
      path: '/:pathMatch(.*)*',
      component: () => import('@/components/cms/CmsPageLayout.vue'),
      meta: { isCmsRoute: true }
    }
  ]
});
```

### Using the CMS Composable

```vue
<script setup lang="ts">
import { useCms } from '@/composables/useCms';

const { pageDefinition, loading, error, getRegion, fetchPageDefinition } = useCms();

// Fetch page definition
await fetchPageDefinition('/about-us');

// Get a specific region
const mainRegion = getRegion('main');
</script>
```

### In a Custom Page Component

```vue
<template>
  <div>
    <!-- Load custom CMS page -->
    <CmsPageLayout />
  </div>
</template>

<script setup lang="ts">
import CmsPageLayout from '@/components/cms/CmsPageLayout.vue';
</script>
```

## Creating Custom Widgets

### 1. Create Widget Vue Component

**File**: `src/components/widgets/MyCustomWidget.vue`

```vue
<template>
  <section class="my-custom-widget">
    <!-- Use settings from database -->
    <h2>{{ settings.title }}</h2>
    <p>{{ settings.description }}</p>
  </section>
</template>

<script setup lang="ts">
interface Props {
  settings: Record<string, any>;
  widgetId: string;
}

defineProps<Props>();
</script>

<style scoped>
.my-custom-widget {
  padding: 2rem;
}
</style>
```

### 2. Register Widget in Backend

In `WidgetRegistrationService.cs`:

```csharp
private void RegisterMyCustomWidget()
{
    var widget = new WidgetDefinition(
        id: "my-custom",
        displayName: "My Custom Widget",
        componentPath: "widgets/MyCustomWidget.vue",
        category: "content")
    {
        Description = "A custom widget",
        DefaultSettings = new List<WidgetSetting>
        {
            new WidgetSetting("title", "Title", WidgetSettingType.Text)
            {
                DisplayOrder = 1,
                IsRequired = true
            },
            new WidgetSetting("description", "Description", WidgetSettingType.Textarea)
            {
                DisplayOrder = 2
            }
        }
    };

    _registry.RegisterWidget(widget);
}
```

### 3. Call Registration in Main Method

```csharp
public void RegisterDefaultWidgets()
{
    // ... existing widgets ...
    RegisterMyCustomWidget();
}
```

## Setting Types

- **Text**: Simple text input
- **Number**: Numeric input
- **Textarea**: Multi-line text
- **RichText**: HTML editor
- **Select**: Dropdown selection
- **MultiSelect**: Multiple selections
- **Toggle**: Boolean checkbox
- **Date**: Date picker
- **DateTime**: Date and time picker
- **Image**: Image upload/selection
- **Video**: Video URL
- **Color**: Color picker
- **Json**: JSON object editor

## API Endpoints

### Get Page Definition
```
GET /api/cms/pages/by-path?path=/about-us
X-Tenant-Id: tenant-1

Response: PageDefinitionDto
```

### Get All Widgets
```
GET /api/cms/widgets
Response: WidgetDefinition[]
```

### Get Widgets for Page Type
```
GET /api/cms/widgets?pageType=home
Response: WidgetDefinition[]
```

### Get Widgets by Category
```
GET /api/cms/widgets/category/media
Response: WidgetDefinition[]
```

## Best Practices

1. **Component Naming**: Use clear, descriptive names for custom widgets
2. **Settings Validation**: Always validate widget settings in components
3. **Responsive Design**: Ensure all widgets are mobile-friendly
4. **Performance**: Lazy-load heavy widgets and optimize images
5. **SEO**: Use semantic HTML and proper heading hierarchy
6. **Accessibility**: Include ARIA labels and proper color contrast
7. **Error Handling**: Gracefully handle missing or invalid settings
8. **Documentation**: Document custom widget settings clearly

## Troubleshooting

### Widget Not Loading
- Check that component path is correct
- Verify widget ID matches registration
- Check browser console for import errors

### Settings Not Applied
- Ensure settings keys match widget component expectations
- Verify JSON settings are properly formatted
- Check widget metadata for validation rules

### Page Not Found
- Ensure page is published (`IsPublished = true`)
- Verify correct tenant ID in request header
- Check page path format matches database

## Next Steps

1. Implement admin page builder UI (for frontend-admin)
2. Add page draft/preview functionality
3. Implement page versioning and rollback
4. Add widget style customization
5. Create page template library
6. Add analytics tracking for widget interactions
