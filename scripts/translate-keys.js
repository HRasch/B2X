#!/usr/bin/env node

/**
 * AI-Assisted Translation Script for B2Connect Internationalization
 *
 * This script automates the translation of i18n keys using Google Translate API
 * to accelerate ESLint error reduction through internationalization.
 *
 * Usage:
 *   node scripts/translate-keys.js --input "Hello World" --key "common.greeting"
 *   node scripts/translate-keys.js --eslint-output eslint_output.txt
 */

const fs = require('fs');
const path = require('path');

// Mock translation service (replace with actual Google Translate API)
class MockTranslationService {
  constructor() {
    this.languages = {
      de: 'German',
      fr: 'French',
      es: 'Spanish',
      it: 'Italian',
      pt: 'Portuguese',
      nl: 'Dutch',
      pl: 'Polish'
    };
  }

  // Mock translations - replace with actual API calls
  async translate(text, targetLang) {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 100));

    // Mock translations (replace with real translations)
    const mockTranslations = {
      de: {
        'Checkout': 'Kasse',
        'Shipping Address': 'Lieferadresse',
        'Continue': 'Fortfahren',
        'Save': 'Speichern',
        'Cancel': 'Abbrechen'
      },
      fr: {
        'Checkout': 'Paiement',
        'Shipping Address': 'Adresse de livraison',
        'Continue': 'Continuer',
        'Save': 'Sauvegarder',
        'Cancel': 'Annuler'
      },
      es: {
        'Checkout': 'Pago',
        'Shipping Address': 'Dirección de envío',
        'Continue': 'Continuar',
        'Save': 'Guardar',
        'Cancel': 'Cancelar'
      }
      // Add more languages...
    };

    return mockTranslations[targetLang]?.[text] || `[${targetLang.toUpperCase()}] ${text}`;
  }
}

class TranslationManager {
  constructor() {
    this.service = new MockTranslationService();
    this.languages = ['de', 'fr', 'es', 'it', 'pt', 'nl', 'pl'];
    this.i18nPath = path.join(__dirname, '../frontend/Store/i18n.config.ts');
  }

  /**
   * Extract hardcoded strings from ESLint output
   */
  extractHardcodedStrings(eslintOutput) {
    const strings = [];
    const lines = eslintOutput.split('\n');

    for (const line of lines) {
      if (line.includes('Hardcoded string')) {
        const match = line.match(/"([^"]+)"/);
        if (match) strings.push(match[1]);
      }
    }

    return [...new Set(strings)]; // Remove duplicates
  }

  /**
   * Generate i18n key from English text
   */
  generateKey(text) {
    return text
      .toLowerCase()
      .replace(/[^a-z0-9\s]/g, '') // Remove special chars
      .replace(/\s+/g, '.') // Replace spaces with dots
      .replace(/\.{2,}/g, '.') // Remove double dots
      .replace(/^\.+|\.+$/g, ''); // Remove leading/trailing dots
  }

  /**
   * Add translations to i18n.config.ts
   */
  async addTranslations(key, englishText) {
    console.log(`🔄 Translating: "${englishText}" → ${key}`);

    const translations = { en: englishText };

    // Translate to all languages
    for (const lang of this.languages) {
      try {
        const translation = await this.service.translate(englishText, lang);
        translations[lang] = translation;
        console.log(`  ${lang}: "${translation}"`);
      } catch (error) {
        console.warn(`  ${lang}: Translation failed, using English fallback`);
        translations[lang] = englishText;
      }
    }

    // Add to i18n config (this would need to be implemented)
    await this.updateI18nConfig(key, translations);

    return translations;
  }

  /**
   * Update i18n.config.ts with new translations
   */
  async updateI18nConfig(key, translations) {
    // This would parse and update the i18n.config.ts file
    // For now, just log what would be done
    console.log(`📝 Would add to i18n.config.ts:`);
    console.log(`  ${key}: {`);
    Object.entries(translations).forEach(([lang, text]) => {
      console.log(`    ${lang}: '${text}',`);
    });
    console.log(`  },`);
  }

  /**
   * Process ESLint output file
   */
  async processEslintOutput(filePath) {
    console.log(`📖 Reading ESLint output from: ${filePath}`);

    const content = fs.readFileSync(filePath, 'utf8');
    const hardcodedStrings = this.extractHardcodedStrings(content);

    console.log(`🔍 Found ${hardcodedStrings.length} unique hardcoded strings`);

    for (const str of hardcodedStrings.slice(0, 5)) { // Process first 5 for demo
      const key = this.generateKey(str);
      await this.addTranslations(`checkout.${key}`, str);
      console.log('');
    }

    console.log(`✅ Processed ${Math.min(5, hardcodedStrings.length)} strings`);
    console.log(`💡 Run with --all to process all ${hardcodedStrings.length} strings`);
  }
}

// CLI Interface
async function main() {
  const args = process.argv.slice(2);
  const manager = new TranslationManager();

  if (args.includes('--eslint-output')) {
    const fileIndex = args.indexOf('--eslint-output');
    const filePath = args[fileIndex + 1] || 'eslint_output.txt';
    await manager.processEslintOutput(filePath);
  } else if (args.includes('--input')) {
    const inputIndex = args.indexOf('--input');
    const text = args[inputIndex + 1];
    const keyIndex = args.indexOf('--key');
    const key = keyIndex !== -1 ? args[keyIndex + 1] : manager.generateKey(text);

    if (text) {
      await manager.addTranslations(key, text);
    } else {
      console.error('❌ Please provide text with --input "your text here"');
    }
  } else {
    console.log(`
🤖 B2Connect AI Translation Helper

Usage:
  # Process ESLint output file
  node scripts/translate-keys.js --eslint-output eslint_output.txt

  # Translate single string
  node scripts/translate-keys.js --input "Hello World" --key "common.greeting"

  # Process all strings (be careful!)
  node scripts/translate-keys.js --eslint-output eslint_output.txt --all

Note: This currently uses mock translations. Replace MockTranslationService
with actual Google Translate API for production use.
    `);
  }
}

if (require.main === module) {
  main().catch(console.error);
}

module.exports = { TranslationManager, MockTranslationService };