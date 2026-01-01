# Requirements Analysis Training Guide

## Overview
This training guide helps the B2Connect team adopt the new Requirements Analysis Methodology. It provides practical guidance for each role in the requirements process.

## Training Objectives
By the end of this training, participants will be able to:
- Understand the 4-phase requirements analysis process
- Use appropriate templates for their role
- Facilitate EventStorming and Domain Storytelling sessions
- Create high-quality BRDs, TRSs, and User Stories
- Collaborate effectively across agent boundaries

## Role-Specific Training

### @ProductOwner Training

#### Core Responsibilities
- Lead requirements capture and business analysis
- Facilitate collaborative modeling sessions
- Create and maintain business requirements documentation
- Ensure business value alignment throughout development

#### Key Skills to Develop

**1. Stakeholder Interview Techniques**
```
Preparation:
- Research stakeholder background and business context
- Prepare open-ended questions focused on business outcomes
- Schedule 60-90 minute sessions with clear agendas

During Interview:
- Start with business context questions
- Use "5 Whys" technique for root cause analysis
- Capture pain points and desired outcomes
- Validate understanding through paraphrasing

Follow-up:
- Send summary notes within 24 hours
- Clarify any ambiguous points
- Share insights with technical team
```

**2. Domain Storytelling Facilitation**
```
Session Structure:
1. Introduction (10 min) - Explain purpose and process
2. Business Context (15 min) - Domain expert presents overview
3. Story Narration (30 min) - Walk through the process step-by-step
4. Clarification (15 min) - Answer questions and fill gaps
5. Validation (10 min) - Confirm accuracy with domain expert

Best Practices:
- Focus on business facts, not technical solutions
- Use simple language and concrete examples
- Document assumptions and open questions
- Keep sessions to 60-90 minutes maximum
```

**3. EventStorming Big Picture Facilitation**
```
Preparation:
- Identify 3-5 key domain experts
- Prepare timeline of major business events
- Set up physical/digital collaboration space
- Communicate session goals and rules

Facilitation Rules:
- One conversation at a time
- Challenge assumptions respectfully
- Focus on business events and processes
- Use sticky notes for all content
- Time-box each activity

Common Pitfalls to Avoid:
- Technical discussions too early
- Dominant voices monopolizing conversation
- Scope creep beyond session boundaries
- Lack of clear outcomes
```

#### Tools and Templates
- **BRD Template:** Use for business requirements documentation
- **User Story Map Template:** For product backlog planning
- **Domain Storytelling Template:** For business process capture
- **EventStorming Session Template:** For workshop planning

#### Success Metrics
- Stakeholder satisfaction >4.5/5.0
- Requirements completeness >95%
- Business alignment maintained throughout project
- Clear business value quantification

### @Architect Training

#### Core Responsibilities
- Create technical requirements specifications
- Define system boundaries and architecture
- Validate technical feasibility
- Lead technical validation of requirements

#### Key Skills to Develop

**1. TRS Creation from BRD**
```
Analysis Process:
1. Map BRD features to technical components
2. Identify architectural patterns and technologies
3. Define API contracts and data structures
4. Assess technical risks and constraints
5. Create implementation roadmap

Technical Validation Checklist:
- Performance requirements achievable?
- Scalability needs addressed?
- Security requirements implementable?
- Integration points defined?
- Technology choices appropriate?
```

**2. EventStorming Design Level Participation**
```
Preparation:
- Review Big Picture and Process Level outputs
- Prepare technical context and constraints
- Identify potential architectural challenges

During Session:
- Focus on technical implications of business processes
- Identify aggregate boundaries and domain events
- Define read models and query requirements
- Note external system interactions

Follow-up:
- Document technical decisions and assumptions
- Create initial architecture sketches
- Identify technical spikes needed
```

**3. Technical Risk Assessment**
```
Risk Categories:
- Performance bottlenecks
- Scalability limitations
- Security vulnerabilities
- Integration complexity
- Technology maturity

Assessment Framework:
- Probability (Low/Medium/High)
- Impact (Low/Medium/High)
- Mitigation strategies
- Contingency plans
```

#### Tools and Templates
- **TRS Template:** For technical specifications
- **ADR Template:** For architecture decisions
- **EventStorming Template:** For technical modeling sessions

#### Success Metrics
- Technical feasibility confirmed for all requirements
- Architecture decisions documented and approved
- Technical risks identified and mitigated
- Implementation estimates accurate within 20%

### @Backend Training

#### Core Responsibilities
- Validate technical implementation feasibility
- Provide domain modeling input
- Define API and data contracts
- Ensure Wolverine architecture alignment

#### Key Skills to Develop

**1. Domain Event Analysis**
```
Event Identification:
- Business events vs. system events
- Event granularity and naming conventions
- Event data structure requirements
- Event ordering and consistency needs

Aggregate Design:
- Identify aggregate boundaries
- Define aggregate responsibilities
- Design aggregate state transitions
- Plan for concurrency and consistency
```

**2. Wolverine Message Contract Design**
```
Command Design:
- Clear, imperative naming (CreateOrder, not OrderCreation)
- Include all required data
- Validate command preconditions
- Design for idempotency where possible

Event Design:
- Past tense naming (OrderCreated, not CreateOrder)
- Include relevant context data
- Design for multiple subscribers
- Consider event versioning

Handler Implementation:
- Single responsibility principle
- Error handling and compensation
- Logging and monitoring
- Testing strategies
```

#### Tools and Templates
- **Wolverine Implementation Patterns** (from knowledgebase)
- **API Design Guidelines** (from knowledgebase)
- **Domain Modeling Checklist**

### @Frontend Training

#### Core Responsibilities
- Define UI/UX requirements from business needs
- Validate frontend technical feasibility
- Design component and interaction patterns
- Ensure accessibility and performance requirements

#### Key Skills to Develop

**1. Read Model Design**
```
From EventStorming:
- Identify required UI views
- Define data requirements for each view
- Design optimal query patterns
- Consider real-time update needs

UI Requirements:
- Screen layouts and navigation flows
- Component interaction patterns
- State management needs
- Performance requirements
```

**2. Vue.js Architecture Planning**
```
Component Design:
- Identify reusable components
- Design component communication patterns
- Plan for component composition
- Consider accessibility requirements

State Management:
- Design Pinia store structure
- Define state update patterns
- Plan for state persistence
- Design error handling
```

#### Tools and Templates
- **Vue.js Component Patterns** (from knowledgebase)
- **UI/UX Guidelines** (from knowledgebase)
- **Accessibility Checklist**

### @QA Training

#### Core Responsibilities
- Ensure requirements testability
- Define acceptance criteria
- Plan testing strategies
- Validate quality throughout process

#### Key Skills to Develop

**1. Acceptance Criteria Writing**
```
GIVEN/WHEN/THEN Format:
- GIVEN clear, testable preconditions
- WHEN specific user actions or events
- THEN measurable, observable outcomes

Best Practices:
- Make criteria independent and atomic
- Include both positive and negative scenarios
- Define performance and usability expectations
- Include edge cases and error conditions
```

**2. Test Strategy Planning**
```
Test Level Planning:
- Unit tests for business logic
- Integration tests for API contracts
- E2E tests for critical user journeys
- Performance tests for SLAs

Test Data Strategy:
- Define test data requirements
- Plan for data setup and teardown
- Design test data management
- Consider production data sensitivity
```

#### Tools and Templates
- **Testing Strategy Guide** (from knowledgebase)
- **Test Case Template**
- **Quality Checklist**

## Training Sessions

### Session 1: Methodology Overview (2 hours)
- Requirements Analysis Methodology presentation
- Role responsibilities and collaboration patterns
- Template introduction and walkthrough
- Q&A and discussion

### Session 2: Hands-on Template Usage (3 hours)
- Template walkthrough with real examples
- Group exercises creating sample documents
- Peer review and feedback practice
- Best practices discussion

### Session 3: EventStorming Workshop (4 hours)
- EventStorming theory and principles
- Hands-on practice session
- Facilitation techniques
- Debrief and lessons learned

### Session 4: Domain Storytelling Practice (2 hours)
- Storytelling techniques
- Practice sessions with real scenarios
- Feedback and improvement
- Integration with EventStorming

### Session 5: Validation and Review Process (2 hours)
- Requirements validation techniques
- Cross-team review processes
- Quality metrics and measurement
- Continuous improvement practices

## Assessment and Certification

### Knowledge Assessment
- Multiple choice quiz on methodology principles
- Template usage evaluation
- Role-specific scenario exercises

### Practical Assessment
- Create complete requirements package for sample feature
- Facilitate mini EventStorming session
- Participate in requirements validation

### Certification Criteria
- 80%+ on knowledge assessment
- Satisfactory completion of practical exercises
- Positive peer feedback on collaboration
- Demonstration of template proficiency

## Resources and Support

### Documentation
- **Requirements Analysis Methodology:** Complete process guide
- **Template Library:** All document templates with examples
- **Knowledge Base:** Best practices and patterns
- **Workflow Guide:** Step-by-step collaboration guide

### Support Channels
- **Methodology Questions:** @ProductOwner or @Architect
- **Template Help:** Check examples in `.ai/templates/`
- **Technical Issues:** @TechLead or relevant specialist
- **Process Improvements:** @SARAH for governance questions

### Continuous Learning
- **Monthly Review Sessions:** Share lessons learned
- **Template Updates:** Regular improvements based on feedback
- **Case Study Reviews:** Analysis of successful implementations
- **External Training:** Industry best practices and updates

## Success Metrics

### Adoption Metrics
- **Training Completion:** >90% of team certified
- **Template Usage:** >95% of requirements use standard templates
- **Process Compliance:** >90% adherence to methodology phases

### Quality Metrics
- **Requirements Quality Score:** >85% average across projects
- **Defect Reduction:** 20% reduction in requirements-related defects
- **Time to Delivery:** 15% improvement in requirements phase duration

### Satisfaction Metrics
- **Team Satisfaction:** >4.0/5.0 rating for new process
- **Stakeholder Satisfaction:** >4.5/5.0 rating for requirements quality
- **Process Efficiency:** >80% of team finds process valuable

---

*This training guide ensures successful adoption of the B2Connect Requirements Analysis Methodology.*