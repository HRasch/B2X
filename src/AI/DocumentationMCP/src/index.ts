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
          {
            name: 'extract_markdown_fragment',
            description:
              'Extract intelligent fragments from large markdown files for token-efficient editing',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to markdown file to fragment',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                maxLines: {
                  type: 'number',
                  description: 'Maximum lines to include in fragment (default: 100)',
                  default: 100,
                  minimum: 50,
                  maximum: 500,
                },
                includeHeaders: {
                  type: 'boolean',
                  description: 'Include all major headers with context',
                  default: true,
                },
                includeFrontmatter: {
                  type: 'boolean',
                  description: 'Include YAML frontmatter if present',
                  default: true,
                },
                sampleContent: {
                  type: 'boolean',
                  description: 'Include representative content samples',
                  default: true,
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'extract_json_fragment',
            description:
              'Extract intelligent fragments from large JSON files for token-efficient editing',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to JSON file to fragment',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                maxKeys: {
                  type: 'number',
                  description: 'Maximum top-level keys to include (default: 50)',
                  default: 50,
                  minimum: 20,
                  maximum: 200,
                },
                preserveStructure: {
                  type: 'boolean',
                  description: 'Preserve object/array structure outlines',
                  default: true,
                },
                includeComments: {
                  type: 'boolean',
                  description: 'Include JSON comments if present',
                  default: false,
                },
                sampleArrays: {
                  type: 'boolean',
                  description: 'Sample array elements instead of including all',
                  default: true,
                },
                schemaPath: {
                  type: 'string',
                  description: 'Path to JSON schema file for enhanced extraction (optional)',
                },
                prioritizeBySchema: {
                  type: 'boolean',
                  description: 'Use schema to prioritize important configuration sections',
                  default: false,
                },
                includeSchemaInfo: {
                  type: 'boolean',
                  description: 'Include schema descriptions, types, and validation info',
                  default: false,
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'extract_yaml_fragment',
            description:
              'Extract intelligent fragments from large YAML files for token-efficient editing',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to YAML file to fragment',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                maxKeys: {
                  type: 'number',
                  description: 'Maximum top-level keys to include (default: 50)',
                  default: 50,
                  minimum: 20,
                  maximum: 200,
                },
                preserveStructure: {
                  type: 'boolean',
                  description: 'Preserve object/array structure outlines',
                  default: true,
                },
                includeComments: {
                  type: 'boolean',
                  description: 'Include YAML comments if present',
                  default: false,
                },
                sampleArrays: {
                  type: 'boolean',
                  description: 'Sample array elements instead of including all',
                  default: true,
                },
                schemaPath: {
                  type: 'string',
                  description: 'Path to JSON schema file for enhanced extraction (optional)',
                },
                prioritizeBySchema: {
                  type: 'boolean',
                  description: 'Use schema to prioritize important configuration sections',
                  default: false,
                },
                includeSchemaInfo: {
                  type: 'boolean',
                  description: 'Include schema descriptions, types, and validation info',
                  default: false,
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'extract_xml_fragment',
            description:
              'Extract intelligent fragments from large XML files for token-efficient editing',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to XML file to fragment',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                maxElements: {
                  type: 'number',
                  description: 'Maximum top-level elements to include (default: 50)',
                  default: 50,
                  minimum: 20,
                  maximum: 200,
                },
                preserveStructure: {
                  type: 'boolean',
                  description: 'Preserve XML element structure outlines',
                  default: true,
                },
                includeComments: {
                  type: 'boolean',
                  description: 'Include XML comments if present',
                  default: false,
                },
                sampleArrays: {
                  type: 'boolean',
                  description: 'Sample repeated elements instead of including all',
                  default: true,
                },
                schemaPath: {
                  type: 'string',
                  description: 'Path to XML schema file (.xsd) for enhanced extraction (optional)',
                },
                prioritizeBySchema: {
                  type: 'boolean',
                  description: 'Use schema to prioritize important XML elements',
                  default: false,
                },
                includeSchemaInfo: {
                  type: 'boolean',
                  description: 'Include schema descriptions, types, and validation info',
                  default: false,
                },
              },
              required: ['filePath', 'workspacePath'],
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
          case 'extract_markdown_fragment':
            return await this.extractMarkdownFragment(args);
          case 'extract_json_fragment':
            return await this.extractJsonFragment(args);
          case 'extract_yaml_fragment':
            return await this.extractYamlFragment(args);
          case 'extract_xml_fragment':
            return await this.extractXmlFragment(args);
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

  private async extractMarkdownFragment(args: any) {
    const filePath = validateString(args.filePath, 'filePath', 1, 500);
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const maxLines = args.maxLines || 100;
    const includeHeaders = args.includeHeaders !== false;
    const includeFrontmatter = args.includeFrontmatter !== false;
    const sampleContent = args.sampleContent !== false;

    try {
      const fullPath = path.resolve(workspacePath, filePath);
      if (!fs.existsSync(fullPath)) {
        throw new Error(`File not found: ${fullPath}`);
      }

      const content = fs.readFileSync(fullPath, 'utf8');
      const lines = content.split('\n');
      const totalLines = lines.length;

      let fragment = '';
      let lineCount = 0;

      // Extract frontmatter if requested
      if (includeFrontmatter && lines[0] === '---') {
        let frontmatterEnd = -1;
        for (let i = 1; i < lines.length; i++) {
          if (lines[i] === '---') {
            frontmatterEnd = i;
            break;
          }
        }
        if (frontmatterEnd > 0) {
          const frontmatter = lines.slice(0, frontmatterEnd + 1).join('\n');
          fragment += frontmatter + '\n\n';
          lineCount += frontmatterEnd + 1;
        }
      }

      // Extract headers and context if requested
      if (includeHeaders) {
        const headers: { level: number; line: number; text: string }[] = [];
        for (let i = 0; i < lines.length; i++) {
          const line = lines[i];
          if (line.startsWith('#')) {
            const level = line.match(/^#+/)?.[0].length || 1;
            const text = line.replace(/^#+\s*/, '');
            headers.push({ level, line: i, text });
          }
        }

        // Include top-level headers with some context
        for (const header of headers.slice(0, 10)) {
          // Limit to first 10 headers
          if (lineCount >= maxLines) break;

          // Add the header
          fragment += lines[header.line] + '\n';
          lineCount++;

          // Add some content after the header (up to 5 lines or next header)
          let contentLines = 0;
          for (let i = header.line + 1; i < lines.length && contentLines < 5; i++) {
            if (lines[i].startsWith('#')) break; // Stop at next header
            if (lines[i].trim() && !lines[i].startsWith('---')) {
              fragment += lines[i] + '\n';
              lineCount++;
              contentLines++;
            }
          }
          fragment += '\n';
        }
      }

      // Add representative content samples if requested and space allows
      if (sampleContent && lineCount < maxLines) {
        const remainingLines = maxLines - lineCount;
        const sampleSize = Math.min(remainingLines, Math.floor(totalLines * 0.1)); // 10% sample

        if (sampleSize > 0) {
          fragment += '---\n\n<!-- Representative content samples -->\n\n';

          // Sample from different sections of the file
          const step = Math.max(1, Math.floor(totalLines / sampleSize));
          for (let i = 0; i < totalLines && lineCount < maxLines; i += step) {
            if (lines[i].trim() && !lines[i].startsWith('#') && !lines[i].startsWith('---')) {
              fragment += lines[i] + '\n';
              lineCount++;
            }
          }
        }
      }

      const tokenSavings = Math.round((1 - fragment.split('\n').length / totalLines) * 100);

      return {
        content: [
          {
            type: 'text',
            text: `Markdown Fragment Extraction Results:\n\nOriginal file: ${totalLines} lines\nFragment: ${lineCount} lines\nToken savings: ${tokenSavings}%\n\n---\n\n${fragment}`,
          },
        ],
      };
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `Markdown Fragment Extraction Failed: ${(error as Error).message}`,
          },
        ],
      };
    }
  }

  private async extractJsonFragment(args: any) {
    const filePath = validateString(args.filePath, 'filePath', 1, 500);
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const maxKeys = args.maxKeys || 50;
    const preserveStructure = args.preserveStructure !== false;
    const includeComments = args.includeComments || false;
    const sampleArrays = args.sampleArrays !== false;
    const schemaPath = args.schemaPath
      ? validateString(args.schemaPath, 'schemaPath', 1, 500)
      : null;
    const prioritizeBySchema = args.prioritizeBySchema || false;
    const includeSchemaInfo = args.includeSchemaInfo || false;

    // Load schema if provided
    let schema: any = null;
    if (schemaPath) {
      try {
        const schemaFullPath = path.resolve(workspacePath, schemaPath);
        if (fs.existsSync(schemaFullPath)) {
          const schemaContent = fs.readFileSync(schemaFullPath, 'utf8');
          schema = JSON.parse(schemaContent);
        }
      } catch (error) {
        // Schema loading failed, continue without schema
        console.warn(`Failed to load schema ${schemaPath}: ${(error as Error).message}`);
      }
    }

    try {
      const fullPath = path.resolve(workspacePath, filePath);
      if (!fs.existsSync(fullPath)) {
        throw new Error(`File not found: ${fullPath}`);
      }

      const content = fs.readFileSync(fullPath, 'utf8');
      const lines = content.split('\n');
      const totalLines = lines.length;

      let fragment = '';
      let keyCount = 0;

      try {
        let jsonContent = content;

        // Try parsing as regular JSON first
        let jsonObj;
        try {
          jsonObj = JSON.parse(jsonContent);
        } catch (parseError) {
          // If parsing fails, check if it contains comments and try stripping them
          if (content.includes('//') || content.includes('/*')) {
            // More careful comment stripping for JSONC
            const lines = content.split('\n');
            const cleanedLines = lines
              .map(line => {
                const trimmed = line.trim();
                // Remove lines that are entirely comments
                if (
                  trimmed.startsWith('//') ||
                  trimmed.startsWith('/*') ||
                  trimmed.endsWith('*/')
                ) {
                  return null; // Mark for removal
                }

                // For inline comments, we need to be careful not to break URLs
                // Look for // but make sure it's not inside a string
                let inString = false;
                let stringChar = '';
                let commentStart = -1;

                for (let i = 0; i < line.length; i++) {
                  const char = line[i];
                  const nextChar = line[i + 1] || '';

                  if (!inString && (char === '"' || char === "'")) {
                    inString = true;
                    stringChar = char;
                  } else if (inString && char === stringChar && line[i - 1] !== '\\') {
                    inString = false;
                    stringChar = '';
                  } else if (!inString && char === '/' && nextChar === '/') {
                    commentStart = i;
                    break;
                  }
                }

                if (commentStart !== -1) {
                  const beforeComment = line.substring(0, commentStart).trimEnd();
                  // If the line becomes empty after removing comment, remove it entirely
                  return beforeComment || null;
                }

                // Handle block comments (simplified - assuming they don't span lines in our JSONC)
                const blockStart = line.indexOf('/*');
                if (blockStart !== -1) {
                  const blockEnd = line.indexOf('*/', blockStart);
                  if (blockEnd !== -1) {
                    const beforeBlock = line.substring(0, blockStart);
                    const afterBlock = line.substring(blockEnd + 2);
                    const result = (beforeBlock + afterBlock).trim();
                    return result || null;
                  }
                }

                return line;
              })
              .filter(line => line !== null); // Remove null lines

            jsonContent = cleanedLines.join('\n');

            // After cleaning, remove trailing commas that are actually invalid
            jsonContent = jsonContent.replace(/,(\s*[}\]])/g, '$1');

            jsonObj = JSON.parse(jsonContent);
          } else {
            // Re-throw the original error if no comments found
            throw parseError;
          }
        }

        // Handle different JSON structures
        if (Array.isArray(jsonObj)) {
          // Array-based JSON (e.g., configuration arrays)
          fragment += '[\n';

          if (sampleArrays && jsonObj.length > 0) {
            // Sample first few, last few, and indicate total count
            const sampleSize = Math.min(maxKeys, jsonObj.length);
            const firstCount = Math.ceil(sampleSize / 2);
            const lastCount = sampleSize - firstCount;

            // Add first elements
            for (let i = 0; i < Math.min(firstCount, jsonObj.length); i++) {
              fragment += `  ${JSON.stringify(jsonObj[i], null, 2).split('\n').join('\n  ')}`;
              if (i < Math.min(firstCount, jsonObj.length) - 1 || lastCount > 0) {
                fragment += ',';
              }
              fragment += '\n';
              keyCount++;
            }

            // Add ellipsis if there are more elements
            if (jsonObj.length > sampleSize) {
              fragment += `  // ... ${jsonObj.length - sampleSize} more items ...\n`;
            }

            // Add last elements
            const startLast = Math.max(firstCount, jsonObj.length - lastCount);
            for (let i = startLast; i < jsonObj.length; i++) {
              fragment += `  ${JSON.stringify(jsonObj[i], null, 2).split('\n').join('\n  ')}`;
              if (i < jsonObj.length - 1) {
                fragment += ',';
              }
              fragment += '\n';
              keyCount++;
            }
          } else {
            // Include all array elements (up to maxKeys)
            for (let i = 0; i < Math.min(maxKeys, jsonObj.length); i++) {
              fragment += `  ${JSON.stringify(jsonObj[i], null, 2).split('\n').join('\n  ')}`;
              if (i < Math.min(maxKeys, jsonObj.length) - 1) {
                fragment += ',';
              }
              fragment += '\n';
              keyCount++;
            }
            if (jsonObj.length > maxKeys) {
              fragment += `  // ... ${jsonObj.length - maxKeys} more items ...\n`;
            }
          }

          fragment += ']';
        } else if (typeof jsonObj === 'object' && jsonObj !== null) {
          // Object-based JSON (most common case)
          fragment += '{\n';

          const keys = Object.keys(jsonObj);

          // Sort keys by schema priority if schema is available and prioritization is enabled
          let keysToInclude: string[];
          if (schema && prioritizeBySchema && schema.properties) {
            // Create priority mapping based on schema
            const priorityMap = new Map<string, number>();
            const schemaProps = schema.properties;

            // Assign priorities: required properties get highest priority
            const requiredProps = schema.required || [];
            requiredProps.forEach((prop: string, index: number) => {
              priorityMap.set(prop, 1000 - index); // Higher numbers = higher priority
            });

            // Other properties get lower priority
            Object.keys(schemaProps).forEach((prop: string) => {
              if (!priorityMap.has(prop)) {
                priorityMap.set(prop, 500);
              }
            });

            // Sort keys by priority, then alphabetically
            keysToInclude = keys
              .sort((a, b) => {
                const priorityA = priorityMap.get(a) || 0;
                const priorityB = priorityMap.get(b) || 0;
                if (priorityA !== priorityB) {
                  return priorityB - priorityA; // Higher priority first
                }
                return a.localeCompare(b); // Alphabetical fallback
              })
              .slice(0, maxKeys);
          } else {
            // Default behavior: take first maxKeys keys
            keysToInclude = keys.slice(0, maxKeys);
          }

          for (let i = 0; i < keysToInclude.length; i++) {
            const key = keysToInclude[i];
            const value = jsonObj[key];

            fragment += `  "${key}": `;

            // Add schema information if requested
            if (includeSchemaInfo && schema && schema.properties && schema.properties[key]) {
              const propSchema = schema.properties[key];
              const schemaInfo = this.formatSchemaInfo(propSchema);
              if (schemaInfo) {
                fragment += `// ${schemaInfo}\n  `;
              }
            }

            if (preserveStructure && typeof value === 'object' && value !== null) {
              // For objects/arrays, show structure outline
              if (Array.isArray(value)) {
                fragment += `[/* ${value.length} items */]`;
              } else {
                const subKeys = Object.keys(value);
                fragment += `{/* ${subKeys.length} keys: ${subKeys.slice(0, 3).join(', ')}${subKeys.length > 3 ? '...' : ''} */}`;
              }
            } else {
              // For primitive values, show actual content
              fragment += JSON.stringify(value);
            }

            if (i < keysToInclude.length - 1) {
              fragment += ',';
            }
            fragment += '\n';
            keyCount++;
          }

          if (keys.length > maxKeys) {
            fragment += `  // ... ${keys.length - maxKeys} more keys ...\n`;
          }

          fragment += '}';
        } else {
          // Primitive JSON (string, number, boolean)
          fragment = JSON.stringify(jsonObj, null, 2);
          keyCount = 1;
        }

        // Handle comments if requested (JSON5/JSONC style)
        if (includeComments) {
          const commentLines: string[] = [];
          for (const line of lines) {
            if (line.trim().startsWith('//') || line.trim().startsWith('/*')) {
              commentLines.push(line);
            }
          }
          if (commentLines.length > 0) {
            fragment = commentLines.join('\n') + '\n\n' + fragment;
          }
        }

        const fragmentLines = fragment.split('\n').length;
        const tokenSavings = Math.round((1 - fragmentLines / totalLines) * 100);

        return {
          content: [
            {
              type: 'text',
              text: `JSON Fragment Extraction Results:\n\nOriginal file: ${totalLines} lines\nFragment: ${fragmentLines} lines\nKeys processed: ${keyCount}\nToken savings: ${tokenSavings}%\n\n---\n\n${fragment}`,
            },
          ],
        };
      } catch (parseError) {
        throw new Error(`Invalid JSON: ${(parseError as Error).message}`);
      }
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `JSON Fragment Extraction Failed: ${(error as Error).message}`,
          },
        ],
      };
    }
  }

  private formatSchemaInfo(propSchema: any): string | null {
    if (!propSchema) return null;

    const parts: string[] = [];

    // Add type information
    if (propSchema.type) {
      parts.push(propSchema.type);
    }

    // Add description if available
    if (propSchema.description) {
      parts.push(propSchema.description);
    }

    // Add constraints
    if (propSchema.minimum !== undefined || propSchema.maximum !== undefined) {
      const min = propSchema.minimum !== undefined ? `min: ${propSchema.minimum}` : '';
      const max = propSchema.maximum !== undefined ? `max: ${propSchema.maximum}` : '';
      if (min || max) {
        parts.push(`${min}${min && max ? ', ' : ''}${max}`);
      }
    }

    if (propSchema.minLength !== undefined || propSchema.maxLength !== undefined) {
      const min = propSchema.minLength !== undefined ? `min: ${propSchema.minLength}` : '';
      const max = propSchema.maxLength !== undefined ? `max: ${propSchema.maxLength}` : '';
      if (min || max) {
        parts.push(`${min}${min && max ? ', ' : ''}${max} chars`);
      }
    }

    if (propSchema.pattern) {
      parts.push(`pattern: ${propSchema.pattern}`);
    }

    if (propSchema.default !== undefined) {
      parts.push(`default: ${JSON.stringify(propSchema.default)}`);
    }

    if (propSchema.examples && propSchema.examples.length > 0) {
      parts.push(`example: ${JSON.stringify(propSchema.examples[0])}`);
    }

    return parts.length > 0 ? parts.join(' | ') : null;
  }

  private async extractYamlFragment(args: any) {
    const filePath = validateString(args.filePath, 'filePath', 1, 500);
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const maxKeys = args.maxKeys || 50;
    const preserveStructure = args.preserveStructure !== false;
    const includeComments = args.includeComments || false;
    const sampleArrays = args.sampleArrays !== false;
    const schemaPath = args.schemaPath
      ? validateString(args.schemaPath, 'schemaPath', 1, 500)
      : null;
    const prioritizeBySchema = args.prioritizeBySchema || false;
    const includeSchemaInfo = args.includeSchemaInfo || false;

    // Load schema if provided
    let schema: any = null;
    if (schemaPath) {
      try {
        const schemaFullPath = path.resolve(workspacePath, schemaPath);
        if (fs.existsSync(schemaFullPath)) {
          const schemaContent = fs.readFileSync(schemaFullPath, 'utf8');
          schema = JSON.parse(schemaContent);
        }
      } catch (error) {
        // Schema loading failed, continue without schema
        console.warn(`Failed to load schema ${schemaPath}: ${(error as Error).message}`);
      }
    }

    try {
      const fullPath = path.resolve(workspacePath, filePath);
      if (!fs.existsSync(fullPath)) {
        throw new Error(`File not found: ${fullPath}`);
      }

      const content = fs.readFileSync(fullPath, 'utf8');
      const lines = content.split('\n');
      const totalLines = lines.length;

      let fragment = '';
      let keyCount = 0;

      try {
        // For YAML, we'll use a simple parsing approach since we don't have a YAML parser
        // This is a basic implementation that works for common YAML structures
        const yamlObj = this.parseYamlBasic(content);

        // Handle different YAML structures
        if (Array.isArray(yamlObj)) {
          // Array-based YAML
          fragment += '[\n';

          if (sampleArrays && yamlObj.length > 0) {
            const sampleSize = Math.min(maxKeys, yamlObj.length);
            const firstCount = Math.ceil(sampleSize / 2);
            const lastCount = sampleSize - firstCount;

            for (let i = 0; i < Math.min(firstCount, yamlObj.length); i++) {
              fragment += `  ${JSON.stringify(yamlObj[i], null, 2).split('\n').join('\n  ')}`;
              if (i < Math.min(firstCount, yamlObj.length) - 1 || lastCount > 0) {
                fragment += ',';
              }
              fragment += '\n';
              keyCount++;
            }

            if (yamlObj.length > sampleSize) {
              fragment += `  # ... ${yamlObj.length - sampleSize} more items ...\n`;
            }

            const startLast = Math.max(firstCount, yamlObj.length - lastCount);
            for (let i = startLast; i < yamlObj.length; i++) {
              fragment += `  ${JSON.stringify(yamlObj[i], null, 2).split('\n').join('\n  ')}`;
              if (i < yamlObj.length - 1) {
                fragment += ',';
              }
              fragment += '\n';
              keyCount++;
            }
          } else {
            for (let i = 0; i < Math.min(maxKeys, yamlObj.length); i++) {
              fragment += `  ${JSON.stringify(yamlObj[i], null, 2).split('\n').join('\n  ')}`;
              if (i < Math.min(maxKeys, yamlObj.length) - 1) {
                fragment += ',';
              }
              fragment += '\n';
              keyCount++;
            }
            if (yamlObj.length > maxKeys) {
              fragment += `  # ... ${yamlObj.length - maxKeys} more items ...\n`;
            }
          }

          fragment += ']';
        } else if (typeof yamlObj === 'object' && yamlObj !== null) {
          // Object-based YAML
          const keys = Object.keys(yamlObj);

          // Sort keys by schema priority if schema is available and prioritization is enabled
          let keysToInclude: string[];
          if (schema && prioritizeBySchema && schema.properties) {
            const priorityMap = new Map<string, number>();
            const schemaProps = schema.properties;

            const requiredProps = schema.required || [];
            requiredProps.forEach((prop: string, index: number) => {
              priorityMap.set(prop, 1000 - index);
            });

            Object.keys(schemaProps).forEach((prop: string) => {
              if (!priorityMap.has(prop)) {
                priorityMap.set(prop, 500);
              }
            });

            keysToInclude = keys
              .sort((a, b) => {
                const priorityA = priorityMap.get(a) || 0;
                const priorityB = priorityMap.get(b) || 0;
                if (priorityA !== priorityB) {
                  return priorityB - priorityA;
                }
                return a.localeCompare(b);
              })
              .slice(0, maxKeys);
          } else {
            keysToInclude = keys.slice(0, maxKeys);
          }

          for (let i = 0; i < keysToInclude.length; i++) {
            const key = keysToInclude[i];
            const value = yamlObj[key];

            fragment += `${key}: `;

            if (includeSchemaInfo && schema && schema.properties && schema.properties[key]) {
              const propSchema = schema.properties[key];
              const schemaInfo = this.formatSchemaInfo(propSchema);
              if (schemaInfo) {
                fragment += `# ${schemaInfo}\n`;
                fragment += `${key}: `;
              }
            }

            if (preserveStructure && typeof value === 'object' && value !== null) {
              if (Array.isArray(value)) {
                fragment += `[# ${value.length} items]`;
              } else {
                const subKeys = Object.keys(value);
                fragment += `{# ${subKeys.length} keys: ${subKeys.slice(0, 3).join(', ')}${subKeys.length > 3 ? '...' : ''}}`;
              }
            } else {
              fragment += JSON.stringify(value);
            }

            if (i < keysToInclude.length - 1) {
              fragment += '\n';
            }
            fragment += '\n';
            keyCount++;
          }

          if (keys.length > maxKeys) {
            fragment += `# ... ${keys.length - maxKeys} more keys ...\n`;
          }
        } else {
          // Primitive YAML
          fragment = JSON.stringify(yamlObj, null, 2);
          keyCount = 1;
        }

        // Handle comments if requested
        if (includeComments) {
          const commentLines: string[] = [];
          for (const line of lines) {
            if (line.trim().startsWith('#')) {
              commentLines.push(line);
            }
          }
          if (commentLines.length > 0) {
            fragment = commentLines.join('\n') + '\n\n' + fragment;
          }
        }

        const fragmentLines = fragment.split('\n').length;
        const tokenSavings = Math.round((1 - fragmentLines / totalLines) * 100);

        return {
          content: [
            {
              type: 'text',
              text: `YAML Fragment Extraction Results:\n\nOriginal file: ${totalLines} lines\nFragment: ${fragmentLines} lines\nKeys processed: ${keyCount}\nToken savings: ${tokenSavings}%\n\n---\n\n${fragment}`,
            },
          ],
        };
      } catch (parseError) {
        throw new Error(`Invalid YAML: ${(parseError as Error).message}`);
      }
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `YAML Fragment Extraction Failed: ${(error as Error).message}`,
          },
        ],
      };
    }
  }

  private parseYamlBasic(content: string): any {
    // Basic YAML parser for simple structures
    // This is a simplified implementation - for production use, consider a full YAML parser
    const lines = content.split('\n').filter(line => line.trim() && !line.trim().startsWith('#'));
    const result: any = {};
    let currentKey = '';
    let currentIndent = 0;
    const stack: any[] = [result];

    for (const line of lines) {
      const indent = line.length - line.trimStart().length;
      const trimmed = line.trim();

      if (trimmed.includes(':')) {
        const [key, ...valueParts] = trimmed.split(':');
        const value = valueParts.join(':').trim();

        if (indent < currentIndent) {
          // Going up in hierarchy
          while (stack.length > 1 && indent < currentIndent) {
            stack.pop();
            currentIndent -= 2; // Assume 2-space indentation
          }
        }

        currentKey = key.trim();
        currentIndent = indent;

        if (value) {
          // Simple value
          if (value.startsWith('[') && value.endsWith(']')) {
            stack[stack.length - 1][currentKey] = value
              .slice(1, -1)
              .split(',')
              .map(v => v.trim());
          } else if (value === 'true' || value === 'false') {
            stack[stack.length - 1][currentKey] = value === 'true';
          } else if (!isNaN(Number(value))) {
            stack[stack.length - 1][currentKey] = Number(value);
          } else {
            stack[stack.length - 1][currentKey] = value;
          }
        } else {
          // Nested object
          const newObj: any = {};
          stack[stack.length - 1][currentKey] = newObj;
          stack.push(newObj);
        }
      }
    }

    return result;
  }

  private async extractXmlFragment(args: any) {
    const filePath = validateString(args.filePath, 'filePath', 1, 500);
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const maxElements = args.maxElements || 50;
    const preserveStructure = args.preserveStructure !== false;
    const includeComments = args.includeComments || false;
    const sampleArrays = args.sampleArrays !== false;
    const schemaPath = args.schemaPath
      ? validateString(args.schemaPath, 'schemaPath', 1, 500)
      : null;
    const prioritizeBySchema = args.prioritizeBySchema || false;
    const includeSchemaInfo = args.includeSchemaInfo || false;

    // Load schema if provided (XSD support would be complex, using JSON schema for now)
    let schema: any = null;
    if (schemaPath) {
      try {
        const schemaFullPath = path.resolve(workspacePath, schemaPath);
        if (fs.existsSync(schemaFullPath)) {
          const schemaContent = fs.readFileSync(schemaFullPath, 'utf8');
          schema = JSON.parse(schemaContent);
        }
      } catch (error) {
        // Schema loading failed, continue without schema
        console.warn(`Failed to load schema ${schemaPath}: ${(error as Error).message}`);
      }
    }

    try {
      const fullPath = path.resolve(workspacePath, filePath);
      if (!fs.existsSync(fullPath)) {
        throw new Error(`File not found: ${fullPath}`);
      }

      const content = fs.readFileSync(fullPath, 'utf8');
      const lines = content.split('\n');
      const totalLines = lines.length;

      let fragment = '';
      let elementCount = 0;

      try {
        // Parse XML into a simplified structure
        const xmlObj = this.parseXmlBasic(content);

        // Handle different XML structures
        if (Array.isArray(xmlObj)) {
          // Array of XML elements
          fragment += '<array>\n';

          if (sampleArrays && xmlObj.length > 0) {
            const sampleSize = Math.min(maxElements, xmlObj.length);
            const firstCount = Math.ceil(sampleSize / 2);
            const lastCount = sampleSize - firstCount;

            for (let i = 0; i < Math.min(firstCount, xmlObj.length); i++) {
              fragment += `  ${this.formatXmlElement(xmlObj[i], preserveStructure, includeSchemaInfo, schema)}\n`;
              elementCount++;
            }

            if (xmlObj.length > sampleSize) {
              fragment += `  <!-- ... ${xmlObj.length - sampleSize} more elements ... -->\n`;
            }

            const startLast = Math.max(firstCount, xmlObj.length - lastCount);
            for (let i = startLast; i < xmlObj.length; i++) {
              fragment += `  ${this.formatXmlElement(xmlObj[i], preserveStructure, includeSchemaInfo, schema)}\n`;
              elementCount++;
            }
          } else {
            for (let i = 0; i < Math.min(maxElements, xmlObj.length); i++) {
              fragment += `  ${this.formatXmlElement(xmlObj[i], preserveStructure, includeSchemaInfo, schema)}\n`;
              elementCount++;
            }
            if (xmlObj.length > maxElements) {
              fragment += `  <!-- ... ${xmlObj.length - maxElements} more elements ... -->\n`;
            }
          }

          fragment += '</array>';
        } else if (typeof xmlObj === 'object' && xmlObj !== null) {
          // Single XML document/object
          const rootElement = Object.keys(xmlObj)[0];
          const rootContent = xmlObj[rootElement];

          // Sort elements by schema priority if schema is available
          let elementsToInclude: string[];
          if (schema && prioritizeBySchema && schema.properties) {
            const priorityMap = new Map<string, number>();
            const schemaProps = schema.properties;

            const requiredProps = schema.required || [];
            requiredProps.forEach((prop: string, index: number) => {
              priorityMap.set(prop, 1000 - index);
            });

            Object.keys(schemaProps).forEach((prop: string) => {
              if (!priorityMap.has(prop)) {
                priorityMap.set(prop, 500);
              }
            });

            const elementKeys = Object.keys(rootContent);
            elementsToInclude = elementKeys
              .sort((a, b) => {
                const priorityA = priorityMap.get(a) || 0;
                const priorityB = priorityMap.get(b) || 0;
                if (priorityA !== priorityB) {
                  return priorityB - priorityA;
                }
                return a.localeCompare(b);
              })
              .slice(0, maxElements);
          } else {
            elementsToInclude = Object.keys(rootContent).slice(0, maxElements);
          }

          fragment += `<${rootElement}>\n`;

          for (const elementName of elementsToInclude) {
            const elementValue = rootContent[elementName];

            if (
              includeSchemaInfo &&
              schema &&
              schema.properties &&
              schema.properties[elementName]
            ) {
              const propSchema = schema.properties[elementName];
              const schemaInfo = this.formatSchemaInfo(propSchema);
              if (schemaInfo) {
                fragment += `  <!-- ${elementName}: ${schemaInfo} -->\n`;
              }
            }

            fragment += `  ${this.formatXmlElement({ [elementName]: elementValue }, preserveStructure, false, null)}\n`;
            elementCount++;
          }

          if (Object.keys(rootContent).length > maxElements) {
            fragment += `  <!-- ... ${Object.keys(rootContent).length - maxElements} more elements ... -->\n`;
          }

          fragment += `</${rootElement}>`;
        } else {
          // Primitive XML content
          fragment = content.trim();
          elementCount = 1;
        }

        // Handle comments if requested
        if (includeComments) {
          const commentLines: string[] = [];
          for (const line of lines) {
            if (line.trim().startsWith('<!--') && line.trim().endsWith('-->')) {
              commentLines.push(line);
            }
          }
          if (commentLines.length > 0) {
            fragment = commentLines.join('\n') + '\n\n' + fragment;
          }
        }

        const fragmentLines = fragment.split('\n').length;
        const tokenSavings = Math.round((1 - fragmentLines / totalLines) * 100);

        return {
          content: [
            {
              type: 'text',
              text: `XML Fragment Extraction Results:\n\nOriginal file: ${totalLines} lines\nFragment: ${fragmentLines} lines\nElements processed: ${elementCount}\nToken savings: ${tokenSavings}%\n\n---\n\n${fragment}`,
            },
          ],
        };
      } catch (parseError) {
        throw new Error(`Invalid XML: ${(parseError as Error).message}`);
      }
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `XML Fragment Extraction Failed: ${(error as Error).message}`,
          },
        ],
      };
    }
  }

  private parseXmlBasic(content: string): any {
    // Basic XML parser for simple structures
    // This is a simplified implementation - for production use, consider a full XML parser
    const result: any = {};

    // Remove XML declaration and DOCTYPE
    let cleanContent = content
      .replace(/<\?xml.*?\?>/g, '')
      .replace(/<!DOCTYPE.*?>[\s\S]*?>/g, '')
      .replace(/<!--[\s\S]*?-->/g, '') // Remove comments
      .trim();

    // Simple regex-based parsing for basic XML structures
    const tagRegex = /<(\w+)([^>]*)>([\s\S]*?)<\/\1>/g;
    const selfClosingRegex = /<(\w+)([^>]*)\/>/g;

    let match;
    while ((match = tagRegex.exec(cleanContent)) !== null) {
      const [, tagName, attributes, innerContent] = match;

      if (!result[tagName]) {
        result[tagName] = {};
      }

      // Parse attributes
      const attrRegex = /(\w+)="([^"]*)"/g;
      let attrMatch;
      while ((attrMatch = attrRegex.exec(attributes)) !== null) {
        result[tagName][attrMatch[1]] = attrMatch[2];
      }

      // Parse inner content (simplified)
      if (innerContent.trim()) {
        const innerTags = innerContent.match(/<(\w+).*?<\/\1>/g);
        if (innerTags) {
          result[tagName] = { ...result[tagName], ...this.parseXmlBasic(innerContent) };
        } else {
          result[tagName].content = innerContent.trim();
        }
      }
    }

    // Handle self-closing tags
    while ((match = selfClosingRegex.exec(cleanContent)) !== null) {
      const [, tagName, attributes] = match;

      if (!result[tagName]) {
        result[tagName] = {};
      }

      const attrRegex = /(\w+)="([^"]*)"/g;
      let attrMatch;
      while ((attrMatch = attrRegex.exec(attributes)) !== null) {
        result[tagName][attrMatch[1]] = attrMatch[2];
      }
    }

    return result;
  }

  private formatXmlElement(
    element: any,
    preserveStructure: boolean,
    includeSchemaInfo: boolean,
    schema: any
  ): string {
    const elementName = Object.keys(element)[0];
    const elementValue = element[elementName];

    if (typeof elementValue === 'object' && elementValue !== null) {
      if (preserveStructure) {
        const subKeys = Object.keys(elementValue);
        return `<${elementName}><!-- ${subKeys.length} attributes/elements --></${elementName}>`;
      } else {
        return `<${elementName}>${JSON.stringify(elementValue)}</${elementName}>`;
      }
    } else {
      return `<${elementName}>${elementValue}</${elementName}>`;
    }
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
