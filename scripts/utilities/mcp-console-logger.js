#!/usr/bin/env node

/**
 * MCP Console Logger
 * Intercepts MCP server calls and logs token usage and output to console
 * Integrated with caching to prevent unnecessary re-runs
 * Usage: node scripts/mcp-console-logger.js <mcp-server-command> <command> [args...]
 */

const { spawn } = require('child_process');
const path = require('path');
const cacheManager = require('./mcp-cache-manager');

// Parse command line arguments
const [,, serverName, command, ...serverArgs] = process.argv;

if (!serverName || !command) {
  console.error('Usage: node scripts/mcp-console-logger.js <server-name> <command> [args...]');
  process.exit(1);
}

// Track token usage (mock implementation - replace with actual API monitoring)
let tokenCount = 0;
const startTime = Date.now();

// Function to log MCP activity
function logMCPActivity(message, tokens = 0) {
  const timestamp = new Date().toISOString();
  console.log(`[${timestamp}] MCP-${serverName}: ${message}`);
  if (tokens > 0) {
    tokenCount += tokens;
    console.log(`[${timestamp}] MCP-${serverName}: Tokens used: ${tokens} (Total: ${tokenCount})`);
  }
}

// Check for cached result (if target file provided)
const targetFile = serverArgs[serverArgs.length - 1];
if (targetFile && targetFile.endsWith('.ts') || targetFile.endsWith('.js') || targetFile.endsWith('.vue') || targetFile.endsWith('.cs')) {
  const cached = cacheManager.getCachedResult(serverName, targetFile);
  if (cached) {
    logMCPActivity('Using cached result (tokens saved)');
    console.log(JSON.stringify(cached, null, 2));
    process.exit(0);
  }
}

// Spawn the MCP server process
const serverProcess = spawn(command, serverArgs, {
  stdio: ['pipe', 'pipe', 'pipe'],
  cwd: process.cwd()
});

logMCPActivity('Server started');

// Handle stdout
serverProcess.stdout.on('data', (data) => {
  const output = data.toString().trim();
  if (output) {
    // Estimate tokens (rough approximation: 1 token per 4 characters)
    const estimatedTokens = Math.ceil(output.length / 4);
    logMCPActivity(`Output: ${output}`, estimatedTokens);
  }
});

// Handle stderr
serverProcess.stderr.on('data', (data) => {
  const error = data.toString().trim();
  if (error) {
    logMCPActivity(`Error: ${error}`);
  }
});

// Handle process exit
serverProcess.on('exit', (code) => {
  const duration = Date.now() - startTime;
  logMCPActivity(`Server exited with code ${code} after ${duration}ms. Total tokens: ${tokenCount}`);
  
  // Cache result if successful
  if (code === 0 && targetFile) {
    const output = { timestamp: new Date().toISOString(), duration, tokenCount };
    cacheManager.cacheResult(serverName, targetFile, output, tokenCount);
  }
  
  process.exit(code);
});

// Handle process errors
serverProcess.on('error', (err) => {
  logMCPActivity(`Process error: ${err.message}`);
  process.exit(1);
});

// Graceful shutdown
process.on('SIGINT', () => {
  logMCPActivity('Received SIGINT, shutting down...');
  serverProcess.kill('SIGINT');
});

process.on('SIGTERM', () => {
  logMCPActivity('Received SIGTERM, shutting down...');
  serverProcess.kill('SIGTERM');
});