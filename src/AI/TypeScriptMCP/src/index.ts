#!/usr/bin/env node

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from '@modelcontextprotocol/sdk/types.js';
import * as ts from 'typescript';
import * as path from 'path';

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

  // Basic path validation - should contain typical project structure
  if (!path.includes('/') && !path.includes('\\')) {
    throw new ValidationError('workspacePath should be a directory path');
  }

  return path;
}

function validateSymbolName(symbolName: any): string {
  const name = validateString(symbolName, 'symbolName', 1, 200);

  // Basic symbol name validation (TypeScript identifiers)
  if (!/^[a-zA-Z_$][a-zA-Z0-9_$]*$/.test(name)) {
    throw new ValidationError('symbolName must be a valid TypeScript identifier');
  }

  return name;
}

function validateFilePath(filePath: any, workspacePath: string): string {
  const filePathStr = validateString(filePath, 'filePath', 1, 500);

  // Ensure file path is within workspace
  if (!filePathStr.startsWith(workspacePath) && !filePathStr.includes(workspacePath)) {
    throw new ValidationError('filePath must be within the workspace directory');
  }

  // Prevent path traversal
  if (filePathStr.includes('..')) {
    throw new ValidationError('filePath cannot contain .. (path traversal protection)');
  }

  return filePathStr;
}

// Export validation functions for testing
export {
  ValidationError,
  validateString,
  validateWorkspacePath,
  validateSymbolName,
  validateFilePath,
};

// Configuration provider for different project types
class ProjectConfigurationProvider {
  static getConfigurationStrategy(rootDir: string): {
    fileNames: string[];
    options: ts.CompilerOptions;
  } {
    const configPath = ts.findConfigFile(rootDir, ts.sys.fileExists, 'tsconfig.json');
    if (!configPath) {
      throw new Error(`Could not find tsconfig.json in ${rootDir}`);
    }

    const configFile = ts.readConfigFile(configPath, ts.sys.readFile);
    if (configFile.error) {
      throw new Error(`Error reading tsconfig.json: ${configFile.error.messageText}`);
    }

    const parsedConfig = ts.parseJsonConfigFileContent(configFile.config, ts.sys, rootDir);

    if (parsedConfig.errors.length > 0) {
      throw new Error(
        `Error parsing tsconfig.json: ${parsedConfig.errors.map(e => e.messageText).join(', ')}`
      );
    }

    let fileNames = parsedConfig.fileNames;

    // Strategy: Handle different project types
    if (this.isNuxtProject(fileNames, rootDir)) {
      fileNames = this.findTypeScriptFilesFallback(rootDir);
    } else if (this.isViteProject(rootDir)) {
      fileNames = this.findTypeScriptFilesFallback(rootDir);
    } else if (fileNames.length === 0) {
      fileNames = this.findTypeScriptFilesFallback(rootDir);
    }

    return { fileNames, options: parsedConfig.options };
  }

  private static isNuxtProject(fileNames: string[], rootDir: string): boolean {
    return (
      fileNames.some(f => f.includes('.nuxt')) ||
      ts.sys.fileExists(path.join(rootDir, 'nuxt.config.ts')) ||
      ts.sys.fileExists(path.join(rootDir, 'nuxt.config.js'))
    );
  }

  private static isViteProject(rootDir: string): boolean {
    return (
      ts.sys.fileExists(path.join(rootDir, 'vite.config.ts')) ||
      ts.sys.fileExists(path.join(rootDir, 'vite.config.js'))
    );
  }

  private static findTypeScriptFilesFallback(rootDir: string): string[] {
    const files: string[] = [];

    const visit = (dir: string) => {
      try {
        const entries = ts.sys.readDirectory(dir, undefined, undefined, ['**/*.ts', '**/*.tsx']);
        for (const entry of entries) {
          const fullPath = path.resolve(dir, entry);
          if (
            ts.sys.fileExists(fullPath) &&
            !fullPath.includes('node_modules') &&
            !fullPath.includes('.nuxt')
          ) {
            files.push(fullPath);
          }
        }
      } catch (error) {
        // Ignore directory read errors
      }
    };

    visit(rootDir);
    return files;
  }
}

class LRUCache<T> {
  private cache: Map<string, { value: T; timestamp: number }> = new Map();
  private maxSize: number;
  private ttlMs: number;

  constructor(maxSize: number = 100, ttlMinutes: number = 30) {
    this.maxSize = maxSize;
    this.ttlMs = ttlMinutes * 60 * 1000;
  }

  get(key: string): T | undefined {
    const entry = this.cache.get(key);
    if (!entry) return undefined;

    // Check if expired
    if (Date.now() - entry.timestamp > this.ttlMs) {
      this.cache.delete(key);
      return undefined;
    }

    // Move to end (most recently used)
    this.cache.delete(key);
    this.cache.set(key, entry);
    return entry.value;
  }

  set(key: string, value: T): void {
    // Remove if already exists
    this.cache.delete(key);

    // Evict oldest if at capacity
    if (this.cache.size >= this.maxSize) {
      const firstKey = this.cache.keys().next().value;
      if (firstKey) {
        this.cache.delete(firstKey);
      }
    }

    this.cache.set(key, { value, timestamp: Date.now() });
  }

  size(): number {
    // Clean expired entries
    const now = Date.now();
    for (const [key, entry] of this.cache.entries()) {
      if (now - entry.timestamp > this.ttlMs) {
        this.cache.delete(key);
      }
    }
    return this.cache.size;
  }

  clear(): void {
    this.cache.clear();
  }
}

export class TypeScriptMCPServer {
  private server: Server;
  private languageService: ts.LanguageService | null = null;
  private program: ts.Program | null = null;
  private host: ts.LanguageServiceHost | null = null;
  private fileVersions: Map<string, number> = new Map();
  private fileContents: LRUCache<string> = new LRUCache<string>(50, 30); // Max 50 files, 30 min TTL

  constructor() {
    this.server = new Server(
      {
        name: 'typescript-mcp-server',
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

  private createLanguageService(rootDir: string): void {
    const { fileNames, options } = ProjectConfigurationProvider.getConfigurationStrategy(rootDir);

    this.host = {
      getCompilationSettings: () => options,
      getScriptFileNames: () => fileNames,
      getScriptVersion: (fileName: string) => {
        return this.fileVersions.get(fileName)?.toString() || '0';
      },
      getScriptSnapshot: (fileName: string) => {
        if (!ts.sys.fileExists(fileName)) {
          return undefined;
        }

        let content = this.fileContents.get(fileName);
        if (content === undefined) {
          content = ts.sys.readFile(fileName);
          if (content === undefined) {
            return undefined;
          }
          this.fileContents.set(fileName, content);
        }

        return ts.ScriptSnapshot.fromString(content);
      },
      getCurrentDirectory: () => rootDir,
      getDefaultLibFileName: (options: ts.CompilerOptions) => ts.getDefaultLibFilePath(options),
      fileExists: ts.sys.fileExists,
      readFile: ts.sys.readFile,
      readDirectory: ts.sys.readDirectory,
      directoryExists: ts.sys.directoryExists,
      getDirectories: ts.sys.getDirectories,
    };

    this.program = ts.createProgram({
      rootNames: fileNames,
      options: options,
    });

    this.languageService = ts.createLanguageService(this.host);
  }

  private ensureLanguageService(workspacePath: string): void {
    if (this.languageService) {
      return;
    }

    // Find the project root (look for tsconfig.json)
    let rootDir = workspacePath;
    const fileDir = workspacePath;

    // Try to find tsconfig.json in the workspace directory
    const configPath = path.join(rootDir, 'tsconfig.json');
    if (!ts.sys.fileExists(configPath)) {
      throw new Error(`Could not find tsconfig.json in ${rootDir}`);
    }

    this.createLanguageService(rootDir);
  }

  private setupToolHandlers() {
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'search_symbols',
            description:
              'Search for TypeScript symbols (classes, interfaces, functions) using patterns',
            inputSchema: {
              type: 'object',
              properties: {
                pattern: {
                  type: 'string',
                  description: 'Search pattern (supports wildcards like *Service)',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory containing tsconfig.json',
                },
                filePath: {
                  type: 'string',
                  description: 'Optional file path to search in',
                },
              },
              required: ['pattern', 'workspacePath'],
            },
          },
          {
            name: 'get_symbol_info',
            description: 'Get detailed information about a TypeScript symbol',
            inputSchema: {
              type: 'object',
              properties: {
                symbolName: {
                  type: 'string',
                  description: 'Name of the symbol',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory containing tsconfig.json',
                },
                filePath: {
                  type: 'string',
                  description: 'File path containing the symbol',
                },
                line: {
                  type: 'number',
                  description: 'Line number of the symbol',
                },
                character: {
                  type: 'number',
                  description: 'Character position of the symbol',
                },
              },
              required: ['symbolName', 'workspacePath', 'filePath'],
            },
          },
          {
            name: 'find_usages',
            description: 'Find all usages of a TypeScript symbol',
            inputSchema: {
              type: 'object',
              properties: {
                symbolName: {
                  type: 'string',
                  description: 'Name of the symbol to find usages for',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory containing tsconfig.json',
                },
                filePath: {
                  type: 'string',
                  description: 'File path containing the symbol',
                },
              },
              required: ['symbolName', 'workspacePath', 'filePath'],
            },
          },
          {
            name: 'analyze_types',
            description: 'Analyze TypeScript types and report errors',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Workspace root directory containing tsconfig.json',
                },
                filePath: {
                  type: 'string',
                  description: 'File path to analyze',
                },
              },
              required: ['workspacePath', 'filePath'],
            },
          },
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'search_symbols':
            return await this.handleSearchSymbols(
              args as { pattern: string; workspacePath: string; filePath?: string }
            );
          case 'get_symbol_info':
            return await this.handleGetSymbolInfo(
              args as {
                symbolName: string;
                workspacePath: string;
                filePath: string;
                line?: number;
                character?: number;
              }
            );
          case 'find_usages':
            return await this.handleFindUsages(
              args as { symbolName: string; workspacePath: string; filePath: string }
            );
          case 'analyze_types':
            return await this.handleAnalyzeTypes(
              args as { workspacePath: string; filePath: string }
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

  private async handleSearchSymbols(args: {
    pattern: string;
    workspacePath: string;
    filePath?: string;
  }) {
    try {
      // Input validation
      const pattern = validateString(args.pattern, 'pattern', 1, 100);
      const workspacePath = validateWorkspacePath(args.workspacePath);
      let filePath: string | undefined;
      if (args.filePath) {
        filePath = validateFilePath(args.filePath, workspacePath);
      }

      this.ensureLanguageService(workspacePath);

      if (!this.languageService || !this.program) {
        throw new Error('Language service not initialized');
      }

      const sourceFiles = filePath
        ? [this.program.getSourceFile(filePath)]
        : this.program
            .getSourceFiles()
            .filter(
              file =>
                file.fileName.includes(workspacePath) &&
                !file.fileName.includes('node_modules') &&
                !file.fileName.includes('.nuxt')
            );

      const results: Array<{
        name: string;
        kind: string;
        file: string;
        line: number;
        character: number;
      }> = [];

      for (const sourceFile of sourceFiles) {
        if (!sourceFile) continue;

        const visit = (node: ts.Node | undefined) => {
          if (!node) return; // Skip undefined nodes

          try {
            if (ts.isClassDeclaration(node) && node.name && ts.isIdentifier(node.name)) {
              this.checkSymbolMatch(node.name, 'class', sourceFile, results, pattern);
            } else if (ts.isInterfaceDeclaration(node) && node.name && ts.isIdentifier(node.name)) {
              this.checkSymbolMatch(node.name, 'interface', sourceFile, results, pattern);
            } else if (ts.isFunctionDeclaration(node) && node.name && ts.isIdentifier(node.name)) {
              this.checkSymbolMatch(node.name, 'function', sourceFile, results, pattern);
            } else if (ts.isMethodDeclaration(node) && node.name && ts.isIdentifier(node.name)) {
              this.checkSymbolMatch(node.name, 'method', sourceFile, results, pattern);
            } else if (ts.isPropertyDeclaration(node) && node.name && ts.isIdentifier(node.name)) {
              this.checkSymbolMatch(node.name, 'property', sourceFile, results, pattern);
            } else if (ts.isVariableDeclaration(node) && node.name && ts.isIdentifier(node.name)) {
              this.checkSymbolMatch(node.name, 'variable', sourceFile, results, pattern);
            }
          } catch (error) {
            // Skip nodes that cause errors during processing
            ts.forEachChild(node, visit);
          }

          ts.forEachChild(node, visit);
        };

        visit(sourceFile);
      }

      const resultText =
        results.length > 0
          ? `Found ${results.length} symbols matching "${args.pattern}":\n\n` +
            results.map(r => `${r.kind} ${r.name} at ${r.file}:${r.line}:${r.character}`).join('\n')
          : `No symbols found matching pattern "${args.pattern}"`;

      return {
        content: [
          {
            type: 'text',
            text: resultText,
          },
        ],
      };
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `Error searching symbols: ${error instanceof Error ? error.message : String(error)}`,
          },
        ],
      };
    }
  }

  private checkSymbolMatch(
    name: ts.Identifier,
    kind: string,
    sourceFile: ts.SourceFile,
    results: Array<any>,
    pattern: string
  ) {
    if (!name || !name.text) {
      return;
    }

    const symbolName = name.text;
    const matches = this.matchesPattern(symbolName, pattern);

    if (matches) {
      const pos = sourceFile.getLineAndCharacterOfPosition(name.pos);
      results.push({
        name: symbolName,
        kind,
        file: sourceFile.fileName,
        line: pos.line + 1,
        character: pos.character + 1,
      });
    }
  }

  private matchesPattern(symbolName: string, pattern: string): boolean {
    // Simple wildcard matching (*)
    const regexPattern = pattern.replace(/\*/g, '.*').replace(/\?/g, '.');
    const regex = new RegExp(`^${regexPattern}$`, 'i');
    return regex.test(symbolName);
  }

  private async handleGetSymbolInfo(args: {
    symbolName: string;
    workspacePath: string;
    filePath: string;
    line?: number;
    character?: number;
  }) {
    // Input validation
    const symbolName = validateSymbolName(args.symbolName);
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const filePath = validateFilePath(args.filePath, workspacePath);

    try {
      this.ensureLanguageService(workspacePath);

      if (!this.languageService || !this.program) {
        throw new Error('Language service not initialized');
      }

      // Find the source file that contains the symbol
      const sourceFiles = this.program
        .getSourceFiles()
        .filter(
          file =>
            file.fileName.includes(workspacePath) &&
            !file.fileName.includes('node_modules') &&
            !file.fileName.includes('.nuxt')
        );

      let sourceFile: ts.SourceFile | undefined;
      let position: number = -1;

      if (args.line !== undefined && args.character !== undefined) {
        // If line/character provided, find the file and position
        sourceFile = sourceFiles.find(
          file => file.fileName === filePath || file.fileName.endsWith(filePath)
        );
        if (sourceFile) {
          position = sourceFile.getPositionOfLineAndCharacter(args.line - 1, args.character - 1);
        }
      } else {
        // Find the symbol by name across all files
        for (const file of sourceFiles) {
          // Find position by searching for the symbol name in the file text
          const text = file.text;
          const lines = text.split('\n');
          for (let i = 0; i < lines.length; i++) {
            const line = lines[i];
            const index = line.indexOf(symbolName);
            if (index !== -1) {
              // Check if this is likely a declaration (preceded by 'interface', 'class', etc.)
              const beforeText = line.substring(0, index).trim();
              if (
                beforeText === 'interface' ||
                beforeText === 'class' ||
                beforeText === 'type' ||
                beforeText.endsWith(':')
              ) {
                position = file.getPositionOfLineAndCharacter(i, index);
                sourceFile = file;
                break;
              }
            }
          }
          if (position !== -1) break;
        }
      }

      if (!sourceFile || position === -1) {
        throw new Error(`Symbol "${symbolName}" not found`);
      }

      const quickInfo = this.languageService.getQuickInfoAtPosition(sourceFile.fileName, position);
      if (!quickInfo) {
        throw new Error(`No information available for symbol at position`);
      }

      const pos = sourceFile.getLineAndCharacterOfPosition(position);

      const resultText = `Symbol: ${symbolName}
Location: ${sourceFile.fileName}:${pos.line + 1}:${pos.character + 1}
Kind: ${quickInfo.kind}
Type: ${Array.isArray(quickInfo.kindModifiers) ? quickInfo.kindModifiers.join(' ') : quickInfo.kindModifiers}
Documentation: ${quickInfo.documentation ? ts.displayPartsToString(quickInfo.documentation) : 'None'}`;

      return {
        content: [
          {
            type: 'text',
            text: resultText,
          },
        ],
      };
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `Error getting symbol info: ${error instanceof Error ? error.message : String(error)}`,
          },
        ],
      };
    }
  }

  private findSymbolPosition(sourceFile: ts.SourceFile, symbolName: string): number {
    let foundPosition = -1;

    const visit = (node: ts.Node) => {
      if (foundPosition !== -1) return;

      if (
        (ts.isClassDeclaration(node) &&
          node.name &&
          ts.isIdentifier(node.name) &&
          node.name.text === symbolName) ||
        (ts.isInterfaceDeclaration(node) &&
          node.name &&
          ts.isIdentifier(node.name) &&
          node.name.text === symbolName) ||
        (ts.isFunctionDeclaration(node) &&
          node.name &&
          ts.isIdentifier(node.name) &&
          node.name.text === symbolName) ||
        (ts.isMethodDeclaration(node) &&
          node.name &&
          ts.isIdentifier(node.name) &&
          node.name.text === symbolName) ||
        (ts.isPropertyDeclaration(node) &&
          node.name &&
          ts.isIdentifier(node.name) &&
          node.name.text === symbolName) ||
        (ts.isVariableDeclaration(node) &&
          node.name &&
          ts.isIdentifier(node.name) &&
          node.name.text === symbolName)
      ) {
        foundPosition = node.name.pos;
        return;
      }

      ts.forEachChild(node, visit);
    };

    visit(sourceFile);
    return foundPosition;
  }

  private async handleFindUsages(args: {
    symbolName: string;
    workspacePath: string;
    filePath: string;
  }) {
    // Input validation
    const symbolName = validateSymbolName(args.symbolName);
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const filePath = validateFilePath(args.filePath, workspacePath);

    try {
      this.ensureLanguageService(workspacePath);

      if (!this.languageService || !this.program) {
        throw new Error('Language service not initialized');
      }

      const sourceFile = this.program.getSourceFile(filePath);
      if (!sourceFile) {
        throw new Error(`File not found: ${filePath}`);
      }

      const position = this.findSymbolPosition(sourceFile, symbolName);
      if (position === -1) {
        throw new Error(`Symbol "${symbolName}" not found in ${filePath}`);
      }

      const references = this.languageService.getReferencesAtPosition(filePath, position);
      if (!references) {
        return {
          content: [
            {
              type: 'text',
              text: `No references found for symbol "${symbolName}"`,
            },
          ],
        };
      }

      const results = references
        .map(ref => {
          const refFile = this.program!.getSourceFile(ref.fileName);
          if (!refFile) return null;

          const pos = refFile.getLineAndCharacterOfPosition(ref.textSpan.start);
          return `${ref.fileName}:${pos.line + 1}:${pos.character + 1}`;
        })
        .filter(Boolean);

      const resultText = `Found ${results.length} references to "${symbolName}":\n\n${results.join('\n')}`;

      return {
        content: [
          {
            type: 'text',
            text: resultText,
          },
        ],
      };
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `Error finding usages: ${error instanceof Error ? error.message : String(error)}`,
          },
        ],
      };
    }
  }

  private async handleAnalyzeTypes(args: { workspacePath: string; filePath: string }) {
    // Input validation
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const filePath = validateFilePath(args.filePath, workspacePath);

    try {
      this.ensureLanguageService(workspacePath);

      if (!this.languageService || !this.program) {
        throw new Error('Language service not initialized');
      }

      const sourceFile = this.program.getSourceFile(filePath);
      if (!sourceFile) {
        throw new Error(`File not found: ${filePath}`);
      }

      // Get semantic diagnostics
      const diagnostics = [
        ...this.program.getSemanticDiagnostics(sourceFile),
        ...this.program.getSyntacticDiagnostics(sourceFile),
        ...this.program.getDeclarationDiagnostics(sourceFile),
      ];

      if (diagnostics.length === 0) {
        return {
          content: [
            {
              type: 'text',
              text: `✅ No type errors found in ${filePath}`,
            },
          ],
        };
      }

      const errorMessages = diagnostics.map(diag => {
        const pos = sourceFile.getLineAndCharacterOfPosition(diag.start || 0);
        const category = ts.DiagnosticCategory[diag.category].toLowerCase();
        return `${category}: ${ts.flattenDiagnosticMessageText(diag.messageText, '\n')} at ${filePath}:${pos.line + 1}:${pos.character + 1}`;
      });

      const resultText = `Found ${diagnostics.length} issues in ${filePath}:\n\n${errorMessages.join('\n\n')}`;

      return {
        content: [
          {
            type: 'text',
            text: resultText,
          },
        ],
      };
    } catch (error) {
      return {
        content: [
          {
            type: 'text',
            text: `Error analyzing types: ${error instanceof Error ? error.message : String(error)}`,
          },
        ],
      };
    }
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('TypeScript MCP server running...');
  }
}

// Run the server
const server = new TypeScriptMCPServer();
server.run().catch(error => {
  console.error('Server error:', error);
  process.exit(1);
});
