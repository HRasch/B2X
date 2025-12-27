import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface AuthState {
  token: string | null
  userId: string | null
  email: string | null
  isAuthenticated: boolean
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  const userId = ref<string | null>(localStorage.getItem('user_id'))
  const email = ref<string | null>(localStorage.getItem('user_email'))

  const isAuthenticated = computed(() => !!token.value)

  const setAuth = (newToken: string, newUserId: string, newEmail: string) => {
    token.value = newToken
    userId.value = newUserId
    email.value = newEmail

    localStorage.setItem('auth_token', newToken)
    localStorage.setItem('user_id', newUserId)
    localStorage.setItem('user_email', newEmail)
  }

  const logout = () => {
    token.value = null
    userId.value = null
    email.value = null

    localStorage.removeItem('auth_token')
    localStorage.removeItem('user_id')
    localStorage.removeItem('user_email')
  }

  return {
    token,
    userId,
    email,
    isAuthenticated,
    setAuth,
    logout
  }
})
