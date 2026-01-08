---
docid: ADR-114
title: MIGRATION_ROADMAP
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Component Migration Roadmap & Progress

**Date:** December 30, 2025  
**Owner:** @Frontend  
**Phase 1 Task:** Component Migration (5 SP)  
**Framework:** DaisyUI v5 (Tailwind CSS based)  
**Status:** Updated with DaisyUI approach  

---

## Migration Strategy

### Approach
1. **Use DaisyUI components** as primary framework (not custom Tailwind)
2. **Extend with Tailwind utilities** for layout and spacing
3. **Batch 1 (Phase 2):** High-priority components (3 components, 5 SP)
4. **Validation:** Daily code review with @TechLead

### Why DaisyUI?
- ✅ Battle-tested, accessible components
- ✅ WCAG 2.1 AA compliant by default
- ✅ Less custom code to maintain
- ✅ Consistent UI across applications
- ✅ Already imported in Store project
- ✅ Strong community support

### Priority Matrix

| Component | Impact | Effort | Priority | Framework |
|-----------|--------|--------|----------|-----------|
| Checkout.vue | Critical | 2h | **P0** | DaisyUI |
| App.vue | Critical | 1.5h | **P0** | DaisyUI |
| RegistrationCheck.vue | High | 1.5h | **P0** | DaisyUI |
| CheckoutTermsStep.vue | High | 1h | **P1** | DaisyUI |
| ProductPrice.vue | Medium | 1h | **P1** | DaisyUI |
| B2BVatIdInput.vue | Low | 0.5h | **P2** | DaisyUI |

---

## Batch 1 Migration (Phase 2 - 5 SP)

### Component 1: Checkout.vue (Critical CTA flow)

**Current State:** Bootstrap + mixed styling  
**Target State:** Tailwind CSS, fully responsive  
**Estimated Effort:** 2 SP  

**Migration Steps:**
1. Map Bootstrap classes to Tailwind equivalents
   - `.container` → `max-w-7xl mx-auto px-4`
   - `.row` → `flex flex-wrap` or `grid grid-cols-*`
   - `.col-*` → `w-*`, `md:w-*` equivalents
   - `.btn btn-primary` → Button component with Tailwind classes
   
2. Convert inline styles to Tailwind utility classes
   - Remove `style=""` attributes
   - Replace with Tailwind classes
   
3. Update component structure if needed
   - Ensure semantic HTML (`<button>`, `<form>`, `<section>`)
   - Add accessibility attributes (`aria-*`)
   
4. Test responsiveness
   - Desktop (1024px+)
   - Tablet (640-768px)
   - Mobile (< 640px)
   
5. Submit PR for code review

**Key Changes:**
- All buttons → use Button.vue component
- Form inputs → use FormInput.vue component
- Cards/sections → use Card.vue component

---

### Component 2: App.vue (Root application layout)

**Current State:** Application shell with Bootstrap  
**Target State:** Root layout with Tailwind CSS  
**Estimated Effort:** 1.5 SP  

**Migration Steps:**
1. Convert header/navigation
   - Navigation container: `flex items-center justify-between px-4 py-3`
   - Navigation links: `text-primary-600 hover:text-primary-700`
   - Mobile menu: responsive `hidden md:flex`
   
2. Convert main layout
   - Main container: `min-h-screen bg-neutral-50`
   - Content area: `max-w-7xl mx-auto`
   
3. Convert footer
   - Footer structure: `mt-auto`
   - Footer links: `text-neutral-600 hover:text-neutral-900`
   
4. Test layout across all page types
   - Home page
   - Product page
   - Checkout page
   
5. Submit PR for code review

**Key Components:**
- Header (navigation)
- Main content area
- Footer
- Responsive mobile menu

---

### Component 3: RegistrationCheck.vue (User authentication)

**Current State:** Bootstrap form  
**Target State:** Tailwind CSS form with validation  
**Estimated Effort:** 1.5 SP  

**Migration Steps:**
1. Convert form structure
   - Form container: `max-w-md mx-auto p-6`
   - Form fields: use FormInput.vue component
   - Submit button: use Button.vue component
   
2. Convert validation feedback
   - Error messages: `text-error-500 text-sm mt-1`
   - Success state: `border-success-500`
   - Loading state: opacity changes, disabled buttons
   
3. Update form styling
   - Labels: `text-sm font-medium text-neutral-700`
   - Placeholders: `placeholder-neutral-400`
   - Help text: `text-neutral-500 text-sm`
   
4. Test form interactions
   - Valid input flow
   - Error validation
   - Submission flow
   
5. Submit PR for code review

**Key Features:**
- Email validation
- Password validation
- Real-time feedback
- Accessibility (labels, error descriptions)

---

## Migration Progress Tracking

### Phase 1 Component Status

| Component | Status | Bootstrap Classes Found | Tailwind Classes Applied | PR Submitted | Code Review | Merged |
|-----------|--------|-------------------------|-------------------------|--------------|------------|--------|
| Checkout.vue | 📋 Planned | 🔍 Pending | ⏳ Ready | ⏳ Pending | ⏳ Pending | ⏳ Pending |
| App.vue | 📋 Planned | 🔍 Pending | ⏳ Ready | ⏳ Pending | ⏳ Pending | ⏳ Pending |
| RegistrationCheck.vue | 📋 Planned | 🔍 Pending | ⏳ Ready | ⏳ Pending | ⏳ Pending | ⏳ Pending |

---

## Bootstrap → Tailwind Mapping Reference

### Layout Classes

| Bootstrap | Tailwind | Notes |
|-----------|----------|-------|
| `.container` | `max-w-7xl mx-auto px-4 sm:px-6 lg:px-8` | Responsive padding |
| `.row` | `flex flex-wrap` or `grid grid-cols-*` | Choose based on layout |
| `.col-md-6` | `md:w-1/2` | Responsive width |
| `.col-12` | `w-full` | Full width |
| `.d-flex` | `flex` | Display flex |
| `.justify-content-center` | `justify-center` | Flexbox alignment |
| `.align-items-center` | `items-center` | Vertical alignment |

### Spacing Classes

| Bootstrap | Tailwind | Notes |
|-----------|----------|-------|
| `.mt-3` | `mt-3` (12px) | Margin top |
| `.mb-4` | `mb-4` (16px) | Margin bottom |
| `.p-3` | `p-3` (12px) | Padding |
| `.px-4` | `px-4` (16px) | Horizontal padding |
| `.py-2` | `py-2` (8px) | Vertical padding |

### Typography Classes

| Bootstrap | Tailwind | Notes |
|-----------|----------|-------|
| `.h1`, `.h2`, `.h3` | `text-4xl`, `text-3xl`, `text-2xl` | Heading sizes |
| `.text-muted` | `text-neutral-600` | Secondary text |
| `.text-danger` | `text-error-600` | Error text |
| `.font-weight-bold` | `font-bold` | Font weight |
| `.text-center` | `text-center` | Text alignment |

### Component Classes

| Bootstrap | Tailwind | Notes |
|-----------|----------|-------|
| `.btn .btn-primary` | Button.vue component | Use Vue component |
| `.form-control` | FormInput.vue component | Use Vue component |
| `.card` | Card.vue component | Use Vue component |
| `.badge .badge-primary` | Badge.vue component | Use Vue component |
| `.alert .alert-danger` | Alert.vue component | Use Vue component |

---

## Code Review Process

### Daily Review Checklist (For @TechLead)

✅ Visual regression: No layout breaks  
✅ Responsive design: Mobile, tablet, desktop  
✅ Accessibility: WCAG 2.1 AA compliance  
✅ Performance: Bundle size impact  
✅ Component usage: Using B2X design system  
✅ Code quality: Clean, maintainable code  
✅ Test coverage: Unit tests for logic  

### Review Turnaround
- Submission: Daily
- Initial review: < 4 hours
- Feedback resolution: Same day
- Merge: After approval

---

## Testing Checklist

### Browser Testing
- ✅ Chrome (latest)
- ✅ Firefox (latest)
- ✅ Safari (latest)
- ✅ Mobile Safari (iOS 15+)
- ✅ Chrome Mobile (Android)

### Responsive Testing
- ✅ Mobile (375px - iPhone SE)
- ✅ Tablet (768px - iPad)
- ✅ Desktop (1024px+)
- ✅ Wide (1536px+ - large desktop)

### Accessibility Testing
- ✅ Keyboard navigation (Tab through all elements)
- ✅ Screen reader (VoiceOver/NVDA)
- ✅ Focus indicators (visible on all interactive elements)
- ✅ Color contrast (WCAG AA minimum)

---

## Known Limitations & Notes

### DaisyUI Integration
- DaisyUI v5 is imported via CSS (not plugin)
- May conflict with some custom Tailwind classes
- Monitor for specificity issues

### Browser Support
- Modern browsers (Chrome, Firefox, Safari, Edge)
- IE 11 not supported (legacy)
- Mobile: iOS 13+, Android 5+

### Performance Considerations
- Tailwind purges unused CSS in production
- Custom components may increase bundle size
- Monitor with: `npm run build`

---

## Success Criteria (Phase 1)

✅ 3 priority components migrated (Checkout, App, Registration)  
✅ All Bootstrap classes removed from migrated components  
✅ Visual parity with original design  
✅ Responsive design verified  
✅ Accessibility compliant (WCAG 2.1 AA)  
✅ Daily code review passed  
✅ All tests passing  
✅ PRs merged to main branch  

---

## Timeline (Phase 1)

- **Day 1 (Today):** Planning + Component 1 Checkout (2 SP)
- **Day 2:** Component 2 App (1.5 SP) + Component 3 Registration (1.5 SP)
- **Code Review:** Daily, turnaround < 4 hours
- **Merge:** After approval same day

---

## Next Steps (Phase 2)

After Phase 1 complete (5 SP):
1. Migrate remaining medium-priority components
2. Update ERP/CMS/Shop subdirectories
3. Comprehensive accessibility audit
4. Performance optimization
5. Documentation update

---

## Summary

**Status:** ✅ PLANNED, ⏳ IMPLEMENTATION READY

Migration roadmap defined. Bootstrap → Tailwind mapping complete. Code review process established. 3 priority components ready for migration. Phase 1 targets 5 SP.

**Next:** Implement Batch 1 migrations (Checkout.vue first)

---

**Planned By:** @Frontend  
**Date:** Dec 30, 2025  
**Migrations Planned:** 3 components in Phase 1  
**Phase 1 Scope:** 5 SP  
**For Code Review:** @TechLead (daily)
