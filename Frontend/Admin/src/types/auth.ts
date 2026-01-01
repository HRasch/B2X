#nullable enable

export interface AdminUser {
  id: string
  email: string
  firstName: string
  lastName: string
  roles: Role[]
  permissions: Permission[]
  tenantId: string
  isActive?: boolean
  lastLogin?: Date
}

export interface Role {
  id: string
  name: string
  description: string
  permissions: Permission[]
}

/**
 * Available system roles:
 * - admin: Full system access across all tenants
 * - tenant-admin: Full access within their tenant (all pages, filtered to tenant data)
 * - content_manager: CMS pages and media management
 * - shop_manager: Shop products, categories, and pricing
 * - catalog_manager: Catalog products, categories, and brands
 * - operator: Job queue management and monitoring
 */
export type SystemRole = 
  | 'admin' 
  | 'tenant-admin' 
  | 'content_manager' 
  | 'shop_manager' 
  | 'catalog_manager' 
  | 'operator'

export interface Permission {
  id: string
  name: string
  resource: string
  action: string
}

export interface LoginRequest {
  email: string
  password: string
  rememberMe?: boolean
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: AdminUser
  expiresIn: number
}

export interface AuthState {
  user: AdminUser | null
  token: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  loading: boolean
  error: string | null
}
