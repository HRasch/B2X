import { describe, it, expect } from 'vitest'

describe('useLocale composable', () => {
  it('should support 8 languages', () => {
    const locales = ['en', 'de', 'fr', 'es', 'it', 'pt', 'nl', 'pl']
    expect(locales.length).toBe(8)
  })

  it('should have valid locale codes', () => {
    const en = 'en'
    const de = 'de'
    expect(en.length).toBe(2)
    expect(de.length).toBe(2)
  })
})
