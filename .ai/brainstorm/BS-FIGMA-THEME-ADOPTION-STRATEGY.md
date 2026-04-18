---
docid: BS-FIGMA-THEME-ADOPTION
title: Figma-Based Theme Adoption Strategy
owner: @SARAH
status: Brainstorm
created: 2026-01-11
related: ADR-041
---

# 🎨 Figma-Based Theme Adoption Strategy

## Problem Statement

**Current pain points:**
- Tenant onboarding requires manual CSS customization
- No visual design tool integration for non-developers
- Style extraction from existing brand websites is manual and error-prone
- Theme changes require developer intervention

**Goal:** Create a frictionless flow where:
1. Extract styles from existing website → auto-generate theme template
2. User/designer edits in Figma (visual tool)
3. Figma export → B2X import
4. Theme applied to tenant store with zero code changes

---

## Proposed Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         THEME ADOPTION PIPELINE                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐    ┌────────────┐ │
│  │  1. EXTRACT  │───▶│  2. CONVERT  │───▶│   3. EDIT    │───▶│  4. APPLY  │ │
│  │              │    │              │    │              │    │            │ │
│  │  Website     │    │  Theme       │    │  Figma       │    │  B2X       │ │
│  │  Scraper     │    │  Generator   │    │  Designer    │    │  Tenant    │ │
│  └──────────────┘    └──────────────┘    └──────────────┘    └────────────┘ │
│        │                    │                   │                   │        │
│        ▼                    ▼                   ▼                   ▼        │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐    ┌────────────┐ │
│  │ Extracted    │    │ B2X Theme    │    │ Figma        │    │ CSS Vars   │ │
│  │ CSS/Assets   │    │ JSON Schema  │    │ Variables    │    │ Runtime    │ │
│  └──────────────┘    └──────────────┘    └──────────────┘    └────────────┘ │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Phase 1: Style Extraction Engine

### 1.1 Website Scraper Service

**Purpose:** Analyze existing website and extract design tokens.

**Inputs:**
- URL of existing tenant website
- Optional: specific pages to analyze

**Outputs:**
- Extracted color palette
- Typography scale
- Spacing values
- Component patterns (buttons, cards, etc.)
- Logo/brand assets

**Technical Approaches:**

| Approach | Pros | Cons |
|----------|------|------|
| **Puppeteer/Playwright** | Renders JS, accurate styles | Heavy, requires browser |
| **CSS Parser (PostCSS)** | Fast, lightweight | Misses computed styles |
| **Chrome DevTools Protocol** | Full style inspection | Complex setup |
| **AI Vision (GPT-4V/Claude)** | Understands visual intent | Costly, non-deterministic |

**Recommended:** Hybrid approach
1. Playwright for full page render + computed styles
2. AI vision for semantic understanding ("this is primary button color")
3. CSS parser for static asset extraction

### 1.2 Extraction Algorithm

```typescript
interface ExtractedTheme {
  colors: {
    primary: ColorScale;      // Auto-detected brand color
    secondary: ColorScale;    // Secondary brand color
    neutral: ColorScale;      // Greys
    success: string;
    warning: string;
    danger: string;
    background: string;
    surface: string;
    text: TextColors;
  };
  typography: {
    fontFamilies: {
      heading: string;
      body: string;
      mono?: string;
    };
    scale: TypeScale;         // Computed from H1-H6, body, etc.
    weights: number[];
  };
  spacing: number[];          // Detected spacing rhythm
  radii: number[];            // Border radii
  shadows: Shadow[];          // Box shadows
  assets: {
    logo: string;             // URL or base64
    favicon: string;
    ogImage?: string;
  };
}
```

### 1.3 Color Extraction Strategy

```typescript
// Step 1: Collect all colors from computed styles
const allColors = await page.evaluate(() => {
  const elements = document.querySelectorAll('*');
  const colors = new Set<string>();
  elements.forEach(el => {
    const styles = getComputedStyle(el);
    colors.add(styles.color);
    colors.add(styles.backgroundColor);
    colors.add(styles.borderColor);
  });
  return Array.from(colors);
});

// Step 2: Cluster similar colors
// Step 3: Identify dominant colors by frequency + prominence
// Step 4: Map to semantic roles (primary, secondary, etc.)
```

---

## Phase 2: Theme Template Generator

### 2.1 B2X Theme Schema

Define a canonical theme format that bridges Figma ↔ B2X:

```json
{
  "$schema": "https://b2x.io/schemas/theme/v1.json",
  "name": "Tenant Theme",
  "version": "1.0.0",
  "tokens": {
    "color": {
      "brand": {
        "primary": {
          "50": "#FEF2F2",
          "500": "#C00018",
          "600": "#9A0013"
        }
      },
      "neutral": { ... },
      "feedback": { ... }
    },
    "typography": {
      "fontFamily": {
        "heading": "Inter",
        "body": "Inter"
      },
      "fontSize": { ... },
      "fontWeight": { ... },
      "lineHeight": { ... }
    },
    "spacing": {
      "1": "4px",
      "2": "8px",
      ...
    },
    "radius": { ... },
    "shadow": { ... }
  },
  "components": {
    "button": {
      "primary": {
        "background": "{color.brand.primary.500}",
        "hover": "{color.brand.primary.600}",
        "text": "#FFFFFF"
      }
    },
    "card": { ... },
    "input": { ... }
  }
}
```

### 2.2 Theme Variants

Support multiple modes per theme:

```json
{
  "modes": {
    "light": { ... },
    "dark": { ... },
    "high-contrast": { ... }
  }
}
```

---

## Phase 3: Figma Integration

### 3.1 Figma Variables API

**Key Capability:** Figma Variables (2023+) allow programmatic import/export of design tokens.

**Workflow:**

```
B2X Theme JSON  ──▶  Figma Variables  ──▶  Edit in Figma  ──▶  Export JSON  ──▶  B2X
```

**Figma API Endpoints:**
- `POST /v1/files/:file_key/variables` - Create/update variables
- `GET /v1/files/:file_key/variables/local` - Export variables

### 3.2 Token Sync Service

```typescript
// b2x-theme-to-figma.ts
async function syncToFigma(theme: B2XTheme, figmaFileKey: string) {
  const figmaVariables = transformToFigmaFormat(theme.tokens);
  
  await figmaApi.post(`/files/${figmaFileKey}/variables`, {
    variableCollections: [{
      name: "B2X Theme",
      modes: [{ name: "Default", modeId: "default" }],
      variables: figmaVariables
    }]
  });
}

// figma-to-b2x-theme.ts
async function syncFromFigma(figmaFileKey: string): Promise<B2XTheme> {
  const { variables } = await figmaApi.get(`/files/${figmaFileKey}/variables/local`);
  return transformToB2XFormat(variables);
}
```

### 3.3 Figma Plugin (Optional Enhancement)

Build a Figma plugin for seamless experience:

**Features:**
- One-click "Import B2X Theme"
- Live preview of token changes
- "Push to B2X" button
- Validation (missing tokens, accessibility contrast checks)

### 3.4 Tokens Studio Integration

Consider supporting [Tokens Studio](https://tokens.studio/) format as alternative:
- Popular Figma plugin with large user base
- Exports W3C Design Tokens format
- Git sync built-in

---

## Phase 4: B2X Theme Runtime

### 4.1 CSS Custom Properties Injection

```typescript
// theme-runtime.ts
export function applyTheme(theme: B2XTheme) {
  const root = document.documentElement;
  
  // Flatten tokens to CSS variables
  const cssVars = flattenTokens(theme.tokens);
  
  Object.entries(cssVars).forEach(([key, value]) => {
    root.style.setProperty(`--${key}`, value);
  });
}

// Example output:
// --color-brand-primary-500: #C00018;
// --typography-font-family-heading: Inter;
// --spacing-4: 16px;
```

### 4.2 Server-Side Theme Loading

```typescript
// Nuxt plugin: plugins/theme.server.ts
export default defineNuxtPlugin(async (nuxtApp) => {
  const tenantId = useTenantId();
  const theme = await $fetch(`/api/tenants/${tenantId}/theme`);
  
  // Inject CSS variables into SSR HTML
  useHead({
    style: [{
      children: `:root { ${generateCSSVars(theme)} }`
    }]
  });
});
```

### 4.3 Theme Admin UI

Build admin interface for non-technical users:

```
┌─────────────────────────────────────────────────────────────┐
│  🎨 Theme Editor                                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  [Import from Website] [Import from Figma] [Upload JSON]    │
│                                                              │
│  ┌─────────────────┐  ┌─────────────────────────────────┐   │
│  │ Token Navigator │  │ Live Preview                     │   │
│  │                 │  │ ┌─────────────────────────────┐ │   │
│  │ ▼ Colors        │  │ │                             │ │   │
│  │   ▼ Brand       │  │ │    [Simulated Store Page]   │ │   │
│  │     Primary     │  │ │                             │ │   │
│  │     Secondary   │  │ │                             │ │   │
│  │   ▼ Neutral     │  │ └─────────────────────────────┘ │   │
│  │ ▼ Typography    │  │                                 │   │
│  │ ▼ Spacing       │  │ [Save Draft] [Publish Theme]    │   │
│  └─────────────────┘  └─────────────────────────────────┘   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Phase 5: Workflow Automation

### 5.1 End-to-End User Journey

```
1. ONBOARDING
   └─▶ Tenant admin enters existing website URL
       └─▶ System extracts theme automatically
           └─▶ Preview generated theme on sample store

2. CUSTOMIZATION (Option A: In-App Editor)
   └─▶ Use built-in theme editor
       └─▶ Adjust colors, fonts, spacing
           └─▶ Live preview updates

2. CUSTOMIZATION (Option B: Figma Workflow)
   └─▶ Export theme to Figma file
       └─▶ Designer edits in Figma
           └─▶ Import updated theme back to B2X

3. ACTIVATION
   └─▶ Review final theme
       └─▶ Publish to staging
           └─▶ QA approval
               └─▶ Publish to production
```

### 5.2 API Endpoints

```
POST   /api/themes/extract          # Extract from URL
POST   /api/themes/import/figma     # Import from Figma file
POST   /api/themes/import/json      # Import from JSON
GET    /api/themes/:id              # Get theme
PATCH  /api/themes/:id              # Update theme
POST   /api/themes/:id/publish      # Publish theme
GET    /api/themes/:id/css          # Get compiled CSS
```

---

## Technical Stack Decisions

### Extraction Engine
| Component | Technology | Reason |
|-----------|------------|--------|
| Browser automation | Playwright | Cross-browser, fast, TypeScript |
| CSS parsing | PostCSS | Industry standard, plugin ecosystem |
| Color analysis | Culori.js | Modern, tree-shakeable |
| AI analysis | Claude API | For semantic color mapping |

### Theme Runtime
| Component | Technology | Reason |
|-----------|------------|--------|
| Token format | W3C Design Tokens | Emerging standard |
| CSS generation | Style Dictionary | Battle-tested, extensible |
| Figma sync | Figma REST API | Official, variables support |
| Admin UI | Vue 3 + Vuetify | Consistent with B2X stack |

---

## Implementation Roadmap

### MVP (4 weeks)
- [ ] Basic color extraction from URL
- [ ] B2X Theme JSON schema
- [ ] CSS variable injection runtime
- [ ] Manual JSON upload in admin

### V1 (8 weeks)
- [ ] Full token extraction (typography, spacing, shadows)
- [ ] Figma Variables API integration
- [ ] In-app theme editor UI
- [ ] Theme versioning & rollback

### V2 (12 weeks)
- [ ] AI-powered semantic mapping
- [ ] Figma plugin
- [ ] Tokens Studio support
- [ ] Multi-mode themes (light/dark)
- [ ] Component-level customization

---

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Figma API changes | Medium | High | Abstract behind adapter layer |
| Extraction accuracy | High | Medium | AI fallback + manual override |
| Performance (CSS size) | Low | Medium | Tree-shake unused tokens |
| Browser compatibility | Low | High | PostCSS autoprefixer |

---

## Success Metrics

- **Onboarding time**: < 30 min from URL to live themed store
- **Designer adoption**: > 60% of theme changes via Figma
- **Support tickets**: < 5% of tenants need manual theme help
- **Theme consistency**: 0 visual regressions after theme import

---

## Next Steps

1. **Proof of Concept**: Build extraction engine for color + typography
2. **Schema Design**: Finalize B2X Theme JSON schema
3. **Figma Spike**: Validate Figma Variables API capabilities
4. **User Research**: Interview 3-5 tenants on current pain points

---

## Related Documents

- [ADR-041] Figma-based Tenant Design Integration
- [ARCH-008] Frontend Architecture
- [GL-012] Frontend Quality Standards

---

## 📋 Backlog Refinement: Open Questions

### OQ-001: Component-Level Styling

**Question:** How can specific components be styled independently?

**Sub-questions:**
- Should every component be customizable, or only key components?
- How granular should component tokens be? (e.g., `button.primary.background` vs `button.primary.hover.background.opacity`)
- How do we handle component variants (e.g., button sizes, card layouts)?
- Should tenants be able to hide/disable components entirely?

**Options to explore:**

| Approach | Description | Complexity |
|----------|-------------|------------|
| **Token-only** | Components use global tokens, no component-specific overrides | Low |
| **Component tokens** | Each component has its own token namespace | Medium |
| **Full component variants** | Complete component definitions with all states | High |
| **Slot-based** | Components have "slots" for customization (header, content, footer) | Medium |

**Proposed component customization tiers:**

```
Tier 1: Token-based (colors, fonts, spacing)
├── All components inherit from global tokens
├── No component-specific overrides
└── ~20 tokens to configure

Tier 2: Component tokens (recommended)
├── Global tokens + component-level overrides
├── Key components: Button, Card, Input, Header, Footer, ProductCard
└── ~60-80 tokens to configure

Tier 3: Full customization
├── Every component state configurable
├── Custom component variants
└── ~200+ tokens to configure
```

**Questions for @ProductOwner:**
- [ ] Which components do tenants most frequently want to customize?
- [ ] Is there demand for hiding/disabling default components?
- [ ] Do tenants want to add custom components (not just style existing ones)?

---

### OQ-002: Default Theme & Baseline

**Question:** What is the default/baseline theme?

**Sub-questions:**
- Should there be ONE canonical default, or multiple starting points?
- How do we ensure the default looks professional out-of-the-box?
- Should the default be "neutral" or "opinionated"?
- How do we handle theme inheritance (default → tenant overrides)?

**Options:**

| Option | Description | Pros | Cons |
|--------|-------------|------|------|
| **Neutral baseline** | Minimal, grey/blue palette, no strong branding | Works for any brand | Looks generic |
| **Opinionated default** | Polished, modern design (like Eisen-Pfeiffer) | Looks professional | May clash with brands |
| **Industry defaults** | Multiple defaults per industry (B2B, fashion, etc.) | Relevant starting point | More to maintain |
| **AI-generated** | Generate default based on tenant's brand URL | Personalized | Complexity, accuracy |

**Proposed approach:**

```
                    ┌─────────────────────┐
                    │   B2X System Theme  │  ← Hardcoded fallback
                    │   (Neutral Baseline)│
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │   Industry Preset   │  ← Optional starting point
                    │  (B2B, Retail, etc.)│
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │   Tenant Theme      │  ← Tenant customizations
                    │   (Overrides only)  │
                    └─────────────────────┘
```

**Default theme requirements:**
- [ ] WCAG AA accessible by default
- [ ] Responsive breakpoints pre-configured
- [ ] RTL support built-in
- [ ] Dark mode variant available
- [ ] Print styles included

---

### OQ-003: Pre-defined Style Templates

**Question:** How can we offer pre-defined styles/templates?

**Sub-questions:**
- What qualifies as a "template" vs a "theme"?
- Should templates be free or part of premium tiers?
- How do we maintain/update templates over time?
- Can tenants share/sell their themes?

**Template categories to consider:**

```
📁 Template Library
├── 🏭 Industry Templates
│   ├── B2B Industrial (tools, machinery)
│   ├── B2B Office Supplies
│   ├── Fashion & Apparel
│   ├── Electronics & Tech
│   ├── Food & Beverage
│   └── Home & Garden
│
├── 🎨 Style Templates
│   ├── Minimal / Clean
│   ├── Bold / Vibrant
│   ├── Classic / Traditional
│   ├── Modern / Tech
│   └── Luxury / Premium
│
└── 🌍 Regional Templates
    ├── DACH (German-speaking)
    ├── Nordic (Scandinavian minimal)
    ├── Southern Europe (warm colors)
    └── Asia-Pacific (compact layouts)
```

**Template structure:**

```json
{
  "templateId": "b2b-industrial-v1",
  "name": "B2B Industrial",
  "description": "Professional theme for tools and machinery wholesalers",
  "thumbnail": "/templates/b2b-industrial/preview.png",
  "category": ["industry/b2b", "style/professional"],
  "version": "1.0.0",
  "theme": {
    "tokens": { /* full token set */ },
    "components": { /* component overrides */ }
  },
  "figmaFileId": "abc123",  // For Figma-based editing
  "license": "free",        // free | premium | custom
  "author": "B2X Team"
}
```

**Template marketplace considerations:**
- [ ] Should tenants be able to create & share templates?
- [ ] Revenue sharing model for community templates?
- [ ] Quality assurance process for published templates?
- [ ] Template versioning & update notifications?

---

### OQ-004: Animation Support

**Question:** Can users add animations to their theme?

**Sub-questions:**
- What types of animations are appropriate for e-commerce?
- How do we balance creativity vs performance?
- Should animations be token-based or component-specific?
- How do we handle "reduce motion" accessibility?

**Animation categories:**

| Category | Examples | Performance Impact |
|----------|----------|-------------------|
| **Micro-interactions** | Button hover, input focus, checkbox toggle | Low |
| **Transitions** | Page transitions, modal open/close | Low-Medium |
| **Loading states** | Skeleton screens, spinners, progress | Low |
| **Scroll effects** | Fade-in on scroll, parallax | Medium |
| **Hero animations** | Animated banners, product showcases | Medium-High |
| **Complex animations** | Lottie, GSAP sequences | High |

**Proposed animation token system:**

```json
{
  "animation": {
    "duration": {
      "instant": "0ms",
      "fast": "100ms",
      "normal": "200ms",
      "slow": "300ms",
      "slower": "500ms"
    },
    "easing": {
      "default": "cubic-bezier(0.4, 0, 0.2, 1)",
      "in": "cubic-bezier(0.4, 0, 1, 1)",
      "out": "cubic-bezier(0, 0, 0.2, 1)",
      "bounce": "cubic-bezier(0.68, -0.55, 0.265, 1.55)"
    },
    "effects": {
      "fadeIn": { "enabled": true, "duration": "normal" },
      "slideUp": { "enabled": true, "distance": "16px" },
      "scale": { "enabled": false },
      "parallax": { "enabled": false, "speed": 0.5 }
    }
  }
}
```

**Animation presets:**

```
🎬 Animation Presets
├── 🚫 None (performance-focused, accessibility-first)
├── 🌱 Subtle (micro-interactions only)
├── ⚡ Standard (transitions + micro-interactions)
├── ✨ Expressive (scroll effects + hero animations)
└── 🎭 Custom (full control, advanced users)
```

**Implementation considerations:**

```vue
<!-- Component with animation tokens -->
<template>
  <button 
    class="btn"
    :style="{
      '--btn-transition-duration': theme.animation.duration.normal,
      '--btn-transition-easing': theme.animation.easing.default
    }"
  >
    <slot />
  </button>
</template>

<style>
.btn {
  transition: 
    background-color var(--btn-transition-duration) var(--btn-transition-easing),
    transform var(--btn-transition-duration) var(--btn-transition-easing);
}
.btn:hover {
  transform: translateY(-1px);
}

/* Respect user preferences */
@media (prefers-reduced-motion: reduce) {
  .btn {
    transition: none;
    transform: none;
  }
}
</style>
```

**Questions for @UX:**
- [ ] Which animations provide clear UX value vs decorative?
- [ ] Should we ship a Lottie integration for complex animations?
- [ ] How do we test animation performance across devices?

---

### OQ-005: Theme Editing Permissions

**Question:** Who can edit themes and at what level?

**Permission matrix:**

| Role | View | Edit Tokens | Edit Components | Publish | Delete |
|------|------|-------------|-----------------|---------|--------|
| Viewer | ✅ | ❌ | ❌ | ❌ | ❌ |
| Designer | ✅ | ✅ | ✅ | ❌ | ❌ |
| Admin | ✅ | ✅ | ✅ | ✅ | ❌ |
| Owner | ✅ | ✅ | ✅ | ✅ | ✅ |

**Questions:**
- [ ] Can multiple users edit simultaneously (conflict resolution)?
- [ ] Should there be an approval workflow before publish?
- [ ] How do we handle theme rollback permissions?

---

### OQ-006: Theme Versioning & Rollback

**Question:** How do we version themes and enable rollback?

**Proposed versioning:**

```
tenant-theme/
├── v1.0.0 (published: 2026-01-01) ← Production
├── v1.1.0 (published: 2026-01-15) ← Previous
├── v1.2.0-draft (modified: 2026-01-20) ← Current draft
└── v1.2.0-preview (deployed: staging)
```

**Features needed:**
- [ ] Auto-save drafts every N minutes
- [ ] Named versions with release notes
- [ ] One-click rollback to any previous version
- [ ] Side-by-side comparison view
- [ ] Preview any version before publishing

---

### OQ-007: Server-Side Template Overrides (Dynamic Components)

**Question:** Can we deliver component templates from the server at runtime, allowing tenants to fully customize component markup?

**Concept:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│                    TEMPLATE OVERRIDE ARCHITECTURE                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│   Request: GET /api/templates/ProductCard                                │
│                     │                                                    │
│                     ▼                                                    │
│   ┌─────────────────────────────────────────┐                           │
│   │         Template Resolution             │                           │
│   │                                         │                           │
│   │  1. Check: Tenant custom template?      │                           │
│   │     └─▶ Yes → Return tenant template    │                           │
│   │     └─▶ No  → Continue                  │                           │
│   │                                         │                           │
│   │  2. Check: Industry template?           │                           │
│   │     └─▶ Yes → Return industry template  │                           │
│   │     └─▶ No  → Continue                  │                           │
│   │                                         │                           │
│   │  3. Return: Default B2X template        │                           │
│   └─────────────────────────────────────────┘                           │
│                     │                                                    │
│                     ▼                                                    │
│   Response: { template: "<div class='product-card'>...", version: "1.2" }│
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

**Implementation approaches:**

| Approach | Description | Pros | Cons |
|----------|-------------|------|------|
| **Vue SFC Compiler** | Compile .vue templates on server, send render function | Full Vue features | Complex, security risk |
| **Template Strings** | Send HTML template, use `v-html` or dynamic component | Simple | XSS risk, no reactivity |
| **Render Functions** | Send serialized render function | Fast, reactive | Hard to author |
| **Slot-based overrides** | Default template with named slots for customization | Safe, structured | Limited flexibility |
| **Block-based templates** | Define "blocks" that can be replaced/extended | Balance of safety & flexibility | Medium complexity |

**Recommended: Block-Based Template System**

```typescript
// Template definition with replaceable blocks
interface ComponentTemplate {
  componentId: string;
  version: string;
  blocks: {
    [blockName: string]: {
      template: string;       // HTML/Vue template fragment
      props: string[];        // Available props in this block
      slots?: string[];       // Named slots exposed
    }
  };
  defaultLayout: string;      // How blocks are composed
}

// Example: ProductCard template
const productCardTemplate: ComponentTemplate = {
  componentId: "ProductCard",
  version: "1.0.0",
  blocks: {
    image: {
      template: `
        <div class="product-card__image">
          <img :src="product.image" :alt="product.title" />
          <slot name="badges" />
        </div>
      `,
      props: ["product"],
      slots: ["badges"]
    },
    content: {
      template: `
        <div class="product-card__content">
          <span class="product-card__brand">{{ product.brand }}</span>
          <h3 class="product-card__title">{{ product.title }}</h3>
          <slot name="price" />
        </div>
      `,
      props: ["product"],
      slots: ["price"]
    },
    actions: {
      template: `
        <div class="product-card__actions">
          <slot name="add-to-cart" />
        </div>
      `,
      props: [],
      slots: ["add-to-cart"]
    }
  },
  defaultLayout: `
    <article class="product-card">
      <block name="image" />
      <block name="content" />
      <block name="actions" />
    </article>
  `
};
```

**Tenant override example:**

```typescript
// Tenant wants to customize the image block
const tenantOverride = {
  componentId: "ProductCard",
  blocks: {
    image: {
      template: `
        <div class="product-card__image product-card__image--with-ribbon">
          <img :src="product.image" :alt="product.title" loading="lazy" />
          <div class="ribbon" v-if="product.isNew">NEU</div>
          <slot name="badges" />
        </div>
      `
    }
    // Other blocks inherit from default
  }
};
```

**Vue runtime component:**

```vue
<!-- DynamicComponent.vue -->
<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue';
import { useTemplateStore } from '@/stores/templates';

const props = defineProps<{
  componentId: string;
  componentProps: Record<string, any>;
}>();

const templateStore = useTemplateStore();

// Fetch and compile template at runtime
const DynamicRenderer = computed(() => {
  const template = templateStore.getTemplate(props.componentId);
  
  return defineAsyncComponent(async () => ({
    template: template.compiledTemplate,
    props: template.props,
    setup() {
      return props.componentProps;
    }
  }));
});
</script>

<template>
  <component :is="DynamicRenderer" v-bind="componentProps" />
</template>
```

**Security considerations:**

```typescript
// Template sanitization pipeline
async function processTemplate(rawTemplate: string): Promise<string> {
  // 1. Parse and validate Vue template syntax
  const ast = parse(rawTemplate);
  
  // 2. Whitelist allowed directives
  const allowedDirectives = ['v-if', 'v-else', 'v-for', 'v-bind', 'v-on', 'v-slot'];
  validateDirectives(ast, allowedDirectives);
  
  // 3. Whitelist allowed components
  const allowedComponents = ['img', 'div', 'span', 'a', 'button', 'slot', 'block'];
  validateComponents(ast, allowedComponents);
  
  // 4. Sanitize expressions (no function calls except whitelisted)
  const allowedFunctions = ['$t', 'formatPrice', 'formatDate'];
  sanitizeExpressions(ast, allowedFunctions);
  
  // 5. Compile to render function
  return compile(ast);
}
```

**Template editing workflow:**

```
┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐
│  1. BROWSE       │────▶│  2. CUSTOMIZE    │────▶│  3. PREVIEW      │
│                  │     │                  │     │                  │
│  Select component│     │  Edit blocks     │     │  Live preview    │
│  to customize    │     │  in code editor  │     │  with test data  │
└──────────────────┘     └──────────────────┘     └──────────────────┘
                                                           │
┌──────────────────┐     ┌──────────────────┐              │
│  5. PUBLISH      │◀────│  4. VALIDATE     │◀─────────────┘
│                  │     │                  │
│  Deploy to prod  │     │  Security scan   │
│                  │     │  + accessibility │
└──────────────────┘     └──────────────────┘
```

**Admin UI for template editing:**

```
┌─────────────────────────────────────────────────────────────────────────┐
│  🧩 Component Templates                                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────────────────────────────────────────────────────────────┐│
│  │ Component: ProductCard                    [Default] [Reset] [Save] ││
│  ├─────────────────────────────────────────────────────────────────────┤│
│  │                                                                     ││
│  │  Blocks:  [Image ▼] [Content] [Actions] [+ Add Block]              ││
│  │                                                                     ││
│  │  ┌─────────────────────────────┐  ┌─────────────────────────────┐  ││
│  │  │ Template Editor             │  │ Live Preview                │  ││
│  │  │                             │  │                             │  ││
│  │  │ <div class="product-card__i│  │  ┌─────────────────────┐    │  ││
│  │  │   <img                      │  │  │  [Product Image]    │    │  ││
│  │  │     :src="product.image"    │  │  │  ───────────────    │    │  ││
│  │  │     :alt="product.title"    │  │  │  Brand Name         │    │  ││
│  │  │     loading="lazy"          │  │  │  Product Title      │    │  ││
│  │  │   />                        │  │  │  €99.00             │    │  ││
│  │  │   <div class="ribbon"       │  │  │  [Add to Cart]      │    │  ││
│  │  │     v-if="product.isNew">   │  │  └─────────────────────┘    │  ││
│  │  │     NEU                     │  │                             │  ││
│  │  │   </div>                    │  │  Test data: [Product A ▼]  │  ││
│  │  │ </div>                      │  │                             │  ││
│  │  │                             │  │                             │  ││
│  │  └─────────────────────────────┘  └─────────────────────────────┘  ││
│  │                                                                     ││
│  │  Available props: product.image, product.title, product.brand,     ││
│  │                   product.price, product.isNew, product.sku        ││
│  │                                                                     ││
│  └─────────────────────────────────────────────────────────────────────┘│
│                                                                          │
│  [Validate] [Preview Full Page] [Publish to Staging] [Publish to Prod]  │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

**API endpoints:**

```
GET    /api/tenants/:id/templates                    # List all templates
GET    /api/tenants/:id/templates/:componentId       # Get specific template
PUT    /api/tenants/:id/templates/:componentId       # Update template
DELETE /api/tenants/:id/templates/:componentId       # Reset to default
POST   /api/tenants/:id/templates/:componentId/validate  # Validate template
GET    /api/templates/defaults                       # Get all default templates
GET    /api/templates/defaults/:componentId          # Get default template
```

**Performance optimization:**

```typescript
// Template caching strategy
class TemplateCache {
  private cache = new Map<string, CompiledTemplate>();
  private ttl = 3600; // 1 hour
  
  async getTemplate(tenantId: string, componentId: string) {
    const cacheKey = `${tenantId}:${componentId}`;
    
    if (this.cache.has(cacheKey)) {
      return this.cache.get(cacheKey);
    }
    
    // Fetch from API, compile, cache
    const template = await this.fetchAndCompile(tenantId, componentId);
    this.cache.set(cacheKey, template);
    
    return template;
  }
  
  invalidate(tenantId: string, componentId?: string) {
    // Called when template is updated
  }
}
```

**Customizable components (Phase 1):**

| Component | Blocks | Complexity |
|-----------|--------|------------|
| ProductCard | image, content, price, actions | Medium |
| ProductList | header, grid, item, pagination | Medium |
| Header | logo, nav, search, actions | High |
| Footer | columns, links, legal, social | Medium |
| CartItem | image, details, quantity, price | Low |
| CategoryTile | image, title, count | Low |

**Questions:**
- [x] Should we use a visual editor (drag-drop) or code editor for templates? → **Both! (like email templates)**
- [ ] How do we handle template migrations when default templates change?
- [ ] Should templates support TypeScript for better autocomplete?
- [ ] Rate limiting on template compilation to prevent abuse?

---

### OQ-008: Dual-Mode Template Editor (Visual + Code)

**Decision:** Offer both visual (WYSIWYG) and code editor modes, similar to the email template builder.

**Reference:** See [REQ-007] Email WYSIWYG Builder, [KB-028] GrapesJS Email Builder Integration

**Dual-mode architecture:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        TEMPLATE EDITOR MODES                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  [🎨 Visual] [</>Code] [👁️ Preview]                    [Save] [Publish]│    │
│  ├─────────────────────────────────────────────────────────────────────┤    │
│  │                                                                     │    │
│  │  VISUAL MODE                          CODE MODE                     │    │
│  │  ┌─────────────────────────┐          ┌─────────────────────────┐  │    │
│  │  │ ┌─────────────────────┐ │          │ <div class="product-   │  │    │
│  │  │ │   [Drag Block Here] │ │          │   card__image">        │  │    │
│  │  │ ├─────────────────────┤ │    ⟷     │   <img                 │  │    │
│  │  │ │ 📷 Image Block      │ │  Synced  │     :src="product.img" │  │    │
│  │  │ │   [Product Image]   │ │          │     :alt="product.name"│  │    │
│  │  │ ├─────────────────────┤ │          │   />                   │  │    │
│  │  │ │ 📝 Content Block    │ │          │   <Badge v-if="isNew"/>│  │    │
│  │  │ │   Brand / Title     │ │          │ </div>                 │  │    │
│  │  │ ├─────────────────────┤ │          │ <div class="product-   │  │    │
│  │  │ │ 💰 Price Block      │ │          │   card__content">      │  │    │
│  │  │ │   €99.00            │ │          │   ...                  │  │    │
│  │  │ └─────────────────────┘ │          └─────────────────────────┘  │    │
│  │  └─────────────────────────┘                                       │    │
│  │                                                                     │    │
│  │  Block Library              Block Properties                        │    │
│  │  ┌───────────────┐          ┌───────────────────────────────────┐  │    │
│  │  │ 📷 Image      │          │ CSS Classes: product-card__image  │  │    │
│  │  │ 📝 Text       │          │ Loading: [lazy ▼]                 │  │    │
│  │  │ 🏷️ Badge      │          │ Show ribbon: [✓] if isNew         │  │    │
│  │  │ 💰 Price      │          │ Ribbon text: [NEU]                │  │    │
│  │  │ 🛒 Add to Cart│          └───────────────────────────────────┘  │    │
│  │  │ ⭐ Rating     │                                                  │    │
│  │  │ 📦 Stock      │                                                  │    │
│  │  └───────────────┘                                                  │    │
│  │                                                                     │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

**Mode synchronization:**

```typescript
// Bidirectional sync between visual and code modes
interface TemplateEditorState {
  mode: 'visual' | 'code' | 'preview';
  template: {
    blocks: Block[];
    layout: string;
  };
  
  // Visual → Code: When user drags/configures in visual mode
  syncToCode(): string;
  
  // Code → Visual: When user edits code directly
  syncFromCode(code: string): Block[];
  
  // Conflict resolution
  lastEditMode: 'visual' | 'code';
  isDirty: boolean;
}

// Block definition for visual editor
interface Block {
  id: string;
  type: 'image' | 'text' | 'badge' | 'price' | 'button' | 'rating' | 'stock' | 'custom';
  props: Record<string, any>;
  children?: Block[];
  template?: string;  // For custom blocks
  cssClasses: string[];
  conditions?: {
    show: string;  // v-if expression
  };
}
```

**Visual editor capabilities:**

| Feature | Description | Complexity |
|---------|-------------|------------|
| **Drag & drop blocks** | Reorder, nest blocks visually | Medium |
| **Block properties panel** | Configure each block's props | Low |
| **Conditional visibility** | Show/hide based on data | Medium |
| **CSS class editor** | Add custom classes to blocks | Low |
| **Responsive preview** | Mobile/tablet/desktop views | Medium |
| **Undo/redo** | Full history of changes | Medium |
| **Block templates** | Save custom blocks for reuse | High |

**Code editor capabilities:**

| Feature | Description | Complexity |
|---------|-------------|------------|
| **Syntax highlighting** | Vue template syntax | Low (Monaco) |
| **Autocomplete** | Props, directives, components | Medium |
| **Error highlighting** | Invalid syntax, unknown props | Medium |
| **Format on save** | Prettier integration | Low |
| **Snippets** | Common patterns | Low |
| **Diff view** | Compare with default template | Medium |

**Technology stack:**

| Component | Technology | Reason |
|-----------|------------|--------|
| Visual editor | **Option A: GrapesJS** | Already used for email templates, proven |
| Visual editor | **Option B: Craft.js pattern** | React-native, but pattern is portable |
| Visual editor | **Option C: Custom (VueDraggable + Sortable)** | Full control, Vue-native |
| Code editor | Monaco Editor | Industry standard, TS support |
| Template parser | @vue/compiler-sfc | Official Vue parser |
| Preview renderer | Vue runtime compiler | Accurate preview |

**Visual editor technology comparison:**

| Technology | Type | Pros | Cons |
|------------|------|------|------|
| **GrapesJS** | Full page builder | Already integrated (email), mature, extensible | Heavy, learning curve for custom blocks |
| **VueDraggable + Sortable.js** | Drag-drop library | Lightweight, Vue-native, full control | Build everything from scratch |
| **Tiptap/ProseMirror** | Rich text + blocks | Great content editing, extensible | More for content than layouts |
| **BlockSuite** | Block-based editor | Modern, collaborative, open source | New, less mature |
| **Builder.io (pattern)** | Visual CMS | Professional UI, proven patterns | React-first, commercial model |
| ~~Vue Flow~~ | ~~Workflow editor~~ | ~~Node-based diagrams~~ | ❌ Wrong tool (flowcharts, not page building) |

**Recommended approach: GrapesJS + Custom Blocks**

Since GrapesJS is already integrated for email templates ([KB-028]), extend it for component templates:

```typescript
// Reuse GrapesJS with custom e-commerce blocks
const editor = grapesjs.init({
  container: '#template-editor',
  plugins: [
    'gjs-preset-webpage',      // Basic web blocks
    'b2x-ecommerce-blocks',    // Custom: price, rating, cart, etc.
    'b2x-vue-export',          // Custom: Export as Vue template
  ],
  storageManager: false,
  
  // Custom block categories
  blockManager: {
    blocks: [
      // E-commerce specific blocks
      {
        id: 'product-price',
        label: 'Price',
        category: 'E-Commerce',
        content: `<div data-b2x-block="price" class="price">
          <span class="price__current">{{ formatPrice(product.price) }}</span>
          <span class="price__old" v-if="product.oldPrice">{{ formatPrice(product.oldPrice) }}</span>
        </div>`,
        attributes: { class: 'fa fa-euro-sign' }
      },
      {
        id: 'add-to-cart',
        label: 'Add to Cart',
        category: 'E-Commerce',
        content: `<button data-b2x-block="add-to-cart" class="btn btn--primary" @click="addToCart(product)">
          {{ $t('cart.add') }}
        </button>`,
      },
      // ... more blocks
    ]
  }
});

// Export to Vue template (custom plugin)
editor.Commands.add('export-vue-template', {
  run(editor) {
    const html = editor.getHtml();
    const css = editor.getCss();
    return convertToVueTemplate(html, css);
  }
});
```

**Alternative: Lightweight custom editor**

If GrapesJS is too heavy for component editing, build a lighter custom solution:

```vue
<!-- LightweightBlockEditor.vue -->
<script setup lang="ts">
import { useSortable } from '@vueuse/integrations/useSortable';
import { ref } from 'vue';

const blocks = ref<Block[]>([
  { id: '1', type: 'image', props: {} },
  { id: '2', type: 'content', props: {} },
  { id: '3', type: 'price', props: {} },
]);

const containerRef = ref<HTMLElement>();
useSortable(containerRef, blocks, {
  animation: 150,
  handle: '.block-handle',
});
</script>

<template>
  <div class="block-editor">
    <!-- Block palette -->
    <aside class="block-palette">
      <BlockPaletteItem 
        v-for="blockDef in blockLibrary" 
        :key="blockDef.type"
        :block="blockDef"
        @add="addBlock"
      />
    </aside>
    
    <!-- Canvas -->
    <div ref="containerRef" class="block-canvas">
      <BlockWrapper 
        v-for="block in blocks" 
        :key="block.id"
        :block="block"
        @select="selectBlock"
        @delete="deleteBlock"
      >
        <component :is="getBlockComponent(block.type)" v-bind="block.props" />
      </BlockWrapper>
    </div>
    
    <!-- Properties panel -->
    <aside class="block-properties">
      <BlockPropertiesEditor 
        v-if="selectedBlock"
        :block="selectedBlock"
        @update="updateBlockProps"
      />
    </aside>
  </div>
</template>
```

**Decision matrix:**

| Criteria | GrapesJS | Custom (VueDraggable) |
|----------|----------|----------------------|
| Development time | Faster (extend existing) | Slower (build from scratch) |
| Bundle size | ~300KB | ~30KB |
| Flexibility | Medium (plugin system) | Full control |
| Learning curve | Medium (GrapesJS API) | Low (Vue-native) |
| Consistency with email editor | ✅ Same UX | Different UX |
| Vue-native feel | Needs adaptation | ✅ Native |

**Recommendation:** Start with **GrapesJS** for consistency with email editor UX, then evaluate if a lighter custom solution is needed for performance.

---

## 🚀 Implementation Plan (Dual-Mode Editor + Server-Side Templates)

### Phase 1 (2-3 weeks) — Foundation
- Scope: Code editor only, server-side template storage, runtime template fetch & cache
- Deliverables:
  - Monaco editor with Vue syntax + basic autocomplete
  - Template API: CRUD + validate (parse + whitelist directives/components)
  - Runtime loader: fetch → compile → cache → render blocks
  - Components covered: ProductCard, CategoryTile, Footer (low risk)
- Owners: @Frontend (editor/runtime), @Backend (template API/security)
- Risks: Vue compiler in browser size (mitigate via async chunk), template injection (mitigate via sanitizer)

### Phase 2 (3-4 weeks) — Visual Editor (Basic)
- Scope: Drag-drop, block properties, bidirectional sync to code
- Deliverables:
  - Visual editor using GrapesJS + custom B2X ecommerce blocks (or lightweight VueDraggable if chosen)
  - Sync engine: visual ↔ code (canonical AST → template)
  - Preview with test data; responsive breakpoints; undo/redo basic
  - More components: Header, ProductList, CartItem
- Owners: @Frontend (visual editor, sync), @UX (block UX), @Security (validation pass)
- Risks: Block library complexity (limit to ~10-15 blocks), sync drift (add tests on AST transform)

### Phase 3 (3-4 weeks) — Advanced Controls & Governance
- Scope: Versioning, staging previews, publish workflow, access control, migrations
- Deliverables:
  - Versioning: draft/save/publish, diff vs default, rollback
  - Permissions: role matrix (Viewer/Designer/Admin/Owner) enforcement in UI/API
  - Migrations: template change detection, guided remap when defaults change
  - Animation presets wired to tokens; micro-interactions default set
- Owners: @Frontend, @Backend, @Security; QA signoff @QA
- Risks: Migration complexity (add compatibility layer), performance under multi-tenant (add per-tenant cache TTL + invalidation)

### Phase 4 (2-3 weeks) — Templates & Marketplace (Optional)
- Scope: Predefined templates, industry presets, optional marketplace
- Deliverables:
  - Template library (3-5 industry presets) + thumbnails
  - Import/export theme + templates as JSON
  - Marketplace scaffolding (if approved): listing + apply flow
- Owners: @Frontend, @ProductOwner; Legal review for licensing if marketplace
- Risks: Curation overhead (limit initial set), licensing (keep internal first)

### Cross-Cutting Requirements
- Accessibility: WCAG AA defaults; respects prefers-reduced-motion
- Security: Sanitization pipeline (directives/components/functions whitelist); rate limiting on compile
- Performance: Async-load editor bundles; cache compiled templates; SSR-safe fallback to default templates
- Telemetry: Capture template errors, publish events, time-to-render

### Success Criteria
- <30 min from template edit to live preview
- 0 security findings on template validation
- Visual-code sync produces identical render (golden snapshot tests)
- Tenant can override 5 key components without codebase changes

**Comparison with email builder:**

| Aspect | Email Builder (GrapesJS) | Component Templates |
|--------|--------------------------|---------------------|
| Output | HTML + CSS (static) | Vue template (reactive) |
| Blocks | Email-specific (columns, images) | Component-specific (price, rating) |
| Data binding | Limited (merge tags) | Full Vue reactivity |
| Styling | Inline styles for email | CSS classes + tokens |
| Preview | Static HTML | Live with test data |

**User journey by skill level:**

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  USER SKILL LEVEL → RECOMMENDED MODE                                         │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  👤 Non-technical user (tenant admin)                                        │
│  └─▶ Visual mode only                                                        │
│      • Drag & drop blocks                                                    │
│      • Configure via properties panel                                        │
│      • Never sees code                                                       │
│                                                                              │
│  👤 Power user (marketing team)                                              │
│  └─▶ Visual mode + occasional code tweaks                                    │
│      • Use visual for layout                                                 │
│      • Switch to code for fine-tuning                                        │
│      • Add custom CSS classes                                                │
│                                                                              │
│  👤 Developer (agency/internal)                                              │
│  └─▶ Code mode primary                                                       │
│      • Full Vue template syntax                                              │
│      • Custom logic, computed props                                          │
│      • Create reusable block templates                                       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

**Block library (pre-built):**

```typescript
const blockLibrary: BlockDefinition[] = [
  // Layout blocks
  { type: 'container', name: 'Container', icon: '📦', category: 'layout' },
  { type: 'row', name: 'Row', icon: '➡️', category: 'layout' },
  { type: 'column', name: 'Column', icon: '⬇️', category: 'layout' },
  
  // Content blocks
  { type: 'image', name: 'Product Image', icon: '📷', category: 'content' },
  { type: 'text', name: 'Text', icon: '📝', category: 'content' },
  { type: 'heading', name: 'Heading', icon: '🔤', category: 'content' },
  
  // E-commerce blocks
  { type: 'price', name: 'Price Display', icon: '💰', category: 'ecommerce' },
  { type: 'badge', name: 'Badge', icon: '🏷️', category: 'ecommerce' },
  { type: 'rating', name: 'Star Rating', icon: '⭐', category: 'ecommerce' },
  { type: 'stock', name: 'Stock Status', icon: '📦', category: 'ecommerce' },
  { type: 'add-to-cart', name: 'Add to Cart', icon: '🛒', category: 'ecommerce' },
  { type: 'quantity', name: 'Quantity Stepper', icon: '🔢', category: 'ecommerce' },
  { type: 'variants', name: 'Variant Selector', icon: '🎨', category: 'ecommerce' },
  
  // Interactive blocks
  { type: 'button', name: 'Button', icon: '🔘', category: 'interactive' },
  { type: 'link', name: 'Link', icon: '🔗', category: 'interactive' },
  { type: 'accordion', name: 'Accordion', icon: '📂', category: 'interactive' },
  
  // Conditional blocks
  { type: 'condition', name: 'Show If', icon: '❓', category: 'logic' },
  { type: 'loop', name: 'Repeat', icon: '🔁', category: 'logic' },
];
```

**Implementation phases:**

```
Phase 1 (MVP): Code editor only
├── Monaco editor with Vue syntax
├── Basic autocomplete for props
├── Live preview with test data
└── Save/publish workflow

Phase 2: Visual editor (basic)
├── Drag & drop block reordering
├── Block properties panel
├── Block library (10-15 blocks)
└── Bidirectional sync with code

Phase 3: Visual editor (advanced)
├── Nested blocks
├── Conditional visibility UI
├── Responsive breakpoints
├── Custom block templates
└── Undo/redo history
```

---

## 🎯 Prioritized Backlog

### Must Have (MVP)
1. [ ] **Default system theme** - Neutral, accessible baseline
2. [ ] **Token-based customization** - Colors, typography, spacing
3. [ ] **Theme preview** - See changes before publishing
4. [ ] **One-click publish** - Apply theme to production

### Should Have (V1)
5. [ ] **3-5 industry templates** - B2B, Retail, Tech
6. [ ] **Component-level tokens** - Button, Card, Header, Footer
7. [ ] **Theme versioning** - Basic version history
8. [ ] **Micro-interaction animations** - Hover, focus, transitions

### Could Have (V2)
9. [ ] **Animation presets** - Subtle to Expressive
10. [ ] **Template marketplace** - Browse & apply templates
11. [ ] **Multi-user editing** - Collaboration features
12. [ ] **Custom component variants** - Beyond token overrides
13. [ ] **Server-side template overrides** - Block-based component customization
14. [ ] **Template editor UI** - Code editor with live preview
15. [ ] **Template validation** - Security scanning, accessibility checks

### Won't Have (Future)
16. [ ] Template revenue sharing
17. [ ] Complex animation builder (Lottie/GSAP UI)
18. [ ] AI-generated themes from brand guidelines PDF
19. [ ] Visual drag-drop template builder

---

**Owner**: @Frontend + @UX  
**Review**: @Architect + @ProductOwner  
**Created**: 2026-01-11

