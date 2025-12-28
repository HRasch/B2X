# 🎨 Frontend Developer - Documentation Guide

**Role:** Frontend Developer | **P0 Components:** P0.6, P0.8  
**Time to Read:** ~3 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als Frontend Developer bist du verantwortlich für:
- **Accessibility (BITV 2.0)** - WCAG 2.1 AA Compliance - P0.8
- **E-Commerce UI** - Widerrufsrecht, Preisangaben, AGB - P0.6
- **Vue.js 3 Components** - Composition API
- **Multi-Tenant UI** - Tenant-aware components
- **i18n/Localization** - DE + EN minimum

---

## 📚 Required Reading (P0)

### Week 1: Frontend Foundation

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **Frontend Feature Guide** | [FRONTEND_FEATURE_INTEGRATION_GUIDE.md](../FRONTEND_FEATURE_INTEGRATION_GUIDE.md) | 30 min |
| 2 | **Admin Frontend Guide** | [ADMIN_FRONTEND_FEATURE_INTEGRATION_GUIDE.md](../ADMIN_FRONTEND_FEATURE_INTEGRATION_GUIDE.md) | 30 min |
| 3 | **Frontend Tenant Setup** | [FRONTEND_TENANT_SETUP.md](../FRONTEND_TENANT_SETUP.md) | 20 min |
| 4 | **Aspire Frontend Integration** | [ASPIRE_FRONTEND_INTEGRATION.md](../ASPIRE_FRONTEND_INTEGRATION.md) | 15 min |

### Week 2: Accessibility (P0.8 - CRITICAL!)

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 5 | **BITV 2.0 Tests** | [compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md](../compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md) | 45 min |
| 6 | **EU Compliance §P0.8** | [compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) | 30 min |

### Week 3: E-Commerce UI

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 7 | **E-Commerce Tests (P0.6)** | [compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md](../compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md) | 30 min |
| 8 | **E-Commerce Overview** | [compliance/ECOMMERCE_LEGAL_OVERVIEW.md](../compliance/ECOMMERCE_LEGAL_OVERVIEW.md) | 20 min |
| 9 | **Localization Guide** | [LOCALIZATION_IMPLEMENTATION_COMPLETE.md](../LOCALIZATION_IMPLEMENTATION_COMPLETE.md) | 20 min |

---

## 🔧 Your P0 Components

### P0.8: Barrierefreiheit / Accessibility (Week 2-4) 🔴 CRITICAL

**Standard:** WCAG 2.1 Level AA  
**Deadline:** 28. Juni 2025 (ALREADY ACTIVE!)  
**Penalty:** €5,000-100,000 per violation  

```
Effort: 45 hours
Tests: 12 (see P0.8_BARRIEREFREIHEIT_BITV_TESTS.md)

Your Tasks:
  ✅ Keyboard Navigation (TAB, ENTER, Escape)
  ✅ Screen Reader Support (ARIA labels, semantic HTML)
  ✅ Color Contrast (4.5:1 minimum)
  ✅ Text Resizing (200% without breaking)
  ✅ Video Captions (DE + EN)
  ✅ Alt-Text for all images
  ✅ Heading Hierarchy (H1-H6, no skips)

Acceptance:
  ✅ All 12 accessibility tests passing
  ✅ Lighthouse Accessibility >= 90
  ✅ NVDA/JAWS manual test passed
  ✅ axe DevTools: 0 critical issues
```

#### Keyboard Navigation Checklist

| Element | TAB | ENTER | Escape | Arrow Keys |
|---------|-----|-------|--------|------------|
| Links | Focus | Activate | - | - |
| Buttons | Focus | Activate | - | - |
| Dropdowns | Focus | Open | Close | Navigate |
| Modals | Focus first elem | - | Close | - |
| Forms | Navigate fields | Submit | Cancel | - |
| Tables | Navigate cells | - | - | Navigate |

#### ARIA Labels Checklist

```html
<!-- Product Card (CORRECT) -->
<article role="article" aria-labelledby="product-title-123">
  <img src="..." alt="iPhone 15 Pro, Space Black, 256GB" />
  <h3 id="product-title-123">iPhone 15 Pro</h3>
  <p aria-label="Preis">€999,00</p>
  <button aria-label="iPhone 15 Pro zum Warenkorb hinzufügen">
    In den Warenkorb
  </button>
</article>

<!-- Form Field (CORRECT) -->
<label for="email">E-Mail-Adresse</label>
<input 
  id="email" 
  type="email" 
  aria-describedby="email-error"
  aria-invalid="true"
/>
<span id="email-error" role="alert">
  Bitte geben Sie eine gültige E-Mail-Adresse ein.
</span>
```

### P0.6: E-Commerce UI (Week 5-6)

```
Effort: 30 hours (UI portion)
Tests: 15 total (UI-related: ~8)

Your Tasks:
  ✅ Price display (Brutto inkl. MwSt)
  ✅ Shipping cost visibility (before checkout!)
  ✅ Widerrufsrecht info (14 Tage)
  ✅ AGB Checkbox (before order)
  ✅ Datenschutz link (footer)
  ✅ Impressum link (footer)

Acceptance:
  ✅ Prices always show "inkl. MwSt"
  ✅ Shipping costs visible before checkout
  ✅ Widerrufsbelehrung accessible
  ✅ AGB checkbox mandatory
```

---

## ⚡ Quick Commands

```bash
# Start Store Frontend
cd Frontend/Store && npm run dev

# Start Admin Frontend  
cd Frontend/Admin && npm run dev

# Run Frontend Tests
npm run test

# Run E2E Tests (Playwright)
npm run test:e2e

# Accessibility Audit (Lighthouse)
npx lighthouse http://localhost:5173 --only-categories=accessibility

# axe DevTools CLI
npx @axe-core/cli http://localhost:5173

# Check Color Contrast
# Use: https://webaim.org/resources/contrastchecker/
```

---

## 🏗️ Component Structure

```vue
<!-- ProductCard.vue (Accessible) -->
<template>
  <article 
    role="article" 
    :aria-labelledby="`product-title-${product.id}`"
    class="product-card"
  >
    <!-- Image with descriptive alt -->
    <img 
      :src="product.imageUrl" 
      :alt="generateAltText(product)"
      loading="lazy"
    />
    
    <!-- Semantic heading -->
    <h3 :id="`product-title-${product.id}`">
      {{ product.name }}
    </h3>
    
    <!-- Price with ARIA -->
    <p :aria-label="$t('price')">
      {{ formatPrice(product.price) }} inkl. MwSt.
    </p>
    
    <!-- Accessible button -->
    <button 
      :aria-label="$t('addToCart', { product: product.name })"
      @click="addToCart"
      @keydown.enter="addToCart"
    >
      {{ $t('addToCart') }}
    </button>
  </article>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Product } from '@/types'

const { t } = useI18n()

const props = defineProps<{
  product: Product
}>()

const emit = defineEmits<{
  (e: 'add-to-cart', product: Product): void
}>()

// Generate descriptive alt text
const generateAltText = (product: Product): string => {
  return `${product.name}, ${product.color}, ${product.size}`
}

// Format price with currency
const formatPrice = (price: number): string => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR'
  }).format(price)
}

const addToCart = () => {
  emit('add-to-cart', props.product)
}
</script>

<style scoped>
/* Ensure focus visibility */
.product-card:focus-within {
  outline: 2px solid var(--focus-color);
  outline-offset: 2px;
}

/* Minimum touch target size (44x44px) */
button {
  min-width: 44px;
  min-height: 44px;
  padding: 12px 24px;
}

/* Color contrast: text vs background >= 4.5:1 */
.product-card {
  color: #1a1a1a;      /* Dark text */
  background: #ffffff; /* White background */
  /* Contrast ratio: 12.63:1 ✅ */
}
</style>
```

---

## 🧪 Testing Tools Setup

### axe DevTools (Chrome Extension)
1. Install: [axe DevTools](https://www.deque.com/axe/devtools/)
2. Open DevTools → axe Tab
3. Click "Analyze"
4. Fix all Critical/Serious issues

### NVDA Screen Reader (Windows)
1. Download: [nvaccess.org](https://www.nvaccess.org/download/)
2. Navigate app with keyboard only
3. Verify all content is announced

### Lighthouse (Built-in Chrome)
1. Open DevTools → Lighthouse Tab
2. Check "Accessibility"
3. Generate report
4. Target score: **90+**

### Color Contrast Checker
- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)
- Normal text: 4.5:1 minimum
- Large text: 3:1 minimum

---

## 📊 Accessibility Checklist (WCAG 2.1 AA)

### Keyboard Navigation
- [ ] All interactive elements focusable via TAB
- [ ] Focus order matches visual order
- [ ] No keyboard traps
- [ ] Skip links for main content
- [ ] Escape closes modals/dropdowns

### Screen Readers
- [ ] All images have alt text
- [ ] Form fields have labels
- [ ] Error messages use role="alert"
- [ ] Headings in correct hierarchy (H1→H2→H3)
- [ ] Tables have proper headers

### Visual
- [ ] Color contrast >= 4.5:1 (text)
- [ ] Color contrast >= 3:1 (large text)
- [ ] Text resizable to 200%
- [ ] No content loss at 320px width
- [ ] Focus indicators visible

### Media
- [ ] Videos have captions (DE + EN)
- [ ] Audio has transcript
- [ ] No auto-playing media

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Accessibility Question | QA Engineer | < 4h |
| Backend API Issue | Backend Developer | < 4h |
| Design Decision | Product Owner | < 24h |
| Legal/Compliance | Legal Officer | < 24h |

---

## ✅ Definition of Done (Frontend)

Before marking any task as complete:

- [ ] Component renders correctly
- [ ] Lighthouse Accessibility >= 90
- [ ] axe DevTools: 0 critical issues
- [ ] Keyboard navigation works
- [ ] Screen reader tested (NVDA/VoiceOver)
- [ ] Responsive (320px - 1920px)
- [ ] i18n keys added (DE + EN)
- [ ] Unit tests written
- [ ] E2E test added (if applicable)

---

**Next:** Start with [P0.8_BARRIEREFREIHEIT_BITV_TESTS.md](../compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md)
