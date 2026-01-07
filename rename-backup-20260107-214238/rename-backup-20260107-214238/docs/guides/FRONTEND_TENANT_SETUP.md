# Frontend-Tenant: Tenant Management Portal

**Status**: ✅ Ready for Development  
**Last Updated**: December 27, 2025  
**Port**: 5175 (Development)

---

## Overview

The **frontend-tenant** application is a dedicated portal for managing B2X tenant operations, specifically:

1. **Store Instance Management** - Create, configure, and manage multiple store instances
2. **Administrator Identity Management** - Manage administrator users and their permissions
3. **Tenant Configuration** - Configure tenant-specific settings and multi-tenancy features

This is the third and final frontend in the B2X system:
- **frontend-store** (Port 5173): Public-facing e-commerce storefront
- **frontend-admin** (Port 5174): Admin panel for product/content management
- **frontend-tenant** (Port 5175): Tenant management portal (NEW)

---

## Architecture

### Role in B2X

```
┌─────────────────────────────────────────────────────┐
│                   Frontends                         │
├─────────────────────────────────────────────────────┤
│  frontend-store  │  frontend-admin  │ frontend-tenant
│  (Storefront)    │  (Operations)    │  (Tenancy)
│  Port: 5173      │  Port: 5174      │  Port: 5175
└─────────────────────────────────────────────────────┘
          ↓                ↓                  ↓
┌─────────────────────────────────────────────────────┐
│                  API Gateways                       │
├─────────────────────────────────────────────────────┤
│            Store Gateway      │     Admin Gateway
│            Port: 8000         │     Port: 8080
└─────────────────────────────────────────────────────┘
          ↓                          ↓
┌─────────────────────────────────────────────────────┐
│              Bounded Contexts                       │
├─────────────────────────────────────────────────────┤
│  Store/Catalog  │  Store/CMS  │  Admin/API  │ Shared
│  Port: 7005     │  Port: 7006 │             │ Identity
│                 │             │             │ Tenancy
└─────────────────────────────────────────────────────┘
```

### Technology Stack

- **Framework**: Vue.js 3 (Composition API)
- **Language**: TypeScript
- **Build Tool**: Vite
- **State Management**: Pinia
- **HTTP Client**: Axios
- **Styling**: Tailwind CSS + custom CSS
- **Testing**: Vitest + Vue Test Utils
- **Router**: Vue Router

### Directory Structure

```
frontend-tenant/
├── src/
│   ├── App.vue                    # Root component with navigation
│   ├── main.ts                    # Application entry point
│   ├── style.css                  # Global styles
│   │
│   ├── components/
│   │   ├── CreateStoreModal.vue   # Create store instance modal
│   │   └── InviteAdminModal.vue   # Invite administrator modal
│   │
│   ├── pages/
│   │   ├── LoginPage.vue          # Login/authentication
│   │   ├── DashboardPage.vue      # Main dashboard
│   │   ├── StoresPage.vue         # Store instances list
│   │   ├── StoreDetailPage.vue    # Store detail view
│   │   ├── AdminsPage.vue         # Administrators list
│   │   ├── AdminDetailPage.vue    # Admin detail view
│   │   ├── SettingsPage.vue       # Tenant settings
│   │   └── NotFoundPage.vue       # 404 page
│   │
│   ├── stores/                    # Pinia state stores
│   │   ├── authStore.ts           # Authentication state
│   │   ├── storeStore.ts          # Store instances state
│   │   └── adminStore.ts          # Administrators state
│   │
│   ├── services/                  # API services
│   │   ├── api.ts                 # Axios client configuration
│   │   ├── adminService.ts        # Admin API calls
│   │   └── storeService.ts        # Store API calls
│   │
│   ├── router/
│   │   └── index.ts               # Vue Router configuration
│   │
│   └── types/                     # TypeScript types (future)
│
├── tests/                         # Test files
├── public/                        # Static assets
├── index.html                     # HTML entry point
├── vite.config.ts                 # Vite configuration
├── vitest.config.ts               # Vitest configuration
├── tsconfig.json                  # TypeScript configuration
├── tailwind.config.js             # Tailwind configuration
├── postcss.config.js              # PostCSS configuration
├── package.json                   # Dependencies
├── README.md                       # Project documentation
└── .env.*                         # Environment files

```

---

## Key Features

### 1. Store Instance Management

**Page**: `/stores`

Manage multiple store instances:
- Create new store instances with custom domain names
- View all store instances with status indicators
- Edit store configurations
- Delete store instances
- Monitor store statistics (products, orders, revenue)

**Store Instance Properties**:
- `id`: Unique identifier
- `name`: Display name
- `domain`: Custom domain (e.g., "mystore.example.com")
- `status`: active | inactive | suspended
- `tenantId`: Parent tenant reference
- `createdAt`: Creation timestamp
- `updatedAt`: Last modification timestamp

### 2. Administrator Management

**Page**: `/admins`

Manage administrator users:
- Invite new administrators via email
- Assign roles: TenantAdmin, StoreManager, SuperAdmin
- View all administrators with details
- Monitor administrator activity (last login)
- Edit administrator details
- Enable/disable administrator accounts
- Delete administrator accounts

**Administrator Properties**:
- `id`: Unique identifier
- `email`: Administrator email
- `firstName`: First name
- `lastName`: Last name
- `role`: TenantAdmin | StoreManager | SuperAdmin
- `tenantId`: Parent tenant
- `status`: active | inactive | suspended
- `lastLogin`: Last login timestamp
- `createdAt`: Creation timestamp
- `updatedAt`: Last modification timestamp

### 3. Dashboard

**Page**: `/dashboard`

Centralized overview:
- Welcome message with authenticated user
- Quick statistics (store count, admin count)
- Quick action buttons
- Navigation to key features

### 4. Authentication

**Page**: `/login`

Secure authentication:
- JWT token-based auth
- Token stored in localStorage
- Auto-logout on 401 response
- Password-based login
- Redirect to dashboard on success

### 5. Navigation

Main navigation bar with:
- Dashboard link
- Stores link
- Administrators link
- Settings link
- Logout button

---

## State Management (Pinia)

### Auth Store

```typescript
interface AuthState {
  token: string | null
  userId: string | null
  email: string | null
  isAuthenticated: boolean
}

// Methods
setAuth(token, userId, email)  // Set authenticated user
logout()                        // Clear auth state
```

### Store Store

```typescript
interface StoreInstance {
  id: string
  name: string
  tenantId: string
  domain: string
  status: 'active' | 'inactive' | 'suspended'
  createdAt: string
  updatedAt: string
}

// Methods
setStores(stores)              // Set list of stores
selectStore(store)             // Select active store
addStore(store)                // Add new store
updateStore(id, updates)       // Update store
deleteStore(id)                // Delete store
```

### Admin Store

```typescript
interface Administrator {
  id: string
  email: string
  firstName: string
  lastName: string
  role: 'SuperAdmin' | 'TenantAdmin' | 'StoreManager'
  tenantId: string
  status: 'active' | 'inactive' | 'suspended'
  lastLogin?: string
  createdAt: string
  updatedAt: string
}

// Methods
setAdmins(admins)              // Set list of admins
selectAdmin(admin)             // Select active admin
addAdmin(admin)                // Add new admin
updateAdmin(id, updates)       // Update admin
deleteAdmin(id)                // Delete admin
```

---

## API Integration

### Base Configuration

```typescript
// api.ts
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Automatic token injection
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Auto-logout on 401
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('auth_token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)
```

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Login with credentials
- `POST /api/auth/logout` - Logout
- `POST /api/auth/refresh` - Refresh token

#### Store Management
- `GET /api/tenant/stores` - List stores (requires X-Tenant-ID header)
- `GET /api/tenant/stores/:id` - Get store details
- `POST /api/tenant/stores` - Create store
- `PUT /api/tenant/stores/:id` - Update store
- `DELETE /api/tenant/stores/:id` - Delete store
- `GET /api/tenant/stores/:id/stats` - Get store statistics

#### Administrator Management
- `GET /api/tenant/admins` - List administrators
- `GET /api/tenant/admins/:id` - Get admin details
- `POST /api/tenant/admins` - Create administrator
- `PUT /api/tenant/admins/:id` - Update administrator
- `DELETE /api/tenant/admins/:id` - Delete administrator
- `POST /api/tenant/admins/invite` - Send invitation email

### Required Headers

All API requests must include:
```
Authorization: Bearer <jwt_token>
X-Tenant-ID: <tenant-uuid>
Content-Type: application/json
```

---

## Routes

| Route | Component | Auth Required | Purpose |
|-------|-----------|---------------|---------|
| `/login` | LoginPage | ❌ | User authentication |
| `/` | → /dashboard | ✅ | Redirect to dashboard |
| `/dashboard` | DashboardPage | ✅ | Main dashboard |
| `/stores` | StoresPage | ✅ | Store instances list |
| `/stores/:id` | StoreDetailPage | ✅ | Store details |
| `/admins` | AdminsPage | ✅ | Administrators list |
| `/admins/:id` | AdminDetailPage | ✅ | Admin details |
| `/settings` | SettingsPage | ✅ | Tenant settings |
| `/*` | NotFoundPage | ❌ | 404 page |

---

## Environment Variables

### Development (`.env.development`)

```
VITE_API_BASE_URL=http://localhost:8080/api
VITE_APP_NAME=B2X Tenant Management
VITE_JWT_TOKEN_KEY=auth_token
VITE_APP_ENV=development
```

### Production (`.env.production`)

```
VITE_API_BASE_URL=/api
VITE_APP_NAME=B2X Tenant Management
VITE_JWT_TOKEN_KEY=auth_token
VITE_APP_ENV=production
```

---

## Development Workflow

### Install Dependencies

```bash
npm install
```

### Start Development Server

```bash
npm run dev
```

Server runs at `http://localhost:5175`

### Build for Production

```bash
npm run build
```

Output: `dist/` directory

### Run Tests

```bash
npm run test
```

### View Test Coverage

```bash
npm run test:coverage
```

### Run Tests in UI

```bash
npm run test:ui
```

### Lint & Format

```bash
npm run lint
```

### Type Checking

```bash
npm run type-check
```

---

## VS Code Tasks

Added to `.vscode/tasks.json`:

| Task | Command | Purpose |
|------|---------|---------|
| `npm-install-tenant` | Install npm dependencies | Setup |
| `dev-tenant` | Start dev server | Development |
| `build-tenant` | Build for production | Deployment |
| `test-tenant` | Run tests | Testing |
| `lint-tenant` | Lint & format code | Code quality |

**Usage**: Press `Ctrl+Shift+B` (or `Cmd+Shift+B` on Mac) to run default build task.

---

## Security Considerations

### ✅ Implemented

- JWT token-based authentication
- Token stored in localStorage (production should use httpOnly cookies)
- Automatic token injection in all API requests
- 401 response handling (auto-logout)
- CORS configuration per environment
- Tenant ID validation via header
- Input validation in forms
- XSS protection via Vue template escaping

### 🔄 Recommended for Production

- Upgrade to httpOnly cookies for token storage
- Implement CSRF token validation
- Add rate limiting on frontend
- Implement token refresh flow
- Add password strength validation
- Implement 2FA/MFA support
- Add audit logging of admin actions
- Implement role-based feature access

---

## Testing Strategy

### Unit Tests

- Component tests with Vue Test Utils
- Store tests with Pinia
- Service tests with Axios mocks

### Integration Tests

- Full user workflows (login → manage stores → logout)
- API error handling
- State management integration

### E2E Tests (Future)

- Playwright tests for critical flows
- Cross-browser testing
- Performance testing

---

## Deployment

### Prerequisites

- Node.js 18+ and npm 9+
- Build output: `dist/` directory
- Static file server (Nginx, Apache, etc.)

### Build Steps

```bash
# Install dependencies
npm install

# Build for production
npm run build

# Output in dist/
# Use dist/ as static file server root
```

### Hosting Options

1. **AWS S3 + CloudFront**
   - Upload `dist/` to S3
   - Configure CloudFront for HTTPS
   - Set SPA redirect rules

2. **Azure Static Web Apps**
   - Connect GitHub repo
   - Auto-deploy on push
   - Free HTTPS

3. **Vercel / Netlify**
   - Connect GitHub
   - Auto-build and deploy
   - Built-in CDN

4. **Traditional Server (Nginx)**
   ```nginx
   server {
     listen 80;
     server_name tenant.example.com;
     
     root /var/www/dist;
     index index.html;
     
     location / {
       try_files $uri $uri/ /index.html;
     }
   }
   ```

---

## Troubleshooting

### Module Not Found

```bash
# Clear node_modules and reinstall
rm -rf node_modules
npm install
```

### Port Already in Use

```bash
# Change port in vite.config.ts
server: {
  port: 5176  // Use different port
}
```

### Build Fails

```bash
# Check TypeScript errors
npm run type-check

# Check linting issues
npm run lint

# Clear Vite cache
rm -rf node_modules/.vite
```

### API Connection Issues

- Verify backend is running on port 8080
- Check `.env.development` API URL
- Check browser console for CORS errors
- Verify X-Tenant-ID header is sent

---

## Integration with Backend

The frontend-tenant expects the following backend APIs to be available:

### Required Services

1. **Identity Service** (Port 7002)
   - Authentication endpoints
   - User management

2. **Admin API Gateway** (Port 8080)
   - Store management endpoints
   - Administrator management endpoints
   - Tenant configuration endpoints

3. **Tenant Service** (Port 7003)
   - Multi-tenancy validation
   - Tenant isolation enforcement

### CORS Configuration

Backend must allow requests from frontend-tenant origin:
- Development: `http://localhost:5175`
- Production: `https://tenant.example.com`

### Authentication Flow

```
1. User enters credentials
   ↓
2. POST /api/auth/login
   ↓
3. Backend validates and returns JWT
   ↓
4. Frontend stores token in localStorage
   ↓
5. All subsequent requests include Authorization header
   ↓
6. Backend validates JWT and returns data
```

---

## Future Enhancements

- [ ] Real-time notifications for admin actions
- [ ] Advanced filtering and search
- [ ] Bulk operations (delete multiple)
- [ ] Export functionality (CSV, PDF)
- [ ] Audit log viewer
- [ ] Advanced tenant settings UI
- [ ] Multi-language support (i18n)
- [ ] Dark mode toggle
- [ ] Mobile responsive improvements
- [ ] Progressive Web App (PWA) features
- [ ] Offline support

---

## Contributing

1. Create feature branch: `git checkout -b feature/my-feature`
2. Write tests for new features
3. Format code: `npm run lint`
4. Commit: `git commit -m "feat: description"`
5. Push and create PR

---

## Related Documentation

- [Application Specifications](../docs/APPLICATION_SPECIFICATIONS.md)
- [Developer Guide](DEVELOPER_GUIDE.md)
- [Frontend Feature Integration](FRONTEND_FEATURE_INTEGRATION_GUIDE.md)
- [Testing Strategy](TESTING_STRATEGY.md)

---

**Created**: December 27, 2025  
**Status**: ✅ Ready for Development
