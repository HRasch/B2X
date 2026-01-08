---
docid: KB-094
title: Npm Package Updates
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# npm Package Updates Guide

**DocID**: `KB-066`  
**Category**: Dependency Management  
**Last Updated**: 8. Januar 2026  
**Owner**: @TechLead

---

## Overview

Comprehensive guide for updating npm packages in the B2X project, including breaking change detection, migration strategies, and common pitfalls.

---

## Pre-Update Checklist

### 1. Check Current Status
```bash
# List outdated packages
npm outdated

# Check for known vulnerabilities
npm audit

# Review dependency tree
npm ls
```

### 2. Review Breaking Changes
- Check package CHANGELOG.md files
- Review GitHub release notes
- Search for migration guides
- Check official documentation updates

### 3. Backup Current State
```bash
# Commit current working state
git add .
git commit -m "chore: pre-update snapshot"

# Or create a backup branch
git checkout -b pre-update-backup
git checkout main
```

### 4. Handle Deprecated Packages

**Common Deprecated Packages and Fixes**:

#### inflight@1.0.6 (Memory Leak)
- **Cause**: Used by glob@7.x and other packages
- **Fix**: Update transitive dependencies or use alternatives
- **Alternative**: Use `lru-cache` for async request coalescing
- **Detection**: `npm warn deprecated inflight@1.0.6`

#### glob@7.2.3 (No Longer Supported)
- **Cause**: Used by file processing packages like `replace-in-file`
- **Fix**: Update packages to use glob@9+ or disable features using old glob
- **Detection**: `npm warn deprecated glob@7.2.3`

#### @koa/router@12.0.2 (Security & Bug Fixes)
- **Cause**: Used by development tools like `tailwind-config-viewer`
- **Fix**: Upgrade to v15+ or disable the tool
- **Detection**: `npm warn deprecated @koa/router@12.0.2`

#### keygrip@1.1.0 (No Longer Supported)
- **Cause**: Used by `cookies` package in Koa-based tools
- **Fix**: Update or remove dependent packages
- **Detection**: `npm warn deprecated keygrip@1.1.0`

#### backbone-undo@0.2.6 (No Longer Supported)
- **Cause**: Legacy undo/redo functionality
- **Fix**: Replace with modern alternatives or remove
- **Detection**: `npm warn deprecated backbone-undo@0.2.6`

**General Fix Strategy**:
1. Identify the root package causing the warning: `npm ls <deprecated-package>`
2. Update the direct dependency or disable the feature
3. Example: Disable Tailwind config viewer to remove deprecated dependencies
   ```typescript
   // nuxt.config.ts
   modules: ['@nuxtjs/tailwindcss'],
   tailwindcss: {
     viewer: false,  // Disables tailwind-config-viewer and its deprecated deps
   }
   ```

---

---

---

## Update Strategies

### Strategy 1: Incremental Updates (Recommended)

Update packages one major version at a time:

```bash
# Update single package
npm install <package>@<next-major-version>

# Test
npm run type-check && npm run lint && npm run build

# Commit
git commit -m "chore: update <package> to v<version>"
```

**Pros**: Easy to isolate breaking changes  
**Cons**: Time-consuming for many packages

### Strategy 2: Batch Updates

Update all packages at once:

```bash
# Update all to latest
npm update

# Or use npm-check-updates
npx npm-check-updates -u
npm install

# Test
npm run quality-check
```

**Pros**: Faster bulk updates  
**Cons**: Harder to debug when multiple packages break

### Strategy 3: Major Version Only

Update only packages with major version changes:

```bash
# List packages with major updates available
npm outdated | grep -v "^Package" | awk '$3 != $4 {print $1}'

# Update selectively
npm install @opentelemetry/resources@latest
```

**Pros**: Minimizes breaking changes  
**Cons**: May miss important minor/patch fixes

---

## Common Breaking Changes by Package Type

### OpenTelemetry (v1 → v2)

**API Changes**:
```typescript
// ❌ BEFORE (v1.x)
import { Resource } from '@opentelemetry/resources';
const resource = new Resource({ ... });

// ✅ AFTER (v2.x)
import { resourceFromAttributes } from '@opentelemetry/resources';
const resource = resourceFromAttributes({ ... });
```

**Detection**: `'Resource' only refers to a type, but is being used as a value`

**Fix**: Replace constructor calls with functional API

---

### Tailwind CSS (v3 → v4)

**PostCSS Plugin Split**:
```typescript
// ❌ BEFORE (v3.x)
import tailwindcss from 'tailwindcss';

// ✅ AFTER (v4.x)
import tailwindcss from '@tailwindcss/postcss';
```

**Additional Package**: `npm install @tailwindcss/postcss`

**Configuration Update**:
```javascript
// postcss.config.js
export default {
  plugins: {
    '@tailwindcss/postcss': {},  // Changed from 'tailwindcss'
    autoprefixer: {},
  },
};
```

**Detection**: Error message explicitly mentions package move

---

### ESLint Plugin Vue (v9 → v10)

**Config Consolidation**:
```javascript
// ❌ BEFORE (v9.x)
import pluginVue from 'eslint-plugin-vue';
import vueTsEslintConfig from '@vue/eslint-config-typescript';

export default [
  ...pluginVue.configs['flat/essential'],
  ...vueTsEslintConfig(),
];

// ✅ AFTER (v10.x with @vue/eslint-config-typescript v14)
import vueTsEslintConfig from '@vue/eslint-config-typescript';

export default [
  ...vueTsEslintConfig(),  // Vue configs now included
];
```

**Detection**: `Cannot redefine plugin "vue"`

**Fix**: Remove duplicate Vue plugin import

---

### Sentry SDK (v7 → v10)

**Generally Backward Compatible** - Update package versions without code changes in most cases.

**Verify**:
```bash
npm run type-check  # Check for type errors
npm run build       # Verify runtime compatibility
```

### date-fns (v3 → v4)

**New Features**:
- Time zone support via `@date-fns/tz` package
- Context `in` option for time zone calculations

**Breaking Changes**:
- ESM-first (CommonJS still supported)
- Type changes (run type checker after upgrade)
- Function arguments can be mixed types with normalization

**Migration**:
```typescript
// Time zone usage
import { addDays, startOfDay } from "date-fns";
import { tz } from "@date-fns/tz";

startOfDay(addDays(Date.now(), 5, { in: tz("Asia/Singapore") }));
```

**Detection**: Type errors after upgrade

### marked (v14 → v17)

**Breaking Changes**:
- Changed how consecutive text tokens work in lists
- Simplified listItem renderer
- Checkbox token changes in list tokenizer

**Migration**:
- Test markdown rendering, especially lists
- Update custom renderers if using listItem

**Detection**: Changed list rendering output

### @types/node (v22 → v25)

**Breaking Changes**:
- Type definitions for newer Node.js APIs
- Potential type strictness changes

**Migration**:
- Run type checker
- Update Node.js version if needed

**Detection**: Type errors related to Node.js APIs

---

### Vue Ecosystem Updates

**Nuxt Built-in Composables**:
```typescript
// ❌ AVOID (unnecessary dependency)
import { useHead } from '@nuxtjs/seo';

// ✅ PREFER (Nuxt built-in)
import { useHead } from '#app';
// Or rely on auto-import (no import needed)
```

**Check Before Installing**:
- `useHead` - Built into Nuxt 3+
- `useFetch` - Built into Nuxt 3+
- `useState` - Built into Nuxt 3+
- `useRouter`, `useRoute` - Built into Nuxt 3+

---

## Testing After Updates

### Validation Sequence

```bash
# 1. Clear caches
rm -rf .nuxt node_modules/.cache dist

# 2. Reinstall dependencies
npm install

# 3. Type checking
npm run type-check

# 4. Linting
npm run lint

# 5. Build
npm run build

# 6. Unit tests
npm run test

# 7. Integration tests
npm run test:integration

# 8. E2E tests
npm run e2e
```

### Common Issues

**Cache Problems**:
```bash
# Clear all caches
rm -rf .nuxt node_modules/.cache dist .output

# Rebuild
npm run build
```

**Type Errors**:
```typescript
// Check for changed type imports
// Look for "cannot find module" or "has no exported member" errors

// Fix: Update import paths or type definitions
```

**Build Failures**:
```bash
# Check build output for specific errors
npm run build 2>&1 | tee build.log

# Common causes:
# - Changed API signatures
# - Removed deprecated features
# - Configuration changes
```

---

## Nuxt-Specific Considerations

### srcDir Configuration

When `srcDir: 'src'` is set in nuxt.config.ts:

```typescript
// All assets must be inside srcDir
export default defineNuxtConfig({
  srcDir: 'src',
  css: ['~/assets/css/main.css'],  // Resolves to src/assets/css/main.css
});
```

**File Structure**:
```
project/
├── nuxt.config.ts (srcDir: 'src')
└── src/
    ├── assets/       ← Place assets here
    ├── components/
    ├── pages/
    └── app.vue
```

**After Updates**: Verify asset paths still resolve correctly

---

### Module Compatibility

Check Nuxt module compatibility:

```bash
# Check module versions
npm ls @nuxtjs/i18n @pinia/nuxt @nuxtjs/tailwindcss

# Verify compatibility with Nuxt version
# Generally: module major version should match Nuxt major version
```

**Example**:
- Nuxt 4.x → @nuxtjs/i18n v10+
- Nuxt 3.x → @nuxtjs/i18n v8-9

---

## Rollback Procedure

### Quick Rollback

```bash
# Revert to previous commit
git reset --hard HEAD~1

# Or checkout backup branch
git checkout pre-update-backup

# Reinstall dependencies
rm -rf node_modules package-lock.json
npm install
```

### Selective Rollback

```bash
# Rollback specific package
npm install <package>@<previous-version>

# Update package-lock.json
npm install

# Test
npm run quality-check
```

---

## Best Practices

### 1. Update Regularly
- **Weekly**: Patch updates (`1.0.x`)
- **Monthly**: Minor updates (`1.x.0`)
- **Quarterly**: Major updates (`x.0.0`)

### 2. Read Documentation
- Always check migration guides
- Review breaking changes section in changelogs
- Check official documentation for new features

### 3. Test Thoroughly
- Run full test suite after updates
- Test in dev environment first
- Verify production build succeeds

### 4. Use Version Control
- Commit before updates
- Atomic commits per package/update
- Descriptive commit messages

### 5. Monitor Warnings
- Fix all npm audit warnings
- Address deprecation warnings
- Update peer dependencies

---

## Troubleshooting Guide

### Issue: Module Resolution Errors

**Symptom**: `Cannot find module 'xxx'`

**Solutions**:
1. Clear node_modules and reinstall
2. Check package.json for correct versions
3. Verify import paths match new package structure
4. Check for peer dependency conflicts

### Issue: Type Errors After Update

**Symptom**: TypeScript compilation fails

**Solutions**:
1. Update `@types/*` packages to match
2. Check for changed type exports
3. Review type definition changes in changelog
4. Use `npm ls <package>` to check version tree

### Issue: Build Failures

**Symptom**: Build process fails with cryptic errors

**Solutions**:
1. Clear all caches (`.nuxt`, `node_modules/.cache`, `dist`)
2. Check for configuration file changes needed
3. Review plugin compatibility
4. Check for deprecated API usage

### Issue: Runtime Errors in Dev/Prod

**Symptom**: Application crashes at runtime

**Solutions**:
1. Check browser console for errors
2. Review changed API behavior
3. Check for removed deprecated features
4. Verify environment-specific configurations

---

## Reference: B2X Package Update History

### January 2026 - Major Frontend Updates

**Successfully Updated**:
- `@nuxt/eslint-config`: 0.7.6 → 1.12.1
- `@opentelemetry/resources`: 1.30.1 → 2.2.0
- `@opentelemetry/sdk-metrics`: 1.30.1 → 2.2.0
- `@opentelemetry/sdk-node`: 0.57.2 → 0.208.0
- `@sentry/vue`: 7.120.4 → 10.32.1
- `@types/node`: 22.19.3 → 25.0.3
- `date-fns`: 3.6.0 → 4.1.0
- `eslint-plugin-vue`: 9.33.0 → 10.6.2
- `tailwindcss`: 3.x → 4.1.18

**Breaking Changes Encountered**:
1. OpenTelemetry Resource API change
2. Tailwind PostCSS plugin split
3. ESLint Vue plugin consolidation
4. Nuxt srcDir asset resolution

**Total Time**: ~45 minutes (including fixes)

**Vulnerabilities Before**: 0  
**Vulnerabilities After**: 0

---

## Related Documentation

- [GL-013] Dependency Management Policy
- [KB-LESSONS] Lessons Learned (npm updates section)
- [INS-007] Dependency Management Instructions
- [WF-003] Dependency Upgrade Workflow

---

## See Also

- npm Documentation: https://docs.npmjs.com/
- npm-check-updates: https://github.com/raineorshine/npm-check-updates
- OpenTelemetry Migration Guide: https://opentelemetry.io/docs/
- Tailwind CSS v4 Docs: https://tailwindcss.com/docs/v4-beta
- Nuxt Migration Guide: https://nuxt.com/docs/getting-started/upgrade

