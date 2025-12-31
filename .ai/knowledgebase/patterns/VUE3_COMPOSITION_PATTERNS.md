# 🎨 Vue 3 Composition Patterns

**Audience**: Frontend developers  
**Purpose**: Consistent Vue3 component patterns across B2Connect  
**Framework**: Vue.js 3, TypeScript, Pinia, Vite

---

## Quick Start: Component Template

```vue
<template>
  <div class="my-component">
    <h1>{{ title }}</h1>
    <button @click="handleClick">Click me</button>
    <p v-if="isLoading" class="loading">Loading...</p>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// Props
interface Props {
  title: string
  initialCount?: number
}

const props = withDefaults(defineProps<Props>(), {
  initialCount: 0,
})

// Emits
const emit = defineEmits<{
  (e: 'count-updated', count: number): void
}>()

// Reactive state
const count = ref(props.initialCount)
const isLoading = ref(false)

// Computed
const doubleCount = computed(() => count.value * 2)

// Methods
const handleClick = async () => {
  isLoading.value = true
  try {
    // Do work
    count.value++
    emit('count-updated', count.value)
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.my-component {
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 0.5rem;
}

.loading {
  color: #666;
  font-style: italic;
}
</style>
```

---

## Pattern 1: Component with Props & Emits

**When**: Parent passes data to child, child emits events back

**Structure**:
1. Define Props interface
2. Define Emits interface
3. Use `withDefaults` for defaults
4. Emit typed events

**Example**:

```vue
<template>
  <button 
    class="btn" 
    :class="variant"
    @click="handleClick"
    :disabled="disabled"
  >
    {{ label }}
  </button>
</template>

<script setup lang="ts">
interface Props {
  label: string
  variant?: 'primary' | 'secondary' | 'danger'
  disabled?: boolean
  size?: 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  disabled: false,
  size: 'md',
})

const emit = defineEmits<{
  (e: 'click'): void
  (e: 'focus'): void
}>()

const handleClick = () => {
  if (!props.disabled) {
    emit('click')
  }
}
</script>

<style scoped>
.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.primary {
  background: #0066cc;
  color: white;
}

.primary:hover:not(:disabled) {
  background: #0052a3;
}

.secondary {
  background: #f0f0f0;
  color: #333;
}

.sm { padding: 0.25rem 0.75rem; font-size: 0.875rem; }
.lg { padding: 0.75rem 1.5rem; font-size: 1.125rem; }
</style>
```

---

## Pattern 2: State Management with Pinia

**When**: Component needs shared state across multiple components

**Structure**:
1. Define store in `stores/`
2. Use `defineStore` with state, getters, actions
3. Import store in components
4. Access via `store.state` or destructure

**Example Store** (`stores/productStore.ts`):

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Product {
  id: string
  name: string
  price: number
  inStock: boolean
}

export const useProductStore = defineStore('products', () => {
  // State
  const products = ref<Product[]>([])
  const selectedProduct = ref<Product | null>(null)
  const isLoading = ref(false)

  // Getters
  const inStockProducts = computed(() =>
    products.value.filter(p => p.inStock)
  )

  const totalValue = computed(() =>
    inStockProducts.value.reduce((sum, p) => sum + p.price, 0)
  )

  // Actions
  const fetchProducts = async () => {
    isLoading.value = true
    try {
      const response = await fetch('/api/products')
      if (!response.ok) throw new Error('Failed to fetch')
      products.value = await response.json()
    } catch (error) {
      console.error('Error fetching products:', error)
    } finally {
      isLoading.value = false
    }
  }

  const selectProduct = (product: Product) => {
    selectedProduct.value = product
  }

  const addProduct = (product: Product) => {
    products.value.push(product)
  }

  const updateProduct = (id: string, updates: Partial<Product>) => {
    const product = products.value.find(p => p.id === id)
    if (product) {
      Object.assign(product, updates)
    }
  }

  return {
    // State
    products,
    selectedProduct,
    isLoading,
    // Getters
    inStockProducts,
    totalValue,
    // Actions
    fetchProducts,
    selectProduct,
    addProduct,
    updateProduct,
  }
})
```

**Use in Component**:

```vue
<template>
  <div>
    <button @click="store.fetchProducts" :disabled="store.isLoading">
      {{ store.isLoading ? 'Loading...' : 'Refresh' }}
    </button>
    
    <div class="product-list">
      <div
        v-for="product in store.inStockProducts"
        :key="product.id"
        class="product-card"
        @click="store.selectProduct(product)"
      >
        <h3>{{ product.name }}</h3>
        <p>${{ product.price }}</p>
      </div>
    </div>
    
    <div v-if="store.selectedProduct" class="details">
      <h2>{{ store.selectedProduct.name }}</h2>
      <p>Price: ${{ store.selectedProduct.price }}</p>
    </div>
    
    <p class="total">Total Value: ${{ store.totalValue }}</p>
  </div>
</template>

<script setup lang="ts">
import { useProductStore } from '@/stores/productStore'

const store = useProductStore()
</script>
```

---

## Pattern 3: Composables (Reusable Logic)

**When**: Multiple components need same logic

**Structure**:
1. Extract logic into `composables/useXxx.ts`
2. Return reactive state and methods
3. Import in components

**Example Composable** (`composables/useFetch.ts`):

```typescript
import { ref, computed, Ref } from 'vue'

export interface UseFetchOptions {
  immediate?: boolean
}

export function useFetch<T>(
  url: string,
  options: UseFetchOptions = {}
) {
  const data = ref<T | null>(null)
  const isLoading = ref(false)
  const error = ref<Error | null>(null)

  const fetch = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch(url)
      if (!response.ok) throw new Error(`HTTP ${response.status}`)
      data.value = await response.json()
    } catch (err) {
      error.value = err instanceof Error ? err : new Error(String(err))
    } finally {
      isLoading.value = false
    }
  }

  const refetch = () => fetch()

  if (options.immediate !== false) {
    fetch()
  }

  return {
    data: computed(() => data.value),
    isLoading: computed(() => isLoading.value),
    error: computed(() => error.value),
    refetch,
  }
}

// Usage in component
export function useProducts() {
  return useFetch<Product[]>('/api/products')
}
```

**Use in Component**:

```vue
<template>
  <div>
    <p v-if="isLoading">Loading products...</p>
    <p v-else-if="error">Error: {{ error.message }}</p>
    <div v-else class="products">
      <div v-for="product in data" :key="product.id">
        {{ product.name }}
      </div>
    </div>
    <button @click="refetch">Reload</button>
  </div>
</template>

<script setup lang="ts">
import { useProducts } from '@/composables/useProducts'

const { data, isLoading, error, refetch } = useProducts()
</script>
```

---

## Pattern 4: Forms with Validation

**When**: Component has user input with validation

**Structure**:
1. Use `ref` for form data
2. Define validation rules
3. Compute validation errors
4. Disable submit until valid

**Example**:

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <div class="form-group">
      <label for="email">Email</label>
      <input
        id="email"
        v-model="form.email"
        type="email"
        placeholder="user@example.com"
        @blur="validateField('email')"
      />
      <p v-if="errors.email" class="error">{{ errors.email }}</p>
    </div>

    <div class="form-group">
      <label for="password">Password</label>
      <input
        id="password"
        v-model="form.password"
        type="password"
        placeholder="••••••••"
        @blur="validateField('password')"
      />
      <p v-if="errors.password" class="error">{{ errors.password }}</p>
    </div>

    <div class="form-group">
      <label for="remember">
        <input id="remember" v-model="form.remember" type="checkbox" />
        Remember me
      </label>
    </div>

    <button type="submit" :disabled="!isFormValid">
      {{ isSubmitting ? 'Signing in...' : 'Sign In' }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'

interface LoginForm {
  email: string
  password: string
  remember: boolean
}

const form = reactive<LoginForm>({
  email: '',
  password: '',
  remember: false,
})

const errors = reactive<Partial<Record<keyof LoginForm, string>>>({})
const isSubmitting = ref(false)

const isFormValid = computed(() => {
  return (
    form.email &&
    form.password &&
    !errors.email &&
    !errors.password
  )
})

const validateField = (field: keyof LoginForm) => {
  errors[field] = ''

  if (field === 'email') {
    if (!form.email) {
      errors.email = 'Email is required'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
      errors.email = 'Invalid email format'
    }
  }

  if (field === 'password') {
    if (!form.password) {
      errors.password = 'Password is required'
    } else if (form.password.length < 8) {
      errors.password = 'Password must be at least 8 characters'
    }
  }
}

const handleSubmit = async () => {
  // Validate all fields
  Object.keys(form).forEach(field => {
    validateField(field as keyof LoginForm)
  })

  if (!isFormValid.value) return

  isSubmitting.value = true
  try {
    const response = await fetch('/api/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form),
    })
    if (!response.ok) throw new Error('Login failed')
    // Redirect or emit event
  } catch (error) {
    console.error('Login error:', error)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.form-group {
  margin-bottom: 1.5rem;
  display: flex;
  flex-direction: column;
}

label {
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #333;
}

input[type="email"],
input[type="password"] {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 0.25rem;
  font-size: 1rem;
}

input[type="email"]:focus,
input[type="password"]:focus {
  outline: none;
  border-color: #0066cc;
  box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
}

.error {
  color: #d32f2f;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

button {
  padding: 0.75rem 1.5rem;
  background: #0066cc;
  color: white;
  border: none;
  border-radius: 0.25rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
}

button:hover:not(:disabled) {
  background: #0052a3;
}

button:disabled {
  background: #ccc;
  cursor: not-allowed;
}
</style>
```

---

## Pattern 5: Async Component Rendering

**When**: Component loads data on mount

**Structure**:
1. Use `onMounted` to fetch
2. Show loading state
3. Show error state
4. Show data when ready

**Example**:

```vue
<template>
  <div>
    <div v-if="isLoading" class="spinner">Loading...</div>
    <div v-else-if="error" class="error">
      <p>{{ error }}</p>
      <button @click="refetch">Try Again</button>
    </div>
    <div v-else class="content">
      <h1>{{ product.name }}</h1>
      <p class="price">${{ product.price }}</p>
      <p>{{ product.description }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const product = ref<any>(null)
const isLoading = ref(false)
const error = ref<string | null>(null)

const loadProduct = async () => {
  isLoading.value = true
  error.value = null
  
  try {
    const response = await fetch(`/api/products/${route.params.id}`)
    if (!response.ok) throw new Error('Product not found')
    product.value = await response.json()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unknown error'
  } finally {
    isLoading.value = false
  }
}

const refetch = () => loadProduct()

onMounted(() => loadProduct())
</script>

<style scoped>
.spinner {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
  font-size: 1.125rem;
  color: #666;
}

.error {
  padding: 1.5rem;
  background: #ffebee;
  border: 1px solid #ef5350;
  border-radius: 0.25rem;
}

.error button {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background: #ef5350;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
}

.price {
  font-size: 1.5rem;
  font-weight: bold;
  color: #0066cc;
}
</style>
```

---

## Component Structure Best Practices

```vue
<!-- Good structure -->
<template>
  <!-- JSX/template here -->
</template>

<script setup lang="ts">
// 1. Imports
import { ref, computed, onMounted } from 'vue'
import { useStore } from '@/stores/store'

// 2. Types/Interfaces
interface Props {
  ...
}

// 3. Props & Emits
const props = withDefaults(defineProps<Props>(), {
  ...
})
const emit = defineEmits<{...}>()

// 4. Stores
const store = useStore()

// 5. Reactive state
const localState = ref(false)

// 6. Computed properties
const derived = computed(() => ...)

// 7. Methods
const handleClick = () => {...}

// 8. Lifecycle
onMounted(() => {...})
</script>

<style scoped>
/* Scoped styles only */
</style>
```

---

## Common Mistakes to Avoid

| ❌ Don't | ✅ Do Instead |
|----------|--------------|
| Use `v-model` without proper handler | Use `@input` with `:value` or `v-model` with proper setup |
| Mutate props directly | Use `@update:prop` pattern for prop mutations |
| Use global styles in components | Use `<style scoped>` always |
| Mix reactive and non-reactive data | Use `ref` or `reactive` consistently |
| Fetch data in setup (non-async) | Use `onMounted` or composables |
| Create new objects in computed | Memoize or use `computed` carefully |
| Forget to unsubscribe in lifecycle | Use composables that handle cleanup |

---

## Checklist: Component Review

- [ ] Props are typed with interfaces?
- [ ] Emits are typed and documented?
- [ ] All reactive data uses `ref` or `reactive`?
- [ ] Async operations in `onMounted` or composables?
- [ ] Error states handled?
- [ ] Loading states shown?
- [ ] Styles scoped with `<style scoped>`?
- [ ] No hardcoded values (use props or config)?
- [ ] Accessibility attributes added (aria-*, labels)?
- [ ] Component tested with different props?

---

## References

- [Wolverine Pattern Reference](../architecture/WOLVERINE_PATTERN_REFERENCE.md)
- [FEATURE_IMPLEMENTATION_PATTERNS](FEATURE_IMPLEMENTATION_PATTERNS.md)
- [Vue 3 Docs](https://vuejs.org/)
- [Pinia Docs](https://pinia.vuejs.org/)

---

*Updated: 30. Dezember 2025*  
*Framework: Vue.js 3 with TypeScript*
