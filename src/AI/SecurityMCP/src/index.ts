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
import glob from 'fast-glob';
import axios from 'axios';
import * as cheerio from 'cheerio';

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

// Main MCP Server class
class SecurityMCP {
  private server: Server;

  constructor() {
    this.server = new Server(
      {
        name: 'security-mcp-server',
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
            name: 'scan_vulnerabilities',
            description: 'Scan for known vulnerabilities in dependencies',
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
            name: 'check_sql_injection',
            description: 'Check for potential SQL injection vulnerabilities',
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
            name: 'validate_input_sanitization',
            description: 'Validate input sanitization and validation patterns',
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
            name: 'check_authentication',
            description: 'Check authentication and authorization implementations',
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
            name: 'scan_xss_vulnerabilities',
            description: 'Scan for potential XSS vulnerabilities in frontend code',
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
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'scan_vulnerabilities':
            return await this.handleScanVulnerabilities(args as { workspacePath: string });
          case 'check_sql_injection':
            return await this.handleCheckSqlInjection(args as { workspacePath: string });
          case 'validate_input_sanitization':
            return await this.handleValidateInputSanitization(args as { workspacePath: string });
          case 'check_authentication':
            return await this.handleCheckAuthentication(args as { workspacePath: string });
          case 'scan_xss_vulnerabilities':
            return await this.handleScanXssVulnerabilities(args as { workspacePath: string });
          default:
            throw new McpError(ErrorCode.MethodNotFound, `Unknown tool: ${name}`);
        }
      } catch (error) {
        if (error instanceof ValidationError) {
          throw new McpError(ErrorCode.InvalidParams, error.message);
        }
        if (error instanceof McpError) {
          throw error;
        }
        throw new McpError(ErrorCode.InternalError, `Internal error: ${error}`);
      }
    });
  }

  private async handleScanVulnerabilities(args: { workspacePath: string }) {
    const workspacePath = validateWorkspacePath(args.workspacePath);

    // TODO: Implement vulnerability scanning
    return {
      content: [
        {
          type: 'text',
          text: 'Vulnerability scanning not yet implemented',
        },
      ],
    };
  }

  private async handleCheckSqlInjection(args: { workspacePath: string }) {
    const workspacePath = validateWorkspacePath(args.workspacePath);

    // TODO: Implement SQL injection checking
    return {
      content: [
        {
          type: 'text',
          text: 'SQL injection checking not yet implemented',
        },
      ],
    };
  }

  private async handleValidateInputSanitization(args: { workspacePath: string }) {
    const workspacePath = validateWorkspacePath(args.workspacePath);

    // TODO: Implement input sanitization validation
    return {
      content: [
        {
          type: 'text',
          text: 'Input sanitization validation not yet implemented',
        },
      ],
    };
  }

  private async handleCheckAuthentication(args: { workspacePath: string }) {
    const workspacePath = validateWorkspacePath(args.workspacePath);

    // TODO: Implement authentication checking
    return {
      content: [
        {
          type: 'text',
          text: 'Authentication checking not yet implemented',
        },
      ],
    };
  }

  private async handleScanXssVulnerabilities(args: { workspacePath: string }) {
    const workspacePath = validateWorkspacePath(args.workspacePath);

    // TODO: Implement XSS scanning
    return {
      content: [
        {
          type: 'text',
          text: 'XSS vulnerability scanning not yet implemented',
        },
      ],
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Security MCP server running on stdio');
  }
}

async function main() {
  const server = new SecurityMCP();
  await server.run();
}

main().catch(console.error);
