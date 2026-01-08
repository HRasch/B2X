# Nuxt 4 Monorepo Configuration Guide

**DocID**: `KB-065`  
**Category**: Tools & Tech  
**Last Updated**: 8. Januar 2026  
**Status**: Active

---

## Overview

This guide documents best practices for configuring Nuxt 4 applications in npm workspace monorepos, addressing common issues with postinstall scripts, module resolution, and dependency management.

---

## Critical Configuration Changes in Nuxt 4

### Postinstall Script Removal

**⚠️ BREAKING CHANGE**: Nuxt 4 no longer requires explicit postinstall scripts in monorepo workspaces.

#### Why Remove Postinstall?

- **Infinite Loop Issue**: In npm workspaces, `postinstall: "nuxt prepare"` triggers recursively
- **GitHub Issue**: [#33382 - Layer with dependencies in monorepo goes in postinstall loop](https://github.com/nuxt/nuxt/issues/33382)
- **Nuxt 4 Behavior**: Prepare step now runs automatically on `dev` and `build` commands

#### Migration Steps

```json
// ❌ BEFORE (Nuxt 3 / causes issues in workspaces)
{
  "scripts": {
    "postinstall": "nuxt prepare",
    "dev": "nuxt dev",
    "build": "nuxt build"
  }
}

// ✅ AFTER (Nuxt 4 monorepo)
{
  "scripts": {
    "dev": "nuxt dev",
    "build": "nuxt build"
  }
}
```

**Official Guidance**: Confirmed by Daniel Roe (Nuxt core team) - postinstall removal is the recommended solution for monorepos.

---

## @nuxt/kit Module Resolution

### The Problem

In npm workspace monorepos, `@nuxt/kit` version mismatches cause module resolution errors:

```
ERROR  Cannot resolve module "@nuxt/kit" (from: C:\...\Store)
```

### Root Cause

- **Workspace Hoisting**: npm workspaces hoist dependencies to root `node_modules`
- **Version Conflicts**: Different packages depend on different `@nuxt/kit` versions
- **Resolution Failure**: Nuxt can't find the correct version during runtime

### Detection

```bash
# Check dependency tree for version conflicts
npm ls @nuxt/kit

# Look for mismatches:
B2X-frontend-monorepo@1.0.0
└─┬ B2X-store-frontend@1.0.0
  ├─┬ @nuxtjs/tailwindcss@6.14.0
  │ └── @nuxt/kit@3.20.2          # ❌ Mismatch!
  └─┬ nuxt@4.2.2
    └── @nuxt/kit@4.2.2            # ✓ Correct version
```

### Solution

Add `@nuxt/kit` as explicit dependency matching your Nuxt version:

```bash
# Install matching version
npm install --save @nuxt/kit@4.2.2
```

```json
// package.json
{
  "dependencies": {
    "@nuxt/kit": "^4.2.2",  // Must match nuxt version exactly
    "nuxt": "^4.2.2"
  }
}
```

**Version Matching Rule**: `@nuxt/kit` version MUST match `nuxt` version:
- Nuxt 4.2.2 → @nuxt/kit 4.2.2
- Nuxt 4.1.0 → @nuxt/kit 4.1.0
- etc.

---

## Workspace Structure Best Practices

### Recommended Monorepo Structure

```
project/
├── package.json (workspace root)
├── Frontend/
│   ├── package.json (workspace config)
│   ├── Store/
│   │   ├── package.json (Nuxt app)
│   │   ├── nuxt.config.ts
│   │   └── src/
│   ├── Admin/
│   │   └── package.json (Vue app)
│   └── Management/
│       └── package.json (Vue app)
└── backend/
```

### Workspace Root Configuration

```json
// Frontend/package.json
{
  "name": "B2X-frontend-monorepo",
  "private": true,
  "workspaces": ["Store", "Admin", "Management"],
  "devDependencies": {
    "@pinia/nuxt": "^0.11.3",
    "pinia": "^3.0.4"
  }
}
```

### Nuxt App Configuration

```json
// Frontend/Store/package.json
{
  "name": "B2X-store-frontend",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "nuxt dev",           // ✓ No postinstall
    "build": "nuxt build",
    "generate": "nuxt generate",
    "preview": "nuxt preview"
  },
  "dependencies": {
    "@nuxt/kit": "^4.2.2",       // ✓ Explicit version
    "nuxt": "^4.2.2",
    "@nuxtjs/i18n": "^10.2.1",
    "@pinia/nuxt": "^0.11.3"
  }
}
```

---

## Troubleshooting Workflow

### Step-by-Step Diagnostic Process

```bash
# 1. Check for known issues
# Search: "Nuxt 4 monorepo workspace @nuxt/kit" on GitHub

# 2. Verify package.json
# - Remove postinstall script
# - Add @nuxt/kit matching Nuxt version

# 3. Clean environment
rm -rf node_modules .nuxt
rm -rf Frontend/Store/node_modules
rm -rf Frontend/Admin/node_modules
rm -rf Frontend/Management/node_modules

# 4. Fresh install
npm install

# 5. Check dependency tree
npm ls @nuxt/kit

# 6. Test individual workspace
cd Frontend/Store
npx nuxt dev

# 7. Test workspace root
cd Frontend
npm run dev
```

### Common Error Patterns

#### Error: "Cannot resolve module '@nuxt/kit'"

**Cause**: Version mismatch or missing explicit dependency

**Solution**:
```bash
npm install --save @nuxt/kit@$(npm view nuxt version)
```

#### Error: "Infinite postinstall loop"

**Cause**: `postinstall: "nuxt prepare"` in package.json

**Solution**: Remove postinstall script entirely

#### Error: "TypeError: useCookie is not a function"

**Cause**: Incorrect mocking in tests (separate from monorepo issue)

**Solution**: See [KB-LESSONS] Session 7. Januar 2026 - Nuxt Composables Mocking

---

## Migration Checklist

Use this checklist when migrating existing Nuxt 3 monorepo to Nuxt 4:

- [ ] **Remove postinstall scripts** from all Nuxt apps
- [ ] **Add @nuxt/kit** as explicit dependency in each Nuxt app
- [ ] **Verify version matching** between nuxt and @nuxt/kit
- [ ] **Clean node_modules** (all workspaces + root)
- [ ] **Fresh npm install**
- [ ] **Check dependency tree** with `npm ls @nuxt/kit`
- [ ] **Test each workspace** individually before testing root
- [ ] **Update CI/CD** to remove postinstall expectations
- [ ] **Document changes** in CHANGELOG or migration guide

---

## Known Limitations

### npm Workspace Hoisting

- **Issue**: npm hoists dependencies unpredictably in workspaces
- **Impact**: Can cause version conflicts even with explicit dependencies
- **Workaround**: Use `npm install --legacy-peer-deps` if needed

### pnpm vs npm Differences

- **pnpm**: Stricter dependency isolation, may require different configuration
- **npm**: More lenient hoisting, issues documented in this guide apply
- **Recommendation**: Stick with one package manager across team

### Module Resolution in Tests

- **Issue**: Test environments may not resolve @nuxt/kit correctly
- **Solution**: Mock Nuxt composables explicitly in test setup
- **Reference**: See [KB-LESSONS] for Vitest mocking strategies

---

## References

### Official Documentation

- [Nuxt 4 Migration Guide](https://nuxt.com/docs/getting-started/upgrade#nuxt-4)
- [npm Workspaces Documentation](https://docs.npmjs.com/cli/v10/using-npm/workspaces)

### GitHub Issues

- [#33382 - Layer with dependencies in monorepo goes in postinstall loop](https://github.com/nuxt/nuxt/issues/33382)
- [#30910 - Ensure nuxt is loaded from cwd rather than parent dir](https://github.com/nuxt/nuxt/pull/30910)

### Internal References

- [KB-007] Vue.js 3 Patterns
- [KB-009] Vite Tooling
- [KB-LESSONS] Lessons Learned - Session 8. Januar 2026

---

## Version History

| Date | Version | Changes |
|------|---------|---------|
| 8. Januar 2026 | 1.0 | Initial documentation based on real-world troubleshooting |

---

**Maintained by**: @Frontend, @TechLead  
**Review Frequency**: After each Nuxt major/minor version update
