# B2Connect Frontend Integration Guide

## ?? Overview

B2Connect has two Vue 3 + Vite frontends:

1. **Frontend/Store** - Public storefront (Port 5173)
   - Customer-facing store interface
   - Communicates with Store Gateway (Port 8000)
2. **Frontend/Admin** - Admin panel (Port 5174)
   - Admin and management dashboard
   - Communicates with Admin Gateway (Port 8080)

## ?? Quick Start

### Option 1: Docker Compose (Recommended)

Start everything with one command:

```bash
# Windows (PowerShell)
.\scripts\start-full-stack.ps1

# Linux/Mac
./scripts/start-full-stack.sh

# Or directly:
docker-compose up -d
```

This starts:

- PostgreSQL, Redis, RabbitMQ, Elasticsearch
- All backend microservices
- Both Vite frontends (with hot reload)

### Option 2: Manual Development

Start services and frontends separately:

```bash
# Terminal 1: Start Docker infrastructure + backend services
docker-compose up -d postgres redis rabbitmq elasticsearch
cd AppHost
dotnet run

# Terminal 2: Frontend Store
cd Frontend/Store
npm install  # (first time only)
npm run dev

# Terminal 3: Frontend Admin
cd Frontend/Admin
npm install  # (first time only)
npm run dev
```

## ?? Frontend Architecture

### Directory Structure

```
Frontend/
??? Store/                    # Public storefront
?   ??? src/
?   ?   ??? components/       # Vue components
?   ?   ??? stores/           # Pinia state management
?   ?   ??? services/         # API clients
?   ?   ??? router/           # Vue Router
?   ?   ??? App.vue
?   ??? package.json
?   ??? vite.config.ts        # Vite configuration
?   ??? Dockerfile            # Container build config
?   ??? .dockerignore
?
??? Admin/                    # Admin panel
    ??? src/
    ?   ??? components/
    ?   ??? stores/
    ?   ??? services/
    ?   ??? router/
    ?   ??? App.vue
    ??? package.json
    ??? vite.config.ts
    ??? Dockerfile
    ??? .dockerignore
```

### Technology Stack

- **Vue 3** - UI framework (Composition API)
- **TypeScript** - Type safety
- **Vite** - Build tool with hot module reload
- **Pinia** - State management
- **Vue Router** - Client-side routing
- **Axios** - HTTP client
- **Tailwind CSS** - Styling (in package.json)

## ?? Configuration

### Environment Variables

Vite frontends read environment variables prefixed with `VITE_`:

**Frontend/Store/vite.config.ts:**

```typescript
proxy: {
  "/api": {
    target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8000",
    changeOrigin: true,
    ws: true,
  }
}
```

**Frontend/Admin/vite.config.ts:**

```typescript
proxy: {
  "/api": {
    target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8080",
    changeOrigin: true,
  }
}
```

### Docker Environment Variables

In `docker-compose.yml`:

```yaml
frontend-store:
  environment:
    VITE_API_GATEWAY_URL: http://store-gateway:8000
    NODE_ENV: development

frontend-admin:
  environment:
    VITE_API_GATEWAY_URL: http://admin-gateway:8080
    NODE_ENV: development
```

## ?? Accessing Frontends

When running locally:

| Application | URL                   | Gateway               | Notes             |
| ----------- | --------------------- | --------------------- | ----------------- |
| Store       | http://localhost:5173 | http://localhost:8000 | Public storefront |
| Admin       | http://localhost:5174 | http://localhost:8080 | Protected admin   |

## ??? Development Commands

### Frontend/Store

```bash
cd Frontend/Store

# Install dependencies
npm install

# Start dev server (http://localhost:5173)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run tests
npm run test
npm run test:watch
npm run test:coverage

# E2E tests (Playwright)
npm run e2e
npm run e2e:ui
npm run e2e:debug

# Linting
npm run lint
npm run format

# Type checking
npm run type-check
```

### Frontend/Admin

Same commands as Store:

```bash
cd Frontend/Admin
npm install
npm run dev
# ... (etc.)
```

## ?? Docker Build

### Build Frontend Images Manually

```bash
# Build Store frontend
docker build -t b2connect-store:latest ./Frontend/Store

# Build Admin frontend
docker build -t b2connect-admin:latest ./Frontend/Admin

# Run Store frontend
docker run -p 5173:5173 \
  -e VITE_API_GATEWAY_URL=http://store-gateway:8000 \
  b2connect-store:latest

# Run Admin frontend
docker run -p 5174:5174 \
  -e VITE_API_GATEWAY_URL=http://admin-gateway:8080 \
  b2connect-admin:latest
```

### Multi-stage Dockerfile

Our Dockerfiles support multiple build targets:

```bash
# Development (with hot reload)
docker build --target development -t b2connect-store:dev ./Frontend/Store

# Production (built + served with `serve`)
docker build --target production -t b2connect-store:prod ./Frontend/Store
```

## ?? API Integration

### Store Frontend ? Store Gateway

**File: `Frontend/Store/src/services/api.ts`**

```typescript
import axios from 'axios';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:8000',
  timeout: 30000,
});

// Request interceptor for auth token
apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  // Add tenant ID if needed
  const tenantId = localStorage.getItem('tenantId');
  if (tenantId) {
    config.headers['X-Tenant-ID'] = tenantId;
  }
  return config;
});

export default apiClient;
```

### Admin Frontend ? Admin Gateway

**File: `Frontend/Admin/src/services/api.ts`**

Same pattern, but proxies to Admin Gateway (port 8080).

## ?? API Communication Flow

```
Browser (Frontend)
    ?
Vite Dev Server (5173/5174)
    ?
Proxy ? API Gateway (8000/8080)
    ?
Microservices (7002-7008)
    ?
PostgreSQL / Elasticsearch / RabbitMQ
```

## ?? State Management (Pinia)

### Store example:

**File: `Frontend/Store/src/stores/productStore.ts`**

```typescript
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiClient from '@/services/api';

export const useProductStore = defineStore('product', () => {
  const products = ref<Product[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const fetchProducts = async () => {
    loading.value = true;
    try {
      const { data } = await apiClient.get('/api/products');
      products.value = data;
      error.value = null;
    } catch (e) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  };

  const filteredProducts = computed(() => products.value.filter(p => p.published));

  return {
    products,
    loading,
    error,
    fetchProducts,
    filteredProducts,
  };
});
```

## ?? Testing

### Unit Tests (Vitest)

```bash
npm run test              # Run once
npm run test:watch       # Watch mode
npm run test:coverage    # With coverage
npm run test:ui          # UI dashboard
```

### E2E Tests (Playwright)

```bash
npm run e2e              # Run tests
npm run e2e:ui          # Interactive mode
npm run e2e:debug       # Debug mode
npm run e2e:report      # View HTML report
```

## ?? Troubleshooting

### Port Already in Use

```bash
# Kill process on port 5173
lsof -ti:5173 | xargs kill -9

# Or use docker-compose
docker-compose down
```

### Frontend Can't Connect to API

1. Check API Gateway is running:

   ```bash
   curl http://localhost:8000/health
   curl http://localhost:8080/health
   ```

2. Check vite.config.ts proxy configuration

3. Check browser console for CORS errors

4. Verify environment variables:
   ```bash
   echo $VITE_API_GATEWAY_URL  # Linux/Mac
   echo %VITE_API_GATEWAY_URL%  # Windows
   ```

### Docker Build Fails

```bash
# Clean up
docker system prune -a

# Rebuild with no cache
docker-compose build --no-cache
```

## ?? Further Reading

- [Vite Documentation](https://vitejs.dev/)
- [Vue 3 Guide](https://vuejs.org/guide/)
- [Pinia State Management](https://pinia.vuejs.org/)
- [Playwright Testing](https://playwright.dev/)
- [Tailwind CSS](https://tailwindcss.com/)

## ?? Related Documentation

- [API Gateway Configuration](../backend/Gateway/README.md)
- [Docker Compose Setup](../docker-compose.yml)
- [Microservices Architecture](../docs/ARCHITECTURE.md)
