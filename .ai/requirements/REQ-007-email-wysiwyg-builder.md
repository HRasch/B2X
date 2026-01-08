---
docid: REQ-058
title: REQ 007 Email Wysiwyg Builder
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# REQ-007: Email WYSIWYG Builder with Drag & Drop

**DocID**: `REQ-007`  
**Status**: Draft  
**Created**: 2026-01-05  
**Owner**: @ProductOwner  
**Domain**: CMS / Email Templates  
**Priority**: P2

---

## Executive Summary

Enhance the email template editor with a WYSIWYG (What You See Is What You Get) builder featuring drag-and-drop widgets, pre-built components, and visual editing. This will enable non-technical users (content creators, marketing) to create professional HTML emails without coding knowledge, while maintaining the Monaco code editor for advanced users.

---

## Current State Analysis

### Existing Implementation
- **Monaco Code Editor**: Raw HTML/Liquid editing with syntax highlighting
- **Target Users**: Developers and technical content creators
- **Limitations**:
  - Requires HTML/CSS knowledge
  - No visual feedback during editing
  - High barrier to entry for non-technical users
  - Manual responsive design implementation
  - No reusable component library

### User Feedback
- Marketing team struggles with HTML syntax
- Content creators want visual editing like WordPress/Mailchimp
- Time-consuming to create consistent email layouts
- Difficult to ensure mobile responsiveness

---

## Business Value

| Benefit | Impact |
|---------|--------|
| **User Empowerment** | Non-technical staff can create emails independently |
| **Time Savings** | 60-80% reduction in email template creation time |
| **Consistency** | Pre-built widgets ensure brand consistency |
| **Reduced Errors** | Visual editing reduces HTML/CSS mistakes |
| **Faster Iteration** | Marketing can A/B test layouts quickly |
| **Developer Bandwidth** | Frees developers from routine email design tasks |

**ROI Estimate**: 20-30 hours/month saved across marketing and development teams

---

## User Stories

### US-7.1: Visual Email Editing

**As a** Content Creator  
**I want** a visual drag-and-drop email builder  
**So that** I can create professional emails without coding

**Acceptance Criteria:**
- [ ] Visual canvas shows email preview while editing
- [ ] Real-time updates as components are modified
- [ ] Desktop and mobile preview modes
- [ ] Undo/redo functionality
- [ ] Auto-save drafts

### US-7.2: Drag & Drop Widgets

**As a** Marketing Manager  
**I want** pre-built email widgets (header, footer, CTA, product grid)  
**So that** I can quickly assemble professional layouts

**Acceptance Criteria:**
- [ ] Widget library with common components (header, hero, text, image, button, product, footer)
- [ ] Drag widgets from sidebar to canvas
- [ ] Reorder widgets via drag-and-drop
- [ ] Delete widgets easily
- [ ] Duplicate existing widgets

### US-7.3: Widget Customization

**As a** Content Creator  
**I want** to customize widget properties visually  
**So that** I can match brand guidelines without CSS

**Acceptance Criteria:**
- [ ] Visual property panel for selected widget
- [ ] Text editing inline or in panel
- [ ] Color picker for backgrounds, text, buttons
- [ ] Font family, size, weight selectors
- [ ] Spacing/padding controls (visual sliders)
- [ ] Image upload/selection from media library
- [ ] Link editor with URL validation

### US-7.4: Responsive Design

**As a** Marketing Manager  
**I want** emails to look good on all devices automatically  
**So that** I don't need to handle responsive CSS manually

**Acceptance Criteria:**
- [ ] All widgets are mobile-responsive by default
- [ ] Desktop/tablet/mobile preview toggle
- [ ] Automatic column stacking on mobile
- [ ] Test on real devices (preview links)

### US-7.5: Code View Toggle

**As a** Technical User  
**I want** to switch between visual and code views  
**So that** I can fine-tune the HTML when needed

**Acceptance Criteria:**
- [ ] Toggle between WYSIWYG and Monaco code editor
- [ ] Changes in code view reflected in visual view
- [ ] Changes in visual view update code
- [ ] Syntax validation prevents breaking visual editor

### US-7.6: Template Library

**As a** Content Creator  
**I want** pre-designed email templates  
**So that** I can start with proven layouts

**Acceptance Criteria:**
- [ ] Gallery of starter templates (newsletter, promotion, transactional)
- [ ] Preview template before selection
- [ ] Duplicate and customize templates
- [ ] Save custom layouts as new templates

---

## Technical Requirements

### Architecture

#### Dual-Mode Editor
```
┌─────────────────────────────────────┐
│  Email Template Editor              │
├─────────────────────────────────────┤
│  [Visual] [Code] Toggle             │
├─────────────────────────────────────┤
│                                     │
│  Visual Mode:                       │
│  ┌─────────┬─────────────┬────────┐│
│  │ Widgets │   Canvas    │ Props  ││
│  │ Library │  (Preview)  │ Panel  ││
│  └─────────┴─────────────┴────────┘│
│                                     │
│  Code Mode:                         │
│  ┌───────────────────────────────┐ │
│  │   Monaco Editor (HTML/Liquid) │ │
│  └───────────────────────────────┘ │
│                                     │
└─────────────────────────────────────┘
```

#### Data Flow
1. **WYSIWYG → JSON**: Visual editor stores component tree as JSON
2. **JSON → HTML**: Renderer converts JSON to HTML/Liquid
3. **HTML → Preview**: Live preview shows rendered output
4. **Code Changes**: Monaco edits HTML directly, re-parsed to JSON for visual mode

### Technology Options

#### Option 1: GrapesJS (Open Source)
**Pros:**
- Open source, free
- Excellent drag-and-drop UX
- Component-based architecture
- Good email template support via `grapesjs-preset-newsletter`
- Active community, extensible

**Cons:**
- Requires custom integration with Vue 3
- Styling needs customization for brand consistency
- Learning curve for advanced customization

**Recommendation:** ⭐ Best fit for B2X

#### Option 2: Unlayer (Commercial)
**Pros:**
- Beautiful UI, professional
- Email-specific features (merge tags, A/B testing)
- Excellent mobile responsiveness
- Vue.js integration available

**Cons:**
- **Cost:** $99-$499/month per editor instance
- Vendor lock-in
- Limited customization without enterprise plan

#### Option 3: Email Editor (React Email)
**Pros:**
- Modern React-based
- Code-first with visual preview
- Good for developers

**Cons:**
- React-only (not Vue compatible)
- Less visual than GrapesJS/Unlayer
- Requires significant custom work for WYSIWYG

#### Option 4: Custom Builder (Build from Scratch)
**Pros:**
- Full control
- Perfect integration with B2X
- No licensing costs

**Cons:**
- **Effort:** 6-8 weeks development time
- Maintenance burden
- Feature parity with GrapesJS requires significant work

### Recommended: GrapesJS Integration

**Package:** `grapesjs` + `grapesjs-preset-newsletter`  
**Vue Wrapper:** Custom Vue 3 component wrapper

#### Installation
```bash
npm install grapesjs grapesjs-preset-newsletter
```

#### Component Structure
```
src/components/email/
├── EmailBuilderWYSIWYG.vue       # GrapesJS wrapper
├── EmailBuilderCodeEditor.vue    # Monaco editor (existing)
├── EmailTemplateEditor.vue       # Parent with mode toggle
└── widgets/
    ├── CustomButton.ts           # Custom widget definitions
    ├── ProductGrid.ts
    └── Header.ts
```

---

## Implementation Plan

### Phase 1: POC (1 week)
- [ ] Install GrapesJS + newsletter preset
- [ ] Create basic Vue wrapper
- [ ] Test drag-and-drop functionality
- [ ] Verify HTML export quality

### Phase 2: Core Integration (2 weeks)
- [ ] Dual-mode editor (WYSIWYG ↔ Code toggle)
- [ ] Save/load JSON component tree
- [ ] Integrate with existing email template API
- [ ] Basic widget library (text, image, button, divider)

### Phase 3: Custom Widgets (1 week)
- [ ] B2X-specific widgets (product grid, customer info)
- [ ] Liquid variable insertion UI
- [ ] Brand color palette integration
- [ ] Custom fonts from B2X theme

### Phase 4: Polish & UX (1 week)
- [ ] Multilingual support (i18n)
- [ ] Responsive preview modes
- [ ] Template library with starters
- [ ] Accessibility improvements
- [ ] User documentation

### Phase 5: Testing & Launch (1 week)
- [ ] Unit tests for Vue wrapper
- [ ] E2E tests for email creation workflow
- [ ] Cross-browser testing
- [ ] Security audit (XSS prevention)
- [ ] Performance optimization

**Total Estimate:** 6 weeks (1.5 sprints)

---

## User Experience Design

### Layout
```
┌────────────────────────────────────────────────────────────┐
│ Email Template Editor                    [Visual] [Code]   │
├──────────┬──────────────────────────┬─────────────────────┤
│          │                          │                      │
│ Widgets  │      Canvas (Preview)    │  Properties Panel    │
│          │                          │                      │
│ □ Text   │  ┌──────────────────┐   │  Selected: Button    │
│ □ Image  │  │     Header       │   │  ┌─────────────────┐ │
│ □ Button │  ├──────────────────┤   │  │ Text: Shop Now  │ │
│ □ Divider│  │   Hero Image     │   │  │ URL: /shop      │ │
│ □ Product│  ├──────────────────┤   │  │ Color: #FF5733  │ │
│ □ Footer │  │  "Lorem ipsum…"  │   │  │ Padding: 12px   │ │
│          │  ├──────────────────┤   │  └─────────────────┘ │
│ + Custom │  │  [Shop Now] CTA  │   │                      │
│          │  ├──────────────────┤   │  [Preview Mobile]    │
│          │  │     Footer       │   │  [Send Test Email]   │
│          │  └──────────────────┘   │                      │
│          │                          │                      │
└──────────┴──────────────────────────┴─────────────────────┘
```

### Interaction Flow
1. User selects widget from sidebar
2. Drags to canvas
3. Clicks widget to select
4. Edits properties in right panel
5. Toggles to code view for advanced tweaks
6. Saves template

---

## Security Considerations

- **XSS Prevention**: Sanitize all HTML output
- **Liquid Injection**: Validate Liquid syntax
- **Image Uploads**: Scan for malware, limit file types
- **Output Validation**: Ensure generated HTML is safe
- **User Permissions**: Restrict WYSIWYG to authorized roles

---

## Accessibility Requirements

- Keyboard navigation for widget library
- ARIA labels for drag-and-drop areas
- Screen reader support for visual editor
- High contrast mode compatibility
- Focus indicators for all interactive elements

---

## Multilingual Support

- Widget labels localized (en, de, fr, es, it, pt, nl, pl)
- Property panel text translated
- Template library with multilingual descriptions
- RTL support for future languages

---

## Testing Strategy

### Unit Tests
- Vue wrapper component mounting
- JSON ↔ HTML conversion
- Widget registration

### Integration Tests
- Save/load templates
- Mode switching (WYSIWYG ↔ Code)
- API integration

### E2E Tests
- Drag widget to canvas
- Edit properties and preview
- Send test email
- Create template from scratch

### Visual Regression
- Screenshot comparison for widget rendering
- Email preview consistency

---

## Success Metrics

| Metric | Target |
|--------|--------|
| **Adoption Rate** | 80% of content creators use WYSIWYG within 2 months |
| **Time to Create** | <15 minutes for standard newsletter |
| **User Satisfaction** | >4.5/5 rating from content creators |
| **Error Rate** | <5% of emails require code-level fixes |
| **Training Time** | <30 minutes for new users |

---

## Dependencies

- REQ-003: Email Template System (base functionality)
- KB-026: Monaco Editor Vue Integration (code editor)
- ADR-027: Email Template Engine (Liquid rendering)

---

## Out of Scope (Future Enhancements)

- **AI-Powered Suggestions**: AI generates layouts from prompts
- **A/B Testing UI**: Visual A/B variant creator
- **Analytics Dashboard**: Email performance metrics
- **Dynamic Content Blocks**: Personalized widget variants
- **Collaboration Features**: Multi-user editing, comments

---

## Risks & Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| GrapesJS incompatibility with Vue 3 | High | POC phase validates integration |
| Performance with large templates | Medium | Lazy load widgets, optimize rendering |
| User learning curve | Medium | Comprehensive onboarding, templates |
| Code ↔ Visual sync issues | High | Robust JSON ↔ HTML parser with validation |

---

## Appendix: Alternative Solutions Considered

### Email Builder Libraries Comparison

| Feature | GrapesJS | Unlayer | Stripo | Custom |
|---------|----------|---------|--------|--------|
| **Cost** | Free | $99-$499/mo | $149+/mo | Dev time |
| **Vue Support** | Custom wrapper | Official | No | Native |
| **Customization** | High | Medium | Low | Full |
| **Email Templates** | Yes (preset) | Excellent | Excellent | Build all |
| **Drag & Drop** | Excellent | Excellent | Good | Build |
| **Mobile Preview** | Yes | Yes | Yes | Build |
| **Code Editor** | Basic | Good | Limited | Monaco |
| **Open Source** | ✅ | ❌ | ❌ | ✅ |

**Winner:** GrapesJS for cost, flexibility, and open-source benefits.

---

## References

- GrapesJS: https://grapesjs.com/
- GrapesJS Newsletter Preset: https://github.com/artf/grapesjs-preset-newsletter
- Unlayer: https://unlayer.com/
- Email HTML Best Practices: https://templates.mailchimp.com/resources/email-client-css-support/
- Liquid Template Language: https://shopify.github.io/liquid/

---

**Next Steps:**
1. @ProductOwner: Review and approve requirement
2. @Architect: Create ADR for GrapesJS vs alternatives
3. @Frontend: POC implementation (1 week)
4. @UX: Design widget library and property panels
5. @UI: Brand integration (colors, fonts, logos)

**Last Updated:** 2026-01-05  
**Review Date:** 2026-01-12 (after POC)
