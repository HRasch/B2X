# Frontend ERP Integration Guide

**Status**: ✅ PRODUCTION READY  
**Framework**: Vue 3 + TypeScript  
**Pattern**: Composable + Component  
**Date**: 29. Dezember 2025

---

## 📖 Overview

The frontend ERP integration provides a complete customer lookup system that works seamlessly with the backend ERP Provider Pattern. This guide covers:

1. **useErpIntegration Composable** - Reactive customer lookup logic
2. **CustomerLookup Component** - Pre-built UI component
3. **Integration Patterns** - How to use in your application
4. **Testing** - Unit and component tests
5. **Styling** - Tailwind CSS with dark mode support
6. **Accessibility** - WCAG 2.1 AA compliance

---

## 🎯 Quick Start

### 1. Use the Composable Directly

```typescript
<script setup lang="ts">
import { ref } from 'vue'
import { useErpIntegration } from '@/composables/useErpIntegration'

const email = ref('')
const { validateCustomerEmail, customer, isLoading, error } = useErpIntegration()

const handleLookup = async () => {
  const result = await validateCustomerEmail(email.value)
  if (result.isValid) {
    console.log('Customer found:', result.customer)
  }
}
</script>

<template>
  <form @submit.prevent="handleLookup">
    <input v-model="email" type="email" placeholder="Email address" />
    <button :disabled="isLoading" type="submit">
      {{ isLoading ? 'Searching...' : 'Search Customer' }}
    </button>
    
    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="customer" class="success">
      Welcome back, {{ customer.customerName }}!
    </div>
  </form>
</script>
```

### 2. Use the Pre-Built Component

```typescript
<script setup lang="ts">
import CustomerLookup from '@/components/ERP/CustomerLookup.vue'

const handleRegister = () => {
  console.log('User wants to register')
}

const handleProceed = (customerNumber: string) => {
  console.log('Proceed with customer:', customerNumber)
}
</script>

<template>
  <CustomerLookup 
    @register="handleRegister" 
    @proceed="handleProceed"
    :isDevelopment="true"
  />
</template>
```

---

## 📋 Composable: useErpIntegration

### State

```typescript
const {
  // Reactive state
  isLoading,           // boolean - loading indicator
  customer,            // ErpCustomer | null - current customer
  error,               // string | null - error message
  lastLookupTime,      // number | null - lookup duration in ms

  // Computed state
  hasCustomer,         // ComputedRef<boolean> - true if customer found
  isPrivateCustomer,   // ComputedRef<boolean> - true if PRIVATE type
  isBusinessCustomer,  // ComputedRef<boolean> - true if BUSINESS type

  // Methods
  validateCustomerEmail,    // (email: string) => Promise<ValidationResult>
  validateCustomerNumber,   // (number: string) => Promise<ValidationResult>
  clearCustomer,            // () => void
} = useErpIntegration()
```

### Methods

#### validateCustomerEmail(email: string)

Lookup customer by email address.

```typescript
const result = await validateCustomerEmail('max.mustermann@example.com')

if (result.isValid) {
  console.log('Customer found:', result.customer)
  console.log('Lookup took:', result.loadingMs, 'ms')
} else {
  console.log('Error:', result.message)
}
```

**Response**: ValidationResult
- `isValid` - boolean (true if customer found)
- `customer` - ErpCustomer | null
- `error` - string (error code)
- `message` - string (user-friendly message)
- `loadingMs` - number (lookup duration)

#### validateCustomerNumber(customerNumber: string)

Lookup customer by customer number (e.g., "CUST-001").

```typescript
const result = await validateCustomerNumber('CUST-100')

if (result.isValid) {
  const customer = result.customer
  if (customer?.businessType === 'BUSINESS') {
    console.log('Business customer:', customer.customerName)
    console.log('Credit limit:', customer.creditLimit)
  }
}
```

#### clearCustomer()

Clear all customer data and reset error state.

```typescript
const { clearCustomer } = useErpIntegration()

const handleNewSearch = () => {
  clearCustomer()
  // UI resets to empty state
}
```

### Types

```typescript
interface ErpCustomer {
  customerNumber: string
  customerName: string
  email: string
  phone?: string
  shippingAddress?: string
  billingAddress?: string
  country: string
  businessType: 'PRIVATE' | 'BUSINESS'
  isActive: boolean
  creditLimit?: number
  lastOrderDate?: string
}

interface ValidationResult {
  isValid: boolean
  customer: ErpCustomer | null
  error?: string
  message?: string
  loadingMs?: number
}
```

---

## 🎨 Component: CustomerLookup

### Props

```typescript
interface Props {
  isDevelopment?: boolean  // Show diagnostic info in dev mode
}
```

### Events

```typescript
// User clicked "Register" button (new customer)
@register

// User found and clicked "Proceed" button
@proceed="handleProceed"  // Receives: customerNumber (string)
```

### Usage Example

```vue
<template>
  <div class="registration-page">
    <CustomerLookup 
      @register="handleNewCustomerRegistration"
      @proceed="handleExistingCustomerFlow"
      :isDevelopment="import.meta.env.MODE === 'development'"
    />
  </div>
</template>

<script setup lang="ts">
import CustomerLookup from '@/components/ERP/CustomerLookup.vue'

const handleNewCustomerRegistration = () => {
  // Navigate to registration form
  router.push('/register')
}

const handleExistingCustomerFlow = (customerNumber: string) => {
  // Navigate to checkout with customer info
  router.push({
    name: 'checkout',
    params: { customerNumber }
  })
}
</script>
```

### Features

- ✅ Email-based customer lookup
- ✅ Real-time validation
- ✅ Loading state with spinner
- ✅ Error handling and retry
- ✅ Dark mode support
- ✅ Responsive design
- ✅ Accessibility (WCAG 2.1 AA)
- ✅ Diagnostic info (dev mode)
- ✅ Performance metrics (lookup time)

---

## 🔌 API Integration

The frontend communicates with the backend via these endpoints:

### POST /api/auth/erp/validate-email

```typescript
// Request
{
  email: "max.mustermann@example.com"
}

// Response (200 OK)
{
  customerNumber: "CUST-001",
  customerName: "Max Mustermann",
  email: "max.mustermann@example.com",
  country: "DE",
  businessType: "PRIVATE",
  isActive: true,
  phone?: "+49 123 456",
  shippingAddress?: "Main Street 123, Berlin",
  billingAddress?: "Main Street 123, Berlin",
  creditLimit?: 0
}

// Response (404 Not Found)
{
  error: "NOT_FOUND",
  message: "Kunde nicht gefunden"
}
```

### POST /api/auth/erp/validate-number

```typescript
// Request
{
  customerNumber: "CUST-100"
}

// Response (200 OK)
{
  customerNumber: "CUST-100",
  customerName: "TechCorp GmbH",
  email: "info@techcorp.de",
  country: "DE",
  businessType: "BUSINESS",
  isActive: true,
  creditLimit: 50000
}
```

---

## 🧪 Testing

### Testing the Composable

```typescript
import { useErpIntegration } from '@/composables/useErpIntegration'
import { vi } from 'vitest'

describe('useErpIntegration', () => {
  it('should validate customer email', async () => {
    global.fetch = vi.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => ({
        customerNumber: 'CUST-001',
        customerName: 'Test User',
        email: 'test@example.com',
        country: 'DE',
        businessType: 'PRIVATE',
        isActive: true,
      }),
    })

    const { validateCustomerEmail } = useErpIntegration()
    const result = await validateCustomerEmail('test@example.com')

    expect(result.isValid).toBe(true)
    expect(result.customer?.customerName).toBe('Test User')
  })

  it('should handle validation errors', async () => {
    const { validateCustomerEmail } = useErpIntegration()
    const result = await validateCustomerEmail('invalid-email')

    expect(result.isValid).toBe(false)
    expect(result.error).toBe('INVALID_EMAIL')
  })
})
```

### Testing the Component

```typescript
import { mount } from '@vue/test-utils'
import CustomerLookup from '@/components/ERP/CustomerLookup.vue'

describe('CustomerLookup Component', () => {
  it('should render email input', () => {
    const wrapper = mount(CustomerLookup)
    const input = wrapper.find('#email')

    expect(input.exists()).toBe(true)
    expect(input.attributes('type')).toBe('email')
  })

  it('should emit register event', async () => {
    const wrapper = mount(CustomerLookup)
    const registerBtn = wrapper.find('button')

    await registerBtn.trigger('click')
    expect(wrapper.emitted('register')).toBeTruthy()
  })

  it('should show dark mode styles', () => {
    const wrapper = mount(CustomerLookup)
    const html = wrapper.html()

    expect(html).toContain('dark:')
  })
})
```

---

## 🎯 Integration Patterns

### Pattern 1: Registration Flow

```typescript
// pages/Register.vue
<script setup lang="ts">
import CustomerLookup from '@/components/ERP/CustomerLookup.vue'

const handleExistingCustomer = (customerNumber: string) => {
  // Skip registration, go to checkout
  router.push({
    name: 'checkout',
    params: { customerNumber }
  })
}

const handleNewCustomer = () => {
  // Show registration form
  showRegistrationForm.value = true
}
</script>

<template>
  <div class="container mx-auto p-4">
    <CustomerLookup 
      @register="handleNewCustomer"
      @proceed="handleExistingCustomer"
    />
    
    <RegistrationForm v-if="showRegistrationForm" />
  </div>
</template>
```

### Pattern 2: Login Flow

```typescript
// pages/Login.vue
<script setup lang="ts">
import { useErpIntegration } from '@/composables/useErpIntegration'
import { useAuth } from '@/composables/useAuth'

const email = ref('')
const { validateCustomerEmail, customer, error } = useErpIntegration()
const { login } = useAuth()

const handleLogin = async () => {
  const result = await validateCustomerEmail(email.value)
  
  if (result.isValid && result.customer) {
    // Customer found, proceed with authentication
    await login({
      email: email.value,
      customerNumber: result.customer.customerNumber
    })
  }
}
</script>

<template>
  <form @submit.prevent="handleLogin">
    <input v-model="email" type="email" required />
    <button type="submit">Login</button>
    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="customer" class="success">
      Welcome {{ customer.customerName }}!
    </p>
  </form>
</template>
```

### Pattern 3: Checkout with Customer Info

```typescript
// pages/Checkout.vue
<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useErpIntegration } from '@/composables/useErpIntegration'

const route = useRoute()
const { validateCustomerNumber, customer } = useErpIntegration()

onMounted(async () => {
  if (route.params.customerNumber) {
    await validateCustomerNumber(route.params.customerNumber as string)
  }
})
</script>

<template>
  <div v-if="customer" class="checkout">
    <h1>Checkout</h1>
    
    <!-- Pre-filled customer info -->
    <div class="customer-info">
      <p><strong>Name:</strong> {{ customer.customerName }}</p>
      <p><strong>Email:</strong> {{ customer.email }}</p>
      <p v-if="customer.shippingAddress">
        <strong>Address:</strong> {{ customer.shippingAddress }}
      </p>
    </div>
    
    <!-- Checkout form continues... -->
  </div>
</template>
```

---

## 🎨 Styling Guide

### Tailwind CSS Classes Used

```css
/* Layout */
.space-y-6           /* Vertical spacing */
.flex                /* Flexbox layout */
.grid grid-cols-2    /* Grid layout */

/* Colors (Light) */
.bg-white            /* White background */
.text-gray-900       /* Dark text */
.border-gray-300     /* Light border */

/* Colors (Dark Mode) */
.dark:bg-gray-800    /* Dark background */
.dark:text-white     /* Light text in dark mode */
.dark:border-gray-600 /* Light border in dark mode */

/* States */
.disabled:bg-gray-300  /* Disabled state */
.hover:bg-blue-700     /* Hover state */
.focus:ring-2          /* Focus state */

/* Components */
.rounded-md            /* Border radius */
.shadow               /* Shadow effect */
.transition-colors    /* Smooth color transitions */
```

### Customization Example

```vue
<!-- Override component styling -->
<template>
  <div class="custom-lookup">
    <CustomerLookup @proceed="handleProceed" />
  </div>
</template>

<style scoped>
.custom-lookup :deep(input) {
  @apply border-2 border-purple-300 focus:ring-purple-500;
}

.custom-lookup :deep(button) {
  @apply rounded-lg font-bold shadow-lg;
}

.custom-lookup :deep(.space-y-6) {
  @apply space-y-8;
}
</style>
```

---

## ♿ Accessibility (WCAG 2.1 AA)

### Features Implemented

✅ **Semantic HTML**
- Proper form structure with labels
- Role attributes on alerts
- Proper heading hierarchy

✅ **Keyboard Navigation**
- Tab through all interactive elements
- Buttons are keyboard accessible
- Proper focus management

✅ **Screen Reader Support**
- aria-label on inputs
- role="alert" on status messages
- Descriptive button text

✅ **Color Contrast**
- 4.5:1 ratio for normal text
- 3:1 ratio for large text
- Proper dark mode support

### Testing Accessibility

```bash
# Run accessibility audit
npx @axe-core/cli http://localhost:5173

# Check with Lighthouse
npx lighthouse http://localhost:5173 --only-categories=accessibility

# Test with screen reader (macOS)
# VoiceOver: Cmd+F5
# Then navigate with VO+arrow keys
```

### Keyboard Navigation Test

```
1. Tab to email input
2. Type email address
3. Tab to "Kundensuche" button
4. Enter to submit
5. Tab through customer details
6. Tab to action buttons
7. Shift+Tab to go backward
8. Escape to close/clear (if implemented)
```

---

## 🚀 Performance Optimization

### Network Performance

```typescript
// Use debouncing for email validation
import { useDebounceFn } from '@vueuse/core'

const debouncedValidateEmail = useDebounceFn(
  validateCustomerEmail,
  500  // 500ms delay
)

// User stops typing for 500ms before lookup starts
watch(email, () => {
  debouncedValidateEmail(email.value)
})
```

### Caching Pattern

```typescript
// Cache lookups within session
const lookupCache = new Map<string, ValidationResult>()

const cachedValidateEmail = async (email: string) => {
  if (lookupCache.has(email)) {
    return lookupCache.get(email)!
  }
  
  const result = await validateCustomerEmail(email)
  lookupCache.set(email, result)
  return result
}
```

### Lazy Loading

```typescript
// Load component only when needed
const CustomerLookup = defineAsyncComponent(() =>
  import('@/components/ERP/CustomerLookup.vue')
)
```

---

## 🔒 Security Best Practices

### Input Validation

```typescript
// Frontend validation (UX improvement)
const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

// Backend validation (security requirement)
// Always validate on backend! Never trust frontend validation alone
```

### HTTPS Requirement

```typescript
// Ensure API calls use HTTPS in production
const apiBaseUrl = import.meta.env.VITE_API_URL
// Must start with https:// in production
```

### Error Message Handling

```typescript
// Never expose internal error details to user
const userFriendlyErrors: Record<string, string> = {
  'NETWORK_ERROR': 'Verbindungsfehler. Bitte versuchen Sie es später erneut.',
  'NOT_FOUND': 'Kunde nicht gefunden.',
  'INVALID_EMAIL': 'Bitte geben Sie eine gültige E-Mail-Adresse ein.',
  'SERVER_ERROR': 'Ein Fehler ist aufgetreten. Bitte kontaktieren Sie den Support.',
}
```

---

## 📝 Sample Data for Testing

Available in Faker:

### B2C Customers
- **CUST-001**: Max Mustermann (max.mustermann@example.com) - Germany
- **CUST-002**: Erika Musterfrau (erika.musterfrau@example.com) - Germany

### B2B Customers
- **CUST-100**: TechCorp GmbH (info@techcorp.de) - €50k credit
- **CUST-101**: InnovateLabs AG (contact@innovatelabs.at) - €75k credit
- **CUST-102**: Global Solutions SA (sales@globalsolutions.ch) - €100k credit

---

## 🐛 Debugging

### Enable Debug Logging

```typescript
// composables/useErpIntegration.ts
const validateCustomerEmail = async (email: string) => {
  if (import.meta.env.MODE === 'development') {
    console.log('[ERP] Looking up email:', email)
  }
  
  // ... rest of code
}
```

### Browser DevTools

```javascript
// In browser console
// Set custom performance mark
performance.mark('erp-lookup-start')
// ... perform lookup ...
performance.mark('erp-lookup-end')
performance.measure('erp-lookup', 'erp-lookup-start', 'erp-lookup-end')

// View measurements
performance.getEntriesByType('measure')
```

### Network Inspection

```
1. Open DevTools (F12)
2. Go to Network tab
3. Trigger customer lookup
4. Check /api/auth/erp/validate-email request
5. View request headers, body, response
6. Check timing in Timing tab
```

---

## 📚 File Structure

```
Frontend/Store/src/
├── composables/
│   ├── useErpIntegration.ts          ← Main composable
│   └── __tests__/
│       └── useErpIntegration.spec.ts ← Tests
├── components/
│   └── ERP/
│       ├── CustomerLookup.vue        ← Main component
│       └── __tests__/
│           └── CustomerLookup.spec.ts ← Component tests
└── pages/
    ├── Register.vue                  ← Registration flow example
    ├── Login.vue                     ← Login flow example
    └── Checkout.vue                  ← Checkout flow example
```

---

## 🎯 Next Steps

1. **Integrate with Registration**: Add CustomerLookup to registration page
2. **Add to Checkout**: Pre-fill customer info in checkout
3. **Implement Caching**: Cache recent lookups for performance
4. **Add Analytics**: Track lookup patterns
5. **Enhance Error Handling**: Add retry logic with exponential backoff

---

**Status**: ✅ PRODUCTION READY

The frontend ERP integration is complete, tested, documented, and ready to integrate into your registration and checkout flows.
