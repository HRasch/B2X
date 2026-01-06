#!/usr/bin/env node

/**
 * Playwright MCP Server for B2Connect
 *
 * This server provides browser automation capabilities using the official
 * Microsoft Playwright MCP implementation.
 *
 * Features:
 * - Browser automation (Chromium, Firefox, WebKit/Safari)
 * - E2E testing support
 * - Screenshot and PDF generation
 * - Network monitoring
 * - Performance profiling
 */

import { spawn } from 'child_process';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

// Get the path to @playwright/mcp
const playwrightMcpPath = join(__dirname, '../node_modules/@playwright/mcp/dist/index.js');

console.error('Starting Playwright MCP Server...');
console.error(`Using: ${playwrightMcpPath}`);

// Spawn the @playwright/mcp server
const mcpServer = spawn('node', [playwrightMcpPath], {
  stdio: 'inherit',
  env: {
    ...process.env,
    NODE_ENV: process.env.NODE_ENV || 'production',
  },
});

mcpServer.on('error', error => {
  console.error('Failed to start Playwright MCP server:', error);
  process.exit(1);
});

mcpServer.on('exit', code => {
  console.error(`Playwright MCP server exited with code ${code}`);
  process.exit(code || 0);
});

// Handle shutdown gracefully
process.on('SIGINT', () => {
  console.error('Shutting down Playwright MCP server...');
  mcpServer.kill('SIGINT');
});

process.on('SIGTERM', () => {
  console.error('Shutting down Playwright MCP server...');
  mcpServer.kill('SIGTERM');
});
