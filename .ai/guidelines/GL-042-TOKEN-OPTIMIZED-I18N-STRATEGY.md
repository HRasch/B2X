---
docid: GL-078
title: GL 042 TOKEN OPTIMIZED I18N STRATEGY
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# GL-042: Token-Optimized i18n Translations Strategy

**DocID**: `GL-042`  
**Status**: Active | **Owner**: @CopilotExpert  
**Created**: 2026-01-06

---

## Purpose

Minimize AI token consumption when working with internationalization (i18n) files while maintaining translation quality across all 8 supported languages.

---

## 🎯 Quick Rules

### 1. Single-Language Development Pattern
```
✅ DO: Work with English (en.json) as source of truth
✅ DO: Request translation batch for specific keys only
❌ DON'T: Load all 8 language files simultaneously
❌ DON'T: Ask AI to "translate the entire file"
```

### 2. Key-Based Requests (Token Efficient)
```
✅ "Add translation key 'checkout.shippingAddress' with value 'Shipping Address'"
✅ "Translate these 5 keys to all languages: auth.login.title, auth.login.submit..."
❌ "Review all translations in the auth section"
❌ "Check if all languages have all keys"
```

### 3. Structured Section Work
```
✅ Work one namespace at a time: auth, ui, errors, store, etc.
❌ Work across multiple namespaces in single request
```

---

## 📁 File Structure (Current)

```
frontend/Store/locales/
├── index.ts              # i18n config (~55 lines)
├── en.json               # English - SOURCE OF TRUTH
├── de.json               # German
├── fr.json               # French  
├── es.json               # Spanish
├── it.json               # Italian
├── pt.json               # Portuguese
├── nl.json               # Dutch
├── pl.json               # Polish
└── default/              # Tenant overrides (sparse)
    └── {lang}/
        └── {namespace}.json
```

**Current Size**: ~160-200 lines per language file (~4-5 KB each)  
**Total**: ~8 files × 5 KB = ~40 KB translation content

---

## 🔄 Token-Optimized Workflows

### Workflow 1: Adding New Translations

**Step 1**: Define keys in English only
```json
// Request: "Add to en.json under 'checkout' namespace"
{
  "checkout": {
    "shippingAddress": "Shipping Address",
    "billingAddress": "Billing Address",
    "paymentMethod": "Payment Method"
  }
}
```

**Step 2**: Batch translate (single request)
```
"Translate these checkout keys to de, fr, es, it, pt, nl, pl:
- checkout.shippingAddress = 'Shipping Address'
- checkout.billingAddress = 'Billing Address'  
- checkout.paymentMethod = 'Payment Method'"
```

**AI Response Format** (compact):
```json
{
  "de": { "shippingAddress": "Lieferadresse", "billingAddress": "Rechnungsadresse", "paymentMethod": "Zahlungsmethode" },
  "fr": { "shippingAddress": "Adresse de livraison", "billingAddress": "Adresse de facturation", "paymentMethod": "Mode de paiement" }
}
```

### Workflow 2: Modifying Existing Translations

**Efficient Pattern**:
```
"Update key 'ui.loading' from 'Loading...' to 'Please wait...' in all languages"
```

**AI applies changes directly** without loading full files.

### Workflow 3: Namespace Review

**When needed**: Pre-release translation QA

```
"List all keys under 'errors' namespace from en.json only"
```

Then selective verification:
```
"Verify 'errors.server_error' exists in de.json and fr.json"
```

---

## 📏 Size Guidelines

| Namespace | Max Keys | Split Threshold |
|-----------|----------|-----------------|
| `ui` | 50 | Split if >50 keys |
| `auth` | 30 | Split into auth.login, auth.register |
| `errors` | 30 | OK |
| `validation` | 20 | OK |
| `store` | 40 | Consider store.product, store.checkout |
| `navigation` | 20 | OK |
| `legal` | 40 | Consider legal.privacy, legal.terms |

**Rule**: If namespace exceeds threshold, split into sub-namespaces.

---

## 🤖 AI Request Templates

### Template 1: Add Single Key (All Languages)
```
Add i18n key '{namespace}.{key}' with English value '{value}'.
Provide translations for: de, fr, es, it, pt, nl, pl
Format: compact JSON per language
```

### Template 2: Add Multiple Keys (Single Namespace)
```
Add these keys to '{namespace}' namespace:
- {key1}: "{english_value1}"
- {key2}: "{english_value2}"

Provide all translations in this format:
{ "lang": { "key1": "...", "key2": "..." } }
```

### Template 3: Update Key Value
```
Update '{namespace}.{key}' across all 8 languages:
Old English: "{old_value}"
New English: "{new_value}"
Adjust other languages accordingly.
```

### Template 4: Delete Keys
```
Remove keys from all language files: {namespace}.{key1}, {namespace}.{key2}
```

---

## 🚫 Anti-Patterns (High Token Cost)

| Anti-Pattern | Token Cost | Better Approach |
|--------------|------------|-----------------|
| "Load all language files" | ~40 KB | Load en.json only |
| "Check translation completeness" | ~80 KB (comparison) | Script-based validation |
| "Review all translations" | ~40 KB | Review by namespace |
| "Translate entire file" | ~40+ KB | Batch specific keys |
| "Show me de.json and fr.json" | ~10 KB | Specific key lookup |

---

## 🛠️ Tooling Recommendations

### 1. Translation Completeness Script
Create `scripts/i18n-check.sh`:
```bash
#!/bin/bash
# Check for missing keys across languages
EN_KEYS=$(jq -r 'paths(scalars) | join(".")' frontend/Store/locales/en.json | sort)
for lang in de fr es it pt nl pl; do
  LANG_KEYS=$(jq -r 'paths(scalars) | join(".")' frontend/Store/locales/${lang}.json | sort)
  MISSING=$(comm -23 <(echo "$EN_KEYS") <(echo "$LANG_KEYS"))
  if [ -n "$MISSING" ]; then
    echo "Missing in $lang:"
    echo "$MISSING"
  fi
done
```

### 2. Key Extraction for AI Requests
```bash
# Extract keys from specific namespace
jq '.errors | keys' frontend/Store/locales/en.json
```

### 3. Batch Translation Generator
```bash
# Generate translation request format
jq -r '.checkout | to_entries | .[] | "- checkout.\(.key): \"\(.value)\""' frontend/Store/locales/en.json
```

---

## 📊 Token Budget per Task

| Task | Estimated Tokens | Files Loaded |
|------|------------------|--------------|
| Add 1 key (all langs) | ~500 | 0 (AI generates) |
| Add 5 keys (all langs) | ~1,500 | 0 |
| Update 1 key (all langs) | ~800 | 0-1 |
| Review namespace (en only) | ~1,000 | 1 partial |
| Full file edit | ~5,000+ | 1 full |

---

## ✅ Checklist for Translation Work

Before starting translation work:

- [ ] Am I working with keys, not full files?
- [ ] Is English (en.json) defined first?
- [ ] Am I batching related keys together?
- [ ] Can I use a script instead of AI review?
- [ ] Is the namespace under 50 keys?

---

## 🔗 Related Documents

- [GL-006] Token Optimization Strategy
- [INS-002] Frontend Instructions (i18n section)
- [ADR-042] Internationalization Strategy (if exists)

---

## 📋 Language Reference

| Code | Language | Native Name |
|------|----------|-------------|
| `en` | English | English |
| `de` | German | Deutsch |
| `fr` | French | Français |
| `es` | Spanish | Español |
| `it` | Italian | Italiano |
| `pt` | Portuguese | Português |
| `nl` | Dutch | Nederlands |
| `pl` | Polish | Polski |

---

**Maintained by**: @CopilotExpert  
**Review**: Quarterly
