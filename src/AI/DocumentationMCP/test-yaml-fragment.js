#!/usr/bin/env node

import { spawn } from 'child_process';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// Start the MCP server
const server = spawn('node', [path.join(__dirname, 'dist/index.js')], {
  stdio: ['pipe', 'pipe', 'pipe'],
  cwd: path.join(__dirname, '..', '..')
});

let responseBuffer = '';

server.stdout.on('data', (data) => {
  responseBuffer += data.toString();
});

server.stderr.on('data', (data) => {
  console.error('Server stderr:', data.toString());
});

// Send initialize request
const initializeRequest = {
  jsonrpc: '2.0',
  id: 1,
  method: 'initialize',
  params: {
    protocolVersion: '2024-11-05',
    capabilities: {},
    clientInfo: {
      name: 'test-client',
      version: '1.0.0'
    }
  }
};

server.stdin.write(JSON.stringify(initializeRequest) + '\n');

// Wait for initialize response, then send tools/list
setTimeout(() => {
  const toolsRequest = {
    jsonrpc: '2.0',
    id: 2,
    method: 'tools/list',
    params: {}
  };

  server.stdin.write(JSON.stringify(toolsRequest) + '\n');
}, 1000);

// Wait for tools/list response, then test extract_yaml_fragment
setTimeout(() => {
  const testRequest = {
    jsonrpc: '2.0',
    id: 3,
    method: 'tools/call',
    params: {
      name: 'extract_yaml_fragment',
      arguments: {
        filePath: 'test-compose.yml',
        workspacePath: 'c:\\Users\\Holge\\repos\\B2Connect',
        maxKeys: 20
      }
    }
  };

  server.stdin.write(JSON.stringify(testRequest) + '\n');
}, 2000);

// Wait for response and exit
setTimeout(() => {
  console.log('Test completed. Server output:');
  console.log(responseBuffer);
  server.kill();
  process.exit(0);
}, 3000);