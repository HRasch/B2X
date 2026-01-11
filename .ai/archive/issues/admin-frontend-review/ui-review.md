---
docid: UNKNOWN-134
title: Ui Review
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Admin Frontend - Design System & Accessibility Review

**DocID**: `REV-UI-001`  
**Date**: 2026-01-01  
**Reviewer**: @UI  
**Scope**: `/frontend/Admin/` - Soft UI Design System & WCAG 2.1 AA Compliance  
**Agents**: @UI | Owner: @Frontend

---

## Summary Assessment

The Admin Frontend features a well-structured **Soft UI Design System** with comprehensive Tailwind configuration, dark mode support, and reusable components. However, **critical accessibility gaps** exist in keyboard navigation, ARIA attributes, and focus management that must be addressed for WCAG 2.1 AA compliance.

---

## ✅ Strengths

| Area | Finding |
|------|---------|
| **Design Tokens** | Excellent color palette with 10-step scales (primary, success, warning, danger, info, soft) providing consistent semantic meaning and sufficient contrast ratios for text |
| **Theme System** | Robust light/dark mode implementation via Pinia store with system preference detection, localStorage persistence, and smooth CSS transitions |
| **Shadow System** | Comprehensive soft shadow scale (xs to 2xl) with dark mode variants that maintain depth perception without harshness |
| **Component Architecture** | Clean, composable Soft UI component library (`SoftCard`, `SoftButton`, `SoftInput`, `SoftBadge`, `SoftPanel`) with consistent prop APIs and slot-based customization |
| **Responsive Foundation** | Proper mobile-first approach with Tailwind breakpoints (md:, lg:) and mobile sidebar toggle pattern in MainLayout |

---

## ⚠️ Issues Found

| Severity | Issue | Location | Recommendation |
|----------|-------|----------|----------------|
| 🔴 **Critical** | Missing `aria-label` on icon-only buttons | [MainLayout.vue](src/components/common/MainLayout.vue#L33-L43), [UserList.vue](src/views/users/UserList.vue#L135-L155) | Add descriptive `aria-label` attributes (e.g., `aria-label="Close sidebar"`, `aria-label="View user"`) |
| 🔴 **Critical** | Modal dialogs lack focus trap and `role="dialog"` | [UserList.vue](src/views/users/UserList.vue#L203-L229) | Implement focus trap, add `role="dialog"`, `aria-modal="true"`, `aria-labelledby` pointing to header |
| 🔴 **Critical** | Missing keyboard navigation for dropdown menus | [MainLayout.vue](src/components/common/MainLayout.vue#L156-L175) | Add `@keydown.escape` close, arrow key navigation, and `role="menu"` / `role="menuitem"` |
| 🟠 **Major** | Tables lack proper `scope` attributes on `<th>` | [UserList.vue](src/views/users/UserList.vue#L91-L98), [Pages.vue](src/views/cms/Pages.vue#L35-L48) | Add `scope="col"` to all `<th>` elements for screen reader column association |
| 🟠 **Major** | Form inputs missing `aria-describedby` for error messages | [SoftInput.vue](src/components/soft-ui/SoftInput.vue#L34-L36) | Connect error message to input via `aria-describedby` and add `aria-invalid="true"` when error exists |
| 🟠 **Major** | Inconsistent focus ring implementation | Various components | `focus:ring-2` used inconsistently; some buttons use `focus:outline-none` without visible focus indicator replacement |
| 🟠 **Major** | CSS style inconsistency between views | [UserList.vue](src/views/users/UserList.vue) vs [Dashboard.vue](src/views/Dashboard.vue) | UserList uses scoped CSS with hardcoded colors; Dashboard uses Tailwind classes. Standardize on Tailwind/design system tokens |
| 🟡 **Minor** | Loading spinner lacks `aria-live` region | [UserList.vue](src/views/users/UserList.vue#L56-L59) | Wrap loading state in `aria-live="polite"` region for screen reader announcement |
| 🟡 **Minor** | Color-only status indication | [UserList.vue](src/views/users/UserList.vue#L118-L126) | Add icon or text pattern alongside color for status badges (colorblind accessibility) |
| 🟡 **Minor** | Missing `<main>` landmark role context | [MainLayout.vue](src/components/common/MainLayout.vue#L229) | Add `aria-label="Main content"` to `<main>` element for better landmark identification |
| 🟡 **Minor** | Conflicting base styles in `style.css` | [style.css](src/style.css) | Contains Vite scaffold defaults that conflict with `main.css`. Consider removing or consolidating |
| 🟡 **Minor** | Notification badge lacks context | [MainLayout.vue](src/components/common/MainLayout.vue#L137-L140) | Add screen reader text for notification count (e.g., `<span class="sr-only">3 unread notifications</span>`) |
| ⚪ **Info** | Theme toggle button missing explicit button type | [ThemeToggle.vue](src/components/common/ThemeToggle.vue#L4) | Add `type="button"` to prevent accidental form submission in nested contexts |
| ⚪ **Info** | German/English language mixing | [UserList.vue](src/views/users/UserList.vue), [MainLayout.vue](src/components/common/MainLayout.vue) | Standardize on one language or implement proper i18n |

---

## 📋 Top 5 Recommendations (Prioritized)

### 1. **Implement Comprehensive ARIA Attributes** (Critical - Sprint Priority)
Add missing ARIA attributes across all interactive elements:
```vue
<!-- Icon button example -->
<button
  @click="sidebarOpen = false"
  type="button"
  aria-label="Close navigation menu"
  class="..."
>
  <svg aria-hidden="true">...</svg>
</button>

<!-- Modal dialog example -->
<div
  v-if="showDeleteModal"
  role="dialog"
  aria-modal="true"
  aria-labelledby="delete-modal-title"
  class="modal-overlay"
>
  <h4 id="delete-modal-title">Benutzer löschen?</h4>
</div>
```

### 2. **Create Focus Management Composable** (Critical)
Build a reusable `useFocusTrap` composable for modals and dropdowns:
```typescript
// src/composables/useFocusTrap.ts
export function useFocusTrap(containerRef: Ref<HTMLElement | null>) {
  // Trap focus within container
  // Handle Escape key
  // Return focus to trigger on close
}
```

### 3. **Standardize Component Styling on Design System** (Major)
Refactor `UserList.vue` and similar views to use Soft UI components and Tailwind classes exclusively:
- Replace hardcoded CSS colors (`#3b82f6`, `#1f2937`) with design tokens (`bg-primary-500`, `text-soft-900`)
- Use `SoftButton`, `SoftCard`, `SoftBadge` instead of custom styled elements
- Remove scoped CSS in favor of utility classes

### 4. **Add Table Accessibility Enhancements** (Major)
Create a reusable accessible table component:
```vue
<!-- SoftTable.vue -->
<table role="grid" aria-label="User list">
  <thead>
    <tr>
      <th scope="col" aria-sort="ascending">Name</th>
      ...
    </tr>
  </thead>
  <tbody role="rowgroup">
    <tr v-for="row in rows" role="row" tabindex="0">
      ...
    </tr>
  </tbody>
</table>
```

### 5. **Implement Skip Links and Landmark Navigation** (Minor but Important)
Add skip link component at the top of the layout:
```vue
<!-- src/components/common/SkipLink.vue -->
<a
  href="#main-content"
  class="sr-only focus:not-sr-only focus:absolute focus:z-50 focus:p-4 focus:bg-white"
>
  Skip to main content
</a>
```

---

## Accessibility Compliance Status

| WCAG 2.1 Criterion | Level | Status | Notes |
|-------------------|-------|--------|-------|
| 1.1.1 Non-text Content | A | ⚠️ Partial | SVG icons need `aria-hidden`, icon buttons need labels |
| 1.3.1 Info and Relationships | A | ❌ Fail | Tables missing `scope`, forms missing `aria-describedby` |
| 1.4.3 Contrast (Minimum) | AA | ✅ Pass | Color palette designed with 4.5:1+ ratios |
| 1.4.11 Non-text Contrast | AA | ✅ Pass | Focus rings and UI components have sufficient contrast |
| 2.1.1 Keyboard | A | ❌ Fail | Dropdowns and modals not keyboard accessible |
| 2.1.2 No Keyboard Trap | A | ⚠️ Partial | No focus trap in modals (both directions issue) |
| 2.4.1 Bypass Blocks | A | ❌ Fail | No skip links implemented |
| 2.4.3 Focus Order | A | ⚠️ Partial | Tab order generally logical but modals break flow |
| 2.4.4 Link Purpose | A | ✅ Pass | Links have clear context |
| 2.4.6 Headings and Labels | AA | ✅ Pass | Good heading structure |
| 2.4.7 Focus Visible | AA | ⚠️ Partial | Inconsistent focus indicators |
| 3.2.1 On Focus | A | ✅ Pass | No unexpected context changes |
| 4.1.2 Name, Role, Value | A | ❌ Fail | Missing ARIA on interactive widgets |

**Overall Compliance**: ⚠️ **Partially Compliant** (60-70%)

---

## Design System Quality Metrics

| Metric | Score | Assessment |
|--------|-------|------------|
| Token Coverage | 9/10 | Excellent color, spacing, shadow, typography tokens |
| Component Consistency | 7/10 | Soft UI components consistent; views vary |
| Dark Mode Support | 9/10 | Well-implemented with CSS variables and Tailwind |
| Responsive Design | 8/10 | Good breakpoints; some views need mobile optimization |
| Animation/Motion | 8/10 | Smooth transitions; respects `prefers-reduced-motion` (implicit via Tailwind) |
| Documentation | 8/10 | Good theme docs; component docs could expand |

---

## Sign-off Recommendation

### 🟡 **Changes Required**

The design system foundation is solid with excellent theming and visual consistency. However, the accessibility gaps—particularly around keyboard navigation, ARIA attributes, and focus management—present **legal and usability risks** that must be resolved before production release.

**Minimum Required Before Approval**:
1. Add ARIA labels to all icon-only buttons
2. Implement focus trap for modal dialogs
3. Add `scope="col"` to all table headers
4. Connect error messages to inputs via `aria-describedby`

**Recommended Timeline**: 2-3 sprints for full WCAG 2.1 AA compliance

---

*Review completed by @UI on 2026-01-01*
