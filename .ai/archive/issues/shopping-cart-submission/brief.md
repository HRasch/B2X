---
docid: ISSUE-001
title: ShoppingCart ERP Submission Hybrid Approach
owner: @SARAH
status: Active
created: 2026-01-10
---

# ShoppingCart ERP Submission Feature

## Brief
Implement hybrid approach for ShoppingCart submission to ERP systems with state control, retry functionality, and admin status overview.

## Requirements
- Asynchronous submission of ShoppingCarts to ERP for order conversion
- State management: Draft, Pending, Submitted, Failed, Retried, Cancelled
- Automatic and manual retry mechanisms
- Admin Frontend dashboard for status overview and actions
- Error handling, security, and audit logging

## Technical Scope
- Backend: Wolverine CQRS commands/events, PostgreSQL persistence
- Frontend: Vue.js 3 dashboard with real-time updates
- Integration: Extend ERP connectors (IdsConnectAdapter)
- Architecture: New ErpIntegration module under Store domain

## Acceptance Criteria
- [ ] ADR created for ErpSubmission bounded context
- [ ] Prototype command/handler implemented
- [ ] State management with persistence
- [ ] Retry logic (auto + manual)
- [ ] Admin UI for status overview
- [ ] Integration tests with mocked ERP
- [ ] Security audit passed

## Stakeholders
- @ProductOwner: Requirements validation
- @Architect: ADR and design
- @Backend: Implementation
- @Frontend: UI development
- @QA: Testing
- @Security: Audit

## Timeline
- Phase 1 (Design): 1-2 days
- Phase 2 (Implementation): 3-5 days
- Phase 3 (Integration): 2-3 days
- Phase 4 (Testing/Release): 1-2 days

## Risks
- ERP connector compatibility
- High-volume performance
- Data consistency during failures

## Next Steps
- @Architect: Create ADR
- @Backend: Prototype skeleton