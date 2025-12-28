# B2Connect Frontend Integration - Aspire 13.1.0 Setup

## ? TL;DR - Quick Start

```bash
# Option 1: Docker Compose (Easiest)
docker-compose up -d

# Option 2: Aspire + Docker (Recommended for Development)
# Terminal 1:
cd AppHost && dotnet run

# Option 3: Manual (Full debugging)
# Terminal 1:
cd AppHost && dotnet run
# Terminal 2:
cd Frontend/Store && npm install && npm run dev
# Terminal 3:
cd Frontend/Admin && npm install && npm run dev
```

Access:
- Store: http://localhost:5173
- Admin: http://localhost:5174
- Aspire Dashboard: http://localhost:15500 (Option 2 only)

## ?? Why `AddNpmApp` Doesn't Exist Anymore

**Aspire 13.1.0 removed `AddNpmApp`** because:

1. **Complexity**: Node.js/npm isn't managed by .NET runtime
2. **Best Practices**: Frontends are typically in separate deployment pipelines
3. **Containers**: Docker Compose or Kubernetes is the standard approach

Your current setup with `docker-compose.yml` is **already the recommended approach** for Aspire 13.1.0! ?

## ?? Current State

? **docker-compose.yml**
- Has all backend services configured
- Has both frontends (Store & Admin)
- Uses multi-stage Dockerfiles for dev/prod builds
- Proper networking and volumes

? **Dockerfiles**
- `Frontend/Store/Dockerfile` - Multi-stage build
- `Frontend/Admin/Dockerfile` - Multi-stage build
- Development target: Hot-reload with `npm run dev`
- Production target: Static serving

? **AppHost/Program.cs**
- Clean configuration (no broken `AddNpmApp`)
- Comment explains manual frontend startup
- Ready for Aspire Dashboard monitoring

? **vite.config.ts**
- Configured for 0.0.0.0 host (Docker-ready)
- Environment variables for API gateway URLs
- Hot module reload enabled

## ?? Three Recommended Approaches

### 1. Docker Compose (Production-Like) ? Simplest

```bash
docker-compose up -d
```

**What starts:**
- PostgreSQL, Redis, RabbitMQ, Elasticsearch
- Identity Service (7002)
- Tenancy Service (7003)
- Localization Service (7004)
- Catalog Service (7005)
- Theming Service (7008)
- Store Gateway (8000)
- Admin Gateway (8080)
- Frontend Store (5173) - with hot reload
- Frontend Admin (5174) - with hot reload

**Pros:** Simple, reproducible, closest to production
**Cons:** Docker must be running, less direct backend debugging

---

### 2. Aspire + Docker Frontends (Hybrid) ? Best for Dev

```bash
# Terminal 1
cd AppHost
dotnet run

# Terminal 2
docker-compose up frontend-store frontend-admin
```

**What starts:**
- Aspire orchestrates: Postgres, Redis, RabbitMQ, Elasticsearch, all microservices
- Docker runs: Frontend containers with hot-reload
- Aspire Dashboard: http://localhost:15500

**Pros:**
- Aspire Dashboard monitoring
- Direct backend debugging
- Frontends still containerized
- Clean separation

**Cons:** Docker + .NET SDKs needed

---

### 3. Manual (Max Debugging) ????? Dev Only

```bash
# Terminal 1
cd AppHost
dotnet run

# Terminal 2
cd Frontend/Store
npm install
npm run dev

# Terminal 3
cd Frontend/Admin
npm install
npm run dev
```

**What starts:**
- Aspire orchestrates backend
- Node processes for frontends (local)

**Pros:** Maximum debugging, hot-reload is very fast
**Cons:** Node.js SDK needed, no containerization

---

## ?? Comparison Table

| Aspect | Option 1 (Compose) | Option 2 (Aspire+Docker) | Option 3 (Manual) |
|--------|-------------------|--------------------------|-------------------|
| **Setup Time** | 30s | 2m | 3m |
| **Docker Needed** | ? Yes | ? Yes | ? No |
| **Node.js Needed** | ? No | ? No | ? Yes |
| **.NET SDK Needed** | ? No | ? Yes | ? Yes |
| **Aspire Dashboard** | ? No | ? Yes | ? Yes |
| **Backend Debugging** | ?? Hard | ? Easy | ? Easy |
| **Frontend Hot-Reload** | ? Works | ? Works | ? Fastest |
| **Production-Ready** | ? Very | ? Very | ? No |
| **Learning Curve** | Easy | Medium | Medium |

**Recommendation for most developers:** **Option 2** (Aspire + Docker)

---

## ?? Configuration Details

### Docker Compose Frontend Services

**Both frontends in docker-compose.yml:**

```yaml
frontend-store:
  build:
    context: ./Frontend/Store
    dockerfile: Dockerfile
    target: development  # Multi-stage target
  environment:
    VITE_API_GATEWAY_URL: http://store-gateway:8000
    NODE_ENV: development
  volumes:
    - ./Frontend/Store:/app
    - /app/node_modules
  command: npm run dev -- --host 0.0.0.0
```

### Vite Configuration

**Both vite.config.ts files support:**

```typescript
server: {
  host: "0.0.0.0",      // Listen on all interfaces (Docker)
  strictPort: false,     // Don't fail if port taken
  proxy: {
    "/api": {
      target: process.env.VITE_API_GATEWAY_URL 
        || "http://localhost:8000",
      changeOrigin: true,
      ws: true,
    }
  }
}
```

### Dockerfile Multi-Stage Build

**Development stage** (used by docker-compose):
```dockerfile
FROM node:20-alpine AS development
...
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
```

**Production stage** (ready when needed):
```dockerfile
FROM node:20-alpine AS production
RUN npm install -g serve
COPY --from=builder /app/dist ./dist
CMD ["serve", "-s", "dist", "-l", "5173"]
```

---

## ?? Docker Compose Commands

```bash
# Start all services
docker-compose up -d

# Start specific services only
docker-compose up -d frontend-store frontend-admin
docker-compose up -d postgres redis

# View logs
docker-compose logs -f frontend-store
docker-compose logs -f admin-gateway

# Rebuild without cache
docker-compose build --no-cache

# Stop all
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Restart a service
docker-compose restart frontend-store
```

---

## ??? Environment Variables

### For docker-compose:

```bash
# In docker-compose.yml or .env file
VITE_API_GATEWAY_URL=http://store-gateway:8000
NODE_ENV=development
```

### For manual run:

```bash
# Linux/Mac
export VITE_API_GATEWAY_URL=http://localhost:8000
cd Frontend/Store && npm run dev

# Windows PowerShell
$env:VITE_API_GATEWAY_URL = "http://localhost:8000"
cd Frontend/Store; npm run dev
```

---

## ?? Access Points

### When using docker-compose up -d:

| Service | URL |
|---------|-----|
| Store Frontend | http://localhost:5173 |
| Admin Frontend | http://localhost:5174 |
| Store Gateway | http://localhost:8000 |
| Admin Gateway | http://localhost:8080 |
| Auth Service | http://localhost:7002 |
| Tenancy Service | http://localhost:7003 |
| Localization | http://localhost:7004 |
| Catalog Service | http://localhost:7005 |
| Theming Service | http://localhost:7008 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |
| RabbitMQ Admin | http://localhost:15672 |
| Elasticsearch | http://localhost:9200 |

### When using Aspire (Option 2):

Same as above, plus:
- **Aspire Dashboard**: http://localhost:15500

---

## ?? Troubleshooting

### Frontend doesn't start

```bash
# Check if npm is installed
npm --version

# Check if Docker is running
docker ps

# Rebuild images
docker-compose build --no-cache frontend-store

# View error logs
docker-compose logs frontend-store
```

### Port already in use

```bash
# Option 1: Kill specific port
lsof -ti:5173 | xargs kill -9  # Mac/Linux
# Windows: Use Resource Monitor or:
netstat -ano | findstr :5173
taskkill /PID <PID> /F

# Option 2: Stop all Docker containers
docker-compose down
```

### Frontend can't reach API

1. Check API Gateway is running:
   ```bash
   curl http://localhost:8000/health
   ```

2. Check VITE_API_GATEWAY_URL environment variable:
   ```bash
   docker-compose logs frontend-store | grep VITE
   ```

3. Check proxy configuration in vite.config.ts

---

## ?? Files You Need to Know

- `docker-compose.yml` - Orchestrates all services
- `Frontend/Store/Dockerfile` - Store frontend build
- `Frontend/Admin/Dockerfile` - Admin frontend build
- `Frontend/Store/vite.config.ts` - Store Vite config
- `Frontend/Admin/vite.config.ts` - Admin Vite config
- `AppHost/Program.cs` - Aspire orchestration (backend only)
- `docs/ASPIRE_FRONTEND_INTEGRATION.md` - Full documentation

---

## ?? Key Takeaways

1. ? `AddNpmApp` is gone in Aspire 13.1.0 - that's normal
2. ? Your `docker-compose.yml` setup is the recommended approach
3. ? Use Option 2 (Aspire + Docker) for best development experience
4. ? Frontends are containerized and production-ready
5. ? Everything has hot-reload for fast development

---

**Need help?** See `docs/ASPIRE_FRONTEND_INTEGRATION.md` for advanced topics.

Run `./STARTUP_GUIDE.ps1` (Windows) or `bash STARTUP_GUIDE.sh` (Mac/Linux) for interactive setup.
