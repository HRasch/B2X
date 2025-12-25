import { apiClient } from '../client'
import type { AdminUser, LoginRequest, LoginResponse } from '@/types/auth'

export const authApi = {
  login(credentials: LoginRequest) {
    return apiClient.post<LoginResponse>('/auth/login', credentials)
  },

  logout() {
    return apiClient.post<void>('/auth/logout', {})
  },

  refreshToken(refreshToken: string) {
    return apiClient.post<LoginResponse>('/auth/refresh', { refreshToken })
  },

  verifyToken(token: string) {
    return apiClient.post<AdminUser>('/auth/verify', { token })
  },

  getCurrentUser() {
    return apiClient.get<AdminUser>('/auth/me')
  },

  updateProfile(data: Partial<AdminUser>) {
    return apiClient.put<AdminUser>('/auth/profile', data)
  },

  changePassword(oldPassword: string, newPassword: string) {
    return apiClient.post<void>('/auth/change-password', { oldPassword, newPassword })
  },

  requestMFA() {
    return apiClient.post<{ method: string }>('/auth/2fa/request', {})
  },

  verifyMFA(code: string) {
    return apiClient.post<LoginResponse>('/auth/2fa/verify', { code })
  },
}
