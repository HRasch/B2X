#!/usr/bin/env node

import { spawn } from 'child_process';

// Test XML fragment extraction
async function testXmlFragmentExtraction() {
  console.log('Testing XML Fragment Extraction...\n');

  const testArgs = {
    method: 'tools/call',
    params: {
      name: 'extract_xml_fragment',
      arguments: {
        filePath: 'test-data/web.config',
        workspacePath: process.cwd(),
        maxElements: 10,
        preserveStructure: true,
        includeComments: false,
        sampleArrays: true,
        includeSchemaInfo: false
      }
    }
  };

  const serverProcess = spawn('node', ['dist/index.js'], {
    stdio: ['pipe', 'pipe', 'pipe'],
    cwd: process.cwd()
  });

  let response = '';
  let errorOutput = '';

  serverProcess.stdout.on('data', (data) => {
    response += data.toString();
  });

  serverProcess.stderr.on('data', (data) => {
    errorOutput += data.toString();
  });

  // Send the test request
  setTimeout(() => {
    serverProcess.stdin.write(JSON.stringify(testArgs) + '\n');
  }, 1000);

  // Wait for response
  setTimeout(() => {
    serverProcess.kill();

    console.log('Response received:');
    console.log(response);

    if (errorOutput) {
      console.log('\nErrors:');
      console.log(errorOutput);
    }

    // Check if response contains expected content
    if (response.includes('XML Fragment Extraction Results') &&
        response.includes('configuration') &&
        response.includes('appSettings')) {
      console.log('\n✅ XML Fragment Extraction Test PASSED');
    } else {
      console.log('\n❌ XML Fragment Extraction Test FAILED');
    }
  }, 3000);
}

testXmlFragmentExtraction();