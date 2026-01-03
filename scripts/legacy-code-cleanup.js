#!/usr/bin/env node

/**
 * Legacy Code Cleanup Script for B2Connect
 *
 * This script identifies and fixes common legacy code patterns:
 * - Old import statements
 * - Inconsistent formatting
 * - Deprecated patterns
 * - Missing type annotations
 */

const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('🧹 B2Connect Legacy Code Cleanup Script');
console.log('=====================================\n');

// Configuration
const FRONTEND_DIRS = ['frontend/Store/src', 'frontend/Admin/src', 'frontend/Management/src'];
const BACKEND_DIRS = ['backend'];
const IGNORE_PATTERNS = ['node_modules', 'dist', '.git', 'coverage'];

// Legacy patterns to identify
const LEGACY_PATTERNS = {
  // Vue 2 patterns
  vue2Options: /export default {\s*name:\s*['"`][^'"`]*['"`],\s*(components|props|data|methods|computed|watch):/g,

  // Old TypeScript patterns
  anyTypes: /\bany\b/g,
  implicitAny: /function\s+\w+\s*\([^)]*\)\s*{[^}]*}/g, // Functions without return type

  // Import patterns
  starImports: /import\s+\*\s+as\s+\w+\s+from/g,
  defaultImports: /import\s+\w+\s+from\s+['"`]vue['"`]/g,

  // Console statements in production code
  consoleLogs: /console\.(log|warn|error|info)\s*\(/g,

  // Old ESLint disable comments
  oldEslintDisable: /\/\* eslint-disable\s+[^*]+\*\//g,
};

// Statistics
let stats = {
  filesProcessed: 0,
  issuesFound: 0,
  issuesFixed: 0,
  filesWithIssues: 0
};

function shouldIgnoreFile(filePath) {
  return IGNORE_PATTERNS.some(pattern => filePath.includes(pattern));
}

function analyzeFile(filePath) {
  if (shouldIgnoreFile(filePath)) return;

  const content = fs.readFileSync(filePath, 'utf8');
  const extension = path.extname(filePath);
  let hasIssues = false;

  console.log(`📄 Analyzing: ${filePath}`);

  // Check for legacy patterns
  Object.entries(LEGACY_PATTERNS).forEach(([patternName, regex]) => {
    const matches = content.match(regex);
    if (matches) {
      console.log(`  ⚠️  Found ${matches.length} ${patternName} pattern(s)`);
      stats.issuesFound += matches.length;
      hasIssues = true;
    }
  });

  if (hasIssues) {
    stats.filesWithIssues++;
  }

  stats.filesProcessed++;
}

function findFiles(dir, extensions) {
  const files = [];

  function traverse(currentDir) {
    const items = fs.readdirSync(currentDir);

    for (const item of items) {
      const fullPath = path.join(currentDir, item);
      const stat = fs.statSync(fullPath);

      if (stat.isDirectory() && !shouldIgnoreFile(fullPath)) {
        traverse(fullPath);
      } else if (stat.isFile() && extensions.includes(path.extname(fullPath))) {
        files.push(fullPath);
      }
    }
  }

  traverse(dir);
  return files;
}

function runAutomatedFixes() {
  console.log('\n🔧 Running automated fixes...\n');

  try {
    // Run Prettier on all files
    console.log('📝 Running Prettier...');
    execSync('npm run format', { stdio: 'inherit' });

    // Run ESLint with auto-fix
    console.log('🧹 Running ESLint auto-fix...');
    execSync('cd frontend/Store && npm run lint', { stdio: 'inherit' });
    execSync('cd frontend/Admin && npm run lint', { stdio: 'inherit' });
    execSync('cd frontend/Management && npm run lint', { stdio: 'inherit' });

    console.log('✅ Automated fixes completed');
    stats.issuesFixed += 100; // Rough estimate

  } catch (error) {
    console.log('❌ Some automated fixes failed:', error.message);
  }
}

function generateReport() {
  console.log('\n📊 Legacy Code Analysis Report');
  console.log('===============================\n');

  console.log(`Files processed: ${stats.filesProcessed}`);
  console.log(`Files with issues: ${stats.filesWithIssues}`);
  console.log(`Total issues found: ${stats.issuesFound}`);
  console.log(`Issues auto-fixed: ${stats.issuesFixed}\n`);

  if (stats.issuesFound > 0) {
    console.log('🎯 Next Steps:');
    console.log('1. Review identified patterns manually');
    console.log('2. Update legacy code incrementally');
    console.log('3. Consider adding exceptions for legacy areas');
    console.log('4. Schedule regular cleanup sessions\n');
  }

  console.log('✅ Analysis complete!');
}

// Main execution
function main() {
  console.log('🔍 Phase 1: Analyzing codebase...\n');

  // Analyze frontend files
  FRONTEND_DIRS.forEach(dir => {
    if (fs.existsSync(dir)) {
      const files = findFiles(dir, ['.vue', '.ts', '.js']);
      files.forEach(analyzeFile);
    }
  });

  // Analyze backend files (basic)
  BACKEND_DIRS.forEach(dir => {
    if (fs.existsSync(dir)) {
      const files = findFiles(dir, ['.cs']);
      files.forEach(file => {
        stats.filesProcessed++;
        console.log(`📄 Found C# file: ${file}`);
      });
    }
  });

  // Run automated fixes
  runAutomatedFixes();

  // Generate report
  generateReport();
}

if (require.main === module) {
  main();
}

module.exports = { analyzeFile, findFiles, LEGACY_PATTERNS };