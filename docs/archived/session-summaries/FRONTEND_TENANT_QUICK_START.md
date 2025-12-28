# Frontend-Tenant Quick Start

**Created**: December 27, 2025

## What is frontend-tenant?

The `frontend-tenant` is a new Vue.js 3 application dedicated to **tenant management** in B2Connect. It provides a portal for managing:

- **Store Instances**: Create and manage multiple online stores
- **Administrator Identities**: Invite and manage administrator users
- **Tenant Configuration**: Configure tenant-specific settings

## Quick Setup

### 1. Install Dependencies

```bash
cd frontend-tenant
npm install
```

Or use the VS Code task: `npm-install-tenant`

### 2. Start Development Server

```bash
npm run dev
```

Or use VS Code task: `dev-tenant`

The app will be available at: **http://localhost:5175**

### 3. Login

1. Navigate to http://localhost:5175
2. You'll be redirected to `/login`
3. Enter your credentials (requires backend auth service running)

## Available Features

### Dashboard (`/dashboard`)
- Overview of store instances
- Administrator count
- Quick action buttons

### Store Management (`/stores`)
- Create new store instances
- View all stores with status
- Edit store details
- Delete stores

### Administrator Management (`/admins`)
- Invite administrators
- Assign roles (TenantAdmin, StoreManager, SuperAdmin)
- View administrator details
- Manage administrator access

## Architecture

```
Frontend Portals (3 apps):
  ├─ frontend-store (Port 5173)     → Public storefront
  ├─ frontend-admin (Port 5174)     → Admin operations
  └─ frontend-tenant (Port 5175)    → Tenant management [NEW]

↓
API Gateways:
  ├─ Store Gateway (Port 8000)      → frontend-store
  └─ Admin Gateway (Port 8080)      → frontend-admin, frontend-tenant

↓
Bounded Contexts:
  ├─ Store/Catalog                  → Products
  ├─ Store/CMS                      → Content
  ├─ Admin/API                      → Operations
  └─ Shared (Identity, Tenancy)     → Cross-cutting
```

## VS Code Tasks

New tasks added to `.vscode/tasks.json`:

```
npm-install-tenant    → Install dependencies
dev-tenant            → Start dev server (Port 5175)
build-tenant          → Production build
test-tenant           → Run tests
lint-tenant           → Lint & format
```

## Environment Variables

Development settings are in `.env.development`:

```
VITE_API_BASE_URL=http://localhost:8080/api
VITE_APP_NAME=B2Connect Tenant Management
VITE_JWT_TOKEN_KEY=auth_token
VITE_APP_ENV=development
```

## File Structure

```
frontend-tenant/
├── src/
│   ├── pages/          → Page components (Login, Dashboard, Stores, Admins)
│   ├── components/     → Reusable components (Modals)
│   ├── stores/         → Pinia state (auth, stores, admins)
│   ├── services/       → API services
│   ├── router/         → Vue Router configuration
│   ├── App.vue         → Root component
│   └── main.ts         → Entry point
├── tests/              → Test files
├── package.json        → Dependencies
└── README.md           → Full documentation
```

## Technology Stack

- **Vue.js 3** (Composition API with TypeScript)
- **Vite** (Build tool)
- **Pinia** (State management)
- **Axios** (HTTP client)
- **Tailwind CSS** (Styling)
- **Vue Router** (Client routing)
- **Vitest** (Testing)

## Key Components

### Pages
- **LoginPage.vue** - User authentication
- **DashboardPage.vue** - Main overview
- **StoresPage.vue** - Store instance management
- **AdminsPage.vue** - Administrator management
- **StoreDetailPage.vue** - Store detail view (placeholder)
- **AdminDetailPage.vue** - Admin detail view (placeholder)
- **SettingsPage.vue** - Tenant settings (placeholder)

### Modals
- **CreateStoreModal.vue** - Create new store instance
- **InviteAdminModal.vue** - Invite new administrator

### Stores (Pinia)
- **authStore** - Authentication state
- **storeStore** - Store instances state
- **adminStore** - Administrators state

### Services
- **api.ts** - Axios client with JWT/tenant interceptors
- **storeService.ts** - Store API calls
- **adminService.ts** - Admin API calls

## API Integration

All requests automatically include:
- `Authorization: Bearer <jwt_token>` - JWT authentication
- `X-Tenant-ID: <tenant-id>` - Tenant isolation
- `Content-Type: application/json` - Content type

## Development Workflow

1. **Start backend services**
   ```bash
   dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj
   ```

2. **Start frontend-tenant**
   ```bash
   cd frontend-tenant
   npm run dev
   ```

3. **Develop** - Edit files in `src/`, changes auto-reload

4. **Test**
   ```bash
   npm run test
   ```

5. **Build for production**
   ```bash
   npm run build
   ```

## Next Steps

1. ✅ Install dependencies: `npm install`
2. ✅ Start dev server: `npm run dev`
3. ✅ Implement remaining detail pages
4. ✅ Add comprehensive tests
5. ✅ Connect to real backend APIs
6. ✅ Deploy to production

## Troubleshooting

**Port 5175 already in use?**
- Edit `vite.config.ts` and change `server.port` to another port
- Or kill the process: `lsof -ti:5175 | xargs kill -9`

**API connection errors?**
- Ensure backend is running on Port 8080
- Check browser console for CORS errors
- Verify `.env.development` API URL

**Module not found?**
- Delete `node_modules/` and run `npm install` again
- Clear Vite cache: `rm -rf node_modules/.vite`

## Documentation

Full documentation available at:
- [docs/FRONTEND_TENANT_SETUP.md](docs/FRONTEND_TENANT_SETUP.md)
- [frontend-tenant/README.md](frontend-tenant/README.md)

---

**Status**: ✅ Ready for Development  
**Created**: December 27, 2025
