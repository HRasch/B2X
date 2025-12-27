import { describe, it, expect } from 'vitest'

// Form validation utilities
export const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export const validatePassword = (password: string): { valid: boolean; errors: string[] } => {
  const errors: string[] = []

  if (password.length < 8) {
    errors.push('Password must be at least 8 characters')
  }
  if (!/[A-Z]/.test(password)) {
    errors.push('Password must contain uppercase letter')
  }
  if (!/[a-z]/.test(password)) {
    errors.push('Password must contain lowercase letter')
  }
  if (!/[0-9]/.test(password)) {
    errors.push('Password must contain number')
  }

  return {
    valid: errors.length === 0,
    errors,
  }
}

export const validateURL = (url: string): boolean => {
  try {
    new URL(url)
    return true
  } catch {
    return false
  }
}

export const validatePhoneNumber = (phone: string): boolean => {
  const phoneRegex = /^[\d\s+\-()]+$/
  return phoneRegex.test(phone) && phone.replace(/\D/g, '').length >= 10
}

describe('Form Validation Utils', () => {
  describe('validateEmail', () => {
    it('should validate valid email addresses', () => {
      expect(validateEmail('user@example.com')).toBe(true)
      expect(validateEmail('admin@domain.co.uk')).toBe(true)
      expect(validateEmail('test.user+tag@example.com')).toBe(true)
    })

    it('should reject invalid email addresses', () => {
      expect(validateEmail('invalid-email')).toBe(false)
      expect(validateEmail('user@')).toBe(false)
      expect(validateEmail('@example.com')).toBe(false)
      expect(validateEmail('')).toBe(false)
    })

    it('should reject email without domain', () => {
      expect(validateEmail('user')).toBe(false)
    })
  })

  describe('validatePassword', () => {
    it('should validate strong password', () => {
      const result = validatePassword('SecurePass123')
      expect(result.valid).toBe(true)
      expect(result.errors).toHaveLength(0)
    })

    it('should reject password too short', () => {
      const result = validatePassword('Short1')
      expect(result.valid).toBe(false)
      expect(result.errors).toContain('Password must be at least 8 characters')
    })

    it('should require uppercase letter', () => {
      const result = validatePassword('nouppercase1')
      expect(result.valid).toBe(false)
      expect(result.errors).toContain('Password must contain uppercase letter')
    })

    it('should require lowercase letter', () => {
      const result = validatePassword('NOLOWERCASE1')
      expect(result.valid).toBe(false)
      expect(result.errors).toContain('Password must contain lowercase letter')
    })

    it('should require number', () => {
      const result = validatePassword('NoNumbers')
      expect(result.valid).toBe(false)
      expect(result.errors).toContain('Password must contain number')
    })

    it('should allow multiple errors', () => {
      const result = validatePassword('weak')
      expect(result.valid).toBe(false)
      expect(result.errors.length).toBeGreaterThan(1)
    })
  })

  describe('validateURL', () => {
    it('should validate valid URLs', () => {
      expect(validateURL('http://example.com')).toBe(true)
      expect(validateURL('https://example.com/path')).toBe(true)
      expect(validateURL('ftp://files.example.com')).toBe(true)
    })

    it('should reject invalid URLs', () => {
      expect(validateURL('not a url')).toBe(false)
      expect(validateURL('example.com')).toBe(false)
      expect(validateURL('')).toBe(false)
    })
  })

  describe('validatePhoneNumber', () => {
    it('should validate valid phone numbers', () => {
      expect(validatePhoneNumber('+1 (555) 123-4567')).toBe(true)
      expect(validatePhoneNumber('555-123-4567')).toBe(true)
      expect(validatePhoneNumber('+49 30 12345678')).toBe(true)
    })

    it('should reject invalid phone numbers', () => {
      expect(validatePhoneNumber('123')).toBe(false)
      expect(validatePhoneNumber('not a phone')).toBe(false)
      expect(validatePhoneNumber('')).toBe(false)
    })

    it('should accept various formats', () => {
      expect(validatePhoneNumber('1234567890')).toBe(true)
      expect(validatePhoneNumber('+1234567890')).toBe(true)
      expect(validatePhoneNumber('(123) 456-7890')).toBe(true)
    })
  })
})

// Permission checking utilities
export const hasPermission = (userPermissions: string[], requiredPermission: string): boolean => {
  return userPermissions.includes(requiredPermission) || userPermissions.includes('*')
}

export const hasAnyPermission = (userPermissions: string[], requiredPermissions: string[]): boolean => {
  return requiredPermissions.some((perm) => hasPermission(userPermissions, perm))
}

export const hasAllPermissions = (userPermissions: string[], requiredPermissions: string[]): boolean => {
  return requiredPermissions.every((perm) => hasPermission(userPermissions, perm))
}

describe('Permission Utils', () => {
  describe('hasPermission', () => {
    it('should check if user has permission', () => {
      const permissions = ['read', 'write', 'delete']
      expect(hasPermission(permissions, 'read')).toBe(true)
      expect(hasPermission(permissions, 'write')).toBe(true)
    })

    it('should reject missing permission', () => {
      const permissions = ['read', 'write']
      expect(hasPermission(permissions, 'delete')).toBe(false)
    })

    it('should allow all permissions with wildcard', () => {
      const permissions = ['*']
      expect(hasPermission(permissions, 'read')).toBe(true)
      expect(hasPermission(permissions, 'delete')).toBe(true)
      expect(hasPermission(permissions, 'any')).toBe(true)
    })
  })

  describe('hasAnyPermission', () => {
    it('should return true if user has any required permission', () => {
      const permissions = ['read', 'write']
      expect(hasAnyPermission(permissions, ['read', 'delete'])).toBe(true)
    })

    it('should return false if user has no required permissions', () => {
      const permissions = ['read']
      expect(hasAnyPermission(permissions, ['write', 'delete'])).toBe(false)
    })

    it('should handle empty required permissions', () => {
      const permissions = ['read', 'write']
      expect(hasAnyPermission(permissions, [])).toBe(false)
    })
  })

  describe('hasAllPermissions', () => {
    it('should return true if user has all required permissions', () => {
      const permissions = ['read', 'write', 'delete']
      expect(hasAllPermissions(permissions, ['read', 'write'])).toBe(true)
    })

    it('should return false if user missing any permission', () => {
      const permissions = ['read', 'write']
      expect(hasAllPermissions(permissions, ['read', 'write', 'delete'])).toBe(false)
    })

    it('should handle empty required permissions', () => {
      const permissions = ['read']
      expect(hasAllPermissions(permissions, [])).toBe(true)
    })
  })
})

// Date formatting utilities
export const formatDate = (date: Date | string): string => {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}

export const formatDateTime = (date: Date | string): string => {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleString('en-US')
}

export const formatTime = (date: Date | string): string => {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleTimeString('en-US')
}

describe('Date Formatting Utils', () => {
  describe('formatDate', () => {
    it('should format date correctly', () => {
      const date = new Date('2024-01-15')
      const formatted = formatDate(date)
      expect(formatted).toContain('Jan')
      expect(formatted).toContain('15')
      expect(formatted).toContain('2024')
    })

    it('should handle string dates', () => {
      const formatted = formatDate('2024-01-15')
      expect(formatted).toBeTruthy()
    })
  })

  describe('formatDateTime', () => {
    it('should format date and time', () => {
      const date = new Date('2024-01-15T14:30:00')
      const formatted = formatDateTime(date)
      expect(formatted).toBeTruthy()
      expect(formatted).toContain('30')
    })
  })

  describe('formatTime', () => {
    it('should format time only', () => {
      const date = new Date('2024-01-15T14:30:00')
      const formatted = formatTime(date)
      expect(formatted).toContain('30')
    })
  })
})
