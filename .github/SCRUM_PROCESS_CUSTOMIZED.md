# 🎯 B2Connect Customized Scrum Process

**Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Model**: Event-Driven Sprints (~50 story points, no time frames)  
**Facilitator**: @team-assistant (coordination & feedback collection)  
**Authority**: @product-owner (issue management, feedback filtering, merging)

---

## 📋 Sprint Overview

### Sprint Characteristics
- **Duration**: Event-driven (not time-based)
- **Capacity**: ~50 story points per sprint
- **Trigger**: Previous sprint complete → Next sprint starts immediately
- **Process**: Backlog Refinement → Planning → Development → Stakeholder Review → Final Review → Merge

### Key Principle
> **Development is bottleneck-driven, not calendar-driven.** Team works continuously until sprint issues are complete, then immediately starts next sprint.

---

## 🔄 Sprint Workflow

```
START SPRINT N
    ↓
BACKLOG REFINEMENT (Team)
├─ Review unrefined issues
├─ Add acceptance criteria
├─ Estimate story points
└─ @team-assistant collects feedback
    ↓
SPRINT PLANNING (@product-owner)
├─ Select top ~50 story points
├─ Move to "Ready" status
└─ Announce sprint start
    ↓
ARCHITECTURE REVIEW (@software-architect, @tech-lead)
├─ Review issue specifications
├─ Ask questions if needed
└─ Approve to proceed
    ↓
PARALLEL DEVELOPMENT
├─ UI/UX Design (if frontend): @frontend-dev requests from @ui-expert, @ux-expert
├─ Development: Developers code features
├─ QA Testing: QA tests features
├─ Documentation: Docs written
└─ Code Review: Peer review
    ↓
STAKEHOLDER REVIEW (All)
├─ @ux-expert, @ui-expert (frontend)
├─ @ai-specialist (AI features)
├─ @legal-compliance (legal)
├─ @security-engineer (security)
├─ @devops-engineer (infrastructure)
├─ @tech-lead (architecture)
└─ Additional feedback → New issues created
    ↓
FINAL QA REVIEW (@qa-review)
├─ Quality gate check
├─ Last validation
└─ Approval to merge
    ↓
MERGE (@product-owner)
├─ Merge PR to main
├─ Close issue
└─ Log completion
    ↓
REPEAT (if more issues) or START SPRINT N+1
    ↓
SPRINT REPORT (@process-controller)
├─ Metrics & costs
├─ AI token usage
├─ Team velocity
└─ Retrospective
```

---

## 👥 Role Responsibilities

### 🎯 @product-owner

**Overall Authority**: Issue management, feedback filtering, merging PRs

**Responsibilities**:

1. **Sprint Planning**
   - Select ~50 story points from "Refined" backlog
   - Move issues to "Ready" status in GitHub Planner
   - Announce sprint start to team

2. **Feedback Filtering During Development**
   - Collect all stakeholder feedback on GitHub Issues
   - **ACCEPT**: Feedback directly targeting the issue scope
   - **REJECT**: Out-of-scope feedback (comment: "Create new issue for this")
   - Update GitHub issue with accepted feedback
   - If feedback changes requirements, restart development loop

3. **Final Merge**
   - Verify @qa-review approval
   - Verify all acceptance criteria met
   - Merge PR to main branch
   - Close completed issues
   - Update issue status to "Done"

4. **Backlog Refinement Participation**
   - Lead refinement sessions with team
   - Ensure issues have clear acceptance criteria
   - Resolve ambiguous requirements with team

**Tools**:
- GitHub Issues (status management)
- GitHub Planner (sprint view)
- GitHub PRs (merge approval)

---

### 🏗️ @software-architect

**Focus**: System design review, architecture alignment

**Responsibilities**:

1. **Issue Architecture Review** (Before development starts)
   - Review issue description and acceptance criteria
   - Verify alignment with system architecture
   - Check for design issues or edge cases
   - Ask team questions if needed (post on GitHub)
   - Approve architectural approach or request changes

2. **Development Support**
   - Available for architecture questions during development
   - Provide guidance on service boundaries, data flow
   - Review domain model design if applicable

3. **Stakeholder Review Participation**
   - Ensure architectural integrity in final review
   - Identify any scaling, performance, or design concerns

**Process**:
- When issue status = "Ready", @product-owner tags @software-architect
- @software-architect posts architecture review comment on GitHub
- If OK: Development can start
- If issues: Team discusses, resolves, then proceeds

---

### 💻 @tech-lead

**Focus**: Code quality, technical implementation, mentoring

**Responsibilities**:

1. **Issue Technical Review** (Before development starts)
   - Review implementation approach
   - Check for potential technical risks
   - Ask for clarification if needed
   - Approve or request changes

2. **Code Review During Development**
   - Peer review of code changes
   - Ensure code quality standards met
   - Provide mentoring and feedback
   - Approve or request changes in PR

3. **Final Code Review**
   - Last technical validation before merge
   - Ensure all standards met
   - Sign off on code quality

**Process**:
- When issue status = "Ready", @product-owner tags @tech-lead
- @tech-lead posts technical review on GitHub
- During development, reviews PR when created
- At final QA stage, does final review pass/fail

---

### 👨‍💻 @backend-developer, @frontend-developer

**Focus**: Feature implementation

**Responsibilities**:

1. **Backlog Refinement**
   - Help estimate story points
   - Ask clarifying questions about requirements
   - Identify technical constraints

2. **Development** (After architecture & tech review approved)
   - Implement feature according to acceptance criteria
   - Write tests (80%+ coverage)
   - Ensure builds pass
   - Create PR when ready

3. **Frontend-Specific: UI/UX Coordination**
   - Frontend developers request UI/UX drafts from @ui-expert, @ux-expert
   - Use drafts as implementation guide
   - Coordinate with design before coding (no surprises)

4. **Peer Code Review**
   - Review teammates' code
   - Provide feedback
   - Approve or request changes

**Process**:
- When issue status = "In Progress", developers start coding
- Follow parallel development: dev → test → doc → review
- Ask @ui-expert, @ux-expert for drafts (if frontend)
- Create PR when feature complete
- Request review from @tech-lead
- Address feedback
- Move to "Code Review" when PR ready

---

### 🧪 @qa-engineer

**Focus**: Quality assurance, testing

**Responsibilities**:

1. **Backlog Refinement**
   - Help define test scenarios in acceptance criteria
   - Estimate test effort

2. **Development Testing** (Parallel with development)
   - Write test cases from acceptance criteria
   - Execute manual & automated tests
   - Test edge cases, error scenarios
   - Report bugs to developers
   - Verify fixes

3. **Final QA Review**
   - Run test suite on final code
   - Verify all acceptance criteria met
   - Check code coverage (80%+ required)
   - Sign off as "QA approved"

**Process**:
- Start testing as soon as developer completes feature
- Create GitHub issues for bugs found
- When feature ready for final review, do comprehensive test
- Post "QA Approved ✅" comment when ready

---

### 📝 @qa-review (Quality Gate)

**Focus**: Final quality gate before merge

**Responsibilities**:

1. **Final Quality Check**
   - Verify PR meets all requirements
   - Check test coverage (80%+)
   - Verify documentation complete
   - Ensure accessibility (if UI)
   - Verify security standards (if applicable)

2. **Final Sign-Off**
   - Post "✅ APPROVED FOR MERGE" comment
   - Or request changes before merge

**Process**:
- When PR ready for final review, @product-owner tags @qa-review
- @qa-review does final quality check
- Posts approval or requests changes
- @product-owner merges when approved

---

### 🎨 @ui-expert, @ux-expert

**Focus**: User interface & experience design

**Responsibilities**:

1. **Backlog Refinement** (If issue involves UI/UX)
   - Help define UI/UX requirements
   - Suggest design approaches
   - Flag accessibility concerns

2. **Frontend Development** (When requested)
   - Create UI/UX drafts for frontend developers
   - Frontend developers request: "Need UI draft for login form"
   - Provide design mockups, component specs
   - Support frontend developers during implementation

3. **Stakeholder Review**
   - Review final implementation against design
   - Verify UX flows work as intended
   - Accessibility validation (WCAG 2.1 AA)
   - Post feedback on GitHub

**Process**:
- Frontend developer posts on GitHub: "@ui-expert need draft for X component"
- @ui-expert & @ux-expert create mockup/spec
- Frontend developer implements from design
- In stakeholder review, they validate implementation

---

### 🤖 @ai-specialist

**Focus**: AI/ML features, algorithm validation

**Responsibilities**:

1. **Backlog Refinement** (If AI feature)
   - Review AI/ML requirements
   - Estimate complexity
   - Flag algorithm concerns

2. **Development Support**
   - Advise on algorithm selection
   - Review training data approach
   - Support during implementation

3. **Stakeholder Review**
   - Validate AI/ML implementation against spec
   - Check for bias, accuracy, edge cases
   - Post feedback on GitHub

---

### 🔐 @security-engineer, @legal-compliance

**Focus**: Security & legal compliance

**Responsibilities**:

1. **Backlog Refinement** (If security/legal implications)
   - Review security requirements
   - Flag compliance concerns
   - Help estimate effort

2. **Development Support**
   - Available for security/legal questions
   - Provide guidance on implementation

3. **Stakeholder Review**
   - Security: Verify encryption, auth, data protection
   - Legal: Verify GDPR, contract, liability compliance
   - Post feedback on GitHub

---

### ⚙️ @devops-engineer

**Focus**: Infrastructure, deployment, operations

**Responsibilities**:

1. **Backlog Refinement** (If infrastructure implications)
   - Review infrastructure requirements
   - Estimate deployment effort
   - Flag operational concerns

2. **Development Support**
   - Support infrastructure setup
   - Advise on scaling, performance
   - Support deployment testing

3. **Stakeholder Review**
   - Verify deployability
   - Check performance, scaling, monitoring
   - Post feedback on GitHub

---

### 📊 @process-controller

**Focus**: Project metrics, costs, efficiency

**Responsibilities**:

1. **Sprint Metrics Collection** (Continuous)
   - Track issues completed per sprint
   - Monitor story points velocity
   - Track AI token usage
   - Monitor team efficiency

2. **AI Token Reporting** (Per feature)
   - Collect token usage from @team-assistant
   - Log tokens per issue
   - Calculate cost per story point
   - Identify optimization opportunities

3. **Final Sprint Report** (After sprint complete)
   - Aggregate all metrics
   - Calculate velocity (story points / sprint)
   - Calculate costs (tokens, hours)
   - Identify trends, improvements
   - Post report to GitHub (sprint dashboard)

**Process**:
- @team-assistant logs token usage with each feature update
- @process-controller tracks in spreadsheet
- After sprint complete, creates comprehensive report
- Report includes: velocity, costs, efficiency trends, recommendations

---

### 🤝 @team-assistant

**Focus**: Process coordination, feedback collection

**Responsibilities**:

1. **Feedback Collection** (During development)
   - Gather feedback from stakeholders
   - Post questions on GitHub when clarification needed
   - Collect all feedback systematically
   - Update @product-owner when feedback ready

2. **Issue Status Management**
   - Update GitHub issue status as work progresses
   - Ensure team knows current status
   - Create links between related issues
   - Track blockers and dependencies

3. **Token Tracking** (Per feature)
   - Log AI tokens used per issue
   - Track token usage for each agent
   - Report to @process-controller for sprint report

4. **Process Facilitation**
   - Facilitate backlog refinement discussions
   - Facilitate sprint planning
   - Ask clarifying questions
   - Ensure stakeholder participation

5. **Communication**
   - Post sprint status updates on GitHub
   - Ensure everyone knows their responsibilities
   - Escalate blockers to @product-owner
   - Celebrate completions

**Process**:
- Start sprint: Coordinate backlog refinement + planning
- During development: Collect feedback, update status, track tokens
- End of development: Summarize feedback for @product-owner
- End of sprint: Hand off metrics to @process-controller

---

## 📊 Development Workflow Detail

### Phase 1: Issue Preparation

**Status**: Issue in Backlog (unrefined)

**Activities**:
1. **Backlog Refinement** (Team discussion)
   - @product-owner: Describes business value
   - @developer: Estimates effort
   - @qa: Defines test approach
   - @tech-lead: Notes technical concerns
   - Result: Issue has clear acceptance criteria, estimate

2. **Sprint Planning** (@product-owner)
   - Select top ~50 story points
   - Move to "Ready" status
   - Tag @software-architect and @tech-lead
   - Post sprint start announcement

---

### Phase 2: Architecture & Technical Review

**Status**: Issue = "Ready"

**Activities**:
1. **Architecture Review** (@software-architect)
   - Post architecture review on GitHub
   - Ask clarifying questions if needed
   - Approve or request changes
   - If OK: Leave ✅ comment

2. **Technical Review** (@tech-lead)
   - Post technical review on GitHub
   - Identify technical risks
   - Approve or request changes
   - If OK: Leave ✅ comment

**Outcome**: Both reviewers approve → Move to "In Progress"

---

### Phase 3: Parallel Development

**Status**: Issue = "In Progress"

**Activities** (All happen in parallel):

**Development** (@backend-dev or @frontend-dev):
- Implement feature per acceptance criteria
- If frontend: Request UI/UX drafts from @ui-expert, @ux-expert
- Write code, commit to feature branch
- Ensure builds pass
- Create PR when ready

**QA Testing** (@qa-engineer):
- Write test cases from acceptance criteria
- Test as development progresses
- Find bugs, post to GitHub
- Verify fixes
- Run full test suite when dev complete

**Documentation**:
- Write API docs (if backend)
- Write component docs (if frontend)
- Write user guides (if user-facing)
- Ensure examples included

**Code Review**:
- Peer review (team members)
- @tech-lead final review when PR created
- Address feedback

---

### Phase 4: Stakeholder Review

**Status**: Feature complete, all tests passing

**Activities**:

1. **Notify Stakeholders** (@team-assistant)
   - Post on GitHub: "Ready for stakeholder review"
   - Tag relevant stakeholders:
     - @ui-expert, @ux-expert (if frontend)
     - @ai-specialist (if AI feature)
     - @legal-compliance (if legal implications)
     - @security-engineer (if security implications)
     - @devops-engineer (if ops implications)
     - @tech-lead (architecture sign-off)

2. **Stakeholder Review** (All)
   - Each stakeholder posts feedback on GitHub
   - **Short & significant**: Focus on critical issues
   - Out-of-scope feedback → Comment: "Create issue #N for this"

3. **Feedback Processing** (@product-owner)
   - Review all feedback comments
   - **ACCEPT**: In-scope feedback directly targeting issue
   - **REJECT**: Out-of-scope (comment with new issue reference)
   - If feedback changes requirements:
     - Update issue acceptance criteria
     - Assign back to developers
     - Development loop starts again from Phase 3
   - If no in-scope feedback:
     - Post: "Feedback processed, ready for final QA review"

4. **Out-of-Scope Issue Creation** (@team-assistant or @product-owner)
   - Feedback that's out-of-scope becomes new GitHub issue
   - Link to original issue: "Follow-up from issue #N"
   - Add to next sprint's backlog

---

### Phase 5: Final QA Review

**Status**: Stakeholder feedback processed, ready for merge

**Activities**:

1. **Quality Gate Check** (@qa-review)
   - Verify all acceptance criteria met ✅
   - Verify 80%+ code coverage ✅
   - Verify documentation complete ✅
   - Verify accessibility standards met (if UI) ✅
   - Verify security review passed (if applicable) ✅
   - Verify no breaking changes ✅

2. **Final Sign-Off**
   - If all OK: Post "✅ APPROVED FOR MERGE"
   - If not OK: Post required changes

---

### Phase 6: Merge & Close

**Status**: Final QA approved

**Activities**:

1. **Merge PR** (@product-owner)
   - Verify @qa-review has approved
   - Merge PR to main
   - Delete feature branch
   - Close related issues

2. **Mark Complete** (@product-owner)
   - Update issue status to "Done"
   - Post completion comment with:
     - Date completed
     - Link to merged PR
     - Story points completed

---

## 📈 Sprint Completion & Reporting

### When Sprint is Complete

All ~50 story points finished:

1. **@product-owner**: Posts sprint complete announcement
2. **@team-assistant**: Compiles all metrics
3. **@process-controller**: Creates Sprint Report

---

### Sprint Report (@process-controller)

**Contents**:
- Issues completed: Count + story points total
- Velocity: Story points / sprint duration
- AI token usage: Total tokens + cost breakdown
- Quality metrics: Code coverage, test pass rate
- Team metrics: Issues per developer, review times
- Trends: Velocity trend, cost per story point trend
- Recommendations: Optimization opportunities

**Published**: GitHub issue or Markdown file

---

## 🔄 Continuous Sprint Cycle

```
Sprint N Complete (All issues done, report published)
    ↓ (Immediately - no waiting)
Sprint N+1 Starts
    ├─ Backlog Refinement (unrefined issues)
    ├─ Sprint Planning (~50 story points selected)
    ├─ Architecture & Technical Review
    ├─ Parallel Development (dev → QA → doc → review)
    ├─ Stakeholder Review
    ├─ Final QA Review
    ├─ Merge & Close
    └─ Sprint Report
    ↓
Sprint N+2 Starts (immediately)
...
```

---

## 📊 Key Metrics

### Per Sprint
- **Velocity**: Story points completed per sprint
- **Cycle Time**: Days from "In Progress" to "Done"
- **Code Coverage**: % of code tested
- **Quality**: Bugs found in testing vs. after merge
- **AI Token Usage**: Total tokens spent
- **Cost per Story Point**: Token usage ÷ story points

### Per Issue
- **Size**: Story points
- **Cycle Time**: Days from "Ready" to "Done"
- **Reviews**: Feedback iterations before approval
- **Token Cost**: Tokens used for issue

### Per Team Member
- **Issues Completed**: Count
- **Story Points**: Total points
- **Review Time**: Avg time to review peer's code
- **Feedback Quality**: Feedback accuracy & relevance

---

## 🚨 Blocker Management

If issue is blocked:

1. **Developer** posts on GitHub: "@product-owner BLOCKED: [reason]"
2. **@product-owner** takes action:
   - Remove blocker if possible
   - Escalate to @tech-lead or @devops-engineer if needed
   - Update issue with status
3. **@team-assistant** tracks blocker duration
4. **Report to @process-controller**: Blocker time (included in sprint report)

---

## 📋 Example: Issue #35 (Checkout Acceptance) Under New Process

```
SPRINT 4 STARTS
    ↓

BACKLOG REFINEMENT (Team)
├─ @product-owner: Explains business value
├─ @frontend-dev: "8-12 hours effort"
├─ @qa-engineer: "Need to test German locale"
├─ @tech-lead: "Wolverine handler pattern"
└─ Result: Acceptance criteria clear, 8 story points

SPRINT PLANNING
├─ @product-owner: Selects #35 + 42 more points
├─ Moves #35 to "Ready"
└─ Post: "Sprint 4 started. Issue #35 leads."

ARCHITECTURE REVIEW
├─ @software-architect: "Entity design looks good ✅"
└─ @tech-lead: "Use Wolverine handler pattern ✅"

Move to "In Progress"

PARALLEL DEVELOPMENT
├─ Frontend Dev: "@ui-expert need draft for checkbox component"
│  ├─ @ui-expert: Creates mockup (dark mode variant)
│  ├─ Frontend Dev: Implements per mockup
│  └─ Tests: accessibility (WCAG 2.1 AA), functionality
├─ Backend Dev: Entity + validator + handler
│  ├─ Tests: Valid/invalid scenarios
│  └─ Tests: German locale handling
└─ QA Engineer: Testing from day 1

PR Created when feature complete
├─ @tech-lead: Code review
│  └─ "Looks good ✅"
└─ Address any feedback

STAKEHOLDER REVIEW
├─ @ui-expert: "Component looks good, dark mode works ✅"
├─ @ux-expert: "Flow is correct ✅"
├─ @legal-compliance: "GDPR consent ✅"
├─ @security-engineer: "Audit logging ✅"
├─ Feedback: None out-of-scope
└─ @product-owner: "Approved, ready for final review"

FINAL QA REVIEW (@qa-review)
├─ Acceptance criteria: All met ✅
├─ Coverage: 82% ✅
├─ Documentation: Complete ✅
├─ Accessibility: WCAG 2.1 AA ✅
└─ Sign-off: "✅ APPROVED FOR MERGE"

MERGE (@product-owner)
├─ Merge PR to main
├─ Close issue #35
└─ Post: "✅ Issue #35 complete (8 story points)"

SPRINT CONTINUES with remaining issues...

SPRINT REPORT (when 50+ story points done)
├─ Issues: 5 completed (52 story points)
├─ Velocity: 52 points
├─ AI Tokens: 45,000 tokens
├─ Cost: $0.45 per story point
├─ Coverage: 81%+ average
├─ Quality: 0 regressions post-merge
├─ Timeline: 3 days sprint duration
└─ Recommendation: Maintain current pace
```

---

## ✅ Checklists

### Backlog Refinement Checklist
- [ ] Issue has clear user story
- [ ] Acceptance criteria are SMART
- [ ] Story point estimate agreed by team
- [ ] Technical risks identified
- [ ] Dependencies documented
- [ ] Acceptance criteria include test scenarios

### Sprint Planning Checklist
- [ ] ~50 story points selected
- [ ] Issues ordered by priority
- [ ] Moved to "Ready" status
- [ ] Team aware of sprint scope
- [ ] @software-architect & @tech-lead tagged

### Development Readiness Checklist
- [ ] Architecture review passed ✅
- [ ] Technical review passed ✅
- [ ] Issue moved to "In Progress"
- [ ] Feature branch created
- [ ] Developers assigned

### Stakeholder Review Checklist
- [ ] All relevant stakeholders tagged
- [ ] Feedback collected
- [ ] Feedback filtered (in-scope vs out-of-scope)
- [ ] Out-of-scope items linked to new issues
- [ ] Requirement changes updated in issue

### Final QA Checklist
- [ ] Acceptance criteria: ALL ✅
- [ ] Code coverage: 80%+ ✅
- [ ] Documentation: Complete ✅
- [ ] Accessibility: WCAG 2.1 AA ✅
- [ ] Security review: Passed ✅
- [ ] No breaking changes ✅

### Merge Checklist
- [ ] @qa-review approval present ✅
- [ ] All feedback addressed ✅
- [ ] PR can merge cleanly ✅
- [ ] Issue ready to close ✅

---

**Version**: 1.0  
**Process Owner**: @product-owner  
**Next Review**: After Sprint 4 completion  
**Contact**: @scrum-master for process questions

