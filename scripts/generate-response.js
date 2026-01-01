#!/usr/bin/env node

const { Octokit } = require('@octokit/rest');
const fs = require('fs').promises;
const path = require('path');

class ResponseGenerator {
    constructor() {
        this.octokit = new Octokit({
            auth: process.env.GITHUB_TOKEN
        });

        this.owner = process.env.GITHUB_REPOSITORY.split('/')[0];
        this.repo = process.env.GITHUB_REPOSITORY.split('/')[1];
    }

    async generateResponse(issueNumber, category, templateDataJson, duplicatesJson, solutionsJson, relatedJson, shouldClose = false) {
        try {
            console.log(`💬 Generating response for issue #${issueNumber} (${category})`);

            const templateData = JSON.parse(templateDataJson);
            const duplicates = JSON.parse(duplicatesJson || '[]');
            const solutions = JSON.parse(solutionsJson || '[]');
            const related = JSON.parse(relatedJson || '[]');

            const response = this.buildResponse(category, templateData, { duplicates, solutions, related });

            // Post comment on the issue
            await this.octokit.issues.createComment({
                owner: this.owner,
                repo: this.repo,
                issue_number: issueNumber,
                body: response
            });

            console.log(`✅ Response posted to issue #${issueNumber}`);

            // Close the issue if requested
            if (shouldClose) {
                await this.octokit.issues.update({
                    owner: this.owner,
                    repo: this.repo,
                    issue_number: issueNumber,
                    state: 'closed'
                });
                console.log(`🔒 Issue #${issueNumber} closed as non-product related`);
            }

            // Log the response (optional)
            try {
                await this.logResponse(issueNumber, category, response, shouldClose);
            } catch (error) {
                console.warn('⚠️ Could not write response log:', error.message);
            }

        } catch (error) {
            console.error('❌ Error generating response:', error);
            throw error;
        }
    }

    buildResponse(category, data, references = {}) {
        const templates = {
            bug: this.buildBugResponse(data, references),
            feature: this.buildFeatureResponse(data, references),
            change: this.buildChangeResponse(data, references),
            knowhow: this.buildKnowhowResponse(data, references),
            nonsense: this.buildNonsenseResponse(data, references),
            'non-product': this.buildNonProductResponse(data, references)
        };

        return templates[category] || this.buildDefaultResponse(data, references);
    }

    buildBugResponse(data, references = {}) {
        const sla = this.getSLATime(data.priority);
        const priorityEmoji = this.getPriorityEmoji(data.priority);

        let duplicateSection = '';
        if (references.duplicates && references.duplicates.length > 0) {
            duplicateSection = `\n\n## ⚠️ Mögliche Duplikate gefunden\n${this.formatDuplicates(references.duplicates)}\n`;
        }

        let solutionSection = '';
        if (references.solutions && references.solutions.length > 0) {
            solutionSection = `\n\n## ✅ Bereits bekannte Lösungen\n${this.formatSolutions(references.solutions)}\n`;
        }

        return `🐛 **Bug Report Acknowledged**

Thank you for reporting this issue! Our QA team has been notified and will investigate.${duplicateSection}${solutionSection}

**Issue Summary:**
- **Priority:** ${priorityEmoji} ${data.priority}
- **Environment:** ${data.environment}
- **Version:** ${data.version}
- **Impact:** ${data.impact}

**Next Steps:**
1. **Reproduction** - We'll attempt to reproduce the issue ${data.reproduction !== 'not provided' ? 'using your steps' : 'and may request additional details'}
2. **Investigation** - Root cause analysis within ${sla}
3. **Fix Development** - Solution implementation and testing
4. **Resolution** - Deployment and verification

**Required Information (if not provided):**
- Exact B2Connect version and environment details
- Browser/OS information for UI issues
- Console logs or error messages
- Screenshots if applicable

**Service Level Agreement:** ${sla} response time

---

**For urgent production issues:** Contact urgent-support@b2connect.com
**For general support:** Visit our [documentation](https://docs.b2connect.com) or [community forum](https://community.b2connect.com)

*This issue has been automatically categorized and routed to our QA team.*`;
    }

    buildFeatureResponse(data, references = {}) {
        const priorityEmoji = this.getPriorityEmoji(data.priority);

        let duplicateSection = '';
        if (references.duplicates && references.duplicates.length > 0) {
            duplicateSection = `\n\n## 📋 Ähnliche Feature-Requests\n${this.formatDuplicates(references.duplicates)}\n`;
        }

        let solutionSection = '';
        if (references.solutions && references.solutions.length > 0) {
            duplicateSection += `\n\n## 💡 Bereits implementierte Features\n${this.formatSolutions(references.solutions)}\n`;
        }

        return `✨ **Feature Request Received**

Thank you for your feature suggestion! This has been forwarded to our Product team for evaluation.${duplicateSection}${solutionSection}

**Request Analysis:**
- **Type:** Feature Enhancement
- **Priority Assessment:** ${priorityEmoji} ${data.priority}
- **Business Impact:** ${data.impact}
- **Topic Area:** ${data.topic}

**Evaluation Process:**
1. **Business Value Assessment** - Alignment with product roadmap (5 business days)
2. **Technical Feasibility** - Implementation complexity analysis
3. **Priority Ranking** - Comparison with existing feature backlog
4. **Timeline Planning** - Potential inclusion in future releases

**To help us evaluate your request:**
- What specific business problem does this solve?
- How many users would benefit from this feature?
- Are there similar features in competing products?
- What's your timeline expectation?

**Response Timeline:** Initial assessment within 5 business days

---

**Track this request:** We'll update this issue with our evaluation results
**Product roadmap:** Check our [public roadmap](https://roadmap.b2connect.com) for planned features
**Community voting:** Similar requests may be grouped for community voting

*This feature request has been automatically categorized and added to our product evaluation queue.*`;
    }

    buildChangeResponse(data) {
        const priorityEmoji = this.getPriorityEmoji(data.priority);

        return `🔄 **Change Request Submitted**

Thank you for your change request. This will undergo formal change control review.

**Change Summary:**
- **Type:** ${data.category} Change
- **Priority:** ${priorityEmoji} ${data.priority}
- **Scope:** ${data.impact}
- **Environment:** ${data.environment}
- **Topic:** ${data.topic}

**Change Management Process:**
1. **Impact Analysis** - Technical and business impact assessment
2. **Risk Evaluation** - Potential disruption and mitigation strategies
3. **Approval Review** - Change Control Board evaluation
4. **Implementation Planning** - Rollout with rollback procedures

**Required Documentation:**
- Detailed change description and business justification
- Affected systems, components, and user groups
- Implementation timeline and resource requirements
- Testing and validation plan
- Rollback procedures and success criteria

**Review Timeline:** 3-5 business days for initial assessment

---

**For urgent changes:** Contact change-control@b2connect.com
**Change policy:** Review our [change management guidelines](https://docs.b2connect.com/change-management)
**Status updates:** We'll provide regular updates on this issue

*This change request has been automatically categorized and routed to our Change Control Board.*`;
    }

    buildKnowhowResponse(data, references = {}) {
        const priorityEmoji = this.getPriorityEmoji(data.urgency);

        let solutionSection = '';
        if (references.solutions && references.solutions.length > 0) {
            solutionSection = `\n\n## 📚 Bereits dokumentierte Lösungen\n${this.formatSolutions(references.solutions)}\n`;
        }

        let relatedSection = '';
        if (references.related && references.related.length > 0) {
            relatedSection = `\n\n## 🔗 Verwandte Fragen\n${this.formatRelated(references.related)}\n`;
        }

        return `📚 **Knowledge Request Received**

Thank you for your question! We're here to help with your B2Connect knowledge needs.${solutionSection}${relatedSection}

**Question Analysis:**
- **Category:** ${data.category}
- **Priority:** ${priorityEmoji} ${data.urgency}
- **Topic Area:** ${data.topic}

**Immediate Resources:**
${this.generateRelevantLinks(data)}

**If these resources don't fully answer your question:**
- Our Developer Relations team will follow up within 24 hours
- Check our [complete documentation](https://docs.b2connect.com)
- Join our [community forum](https://community.b2connect.com) for peer support
- Schedule a [support call](https://support.b2connect.com) for personalized assistance

**Help us improve:** Was this helpful? [Yes/No feedback link]

---

**For urgent technical issues:** Contact technical-support@b2connect.com
**For account/billing questions:** Visit our [help center](https://help.b2connect.com)
**For training resources:** Check our [learning portal](https://learn.b2connect.com)

*This knowledge request has been automatically categorized and routed to our support team.*`;
    }

    buildNonsenseResponse(data, references = {}) {
        return `🤔 **Nicht-produktbezogene Anfrage**

Vielen Dank für Ihre Nachricht! Es scheint, als ob diese Anfrage nicht direkt mit B2Connect zusammenhängt.

**Hinweis:** Dieses Issue wurde automatisch als nicht produktbezogen eingestuft und wird daher geschlossen.

Falls Sie doch eine Frage zu B2Connect haben, zögern Sie nicht, ein neues Issue mit spezifischen Details zu erstellen. Wir helfen Ihnen gerne weiter!

**Nützliche Links:**
- [B2Connect Dokumentation](https://docs.b2connect.com)
- [Community Forum](https://community.b2connect.com)
- [Support Kontakt](https://support.b2connect.com)

Bei Fragen zu unserem Produkt stehen wir Ihnen jederzeit zur Verfügung.

*Diese Anfrage wurde automatisch verarbeitet und als nicht relevant für B2Connect eingestuft.*`;
    }

    buildNonProductResponse(data, references = {}) {
        return `❓ **Anfrage ohne klaren Produktbezug**

Vielen Dank für Ihre Nachricht! Leider konnten wir in Ihrer Anfrage keinen eindeutigen Bezug zu B2Connect erkennen.

**Wichtiger Hinweis:** Um Ihnen bestmöglich helfen zu können, benötigen wir spezifische Informationen zu unserem Produkt. Dieses Issue wird daher geschlossen.

**Für B2Connect-spezifische Fragen bitte angeben:**
- Welches B2Connect-Modul betreffen Sie? (API, Frontend, Backend, Datenbank, etc.)
- Welche Technologie verwenden Sie? (ASP.NET, Vue.js, CQRS, etc.)
- Welche Version von B2Connect nutzen Sie?
- Können Sie das Problem/die Frage genauer beschreiben?

**Nützliche Links:**
- [B2Connect Produktübersicht](https://docs.b2connect.com/overview)
- [Erste Schritte](https://docs.b2connect.com/getting-started)
- [API Dokumentation](https://docs.b2connect.com/api)
- [Support Kontakt](https://support.b2connect.com)

Falls Sie eine B2Connect-bezogene Frage haben, erstellen Sie bitte ein neues Issue mit diesen Details. Wir freuen uns auf Ihre Rückmeldung!

*Diese Anfrage wurde automatisch als nicht eindeutig produktbezogen eingestuft und geschlossen.*`;
    }

    buildDefaultResponse(data, references = {}) {
        let duplicateSection = '';
        if (references.duplicates && references.duplicates.length > 0) {
            duplicateSection = `\n\n## ⚠️ Ähnliche Issues gefunden\n${this.formatDuplicates(references.duplicates)}\n`;
        }

        return `📋 **Issue Received**

Thank you for your submission! We've received your issue and will review it shortly.${duplicateSection}

**Issue Details:**
- **Category:** ${data.category || 'General'}
- **Priority:** ${data.priority || 'Medium'}
- **Topic:** ${data.topic || 'General'}

Our team will review this and respond with next steps.

For immediate assistance, please visit our [documentation](https://docs.b2connect.com) or contact support@b2connect.com.

*This issue has been automatically processed and is pending human review.*`;
    }

    generateRelevantLinks(data) {
        const links = [];

        // Generate contextual links based on topic
        const linkMap = {
            api: [
                '- [API Documentation](https://docs.b2connect.com/api)',
                '- [API Examples](https://docs.b2connect.com/api/examples)',
                '- [Authentication Guide](https://docs.b2connect.com/api/auth)'
            ],
            ui: [
                '- [UI Components Guide](https://docs.b2connect.com/ui)',
                '- [Styling Guide](https://docs.b2connect.com/ui/styling)',
                '- [Accessibility Guidelines](https://docs.b2connect.com/ui/accessibility)'
            ],
            database: [
                '- [Database Schema](https://docs.b2connect.com/database)',
                '- [Data Migration Guide](https://docs.b2connect.com/database/migration)',
                '- [Query Optimization](https://docs.b2connect.com/database/optimization)'
            ],
            authentication: [
                '- [Authentication Guide](https://docs.b2connect.com/auth)',
                '- [User Management](https://docs.b2connect.com/auth/users)',
                '- [Security Best Practices](https://docs.b2connect.com/auth/security)'
            ],
            performance: [
                '- [Performance Guide](https://docs.b2connect.com/performance)',
                '- [Monitoring Tools](https://docs.b2connect.com/performance/monitoring)',
                '- [Optimization Tips](https://docs.b2connect.com/performance/tips)'
            ],
            integration: [
                '- [Integration Guide](https://docs.b2connect.com/integrations)',
                '- [Webhooks](https://docs.b2connect.com/integrations/webhooks)',
                '- [Third-party Connectors](https://docs.b2connect.com/integrations/connectors)'
            ],
            setup: [
                '- [Setup Guide](https://docs.b2connect.com/setup)',
                '- [Installation](https://docs.b2connect.com/setup/install)',
                '- [Configuration](https://docs.b2connect.com/setup/config)'
            ],
            configuration: [
                '- [Configuration Guide](https://docs.b2connect.com/config)',
                '- [Environment Variables](https://docs.b2connect.com/config/env)',
                '- [Settings Reference](https://docs.b2connect.com/config/settings)'
            ]
        };

        const topicLinks = linkMap[data.topic] || [
            '- [General Documentation](https://docs.b2connect.com)',
            '- [Getting Started](https://docs.b2connect.com/getting-started)',
            '- [FAQ](https://docs.b2connect.com/faq)'
        ];

        return topicLinks.join('\n');
    }

    getSLATime(priority) {
        const slas = {
            critical: '2 hours',
            high: '4 hours',
            medium: '24 hours',
            low: '48 hours'
        };
        return slas[priority] || '24 hours';
    }

    getPriorityEmoji(priority) {
        const emojis = {
            critical: '🔴',
            high: '🟠',
            medium: '🟡',
            low: '🟢'
        };
        return emojis[priority] || '🟡';
    }

    formatDuplicates(duplicates) {
        if (!duplicates || duplicates.length === 0) return '';

        return duplicates.map(duplicate =>
            `🔗 **[#${duplicate.number}](${duplicate.html_url})** - ${duplicate.title.substring(0, 80)}${duplicate.title.length > 80 ? '...' : ''}\n` +
            `   💡 Ähnlichkeit: ${(duplicate.similarity * 100).toFixed(0)}% | Status: ${duplicate.state === 'open' ? '🟢 Offen' : '🔒 Geschlossen'}`
        ).join('\n');
    }

    formatSolutions(solutions) {
        if (!solutions || solutions.length === 0) return '';

        return solutions.map(solution =>
            `✅ **[#${solution.number}](${solution.html_url})** - ${solution.title.substring(0, 80)}${solution.title.length > 80 ? '...' : ''}\n` +
            `   💡 Ähnlichkeit: ${(solution.similarity * 100).toFixed(0)}% | Lösung: ${solution.solution?.comment?.substring(0, 100) || 'Siehe Issue für Details'}${solution.solution?.comment?.length > 100 ? '...' : ''}`
        ).join('\n');
    }

    formatRelated(related) {
        if (!related || related.length === 0) return '';

        return related.map(item =>
            `🔗 **[#${item.number}](${item.html_url})** - ${item.title.substring(0, 80)}${item.title.length > 80 ? '...' : ''}\n` +
            `   💡 Ähnlichkeit: ${(item.similarity * 100).toFixed(0)}%`
        ).join('\n');
    }
}

    async logResponse(issueNumber, category, response, shouldClose = false) {
        const logEntry = {
            timestamp: new Date().toISOString(),
            issueNumber: issueNumber,
            category: category,
            responseLength: response.length,
            responsePreview: response.substring(0, 200) + '...',
            closed: shouldClose
        };

        const logPath = path.join(__dirname, '..', '.ai', 'logs', 'responses.jsonl');

        try {
            await fs.appendFile(logPath, JSON.stringify(logEntry) + '\n');
        } catch (error) {
            console.warn('⚠️ Could not write response log:', error.message);
        }
    }
}

// Main execution
async function main() {
    const [,, issueNumber, category, templateData, duplicates, solutions, related, shouldClose] = process.argv;

    if (!issueNumber || !category || !templateData) {
        console.error('Usage: node generate-response.js <issue-number> <category> <template-data-json> [duplicates] [solutions] [related] [should-close]');
        process.exit(1);
    }

    const generator = new ResponseGenerator();
    await generator.generateResponse(parseInt(issueNumber), category, templateData, duplicates, solutions, related, shouldClose === 'true');
}

if (require.main === module) {
    main().catch(console.error);
}

module.exports = ResponseGenerator;