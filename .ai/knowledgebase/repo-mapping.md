Repository mapping — where to look
---------------------------------
Quick map of this workspace to the knowledgebase topics.

- Backend message bus & CQRS: `backend/` and `AppHost/` — see `wolverine.md` and `patterns-antipatterns.md`.
- API gateways: `backend/BoundedContexts/*/API/` — follow Identity and authentication guidance in `dotnet-identity.md`.
- Domain projects and EF/DB work: `backend/Domain/` — consult `dotnet-breaking-changes.md` before library API changes.
- Frontend Store and Management UI: `frontend/Store`, `frontend/Admin`, `frontend/Management` — see `vue.md`, `pinia.md`, `vite.md`, and `patterns-antipatterns.md`.
- Localization resources and verification scripts: `backend/Domain/Localization/`, `verify-localization.sh` — see `dotnet-localization.md`.
- CI / DevOps scripts: `scripts/`, `docker-compose.yml`, `backend/docker-compose.aspire.yml` — refer to `owasp-top-ten.md` and `dotnet-breaking-changes.md` for security and compatibility checks.

When adding or modifying components, update this file with direct file references to help future agents find the right places to change.
