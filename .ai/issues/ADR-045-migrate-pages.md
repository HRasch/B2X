---
title: "Migrate Pages.vue to unified layout components (ADR-045)"
owner: "@CMS"
estimate: 0.5d
labels: [frontend, ADR-045, migration, cms]
---

## Summary
Replace ad-hoc layout in `Pages.vue` with ADR-045 layout primitives: `PageHeader`, `CardContainer`, and appropriate `Form*` components where applicable.

## Background
This is the first incremental migration task for ADR-045 (Unified Admin Layout). The goal is a small, low-risk PR that converts the CMS pages list view to use shared layout components and i18n keys.

## Acceptance Criteria
- Page uses `PageHeader` for title, subtitle and actions.
- Main content (list/grid) is wrapped in `CardContainer` with correct padding and responsive behaviour.
- No hardcoded user-facing strings remain; translations use existing i18n keys or new keys are added under `cms.pages.*` in `en.json` and `de.json`.
- Unit tests for any new display logic are added/updated.
- Linting, type-check and build pass locally (`npm run lint`, `npm run type-check`, `npm run build` for frontend).
- Visual regression snapshot updated and reviewed (if CI uses visual tests).

## Tasks
- [ ] Create/modify `Pages.vue` to use `PageHeader` + `CardContainer`.
- [ ] Replace hardcoded strings with `$t('cms.pages.xxx')` keys; add missing keys to `en.json` and `de.json`.
- [ ] Add unit tests for `Pages.vue` rendering of header and container.
- [ ] Run `npm run lint` and `npm run test` and fix issues.
- [ ] Create PR with single-task scope and link to ADR-045.

## Testing Steps (manual)
1. Start the frontend app (Admin) locally.
2. Navigate to CMS → Pages.
3. Verify header shows correct title, subtitle and action buttons.
4. Verify list/grid is inside a Card with expected spacing.
5. Switch locale to German and verify translations.

## Files touched (expected)
- `frontend/Management/src/views/cms/Pages.vue`
- `frontend/Management/src/components/layout/PageHeader.vue` (already exists)
- `frontend/Management/src/components/layout/CardContainer.vue` (already exists)
- `frontend/Management/src/locales/en.json` (update)
- `frontend/Management/src/locales/de.json` (update)
- `frontend/Management/tests/Pages.spec.ts` (add/update)

## PR Checklist (to include in PR body)
- [ ] PR title follows convention: `feat(cms): migrate Pages.vue to unified layout`
- [ ] Linked to ADR-045
- [ ] Single-task PR (does not migrate other views)
- [ ] Unit tests added/updated and passing
- [ ] Linting and type-checking pass
- [ ] i18n keys added for `en` and `de` (if new keys required)
- [ ] Visual snapshots updated (if applicable)
- [ ] QA: visual smoke check performed

## Notes / Risks
- Keep changes small to avoid visual regressions.
- If Page contains custom actions or complex slots, validate that `PageHeader` supports required slots; if not, extend `PageHeader` in a follow-up small PR.

---

Status: ready for implementation
