---
docid: PRM-033
title: Bug Quick I18n Missing.Prompt
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

---
docid: PRM-QBF-I18N
title: Quick Bug Fix - Missing Translation Keys
command: /bug-i18n-missing
owner: @TechLead
status: Active
references: ["ADR-052", "KB-060", "INS-002"]
---

# /bug-i18n-missing - Quick i18n Key Fix

**Use when**: Missing translation key, undefined i18n string, hardcoded text found in components

---

## Diagnostic MCP Chain

```bash
# 1. Find i18n key usages
typescript-mcp/find_symbol_usages symbol="[I18N_KEY]"

# 2. Validate key exists in all locales
i18n-mcp/validate_translation_keys workspacePath="frontend/Store"

# 3. Check for hardcoded strings
typescript-mcp/analyze_types filePath="[FILE_PATH]"
```

---

## Execution Steps

### Step 1: Gather Context
Provide:
- **Key**: Translation key name (e.g., `common.save_button`)
- **Supported Languages**: en, de, fr, es, it, pt, nl, pl
- **Current Status**: Missing in? (e.g., "de, fr, es")

### Step 2: Add to Locale Files

**Location**: `frontend/[Project]/src/locales/[lang].json`

**Pattern**: `module.section.key`

```json
{
  "common": {
    "save_button": "Save",
    "cancel_button": "Cancel",
    "delete_confirm": "Are you sure?"
  },
  "product": {
    "price_label": "Price",
    "in_stock": "In Stock"
  }
}
```

### Step 3: Add Key to All Languages

**Step 3a: English (en.json)** - Source of truth
```json
{
  "common": {
    "save_button": "Save"
  }
}
```

**Step 3b: German (de.json)**
```json
{
  "common": {
    "save_button": "Speichern"
  }
}
```

**Step 3c: French (fr.json)**
```json
{
  "common": {
    "save_button": "Enregistrer"
  }
}
```

**Step 3d: Spanish (es.json)**
```json
{
  "common": {
    "save_button": "Guardar"
  }
}
```

**Step 3e: Italian (it.json), Portuguese (pt.json), Dutch (nl.json), Polish (pl.json)**
- Add corresponding translations
- If translation unknown, use `[i18n-todo]` placeholder

```json
{
  "common": {
    "save_button": "[i18n-todo]"
  }
}
```

### Step 4: Update Component Reference

**Vue Template**:
```vue
<!-- ❌ Wrong: Hardcoded text -->
<button>Save</button>

<!-- ✅ Right: Use i18n key -->
<button>{{ $t('common.save_button') }}</button>
```

**Vue Script**:
```typescript
// ❌ Wrong: Hardcoded text
const label = 'Save';

// ✅ Right: Use i18n
import { useI18n } from 'vue-i18n';
const { t } = useI18n();
const label = t('common.save_button');
```

### Step 5: Validate

```bash
# Check all keys exist in all locales
i18n-mcp/validate_translation_keys workspacePath="frontend/Store"

# Check for hardcoded strings
typescript-mcp/analyze_types filePath="src/components/Button.vue"

# No missing keys should remain
```

### Step 6: Document
Add to `.ai/knowledgebase/lessons.md`:
```markdown
## [Quick Fix] Missing i18n Keys

**Pattern**: ZERO hardcoded strings - all user-facing text uses $t()
**Files**: [list of files]
**Prevention**: 
  - Use $t('key') in templates
  - Use useI18n().t('key') in scripts
  - Key pattern: module.section.key
  - Add to all 8 supported locales: en, de, fr, es, it, pt, nl, pl
  - Use [i18n-todo] for untranslated strings (temporary)
**Tool**: typescript-mcp, i18n-mcp
```

---

## Key Naming Convention

```
module.section.key

Examples:
✅ common.button.save
✅ product.details.price_label
✅ cart.items.empty_message
✅ auth.login.title
✅ error.validation.email_required

❌ save (too short)
❌ buttonSave (wrong format)
❌ common_button_save (wrong separator)
```

---

## Supported Locales

| Code | Language | Status |
|------|----------|--------|
| `en` | English | Source of truth |
| `de` | Deutsch | Required |
| `fr` | Français | Required |
| `es` | Español | Required |
| `it` | Italiano | Required |
| `pt` | Português | Required |
| `nl` | Nederlands | Required |
| `pl` | Polski | Required |

**All 8 languages must have the key, even if using `[i18n-todo]` placeholder.**

---

## Common Patterns

### Pattern 1: Button Labels
```json
{
  "common": {
    "button": {
      "save": "Save",
      "cancel": "Cancel",
      "delete": "Delete",
      "close": "Close",
      "submit": "Submit"
    }
  }
}
```

### Pattern 2: Form Labels
```json
{
  "forms": {
    "fields": {
      "email": "Email Address",
      "password": "Password",
      "name": "Full Name",
      "phone": "Phone Number"
    }
  }
}
```

### Pattern 3: Messages
```json
{
  "messages": {
    "success": "Operation successful",
    "error": "An error occurred",
    "loading": "Loading...",
    "empty": "No items found"
  }
}
```

### Pattern 4: Validation
```json
{
  "validation": {
    "required": "This field is required",
    "invalid_email": "Please enter a valid email",
    "too_short": "Must be at least 3 characters",
    "password_mismatch": "Passwords do not match"
  }
}
```

---

## Quick Checklist

- [ ] Key added to `en.json` (English)
- [ ] Key added to `de.json` (German)
- [ ] Key added to `fr.json` (French)
- [ ] Key added to `es.json` (Spanish)
- [ ] Key added to `it.json` (Italian)
- [ ] Key added to `pt.json` (Portuguese)
- [ ] Key added to `nl.json` (Dutch)
- [ ] Key added to `pl.json` (Polish)
- [ ] Component uses `$t('key')` or `useI18n().t('key')`
- [ ] No hardcoded user-facing strings remain
- [ ] i18n validation passes
- [ ] Type check passes
- [ ] Lesson documented in lessons.md

---

## Translation Todo Items

If translation is not available yet, use `[i18n-todo]` placeholder:

```json
{
  "common": {
    "save_button": "[i18n-todo]"
  }
}
```

Then create a GitHub issue:
```
Title: i18n - Translate key: common.save_button
Languages needed: de, fr, es, it, pt, nl, pl
Labels: i18n, translation-needed
```

---

## See Also

- [ADR-052] MCP-Enhanced Bugfixing Workflow
- [KB-060] i18n MCP Usage Guide
- [INS-002] Frontend Essentials - i18n Requirements
- [ADR-030] Vue-i18n v10 to v11 Migration
- Vue i18n: https://vue-i18n.intlify.dev/
