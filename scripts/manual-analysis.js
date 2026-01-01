#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const IssueAnalyzer = require('./analyze-issue');

class ManualAnalyzer {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];
    }

    async analyzeIssueManually(issueNumber) {
        try {
            console.log(`🔍 Manual analysis of issue #${issueNumber}`);

            // Get issue details
            const { data: issue } = await this.octokit.issues.get({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber
            });

            console.log(`📝 Title: ${issue.title}`);
            console.log(`👤 Author: ${issue.user.login}`);
            console.log(`📅 Created: ${new Date(issue.created_at).toLocaleString()}`);
            console.log(`🏷️ Current Labels: ${issue.labels.map(l => typeof l === 'string' ? l : l.name).join(', ') || 'none'}`);

            // Analyze with our analyzer
            const analyzer = new IssueAnalyzer();
            const result = await analyzer.analyzeIssue(issueNumber, issue.title, issue.body || '');

            // Display detailed analysis
            console.log(`\n🤖 Automated Analysis:`);
            console.log(`   Category: ${result.category}`);
            console.log(`   Confidence: ${(result.confidence * 100).toFixed(1)}%`);
            console.log(`   Priority: ${result.priority}`);
            console.log(`   Agent: ${result.agent}`);
            console.log(`   Labels: ${result.labels.join(', ')}`);

            console.log(`\n📊 Analysis Scores:`);
            Object.entries(result.scores).forEach(([category, score]) => {
                const percentage = (score * 100).toFixed(1);
                const marker = category === result.category ? ' ← SELECTED' : '';
                console.log(`   ${category}: ${percentage}%${marker}`);
            });

            console.log(`\n📋 Template Data:`);
            console.log(`   Version: ${result.templateData.version}`);
            console.log(`   Environment: ${result.templateData.environment}`);
            console.log(`   Impact: ${result.templateData.impact}`);
            console.log(`   Topic: ${result.templateData.topic}`);
            console.log(`   Urgency: ${result.templateData.urgency}`);

            // Show issue body preview
            const bodyPreview = (issue.body || '').substring(0, 300);
            console.log(`\n📄 Issue Body Preview:`);
            console.log(bodyPreview + (issue.body && issue.body.length > 300 ? '...' : ''));

            return result;

        } catch (error) {
            console.error('❌ Error in manual analysis:', error);
            throw error;
        }
    }

    async testClassificationAccuracy(testIssues) {
        console.log(`🧪 Testing classification accuracy on ${testIssues.length} issues`);

        const results = [];

        for (const issueNumber of testIssues) {
            try {
                console.log(`\n--- Testing Issue #${issueNumber} ---`);
                const result = await this.analyzeIssueManually(issueNumber);
                results.push({
                    issueNumber,
                    automated: result.category,
                    confidence: result.confidence
                });
            } catch (error) {
                console.error(`❌ Failed to analyze issue #${issueNumber}:`, error.message);
            }
        }

        // Summary
        console.log(`\n📊 Test Results Summary:`);
        console.log(`Total Issues Tested: ${results.length}`);
        console.log(`High Confidence (>80%): ${results.filter(r => r.confidence > 0.8).length}`);
        console.log(`Medium Confidence (60-80%): ${results.filter(r => r.confidence > 0.6 && r.confidence <= 0.8).length}`);
        console.log(`Low Confidence (<60%): ${results.filter(r => r.confidence <= 0.6).length}`);

        const categoryCounts = {};
        results.forEach(r => {
            categoryCounts[r.automated] = (categoryCounts[r.automated] || 0) + 1;
        });

        console.log(`\n🏷️ Category Distribution:`);
        Object.entries(categoryCounts).forEach(([category, count]) => {
            console.log(`   ${category}: ${count} issues`);
        });

        return results;
    }

    async findTestIssues(count = 10) {
        try {
            // Get recent issues for testing
            const { data: issues } = await this.octokit.issues.listForRepo({
                owner: this.owner,
                repo: this.repo,
                state: 'all',
                per_page: count,
                sort: 'created',
                direction: 'desc'
            });

            // Filter out pull requests and bot issues
            const testIssues = issues
                .filter(issue => !issue.pull_request && issue.user.type !== 'Bot')
                .slice(0, count)
                .map(issue => issue.number);

            console.log(`🎯 Found ${testIssues.length} test issues: ${testIssues.join(', ')}`);
            return testIssues;

        } catch (error) {
            console.error('❌ Error finding test issues:', error);
            throw error;
        }
    }
}

// Main execution
async function main() {
    const [,, command, ...args] = process.argv;

    const analyzer = new ManualAnalyzer();

    if (command === 'test') {
        // Run accuracy tests
        const count = parseInt(args[0]) || 10;
        const testIssues = await analyzer.findTestIssues(count);
        await analyzer.testClassificationAccuracy(testIssues);

    } else if (command === 'analyze') {
        // Analyze specific issue
        const issueNumber = parseInt(args[0]);
        if (!issueNumber) {
            console.error('Usage: node manual-analysis.js analyze <issue-number>');
            process.exit(1);
        }
        await analyzer.analyzeIssueManually(issueNumber);

    } else {
        console.log(`📖 Manual Issue Analysis Tool

Usage:
  node manual-analysis.js analyze <issue-number>  - Analyze a specific issue
  node manual-analysis.js test [count]           - Test accuracy on recent issues

Examples:
  node manual-analysis.js analyze 123
  node manual-analysis.js test 20`);
    }
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = ManualAnalyzer;