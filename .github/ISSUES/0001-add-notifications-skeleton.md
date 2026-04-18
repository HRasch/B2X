---
title: Implement Notifications gateway skeleton
labels: backend, enhancement, infra
assignees: []
---

# Implement Notifications gateway skeleton

Summary
-------
Create a minimal, compile-able Notifications gateway that will serve as the central entrypoint for multi-channel notifications (email, push, SMS, WhatsApp). A lightweight skeleton has been scaffolded; this issue tracks finishing the minimal implementation and next integration steps.

Background
----------
We added a scaffolded project at `src/backend/Notifications/Gateway/B2X.Notifications.csproj` with a minimal `Program.cs` exposing `/health` and `/api/notifications/send`. The project has been added to `B2X.slnx` and builds successfully.

Goal
----
Deliver a small but complete skeleton that other teams can extend with adapters and message-bus integration.

Acceptance criteria
-------------------
- [x] `B2X.Notifications` exists and is added to the solution
- [x] Project builds without errors (warnings allowed)
- [x] Exposes `/health` returning 200
- [x] Exposes `/api/notifications/send` POST accepting a simple JSON payload
- [ ] Add an `Adapters` folder with interfaces for `IEmailAdapter`, `IPushAdapter`, `ISmsAdapter`, `IWhatsAppAdapter` and simple no-op implementations
- [ ] Add a small unit test project `src/backend/Notifications/Tests` with one test verifying the health endpoint
- [ ] Document next steps in `src/backend/Notifications/README.md`

Tasks
-----
- [x] Verify project added to solution
- [x] Verify build succeeds
- [x] Commit initial scaffold (Program.cs + csproj + README)
- [ ] Implement adapters folder and interfaces
- [ ] Add unit test project and basic tests
- [ ] Add YARP reverse-proxy rules in the Management gateway to route `/api/notifications` → `http://notifications:port` (configuration PR)

References
----------
- Files added: 
  - [src/backend/Notifications/Gateway/B2X.Notifications.csproj](src/backend/Notifications/Gateway/B2X.Notifications.csproj)
  - [src/backend/Notifications/Gateway/Program.cs](src/backend/Notifications/Gateway/Program.cs)
  - [src/backend/Notifications/README.md](src/backend/Notifications/README.md)
- Related docs: [docs/architecture/PROJECT_DEPENDENCY_GRAPH.md](docs/architecture/PROJECT_DEPENDENCY_GRAPH.md)

Notes
-----
This issue focuses on a lightweight, non-blocking skeleton so features can be iterated on quickly. After adapters are in place, we should add Wolverine message integration for retries and audit.
