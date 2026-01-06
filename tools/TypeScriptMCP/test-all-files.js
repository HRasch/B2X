import { spawn } from 'child_process';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// Start the MCP server
const server = spawn('npx', ['ts-node', './src/index.ts'], {
  cwd: __dirname,
  stdio: ['pipe', 'pipe', 'pipe']
});

let buffer = '';

server.stdout.on('data', (data) => {
  buffer += data.toString();
  console.log('SERVER OUTPUT:', data.toString());

  // Check if we have a complete JSON response
  try {
    const response = JSON.parse(buffer.trim());
    console.log('PARSED RESPONSE:', JSON.stringify(response, null, 2));
    if (response.id === 1) {
      console.log('Received response for our request!');
      server.kill();
      process.exit(0);
    }
  } catch (e) {
    // Not a complete JSON yet
  }
});

server.stderr.on('data', (data) => {
  console.error('SERVER ERROR:', data.toString());
});

// Wait a bit for server to start
setTimeout(() => {
  // Send a search_symbols request without filePath to see all files
  const request = {
    jsonrpc: '2.0',
    id: 1,
    method: 'tools/call',
    params: {
      name: 'search_symbols',
      arguments: {
        workspacePath: '/Users/holger/Documents/Projekte/B2Connect/frontend/Store',
        pattern: 'RegistrationCheckVM'
      }
    }
  };

  console.log('Sending request:', JSON.stringify(request, null, 2));
  server.stdin.write(JSON.stringify(request) + '\n');
}, 2000);