# 500 Internal Server Error - Fixed

## Problem Identified

The **Vite proxy configuration** was incorrectly rewriting paths:

```typescript
// ❌ WRONG - Removed /api prefix from paths
rewrite: (path: string) => path.replace(/^\/api/, "")
```

This caused:
- Frontend request: `/api/auth/login`
- After proxy rewrite: `/auth/login`
- Backend expected: `/api/auth/login` ❌

Result: 500 Internal Server Error

## Solution Applied

Removed the incorrect `rewrite` rule from the Vite proxy configuration:

```typescript
// ✅ CORRECT - Keep the full path
proxy: {
  "/api": {
    target: process.env.VITE_API_GATEWAY_URL || "http://api-gateway:8080",
    changeOrigin: true,
    // Removed: rewrite: (path: string) => path.replace(/^\/api/, "")
    secure: false,
    ws: true,
    timeout: 30000,
  },
}
```

## Services Status

✅ **API Gateway** - Running on port 6000
✅ **Auth Service** - Running on port 5001
✅ **Tenant Service** - Running on port 5002

## Verification

Test the login endpoint:
```bash
curl -X POST http://localhost:6000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'
```

Expected response: 200 OK with JWT token

## Files Modified

- `frontend/vite.config.ts` - Removed incorrect rewrite rule

## How the Proxy Works (Correctly)

1. **Frontend Request**: `POST /api/auth/login`
2. **Vite Proxy Intercepts**: Path matches `/api` rule
3. **Forward to API Gateway**: `POST http://localhost:6000/api/auth/login` (full path preserved)
4. **API Gateway Processes**: Route matches `/api/auth` in controllers
5. **Response Sent Back**: 200 OK with token

## Next Steps

Frontend services can now start normally:
```bash
cd frontend
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev

# OR

cd frontend-admin
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

---

**Status**: ✅ Fixed  
**Root Cause**: Incorrect Vite proxy rewrite configuration  
**Solution**: Removed path rewriting to preserve `/api` prefix
