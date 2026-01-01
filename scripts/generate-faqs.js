#!/usr/bin/env node

/**
 * B2Connect FAQ Generation Script
 * Automatically generates FAQ candidates from process documentation
 */

const fs = require('fs');
const path = require('path');

class FAQGenerator {
    constructor() {
        this.baseDir = path.join(__dirname, '..', '..');
        this.faqDir = path.join(this.baseDir, '.ai', 'faq');
        this.candidatesDir = path.join(this.faqDir, 'candidates');
    }

    /**
     * Scan documentation for potential FAQ content
     */
    scanDocumentation() {
        const scanPaths = [
            '.ai/workflows',
            '.ai/operations',
            '.ai/collaboration',
            '.ai/knowledgebase',
            '.github/instructions'
        ];

        const candidates = [];

        scanPaths.forEach(scanPath => {
            const fullPath = path.join(this.baseDir, scanPath);
            if (fs.existsSync(fullPath)) {
                this.scanDirectory(fullPath, candidates, scanPath);
            }
        });

        return candidates;
    }

    /**
     * Recursively scan directory for documentation files
     */
    scanDirectory(dirPath, candidates, relativePath) {
        const items = fs.readdirSync(dirPath);

        items.forEach(item => {
            const fullPath = path.join(dirPath, item);
            const stat = fs.statSync(fullPath);

            if (stat.isDirectory()) {
                this.scanDirectory(fullPath, candidates, path.join(relativePath, item));
            } else if (item.endsWith('.md')) {
                const content = fs.readFileSync(fullPath, 'utf8');
                const fileCandidates = this.extractCandidates(content, path.join(relativePath, item));
                candidates.push(...fileCandidates);
            }
        });
    }

    /**
     * Extract potential FAQ questions from content
     */
    extractCandidates(content, sourceFile) {
        const candidates = [];

        // Improved question patterns
        const questionPatterns = [
            // Direct questions
            /\b(How|What|Why|When|Where|Who|Can|Do|Does|Is|Are|Will|Should)\b[^?!.]*[?]/gi,
            // Common FAQ patterns
            /(?:Question|FAQ|Help|Guide):\s*([^.\n?]+[?])/gi,
            // Section headers that are questions
            /^##+\s*(.+[?])\s*$/gm,
            // Process steps that imply questions
            /(?:Step|To|For)\s+\w+.*:/gi,
            // Troubleshooting patterns
            /(?:Issue|Problem|Error|Fix|Resolve):\s*([^.\n]+)/gi
        ];

        // Also look for imperative statements that could be questions
        const imperativePatterns = [
            /\b(Create|Set up|Configure|Install|Deploy|Monitor|Manage|Update|Change|Reset)\b[^.]*\./gi,
            /\b(Enable|Disable|Add|Remove|Modify|Access|Login|Register)\b[^.]*\./gi
        ];

        // Process question patterns
        questionPatterns.forEach(pattern => {
            let match;
            while ((match = pattern.exec(content)) !== null) {
                const question = this.cleanQuestion(match[1] || match[0]);
                if (question && question.length > 10) { // Minimum length filter
                    const context = this.extractContext(content, match.index);

                    candidates.push({
                        question: question,
                        context: context,
                        sourceFile: sourceFile,
                        extractedAt: new Date().toISOString(),
                        stakeholderGroup: this.inferStakeholderGroup(sourceFile, question),
                        confidence: this.calculateConfidence(question, context)
                    });
                }
            }
        });

        // Process imperative patterns and convert to questions
        imperativePatterns.forEach(pattern => {
            let match;
            while ((match = pattern.exec(content)) !== null) {
                const statement = match[0];
                const question = this.imperativeToQuestion(statement);
                if (question && question.length > 10) {
                    const context = this.extractContext(content, match.index);

                    candidates.push({
                        question: question,
                        context: context,
                        sourceFile: sourceFile,
                        extractedAt: new Date().toISOString(),
                        stakeholderGroup: this.inferStakeholderGroup(sourceFile, question),
                        confidence: 0.7 // Lower confidence for converted questions
                    });
                }
            }
        });

        // Remove duplicates
        const uniqueCandidates = candidates.filter((candidate, index, self) =>
            index === self.findIndex(c => c.question === candidate.question)
        );

        return uniqueCandidates;
    }

    /**
     * Clean and normalize question text
     */
    cleanQuestion(question) {
        return question
            .trim()
            .replace(/^[-•*]\s*/, '') // Remove bullets
            .replace(/\s+/g, ' ') // Normalize whitespace
            .replace(/:$/, '?') // Convert colons to question marks
            .replace(/^[a-z]/, l => l.toUpperCase()); // Capitalize first letter
    }

    /**
     * Convert imperative statements to questions
     */
    imperativeToQuestion(statement) {
        const cleaned = statement.replace(/\.$/, '').trim();

        // Common conversions
        const conversions = [
            [/^Create (.+)/, 'How do I create $1?'],
            [/^Set up (.+)/, 'How do I set up $1?'],
            [/^Configure (.+)/, 'How do I configure $1?'],
            [/^Install (.+)/, 'How do I install $1?'],
            [/^Deploy (.+)/, 'How do I deploy $1?'],
            [/^Monitor (.+)/, 'How do I monitor $1?'],
            [/^Manage (.+)/, 'How do I manage $1?'],
            [/^Update (.+)/, 'How do I update $1?'],
            [/^Change (.+)/, 'How do I change $1?'],
            [/^Reset (.+)/, 'How do I reset $1?'],
            [/^Enable (.+)/, 'How do I enable $1?'],
            [/^Disable (.+)/, 'How do I disable $1?'],
            [/^Add (.+)/, 'How do I add $1?'],
            [/^Remove (.+)/, 'How do I remove $1?'],
            [/^Access (.+)/, 'How do I access $1?']
        ];

        for (const [pattern, template] of conversions) {
            const match = cleaned.match(pattern);
            if (match) {
                return template.replace('$1', match[1]);
            }
        }

        // Fallback: add "How do I" prefix
        if (!cleaned.match(/^(How|What|Why|When|Where)/i)) {
            return `How do I ${cleaned.toLowerCase()}?`;
        }

        return cleaned + '?';
    }

    /**
     * Calculate confidence score for candidate
     */
    calculateConfidence(question, context) {
        let confidence = 0.5; // Base confidence

        // Increase confidence for clear question words
        if (question.match(/^(How|What|Why|When|Where|Who|Can|Do|Does|Is|Are)/i)) {
            confidence += 0.3;
        }

        // Increase for question marks
        if (question.includes('?')) {
            confidence += 0.2;
        }

        // Increase for technical context
        if (context.toLowerCase().includes('error') ||
            context.toLowerCase().includes('issue') ||
            context.toLowerCase().includes('problem')) {
            confidence += 0.1;
        }

        // Decrease for very short questions
        if (question.length < 15) {
            confidence -= 0.2;
        }

        return Math.min(1.0, Math.max(0.0, confidence));
    }

    /**
     * Extract context around the matched text
     */
    extractContext(content, index, contextLength = 200) {
        const start = Math.max(0, index - contextLength);
        const end = Math.min(content.length, index + contextLength);
        return content.substring(start, end).replace(/\n/g, ' ').trim();
    }

    /**
     * Infer stakeholder group based on file path and content
     */
    inferStakeholderGroup(sourceFile, question) {
        const filePath = sourceFile.toLowerCase();
        const questionText = question.toLowerCase();

        if (filePath.includes('admin') || questionText.includes('admin')) {
            return 'administrators';
        } else if (filePath.includes('devops') || filePath.includes('infrastructure') || questionText.includes('deploy')) {
            return 'devops';
        } else if (filePath.includes('marketing') || filePath.includes('sales') || questionText.includes('pricing')) {
            return 'prospects';
        } else {
            return 'end-users';
        }
    }

    /**
     * Generate FAQ answer from context and source
     */
    generateAnswer(candidate) {
        // This is a simplified answer generation
        // In a real implementation, this would use AI/ML to generate comprehensive answers
        const sourceContent = fs.readFileSync(path.join(this.baseDir, candidate.sourceFile), 'utf8');

        // Find the section containing the question
        const lines = sourceContent.split('\n');
        const questionIndex = lines.findIndex(line =>
            line.toLowerCase().includes(candidate.question.toLowerCase().split(' ')[0])
        );

        if (questionIndex !== -1) {
            // Extract the next few paragraphs as the answer
            const answerLines = [];
            for (let i = questionIndex + 1; i < Math.min(lines.length, questionIndex + 10); i++) {
                if (lines[i].trim() === '' || lines[i].startsWith('#')) break;
                answerLines.push(lines[i]);
            }

            return answerLines.join(' ').trim();
        }

        return candidate.context; // Fallback to context
    }

    /**
     * Process candidates into draft FAQs
     */
    processCandidates() {
        const candidates = this.scanDocumentation();
        const drafts = [];

        candidates.forEach((candidate, index) => {
            const answer = this.generateAnswer(candidate);

            const faq = {
                id: `faq_${Date.now()}_${index}`,
                category: this.inferCategory(candidate.question),
                stakeholderGroup: candidate.stakeholderGroup,
                question: candidate.question,
                answer: answer,
                tags: this.extractTags(candidate.question),
                sourceDocuments: [candidate.sourceFile],
                lastUpdated: candidate.extractedAt,
                reviewCycle: 'monthly',
                accessCount: 0,
                satisfactionScore: null,
                status: 'draft'
            };

            drafts.push(faq);
        });

        return drafts;
    }

    /**
     * Infer FAQ category from question
     */
    inferCategory(question) {
        const q = question.toLowerCase();

        if (q.includes('account') || q.includes('login') || q.includes('password')) {
            return 'account_management';
        } else if (q.includes('order') || q.includes('cart') || q.includes('checkout')) {
            return 'shopping_process';
        } else if (q.includes('deploy') || q.includes('infrastructure') || q.includes('monitor')) {
            return 'infrastructure';
        } else if (q.includes('security') || q.includes('compliance') || q.includes('gdpr')) {
            return 'security_compliance';
        } else {
            return 'general';
        }
    }

    /**
     * Extract tags from question
     */
    extractTags(question) {
        const tags = [];
        const q = question.toLowerCase();

        if (q.includes('how')) tags.push('how-to');
        if (q.includes('what')) tags.push('explanation');
        if (q.includes('why')) tags.push('reasoning');
        if (q.includes('security')) tags.push('security');
        if (q.includes('deploy')) tags.push('deployment');

        return tags;
    }

    /**
     * Save candidates to files
     */
    saveCandidates() {
        const candidates = this.scanDocumentation();

        if (!fs.existsSync(this.candidatesDir)) {
            fs.mkdirSync(this.candidatesDir, { recursive: true });
        }

        const outputFile = path.join(this.candidatesDir, `candidates_${new Date().toISOString().split('T')[0]}.json`);
        fs.writeFileSync(outputFile, JSON.stringify(candidates, null, 2));

        console.log(`Generated ${candidates.length} FAQ candidates and saved to ${outputFile}`);
        return candidates.length;
    }

    /**
     * Save processed drafts
     */
    saveDrafts() {
        const drafts = this.processCandidates();
        const draftsDir = path.join(this.faqDir, 'drafts');

        if (!fs.existsSync(draftsDir)) {
            fs.mkdirSync(draftsDir, { recursive: true });
        }

        const outputFile = path.join(draftsDir, `drafts_${new Date().toISOString().split('T')[0]}.json`);
        fs.writeFileSync(outputFile, JSON.stringify(drafts, null, 2));

        console.log(`Generated ${drafts.length} FAQ drafts and saved to ${outputFile}`);
        return drafts.length;
    }
}

// CLI interface
if (require.main === module) {
    const generator = new FAQGenerator();
    const command = process.argv[2];

    switch (command) {
        case 'scan':
            generator.saveCandidates();
            break;
        case 'process':
            generator.saveDrafts();
            break;
        case 'full':
            generator.saveCandidates();
            generator.saveDrafts();
            break;
        default:
            console.log('Usage: node generate-faqs.js [scan|process|full]');
            console.log('  scan: Extract FAQ candidates from documentation');
            console.log('  process: Generate FAQ drafts from candidates');
            console.log('  full: Run both scan and process');
    }
}

module.exports = FAQGenerator;