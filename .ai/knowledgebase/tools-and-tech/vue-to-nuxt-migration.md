---
docid: KB-068
title: Vue to Nuxt Migration Guide & Best Practices
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Vue to Nuxt Migration Guide & Best Practices

**DocID**: `KB-068`  
**Category**: Tools & Tech  
**Last Updated**: 8. Januar 2026  
**Status**: Active

---

## Overview

This comprehensive guide covers migrating from standalone Vue.js 3 applications to Nuxt 4, including best practices, architecture patterns, and key differences between the frameworks.

---

## Nuxt 4 Key Features (Released July 2025)

### New Project Structure (`app/` directory)

Nuxt 4 introduces a cleaner project organization with application code in an `app/` directory:

```
my-nuxt-app/
├─ app/                    # Application code (NEW in Nuxt 4)
│  ├─ assets/
│  ├─ components/
│  ├─ composables/
│  ├─ layouts/
│  ├─ middleware/
│  ├─ pages/
│  ├─ plugins/
│  ├─ utils/
│  ├─ app.vue
│  ├─ app.config.ts
│  └─ error.vue
├─ content/
├─ public/
├─ shared/                 # Shared utilities
├─ server/                 # Server-side code
└─ nuxt.config.ts
```

> **Note**: Nuxt will auto-detect existing structures and continue working without migration.

### Key Improvements in Nuxt 4

| Feature | Description |
|---------|-------------|
| **Smarter Data Fetching** | `useAsyncData` and `useFetch` share data automatically, auto-cleanup on unmount |
| **Better TypeScript** | Separate TS projects for app, server, shared, and builder code |
| **Faster CLI** | Native file watching, socket-based communication, V8 compile cache |
| **Single tsconfig.json** | Only one TypeScript config needed at project root |

---

## Migration Steps: Vue to Nuxt

### 1. Project Initialization

```bash
# Create new Nuxt 4 project
npx nuxi@latest init my-nuxt-app

# Or upgrade existing Nuxt 3
npx nuxt upgrade --dedupe

# Optional: Run automated migration
npx codemod@latest nuxt/4/migration-recipe
```

### 2. Directory Structure Mapping

| Vue.js | Nuxt | Notes |
|--------|------|-------|
| `src/components/` | `app/components/` | Auto-imported by convention |
| `src/views/` | `app/pages/` | File-based routing |
| `src/router/` | **Not needed** | File-based routing auto-generated |
| `src/composables/` | `app/composables/` | Auto-imported |
| `src/utils/` | `app/utils/` | Auto-imported |
| `src/store/` | `app/stores/` | Pinia stores with Nuxt module |
| `src/assets/` | `app/assets/` | Processed by build tool |
| `public/` | `public/` | Static assets (unchanged) |
| `src/App.vue` | `app/app.vue` | Entry component |
| `src/main.ts` | **Not needed** | Nuxt handles bootstrapping |

### 3. Router Migration

**Vue Router Configuration → Nuxt File-based Routing**

```typescript
// ❌ Vue Router (manual configuration)
// src/router/index.ts
const routes = [
  { path: '/', name: 'home', component: () => import('@/views/Home.vue') },
  { path: '/about', name: 'about', component: () => import('@/views/About.vue') },
  { path: '/users/:id', name: 'user', component: () => import('@/views/User.vue') },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/NotFound.vue') }
]
```

```
// ✅ Nuxt File-based Routing (automatic)
app/pages/
├── index.vue              # → /
├── about.vue              # → /about
├── users/
│   └── [id].vue           # → /users/:id (dynamic route)
└── [...slug].vue          # → catch-all route
```

### 4. Component Changes

**Vue Component → Nuxt Component**

```vue
<!-- ❌ Vue.js Component -->
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const data = ref(null)

onMounted(async () => {
  data.value = await fetch('/api/data').then(r => r.json())
})

function navigate() {
  router.push('/about')
}
</script>
```

```vue
<!-- ✅ Nuxt Component (auto-imports, SSR-friendly) -->
<script setup lang="ts">
// No imports needed - ref, useRouter, useFetch are auto-imported

const router = useRouter()
const { data } = await useFetch('/api/data')  // SSR-safe

function navigate() {
  await navigateTo('/about')  // Nuxt navigation helper
}
</script>
```

### 5. Data Fetching Migration

#### Replace `fetch`/`axios` with Nuxt Composables

```typescript
// ❌ Vue.js (causes hydration issues in SSR)
const data = ref(null)
onMounted(async () => {
  data.value = await fetch('/api/users').then(r => r.json())
})
```

```typescript
// ✅ Nuxt - SSR-safe with automatic deduplication
const { data, status, error, refresh } = await useFetch('/api/users')

// With options
const { data: users } = await useFetch('/api/users', {
  pick: ['id', 'name'],        // Minimize payload
  lazy: true,                   // Non-blocking navigation
  server: false,                // Client-only fetch
  watch: [userId],              // Re-fetch on change
  transform: (data) => data.map(u => ({ ...u, fullName: `${u.first} ${u.last}` }))
})
```

#### `useAsyncData` for Complex Fetching

```typescript
// Multiple parallel requests
const { data: discounts } = await useAsyncData('cart-discount', async () => {
  const [coupons, offers] = await Promise.all([
    $fetch('/cart/coupons'),
    $fetch('/cart/offers')
  ])
  return { coupons, offers }
})
```

---

## Key Nuxt Concepts

### Auto-Imports

Nuxt automatically imports Vue APIs, composables, and utilities:

```typescript
// ❌ Not needed in Nuxt
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

// ✅ Just use them directly
const count = ref(0)
const route = useRoute()
const router = useRouter()
```

**Auto-imported from:**
- Vue Reactivity: `ref`, `reactive`, `computed`, `watch`, etc.
- Vue Router: `useRoute`, `useRouter`
- Nuxt Composables: `useFetch`, `useAsyncData`, `useState`, `useCookie`, `useHead`, etc.
- Custom composables from `app/composables/`
- Utilities from `app/utils/`

### State Management

#### Using `useState` (Built-in)

```typescript
// app/composables/states.ts
export const useCounter = () => useState<number>('counter', () => 0)
export const useUser = () => useState<User | null>('user', () => null)

// In components (shared state)
const counter = useCounter()
counter.value++
```

#### Using Pinia (Recommended for complex apps)

```bash
npx nuxt module add pinia
```

```typescript
// app/stores/user.ts
export const useUserStore = defineStore('user', {
  state: () => ({
    name: '',
    isLoggedIn: false
  }),
  actions: {
    async login(credentials) {
      const user = await $fetch('/api/login', { method: 'POST', body: credentials })
      this.name = user.name
      this.isLoggedIn = true
    }
  }
})
```

### Page Metadata (`definePageMeta`)

```vue
<script setup lang="ts">
definePageMeta({
  title: 'Dashboard',
  layout: 'admin',                    // Use specific layout
  middleware: ['auth'],               // Route middleware
  keepalive: true,                    // Preserve state across navigation
  pageTransition: { name: 'fade' },   // Page transitions
  validate: async (route) => {        // Route validation
    return /^\d+$/.test(route.params.id as string)
  }
})
</script>
```

### SEO & Head Management

```vue
<script setup lang="ts">
// Reactive head management
const title = ref('My Page')

useHead({
  title: computed(() => title.value),
  meta: [
    { name: 'description', content: 'Page description' },
    { property: 'og:title', content: title }
  ],
  htmlAttrs: { lang: 'en' },
  bodyAttrs: { class: 'dark-mode' }
})

// Or use useSeoMeta for simpler SEO
useSeoMeta({
  title: 'My Page',
  ogTitle: 'My Page',
  description: 'Page description',
  ogDescription: 'Page description',
  ogImage: 'https://example.com/image.png'
})
</script>
```

### Cookies (SSR-friendly)

```typescript
// SSR-safe cookie management
const token = useCookie('auth-token', {
  maxAge: 60 * 60 * 24 * 7,  // 1 week
  secure: true,
  sameSite: 'strict'
})

// Set cookie
token.value = 'new-token'

// Clear cookie
token.value = null
```

### Navigation

```typescript
// Programmatic navigation
await navigateTo('/dashboard')

// With options
await navigateTo({
  path: '/search',
  query: { q: 'nuxt' }
}, {
  replace: true,      // Replace history entry
  redirectCode: 301   // Server redirect code
})

// External navigation
await navigateTo('https://nuxt.com', { external: true })
```

---

## Component Patterns

### Lazy Loading Components

```vue
<template>
  <div>
    <!-- Lazy-load with "Lazy" prefix -->
    <LazyHeavyComponent v-if="showComponent" />
    
    <!-- Delayed hydration strategies -->
    <LazyMyComponent hydrate-on-visible />
    <LazyMyComponent hydrate-on-idle />
    <LazyMyComponent :hydrate-after="2000" />
    <LazyMyComponent hydrate-never />
  </div>
</template>
```

### Client-Only & Server-Only Components

```
components/
├── Comments.client.vue    # Only rendered on client
├── Analytics.server.vue   # Only rendered on server
└── Header.vue             # Universal
```

```vue
<!-- Use <ClientOnly> wrapper -->
<template>
  <ClientOnly fallback-tag="span" fallback="Loading...">
    <BrowserOnlyWidget />
  </ClientOnly>
</template>
```

### Dynamic Components

```vue
<script setup lang="ts">
import { SomeComponent } from '#components'

const MyButton = resolveComponent('MyButton')
</script>

<template>
  <component :is="clickable ? MyButton : 'div'" />
  <component :is="SomeComponent" />
</template>
```

---

## Configuration

### `nuxt.config.ts`

```typescript
export default defineNuxtConfig({
  // Environment-specific overrides
  $production: {
    routeRules: {
      '/**': { isr: true }
    }
  },
  $development: {
    devtools: { enabled: true }
  },

  // Runtime config (use env vars)
  runtimeConfig: {
    apiSecret: '',  // Server-only
    public: {
      apiBase: '/api'  // Client-accessible
    }
  },

  // App config (build-time)
  app: {
    head: {
      title: 'My App',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ]
    }
  },

  // Modules
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@nuxtjs/i18n'
  ],

  // Vue configuration
  vue: {
    propsDestructure: true
  },

  // Vite configuration
  vite: {
    vue: {
      customElement: true
    }
  }
})
```

### `app.config.ts` (Public build-time config)

```typescript
export default defineAppConfig({
  title: 'Hello Nuxt',
  theme: {
    dark: true,
    colors: {
      primary: '#ff0000'
    }
  }
})
```

---

## Server-Side Features

### API Routes

```typescript
// server/api/users.ts
export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const users = await fetchUsers(query)
  return users
})

// server/api/users/[id].ts (dynamic route)
export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  return await getUserById(id)
})

// POST handler
// server/api/users.post.ts
export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  return await createUser(body)
})
```

### Server Middleware

```typescript
// server/middleware/auth.ts
export default defineEventHandler((event) => {
  const token = getCookie(event, 'auth-token')
  if (!token && event.path.startsWith('/api/protected')) {
    throw createError({ statusCode: 401, message: 'Unauthorized' })
  }
})
```

---

## Best Practices

### 1. Data Fetching
- Use `useFetch` for simple API calls
- Use `useAsyncData` for complex/multiple requests
- Use `$fetch` for client-only interactions (form submissions, etc.)
- Always use `pick` or `transform` to minimize payload

### 2. State Management
- Use `useState` for simple shared state
- Use Pinia for complex state with actions and getters
- Never define `const state = ref()` outside `<script setup>`

### 3. Performance
- Use lazy components (`Lazy` prefix) for below-fold content
- Use hydration strategies for non-critical components
- Leverage ISR (Incremental Static Regeneration) for static pages
- Use `pick` option in `useFetch` to minimize data transfer

### 4. TypeScript
- Enable strict mode in `tsconfig.json`
- Use `definePageMeta` for type-safe page metadata
- Leverage auto-generated types from `.nuxt/`

### 5. SEO
- Use `useHead` or `useSeoMeta` for meta tags
- Define `title` and `meta` in page components
- Use `nuxt/image` module for optimized images

---

## Common Migration Issues

### 1. Hydration Mismatches
```typescript
// ❌ Causes hydration issues
const date = new Date().toLocaleDateString()

// ✅ Use client-only rendering
const date = ref('')
onMounted(() => {
  date.value = new Date().toLocaleDateString()
})
```

### 2. Composable Context Errors
```typescript
// ❌ Called outside Nuxt context
const config = useRuntimeConfig()  // Error!
export const myHelper = () => { /* ... */ }

// ✅ Call inside composable
export const myHelper = () => {
  const config = useRuntimeConfig()  // Works!
  // ...
}
```

### 3. Import Errors
```typescript
// ❌ Don't mix imports
import { useFetch } from '@vueuse/core'  // Wrong useFetch!

// ✅ Use Nuxt's auto-imported version or explicit import
import { useFetch } from '#app'
```

---

## Migration Checklist

- [ ] Initialize Nuxt project or upgrade existing
- [ ] Move components to `app/components/`
- [ ] Convert views to `app/pages/`
- [ ] Remove Vue Router configuration
- [ ] Remove manual imports (use auto-imports)
- [ ] Replace `fetch`/`axios` with `useFetch`/`useAsyncData`
- [ ] Migrate Vuex to Pinia with `@pinia/nuxt`
- [ ] Convert middleware to Nuxt middleware
- [ ] Update head management to `useHead`
- [ ] Configure `nuxt.config.ts`
- [ ] Test SSR rendering
- [ ] Verify hydration (no mismatches)
- [ ] Test all routes and navigation
- [ ] Validate SEO meta tags

---

## References

- [Nuxt 4 Announcement](https://nuxt.com/blog/v4)
- [Nuxt Documentation](https://nuxt.com/docs)
- [Nuxt Upgrade Guide](https://nuxt.com/docs/4.x/getting-started/upgrade)
- [Nuxt Migration Overview](https://nuxt.com/docs/migration/overview)
- [Nuxt Configuration](https://nuxt.com/docs/4.x/getting-started/configuration)
- [Data Fetching](https://nuxt.com/docs/4.x/getting-started/data-fetching)
- [State Management](https://nuxt.com/docs/4.x/getting-started/state-management)
- [Auto-imports](https://nuxt.com/docs/4.x/guide/concepts/auto-imports)

---

## Related Knowledge Base Articles

- [KB-065] Nuxt 4 Monorepo Configuration
- [KB-007] Vue.js 3
- [KB-008] Pinia State
- [KB-009] Vite Tooling

---

**Maintained by**: @Frontend  
**Last Review**: 8. Januar 2026
