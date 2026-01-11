# frontend-tenant

B2X Tenant Management Portal - Manage store instances and administrator identities.

## Overview

The tenant management frontend provides a dedicated portal for:

- **Store Instance Management**: Create, view, edit, and delete store instances
- **Administrator Identity Management**: Manage administrator users and their roles
- **Tenant Configuration**: Configure tenant-specific settings and branding

## Features

### Dashboard

- Overview of store instances and administrators
- Quick access to key management functions
- Real-time statistics and metrics

### Store Management

- Create and manage multiple store instances
- Monitor store status (active, inactive, suspended)
- View store-specific metrics and statistics
- Manage store domain configuration

### Administrator Management

- Invite and manage administrator users
- Assign roles (TenantAdmin, StoreManager, SuperAdmin)
- Track administrator activity and last login
- Enable/disable administrator accounts

### Security

- JWT-based authentication
- Role-based access control (RBAC)
- Tenant isolation via X-Tenant-ID header
- Secure password management

## Tech Stack

- **Frontend Framework**: Vue.js 3 with TypeScript
- **Build Tool**: Vite
- **State Management**: Pinia
- **HTTP Client**: Axios
- **Styling**: Tailwind CSS
- **Testing**: Vitest + Vue Test Utils

## Getting Started

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

The application will start at `http://localhost:5175`

### Build

```bash
npm run build
```

### Testing

```bash
npm run test
npm run test:coverage
npm run test:ui
```

## Architecture

### Directory Structure

```
frontend-tenant/
├── src/
│   ├── components/     # Reusable Vue components
│   ├── pages/          # Page components
│   ├── stores/         # Pinia state stores
│   ├── services/       # API services
│   ├── router/         # Vue Router configuration
│   ├── types/          # TypeScript type definitions
│   ├── App.vue         # Root component
│   ├── main.ts         # Entry point
│   └── style.css       # Global styles
├── tests/              # Test files
├── public/             # Static assets
└── package.json        # Dependencies
```

### State Management (Pinia Stores)

- **authStore**: Authentication state and login management
- **storeStore**: Store instance management and selection
- **adminStore**: Administrator management and selection

### API Services

- **api.ts**: Configured Axios client with interceptors
- **adminService.ts**: Administrator-related API calls
- **storeService.ts**: Store-related API calls

### Router

Protected routes that require authentication:

- `/dashboard` - Main dashboard
- `/stores` - Store management
- `/stores/:id` - Store details
- `/admins` - Administrator management
- `/admins/:id` - Administrator details
- `/settings` - Tenant settings

Public routes:

- `/login` - Login page

## Environment Variables

Create `.env.local` or use environment-specific `.env.development`:

```
VITE_API_BASE_URL=http://localhost:8080/api
VITE_APP_NAME=B2X Tenant Management
VITE_JWT_TOKEN_KEY=auth_token
VITE_APP_ENV=development
```

## API Integration

The frontend connects to the B2X backend APIs:

### Base URL

- Development: `http://localhost:8080/api`
- Production: `/api` (relative path)

### Headers

- `Authorization: Bearer <jwt_token>`
- `X-Tenant-ID: <tenant-id>` (for tenant-scoped requests)
- `Content-Type: application/json`

### Authentication Flow

1. User logs in via `/api/auth/login`
2. JWT token is stored in localStorage
3. Token is automatically included in all API requests
4. On 401 response, user is redirected to login

## Testing

### Unit Tests

```bash
npm run test
```

### Test Coverage

```bash
npm run test:coverage
```

### UI Test Browser

```bash
npm run test:ui
```

## Deployment

### Production Build

```bash
npm run build
```

Output goes to `dist/` directory.

### Hosting Options

- Static file server (Nginx, Apache)
- Azure Static Web Apps
- AWS S3 + CloudFront
- Vercel, Netlify, etc.

### Environment Configuration

Update `.env.production` with production API URL before building.

## Security Considerations

- ✅ JWT tokens stored in localStorage (consider httpOnly cookies for production)
- ✅ All API calls include CSRF protection headers
- ✅ Tenant ID validation via JWT claims
- ✅ Role-based access control on frontend
- ✅ Input validation before submission
- ✅ XSS protection via Vue's template escaping

## Performance

- ✅ Code splitting via lazy-loaded routes
- ✅ Tree-shaking for unused imports
- ✅ CSS purging via Tailwind
- ✅ Image optimization recommended
- ✅ Gzip compression enabled on server

## Contributing

1. Create feature branch: `git checkout -b feature/my-feature`
2. Write tests for new features
3. Format code: `npm run lint`
4. Commit changes: `git commit -m "feat: description"`
5. Push branch and create pull request

## License

Copyright © 2025 B2X. All rights reserved.

## Support

For issues or questions:

1. Check existing GitHub issues
2. Create new issue with detailed description
3. Contact support@B2X.com
