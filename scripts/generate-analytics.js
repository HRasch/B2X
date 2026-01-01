#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class AnalyticsGenerator {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];
    }

    async generateWeeklyAnalytics() {
        try {
            console.log(`📊 Generating weekly analytics report`);

            const now = new Date();
            const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);

            // Collect data from various logs
            const analytics = {
                period: {
                    start: oneWeekAgo.toISOString(),
                    end: now.toISOString()
                },
                issues: await this.analyzeIssues(oneWeekAgo),
                classifications: await this.analyzeClassifications(oneWeekAgo),
                responses: await this.analyzeResponses(oneWeekAgo),
                processes: await this.analyzeProcesses(oneWeekAgo),
                accuracy: await this.analyzeAccuracy(oneWeekAgo),
                summary: {}
            };

            // Calculate summary metrics
            analytics.summary = this.calculateSummaryMetrics(analytics);

            // Generate markdown report
            const report = this.generateMarkdownReport(analytics);

            // Write report
            const reportPath = path.join(__dirname, '..', '.ai', 'status', 'weekly-analytics.md');
            await fs.writeFile(reportPath, report);

            // Also write JSON data
            const jsonPath = path.join(__dirname, '..', '.ai', 'status', 'weekly-analytics.json');
            await fs.writeFile(jsonPath, JSON.stringify(analytics, null, 2));

            console.log(`✅ Weekly analytics generated: ${reportPath}`);

            return analytics;

        } catch (error) {
            console.error('❌ Error generating weekly analytics:', error);
            throw error;
        }
    }

    async analyzeIssues(since) {
        try {
            const { data: issues } = await this.octokit.issues.listForRepo({
                owner: this.owner,
                repo: this.repo,
                state: 'all',
                since: since.toISOString(),
                per_page: 100
            });

            const customerIssues = issues.filter(issue =>
                !issue.pull_request && issue.user.type !== 'Bot'
            );

            return {
                total: customerIssues.length,
                opened: customerIssues.filter(i => i.state === 'open').length,
                closed: customerIssues.filter(i => i.state === 'closed').length,
                byCategory: this.categorizeIssues(customerIssues),
                averageTimeToClose: this.calculateAverageTimeToClose(customerIssues)
            };

        } catch (error) {
            console.warn('⚠️ Could not analyze issues:', error.message);
            return { total: 0, opened: 0, closed: 0, byCategory: {}, averageTimeToClose: 0 };
        }
    }

    async analyzeClassifications(since) {
        try {
            const logPath = path.join(__dirname, '..', '.ai', 'logs', 'issue-analysis.jsonl');
            const classifications = await this.readLogFile(logPath, since);

            const stats = {
                total: classifications.length,
                byCategory: {},
                confidenceDistribution: {
                    high: 0, // >80%
                    medium: 0, // 60-80%
                    low: 0 // <60%
                },
                averageConfidence: 0
            };

            let totalConfidence = 0;

            classifications.forEach(entry => {
                // Count by category
                stats.byCategory[entry.category] = (stats.byCategory[entry.category] || 0) + 1;

                // Count by confidence
                if (entry.confidence > 0.8) stats.confidenceDistribution.high++;
                else if (entry.confidence > 0.6) stats.confidenceDistribution.medium++;
                else stats.confidenceDistribution.low++;

                totalConfidence += entry.confidence;
            });

            stats.averageConfidence = stats.total > 0 ? totalConfidence / stats.total : 0;

            return stats;

        } catch (error) {
            console.warn('⚠️ Could not analyze classifications:', error.message);
            return { total: 0, byCategory: {}, confidenceDistribution: { high: 0, medium: 0, low: 0 }, averageConfidence: 0 };
        }
    }

    async analyzeResponses(since) {
        try {
            const logPath = path.join(__dirname, '..', '.ai', 'logs', 'responses.jsonl');
            const responses = await this.readLogFile(logPath, since);

            return {
                total: responses.length,
                averageLength: responses.length > 0 ?
                    responses.reduce((sum, r) => sum + r.responseLength, 0) / responses.length : 0,
                byCategory: this.groupBy(responses, 'category')
            };

        } catch (error) {
            console.warn('⚠️ Could not analyze responses:', error.message);
            return { total: 0, averageLength: 0, byCategory: {} };
        }
    }

    async analyzeProcesses(since) {
        try {
            const logPath = path.join(__dirname, '..', '.ai', 'logs', 'process-triggers.jsonl');
            const processes = await this.readLogFile(logPath, since);

            return {
                total: processes.length,
                byCategory: this.groupBy(processes, 'category'),
                byAgent: this.groupBy(processes, 'agent')
            };

        } catch (error) {
            console.warn('⚠️ Could not analyze processes:', error.message);
            return { total: 0, byCategory: {}, byAgent: {} };
        }
    }

    async analyzeAccuracy(since) {
        try {
            const logPath = path.join(__dirname, '..', '.ai', 'logs', 'accuracy-validation.jsonl');
            const validations = await this.readLogFile(logPath, since);

            if (validations.length === 0) {
                return { total: 0, accuracy: 0, categoryAccuracy: 0, priorityAccuracy: 0 };
            }

            const correct = validations.filter(v => v.accuracy.overall).length;
            const categoryCorrect = validations.filter(v => v.accuracy.category).length;
            const priorityCorrect = validations.filter(v => v.accuracy.priority).length;

            return {
                total: validations.length,
                accuracy: (correct / validations.length) * 100,
                categoryAccuracy: (categoryCorrect / validations.length) * 100,
                priorityAccuracy: (priorityCorrect / validations.length) * 100
            };

        } catch (error) {
            console.warn('⚠️ Could not analyze accuracy:', error.message);
            return { total: 0, accuracy: 0, categoryAccuracy: 0, priorityAccuracy: 0 };
        }
    }

    async readLogFile(logPath, since) {
        try {
            const content = await fs.readFile(logPath, 'utf8');
            const lines = content.trim().split('\n');

            return lines
                .map(line => JSON.parse(line))
                .filter(entry => new Date(entry.timestamp) >= since);

        } catch (error) {
            return [];
        }
    }

    categorizeIssues(issues) {
        const categories = {};

        issues.forEach(issue => {
            const labels = issue.labels.map(l => typeof l === 'string' ? l : l.name);
            const categoryLabel = labels.find(l => l.startsWith('type/'));

            if (categoryLabel) {
                const category = categoryLabel.replace('type/', '');
                categories[category] = (categories[category] || 0) + 1;
            } else {
                categories['unclassified'] = (categories['unclassified'] || 0) + 1;
            }
        });

        return categories;
    }

    calculateAverageTimeToClose(issues) {
        const closedIssues = issues.filter(i => i.state === 'closed' && i.closed_at);

        if (closedIssues.length === 0) return 0;

        const totalTime = closedIssues.reduce((sum, issue) => {
            const created = new Date(issue.created_at);
            const closed = new Date(issue.closed_at);
            return sum + (closed - created);
        }, 0);

        // Return average in hours
        return Math.round((totalTime / closedIssues.length) / (1000 * 60 * 60));
    }

    groupBy(array, key) {
        return array.reduce((groups, item) => {
            const value = item[key];
            groups[value] = (groups[value] || 0) + 1;
            return groups;
        }, {});
    }

    calculateSummaryMetrics(analytics) {
        return {
            automationRate: analytics.issues.total > 0 ?
                (analytics.classifications.total / analytics.issues.total) * 100 : 0,
            highConfidenceRate: analytics.classifications.total > 0 ?
                (analytics.classifications.confidenceDistribution.high / analytics.classifications.total) * 100 : 0,
            averageResolutionTime: analytics.issues.averageTimeToClose,
            classificationAccuracy: analytics.accuracy.accuracy,
            topCategory: Object.entries(analytics.classifications.byCategory)
                .sort(([,a], [,b]) => b - a)[0]?.[0] || 'none',
            generatedAt: new Date().toISOString()
        };
    }

    generateMarkdownReport(analytics) {
        const { summary } = analytics;

        return `# Weekly Issue Analysis Report

**Period:** ${new Date(analytics.period.start).toLocaleDateString()} - ${new Date(analytics.period.end).toLocaleDateString()}
**Generated:** ${new Date().toISOString()}

## Executive Summary

- **Issues Processed:** ${analytics.issues.total}
- **Automation Rate:** ${summary.automationRate.toFixed(1)}%
- **High Confidence Classifications:** ${summary.highConfidenceRate.toFixed(1)}%
- **Average Resolution Time:** ${summary.averageResolutionTime} hours
- **Classification Accuracy:** ${summary.classificationAccuracy.toFixed(1)}%
- **Top Category:** ${summary.topCategory}

## Issue Volume

### Total Issues: ${analytics.issues.total}
- **Opened:** ${analytics.issues.opened}
- **Closed:** ${analytics.issues.closed}
- **Average Time to Close:** ${analytics.issues.averageTimeToClose} hours

### Issues by Category
${Object.entries(analytics.issues.byCategory).map(([category, count]) =>
    `- **${category}:** ${count} issues`
).join('\n')}

## Classification Performance

### Classification Distribution
- **High Confidence (>80%):** ${analytics.classifications.confidenceDistribution.high}
- **Medium Confidence (60-80%):** ${analytics.classifications.confidenceDistribution.medium}
- **Low Confidence (<60%):** ${analytics.classifications.confidenceDistribution.low}
- **Average Confidence:** ${(analytics.classifications.averageConfidence * 100).toFixed(1)}%

### Classifications by Category
${Object.entries(analytics.classifications.byCategory).map(([category, count]) =>
    `- **${category}:** ${count} classifications`
).join('\n')}

## Response Analytics

- **Automated Responses:** ${analytics.responses.total}
- **Average Response Length:** ${analytics.responses.averageLength.toFixed(0)} characters

### Responses by Category
${Object.entries(analytics.responses.byCategory).map(([category, count]) =>
    `- **${category}:** ${count} responses`
).join('\n')}

## Process Triggers

- **Processes Started:** ${analytics.processes.total}

### Processes by Category
${Object.entries(analytics.processes.byCategory).map(([category, count]) =>
    `- **${category}:** ${count} processes`
).join('\n')}

### Processes by Agent
${Object.entries(analytics.processes.byAgent).map(([agent, count]) =>
    `- **${agent}:** ${count} assignments`
).join('\n')}

## Quality Metrics

- **Accuracy Validations:** ${analytics.accuracy.total}
- **Overall Accuracy:** ${analytics.accuracy.accuracy.toFixed(1)}%
- **Category Accuracy:** ${analytics.accuracy.categoryAccuracy.toFixed(1)}%
- **Priority Accuracy:** ${analytics.accuracy.priorityAccuracy.toFixed(1)}%

## Recommendations

${this.generateRecommendations(analytics)}

---

*This report is automatically generated weekly to track the performance of the automated issue analysis system.*`;
    }

    generateRecommendations(analytics) {
        const recommendations = [];

        if (analytics.classifications.confidenceDistribution.low > analytics.classifications.total * 0.2) {
            recommendations.push('⚠️ **High low-confidence classifications detected** - Consider improving keyword patterns or adding manual review for classifications below 60% confidence.');
        }

        if (analytics.accuracy.accuracy < 85) {
            recommendations.push('📈 **Classification accuracy below target** - Review misclassifications and update training patterns to improve accuracy above 85%.');
        }

        if (analytics.issues.averageTimeToClose > 48) {
            recommendations.push('⏱️ **Resolution times are high** - Review process efficiency and consider optimizing automated routing to reduce average resolution time below 48 hours.');
        }

        if (analytics.summary.automationRate < 90) {
            recommendations.push('🤖 **Low automation rate** - Investigate why only ' + analytics.summary.automationRate.toFixed(1) + '% of issues are being automatically processed.');
        }

        if (recommendations.length === 0) {
            recommendations.push('✅ **All metrics within acceptable ranges** - Continue monitoring and consider fine-tuning for further optimization.');
        }

        return recommendations.join('\n\n');
    }
}

// Main execution
async function main() {
    const generator = new AnalyticsGenerator();
    await generator.generateWeeklyAnalytics();
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = AnalyticsGenerator;