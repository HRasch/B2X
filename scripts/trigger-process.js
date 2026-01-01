#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class ProcessTrigger {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];

        // Process mappings for each category
        this.processMappings = {
            bug: {
                agent: 'qa-team',
                process: 'Bug Investigation Process',
                template: this.createBugInvestigationIssue,
                project: 'Bug Fixes'
            },
            feature: {
                agent: 'product-owner',
                process: 'Feature Evaluation Process',
                template: this.createFeatureEvaluationIssue,
                project: 'Product Backlog'
            },
            change: {
                agent: 'architect',
                process: 'Change Control Process',
                template: this.createChangeControlIssue,
                project: 'Change Management'
            },
            knowhow: {
                agent: 'devrel',
                process: 'Knowledge Support Process',
                template: this.createKnowledgeSupportIssue,
                project: 'Support'
            }
        };
    }

    async triggerProcess(issueNumber, category, agent) {
        try {
            console.log(`🚀 Triggering ${category} process for issue #${issueNumber}`);

            const mapping = this.processMappings[category];
            if (!mapping) {
                console.warn(`⚠️ No process mapping found for category: ${category}`);
                return;
            }

            // Get original issue details
            const { data: issue } = await this.octokit.issues.get({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber
            });

            // Create follow-up issue for the process
            const followUpIssue = await mapping.template.call(this, issue, mapping);

            // Assign the issue to the appropriate agent/team
            await this.assignIssue(followUpIssue.number, agent);

            // Add to project board if available
            await this.addToProject(followUpIssue.number, mapping.project);

            // Link back to original issue
            await this.linkIssues(issueNumber, followUpIssue.number, category);

            // Log the process trigger
            await this.logProcessTrigger(issueNumber, followUpIssue.number, category, agent);

            console.log(`✅ Process triggered: ${followUpIssue.html_url}`);

        } catch (error) {
            console.error('❌ Error triggering process:', error);
            throw error;
        }
    }

    async createBugInvestigationIssue(originalIssue, mapping) {
        const title = `🐛 Bug Investigation: ${originalIssue.title}`;
        const body = `## Bug Investigation Process

**Original Issue:** #${originalIssue.number} - ${originalIssue.title}
**Reported by:** @${originalIssue.user.login}
**Priority:** ${this.extractPriorityFromLabels(originalIssue.labels)}

### Investigation Checklist
- [ ] **Reproduction Attempt** - Try to reproduce the issue
- [ ] **Environment Setup** - Ensure test environment matches reported conditions
- [ ] **Root Cause Analysis** - Identify the underlying cause
- [ ] **Impact Assessment** - Determine scope and severity of the issue
- [ ] **Fix Planning** - Develop solution approach
- [ ] **Testing Strategy** - Plan validation and regression tests

### Required Information
- [ ] Reproduction steps confirmed
- [ ] Environment details verified
- [ ] Logs and error messages collected
- [ ] Screenshots/videos if applicable

### Next Steps
1. Review original issue for complete information
2. Attempt reproduction within 24 hours
3. Escalate to development team if confirmed
4. Coordinate fix implementation and testing

**SLA:** Complete investigation within ${this.getInvestigationSLA(originalIssue.labels)}`;

        return await this.octokit.issues.create({
            owner: this.owner,
            repo: this.repo,
            title: title,
            body: body,
            labels: ['investigation', 'bug', 'process-triggered']
        });
    }

    async createFeatureEvaluationIssue(originalIssue, mapping) {
        const title = `✨ Feature Evaluation: ${originalIssue.title}`;
        const body = `## Feature Evaluation Process

**Original Issue:** #${originalIssue.number} - ${originalIssue.title}
**Requested by:** @${originalIssue.user.login}
**Priority:** ${this.extractPriorityFromLabels(originalIssue.labels)}

### Evaluation Checklist
- [ ] **Business Value Assessment** - ROI and strategic alignment
- [ ] **Technical Feasibility** - Implementation complexity and effort
- [ ] **Market Research** - Similar features in competing products
- [ ] **User Research** - Validate need with user feedback
- [ ] **Priority Ranking** - Compare with existing backlog
- [ ] **Timeline Planning** - Sprint/release planning

### Analysis Framework
- **Problem Statement:** What problem does this solve?
- **Target Users:** Who benefits from this feature?
- **Success Metrics:** How will we measure success?
- **Dependencies:** What other features/systems are affected?
- **Risk Assessment:** What could go wrong?

### Decision Criteria
- [ ] **Strategic Alignment:** Does this support our product vision?
- [ ] **Customer Value:** High impact for significant user base?
- [ ] **Technical Debt:** Will this create maintenance burden?
- [ ] **Opportunity Cost:** What won't we work on instead?

### Recommendation
- [ ] **Approve** - Add to backlog with priority
- [ ] **Reject** - Not aligned with strategy
- [ ] **Defer** - Valid but not current priority
- [ ] **Need More Info** - Additional research required

**Timeline:** Complete evaluation within 5 business days`;

        return await this.octokit.issues.create({
            owner: this.owner,
            repo: this.repo,
            title: title,
            body: body,
            labels: ['evaluation', 'feature-request', 'process-triggered']
        });
    }

    async createChangeControlIssue(originalIssue, mapping) {
        const title = `🔄 Change Control: ${originalIssue.title}`;
        const body = `## Change Control Process

**Original Issue:** #${originalIssue.number} - ${originalIssue.title}
**Requested by:** @${originalIssue.user.login}
**Priority:** ${this.extractPriorityFromLabels(originalIssue.labels)}

### Change Control Checklist
- [ ] **Change Classification** - Standard/Emergency/Normal change
- [ ] **Impact Analysis** - Technical and business impact assessment
- [ ] **Risk Assessment** - Potential disruption and mitigation
- [ ] **Rollback Plan** - Recovery procedures documented
- [ ] **Testing Strategy** - Validation and regression testing
- [ ] **Communication Plan** - Stakeholder notification

### Required Documentation
- [ ] **Change Description:** Detailed description of what will change
- [ ] **Business Justification:** Why is this change necessary?
- [ ] **Affected Systems:** List of impacted components
- [ ] **Implementation Plan:** Step-by-step execution plan
- [ ] **Success Criteria:** How will success be measured?

### Approval Requirements
- [ ] **Technical Review:** Development team sign-off
- [ ] **Business Review:** Product owner approval
- [ ] **Security Review:** If security implications exist
- [ ] **Change Control Board:** Final approval for production changes

### Implementation Timeline
- **Planning:** Complete analysis within 2 business days
- **Approval:** CCB review within 3 business days
- **Implementation:** Execute during approved change window
- **Validation:** Complete testing within 1 week post-implementation

**Change Window:** ${this.getNextChangeWindow()}`;

        return await this.octokit.issues.create({
            owner: this.owner,
            repo: this.repo,
            title: title,
            body: body,
            labels: ['change-control', 'change-request', 'process-triggered']
        });
    }

    async createKnowledgeSupportIssue(originalIssue, mapping) {
        const title = `📚 Knowledge Support: ${originalIssue.title}`;
        const body = `## Knowledge Support Process

**Original Issue:** #${originalIssue.number} - ${originalIssue.title}
**Requested by:** @${originalIssue.user.login}
**Priority:** ${this.extractPriorityFromLabels(originalIssue.labels)}

### Support Checklist
- [ ] **Question Analysis** - Understand the specific knowledge gap
- [ ] **Resource Identification** - Find relevant documentation/examples
- [ ] **Answer Formulation** - Provide clear, actionable guidance
- [ ] **Documentation Enhancement** - Identify gaps in existing docs
- [ ] **Follow-up** - Ensure customer understands the solution

### Knowledge Areas to Check
- [ ] **Documentation:** Official docs and guides
- [ ] **FAQ Database:** Existing Q&A in our knowledge base
- [ ] **Community Forum:** Similar questions from other users
- [ ] **Code Examples:** Sample implementations and patterns
- [ ] **Video Tutorials:** Visual guides and walkthroughs

### Response Framework
1. **Immediate Answer:** Provide direct solution to the question
2. **Context & Explanation:** Explain why this is the right approach
3. **Additional Resources:** Link to related documentation
4. **Best Practices:** Include relevant recommendations
5. **Follow-up Questions:** Ask if more clarification is needed

### Documentation Improvement
- [ ] **Gap Identified:** Is this a documentation gap?
- [ ] **FAQ Enhancement:** Should this be added to FAQ?
- [ ] **Guide Creation:** Is a new guide needed?
- [ ] **Example Addition:** Would a code example help?

**Response SLA:** ${this.getSupportSLA(originalIssue.labels)}`;

        return await this.octokit.issues.create({
            owner: this.owner,
            repo: this.repo,
            title: title,
            body: body,
            labels: ['knowledge-support', 'documentation', 'process-triggered']
        });
    }

    async assignIssue(issueNumber, assignee) {
        try {
            // Try to assign to individual or team
            await this.octokit.issues.addAssignees({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber,
                assignees: [assignee]
            });
            console.log(`👤 Assigned to: ${assignee}`);
        } catch (error) {
            console.warn(`⚠️ Could not assign to ${assignee}:`, error.message);
        }
    }

    async addToProject(issueNumber, projectName) {
        try {
            // Note: This requires GitHub Projects API
            // For now, we'll just log the intent
            console.log(`📋 Would add to project: ${projectName}`);
        } catch (error) {
            console.warn('⚠️ Could not add to project:', error.message);
        }
    }

    async linkIssues(originalNumber, followUpNumber, category) {
        // Add a comment linking the issues
        const linkComment = `🔗 **Related Issues**

This ${category} process is linked to the original customer issue #${originalNumber}.

**Process Flow:**
1. **Original Issue** (#${originalNumber}): Customer report
2. **Process Issue** (#${followUpNumber}): Internal handling
3. **Resolution**: Updates will be posted to both issues

Please keep both issues updated with progress and resolution details.`;

        await this.octokit.issues.createComment({
            owner: this.owner,
            repo: this.repo,
            issue_number: followUpNumber,
            body: linkComment
        });

        // Also add a reference comment to the original issue
        const originalComment = `🔗 **Process Started**

We've started our ${category} process to address your issue. Track progress here: #${followUpNumber}

Our team will keep you updated on the resolution timeline and any required information.`;

        await this.octokit.issues.createComment({
            owner: this.owner,
            repo: this.repo,
            issue_number: originalNumber,
            body: originalComment
        });
    }

    extractPriorityFromLabels(labels) {
        const priorityLabels = labels.filter(label =>
            typeof label === 'object' ? label.name.startsWith('priority/') : label.startsWith('priority/')
        );

        if (priorityLabels.length > 0) {
            const priorityLabel = typeof priorityLabels[0] === 'object' ? priorityLabels[0].name : priorityLabels[0];
            return priorityLabel.replace('priority/', '').toUpperCase();
        }

        return 'MEDIUM';
    }

    getInvestigationSLA(labels) {
        const priority = this.extractPriorityFromLabels(labels).toLowerCase();
        const slas = {
            critical: '4 hours',
            high: '24 hours',
            medium: '3 days',
            low: '1 week'
        };
        return slas[priority] || '3 days';
    }

    getSupportSLA(labels) {
        const priority = this.extractPriorityFromLabels(labels).toLowerCase();
        const slas = {
            critical: '1 hour',
            high: '4 hours',
            medium: '24 hours',
            low: '48 hours'
        };
        return slas[priority] || '24 hours';
    }

    getNextChangeWindow() {
        // Calculate next Tuesday/Thursday change window
        const now = new Date();
        const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        const dayOfWeek = now.getDay();

        let daysUntilNextWindow = 0;
        if (dayOfWeek < 2) { // Before Tuesday
            daysUntilNextWindow = 2 - dayOfWeek;
        } else if (dayOfWeek < 4) { // Before Thursday
            daysUntilNextWindow = 4 - dayOfWeek;
        } else { // Next Tuesday
            daysUntilNextWindow = (2 + 7) - dayOfWeek;
        }

        const nextWindow = new Date(now);
        nextWindow.setDate(now.getDate() + daysUntilNextWindow);
        nextWindow.setHours(18, 0, 0, 0); // 6 PM

        return nextWindow.toLocaleDateString() + ' 18:00-22:00 UTC';
    }

    async logProcessTrigger(originalIssue, processIssue, category, agent) {
        const logEntry = {
            timestamp: new Date().toISOString(),
            originalIssue: originalIssue,
            processIssue: processIssue,
            category: category,
            agent: agent,
            process: this.processMappings[category].process
        };

        const logPath = path.join(__dirname, '..', '.ai', 'logs', 'process-triggers.jsonl');

        try {
            await fs.appendFile(logPath, JSON.stringify(logEntry) + '\n');
        } catch (error) {
            console.warn('⚠️ Could not write process trigger log:', error.message);
        }
    }
}

// Main execution
async function main() {
    const [,, issueNumber, category, agent] = process.argv;

    if (!issueNumber || !category || !agent) {
        console.error('Usage: node trigger-process.js <issue-number> <category> <agent>');
        process.exit(1);
    }

    const trigger = new ProcessTrigger();
    await trigger.triggerProcess(parseInt(issueNumber), category, agent);
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = ProcessTrigger;