const fs = require('fs');
const path = require('path');

const localesDir = path.join(__dirname, 'locales');
const languages = ['en', 'de', 'fr', 'es', 'it', 'pt', 'nl', 'pl'];

function flattenObject(obj, prefix = '') {
  let flattened = {};
  for (let key in obj) {
    if (obj.hasOwnProperty(key)) {
      let newKey = prefix ? `${prefix}.${key}` : key;
      if (typeof obj[key] === 'object' && obj[key] !== null && !Array.isArray(obj[key])) {
        Object.assign(flattened, flattenObject(obj[key], newKey));
      } else {
        flattened[newKey] = obj[key];
      }
    }
  }
  return flattened;
}

function loadJson(filePath) {
  return JSON.parse(fs.readFileSync(filePath, 'utf8'));
}

const enFile = path.join(localesDir, 'en.json');
const enData = loadJson(enFile);
const enKeys = Object.keys(flattenObject(enData));

let allMissing = {};

languages.forEach(lang => {
  if (lang === 'en') return;
  const langFile = path.join(localesDir, `${lang}.json`);
  if (!fs.existsSync(langFile)) {
    console.error(`File not found: ${langFile}`);
    return;
  }
  const langData = loadJson(langFile);
  const langKeys = Object.keys(flattenObject(langData));
  const missing = enKeys.filter(key => !langKeys.includes(key));
  if (missing.length > 0) {
    allMissing[lang] = missing;
  }
});

if (Object.keys(allMissing).length === 0) {
  console.log('✅ All translation keys are present in all supported languages.');
} else {
  console.log('❌ Missing translation keys:');
  Object.entries(allMissing).forEach(([lang, keys]) => {
    console.log(`Language: ${lang}`);
    keys.forEach(key => console.log(`  - ${key}`));
  });
  process.exit(1);
}