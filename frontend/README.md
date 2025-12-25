# B2Connect Frontend (Customer App)

Vue 3 + TypeScript customer-facing application for B2Connect marketplace.

## Quick Start

```bash
cd frontend
npm install
npm run dev
```

Öffnet sich unter http://localhost:5173

## Features

- 🛍️ Product Catalog Browser
- 🔍 Advanced Search with Elasticsearch
- 🛒 Shopping Cart
- 📦 Order Management
- 🔐 User Authentication
- 🌍 Multi-language Support
- 💳 Payment Integration
- ⭐ Product Reviews & Ratings

## Project Structure

```
frontend/
├── src/
│   ├── components/         # Vue components
│   │   ├── catalog/       # Product & category components
│   │   ├── cart/          # Shopping cart
│   │   ├── checkout/      # Checkout flow
│   │   ├── shared/        # Reusable components
│   │   └── layout/        # Layout components
│   ├── pages/             # Page components
│   ├── stores/            # Pinia state management
│   ├── api/               # API services
│   ├── types/             # TypeScript types
│   ├── i18n/              # Localization
│   ├── router/            # Vue Router
│   ├── styles/            # Global styles
│   └── main.ts            # Entry point
├── tests/                 # Test files
├── package.json
├── vite.config.ts
├── vitest.config.ts
├── tsconfig.json
└── playwright.config.ts
```

## Development

### Installation
```bash
npm install
```

### Development Server
```bash
npm run dev
```

### Build
```bash
npm run build
```

### Tests
```bash
# Unit tests
npm run test

# E2E tests
npm run e2e

# Watch mode
npm run test:watch
```

### Linting
```bash
npm run lint
npm run format
```

## Stack

- **Vue 3** - Progressive JS framework
- **TypeScript** - Type safety
- **Pinia** - State management
- **Vue Router** - Routing
- **Tailwind CSS** - Styling
- **Axios** - HTTP client
- **Vitest** - Unit testing
- **Playwright** - E2E testing

## API Integration

All API calls routed through [src/api/](src/api/):

```typescript
import { catalogApi } from '@/api/catalogApi'

// Browse products
await catalogApi.getProducts()
await catalogApi.getProductDetails(id)

// Search with Elasticsearch
await catalogApi.searchProducts(query)
```

## State Management

Pinia stores in [src/stores/](src/stores/):

```typescript
import { useCatalogStore } from '@/stores/catalogStore'
import { useCartStore } from '@/stores/cartStore'

const catalogStore = useCatalogStore()
const cartStore = useCartStore()
```

## Localization

Multi-language support in [src/i18n/](src/i18n/):

```typescript
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
t('catalog.products')
```

## Performance

- 🚀 Code splitting per route
- 📦 Tree-shaking enabled
- 🗜️ Gzip compression
- 🖼️ Image optimization
- 💾 Caching strategy

## Documentation

- [Admin Frontend](../frontend-admin/README.md)
- [Backend](../backend/README.md)
- [Main Docs](../docs/)
- [Development Guide](../DEVELOPMENT.md)
- [Getting Started](../GETTING_STARTED.md)

## Support

See [../docs/](../docs/) für detailed documentation.
