# Frontend Documentation

Guides und Referenzen für die Frontend-Entwicklung (Customer App).

## Inhalte

Dokumentation für die Customer-Facing Application wird hier gesammelt.

### Struktur

```
frontend/
├── src/
│   ├── components/
│   │   ├── catalog/      # Product catalog components
│   │   ├── cart/         # Shopping cart
│   │   ├── checkout/     # Checkout flow
│   │   ├── shared/       # Shared components
│   │   └── layout/       # Layout
│   ├── pages/
│   ├── stores/           # Pinia stores
│   ├── api/              # API services
│   ├── types/
│   ├── router/
│   └── i18n/             # Localization
├── tests/                # Test files
├── docs/ (Sie sind hier) # Documentation
└── package.json
```

## Quick Links

- [Frontend README](../README.md) - Projektübersicht
- [Admin Frontend](../../frontend-admin/README.md)
- [Backend](../../backend/README.md)
- [Main Docs](../../docs/) - Main documentation
- [Feature Implementations](../../docs/features/)
- [Development Guide](../../DEVELOPMENT.md)

## Development

```bash
cd frontend

# Install
npm install

# Dev server
npm run dev

# Tests
npm run test
npm run e2e

# Build
npm run build
```

## Key Features

✅ Product Catalog
✅ Advanced Search (Elasticsearch)
✅ Shopping Cart
✅ Order Management
✅ Multi-language Support
✅ User Authentication

## Testing

```bash
# Unit tests
npm run test

# E2E tests
npm run e2e

# Watch mode
npm run test:watch
```

## Stack

- Vue 3 + TypeScript
- Pinia for state
- Axios for HTTP
- Vitest for testing
- Playwright for E2E
- Tailwind CSS
