#nullable enable

export interface AdminUser {
  id: string
  email: string
  firstName: string
  lastName: string
  roles: Role[]
  permissions: Permission[]
  tenantId: string
  isActive: boolean
  lastLogin?: Date
}

export interface Role {
  id: string
  name: string
  description: string
  permissions: Permission[]
}

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
}

export interface AuthState {
  user: AdminUser | null
  token: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  loading: boolean
  error: string | null
}
