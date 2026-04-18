#!/usr/bin/env node

/**
 * TypeScript MCP Integration Test
 * Verifies that MCP tools are working correctly in the B2X environment
 */

import { spawn } from 'child_process';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const projectRoot = path.resolve(__dirname, '../../');

console.log('🧪 TypeScript MCP Integration Test');
console.log('=====================================\n');

// Test 1: Build verification
console.log('1. Building TypeScript MCP server...');
const buildProcess = spawn('npm', ['run', 'build'], {
  cwd: __dirname,
  stdio: 'inherit'
});

await new Promise((resolve, reject) => {
  buildProcess.on('close', (code) => {
    if (code === 0) {
      console.log('✅ Build successful\n');
      resolve();
    } else {
      console.log('❌ Build failed\n');
      reject(new Error('Build failed'));
    }
  });
});

// Test 2: Server startup test
console.log('2. Testing MCP server startup...');
const serverProcess = spawn('node', ['dist/index.js'], {
  cwd: __dirname,
  stdio: ['pipe', 'pipe', 'pipe']
});

let serverReady = false;
let startupTimeout;

const startupPromise = new Promise((resolve, reject) => {
  startupTimeout = setTimeout(() => {
    reject(new Error('Server startup timeout'));
  }, 5000);

  serverProcess.stdout.on('data', (data) => {
    const output = data.toString();
    if (output.includes('TypeScript MCP server running') || output.includes('Server started')) {
      clearTimeout(startupTimeout);
      serverReady = true;
      console.log('✅ Server started successfully\n');
      resolve();
    }
  });

  serverProcess.stderr.on('data', (data) => {
    const output = data.toString();
    if (output.includes('TypeScript MCP server running')) {
      clearTimeout(startupTimeout);
      serverReady = true;
      console.log('✅ Server started successfully\n');
      resolve();
    }
  });

  serverProcess.on('close', (code) => {
    if (!serverReady && code !== null) {
      clearTimeout(startupTimeout);
      reject(new Error(`Server exited with code ${code}`));
    } else if (serverReady) {
      // Server started and then exited normally (expected for MCP servers)
      resolve();
    }
  });
});

// Wait a bit for server to initialize
setTimeout(() => {
  if (!serverReady) {
    serverProcess.kill();
  }
}, 3000);

try {
  await startupPromise;
} catch (error) {
  console.log('❌ Server startup failed:', error.message);
  process.exit(1);
} finally {
  if (serverProcess && !serverProcess.killed) {
    serverProcess.kill();
  }
}

// Test 3: Configuration verification
console.log('3. Verifying MCP configuration...');
const fs = await import('fs');

try {
  const mcpConfig = JSON.parse(fs.readFileSync(path.join(projectRoot, '.vscode/mcp.json'), 'utf8'));

  if (mcpConfig.mcpServers && mcpConfig.mcpServers['typescript-mcp']) {
    console.log('✅ MCP configuration found');
    const config = mcpConfig.mcpServers['typescript-mcp'];
    if (config.command === 'node' && config.args[0].includes('TypeScriptMCP/dist/index.js')) {
      console.log('✅ Configuration is correct\n');
    } else {
      console.log('⚠️ Configuration may need verification\n');
    }
  } else {
    console.log('❌ TypeScript MCP not found in configuration\n');
  }
} catch (error) {
  console.log('❌ Configuration file error:', error.message, '\n');
}

// Test 4: Agent configuration verification
console.log('4. Verifying agent configurations...');

const agentFiles = [
  'Frontend.agent.md',
  'TechLead.agent.md'
];

for (const agentFile of agentFiles) {
  try {
    const content = fs.readFileSync(path.join(projectRoot, '.github/agents', agentFile), 'utf8');
    if (content.includes('typescript-mcp/*')) {
      console.log(`✅ ${agentFile} includes MCP tools`);
    } else {
      console.log(`❌ ${agentFile} missing MCP tools`);
    }
  } catch (error) {
    console.log(`❌ Error reading ${agentFile}:`, error.message);
  }
}
console.log();

// Test 5: Documentation verification
console.log('5. Verifying documentation...');

const docFiles = [
  '.github/prompts/typescript-review.prompt.md',
  '.ai/knowledgebase/tools-and-tech/typescript-mcp-integration.md',
  'tools/TypeScriptMCP/INTEGRATION_DEMO.md'
];

for (const docFile of docFiles) {
  try {
    fs.accessSync(path.join(projectRoot, docFile));
    console.log(`✅ ${docFile} exists`);
  } catch (error) {
    console.log(`❌ ${docFile} missing`);
  }
}
console.log();

// Final summary
console.log('🎉 Integration Test Complete!');
console.log('===============================');
console.log('If all checks are ✅, TypeScript MCP is fully integrated.');
console.log('Run the demo: cat tools/TypeScriptMCP/INTEGRATION_DEMO.md');
console.log('Start using: @TechLead /typescript-review Component:frontend Scope:src/components/');