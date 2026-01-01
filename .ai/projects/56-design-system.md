# Design System Specification - Issue #56 Phase 2

**Date:** January 2, 2026  
**Status:** In Progress  
**Component Library:** DaisyUI 5.5.14 + Tailwind CSS 4.1.18

## 🎨 B2Connect Design System

### Component Library Strategy

**Primary Framework:** DaisyUI (semantic component classes)
- **Why DaisyUI:** Pre-built, accessible components with consistent design tokens
- **Integration:** Built on Tailwind CSS with custom theme configuration
- **Customization:** Extended with project-specific variants and utilities
- **Accessibility:** WCAG 2.1 AA compliant out-of-the-box

**Base Components Available:**
- ✅ **Button** (`btn` classes) - Variants: primary, secondary, outline, ghost, success, warning, error
- ✅ **Input** (`input` classes) - Types: text, email, password, number, tel, url, search
- ✅ **Card** (`card` classes) - Variants: default, elevated, outlined, filled
- ✅ **Navigation** - Breadcrumb, pagination (Phase 2B)
- ✅ **Forms** - Select dropdown, textarea (Phase 2B)
- ✅ **Interactive** - Modal, dropdown, tooltip, alert (Phase 2B)

### Color Palette

#### Primary Colors
- **Primary:** `hsl(221 83% 53%)` - `#2563eb` (Tailwind blue-600)
- **Primary Content:** `hsl(210 40% 98%)` - `#f8fafc` (Slate-50)

#### Semantic Colors
- **Success:** `hsl(142 76% 36%)` - `#16a34a` (Emerald-600)
- **Warning:** `hsl(38 92% 50%)` - `#f59e0b` (Amber-500)
- **Error:** `hsl(0 84% 60%)` - `#dc2626` (Red-600)
- **Info:** `hsl(199 89% 48%)` - `#0284c7` (Sky-600)

#### Neutral Colors
- **Base-100:** `hsl(0 0% 100%)` - White
- **Base-200:** `hsl(210 20% 96%)` - `#f1f5f9` (Slate-100)
- **Base-300:** `hsl(210 16% 93%)` - `#e2e8f0` (Slate-200)
- **Base-Content:** `hsl(222 84% 5%)` - `#0f172a` (Slate-900)

### Typography Scale

#### Font Families
- **Sans:** Inter, system-ui, sans-serif
- **Mono:** JetBrains Mono, monospace

#### Text Sizes
- **xs:** 0.75rem (12px) - line-height: 1rem
- **sm:** 0.875rem (14px) - line-height: 1.25rem
- **base:** 1rem (16px) - line-height: 1.5rem
- **lg:** 1.125rem (18px) - line-height: 1.75rem
- **xl:** 1.25rem (20px) - line-height: 2rem
- **2xl:** 1.5rem (24px) - line-height: 2rem
- **3xl:** 1.875rem (30px) - line-height: 2.25rem

#### Font Weights
- **normal:** 400
- **medium:** 500
- **semibold:** 600
- **bold:** 700

### Spacing Scale

#### Spacing Values
- **1:** 0.25rem (4px)
- **2:** 0.5rem (8px)
- **3:** 0.75rem (12px)
- **4:** 1rem (16px)
- **5:** 1.25rem (20px)
- **6:** 1.5rem (24px)
- **8:** 2rem (32px)
- **10:** 2.5rem (40px)
- **12:** 3rem (48px)
- **16:** 4rem (64px)

### Component Specifications

#### Button System

##### Primary Button
```vue
<button class="btn btn-primary">
  <span>Button Text</span>
</button>
```

**Styles:**
- Background: `bg-primary`
- Text: `text-primary-content`
- Padding: `px-4 py-2`
- Border radius: `rounded-btn`
- Hover: `hover:bg-primary-focus`
- Focus: `focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2`

##### Secondary Button
```vue
<button class="btn btn-outline">
  <span>Button Text</span>
</button>
```

##### Ghost Button
```vue
<button class="btn btn-ghost">
  <span>Button Text</span>
</button>
```

##### Sizes
- **xs:** `btn-xs` - Small buttons for tight spaces
- **sm:** `btn-sm` - Default small size
- **md:** `btn` - Default size (base)
- **lg:** `btn-lg` - Large buttons for CTAs
- **xl:** `btn-xl` - Extra large for hero sections

#### Form Components

##### Input Field
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Field Label</span>
  </label>
  <input
    type="text"
    class="input input-bordered"
    :aria-describedby="errorId"
  />
  <div v-if="error" :id="errorId" class="label">
    <span class="label-text-alt text-error">{{ error }}</span>
  </div>
</div>
```

##### Select Dropdown
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Select Option</span>
  </label>
  <select class="select select-bordered">
    <option value="">Choose...</option>
    <option value="1">Option 1</option>
  </select>
</div>
```

##### Checkbox
```vue
<div class="form-control">
  <label class="label cursor-pointer">
    <span class="label-text">Accept terms</span>
    <input type="checkbox" class="checkbox checkbox-primary" />
  </label>
</div>
```

##### Radio Group
```vue
<div class="form-control">
  <label class="label">
    <span class="label-text">Choose option</span>
  </label>
  <div class="flex flex-col gap-2">
    <label class="label cursor-pointer">
      <span class="label-text">Option 1</span>
      <input type="radio" name="option" value="1" class="radio radio-primary" />
    </label>
  </div>
</div>
```

#### Card System

##### Product Card
```vue
<article class="card bg-base-100 shadow-lg hover:shadow-xl transition-all hover:-translate-y-1">
  <figure class="relative">
    <img :src="image" :alt="alt" class="w-full h-48 object-cover" />
  </figure>
  <div class="card-body">
    <h3 class="card-title">{{ title }}</h3>
    <p class="text-sm opacity-70">{{ description }}</p>
    <div class="card-actions justify-end">
      <button class="btn btn-primary">Add to Cart</button>
    </div>
  </div>
</article>
```

##### Content Card
```vue
<div class="card bg-base-100 shadow-md">
  <div class="card-body">
    <h3 class="card-title">{{ title }}</h3>
    <p>{{ content }}</p>
  </div>
</div>
```

### Layout Patterns

#### Container
```vue
<div class="container mx-auto px-4">
  <!-- Content -->
</div>
```

#### Grid System
```vue
<!-- Product Grid -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
  <!-- Product cards -->
</div>

<!-- Feature Grid -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
  <!-- Feature items -->
</div>
```

#### Flex Layouts
```vue
<!-- Navigation -->
<nav class="flex items-center justify-between">
  <!-- Logo -->
  <!-- Menu -->
</nav>

<!-- Card Actions -->
<div class="flex items-center justify-between">
  <span>Price</span>
  <button>Add to Cart</button>
</div>
```

### Responsive Breakpoints

#### Mobile First
- **sm:** 640px - Small tablets and up
- **md:** 768px - Tablets and up
- **lg:** 1024px - Laptops and up
- **xl:** 1280px - Desktops and up
- **2xl:** 1536px - Large screens

#### Usage Examples
```vue
<!-- Mobile: stacked, Tablet+: side by side -->
<div class="flex flex-col md:flex-row gap-4">
  <div class="md:w-1/2">Content 1</div>
  <div class="md:w-1/2">Content 2</div>
</div>

<!-- Mobile: 1 col, Tablet: 2 cols, Desktop: 3 cols -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  <!-- Items -->
</div>
```

### Accessibility Guidelines

#### Focus Management
- All interactive elements must have visible focus indicators
- Focus order must follow logical tab sequence
- Skip links for main navigation sections

#### Color Contrast
- Normal text: 4.5:1 minimum
- Large text: 3:1 minimum
- Interactive elements: 3:1 minimum

#### Semantic HTML
- Use appropriate heading hierarchy (h1-h6)
- Form elements properly labeled
- ARIA attributes for complex widgets

#### Keyboard Navigation
- TAB: Move focus between interactive elements
- ENTER/SPACE: Activate buttons and links
- ESC: Close modals and dropdowns
- Arrow keys: Navigate within components (radio groups, menus)

### Implementation Checklist

#### Phase 2A: Core Components
- [ ] Button component variants
- [ ] Input field component
- [ ] Form validation styling
- [ ] Card component variants

#### Phase 2B: Navigation Components
- [ ] Navigation menu component
- [ ] Breadcrumb component
- [ ] Pagination component
- [ ] Search component

#### Phase 2C: Interactive Components
- [ ] Modal component
- [ ] Dropdown component
- [ ] Tooltip component
- [ ] Loading states

#### Phase 2D: E-commerce Specific
- [ ] Product card component
- [ ] Shopping cart component
- [ ] Price display component
- [ ] Rating component

---

**Design System Status:** Specification complete  
**Implementation:** Ready for component development  
**Next:** Create reusable component library</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/issues/56-design-system.md