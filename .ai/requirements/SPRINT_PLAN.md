# B2Connect — 2‑Sprint MVP Plan

Purpose: pragmatic first two sprints to deliver a shippable MVP (focus on P0). Link to created P0 issues for implementation.

Sprint length: 2 weeks each. Goal: deliver a working demo storefront + core platform workflows by end of Sprint 2.

---

## Sprint 1 (Weeks 1–2)
- Focus: Core API foundation, authentication, developer DX and minimal storefront wiring.

- Tasks:
  - `Core API` — basic product & SKU models, CRUD endpoints ([issue #76](https://github.com/HRasch/B2Connect/issues/76))
  - `Auth & RBAC` — JWT/API key support, admin role model ([issue #77](https://github.com/HRasch/B2Connect/issues/77))
  - `Developer DX` — OpenAPI + GraphQL Playground + quickstart docs ([issue #83](https://github.com/HRasch/B2Connect/issues/83))
  - `Starter Storefront wiring` — connect Next.js demo to staging API (skeleton) ([issue #82](https://github.com/HRasch/B2Connect/issues/82))

- Acceptance criteria:
  - CRUD operations functional end-to-end locally.
  - Auth protects API endpoints; a simple admin user can perform CRUD.
  - Developer quickstart runs locally with documented steps.

## Sprint 2 (Weeks 3–4)
- Focus: Checkout & Orders, import/export and eventing; make demo checkout complete.

- Tasks:
  - `Checkout & Payments` — cart → checkout → order flow; integrate sandbox adapter ([issue #78](https://github.com/HRasch/B2Connect/issues/78))
  - `Order Management & Fulfillment` — status transitions and webhooks ([issue #79](https://github.com/HRasch/B2Connect/issues/79))
  - `Product Import/Export` — CSV import (validation) ([issue #80](https://github.com/HRasch/B2Connect/issues/80))
  - `Webhooks & Eventing` — event subscriptions and delivery retries ([issue #81](https://github.com/HRasch/B2Connect/issues/81))

- Acceptance criteria:
  - Demo storefront can complete checkout and create an order in the API.
  - Orders show in admin and webhook events are emitted for order created.
  - CSV import successfully creates products in staging.

---

## Planning notes
- Split each task into subtasks (API, DB migrations, tests, docs).
- Add owners and assign issues to engineers; create sprint milestone `MVP-Sprint-1-2` (I can create this if you want).

## Next steps
1. Confirm sprint length and available team capacity.  
2. Assign owners to P0 issues and create milestone `MVP-Sprint-1-2`.  
3. Convert remaining P1 items into issues (I am creating these now).
