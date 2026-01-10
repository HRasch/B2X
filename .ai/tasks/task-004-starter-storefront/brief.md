---
docid: TASK-004-BRIEF
title: Starter Headless Storefront Implementation Brief
owner: "@SARAH"
status: Active
created: 2026-01-10
---

# TASK-004: Starter Headless Storefront + CLI

**Priority**: P0 (Critical)  
**Domain**: Frontend  
**Effort Estimate**: Medium (3-6 dev-weeks)  
**Owner**: @Frontend  

## Objective
Implement a starter headless storefront using Next.js that connects to the Core API, plus a CLI tool (`b2c-cli dev`) for local development.

## Acceptance Criteria
- [ ] Next.js storefront with product listing and detail pages
- [ ] Cart functionality with add/remove items
- [ ] Checkout flow integration (when available)
- [ ] Responsive design (mobile-first)
- [ ] i18n support (en/de/fr minimum)
- [ ] CLI tool for local development (`b2c-cli dev`)
- [ ] API integration with Core API endpoints
- [ ] Basic SEO and performance optimization
- [ ] Unit and integration tests

## Technical Requirements
- Next.js 14+ with App Router
- TypeScript throughout
- Tailwind CSS for styling
- React Query for API state management
- Follow ADR-030 Vue-i18n migration patterns (adapted for React)
- Mobile-first responsive design
- Accessibility (WCAG 2.1 AA)

## Dependencies
- Core API (TASK-003) - REST endpoints available
- Authentication system (future integration)

## Deliverables
- Next.js storefront application
- CLI tool package
- Component library (reusable UI components)
- API client library
- E2E tests with Playwright
- Documentation and setup guide

## Timeline
- **Start**: 2026-01-10
- **Milestone 1**: Basic storefront structure (1 week)
- **Milestone 2**: Product pages and cart (2 weeks)
- **Milestone 3**: CLI tool and testing (2 weeks)
- **Completion**: 2026-02-01

## Risk Assessment
- **High**: API integration dependency on TASK-003
- **Medium**: CLI tool cross-platform compatibility
- **Low**: Next.js adoption (standard framework)

## Related Documents
- [REQ-048] MVP Backlog
- [ADR-030] Vue-i18n v10 to v11 Migration (patterns)
- [ARCH-008] Frontend Architecture
- [GL-012] Frontend Quality Standards</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\tasks\task-004-starter-storefront\brief.md