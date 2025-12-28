# 🎨 Frontend Developer Quick Start

**Role Focus:** Vue.js 3, Tailwind CSS, Accessibility (WCAG 2.1 AA), UX  
**Time to Productivity:** 3 weeks  
**Critical Deadline:** 28. Juni 2025 (Accessibility/BITV)

---

## ⚡ Week 1: Vue 3 & Project Setup

### Day 1-2: Project Structure
```bash
# Store Frontend (customer-facing)
cd Frontend/Store
npm install
npm run dev  # Port 5173

# Admin Frontend (staff)
cd Frontend/Admin
npm install
npm run dev  # Port 5174
```

### Day 3-4: Vue 3 Composition API
```vue
<!-- ✅ CORRECT: Composition API -->
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useProductStore } from '@/stores/productStore'

const products = ref<Product[]>([])
const selectedCategory = ref('')

const filtered = computed(() => 
  products.value.filter(p => p.category === selectedCategory.value)
)

const loadProducts = async () => {
  products.value = await fetchProducts()
}
</script>

<!-- ❌ WRONG: Options API -->
<!-- DO NOT USE in new code! -->
```

### Day 5: Component Structure
```
src/
├── components/
│   ├── base/           # Reusable (BaseButton, BaseInput)
│   ├── layout/         # TheHeader, TheSidebar (The = singleton)
│   └── feature/        # ProductCard, CartItem
├── stores/             # Pinia state management
├── services/           # API clients (catalogService.ts)
├── types/              # TypeScript interfaces
└── views/              # Page components
```

---

## 🎨 Week 2: Tailwind CSS & Styling

### Day 1-2: Utility-First (Mobile-First!)
```vue
<!-- ✅ CORRECT: Mobile-first breakpoints -->
<div class="block md:flex gap-4">
  <!-- block on mobile, flex on md+ (768px) -->
</div>

<!-- ❌ WRONG: Using sm: for mobile -->
<div class="sm:block hidden">
  <!-- sm: means 640px+, NOT mobile! -->
</div>
```

### Day 3-4: Dark Mode + State Variants
```vue
<!-- Colors adapt to dark/light mode -->
<button class="bg-white dark:bg-slate-800
               text-slate-900 dark:text-white
               hover:bg-gray-100 dark:hover:bg-slate-700
               disabled:opacity-50">
  Click me
</button>

<!-- Minimum color contrast: 4.5:1 -->
<!-- Verify with: https://webaim.org/resources/contrastchecker/ -->
```

### Day 5: Design System
```vue
<!-- Use design tokens, not magic values -->
<div class="p-4 gap-4 text-base">  <!-- ✅ Uses theme -->
  <!-- NOT: p-5 gap-3.5 text-15px -->
</div>
```

**Color Contrast Checking:**
```bash
# Install axe DevTools Chrome Extension
# Open DevTools → axe Tab → Click "Scan"
# Target: All colors >= 4.5:1 contrast ratio
```

---

## ♿ Week 3: Accessibility (WCAG 2.1 AA) - **CRITICAL!**

### Day 1-2: Keyboard Navigation
```vue
<!-- Every interactive element must be keyboard accessible -->
<button @click="handleClick" @keydown.enter="handleClick">
  Action Button
</button>

<!-- Use semantic HTML -->
<nav>
  <a href="/products">Products</a>
  <button @click="openMenu">Menu</button>
</nav>

<!-- NOT: <div @click="..."> ← Wrong! -->
```

### Day 3-4: Screen Reader Support
```vue
<!-- ARIA labels for non-obvious elements -->
<img src="product.jpg" alt="iPhone 15 Pro, Space Black, 256GB" />

<!-- Form labels -->
<label for="email">Email Address</label>
<input id="email" type="email" aria-describedby="email-error" />
<span id="email-error" role="alert">Invalid email</span>

<!-- Heading hierarchy (no skips: H1→H2→H3) -->
<h1>Products</h1>
<h2>Category</h2>
<!-- NOT: <h1><h3> with h2 missing -->
```

### Day 5: Testing Accessibility
```bash
# Install tools:
npm install -D @axe-core/cli

# Run axe tests:
axe http://localhost:5173

# Manual testing:
# 1. Tab through page (no traps)
# 2. Escape closes modals
# 3. Arrow keys navigate dropdowns
# 4. Screen reader (NVDA Windows, VoiceOver Mac) announces all content

# Lighthouse audit:
# DevTools → Lighthouse → check "Accessibility" → Run
# Target: >= 90 score
```

---

## 🧪 Testing Setup

```bash
# Unit tests
npm run test

# E2E tests (Playwright)
npm run test:e2e

# Lint & format
npm run lint

# Type checking
npm run type-check
```

---

## ⚡ Quick Commands

```bash
# Development
cd Frontend/Store && npm run dev        # Port 5173
cd Frontend/Admin && npm run dev        # Port 5174

# Testing
npm run test                             # Unit tests
npm run test:e2e                         # E2E tests
npm run lint                             # ESLint

# Accessibility
npx @axe-core/cli http://localhost:5173
npx lighthouse http://localhost:5173 --only-categories=accessibility

# Build
npm run build
npm run preview
```

---

## 📚 Critical Resources

| Topic | File | Time |
|-------|------|------|
| Component Patterns | `FRONTEND_FEATURE_INTEGRATION_GUIDE.md` | 30 min |
| Admin Frontend | `ADMIN_FRONTEND_FEATURE_INTEGRATION_GUIDE.md` | 20 min |
| Accessibility (P0.8) | `docs/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md` | 45 min |
| Tailwind Best Practices | `.github/copilot-instructions.md` §Tailwind | 20 min |
| Localization | `docs/LOCALIZATION_IMPLEMENTATION_COMPLETE.md` | 15 min |

---

## 🎯 First Task: Accessible Product Card

**Requirements:**
1. ✅ Keyboard navigation (Tab through all buttons)
2. ✅ Screen reader support (all text announced)
3. ✅ Color contrast 4.5:1 (test with WebAIM)
4. ✅ Mobile responsive (< 320px works)
5. ✅ Dark mode support (dark: variants)

**Implementation:**
```vue
<template>
  <article 
    role="article"
    :aria-labelledby="`product-${id}`"
    class="p-4 rounded-lg bg-white dark:bg-slate-800"
  >
    <img 
      :src="imageUrl"
      :alt="generateAltText()"
      class="w-full h-48 object-cover rounded"
    />
    <h3 :id="`product-${id}`" class="mt-2 text-lg font-bold">
      {{ name }}
    </h3>
    <p class="text-base text-gray-700 dark:text-gray-300">
      {{ formatPrice() }} inkl. MwSt.
    </p>
    <button
      :aria-label="`Add ${name} to cart`"
      @click="addToCart"
      @keydown.enter="addToCart"
      class="mt-4 px-4 py-2 bg-blue-500 text-white rounded
             hover:bg-blue-600 focus:outline-none focus:ring-2"
    >
      Add to Cart
    </button>
  </article>
</template>

<script setup lang="ts">
const generateAltText = () => `${name}, ${color}, ${size}`
</script>
```

**Test with:**
```bash
# 1. Tab through component
# 2. Press Enter on button
# 3. Use NVDA/VoiceOver - verify all text announced
# 4. Check contrast ratio
# 5. Resize browser to 320px - verify responsive
```

**Time Estimate:** 4 hours  
**Success:** All accessibility tests passing + code review approved

---

## 🚨 Accessibility Deadline: 28. Juni 2025

**This is ACTIVE** (6 months away). Federal office can fine €5,000-100,000 per violation.

**Before June 28, ALL pages must have:**
- ✅ Keyboard navigation
- ✅ Screen reader support  
- ✅ Color contrast 4.5:1
- ✅ Text resizing (200% without breaking)
- ✅ ARIA labels
- ✅ Lighthouse Accessibility >= 90

**No excuses - this is mandatory.** 🚨

---

## 📞 Getting Help

- **Vue 3 Question:** Check `FRONTEND_FEATURE_INTEGRATION_GUIDE.md`
- **Accessibility Issue:** See `P0.8_BARRIEREFREIHEIT_BITV_TESTS.md`
- **API Integration:** Ask Backend Developer
- **Design Question:** Ask Product Owner

---

**Key Reminders:**
- Mobile-first (unprefixed = mobile)
- Composition API only (no Options API)
- Every interactive element keyboard-accessible
- Every image needs alt-text
- Test with screen reader before submitting
