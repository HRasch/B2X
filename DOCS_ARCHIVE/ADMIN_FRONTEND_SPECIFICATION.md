# Admin Frontend Specification für B2Connect

## 1. Übersicht

### 1.1 Zweck
Das Admin-Frontend ist eine Enterprise-Grade-Anwendung für Administratoren und Content-Manager zur:
- **Konfiguration** von Shop-Inhalten und Systemparametern
- **Monitoring** von Background Jobs und Systemgesundheit
- **CMS-Management** für Seiten, Kategorien und Dynamic Content
- **Benutzer- und Tenant-Verwaltung**
- **Reporting und Analytics**
- **Suchmaschinenoptimierung (SEO)**

### 1.2 Zielgruppen
- **Super Admins**: Globale Systemverwaltung, Tenants, Lizenzen
- **Tenant Admins**: Tenant-spezifische Konfiguration
- **Content Manager**: CMS-Inhalte, Kampagnen
- **Shop Manager**: Katalog, Preise, Rabatte
- **Operations**: Monitoring, Fehlerbehandlung

### 1.3 Architekturäbersicht
```
Admin Frontend (Single Page Application)
├── Auth Module (SSO, RBAC)
├── Dashboard
├── Content Management (CMS)
├── Shop Configuration
├── Job Monitoring
├── Analytics & Reporting
├── Tenant Management
└── System Settings
```

## 2. Frontend-Architektur

### 2.1 Projektstruktur

```
frontend-admin/
├── src/
│   ├── components/
│   │   ├── common/                     # Globale Komponenten
│   │   │   ├── Navbar.vue
│   │   │   ├── Sidebar.vue
│   │   │   ├── MainLayout.vue
│   │   │   ├── LoadingSpinner.vue
│   │   │   ├── ErrorAlert.vue
│   │   │   └── ConfirmDialog.vue
│   │   ├── auth/
│   │   │   ├── LoginForm.vue
│   │   │   ├── LogoutButton.vue
│   │   │   └── PermissionGuard.vue
│   │   ├── cms/                       # CMS-Komponenten
│   │   │   ├── PageEditor.vue
│   │   │   ├── BlockLibrary.vue
│   │   │   ├── BlockEditor.vue
│   │   │   ├── ContentPreview.vue
│   │   │   ├── SEOEditor.vue
│   │   │   └── VersionHistory.vue
│   │   ├── shop/                      # Shop-Konfiguration
│   │   │   ├── CatalogManager.vue
│   │   │   ├── PricingRules.vue
│   │   │   ├── DiscountManager.vue
│   │   │   ├── CategoryEditor.vue
│   │   │   └── ProductBulkUpload.vue
│   │   ├── jobs/                      # Job-Monitoring
│   │   │   ├── JobMonitor.vue
│   │   │   ├── JobDetail.vue
│   │   │   ├── JobHistory.vue
│   │   │   └── JobScheduler.vue
│   │   ├── analytics/                 # Analytics & Reporting
│   │   │   ├── AnalyticsDashboard.vue
│   │   │   ├── ReportBuilder.vue
│   │   │   ├── ChartComponents.vue
│   │   │   └── ExportManager.vue
│   │   ├── tenants/                   # Tenant-Management
│   │   │   ├── TenantList.vue
│   │   │   ├── TenantForm.vue
│   │   │   └── TenantSettings.vue
│   │   └── system/                    # System-Einstellungen
│   │       ├── SystemHealth.vue
│   │       ├── ConfigSettings.vue
│   │       ├── UserManagement.vue
│   │       └── AuditLog.vue
│   ├── views/                         # Page-Level Komponenten (Routes)
│   │   ├── Dashboard.vue
│   │   ├── cms/
│   │   │   ├── Pages.vue
│   │   │   ├── PageDetail.vue
│   │   │   ├── Templates.vue
│   │   │   └── MediaLibrary.vue
│   │   ├── shop/
│   │   │   ├── Products.vue
│   │   │   ├── Categories.vue
│   │   │   ├── Pricing.vue
│   │   │   └── Inventory.vue
│   │   ├── jobs/
│   │   │   ├── JobQueue.vue
│   │   │   ├── JobLogs.vue
│   │   │   └── Scheduling.vue
│   │   ├── analytics/
│   │   │   ├── Reports.vue
│   │   │   ├── Metrics.vue
│   │   │   └── Exports.vue
│   │   ├── tenants/
│   │   │   ├── Tenants.vue
│   │   │   └── TenantDetail.vue
│   │   ├── settings/
│   │   │   ├── System.vue
│   │   │   ├── Users.vue
│   │   │   ├── Roles.vue
│   │   │   └── Security.vue
│   │   └── Login.vue
│   ├── composables/
│   │   ├── useAuth.ts                 # Authentication logic
│   │   ├── usePagination.ts           # Pagination logic
│   │   ├── useSearch.ts               # Search/filter logic
│   │   ├── useBulkActions.ts          # Bulk operation logic
│   │   ├── useFormValidation.ts       # Form validation
│   │   ├── useDragDrop.ts             # Drag-and-drop functionality
│   │   └── useNotification.ts         # Toast/notification system
│   ├── stores/                        # Pinia state management
│   │   ├── auth.ts                    # Auth state
│   │   ├── tenant.ts                  # Current tenant state
│   │   ├── cms.ts                     # CMS content state
│   │   ├── shop.ts                    # Shop configuration state
│   │   ├── jobs.ts                    # Jobs state
│   │   ├── analytics.ts               # Analytics data
│   │   ├── notifications.ts           # Notification queue
│   │   └── settings.ts                # System settings
│   ├── services/                      # API clients
│   │   ├── api/
│   │   │   ├── auth.ts                # Auth API client
│   │   │   ├── cms.ts                 # CMS API client
│   │   │   ├── shop.ts                # Shop API client
│   │   │   ├── jobs.ts                # Jobs API client
│   │   │   ├── analytics.ts           # Analytics API client
│   │   │   ├── tenants.ts             # Tenant API client
│   │   │   └── system.ts              # System API client
│   │   ├── client.ts                  # Axios instance configuration
│   │   └── websocket.ts               # WebSocket for real-time updates
│   ├── types/                         # TypeScript interfaces
│   │   ├── auth.ts                    # Auth types
│   │   ├── cms.ts                     # CMS types
│   │   ├── shop.ts                    # Shop types
│   │   ├── jobs.ts                    # Job types
│   │   ├── analytics.ts               # Analytics types
│   │   ├── pagination.ts              # Pagination types
│   │   ├── api.ts                     # Common API types
│   │   └── entity.ts                  # Entity types
│   ├── utils/
│   │   ├── format.ts                  # Date, currency, etc.
│   │   ├── validation.ts              # Form validation rules
│   │   ├── constants.ts               # App constants
│   │   ├── permissions.ts             # Permission utilities
│   │   └── helpers.ts                 # Utility functions
│   ├── directives/
│   │   ├── v-focus.ts                 # Auto-focus directive
│   │   ├── v-loading.ts               # Loading state directive
│   │   └── v-permission.ts            # Permission directive
│   ├── middleware/
│   │   ├── auth.ts                    # Auth guard
│   │   ├── tenant.ts                  # Tenant context middleware
│   │   └── permission.ts              # Permission guard
│   ├── router/
│   │   ├── index.ts                   # Router configuration
│   │   └── routes.ts                  # Route definitions
│   ├── App.vue                        # Root component
│   ├── main.ts                        # Entry point
│   └── main.css                       # Global styles
├── tests/
│   ├── unit/
│   │   ├── composables/
│   │   ├── stores/
│   │   ├── services/
│   │   └── utils/
│   ├── components/
│   │   ├── cms/
│   │   ├── shop/
│   │   ├── jobs/
│   │   └── common/
│   └── e2e/
│       ├── auth.spec.ts
│       ├── cms.spec.ts
│       ├── shop.spec.ts
│       ├── jobs.spec.ts
│       └── analytics.spec.ts
├── public/
│   ├── images/
│   ├── icons/
│   └── svg/
├── vite.config.ts
├── tsconfig.json
├── vitest.config.ts
├── playwright.config.ts
├── package.json
├── .env.example
└── .prettierrc
```

## 3. Kernmodule

### 3.1 Dashboard-Modul

**Komponenten:**
- Real-time Systemgesundheit (CPU, Memory, Disk)
- Aktive Jobs mit Status
- Neueste Fehler und Warnungen
- Quick-Links zu häufig genutzten Funktionen
- Benachrichtigungen und Aufgaben

**Datenquellen:**
- WebSocket für Live-Metriken
- Real-time Job-Status-Updates
- Fehlerlog-Streaming

### 3.2 CMS-Modul

**Funktionen:**
- **Page Management**
  - WYSIWYG Editor für Seiteninhalte
  - Drag-and-Drop Block-Editor
  - Template-System für wiederverwendbare Layouts
  - Preview vor Publikation
  - Version History und Rollback
  
- **Content Blocks**
  - Text/Rich-Text-Block
  - Image/Gallery Block
  - Video Block
  - Tabellen-Block
  - Custom HTML Block
  - Product Grid Block
  
- **SEO Management**
  - Meta-Tags Editor
  - URL Slug Management
  - Sitemap-Generator
  - Meta-Beschreibung
  - OpenGraph Tags
  
- **Media Library**
  - Image Upload/Cropping
  - Video Management
  - Asset Organization
  - CDN Integration

- **Lokalisierung**
  - Multi-language Content
  - Language-spezifische Drafts
  - Translation Workflow

**Datenquellen:**
```
GET /api/admin/cms/pages
GET /api/admin/cms/pages/{id}
POST /api/admin/cms/pages
PUT /api/admin/cms/pages/{id}
DELETE /api/admin/cms/pages/{id}
POST /api/admin/cms/pages/{id}/publish
GET /api/admin/cms/pages/{id}/versions
POST /api/admin/cms/pages/{id}/versions/{version}/restore

GET /api/admin/cms/templates
POST /api/admin/cms/templates

GET /api/admin/cms/media
POST /api/admin/cms/media/upload
DELETE /api/admin/cms/media/{id}
```

### 3.3 Shop-Konfiguration

**Funktionen:**
- **Katalog-Management**
  - Produkt-CRUD
  - Kategorie-Management
  - Attribute und Varianten
  - SKU Management
  - Bulk Operations (Import/Export)
  
- **Preise und Rabatte**
  - Basis-Preise
  - Tierpreise (Volume-Discount)
  - Dynamische Preisregeln
  - Promotionen und Kampagnen
  - Coupon-Management
  
- **Inventur**
  - Stock-Level Management
  - Lagerort-Verwaltung
  - Stock-Alerts
  - Reservierungen
  
- **Kategorien**
  - Hierarchische Struktur
  - Category Meta (SEO)
  - Category Images
  - Filter und Facets

**Datenquellen:**
```
GET /api/admin/shop/products
GET /api/admin/shop/products/{id}
POST /api/admin/shop/products
PUT /api/admin/shop/products/{id}
DELETE /api/admin/shop/products/{id}
POST /api/admin/shop/products/bulk-import

GET /api/admin/shop/categories
POST /api/admin/shop/categories

GET /api/admin/shop/pricing/rules
POST /api/admin/shop/pricing/rules

GET /api/admin/shop/inventory
PUT /api/admin/shop/inventory/{productId}
```

### 3.4 Job-Monitoring

**Funktionen:**
- **Job Dashboard**
  - Real-time Job-Queue Anzeige
  - Job-Status (Pending, Running, Completed, Failed)
  - Progress Tracking
  - Durchsatzmetriken
  
- **Job Scheduler**
  - Recurring Jobs konfigurieren
  - Zeitbasierte Trigger
  - Webhook-Trigger
  
- **Job History**
  - Ausführungs-Logs
  - Performance-Metriken
  - Fehlerbehandlung und Retry-Logik
  - Audit-Trail
  
- **Verschiedene Job-Typen**
  - Data Synchronization Jobs
  - Report Generation
  - Email Campaigns
  - Image Processing
  - ETL Operations
  - Backup Jobs

**Datenquellen:**
```
GET /api/admin/jobs/queue
GET /api/admin/jobs/{id}
GET /api/admin/jobs/{id}/logs
POST /api/admin/jobs/{id}/retry
DELETE /api/admin/jobs/{id}/cancel

GET /api/admin/jobs/scheduled
POST /api/admin/jobs/scheduled
PUT /api/admin/jobs/scheduled/{id}
DELETE /api/admin/jobs/scheduled/{id}

WS /api/admin/jobs/stream (WebSocket für Live-Updates)
```

### 3.5 Analytics & Reporting

**Funktionen:**
- **Pre-built Reports**
  - Sales Reports
  - Traffic Reports
  - Conversion Funnels
  - Customer Segments
  - Product Performance
  
- **Report Builder**
  - Custom Metric Selection
  - Filter/Group By
  - Time-Range Selection
  - Visualization Options
  
- **Dashboards**
  - Sales Dashboard
  - Traffic Dashboard
  - Customer Dashboard
  - Inventory Dashboard
  
- **Export Optionen**
  - PDF Export
  - Excel Export
  - CSV Export
  - Scheduled Report Delivery

**Datenquellen:**
```
GET /api/admin/analytics/sales
GET /api/admin/analytics/traffic
GET /api/admin/analytics/customers
GET /api/admin/analytics/products
GET /api/admin/analytics/reports
POST /api/admin/analytics/reports
GET /api/admin/analytics/reports/{id}/export
```

### 3.6 Tenant-Management

**Funktionen:**
- **Tenant CRUD**
  - Tenant erstellen/editieren
  - Tenant-Einstellungen
  - Branding (Logo, Farben)
  - Domain-Verwaltung
  
- **Tenant Isolation**
  - Daten-Zugriffskontrolle
  - Tenant-spezifische Features
  - Storage Limits
  
- **Tenant Analytics**
  - Usage Statistics
  - Active Users
  - API Calls

**Datenquellen:**
```
GET /api/admin/tenants
GET /api/admin/tenants/{id}
POST /api/admin/tenants
PUT /api/admin/tenants/{id}
DELETE /api/admin/tenants/{id}
```

### 3.7 System-Einstellungen

**Funktionen:**
- **System Configuration**
  - Email Settings
  - Payment Gateway Settings
  - API Keys Management
  - Feature Flags
  - System Parameters
  
- **User Management**
  - Admin User CRUD
  - Role-Based Access Control (RBAC)
  - Permission Management
  - Two-Factor Authentication
  
- **Security**
  - Password Policies
  - Session Management
  - Audit Logging
  - API Rate Limiting
  
- **Backup & Recovery**
  - System Backup
  - Recovery Procedures

**Datenquellen:**
```
GET /api/admin/system/settings
PUT /api/admin/system/settings

GET /api/admin/users
POST /api/admin/users
PUT /api/admin/users/{id}
DELETE /api/admin/users/{id}

GET /api/admin/roles
POST /api/admin/roles
PUT /api/admin/roles/{id}

GET /api/admin/audit-logs
```

## 4. State Management mit Pinia

### 4.1 Auth Store

```typescript
// stores/auth.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AdminUser, Permission, Role } from '@/types/auth'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AdminUser | null>(null)
  const permissions = ref<Permission[]>([])
  const roles = ref<Role[]>([])
  const isAuthenticated = computed(() => user.value !== null)

  async function login(email: string, password: string) {
    // Implementation
  }

  async function logout() {
    // Implementation
  }

  function hasPermission(permission: string): boolean {
    // Implementation
  }

  function hasRole(role: string): boolean {
    // Implementation
  }

  return {
    user,
    permissions,
    roles,
    isAuthenticated,
    login,
    logout,
    hasPermission,
    hasRole,
  }
})
```

### 4.2 CMS Store

```typescript
// stores/cms.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Page, Template, MediaItem } from '@/types/cms'

export const useCmsStore = defineStore('cms', () => {
  const pages = ref<Page[]>([])
  const templates = ref<Template[]>([])
  const mediaItems = ref<MediaItem[]>([])
  const currentPage = ref<Page | null>(null)
  const loading = ref(false)

  const pageCount = computed(() => pages.value.length)

  async function fetchPages(filters?: any) {
    loading.value = true
    try {
      // API call implementation
    } finally {
      loading.value = false
    }
  }

  async function savePage(page: Page) {
    // API call implementation
  }

  async function publishPage(pageId: string) {
    // API call implementation
  }

  return {
    pages,
    templates,
    mediaItems,
    currentPage,
    loading,
    pageCount,
    fetchPages,
    savePage,
    publishPage,
  }
})
```

### 4.3 Jobs Store

```typescript
// stores/jobs.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Job, JobLog } from '@/types/jobs'

export const useJobsStore = defineStore('jobs', () => {
  const jobs = ref<Job[]>([])
  const currentJob = ref<Job | null>(null)
  const jobLogs = ref<JobLog[]>([])
  const isMonitoring = ref(false)

  const runningJobs = computed(() => 
    jobs.value.filter(j => j.status === 'running')
  )

  const failedJobs = computed(() =>
    jobs.value.filter(j => j.status === 'failed')
  )

  async function fetchJobs() {
    // Implementation
  }

  async function startMonitoring() {
    // WebSocket connection
  }

  async function retryJob(jobId: string) {
    // Implementation
  }

  return {
    jobs,
    currentJob,
    jobLogs,
    isMonitoring,
    runningJobs,
    failedJobs,
    fetchJobs,
    startMonitoring,
    retryJob,
  }
})
```

## 5. Routing

```typescript
// router/routes.ts
export const routes = [
  {
    path: '/login',
    component: () => import('@/views/Login.vue'),
    meta: { requiresAuth: false },
  },
  {
    path: '/dashboard',
    component: () => import('@/views/Dashboard.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/cms',
    meta: { requiresAuth: true, requiresRole: 'content_manager' },
    children: [
      {
        path: 'pages',
        component: () => import('@/views/cms/Pages.vue'),
      },
      {
        path: 'pages/:id',
        component: () => import('@/views/cms/PageDetail.vue'),
      },
      {
        path: 'templates',
        component: () => import('@/views/cms/Templates.vue'),
      },
      {
        path: 'media',
        component: () => import('@/views/cms/MediaLibrary.vue'),
      },
    ],
  },
  {
    path: '/shop',
    meta: { requiresAuth: true, requiresRole: 'shop_manager' },
    children: [
      {
        path: 'products',
        component: () => import('@/views/shop/Products.vue'),
      },
      {
        path: 'categories',
        component: () => import('@/views/shop/Categories.vue'),
      },
      {
        path: 'pricing',
        component: () => import('@/views/shop/Pricing.vue'),
      },
    ],
  },
  {
    path: '/jobs',
    meta: { requiresAuth: true, requiresRole: 'operator' },
    children: [
      {
        path: 'queue',
        component: () => import('@/views/jobs/JobQueue.vue'),
      },
      {
        path: 'history',
        component: () => import('@/views/jobs/JobLogs.vue'),
      },
    ],
  },
  {
    path: '/analytics',
    meta: { requiresAuth: true, requiresRole: 'analyst' },
    component: () => import('@/views/analytics/Reports.vue'),
  },
  {
    path: '/tenants',
    meta: { requiresAuth: true, requiresRole: 'super_admin' },
    component: () => import('@/views/tenants/Tenants.vue'),
  },
  {
    path: '/settings',
    meta: { requiresAuth: true, requiresRole: 'super_admin' },
    children: [
      {
        path: 'system',
        component: () => import('@/views/settings/System.vue'),
      },
      {
        path: 'users',
        component: () => import('@/views/settings/Users.vue'),
      },
      {
        path: 'security',
        component: () => import('@/views/settings/Security.vue'),
      },
    ],
  },
]
```

## 6. API-Integration

### 6.1 Axios Client Setup

```typescript
// services/client.ts
import axios, { AxiosInstance } from 'axios'
import type { AxiosRequestConfig } from 'axios'

class ApiClient {
  private instance: AxiosInstance

  constructor(baseURL: string) {
    this.instance = axios.create({
      baseURL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    this.setupInterceptors()
  }

  private setupInterceptors() {
    // Request Interceptor
    this.instance.interceptors.request.use((config) => {
      const token = localStorage.getItem('authToken')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    })

    // Response Interceptor
    this.instance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Handle unauthorized
        }
        return Promise.reject(error)
      }
    )
  }

  public get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return this.instance.get(url, config).then(res => res.data)
  }

  public post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return this.instance.post(url, data, config).then(res => res.data)
  }

  public put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return this.instance.put(url, data, config).then(res => res.data)
  }

  public delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return this.instance.delete(url, config).then(res => res.data)
  }
}

export const apiClient = new ApiClient(import.meta.env.VITE_ADMIN_API_URL)
```

### 6.2 Service-Layer Pattern

```typescript
// services/api/cms.ts
import { apiClient } from '../client'
import type { Page, Template } from '@/types/cms'

export const cmsApi = {
  // Pages
  getPages(filters?: any) {
    return apiClient.get<Page[]>('/admin/cms/pages', { params: filters })
  },

  getPage(id: string) {
    return apiClient.get<Page>(`/admin/cms/pages/${id}`)
  },

  createPage(data: Omit<Page, 'id'>) {
    return apiClient.post<Page>('/admin/cms/pages', data)
  },

  updatePage(id: string, data: Partial<Page>) {
    return apiClient.put<Page>(`/admin/cms/pages/${id}`, data)
  },

  deletePage(id: string) {
    return apiClient.delete(`/admin/cms/pages/${id}`)
  },

  publishPage(id: string) {
    return apiClient.post(`/admin/cms/pages/${id}/publish`, {})
  },

  // Versions
  getPageVersions(id: string) {
    return apiClient.get(`/admin/cms/pages/${id}/versions`)
  },

  restorePageVersion(id: string, version: number) {
    return apiClient.post(`/admin/cms/pages/${id}/versions/${version}/restore`, {})
  },

  // Templates
  getTemplates() {
    return apiClient.get<Template[]>('/admin/cms/templates')
  },

  createTemplate(data: Omit<Template, 'id'>) {
    return apiClient.post<Template>('/admin/cms/templates', data)
  },
}
```

## 7. WebSocket Integration für Real-time Updates

```typescript
// services/websocket.ts
export class WebSocketService {
  private ws: WebSocket | null = null
  private url: string
  private reconnectAttempts = 0
  private maxReconnectAttempts = 5

  constructor(url: string) {
    this.url = url
  }

  connect(): Promise<void> {
    return new Promise((resolve, reject) => {
      try {
        this.ws = new WebSocket(this.url)

        this.ws.onopen = () => {
          console.log('WebSocket connected')
          this.reconnectAttempts = 0
          resolve()
        }

        this.ws.onerror = (error) => {
          console.error('WebSocket error:', error)
          reject(error)
        }

        this.ws.onclose = () => {
          this.attemptReconnect()
        }
      } catch (error) {
        reject(error)
      }
    })
  }

  subscribe(channel: string, callback: (data: any) => void) {
    if (!this.ws) return

    this.ws.onmessage = (event) => {
      const message = JSON.parse(event.data)
      if (message.channel === channel) {
        callback(message.data)
      }
    }
  }

  send(action: string, data: any) {
    if (this.ws?.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify({ action, data }))
    }
  }

  private attemptReconnect() {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++
      const delay = Math.pow(2, this.reconnectAttempts) * 1000
      setTimeout(() => this.connect(), delay)
    }
  }

  disconnect() {
    if (this.ws) {
      this.ws.close()
      this.ws = null
    }
  }
}

export const wsService = new WebSocketService(
  `${import.meta.env.VITE_ADMIN_WS_URL}`
)
```

## 8. Authentifizierung und Autorisierung

### 8.1 Auth Middleware

```typescript
// middleware/auth.ts
import { Router } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

export function setupAuthMiddleware(router: Router) {
  router.beforeEach((to, from, next) => {
    const authStore = useAuthStore()

    const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
    const requiredRole = to.meta.requiresRole as string | undefined

    if (requiresAuth && !authStore.isAuthenticated) {
      next('/login')
      return
    }

    if (requiredRole && !authStore.hasRole(requiredRole)) {
      next('/unauthorized')
      return
    }

    next()
  })
}
```

### 8.2 Permission Utilities

```typescript
// utils/permissions.ts
import { useAuthStore } from '@/stores/auth'

export function canEdit(resource: string): boolean {
  const authStore = useAuthStore()
  return authStore.hasPermission(`${resource}:edit`)
}

export function canDelete(resource: string): boolean {
  const authStore = useAuthStore()
  return authStore.hasPermission(`${resource}:delete`)
}

export function canManageTenants(): boolean {
  const authStore = useAuthStore()
  return authStore.hasRole('super_admin')
}

export function canManageUsers(): boolean {
  const authStore = useAuthStore()
  return authStore.hasRole('super_admin') || authStore.hasRole('admin')
}
```

## 9. Form Handling

### 9.1 Form Validation

```typescript
// utils/validation.ts
export const validationRules = {
  email: (value: string) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return re.test(value) || 'Invalid email'
  },

  required: (value: string | any[]) => {
    return (value && value.length > 0) || 'This field is required'
  },

  minLength: (min: number) => (value: string) => {
    return value.length >= min || `Minimum ${min} characters`
  },

  password: (value: string) => {
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/
    return regex.test(value) || 'Password must contain uppercase, lowercase, and number'
  },
}
```

### 9.2 Form Composable

```typescript
// composables/useFormValidation.ts
import { ref, computed } from 'vue'

export function useFormValidation<T>(initialData: T) {
  const data = ref<T>(initialData)
  const errors = ref<Record<string, string>>({})
  const touched = ref<Record<string, boolean>>({})

  const isValid = computed(() => Object.keys(errors.value).length === 0)

  function validate(fieldName: keyof T, rule: (value: any) => boolean | string) {
    const result = rule(data.value[fieldName])
    if (result === true) {
      delete errors.value[fieldName as string]
    } else {
      errors.value[fieldName as string] = result as string
    }
  }

  function setFieldError(fieldName: keyof T, message: string) {
    errors.value[fieldName as string] = message
  }

  function clearFieldError(fieldName: keyof T) {
    delete errors.value[fieldName as string]
  }

  function reset() {
    data.value = initialData
    errors.value = {}
    touched.value = {}
  }

  return {
    data,
    errors,
    touched,
    isValid,
    validate,
    setFieldError,
    clearFieldError,
    reset,
  }
}
```

## 10. UI/UX Best Practices

### 10.1 Design System
- **Components**: Reusable button, input, modal, table components
- **Layout**: Responsive grid system
- **Typography**: Clear hierarchy with semantic HTML
- **Colors**: Consistent color palette
- **Icons**: SVG icons for consistency

### 10.2 User Experience
- **Loading States**: Visual feedback für asynchrone Operationen
- **Error Handling**: User-freundliche Fehlermeldungen
- **Notifications**: Toast für Aktionen und Erfolg/Fehler
- **Pagination**: Effiziente Navigation großer Datenmengen
- **Search/Filter**: Schnelle Daten-Suche
- **Keyboard Navigation**: Accessibility-Support

### 10.3 Performance
- **Code Splitting**: Lazy-loading von Routes
- **Component Caching**: Cache häufig verwendete Komponenten
- **Image Optimization**: Responsive Images mit Lazy-loading
- **Bundle Size**: Tree-shaking unbenötigter Code

## 11. Backend API Anforderungen

### 11.1 Admin API Endpoints

Die Backend-Services müssen folgende Admin-Endpoints bereitstellen:

```
# Auth
POST   /api/admin/auth/login
POST   /api/admin/auth/logout
POST   /api/admin/auth/refresh
POST   /api/admin/auth/2fa/verify

# CMS
GET    /api/admin/cms/pages
POST   /api/admin/cms/pages
GET    /api/admin/cms/pages/{id}
PUT    /api/admin/cms/pages/{id}
DELETE /api/admin/cms/pages/{id}
POST   /api/admin/cms/pages/{id}/publish
GET    /api/admin/cms/pages/{id}/versions
POST   /api/admin/cms/pages/{id}/versions/{version}/restore

GET    /api/admin/cms/templates
POST   /api/admin/cms/templates

GET    /api/admin/cms/media
POST   /api/admin/cms/media/upload
DELETE /api/admin/cms/media/{id}

# Shop
GET    /api/admin/shop/products
POST   /api/admin/shop/products
GET    /api/admin/shop/products/{id}
PUT    /api/admin/shop/products/{id}
DELETE /api/admin/shop/products/{id}
POST   /api/admin/shop/products/bulk-import
POST   /api/admin/shop/products/bulk-delete

GET    /api/admin/shop/categories
POST   /api/admin/shop/categories
GET    /api/admin/shop/categories/{id}
PUT    /api/admin/shop/categories/{id}
DELETE /api/admin/shop/categories/{id}

GET    /api/admin/shop/pricing/rules
POST   /api/admin/shop/pricing/rules
PUT    /api/admin/shop/pricing/rules/{id}
DELETE /api/admin/shop/pricing/rules/{id}

GET    /api/admin/shop/inventory
PUT    /api/admin/shop/inventory/{productId}

# Jobs
GET    /api/admin/jobs/queue
GET    /api/admin/jobs/{id}
GET    /api/admin/jobs/{id}/logs
POST   /api/admin/jobs/{id}/retry
POST   /api/admin/jobs/{id}/cancel

GET    /api/admin/jobs/scheduled
POST   /api/admin/jobs/scheduled
PUT    /api/admin/jobs/scheduled/{id}
DELETE /api/admin/jobs/scheduled/{id}

# Analytics
GET    /api/admin/analytics/sales
GET    /api/admin/analytics/traffic
GET    /api/admin/analytics/customers
GET    /api/admin/analytics/products
GET    /api/admin/analytics/reports
POST   /api/admin/analytics/reports
GET    /api/admin/analytics/reports/{id}/export

# Tenants
GET    /api/admin/tenants
POST   /api/admin/tenants
GET    /api/admin/tenants/{id}
PUT    /api/admin/tenants/{id}
DELETE /api/admin/tenants/{id}

# System
GET    /api/admin/system/settings
PUT    /api/admin/system/settings
GET    /api/admin/system/health

GET    /api/admin/users
POST   /api/admin/users
PUT    /api/admin/users/{id}
DELETE /api/admin/users/{id}

GET    /api/admin/roles
POST   /api/admin/roles
PUT    /api/admin/roles/{id}
DELETE /api/admin/roles/{id}

GET    /api/admin/audit-logs

# WebSocket
WS     /api/admin/jobs/stream
WS     /api/admin/system/health/stream
```

### 11.2 Response Format

```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful",
  "timestamp": "2025-12-25T10:00:00Z"
}
```

Error Response:
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format"
      }
    ]
  },
  "timestamp": "2025-12-25T10:00:00Z"
}
```

## 12. Testing Strategy

### 12.1 Unit Tests
- Store/State Tests
- Composable Tests
- Utility Function Tests
- API Service Tests

### 12.2 Component Tests
- Render Tests
- User Interaction Tests
- Event Emission Tests
- Props/Slots Tests

### 12.3 E2E Tests
- Auth Flow
- CMS Workflow
- Shop Management
- Job Monitoring
- Analytics Reporting

### 12.4 Test Tools
- **Vitest**: Unit und Component Tests
- **Vue Test Utils**: Component Testing
- **Playwright**: E2E Testing
- **Cypress**: Optional für komplexere E2E Szenarien

## 13. Sicherheit

### 13.1 Best Practices
- HTTPS everywhere
- CSP (Content Security Policy) Headers
- XSRF Protection (CSRF Tokens)
- Input Sanitization
- Rate Limiting
- API Key Rotation
- Secure Session Management

### 13.2 Data Protection
- Encryption at Rest
- Encryption in Transit
- PII Data Masking in Logs
- Secure Password Storage
- Two-Factor Authentication

## 14. Deployment

### 14.1 Build Prozess
```bash
npm install
npm run build
```

### 14.2 Environment Variables
```
VITE_ADMIN_API_URL=https://api.example.com
VITE_ADMIN_WS_URL=wss://ws.example.com
VITE_APP_NAME=B2Connect Admin
VITE_SENTRY_DSN=https://...
```

### 14.3 Docker Support
```dockerfile
FROM node:20-alpine
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build
EXPOSE 3000
CMD ["npm", "run", "preview"]
```

## 15. Monitoring & Logging

### 15.1 Frontend Monitoring
- **Error Tracking**: Sentry Integration
- **Performance Monitoring**: Core Web Vitals
- **User Analytics**: Google Analytics / Mixpanel
- **Logging**: Winston / Morgan für API Logs

### 15.2 Frontend Metrics
- Page Load Time
- First Contentful Paint (FCP)
- Largest Contentful Paint (LCP)
- Cumulative Layout Shift (CLS)
- Time to Interactive (TTI)

## 16. Lokalisierung

Das Admin-Frontend sollte mehrsprachig unterstützen:

```typescript
// locales/en.json
{
  "common": {
    "save": "Save",
    "cancel": "Cancel",
    "delete": "Delete",
    "edit": "Edit"
  },
  "cms": {
    "pages": "Pages",
    "addPage": "Add Page",
    "editPage": "Edit Page"
  }
}

// locales/de.json
{
  "common": {
    "save": "Speichern",
    "cancel": "Abbrechen",
    "delete": "Löschen",
    "edit": "Bearbeiten"
  },
  "cms": {
    "pages": "Seiten",
    "addPage": "Seite hinzufügen",
    "editPage": "Seite bearbeiten"
  }
}
```

## 17. Implementation Roadmap

### Phase 1: Foundation (Woche 1-2)
- [ ] Project Setup & Build Pipeline
- [ ] Auth Module (Login/Logout)
- [ ] Basic Layout & Navigation
- [ ] Dashboard Skeleton

### Phase 2: CMS (Woche 3-4)
- [ ] Page Management
- [ ] Content Blocks
- [ ] Media Library
- [ ] SEO Management

### Phase 3: Shop (Woche 5-6)
- [ ] Product Management
- [ ] Category Management
- [ ] Pricing Rules
- [ ] Inventory Management

### Phase 4: Monitoring (Woche 7-8)
- [ ] Job Dashboard
- [ ] Job Monitoring
- [ ] System Health
- [ ] Real-time Updates

### Phase 5: Analytics (Woche 9-10)
- [ ] Pre-built Reports
- [ ] Report Builder
- [ ] Export Functionality
- [ ] Dashboards

### Phase 6: Administration (Woche 11-12)
- [ ] Tenant Management
- [ ] User Management
- [ ] Security Settings
- [ ] Audit Logging

---

**Version**: 1.0.0  
**Letzte Aktualisierung**: 25. Dezember 2025  
**Status**: In Development
