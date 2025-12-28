# 🔐 Security Quick Reference - Multi-Tenant

## Development Setup

```bash
# 1. Install dependencies
dotnet add package Microsoft.Extensions.Caching.Memory
dotnet add package Polly

# 2. Update appsettings.Development.json
{
  "Tenant": {
    "Development": {
      "UseFallback": true,
      "FallbackTenantId": "00000000-0000-0000-0000-000000000001"
    }
  }
}

# 3. Apply Global Query Filter in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyGlobalTenantFilter(_tenantContext);
}

# 4. Entities implement IHasTenantId
public class Product : IHasTenantId
{
    public Guid TenantId { get; set; }
    // ...
}
```

---

## JWT Token Format

```json
{
  "sub": "user-123",
  "tenant_id": "550e8400-e29b-41d4-a716-446655440000",
  "roles": ["admin"],
  "exp": 1735488000
}
```

**Required Claims:**
- `tenant_id` - Tenant GUID
- `sub` - User ID

---

## API Requests

### Authenticated Request (JWT)
```bash
curl /api/products \
  -H "Authorization: Bearer <jwt_token>"
# Tenant aus JWT extrahiert
```

### API Key Request (Header)
```bash
curl /api/products \
  -H "X-Tenant-ID: 550e8400-e29b-41d4-a716-446655440000" \
  -H "X-API-Key: <api_key>"
# Tenant aus Header
```

### Development (Fallback)
```bash
curl http://localhost:7002/api/products
# Verwendet Fallback: 00000000-0000-0000-0000-000000000001
```

---

## Security Checks

### ✅ DO
- Verwende JWT `tenant_id` claim (source of truth)
- Implementiere `IHasTenantId` für alle Entities
- Verwende Global Query Filter
- Log security events
- Validate user tenant ownership

### ❌ DON'T
- Trust X-Tenant-ID header ohne JWT validation
- Use Development fallback in production
- Return detailed errors to client
- Accept invalid host formats
- Skip input validation

---

## Error Codes

| Code | Meaning | Cause |
|------|---------|-------|
| 400 | Bad Request | Invalid host, missing tenant |
| 403 | Forbidden | Tenant mismatch, unauthorized access |
| 500 | Internal Error | Development fallback in production |

---

## Testing

```bash
# Run security tests
dotnet test --filter "Category=Security"

# Check tenant isolation
curl /api/products \
  -H "X-Tenant-ID: other-tenant" \
  -H "Authorization: Bearer <my-jwt>"
# → 403 Forbidden

# Check host validation
curl /api/products \
  -H "Host: invalid'; DROP TABLE--"
# → 400 Bad Request
```

---

## Production Checklist

- [ ] `UseFallback = false`
- [ ] JWT secret > 32 chars
- [ ] Global Query Filter active
- [ ] Generic error messages
- [ ] Monitoring enabled
- [ ] Audit logging active

---

**Full Docs:** [SECURITY_FIXES_IMPLEMENTATION.md](SECURITY_FIXES_IMPLEMENTATION.md)
