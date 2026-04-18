#!/usr/bin/env node
const fs = require('fs');
const path = require('path');

const localesDir = path.resolve(__dirname, '../src/locales');
const languages = ['en.json', 'de.json'];

function loadJson(p) {
  if (!fs.existsSync(p)) return null;
  return JSON.parse(fs.readFileSync(p, 'utf8'));
}

const localeFiles = languages.map(f => path.join(localesDir, f));

const locales = localeFiles.map(p => ({ path: p, data: loadJson(p) }));

if (locales.some(l => !l.data)) {
  console.warn('Some locale files are missing under', localesDir);
  process.exit(0);
}

function flatten(obj, prefix = '') {
  return Object.keys(obj).reduce((acc, key) => {
    const val = obj[key];
    const newKey = prefix ? `${prefix}.${key}` : key;
    if (val && typeof val === 'object' && !Array.isArray(val)) {
      Object.assign(acc, flatten(val, newKey));
    } else {
      acc[newKey] = val;
    }
    return acc;
  }, {});
}

const flattened = locales.map(l => ({ path: l.path, keys: Object.keys(flatten(l.data)) }));

const base = flattened[0];

const missing = [];
for (let i = 1; i < flattened.length; i++) {
  const lang = flattened[i];
  base.keys.forEach(k => {
    if (!lang.keys.includes(k)) missing.push({ key: k, missingIn: path.basename(lang.path) });
  });
}

if (missing.length === 0) {
  console.log('i18n check: OK — all keys present in', languages.join(', '));
  process.exit(0);
}

console.log('Missing keys:');
missing.forEach(m => console.log(` - ${m.key} (missing in ${m.missingIn})`));
process.exit(2);
