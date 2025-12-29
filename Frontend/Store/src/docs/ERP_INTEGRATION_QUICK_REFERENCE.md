# Frontend ERP Integration - Quick Reference

**One-Page Cheat Sheet for Developers**

---

## 🚀 5-Minute Setup

### Use the Pre-Built Component
```vue
<script setup lang="ts">
import CustomerLookup from '@/components/ERP/CustomerLookup.vue'

const handleRegister = () => router.push('/register')
const handleProceed = (customerNumber: string) => {
  router.push({ name: 'checkout', params: { customerNumber } })
}
</script>

<template>
  <CustomerLookup @register="handleRegister" @proceed="handleProceed" />
</template>
```

### Or Use the Composable Directly
```vue
<script setup lang="ts">
import { ref } from 'vue'
import { useErpIntegration } from '@/composables/useErpIntegration'

const email = ref('')
const { validateCustomerEmail, customer, isLoading, error } = useErpIntegration()
</script>

<template>
  <input v-model="email" type="email" />
  <button @click="validateCustomerEmail(email)">Search</button>
  <div v-if="customer">Found: {{ customer.customerName }}</div>
</template>
```

---

## 📚 API Reference

### useErpIntegration()

**State**
```typescript
const {
  isLoading,          // boolean
  customer,           // ErpCustomer | null
  error,              // string | null
  lastLookupTime,     // number | null (ms)
  hasCustomer,        // ComputedRef<boolean>
  isPrivateCustomer,  // ComputedRef<boolean>
  isBusinessCustomer, // ComputedRef<boolean>
} = useErpIntegration()
```

**Methods**
```typescript
// Lookup by email
const result = await validateCustomerEmail('test@example.com')
// Returns: { isValid, customer, error, message, loadingMs }

// Lookup by number
const result = await validateCustomerNumber('CUST-001')

// Clear state
clearCustomer()
```

---

## 💾 Test Data (Development)

| Customer | Email | Type | Country |
|----------|-------|------|---------|
| CUST-001 | max.mustermann@example.com | B2C | DE |
| CUST-002 | erika.musterfrau@example.com | B2C | DE |
| CUST-100 | info@techcorp.de | B2B | DE |
| CUST-101 | contact@innovatelabs.at | B2B | AT |
| CUST-102 | sales@globalsolutions.ch | B2B | CH |

---

## 🎨 Component Props & Events

**Props**
```typescript
<CustomerLookup :isDevelopment="true" />
```

**Events**
```typescript
// User wants to register (new customer)
@register

// User found, proceed to checkout
@proceed="(customerNumber) => { ... }"
```

---

## 🧪 Testing Quick Tips

```typescript
// Mock the composable
vi.mock('@/composables/useErpIntegration', () => ({
  useErpIntegration: () => ({
    validateCustomerEmail: vi.fn(),
    customer: ref(null),
    isLoading: ref(false),
    error: ref(null),
    hasCustomer: computed(() => false),
  })
}))

// Mount component
const wrapper = mount(CustomerLookup)
expect(wrapper.find('#email').exists()).toBe(true)
```

---

## 🎯 Common Use Cases

### Registration Flow
```typescript
const handleExistingCustomer = (customerNumber: string) => {
  // Skip registration, go to checkout
  router.push({ name: 'checkout', params: { customerNumber } })
}

const handleNewCustomer = () => {
  // Show registration form
  showForm.value = true
}
```

### Pre-Fill Checkout
```typescript
const { customer } = useErpIntegration()

onMounted(async () => {
  await validateCustomerNumber(route.params.customerNumber)
  // customer.value now has shipping info
})
```

### Add to Login
```typescript
const handleLogin = async () => {
  const result = await validateCustomerEmail(email.value)
  if (result.isValid) {
    await authService.login({
      email: email.value,
      customerNumber: result.customer?.customerNumber
    })
  }
}
```

---

## ⚙️ Configuration

**API Endpoint** (Backend provides via ENV)
```
POST /api/auth/erp/validate-email
POST /api/auth/erp/validate-number
```

**Environment Variables**
```
VITE_API_URL=http://localhost:8000  # Store Gateway
```

---

## 🔍 Debugging

```typescript
// Enable debug logging
if (import.meta.env.MODE === 'development') {
  console.log('[ERP] Customer lookup:', result)
}

// Check performance
console.log('Lookup took:', result.loadingMs, 'ms')

// View diagnostic info in component
<CustomerLookup :isDevelopment="true" />  <!-- Shows state -->
```

---

## ✅ Checklist for Integration

- [ ] Import `useErpIntegration` or `CustomerLookup`
- [ ] Add to registration/login page
- [ ] Handle `@register` and `@proceed` events
- [ ] Style with Tailwind (component has classes)
- [ ] Test with sample data (CUST-001, etc.)
- [ ] Verify dark mode works
- [ ] Test keyboard navigation
- [ ] Test with screen reader (VoiceOver on macOS)
- [ ] Check API calls in DevTools Network tab

---

## 🚀 Performance Tips

```typescript
// Debounce email input (avoid excessive API calls)
import { useDebounceFn } from '@vueuse/core'
const debouncedLookup = useDebounceFn(validateCustomerEmail, 500)

// Cache results (for same session)
const lookupCache = new Map()
const cachedLookup = async (email) => {
  if (lookupCache.has(email)) return lookupCache.get(email)
  const result = await validateCustomerEmail(email)
  lookupCache.set(email, result)
  return result
}

// Lazy load component
const CustomerLookup = defineAsyncComponent(() =>
  import('@/components/ERP/CustomerLookup.vue')
)
```

---

## 🔒 Security Notes

- ✅ Input validation (email format)
- ✅ HTTPS in production only
- ✅ No PII in console logs
- ✅ Error messages don't expose internal details
- ✅ Backend validates all inputs (never trust frontend)

---

## 📊 What Works

✅ Email lookup (any valid email)  
✅ Customer number lookup (CUST-001, etc.)  
✅ B2C and B2B customers  
✅ Dark mode support  
✅ Loading states  
✅ Error handling with retry  
✅ Responsive design (mobile to desktop)  
✅ Keyboard navigation  
✅ Screen reader support (WCAG AA)  

---

## 🆘 Troubleshooting

| Issue | Solution |
|-------|----------|
| API 404 | Check VITE_API_URL env variable, backend running? |
| "Kunde nicht gefunden" | Try sample data: max.mustermann@example.com |
| No dark mode | Check parent has `dark` class on root element |
| Component not styled | Verify Tailwind CSS is configured |
| Tests failing | Mock `useErpIntegration` in test file |

---

## 📖 Full Documentation

→ See: `ERP_INTEGRATION_GUIDE.md`

---

**Last Updated**: 29. Dezember 2025  
**Status**: ✅ PRODUCTION READY
