#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class IssueAnalyzer {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];

        // Classification patterns with weights
        this.categories = {
            bug: {
                keywords: [
                    'error', 'bug', 'crash', 'fail', 'broken', 'issue', 'problem',
                    'exception', 'not working', 'doesn\'t work', 'stopped working',
                    'unusable', 'broken', 'defect', 'glitch', 'malfunction',
                    'security vulnerability', 'data loss', 'corruption', 'leak',
                    'performance', 'slow', 'hang', 'freeze', 'timeout'
                ],
                weight: 1.0,
                agent: 'qa-team',
                labels: ['bug', 'triage']
            },
            feature: {
                keywords: [
                    'feature', 'enhancement', 'add', 'new', 'would like', 'should have',
                    'need', 'request', 'suggest', 'propose', 'implement', 'create',
                    'build', 'develop', 'integration', 'api', 'endpoint', 'functionality'
                ],
                weight: 1.0,
                agent: 'product-owner',
                labels: ['enhancement', 'feature-request']
            },
            change: {
                keywords: [
                    'change', 'modify', 'update', 'alter', 'adjust', 'configure',
                    'setting', 'parameter', 'workflow', 'process', 'migration',
                    'upgrade', 'refactor', 'restructure', 'optimize', 'improve'
                ],
                weight: 0.9,
                agent: 'architect',
                labels: ['change-request', 'enhancement']
            },
            knowhow: {
                keywords: [
                    'how', 'help', 'guide', 'documentation', 'setup', 'configure',
                    'integrate', 'tutorial', 'example', 'best practice', 'question',
                    'support', 'assistance', 'learn', 'understand', 'explain',
                    'what is', 'where', 'when', 'why', 'faq', 'knowledge'
                ],
                weight: 0.8,
                agent: 'devrel',
                labels: ['question', 'documentation']
            },
            nonsense: {
                keywords: [
                    'test', 'hello world', 'lorem ipsum', 'random', 'spam', 'joke',
                    'meme', 'funny', 'lol', 'haha', 'wtf', 'stupid', 'nonsense',
                    'gibberish', 'meaningless', 'irrelevant', 'off-topic', 'unrelated',
                    'nicht relevant', 'unsinn', 'spaß', 'witz', 'testnachricht',
                    'dummy', 'placeholder', 'example text', 'bla bla', 'yada yada'
                ],
                weight: 1.2,
                agent: 'none',
                labels: ['invalid', 'nonsense'],
                close: true
            },
            'non-product': {
                keywords: [
                    'general', 'question', 'help', 'support', 'how to', 'what is',
                    'please', 'thank you', 'hi', 'hello', 'hey', 'dear',
                    'i need', 'i want', 'can you', 'could you', 'would you',
                    'personal', 'private', 'unrelated', 'different topic'
                ],
                weight: 0.5,
                agent: 'none',
                labels: ['invalid', 'non-product'],
                close: true
            }
        };

        // Context multipliers for more accurate classification
        this.contextMultipliers = {
            bug: {
                'urgent': 1.5, 'critical': 1.5, 'production': 1.3, 'security': 1.5,
                'data': 1.2, 'crash': 1.4, 'error': 1.3, 'fail': 1.3
            },
            feature: {
                'new': 1.3, 'add': 1.2, 'create': 1.2, 'implement': 1.2,
                'integration': 1.3, 'api': 1.2, 'enhancement': 1.2
            },
            change: {
                'modify': 1.3, 'update': 1.2, 'change': 1.2, 'configure': 1.2,
                'workflow': 1.3, 'process': 1.2, 'migration': 1.3
            },
            knowhow: {
                'how': 1.4, 'help': 1.3, 'guide': 1.2, 'documentation': 1.3,
                'setup': 1.2, 'configure': 1.2, 'question': 1.2
            },
            nonsense: {
                'test': 1.5, 'spam': 1.5, 'joke': 1.4, 'meme': 1.4, 'lol': 1.3,
                'wtf': 1.3, 'unsinn': 1.5, 'spaß': 1.4, 'witz': 1.4, 'nonsense': 1.5
            },
            'non-product': {
                'general': 1.3, 'question': 1.2, 'help': 1.2, 'support': 1.2,
                'personal': 1.4, 'private': 1.4, 'unrelated': 1.5
            }
        };

        // Product reference validation
        this.productKeywords = [
            'b2connect', 'b2c', 'api', 'endpoint', 'service', 'microservice',
            'database', 'frontend', 'backend', 'ui', 'component', 'vue', 'dotnet',
            'cqrs', 'wolverine', 'aspire', 'entity framework', 'ef core',
            'authentication', 'authorization', 'security', 'login', 'user',
            'customer', 'order', 'product', 'catalog', 'cms', 'content',
            'localization', 'translation', 'search', 'elasticsearch', 'index',
            'deployment', 'docker', 'kubernetes', 'ci/cd', 'pipeline',
            'testing', 'unit test', 'integration test', 'e2e', 'qa'
        ];

        // Minimum confidence threshold for product-related issues
        this.minProductConfidence = 0.6; // 60% minimum for clear product reference
    }

    async analyzeIssue(issueNumber, title, body) {
        try {
            console.log(`🔍 Analyzing issue #${issueNumber}: ${title}`);

            const content = `${title} ${body}`.toLowerCase();
            const scores = {};

            // Calculate base scores for each category
            for (const [category, config] of Object.entries(this.categories)) {
                scores[category] = this.calculateScore(content, config);
            }

            // Apply context multipliers
            for (const [category, score] of Object.entries(scores)) {
                scores[category] *= this.applyContextMultiplier(content, category);
            }

            // Validate product reference
            const hasProductReference = this.hasClearProductReference(content);
            const productConfidence = this.calculateProductConfidence(content);

            // Find best category
            const bestCategory = Object.keys(scores).reduce((a, b) =>
                scores[a] > scores[b] ? a : b
            );

            const confidence = scores[bestCategory];
            const categoryConfig = this.categories[bestCategory];

            // Strict product reference validation
            let finalCategory = bestCategory;
            let finalConfidence = confidence;
            let shouldClose = categoryConfig.close || false;

            if (!hasProductReference || productConfidence < this.minProductConfidence) {
                // Not clearly product-related, treat as non-product
                finalCategory = 'non-product';
                finalConfidence = Math.max(productConfidence, 0.1);
                shouldClose = true;
            }

            // Get final category config
            const finalConfig = this.categories[finalCategory] || {
                agent: 'none',
                labels: ['invalid', 'non-product'],
                close: true
            };

            // Assess priority
            const priority = this.assessPriority(content, finalCategory);

            // Extract template data
            const templateData = this.extractTemplateData(content, finalCategory);

            const result = {
                category: finalCategory,
                confidence: Math.min(finalConfidence, 1.0),
                priority: priority,
                agent: finalConfig.agent,
                labels: finalConfig.labels,
                close: shouldClose,
                templateData: templateData,
                scores: scores,
                productReference: {
                    hasReference: hasProductReference,
                    confidence: productConfidence
                }
            };

            console.log(`✅ Classification: ${bestCategory} (confidence: ${(result.confidence * 100).toFixed(1)}%)`);
            console.log(`🏷️ Labels: ${result.labels.join(', ')}`);
            console.log(`👤 Agent: ${result.agent}`);
            console.log(`🎯 Priority: ${result.priority}`);

            // Log analysis for monitoring
            await this.logAnalysis(issueNumber, result);

            return result;

        } catch (error) {
            console.error('❌ Error analyzing issue:', error);
            throw error;
        }
    }

    calculateScore(content, config) {
        let score = 0;
        let totalMatches = 0;

        for (const keyword of config.keywords) {
            const regex = new RegExp(`\\b${keyword.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, 'gi');
            const matches = content.match(regex);
            if (matches) {
                score += matches.length * config.weight;
                totalMatches += matches.length;
            }
        }

        // Normalize score based on content length and keyword density
        const contentWords = content.split(/\s+/).length;
        const density = totalMatches / Math.max(contentWords, 1);

        return Math.min(score * density * 10, 1.0); // Scale and cap
    }

    applyContextMultiplier(content, category) {
        let multiplier = 1.0;
        const multipliers = this.contextMultipliers[category];

        for (const [word, mult] of Object.entries(multipliers)) {
            if (content.includes(word)) {
                multiplier *= mult;
            }
        }

        return Math.min(multiplier, 2.0); // Cap multiplier
    }

    assessPriority(content, category) {
        const priorityIndicators = {
            critical: ['security', 'data loss', 'production down', 'all users', 'urgent', 'emergency'],
            high: ['crash', 'unusable', 'blocking', 'production', 'customer impact', 'deadline'],
            medium: ['error', 'broken', 'slow', 'inconsistent', 'annoying'],
            low: ['cosmetic', 'minor', 'suggestion', 'nice to have', 'future']
        };

        for (const [level, indicators] of Object.entries(priorityIndicators)) {
            for (const indicator of indicators) {
                if (content.includes(indicator)) {
                    return level;
                }
            }
        }

        // Default priorities by category
        const defaults = {
            bug: 'medium',
            feature: 'low',
            change: 'medium',
            knowhow: 'low',
            nonsense: 'low',
            'non-product': 'low'
        };

        return defaults[category] || 'medium';
    }

    extractTemplateData(content, category) {
        // Extract relevant data for response templates
        const data = {
            category: category,
            environment: this.extractEnvironment(content),
            version: this.extractVersion(content),
            impact: this.extractImpact(content),
            topic: this.extractTopic(content),
            reproduction: this.extractReproduction(content),
            urgency: this.assessPriority(content, category)
        };

        return data;
    }

    extractEnvironment(content) {
        const environments = ['production', 'staging', 'development', 'test', 'local'];
        for (const env of environments) {
            if (content.includes(env)) return env;
        }
        return 'not specified';
    }

    extractVersion(content) {
        const versionRegex = /version[:\s]*([0-9]+\.[0-9]+(?:\.[0-9]+)?)/i;
        const match = content.match(versionRegex);
        return match ? match[1] : 'not specified';
    }

    extractImpact(content) {
        if (content.includes('all users') || content.includes('production down')) return 'critical';
        if (content.includes('many users') || content.includes('blocking')) return 'high';
        if (content.includes('some users') || content.includes('inconsistent')) return 'medium';
        return 'low';
    }

    extractTopic(content) {
        // Extract main topic from content
        const topics = ['api', 'ui', 'database', 'authentication', 'performance', 'security'];
        for (const topic of topics) {
            if (content.includes(topic)) return topic;
        }
        return 'general';
    }

    extractReproduction(content) {
        if (content.includes('steps to reproduce') || content.includes('reproduction steps')) {
            return 'provided';
        }
        return 'not provided';
    }

    hasClearProductReference(content) {
        // Check for explicit B2Connect references - highest priority
        const explicitRefs = ['b2connect', 'b2c'];
        for (const ref of explicitRefs) {
            if (content.includes(ref)) {
                return true;
            }
        }

        // Check for B2Connect-specific technical combinations
        const b2cSpecificTerms = ['aspire', 'wolverine'];
        let b2cSpecificCount = 0;
        for (const term of b2cSpecificTerms) {
            if (content.includes(term)) {
                b2cSpecificCount++;
            }
        }

        // One B2Connect-specific term is enough
        if (b2cSpecificCount >= 1) {
            return true;
        }

        // Check for general technical terms that need more context
        const generalTechTerms = ['cqrs', 'ef core', 'entity framework', 'vue', 'dotnet'];
        let generalTechCount = 0;
        for (const term of generalTechTerms) {
            if (content.includes(term)) {
                generalTechCount++;
            }
        }

        // Need at least 3 general technical terms to be considered B2Connect-related
        // This prevents false positives from generic .NET/Vue.js questions
        return generalTechCount >= 3;
    }

    calculateProductConfidence(content) {
        let score = 0;
        let totalMatches = 0;

        for (const keyword of this.productKeywords) {
            const regex = new RegExp(`\\b${keyword.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, 'gi');
            const matches = content.match(regex);
            if (matches) {
                score += matches.length;
                totalMatches += matches.length;
            }
        }

        // Normalize based on content length
        const contentWords = content.split(/\s+/).length;
        const density = totalMatches / Math.max(contentWords, 1);

        return Math.min(score * density * 5, 1.0); // Scale and cap
    }

    async logAnalysis(issueNumber, result) {
        const logEntry = {
            timestamp: new Date().toISOString(),
            issueNumber: issueNumber,
            category: result.category,
            confidence: result.confidence,
            priority: result.priority,
            agent: result.agent,
            labels: result.labels,
            close: result.close,
            scores: result.scores,
            productReference: result.productReference
        };

        const logPath = path.join(__dirname, '..', '.ai', 'logs', 'issue-analysis.jsonl');

        try {
            await fs.appendFile(logPath, JSON.stringify(logEntry) + '\n');
        } catch (error) {
            console.warn('⚠️ Could not write analysis log:', error.message);
        }
    }
}

// Main execution
async function main() {
    const [,, issueNumber, title, body] = process.argv;

    if (!issueNumber || !title) {
        console.error('Usage: node analyze-issue.js <issue-number> <title> <body>');
        process.exit(1);
    }

    const analyzer = new IssueAnalyzer();
    const result = await analyzer.analyzeIssue(parseInt(issueNumber), title, body || '');

    // Output for GitHub Actions
    console.log(`::set-output name=category::${result.category}`);
    console.log(`::set-output name=priority::${result.priority}`);
    console.log(`::set-output name=confidence::${result.confidence}`);
    console.log(`::set-output name=agent::${result.agent}`);
    console.log(`::set-output name=close::${result.close}`);
    console.log(`::set-output name=template_data::${JSON.stringify(result.templateData)}`);
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = IssueAnalyzer;
