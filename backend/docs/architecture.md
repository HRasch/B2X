# B2Connect Architecture

## Overview

B2Connect is a modern multitenant SaaS platform built with a microservices architecture. It leverages .NET Aspire for orchestration, Wolverine for message-driven patterns, and Vue.js 3 for the frontend.

## System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Frontend (Vue.js 3)                   │
│                    (Port 3000 / 5173)                    │
└──────────────────────────┬──────────────────────────────┘
                           │
                    API Call (HTTP/REST)
                           │
┌──────────────────────────▼──────────────────────────────┐
│                   API Gateway (YARP)                     │
│                    (Port 5000)                           │
│         - Request Routing                               │
│         - Tenant Context Propagation                    │
│         - CORS Handling                                 │
└──────┬─────────────────────────────────────┬────────────┘
       │                                     │
       ▼                                     ▼
   ┌────────────────┐            ┌────────────────────┐
   │  Auth Service  │            │  Tenant Service    │
   │  (Port 5001)   │            │   (Port 5002)      │
   ├────────────────┤            ├────────────────────┤
   │ - Login        │            │ - Tenant CRUD      │
   │ - JWT Token    │            │ - Provisioning     │
   │ - Validation   │            │ - User Management  │
   └────────────────┘            └────────────────────┘
       │                                     │
       └──────────────┬──────────────────────┘
                      │
           ┌──────────┴──────────┐
           │                     │
           ▼                     ▼
      ┌─────────────┐      ┌──────────────┐
      │ PostgreSQL  │      │  RabbitMQ    │
      │ (Port 5432) │      │ (Port 5672)  │
      │             │      │              │
      │ - Data      │      │ - Message    │
      │   Storage   │      │   Queue      │
      │ - Tenant    │      │ - Events     │
      │   Isolation │      │ - Commands   │
      └─────────────┘      └──────────────┘
           │                     │
           └──────────┬──────────┘
                      │
                      ▼
           ┌─────────────────────┐
           │      Redis          │
           │    (Port 6379)      │
           │   - Caching         │
           │   - Sessions        │
           └─────────────────────┘
```

## Services

### API Gateway (Port 5000)
- **Role**: Entry point for all client requests
- **Technology**: ASP.NET Core with YARP (Yet Another Reverse Proxy)
- **Responsibilities**:
  - Request routing to microservices
  - Tenant context propagation via `X-Tenant-ID` header
  - CORS configuration
  - API versioning

### Auth Service (Port 5001)
- **Role**: Authentication and authorization
- **Technology**: ASP.NET Core with JWT
- **Responsibilities**:
  - User login and token generation
  - JWT token validation and refresh
  - User authentication
  - Role and permission management

### Tenant Service (Port 5002)
- **Role**: Tenant management and provisioning
- **Technology**: ASP.NET Core with Entity Framework Core
- **Responsibilities**:
  - Tenant CRUD operations
  - Tenant provisioning and deprovisioning
  - User management within tenants
  - Tenant-specific configurations

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
