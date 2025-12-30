# daisyUI v5 Components Reference for B2Connect

**Last Updated**: 30. Dezember 2025  
**Source**: Official daisyUI Documentation  
**Purpose**: Frontend agent development support and component reference  
**Target Audience**: @frontend-developer, @ui-expert, @ux-expert, @frontend-store, @frontend-admin  
**Index**: ⬅️ [Back to AI Knowledge Base](./INDEX.md)

---

## 🎯 Quick Navigation

| Category | Components | Priority | Use Cases |
|----------|-----------|----------|-----------|
| [Forms](#forms) | Input, Textarea, Select, Checkbox, Radio, Toggle, File Input | 🔴 HIGH | User data entry, validation |
| [Navigation](#navigation) | Navbar, Breadcrumbs, Pagination, Tabs, Menu, Dock | 🔴 HIGH | Site structure, wayfinding |
| [Data Display](#data-display) | Table, Card, Badge, Avatar, Indicator, Status | 🔴 HIGH | Product listings, info display |
| [Feedback](#feedback) | Alert, Toast, Modal, Loading, Progress, Tooltip | 🔴 HIGH | User notifications, status |
| [Layout](#layout) | Stack, Join, Hero, Divider, Drawer, Footer | 🟡 MEDIUM | Page structure, organization |
| [Interactive](#interactive) | Dropdown, Accordion, Collapse, Swap, FAB/Speed Dial | 🟡 MEDIUM | User interactions, menus |
| [Specialty](#specialty) | Countdown, Text Rotate, Timeline, Carousel, Rating, Filter | 🟢 MEDIUM | Special effects, reviews |
| [Mockups](#mockups) | Browser, Code, Phone, Window | 🟢 LOW | Documentation, demos |

---

## 📦 Installation

### Quick Setup (Vite)

```bash
# 1. Install daisyUI
npm i -D daisyui@latest

# 2. Add to src/main.css
@import "tailwindcss";
@plugin "daisyui";
```

### For Vue 3 + Vite (B2Connect)

```javascript
// vite.config.js
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
})

// tailwind.config.js
export default {
  content: ['./index.html', './src/**/*.{vue,js}'],
  theme: { extend: {} },
  plugins: [require('daisyui')],
}
```

### Installation Issues

```bash
# If daisyUI classes not appearing:
npm install -D daisyui@latest tailwindcss postcss autoprefixer

# Verify installation:
# 1. Check node_modules/daisyui exists
# 2. Verify @plugin "daisyui"; in CSS
# 3. Rebuild: npm run dev
```

---

## 🎨 Themes

**Key Feature**: daisyUI provides **30+ built-in themes** with zero code changes.

### Using Themes

```html
<!-- Apply theme via HTML data attribute -->
<html data-theme="dark">
  <!-- All daisyUI components use "dark" theme -->
</html>

<!-- Or via CSS class -->
<html class="dark">
  <!-- daisyUI respects prefers-color-scheme -->
</html>
```

### Available Themes (Popular for B2Connect)

```javascript
const themes = [
  'light',      // Default light theme
  'dark',       // Default dark theme
  'cupcake',    // Pastel pink
  'bumblebee',  // Yellow/gold (good for e-commerce)
  'emerald',    // Green (nature, eco)
  'corporate',  // Blue (professional)
  'synthwave',  // Retro neon
  'retro',      // Retro warm
  'cyberpunk',  // High contrast
  'valentine',  // Pink/red
  'halloween',  // Orange/purple
  'garden',     // Green
  'forest',     // Dark green
  'aqua',       // Cyan
  'lofi',       // Monochrome
  'pastel',     // Pastel colors
  'fantasy',    // Purple/pink
  'wireframe',  // Minimal
  'black',      // Pure black
  'luxury',     // Gold/black
  'dracula',    // Dark theme
  'cmyk',       // Print colors
]
```

### B2Connect Theme Recommendation

**For Store Frontend** (5173):
```html
<!-- Light by default, dark on user preference -->
<html :data-theme="isDarkMode ? 'dark' : 'light'">
```

**For Admin Frontend** (5174):
```html
<!-- Professional corporate theme -->
<html data-theme="corporate">
```

---

## 🏗️ Core Components

### Forms

#### Button

```html
<!-- Basic buttons -->
<button class="btn">Default</button>
<button class="btn btn-primary">Primary</button>
<button class="btn btn-secondary">Secondary</button>
<button class="btn btn-accent">Accent</button>

<!-- Sizes -->
<button class="btn btn-sm">Small</button>
<button class="btn btn-lg">Large</button>

<!-- States -->
<button class="btn" disabled>Disabled</button>
<button class="btn btn-active">Active</button>
<button class="btn btn-disabled">Disabled style</button>

<!-- Variants -->
<button class="btn btn-outline">Outline</button>
<button class="btn btn-ghost">Ghost</button>
<button class="btn btn-link">Link</button>

<!-- With icon -->
<button class="btn gap-2">
  <svg>icon</svg>
  Click me
</button>

<!-- Loading state -->
<button class="btn" disabled>
  <span class="loading loading-spinner"></span>
  Loading
</button>
```

**B2Connect Usage** (Store):
```vue
<template>
  <!-- Add to Cart button -->
  <button 
    @click="addToCart"
    :disabled="isAdding || !inStock"
    class="btn btn-primary gap-2">
    <svg v-if="isAdding" class="loading loading-spinner"></svg>
    {{ isAdding ? 'Adding...' : 'Add to Cart' }}
  </button>

  <!-- Checkout button (accent) -->
  <button class="btn btn-accent w-full">
    Proceed to Checkout
  </button>

  <!-- Secondary actions -->
  <button class="btn btn-ghost">
    Continue Shopping
  </button>
</template>
```

#### Input

```html
<!-- Text input -->
<input type="text" placeholder="Type here" class="input input-bordered" />

<!-- With label -->
<label class="form-control">
  <div class="label">
    <span class="label-text">Email</span>
  </div>
  <input type="email" placeholder="user@example.com" class="input input-bordered" />
</label>

<!-- Input sizes -->
<input class="input input-sm input-bordered" placeholder="Small" />
<input class="input input-md input-bordered" placeholder="Medium" />
<input class="input input-lg input-bordered" placeholder="Large" />

<!-- Input states -->
<input class="input input-bordered" value="Default" />
<input class="input input-bordered" disabled value="Disabled" />
<input class="input input-bordered-error" placeholder="Error state" />
<input class="input input-bordered-success" value="Valid" />

<!-- With prefix/suffix -->
<div class="join">
  <span class="join-item bg-base-200 px-4">$</span>
  <input type="number" class="input input-bordered join-item" />
</div>
```

**B2Connect Usage** (Catalog search):
```vue
<template>
  <!-- Product search -->
  <div class="form-control">
    <label class="label">
      <span class="label-text">Search Products</span>
    </label>
    <div class="input-group">
      <input 
        v-model="searchQuery"
        type="text" 
        placeholder="Search SKU, name, or description..."
        class="input input-bordered w-full"
        @input="performSearch" />
      <button class="btn btn-square">
        <svg>search icon</svg>
      </button>
    </div>
  </div>

  <!-- Price range filter -->
  <label class="form-control">
    <div class="label">
      <span class="label-text">Price Range</span>
    </div>
    <div class="join">
      <input 
        v-model.number="minPrice"
        type="number" 
        placeholder="Min"
        class="input input-bordered join-item" />
      <span class="join-item bg-base-200">-</span>
      <input 
        v-model.number="maxPrice"
        type="number" 
        placeholder="Max"
        class="input input-bordered join-item" />
    </div>
  </label>
</template>
```

#### Checkbox & Radio

```html
<!-- Checkbox -->
<div class="form-control">
  <label class="label cursor-pointer">
    <span class="label-text">Remember me</span>
    <input type="checkbox" class="checkbox checkbox-primary" />
  </label>
</div>

<!-- Radio button group -->
<div class="form-control gap-2">
  <div class="label">Shipping Method</div>
  <label class="label cursor-pointer gap-2">
    <input type="radio" name="shipping" class="radio radio-primary" />
    <span class="label-text">Standard (5-7 days)</span>
  </label>
  <label class="label cursor-pointer gap-2">
    <input type="radio" name="shipping" class="radio radio-secondary" />
    <span class="label-text">Express (2-3 days)</span>
  </label>
</div>
```

#### Toggle (Switch)

```html
<!-- Simple toggle -->
<input type="checkbox" class="toggle" />

<!-- With labels -->
<label class="label cursor-pointer flex gap-2">
  <span class="label-text">Dark Mode</span>
  <input 
    type="checkbox" 
    class="toggle toggle-primary"
    @change="toggleDarkMode" />
</label>
```

#### Select & Dropdown

```html
<!-- Select dropdown -->
<select class="select select-bordered">
  <option disabled selected>Pick your favorite color</option>
  <option>Red</option>
  <option>Blue</option>
  <option>Green</option>
</select>

<!-- File input -->
<input type="file" class="file-input file-input-bordered" />
<input type="file" class="file-input file-input-primary" />

<!-- Textarea -->
<textarea 
  class="textarea textarea-bordered"
  placeholder="Your message..."></textarea>
```

### Navigation

#### Navbar

```html
<!-- Simple navbar -->
<div class="navbar bg-base-100 shadow-lg">
  <div class="flex-1">
    <a class="btn btn-ghost text-xl">B2Connect</a>
  </div>
  <div class="flex-none gap-2">
    <div class="dropdown dropdown-end">
      <button class="btn btn-circle btn-ghost">
        <svg>menu icon</svg>
      </button>
      <ul class="dropdown-content menu bg-base-100 rounded-box">
        <li><a href="/products">Products</a></li>
        <li><a href="/categories">Categories</a></li>
        <li><a href="/account">My Account</a></li>
      </ul>
    </div>
  </div>
</div>
```

**B2Connect Usage** (Store header):
```vue
<template>
  <div class="navbar bg-base-100 sticky top-0 z-50 shadow-md">
    <!-- Logo -->
    <div class="flex-1">
      <router-link to="/" class="btn btn-ghost text-xl font-bold">
        B2Connect Store
      </router-link>
    </div>

    <!-- Search (center on desktop) -->
    <div class="flex-none gap-2">
      <div class="form-control hidden sm:block">
        <input 
          v-model="searchQuery"
          type="text" 
          placeholder="Search products..."
          class="input input-bordered input-sm w-48"
          @keyup.enter="search" />
      </div>

      <!-- Cart icon with badge -->
      <div class="indicator">
        <span v-if="cartCount > 0" class="indicator-item badge badge-primary">
          {{ cartCount }}
        </span>
        <router-link to="/cart" class="btn btn-ghost btn-circle">
          <svg>cart icon</svg>
        </router-link>
      </div>

      <!-- Account dropdown -->
      <div class="dropdown dropdown-end">
        <button class="btn btn-circle btn-ghost">
          <svg>user icon</svg>
        </button>
        <ul class="dropdown-content menu bg-base-100 rounded-box shadow">
          <li v-if="!isLoggedIn">
            <router-link to="/login">Login</router-link>
          </li>
          <li v-else>
            <router-link to="/account">My Account</router-link>
          </li>
          <li><router-link to="/wishlist">Wishlist</router-link></li>
          <li><router-link to="/orders">Orders</router-link></li>
        </ul>
      </div>
    </div>
  </div>
</template>
```

#### Breadcrumbs

```html
<!-- Navigation breadcrumbs -->
<div class="breadcrumbs text-sm">
  <ul>
    <li><a href="/">Home</a></li>
    <li><a href="/categories">Categories</a></li>
    <li>Electronics</li>
  </ul>
</div>
```

#### Pagination

```html
<!-- Pagination controls -->
<div class="join">
  <button class="join-item btn">«</button>
  <button class="join-item btn">Page 22</button>
  <button class="join-item btn btn-active">23</button>
  <button class="join-item btn">24</button>
  <button class="join-item btn">»</button>
</div>
```

### Data Display

#### Card

```html
<!-- Basic card -->
<div class="card bg-base-100 shadow-xl">
  <figure>
    <img src="product.jpg" alt="product" />
  </figure>
  <div class="card-body">
    <h2 class="card-title">Product Name</h2>
    <p>Product description goes here</p>
    <div class="card-actions justify-end">
      <button class="btn btn-primary">Buy Now</button>
    </div>
  </div>
</div>

<!-- Compact card -->
<div class="card compact bg-base-100 shadow">
  <div class="card-body">
    <h3 class="card-title">Compact Card</h3>
    <p>Less padding, more compact</p>
  </div>
</div>
```

**B2Connect Usage** (Product card):
```vue
<template>
  <!-- Product listing card -->
  <div class="card bg-base-100 shadow-lg hover:shadow-xl transition">
    <!-- Image with hover effect -->
    <figure class="relative overflow-hidden h-48">
      <img 
        :src="product.image"
        :alt="product.name"
        class="w-full h-full object-cover hover:scale-110 transition" />
      <div v-if="product.discount" class="absolute top-2 right-2 badge badge-error">
        -{{ product.discount }}%
      </div>
    </figure>

    <div class="card-body">
      <!-- Product info -->
      <h3 class="card-title text-lg">{{ product.name }}</h3>
      <p class="text-sm text-base-content/70">SKU: {{ product.sku }}</p>

      <!-- Rating -->
      <div class="flex items-center gap-1">
        <div class="rating rating-sm gap-1">
          <input 
            v-for="i in 5" 
            :key="i"
            type="radio" 
            :name="`rating-${product.id}`"
            :class="i <= product.rating ? 'bg-warning' : 'bg-base-300'"
            class="mask mask-star-2 w-4 h-4"
            :checked="i === product.rating"
            disabled />
        </div>
        <span class="text-xs">({{ product.reviews }} reviews)</span>
      </div>

      <!-- Price -->
      <div class="py-2">
        <div class="flex justify-between items-center">
          <span class="text-2xl font-bold text-primary">
            €{{ product.price }}
          </span>
          <div v-if="product.originalPrice" class="text-sm line-through text-base-content/50">
            €{{ product.originalPrice }}
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="card-actions justify-between gap-2">
        <button 
          @click="addToWishlist"
          class="btn btn-ghost btn-sm">
          <svg v-if="isInWishlist" class="fill-current">heart</svg>
          <svg v-else>heart outline</svg>
        </button>
        <button 
          @click="addToCart"
          :disabled="!inStock"
          class="btn btn-primary btn-sm flex-1">
          {{ inStock ? 'Add to Cart' : 'Out of Stock' }}
        </button>
      </div>
    </div>
  </div>
</template>
```

#### Badge

```html
<!-- Badges for status -->
<div class="badge">Default</div>
<div class="badge badge-primary">Primary</div>
<div class="badge badge-success">In Stock</div>
<div class="badge badge-error">Out of Stock</div>
<div class="badge badge-warning">Limited</div>

<!-- With custom text -->
<div class="badge badge-lg badge-primary">New</div>
```

#### Table

```html
<!-- Data table -->
<div class="overflow-x-auto">
  <table class="table">
    <thead>
      <tr>
        <th>Product</th>
        <th>Price</th>
        <th>In Stock</th>
        <th>Action</th>
      </tr>
    </thead>
    <tbody>
      <tr class="hover">
        <td>Laptop</td>
        <td>€999</td>
        <td>
          <span class="badge badge-success">Yes</span>
        </td>
        <td>
          <button class="btn btn-sm">View</button>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

### Feedback

#### Alert

```html
<!-- Info alert -->
<div class="alert alert-info">
  <svg>info icon</svg>
  <span>New products added!</span>
</div>

<!-- Success alert -->
<div class="alert alert-success">
  <svg>success icon</svg>
  <span>Order placed successfully</span>
</div>

<!-- Warning alert -->
<div class="alert alert-warning">
  <svg>warning icon</svg>
  <span>Stock running low</span>
</div>

<!-- Error alert -->
<div class="alert alert-error">
  <svg>error icon</svg>
  <span>Something went wrong</span>
</div>
```

#### Toast

```html
<!-- Toast notifications (stacked) -->
<div class="toast toast-top toast-end">
  <div class="alert alert-info">
    <span>New message from admin</span>
  </div>
  <div class="alert alert-success">
    <span>Item added to cart</span>
  </div>
</div>
```

**B2Connect Usage** (Vue composable):
```javascript
// composables/useToast.js
import { ref } from 'vue'

export function useToast() {
  const toasts = ref([])

  const showToast = (message, type = 'info', duration = 3000) => {
    const id = Date.now()
    toasts.value.push({ id, message, type })
    
    setTimeout(() => {
      toasts.value = toasts.value.filter(t => t.id !== id)
    }, duration)
  }

  return { toasts, showToast }
}
```

```vue
<!-- ToastContainer.vue -->
<template>
  <div class="toast toast-top toast-end">
    <div 
      v-for="toast in toasts"
      :key="toast.id"
      :class="`alert alert-${toast.type}`">
      <span>{{ toast.message }}</span>
    </div>
  </div>
</template>

<script setup>
import { useToast } from '@/composables/useToast'

const { toasts } = useToast()
</script>
```

#### Modal

```html
<!-- Modal trigger -->
<button class="btn" onclick="my_modal.showModal()">Open Modal</button>

<!-- Modal dialog -->
<dialog id="my_modal" class="modal">
  <div class="modal-box">
    <h3 class="font-bold text-lg">Confirm Order</h3>
    <p class="py-4">Are you sure?</p>
    <div class="modal-action">
      <button class="btn" onclick="my_modal.close()">Cancel</button>
      <button class="btn btn-primary">Confirm</button>
    </div>
  </div>
</dialog>
```

#### Loading States

```html
<!-- Spinners -->
<span class="loading loading-spinner"></span>
<span class="loading loading-spinner loading-lg"></span>

<!-- Progress bar -->
<progress class="progress progress-primary w-56" value="70" max="100"></progress>

<!-- Radial progress -->
<div class="radial-progress" style="--value:70">70%</div>
```

---

## 🎨 Advanced Patterns

### Dropdown Menu

```html
<div class="dropdown">
  <button class="btn">Menu</button>
  <ul tabindex="0" class="dropdown-content menu bg-base-100 rounded-box shadow">
    <li><a>Item 1</a></li>
    <li><a>Item 2</a></li>
  </ul>
</div>
```

### Accordion/Collapse

```html
<!-- Accordion (only one open) -->
<div class="join join-vertical w-full">
  <div class="collapse collapse-arrow join-item">
    <input type="radio" name="accordion" />
    <div class="collapse-title">What is daisyUI?</div>
    <div class="collapse-content">
      <p>daisyUI is a component library for Tailwind CSS</p>
    </div>
  </div>
  <div class="collapse collapse-arrow join-item">
    <input type="radio" name="accordion" />
    <div class="collapse-title">How to install?</div>
    <div class="collapse-content">
      <p>npm install daisyui</p>
    </div>
  </div>
</div>
```

---

## 📊 Component Count & Coverage

| Category | Components | Status |
|----------|-----------|--------|
| Forms | 9 (Input, Select, Checkbox, Radio, Toggle, Textarea, etc) | ✅ Complete |
| Navigation | 6 (Navbar, Breadcrumbs, Pagination, Tabs, Menu, Dock) | ✅ Complete |
| Data Display | 15+ (Table, Card, Badge, Avatar, Indicator, Timeline, etc) | ✅ Complete |
| Feedback | 8 (Alert, Toast, Modal, Loading, Progress, Tooltip, etc) | ✅ Complete |
| Layout | 8 (Stack, Join, Hero, Divider, Drawer, Footer, etc) | ✅ Complete |
| Interactive | 12+ (Dropdown, Accordion, Collapse, Swap, FAB, etc) | ✅ Complete |
| Specialty | 8 (Countdown, Text Rotate, Timeline, Carousel, Rating, etc) | ✅ Complete |
| Mockups | 4 (Browser, Code, Phone, Window) | ✅ Complete |
| **TOTAL** | **65+ Components** | **✅ COMPREHENSIVE** |

---

## 🚀 B2Connect Implementation Strategy

### Phase 1 (Sprint 1-2) - Core Components
- ✅ Button (all variants)
- ✅ Input & Form controls
- ✅ Card for product listings
- ✅ Navigation (Navbar, Breadcrumbs)
- ✅ Alerts & Toasts for feedback

### Phase 2 (Sprint 3-4) - Data Display
- ✅ Tables for admin
- ✅ Pagination
- ✅ Badge status indicators
- ✅ Avatar for user profiles
- ✅ Timeline for order tracking

### Phase 3 (Sprint 5+) - Advanced
- ✅ Modal dialogs
- ✅ Dropdown menus
- ✅ Accordion/Collapse
- ✅ Carousel for featured products
- ✅ Rating component

---

## 🎯 Customization (Tailwind Utilities)

**Key Strength**: Customize any daisyUI component with Tailwind utilities

```html
<!-- Default button -->
<button class="btn">Standard</button>

<!-- Customized with Tailwind -->
<button class="btn rounded-full gap-2 text-lg shadow-xl hover:shadow-2xl">
  Customized
</button>

<!-- Dark mode variant -->
<button class="btn dark:btn-ghost dark:text-white">
  Toggle for dark mode
</button>

<!-- Responsive sizing -->
<button class="btn btn-sm md:btn-md lg:btn-lg">
  Responsive button
</button>
```

---

## 📚 Official References

- **Main Site**: https://daisyui.com/
- **Components**: https://daisyui.com/components/
- **Installation Guide**: https://daisyui.com/docs/install/
- **Theme Generator**: https://daisyui.com/theme-generator/
- **Customization**: https://daisyui.com/docs/customize/
- **GitHub**: https://github.com/saadeghi/daisyui
- **NPM**: https://www.npmjs.com/package/daisyui

---

## ✅ Adoption Checklist

Before using daisyUI in features:

- [ ] Install latest version: `npm i -D daisyui@latest`
- [ ] Verify in `tailwind.config.js`: `plugins: [require('daisyui')]`
- [ ] Verify in CSS: `@plugin "daisyui";`
- [ ] Run `npm run dev` to rebuild
- [ ] Test components in browser DevTools
- [ ] Verify dark mode switching works
- [ ] Check mobile responsiveness
- [ ] Accessibility: Test keyboard navigation, color contrast
- [ ] Performance: Check file size impact

---

## 🔗 Integration with B2Connect Components

### Vue 3 Component Library Example

```vue
<!-- Button.vue -->
<template>
  <button :class="buttonClass">
    <slot />
  </button>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  variant: { type: String, default: 'primary' },
  size: { type: String, default: 'md' },
  disabled: Boolean,
})

const buttonClass = computed(() => [
  'btn',
  `btn-${props.variant}`,
  `btn-${props.size}`,
  { 'btn-disabled': props.disabled }
])
</script>
```

---

**Last Updated**: 30. Dezember 2025  
**Created by**: AI Agent (Technical Research)  
**For**: All B2Connect Frontend Agents  
**Version**: v5.0+ compatible  
**Questions?** Reference official docs or ask @ui-expert / @frontend-developer
