# Admin Frontend Documentation

Guides und Referenzen für die Admin-Frontend-Entwicklung.

## Inhalte

### Implementation & Status

**[Catalog Integration](CATALOG_INTEGRATION.md)**
- Catalog Feature in Admin Frontend
- Component integration
- Store management
- API integration

**[Implementation Checklist](IMPLEMENTATION_CHECKLIST.md)**
- Feature completeness checklist
- Component status
- Test coverage

**[Final Status](FINAL_STATUS.md)**
- Overall project status
- Completed features
- Performance metrics

### Testing & QA

**[Testing Guide](TESTING_GUIDE.md)**
- Unit test setup
- E2E test setup
- Test patterns
- Running tests

**[Test Status](TEST_STATUS.md)**
- Test results summary
- Coverage metrics

**[Test Execution Complete](TEST_EXECUTION_COMPLETE.md)**
- Final test execution report
- All results

**[Test Implementation Summary](TEST_IMPLEMENTATION_SUMMARY.md)**
- Test implementation details
- Patterns used

## Quick Links

- [Frontend-Admin README](../README.md) - Projektübersicht
- [Main Docs](../../docs/) - Main documentation
- [Feature Implementations](../../docs/features/)
- [Development Guide](../../DEVELOPMENT.md)

## Development

```bash
cd frontend-admin

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

## Structure

```
frontend-admin/
├── src/
│   ├── components/
│   │   ├── catalog/      # Catalog management
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

## Key Features

✅ Product Management
✅ Category Management
✅ Type-safe TypeScript
✅ Comprehensive Testing
✅ Multi-language Support
✅ Role-based Access

## Testing

```bash
# Unit tests
npm run test

# E2E tests
npm run e2e

# Watch
npm run test:watch

# Coverage
npm run test:coverage
```

## Stack

- Vue 3 + TypeScript
- Pinia for state
- Axios for HTTP
- Vitest for testing
- Playwright for E2E
- Tailwind CSS
