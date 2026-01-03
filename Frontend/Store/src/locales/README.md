# Frontend Localization - Vue.js i18n

## Overview

This directory contains the Vue.js 3 internationalization (i18n) setup using vue-i18n v9 for the B2Connect frontend application.

## Structure

```
locales/
├── index.ts          # i18n configuration and setup
├── en.json           # English translations
├── de.json           # German translations
├── fr.json           # French translations
├── es.json           # Spanish translations
├── it.json           # Italian translations
├── pt.json           # Portuguese translations
├── nl.json           # Dutch translations
└── pl.json           # Polish translations
```

## Configuration (index.ts)

The `index.ts` file exports:

- **i18n instance**: Configured vue-i18n instance for Vue 3 Composition API
- **SUPPORTED_LOCALES**: Array of locale metadata with code, name, and flag emoji

```typescript
export const SUPPORTED_LOCALES = [
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
  // ... more locales
];
```

## Translation Files (JSON)

Each language JSON file is organized by category:

### Categories

1. **auth** - Authentication related strings
   - login
   - register
   - logout
   - forgot_password

2. **ui** - User interface labels
   - save, cancel, delete, edit, close
   - submit, next, previous, back
   - loading, error, success, warning

3. **errors** - Error messages
   - required_field
   - invalid_email
   - network_error
   - etc.

4. **validation** - Form validation messages
   - min_length
   - max_length
   - password_match
   - etc.

5. **common** - Common phrases
   - welcome, goodbye
   - yes, no
   - loading, empty state
   - etc.

### Example Structure

```json
{
  "auth": {
    "login": "Log In",
    "register": "Sign Up",
    "logout": "Log Out",
    "forgot_password": "Forgot Password?"
  },
  "ui": {
    "save": "Save",
    "cancel": "Cancel",
    "delete": "Delete",
    "edit": "Edit"
  },
  "errors": {
    "required_field": "This field is required",
    "invalid_email": "Invalid email address"
  },
  "validation": {
    "email_format": "Please enter a valid email",
    "min_length": "Must be at least {min} characters"
  },
  "common": {
    "welcome": "Welcome",
    "loading": "Loading..."
  }
}
```

## Usage in Components

### Using the useLocale Composable

```vue
<script setup>
import { useLocale } from '@/composables/useLocale';

const { t, locale, currentLocale, setLocale } = useLocale();
</script>

<template>
  <!-- Translate a string -->
  <button>{{ t('auth.login') }}</button>

  <!-- Use with variables -->
  <p>{{ t('validation.min_length', { min: 8 }) }}</p>

  <!-- Conditional rendering based on locale -->
  <div v-if="locale === 'de'">Deutsche Version</div>

  <!-- Switch language -->
  <button @click="setLocale('de')">Switch to German</button>

  <!-- Get current locale info -->
  <p>Current: {{ currentLocale.name }} {{ currentLocale.flag }}</p>
</template>
```

### Using vue-i18n Directly

```vue
<script setup>
import { useI18n } from 'vue-i18n';

const { t, locale } = useI18n();
</script>

<template>
  <button>{{ t('auth.login') }}</button>
</template>
```

## Supported Languages

| Code | Language   | Flag |
| ---- | ---------- | ---- |
| en   | English    | 🇬🇧   |
| de   | Deutsch    | 🇩🇪   |
| fr   | Français   | 🇫🇷   |
| es   | Español    | 🇪🇸   |
| it   | Italiano   | 🇮🇹   |
| pt   | Português  | 🇵🇹   |
| nl   | Nederlands | 🇳🇱   |
| pl   | Polski     | 🇵🇱   |

## Features

✅ **Composition API Compatible**: Works seamlessly with Vue 3 Composition API  
✅ **Type-Safe**: Full TypeScript support  
✅ **localStorage Persistence**: User language preference is saved  
✅ **Browser Language Detection**: Automatically detects user's browser language  
✅ **Fallback Support**: Falls back to English if language not found  
✅ **Dynamic Translation Updates**: Can fetch translations from backend API  
✅ **Custom Events**: Emits locale-changed events for external listeners

## Adding New Translations

### To add a new translation key:

1. Add the key to **all language files** to maintain consistency:

   ```json
   {
     "category": {
       "new_key": "English text"
     }
   }
   ```

2. Use in components:
   ```vue
   <template>
     <p>{{ t('category.new_key') }}</p>
   </template>
   ```

### To add a new language:

1. Create a new JSON file (e.g., `ja.json` for Japanese)
2. Copy the structure from an existing language
3. Translate all strings
4. Update `locales/index.ts`:

   ```typescript
   import ja from './ja.json';

   export const SUPPORTED_LOCALES = [
     // ... existing locales
     { code: 'ja', name: '日本語', flag: '🇯🇵' },
   ];
   ```

5. Update `src/main.ts` if needed for locale initialization

## Language Switching

The language switcher component is located at:

- **Component**: `src/components/common/LanguageSwitcher.vue`
- **Composable**: `src/composables/useLocale.ts`

### How Language Switching Works:

1. User clicks language switcher dropdown
2. Selects a language
3. `setLocale()` is called:
   - Updates vue-i18n locale
   - Saves to localStorage
   - Updates document `lang` attribute
   - Emits `locale-changed` event
4. Vue reactively updates all translated content

## Storage

### localStorage Key

- **Key**: `locale`
- **Value**: Language code (e.g., `"de"`)
- **Persistence**: Survives page reloads and browser restarts

### Fallback Chain

1. Check localStorage
2. Check browser language (`navigator.language`)
3. Default to English (`"en"`)

## Integration with Backend

The application can fetch translations from the backend LocalizationService API:

```typescript
import localizationApi from '@/services/localizationApi';

// Fetch single translation
const loginText = await localizationApi.getString('auth', 'login', 'de');

// Fetch all category translations
const authTranslations = await localizationApi.getCategory('auth', 'de');

// Pre-fetch multiple categories
await localizationApi.prefetchCategories(['auth', 'ui', 'errors'], 'de');
```

See `src/services/localizationApi.ts` for full API documentation.

## Testing

### Unit Tests

- `tests/unit/useLocale.spec.ts` - Composable tests
- `tests/unit/localizationApi.spec.ts` - API service tests
- `tests/unit/i18n.integration.spec.ts` - Integration tests
- `tests/components/LanguageSwitcher.spec.ts` - Component tests

### E2E Tests

- `tests/e2e/localization.spec.ts` - End-to-end language switching tests

Run tests with:

```bash
npm run test:unit
npm run test:e2e
```

## Performance Considerations

1. **Caching**: API responses are cached to prevent duplicate requests
2. **Lazy Loading**: Categories are loaded on-demand
3. **Prefetching**: Use `prefetchCategories()` to pre-load likely-needed translations
4. **Streaming**: Large translation files are served with gzip compression

## Browser DevTools

### Checking Current Locale

Open browser console and run:

```javascript
// Vue i18n instance
i18n.global.locale.value;

// localStorage
localStorage.getItem('locale');

// HTML lang attribute
document.documentElement.lang;
```

### Listening for Locale Changes

```javascript
window.addEventListener('locale-changed', event => {
  console.log('Locale changed to:', event.detail.locale);
});
```

## Troubleshooting

### Translations not showing

1. Check that language code matches a supported locale
2. Verify translation key exists in JSON files
3. Check browser console for errors
4. Ensure `useLocale()` is called in component setup

### localStorage not working

1. Check browser storage settings (not disabled by user)
2. Verify localStorage quota not exceeded
3. Check browser console for errors

### Language detection not working

1. Verify `navigator.language` is available in browser
2. Check browser language settings
3. Fall back to default locale (English)

## Documentation

For complete documentation, see:

- [I18N_SPECIFICATION.md](../../I18N_SPECIFICATION.md) - Full requirements & roadmap
- [LOCALIZATION_PHASE2_COMPLETE.md](../../LOCALIZATION_PHASE2_COMPLETE.md) - Frontend implementation details
- [vue-i18n Docs](https://vue-i18n.intlify.dev/) - Official documentation

## Contributing

When adding or modifying translations:

1. **Consistency**: Keep all language files in sync
2. **Structure**: Use same categories across all languages
3. **Naming**: Use snake_case for translation keys
4. **Testing**: Add tests for new features
5. **Documentation**: Update relevant docs

## License

Part of B2Connect - See root LICENSE file
