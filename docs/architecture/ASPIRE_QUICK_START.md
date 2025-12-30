# 🚀 Quick Start - Aspire Integration

**Status**: ✅ Ready to Use

---

## 1️⃣ Immediate Setup (5 min)

### Install Required NuGet Packages

```bash
cd /Users/holger/Documents/Projekte/B2Connect

# PostgreSQL Support
dotnet add package Aspire.Hosting.PostgreSQL --version 8.0.0

# Redis Support
dotnet add package Aspire.Hosting.Redis --version 8.0.0

# Redis Client
dotnet add package StackExchange.Redis --version 2.7.0

# EF Core Naming Convention
dotnet add package EFCore.NamingConventions --version 8.0.0

# Caching
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis --version 8.0.0

# JWT Support (already included)
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.0.0

# Optional: Fido2 for Production Passkeys
dotnet add package Fido2.NetFramework --version 3.4.0
```

---

## 2️⃣ Configure Azure Key Vault (15 min)

### Local Development (appsettings.Development.json)

```json
{
  "Azure": {
    "KeyVault": {
      "Uri": "https://b2connect-dev.vault.azure.net/"
    },
    "TenantId": "<your-tenant-id>",
    "ClientId": "<your-client-id>",
    "ClientSecret": "<your-client-secret>"
  },
  "Jwt": {
    "Secret": "dev-jwt-secret-minimum-32-characters-required!!!",
    "Issuer": "B2Connect",
    "Audience": "B2Connect"
  },
  "Database": {
    "Password": "postgres"
  },
  "Redis": {
    "Password": "redis-password"
  },
  "Encryption": {
    "Key": "dev-encryption-key-minimum-32-characters-required!!!"
  }
}
```

### Azure Portal Setup

```bash
# 1. Create Key Vault
az keyvault create \
  --name b2connect-dev-vault \
  --resource-group b2connect-dev \
  --location westeurope

# 2. Set Secrets
az keyvault secret set \
  --vault-name b2connect-dev-vault \
  --name "Jwt--Secret" \
  --value "$(openssl rand -base64 32)"

az keyvault secret set \
  --vault-name b2connect-dev-vault \
  --name "Database--Password" \
  --value "$(openssl rand -base64 16)"

az keyvault secret set \
  --vault-name b2connect-dev-vault \
  --name "Encryption--Key" \
  --value "$(openssl rand -base64 32)"

# 3. Configure Access Policy
az keyvault set-policy \
  --name b2connect-dev-vault \
  --object-id $(az ad user show --id you@example.com --query id -o tsv) \
  --secret-permissions get list set delete

# 4. Get Resource IDs for Aspire
export KEYVAULT_URI=$(az keyvault show \
  --name b2connect-dev-vault \
  --query properties.vaultUri -o tsv)
```

---

## 3️⃣ Start Aspire Orchestration (5 min)

### Run with Docker Desktop

```bash
# Ensure Docker Desktop is running
open -a Docker

# Build Orchestration
cd AppHost
dotnet build

# Run Aspire
dotnet run

# Output:
# Aspire.Hosting: Now listening on: http://localhost:15500
# Dashboard URL: http://localhost:15500
```

### Access Aspire Dashboard

```
http://localhost:15500
```

Dashboard shows:
- ✅ PostgreSQL Status
- ✅ Redis Status
- ✅ All Microservices
- ✅ Health Checks
- ✅ Logs & Traces
- ✅ Metrics

---

## 4️⃣ Verify All Services

### Check Service Health

```bash
# Auth Service (with Passkeys)
curl http://localhost:7002/health

# Tenant Service
curl http://localhost:7003/health

# Localization Service
curl http://localhost:7004/health

# Catalog Service
curl http://localhost:7005/health

# Theming Service
curl http://localhost:7008/health

# Store Gateway
curl http://localhost:8000/health

# Admin Gateway
curl http://localhost:8080/health
```

Expected Response:
```json
{
  "status": "Healthy",
  "checks": {
    "PostgreSQL": "Healthy",
    "Redis": "Healthy",
    "JWT": "Healthy"
  }
}
```

---

## 5️⃣ Test Passkeys Registration

### Step 1: Start Registration

```bash
curl -X POST http://localhost:7002/api/auth/passkeys/registration/start \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "userName": "john.doe",
    "userEmail": "john@example.com"
  }'
```

Response:
```json
{
  "challenge": "base64-encoded-challenge",
  "rp": {
    "name": "B2Connect",
    "id": "localhost"
  },
  "user": {
    "id": "dXNlci0xMjM=",
    "name": "john.doe",
    "displayName": "john@example.com"
  },
  "pubKeyCredParams": [...],
  "attestation": "none",
  "timeout": 60000
}
```

### Step 2: User Registers Passkey (Browser)

```javascript
// In Browser Console
const options = response.json(); // from above

const credential = await navigator.credentials.create({
  publicKey: {
    challenge: new Uint8Array(
      atob(options.challenge)
        .split('')
        .map(c => c.charCodeAt(0))
    ),
    rp: options.rp,
    user: options.user,
    pubKeyCredParams: options.pubKeyCredParams,
    attestation: options.attestation,
    timeout: options.timeout
  }
});

// Send credential back to server
fetch('http://localhost:7002/api/auth/passkeys/registration/complete', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(credential)
});
```

### Step 3: Server Completes Registration

```json
{
  "success": true,
  "credentialId": "credential-123",
  "message": "Passkey registered successfully"
}
```

---

## 6️⃣ Test Database Connection

### Check PostgreSQL

```bash
# From Docker
docker ps | grep postgres

# Connect to DB
psql -h localhost -U postgres -d b2connect_auth

# List tables
\dt

# Sample query
SELECT * FROM aspnetcore_users LIMIT 5;
```

### Check Redis

```bash
# From Docker
docker ps | grep redis

# Connect to Redis
redis-cli -h localhost -p 6379

# Ping
> PING
PONG

# Check Keys
> KEYS *

# Sample Get
> GET "jwt:token:user-123"
```

---

## 7️⃣ Test Rate Limiting

### Run Load Test

```bash
# Using Apache Bench
ab -n 1000 -c 10 http://localhost:7002/api/auth/login

# Expected: After 5 requests in 5 min window, return 429 Too Many Requests
```

---

## 8️⃣ Verify Encryption at Rest

### Check Encrypted Data

```bash
# Connect to PostgreSQL
psql -h localhost -U postgres -d b2connect_auth

# Select encrypted column
SELECT id, email_encrypted, email_iv FROM users LIMIT 1;

# Data should be base64-encoded (not readable plaintext)
```

---

## 9️⃣ Check Audit Logging

### View Audit Logs

```bash
# In PostgreSQL
SELECT 
  entity_type,
  action,
  user_id,
  changed_at,
  changes
FROM audit_logs
ORDER BY changed_at DESC
LIMIT 10;
```

Expected:
```
entity_type | action | user_id | changed_at | changes
------------|--------|---------|------------|--------
User        | INSERT | admin   | 2024-01-01 | {"email":"..."}
User        | UPDATE | admin   | 2024-01-01 | {"name":"..."}
Passkey     | INSERT | user-1  | 2024-01-01 | {"credentialId":"..."}
```

---

## 🔟 Troubleshooting

### PostgreSQL Connection Failed

```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# If not, create container
docker run --name postgres-b2connect \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16

# Check logs
docker logs postgres-b2connect
```

### Redis Connection Failed

```bash
# Check if Redis is running
docker ps | grep redis

# If not, create container
docker run --name redis-b2connect \
  -p 6379:6379 \
  -d redis:7

# Test connection
redis-cli ping
```

### Azure Key Vault Access Denied

```bash
# Check Managed Identity (if using)
az identity show --name b2connect-identity

# Or check Service Principal Credentials
az ad sp show --id $(az account show --query user.name -o tsv)

# Add Access Policy
az keyvault set-policy \
  --name b2connect-dev-vault \
  --object-id <identity-object-id> \
  --secret-permissions get list
```

---

## 📊 Performance Tips

### Optimize PostgreSQL

```sql
-- Add indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_passkeys_user_id ON passkeys(user_id);

-- Analyze table
ANALYZE users;

-- Vacuum
VACUUM ANALYZE;
```

### Optimize Redis

```bash
# Increase cache timeout for frequently accessed data
redis-cli CONFIG SET maxmemory-policy allkeys-lru

# Monitor cache hit rate
redis-cli INFO stats | grep hits
```

---

## ✅ Success Checklist

- [ ] All NuGet packages installed
- [ ] Azure Key Vault configured
- [ ] PostgreSQL running (docker or local)
- [ ] Redis running (docker or local)
- [ ] Aspire Orchestration started
- [ ] All 7 services healthy
- [ ] Passkeys registration working
- [ ] Database connections verified
- [ ] Audit logging functional
- [ ] Rate limiting working

---

## 🎯 Next Steps

1. ✅ This Quick Start - **DONE**
2. Implement Passkeys API Controllers (1-2h)
3. Update Frontend for Passkeys Support (2-3h)
4. E2E Tests (2h)
5. Staging Deployment (2-4h)
6. Production Deployment (1 day)

---

**Time to Complete**: ~30 minutes  
**Prerequisites**: Docker Desktop, .NET 10 SDK, Azure Account  
**Status**: ✅ Ready to Start
