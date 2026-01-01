#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class IssueDashboard {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];
    }

    async updateDashboard(issueNumber, category, status) {
        try {
            console.log(`📊 Updating dashboard for issue #${issueNumber}`);

            // Read current dashboard data
            const dashboardPath = path.join(__dirname, '..', '.ai', 'status', 'issue-dashboard.json');
            let dashboard = { issues: {}, summary: {} };

            try {
                const dashboardContent = await fs.readFile(dashboardPath, 'utf8');
                dashboard = JSON.parse(dashboardContent);
            } catch (error) {
                // Dashboard doesn't exist yet, use default
            }

            // Update issue status
            dashboard.issues[issueNumber] = {
                category: category,
                status: status,
                lastUpdated: new Date().toISOString(),
                automated: true
            };

            // Update summary statistics
            dashboard.summary = await this.calculateSummary(dashboard.issues);

            // Write updated dashboard
            await fs.writeFile(dashboardPath, JSON.stringify(dashboard, null, 2));

            console.log(`✅ Dashboard updated`);

        } catch (error) {
            console.error('❌ Error updating dashboard:', error);
            throw error;
        }
    }

    async calculateSummary(issues) {
        const summary = {
            total: Object.keys(issues).length,
            byCategory: {},
            byStatus: {},
            automatedClassifications: 0,
            lastUpdated: new Date().toISOString()
        };

        for (const issue of Object.values(issues)) {
            // Count by category
            summary.byCategory[issue.category] = (summary.byCategory[issue.category] || 0) + 1;

            // Count by status
            summary.byStatus[issue.status] = (summary.byStatus[issue.status] || 0) + 1;

            // Count automated classifications
            if (issue.automated) {
                summary.automatedClassifications++;
            }
        }

        return summary;
    }

    async generateDashboardReport() {
        try {
            console.log(`📈 Generating dashboard report`);

            const dashboardPath = path.join(__dirname, '..', '.ai', 'status', 'issue-dashboard.json');
            const dashboard = JSON.parse(await fs.readFile(dashboardPath, 'utf8'));

            // Generate markdown report
            const report = this.generateMarkdownReport(dashboard);

            // Write report
            const reportPath = path.join(__dirname, '..', '.ai', 'status', 'issue-dashboard.md');
            await fs.writeFile(reportPath, report);

            console.log(`✅ Dashboard report generated: ${reportPath}`);

            return report;

        } catch (error) {
            console.error('❌ Error generating dashboard report:', error);
            throw error;
        }
    }

    generateMarkdownReport(dashboard) {
        const { summary, issues } = dashboard;

        return `# Issue Analysis Dashboard

**Generated:** ${new Date().toISOString()}
**Total Issues Processed:** ${summary.total}

## Summary Statistics

### By Category
${Object.entries(summary.byCategory).map(([category, count]) =>
    `- **${category}:** ${count} issues`
).join('\n')}

### By Status
${Object.entries(summary.byStatus).map(([status, count]) =>
    `- **${status}:** ${count} issues`
).join('\n')}

### Automation Metrics
- **Automated Classifications:** ${summary.automatedClassifications}
- **Automation Rate:** ${((summary.automatedClassifications / summary.total) * 100).toFixed(1)}%

## Recent Issues

${Object.entries(issues)
    .sort(([,a], [,b]) => new Date(b.lastUpdated) - new Date(a.lastUpdated))
    .slice(0, 10)
    .map(([number, data]) =>
        `### Issue #${number}
- **Category:** ${data.category}
- **Status:** ${data.status}
- **Automated:** ${data.automated ? '✅' : '❌'}
- **Last Updated:** ${new Date(data.lastUpdated).toLocaleString()}`
    ).join('\n\n')}

## Performance Metrics

### Response Times (Target vs Actual)
- **Average Response Time:** [To be calculated from logs]
- **Target SLA Compliance:** [To be calculated]

### Classification Accuracy
- **High Confidence Rate:** [To be calculated from logs]
- **Manual Override Rate:** [To be calculated]

---

*This dashboard is automatically updated with each issue analysis.*`;
    }
}

// Main execution
async function main() {
    const args = process.argv.slice(2);

    if (args.length === 0) {
        // Generate full report
        const dashboard = new IssueDashboard();
        await dashboard.generateDashboardReport();
    } else {
        // Update specific issue
        const [issueNumber, category, status] = args;
        const dashboard = new IssueDashboard();
        await dashboard.updateDashboard(parseInt(issueNumber), category, status);
    }
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = IssueDashboard;