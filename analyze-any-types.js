const fs = require('fs');
const path = require('path');

// Find all TypeScript/Vue files
function findFiles(dir, ext) {
  const files = [];
  const items = fs.readdirSync(dir);
  for (const item of items) {
    const fullPath = path.join(dir, item);
    const stat = fs.statSync(fullPath);
    if (stat.isDirectory() && !item.startsWith('.') && item !== 'node_modules') {
      files.push(...findFiles(fullPath, ext));
    } else if (stat.isFile() && ext.includes(path.extname(fullPath))) {
      files.push(fullPath);
    }
  }
  return files;
}

const tsFiles = findFiles('.', ['.ts', '.vue', '.tsx', '.jsx']);
console.log('Found', tsFiles.length, 'TypeScript/Vue files');

// Count any types in each file
const results = [];
for (const file of tsFiles) {
  try {
    const content = fs.readFileSync(file, 'utf8');
    const anyCount = (content.match(/:\s*any\b/g) || []).length;
    if (anyCount > 0) {
      results.push({ file, count: anyCount });
    }
  } catch (e) {
    // Skip files that can't be read
  }
}

results.sort((a, b) => b.count - a.count);
console.log('\nTop 20 files with most any types:');
results.slice(0, 20).forEach((r, i) => {
  console.log(`${i+1}. ${r.file}: ${r.count} any types`);
});

const totalAny = results.reduce((sum, r) => sum + r.count, 0);
console.log(`\nTotal any types found: ${totalAny}`);