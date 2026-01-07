# B2X — MVP Backlog

This file lists a prioritized, minimal set of features to ship an initial, market-viable product (MVP) for B2X. Each item includes a short description, priority, rough effort estimate and suggested owner.

Notes on estimates: effort shown as **S/M/L** (Small ≈ 1-2 dev-weeks, Medium ≈ 3-6 dev-weeks, Large ≈ 2+ months) assuming a small cross-functional team.

---

## P0 — Core Platform (MVP)

- **Core API (REST + GraphQL read)**: CRUD for Products, SKUs, Categories, Inventory, Prices, Orders
  - Priority: P0
  - Effort: L
  - Owner: Backend
  - Acceptance: CRUD endpoints + GraphQL read schema; basic auth; integration tests

- **Authentication & Authorization (RBAC)**
  - Priority: P0
  - Effort: M
  - Owner: Security / Backend
  - Acceptance: Users, roles, admin UI for roles, token-based auth (JWT/API keys)

- **Checkout + Payments (pluggable)**
  - Priority: P0
  - Effort: M
  - Owner: Backend
  - Acceptance: Cart→Checkout→Order flow; one payment provider adapter (Stripe/payments sandbox)

- **Order Management & Fulfillment basics**
  - Priority: P0
  - Effort: M
  - Owner: Backend
  - Acceptance: Order statuses, simple fulfilment workflow, order webhooks

- **Product Import/Export (CSV first)**
  - Priority: P0
  - Effort: S
  - Owner: Backend / Integrations
  - Acceptance: CSV import of products/variants, validation and sample mapping

- **Webhooks & Eventing (basic)**
  - Priority: P0
  - Effort: S
  - Owner: Backend
  - Acceptance: Event subscription API + delivery retries/logging

- **Starter Headless Storefront + CLI**
  - Priority: P0
  - Effort: M
  - Owner: Frontend
  - Acceptance: Next.js demo storefront connected to API + `b2c-cli dev` to run locally

- **Developer DX: Docs & OpenAPI / GraphQL Playground**
  - Priority: P0
  - Effort: S
  - Owner: TechDocs / Backend
  - Acceptance: Hosted API docs, quickstart for running locally

---

## P1 — High Value, Short to Mid-term

- **Search (faceted)** — integrate Elasticsearch or compatible search service
  - Priority: P1
  - Effort: M
  - Owner: Backend / Search
  - Acceptance: Faceted product search, basic filters, relevance tuning

- **Pluggable Payment & Tax adapters (additional providers)**
  - Priority: P1
  - Effort: S
  - Owner: Backend
  - Acceptance: 2 more payment gateways + basic tax calculation integration

- **Basic PIM/ERP connector (skeleton)** — webhooks + CSV mappings, sample adapter
  - Priority: P1
  - Effort: M
  - Owner: Integrations
  - Acceptance: One example connector + docs

- **Observability: Logging, Metrics, Traces (basic)**
  - Priority: P1
  - Effort: S
  - Owner: DevOps
  - Acceptance: Structured logs, Prometheus metrics endpoint, Jaeger traces for key flows

- **Tenant model & simple tenant isolation**
  - Priority: P1
  - Effort: M
  - Owner: Backend / Security
  - Acceptance: Per-tenant data separation, config and onboarding flow

---

## P2 — Differentiators / Post-MVP

- **GraphQL write operations & persisted queries**
  - Priority: P2
  - Effort: M
  - Owner: Backend

- **Marketplace / Multi‑seller support**
  - Priority: P2
  - Effort: L
  - Owner: Product / Backend

- **POS & Omnichannel flows (basic)**
  - Priority: P2
  - Effort: L
  - Owner: Product / Integrations

- **Advanced personalization & recommendations (ML pipeline)**
  - Priority: P2
  - Effort: L
  - Owner: ML / Data

- **Image/media pipeline (CDN + AVIF/WebP + transforms)**
  - Priority: P2
  - Effort: M
  - Owner: DevOps / Frontend

---

## Cross-cutting non-functional items (include early)

- **Security & Compliance:** PCI scoping guidance, input validation, secrets mgmt
  - Priority: P0
  - Effort: Ongoing
  - Owner: Security

- **SLA / Multi-region readiness:** design decisions for HA and backups
  - Priority: P1
  - Effort: Architectural and Ops work
  - Owner: DevOps / Architect

- **Accessibility & Localization:** i18n basics and accessibility checks in storefront
  - Priority: P1
  - Effort: S
  - Owner: Frontend / UX

---

## Acceptance criteria & delivery cadence

- Recommend 6–10 week MVP sprint (iterative releases every 2 weeks). Focus first two sprints on Core API, Auth, Checkout plumbing and a demo storefront. 
- Convert P0 items into issues and assign owners; include small acceptance tests per item.

---

## Next steps (suggested immediate actions)

1. Convert each P0 line into an issue with owner and estimate (I can create these). 
2. Review estimates with TechLead and ProductOwner, adjust owners.
3. Schedule first sprint with the Core API/Checkout/Storefront tasks.

---

File created by automation on request — propose any edits and I will update.
