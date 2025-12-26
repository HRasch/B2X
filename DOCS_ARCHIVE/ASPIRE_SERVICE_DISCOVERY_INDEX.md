# Aspire Service Discovery - Documentation Index

## Quick Navigation

### 🚀 Getting Started
1. **[ASPIRE_SETUP_QUICKSTART.md](./ASPIRE_SETUP_QUICKSTART.md)** ← **START HERE**
   - Quick start instructions
   - Service ports reference
   - Common commands
   - Troubleshooting tips

### 📚 Complete Documentation
2. **[ASPIRE_SERVICE_DISCOVERY.md](./ASPIRE_SERVICE_DISCOVERY.md)**
   - Full architecture explanation
   - How service discovery works
   - Configuration files details
   - Environment variables
   - Docker/Kubernetes deployment

3. **[ASPIRE_IMPLEMENTATION_COMPLETE.md](./ASPIRE_IMPLEMENTATION_COMPLETE.md)**
   - What was implemented
   - All configuration changes
   - Benefits and features
   - Technical details
   - Next steps

4. **[MIGRATION_SUMMARY.md](./MIGRATION_SUMMARY.md)**
   - Before/after comparison
   - Files modified list
   - Quick reference guide
   - Migration checklist
   - Current status

### 🛠️ Scripts & Tools

#### Start Services
```bash
bash start-services-local.sh
```
Located: `/Users/holger/Documents/Projekte/B2Connect/start-services-local.sh`

**What it does:**
- Starts all backend services
- Verifies services are healthy
- Shows configuration summary
- Monitors for crashes

#### Stop Services
```bash
bash stop-services-local.sh
```
Located: `/Users/holger/Documents/Projekte/B2Connect/stop-services-local.sh`

**What it does:**
- Stops all running services
- Clears reserved ports
- Saves logs for debugging

### 📋 Configuration Files

#### Frontend Environment Variables
- `frontend/.env.local` - Main app environment
- `frontend-admin/.env.local` - Admin app environment

**Content:**
```bash
VITE_API_GATEWAY_URL=http://localhost:6000
```

#### Backend Configuration
- `backend/services/AppHost/appsettings.json` - Service registry
- `backend/services/api-gateway/appsettings.json` - Gateway routing
- `backend/services/auth-service/appsettings.json` - Auth config
- `backend/services/api-gateway/Properties/launchSettings.json` - Port settings

### 🔍 Key Concepts

#### Service Names
Services reference each other by name, not ports:
```
Local Dev: http://localhost:6000 (overridden via env var)
Docker: http://api-gateway:8080 (DNS resolution)
```

#### Environment Variable Priority
1. **Environment Variable** (highest) - `VITE_API_GATEWAY_URL=http://localhost:6000`
2. **appsettings.json** - Service configuration
3. **Default Fallback** (lowest) - `http://api-gateway:8080`

#### Service Port Mapping
| Service | Local Port | Docker Service |
|---------|-----------|----------------|
| API Gateway | 6000 | api-gateway:8080 |
| Auth Service | 5001 | auth-service:8080 |
| Tenant Service | 5002 | tenant-service:8080 |
| Localization | 5003 | localization-service:8080 |
| Frontend | 5173 | localhost:5173 |
| Admin | 5174 | localhost:5174 |

## 📖 How to Use This Documentation

### I just want to get started
→ Read **[ASPIRE_SETUP_QUICKSTART.md](./ASPIRE_SETUP_QUICKSTART.md)**

### I want to understand how it works
→ Read **[ASPIRE_SERVICE_DISCOVERY.md](./ASPIRE_SERVICE_DISCOVERY.md)**

### I need to know what changed
→ Read **[MIGRATION_SUMMARY.md](./MIGRATION_SUMMARY.md)**

### I need to see everything
→ Read **[ASPIRE_IMPLEMENTATION_COMPLETE.md](./ASPIRE_IMPLEMENTATION_COMPLETE.md)**

## 🎯 Common Tasks

### Start Everything
```bash
# Terminal 1: Start backend services
bash start-services-local.sh

# Terminal 2: Start main frontend
cd frontend
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev

# Terminal 3: Start admin frontend
cd frontend-admin
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

### Stop Everything
```bash
bash stop-services-local.sh
```

### Test Services
```bash
# API Gateway health
curl http://localhost:6000/health

# Auth Service health
curl http://localhost:5001/health

# Test login
curl -X POST http://localhost:6000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'
```

### Check Logs
```bash
tail -f /tmp/b2connect-api-gateway.log
tail -f /tmp/b2connect-auth-service.log
tail -f /tmp/b2connect-tenant-service.log
```

## 🐳 Docker/Aspire Deployment

### Using Docker Compose
```bash
docker-compose -f backend/docker-compose.aspire.yml up
```

**Services automatically discover each other:**
- No environment variables needed
- DNS resolves service names to container IPs
- Same code works locally and in production

## ✅ Verification Checklist

- [ ] Run `bash start-services-local.sh`
- [ ] Verify API Gateway responds: `curl http://localhost:6000/health`
- [ ] Verify Auth Service responds: `curl http://localhost:5001/health`
- [ ] Start frontend with: `VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev`
- [ ] Open http://localhost:5174 in browser
- [ ] Verify login works without CORS errors
- [ ] Check `.env.local` files have correct VITE_API_GATEWAY_URL

## 🆘 Troubleshooting

### Service won't start
Check logs:
```bash
tail /tmp/b2connect-*.log
```

Verify port availability:
```bash
lsof -i :6000  # API Gateway
lsof -i :5001  # Auth Service
```

### Frontend can't connect to API
Check environment variable:
```bash
echo $VITE_API_GATEWAY_URL
# Should show: http://localhost:6000
```

### Port conflicts
```bash
bash stop-services-local.sh
# Wait a few seconds
bash start-services-local.sh
```

## 📞 Support Resources

1. Check the relevant documentation file above
2. Search for the issue in logs: `/tmp/b2connect-*.log`
3. Verify environment variables are set: `printenv | grep VITE`
4. Ensure all services are running: `lsof -i -P -n | grep LISTEN`

## 🗂️ File Organization

```
/Users/holger/Documents/Projekte/B2Connect/
├── start-services-local.sh              ← Start all services
├── stop-services-local.sh               ← Stop all services
├── ASPIRE_SETUP_QUICKSTART.md           ← Quick start guide
├── ASPIRE_SERVICE_DISCOVERY.md          ← Full documentation
├── ASPIRE_IMPLEMENTATION_COMPLETE.md    ← Implementation details
├── MIGRATION_SUMMARY.md                 ← Migration summary
├── ASPIRE_SERVICE_DISCOVERY_INDEX.md    ← This file
├── frontend/
│   └── .env.local                       ← Frontend env vars
├── frontend-admin/
│   └── .env.local                       ← Admin env vars
└── backend/
    └── services/
        ├── api-gateway/
        │   ├── appsettings.json
        │   └── Properties/launchSettings.json
        ├── auth-service/
        │   └── appsettings.json
        ├── tenant-service/
        │   └── appsettings.json
        └── localization-service/
            └── appsettings.json
```

## 📊 Current Status

✅ **Configuration** - Complete
✅ **Scripts** - Ready to use
✅ **Documentation** - Comprehensive
✅ **Local Development** - Ready
⏳ **Docker Testing** - Next phase
⏳ **Production Deployment** - Planned

## 🚀 Next Steps

1. Start services: `bash start-services-local.sh`
2. Run frontend with env var: `VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev`
3. Test login flow in browser
4. Verify everything works
5. Deploy to Docker when ready

---

**Last Updated**: December 26, 2025  
**Status**: ✅ Ready for Use  
**Version**: 1.0

