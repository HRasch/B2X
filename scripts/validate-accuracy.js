#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class AccuracyValidator {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];
    }

    async validateClassification(issueNumber) {
        try {
            console.log(`🔍 Validating classification accuracy for issue #${issueNumber}`);

            // Get issue details
            const { data: issue } = await this.octokit.issues.get({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber
            });

            // Check if issue was closed (indicating manual review)
            if (issue.state !== 'closed') {
                console.log('ℹ️ Issue not closed yet, skipping accuracy validation');
                return;
            }

            // Get final labels after manual review
            const finalLabels = issue.labels.map(label =>
                typeof label === 'string' ? label : label.name
            );

            // Read analysis log to get automated classification
            const automatedResult = await this.getAutomatedClassification(issueNumber);

            if (!automatedResult) {
                console.log('⚠️ No automated classification found for this issue');
                return;
            }

            // Compare classifications
            const accuracy = this.compareClassifications(automatedResult, finalLabels);

            // Log validation result
            await this.logValidation(issueNumber, automatedResult, finalLabels, accuracy);

            console.log(`✅ Validation complete: ${accuracy.correct ? 'Correct' : 'Incorrect'} classification`);

            return accuracy;

        } catch (error) {
            console.error('❌ Error validating classification:', error);
            throw error;
        }
    }

    async getAutomatedClassification(issueNumber) {
        try {
            const logPath = path.join(__dirname, '..', '.ai', 'logs', 'issue-analysis.jsonl');

            const logContent = await fs.readFile(logPath, 'utf8');
            const logLines = logContent.trim().split('\n');

            // Find the most recent entry for this issue
            for (let i = logLines.length - 1; i >= 0; i--) {
                const entry = JSON.parse(logLines[i]);
                if (entry.issueNumber === issueNumber) {
                    return {
                        category: entry.category,
                        confidence: entry.confidence,
                        priority: entry.priority,
                        agent: entry.agent,
                        timestamp: entry.timestamp
                    };
                }
            }

            return null;

        } catch (error) {
            console.warn('⚠️ Could not read analysis log:', error.message);
            return null;
        }
    }

    compareClassifications(automated, finalLabels) {
        // Extract category from final labels
        const categoryLabels = finalLabels.filter(label =>
            label.startsWith('type/')
        );

        let finalCategory = 'unknown';
        if (categoryLabels.length > 0) {
            const categoryLabel = categoryLabels[0];
            if (categoryLabel.includes('bug')) finalCategory = 'bug';
            else if (categoryLabel.includes('feature')) finalCategory = 'feature';
            else if (categoryLabel.includes('change')) finalCategory = 'change';
            else if (categoryLabel.includes('knowhow')) finalCategory = 'knowhow';
        }

        // Extract priority from final labels
        const priorityLabels = finalLabels.filter(label =>
            label.startsWith('priority/')
        );

        let finalPriority = 'medium';
        if (priorityLabels.length > 0) {
            finalPriority = priorityLabels[0].replace('priority/', '');
        }

        // Compare
        const categoryCorrect = automated.category === finalCategory;
        const priorityCorrect = automated.priority === finalPriority;

        return {
            correct: categoryCorrect && priorityCorrect,
            categoryCorrect: categoryCorrect,
            priorityCorrect: priorityCorrect,
            automated: {
                category: automated.category,
                priority: automated.priority,
                confidence: automated.confidence
            },
            manual: {
                category: finalCategory,
                priority: finalPriority
            }
        };
    }

    async logValidation(issueNumber, automated, finalLabels, accuracy) {
        const validationEntry = {
            timestamp: new Date().toISOString(),
            issueNumber: issueNumber,
            automated: accuracy.automated,
            manual: accuracy.manual,
            accuracy: {
                overall: accuracy.correct,
                category: accuracy.categoryCorrect,
                priority: accuracy.priorityCorrect
            },
            finalLabels: finalLabels
        };

        const logPath = path.join(__dirname, '..', '.ai', 'logs', 'accuracy-validation.jsonl');

        try {
            await fs.appendFile(logPath, JSON.stringify(validationEntry) + '\n');
        } catch (error) {
            console.warn('⚠️ Could not write validation log:', error.message);
        }
    }

    async generateAccuracyReport() {
        try {
            console.log(`📊 Generating accuracy report`);

            const logPath = path.join(__dirname, '..', '.ai', 'logs', 'accuracy-validation.jsonl');

            const logContent = await fs.readFile(logPath, 'utf8');
            const logLines = logContent.trim().split('\n');

            const validations = logLines.map(line => JSON.parse(line));

            // Calculate statistics
            const total = validations.length;
            const correct = validations.filter(v => v.accuracy.overall).length;
            const categoryCorrect = validations.filter(v => v.accuracy.category).length;
            const priorityCorrect = validations.filter(v => v.accuracy.priority).length;

            const report = {
                summary: {
                    totalValidations: total,
                    overallAccuracy: total > 0 ? (correct / total) * 100 : 0,
                    categoryAccuracy: total > 0 ? (categoryCorrect / total) * 100 : 0,
                    priorityAccuracy: total > 0 ? (priorityCorrect / total) * 100 : 0,
                    generatedAt: new Date().toISOString()
                },
                recentValidations: validations.slice(-10).reverse(),
                improvementAreas: this.identifyImprovementAreas(validations)
            };

            // Write report
            const reportPath = path.join(__dirname, '..', '.ai', 'status', 'accuracy-report.json');
            await fs.writeFile(reportPath, JSON.stringify(report, null, 2));

            console.log(`✅ Accuracy report generated: ${reportPath}`);
            console.log(`📈 Overall Accuracy: ${(report.summary.overallAccuracy).toFixed(1)}%`);

            return report;

        } catch (error) {
            console.error('❌ Error generating accuracy report:', error);
            throw error;
        }
    }

    identifyImprovementAreas(validations) {
        const issues = [];

        // Find common misclassifications
        const misclassifications = validations.filter(v => !v.accuracy.category);

        if (misclassifications.length > 0) {
            const categoryErrors = {};
            misclassifications.forEach(v => {
                const key = `${v.automated.category}->${v.manual.category}`;
                categoryErrors[key] = (categoryErrors[key] || 0) + 1;
            });

            const topErrors = Object.entries(categoryErrors)
                .sort(([,a], [,b]) => b - a)
                .slice(0, 3);

            issues.push({
                type: 'category_misclassification',
                description: 'Most common category misclassifications',
                details: topErrors.map(([transition, count]) => `${transition}: ${count} times`)
            });
        }

        // Check confidence levels for incorrect classifications
        const lowConfidenceErrors = validations.filter(v =>
            !v.accuracy.overall && v.automated.confidence < 0.7
        );

        if (lowConfidenceErrors.length > 0) {
            issues.push({
                type: 'low_confidence_errors',
                description: 'Incorrect classifications with low confidence',
                count: lowConfidenceErrors.length,
                recommendation: 'Consider requiring manual review for classifications below 70% confidence'
            });
        }

        return issues;
    }
}

// Main execution
async function main() {
    const [,, issueNumber] = process.argv;

    const validator = new AccuracyValidator();

    if (issueNumber) {
        // Validate specific issue
        await validator.validateClassification(parseInt(issueNumber));
    } else {
        // Generate full report
        await validator.generateAccuracyReport();
    }
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = AccuracyValidator;