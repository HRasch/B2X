export type LocaleCode = 'en' | 'de' | 'fr' | 'es' | 'it' | 'pt' | 'nl' | 'pl';

export interface UserDto {
  id: string;
  tenantId: string;
  email: string;
  firstName: string;
  lastName: string;
  avatar?: string;
  status: string;
  lastLoginAt: Date;
  emailConfirmed: boolean;
  roles?: string[];
  permissions?: string[];
}

export interface TenantDto {
  id: string;
  name: string;
  slug: string;
  description?: string;
  logoUrl?: string;
  status: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
  errors?: Record<string, string>;
  timestamp: Date;
}

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
  tenantId?: string;
}
