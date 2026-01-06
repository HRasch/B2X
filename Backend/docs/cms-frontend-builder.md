# Frontend CMS & Layout Builder

## Overview

B2Connect includes an integrated Content Management System (CMS) and Visual Layout Builder that enables customers to create and customize their storefront layout and theme without coding. This system provides enterprise-grade customization capabilities while maintaining brand consistency and performance.

## Architecture

### CMS Components

```
┌─────────────────────────────────────────────────────────────────┐
│                   Frontend CMS Portal                           │
│              (Customer-facing Admin Interface)                  │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  Dashboard                                              │   │
│  │  - Quick Stats                                          │   │
│  │  - Theme Overview                                       │   │
│  │  - Recent Changes                                       │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌──────────────────────┬──────────────────────────────────┐   │
│  │ Visual Layout Builder │ Theme Customizer                 │   │
│  │ ─────────────────────┼──────────────────────────────────│   │
│  │ - Drag & Drop        │ - Color Schemes                  │   │
│  │ - Component Library   │ - Typography Settings            │   │
│  │ - Page Templates      │ - Spacing & Sizing               │   │
│  │ - Live Preview        │ - Custom CSS                     │   │
│  │ - Section Management  │ - Asset Library                  │   │
│  └──────────────────────┴──────────────────────────────────┘   │
│                                                                 │
│  ┌──────────────────────┬──────────────────────────────────┐   │
│  │ Content Management    │ SEO & Analytics                  │   │
│  │ ─────────────────────┼──────────────────────────────────│   │
│  │ - Page Editor         │ - Meta Tags                      │   │
│  │ - Block Management    │ - Structured Data                │   │
│  │ - Version History     │ - Performance Metrics            │   │
│  │ - Publishing Workflow │ - Search Optimization            │   │
│  └──────────────────────┴──────────────────────────────────┘   │
└──────────────────────────────┬──────────────────────────────────┘
                               │
                    REST API & WebSocket
                               │
┌──────────────────────────────▼──────────────────────────────────┐
│                   CMS Backend Service                           │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Layout Service (Port 5008)                               │   │
│  │ - Page Structure Management                              │   │
│  │ - Component Registry & Validation                        │   │
│  │ - Template Management                                    │   │
│  │ - Live Preview Generation                                │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Theme Service (Port 5009)                                │   │
│  │ - Theme Configuration Storage                            │   │
│  │ - Style Variable Management                              │   │
│  │ - CSS Generation & Optimization                          │   │
│  │ - Theme Publishing & Rollback                            │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Content Service (Port 5011)                              │   │
│  │ - Page & Block Versioning                                │   │
│  │ - Publishing & Scheduling                                │   │
│  │ - SEO Metadata Management                                │   │
│  │ - Asset Management (Images, Videos)                      │   │
│  └──────────────────────────────────────────────────────────┘   │
└──────────────────────────────┬──────────────────────────────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
        ▼                      ▼                      ▼
    ┌────────┐        ┌──────────────┐        ┌────────┐
    │PostgreSQL│        │   S3/Blob    │        │ Redis  │
    │          │        │   Storage    │        │ Cache  │
    │- Layouts │        │              │        │        │
    │- Themes  │        │- Assets      │        │- Style │
    │- Content │        │- Media       │        │  Cache │
    │- SEO     │        │- Backups     │        │- Live  │
    └────────┘        └──────────────┘        │  Preview
                                                └────────┘
```

## Core Services

### 1. Layout Service (Port 5008)

**Purpose**: Manage page structure, components, and layout configuration

**Key Endpoints**:

```csharp
// Pages
GET    /api/cms/pages                    // List all pages
POST   /api/cms/pages                    // Create new page
GET    /api/cms/pages/{pageId}           // Get page structure
PUT    /api/cms/pages/{pageId}           // Update page layout
DELETE /api/cms/pages/{pageId}           // Delete page

// Sections/Blocks
POST   /api/cms/pages/{pageId}/sections  // Add section
PUT    /api/cms/pages/{pageId}/sections/{sectionId}
DELETE /api/cms/pages/{pageId}/sections/{sectionId}

// Components
GET    /api/cms/components               // Available components
POST   /api/cms/pages/{pageId}/sections/{sectionId}/components
PUT    /api/cms/pages/{pageId}/sections/{sectionId}/components/{componentId}

// Live Preview
POST   /api/cms/pages/{pageId}/preview   // Generate live preview
GET    /api/cms/pages/{pageId}/preview-html
```

**Data Model**:

```csharp
public class CmsPage
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    
    public List<CmsSection> Sections { get; set; }
    
    public PageVisibility Visibility { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime ScheduledPublish { get; set; }
    
    public SeoMetadata SeoMetadata { get; set; }
    
    public Guid TenantId { get; set; }
    public int Version { get; set; }
}

public class CmsSection
{
    public Guid Id { get; set; }
    public string Type { get; set; }  // "hero", "features", "testimonials", etc.
    public int Order { get; set; }
    
    public SectionLayout Layout { get; set; }
    public List<CmsComponent> Components { get; set; }
    
    public Dictionary<string, object> Settings { get; set; }
}

public class CmsComponent
{
    public Guid Id { get; set; }
    public string Type { get; set; }  // "button", "text", "image", etc.
    public string Content { get; set; }
    
    public List<ComponentVariable> Variables { get; set; }
    public Dictionary<string, string> Styling { get; set; }
    
    public bool IsVisible { get; set; }
}

public class SeoMetadata
{
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaKeywords { get; set; }
    public string OgImage { get; set; }
    public string CanonicalUrl { get; set; }
    public string StructuredData { get; set; }  // JSON-LD
}
```

### 2. Theme Service (Port 5009)

**Purpose**: Manage visual styling, colors, typography, and design system

**Key Endpoints**:

```csharp
// Theme Configuration
GET    /api/cms/themes                   // List available themes
POST   /api/cms/themes                   // Create new theme
GET    /api/cms/themes/{themeId}         // Get theme details
PUT    /api/cms/themes/{themeId}         // Update theme
DELETE /api/cms/themes/{themeId}         // Delete theme

// Style Variables
GET    /api/cms/themes/{themeId}/variables
PUT    /api/cms/themes/{themeId}/variables // Batch update

// CSS Generation
POST   /api/cms/themes/{themeId}/generate-css
GET    /api/cms/themes/{themeId}/css     // Get compiled CSS

// Publishing
POST   /api/cms/themes/{themeId}/publish // Activate theme
POST   /api/cms/themes/{themeId}/preview // Generate preview

// Asset Management
POST   /api/cms/themes/{themeId}/assets  // Upload logo, favicon, etc.
GET    /api/cms/themes/{themeId}/assets
DELETE /api/cms/themes/{themeId}/assets/{assetId}
```

**Data Model**:

```csharp
public class CmsTheme
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    
    public ColorScheme Colors { get; set; }
    public Typography Typography { get; set; }
    public Spacing Spacing { get; set; }
    
    public Dictionary<string, StyleVariable> Variables { get; set; }
    public List<ThemeAsset> Assets { get; set; }
    
    public CustomCssConfig CustomCss { get; set; }
    
    public Guid TenantId { get; set; }
    public int Version { get; set; }
    public DateTime PublishedAt { get; set; }
}

public class ColorScheme
{
    public string Primary { get; set; }      // #FF6B6B
    public string Secondary { get; set; }
    public string Accent { get; set; }
    
    public string Background { get; set; }
    public string Surface { get; set; }
    public string Text { get; set; }
    public string TextSecondary { get; set; }
    
    public string Success { get; set; }
    public string Warning { get; set; }
    public string Error { get; set; }
    public string Info { get; set; }
}

public class Typography
{
    public FontConfig Heading1 { get; set; }
    public FontConfig Heading2 { get; set; }
    public FontConfig Heading3 { get; set; }
    public FontConfig Body { get; set; }
    public FontConfig Caption { get; set; }
    
    public List<string> FontFamilies { get; set; }
}

public class FontConfig
{
    public string FontFamily { get; set; }
    public int FontSize { get; set; }
    public int FontWeight { get; set; }
    public double LineHeight { get; set; }
    public int LetterSpacing { get; set; }
}

public class Spacing
{
    public int XSmall { get; set; }  // 4px
    public int Small { get; set; }   // 8px
    public int Base { get; set; }    // 16px
    public int Large { get; set; }   // 24px
    public int XLarge { get; set; }  // 32px
    public int XXLarge { get; set; } // 48px
}

public class ThemeAsset
{
    public Guid Id { get; set; }
    public string Type { get; set; }  // "logo", "favicon", "hero-image"
    public string Url { get; set; }
    public string AltText { get; set; }
}

public class StyleVariable
{
    public string Key { get; set; }        // "--color-primary"
    public string Value { get; set; }      // "#FF6B6B"
    public string Category { get; set; }   // "color", "spacing", "typography"
    public string Description { get; set; }
}
```

### 3. Content Service (Port 5011)

**Purpose**: Manage content, versioning, publishing, and SEO

**Key Endpoints**:

```csharp
// Publishing
POST   /api/cms/pages/{pageId}/publish    // Publish page
POST   /api/cms/pages/{pageId}/unpublish  // Unpublish page
POST   /api/cms/pages/{pageId}/schedule   // Schedule publishing

// Versioning
GET    /api/cms/pages/{pageId}/versions   // Get all versions
GET    /api/cms/pages/{pageId}/versions/{version}
POST   /api/cms/pages/{pageId}/revert     // Revert to previous version

// SEO
GET    /api/cms/seo-analysis/{pageId}     // SEO score & recommendations
PUT    /api/cms/pages/{pageId}/seo        // Update SEO settings

// Publishing
GET    /api/cms/publishing-queue          // Scheduled changes
POST   /api/cms/publishing-queue/{id}/execute

// Assets
POST   /api/cms/assets/upload             // Upload image/video
GET    /api/cms/assets/{assetId}/info
DELETE /api/cms/assets/{assetId}
```

**Data Model**:

```csharp
public class PublishingState
{
    public Guid PageId { get; set; }
    public PublishStatus Status { get; set; }  // Draft, Published, Scheduled
    
    public CmsPageVersion CurrentVersion { get; set; }
    public CmsPageVersion PublishedVersion { get; set; }
    
    public DateTime PublishedAt { get; set; }
    public DateTime ScheduledPublishAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}

public class CmsPageVersion
{
    public int VersionNumber { get; set; }
    public CmsPage PageSnapshot { get; set; }
    
    public string Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ChangeDescription { get; set; }
}

public enum PublishStatus
{
    Draft,
    Published,
    Scheduled,
    Archived
}

public class CmsAsset
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    
    public string StoragePath { get; set; }
    public List<string> ThumbnailUrls { get; set; }
    
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; }
}
```

## Component Library

### Built-in Components

The CMS includes a comprehensive library of reusable components:

```
UI Components:
├── Button
│   ├── Primary
│   ├── Secondary
│   ├── Outline
│   ├── Text
│   └── Icon Button
│
├── Input Elements
│   ├── Text Input
│   ├── Textarea
│   ├── Select
│   ├── Checkbox
│   └── Radio Button
│
├── Display Components
│   ├── Card
│   ├── Alert
│   ├── Badge
│   ├── Tag
│   ├── Progress Bar
│   └── Tooltip
│
├── Media Components
│   ├── Image
│   ├── Video Player
│   ├── Carousel
│   ├── Lightbox
│   └── Image Gallery
│
└── Navigation
    ├── Navbar
    ├── Breadcrumb
    ├── Pagination
    ├── Tabs
    └── Accordion

Layout Components:
├── Hero Section
├── Feature Section
├── CTA (Call-to-Action)
├── Testimonial Carousel
├── Product Grid
├── Blog Post List
├── Newsletter Signup
├── FAQ Section
├── Contact Form
└── Footer

E-Commerce Components:
├── Product Card
├── Product Carousel
├── Product Filter
├── Product Search
├── Shopping Cart Summary
├── Checkout Summary
├── Payment Methods
├── Order Status Tracker
└── Review Section
```

### Component Schema

```csharp
public class ComponentDefinition
{
    public string ComponentType { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Icon { get; set; }
    
    public List<ComponentProp> Props { get; set; }
    public List<ComponentSlot> Slots { get; set; }
    
    public ComponentPreset[] PresetVariants { get; set; }
}

public class ComponentProp
{
    public string Name { get; set; }
    public string Type { get; set; }  // "text", "number", "boolean", "select", etc.
    public object DefaultValue { get; set; }
    
    public List<string> Options { get; set; }  // For select
    public ComponentPropValidation Validation { get; set; }
    public string Description { get; set; }
}

public class ComponentPreset
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> Props { get; set; }
    public string PreviewImage { get; set; }
}
```

## Visual Layout Builder Features

### 1. Drag & Drop Interface

- **Section Management**: Add, remove, reorder sections
- **Component Placement**: Drag components within sections
- **Responsive Preview**: See changes on desktop, tablet, mobile
- **Live Preview Mode**: See changes in real-time
- **Undo/Redo Stack**: Full change history

### 2. Page Templates

Pre-built template categories:

```
E-Commerce:
- Product Showcase
- Shop Homepage
- Product Details
- Shopping Cart
- Checkout
- Order Confirmation

Information:
- About Us
- Contact Us
- FAQ
- Team
- Blog

Marketing:
- Landing Page
- Feature Highlight
- Pricing
- Comparison
- Case Study

Integration:
- Procurement Dashboard
- Supplier Portal
- Order Management
```

### 3. Responsive Design

- **Breakpoints**: Mobile (320px), Tablet (768px), Desktop (1024px)
- **Layout Grid**: 12-column grid system
- **Device Preview**: Switch between device types
- **Mobile-first**: Build from mobile up

### 4. Component Settings Panel

```
General
├── Component Type
├── Visibility
└── ID & Classes

Content
├── Text Content
├── Dynamic Data Binding
└── Links & Actions

Styling
├── Colors (override theme)
├── Typography (override theme)
├── Spacing & Sizing
├── Borders & Shadows
├── Animation
└── Custom CSS

SEO (for specific components)
├── Heading Structure
├── Alt Text
└── Schema Markup

Advanced
├── Conditional Display
├── Data Source
└── Event Handlers
```

## Theme Customization System

### 1. Design System Variables

All theme variables follow CSS custom properties (variables):

```css
/* Colors */
--color-primary: #FF6B6B;
--color-secondary: #4ECDC4;
--color-accent: #FFE66D;
--color-background: #FFFFFF;
--color-surface: #F7F7F7;
--color-text: #2C3E50;
--color-text-secondary: #95A5A6;

/* Semantic Colors */
--color-success: #27AE60;
--color-warning: #F39C12;
--color-error: #E74C3C;
--color-info: #3498DB;

/* Typography */
--font-primary: 'Inter', sans-serif;
--font-secondary: 'Poppins', sans-serif;
--font-size-sm: 12px;
--font-size-base: 16px;
--font-size-lg: 18px;
--font-size-xl: 24px;
--font-size-2xl: 32px;
--font-weight-normal: 400;
--font-weight-semibold: 600;
--font-weight-bold: 700;

/* Spacing */
--spacing-xs: 4px;
--spacing-sm: 8px;
--spacing-md: 16px;
--spacing-lg: 24px;
--spacing-xl: 32px;
--spacing-2xl: 48px;

/* Shadows */
--shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
--shadow-md: 0 4px 6px rgba(0, 0, 0, 0.1);
--shadow-lg: 0 10px 15px rgba(0, 0, 0, 0.1);
```

### 2. Custom CSS Support

Customers can add custom CSS while maintaining security:

```csharp
public class CustomCssConfig
{
    public string CustomCss { get; set; }  // Sanitized & scoped
    
    // Settings
    public bool AllowCustomCss { get; set; }
    public int CustomCssCharLimit { get; set; }
    
    // Safety
    public List<string> BlockedSelectors { get; set; }
    public List<string> BlockedProperties { get; set; }
}
```

### 3. Color Scheme Generator

- **Base Color Input**: Choose primary color
- **Auto-generate**: Complementary colors, shades, tints
- **Presets**: Light, Dark, High Contrast modes
- **Preview**: See colors in context

### 4. Font Management

- **Google Fonts Integration**: Browse and preview
- **Font Upload**: Support for custom fonts
- **Font Pairing**: AI recommendations
- **Performance**: Font subsetting and optimization

## Frontend Rendering

### Dynamic Theme Application

The frontend automatically loads and applies theme:

```typescript
// src/composables/useTheme.ts
export const useTheme = () => {
  const themeId = useRoute().params.tenantId;
  
  const loadTheme = async () => {
    const theme = await api.get(`/api/cms/themes/${themeId}`);
    applyThemeVariables(theme);
    loadCustomCss(theme.customCss);
  };
  
  const applyThemeVariables = (theme) => {
    const root = document.documentElement;
    Object.entries(theme.variables).forEach(([key, value]) => {
      root.style.setProperty(key, value);
    });
  };
  
  onMounted(loadTheme);
  
  return { loadTheme };
};
```

### Layout Rendering

```typescript
// src/components/CmsPageRenderer.vue
<template>
  <div class="cms-page">
    <CmsSection
      v-for="section in page.sections"
      :key="section.id"
      :section="section"
      :components="section.components"
    />
  </div>
</template>

<script setup>
import CmsSection from './CmsSection.vue';
import { useCms } from '@/composables/useCms';

const props = defineProps({
  pageId: String
});

const { page } = await useCms().fetchPage(props.pageId);
</script>
```

## CMS Workflow

### 1. Create/Edit Pages

```
User opens Page Builder
    ↓
Selects Template or Starts Blank
    ↓
Drags & Drops Sections
    ↓
Configures Components
    ↓
Customizes Theme Variables
    ↓
Live Preview
    ↓
Save as Draft
    ↓
Schedule or Publish
```

### 2. Publishing Process

```
Page Status Transitions:
    
    Draft ──Save──→ Draft
      ↓
      └──Publish──→ Published ──Unpublish──→ Draft
      
    Draft ──Schedule──→ Scheduled ──Auto Publish──→ Published
```

### 3. Version Control

- **Auto-save**: Every 30 seconds
- **Manual Save Points**: Named versions
- **Revision History**: View all changes
- **Restore**: Rollback to any version
- **Diff View**: Compare versions

## SEO Features

### 1. On-Page SEO

- **Meta Tags**: Title, description, keywords
- **Headings**: Proper h1, h2, h3 structure
- **Image Alt Text**: Required for all images
- **Internal Links**: Link suggestions
- **URL Structure**: Customizable slugs

### 2. SEO Analysis

```csharp
public class SeoAnalysis
{
    public int OverallScore { get; set; }  // 0-100
    
    public List<SeoIssue> Issues { get; set; }
    public List<SeoRecommendation> Recommendations { get; set; }
    
    public int WordCount { get; set; }
    public double ReadingTime { get; set; }
    public List<string> Keywords { get; set; }
}

public class SeoIssue
{
    public SeoIssueSeverity Severity { get; set; }  // Critical, Warning, Info
    public string Title { get; set; }
    public string Description { get; set; }
    public string FixSuggestion { get; set; }
}
```

### 3. Structured Data (Schema Markup)

- **Product Schema**: For product pages
- **Organization Schema**: Company information
- **Article Schema**: Blog posts
- **LocalBusiness**: For local shops
- **FAQ Schema**: FAQ sections

## Performance Optimization

### 1. Asset Optimization

- **Image Compression**: Automatic optimization
- **Format Conversion**: WebP with fallback
- **Lazy Loading**: Images below fold
- **Responsive Images**: srcset generation

### 2. CSS & JS Optimization

- **CSS Minification**: Production CSS compressed
- **Critical CSS**: Inline for above-fold
- **Code Splitting**: Per-section loading
- **Caching**: Long-term cache headers

### 3. CDN Integration

- **Global Distribution**: CloudFront/Cloudflare
- **Edge Caching**: Theme & asset distribution
- **Cache Invalidation**: On publish
- **Performance Monitoring**: Core Web Vitals

## Security Considerations

### 1. Input Validation

- **Content Sanitization**: Prevent XSS attacks
- **HTML Sanitization**: Using Sanitize.dll
- **CSS Validation**: Block dangerous properties
- **URL Validation**: Safe redirect URLs

### 2. Access Control

- **Role-based Access**: Admin, Editor, Viewer
- **Draft Preview**: Secure token-based access
- **Publish Approval**: Workflow approval if enabled
- **Audit Logging**: Track all changes

### 3. Data Protection

- **Tenant Isolation**: Strict RBAC
- **Encryption**: Sensitive data at rest
- **Versioning**: Safe rollback capability
- **Backup**: Regular automated backups

## Integration Points

### 1. E-Commerce Integration

```csharp
// Dynamic product data in CMS
POST /api/cms/pages/{pageId}/data-bindings
{
    "componentId": "product-grid-1",
    "dataSource": "catalog-service",
    "query": "/api/products?category=electronics",
    "mapping": {
        "title": "productName",
        "image": "imageUrl",
        "price": "price"
    }
}
```

### 2. Marketing Tools

- **Email Integration**: Newsletter signup forms
- **Analytics**: GA4, Hotjar integration
- **A/B Testing**: Multiple variants
- **Forms**: Lead capture, contact forms

### 3. Third-party Services

- **Google Fonts**: Font library
- **Unsplash/Pexels**: Stock images
- **Payment Processors**: Stripe badges
- **Social Media**: Embedding

## Database Schema

```sql
-- CMS Tables
CREATE TABLE cms_pages (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    title VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL,
    description TEXT,
    layout_json JSONB,
    visibility VARCHAR(50),
    published_at TIMESTAMP,
    scheduled_publish_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    created_by UUID,
    updated_by UUID,
    version INT DEFAULT 1,
    UNIQUE(tenant_id, slug)
);

CREATE TABLE cms_sections (
    id UUID PRIMARY KEY,
    page_id UUID NOT NULL REFERENCES cms_pages(id),
    type VARCHAR(50) NOT NULL,
    order_index INT,
    settings_json JSONB,
    created_at TIMESTAMP,
    FOREIGN KEY (page_id) REFERENCES cms_pages(id)
);

CREATE TABLE cms_components (
    id UUID PRIMARY KEY,
    section_id UUID NOT NULL REFERENCES cms_sections(id),
    type VARCHAR(50) NOT NULL,
    content TEXT,
    props_json JSONB,
    styling_json JSONB,
    is_visible BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP,
    FOREIGN KEY (section_id) REFERENCES cms_sections(id)
);

CREATE TABLE cms_themes (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    is_active BOOLEAN DEFAULT FALSE,
    is_default BOOLEAN DEFAULT FALSE,
    colors_json JSONB NOT NULL,
    typography_json JSONB NOT NULL,
    spacing_json JSONB NOT NULL,
    variables_json JSONB,
    custom_css TEXT,
    published_at TIMESTAMP,
    created_at TIMESTAMP,
    updated_at TIMESTAMP,
    version INT DEFAULT 1
);

CREATE TABLE cms_theme_assets (
    id UUID PRIMARY KEY,
    theme_id UUID NOT NULL REFERENCES cms_themes(id),
    type VARCHAR(50),
    url VARCHAR(1024),
    alt_text VARCHAR(255),
    created_at TIMESTAMP
);

CREATE TABLE cms_page_versions (
    id UUID PRIMARY KEY,
    page_id UUID NOT NULL REFERENCES cms_pages(id),
    version_number INT NOT NULL,
    snapshot JSONB NOT NULL,
    author_id UUID,
    change_description TEXT,
    created_at TIMESTAMP,
    UNIQUE(page_id, version_number)
);

CREATE TABLE cms_assets (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    content_type VARCHAR(100),
    file_size BIGINT,
    storage_path VARCHAR(1024),
    uploaded_at TIMESTAMP,
    uploaded_by UUID,
    created_at TIMESTAMP
);

CREATE TABLE cms_seo_metadata (
    id UUID PRIMARY KEY,
    page_id UUID NOT NULL REFERENCES cms_pages(id),
    meta_title VARCHAR(255),
    meta_description TEXT,
    meta_keywords VARCHAR(1024),
    og_image VARCHAR(1024),
    canonical_url VARCHAR(1024),
    structured_data JSONB,
    created_at TIMESTAMP,
    updated_at TIMESTAMP
);

-- Indexes
CREATE INDEX idx_cms_pages_tenant ON cms_pages(tenant_id);
CREATE INDEX idx_cms_pages_slug ON cms_pages(slug);
CREATE INDEX idx_cms_sections_page ON cms_sections(page_id);
CREATE INDEX idx_cms_components_section ON cms_components(section_id);
CREATE INDEX idx_cms_themes_tenant ON cms_themes(tenant_id);
CREATE INDEX idx_cms_themes_active ON cms_themes(tenant_id, is_active);
CREATE INDEX idx_cms_assets_tenant ON cms_assets(tenant_id);
```

## API Response Examples

### Create Page

```json
POST /api/cms/pages
{
  "title": "Summer Collection",
  "slug": "summer-collection",
  "description": "Our latest summer products",
  "sections": [
    {
      "type": "hero",
      "components": [
        {
          "type": "text",
          "content": "Summer Collection 2025"
        }
      ]
    }
  ]
}

Response 201:
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Summer Collection",
  "slug": "summer-collection",
  "visibility": "draft",
  "version": 1,
  "createdAt": "2025-12-25T10:00:00Z"
}
```

### Update Theme

```json
PUT /api/cms/themes/550e8400-e29b-41d4-a716-446655440001
{
  "name": "Corporate Blue Theme",
  "colors": {
    "primary": "#0066CC",
    "secondary": "#00CCFF",
    "accent": "#FF9900"
  },
  "typography": {
    "heading1": {
      "fontFamily": "Poppins",
      "fontSize": 32,
      "fontWeight": 700
    }
  },
  "variables": {
    "--border-radius": "8px",
    "--shadow-lg": "0 10px 15px rgba(0,0,0,0.2)"
  }
}

Response 200: OK
```

## Client-side Usage

### Vue 3 Hook

```typescript
// src/composables/useCmsBuilder.ts
export const useCmsBuilder = (pageId: string) => {
  const page = ref<CmsPage>();
  const theme = ref<CmsTheme>();
  const isDirty = ref(false);
  const history = ref<CmsPageVersion[]>([]);
  
  const loadPage = async () => {
    page.value = await api.get(`/api/cms/pages/${pageId}`);
  };
  
  const loadTheme = async () => {
    theme.value = await api.get(`/api/cms/themes/active`);
  };
  
  const savePage = async () => {
    await api.put(`/api/cms/pages/${pageId}`, page.value);
    isDirty.value = false;
  };
  
  const publishPage = async (schedule?: Date) => {
    await api.post(`/api/cms/pages/${pageId}/publish`, { 
      scheduledAt: schedule 
    });
  };
  
  const updateTheme = async (themeUpdates: Partial<CmsTheme>) => {
    theme.value = { ...theme.value, ...themeUpdates };
    await api.put(`/api/cms/themes/${theme.value.id}`, theme.value);
  };
  
  const previewPage = async () => {
    return await api.post(`/api/cms/pages/${pageId}/preview`);
  };
  
  watch(page, () => { isDirty.value = true; }, { deep: true });
  
  onMounted(() => {
    loadPage();
    loadTheme();
  });
  
  return {
    page,
    theme,
    isDirty,
    history,
    savePage,
    publishPage,
    updateTheme,
    previewPage
  };
};
```

## Benefits

✅ **No-Code Solution**: Customers can build beautiful storefronts without developers  
✅ **Brand Consistency**: Design system ensures consistent branding  
✅ **Quick Time-to-Market**: Pre-built components and templates accelerate launch  
✅ **SEO Optimization**: Built-in SEO tools improve search visibility  
✅ **Multi-tenant Isolation**: Secure separation of customer data  
✅ **Version Control**: Safe experimentation with rollback capability  
✅ **Performance**: Optimized assets and caching for fast load times  
✅ **Responsive Design**: Mobile-first approach ensures mobile-friendly sites  
✅ **Extensible**: REST APIs allow custom integrations  

## Roadmap

### Phase 1 (MVP)
- ✅ Basic page builder
- ✅ Component library (30+ components)
- ✅ Theme customization
- ✅ Publishing workflow
- ✅ SEO basics

### Phase 2 (v1.5)
- AI-powered design suggestions
- A/B testing framework
- Email editor integration
- Advanced analytics
- Custom component creation

### Phase 3 (v2.0)
- Headless CMS mode
- GraphQL API
- Webhooks & automation
- Marketplace for third-party components
- Multi-language support

### Phase 4 (v3.0)
- AI content generation
- Personalization engine
- Progressive Web App builder
- Mobile app builder
- Omnichannel page management
