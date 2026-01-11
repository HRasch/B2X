#!/usr/bin/env node

// Simple script to run playwright without vitest interference
import { spawn } from 'child_process';

const args = ['./node_modules/.bin/playwright', 'test', '--reporter=list'];
const child = spawn('node', args, {
  cwd: process.cwd(),
  stdio: 'inherit',
  env: {
    ...process.env,
    NODE_OPTIONS: '',
  },
});

process.exit(child.exitCode);
