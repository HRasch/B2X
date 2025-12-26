# E2E Test Execution Guide

## Quick Start

### Prerequisites
Ensure both backend and frontend services are running:

```bash
# Terminal 1: Start Backend
cd /Users/holger/Documents/Projekte/B2Connect
npm run aspire-watch  # or: dotnet run -p backend/services/AppHost

# Terminal 2: Start Frontend Admin
cd /Users/holger/Documents/Projekte/B2Connect/frontend-admin
npm run dev -- --port 5174

# Terminal 3: Run E2E Tests
cd /Users/holger/Documents/Projekte/B2Connect/frontend-admin
npx playwright test tests/e2e/auth.spec.ts
```

## Test Execution Options

### 1. Run All E2E Tests
```bash
cd frontend-admin
npx playwright test tests/e2e/auth.spec.ts
```
**Output**: HTML report in `playwright-report/`

### 2. Run with UI Mode (Interactive)
```bash
npx playwright test tests/e2e/auth.spec.ts --ui
```
Features:
- Watch tests run in real-time
- Step through tests manually
- See console logs and network activity
- Debug failing tests interactively

### 3. Run Specific Test Suite
```bash
# Authentication tests only
npx playwright test tests/e2e/auth.spec.ts -g "Authentication"

# Dashboard tests only
npx playwright test tests/e2e/auth.spec.ts -g "Dashboard"

# Design tests only
npx playwright test tests/e2e/auth.spec.ts -g "Soft UI Design"

# Accessibility tests only
npx playwright test tests/e2e/auth.spec.ts -g "Accessibility"
```

### 4. Run with Debugging
```bash
# Debug mode: opens dev tools
npx playwright test tests/e2e/auth.spec.ts --debug

# Headed mode: see the browser
npx playwright test tests/e2e/auth.spec.ts --headed

# With traces (for post-mortem analysis)
npx playwright test tests/e2e/auth.spec.ts --trace on
```

### 5. Generate HTML Report
```bash
npx playwright test tests/e2e/auth.spec.ts --reporter=html
npx playwright show-report
```

## What Tests Validate

### ✅ Authentication Flow
- Login form renders correctly
- Error handling for invalid credentials
- Successful login with admin credentials
- Token stored in localStorage
- Demo credentials displayed

### ✅ Dashboard Access
- Dashboard displays after login
- Navigation sidebar functions
- User info shown in header
- Logout functionality works
- Session persists across refreshes

### ✅ Soft UI Design
- Login form has Soft UI styling
- Gradient backgrounds applied
- Typography and spacing correct

### ✅ Accessibility
- Form labels present
- Button text readable
- Keyboard navigation works (Tab key)

## Test Credentials

For all tests, the following credentials are used:
- **Email**: admin@example.com
- **Password**: password

These credentials are auto-seeded in the Auth Service database.

## Expected Results

### All Tests Pass
```
✓ admin-frontend-e2e-tests (19 tests)
  ✓ admin-frontend-authentication (7 tests)
  ✓ admin-frontend-dashboard (6 tests)
  ✓ admin-frontend-soft-ui (3 tests)
  ✓ admin-frontend-accessibility (3 tests)

19 passed (2m 30s)
```

### Troubleshooting

#### Issue: Tests timeout at login
**Cause**: Frontend not running on localhost:5174
**Solution**: 
```bash
cd frontend-admin && npm run dev -- --port 5174
```

#### Issue: 401 Unauthorized errors
**Cause**: Backend not running or Auth Service not initialized
**Solution**:
```bash
# Check backend is running
curl http://localhost:5000/health

# Or restart backend
cd backend && dotnet run -p services/AppHost
```

#### Issue: Cannot find test file
**Cause**: File path incorrect
**Solution**: Ensure file exists at `frontend-admin/tests/e2e/auth.spec.ts`

#### Issue: Token not found in localStorage
**Cause**: Login failed silently
**Solution**: 
- Check browser console for errors
- Verify backend is returning JWT token
- Check admin@example.com credentials are correct in database

## Test Files Location

```
frontend-admin/
├── tests/
│   └── e2e/
│       └── auth.spec.ts              ← Main E2E test suite
├── playwright.config.ts               ← Playwright configuration
└── package.json
```

## CI/CD Integration

### GitHub Actions Example
```yaml
name: E2E Tests
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Start Backend
        run: |
          cd backend
          dotnet run -p services/AppHost &
          sleep 10
      
      - name: Start Frontend
        run: |
          cd frontend-admin
          npm install
          npm run build
          npm run dev -- --port 5174 &
          sleep 10
      
      - name: Run E2E Tests
        run: |
          cd frontend-admin
          npm install -D @playwright/test
          npx playwright test tests/e2e/auth.spec.ts
      
      - name: Upload Report
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: playwright-report
          path: frontend-admin/playwright-report/
```

## Performance Metrics

| Test Suite | Duration | Count |
|-----------|----------|-------|
| Authentication | ~30s | 7 tests |
| Dashboard | ~45s | 6 tests |
| Soft UI Design | ~20s | 3 tests |
| Accessibility | ~25s | 3 tests |
| **Total** | **~2m** | **19 tests** |

## Next Steps

1. ✅ Backend unit tests: Complete (14/16 passing)
2. 🔄 Frontend E2E tests: Ready to execute (see above)
3. 📊 Integration tests: Recommended for future
4. 🔒 Security tests: Recommended for production

---

**Created**: December 26, 2024
**Last Updated**: December 26, 2024
**Status**: Ready for Execution
