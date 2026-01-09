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
import { parseDocument } from 'htmlparser2';
import { DomHandler } from 'domhandler';
import * as parse5 from 'parse5';
import * as csstree from 'css-tree';
import { JSDOM } from 'jsdom';
import * as axe from 'axe-core';
import glob from 'fast-glob';
import postcss from 'postcss';

// Input validation utilities
class ValidationError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'ValidationError';
  }
}

function validateString(value: any, fieldName: string, minLength = 1, maxLength = 1000): string {
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
  const pathStr = validateString(workspacePath, 'workspacePath', 1, 500);

  // Prevent path traversal attacks
  if (pathStr.includes('..') || pathStr.includes('../') || pathStr.startsWith('/')) {
    throw new ValidationError('workspacePath must be a relative path without .. or absolute paths');
  }

  return pathStr;
}

function validateFilePath(filePath: any, workspacePath: string): string {
  const filePathStr = validateString(filePath, 'filePath', 1, 500);

  // Ensure file path is within workspace
  if (!filePathStr.startsWith(workspacePath) && !filePathStr.includes(workspacePath)) {
    throw new ValidationError('filePath must be within the workspace directory');
  }

  return filePathStr;
}

// HTML Analysis
class HtmlAnalyzer {
  private workspacePath: string;

  constructor(workspacePath: string) {
    this.workspacePath = workspacePath;
  }

  async findHtmlFiles(includePattern?: string): Promise<string[]> {
    const pattern = includePattern || '**/*.{html,htm}';
    const files = await glob(pattern, {
      cwd: this.workspacePath,
      absolute: true,
      ignore: ['**/node_modules/**', '**/dist/**', '**/build/**'],
    });
    return files;
  }

  parseHtml(content: string): any {
    return parse5.parse(content);
  }

  analyzeStructure(htmlContent: string): {
    doctype: string | null;
    lang: string | null;
    title: string | null;
    headings: Array<{ level: number; text: string }>;
    links: Array<{ href: string; text: string; external: boolean }>;
    images: Array<{ src: string; alt: string | null }>;
    forms: Array<{ action: string | null; method: string; inputs: number }>;
    semanticElements: string[];
    metaTags: Array<{ name: string; content: string }>;
  } {
    const dom = new JSDOM(htmlContent);
    const document = dom.window.document;

    // Extract doctype
    const doctype = document.doctype ? `<!DOCTYPE ${document.doctype.name}>` : null;

    // Extract language
    const lang = document.documentElement.getAttribute('lang');

    // Extract title
    const title = document.querySelector('title')?.textContent || null;

    // Extract headings
    const headings = Array.from(document.querySelectorAll('h1, h2, h3, h4, h5, h6')).map(h => ({
      level: parseInt(h.tagName.charAt(1)),
      text: h.textContent?.trim() || '',
    }));

    // Extract links
    const links = Array.from(document.querySelectorAll('a[href]')).map(a => {
      const href = a.getAttribute('href') || '';
      return {
        href,
        text: a.textContent?.trim() || '',
        external: href.startsWith('http://') || href.startsWith('https://'),
      };
    });

    // Extract images
    const images = Array.from(document.querySelectorAll('img')).map(img => ({
      src: img.getAttribute('src') || '',
      alt: img.getAttribute('alt'),
    }));

    // Extract forms
    const forms = Array.from(document.querySelectorAll('form')).map(form => ({
      action: form.getAttribute('action'),
      method: form.getAttribute('method') || 'get',
      inputs: form.querySelectorAll('input, select, textarea').length,
    }));

    // Identify semantic HTML5 elements
    const semanticTags = [
      'header',
      'nav',
      'main',
      'article',
      'section',
      'aside',
      'footer',
      'figure',
      'figcaption',
      'time',
      'mark',
      'details',
      'summary',
    ];
    const semanticElements = semanticTags.filter(tag => document.querySelector(tag) !== null);

    // Extract meta tags
    const metaTags = Array.from(document.querySelectorAll('meta[name], meta[property]')).map(
      meta => ({
        name: meta.getAttribute('name') || meta.getAttribute('property') || '',
        content: meta.getAttribute('content') || '',
      })
    );

    return {
      doctype,
      lang,
      title,
      headings,
      links,
      images,
      forms,
      semanticElements,
      metaTags,
    };
  }

  async checkAccessibility(htmlContent: string): Promise<any> {
    const dom = new JSDOM(htmlContent);
    const results = await axe.run(dom.window.document);

    return {
      violations: results.violations.map(v => ({
        id: v.id,
        impact: v.impact,
        description: v.description,
        help: v.help,
        helpUrl: v.helpUrl,
        nodes: v.nodes.length,
      })),
      passes: results.passes.length,
      incomplete: results.incomplete.length,
    };
  }

  validateSemantics(htmlContent: string): {
    issues: Array<{ type: string; message: string; severity: string }>;
    recommendations: string[];
  } {
    const dom = new JSDOM(htmlContent);
    const document = dom.window.document;
    const issues: Array<{ type: string; message: string; severity: string }> = [];
    const recommendations: string[] = [];

    // Check for missing lang attribute
    if (!document.documentElement.getAttribute('lang')) {
      issues.push({
        type: 'missing-lang',
        message: 'Missing lang attribute on <html> element',
        severity: 'error',
      });
    }

    // Check for multiple h1 tags
    const h1Count = document.querySelectorAll('h1').length;
    if (h1Count > 1) {
      issues.push({
        type: 'multiple-h1',
        message: `Found ${h1Count} <h1> elements. Best practice is to have only one.`,
        severity: 'warning',
      });
    }

    // Check for images without alt text
    const imagesWithoutAlt = Array.from(document.querySelectorAll('img')).filter(
      img => !img.hasAttribute('alt')
    );
    if (imagesWithoutAlt.length > 0) {
      issues.push({
        type: 'missing-alt',
        message: `${imagesWithoutAlt.length} images missing alt attribute`,
        severity: 'error',
      });
    }

    // Check for form inputs without labels
    const inputsWithoutLabels = Array.from(
      document.querySelectorAll('input:not([type="hidden"])')
    ).filter(input => {
      const id = input.getAttribute('id');
      return !id || !document.querySelector(`label[for="${id}"]`);
    });
    if (inputsWithoutLabels.length > 0) {
      issues.push({
        type: 'unlabeled-inputs',
        message: `${inputsWithoutLabels.length} form inputs without associated labels`,
        severity: 'warning',
      });
    }

    // Recommendations
    if (document.querySelector('div.header') && !document.querySelector('header')) {
      recommendations.push('Consider using semantic <header> instead of <div class="header">');
    }
    if (document.querySelector('div.footer') && !document.querySelector('footer')) {
      recommendations.push('Consider using semantic <footer> instead of <div class="footer">');
    }
    if (!document.querySelector('main')) {
      recommendations.push('Consider adding a <main> element for the primary content');
    }

    return { issues, recommendations };
  }
}

// CSS Analysis
class CssAnalyzer {
  private workspacePath: string;

  constructor(workspacePath: string) {
    this.workspacePath = workspacePath;
  }

  async findCssFiles(includePattern?: string): Promise<string[]> {
    const pattern = includePattern || '**/*.{css,scss,sass,less}';
    const files = await glob(pattern, {
      cwd: this.workspacePath,
      absolute: true,
      ignore: ['**/node_modules/**', '**/dist/**', '**/build/**'],
    });
    return files;
  }

  parseCSS(content: string): any {
    try {
      return csstree.parse(content);
    } catch (error: any) {
      throw new Error(`CSS parse error: ${error.message}`);
    }
  }

  analyzeCSS(cssContent: string): {
    rules: number;
    selectors: string[];
    properties: Map<string, number>;
    mediaQueries: string[];
    variables: string[];
    imports: string[];
    colors: Set<string>;
    fonts: Set<string>;
  } {
    const ast = this.parseCSS(cssContent);
    const selectors: string[] = [];
    const properties = new Map<string, number>();
    const mediaQueries: string[] = [];
    const variables: string[] = [];
    const imports: string[] = [];
    const colors = new Set<string>();
    const fonts = new Set<string>();
    let ruleCount = 0;

    csstree.walk(ast, {
      visit: 'Rule',
      enter(node: any) {
        ruleCount++;
        const selectorText = csstree.generate(node.prelude);
        selectors.push(selectorText);
      },
    });

    csstree.walk(ast, {
      visit: 'Declaration',
      enter(node: any) {
        const prop = node.property;
        properties.set(prop, (properties.get(prop) || 0) + 1);

        // Extract colors
        if (prop.includes('color') || prop === 'background') {
          csstree.walk(node.value, {
            visit: 'Hash',
            enter(hashNode: any) {
              colors.add(`#${hashNode.value}`);
            },
          });
        }

        // Extract fonts
        if (prop === 'font-family') {
          const value = csstree.generate(node.value);
          fonts.add(value);
        }
      },
    });

    csstree.walk(ast, {
      visit: 'Atrule',
      enter(node: any) {
        if (node.name === 'media') {
          const query = csstree.generate(node.prelude);
          mediaQueries.push(query);
        } else if (node.name === 'import') {
          const importPath = csstree.generate(node.prelude);
          imports.push(importPath);
        }
      },
    });

    // Extract CSS variables
    csstree.walk(ast, {
      visit: 'Declaration',
      enter(node: any) {
        if (node.property.startsWith('--')) {
          variables.push(node.property);
        }
      },
    });

    return {
      rules: ruleCount,
      selectors,
      properties,
      mediaQueries,
      variables,
      imports,
      colors,
      fonts,
    };
  }

  detectIssues(cssContent: string): {
    issues: Array<{ type: string; message: string; severity: string; line?: number }>;
    suggestions: string[];
  } {
    const issues: Array<{ type: string; message: string; severity: string; line?: number }> = [];
    const suggestions: string[] = [];

    try {
      const ast = this.parseCSS(cssContent);
      const analysis = this.analyzeCSS(cssContent);

      // Check for duplicate selectors
      const selectorCounts = new Map<string, number>();
      analysis.selectors.forEach(selector => {
        selectorCounts.set(selector, (selectorCounts.get(selector) || 0) + 1);
      });

      selectorCounts.forEach((count, selector) => {
        if (count > 1) {
          issues.push({
            type: 'duplicate-selector',
            message: `Selector "${selector}" appears ${count} times`,
            severity: 'warning',
          });
        }
      });

      // Check for !important usage
      csstree.walk(ast, {
        visit: 'Declaration',
        enter(node: any) {
          if (node.important) {
            issues.push({
              type: 'important-usage',
              message: `!important used on property "${node.property}"`,
              severity: 'info',
            });
          }
        },
      });

      // Check for vendor prefixes (suggest autoprefixer)
      const vendorPrefixes = ['-webkit-', '-moz-', '-ms-', '-o-'];
      analysis.selectors.forEach(selector => {
        vendorPrefixes.forEach(prefix => {
          if (selector.includes(prefix)) {
            suggestions.push('Consider using autoprefixer instead of manual vendor prefixes');
          }
        });
      });

      // Check for unused CSS variables
      const declaredVars = new Set(analysis.variables);
      const usedVars = new Set<string>();

      csstree.walk(ast, {
        visit: 'Function',
        enter(node: any) {
          if (node.name === 'var') {
            const varName = csstree.generate(node);
            const match = varName.match(/var\((--[^,)]+)/);
            if (match) {
              usedVars.add(match[1]);
            }
          }
        },
      });

      declaredVars.forEach(varName => {
        if (!usedVars.has(varName)) {
          issues.push({
            type: 'unused-variable',
            message: `CSS variable "${varName}" is declared but never used`,
            severity: 'info',
          });
        }
      });

      // Suggestions for best practices
      if (analysis.rules > 500) {
        suggestions.push('Consider splitting large CSS file into smaller modules');
      }

      if (analysis.colors.size > 20) {
        suggestions.push('Consider using CSS variables for color management');
      }
    } catch (error: any) {
      issues.push({
        type: 'parse-error',
        message: error.message,
        severity: 'error',
      });
    }

    return { issues, suggestions };
  }

  async optimizeCSS(cssContent: string): Promise<{ optimized: string; savings: number }> {
    const original = cssContent.length;

    // Use PostCSS for optimization
    const result = await postcss([]).process(cssContent, { from: undefined });
    const optimized = result.css;

    return {
      optimized,
      savings: ((original - optimized.length) / original) * 100,
    };
  }
}

// Create MCP Server
const server = new Server(
  {
    name: 'htmlcss-mcp-server',
    version: '1.0.0',
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

// Tool definitions
server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: [
      {
        name: 'analyze_html_structure',
        description:
          'Analyze HTML document structure, extracting headings, links, images, forms, semantic elements, and metadata',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            filePath: {
              type: 'string',
              description: 'Path to HTML file to analyze',
            },
          },
          required: ['workspacePath', 'filePath'],
        },
      },
      {
        name: 'check_html_accessibility',
        description: 'Run accessibility checks on HTML using axe-core, identifying WCAG violations',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            filePath: {
              type: 'string',
              description: 'Path to HTML file to check',
            },
          },
          required: ['workspacePath', 'filePath'],
        },
      },
      {
        name: 'validate_html_semantics',
        description: 'Validate HTML semantic correctness and best practices',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            filePath: {
              type: 'string',
              description: 'Path to HTML file to validate',
            },
          },
          required: ['workspacePath', 'filePath'],
        },
      },
      {
        name: 'analyze_css',
        description:
          'Analyze CSS file structure, rules, selectors, properties, media queries, and variables',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            filePath: {
              type: 'string',
              description: 'Path to CSS file to analyze',
            },
          },
          required: ['workspacePath', 'filePath'],
        },
      },
      {
        name: 'detect_css_issues',
        description:
          'Detect CSS issues like duplicate selectors, !important usage, unused variables, and provide optimization suggestions',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            filePath: {
              type: 'string',
              description: 'Path to CSS file to check',
            },
          },
          required: ['workspacePath', 'filePath'],
        },
      },
      {
        name: 'find_html_files',
        description: 'Find all HTML files in workspace',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            includePattern: {
              type: 'string',
              description: 'Optional glob pattern to filter files (e.g., "**/*.html")',
            },
          },
          required: ['workspacePath'],
        },
      },
      {
        name: 'find_css_files',
        description: 'Find all CSS files in workspace',
        inputSchema: {
          type: 'object',
          properties: {
            workspacePath: {
              type: 'string',
              description: 'Workspace root path (relative path)',
            },
            includePattern: {
              type: 'string',
              description: 'Optional glob pattern to filter files (e.g., "**/*.css")',
            },
          },
          required: ['workspacePath'],
        },
      },
    ],
  };
});

// Tool handlers
server.setRequestHandler(CallToolRequestSchema, async request => {
  try {
    const { name, arguments: args } = request.params;

    if (!args) {
      throw new McpError(ErrorCode.InvalidParams, 'Missing arguments');
    }

    switch (name) {
      case 'analyze_html_structure': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const filePath = validateFilePath(args.filePath, workspacePath);

        const htmlAnalyzer = new HtmlAnalyzer(workspacePath);
        const content = fs.readFileSync(filePath, 'utf-8');
        const structure = htmlAnalyzer.analyzeStructure(content);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(structure, null, 2),
            },
          ],
        };
      }

      case 'check_html_accessibility': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const filePath = validateFilePath(args.filePath, workspacePath);

        const htmlAnalyzer = new HtmlAnalyzer(workspacePath);
        const content = fs.readFileSync(filePath, 'utf-8');
        const results = await htmlAnalyzer.checkAccessibility(content);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(results, null, 2),
            },
          ],
        };
      }

      case 'validate_html_semantics': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const filePath = validateFilePath(args.filePath, workspacePath);

        const htmlAnalyzer = new HtmlAnalyzer(workspacePath);
        const content = fs.readFileSync(filePath, 'utf-8');
        const validation = htmlAnalyzer.validateSemantics(content);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(validation, null, 2),
            },
          ],
        };
      }

      case 'analyze_css': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const filePath = validateFilePath(args.filePath, workspacePath);

        const cssAnalyzer = new CssAnalyzer(workspacePath);
        const content = fs.readFileSync(filePath, 'utf-8');
        const analysis = cssAnalyzer.analyzeCSS(content);

        // Convert Map and Set to serializable format
        const serializable = {
          ...analysis,
          properties: Array.from(analysis.properties.entries()).map(([k, v]) => ({
            property: k,
            count: v,
          })),
          colors: Array.from(analysis.colors),
          fonts: Array.from(analysis.fonts),
        };

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(serializable, null, 2),
            },
          ],
        };
      }

      case 'detect_css_issues': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const filePath = validateFilePath(args.filePath, workspacePath);

        const cssAnalyzer = new CssAnalyzer(workspacePath);
        const content = fs.readFileSync(filePath, 'utf-8');
        const results = cssAnalyzer.detectIssues(content);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(results, null, 2),
            },
          ],
        };
      }

      case 'find_html_files': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const htmlAnalyzer = new HtmlAnalyzer(workspacePath);
        const includePattern = args.includePattern as string | undefined;
        const files = await htmlAnalyzer.findHtmlFiles(includePattern);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify({ files, count: files.length }, null, 2),
            },
          ],
        };
      }

      case 'find_css_files': {
        const workspacePath = validateWorkspacePath(args.workspacePath);
        const cssAnalyzer = new CssAnalyzer(workspacePath);
        const includePattern = args.includePattern as string | undefined;
        const files = await cssAnalyzer.findCssFiles(includePattern);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify({ files, count: files.length }, null, 2),
            },
          ],
        };
      }

      default:
        throw new McpError(ErrorCode.MethodNotFound, `Unknown tool: ${name}`);
    }
  } catch (error: any) {
    if (error instanceof ValidationError) {
      throw new McpError(ErrorCode.InvalidParams, error.message);
    }
    throw error;
  }
});

// Start server
async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error('HTML/CSS MCP Server running on stdio');
}

main().catch(error => {
  console.error('Fatal error in main():', error);
  process.exit(1);
});
