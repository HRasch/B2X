---
docid: KB-134
title: Frontend Index
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Frontend Lessons Index

**DocID**: KB-LESSONS-FRONTEND-INDEX
**Category**: Frontend | **Priority Focus**: Vue.js, Nuxt, TypeScript, CSS, Testing
**Last Updated**: 8. Januar 2026 | **Owner**: @DocMaintainer

---

## � Search & Tags

**Technology Tags**: `vue` `nuxt` `typescript` `eslint` `tailwind` `opentelemetry` `postcss` `composables` `assets`
**Problem Tags**: `breaking-change` `migration` `performance` `configuration` `plugins` `imports` `paths`
**Solution Tags**: `refactor` `update` `config` `auto-import` `aliases` `functional-api`
**Impact Tags**: `security` `build-failure` `runtime-error` `performance` `dx`

**Quick Search**:
- ESLint issues: [🔴 ESLint Plugin Conflicts](#eslint--linting)
- Migration problems: [🔴 OpenTelemetry v2 Migration](#package-updates--breaking-changes), [🔴 Tailwind CSS v4 Migration](#package-updates--breaking-changes)
- Performance: [🟡 Nuxt Built-in Composables](#-important-should-know---common-performance--workflow-issues)
- Configuration: [🟡 Asset Path Resolution](#-important-should-know---common-performance--workflow-issues)

---

## �🔴 Critical (Must Know - Breaking Changes & Security)

### ESLint & Linting
1. **ESLint Plugin Conflicts** - Plugin redefinition in flat config
   - **Issue**: ConfigError: Key "plugins": Cannot redefine plugin "vue"
   - **Root Cause**: @vue/eslint-config-typescript v14+ bundles Vue plugins
   - **Solution**: Remove duplicate eslint-plugin-vue import
   - **Reference**: [KB-LESSONS-FRONTEND-RED-ESLINT]
   - **Related**: [ADR-042] i18n strategy for ESLint reduction, [KB-007] Vue.js 3 patterns

### Package Updates & Breaking Changes
2. **OpenTelemetry v2 Migration** - Resource API functional change
   - **Issue**: 'Resource' only refers to a type TypeScript error
   - **Root Cause**: OpenTelemetry v2 uses resourceFromAttributes() function
   - **Solution**: Replace new Resource() with functional API
   - **Reference**: [KB-LESSONS-FRONTEND-RED-OPENTELEMETRY]
   - **Related**: [KB-009] Vite tooling integration

3. **Tailwind CSS v4 Migration** - PostCSS plugin separation
   - **Issue**: Cannot resolve 'tailwindcss' as PostCSS plugin
   - **Root Cause**: Tailwind v4 split PostCSS plugin to separate package
   - **Solution**: Install and import @tailwindcss/postcss
   - **Reference**: [KB-LESSONS-FRONTEND-RED-TAILWIND]
   - **Related**: [KB-009] Vite tooling integration

---

## 🟡 Important (Should Know - Common Performance & Workflow Issues)

4. **Nuxt Built-in Composables** - Avoid unnecessary packages
   - **Issue**: Installing @nuxtjs/seo for useHead
   - **Root Cause**: useHead is built-in to Nuxt 3+
   - **Solution**: Use auto-imported composables
   - **Reference**: [KB-LESSONS-FRONTEND-YELLOW-COMPOSABLES]
   - **Related**: [KB-007] Vue.js 3 patterns, [ADR-030] Vue-i18n migration

5. **Asset Path Resolution** - srcDir configuration impact
   - **Issue**: ENOENT for existing CSS files
   - **Root Cause**: Nuxt srcDir affects asset resolution
   - **Solution**: Use ~ or @ aliases
   - **Reference**: [KB-LESSONS-FRONTEND-YELLOW-ASSETS]
   - **Related**: [KB-009] Vite tooling integration

---


## 🟢 Recent Additions (Last 30 Days)

*Recent lessons added to the knowledge base*

---

*Full details in archive files. This index prioritizes prevention.*

