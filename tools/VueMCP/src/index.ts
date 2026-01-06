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
import { compileScript, compileTemplate, parse } from '@vue/compiler-sfc';
import { compile } from '@vue/compiler-dom';
import glob from 'fast-glob';
import { JSDOM } from 'jsdom';
import * as axe from 'axe-core';

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
  const path = validateString(workspacePath, 'workspacePath', 1, 500);

  // Prevent path traversal attacks
  if (path.includes('..') || path.includes('../') || path.startsWith('/')) {
    throw new ValidationError('workspacePath must be a relative path without .. or absolute paths');
  }

  return path;
}

function validateFilePath(filePath: any, workspacePath: string): string {
  const filePathStr = validateString(filePath, 'filePath', 1, 500);

  // Ensure file path is within workspace
  if (!filePathStr.startsWith(workspacePath) && !filePathStr.includes(workspacePath)) {
    throw new ValidationError('filePath must be within the workspace directory');
  }

  return filePathStr;
}

// Vue.js analysis utilities
class VueAnalyzer {
  private workspacePath: string;

  constructor(workspacePath: string) {
    this.workspacePath = workspacePath;
  }

  async findVueFiles(): Promise<string[]> {
    const pattern = '**/*.vue';
    const files = await glob(pattern, {
      cwd: this.workspacePath,
      absolute: true,
    });
    return files;
  }

  async parseVueComponent(filePath: string): Promise<any> {
    const content = fs.readFileSync(filePath, 'utf-8');
    const { descriptor, errors } = parse(content);

    if (errors.length > 0) {
      throw new Error(`Parse errors in ${filePath}: ${errors.map(e => e.message).join(', ')}`);
    }

    return descriptor;
  }

  analyzeComponentStructure(descriptor: any): {
    name: string;
    props: string[];
    emits: string[];
    slots: string[];
    hasScript: boolean;
    hasTemplate: boolean;
    hasStyle: boolean;
  } {
    const props = descriptor.props ? Object.keys(descriptor.props) : [];
    const emits = descriptor.emits || [];
    const slots = descriptor.slots ? Object.keys(descriptor.slots) : [];

    return {
      name: descriptor.name || 'Anonymous',
      props,
      emits,
      slots,
      hasScript: !!descriptor.script || !!descriptor.scriptSetup,
      hasTemplate: !!descriptor.template,
      hasStyle: !!descriptor.styles?.length,
    };
  }

  analyzeScriptSetup(descriptor: any): {
    bindings: string[];
    imports: string[];
    reactiveVars: string[];
    computedProps: string[];
    methods: string[];
  } {
    const scriptSetup = descriptor.scriptSetup;
    if (!scriptSetup) {
      return { bindings: [], imports: [], reactiveVars: [], computedProps: [], methods: [] };
    }

    const content = scriptSetup.content;
    const bindings = scriptSetup.bindings || [];

    // Extract imports
    const importRegex = /import\s+{([^}]+)}\s+from\s+['"]([^'"]+)['"]/g;
    const imports: string[] = [];
    let match;
    while ((match = importRegex.exec(content)) !== null) {
      imports.push(match[1].trim());
    }

    // Extract reactive variables (ref, reactive)
    const reactiveRegex = /(?:const|let)\s+(\w+)\s*=\s*(?:ref|reactive)\(/g;
    const reactiveVars: string[] = [];
    while ((match = reactiveRegex.exec(content)) !== null) {
      reactiveVars.push(match[1]);
    }

    // Extract computed properties
    const computedRegex = /(?:const|let)\s+(\w+)\s*=\s*computed\(/g;
    const computedProps: string[] = [];
    while ((match = computedRegex.exec(content)) !== null) {
      computedProps.push(match[1]);
    }

    // Extract methods (functions)
    const methodRegex =
      /(?:function\s+(\w+)|(?:const|let)\s+(\w+)\s*=\s*(?:\([^)]*\)\s*=>|function))/g;
    const methods: string[] = [];
    while ((match = methodRegex.exec(content)) !== null) {
      methods.push(match[1] || match[2]);
    }

    return {
      bindings: Object.keys(bindings),
      imports,
      reactiveVars,
      computedProps,
      methods: methods.filter(m => m && !reactiveVars.includes(m) && !computedProps.includes(m)),
    };
  }

  analyzeTemplateAST(descriptor: any): {
    elements: string[];
    directives: string[];
    interpolations: number;
    eventHandlers: string[];
  } {
    const template = descriptor.template;
    if (!template) {
      return { elements: [], directives: [], interpolations: 0, eventHandlers: [] };
    }

    try {
      const { ast } = compile(template.content, { prefixIdentifiers: true });
      const elements: string[] = [];
      const directives: string[] = [];
      let interpolations = 0;
      const eventHandlers: string[] = [];

      function traverse(node: any) {
        if (node.type === 1) {
          // ELEMENT
          elements.push(node.tag);
          if (node.props) {
            node.props.forEach((prop: any) => {
              if (prop.type === 7) {
                // DIRECTIVE
                directives.push(prop.name);
                if (prop.name === 'on') {
                  eventHandlers.push(prop.arg?.content || 'unknown');
                }
              }
            });
          }
          if (node.children) {
            node.children.forEach(traverse);
          }
        } else if (node.type === 5) {
          // INTERPOLATION
          interpolations++;
        }
      }

      if (ast) {
        traverse(ast);
      }

      return {
        elements: [...new Set(elements)],
        directives: [...new Set(directives)],
        interpolations,
        eventHandlers: [...new Set(eventHandlers)],
      };
    } catch (error) {
      return { elements: [], directives: [], interpolations: 0, eventHandlers: [] };
    }
  }

  extractI18nKeys(content: string): string[] {
    const keyRegex = /\$t\(['"]([^'"]+)['"]\)/g;
    const keys: string[] = [];
    let match;

    while ((match = keyRegex.exec(content)) !== null) {
      keys.push(match[1]);
    }

    return [...new Set(keys)]; // Remove duplicates
  }

  async validateI18nKeysAgainstLocales(
    workspacePath: string,
    keys: string[]
  ): Promise<{
    missingKeys: string[];
    availableLocales: string[];
  }> {
    const localeFiles = await glob('**/locales/*.json', {
      cwd: workspacePath,
      absolute: true,
    });

    const availableLocales: string[] = [];
    const localeData: { [key: string]: any } = {};

    for (const file of localeFiles) {
      const locale = path.basename(file, '.json');
      availableLocales.push(locale);
      try {
        const content = fs.readFileSync(file, 'utf-8');
        localeData[locale] = JSON.parse(content);
      } catch (error) {
        // Skip invalid JSON files
      }
    }

    const missingKeys: string[] = [];
    for (const key of keys) {
      let found = false;
      for (const locale of availableLocales) {
        if (this.hasNestedKey(localeData[locale], key)) {
          found = true;
          break;
        }
      }
      if (!found) {
        missingKeys.push(key);
      }
    }

    return { missingKeys, availableLocales };
  }

  private hasNestedKey(obj: any, key: string): boolean {
    const keys = key.split('.');
    let current = obj;
    for (const k of keys) {
      if (current && typeof current === 'object' && k in current) {
        current = current[k];
      } else {
        return false;
      }
    }
    return true;
  }

  analyzeResponsiveClasses(content: string): {
    breakpoints: string[];
    utilities: string[];
  } {
    // Extract Tailwind responsive classes
    const responsiveRegex = /\b(sm|md|lg|xl|2xl):[^ ]+/g;
    const matches = content.match(responsiveRegex) || [];

    const breakpoints = [...new Set(matches.map(cls => cls.split(':')[0]))];
    const utilities = [...new Set(matches.map(cls => cls.split(':')[1]))];

    return { breakpoints, utilities };
  }

  async analyzeViteConfig(workspacePath: string): Promise<{
    hasConfig: boolean;
    buildConfig: any;
    plugins: string[];
  }> {
    const configFiles = ['vite.config.ts', 'vite.config.js', 'vite.config.mjs'];
    let configPath: string | null = null;

    for (const file of configFiles) {
      const fullPath = path.join(workspacePath, file);
      if (fs.existsSync(fullPath)) {
        configPath = fullPath;
        break;
      }
    }

    if (!configPath) {
      return { hasConfig: false, buildConfig: null, plugins: [] };
    }

    try {
      const content = fs.readFileSync(configPath, 'utf-8');
      // Basic analysis - extract build config and plugins
      const buildMatch = content.match(/build:\s*{([^}]*)}/s);
      const pluginsMatch = content.match(/plugins:\s*\[([^\]]*)\]/s);

      const plugins: string[] = [];
      if (pluginsMatch) {
        const pluginMatches = pluginsMatch[1].match(/(\w+)\(\)/g);
        if (pluginMatches) {
          plugins.push(...pluginMatches.map(p => p.replace('()', '')));
        }
      }

      return {
        hasConfig: true,
        buildConfig: buildMatch ? buildMatch[1].trim() : null,
        plugins,
      };
    } catch (error) {
      return { hasConfig: true, buildConfig: null, plugins: [] };
    }
  }

  async analyzeBundleSize(workspacePath: string): Promise<{
    hasStats: boolean;
    totalSize: number;
    chunks: { name: string; size: number }[];
  }> {
    // Look for build stats or dist folder analysis
    const distPath = path.join(workspacePath, 'dist');
    const statsPath = path.join(workspacePath, 'dist/stats.html');

    if (!fs.existsSync(distPath)) {
      return { hasStats: false, totalSize: 0, chunks: [] };
    }

    try {
      const jsFiles = await glob('**/*.js', { cwd: distPath, absolute: true });
      const cssFiles = await glob('**/*.css', { cwd: distPath, absolute: true });

      const chunks: { name: string; size: number }[] = [];
      let totalSize = 0;

      for (const file of [...jsFiles, ...cssFiles]) {
        const stats = fs.statSync(file);
        const relativePath = path.relative(distPath, file);
        chunks.push({ name: relativePath, size: stats.size });
        totalSize += stats.size;
      }

      return { hasStats: true, totalSize, chunks };
    } catch (error) {
      return { hasStats: false, totalSize: 0, chunks: [] };
    }
  }

  async checkAccessibility(templateContent: string): Promise<{
    violations: string[];
    score: number;
  }> {
    try {
      const dom = new JSDOM(`<!DOCTYPE html><html><body>${templateContent}</body></html>`);
      const { window } = dom;

      // Run axe-core analysis
      const results = await axe.run(window.document.body);

      const violations = results.violations.map(v => `${v.id}: ${v.description}`);
      const score = Math.max(0, 100 - violations.length * 10); // Simple scoring

      return { violations, score };
    } catch (error) {
      return { violations: ['Analysis failed'], score: 0 };
    }
  }
}

// Main MCP Server class
class VueMCPServer {
  private server: Server;
  private analyzer: VueAnalyzer | null = null;

  constructor() {
    this.server = new Server(
      {
        name: 'vue-mcp-server',
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
            name: 'analyze_vue_component',
            description: 'Analyze a Vue component for structure, props, emits, and composition',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the Vue component file',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'find_component_usage',
            description: 'Find all usages of a Vue component across the project',
            inputSchema: {
              type: 'object',
              properties: {
                componentName: {
                  type: 'string',
                  description: 'Name of the component to find',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['componentName', 'workspacePath'],
            },
          },
          {
            name: 'validate_i18n_keys',
            description:
              'Validate i18n key usage in Vue components and check for missing translations',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                componentPath: {
                  type: 'string',
                  description: 'Optional specific component to analyze',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'check_responsive_design',
            description: 'Analyze responsive design implementation using Tailwind CSS classes',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the Vue component file',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'analyze_pinia_store',
            description: 'Analyze Pinia store structure and usage',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the Pinia store file',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'analyze_script_setup',
            description: 'Analyze Vue 3 script setup composition API usage',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the Vue component file',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'analyze_template_ast',
            description: 'Analyze Vue template AST structure and directives',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the Vue component file',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['filePath', 'workspacePath'],
            },
          },
          {
            name: 'validate_i18n_locales',
            description: 'Validate i18n keys against locale files',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                componentPath: {
                  type: 'string',
                  description: 'Optional specific component to analyze',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'analyze_vite_config',
            description: 'Analyze Vite configuration and build setup',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'analyze_bundle_size',
            description: 'Analyze bundle size and chunk information',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'check_accessibility',
            description: 'Check Vue component template for accessibility violations',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the Vue component file',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
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
          case 'analyze_vue_component':
            return await this.handleAnalyzeComponent(
              args as { filePath: string; workspacePath: string }
            );
          case 'find_component_usage':
            return await this.handleFindComponentUsage(
              args as { componentName: string; workspacePath: string }
            );
          case 'validate_i18n_keys':
            return await this.handleValidateI18nKeys(
              args as { workspacePath: string; componentPath?: string }
            );
          case 'check_responsive_design':
            return await this.handleCheckResponsiveDesign(
              args as { filePath: string; workspacePath: string }
            );
          case 'analyze_pinia_store':
            return await this.handleAnalyzePiniaStore(
              args as { filePath: string; workspacePath: string }
            );
          case 'analyze_script_setup':
            return await this.handleAnalyzeScriptSetup(
              args as { filePath: string; workspacePath: string }
            );
          case 'analyze_template_ast':
            return await this.handleAnalyzeTemplateAST(
              args as { filePath: string; workspacePath: string }
            );
          case 'validate_i18n_locales':
            return await this.handleValidateI18nLocales(
              args as { workspacePath: string; componentPath?: string }
            );
          case 'analyze_vite_config':
            return await this.handleAnalyzeViteConfig(args as { workspacePath: string });
          case 'analyze_bundle_size':
            return await this.handleAnalyzeBundleSize(args as { workspacePath: string });
          case 'check_accessibility':
            return await this.handleCheckAccessibility(
              args as { filePath: string; workspacePath: string }
            );
          default:
            throw new McpError(ErrorCode.MethodNotFound, `Unknown tool: ${name}`);
        }
      } catch (error) {
        throw new McpError(
          ErrorCode.InternalError,
          `Tool execution failed: ${error instanceof Error ? error.message : String(error)}`
        );
      }
    });
  }

  private async handleAnalyzeComponent(args: { filePath: string; workspacePath: string }) {
    const { filePath, workspacePath } = args;
    validateWorkspacePath(workspacePath);
    validateFilePath(filePath, workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const descriptor = await this.analyzer.parseVueComponent(filePath);
    const analysis = this.analyzer.analyzeComponentStructure(descriptor);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(analysis, null, 2),
        },
      ],
    };
  }

  private async handleFindComponentUsage(args: { componentName: string; workspacePath: string }) {
    const { componentName, workspacePath } = args;
    validateString(componentName, 'componentName', 1, 100);
    validateWorkspacePath(workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const vueFiles = await this.analyzer.findVueFiles();

    const usages: { file: string; lines: number[] }[] = [];

    for (const file of vueFiles) {
      const content = fs.readFileSync(file, 'utf-8');
      const lines = content.split('\n');
      const matchingLines: number[] = [];

      lines.forEach((line, index) => {
        if (
          line.includes(`<${componentName}`) ||
          line.includes(`componentName="${componentName}"`)
        ) {
          matchingLines.push(index + 1);
        }
      });

      if (matchingLines.length > 0) {
        usages.push({
          file: path.relative(workspacePath, file),
          lines: matchingLines,
        });
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Found ${usages.length} files using component "${componentName}":\n${JSON.stringify(usages, null, 2)}`,
        },
      ],
    };
  }

  private async handleValidateI18nKeys(args: { workspacePath: string; componentPath?: string }) {
    const { workspacePath, componentPath } = args;
    validateWorkspacePath(workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const files = componentPath
      ? [path.resolve(workspacePath, componentPath)]
      : await this.analyzer.findVueFiles();

    const results: { file: string; keys: string[]; issues: string[] }[] = [];

    for (const file of files) {
      const content = fs.readFileSync(file, 'utf-8');
      const keys = this.analyzer.extractI18nKeys(content);
      const issues: string[] = [];

      // Check for hardcoded strings (basic check)
      const hardcodedRegex = />[^<{$]*[A-Z][a-z]+.*</g;
      const hardcodedMatches = content.match(hardcodedRegex);
      if (hardcodedMatches) {
        issues.push('Potential hardcoded strings found');
      }

      results.push({
        file: path.relative(workspacePath, file),
        keys,
        issues,
      });
    }

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(results, null, 2),
        },
      ],
    };
  }

  private async handleCheckResponsiveDesign(args: { filePath: string; workspacePath: string }) {
    const { filePath, workspacePath } = args;
    validateWorkspacePath(workspacePath);
    validateFilePath(filePath, workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const content = fs.readFileSync(filePath, 'utf-8');
    const responsive = this.analyzer.analyzeResponsiveClasses(content);

    return {
      content: [
        {
          type: 'text',
          text: `Responsive Design Analysis:\nBreakpoints used: ${responsive.breakpoints.join(', ')}\nUtilities: ${responsive.utilities.join(', ')}`,
        },
      ],
    };
  }

  private async handleAnalyzePiniaStore(args: { filePath: string; workspacePath: string }) {
    const { filePath, workspacePath } = args;
    validateWorkspacePath(workspacePath);
    validateFilePath(filePath, workspacePath);

    const content = fs.readFileSync(filePath, 'utf-8');

    // Basic analysis for Pinia store patterns
    const hasDefineStore = content.includes('defineStore');
    const hasState = /state:\s*\(/g.test(content);
    const hasGetters = /getters:\s*\{/g.test(content);
    const hasActions = /actions:\s*\{/g.test(content);

    const analysis = {
      isPiniaStore: hasDefineStore,
      hasState,
      hasGetters,
      hasActions,
      file: path.relative(workspacePath, filePath),
    };

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(analysis, null, 2),
        },
      ],
    };
  }

  private async handleAnalyzeScriptSetup(args: { filePath: string; workspacePath: string }) {
    const { filePath, workspacePath } = args;
    validateWorkspacePath(workspacePath);
    validateFilePath(filePath, workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const descriptor = await this.analyzer.parseVueComponent(filePath);
    const scriptSetupAnalysis = this.analyzer.analyzeScriptSetup(descriptor);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(scriptSetupAnalysis, null, 2),
        },
      ],
    };
  }

  private async handleAnalyzeTemplateAST(args: { filePath: string; workspacePath: string }) {
    const { filePath, workspacePath } = args;
    validateWorkspacePath(workspacePath);
    validateFilePath(filePath, workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const descriptor = await this.analyzer.parseVueComponent(filePath);
    const templateASTAnalysis = this.analyzer.analyzeTemplateAST(descriptor);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(templateASTAnalysis, null, 2),
        },
      ],
    };
  }

  private async handleValidateI18nLocales(args: { workspacePath: string; componentPath?: string }) {
    const { workspacePath, componentPath } = args;
    validateWorkspacePath(workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const files = componentPath
      ? [path.resolve(workspacePath, componentPath)]
      : await this.analyzer.findVueFiles();

    const results: { file: string; keys: string[]; validation: any }[] = [];

    for (const file of files) {
      const content = fs.readFileSync(file, 'utf-8');
      const keys = this.analyzer.extractI18nKeys(content);
      const validation = await this.analyzer.validateI18nKeysAgainstLocales(workspacePath, keys);

      results.push({
        file: path.relative(workspacePath, file),
        keys,
        validation,
      });
    }

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(results, null, 2),
        },
      ],
    };
  }

  private async handleAnalyzeViteConfig(args: { workspacePath: string }) {
    const { workspacePath } = args;
    validateWorkspacePath(workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const viteAnalysis = await this.analyzer.analyzeViteConfig(workspacePath);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(viteAnalysis, null, 2),
        },
      ],
    };
  }

  private async handleAnalyzeBundleSize(args: { workspacePath: string }) {
    const { workspacePath } = args;
    validateWorkspacePath(workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const bundleAnalysis = await this.analyzer.analyzeBundleSize(workspacePath);

    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(bundleAnalysis, null, 2),
        },
      ],
    };
  }

  private async handleCheckAccessibility(args: { filePath: string; workspacePath: string }) {
    const { filePath, workspacePath } = args;
    validateWorkspacePath(workspacePath);
    validateFilePath(filePath, workspacePath);

    this.analyzer = new VueAnalyzer(workspacePath);
    const descriptor = await this.analyzer.parseVueComponent(filePath);
    const templateContent = descriptor.template?.content || '';

    const accessibilityAnalysis = await this.analyzer.checkAccessibility(templateContent);

    return {
      content: [
        {
          type: 'text',
          text: `Accessibility Analysis:\nScore: ${accessibilityAnalysis.score}/100\nViolations: ${accessibilityAnalysis.violations.join(', ')}`,
        },
      ],
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Vue MCP Server running...');
  }
}

// Start the server
const server = new VueMCPServer();
server.run().catch(console.error);
