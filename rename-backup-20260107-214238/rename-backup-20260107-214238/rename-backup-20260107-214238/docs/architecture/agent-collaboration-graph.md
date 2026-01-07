# Agent Collaboration Graph

This diagram illustrates the collaboration structure among the B2X project agents, coordinated by @SARAH.

```mermaid
flowchart TD
    SARAH[SARAH<br/>Coordinator] --> Backend[Backend<br/>.NET/Wolverine]
    SARAH --> Frontend[Frontend<br/>Vue.js 3]
    SARAH --> QA[QA<br/>Testing]
    SARAH --> Architect[Architect<br/>System Design]
    SARAH --> TechLead[TechLead<br/>Code Quality]
    SARAH --> Security[Security<br/>Security]
    SARAH --> DevOps[DevOps<br/>Infrastructure]
    SARAH --> ScrumMaster[ScrumMaster<br/>Process]
    SARAH --> ProductOwner[ProductOwner<br/>Requirements]
    SARAH --> Legal[Legal<br/>Compliance]
    SARAH --> UX[UX<br/>User Research]
    SARAH --> UI[UI<br/>Components]
    SARAH --> SEO[SEO<br/>Search Optimization]
    SARAH --> GitManager[GitManager<br/>Git Workflow]
    SARAH --> DocMaintainer[DocMaintainer<br/>Documentation]
    SARAH --> Enventa[Enventa<br/>enventa Trade ERP]
    SARAH --> CopilotExpert[CopilotExpert<br/>Copilot Config]

    Backend --> TechLead
    Frontend --> TechLead
    QA --> TechLead
    Security --> TechLead
    DevOps --> TechLead
    Architect --> TechLead

    TechLead --> SARAH
    Security --> SARAH
    QA --> SARAH
    DevOps --> SARAH
    Architect --> SARAH
    ProductOwner --> SARAH
    ScrumMaster --> SARAH

    Enventa --> Backend
    CopilotExpert --> SARAH
    DocMaintainer --> All[All Agents]
```

**Legend:**
- **SARAH** (Coordinator): Handles delegation, quality gates, and conflict resolution
- **Domain Agents**: Execute specific tasks in their areas
- **Collaboration Lines**: Show primary interaction flows
- **Quality Gates**: Feedback loops for approval and sign-off

This graph represents the hierarchical coordination with @SARAH at the center, ensuring compliance with [GL-008] Governance Policies and [GL-002] Subagent Delegation.