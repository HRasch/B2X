# Frontend ERP Integration - Implementation Guide

**Complete guide for integrating ERP customer lookup into your pages**

---

## 📑 Contents

1. [Registration Page](#registration-page)
2. [Login Page](#login-page)
3. [Checkout Page](#checkout-page)
4. [API Setup](#api-setup)
5. [Testing](#testing)

---

## Registration Page

### Implementation

```vue
<!-- pages/Register.vue -->
<script setup lang="ts">
import { ref, reactive } from 'vue'
import CustomerLookup from '@/components/ERP/CustomerLookup.vue'
import RegistrationForm from '@/components/Registration/RegistrationForm.vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// State
const step = ref<'lookup' | 'register' | 'confirm'>('lookup')
const foundCustomer = reactive({
  customerNumber: '',
  customerName: '',
  email: '',
  businessType: 'PRIVATE' as 'PRIVATE' | 'BUSINESS',
})

// Handle existing customer
const handleProceedWithExistingCustomer = (customerNumber: string) => {
  foundCustomer.customerNumber = customerNumber
  
  // Option 1: Go directly to checkout (skip registration)
  router.push({
    name: 'checkout',
    params: { customerNumber },
    query: { existing: 'true' }
  })
  
  // Option 2: Show confirmation page
  // step.value = 'confirm'
}

// Handle new customer
const handleRegisterNewCustomer = () => {
  step.value = 'register'
}

// Handle registration form submission
const handleRegistrationComplete = async (formData: any) => {
  try {
    // Submit to API
    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    })

    if (response.ok) {
      const { customerNumber } = await response.json()
      
      // Redirect to checkout
      router.push({
        name: 'checkout',
        params: { customerNumber },
        query: { new: 'true' }
      })
    }
  } catch (error) {
    console.error('Registration failed:', error)
  }
}

// Back button handler
const handleBack = () => {
  if (step.value === 'register') {
    step.value = 'lookup'
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 px-4">
    <div class="max-w-md mx-auto">
      <!-- Step: Lookup Existing Customer -->
      <div v-if="step === 'lookup'">
        <CustomerLookup
          :isDevelopment="$config.isDevelopment"
          @register="handleRegisterNewCustomer"
          @proceed="handleProceedWithExistingCustomer"
        />
      </div>

      <!-- Step: Register New Customer -->
      <div v-if="step === 'register'">
        <div class="mb-4">
          <button
            @click="handleBack"
            class="text-blue-600 hover:text-blue-700 flex items-center gap-2"
          >
            ← Back
          </button>
        </div>

        <RegistrationForm
          @submit="handleRegistrationComplete"
          @cancel="handleBack"
        />
      </div>

      <!-- Step: Confirmation -->
      <div v-if="step === 'confirm'" class="bg-white dark:bg-gray-800 rounded-lg p-6">
        <h2 class="text-2xl font-bold mb-4">Registration erfolgreich</h2>
        <p>{{ foundCustomer.customerName }}</p>
        <button
          @click="router.push({ name: 'checkout', params: { customerNumber: foundCustomer.customerNumber } })"
          class="mt-6 w-full bg-blue-600 text-white py-2 rounded"
        >
          Zur Bestellung
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Add your styles here */
</style>
```

### Usage in App

```typescript
// router/index.ts
const routes = [
  {
    path: '/register',
    name: 'register',
    component: () => import('@/pages/Register.vue')
  },
  // ... other routes
]
```

---

## Login Page

### Implementation

```vue
<!-- pages/Login.vue -->
<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import { useErpIntegration } from '@/composables/useErpIntegration'

const router = useRouter()
const { login } = useAuth()
const { validateCustomerEmail, customer, isLoading, error, clearCustomer } = useErpIntegration()

// Form state
const email = ref('')
const password = ref('')
const formError = ref<string | null>(null)

// Step tracking
const step = ref<'email' | 'password'>('email')

// Step 1: Verify customer email exists
const handleValidateEmail = async () => {
  formError.value = null
  const result = await validateCustomerEmail(email.value)

  if (result.isValid) {
    // Email exists in ERP
    step.value = 'password'
  } else {
    formError.value = result.message || 'Kunde nicht gefunden'
  }
}

// Step 2: Submit login
const handleSubmitLogin = async () => {
  formError.value = null

  try {
    await login({
      email: email.value,
      password: password.value,
      customerNumber: customer.value?.customerNumber,
    })

    // Redirect to dashboard
    router.push('/dashboard')
  } catch (err: any) {
    formError.value = err.message || 'Login fehlgeschlagen'
  }
}

// Reset form
const handleReset = () => {
  email.value = ''
  password.value = ''
  step.value = 'email'
  formError.value = null
  clearCustomer()
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center py-12 px-4">
    <div class="max-w-md w-full bg-white dark:bg-gray-800 rounded-lg shadow p-6">
      <h1 class="text-2xl font-bold mb-6 text-center">Anmelden</h1>

      <!-- Error Alert -->
      <div v-if="formError" class="mb-4 p-3 bg-red-100 dark:bg-red-900/20 text-red-600 dark:text-red-400 rounded">
        {{ formError }}
      </div>

      <!-- Step 1: Email Validation -->
      <form v-if="step === 'email'" @submit.prevent="handleValidateEmail" class="space-y-4">
        <div>
          <label class="block text-sm font-medium mb-2">E-Mail-Adresse</label>
          <input
            v-model="email"
            type="email"
            required
            placeholder="name@example.com"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md dark:bg-gray-700"
          />
        </div>

        <p v-if="customer" class="text-sm text-green-600 dark:text-green-400">
          ✓ {{ customer.customerName }} gefunden
        </p>

        <button
          type="submit"
          :disabled="isLoading || !email"
          class="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-gray-300 text-white py-2 rounded"
        >
          {{ isLoading ? 'Wird überprüft...' : 'Weiter' }}
        </button>
      </form>

      <!-- Step 2: Password Entry -->
      <form v-if="step === 'password'" @submit.prevent="handleSubmitLogin" class="space-y-4">
        <!-- Show customer info -->
        <div class="p-3 bg-green-50 dark:bg-green-900/20 rounded">
          <p class="text-sm font-medium">
            Willkommen, <strong>{{ customer?.customerName }}</strong>
          </p>
          <p class="text-xs text-gray-600 dark:text-gray-400">{{ email }}</p>
        </div>

        <!-- Password input -->
        <div>
          <label class="block text-sm font-medium mb-2">Passwort</label>
          <input
            v-model="password"
            type="password"
            required
            placeholder="••••••••"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md dark:bg-gray-700"
          />
        </div>

        <!-- Submit buttons -->
        <div class="flex gap-2">
          <button
            type="button"
            @click="handleReset"
            class="flex-1 bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-white py-2 rounded hover:bg-gray-300"
          >
            Zurück
          </button>
          <button
            type="submit"
            class="flex-1 bg-blue-600 hover:bg-blue-700 text-white py-2 rounded"
          >
            Anmelden
          </button>
        </div>
      </form>

      <!-- Signup link -->
      <div class="mt-4 text-center">
        <p class="text-sm">
          Noch kein Konto?
          <router-link to="/register" class="text-blue-600 hover:underline">
            Jetzt registrieren
          </router-link>
        </p>
      </div>
    </div>
  </div>
</template>
```

---

## Checkout Page

### Implementation

```vue
<!-- pages/Checkout.vue -->
<script setup lang="ts">
import { onMounted, reactive } from 'vue'
import { useRoute } from 'vue-router'
import { useErpIntegration } from '@/composables/useErpIntegration'

const route = useRoute()
const { validateCustomerNumber, customer } = useErpIntegration()

const checkoutData = reactive({
  customerNumber: '',
  customerName: '',
  email: '',
  shippingAddress: '',
  billingAddress: '',
  items: [] as any[],
  total: 0,
})

// Pre-fill with customer info if available
onMounted(async () => {
  const customerNumber = route.params.customerNumber as string

  if (customerNumber) {
    // Lookup customer details
    await validateCustomerNumber(customerNumber)

    if (customer.value) {
      checkoutData.customerNumber = customer.value.customerNumber
      checkoutData.customerName = customer.value.customerName
      checkoutData.email = customer.value.email
      checkoutData.shippingAddress = customer.value.shippingAddress || ''
      checkoutData.billingAddress = customer.value.billingAddress || ''
    }
  }
})

const handleSubmitOrder = async () => {
  try {
    const response = await fetch('/api/orders', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(checkoutData)
    })

    const { orderId } = await response.json()
    window.location.href = `/order-confirmation/${orderId}`
  } catch (error) {
    console.error('Order submission failed:', error)
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 py-12">
    <div class="max-w-2xl mx-auto px-4">
      <h1 class="text-3xl font-bold mb-8">Bestellung</h1>

      <!-- Customer Info (Pre-filled if available) -->
      <div v-if="customer" class="bg-white dark:bg-gray-800 rounded-lg p-6 mb-6">
        <h2 class="text-lg font-semibold mb-4">Kundeninformationen</h2>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Name</p>
            <p class="font-semibold">{{ checkoutData.customerName }}</p>
          </div>
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">E-Mail</p>
            <p class="font-semibold">{{ checkoutData.email }}</p>
          </div>
          <div v-if="checkoutData.shippingAddress" class="col-span-2">
            <p class="text-sm text-gray-600 dark:text-gray-400">Lieferadresse</p>
            <p class="font-semibold">{{ checkoutData.shippingAddress }}</p>
          </div>
        </div>
      </div>

      <!-- Order Items -->
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 mb-6">
        <h2 class="text-lg font-semibold mb-4">Bestellpositionen</h2>
        <!-- Your cart items here -->
      </div>

      <!-- Order Summary -->
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 mb-6">
        <div class="flex justify-between mb-4">
          <span>Subtotal:</span>
          <span>€ 99,99</span>
        </div>
        <div class="flex justify-between mb-4">
          <span>Versand:</span>
          <span>€ 5,99</span>
        </div>
        <div class="border-t border-gray-200 dark:border-gray-700 pt-4 flex justify-between font-bold text-lg">
          <span>Gesamt:</span>
          <span>€ 105,98</span>
        </div>
      </div>

      <!-- Submit Order -->
      <button
        @click="handleSubmitOrder"
        class="w-full bg-green-600 hover:bg-green-700 text-white font-bold py-3 rounded-lg"
      >
        Bestellung abschließen
      </button>
    </div>
  </div>
</template>
```

---

## API Setup

### Backend Endpoints

Make sure your backend API has these endpoints:

```
POST /api/auth/erp/validate-email
  Request: { email: string }
  Response: ErpCustomer

POST /api/auth/erp/validate-number
  Request: { customerNumber: string }
  Response: ErpCustomer

POST /api/auth/register
  Request: { email, password, name, ... }
  Response: { customerNumber }

POST /api/auth/login
  Request: { email, password, customerNumber? }
  Response: { token, customerNumber }

POST /api/orders
  Request: { customerNumber, items, ... }
  Response: { orderId }
```

### Environment Configuration

```typescript
// .env.development
VITE_API_URL=http://localhost:8000

// .env.production
VITE_API_URL=https://api.example.com
```

### API Client Setup

```typescript
// composables/useApi.ts
import { ref } from 'vue'

export function useApi() {
  const baseUrl = import.meta.env.VITE_API_URL

  const fetchJson = async (endpoint: string, options: any = {}) => {
    const response = await fetch(`${baseUrl}${endpoint}`, {
      headers: { 'Content-Type': 'application/json' },
      ...options
    })

    if (!response.ok) {
      const error = await response.json()
      throw new Error(error.message || 'API error')
    }

    return response.json()
  }

  return { fetchJson }
}
```

---

## Testing

### Integration Test Example

```typescript
import { mount, flushPromises } from '@vue/test-utils'
import Register from '@/pages/Register.vue'

describe('Register Page Integration', () => {
  it('should show CustomerLookup on load', () => {
    const wrapper = mount(Register)
    expect(wrapper.findComponent({ name: 'CustomerLookup' }).exists()).toBe(true)
  })

  it('should navigate to registration form when register is clicked', async () => {
    const wrapper = mount(Register)
    const component = wrapper.findComponent({ name: 'CustomerLookup' })
    
    await component.vm.$emit('register')
    await flushPromises()

    expect(wrapper.find('.registration-form').exists()).toBe(true)
  })

  it('should navigate to checkout when existing customer proceeds', async () => {
    const mockRouter = {
      push: vi.fn()
    }

    const wrapper = mount(Register, {
      global: {
        stubs: {
          'router-link': true
        },
        mocks: {
          $router: mockRouter
        }
      }
    })

    const component = wrapper.findComponent({ name: 'CustomerLookup' })
    await component.vm.$emit('proceed', 'CUST-001')

    expect(mockRouter.push).toHaveBeenCalledWith(
      expect.objectContaining({
        name: 'checkout',
        params: { customerNumber: 'CUST-001' }
      })
    )
  })
})
```

---

## ✅ Checklist

- [ ] Create `/pages/Register.vue` with CustomerLookup
- [ ] Create `/pages/Login.vue` with email validation
- [ ] Create `/pages/Checkout.vue` with customer pre-fill
- [ ] Setup API endpoints on backend
- [ ] Configure environment variables
- [ ] Test with sample data (CUST-001, etc.)
- [ ] Test dark mode support
- [ ] Test keyboard navigation
- [ ] Test on mobile devices
- [ ] Verify accessibility with screen reader
- [ ] Add integration tests
- [ ] Deploy to production

---

**Status**: ✅ READY TO IMPLEMENT

You now have complete implementation examples for all three major user flows!
