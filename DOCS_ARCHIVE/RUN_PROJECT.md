# 🚀 Running B2Connect - Quick Start Guide

## Prerequisites Check

### Required Software
```bash
# Check if Docker is installed and running
docker --version
docker ps

# Check if .NET SDK is installed
dotnet --version

# Check if Node.js is installed
node --version
npm --version
```

## 🎯 RECOMMENDED: Start Everything with Aspire

.NET Aspire orchestriert **alle Services automatisch** (PostgreSQL, RabbitMQ, Redis, und alle Microservices).

### One-Command Startup (Alles in Einem)

```bash
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/AppHost
dotnet restore
dotnet run
```

**Das ist es!** Aspire wird:
- ✅ PostgreSQL starten
- ✅ RabbitMQ starten
- ✅ Redis starten
- ✅ Auth Service starten (Port 5001)
- ✅ Tenant Service starten (Port 5002)
- ✅ API Gateway starten (Port 5000)

**Aspire Dashboard öffnet sich automatisch** (oder: http://localhost:5265)

### Frontend starten (separate Terminal)

```bash
cd /Users/holger/Documents/Projekte/B2Connect/frontend
npm install
npm run dev
```

**Expected output**:
- Frontend running at: http://localhost:3000 (oder http://localhost:5173)

---

## Alternative: Manuelle Docker-Compose Methode

Falls du Docker-Compose bevorzugst oder Aspire Probleme hat:

### Step 1: Start Docker Desktop ⚠️
**WICHTIG**: Docker daemon muss laufen:

1. **On macOS**: 
   - Open Applications → Docker.app
   - Warte auf "Docker is running" Status in der Menu Bar

2. **Verify Docker is running**:
   ```bash
   docker ps
   ```

### Step 2: Start Infrastructure (PostgreSQL, RabbitMQ, Redis)

```bash
cd /Users/holger/Documents/Projekte/B2Connect
docker-compose -f backend/infrastructure/docker-compose.yml up -d
```

**Verify infrastructure is running**:
```bash
docker-compose -f backend/infrastructure/docker-compose.yml ps
```

Expected output:
```
NAME                    COMMAND                  SERVICE             STATUS
b2connect-postgres      postgres                 postgres            Up
b2connect-rabbitmq      rabbitmq                 rabbitmq            Up
b2connect-redis         redis                    redis               Up
```

### Step 3: Run Backend Services (Terminal 1)

```bash
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/AppHost
dotnet restore
dotnet run
```

**Expected output**:
- Services starting on ports:
  - API Gateway: http://localhost:5000
  - Auth Service: http://localhost:5001
  - Tenant Service: http://localhost:5002

### Step 4: Run Frontend (Terminal 2)

```bash
cd /Users/holger/Documents/Projekte/B2Connect/frontend
npm install
npm run dev
```

**Expected output**:
- Frontend running at: http://localhost:3000 (oder http://localhost:5173)

### Step 5: Access the Application

Open your browser and navigate to:
```
http://localhost:3000
```

### Aspire Dashboard

Wenn du Aspire nutzt, öffnet sich automatisch das Dashboard:
```
http://localhost:5265
```

Das Dashboard zeigt dir:
- ✅ Status aller Services (PostgreSQL, RabbitMQ, Redis)
- ✅ Status aller Microservices (Auth, Tenant, Gateway)
- ✅ Logs von allen Services
- ✅ Environment Variables
- ✅ Resource Usage
- ✅ Links zu den Services

## Services Overview

### Infrastructure Services
- **PostgreSQL** (Port 5432)
  - Default: postgres/postgres
  - Database: b2connect
  
- **RabbitMQ** (Port 5672, Management UI: 15672)
  - Default: guest/guest
  - Management UI: http://localhost:15672

- **Redis** (Port 6379)

### Backend Services
- **API Gateway** (Port 5000)
  - Base URL: http://localhost:5000/api
  
- **Auth Service** (Port 5001)
  - Swagger: http://localhost:5001/swagger

- **Tenant Service** (Port 5002)
  - Swagger: http://localhost:5002/swagger

### Frontend
- **Vue.js App** (Port 3000 or 5173)
  - URL: http://localhost:3000

## Troubleshooting

### Docker daemon not running
```bash
# Check status
docker ps

# If error: "Cannot connect to Docker daemon"
# → Start Docker Desktop application
```

### Port already in use
```bash
# Find process using port (e.g., 5000)
lsof -i :5000

# Kill process
kill -9 <PID>
```

### Dependencies not restored
```bash
cd backend
dotnet restore
```

### npm modules not installed
```bash
cd frontend
rm -rf node_modules
npm install
```

### PostgreSQL connection error
```bash
# Check if containers are running
docker ps

# Check logs
docker logs b2connect-postgres

# Restart containers
docker-compose -f backend/infrastructure/docker-compose.yml restart
```

## Stopping the Project

### Stop Frontend
```
Press Ctrl+C in the frontend terminal
```

### Stop Backend
```
Press Ctrl+C in the backend terminal
```

### Stop Infrastructure (Database, Message Queue, Cache)
```bash
docker-compose -f backend/infrastructure/docker-compose.yml down
```

## Testing the Application

### Test Authentication
1. Navigate to http://localhost:3000
2. Click "Get Started" or go to http://localhost:3000/login
3. (Note: You'll need to implement the login backend endpoint first)

### Test Backend API
```bash
# Get health check
curl http://localhost:5000/health

# Or use Swagger UI
# Auth Service: http://localhost:5001/swagger
# Tenant Service: http://localhost:5002/swagger
```

### View Logs
```bash
# Frontend dev server logs
# (visible in terminal where npm run dev is running)

# Backend logs
# (visible in terminal where dotnet run is running)

# Docker logs
docker logs b2connect-postgres
docker logs b2connect-rabbitmq
docker logs b2connect-redis
```

## Development Tips

### Hot Reload
- **Frontend**: Changes automatically reload (Vite hot reload)
- **Backend**: Use `dotnet watch run` for auto-rebuild

### Database Inspection
```bash
# Connect to PostgreSQL
psql -h localhost -U postgres -d b2connect

# List tables
\dt

# Exit
\q
```

### Message Queue Inspection
- Open http://localhost:15672
- Login: guest/guest
- View message queue stats

### Cache Testing
```bash
# Connect to Redis
redis-cli

# Test connection
ping
# Should respond with: PONG

# Exit
exit
```

## Next Steps

1. ✅ Start Docker Desktop
2. ✅ Run infrastructure: `docker-compose up -d`
3. ✅ Run backend: `dotnet run` (from AppHost)
4. ✅ Run frontend: `npm run dev`
5. ✅ Open http://localhost:3000
6. 📚 Read [DEVELOPMENT.md](DEVELOPMENT.md) for feature creation
7. 🧪 Run tests: `npm run test` or `dotnet test`

## Quick Command Reference

```bash
# 🎯 RECOMMENDED: Alles mit Aspire starten (1 Terminal)
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/AppHost && \
dotnet restore && \
dotnet run

# Frontend in separatem Terminal
cd /Users/holger/Documents/Projekte/B2Connect/frontend && \
npm install && \
npm run dev

# Dann öffne:
# - Frontend: http://localhost:3000
# - Aspire Dashboard: http://localhost:5265
```

### Alternative: Docker-Compose Methode

```bash
# Everything at once (terminal 1)
cd /Users/holger/Documents/Projekte/B2Connect && \
docker-compose -f backend/infrastructure/docker-compose.yml up -d && \
echo "Infrastructure started. Now run backend in separate terminal."

# Terminal 2: Backend
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/AppHost && \
dotnet run

# Terminal 3: Frontend
cd /Users/holger/Documents/Projekte/B2Connect/frontend && \
npm run dev
```

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Docker daemon not running | Open Docker Desktop app |
| Port 5000 in use | `lsof -i :5000` and `kill -9 <PID>` |
| npm modules missing | `cd frontend && npm install` |
| DB connection error | Check `docker-compose ps` |
| Frontend shows blank page | Check browser console (F12) |
| API Gateway 404 | Backend services must be running |

---

**Status**: ✅ Ready to run  
**Last Updated**: 2025-12-25  
**Setup Time**: ~5 minutes (after Docker starts)
