# Admin Frontend - B2X

Professional admin dashboard for B2X platform built with Vue 3, TypeScript, and Vite.

**Status**: ✅ Production Ready | **Coverage**: 91% | **Tests**: 230+

📚 **[Documentation](docs/)** | 🎨 **[Theme System](THEME_IMPLEMENTATION.md)** | 🔧 [Main Docs](../docs/) | 📖 [Getting Started](../GETTING_STARTED.md)

## 🚀 Quick Start

### Prerequisites

- Node.js 18+
- npm 9+

### Installation & Running

```bash
# Install dependencies
npm install

# Start development server
npm run dev
```

Server runs on [http://localhost:5174](http://localhost:5174)  
API proxy configured to `http://localhost:9000`

## 🎨 Light/Dark Theme

The admin dashboard features a full light/dark theme system:

- **Auto Mode**: Follows system preferences (default)
- **Light Mode**: Clean, bright interface
- **Dark Mode**: Easy on the eyes, perfect for low-light environments
- **Persistent**: Theme choice is saved in browser

Located in the sidebar settings section. See [THEME_IMPLEMENTATION.md](THEME_IMPLEMENTATION.md) for developer documentation.

## 📋 Project Structure

```
src/
├── components/
│   ├── layout/MainLayout.vue         # Main application layout
│   ├── auth/LoginForm.vue            # Login form
│   └── common/                       # Reusable components
├── views/
│   ├── Dashboard.vue                 # Admin dashboard
│   ├── auth/Login.vue               # Login page
│   ├── cms/                         # CMS modules
│   ├── shop/                        # Shop modules
│   └── jobs/                        # Jobs modules
├── router/index.ts                   # Routes + guards
├── stores/                           # Pinia state (auth, cms, shop, jobs)
├── services/
│   ├── client.ts                    # HTTP client
│   └── api/                         # API services
├── types/                           # TypeScript interfaces
├── composables/                     # Reusable logic
├── middleware/                      # Route guards
└── main.ts                          # Entry point

tests/
├── setup.ts                         # Global mocks
├── unit/                           # Unit tests (Vitest)
│   ├── stores/
│   ├── services/
│   ├── components/
│   ├── router/
│   └── utils/
└── e2e/                            # E2E tests (Playwright)
    ├── auth.spec.ts
    ├── cms.spec.ts
    ├── shop.spec.ts
    └── jobs.spec.ts
```

## 🎯 Core Features

### ✅ Authentication Module

- Email/password login with "Remember Me"
- Multi-factor authentication (MFA)
- JWT token management
- Profile management
- Session handling
- Automatic logout on expiration

### ✅ Content Management System (CMS)

- Page CRUD operations
- Template management
- Media library with upload
- Block-based content editor
- SEO optimization
- Publishing workflow (draft/published)
- Bulk operations

### ✅ Shop Management

- Product catalog CRUD
- Category organization
- Pricing rules and discounts
- Stock tracking
- Product attributes
- Bulk import/export
- Image management

### ✅ Jobs & Scheduling

- Real-time job queue monitoring
- Progress tracking & visual indicators
- Retry failed jobs
- Cancel running jobs
- Job history and logs
- Cron-based scheduling
- Job metrics & statistics

## 🛠️ Technology Stack

| Category      | Technology                   |
| ------------- | ---------------------------- |
| **Framework** | Vue 3.5 + TypeScript 5.9     |
| **Build**     | Vite 7.2                     |
| **State**     | Pinia 2.1                    |
| **HTTP**      | Axios 1.6                    |
| **Routing**   | Vue Router 4.3               |
| **Styling**   | Tailwind CSS                 |
| **Testing**   | Vitest 1.0 + Playwright 1.40 |

## 📦 Available Scripts

```bash
# Development
npm run dev              # Start dev server (port 5174)
npm run dev:telemetry   # Start with OpenTelemetry tracing enabled
npm run type-check      # TypeScript checking

# Testing
npm run test            # Run unit tests
npm run test:watch     # Watch mode
npm run test:ui        # Vitest UI
npm run e2e            # Run E2E tests
npm run e2e:ui         # Playwright UI
npm run test:coverage  # Coverage report

# Production
npm run build          # Build for production
npm run preview        # Preview build
npm run lint           # ESLint
npm run format         # Prettier
```

## 📊 OpenTelemetry Integration

The admin dashboard supports distributed tracing and metrics via OpenTelemetry, integrated with the Aspire Dashboard.

### Quick Start

```bash
# Run with telemetry enabled
npm run dev:telemetry
```

### Configuration

| Variable                      | Default                 | Description                          |
| ----------------------------- | ----------------------- | ------------------------------------ |
| `ENABLE_TELEMETRY`            | `false`                 | Enable OpenTelemetry instrumentation |
| `OTEL_SERVICE_NAME`           | `frontend-admin`        | Service name for traces              |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | `http://localhost:4318` | OTLP collector endpoint              |

### Viewing Traces

1. Start Aspire: `cd AppHost && dotnet run`
2. Open Dashboard: http://localhost:15500
3. Start admin with telemetry: `npm run dev:telemetry`
4. Navigate Traces tab to see spans

## 🧪 Testing

### Test Coverage

- **Unit Tests**: 130+ tests for stores, services, utilities
- **Component Tests**: 40+ tests for UI components
- **E2E Tests**: 60+ user workflow tests
- **Total Coverage**: 91% code coverage
- **Test Files**: 13 comprehensive test suites

### Running Tests

```bash
# All tests
npm run test

# Specific test file
npm run test -- auth.spec.ts

# Watch mode
npm run test -- --watch

# Coverage report
npm run test:coverage

# E2E tests
npm run e2e

# E2E with UI
npm run e2e:ui
```

### Test Structure

- Unit tests in `tests/unit/` (Vitest)
- Component tests in `tests/unit/components/` (Vue Test Utils)
- E2E tests in `tests/e2e/` (Playwright)
- Global setup in `tests/setup.ts`
- Configuration in `vitest.config.ts`

## 📊 API Integration

### Base URL

```
Development: http://localhost:9000/api
Production: https://api.B2X.com/api
```

### Request Format

```
Headers:
  Authorization: Bearer {token}
  X-Tenant-Id: {tenantId}
  Content-Type: application/json
```

### Response Format

```typescript
{
  success: boolean
  data?: any
  error?: { code: string; message: string }
  pagination?: { page, pageSize, total, totalPages }
}
```

## 🔐 Security Features

- **Authentication**: JWT with auto-refresh
- **Authorization**: RBAC + permission-based access
- **Route Guards**: Protected routes with role/permission checks
- **API Security**: Tenant isolation headers, CSRF ready
- **Data Protection**: LocalStorage cleanup on logout

## 📱 Responsive Design

- **Mobile**: < 640px
- **Tablet**: 640px - 1024px
- **Desktop**: > 1024px
- Touch-friendly interface
- Adaptive layouts
- Mobile-first approach

## ♿ Accessibility

- WCAG 2.1 Level AA compliant
- ARIA labels and roles
- Semantic HTML
- Keyboard navigation
- Screen reader support
- Focus management

## 📚 Documentation

- [Testing Guide](./TESTING_GUIDE.md) - Comprehensive testing documentation
- [Test Implementation Summary](./TEST_IMPLEMENTATION_SUMMARY.md) - Overview of test structure
- [Type Definitions](./src/types/) - TypeScript interface documentation

## 🐛 Troubleshooting

**Port 5174 in use?**

```bash
npm run dev -- --port 5175
```

**API connection failed?**

- Check backend running on port 9000
- Verify VITE_API_URL in .env
- Check CORS configuration

**Tests failing?**

```bash
rm -rf node_modules package-lock.json
npm install
npm run test
```

## 🤝 Contributing

- Write tests for new features
- Maintain 70%+ code coverage
- Follow TypeScript strict mode
- Use meaningful commit messages
- Update documentation

## 📄 License

Proprietary - B2X Platform

- `npm run preview` - Preview production build
- `npm run test` - Run unit tests
- `npm run test:ui` - Run tests with UI
- `npm run e2e` - Run E2E tests
- `npm run lint` - Lint and fix code
- `npm run format` - Format code

## Configuration

Copy `.env.example` to `.env.local` and update the API URLs:

```
VITE_ADMIN_API_URL=http://localhost:9000/api/admin
VITE_ADMIN_WS_URL=ws://localhost:9000
```

## Technologies

- **Framework**: Vue 3
- **Language**: TypeScript
- **Build Tool**: Vite
- **State Management**: Pinia
- **Router**: Vue Router
- **HTTP Client**: Axios
- **Testing**: Vitest, Playwright
- **Styling**: Tailwind CSS

## Documentation

See `ADMIN_FRONTEND_SPECIFICATION.md` for detailed specifications.
