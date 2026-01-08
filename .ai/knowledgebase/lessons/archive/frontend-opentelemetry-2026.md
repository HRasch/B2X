---
docid: KB-128
title: Frontend Opentelemetry 2026
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: frontend-opentelemetry-2026
title: 8. Januar 2026 - npm Package Updates to Latest Versions & Breaking Changes
category: frontend
migrated: 2026-01-08
---
### OpenTelemetry v2 Breaking Changes - Resource API

**Issue**: TypeScript error `'Resource' only refers to a type, but is being used as a value here` after updating OpenTelemetry packages from v1.x to v2.x.

**Root Cause**: OpenTelemetry v2.x changed the Resource API - `new Resource()` constructor is no longer exported, replaced with `resourceFromAttributes()` function.

**Lesson**: OpenTelemetry v2.x uses functional API for resource creation instead of class constructors.

**Solution**: Replace constructor with functional API:
```typescript
// BEFORE (OpenTelemetry v1.x)
import { Resource } from '@opentelemetry/resources';

const resource = new Resource({
  [ATTR_SERVICE_NAME]: 'my-service',
  [ATTR_SERVICE_VERSION]: '1.0.0',
});

// AFTER (OpenTelemetry v2.x)
import { resourceFromAttributes } from '@opentelemetry/resources';

const resource = resourceFromAttributes({
  [ATTR_SERVICE_NAME]: 'my-service',
  [ATTR_SERVICE_VERSION]: '1.0.0',
});
```

**Key Insights**:
- **API Change**: `Resource` class → `resourceFromAttributes()` function
- **Import Change**: Import from `@opentelemetry/resources` remains the same
- **Verification**: Check available exports with `node -e "const r = require('@opentelemetry/resources'); console.log(Object.keys(r))"`
- **Migration Impact**: Single line change, same functionality

**Reference**: OpenTelemetry v2.0 Migration Guide

---

### ESLint Plugin Vue v10 - Flat Config Plugin Conflicts

**Issue**: `ConfigError: Config "vue/base/setup": Key "plugins": Cannot redefine plugin "vue"` when using eslint-plugin-vue v10 with @vue/eslint-config-typescript.

**Root Cause**: @vue/eslint-config-typescript v14 now includes Vue plugin configs internally, causing duplicate plugin registration when combined with eslint-plugin-vue.

**Lesson**: @vue/eslint-config-typescript v14+ bundles Vue ESLint rules - don't import eslint-plugin-vue separately.

**Solution**: Remove duplicate Vue plugin import:
```javascript
// BEFORE (causes plugin conflict)
import pluginVue from 'eslint-plugin-vue';
import vueTsEslintConfig from '@vue/eslint-config-typescript';

export default [
  ...pluginVue.configs['flat/essential'],
  ...vueTsEslintConfig(),
];

// AFTER (v14 compatible)
import vueTsEslintConfig from '@vue/eslint-config-typescript';

export default [
  ...vueTsEslintConfig(),  // Vue configs included
];
```

**Key Insights**:
- **Breaking Change**: @vue/eslint-config-typescript v14 consolidates Vue + TypeScript configs
- **Flat Config**: ESLint v9 flat config doesn't allow duplicate plugin names
- **Detection**: Error message explicitly states "Cannot redefine plugin 'vue'"
- **Simplification**: Fewer imports needed in v14+

**Reference**: @vue/eslint-config-typescript v14 Release Notes

---

### Tailwind CSS v4 - PostCSS Plugin Migration

**Issue**: `It looks like you're trying to use 'tailwindcss' directly as a PostCSS plugin. The PostCSS plugin has moved to a separate package` error during build.

**Root Cause**: Tailwind CSS v4 split PostCSS plugin into separate `@tailwindcss/postcss` package.

**Lesson**: Tailwind CSS v4 requires `@tailwindcss/postcss` for PostCSS integration instead of main `tailwindcss` package.

**Solution**: Update imports to use new package:
```typescript
// BEFORE (Tailwind CSS v3)
import tailwindcss from 'tailwindcss';

export default defineNuxtConfig({
  vite: {
    css: {
      postcss: {
        plugins: [tailwindcss, autoprefixer],
      },
    },
  },
});

// AFTER (Tailwind CSS v4)
import tailwindcss from '@tailwindcss/postcss';

export default defineNuxtConfig({
  vite: {
    css: {
      postcss: {
        plugins: [tailwindcss, autoprefixer],
      },
    },
  },
});
```

**Key Insights**:
- **Package Split**: PostCSS plugin now in `@tailwindcss/postcss`
- **Installation**: Both `tailwindcss` and `@tailwindcss/postcss` needed
- **Configuration**: Update `postcss.config.js` AND build tool configs
- **Error Message**: Clear migration instruction in error output

**Reference**: Tailwind CSS v4 Migration Guide

---

### Nuxt Built-in Composables vs External Packages

**Issue**: `Could not resolve import "@nuxtjs/seo"` when using `useHead` from @nuxtjs/seo package.

**Root Cause**: `useHead` is a built-in Nuxt 3+ composable, doesn't require external @nuxtjs/seo package.

**Lesson**: Nuxt 3+ includes many composables built-in - check Nuxt docs before installing external packages.

**Solution**: Use built-in composable from Nuxt:
```typescript
// BEFORE (unnecessary dependency)
import { useHead } from '@nuxtjs/seo';

useHead({
  title: 'My Page',
});

// AFTER (Nuxt built-in)
import { useHead } from '#app';  // or auto-imported

useHead({
  title: 'My Page',
});
```

**Built-in Nuxt Composables** (don't need external packages):
- `useHead` - SEO meta management
- `useFetch` - Data fetching
- `useState` - State management
- `useRoute`, `useRouter` - Routing
- `useCookie` - Cookie handling
- `useRuntimeConfig` - Config access

**Key Insights**:
- **Auto-Import**: Most Nuxt composables are auto-imported, no import needed
- **Migration**: Projects migrating from Nuxt 2 may have outdated imports
- **Documentation**: Always check Nuxt 3 composables docs before adding packages
- **Import Alias**: Use `#app` for explicit imports

**Reference**: Nuxt 3 Composables Documentation

---

### Nuxt srcDir and Asset Path Resolution

**Issue**: `ENOENT: no such file or directory, open 'src/assets/css/main.css'` when CSS file exists at `assets/css/main.css`.

**Root Cause**: When `srcDir: 'src'` is configured in nuxt.config.ts, Nuxt expects all source files (including assets) inside src/ directory.

**Lesson**: Nuxt srcDir setting affects asset path resolution - assets must be inside srcDir when configured.

**Solution**: Place assets inside srcDir or update paths:
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  srcDir: 'src',  // Source directory
  
  // Option 1: Use ~ alias (resolves to srcDir)
  css: ['~/assets/css/main.css'],  // Looks in src/assets/css/
  
  // Option 2: Use @ alias (resolves to srcDir)
  css: ['@/assets/css/main.css'],  // Looks in src/assets/css/
});
```

**File Structure**:
```
Frontend/Store/
├── nuxt.config.ts (srcDir: 'src')
├── package.json
└── src/                    # ← srcDir root
    ├── assets/            # ← Assets go here
    │   └── css/
    │       └── main.css
    ├── components/
    ├── pages/
    └── app.vue
```

**Key Insights**:
- **Consistency**: When srcDir is set, ALL source files must be inside it
- **Aliases**: `~` and `@` resolve relative to srcDir, not project root
- **Migration**: Moving to srcDir requires copying/moving assets into src/
- **Cache Issues**: Clear `.nuxt` and `node_modules/.cache` after structural changes

**Reference**: Nuxt Configuration - srcDir

---

### npm Package Update Strategy - Breaking Changes Management

**Issue**: Multiple breaking changes when updating npm packages to latest versions required systematic fixes across codebase.

**Root Cause**: Major version updates often include breaking API changes that require code modifications.

**Lesson**: When updating multiple packages to latest, expect breaking changes and test incrementally.

**Update Workflow**:
```bash
# 1. Check for outdated packages
npm outdated

# 2. Update packages (major versions)
npm install package@latest --save

# 3. Clear build cache
rm -rf .nuxt node_modules/.cache dist

# 4. Run type check (catches API changes)
npm run type-check

# 5. Fix TypeScript errors

# 6. Run linter (catches code style issues)
npm run lint

# 7. Fix linting errors

# 8. Build project (catches runtime issues)
npm run build

# 9. Run tests
npm test
```

**Common Breaking Changes**:
1. **API Changes**: Method renames, parameter changes
   - Fix: Check package CHANGELOG.md or migration guide
2. **Import Paths**: Module reorganization
   - Fix: Update import statements
3. **Configuration**: Config format changes
   - Fix: Update config files
4. **Type Changes**: TypeScript types modified
   - Fix: Update type annotations

**Prevention Strategies**:
- **Incremental Updates**: Update one major package at a time
- **Read Changelogs**: Check BREAKING CHANGES section before updating
- **Test Coverage**: Good tests catch breaking changes early
- **Version Pinning**: Use exact versions (no ^) for critical dependencies

**Key Insights**:
- **Type Safety First**: TypeScript catches most API changes immediately
- **Build Validation**: Build errors reveal integration issues
- **Documentation**: Package docs often include migration guides
- **Rollback Ready**: Commit before updates, easy to revert if needed

**Packages Updated Successfully** (this session):
- OpenTelemetry: v1.x → v2.x (API change)
- ESLint Plugin Vue: v9 → v10 (config change)
- Tailwind CSS: v3 → v4 (plugin split)
- Sentry: v7 → v10 (no breaking changes encountered)
- date-fns: v3 → v4 (no breaking changes encountered)

---
