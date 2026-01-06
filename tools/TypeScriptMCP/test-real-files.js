#!/usr/bin/env node

import { spawn } from 'child_process';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const serverPath = path.join(__dirname, 'dist', 'index.js');

async function testTool(toolName, args) {
  return new Promise((resolve, reject) => {
    const server = spawn('node', [serverPath], {
      stdio: ['pipe', 'pipe', 'pipe'] // Don't inherit stderr to avoid noise
    });

    let responseBuffer = '';
    let errorBuffer = '';

    server.stdout.on('data', (data) => {
      responseBuffer += data.toString();
    });

    server.stderr.on('data', (data) => {
      errorBuffer += data.toString();
    });

    server.on('close', (code) => {
      if (code !== 0) {
        reject(new Error(`Server exited with code ${code}. Stderr: ${errorBuffer}`));
        return;
      }

      try {
        const response = JSON.parse(responseBuffer.trim());
        resolve(response);
      } catch (e) {
        reject(new Error(`Failed to parse response: ${responseBuffer}. Error: ${e.message}`));
      }
    });

    // Send the tool call request
    const request = JSON.stringify({
      jsonrpc: '2.0',
      id: 1,
      method: 'tools/call',
      params: {
        name: toolName,
        arguments: args
      }
    }) + '\n';

    server.stdin.write(request);

    // Close stdin after a short delay
    setTimeout(() => {
      server.stdin.end();
    }, 3000); // Increased timeout
  });
}

async function runTests() {
  const testFile = '/Users/holger/Documents/Projekte/B2Connect/frontend/Store/components/RegistrationCheck.spec.ts';

  console.log('🧪 Testing TypeScript MCP Server with real project files...\n');

  try {
    // Test 1: Search for symbols with pattern
    console.log('1️⃣ Testing search_symbols with pattern "describe"...');
    const searchResult = await testTool('search_symbols', {
      pattern: 'describe',
      filePath: testFile
    });
    console.log('✅ Result:', JSON.stringify(searchResult.result, null, 2));

    console.log('\n🎉 Test completed successfully!');

  } catch (error) {
    console.error('❌ Test failed:', error.message);
    process.exit(1);
  }
}

runTests();