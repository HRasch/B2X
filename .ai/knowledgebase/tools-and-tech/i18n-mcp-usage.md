---
docid: KB-174
title: I18n Mcp Usage
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# i18n MCP Usage Guide

**DocID**: `KB-060`  
**Title**: i18n MCP Usage Guide  
**Owner**: @Frontend  
**Status**: Active  
**Last Updated**: 6. Januar 2026

---

## Overview

The i18n MCP (Internationalization Model Context Protocol) server provides comprehensive internationalization validation and management for B2X's multi-language support. It ensures translation completeness, consistency, and proper implementation across all supported languages.

**Supported Languages**: en, de, fr, es, it, pt, nl, pl  
**Source Language**: English (en) - all translations derive from English keys

---

## Core Features

### Translation Key Validation
Validates that all translation keys exist and are properly structured across all supported languages.

```bash
# Validate translation keys across all languages
i18n-mcp/validate_translation_keys workspacePath="." languages="en,de,fr,es,it,pt,nl,pl"

# Validate specific component keys
i18n-mcp/validate_translation_keys workspacePath="frontend/Store" componentPath="src/components/LoginForm.vue"
```

### Missing Translation Detection
Identifies missing translations in any supported language.

```bash
# Check for missing translations
i18n-mcp/check_missing_translations workspacePath="." languages="en,de,fr,es,it,pt,nl,pl"

# Check specific language
i18n-mcp/check_missing_translations workspacePath="." languages="de,fr"
```

### Translation Consistency Validation
Ensures translation keys are used consistently across the codebase.

```bash
# Validate translation consistency
i18n-mcp/validate_consistency workspacePath="."

# Check specific namespace
i18n-mcp/validate_consistency workspacePath="." namespace="auth"
```

### Locale File Analysis
Analyzes locale files for structure, completeness, and best practices.

```bash
# Analyze all locale files
i18n-mcp/analyze_locale_files workspacePath="."

# Analyze specific locale file
i18n-mcp/analyze_locale_files workspacePath="." localeFile="locales/de.json"
```

### Pluralization Validation
Validates proper pluralization handling across languages.

```bash
# Check pluralization rules
i18n-mcp/check_pluralization workspacePath="."

# Validate specific language pluralization
i18n-mcp/check_pluralization workspacePath="." languages="de,fr,pl"
```

### Interpolation Validation
Ensures proper variable interpolation in translation strings.

```bash
# Validate interpolation syntax
i18n-mcp/validate_interpolation workspacePath="."

# Check specific component interpolation
i18n-mcp/validate_interpolation workspacePath="frontend/Store" componentPath="src/components/ProductCard.vue"
```

### RTL Language Support (Optional)
Validates right-to-left language support for Arabic, Hebrew, etc.

```bash
# Check RTL language support
i18n-mcp/check_rtl_support workspacePath="." languages="ar,he"

# Validate RTL layout components
i18n-mcp/check_rtl_support workspacePath="frontend/Store" componentPath="src/components/Layout.vue"
```

### Translation Extraction (Optional)
Extracts hardcoded strings for translation.

```bash
# Extract translatable strings from Vue components
i18n-mcp/extract_translations workspacePath="frontend/Store" fileType="vue"

# Extract from TypeScript files
i18n-mcp/extract_translations workspacePath="frontend/Store" fileType="ts"
```

---

## Integration with Development Workflow

### Pre-Commit Validation

```bash
#!/bin/bash
# Run before every commit

# 1. Translation key validation (MANDATORY)
i18n-mcp/validate_translation_keys workspacePath="."
if [ $? -ne 0 ]; then
  echo "❌ Missing translation keys found"
  exit 1
fi

# 2. Check for missing translations
i18n-mcp/check_missing_translations workspacePath="." languages="en,de,fr,es,it,pt,nl,pl"
if [ $? -ne 0 ]; then
  echo "❌ Missing translations detected"
  exit 1
fi

# 3. Validate consistency
i18n-mcp/validate_consistency workspacePath="."
if [ $? -ne 0 ]; then
  echo "❌ Translation inconsistencies found"
  exit 1
fi

echo "✅ i18n validation passed"
```

### CI/CD Integration

```yaml
# .github/workflows/i18n-validation.yml
name: i18n Validation

on:
  pull_request:
    paths:
      - 'frontend/**/locales/**'
      - 'frontend/**/*.vue'
      - 'frontend/**/*.ts'

jobs:
  i18n-check:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Run i18n MCP validation
        run: |
          i18n-mcp/validate_translation_keys workspacePath="."
          i18n-mcp/check_missing_translations workspacePath="." languages="en,de,fr,es,it,pt,nl,pl"
          i18n-mcp/validate_consistency workspacePath="."
          i18n-mcp/validate_interpolation workspacePath="."
```

### Vue.js Component Integration

```typescript
// src/composables/useI18nValidation.ts
import { useI18n } from 'vue-i18n'

export function useI18nValidation() {
  const { t, te } = useI18n()

  const validateKey = (key: string): boolean => {
    return te(key)
  }

  const getTranslation = (key: string, fallback?: string): string => {
    return t(key, fallback || key)
  }

  return {
    validateKey,
    getTranslation
  }
}
```

---

## Key Naming Conventions

### Namespace Structure
```
{module}.{section}.{key}
```

**Examples**:
- `auth.login.title` - Authentication login page title
- `catalog.product.name` - Product catalog item name
- `common.button.save` - Common save button text

### Pluralization Keys
```
{baseKey}_zero
{baseKey}_one
{baseKey}_two
{baseKey}_few
{baseKey}_many
{baseKey}_other
```

**Example**:
```json
{
  "cart.items": "No items | {count} item | {count} items",
  "cart.items_zero": "No items",
  "cart.items_one": "{count} item",
  "cart.items_other": "{count} items"
}
```

### Interpolation Syntax
```json
{
  "user.welcome": "Welcome, {name}!",
  "product.price": "Price: {currency}{amount}"
}
```

---

## Best Practices

### Translation Key Management

1. **Always use translation keys** - Never hardcoded strings in components
2. **English as source** - All new keys must be defined in English first
3. **Consistent naming** - Follow established namespace patterns
4. **Regular validation** - Run i18n MCP checks before commits

### Component Implementation

```vue
<!-- ✅ CORRECT: Using translation keys -->
<template>
  <div>
    <h1>{{ $t('auth.login.title') }}</h1>
    <button @click="login">
      {{ $t('auth.login.submit') }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const login = () => {
  // Use translation in logic
  alert(t('auth.login.success'))
}
</script>
```

```vue
<!-- ❌ WRONG: Hardcoded strings -->
<template>
  <div>
    <h1>Login</h1>
    <button @click="login">
      Sign In
    </button>
  </div>
</template>
```

### Error Handling

```typescript
// Graceful fallback for missing translations
const safeTranslate = (key: string, fallback?: string): string => {
  try {
    return i18n.te(key) ? i18n.t(key) : (fallback || key)
  } catch {
    return fallback || key
  }
}
```

---

## Troubleshooting

### Common Issues

#### Missing Translation Keys
```bash
# Find missing keys
i18n-mcp/check_missing_translations workspacePath="." languages="de,fr"

# Output shows which keys are missing in which languages
```

#### Inconsistent Key Usage
```bash
# Check consistency
i18n-mcp/validate_consistency workspacePath="."

# Review output for duplicate or conflicting keys
```

#### Interpolation Errors
```bash
# Validate interpolation
i18n-mcp/validate_interpolation workspacePath="."

# Check for malformed {variable} syntax
```

### Performance Optimization

```typescript
// Lazy load locale files
const loadLocale = async (locale: string) => {
  const messages = await import(`./locales/${locale}.json`)
  i18n.setLocaleMessage(locale, messages.default)
}
```

### Testing i18n

```typescript
// Test translation completeness
describe('i18n', () => {
  it('should have all required keys', () => {
    const requiredKeys = ['auth.login.title', 'auth.login.submit']
    requiredKeys.forEach(key => {
      expect(te(key)).toBe(true)
    })
  })
})
```

---

## Configuration

### MCP Server Configuration

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "i18n-mcp": {
      "disabled": false,
      "config": {
        "sourceLanguage": "en",
        "supportedLanguages": ["en", "de", "fr", "es", "it", "pt", "nl", "pl"],
        "keyPattern": "{module}.{section}.{key}",
        "validateOnSave": true
      }
    }
  }
}
```

### Vue i18n Configuration

```typescript
// src/i18n/index.ts
import { createI18n } from 'vue-i18n'

export const i18n = createI18n({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  messages: {
    en: () => import('./locales/en.json'),
    de: () => import('./locales/de.json'),
    // ... other languages
  },
  missingWarn: process.env.NODE_ENV === 'development',
  fallbackWarn: process.env.NODE_ENV === 'development'
})
```

---

## Related Documentation

- [GL-042] Token-Optimized i18n Strategy
- [KB-007] Vue.js 3 Knowledge Base
- [KB-053] TypeScript MCP Integration Guide
- [KB-054] Vue MCP Integration Guide

---

**Maintained by**: @Frontend  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/knowledgebase/tools-and-tech/i18n-mcp-usage.md