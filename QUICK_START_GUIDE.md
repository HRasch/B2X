# 🚀 B2X Quick Start Guide

**Last Updated**: 30. Dezember 2025  
**Status**: ✅ Legacy Cleanup Complete  
**Archive**: See [docs/archive/](./docs/archive/) for historical sprint/session notes

---

## 📋 Choose Your Path

### 👨‍💼 **Project Leadership**
- **Tech Lead**: Read [KB-013]
- **Product Owner**: See [DOC-006]
- **Scrum Master**: Check [`.github/agents/scrum-master.agent.md`](./.github/agents/scrum-master.agent.md)

### 👨‍💻 **Active Development**
- **Build & Run**: See [README.md](./README.md) Quick Start section
- **Test Commands**: `dotnet test B2X.slnx` (backend) | `npm run test` (frontend)
- **Architecture Docs**: [docs/architecture/](./docs/architecture/)

### 🎯 **Role-Based Setup** (in `.github/docs/roles/`)
- Backend Developer: [`.github/docs/roles/backend-developer.md`](./.github/docs/roles/backend-developer.md)
- Frontend Developer: [`.github/docs/roles/frontend-developer.md`](./.github/docs/roles/frontend-developer.md)
- QA Engineer: [`.github/docs/roles/qa-engineer.md`](./.github/docs/roles/qa-engineer.md)
- DevOps Engineer: [`.github/docs/roles/devops-engineer.md`](./.github/docs/roles/devops-engineer.md)
- Security Engineer: [`.github/docs/roles/security-engineer.md`](./.github/docs/roles/security-engineer.md)

### 📚 **Complete Documentation**
- **Main Instructions**: [`.github/copilot-instructions.md`](./.github/copilot-instructions.md)
- **AI Knowledge Base**: [`.ai/knowledgebase/INDEX.md`](./.ai/knowledgebase/INDEX.md)
- **Architecture**: [`docs/architecture/`](./docs/architecture/)
- **Compliance**: [`docs/compliance/`](./docs/compliance/)
- **User Guides**: [`docs/user-guides/`](./docs/user-guides/)
- **Historical Archive**: [`docs/archive/`](./docs/archive/) (legacy sprint/session notes)

---

## 🏗️ Directory Structure

```
B2X/
├── .github/              ← Process & copilot instructions
├── docs/                 ← Product documentation
├── backend/              ← .NET 10 microservices
├── frontend/             ← Vue.js applications
├── scripts/              ← Automation scripts
└── ROOT_LEVEL_DOCS/      ← Executive summaries
```

---

## ⚡ Quick Commands

### **Backend**
```bash
dotnet build B2X.slnx
dotnet test B2X.slnx -v minimal
cd AppHost && dotnet run  # Dashboard: http://localhost:15500
```

### **Frontend**
```bash
cd frontend/Store && npm run dev         # Port 5173
cd frontend/Admin && npm run dev         # Port 5174
```

---

## 📊 Current Status

| Phase | Status | Details |
|-------|--------|---------|
| Sprint 3 Phase 1 | ✅ Complete | Checkout.vue (1,518 lines) |
| Sprint 3 Phase 2 | ✅ Complete | 31/31 tests, 80.7% coverage |
| Sprint 3 Phase 3 | 🔄 In Progress | Documentation writing |
| Launch | ⏳ 4 Jan 2026 | Secure timeline |

---

## 🎯 Next Actions

**For Next Development Session**:
1. Read: [`SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md`](./SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md) ⭐
2. Execute: E2E tests + accessibility audit
3. Complete: Phase 3 documentation

---

## 📖 Full Documentation Index

| Purpose | Location |
|---------|----------|
| Main Reference | [`.github/copilot-instructions.md`](./.github/copilot-instructions.md) |
| All Agents | [`.github/AGENTS_INDEX.md`](./.github/AGENTS_INDEX.md) |
| Role Guides | [`.github/docs/roles/`](./.github/docs/roles/) |
| Architecture | [`docs/architecture/`](./docs/architecture/) |
| Compliance | [`docs/compliance/`](./docs/compliance/) |
| User Guides | [`docs/user-guides/`](./docs/user-guides/) (EN/DE) |
| Historical | [`docs/archive/`](./docs/archive/) |

---

🚀 **Ready to build!**
