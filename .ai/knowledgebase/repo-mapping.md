---
docid: KB-151
title: Repo Mapping
owner: @DocMaintainer
status: Active
created: 2026-01-08
updated: 2026-01-09
---

Repository mapping — where to look
---------------------------------
Quick map of this workspace to the knowledgebase topics.

- Backend message bus & CQRS: `backend/` and `AppHost/` — see `wolverine.md` and `patterns-antipatterns.md`.
- API gateways: `backend/BoundedContexts/*/API/` — follow Identity and authentication guidance in `dotnet-identity.md`.
- Domain projects and EF/DB work: `backend/Domain/` — consult `dotnet-breaking-changes.md` before library API changes.
- Frontend Store and Management UI: `frontend/Store`, `frontend/Admin`, `frontend/Management` — see `vue.md`, `pinia.md`, `vite.md`, and `patterns-antipatterns.md`.
- Localization resources and verification scripts: `backend/Domain/Localization/`, `verify-localization.sh` — see `dotnet-localization.md`.
- CI / DevOps scripts: `scripts/`, `docker-compose.yml`, `backend/docker-compose.aspire.yml` — refer to `owasp-top-ten.md` and `dotnet-breaking-changes.md` for security and compatibility checks.
- MCP AI Tools: `backend/BoundedContexts/Admin/MCP/B2X.Admin.MCP/Tools/` — organized as individual tool files (see `patterns-antipatterns.md` for file organization guidelines). Each tool follows AI gateway patterns with tenant context and GDPR compliance.
- Code Organization: All projects follow systematic refactoring patterns — see `patterns-antipatterns.md` for file size limits and extraction strategies.
- **Solution Structure**: B2X.slnx defines project organization with `/src/` prefixed paths; test projects in `/tests/` require 5-level up navigation (`../../../../../src/`) for references
- **Package Dependencies**: EF Core projects need complete package sets (runtime + relational + provider); ASP.NET projects require explicit `Microsoft.Extensions.*` references for DI services

When adding or modifying components, update this file with direct file references to help future agents find the right places to change.
