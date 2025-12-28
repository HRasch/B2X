# 👥 Role-Based Documentation Index

**Navigation:** Wählen Sie Ihre Rolle für personalisierte Onboarding-Dokumentation.

---

## Team Roles

| Role | Primary Focus | P0 Components | Est. Reading |
|------|---------------|---------------|--------------|
| [🔐 Security Engineer](SECURITY_ENGINEER.md) | Encryption, Audit, Incident Response | P0.1, P0.2, P0.3, P0.5, P0.7 | 4h |
| [⚙️ DevOps Engineer](DEVOPS_ENGINEER.md) | Infrastructure, Network, Aspire | P0.3, P0.4, P0.5 | 3h |
| [💻 Backend Developer](BACKEND_DEVELOPER.md) | Wolverine, CQRS, Compliance APIs | P0.1, P0.6, P0.7, P0.9 | 5h |
| [🎨 Frontend Developer](FRONTEND_DEVELOPER.md) | Vue.js, Accessibility, UX | P0.6, P0.8 | 3h |
| [🧪 QA Engineer](QA_ENGINEER.md) | Testing (52 Compliance Tests) | ALL (Test Execution) | 4h |
| [📋 Product Owner](PRODUCT_OWNER.md) | Prioritization, Go/No-Go Gates | Executive Oversight | 2h |
| [⚖️ Legal/Compliance](LEGAL_COMPLIANCE.md) | Regulations, Legal Review | P0.6, P0.7, P0.8, P0.9 | 3h |
| [👔 Tech Lead/Architect](TECH_LEAD.md) | Architecture, Code Review | ALL (Oversight) | 6h |

---

## Quick Start by Role

### New to the Project?

1. **Identify your role** from the table above
2. **Click your role link** for personalized documentation
3. **Follow the 3-week onboarding path** in your role document
4. **Complete the P0 components** assigned to your role

### Cross-Role Collaboration

| From Role | To Role | Typical Handoff |
|-----------|---------|-----------------|
| Backend Dev | QA Engineer | Feature ready for testing |
| Frontend Dev | QA Engineer | UI ready for accessibility testing |
| Security Eng | DevOps | Encryption keys for infrastructure |
| Legal | All Roles | Compliance requirements |
| Product Owner | All Roles | Feature prioritization |
| Tech Lead | All Roles | Architecture decisions |

---

## P0 Component Ownership Matrix

| Component | Primary Owner | Support | QA | Legal Review |
|-----------|---------------|---------|-----|--------------|
| **P0.1 Audit Logging** | Security Eng | Backend Dev | ✅ | - |
| **P0.2 Encryption** | Security Eng | Backend Dev | ✅ | - |
| **P0.3 Incident Response** | Security Eng | DevOps | ✅ | ✅ |
| **P0.4 Network** | DevOps | Security Eng | ✅ | - |
| **P0.5 Key Management** | DevOps | Security Eng | ✅ | - |
| **P0.6 E-Commerce** | Backend Dev | Frontend Dev | ✅ | ✅ |
| **P0.7 AI Act** | Backend Dev | Security Eng | ✅ | ✅ |
| **P0.8 BITV** | Frontend Dev | QA Engineer | ✅ | ✅ |
| **P0.9 E-Rechnung** | Backend Dev | - | ✅ | ✅ |

---

## Documentation Structure

```
docs/
├── by-role/                    # 👈 You are here
│   ├── README.md               # This file
│   ├── SECURITY_ENGINEER.md
│   ├── DEVOPS_ENGINEER.md
│   ├── BACKEND_DEVELOPER.md
│   ├── FRONTEND_DEVELOPER.md
│   ├── QA_ENGINEER.md
│   ├── PRODUCT_OWNER.md
│   ├── LEGAL_COMPLIANCE.md
│   └── TECH_LEAD.md
│
├── compliance/                 # Compliance specifications
│   ├── EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md
│   ├── P0.6_ECOMMERCE_LEGAL_TESTS.md
│   ├── P0.7_AI_ACT_TESTS.md
│   ├── P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
│   └── P0.9_ERECHNUNG_TESTS.md
│
├── architecture/               # Architecture docs
├── guides/                     # How-to guides
├── api/                        # API documentation
└── ROLE_BASED_DOCUMENTATION_MAP.md  # Complete mapping
```

---

**Last Updated:** 28. Dezember 2025
