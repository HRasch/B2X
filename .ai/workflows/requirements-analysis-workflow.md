# Requirements Analysis Workflow

## Overview
This document outlines the collaborative workflow between agents for requirements analysis in B2Connect projects, following the established Requirements Analysis Methodology.

## Workflow Phases

### Phase 1: Requirements Capture (Led by @ProductOwner)
**Duration:** 1-2 days
**Participants:** @ProductOwner, Business Stakeholders, Domain Experts

**Activities:**
1. **Initial Stakeholder Interviews** - @ProductOwner conducts interviews
2. **Domain Storytelling Sessions** - Capture business processes
3. **User Story Map Creation** - Define user journeys and prioritize features
4. **BRD Draft Creation** - Document business requirements

**Deliverables:**
- Domain Storytelling documents in `.ai/requirements/`
- Initial User Story Map in `.ai/requirements/`
- BRD Draft in `.ai/requirements/`

**Next:** @ProductOwner schedules EventStorming session

### Phase 2: Domain Modeling (Collaborative)
**Duration:** 2-3 days
**Participants:** @ProductOwner, @Architect, @Backend, @Frontend, Domain Experts

**Activities:**
1. **EventStorming Big Picture** - @ProductOwner facilitates with all participants
2. **Process Level Deep Dive** - Identify commands, events, aggregates
3. **Design Level Technical Modeling** - Define read models and policies
4. **Aggregate Boundary Definition** - @Architect leads technical validation

**Deliverables:**
- EventStorming session documentation in `.ai/requirements/`
- Identified aggregates and domain events
- Initial technical constraints identified

**Next:** @Architect begins TRS creation

### Phase 3: Technical Specification (Led by @Architect)
**Duration:** 3-5 days
**Participants:** @Architect, @Backend, @Frontend, @Security, @DevOps

**Activities:**
1. **TRS Creation** - @Architect creates technical requirements specification
2. **API Design** - Define REST endpoints and message contracts
3. **Data Architecture** - Design database schema and relationships
4. **Integration Points** - Identify external system interactions
5. **Technical Feasibility Review** - @Backend and @Frontend validate implementation approach

**Deliverables:**
- Complete TRS in `.ai/decisions/`
- API specifications and data models
- Technical risk assessment

**Next:** @ProductOwner coordinates requirements validation

### Phase 4: Requirements Validation (Collaborative Review)
**Duration:** 1-2 days
**Participants:** All agents, Business Stakeholders, QA

**Activities:**
1. **Cross-Document Review** - Validate BRD ↔ TRS alignment
2. **Technical Review** - @Architect, @Backend, @Frontend review technical specs
3. **Business Review** - @ProductOwner validates business requirements
4. **Quality Assurance** - @QA reviews testability and acceptance criteria
5. **Security & Compliance Review** - @Security, @Legal review constraints

**Deliverables:**
- Validation report in `.ai/requirements/`
- Approved BRD and TRS
- Action items for any required changes

**Next:** @ProductOwner creates detailed user stories

### Phase 5: User Story Refinement (Led by @ProductOwner)
**Duration:** 2-3 days
**Participants:** @ProductOwner, @Architect, @QA, Development Team

**Activities:**
1. **Story Writing** - @ProductOwner creates detailed user stories
2. **Acceptance Criteria Definition** - Collaborates with @QA for testability
3. **Technical Refinement** - @Architect provides technical context
4. **Story Point Estimation** - Team estimates complexity
5. **Definition of Ready Check** - Ensure all stories meet DoR

**Deliverables:**
- Refined user stories in `.ai/requirements/`
- Updated User Story Map with estimates
- Sprint-ready backlog

## Agent Responsibilities in Workflow

### @ProductOwner Responsibilities
- **Lead Requirements Capture** - Stakeholder interviews, domain storytelling
- **Facilitate EventStorming** - Big Picture and Process Level sessions
- **Create BRDs** - Business requirements documentation
- **Maintain User Story Maps** - Product backlog management
- **Coordinate Validation Sessions** - Cross-team reviews
- **Write User Stories** - Detailed story creation with acceptance criteria

### @Architect Responsibilities
- **Lead Technical Specification** - TRS creation and maintenance
- **Participate in EventStorming** - Design Level technical modeling
- **Define System Boundaries** - Service architecture and integration points
- **Technical Feasibility Analysis** - Validate implementation approaches
- **API and Data Design** - REST contracts and database schemas
- **Technical Risk Assessment** - Identify and mitigate technical risks

### @Backend Responsibilities
- **Participate in EventStorming** - Domain modeling and aggregate design
- **Validate Technical Feasibility** - Implementation approach review
- **API Implementation Planning** - Wolverine handlers and message contracts
- **Database Design Input** - EF Core entities and relationships
- **Integration Requirements** - External system interactions

### @Frontend Responsibilities
- **Participate in EventStorming** - Read model and UI requirements
- **Validate Technical Feasibility** - Vue.js implementation approach
- **UI/UX Requirements** - Component and interaction design
- **State Management Design** - Pinia store architecture
- **Performance Requirements** - Frontend performance constraints

### @QA Responsibilities
- **Review Testability** - Ensure requirements are testable
- **Acceptance Criteria Validation** - Collaborate on story acceptance
- **Testing Strategy Input** - Unit, integration, E2E approaches
- **Quality Gates** - Requirements validation participation
- **Defect Prevention** - Early identification of testing gaps

### @Security Responsibilities
- **Security Requirements Review** - Authentication, authorization needs
- **Compliance Validation** - GDPR, NIS2, AI Act requirements
- **Threat Modeling Input** - Security considerations in design
- **Data Protection Requirements** - Encryption and privacy needs

## Communication Patterns

### Daily Standups
- **Requirements Progress** - @ProductOwner reports on capture/validation status
- **Technical Clarifications** - @Architect addresses technical questions
- **Blocker Resolution** - Cross-agent collaboration for issues

### Weekly Reviews
- **Requirements Status** - Review completion of each phase
- **Quality Metrics** - Track requirements quality scores
- **Risk Assessment** - Identify and mitigate project risks

### Milestone Reviews
- **Phase Completion** - Formal review and approval of deliverables
- **Stakeholder Demos** - Present progress to business stakeholders
- **Go/No-Go Decisions** - Assess readiness to proceed

## Quality Gates

### Phase Exit Criteria
- **Phase 1 Exit:** BRD draft approved by business stakeholders
- **Phase 2 Exit:** EventStorming completed with identified aggregates
- **Phase 3 Exit:** TRS approved by technical team
- **Phase 4 Exit:** Validation report with <5 major issues
- **Phase 5 Exit:** Sprint-ready backlog with estimated stories

### Quality Metrics
- **Requirements Completeness:** >95% coverage of business needs
- **Technical Feasibility:** All requirements validated as implementable
- **Testability Score:** >90% of requirements have testable acceptance criteria
- **Business Alignment:** >95% stakeholder satisfaction with requirements

## Escalation Paths

### Issue Resolution
- **Minor Issues:** Resolved within agent team
- **Major Issues:** Escalate to @TechLead for technical guidance
- **Business Conflicts:** Escalate to @ProductOwner for business decision
- **Architecture Disputes:** Escalate to @Architect for technical decision
- **Blockers:** Escalate to @SARAH for cross-agent coordination

### Timeline Risks
- **Delays in Stakeholder Availability:** @ProductOwner coordinates rescheduling
- **Technical Complexity Discovery:** @Architect leads impact analysis
- **Scope Creep:** @ProductOwner manages scope boundaries
- **Resource Constraints:** @ScrumMaster manages team capacity

## Tool Integration

### Document Storage
- **Requirements:** `.ai/requirements/` - BRDs, User Stories, EventStorming docs
- **Decisions:** `.ai/decisions/` - TRSs, ADRs, technical specifications
- **Templates:** `.ai/templates/` - Standardized document templates

### Collaboration Tools
- **Session Facilitation:** Miro/Mural for EventStorming sessions
- **Document Review:** GitHub PRs for requirements validation
- **Communication:** Slack/Teams for daily coordination

### Automation
- **Template Usage:** Automated template population from previous phases
- **Quality Checks:** Automated validation of document completeness
- **Progress Tracking:** Automated status updates in project dashboards

## Success Metrics

### Process Metrics
- **Time to Requirements Complete:** <2 weeks for standard features
- **Requirements Defect Rate:** <5% requirements changes post-implementation
- **Stakeholder Satisfaction:** >4.5/5.0 rating for requirements process
- **Team Alignment:** >90% agreement on requirements interpretation

### Quality Metrics
- **Requirements Traceability:** 100% traceability from BRD to code
- **Test Coverage:** >95% acceptance criteria covered by tests
- **Implementation Match:** >95% requirements implemented as specified
- **Change Request Rate:** <10% change requests during development

---

*This workflow ensures collaborative, high-quality requirements analysis following B2Connect methodology.*