#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');

class LabelManager {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];

        // Priority labels with colors
        this.priorityLabels = {
            critical: { name: 'priority/critical', color: 'd73a49', description: 'Critical priority - immediate attention required' },
            high: { name: 'priority/high', color: 'b60205', description: 'High priority - urgent attention needed' },
            medium: { name: 'priority/medium', color: 'fbca04', description: 'Medium priority - should be addressed soon' },
            low: { name: 'priority/low', color: '0e8a16', description: 'Low priority - nice to have' }
        };

        // Category labels with colors
        this.categoryLabels = {
            bug: { name: 'type/bug', color: 'd73a49', description: 'Bug report - something is broken' },
            feature: { name: 'type/feature-request', color: '0052cc', description: 'Feature request - new functionality' },
            change: { name: 'type/change-request', color: 'fbca04', description: 'Change request - modify existing functionality' },
            knowhow: { name: 'type/knowhow', color: '0e8a16', description: 'Knowledge request - how-to questions' },
            nonsense: { name: 'type/nonsense', color: '586069', description: 'Non-product related - will be closed' },
            'non-product': { name: 'type/non-product', color: '6f42c1', description: 'Unclear product reference - will be closed' }
        };

        // Status labels
        this.statusLabels = {
            triage: { name: 'status/triage', color: 'bfdadc', description: 'Issue is being triaged' },
            investigation: { name: 'status/investigation', color: 'd4c5f9', description: 'Issue is under investigation' },
            evaluation: { name: 'status/evaluation', color: 'c2e0c6', description: 'Issue is being evaluated' },
            planning: { name: 'status/planning', color: 'f9d0c4', description: 'Issue is in planning phase' }
        };
    }

    async applyLabels(issueNumber, category, priority, confidence) {
        try {
            console.log(`🏷️ Applying labels to issue #${issueNumber}`);

            // Ensure required labels exist
            await this.ensureLabelsExist();

            // Get current labels
            const { data: issue } = await this.octokit.issues.get({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber
            });

            const currentLabels = issue.labels.map(label =>
                typeof label === 'string' ? label : label.name
            );

            // Determine labels to add
            const labelsToAdd = [];

            // Add category label
            if (this.categoryLabels[category]) {
                labelsToAdd.push(this.categoryLabels[category].name);
            }

            // Add priority label
            if (this.priorityLabels[priority]) {
                labelsToAdd.push(this.priorityLabels[priority].name);
            }

            // Add status label based on category
            const statusLabel = this.getStatusLabelForCategory(category);
            if (statusLabel) {
                labelsToAdd.push(statusLabel);
            }

            // Add confidence label if low confidence
            if (confidence < 0.7) {
                labelsToAdd.push('needs-review');
            }

            // Remove conflicting labels and add new ones
            const labelsToRemove = this.getConflictingLabels(currentLabels, labelsToAdd);

            // Remove old labels
            for (const label of labelsToRemove) {
                try {
                    await this.octokit.issues.removeLabel({
                        owner: this.owner,
                        repo: this.repo,
                        issue_number: issueNumber,
                        name: label
                    });
                    console.log(`➖ Removed label: ${label}`);
                } catch (error) {
                    // Label might not exist, continue
                }
            }

            // Add new labels
            if (labelsToAdd.length > 0) {
                await this.octokit.issues.addLabels({
                    owner: this.owner,
                    repo: this.repo,
                    issue_number: issueNumber,
                    labels: labelsToAdd
                });

                console.log(`➕ Added labels: ${labelsToAdd.join(', ')}`);
            }

            // Set milestone if applicable
            await this.setMilestone(issueNumber, priority);

            console.log(`✅ Labels applied successfully`);

        } catch (error) {
            console.error('❌ Error applying labels:', error);
            throw error;
        }
    }

    async ensureLabelsExist() {
        const allLabels = {
            ...this.priorityLabels,
            ...this.categoryLabels,
            ...this.statusLabels,
            'needs-review': { name: 'needs-review', color: 'ffa500', description: 'Low confidence classification - needs human review' }
        };

        for (const [key, labelConfig] of Object.entries(allLabels)) {
            try {
                await this.octokit.issues.createLabel({
                    owner: this.owner,
                    repo: this.repo,
                    name: labelConfig.name,
                    color: labelConfig.color,
                    description: labelConfig.description
                });
                console.log(`📝 Created label: ${labelConfig.name}`);
            } catch (error) {
                if (error.status !== 422) { // 422 means label already exists
                    console.warn(`⚠️ Could not create label ${labelConfig.name}:`, error.message);
                }
            }
        }
    }

    getConflictingLabels(currentLabels, newLabels) {
        const toRemove = [];

        // Remove old priority labels
        const priorityPrefixes = ['priority/critical', 'priority/high', 'priority/medium', 'priority/low'];
        for (const label of currentLabels) {
            if (priorityPrefixes.some(prefix => label.startsWith(prefix))) {
                toRemove.push(label);
            }
        }

        // Remove old category labels
        const categoryPrefixes = ['type/bug', 'type/feature-request', 'type/change-request', 'type/knowhow'];
        for (const label of currentLabels) {
            if (categoryPrefixes.some(prefix => label.startsWith(prefix))) {
                toRemove.push(label);
            }
        }

        // Remove old status labels if we're adding a new status
        const statusPrefixes = ['status/triage', 'status/investigation', 'status/evaluation', 'status/planning'];
        const hasNewStatus = newLabels.some(label => statusPrefixes.some(prefix => label.startsWith(prefix)));
        if (hasNewStatus) {
            for (const label of currentLabels) {
                if (statusPrefixes.some(prefix => label.startsWith(prefix))) {
                    toRemove.push(label);
                }
            }
        }

        return toRemove;
    }

    getStatusLabelForCategory(category) {
        const statusMap = {
            bug: 'status/investigation',
            feature: 'status/evaluation',
            change: 'status/evaluation',
            knowhow: 'status/triage'
        };
        return statusMap[category];
    }

    async setMilestone(issueNumber, priority) {
        try {
            // Get available milestones
            const { data: milestones } = await this.octokit.issues.listMilestones({
                owner: this.owner,
                repo: this.repo,
                state: 'open'
            });

            // Find appropriate milestone based on priority
            let targetMilestone = null;

            if (priority === 'critical') {
                // Find the earliest open milestone
                targetMilestone = milestones.find(m => m.due_on) || milestones[0];
            } else if (priority === 'high') {
                // Find milestone due within 2 weeks
                const twoWeeksFromNow = new Date();
                twoWeeksFromNow.setDate(twoWeeksFromNow.getDate() + 14);

                targetMilestone = milestones.find(m => {
                    if (!m.due_on) return false;
                    return new Date(m.due_on) <= twoWeeksFromNow;
                }) || milestones[0];
            }

            if (targetMilestone) {
                await this.octokit.issues.update({
                    owner: this.owner,
                    repo: this.repo,
                    issue_number: issueNumber,
                    milestone: targetMilestone.number
                });
                console.log(`🎯 Set milestone: ${targetMilestone.title}`);
            }

        } catch (error) {
            console.warn('⚠️ Could not set milestone:', error.message);
        }
    }
}

// Main execution
async function main() {
    const [,, issueNumber, category, priority, confidence] = process.argv;

    if (!issueNumber || !category || !priority) {
        console.error('Usage: node apply-labels.js <issue-number> <category> <priority> <confidence>');
        process.exit(1);
    }

    const labelManager = new LabelManager();
    await labelManager.applyLabels(
        parseInt(issueNumber),
        category,
        priority,
        parseFloat(confidence)
    );
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = LabelManager;