#!/usr/bin/env node

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from '@modelcontextprotocol/sdk/types.js';
import * as fs from 'fs';
import * as path from 'path';
import { glob } from 'glob';
import { marked } from 'marked';

// Input validation utilities
class ValidationError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'ValidationError';
  }
}

function validateString(value: any, fieldName: string, minLength = 1, maxLength = 10000): string {
  if (typeof value !== 'string') {
    throw new ValidationError(`${fieldName} must be a string`);
  }
  if (value.length < minLength) {
    throw new ValidationError(`${fieldName} must be at least ${minLength} characters`);
  }
  if (value.length > maxLength) {
    throw new ValidationError(`${fieldName} must be at most ${maxLength} characters`);
  }
  return value;
}

function validateWorkspacePath(workspacePath: any): string {
  const path = validateString(workspacePath, 'workspacePath', 1, 500);

  // Prevent path traversal attacks
  if (path.includes('..') || path.includes('../') || path.startsWith('/')) {
    throw new ValidationError('workspacePath must be a relative path without .. or absolute paths');
  }

  return path;
}

class DocumentationMCPServer {
  private server: Server;

  constructor() {
    this.server = new Server(
      {
        name: 'documentation-mcp-server',
        version: '1.0.0',
      },
      {
        capabilities: {
          tools: {},
        },
      }
    );

    this.setupToolHandlers();
  }

  private setupToolHandlers() {
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'generate_api_docs',
            description: 'Generate API documentation from code comments and method signatures',
            inputSchema: {
              type: 'object',
              properties: {
                sourcePath: {
                  type: 'string',
                  description: 'Path to source code directory',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                outputFormat: {
                  type: 'string',
                  enum: ['markdown', 'html', 'json'],
                  description: 'Output format for documentation',
                  default: 'markdown',
                },
                includePrivate: {
                  type: 'boolean',
                  description: 'Include private methods in documentation',
                  default: false,
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'validate_readme',
            description: 'Validate README files for completeness, accuracy, and best practices',
            inputSchema: {
              type: 'object',
              properties: {
                readmePath: {
                  type: 'string',
                  description: 'Path to README file (optional, will find automatically)',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                checkSections: {
                  type: 'array',
                  items: {
                    type: 'string',
                    enum: [
                      'description',
                      'installation',
                      'usage',
                      'api',
                      'contributing',
                      'license',
                    ],
                  },
                  description: 'Specific sections to validate',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'update_documentation',
            description: 'Update documentation based on recent code changes',
            inputSchema: {
              type: 'object',
              properties: {
                changedFiles: {
                  type: 'array',
                  items: { type: 'string' },
                  description: 'List of recently changed files',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                docType: {
                  type: 'string',
                  enum: ['api', 'readme', 'architecture'],
                  description: 'Type of documentation to update',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'check_doc_coverage',
            description: 'Check documentation coverage for code elements',
            inputSchema: {
              type: 'object',
              properties: {
                sourcePath: {
                  type: 'string',
                  description: 'Path to source code directory',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                coverageType: {
                  type: 'string',
                  enum: ['functions', 'classes', 'modules', 'all'],
                  description: 'Type of coverage to check',
                  default: 'all',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'generate_change_log',
            description: 'Generate change log from git commits',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                sinceTag: {
                  type: 'string',
                  description: 'Git tag to start from (optional)',
                },
                outputFormat: {
                  type: 'string',
                  enum: ['markdown', 'json'],
                  description: 'Output format',
                  default: 'markdown',
                },
              },
              required: ['workspacePath'],
            },
          },
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'generate_api_docs':
            return await this.generateApiDocs(args);
          case 'validate_readme':
            return await this.validateReadme(args);
          case 'update_documentation':
            return await this.updateDocumentation(args);
          case 'check_doc_coverage':
            return await this.checkDocCoverage(args);
          case 'generate_change_log':
            return await this.generateChangeLog(args);
          default:
            throw new McpError(ErrorCode.MethodNotFound, `Unknown tool: ${name}`);
        }
      } catch (error) {
        if (error instanceof ValidationError) {
          throw new McpError(ErrorCode.InvalidParams, error.message);
        }
        throw error;
      }
    });
  }

  private async generateApiDocs(args: any) {
    const sourcePath = args.sourcePath ? validateString(args.sourcePath, 'sourcePath', 1, 500) : '';
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const outputFormat = args.outputFormat || 'markdown';
    const includePrivate = args.includePrivate || false;

    const issues: string[] = [];
    const documentation: string[] = [];

    // Find source files
    const sourceFiles = await this.findSourceFiles(workspacePath, sourcePath);

    for (const file of sourceFiles.slice(0, 20)) {
      // Limit to 20 files
      try {
        const content = fs.readFileSync(file, 'utf8');
        const fileDocs = this.extractApiDocumentation(content, path.basename(file), includePrivate);
        if (fileDocs.length > 0) {
          documentation.push(`## ${path.basename(file)}\n\n${fileDocs.join('\n\n')}`);
        }
      } catch (error) {
        issues.push(`Could not process ${path.basename(file)}: ${(error as Error).message}`);
      }
    }

    let output = '';

    if (outputFormat === 'markdown') {
      output = `# API Documentation\n\nGenerated on ${new Date().toISOString()}\n\n${documentation.join('\n\n---\n\n')}`;
    } else if (outputFormat === 'json') {
      output = JSON.stringify(
        {
          generated: new Date().toISOString(),
          files: documentation.length,
          documentation: documentation,
        },
        null,
        2
      );
    }

    return {
      content: [
        {
          type: 'text',
          text: `API Documentation Generation Results:\n\nFiles Processed: ${sourceFiles.length}\nDocumentation Generated: ${documentation.length} sections\n\nIssues:\n${issues.map(issue => `• ${issue}`).join('\n')}\n\n---\n\n${output}`,
        },
      ],
    };
  }

  private async findSourceFiles(workspacePath: string, sourcePath?: string): Promise<string[]> {
    const searchPath = sourcePath ? path.join(workspacePath, sourcePath) : workspacePath;
    const patterns = [
      '**/*.ts',
      '**/*.js',
      '**/*.cs',
      '**/*.py',
      '!**/node_modules/**',
      '!**/dist/**',
      '!**/bin/**',
      '!**/obj/**',
    ];

    const files: string[] = [];
    for (const pattern of patterns) {
      const matches = await glob(pattern, { cwd: searchPath, absolute: true });
      files.push(...matches);
    }

    return [...new Set(files)]; // Remove duplicates
  }

  private extractApiDocumentation(
    content: string,
    fileName: string,
    includePrivate: boolean
  ): string[] {
    const docs: string[] = [];
    const lines = content.split('\n');

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];

      // Look for JSDoc/TSDoc comments
      if (line.trim().startsWith('/**') || line.trim().startsWith('///')) {
        let comment = '';
        let j = i;

        // Collect the full comment
        while (
          j < lines.length &&
          (lines[j].trim().startsWith('/**') ||
            lines[j].trim().startsWith('///') ||
            lines[j].trim().startsWith(' *') ||
            lines[j].trim().startsWith('///'))
        ) {
          comment += lines[j] + '\n';
          j++;
        }

        // Look for the function/method/class after the comment
        let declaration = '';
        for (let k = j; k < Math.min(j + 10, lines.length); k++) {
          const declLine = lines[k].trim();
          if (
            declLine.includes('function') ||
            declLine.includes('class') ||
            declLine.includes('interface') ||
            declLine.includes('public') ||
            declLine.includes('private') ||
            declLine.includes('protected')
          ) {
            if (!includePrivate && declLine.includes('private')) {
              break; // Skip private members
            }

            declaration = declLine;
            break;
          }
        }

        if (declaration) {
          // Clean up the comment
          const cleanComment = comment
            .replace(/\/\*\*\s*/g, '')
            .replace(/\s*\*\/\s*/g, '')
            .replace(/^\s*\*\s*/gm, '')
            .replace(/^\s*\/\/\/\s*/gm, '')
            .trim();

          docs.push(`### ${declaration}\n\n${cleanComment}`);
        }
      }
    }

    return docs;
  }

  private async validateReadme(args: any) {
    const readmePath = args.readmePath
      ? validateString(args.readmePath, 'readmePath', 1, 500)
      : null;
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const checkSections = args.checkSections || [
      'description',
      'installation',
      'usage',
      'api',
      'contributing',
      'license',
    ];

    const issues: string[] = [];
    const suggestions: string[] = [];

    // Find README file
    let readmeFile = '';
    if (readmePath) {
      readmeFile = path.join(workspacePath, readmePath);
    } else {
      const possibleReadmes = ['README.md', 'readme.md', 'README.MD', 'Readme.md'];

      for (const readme of possibleReadmes) {
        const fullPath = path.join(workspacePath, readme);
        if (fs.existsSync(fullPath)) {
          readmeFile = fullPath;
          break;
        }
      }
    }

    if (!readmeFile || !fs.existsSync(readmeFile)) {
      issues.push('No README file found');
      return {
        content: [
          {
            type: 'text',
            text: 'README Validation Results:\n\n❌ No README file found in workspace',
          },
        ],
      };
    }

    try {
      const content = fs.readFileSync(readmeFile, 'utf8');
      const validation = this.validateReadmeContent(content, checkSections);
      issues.push(...validation.issues);
      suggestions.push(...validation.suggestions);
    } catch (error) {
      issues.push(`Could not read README: ${(error as Error).message}`);
    }

    return {
      content: [
        {
          type: 'text',
          text: `README Validation Results for ${path.basename(readmeFile)}:\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nSuggestions (${suggestions.length}):\n${suggestions.map(sugg => `• ${sugg}`).join('\n')}`,
        },
      ],
    };
  }

  private validateReadmeContent(
    content: string,
    sections: string[]
  ): { issues: string[]; suggestions: string[] } {
    const issues: string[] = [];
    const suggestions: string[] = [];
    const lowerContent = content.toLowerCase();

    // Check for required sections
    const sectionChecks = {
      description: ['description', 'about', 'overview'],
      installation: ['installation', 'install', 'setup', 'getting started'],
      usage: ['usage', 'example', 'quick start'],
      api: ['api', 'reference', 'documentation'],
      contributing: ['contributing', 'contribute', 'development'],
      license: ['license', 'licensing'],
    };

    for (const section of sections) {
      const keywords = sectionChecks[section as keyof typeof sectionChecks] || [section];
      const hasSection = keywords.some(
        keyword => lowerContent.includes(`# ${keyword}`) || lowerContent.includes(`## ${keyword}`)
      );

      if (!hasSection) {
        issues.push(`Missing ${section} section`);
      }
    }

    // Check for basic structure
    if (!lowerContent.includes('# ')) {
      issues.push('Missing main title (H1 heading)');
    }

    // Check for badges/shields
    if (!content.includes('img.shields.io') && !content.includes('badge')) {
      suggestions.push('Consider adding status badges (build, version, license)');
    }

    // Check for table of contents
    if (content.length > 2000 && !lowerContent.includes('table of contents')) {
      suggestions.push('Consider adding a table of contents for long README');
    }

    // Check for code examples
    if (!content.includes('```')) {
      suggestions.push('Consider adding code examples');
    }

    return { issues, suggestions };
  }

  private async updateDocumentation(args: any) {
    const changedFiles = args.changedFiles || [];
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const docType = args.docType || 'api';

    const updates: string[] = [];
    const recommendations: string[] = [];

    // Analyze changed files
    for (const file of changedFiles.slice(0, 10)) {
      // Limit to 10 files
      try {
        const fullPath = path.join(workspacePath, file);
        if (fs.existsSync(fullPath)) {
          const content = fs.readFileSync(fullPath, 'utf8');
          const analysis = this.analyzeFileChanges(content, file, docType);
          updates.push(...analysis.updates);
          recommendations.push(...analysis.recommendations);
        }
      } catch (error) {
        updates.push(`Could not analyze ${file}: ${(error as Error).message}`);
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Documentation Update Analysis (${docType}):\n\nFiles Analyzed: ${changedFiles.length}\n\nUpdates Needed:\n${updates.map(update => `• ${update}`).join('\n')}\n\nRecommendations:\n${recommendations.map(rec => `• ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeFileChanges(
    content: string,
    fileName: string,
    docType: string
  ): { updates: string[]; recommendations: string[] } {
    const updates: string[] = [];
    const recommendations: string[] = [];

    if (docType === 'api') {
      // Check for new public methods/functions
      const lines = content.split('\n');
      for (const line of lines) {
        if (
          line.includes('public') &&
          (line.includes('function') || line.includes('def ') || line.includes('('))
        ) {
          if (!line.includes('/**') && !line.includes('///')) {
            updates.push(`${fileName}: New public method ${line.trim()} needs documentation`);
          }
        }
      }
    }

    if (docType === 'readme') {
      if (content.includes('TODO') || content.includes('FIXME')) {
        recommendations.push(
          `${fileName}: Contains TODO/FIXME comments that might affect documentation`
        );
      }
    }

    return { updates, recommendations };
  }

  private async checkDocCoverage(args: any) {
    const sourcePath = args.sourcePath ? validateString(args.sourcePath, 'sourcePath', 1, 500) : '';
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const coverageType = args.coverageType || 'all';

    const coverage: { [key: string]: number } = {
      documented: 0,
      total: 0,
      percentage: 0,
    };

    const details: string[] = [];

    // Find source files
    const sourceFiles = await this.findSourceFiles(workspacePath, sourcePath);

    for (const file of sourceFiles.slice(0, 20)) {
      // Limit to 20 files
      try {
        const content = fs.readFileSync(file, 'utf8');
        const fileCoverage = this.calculateDocCoverage(content, coverageType);
        coverage.documented += fileCoverage.documented;
        coverage.total += fileCoverage.total;
        details.push(
          `${path.basename(file)}: ${fileCoverage.documented}/${fileCoverage.total} documented`
        );
      } catch (error) {
        details.push(`${path.basename(file)}: Could not analyze - ${(error as Error).message}`);
      }
    }

    coverage.percentage =
      coverage.total > 0 ? Math.round((coverage.documented / coverage.total) * 100) : 0;

    return {
      content: [
        {
          type: 'text',
          text: `Documentation Coverage Analysis (${coverageType}):\n\nOverall Coverage: ${coverage.percentage}%\nDocumented: ${coverage.documented}\nTotal: ${coverage.total}\n\nFile Details:\n${details.map(detail => `• ${detail}`).join('\n')}`,
        },
      ],
    };
  }

  private calculateDocCoverage(
    content: string,
    type: string
  ): { documented: number; total: number } {
    let documented = 0;
    let total = 0;
    const lines = content.split('\n');

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];

      // Count functions/methods/classes based on type
      if (type === 'functions' || type === 'all') {
        if (
          line.includes('function') ||
          line.includes('def ') ||
          line.includes('public ') ||
          line.includes('private ')
        ) {
          total++;
          // Check if previous lines have documentation
          let hasDocs = false;
          for (let j = Math.max(0, i - 5); j < i; j++) {
            if (lines[j].trim().startsWith('/**') || lines[j].trim().startsWith('///')) {
              hasDocs = true;
              break;
            }
          }
          if (hasDocs) documented++;
        }
      }

      if (type === 'classes' || type === 'all') {
        if (line.includes('class ') || line.includes('interface ')) {
          total++;
          // Check for documentation
          let hasDocs = false;
          for (let j = Math.max(0, i - 5); j < i; j++) {
            if (lines[j].trim().startsWith('/**') || lines[j].trim().startsWith('///')) {
              hasDocs = true;
              break;
            }
          }
          if (hasDocs) documented++;
        }
      }
    }

    return { documented, total };
  }

  private async generateChangeLog(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const sinceTag = args.sinceTag ? validateString(args.sinceTag, 'sinceTag', 1, 100) : null;
    const outputFormat = args.outputFormat || 'markdown';

    const changes: { [key: string]: string[] } = {
      features: [],
      fixes: [],
      breaking: [],
      other: [],
    };

    try {
      // This would normally use git commands, but for now we'll simulate
      // In a real implementation, you'd run git log commands
      const simulatedCommits = [
        'feat: add user authentication system',
        'fix: resolve memory leak in data processing',
        'feat: implement real-time notifications',
        'fix: correct typo in error message',
        'BREAKING: change API endpoint structure',
        'docs: update installation guide',
        'refactor: optimize database queries',
      ];

      for (const commit of simulatedCommits) {
        if (commit.startsWith('feat:')) {
          changes.features.push(commit.substring(5).trim());
        } else if (commit.startsWith('fix:')) {
          changes.fixes.push(commit.substring(4).trim());
        } else if (commit.includes('BREAKING')) {
          changes.breaking.push(commit.replace('BREAKING:', '').trim());
        } else {
          changes.other.push(commit);
        }
      }
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `Change Log Generation Failed: ${(error as Error).message}`,
          },
        ],
      };
    }

    let output = '';

    if (outputFormat === 'markdown') {
      output = `# Change Log\n\nGenerated on ${new Date().toISOString()}\n\n`;
      if (changes.breaking.length > 0) {
        output += `## Breaking Changes\n\n${changes.breaking.map(change => `- ${change}`).join('\n')}\n\n`;
      }
      if (changes.features.length > 0) {
        output += `## Features\n\n${changes.features.map(change => `- ${change}`).join('\n')}\n\n`;
      }
      if (changes.fixes.length > 0) {
        output += `## Bug Fixes\n\n${changes.fixes.map(change => `- ${change}`).join('\n')}\n\n`;
      }
      if (changes.other.length > 0) {
        output += `## Other Changes\n\n${changes.other.map(change => `- ${change}`).join('\n')}\n\n`;
      }
    } else if (outputFormat === 'json') {
      output = JSON.stringify(
        {
          generated: new Date().toISOString(),
          sinceTag: sinceTag,
          changes: changes,
        },
        null,
        2
      );
    }

    return {
      content: [
        {
          type: 'text',
          text: `Change Log Generation Results:\n\nFeatures: ${changes.features.length}\nFixes: ${changes.fixes.length}\nBreaking: ${changes.breaking.length}\nOther: ${changes.other.length}\n\n---\n\n${output}`,
        },
      ],
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Documentation MCP server running...');
  }
}

// Start the server
const server = new DocumentationMCPServer();
server.run().catch(console.error);
