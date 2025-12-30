# B2Connect - B2B/B2C Shop Platform & Procurement Gateway

A modern, scalable multitenant SaaS solution combining a comprehensive B2B/B2C e-commerce platform with a Procurement-Platform Gateway. Built with .NET Aspire (C# backend) and Vue.js 3 (frontend), deployable on AWS, Azure, and Google Cloud.

**Status:** ✅ Production Ready | **Last Updated:** 27. Dezember 2025

## 🚀 Quick Links

- � **[Documentation Quick Reference](docs/QUICK_REFERENCE.md)** - Find the documentation you need
- 📖 **[AI Knowledge Base](docs/ai/INDEX.md)** - AI agent triggers & reference
- 🚀 **[Quick Start Guide](QUICK_START_GUIDE.md)** - Get started in 5 minutes
- 🔒 **[Security Instructions](.github/instructions/security.instructions.md)** - Security implementation
- 📊 **[Project Dashboard](PROJECT_DASHBOARD.md)** - Metrics & status

## 📊 Platform Overview

**B2Connect** consists of three integrated components:

1. **Shop Platform**: Full-featured e-commerce solution supporting both B2B and B2C models
   - Multi-channel order management
   - Product catalog with advanced filtering and recommendations
   - Shopping cart and checkout with multiple payment methods
   - Order fulfillment and logistics integration
   - Customer relationship management

2. **Procurement Gateway**: Integration hub for B2B procurement platforms
   - Unified API for third-party procurement platforms
   - Order synchronization and automation
   - Real-time inventory management
   - Supplier integration
   - EDI and API-based integrations

3. **Frontend CMS & Layout Builder**: Integrated no-code customization system
   - Visual page builder with drag-and-drop interface
   - 50+ pre-built components (UI, layout, e-commerce)
   - Theme customizer (colors, fonts, spacing)
   - Publishing workflow with version control
   - SEO optimization tools
   - **Allows customers to build custom storefronts without developers**

## 🏗️ Architecture

**Microservices:**
- Domain-Driven Design with bounded contexts
- .NET Aspire orchestration with observability
- Separate API Gateways for Store (public) and Admin (protected)

**Infrastructure:**
- PostgreSQL 16.11 (7 databases)
- Redis 8.4.0 (caching & sessions)
- Elasticsearch 9.2.3 (search)
- RabbitMQ 4.2.2 (messaging)

**Frontend:**
- Vue.js 3 with TypeScript and Vite
- Separate apps for Store and Admin

See [Architecture Documentation](docs/architecture/) for details.

## 🚀 Quick Start

### Prerequisites
- **.NET 10 SDK**
- **Node.js 18+** and **npm**
- **No Docker required** - Aspire handles orchestration

### Start Aspire (Recommended)

```bash
cd backend/Orchestration
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Services orchestrated by Aspire:
# - Store Gateway: http://localhost:6000
# - Admin Gateway: http://localhost:6100
# - Aspire Dashboard: http://localhost:15500
```

### Frontend Setup (In Another Terminal)

```bash
cd frontend/Store
npm install && npm run dev

# Frontend will be available at http://localhost:5173
```

**That's it!** You now have a complete local development environment with:
- ✅ **Auth Service** - Authentication & Authorization
- ✅ **Tenant Service** - Multi-tenant Management
- ✅ **Localization Service** - i18n & Translations
- ✅ **Vue.js Frontend** - Full UI

**Why AppHost?**
- 🎯 **All-in-one**: Orchestrates all services in one command
- 📍 **Cross-platform**: Works on macOS, Windows, and Linux identically
- ⚡ **Zero dependencies**: No Docker, DCP, or external tools required
- 🔧 **Simple configuration**: Plain .NET Process API - easy to extend
- 📊 **Clear visibility**: All logs in one terminal

👉 **[Full AppHost Guide →](APPHOST_SPECIFICATIONS.md)** | **[Quick Reference →](APPHOST_QUICKSTART.md)**

## 📋 Technology Stack

### Backend
- **Runtime**: .NET 8
- **Framework**: ASP.NET Core
- **Orchestration**: .NET Aspire
- **Message Broker**: Wolverine (CQRS, events)
- **Data Access**: Entity Framework Core
- **Observability**: OpenTelemetry, Serilog
- **Cloud Support**: AWS, Azure, Google Cloud SDKs

### Frontend
- **Framework**: Vue.js 3
- **Build Tool**: Vite
- **Language**: TypeScript
- **State Management**: Pinia
- **HTTP Client**: Axios
- **Testing**: Vitest, Vue Test Utils, Playwright
- **UI Components**: PrimeVue or TailwindCSS

## 📚 Documentation

- [Backend Architecture](backend/docs/architecture.md)
- [API Specifications](backend/docs/api-specifications.md)
- [Shop Platform Specifications](backend/docs/shop-platform-specs.md)
- [Procurement Gateway Specifications](backend/docs/procurement-gateway-specs.md)
- **[Frontend CMS & Layout Builder](backend/docs/cms-frontend-builder.md)** - NEW: Customer-facing customization system
- **[CMS Overview](CMS_OVERVIEW.md)** - NEW: Business-friendly CMS introduction
- **[CMS Implementation Details](CMS_IMPLEMENTATION_UPDATE.md)** - NEW: Technical CMS architecture
- [Multitenant Design](backend/docs/tenant-isolation.md)
- [Development Guidelines](.copilot-specs.md)

## 🧪 Testing

### Backend
```bash
cd backend/services/auth-service
dotnet test tests/
```

### Frontend
```bash
cd Frontend/Store
npm run test              # Unit tests
npm run test:components   # Component tests
npm run test:e2e          # End-to-end tests
npm run coverage          # Coverage report
```

## 🐳 Docker & Deployment

### Local Development with Docker
```bash
# Build and run with docker-compose
cd backend/infrastructure
docker-compose up

# Frontend is served separately
cd Frontend/Store
npm run dev
```

### Cloud Deployment
- **AWS**: See `backend/infrastructure/terraform/aws/`
- **Azure**: See `backend/infrastructure/terraform/azure/`
- **Google Cloud**: See `backend/infrastructure/terraform/gcp/`

## 🔐 Environment Configuration

Copy `.env.example` to `.env` and configure:

```bash
# Backend (.env in AppHost)
ASPNETCORE_ENVIRONMENT=Development
DATABASE_URL=Server=localhost;Database=b2connect;User Id=sa;Password=YourPassword;
RABBITMQ_URL=amqp://guest:guest@localhost:5672/

# Frontend (.env in frontend/Store/)
VITE_API_URL=http://localhost:5000/api
VITE_APP_NAME=B2Connect
```

## 👥 Multitenant Architecture

- **Data Isolation**: Row-Level Security (RLS) or separate schemas per tenant
- **Authentication**: OpenID Connect with JWT (tenant ID in claims)
- **API**: All requests include `X-Tenant-ID` header
- **Frontend**: Tenant context managed via Pinia store
- **Scaling**: Tenant-aware auto-scaling policies

## 📈 Development Workflow

### Test-Driven Development
1. Write failing test
2. Implement minimal code to pass
3. Refactor while keeping tests green

### Code Generation with Copilot
See [.copilot-specs.md](.copilot-specs.md) for detailed guidelines on:
- When to use Copilot (✅ boilerplate, ❌ security-critical code)
- Review checklist for generated code
- Multitenant safety requirements

## 🚦 CI/CD Pipeline

- **Trigger**: Push to main/develop branches
- **Backend**: Restore, build, test, lint
- **Frontend**: Install, build, test, lint
- **Coverage**: Minimum 80% for new code
- **Security**: Scan dependencies, check credentials

## 📝 Contributing

1. Follow TDD approach - tests first, then implementation
2. Review [.copilot-specs.md](.copilot-specs.md) for standards
3. Ensure all tests pass before submitting PR
4. Maintain test coverage above 80%
5. Include documentation for public APIs

## 📄 License

TBD

## 👨‍💻 Team

B2Connect Development Team

---

**Last Updated**: 2025-12-30
**Version**: 1.0.0-beta
