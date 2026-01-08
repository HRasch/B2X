---
docid: UNKNOWN-173
title: Bug I18n Missing.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: TechLead
description: Quick fix for missing i18n translation keys
---

# Bug Quick Fix: Missing i18n Keys

Add missing translation keys to prevent i18n errors.

## Required Information
- File Path: 
- Missing Key: 
- Language File: 
- Context: 

## Quick Fix Pattern

### Vue Component - Add Translation Key
```vue
<script setup>
// BEFORE - Hardcoded string
const errorMessage = 'Invalid email format'

// AFTER - Use translation key
const { t } = useI18n()
const errorMessage = t('validation.email.invalid')
</script>

<template>
  <!-- BEFORE -->
  <div>Invalid email format</div>
  
  <!-- AFTER -->
  <div>{{ $t('validation.email.invalid') }}</div>
</template>
```

### Add to Language Files
```json
// locales/en.json - BEFORE
{
  "validation": {
    "required": "This field is required"
  }
}

// locales/en.json - AFTER
{
  "validation": {
    "required": "This field is required",
    "email": {
      "invalid": "Invalid email format"
    }
  }
}
```

### Multiple Languages
```json
// locales/de.json
{
  "validation": {
    "email": {
      "invalid": "Ungültiges E-Mail-Format"
    }
  }
}

// locales/fr.json
{
  "validation": {
    "email": {
      "invalid": "Format d'e-mail invalide"
    }
  }
}
```

## Output Format

```
✅ i18n Key Added

Modified: [file path]
- Replaced hardcoded string with $t('[key]')
- Added translation key to [language files]
- Supports [list of languages]
```

## Testing
- [ ] All supported languages have the key
- [ ] No hardcoded strings remain
- [ ] Translation loading works