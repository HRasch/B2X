# DaisyUI Component Inventory for B2Connect Store

**Date**: 29. Dezember 2025  
**Status**: ✅ Configured and Ready for Implementation  
**Framework**: Vue 3 + Tailwind CSS + DaisyUI

---

## 🎯 Component Mapping: DaisyUI → B2Connect Store

### **Navigation & Layout**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `navbar` | Store header with logo, search, cart | `components/StoreHeader.vue` | P0 | 📋 Planned |
| `drawer` | Mobile sidebar menu | `components/MobileMenu.vue` | P0 | 📋 Planned |
| `breadcrumb` | Product navigation breadcrumb | `components/Breadcrumb.vue` | P1 | 📋 Planned |
| `footer` | Store footer with links | `components/StoreFooter.vue` | P1 | 📋 Planned |
| `tabs` | Product detail tabs (specs, reviews) | `components/ProductTabs.vue` | P1 | 📋 Planned |

---

### **Product Listing & Cards**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `card` | Product card container | `components/ProductCard.vue` | P0 | 📋 Planned |
| `image` | Product image with overlay | `components/ProductImage.vue` | P0 | 📋 Planned |
| `badge` | Price label, rating badge | `components/Badge.vue` | P0 | 📋 Planned |
| `rating` | Star rating display | `components/Rating.vue` | P1 | 📋 Planned |
| `grid` | Product grid layout (responsive) | CSS Utility | P0 | ✅ Built-in |

---

### **Buttons & Actions**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `btn` | All buttons (add to cart, checkout, etc.) | Global Utility | P0 | ✅ Built-in |
| `btn-group` | Related buttons (size selector) | Global Utility | P1 | 📋 Planned |
| `dropdown` | Sort/filter dropdown | `components/Dropdown.vue` | P1 | 📋 Planned |
| `loading` | Spinner during cart/checkout | `components/LoadingSpinner.vue` | P0 | 📋 Planned |

---

### **Forms & Input**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `input` | Text inputs (search, address, etc.) | Global Utility | P0 | ✅ Built-in |
| `select` | Dropdown selects (country, category) | Global Utility | P1 | ✅ Built-in |
| `checkbox` | Agreement checkbox, filters | Global Utility | P1 | 📋 Planned |
| `radio` | Shipping method selection | Global Utility | P0 | 📋 Planned |
| `form-control` | Form field wrapper | Global Utility | P0 | ✅ Built-in |
| `label` | Form labels (accessible) | Global Utility | P0 | ✅ Built-in |
| `input-group` | Search input with icon | `components/SearchBar.vue` | P0 | 📋 Planned |

---

### **Modals & Overlays**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `modal` | Dialogs (confirm, quick view) | `components/Modal.vue` | P1 | 📋 Planned |
| `alert` | Error/success messages | `components/Alert.vue` | P0 | 📋 Planned |
| `toast` | Toast notifications | `composables/useToast.ts` | P1 | 📋 Planned |

---

### **Tables & Lists**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `table` | Cart items, order history | `components/CartTable.vue` | P0 | 📋 Planned |
| `divider` | Visual separator | CSS Utility | P1 | ✅ Built-in |

---

### **Pricing & Cart**

| DaisyUI Component | B2Connect Usage | Vue File | Priority | Status |
|---|---|---|---|---|
| `stat` | Price breakdown (Net + VAT) | `components/PriceBreakdown.vue` | P0 | 📋 Planned |
| `steps` | Checkout progress (step 1-3) | `components/CheckoutSteps.vue` | P0 | 📋 Planned |
| `progress` | Order progress indicator | `components/Progress.vue` | P1 | 📋 Planned |

---

## 📋 Custom Components to Build (Vue 3 Wrappers)

### **Week 1 Foundation Components**
These wrap DaisyUI components for reusability:

```typescript
// components/Button.vue
// Base button with variants (primary, secondary, danger)

// components/Card.vue
// Styled card for products

// components/Input.vue
// Form input with label & error support

// components/Select.vue
// Dropdown select with icon

// components/Modal.vue
// Accessible modal dialog

// components/Alert.vue
// Success/error/warning messages

// components/Badge.vue
// Price/rating badges
```

---

## 🎨 Color Tokens (Tailwind Theme)

### **Light Theme**
- **Primary**: `#0b98ff` (B2Connect Blue)
- **Secondary**: `#8b5cf6` (Purple)
- **Success**: `#22c55e` (Green)
- **Warning**: `#f59e0b` (Amber)
- **Danger**: `#ef4444` (Red)
- **Base**: White `#ffffff`, Light Gray `#f2f2f2`

### **Dark Theme**
- **Primary**: `#36b0ff` (Light Blue)
- **Secondary**: `#a78bfa` (Light Purple)
- **Success**: `#4ade80` (Light Green)
- **Warning**: `#fbbf24` (Light Amber)
- **Danger**: `#f87171` (Light Red)
- **Base**: Dark Gray `#1f2937`, Dark `#111827`

---

## 📐 Responsive Breakpoints (Tailwind)

| Breakpoint | Size | Usage |
|---|---|---|
| `sm` | 640px | Small phones |
| `md` | 768px | Tablets |
| `lg` | 1024px | Desktops |
| `xl` | 1280px | Large desktops |
| `2xl` | 1536px | Ultra-wide |

**Product Grid Columns**:
- Mobile (sm): 1 column
- Tablet (md): 2 columns
- Desktop (lg): 3 columns
- Large (xl): 4 columns

---

## 🚀 Implementation Priority

### **Sprint 1 (Week 1)** - Foundation ✅
- ✅ DaisyUI installed
- ✅ Tailwind configured
- ✅ Color theme defined
- ✅ Component inventory mapped
- 📋 Create 5 base wrapper components (Button, Card, Input, Modal, Alert)

### **Sprint 2 (Week 2)** - Product Pages
- Product listing (use Card, Button, Badge)
- Product detail (use Input, Image, Tabs)
- Shopping cart (use Table, Button, Card)

### **Sprint 3 (Week 3)** - Checkout
- Checkout flow (use Form, Steps, Modal)
- Testing & optimization

---

## 🔗 Component Usage Examples

### **ProductCard.vue** (Using DaisyUI)
```vue
<template>
  <div class="card bg-base-100 shadow-md hover:shadow-lg transition-shadow">
    <figure>
      <img :src="product.image" :alt="product.name" class="w-full h-48 object-cover" />
    </figure>
    <div class="card-body">
      <h2 class="card-title text-lg">{{ product.name }}</h2>
      <p class="text-sm text-base-content/70">{{ product.description }}</p>
      <div class="card-actions justify-between items-center">
        <div class="text-right">
          <div class="text-2xl font-bold text-primary">{{ formatPrice(product.price) }}</div>
          <p class="text-vat">incl. VAT</p>
        </div>
        <button class="btn btn-primary btn-sm" @click="addToCart">
          Add to Cart
        </button>
      </div>
    </div>
  </div>
</template>
```

### **CheckoutForm.vue** (Using DaisyUI Form)
```vue
<template>
  <form class="form-control w-full max-w-lg">
    <label class="label">
      <span class="label-text">Street Address</span>
    </label>
    <input type="text" placeholder="123 Main St" class="input input-bordered" />
    
    <label class="label">
      <span class="label-text">City</span>
    </label>
    <input type="text" placeholder="New York" class="input input-bordered" />
    
    <button type="submit" class="btn btn-primary mt-4">Continue</button>
  </form>
</template>
```

---

## ✅ Week 1 Deliverables Checklist

- [x] DaisyUI installed (`npm install daisyui`)
- [x] Tailwind configured (`tailwind.config.ts`)
- [x] PostCSS configured (`postcss.config.js`)
- [x] Main CSS updated with Tailwind directives
- [x] Color theme defined (light + dark)
- [x] Component inventory documented (this file)
- [ ] Base wrapper components created (5 components)
- [ ] Component usage guide written
- [ ] Dark mode tested
- [ ] Build verified (zero errors)

---

## 🔍 Testing & Quality Checklist

- [ ] Tailwind build working (`npm run build`)
- [ ] DaisyUI components rendering
- [ ] Dark mode theme switching works
- [ ] Colors meet contrast requirements (WCAG 2.1 AA)
- [ ] Responsive breakpoints working (sm, md, lg, xl)
- [ ] No console errors
- [ ] Bundle size within limits (<100KB increase)

---

## 📚 Documentation & Resources

**Official Links**:
- [DaisyUI Components](https://daisyui.com/components/)
- [Tailwind CSS Docs](https://tailwindcss.com/docs)
- [DaisyUI Themes](https://daisyui.com/docs/themes/)
- [Vue 3 + Tailwind Guide](https://tailwindcss.com/docs/guides/vuepress)

**Component Usage**:
- All components use Tailwind utility classes
- No custom CSS unless absolutely necessary
- Custom colors use Tailwind's `@apply` directive
- Responsive design uses Tailwind breakpoints (sm:, md:, lg:)

---

**Status**: ✅ READY FOR IMPLEMENTATION  
**Next**: Build base wrapper components (Sprint 1, Task 1.3)
