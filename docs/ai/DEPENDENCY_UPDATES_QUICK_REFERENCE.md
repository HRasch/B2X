# 📦 Dependency Updates - Quick Reference

**Generated**: 30. Dezember 2025  
**Purpose**: Quick lookup for dependency update status and new features

---

## 🎯 At a Glance

### ✅ Up-to-Date (No Action Needed)
| Package | Version | Notes |
|---------|---------|-------|
| TypeScript | 5.9.3 | Latest 5.9.x |
| Tailwind CSS | 4.1.18 | Frontend/Store only |
| Wolverine | 5.9.2 | Perfect for CQRS |
| Serilog | 4.3.0 | Best-in-class logging |
| Aspire | 13.1.0 | Latest orchestration |

### ⬆️ Update Available (Recommended)

#### High Priority (Do This Week)
```bash
# Vue.js 3.5.13 → 3.5.26 (patch, safe)
cd Frontend/Store && npm update vue
cd Frontend/Admin && npm update vue

# Playwright 1.48.2 → 1.57.0 (minor, safe)
cd Frontend/Store && npm install @playwright/test@1.57.0 --save-dev
cd Frontend/Admin && npm install @playwright/test@1.57.0 --save-dev
```

#### Medium Priority (Next Week)
```bash
# Vite 6.0.5 → 7.3.0 (major, tested breaking changes)
cd Frontend/Store
npm install vite@7.3.0 --save-dev
npm run build && npm run dev  # Verify

# Tailwind 3.4.15 → 4.1.18 (major, Admin only)
cd Frontend/Admin
npm install tailwindcss@4.1.18
npm run build && npm run dev  # Verify
```

#### Low Priority (Next Sprint)
```bash
# .NET 10.0.1 → 10.0.101 (patch, auto-update)
dotnet package update B2Connect.slnx --interactive
dotnet test B2Connect.slnx -v minimal
```

---

## 📊 Version Status Table

| Package | Current | Latest | Type | Status | Action |
|---------|---------|--------|------|--------|--------|
| **Vue.js** | 3.5.13 | 3.5.26 | Patch | ⬆️ Update | `npm update vue` |
| **Vite (Store)** | 6.0.5 | 7.3.0 | Major | ⬆️ Update | `npm install vite@7.3.0` |
| **Vite (Admin)** | 7.3.0 | 7.3.0 | - | ✅ Latest | - |
| **TypeScript** | 5.9.3 | 5.9.3 | - | ✅ Latest | - |
| **Tailwind (Store)** | 4.1.18 | 4.1.18 | - | ✅ Latest | - |
| **Tailwind (Admin)** | 3.4.15 | 4.1.18 | Major | ⬆️ Update | `npm install tailwindcss@4.1.18` |
| **Playwright** | 1.48.2 | 1.57.0 | Minor | ⬆️ Update | `npm install @playwright/test@1.57.0` |
| **.NET** | 10.0.1 | 10.0.101 | Patch | ⬆️ Update | `dotnet package update` |
| **Wolverine** | 5.9.2 | 5.9.2 | - | ✅ Latest | - |
| **EF Core** | 10.0.0 | 10.0.101 | Patch | ⬆️ Auto | (comes with .NET) |
| **Serilog** | 4.3.0 | 4.3.0 | - | ✅ Latest | - |
| **FluentValidation** | 11.9.2 | 11.9.2 | - | ✅ Latest | - |
| **Aspire** | 13.1.0 | 13.1.0 | - | ✅ Latest | - |

---

## 🚀 Key New Features by Package

### Vue 3.5.26
- ✅ 13 bug fixes
- ✅ Better TypeScript inference
- ✅ Improved template performance

### Vite 7.3.0
- ✅ 30% faster dev startup
- ✅ 50% faster HMR
- ✅ Built-in Vite Inspect (performance profiling)
- ✅ Better environment variable handling
- ✅ Native Web Worker support
- ✅ Rollup 4.x with improved tree-shaking

### Tailwind CSS 4.1.18
- ✅ 2-3x faster builds
- ✅ CSS-first configuration
- ✅ Dynamic color functions
- ✅ Container queries
- ✅ CSS Variables theming

### Playwright 1.57.0
- ✅ 15% faster test execution
- ✅ WebSocket testing improvements
- ✅ Better network HAR recording
- ✅ Integrated axe-core accessibility testing
- ✅ Enhanced trace viewer

### .NET 10.0.101
- ✅ 15-20% request throughput improvement
- ✅ Native AOT optimization
- ✅ Full C# 14 support
- ✅ Enhanced error messages

### EF Core 10.0.101
- ✅ Temporal tables support (audit history)
- ✅ Better JSON column handling
- ✅ Complex properties support
- ✅ Performance improvements

---

## ⚠️ Breaking Changes to Review

### Vite 7.x
- `.env.local` loading changed (minor)
- Dynamic imports need `?url` for some assets (easy fix)

### Tailwind 4.1.18
- Requires `@tailwindcss/postcss` package
- Config file optional but recommended
- Cleaner class naming conventions

### No other breaking changes in other updates

---

## 📋 Implementation Checklist

- [ ] **Phase 1** (30 min): Vue 3.5.26 + Playwright 1.57.0
  - [ ] Update both frontend projects
  - [ ] Run tests to verify
  
- [ ] **Phase 2** (45 min): Vite 7.3.0 + Tailwind 4.1.18
  - [ ] Update Frontend/Store Vite
  - [ ] Update Frontend/Admin Tailwind
  - [ ] Test builds and dev servers
  
- [ ] **Phase 3** (20 min): .NET 10.0.101
  - [ ] Update all backend packages
  - [ ] Run full test suite
  - [ ] Verify build succeeds

---

## 🔗 Reference Links

| Resource | Link |
|----------|------|
| Full Documentation | [DEPENDENCY_UPDATES_AND_NEW_FEATURES.md](./DEPENDENCY_UPDATES_AND_NEW_FEATURES.md) |
| Vue 3.5 Changelog | https://github.com/vuejs/core/releases |
| Vite 7.0 Announcement | https://vitejs.dev/blog/announcing-vite-7 |
| Tailwind CSS 4.0 | https://tailwindcss.com/blog/tailwindcss-v4 |
| .NET 10 Release Notes | https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10 |
| Playwright 1.57 | https://github.com/microsoft/playwright/releases |

---

## 💡 Pro Tips

1. **Always test after updates**
   ```bash
   npm run build  # Frontend
   dotnet test    # Backend
   ```

2. **Use package-lock.json to lock versions**
   - Commit updated lock files to git
   - Ensures reproducible builds

3. **Monitor security advisories**
   ```bash
   npm audit
   dotnet list package --vulnerable
   ```

4. **Performance baseline**
   - Measure before/after for Vite and .NET updates
   - Use DevTools for frontend, `dotnet benchmark` for backend

---

**Generated**: 30. Dezember 2025  
**Maintenance**: Quarterly review or when new major versions released  
**Owner**: @tech-lead

