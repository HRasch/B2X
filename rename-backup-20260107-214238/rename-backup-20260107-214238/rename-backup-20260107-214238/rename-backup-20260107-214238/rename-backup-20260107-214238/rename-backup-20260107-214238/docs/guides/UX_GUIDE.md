# 🎨 B2Connect UX Guide

**Version**: 1.0.0  
**Last Updated**: 1. Januar 2026  
**Status**: ✅ Active  
**Agents**: @UX, @UI, @Frontend, @TechLead | Owner: @UX

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Design Philosophy](#design-philosophy)
3. [Application Structure](#application-structure)
4. [Design Systems](#design-systems)
5. [Component Library](#component-library)
6. [Typography & Colors](#typography--colors)
7. [Spacing & Layout](#spacing--layout)
8. [Accessibility (WCAG 2.1)](#accessibility-wcag-21)
9. [Responsive Design](#responsive-design)
10. [User Flows](#user-flows)
11. [Forms & Validation](#forms--validation)
12. [Feedback & States](#feedback--states)
13. [Performance Guidelines](#performance-guidelines)
14. [Dark Mode](#dark-mode)
15. [Testing UX](#testing-ux)

---

## Overview

B2Connect consists of **three frontend applications**, each serving different user personas:

| Application | Purpose | Target Users | Tech Stack |
|-------------|---------|--------------|------------|
| **Store** | B2B E-Commerce Shop | Business Customers | Vue 3 + DaisyUI v5 |
| **Admin** | System Administration | Internal Staff, IT | Vue 3 + Soft UI Design |
| **Management** | Tenant/Shop Management | Shop Owners, Managers | Vue 3 + Tailwind CSS |

### Shared Principles

- ✅ **Consistency** — Same patterns across all apps
- ✅ **Accessibility** — WCAG 2.1 Level AA compliance
- ✅ **Performance** — Fast, responsive interfaces
- ✅ **Mobile-First** — Works on all devices
- ✅ **Dark Mode** — System preference support

---

## Design Philosophy

### 1. User-Centered Design

```
┌─────────────────────────────────────────────────────────────┐
│                    DESIGN PRINCIPLES                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  👤 USER FIRST                                              │
│     • Understand user goals before designing                │
│     • Reduce cognitive load                                 │
│     • Progressive disclosure of complexity                  │
│                                                             │
│  🎯 TASK FOCUSED                                            │
│     • Clear call-to-actions                                 │
│     • Minimize steps to completion                          │
│     • Provide clear feedback                                │
│                                                             │
│  ♿ INCLUSIVE                                                │
│     • Accessible to all users                               │
│     • Multiple input methods (mouse, keyboard, touch)       │
│     • Support assistive technologies                        │
│                                                             │
│  ⚡ PERFORMANT                                              │
│     • Fast load times (< 3s initial, < 1s subsequent)       │
│     • Smooth animations (60fps)                             │
│     • Optimistic UI updates                                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 2. Information Architecture

```
Store (Customer Journey)
├── Home / Landing
├── Catalog
│   ├── Categories
│   ├── Product List
│   └── Product Detail
├── Cart
├── Checkout
│   ├── Shipping
│   ├── Payment
│   └── Confirmation
└── Account
    ├── Orders
    ├── Profile
    └── Settings

Admin (System Management)
├── Dashboard
├── CMS
│   ├── Pages
│   ├── Blocks
│   └── Templates
├── Shop
│   ├── Products
│   ├── Orders
│   └── Customers
├── Jobs
└── Settings
    └── Users

Management (Tenant Operations)
├── Dashboard
├── Configuration
├── Analytics
└── Settings
```

---

## Application Structure

### Store Frontend

**Location**: `frontend/Store/`

```
src/
├── components/
│   ├── common/           # Shared components
│   ├── shop/             # E-commerce components
│   ├── cms/              # Content blocks
│   └── widgets/          # Reusable widgets
├── views/                # Page components
├── stores/               # Pinia state stores
├── composables/          # Vue composition functions
└── assets/               # Static assets
```

**Design System**: DaisyUI v5 (Tailwind-based component library)

### Admin Frontend

**Location**: `frontend/Admin/`

```
src/
├── components/
│   ├── common/           # Layout, navigation
│   ├── soft-ui/          # Soft UI components
│   └── PimSyncDashboard.vue
├── views/                # Page views
└── stores/               # State management
```

**Design System**: Soft UI Dashboard (custom Tailwind configuration)

### Management Frontend

**Location**: `frontend/Management/`

```
src/
├── components/
├── views/
└── stores/
```

**Design System**: Tailwind CSS (base configuration)

---

## Design Systems

### Store: DaisyUI v5

DaisyUI provides semantic component classes built on Tailwind CSS.

```vue
<!-- Button variants -->
<button class="btn btn-primary">Primary Action</button>
<button class="btn btn-secondary">Secondary Action</button>
<button class="btn btn-accent">Accent</button>
<button class="btn btn-ghost">Ghost</button>

<!-- Card -->
<div class="card bg-base-100 shadow-xl">
  <div class="card-body">
    <h2 class="card-title">Card Title</h2>
    <p>Card content</p>
    <div class="card-actions justify-end">
      <button class="btn btn-primary">Action</button>
    </div>
  </div>
</div>

<!-- Badge -->
<span class="badge badge-success">Active</span>
<span class="badge badge-warning">Pending</span>
<span class="badge badge-error">Error</span>
```

### Admin: Soft UI Design System

Custom Vue components with soft shadows and gradient effects.

```vue
<!-- Soft Card -->
<SoftCard variant="gradient">
  <h3>Dashboard Widget</h3>
  <p>Content here</p>
</SoftCard>

<!-- Soft Button -->
<SoftButton variant="primary" size="lg">
  Save Changes
</SoftButton>
<SoftButton variant="secondary">Cancel</SoftButton>
<SoftButton variant="danger">Delete</SoftButton>
<SoftButton variant="ghost">More Info</SoftButton>

<!-- Soft Badge -->
<SoftBadge variant="success">Active</SoftBadge>
<SoftBadge variant="warning">Pending</SoftBadge>
<SoftBadge variant="danger">Error</SoftBadge>

<!-- Soft Panel -->
<SoftPanel title="User Management" description="Manage system users">
  <template #content>
    <!-- Panel content -->
  </template>
  <template #footer>
    <SoftButton variant="primary">Save</SoftButton>
  </template>
</SoftPanel>

<!-- Soft Input -->
<SoftInput
  v-model="email"
  type="email"
  label="Email Address"
  placeholder="user@example.com"
  :error="emailError"
/>
```

### Management: Tailwind CSS Base

Standard Tailwind utility classes with minimal customization.

```vue
<!-- Basic card -->
<div class="bg-white rounded-lg shadow p-6">
  <h3 class="text-lg font-semibold text-gray-900">Title</h3>
  <p class="text-gray-600">Content</p>
</div>

<!-- Button -->
<button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
  Action
</button>
```

---

## Component Library

### Shared Components (Cross-Application)

#### Loading States

```vue
<!-- Loading Spinner -->
<template>
  <div class="flex items-center justify-center p-8">
    <svg class="animate-spin h-8 w-8 text-primary-500" viewBox="0 0 24 24">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none"/>
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/>
    </svg>
    <span class="ml-2 text-gray-600">Loading...</span>
  </div>
</template>
```

#### Empty States

```vue
<!-- Empty State Pattern -->
<template>
  <div class="text-center py-12">
    <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <!-- Icon -->
    </svg>
    <h3 class="mt-2 text-sm font-semibold text-gray-900">No items found</h3>
    <p class="mt-1 text-sm text-gray-500">Get started by creating a new item.</p>
    <div class="mt-6">
      <button class="btn btn-primary">
        <PlusIcon class="h-5 w-5 mr-2" />
        New Item
      </button>
    </div>
  </div>
</template>
```

#### Error States

```vue
<!-- Error State Pattern -->
<template>
  <div class="rounded-lg bg-danger-50 p-4" role="alert">
    <div class="flex">
      <ExclamationCircleIcon class="h-5 w-5 text-danger-400" />
      <div class="ml-3">
        <h3 class="text-sm font-medium text-danger-800">{{ title }}</h3>
        <p class="mt-2 text-sm text-danger-700">{{ message }}</p>
        <div class="mt-4">
          <button @click="retry" class="btn btn-sm btn-danger">
            Try Again
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
```

### Store-Specific Components

| Component | Purpose | Usage |
|-----------|---------|-------|
| `ProductCard` | Display product in grid | Catalog, search results |
| `CartDrawer` | Slide-out cart | Header icon click |
| `CheckoutStepper` | Multi-step checkout | Checkout flow |
| `PriceDisplay` | Formatted price with VAT | Product, cart, checkout |
| `ShippingSelector` | Shipping method selection | Checkout |

### Admin-Specific Components

| Component | Purpose | Usage |
|-----------|---------|-------|
| `SoftCard` | Content container | Dashboard widgets, forms |
| `SoftButton` | Action buttons | Forms, toolbars |
| `SoftBadge` | Status indicators | Tables, lists |
| `SoftPanel` | Sectioned container | Complex forms |
| `SoftInput` | Form inputs | All forms |
| `MainLayout` | App shell | Page wrapper |

---

## Typography & Colors

### Typography Scale

```css
/* Font Sizes */
--text-xs:   0.75rem;   /* 12px - Labels, captions */
--text-sm:   0.875rem;  /* 14px - Secondary text */
--text-base: 1rem;      /* 16px - Body text */
--text-lg:   1.125rem;  /* 18px - Subheadings */
--text-xl:   1.25rem;   /* 20px - Card titles */
--text-2xl:  1.5rem;    /* 24px - Section headings */
--text-3xl:  1.875rem;  /* 30px - Page titles */
--text-4xl:  2.25rem;   /* 36px - Hero headings */

/* Font Weights */
--font-normal:   400;   /* Body text */
--font-medium:   500;   /* Buttons, labels */
--font-semibold: 600;   /* Subheadings */
--font-bold:     700;   /* Headings, emphasis */
```

### Color Palette

#### Brand Colors (Shared)

```css
/* Primary - Corporate Blue */
--primary-50:  #f0f9ff;
--primary-100: #e0f2fe;
--primary-200: #bae6fd;
--primary-300: #7dd3fc;
--primary-400: #38bdf8;
--primary-500: #0ea5e9;  /* Main */
--primary-600: #0284c7;
--primary-700: #0369a1;
--primary-800: #075985;
--primary-900: #0c4a6e;

/* Success - Green */
--success-500: #22c55e;

/* Warning - Amber */
--warning-500: #f59e0b;

/* Danger - Red */
--danger-500: #ef4444;

/* Info - Indigo */
--info-500: #6366f1;
```

#### Neutral Colors

```css
/* Soft UI Neutrals (Admin) */
--soft-50:  #f8f9fa;  /* Background */
--soft-100: #f0f2f5;
--soft-200: #e9ecef;
--soft-300: #dee2e6;
--soft-400: #ced4da;
--soft-500: #adb5bd;  /* Muted text */
--soft-600: #6c757d;
--soft-700: #495057;  /* Body text */
--soft-800: #343a40;
--soft-900: #212529;  /* Headings */
```

### Color Usage Guidelines

| Use Case | Light Mode | Dark Mode |
|----------|------------|-----------|
| Background | `soft-50` / `white` | `soft-900` / `#1a1a1a` |
| Text (primary) | `soft-700` / `#495057` | `#e4e4e7` |
| Text (secondary) | `soft-500` / `#adb5bd` | `#a1a1aa` |
| Borders | `soft-200` / `#e9ecef` | `soft-700` / `#404040` |
| Links | `primary-600` | `primary-400` |
| Success | `success-600` | `success-400` |
| Warning | `warning-600` | `warning-400` |
| Error | `danger-600` | `danger-400` |

---

## Spacing & Layout

### Spacing Scale

```css
/* Spacing (4px base unit) */
--space-0:    0;
--space-1:    0.25rem;  /* 4px */
--space-2:    0.5rem;   /* 8px */
--space-3:    0.75rem;  /* 12px */
--space-4:    1rem;     /* 16px - Default gap */
--space-5:    1.25rem;  /* 20px */
--space-6:    1.5rem;   /* 24px - Section gap */
--space-8:    2rem;     /* 32px */
--space-10:   2.5rem;   /* 40px */
--space-12:   3rem;     /* 48px */
--space-16:   4rem;     /* 64px - Page padding */

/* Safe Area (Soft UI specific) */
--space-safe:    1.25rem;  /* 20px - Component padding */
--space-safe-lg: 1.5rem;   /* 24px - Large component padding */
```

### Layout Patterns

#### Page Layout

```vue
<template>
  <!-- Standard page layout -->
  <div class="min-h-screen bg-soft-50 dark:bg-soft-900">
    <!-- Header -->
    <header class="sticky top-0 z-40 bg-white dark:bg-soft-800 shadow-sm">
      <!-- Navigation -->
    </header>
    
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Page content -->
    </main>
    
    <!-- Footer -->
    <footer class="bg-white dark:bg-soft-800 border-t">
      <!-- Footer content -->
    </footer>
  </div>
</template>
```

#### Grid Systems

```vue
<!-- Responsive grid -->
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
  <!-- Grid items -->
</div>

<!-- Dashboard stats (4-column) -->
<div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6">
  <SoftCard v-for="stat in stats" :key="stat.id">
    <!-- Stat content -->
  </SoftCard>
</div>

<!-- Two-column form layout -->
<div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
  <SoftInput v-model="firstName" label="First Name" />
  <SoftInput v-model="lastName" label="Last Name" />
</div>
```

---

## Accessibility (WCAG 2.1)

### Level AA Compliance Requirements

#### 1. Keyboard Navigation

```vue
<!-- ✅ All interactive elements must be keyboard accessible -->
<button @click="handleAction" @keydown.enter="handleAction">
  Action
</button>

<!-- ✅ Custom components need tabindex -->
<div 
  role="button" 
  tabindex="0" 
  @click="handleClick"
  @keydown.enter="handleClick"
  @keydown.space.prevent="handleClick"
>
  Custom Button
</div>

<!-- ✅ Skip links for main content -->
<a href="#main-content" class="sr-only focus:not-sr-only">
  Skip to main content
</a>
```

#### 2. Focus Indicators

```css
/* Visible focus states (minimum 2px solid) */
:focus-visible {
  outline: 2px solid var(--primary-500);
  outline-offset: 2px;
}

/* Custom focus ring */
.focus-ring {
  @apply focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2;
}
```

#### 3. Color Contrast

| Element | Minimum Ratio | Recommended |
|---------|---------------|-------------|
| Normal text | 4.5:1 | 7:1 (AAA) |
| Large text (18px+) | 3:1 | 4.5:1 |
| UI components | 3:1 | 4.5:1 |
| Non-text content | 3:1 | 4.5:1 |

```css
/* ✅ Good contrast combinations */
.text-on-white { color: #495057; }     /* 8.89:1 */
.link-on-white { color: #0284c7; }     /* 5.02:1 */
.error-on-light { color: #dc2626; }    /* 5.98:1 */
```

#### 4. Form Labels

```vue
<!-- ✅ Always associate labels with inputs -->
<label for="email" class="block text-sm font-medium text-gray-700">
  Email Address
</label>
<input 
  id="email" 
  type="email" 
  v-model="email"
  aria-describedby="email-error"
  :aria-invalid="!!emailError"
/>
<p v-if="emailError" id="email-error" class="text-sm text-danger-600" role="alert">
  {{ emailError }}
</p>
```

#### 5. ARIA Attributes

```vue
<!-- ✅ Loading states -->
<div aria-busy="true" aria-live="polite">
  <LoadingSpinner />
</div>

<!-- ✅ Alerts and notifications -->
<div role="alert" aria-live="assertive">
  {{ errorMessage }}
</div>

<!-- ✅ Navigation landmarks -->
<nav aria-label="Main navigation">
  <!-- Nav items -->
</nav>

<main id="main-content" aria-label="Page content">
  <!-- Main content -->
</main>
```

#### 6. Screen Reader Support

```vue
<!-- ✅ Visually hidden but accessible -->
<span class="sr-only">Open menu</span>

<!-- ✅ Decorative images -->
<img src="decorative.svg" alt="" aria-hidden="true" />

<!-- ✅ Meaningful images -->
<img src="product.jpg" :alt="`${product.name} - ${product.description}`" />
```

### Testing Accessibility

```bash
# Run axe-core tests
npm run test:a11y

# Use browser extensions
# - axe DevTools
# - WAVE
# - Lighthouse

# Manual testing checklist
# - [ ] Tab through entire page
# - [ ] Use screen reader (VoiceOver/NVDA)
# - [ ] Test with 200% zoom
# - [ ] Test with high contrast mode
```

---

## Responsive Design

### Breakpoints

```css
/* Tailwind breakpoints */
--screen-sm:  640px;   /* Mobile landscape */
--screen-md:  768px;   /* Tablet portrait */
--screen-lg:  1024px;  /* Tablet landscape / Small desktop */
--screen-xl:  1280px;  /* Desktop */
--screen-2xl: 1536px;  /* Large desktop */
```

### Mobile-First Approach

```vue
<template>
  <!-- Mobile-first: base styles for mobile, then add breakpoints -->
  <div class="
    p-4          /* Mobile: 16px padding */
    md:p-6       /* Tablet: 24px padding */
    lg:p-8       /* Desktop: 32px padding */
  ">
    <h1 class="
      text-2xl     /* Mobile: 24px */
      md:text-3xl  /* Tablet: 30px */
      lg:text-4xl  /* Desktop: 36px */
    ">
      Page Title
    </h1>
    
    <div class="
      grid 
      grid-cols-1      /* Mobile: 1 column */
      md:grid-cols-2   /* Tablet: 2 columns */
      lg:grid-cols-4   /* Desktop: 4 columns */
      gap-4 md:gap-6
    ">
      <!-- Grid items -->
    </div>
  </div>
</template>
```

### Responsive Patterns

#### Navigation

```vue
<template>
  <!-- Mobile: hamburger menu -->
  <nav class="lg:hidden">
    <button @click="menuOpen = !menuOpen" aria-label="Toggle menu">
      <MenuIcon v-if="!menuOpen" />
      <XIcon v-else />
    </button>
    <div v-show="menuOpen" class="absolute top-full left-0 right-0 bg-white">
      <!-- Mobile menu items -->
    </div>
  </nav>
  
  <!-- Desktop: horizontal nav -->
  <nav class="hidden lg:flex items-center space-x-8">
    <!-- Desktop menu items -->
  </nav>
</template>
```

#### Tables

```vue
<template>
  <!-- Desktop: full table -->
  <table class="hidden md:table w-full">
    <thead>
      <tr>
        <th>Name</th>
        <th>Email</th>
        <th>Status</th>
        <th>Actions</th>
      </tr>
    </thead>
    <tbody>
      <!-- Table rows -->
    </tbody>
  </table>
  
  <!-- Mobile: card list -->
  <div class="md:hidden space-y-4">
    <div v-for="item in items" :key="item.id" class="card p-4">
      <div class="flex justify-between">
        <span class="font-medium">{{ item.name }}</span>
        <SoftBadge :variant="item.status">{{ item.status }}</SoftBadge>
      </div>
      <div class="text-sm text-gray-500 mt-1">{{ item.email }}</div>
    </div>
  </div>
</template>
```

---

## User Flows

### Store: Checkout Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     CHECKOUT FLOW                            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. CART REVIEW                                             │
│     ├─ Display cart items                                   │
│     ├─ Allow quantity changes                               │
│     ├─ Show subtotal                                        │
│     └─ "Proceed to Checkout" button                         │
│              ↓                                              │
│  2. SHIPPING                                                │
│     ├─ Select delivery country                              │
│     ├─ Display available shipping methods                   │
│     ├─ Calculate shipping costs                             │
│     └─ "Continue to Payment" button                         │
│              ↓                                              │
│  3. PAYMENT                                                 │
│     ├─ Select payment method                                │
│     ├─ Enter payment details                                │
│     ├─ Apply discount codes                                 │
│     └─ "Place Order" button                                 │
│              ↓                                              │
│  4. CONFIRMATION                                            │
│     ├─ Show order number                                    │
│     ├─ Display order summary                                │
│     ├─ Send confirmation email                              │
│     └─ "Continue Shopping" / "View Order" buttons           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Admin: User Management Flow

```
┌─────────────────────────────────────────────────────────────┐
│                 USER MANAGEMENT FLOW                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. USER LIST VIEW                                          │
│     ├─ Search/filter users                                  │
│     ├─ Sort by columns                                      │
│     ├─ Pagination                                           │
│     └─ "Add User" button                                    │
│              ↓                                              │
│  2. USER DETAIL/EDIT                                        │
│     ├─ View user information                                │
│     ├─ Edit profile fields                                  │
│     ├─ Manage roles/permissions                             │
│     ├─ View activity log                                    │
│     └─ "Save" / "Delete" actions                            │
│              ↓                                              │
│  3. CONFIRMATION DIALOGS                                    │
│     ├─ Confirm destructive actions                          │
│     ├─ Show success/error feedback                          │
│     └─ Return to list view                                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Forms & Validation

### Form Design Principles

1. **Label all inputs** — Always visible, above the input
2. **Provide hints** — Help text below inputs when needed
3. **Validate in real-time** — Show errors as user types (debounced)
4. **Group related fields** — Use fieldsets and logical sections
5. **Mark required fields** — Use asterisk (*) or "Required" label
6. **Show progress** — Multi-step forms need progress indicators

### Validation Patterns

```vue
<template>
  <form @submit.prevent="handleSubmit" novalidate>
    <!-- Text input with validation -->
    <div class="mb-4">
      <label for="email" class="block text-sm font-medium mb-1">
        Email Address <span class="text-danger-500">*</span>
      </label>
      <input
        id="email"
        v-model="email"
        type="email"
        class="w-full rounded-lg border"
        :class="emailError ? 'border-danger-500' : 'border-gray-300'"
        :aria-invalid="!!emailError"
        aria-describedby="email-error email-hint"
        @blur="validateEmail"
      />
      <p id="email-hint" class="mt-1 text-sm text-gray-500">
        We'll never share your email.
      </p>
      <p v-if="emailError" id="email-error" class="mt-1 text-sm text-danger-600" role="alert">
        {{ emailError }}
      </p>
    </div>
    
    <!-- Submit button with loading state -->
    <SoftButton type="submit" :loading="isSubmitting" :disabled="!isValid">
      {{ isSubmitting ? 'Saving...' : 'Save Changes' }}
    </SoftButton>
  </form>
</template>
```

### Error Message Guidelines

| Error Type | Message Format | Example |
|------------|----------------|---------|
| Required | "Please enter your [field]" | "Please enter your email" |
| Format | "[Field] must be [format]" | "Email must be valid" |
| Length | "[Field] must be [min-max] characters" | "Password must be 8-50 characters" |
| Match | "[Field] doesn't match [other field]" | "Passwords don't match" |
| Server | "Unable to [action]. Please try again." | "Unable to save. Please try again." |

---

## Feedback & States

### Loading States

```vue
<!-- Page loading -->
<template>
  <div v-if="isLoading" class="min-h-[400px] flex items-center justify-center">
    <LoadingSpinner size="lg" />
  </div>
  <div v-else>
    <!-- Content -->
  </div>
</template>

<!-- Button loading -->
<SoftButton :loading="isSaving">
  {{ isSaving ? 'Saving...' : 'Save' }}
</SoftButton>

<!-- Skeleton loading -->
<div v-if="isLoading" class="animate-pulse space-y-4">
  <div class="h-4 bg-gray-200 rounded w-3/4"></div>
  <div class="h-4 bg-gray-200 rounded w-1/2"></div>
</div>
```

### Toast Notifications

```vue
<!-- Success toast -->
<Toast type="success" message="Changes saved successfully" />

<!-- Error toast -->
<Toast type="error" message="Failed to save changes. Please try again." />

<!-- Warning toast -->
<Toast type="warning" message="Your session will expire in 5 minutes" />

<!-- Info toast -->
<Toast type="info" message="New features are available" />
```

### Confirmation Dialogs

```vue
<template>
  <ConfirmDialog
    v-model="showDialog"
    title="Delete User"
    message="Are you sure you want to delete this user? This action cannot be undone."
    confirm-text="Delete"
    confirm-variant="danger"
    @confirm="handleDelete"
    @cancel="showDialog = false"
  />
</template>
```

---

## Performance Guidelines

### Core Web Vitals Targets

| Metric | Target | Description |
|--------|--------|-------------|
| LCP | < 2.5s | Largest Contentful Paint |
| FID | < 100ms | First Input Delay |
| CLS | < 0.1 | Cumulative Layout Shift |
| TTI | < 3s | Time to Interactive |

### Optimization Techniques

#### 1. Lazy Loading

```vue
<!-- Route-level code splitting -->
const routes = [
  {
    path: '/dashboard',
    component: () => import('./views/DashboardView.vue')
  }
]

<!-- Component lazy loading -->
<script setup>
import { defineAsyncComponent } from 'vue'

const HeavyChart = defineAsyncComponent(() =>
  import('./components/HeavyChart.vue')
)
</script>

<!-- Image lazy loading -->
<img loading="lazy" src="product.jpg" alt="Product" />
```

#### 2. Optimistic UI Updates

```typescript
// Update UI immediately, then sync with server
async function updateItem(id: string, data: ItemData) {
  // Optimistic update
  items.value = items.value.map(item =>
    item.id === id ? { ...item, ...data } : item
  )
  
  try {
    await api.updateItem(id, data)
  } catch (error) {
    // Rollback on error
    items.value = originalItems
    showError('Failed to update item')
  }
}
```

#### 3. Debounced Inputs

```vue
<script setup>
import { useDebounceFn } from '@vueuse/core'

const search = ref('')
const debouncedSearch = useDebounceFn((value: string) => {
  performSearch(value)
}, 300)

watch(search, debouncedSearch)
</script>
```

---

## Dark Mode

### Implementation

```vue
<!-- Theme toggle component -->
<template>
  <button
    @click="toggleTheme"
    class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700"
    :aria-label="isDark ? 'Switch to light mode' : 'Switch to dark mode'"
  >
    <SunIcon v-if="isDark" class="h-5 w-5" />
    <MoonIcon v-else class="h-5 w-5" />
  </button>
</template>

<script setup>
import { useColorMode } from '@vueuse/core'

const { store, system, state } = useColorMode()
const isDark = computed(() => state.value === 'dark')

function toggleTheme() {
  store.value = isDark.value ? 'light' : 'dark'
}
</script>
```

### Dark Mode Classes

```vue
<template>
  <!-- Background colors -->
  <div class="bg-white dark:bg-soft-800">
    <!-- Text colors -->
    <h1 class="text-soft-900 dark:text-white">Title</h1>
    <p class="text-soft-600 dark:text-soft-300">Body text</p>
    
    <!-- Border colors -->
    <div class="border border-soft-200 dark:border-soft-700">
      <!-- Content -->
    </div>
  </div>
</template>
```

### Color Mappings

| Element | Light Mode | Dark Mode |
|---------|------------|-----------|
| Page background | `bg-soft-50` | `bg-soft-900` |
| Card background | `bg-white` | `bg-soft-800` |
| Primary text | `text-soft-900` | `text-white` |
| Secondary text | `text-soft-600` | `text-soft-300` |
| Borders | `border-soft-200` | `border-soft-700` |
| Hover states | `hover:bg-soft-100` | `dark:hover:bg-soft-700` |

---

## Testing UX

### Manual Testing Checklist

```markdown
## Pre-Release UX Checklist

### Visual
- [ ] All text is readable (contrast check)
- [ ] Images have alt text
- [ ] Icons have labels or aria-labels
- [ ] Colors work in dark mode
- [ ] Layout is responsive at all breakpoints

### Interaction
- [ ] All buttons/links are clickable
- [ ] Forms validate correctly
- [ ] Error messages are helpful
- [ ] Loading states are shown
- [ ] Success feedback is provided

### Accessibility
- [ ] Page can be navigated with keyboard only
- [ ] Focus order is logical
- [ ] Screen reader announces content correctly
- [ ] Works at 200% zoom
- [ ] Works with reduced motion preference

### Performance
- [ ] Page loads in < 3 seconds
- [ ] No layout shift during load
- [ ] Interactions respond in < 100ms
- [ ] Images are optimized
```

### Automated Testing

```typescript
// E2E accessibility test with Playwright + axe-core
import { test, expect } from '@playwright/test'
import AxeBuilder from '@axe-core/playwright'

test('homepage should be accessible', async ({ page }) => {
  await page.goto('/')
  
  const accessibilityScanResults = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa'])
    .analyze()
  
  expect(accessibilityScanResults.violations).toEqual([])
})

// Visual regression test
test('dashboard matches snapshot', async ({ page }) => {
  await page.goto('/dashboard')
  await expect(page).toHaveScreenshot('dashboard.png')
})
```

---

## Quick Reference

### Component Cheat Sheet

| Need | Store (DaisyUI) | Admin (Soft UI) |
|------|-----------------|-----------------|
| Button | `btn btn-primary` | `<SoftButton variant="primary">` |
| Card | `card bg-base-100` | `<SoftCard>` |
| Badge | `badge badge-success` | `<SoftBadge variant="success">` |
| Input | `input input-bordered` | `<SoftInput>` |
| Alert | `alert alert-error` | `<div role="alert">` |

### Spacing Quick Reference

| Class | Value | Use Case |
|-------|-------|----------|
| `space-y-4` | 16px | List items |
| `gap-6` | 24px | Grid items |
| `p-4` | 16px | Card padding |
| `mb-8` | 32px | Section spacing |
| `px-safe` | 20px | Soft UI padding |

### Breakpoint Quick Reference

| Class Prefix | Min Width | Device |
|--------------|-----------|--------|
| (none) | 0 | Mobile |
| `sm:` | 640px | Mobile landscape |
| `md:` | 768px | Tablet |
| `lg:` | 1024px | Desktop |
| `xl:` | 1280px | Large desktop |

---

## Resources

### Internal Documentation

- [SOFT_UI_IMPLEMENTATION.md](../../frontend/Admin/SOFT_UI_IMPLEMENTATION.md) — Admin design system
- [THEME_VISUAL_GUIDE.md](../../frontend/Admin/THEME_VISUAL_GUIDE.md) — Theme toggle guide
- [ACCESSIBILITY_COMPLIANCE_REPORT.md](../../ACCESSIBILITY_COMPLIANCE_REPORT.md) — A11y audit

### External Resources

- [Tailwind CSS](https://tailwindcss.com/docs) — Utility CSS framework
- [DaisyUI](https://daisyui.com/) — Tailwind component library
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/) — Accessibility standards
- [Vue.js 3](https://vuejs.org/) — Frontend framework

---

**Document Owner**: @UX  
**Contributors**: @UI, @Frontend, @TechLead  
**Last Review**: 1. Januar 2026  
**Next Review**: 1. Februar 2026
