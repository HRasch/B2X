# B2Connect Architecture

## Overview

B2Connect is a modern multitenant SaaS platform combining a B2B/B2C e-commerce shop with a Procurement-Platform Gateway. It's built with a microservices architecture, leveraging .NET Aspire for orchestration, Wolverine for message-driven patterns, and Vue.js 3 for the frontend.

## Key Features

### Shop Platform
- **B2B & B2C Support**: Flexible pricing, bulk ordering, and customer segments
- **Product Management**: Rich catalog with categories, variants, and recommendations
- **Order Management**: From cart to fulfillment with multi-channel support
- **Payment Integration**: Stripe, PayPal, bank transfers, and B2B payment methods
- **Inventory Management**: Real-time stock tracking and warehouse management

### Procurement Gateway
- **Platform Integration**: Connect to Coupa, Ariba, Jaggr, and other procurement systems
- **Order Synchronization**: Bi-directional sync with upstream procurement platforms
- **API-First Design**: RESTful APIs for seamless integration
- **Supplier Management**: Manage suppliers, contracts, and performance metrics

## System Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                    Frontend (Vue.js 3)                                   │
│        (Shop, Admin Dashboard, Procurement Portal)                       │
│                    (Port 3000 / 5173)                                    │
└──────────────────────────┬──────────────────────────────────────────────┘
                           │
                    API Call (HTTP/REST)
                           │
┌──────────────────────────▼──────────────────────────────────────────────┐
│                   API Gateway (YARP)                                    │
│                    (Port 5000)                                          │
│         - Request Routing & Load Balancing                             │
│         - Tenant Context Propagation                                   │
│         - CORS Handling & Rate Limiting                                │
└──┬──────────┬──────────┬──────────┬──────────┬──────────┬──────────┬──┘
   │          │          │          │          │          │          │
   ▼          ▼          ▼          ▼          ▼          ▼          ▼
┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐
│Auth  │ │Tenant│ │Shop  │ │Order │ │Procur│ │Catalog│ │Inv.  │ │Supplier
│Srv   │ │Srv   │ │Srv   │ │Srv   │ │Gateway │ │Srv   │ │Srv   │ │Srv
│(5001)│ │(5002)│ │(5003)│ │(5004)│ │(5005)│ │(5006)│ │(5007)│ │(5010)
└──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘ └──┬───┘
   │        │        │        │        │        │        │        │
   └────────┼────────┼────────┼────────┼────────┼────────┼────────┘
            │        │        │        │        │        │
            └────┬───┼────────┼────────┼────────┼────────┘
                 │   │        │        │        │
              ┌──┴───┴────────┴────────┴────────┴──┐
              │                                    │
              ▼                                    ▼
         ┌──────────────┐              ┌──────────────────┐
         │  PostgreSQL  │              │    RabbitMQ      │
         │ (Port 5432)  │              │  (Port 5672)     │
         │              │              │                  │
         │- Data        │              │- Messages        │
         │- Tenant      │              │- Events          │
         │- Isolation   │              │- Commands        │
         └──────────────┘              └──────────────────┘
              │                            │
              └─────────────┬──────────────┘
                            │
              ┌─────────────┴──────────────┬──────────────┐
              │                            │              │
              ▼                            ▼              ▼
         ┌──────────┐             ┌──────────────┐ ┌──────────────┐
         │  Redis   │             │ElasticSearch │ │Payment APIs  │
         │(6379)    │             │ (9200)       │ │Stripe/PayPal │
         │          │             │              │ │              │
         │- Cache   │             │- Search      │ │- Processing  │
         │- Session │             │- Logs        │ │- Webhooks    │
         └──────────┘             └──────────────┘ └──────────────┘
              │                            │              │
              └─────────────┬──────────────┴──────────────┘
                            │
              ┌─────────────┴──────────────┬──────────────┐
              │                            │              │
              ▼                            ▼              ▼
         ┌──────────────┐        ┌──────────────────┐ ┌──────────────┐
         │Procurement   │        │ Notification API │ │ Warehouse &  │
         │Platform APIs │        │ (Email/SMS/Push) │ │ Logistics    │
         │Coupa, Ariba  │        │                  │ │ Integration  │
         │Jaggr, etc.   │        │                  │ │              │
         └──────────────┘        └──────────────────┘ └──────────────┘
```

## Core Services

### API Gateway (Port 5000)
- **Role**: Entry point for all client requests
- **Technology**: ASP.NET Core with YARP (Yet Another Reverse Proxy)
- **Responsibilities**:
  - Request routing to microservices
  - Tenant context propagation via `X-Tenant-ID` header
  - CORS configuration and rate limiting
  - API versioning and request authentication

### Auth Service (Port 5001)
- **Role**: Authentication and authorization
- **Technology**: ASP.NET Core with JWT
- **Responsibilities**:
  - User login and token generation
  - JWT token validation and refresh
  - Multi-factor authentication
  - Role-based access control (RBAC)
  - OAuth2 integration for third-party logins

### Tenant Service (Port 5002)
- **Role**: Tenant management and provisioning
- **Technology**: ASP.NET Core with Entity Framework Core
- **Responsibilities**:
  - Tenant CRUD operations
  - Tenant provisioning and deprovisioning
  - User management within tenants
  - Tenant-specific configurations and branding
  - Subscription and billing management

## Shop Platform Services

### Shop Service (Port 5003)
- **Role**: Core e-commerce functionality
- **Technology**: ASP.NET Core with Event Sourcing
- **Responsibilities**:
  - Shopping cart management (B2B & B2C)
  - Pricing engine (tiered pricing for B2B)
  - Promotions and discounts
  - Customer account management
  - Wishlist and saved items

### Order Service (Port 5004)
- **Role**: Order management and fulfillment
- **Technology**: ASP.NET Core with Saga pattern
- **Responsibilities**:
  - Order creation and confirmation
  - Order status tracking
  - Order fulfillment workflow
  - Returns and refunds processing
  - Order history and analytics
  - Multi-warehouse fulfillment

### Catalog Service (Port 5006)
- **Role**: Product catalog management
- **Technology**: ASP.NET Core with Elasticsearch
- **Responsibilities**:
  - Product CRUD operations
  - Categories, tags, and attributes
  - Product variants and SKUs
  - Search and filtering
  - Product recommendations (ML-powered)
  - Inventory visibility

### Inventory Service (Port 5007)
- **Role**: Stock and warehouse management
- **Technology**: ASP.NET Core with real-time updates
- **Responsibilities**:
  - Real-time inventory tracking
  - Stock reservations
  - Warehouse and location management
  - Stock transfers between locations
  - Inventory forecasting
  - Low-stock alerts

### Payment Service (Port 5008)
- **Role**: Payment processing and reconciliation
- **Technology**: ASP.NET Core with PCI compliance
- **Responsibilities**:
  - Payment gateway integration (Stripe, PayPal)
  - B2B payment methods (invoicing, bank transfer)
  - Subscription management
  - PCI compliance handling
  - Payment reconciliation
  - Refund processing

### Notification Service (Port 5009)
- **Role**: Multi-channel notifications
- **Technology**: ASP.NET Core with message queue
- **Responsibilities**:
  - Email notifications (order confirmation, shipping, etc.)
  - SMS alerts
  - Push notifications
  - In-app messaging
  - Notification templates
  - Communication preferences management

## Procurement Gateway Services

### Procurement Gateway (Port 5005)
- **Role**: Integration hub for B2B procurement platforms
- **Technology**: ASP.NET Core with adapter pattern
- **Responsibilities**:
  - Unified API for procurement platform integration
  - Adapter implementations (Coupa, Ariba, Jaggr, etc.)
  - Order synchronization from procurement platforms
  - Real-time inventory updates to procurement systems
  - EDI/API message translation
  - Webhook management

### Supplier Service (Port 5010)
- **Role**: Supplier management and performance
- **Technology**: ASP.NET Core with Entity Framework Core
- **Responsibilities**:
  - Supplier CRUD operations
  - Supplier performance metrics
  - Contract management
  - Supplier onboarding workflow
  - Rating and compliance tracking
  - Supplier communication

## Data Isolation Strategy

### Row-Level Security (RLS)
Default approach using PostgreSQL RLS policies:
- Each table has a `tenant_id` column
- RLS policies enforce tenant isolation at the database level
- Database user context includes current tenant ID

### Implementation
```sql
-- Enable RLS on tables
ALTER TABLE users ENABLE ROW LEVEL SECURITY;

-- Create policy for tenant isolation
CREATE POLICY tenant_isolation ON users
  USING (tenant_id = current_setting('app.current_tenant_id')::uuid);
```

## Message-Driven Communication

### Wolverine Integration
- **Event Publishing**: Services publish domain events through Wolverine
- **Command Processing**: Commands are routed to appropriate handlers
- **Saga Support**: Complex workflows with multiple service interactions

### Message Flow
1. Service A publishes an event
2. Message broker (RabbitMQ) queues the message
3. Service B subscribes and processes the event
4. Saga orchestrates multi-service transactions

## Service Orchestration with .NET Aspire

### Aspire AppHost (Port 15500)
- **Central Orchestration Platform**: All services are registered and managed through Aspire
- **Service Discovery**: Automatic service registration and DNS-like resolution
- **Lifecycle Management**: Aspire controls startup order, health checks, and dependencies
- **Dashboard**: Real-time monitoring of all services, resources, and health status

### Service Registration & Management

**Each service is registered in AppHost:**
```csharp
// Example from AppHost/Program.cs
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("b2connect-db");

var authService = builder
    .AddProject<Projects.B2Connect_AuthService>("auth-service")
    .WithReference(postgres)
    .WithHttpEndpoint(port: 5001);

var apiGateway = builder
    .AddProject<Projects.B2Connect_ApiGateway>("api-gateway")
    .WithReference(authService)
    .WithHttpEndpoint(port: 5000);
```

### Automatic Features via Aspire

1. **Service Discovery**: Services automatically know about each other
2. **Environment Variables**: Dependencies injected automatically
3. **Health Checks**: Built-in `/health` endpoints with Aspire monitoring
4. **Logging**: Centralized log collection and viewing
5. **Metrics**: Prometheus-compatible metrics collection
6. **Distributed Tracing**: OpenTelemetry integration

### Startup Orchestration

Aspire automatically:
1. **Builds** all service projects
2. **Starts** resources in dependency order (PostgreSQL → Services)
3. **Injects** environment variables with service URLs
4. **Monitors** health checks continuously
5. **Logs** all service output to unified dashboard
6. **Tracks** metrics in real-time

### Health Check Strategy

Each service implements health checks:
- **Liveness**: Is the service running? (`/health/live`)
- **Readiness**: Can it accept requests? (`/health/ready`)
- **Startup**: Has initialization completed? (`/health/startup`)

Aspire Dashboard shows:
- 🟢 Healthy - Service running, checks passing
- 🟡 Degraded - Service running, checks failing
- 🔴 Unhealthy - Service not responding

### Benefits Over Manual Management

| Aspect | Manual | Aspire |
|--------|--------|--------|
| Service startup | 4+ terminals needed | Single command |
| Port management | Manual tracking | Automatic |
| Dependency injection | Manual URLs | Automatic |
| Health monitoring | Manual or third-party | Built-in dashboard |
| Log aggregation | Multiple streams | Unified view |
| Development experience | Complex setup | Simple `dotnet run` |

---

## Technology Stack

- **Backend**: .NET 8+, ASP.NET Core
- **Orchestration**: .NET Aspire
- **Message Broker**: Wolverine, RabbitMQ
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **API Gateway**: YARP
- **Frontend**: Vue.js 3, Vite
- **State Management**: Pinia
- **Testing**: xUnit, Moq, Vitest, Playwright
- **CMS**: Integrated Frontend Builder with drag-and-drop layout editor

## Frontend CMS & Layout Builder

B2Connect includes an integrated Content Management System (CMS) that enables customers to build and customize their storefront layout and design without coding.

### CMS Architecture

```
┌─────────────────────────────────────────────────────┐
│    CMS Admin Portal (Layout Builder + Theme)        │
│                                                     │
│  ┌──────────────────┬──────────────────────────┐   │
│  │ Layout Builder   │ Theme Customizer         │   │
│  │ - Drag & Drop    │ - Color Schemes          │   │
│  │ - Templates      │ - Typography             │   │
│  │ - Live Preview   │ - Custom CSS             │   │
│  └──────────────────┴──────────────────────────┘   │
└──────────────────────────┬──────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
        ▼                  ▼                  ▼
    ┌──────────┐      ┌──────────┐      ┌──────────┐
    │ Layout   │      │ Theme    │      │ Content  │
    │ Service  │      │ Service  │      │ Service  │
    │(5008)    │      │(5009)    │      │(5011)    │
    └──────────┘      └──────────┘      └──────────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           │
                    ┌──────┴──────┐
                    │             │
                    ▼             ▼
                ┌────────┐    ┌──────────┐
                │PostgreSQL   │S3/Blob   │
                │Database     │Storage   │
                └────────┘    └──────────┘
```

### CMS Services

#### Layout Service (Port 5008)
- **Page Structure Management**: Create, edit, organize pages
- **Component Registry**: Library of 50+ pre-built components
- **Drag-and-Drop Builder**: Visual page composition
- **Responsive Preview**: Desktop, tablet, mobile views
- **Template Library**: 20+ pre-designed page templates

#### Theme Service (Port 5009)
- **Visual Theme Editor**: Color schemes, typography, spacing
- **Design System Variables**: CSS custom properties management
- **Custom CSS Support**: Advanced styling with sanitization
- **Asset Management**: Logos, icons, images
- **Theme Publishing**: Activate and rollback themes

#### Content Service (Port 5011)
- **Publishing Workflow**: Draft, scheduled, published states
- **Version Control**: Complete change history and rollback
- **SEO Management**: Meta tags, structured data, optimization
- **Asset Library**: Image and media management
- **Scheduling**: Automated publish dates

### Component Library

**Categories**:
- **UI Components** (20+): Buttons, inputs, cards, badges, etc.
- **Layout Components** (15+): Hero, features, testimonials, CTA sections
- **E-Commerce** (15+): Product cards, cart, checkout, reviews

**Features**:
- Props-based configuration
- Theme-aware styling
- Responsive design built-in
- Data binding support

### Key Features

1. **No-Code Builder**
   - Drag-and-drop interface
   - Live real-time preview
   - Mobile-first responsive design

2. **Theme Customization**
   - Color scheme editor
   - Typography manager
   - Spacing & sizing controls
   - Custom CSS with safety

3. **Content Management**
   - Version control with rollback
   - Publishing workflow
   - Schedule changes for future publication
   - Audit trail of all modifications

4. **SEO Optimization**
   - Meta tags editor
   - Structured data (JSON-LD)
   - Keyword analysis
   - SEO scoring

5. **Asset Management**
   - Image optimization & CDN delivery
   - Lazy loading
   - Responsive image generation
   - Performance monitoring

6. **Integration Points**
   - E-commerce data binding (dynamic product lists)
   - Form integration (lead capture, contact)
   - Analytics integration (GA4, Hotjar)
   - Marketing tools (email signup, retargeting)

### Database Schema

CMS data stored in PostgreSQL:
- `cms_pages`: Page structures and content
- `cms_sections`: Layout sections within pages
- `cms_components`: Components within sections
- `cms_themes`: Theme configurations and styles
- `cms_page_versions`: Complete version history
- `cms_assets`: Uploaded images and media
- `cms_seo_metadata`: SEO optimization data

### API Integration

Frontend receives theme and layout via REST API:

```csharp
// Fetch active theme
GET /api/cms/themes/active
→ Returns: Colors, typography, spacing, custom CSS

// Fetch page structure
GET /api/cms/pages/{pageId}
→ Returns: Sections, components, content

// Publish changes
POST /api/cms/pages/{pageId}/publish
→ Updates live storefront
```

### Frontend Rendering

Vue.js dynamically applies theme and renders CMS layouts:

```typescript
// Load and apply theme
const loadTheme = async (tenantId) => {
  const theme = await api.get(`/api/cms/themes/active`);
  applyThemeVariables(theme);
};

// Render CMS page
<CmsPageRenderer :pageId="pageId" :theme="activeTheme" />
```

### Benefits

✅ **Customers can customize storefront without developers**
✅ **Rapid time-to-market for new storefronts**
✅ **Brand consistency through design system**
✅ **SEO-friendly with built-in optimization**
✅ **Safe customization with version control**
✅ **Performance optimized with CDN & caching**

## Deployment Topology

### Development
- Local Docker Compose with all services
- Aspire orchestration for simplified local development
- Hot reload for code changes

### Production
- Kubernetes deployment
- Multi-cloud support (AWS, Azure, Google Cloud)
- Service mesh for inter-service communication
- Distributed tracing with OpenTelemetry

## Security Considerations

1. **Authentication**: JWT tokens with tenant claims
2. **Tenant Isolation**: RLS at database level + application-level validation
3. **API Security**: CORS, rate limiting, input validation
4. **Data Encryption**: TLS for transit, encryption at rest for sensitive data
5. **Audit Logging**: All changes logged with tenant context

## Performance Optimization

1. **Caching**: Redis for session and query result caching
2. **Database**: Connection pooling, query optimization, indexing
3. **Frontend**: Code splitting, lazy loading, asset optimization
4. **API**: Pagination, compression, request batching
