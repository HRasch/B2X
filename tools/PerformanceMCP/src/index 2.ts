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
import * as acorn from 'acorn';
import * as walk from 'acorn-walk';

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

class PerformanceMCPServer {
  private server: Server;

  constructor() {
    this.server = new Server(
      {
        name: 'performance-mcp-server',
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
            name: 'analyze_code_performance',
            description:
              'Analyze code for performance issues, bottlenecks, and optimization opportunities',
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
                language: {
                  type: 'string',
                  enum: ['typescript', 'javascript', 'csharp', 'python'],
                  description: 'Programming language to analyze',
                  default: 'typescript',
                },
                focus: {
                  type: 'string',
                  enum: ['all', 'loops', 'memory', 'async', 'algorithms'],
                  description: 'Specific performance aspect to focus on',
                  default: 'all',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'profile_memory_usage',
            description:
              'Analyze code for memory leaks, inefficient allocations, and memory optimization opportunities',
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
                language: {
                  type: 'string',
                  enum: ['typescript', 'javascript', 'csharp', 'python'],
                  description: 'Programming language to analyze',
                  default: 'typescript',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'optimize_loops',
            description: 'Analyze loops for performance issues and suggest optimizations',
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
                language: {
                  type: 'string',
                  enum: ['typescript', 'javascript', 'csharp', 'python'],
                  description: 'Programming language to analyze',
                  default: 'typescript',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'check_bundle_size',
            description: 'Analyze JavaScript/TypeScript bundle sizes and suggest optimizations',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory',
                },
                buildConfig: {
                  type: 'string',
                  description: 'Path to build configuration file (optional)',
                },
                targetSize: {
                  type: 'number',
                  description: 'Target bundle size in KB (optional)',
                  default: 500,
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'performance_benchmarks',
            description: 'Generate performance benchmarks and compare implementations',
            inputSchema: {
              type: 'object',
              properties: {
                codeSnippet: {
                  type: 'string',
                  description: 'Code snippet to benchmark',
                },
                language: {
                  type: 'string',
                  enum: ['typescript', 'javascript', 'csharp', 'python'],
                  description: 'Programming language',
                  default: 'typescript',
                },
                iterations: {
                  type: 'number',
                  description: 'Number of benchmark iterations',
                  default: 1000,
                },
              },
              required: ['codeSnippet'],
            },
          },
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'analyze_code_performance':
            return await this.analyzeCodePerformance(args);
          case 'profile_memory_usage':
            return await this.profileMemoryUsage(args);
          case 'optimize_loops':
            return await this.optimizeLoops(args);
          case 'check_bundle_size':
            return await this.checkBundleSize(args);
          case 'performance_benchmarks':
            return await this.performanceBenchmarks(args);
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

  private async analyzeCodePerformance(args: any) {
    const sourcePath = args.sourcePath ? validateString(args.sourcePath, 'sourcePath', 1, 500) : '';
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const language = args.language || 'typescript';
    const focus = args.focus || 'all';

    const issues: string[] = [];
    const suggestions: string[] = [];
    const metrics: { [key: string]: any } = {};

    // Find source files
    const sourceFiles = await this.findSourceFiles(workspacePath, sourcePath, language);

    for (const file of sourceFiles.slice(0, 10)) {
      // Limit to 10 files
      try {
        const content = fs.readFileSync(file, 'utf8');
        const analysis = this.analyzeFilePerformance(content, language, focus);
        issues.push(...analysis.issues.map(issue => `${path.basename(file)}: ${issue}`));
        suggestions.push(...analysis.suggestions.map(sugg => `${path.basename(file)}: ${sugg}`));

        // Aggregate metrics
        if (analysis.metrics) {
          Object.keys(analysis.metrics).forEach(key => {
            if (!metrics[key]) metrics[key] = 0;
            metrics[key] += analysis.metrics[key];
          });
        }
      } catch (error) {
        issues.push(`${path.basename(file)}: Could not analyze - ${(error as Error).message}`);
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Performance Analysis Results (${language}, focus: ${focus}):\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nSuggestions (${suggestions.length}):\n${suggestions.map(sugg => `• ${sugg}`).join('\n')}\n\nMetrics:\n${Object.entries(
            metrics
          )
            .map(([key, value]) => `• ${key}: ${value}`)
            .join('\n')}`,
        },
      ],
    };
  }

  private analyzeFilePerformance(
    content: string,
    language: string,
    focus: string
  ): { issues: string[]; suggestions: string[]; metrics: { [key: string]: any } } {
    const issues: string[] = [];
    const suggestions: string[] = [];
    const metrics: { [key: string]: any } = {};

    if (language === 'typescript' || language === 'javascript') {
      return this.analyzeJSPerformance(content, focus);
    } else if (language === 'csharp') {
      return this.analyzeCSharpPerformance(content, focus);
    } else if (language === 'python') {
      return this.analyzePythonPerformance(content, focus);
    }

    return { issues, suggestions, metrics };
  }

  private analyzeJSPerformance(
    content: string,
    focus: string
  ): { issues: string[]; suggestions: string[]; metrics: { [key: string]: any } } {
    const issues: string[] = [];
    const suggestions: string[] = [];
    const metrics: { [key: string]: any } = {};

    try {
      const ast = acorn.parse(content, {
        ecmaVersion: 2022,
        sourceType: 'module',
        allowImportExportEverywhere: true,
      });

      // Count various constructs
      let loopCount = 0;
      let asyncCount = 0;
      let nestedLoopDepth = 0;
      let maxNestedDepth = 0;

      walk.simple(ast, {
        ForStatement(node: any) {
          loopCount++;
          if (focus === 'all' || focus === 'loops') {
            suggestions.push('Consider using for-of loops or array methods for better performance');
          }
        },
        ForInStatement(node: any) {
          loopCount++;
        },
        ForOfStatement(node: any) {
          loopCount++;
        },
        WhileStatement(node: any) {
          loopCount++;
        },
        DoWhileStatement(node: any) {
          loopCount++;
        },
        FunctionDeclaration(node: any) {
          if (node.async) asyncCount++;
        },
        ArrowFunctionExpression(node: any) {
          if (node.async) asyncCount++;
        },
        FunctionExpression(node: any) {
          if (node.async) asyncCount++;
        },
      });

      metrics.loops = loopCount;
      metrics.asyncFunctions = asyncCount;

      // Performance issues
      if (content.includes('for (let i = 0; i < arr.length; i++)')) {
        issues.push('Cache array length in loop condition for better performance');
        suggestions.push('Use: for (let i = 0, len = arr.length; i < len; i++)');
      }

      if (content.includes('.forEach(') && loopCount > 0) {
        suggestions.push('Consider using for-of loops instead of forEach for better performance');
      }

      if (
        content.includes('console.log') &&
        !content.includes('NODE_ENV') !== !content.includes('production')
      ) {
        issues.push('Console statements left in production code');
        suggestions.push('Remove console.log statements or use proper logging');
      }

      if (focus === 'memory' || focus === 'all') {
        if (content.includes('new Array(') && content.includes('.fill(')) {
          suggestions.push('Consider using Array.from() or spread operator for array creation');
        }

        if (content.includes('JSON.parse(JSON.stringify(')) {
          issues.push('Deep cloning with JSON.parse/stringify is inefficient');
          suggestions.push('Use structuredClone() or implement proper deep clone');
        }
      }

      if (focus === 'async' || focus === 'all') {
        if (content.includes('await') && content.includes('Promise.all(') === false) {
          suggestions.push('Consider using Promise.all() for concurrent async operations');
        }
      }
    } catch (error) {
      issues.push(`Could not parse JavaScript/TypeScript: ${(error as Error).message}`);
    }

    return { issues, suggestions, metrics };
  }

  private analyzeCSharpPerformance(
    content: string,
    focus: string
  ): { issues: string[]; suggestions: string[]; metrics: { [key: string]: any } } {
    const issues: string[] = [];
    const suggestions: string[] = [];
    const metrics: { [key: string]: any } = {};

    const lines = content.split('\n');
    let loopCount = 0;
    let linqCount = 0;

    for (const line of lines) {
      const trimmed = line.trim();

      // Count loops
      if (
        trimmed.startsWith('for ') ||
        trimmed.startsWith('foreach ') ||
        trimmed.startsWith('while ') ||
        trimmed.startsWith('do ')
      ) {
        loopCount++;
      }

      // Count LINQ operations
      if (
        trimmed.includes('.Where(') ||
        trimmed.includes('.Select(') ||
        trimmed.includes('.First(') ||
        trimmed.includes('.Any(')
      ) {
        linqCount++;
      }
    }

    metrics.loops = loopCount;
    metrics.linqOperations = linqCount;

    // Performance issues
    if (content.includes('.ToList()') && content.includes('.Where(')) {
      issues.push('Calling ToList() before Where() forces full enumeration');
      suggestions.push('Use AsQueryable() or move ToList() after filtering');
    }

    if (content.includes('StringBuilder') === false && content.includes('+=')) {
      suggestions.push('Consider using StringBuilder for string concatenation in loops');
    }

    if (focus === 'memory' || focus === 'all') {
      if (content.includes('new List<') && content.includes('.Add(')) {
        suggestions.push('Consider using collection initializer or LINQ for better performance');
      }
    }

    return { issues, suggestions, metrics };
  }

  private analyzePythonPerformance(
    content: string,
    focus: string
  ): { issues: string[]; suggestions: string[]; metrics: { [key: string]: any } } {
    const issues: string[] = [];
    const suggestions: string[] = [];
    const metrics: { [key: string]: any } = {};

    const lines = content.split('\n');
    let loopCount = 0;
    let listCompCount = 0;

    for (const line of lines) {
      const trimmed = line.trim();

      // Count loops
      if (trimmed.startsWith('for ') || trimmed.startsWith('while ')) {
        loopCount++;
      }

      // Count list comprehensions
      if (trimmed.includes('[') && trimmed.includes('for ') && trimmed.includes('in ')) {
        listCompCount++;
      }
    }

    metrics.loops = loopCount;
    metrics.listComprehensions = listCompCount;

    // Performance issues
    if (content.includes('range(len(')) {
      issues.push('Using range(len()) is less efficient than enumerate()');
      suggestions.push('Use enumerate() instead of range(len()) for indexing');
    }

    if (content.includes('.append(') && loopCount > 0) {
      suggestions.push('Consider using list comprehensions for better performance');
    }

    if (focus === 'memory' || focus === 'all') {
      if (content.includes('global ') && loopCount > 0) {
        issues.push('Using global variables in loops can be slow');
        suggestions.push('Avoid global variable access in performance-critical loops');
      }
    }

    return { issues, suggestions, metrics };
  }

  private async findSourceFiles(
    workspacePath: string,
    sourcePath?: string,
    language?: string
  ): Promise<string[]> {
    const searchPath = sourcePath ? path.join(workspacePath, sourcePath) : workspacePath;

    const patterns: string[] = [];
    if (language === 'typescript') {
      patterns.push('**/*.ts', '!**/*.d.ts');
    } else if (language === 'javascript') {
      patterns.push('**/*.js');
    } else if (language === 'csharp') {
      patterns.push('**/*.cs');
    } else if (language === 'python') {
      patterns.push('**/*.py');
    } else {
      patterns.push('**/*.ts', '**/*.js', '**/*.cs', '**/*.py');
    }

    const files: string[] = [];
    for (const pattern of patterns) {
      try {
        const matches = await glob(pattern, { cwd: searchPath, absolute: true });
        files.push(
          ...matches.filter(
            f => !f.includes('node_modules') && !f.includes('dist') && !f.includes('bin')
          )
        );
      } catch (error) {
        // Skip patterns that don't match
      }
    }

    return [...new Set(files)]; // Remove duplicates
  }

  private async profileMemoryUsage(args: any) {
    const sourcePath = args.sourcePath ? validateString(args.sourcePath, 'sourcePath', 1, 500) : '';
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const language = args.language || 'typescript';

    const issues: string[] = [];
    const suggestions: string[] = [];

    // Find source files
    const sourceFiles = await this.findSourceFiles(workspacePath, sourcePath, language);

    for (const file of sourceFiles.slice(0, 10)) {
      // Limit to 10 files
      try {
        const content = fs.readFileSync(file, 'utf8');
        const analysis = this.analyzeMemoryUsage(content, language);
        issues.push(...analysis.issues.map(issue => `${path.basename(file)}: ${issue}`));
        suggestions.push(...analysis.suggestions.map(sugg => `${path.basename(file)}: ${sugg}`));
      } catch (error) {
        issues.push(`${path.basename(file)}: Could not analyze - ${(error as Error).message}`);
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Memory Usage Analysis Results (${language}):\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nSuggestions (${suggestions.length}):\n${suggestions.map(sugg => `• ${sugg}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeMemoryUsage(
    content: string,
    language: string
  ): { issues: string[]; suggestions: string[] } {
    const issues: string[] = [];
    const suggestions: string[] = [];

    if (language === 'typescript' || language === 'javascript') {
      if (content.includes('addEventListener') && !content.includes('removeEventListener')) {
        issues.push('Event listeners added without cleanup - potential memory leak');
        suggestions.push('Always remove event listeners in cleanup functions');
      }

      if (content.includes('setInterval') && !content.includes('clearInterval')) {
        issues.push('setInterval without clearInterval - potential memory leak');
        suggestions.push('Store interval IDs and clear them when component unmounts');
      }

      if (
        content.includes('setTimeout') &&
        content.includes('setTimeout') &&
        !content.includes('clearTimeout')
      ) {
        suggestions.push('Consider using clearTimeout for long-running timeouts');
      }

      if (content.includes('new ') && content.includes('[]') && content.includes('.push(')) {
        suggestions.push('Consider pre-allocating array size if known');
      }
    } else if (language === 'csharp') {
      if (content.includes('new ') && !content.includes('using ') && !content.includes('Dispose')) {
        suggestions.push(
          'Consider implementing IDisposable for objects that hold unmanaged resources'
        );
      }

      if (
        content.includes('Subscribe') &&
        !content.includes('Unsubscribe') &&
        !content.includes('Dispose')
      ) {
        issues.push('Observable subscriptions without disposal - potential memory leak');
        suggestions.push('Use CompositeDisposable or unsubscribe in Dispose method');
      }
    }

    return { issues, suggestions };
  }

  private async optimizeLoops(args: any) {
    const sourcePath = args.sourcePath ? validateString(args.sourcePath, 'sourcePath', 1, 500) : '';
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const language = args.language || 'typescript';

    const optimizations: string[] = [];
    const issues: string[] = [];

    // Find source files
    const sourceFiles = await this.findSourceFiles(workspacePath, sourcePath, language);

    for (const file of sourceFiles.slice(0, 10)) {
      // Limit to 10 files
      try {
        const content = fs.readFileSync(file, 'utf8');
        const analysis = this.analyzeLoopOptimizations(content, language);
        optimizations.push(...analysis.optimizations.map(opt => `${path.basename(file)}: ${opt}`));
        issues.push(...analysis.issues.map(issue => `${path.basename(file)}: ${issue}`));
      } catch (error) {
        issues.push(`${path.basename(file)}: Could not analyze - ${(error as Error).message}`);
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Loop Optimization Analysis (${language}):\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nOptimizations (${optimizations.length}):\n${optimizations.map(opt => `• ${opt}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeLoopOptimizations(
    content: string,
    language: string
  ): { optimizations: string[]; issues: string[] } {
    const optimizations: string[] = [];
    const issues: string[] = [];

    if (language === 'typescript' || language === 'javascript') {
      if (content.includes('for (let i = 0; i < arr.length; i++)')) {
        optimizations.push(
          'Cache array.length outside loop: for (let i = 0, len = arr.length; i < len; i++)'
        );
      }

      if (content.includes('.forEach(')) {
        optimizations.push('Consider for-of loops for better performance than forEach');
      }

      if (content.includes('for (let i in arr)')) {
        issues.push('for-in loops are slow for arrays, use for-of instead');
        optimizations.push('Replace for-in with for-of: for (const item of arr)');
      }
    } else if (language === 'csharp') {
      if (content.includes('for (int i = 0; i < list.Count; i++)')) {
        optimizations.push(
          'Cache Count property: int count = list.Count; for (int i = 0; i < count; i++)'
        );
      }

      if (content.includes('foreach ') && content.includes('.Count')) {
        optimizations.push('Consider for loop with cached count for better performance');
      }
    } else if (language === 'python') {
      if (content.includes('for i in range(len(')) {
        optimizations.push('Use enumerate() instead of range(len()) for better performance');
      }

      if (content.includes('.append(') && content.includes('for ')) {
        optimizations.push('Use list comprehensions instead of append in loops');
      }
    }

    return { optimizations, issues };
  }

  private async checkBundleSize(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const buildConfig = args.buildConfig
      ? validateString(args.buildConfig, 'buildConfig', 1, 500)
      : null;
    const targetSize = args.targetSize || 500; // KB

    const analysis: string[] = [];

    // Look for build configuration files
    const configFiles = [
      'package.json',
      'vite.config.ts',
      'vite.config.js',
      'webpack.config.js',
      'rollup.config.js',
    ];

    let foundConfig = false;
    for (const configFile of configFiles) {
      const configPath = path.join(workspacePath, configFile);
      if (fs.existsSync(configPath)) {
        foundConfig = true;
        try {
          const content = fs.readFileSync(configPath, 'utf8');
          const bundleAnalysis = this.analyzeBundleConfig(content, configFile);
          analysis.push(...bundleAnalysis);
        } catch (error) {
          analysis.push(`Could not analyze ${configFile}: ${(error as Error).message}`);
        }
      }
    }

    if (!foundConfig) {
      analysis.push('No build configuration found - looking for common patterns in source code');
    }

    // Look for bundle size indicators in source
    const sourceFiles = await this.findSourceFiles(workspacePath, '', 'typescript');
    for (const file of sourceFiles.slice(0, 5)) {
      try {
        const content = fs.readFileSync(file, 'utf8');
        const sizeAnalysis = this.analyzeSourceForBundleSize(content, path.basename(file));
        analysis.push(...sizeAnalysis);
      } catch (error) {
        // Skip files that can't be read
      }
    }

    analysis.push(`Target bundle size: ${targetSize}KB`);
    analysis.push('Note: Actual bundle size analysis requires running the build process');

    return {
      content: [
        {
          type: 'text',
          text: `Bundle Size Analysis:\n\n${analysis.map(item => `• ${item}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeBundleConfig(content: string, configFile: string): string[] {
    const analysis: string[] = [];

    if (configFile === 'package.json') {
      if (content.includes('"vite"')) {
        analysis.push('Using Vite - generally good for bundle optimization');
      }
      if (content.includes('"webpack"')) {
        analysis.push('Using Webpack - ensure proper chunk splitting and tree shaking');
      }
    }

    if (configFile.includes('vite.config')) {
      if (!content.includes('build:')) {
        analysis.push('Consider configuring Vite build options for better optimization');
      }
      if (content.includes('minify:')) {
        analysis.push('Minification is enabled - good for bundle size');
      }
    }

    return analysis;
  }

  private analyzeSourceForBundleSize(content: string, fileName: string): string[] {
    const analysis: string[] = [];

    // Count imports (rough indicator of dependencies)
    const importMatches = content.match(/import\s+.*from\s+['"]/g) || [];
    if (importMatches.length > 20) {
      analysis.push(
        `${fileName}: High number of imports (${importMatches.length}) - consider lazy loading`
      );
    }

    // Check for large inline data
    if (content.includes('data:image') || content.length > 50000) {
      analysis.push(`${fileName}: Large file detected - consider code splitting`);
    }

    return analysis;
  }

  private async performanceBenchmarks(args: any) {
    const codeSnippet = validateString(args.codeSnippet, 'codeSnippet', 1, 5000);
    const language = args.language || 'typescript';
    const iterations = args.iterations || 1000;

    const results: string[] = [];

    // This is a simplified benchmark - in reality, you'd run the code multiple times
    results.push(`Benchmarking ${language} code snippet`);
    results.push(`Iterations: ${iterations}`);
    results.push(`Code length: ${codeSnippet.length} characters`);

    // Basic analysis
    if (language === 'typescript' || language === 'javascript') {
      if (codeSnippet.includes('for ') && codeSnippet.includes('.length')) {
        results.push('⚠️  Potential performance issue: Array length accessed in loop condition');
        results.push('💡 Suggestion: Cache array length outside loop');
      }

      if (codeSnippet.includes('console.log')) {
        results.push('⚠️  Console statements in benchmark code may affect performance');
      }
    }

    results.push(
      'Note: This is a static analysis. For accurate benchmarks, run the code in a proper benchmarking environment.'
    );

    return {
      content: [
        {
          type: 'text',
          text: `Performance Benchmark Analysis:\n\n${results.map(result => `• ${result}`).join('\n')}`,
        },
      ],
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Performance MCP server running...');
  }
}

// Start the server
const server = new PerformanceMCPServer();
server.run().catch(console.error);
