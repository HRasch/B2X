# Frontend Lessons Index

**DocID**: `KB-LESSONS-FRONTEND-INDEX`  
**Category**: Frontend | **Priority Focus**: Vue.js, Nuxt, TypeScript, CSS, Testing  
**Last Updated**: 8. Januar 2026 | **Owner**: @DocMaintainer

---

## 🎯 Category Overview

Frontend lessons covering Vue.js 3, Nuxt 4, TypeScript, CSS frameworks, and testing. Focus on breaking changes, performance optimization, and development workflow improvements.

**Key Technologies**: Vue.js, Nuxt, TypeScript, Tailwind CSS, ESLint, Vite, Playwright

---

## 🔴 Critical (Must Know - Breaking Changes & Security)

### ESLint & Linting
1. **ESLint Plugin Conflicts** - Plugin redefinition in flat config
   - **Issue**: `ConfigError: Key "plugins": Cannot redefine plugin "vue"`
   - **Root Cause**: @vue/eslint-config-typescript v14+ bundles Vue plugins
   - **Solution**: Remove duplicate eslint-plugin-vue import
   - **Impact**: Build failures, CI blocking
   - **Reference**: [KB-LESSONS-FRONTEND-RED-ESLINT]

### Package Updates & Breaking Changes
2. **OpenTelemetry v2 Migration** - Resource API functional change
   - **Issue**: `Resource only refers to a type` TypeScript error
   - **Root Cause**: OpenTelemetry v2 uses `resourceFromAttributes()` function
   - **Solution**: Replace `new Resource()` with functional API
   - **Impact**: Build failures, telemetry data loss
   - **Reference**: [KB-LESSONS-FRONTEND-RED-OPENTELEMETRY]

3. **Tailwind CSS v4 Migration** - PostCSS plugin separation
   - **Issue**: `Cannot resolve 'tailwindcss' as PostCSS plugin`
   - **Root Cause**: Tailwind v4 split PostCSS plugin to separate package
   - **Solution**: Install and import `@tailwindcss/postcss`
   - **Impact**: Build failures, styling broken
   - **Reference**: [KB-LESSONS-FRONTEND-RED-TAILWIND]

---

## 🟡 Important (Should Know - Common Performance & Workflow Issues)

### Nuxt Framework
4. **Built-in Composables** - Avoid unnecessary packages
   - **Issue**: Installing @nuxtjs/seo for `useHead`
   - **Root Cause**: `useHead` is built-in to Nuxt 3+
   - **Solution**: Use auto-imported composables, check docs first
   - **Impact**: Bundle bloat, maintenance overhead
   - **Reference**: [KB-LESSONS-FRONTEND-YELLOW-COMPOSABLES]

5. **Asset Path Resolution** - srcDir configuration impact
   - **Issue**: `ENOENT: no such file` for existing CSS files
   - **Root Cause**: Nuxt srcDir affects asset resolution paths
   - **Solution**: Use `~` or `@` aliases for srcDir-relative paths
   - **Impact**: Asset loading failures, broken styling
   - **Reference**: [KB-LESSONS-FRONTEND-YELLOW-ASSETS]

### TypeScript & Build Tools
6. **Vite Configuration** - Plugin compatibility issues
   - **Issue**: Build errors with Vite plugins in Nuxt
   - **Root Cause**: Plugin version mismatches or configuration conflicts
   - **Solution**: Verify plugin compatibility, update configurations
   - **Impact**: Build failures, development slowdown
   - **Reference**: [KB-LESSONS-FRONTEND-YELLOW-VITE]

---

## 🟢 Nice-to-Know (Optimization & Best Practices)

### CSS & Styling
7. **CSS Optimization** - Bundle size reduction techniques
   - **Issue**: Large CSS bundles impacting performance
   - **Root Cause**: Unused styles, inefficient selectors
   - **Solution**: Purge unused CSS, optimize selectors
   - **Impact**: Faster load times, better UX
   - **Reference**: [KB-LESSONS-FRONTEND-GREEN-CSS-OPTIMIZATION]

### Testing & Quality
8. **E2E Test Optimization** - Faster, more reliable tests
   - **Issue**: Slow or flaky end-to-end tests
   - **Root Cause**: Improper waits, network dependencies
   - **Solution**: Use proper selectors, mock external services
   - **Impact**: Faster CI/CD, more reliable deployments
   - **Reference**: [KB-LESSONS-FRONTEND-GREEN-E2E-OPTIMIZATION]

---

## 📅 Recent Additions (Last 90 Days)

| Date | Priority | Title | Key Learning |
|------|----------|-------|--------------|
| 8. Jan | 🔴 | ESLint Plugin Conflicts | @vue/eslint-config-typescript v14+ includes Vue plugins |
| 8. Jan | 🔴 | OpenTelemetry v2 Migration | Use `resourceFromAttributes()` instead of `new Resource()` |
| 8. Jan | 🟡 | Nuxt Built-in Composables | Check Nuxt docs before installing external packages |
| 7. Jan | 🟡 | Nuxt 4 Monorepo Config | Proper workspace configuration prevents resolution errors |

---

## 🏷️ Tag Index

**Technology Tags**: `vue`, `nuxt`, `typescript`, `tailwind`, `eslint`, `vite`, `playwright`

**Problem Tags**: `breaking-change`, `migration`, `compatibility`, `performance`, `build-error`

**Solution Tags**: `configuration`, `optimization`, `refactoring`, `automation`

---

## 📊 Category Statistics

- **Total Lessons**: 12 (organized by priority)
- **Critical**: 3 (25%) - Focus on breaking changes
- **Important**: 5 (42%) - Common workflow issues
- **Nice-to-Know**: 4 (33%) - Optimization opportunities
- **Coverage**: Vue.js, Nuxt, TypeScript, CSS frameworks, testing

---

## 🔄 Maintenance

- **Updated**: When new frontend lessons are added
- **Reviewed**: Monthly for relevance and completeness
- **Archived**: Lessons >6 months moved to `archive/frontend-2026.md`
- **Cross-Referenced**: Links to related ADRs and KB articles

**For new lessons**: Add to appropriate priority section and update statistics

---

## 📋 Prevention Checklist

**Before starting frontend development:**
- [ ] Check ESLint configuration compatibility
- [ ] Verify package versions against breaking changes
- [ ] Review Nuxt composables availability
- [ ] Test asset path resolution with srcDir
- [ ] Validate Vite plugin compatibility

**During development:**
- [ ] Monitor bundle sizes for CSS optimization opportunities
- [ ] Use proper TypeScript types for OpenTelemetry
- [ ] Test E2E scenarios with proper waits and selectors

**Before deployment:**
- [ ] Run full build to catch configuration issues
- [ ] Verify all assets load correctly
- [ ] Test critical user journeys in staging</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\knowledgebase\lessons\frontend-index.md