const fs = require('fs');
const path = require('path');

function findHardcodedStrings(dir) {
  const results = [];
  const files = fs.readdirSync(dir, { recursive: true }).filter(f => f.endsWith('.vue'));

  for (const file of files) {
    const fullPath = path.join(dir, file);
    const content = fs.readFileSync(fullPath, 'utf8');
    const lines = content.split('\n');

    let inTemplate = false;

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];
      if (line.includes('<template>')) {
        inTemplate = true;
      } else if (line.includes('</template>')) {
        inTemplate = false;
      } else if (inTemplate) {
        // Check for user-facing attributes
        const attrRegex = /(placeholder|title|alt|aria-label|label)="([^"]*)"|title='([^']*)'|placeholder='([^']*)'|alt='([^']*)'|aria-label='([^']*)'|label='([^']*)'/g;
        let match;
        while ((match = attrRegex.exec(line)) !== null) {
          const text = match[2] || match[3] || match[4] || match[5] || match[6] || match[7];
          if (text && !line.includes('$t') && text.length > 2 && !text.includes('@') && !text.includes('http') && !text.match(/^\d/) && !text.includes('currentColor') && !text.includes('none')) {
            results.push({
              file: fullPath,
              line: i + 1,
              text: text,
              type: 'attribute',
              attr: match[1] || 'title'
            });
          }
        }

        // Check for text content (simple heuristic: lines with > text < or just text)
        if (!line.includes('=') && !line.includes('<') && !line.includes('>') && line.trim().match(/[A-Za-z]/)) {
          const textMatch = line.trim().match(/^([^<>\n]+)$/);
          if (textMatch && textMatch[1].trim() && !line.includes('$t') && textMatch[1].trim().length > 2) {
            results.push({
              file: fullPath,
              line: i + 1,
              text: textMatch[1].trim(),
              type: 'text',
              fullLine: line.trim()
            });
          }
        }

        // Also check for text in {{ }} but not $t, simple strings
        const interpolationRegex = /\{\{\s*['"]([^'"]+)['"]\s*\}\}/g;
        while ((match = interpolationRegex.exec(line)) !== null) {
          const expr = match[1];
          if (!expr.includes('$t') && expr.match(/[A-Za-z]/) && expr.length > 2) {
            results.push({
              file: fullPath,
              line: i + 1,
              text: expr,
              type: 'interpolation'
            });
          }
        }
      }
    }
  }

  return results;
}

function suggestKey(file, text) {
  const relative = file.replace('frontend/', '').replace('.vue', '').replace(/\//g, '.').toLowerCase();
  const cleanText = text.toLowerCase().replace(/[^a-z0-9]/g, '');
  return `${relative}.${cleanText}`;
}

function getPriority(text, type) {
  if (type === 'text' && text.length > 10) return 'high';
  if (type === 'attribute' && ['title', 'aria-label'].includes(type)) return 'medium';
  if (text.includes('Loading') || text.includes('Error') || text.includes('Success')) return 'high';
  return 'low';
}

const hardcoded = findHardcodedStrings('frontend');
const grouped = {};
hardcoded.forEach(h => {
  if (!grouped[h.file]) grouped[h.file] = [];
  grouped[h.file].push(h);
});

let total = 0;
const report = [];
for (const file in grouped) {
  const items = grouped[file];
  total += items.length;
  report.push(`## ${file} (${items.length} strings)`);
  items.forEach(h => {
    const key = suggestKey(file, h.text);
    const priority = getPriority(h.text, h.type);
    report.push(`- Line ${h.line}: "${h.text}" (${h.type}) - Key: ${key} - Priority: ${priority}`);
  });
}

fs.writeFileSync('audit-report.md', report.join('\n'));
console.log(`Total hardcoded strings: ${total}`);
console.log('Report saved to audit-report.md');