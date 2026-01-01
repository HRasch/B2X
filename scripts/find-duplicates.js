#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class DuplicateFinder {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];

        // Similarity thresholds
        this.similarityThreshold = 0.7; // Minimum similarity score (0-1)
        this.maxResults = 10; // Maximum similar issues to return
        this.searchWindowDays = 365; // Search in last X days for open issues
    }

    async findDuplicatesAndSolutions(issueNumber, title, body) {
        try {
            console.log(`🔍 Searching for duplicates and solutions for issue #${issueNumber}`);

            const content = `${title} ${body}`.toLowerCase();

            // Search for similar issues
            const similarIssues = await this.searchSimilarIssues(content);

            // Categorize results
            const duplicates = [];
            const solutions = [];
            const related = [];

            for (const issue of similarIssues) {
                const similarity = this.calculateSimilarity(content, `${issue.title} ${issue.body || ''}`);

                if (similarity >= this.similarityThreshold) {
                    if (issue.state === 'closed') {
                        // Check if it has a solution
                        const hasSolution = await this.issueHasSolution(issue.number);
                        if (hasSolution) {
                            solutions.push({
                                ...issue,
                                similarity: similarity,
                                solution: hasSolution
                            });
                        } else {
                            related.push({
                                ...issue,
                                similarity: similarity,
                                reason: 'closed_without_solution'
                            });
                        }
                    } else {
                        // Open issue - potential duplicate
                        duplicates.push({
                            ...issue,
                            similarity: similarity,
                            reason: 'open_similar'
                        });
                    }
                } else if (similarity >= 0.4) {
                    related.push({
                        ...issue,
                        similarity: similarity,
                        reason: 'partial_match'
                    });
                }
            }

            // Sort by similarity (highest first)
            duplicates.sort((a, b) => b.similarity - a.similarity);
            solutions.sort((a, b) => b.similarity - a.similarity);
            related.sort((a, b) => b.similarity - a.similarity);

            const result = {
                duplicates: duplicates.slice(0, 5), // Top 5 duplicates
                solutions: solutions.slice(0, 3),   // Top 3 solutions
                related: related.slice(0, 5),       // Top 5 related
                searchPerformed: true,
                totalFound: similarIssues.length
            };

            console.log(`✅ Found ${result.duplicates.length} potential duplicates, ${result.solutions.length} solutions, ${result.related.length} related issues`);

            // Log the search
            await this.logDuplicateSearch(issueNumber, result);

            return result;

        } catch (error) {
            console.error('❌ Error finding duplicates:', error);
            return {
                duplicates: [],
                solutions: [],
                related: [],
                searchPerformed: false,
                error: error.message
            };
        }
    }

    async searchSimilarIssues(content) {
        try {
            const issues = [];

            // Search in open issues (recent)
            const since = new Date();
            since.setDate(since.getDate() - this.searchWindowDays);

            const { data: openIssues } = await this.octokit.issues.listForRepo({
                owner: this.owner,
                repo: this.repo,
                state: 'open',
                since: since.toISOString(),
                per_page: 50
            });

            issues.push(...openIssues.filter(i => !i.pull_request));

            // Search in closed issues (broader search)
            const { data: closedIssues } = await this.octokit.issues.listForRepo({
                owner: this.owner,
                repo: this.repo,
                state: 'closed',
                per_page: 50,
                sort: 'updated',
                direction: 'desc'
            });

            issues.push(...closedIssues.filter(i => !i.pull_request));

            // Extract keywords for better matching
            const keywords = this.extractKeywords(content);

            // Filter and score issues based on keyword matches
            const scoredIssues = issues.map(issue => ({
                ...issue,
                keywordMatches: this.countKeywordMatches(keywords, `${issue.title} ${issue.body || ''}`)
            }));

            // Sort by keyword matches and recency
            scoredIssues.sort((a, b) => {
                if (a.keywordMatches !== b.keywordMatches) {
                    return b.keywordMatches - a.keywordMatches;
                }
                // If same keyword matches, prefer more recent
                return new Date(b.updated_at) - new Date(a.updated_at);
            });

            return scoredIssues.slice(0, this.maxResults);

        } catch (error) {
            console.warn('⚠️ Could not search issues:', error.message);
            return [];
        }
    }

    calculateSimilarity(text1, text2) {
        // Simple Jaccard similarity based on word overlap
        const words1 = new Set(text1.toLowerCase().split(/\W+/).filter(w => w.length > 2));
        const words2 = new Set(text2.toLowerCase().split(/\W+/).filter(w => w.length > 2));

        const intersection = new Set([...words1].filter(x => words2.has(x)));
        const union = new Set([...words1, ...words2]);

        return intersection.size / union.size;
    }

    extractKeywords(text) {
        // Extract important keywords, removing stop words
        const stopWords = new Set([
            'the', 'a', 'an', 'and', 'or', 'but', 'in', 'on', 'at', 'to', 'for',
            'of', 'with', 'by', 'is', 'are', 'was', 'were', 'be', 'been', 'being',
            'have', 'has', 'had', 'do', 'does', 'did', 'will', 'would', 'could',
            'should', 'may', 'might', 'must', 'can', 'this', 'that', 'these',
            'those', 'i', 'me', 'my', 'myself', 'we', 'our', 'ours', 'you', 'your',
            'yours', 'he', 'him', 'his', 'she', 'her', 'hers', 'it', 'its', 'they',
            'them', 'their', 'theirs', 'what', 'which', 'who', 'when', 'where',
            'why', 'how', 'all', 'any', 'both', 'each', 'few', 'more', 'most',
            'other', 'some', 'such', 'no', 'nor', 'not', 'only', 'own', 'same',
            'so', 'than', 'too', 'very', 'just', 'now'
        ]);

        const words = text.toLowerCase().split(/\W+/).filter(word =>
            word.length > 2 && !stopWords.has(word)
        );

        // Count frequency and return top keywords
        const frequency = {};
        words.forEach(word => {
            frequency[word] = (frequency[word] || 0) + 1;
        });

        return Object.entries(frequency)
            .sort(([,a], [,b]) => b - a)
            .slice(0, 20)
            .map(([word]) => word);
    }

    countKeywordMatches(keywords, text) {
        const textWords = new Set(text.toLowerCase().split(/\W+/));
        return keywords.filter(keyword => textWords.has(keyword)).length;
    }

    async issueHasSolution(issueNumber) {
        try {
            // Check if the issue has comments indicating a solution
            const { data: comments } = await this.octokit.issues.listComments({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber,
                per_page: 10
            });

            // Look for solution indicators in comments
            for (const comment of comments) {
                const body = comment.body.toLowerCase();
                if (body.includes('fixed') || body.includes('resolved') ||
                    body.includes('solution') || body.includes('workaround') ||
                    body.includes('here\'s how') || body.includes('this was caused by')) {
                    return {
                        found: true,
                        comment: comment.body.substring(0, 300) + (comment.body.length > 300 ? '...' : ''),
                        author: comment.user.login,
                        url: comment.html_url
                    };
                }
            }

            return null;

        } catch (error) {
            console.warn(`⚠️ Could not check solution for issue #${issueNumber}:`, error.message);
            return null;
        }
    }

    async logDuplicateSearch(issueNumber, result) {
        const logEntry = {
            timestamp: new Date().toISOString(),
            issueNumber: issueNumber,
            duplicatesFound: result.duplicates.length,
            solutionsFound: result.solutions.length,
            relatedFound: result.related.length,
            totalSearched: result.totalFound,
            searchPerformed: result.searchPerformed
        };

        const logPath = path.join(__dirname, '..', '.ai', 'logs', 'duplicate-searches.jsonl');

        try {
            await fs.appendFile(logPath, JSON.stringify(logEntry) + '\n');
        } catch (error) {
            console.warn('⚠️ Could not write duplicate search log:', error.message);
        }
    }
}

// Main execution
async function main() {
    const [,, issueNumber, title, body] = process.argv;

    if (!issueNumber || !title) {
        console.error('Usage: node find-duplicates.js <issue-number> <title> <body>');
        process.exit(1);
    }

    const finder = new DuplicateFinder();
    const result = await finder.findDuplicatesAndSolutions(parseInt(issueNumber), title, body || '');

    // Output for GitHub Actions
    console.log(`::set-output name=duplicates::${JSON.stringify(result.duplicates)}`);
    console.log(`::set-output name=solutions::${JSON.stringify(result.solutions)}`);
    console.log(`::set-output name=related::${JSON.stringify(result.related)}`);
    console.log(`::set-output name=search_performed::${result.searchPerformed}`);
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = DuplicateFinder;