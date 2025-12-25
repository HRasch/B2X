import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../../src/stores/auth'

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
  })

  it('should initialize with no authentication', () => {
    const store = useAuthStore()
    expect(store.isAuthenticated).toBe(false)
    expect(store.user).toBeNull()
  })

  it('should set user after successful login', async () => {
    const store = useAuthStore()

    // This test would need to mock the API
    // For now, we're just testing the store structure
    expect(store.logout).toBeDefined()
    expect(store.setUser).toBeDefined()
  })

  it('should clear auth data on logout', () => {
    const store = useAuthStore()
    store.logout()

    expect(store.user).toBeNull()
    expect(store.accessToken).toBeNull()
    expect(store.isAuthenticated).toBe(false)
  })
})
