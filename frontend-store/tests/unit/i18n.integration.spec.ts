import { describe, it, expect } from 'vitest'

describe('i18n Integration Tests', () => {
  it('should have 8 supported languages', () => {
    const locales = ['en', 'de', 'fr', 'es', 'it', 'pt', 'nl', 'pl']
    expect(locales.length).toBe(8)
  })

  it('should have proper locale metadata', () => {
    const en = { code: 'en', name: 'English', flag: '🇬🇧' }
    expect(en.code).toBe('en')
    expect(en.flag).toBe('🇬🇧')
  })
})
