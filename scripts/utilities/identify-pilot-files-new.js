#!/usr/bin/env node

/**
 * Pilot Migration - Top Critical Files Identifier
 * Identifies the most critical files for initial cleanup based on ESLint results
 */

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

console.log('🔍 Identifying Top Critical Files for Pilot Migration...\n');

// Run ESLint and capture output
function runEslint(projectPath, projectName) {
  try {
    const output = execSync(`cd ${projectPath} && npm run lint:check 2>&1 || true`, {
      encoding: 'utf8',
      maxBuffer: 1024 * 1024 * 10 // 10MB buffer
    });

    return { output, projectName };
  } catch (error) {
    return { output: error.stdout || error.stderr || '', projectName };
  }
}

// Parse ESLint output to extract file issues
function parseEslintOutput(output, projectName) {
  const lines = output.split('\n');
  const fileIssues = {};
  let currentFile = null;

  console.log(`   Parsing ${lines.length} lines of ESLint output...`);

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i].trim();

    // Check if this line is a file path (contains .vue, .ts, .js and no colons)
    if (line && (line.includes('.vue') || line.includes('.ts') || line.includes('.js')) &&
        !line.includes(':') && !line.match(/^\d/)) {
      currentFile = line;
      if (!fileIssues[currentFile]) {
        fileIssues[currentFile] = { errors: 0, warnings: 0, issues: [] };
      }
      continue;
    }

    // If we have a current file and this line looks like an error/warning
    if (currentFile && (line.includes(' error ') || line.includes(' warning '))) {
      const isError = line.includes(' error ');
      const type = isError ? 'error' : 'warning';

      // Extract line number and message
      const lineMatch = line.match(/^(\d+):(\d+)\s+(error|warning)\s+(.+)$/);
      if (lineMatch) {
        const [, lineNum, col, , message] = lineMatch;

        if (isError) {
          fileIssues[currentFile].errors++;
        } else {
          fileIssues[currentFile].warnings++;
        }

        fileIssues[currentFile].issues.push({
          type,
          message,
          line: `${lineNum}:${col}`,
          fullLine: line
        });
      }
    }
  }

  const files = Object.entries(fileIssues).map(([file, data]) => ({
    file: path.relative(process.cwd(), file),
    project: projectName,
    errors: data.errors,
    warnings: data.warnings,
    totalIssues: data.errors + data.warnings,
    criticalScore: data.errors * 10 + data.warnings, // Errors are 10x more critical
    issues: data.issues
  }));

  console.log(`   Found ${files.length} files with issues`);
  return files;
}

// Analyze file importance
function analyzeFileImportance(fileData) {
  const file = fileData.file.toLowerCase();

  let priority = 1; // Base priority
  let reasons = [];

  // Auth-related files (highest priority)
  if (file.includes('auth') || file.includes('login') || file.includes('security')) {
    priority += 10;
    reasons.push('🔐 Authentication/Security');
  }

  // API-related files
  if (file.includes('api') || file.includes('service') || file.includes('client')) {
    priority += 8;
    reasons.push('🔗 API/Service Layer');
  }

  // Core components
  if (file.includes('component') && (file.includes('checkout') || file.includes('cart') || file.includes('payment'))) {
    priority += 7;
    reasons.push('🛒 Core Business Logic');
  }

  // Frequently modified files (based on patterns)
  if (file.includes('store') || file.includes('composable') || file.includes('hook')) {
    priority += 5;
    reasons.push('🔄 Frequently Modified');
  }

  // Test files (lower priority for initial cleanup)
  if (file.includes('test') || file.includes('spec')) {
    priority -= 3;
    reasons.push('🧪 Test File (Lower Priority)');
  }

  // Legacy patterns
  if (fileData.issues.some(issue => issue.message.includes('any') || issue.message.includes('ts-ignore'))) {
    priority += 3;
    reasons.push('📜 Legacy TypeScript Patterns');
  }

  return {
    ...fileData,
    priority,
    reasons,
    finalScore: fileData.criticalScore * priority
  };
}

// Main execution
async function main() {
  const projects = [
    { path: 'frontend/Store', name: 'Store Frontend' },
    { path: 'frontend/Admin', name: 'Admin Frontend' },
    { path: 'frontend/Management', name: 'Management Frontend' }
  ];

  console.log('📊 Analyzing ESLint results across all frontend projects...\n');

  const allFiles = [];

  for (const project of projects) {
    console.log(`🔍 Scanning ${project.name}...`);
    const result = runEslint(project.path, project.name);

    // ESLint always returns output, even with issues
    if (result.output && result.output.trim().length > 0) {
      const files = parseEslintOutput(result.output, project.name);
      console.log(`   Found ${files.length} files with issues`);

      const analyzedFiles = files.map(analyzeFileImportance);
      allFiles.push(...analyzedFiles);
    } else {
      console.log(`   No output from ESLint`);
    }
  }

  // Sort by final score (criticality * priority)
  const sortedFiles = allFiles
    .sort((a, b) => b.finalScore - a.finalScore)
    .slice(0, 10); // Top 10

  console.log('\n🎯 TOP 10 CRITICAL FILES FOR PILOT MIGRATION');
  console.log('=' .repeat(60));

  sortedFiles.forEach((file, index) => {
    console.log(`\n${index + 1}. ${file.file}`);
    console.log(`   📁 Project: ${file.project}`);
    console.log(`   🚨 Errors: ${file.errors}, Warnings: ${file.warnings}`);
    console.log(`   🎯 Critical Score: ${file.criticalScore}, Priority: ${file.priority}`);
    console.log(`   📊 Final Score: ${file.finalScore}`);
    console.log(`   💡 Reasons: ${file.reasons.join(', ')}`);

    // Show top 3 issues
    if (file.issues.length > 0) {
      console.log(`   🔧 Top Issues:`);
      file.issues.slice(0, 3).forEach((issue, i) => {
        console.log(`      ${i + 1}. ${issue.type}: ${issue.message.substring(0, 80)}${issue.message.length > 80 ? '...' : ''}`);
      });
    }
  });

  // Save detailed report
  const reportPath = 'PILOT_MIGRATION_CANDIDATES.md';
  const report = `# Pilot Migration Candidates

**Generated:** ${new Date().toISOString()}
**Total Files Analyzed:** ${allFiles.length}
**Top 10 Selected:** Based on criticality and business impact

## Top 10 Critical Files

${sortedFiles.map((file, index) => `
### ${index + 1}. ${file.file}

**Project:** ${file.project}
**Errors:** ${file.errors} | **Warnings:** ${file.warnings}
**Critical Score:** ${file.criticalScore} | **Priority:** ${file.priority}
**Final Score:** ${file.finalScore}

**Reasons:** ${file.reasons.join(', ')}

**Top Issues:**
${file.issues.slice(0, 5).map(issue => `- ${issue.type}: ${issue.message}`).join('\n')}

**Migration Priority:** ${file.priority >= 8 ? '🔴 HIGH' : file.priority >= 5 ? '🟡 MEDIUM' : '🟢 LOW'}
`).join('\n')}

## Migration Strategy

1. **Phase 1 (Week 1):** Files 1-3 (High Priority - Auth/Security)
2. **Phase 2 (Week 1):** Files 4-6 (Medium Priority - Core Business)
3. **Phase 3 (Week 2):** Files 7-10 (Remaining High Impact)

## Success Criteria
- ✅ Zero ESLint errors in migrated files
- ✅ TypeScript strict mode compliance
- ✅ No breaking changes to functionality
- ✅ Tests still pass
`;

  fs.writeFileSync(reportPath, report);
  console.log(`\n📄 Detailed report saved to: ${reportPath}`);

  console.log('\n✅ Pilot Migration Candidates Identified!');
  console.log('🚀 Ready to start manual cleanup of top critical files.');
}

main().catch(console.error);