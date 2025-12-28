# 🎯 Tenant-Ermittlung Quick Reference

## Development (lokal)

**Backend Config:**
```json
// appsettings.Development.json
{
  "Tenant": {
    "Development": {
      "UseFallback": true,
      "FallbackTenantId": "00000000-0000-0000-0000-000000000001"
    }
  }
}
```

**Frontend Config:**
```env
# .env.development
VITE_DEFAULT_TENANT_ID=00000000-0000-0000-0000-000000000001
```

**API Call:**
```bash
# Kein X-Tenant-ID Header nötig!
curl http://localhost:7002/api/products
# → Verwendet automatisch: 00000000-0000-0000-0000-000000000001
```

---

## Production (host-basiert)

**Tenant registrieren:**
```bash
POST /api/tenants
{
  "name": "Kunde 1",
  "domain": "kunde1.b2connect.com",
  "isActive": true
}
```

**API Call:**
```bash
# Kein Header nötig - Host wird automatisch ermittelt!
curl https://kunde1.b2connect.com/api/products
# → Middleware ermittelt Tenant aus Host
```

---

## Admin Frontend (mit Header)

**TypeScript:**
```typescript
const tenantId = localStorage.getItem("tenantId") || 
                 "00000000-0000-0000-0000-000000000001";

await axios.post("/api/auth/login", credentials, {
  headers: {
    "X-Tenant-ID": tenantId
  }
});
```

---

## Priorität

1. **X-Tenant-ID Header** (höchste)
2. **Host-basierte Lookup**
3. **Development Fallback** (nur Development)

---

## Public Endpoints (kein Tenant nötig)

- `/api/auth/login`
- `/api/auth/register`
- `/api/auth/refresh`
- `/health`, `/healthz`
- `/swagger`

---

## Troubleshooting

**Error: "Tenant could not be resolved"**
```bash
# Check 1: Development Fallback aktiviert?
cat appsettings.Development.json | grep UseFallback

# Check 2: Host registriert?
curl http://localhost:7003/api/tenants/by-domain/localhost

# Check 3: Header gesendet?
curl -v http://localhost:7002/api/products | grep X-Tenant-ID
```

---

**Vollständige Dokumentation:** [TENANT_RESOLUTION_GUIDE.md](TENANT_RESOLUTION_GUIDE.md)
