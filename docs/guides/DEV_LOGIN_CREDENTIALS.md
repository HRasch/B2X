# Development Login Credentials

**Environment**: Development (InMemory Database)  
**Purpose**: Test user accounts for local development  
**Last Updated**: 31. Dezember 2025

---

## 🚨 Quick Answer: How to Login

### Seeded Credentials (Backend Running)

When the backend is running with the InMemory database, use these pre-seeded accounts:

| User Type | Email | Password | Role |
|-----------|-------|----------|------|
| **Admin** | `admin@example.com` | `password` | Administrator |
| **Demo User** | `user@example.com` | `password` | User |

### Steps to Login

1. **Start the backend**:
   ```bash
   cd backend/Orchestration
   dotnet run
   ```

2. **Start the frontend**:
   ```bash
   cd frontend/Store
   npm run dev
   ```

3. **Login** at http://localhost:5173/login with:
   - Email: `admin@example.com`
   - Password: `password`

---

## 📋 Pre-Seeded Test Users

The Identity service automatically creates these users on startup (see `backend/Domain/Identity/Program.cs` lines 245-320):

### Admin User
```
Email:    admin@example.com
Password: password
Role:     Admin
Status:   Email confirmed, active
```

### Demo User
```
Email:    user@example.com  
Password: password
Role:     User
Status:   Email confirmed, active
```

---

## 🧪 E2E Test Mode

**E2E tests use mock authentication that bypasses the backend entirely.**

This is controlled by the `VITE_E2E_TEST` environment variable:

- **Normal development** (`VITE_E2E_TEST` not set): Uses real backend with seeded credentials
- **E2E tests** (`VITE_E2E_TEST=true`): Uses mock authentication bypass

### Running E2E Tests

```bash
cd frontend/Store
npm run test:e2e
```

The Playwright config automatically sets `VITE_E2E_TEST=true` when running E2E tests.

### Why This Design?

| Mode | Backend Required | Why |
|------|------------------|-----|
| **Development** | ✅ Yes | Test real authentication flow |
| **E2E Tests** | ❌ No | Fast, isolated, no backend dependencies |

---

## 🔐 Password Requirements

For registering new users, passwords must contain:
- ✅ Minimum **8 characters**
- ✅ At least **1 uppercase** letter (A-Z)
- ✅ At least **1 lowercase** letter (a-z)
- ✅ At least **1 digit** (0-9)
- ✅ At least **1 special character** (@, $, !, %, *, ?, &)

**Note**: The seeded users use `password` which doesn't meet these requirements. This is intentional for easy development testing.

---

## 🚀 Quick Development Setup

### VS Code Tasks (Recommended)

1. Open Command Palette (`Cmd+Shift+P` on macOS)
2. Run "Tasks: Run Task"
3. Select `backend-start` → Starts backend services
4. Select `frontend-start` → Starts frontend on port 5173
5. Login with `admin@example.com` / `password`

### Manual Setup

```bash
# Terminal 1: Backend
cd backend/Orchestration
dotnet run

# Terminal 2: Frontend  
cd frontend/Store
npm run dev

# Open http://localhost:5173/login
# Use: admin@example.com / password
```

---

## 🧪 Testing Different User Scenarios

### Test Multi-Tenant Isolation

1. Login as `admin@example.com` → See admin tenant data
2. Register new user in different tenant
3. Login as new user → See only that tenant's data
4. Admin cannot see other tenant's data ✅

### Test B2B vs B2C

**B2C Customer** (Private):
- Register at: `/register/private`
- No VAT-ID required
- Sees B2C prices (incl. VAT)

**B2B Customer** (Business):
- Register at: `/register/business`
- VAT-ID required
- Sees B2B prices (excl. VAT if valid VAT-ID)

---

## 🔍 Debugging Authentication

### Check If User is Logged In

Open browser console (F12):
```javascript
// Check localStorage
localStorage.getItem('access_token');  // Should return a token string
localStorage.getItem('tenant_id');     // Should return a GUID
```

### Check Backend Health

```bash
curl http://localhost:7002/health
```

### View Running Services

Open Aspire Dashboard: http://localhost:15500

---

## 🛠️ How to Create Additional Test Users

### Via Frontend Registration

1. Navigate to: http://localhost:5173/register/private
2. Complete the registration form
3. The new user will be created in the InMemory database
4. Login with the email and password you just created

### Via API (curl)

```bash
curl -X POST http://localhost:7002/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "yourtest@example.com",
    "password": "YourPass123!",
    "firstName": "Test",
    "lastName": "User",
    "acceptTerms": true,
    "acceptPrivacy": true
  }'
```

---

## ⚠️ Important Notes

- **InMemory database resets on restart** - All registered users are lost when the backend restarts. Only seeded users persist.
- **Seeded credentials are for development only** - Never use these in production.
- **E2E mode is only for automated tests** - Development should always use the real backend.

---

## 📊 Quick Reference

| Environment | Database | Credentials | Backend |
|-------------|----------|-------------|---------|
| Development | InMemory | `admin@example.com` / `password` | ✅ Required |
| E2E Tests | Mock | Any (bypassed) | ❌ Not needed |
| Production | PostgreSQL | User-created | ✅ Required |
